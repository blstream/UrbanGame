﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using Common;
using Caliburn.Micro;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Reflection;

namespace WebService
{
    public class SubmittedSolution
    {
        public int TaskId { get; set; }
    }

    public class GameChangesManager : IGameChangesManager
    {
        IGameWebService _gameWebService;
        IEventAggregator _gameEventAggregator;
        Func<IUnitOfWork> _unitOfWorkLocator;
        ILocalizationService _localizationService;
        IToastPromptService _toastPromptService;
        Timer _solutionUpdaterTimer;
        Timer _taskUpdaterTimer;
        Timer _gameUpdaterTimer;
        Timer _gameStateUpdaterTimer;
        IGameAuthorizationService _authorizationService;

        private const int _gameStateUpdaterPeriod = 20 * 1000;
        private const int _taskUpdaterPeriod = 50 * 1000;
        private const int _gameUpdaterPeriod = 60 * 1000;
        private const int _solutionUpdaterPeriod = 30 * 1000;

        public GameChangesManager(IGameWebService gameWebService, IEventAggregator gameEventAggregator,
                                  Func<IUnitOfWork> unitOfWorkLocator, ILocalizationService localizationService,
                                  IToastPromptService toastPromptService, IGameAuthorizationService authorizationService)
        {
            _gameWebService = gameWebService;
            _gameEventAggregator = gameEventAggregator;
            _unitOfWorkLocator = unitOfWorkLocator;
            _localizationService = localizationService;
            _toastPromptService = toastPromptService;
            _authorizationService = authorizationService;

            _solutionUpdaterTimer = new Timer(new TimerCallback(CheckSolutionStatusChanged), null, _solutionUpdaterPeriod, _solutionUpdaterPeriod);
            _taskUpdaterTimer = new Timer(new TimerCallback(CheckTaskChanges), null, _taskUpdaterPeriod, _taskUpdaterPeriod);
            _gameUpdaterTimer = new Timer(new TimerCallback(CheckGameChanges), null, _gameUpdaterPeriod, _gameUpdaterPeriod);
            _gameStateUpdaterTimer = new Timer(new TimerCallback(CheckGameStateChanges), null, _gameStateUpdaterPeriod, _gameStateUpdaterPeriod);
        }

        #region GetPublicProperties

        private PropertyInfo[] GetPublicProperties(Type type)
        {
            if (type.IsInterface)
            {
                var propertyInfos = new List<PropertyInfo>();

                var considered = new List<Type>();
                var queue = new Queue<Type>();
                considered.Add(type);
                queue.Enqueue(type);
                while (queue.Count > 0)
                {
                    var subType = queue.Dequeue();
                    foreach (var subInterface in subType.GetInterfaces())
                    {
                        if (considered.Contains(subInterface)) continue;

                        considered.Add(subInterface);
                        queue.Enqueue(subInterface);
                    }

                    var typeProperties = subType.GetProperties(
                        BindingFlags.FlattenHierarchy
                        | BindingFlags.Public
                        | BindingFlags.Instance);

                    var newPropertyInfos = typeProperties
                        .Where(x => !propertyInfos.Contains(x));

                    propertyInfos.InsertRange(0, newPropertyInfos);
                }

                return propertyInfos.ToArray();
            }

            return type.GetProperties(BindingFlags.FlattenHierarchy
                | BindingFlags.Public | BindingFlags.Instance);
        }

        #endregion

        #region UpdateObject

