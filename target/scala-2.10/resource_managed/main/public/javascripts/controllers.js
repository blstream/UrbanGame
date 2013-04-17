/**Copyright 2013 BLStream, BLStream's Patronage Program Contributors
 *       http://blstream.github.com/UrbanGame/
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *       http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/
var MenuCtrl, NewGameCtrl, app;
app = angular.module('web', ['ui']);
NewGameCtrl = function($scope, $location) {
  $scope.steps = ['Start', '1', '2', '3', 'Finish'];
  $scope.selection = $scope.steps[0];
  $scope.getCurrentStepIndex = function() {
    return _.indexOf($scope.steps, $scope.selection);
  };
  $scope.goToStep = function(index) {
    if (!_.isUndefined($scope.steps[index])) {
      return $scope.selection = $scope.steps[index];
    }
  };
  $scope.hasNextStep = function() {
    var nextStep, stepIndex;
    stepIndex = $scope.getCurrentStepIndex();
    nextStep = stepIndex + 1;
    return !_.isUndefined($scope.steps[nextStep]);
  };
  $scope.hasPreviousStep = function() {
    var previousStep, stepIndex;
    stepIndex = $scope.getCurrentStepIndex();
    previousStep = stepIndex - 1;
    return !_.isUndefined($scope.steps[previousStep]);
  };
  $scope.incrementStep = function() {
    var nextStep, stepIndex;
    if ($scope.hasNextStep()) {
      stepIndex = $scope.getCurrentStepIndex();
      nextStep = stepIndex + 1;
      return $scope.selection = $scope.steps[nextStep];
    }
  };
  return $scope.decrementStep = function() {
    var previousStep, stepIndex;
    if ($scope.hasPreviousStep()) {
      stepIndex = $scope.getCurrentStepIndex();
      previousStep = stepIndex - 1;
      return $scope.selection = $scope.steps[previousStep];
    }
  };
};
MenuCtrl = function($scope) {
  return $scope.menuitems = [
    {
      name: "Page1",
      href: "/"
    }, {
      name: "Page2",
      href: "/"
    }, {
      name: "Page3",
      href: "/"
    }
  ];
};