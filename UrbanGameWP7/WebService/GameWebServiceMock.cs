﻿using Common;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebService.DTOs;

namespace WebService
{
    public class GameWebServiceMock : IGameWebService
    {
        #region GameWebServiceMock
        /// <summary>
        /// Simple constructor
        /// </summary>
        public GameWebServiceMock()
        {
            ListOfGames = new List<IGame>();
            ListOfTasks = new List<ITask>();
            InitializeData();
        }
        #endregion

        #region InitializeData
        private void InitializeData()
        {
            ListOfTasks.Clear();
            ListOfGames.Clear();

            string description = "Le 10 septembre 2008, quelques jours après avoir fêté son vingtième anniversaire, Lewandowski débute sa carrière internationale avec la Pologne face à Saint-Marin, lors des éliminatoires de la coupe du monde 2010.";

            ListOfGames.AddRange(new IGame[] {
                new Game(){Name = "Hydromystery", GameType = GameType.ScoreAttack, OperatorName = "Cafeteria", NumberOfPlayers = 23, NumberOfSlots = 48, Id = 1, GameLogo = "/Images/gameicon.png", GameStart = new DateTime(2013, 4, 8, 12, 12,0) ,GameEnd = DateTime.Now.AddDays(2).AddHours(10), Difficulty = GameDifficulty.Easy, Description = description, Localization = "Wroclaw"},
                new Game(){Name = "North & South", GameType = GameType.Race, OperatorName = "Infogrames", NumberOfPlayers = 23, Id = 2, GameLogo = "/Images/gameicon.png", GameStart = new DateTime(2013, 5, 8, 12, 12,0), GameEnd = DateTime.Now.AddDays(3).AddHours(12), Description = description},
                new Game(){Name = "Ultimate Quest", GameType = GameType.ScoreAttack, OperatorName = "JCVD", NumberOfPlayers = 23,Id = 3, GameLogo = "/Images/gameicon.png", GameStart = DateTime.Now.AddDays(1).AddHours(12), GameEnd = DateTime.Now.AddDays(10).AddHours(2), Description = description},
                new Game(){Name = "Galaxy Quest", GameType = GameType.Race, OperatorName = "NSEA", NumberOfPlayers = 23,Id = 4, GameLogo = "/Images/gameicon.png", GameStart = new DateTime(2013,4,10,8,12,0), GameEnd = DateTime.Now.AddDays(3).AddHours(7).AddMinutes(18).AddSeconds(43), Description = description},
                new Game(){Name = "The Quest for NEETs", GameType = GameType.Race, OperatorName = "Ron Jeremy", NumberOfPlayers = 23,Id = 5, GameLogo = "/Images/gameicon.png", GameStart = new DateTime(2013,5,9,21,5,8),GameEnd = DateTime.Now.AddDays(2).AddHours(10), Description = description}});

            string lorem = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam aliquam mauris vel elit tincidunt ac bibendum tortor scelerisque. Mauris nisi augue, malesuada ac lobortis sed, rhoncus et mauris. Vivamus dictum turpis congue arcu euismod in pulvinar mi volutpat. Aliquam euismod pharetra velit eu sagittis. Proin et nisi nibh, ut egestas enim.";
            string accident = "There might be a problem getting to center, bacause of bus crash. Furthermore the police imidiately came and now tries to figure out what realy happened.";

            int taskId = 1;
            int alertId = 1;
            int highScoreId = 1;

            foreach(var g in ListOfGames)
            {
                var task = new GameTask() { Id = taskId++, Name = "IQ Test!", AdditionalText = "What is red with dots?", Type = TaskType.ABCD, Description = lorem, Picture = "/ApplicationIcon.png", SolutionStatus = SolutionStatus.NotSend, IsRepeatable = false, UserPoints = null, MaxPoints = 20, EndDate = DateTime.Now.AddDays(1), Version = 1 };
                task.ABCDPossibleAnswers.Add(new ABCDPossibleAnswer() { CharId = 'a', Answer = "Zebra" });
                task.ABCDPossibleAnswers.Add(new ABCDPossibleAnswer() { CharId = 'b', Answer = "Dragon" });
                task.ABCDPossibleAnswers.Add(new ABCDPossibleAnswer() { CharId = 'c', Answer = "Leaf" });
                task.ABCDPossibleAnswers.Add(new ABCDPossibleAnswer() { CharId = 'd', Answer = "Ladybug" });
                g.Tasks.Add(task);
                ListOfTasks.Add(task);

                var task1 = new GameTask() { Id = taskId++, Name = "Find Wally", Type = TaskType.GPS, Description = lorem, Picture = "/ApplicationIcon.png", SolutionStatus = SolutionStatus.NotSend, IsRepeatable = true, UserPoints = null, MaxPoints = 20, EndDate = DateTime.Now.AddDays(1), Version = 1 };
                g.Tasks.Add(task1);
                ListOfTasks.Add(task1);

                var task2 = new GameTask() { Id = taskId++, Name = "Brain Storm!", AdditionalText = "What color is a red car?", Type = TaskType.ABCD, Description = lorem, Picture = "/ApplicationIcon.png", SolutionStatus = SolutionStatus.NotSend, IsRepeatable = true, UserPoints = null, MaxPoints = 30, EndDate = DateTime.Now.AddDays(1).AddHours(15), Version = 1 };
                task2.ABCDPossibleAnswers.Add(new ABCDPossibleAnswer() { CharId = 'a', Answer = "Green" });
                task2.ABCDPossibleAnswers.Add(new ABCDPossibleAnswer() { CharId = 'b', Answer = "Red" });
                task2.ABCDPossibleAnswers.Add(new ABCDPossibleAnswer() { CharId = 'c', Answer = "Blue" });
                task2.ABCDPossibleAnswers.Add(new ABCDPossibleAnswer() { CharId = 'd', Answer = "DarkBlack" });
                task2.ABCDPossibleAnswers.Add(new ABCDPossibleAnswer() { CharId = 'e', Answer = "Lime" });
                task2.ABCDPossibleAnswers.Add(new ABCDPossibleAnswer() { CharId = 'f', Answer = "Pink" });
                task2.ABCDPossibleAnswers.Add(new ABCDPossibleAnswer() { CharId = 'g', Answer = "Brown" });
                task2.ABCDPossibleAnswers.Add(new ABCDPossibleAnswer() { CharId = 'h', Answer = "Yellow" });
                g.Tasks.Add(task2);
                ListOfTasks.Add(task2);

                var task3 = new GameTask() { Id = taskId++, Name = "Find Wally 2", Type = TaskType.GPS, Description = lorem, Picture = "/ApplicationIcon.png", SolutionStatus = SolutionStatus.NotSend, IsRepeatable = false, UserPoints = null, MaxPoints = 20, EndDate = DateTime.Now.AddDays(1), Version = 1 };
                g.Tasks.Add(task3);
                ListOfTasks.Add(task3);

                g.HighScores.Add(new HighScore() { Id = highScoreId++, UserLogin = "XTerminator", Points = 329, AchievedAt = DateTime.Now.AddDays(1).AddHours(13).AddMinutes(37) });
                g.HighScores.Add(new HighScore() { Id = highScoreId++, UserLogin = "RunnungRabit", Points = 310, AchievedAt = DateTime.Now.AddDays(1).AddHours(3).AddMinutes(7) });
                g.HighScores.Add(new HighScore() { Id = highScoreId++, UserLogin = "$ebastian", Points = 150, AchievedAt = DateTime.Now.AddHours(9).AddMinutes(27) });
                g.HighScores.Add(new HighScore() { Id = highScoreId++, UserLogin = "xX_Warior_Xx", Points = 90, AchievedAt = DateTime.Now.AddHours(1).AddMinutes(1) });

                g.Alerts.Add(new Alert() { Id = alertId++, Topic = "Information", Description = accident, AlertAppear = new DateTime(2013, 6, 27, 12, 10, 0) });
                g.Alerts.Add(new Alert() { Id = alertId++, Topic = "Information", Description = accident, AlertAppear = new DateTime(2013, 6, 27, 11, 10, 0) });
                g.Alerts.Add(new Alert() { Id = alertId++, Topic = "Information", Description = accident, AlertAppear = new DateTime(2013, 7, 4, 12, 1, 0) });
            }

        }
        #endregion