        protected IList<string> UpdateObject<T>(T oldObject, T newObject, List<string> skipFields = null)
        {
            List<string> differences = new List<string>();
            PropertyInfo[] newProperties = GetPublicProperties(newObject.GetType());
            PropertyInfo[] oldProperties = GetPublicProperties(oldObject.GetType());

            foreach (PropertyInfo oldProperty in oldProperties)
            {
                PropertyInfo newProperty = newProperties.FirstOrDefault(p => p.Name == oldProperty.Name);

                if (newProperty != null && oldProperty.CanWrite && 
                    (skipFields == null || !skipFields.Contains(oldProperty.Name)))
                {
                    //skip collections - only basic data
                    if (!oldProperty.PropertyType.IsGenericType ||
                        (oldProperty.PropertyType.GetGenericTypeDefinition() != typeof(IEntityEnumerable<>) &&
                         oldProperty.PropertyType.GetGenericTypeDefinition() != typeof(EntitySet<>)))
                    {
                        object oldValue = oldProperty.GetValue(oldObject, null);
                        object newValue = newProperty.GetValue(newObject, null);

                        if (((oldValue == null || newValue == null) && oldValue != newValue) ||
                              (oldValue != null && newValue != null && !oldValue.Equals(newValue)))
                        {
                            if (oldProperty.PropertyType == typeof(DateTime) &&
                                ((DateTime)oldValue - (DateTime)newValue).TotalSeconds <= 1)
                                continue;
                            if (oldValue != null && newValue != null && 
                                oldProperty.PropertyType == typeof(DateTime?) &&
                                ((DateTime?)oldValue - (DateTime?)newValue).Value.TotalSeconds <= 1)
                                continue;

                            oldProperty.SetValue(oldObject, newProperty.GetValue(newObject, null), null);
                            differences.Add(oldProperty.Name);
                        }
                    }
                }
            }
            return differences;
        }

        #endregion



        #region GameChanges

        protected void CheckGameChanges(object obj)
        {
            if (!_authorizationService.IsUserAuthenticated())
                return;

            _gameUpdaterTimer.Change(Timeout.Infinite, Timeout.Infinite);
            Task.Factory.StartNew(() =>
                {                    
                    try
                    {
                        using (var uow = _unitOfWorkLocator())
                        {
                            var activeGames = uow.GetRepository<IGame>().All().Where(g => g.GameState == GameState.Joined);

                            foreach (var oldGame in activeGames)
                            {
                                //there cannot be await, because it causes InvalidOperationExceptions by uow.Commit()
                                var task = _gameWebService.GetGameInfo(oldGame.Id);
                                task.Wait();
                                IGame newGame = task.Result;

                                if (newGame.Version != oldGame.Version)
                                {
                                    IList<string> diff = UpdateObject(oldGame, newGame, new List<string>() { "GameState", "ListOfChanges", "GameOverDisplayed" });

                                    if (diff.Count == 0)
                                        return;

                                    if (_toastPromptService != null)
                                        oldGame.ListOfChanges = _toastPromptService.GetDifferencesText(diff);

                                    uow.Commit();                                    
                                                                            
                                    if (_toastPromptService != null)
                                        _toastPromptService.ShowGameChanged(oldGame.Id, oldGame.Name, _localizationService.GetText("GameChangedToast"));

                                    _gameEventAggregator.Publish(new GameChangedEvent() { Id = oldGame.Id });
                                }
                            }
                        }
                    }
                    finally
                    {
                        _gameUpdaterTimer.Change(_gameUpdaterPeriod, _gameUpdaterPeriod);
                    }                    
                });
        }

        #endregion

        #region GameStateChanges

        protected void CheckGameStateChanges(object obj)
        {
            if (!_authorizationService.IsUserAuthenticated())
                return;

            _gameStateUpdaterTimer.Change(Timeout.Infinite, Timeout.Infinite);
            Task.Factory.StartNew(() =>
            {               
                try
                {
                    using (var uow = _unitOfWorkLocator())
                    {
                        var activeGames = uow.GetRepository<IGame>().All().Where(g => g.GameState == GameState.Joined);

                        foreach (var oldGame in activeGames)
                        {
                            Task<GameOverResponse> task = _gameWebService.CheckGameOver(oldGame.Id);
                            task.Wait();
                            GameOverResponse gameOverResp = task.Result; 

                            if (gameOverResp.IsGameOver)
                            {
                                oldGame.GameState = gameOverResp.State;
                                oldGame.Rank = gameOverResp.Rank;
                                uow.Commit();

                                if (_toastPromptService != null)
                                    _toastPromptService.ShowGameChanged(oldGame.Id, oldGame.Name, _localizationService.GetText("GameStateChangedToast"));

                                _gameEventAggregator.Publish(new GameStateChangedEvent() { Id = oldGame.Id, NewState = gameOverResp.State, Rank = gameOverResp.Rank });
                            }
                        }
                    }
                }
                finally
                {
                    _gameStateUpdaterTimer.Change(_gameStateUpdaterPeriod, _gameStateUpdaterPeriod);
                }
            });
        }

