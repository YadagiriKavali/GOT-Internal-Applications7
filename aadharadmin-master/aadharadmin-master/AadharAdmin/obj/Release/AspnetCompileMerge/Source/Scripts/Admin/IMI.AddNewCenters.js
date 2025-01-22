/// <reference path="../angular.min.js" />

Aadhar.controller("AddCenterCtrl", ['$scope', 'AadharFactory', function ($scope, AadharFactory) {
    $('.basic_select2').select2();
    $scope.LoadDistricts = function () {
        $scope.isLoading = true;
        AadharFactory.rawItems($('#hdnGetDistricts').val(), { Securitykey: $('#Securitykey').val() })
        .then(function (res) {
            if (res) {
                var rawData = res.rawData;
                if (rawData.resCode == '000') {
                    $scope.optDistrict = rawData.districts;
                    $scope.isLoading = false;                   
                } else {
                    displayMessage('danger', rawData.resDesc);
                    $scope.isLoading = false;
                }
            } else {
                $scope.isLoading = false;
            }
        });
    }
  
    $scope.LoadDistricts();     
    $scope.CheckFormValid = function ($event) {
        if ($scope.district == null || $scope.district == "") {
            $event.preventDefault();
            displayMessage('danger', 'Please select districts');
        } 
    }
}]);