        #region Containers
        #region ListOfGames
        /// <summary>
        /// Game's containter
        /// </summary>
        public List<IGame> ListOfGames
        {
            get;
            private set;
        }
        #endregion

        #region ListOfTasks
        /// <summary>
        /// Task's containter
        /// </summary>
        public List<ITask> ListOfTasks
        {
            get;
            private set;
        }
        #endregion
        #endregion

        #region JoinGame
        public async Task<bool> JoinGame(int gid)
        {
            return true;
        }
        #endregion

        #region LeaveGame
        public async Task<bool> LeaveGame(int gid)
        {
            return true;
        }
        #endregion

        #region GetGameInfo
        public async Task<IGame> GetGameInfo(int gid)
        {
            return (await UserNearbyGames(new GeoCoordinate(1,1))).FirstOrDefault(x => x.Id == gid);
        }
        #endregion

        #region CheckGameOver
        public async Task<GameOverResponse> CheckGameOver(int gid)
        {
            /*int r = new Random().Next(100);
            int rank = new Random().Next(1, 100);

            if (r < 20)
                return new GameOverResponse() { State = GameState.Won, Rank = rank, IsGameOver = true };
            if (r < 35)
                return new GameOverResponse() { State = GameState.Lost, Rank = rank, IsGameOver = true };
            else*/
                return new GameOverResponse() { State = GameState.Joined, IsGameOver = false };
        }
        #endregion