        #endregion

        #region TaskChanges

        protected void CheckTaskChanges(object obj)
        {
            if (!_authorizationService.IsUserAuthenticated())
                return;

            _taskUpdaterTimer.Change(Timeout.Infinite, Timeout.Infinite);
            Task.Factory.StartNew(() =>
            {               
                try
                {
                    using (var uow = _unitOfWorkLocator())
                    {
                        var activeGames = uow.GetRepository<IGame>().All().Where(g => g.GameState == GameState.Joined);

                        foreach (var game in activeGames)
                        {
                            //there cannot be await, because it causes InvalidOperationExceptions by uow.Commit()
                            var t = _gameWebService.GetTasks(game.Id);
                            t.Wait();
                            ITask[] listOfTasks = t.Result.Where(tt => tt.State == TaskState.Active).ToArray();

                            foreach (var newTask in listOfTasks)
                            {
                                var oldTask = uow.GetRepository<ITask>().All().First(gt => gt.Id == newTask.Id);

                                if (newTask.Version != oldTask.Version)
                                {
                                    IList<string> diff = UpdateObject(oldTask, newTask, new List<string>() { "Game", "ListOfChanges", "UserPoints" });
                                   
                                    if (diff.Count == 0)
                                        return;

                                    if (_toastPromptService != null)
                                        oldTask.ListOfChanges = _toastPromptService.GetDifferencesText(diff);

                                    uow.Commit();

                                    if (_toastPromptService != null)
                                        _toastPromptService.ShowTaskChanged(game.Id, oldTask.Id, oldTask.Name, _localizationService.GetText("TaskChangedToast"));

                                    _gameEventAggregator.Publish(new TaskChangedEvent() { Id = oldTask.Id, GameId = game.Id });
                                }
                                else if (newTask.State != oldTask.State)
                                {
                                    oldTask.State = newTask.State;
                                    uow.Commit();

                                    _gameEventAggregator.Publish(new TaskChangedEvent() { Id = oldTask.Id, GameId = game.Id });
                                }
                            }
                        }
                    }
                }
                finally
                {
                    _taskUpdaterTimer.Change(_taskUpdaterPeriod, _taskUpdaterPeriod);
                }
            });
        }

        #endregion

        #region SolutionChanges

        protected void CheckSolutionStatusChanged(object obj)
        {
            if (!_authorizationService.IsUserAuthenticated())
                return;

            _solutionUpdaterTimer.Change(Timeout.Infinite, Timeout.Infinite);
            Task.Factory.StartNew(() =>
            {               
                try
                {
                    using (var uow = _unitOfWorkLocator())
                    {
                        var pendingSolutions = uow.GetRepository<IBaseSolution>().All().Where(s => s.Task.SolutionStatus == SolutionStatus.Pending);

                        foreach (var solution in pendingSolutions)
                        {
                            var task = _gameWebService.GetSolutionStatus(solution.Task.Id);
                            task.Wait();
                            SolutionStatusResponse response = task.Result;

                            if (response.Status == SolutionStatus.Accepted || response.Status == SolutionStatus.Rejected)
                            {
                                if ((response.Status == SolutionStatus.Accepted && response.Points == solution.Task.MaxPoints) || !solution.Task.IsRepeatable)
                                    solution.Task.State = TaskState.Accomplished;

                                solution.Task.SolutionStatus = response.Status;
                                solution.Task.UserPoints = response.Points;                 
                                uow.Commit();

                                string message = _localizationService.GetText("SolutionStatusChanged") + " " +
                                                 (response.Status == SolutionStatus.Accepted ? _localizationService.GetText("Accepted") : _localizationService.GetText("Rejected"));
                                if (_toastPromptService != null)
                                    _toastPromptService.ShowSolutionStatusChanged(solution.Task.Id, solution.Task.Game.Id, solution.Task.Name, message);

                                _gameEventAggregator.Publish(new SolutionStatusChanged() { Status = response.Status, Points = response.Points, TaskId = solution.Task.Id, GameId = solution.Task.Game.Id });
                            }
                        }                        
                    }
                }
                finally
                {
                    _solutionUpdaterTimer.Change(_solutionUpdaterPeriod, _solutionUpdaterPeriod);
                }
            });
        }

        #endregion
    }
}