        #region GetTasks
        public async Task<ITask[]> GetTasks(int gid)
        {
            return ListOfTasks.Where(t => t.Game.Id == gid).ToArray();
        }
        #endregion

        #region GetTaskDetails
        public async Task<ITask> GetTaskDetails(int gid, int tid)
        {
            return ListOfTasks.FirstOrDefault(t => t.Id == tid);
        }
        #endregion

        #region GetTaskDetails generic
        public async Task<TTaskType> GetTaskDetails<TTaskType>(int gid, int tid) 
            where TTaskType : ITask
        {
            return (TTaskType)await GetTaskDetails(gid, tid);
        }
        #endregion

        #region SubmitTaskSolution
        public async Task<SolutionResultScore> SubmitTaskSolution(int gid, int tid, IBaseSolution solution)
        {
            int r = new Random().Next(100);
            var task = ListOfTasks.First(t => t.Id == tid);

            if (r < 20)
            {
                if (!task.IsRepeatable)
                    task.State = TaskState.Accomplished;
                task.UserPoints = r;
                return new SolutionResultScore() { SubmitResult = SubmitResult.AnswerIncorrect, ScoredPoints = 0 };
            }
            else if (r < 60)
            {
                if (!task.IsRepeatable || task.MaxPoints == r)
                    task.State = TaskState.Accomplished;
                task.UserPoints = r;
                return new SolutionResultScore() { SubmitResult = SubmitResult.AnswerCorrect, ScoredPoints = r };
            }
            else
            {
                return new SolutionResultScore() { SubmitResult = SubmitResult.ScoreDelayed };
            }
        }
        #endregion

        #region GetSolutionStatus

        public async Task<SolutionStatusResponse> GetSolutionStatus(int taskId)
        {
            SolutionStatusResponse result = new SolutionStatusResponse();
            result.Status = new Random().Next(10) >= 5 ? SolutionStatus.Accepted : SolutionStatus.Rejected;
            result.Points = result.Status == SolutionStatus.Rejected ? 0 : new Random().Next(30);

            var task = ListOfTasks.First(t => t.Id == taskId);
            if ((result.Status == SolutionStatus.Accepted && result.Points == task.MaxPoints) || !task.IsRepeatable)
                task.State = TaskState.Accomplished;
            task.UserPoints = result.Points;

            return result;
        }

        #endregion

        #region Authorize
        public async Task<AuthorizeState> Authorize(string username, string password)
        {
            return AuthorizeState.Success;
        }
        #endregion

        #region CreateAccount

        public async Task<CreateAccountResponse> CreateAccount(string username, string email, string password)
        {
            return CreateAccountResponse.Success;
        }

        #endregion

        #region UserNearbyGames
        public async Task<IGame[]> UserNearbyGames(GeoCoordinate coordinate)
        {        
            return ListOfGames.ToArray();
        }
        #endregion

        #region Alerts
        public IAlert[] Alerts()
        {
            return new IAlert[] {
                new Alert(){Id = 1, Topic = "Unreal alert title", Description = "Sth happened at route 27"},
                new Alert(){Id = 2, Topic = "Unreal alert title", Description = "Sth happened at route 27"}};
        }
        #endregion

        #region HighScores
        public IHighScore[] HighScores()
        {
            return new IHighScore[]{
                new HighScore(){Id = 1, UserLogin = "Korona", Points =199},
                new HighScore(){Id = 1, UserLogin = "Amanda99", Points =99},
                new HighScore(){Id = 2, UserLogin = "LoganXxX", Points =299}};
        }
        #endregion
    }
}
