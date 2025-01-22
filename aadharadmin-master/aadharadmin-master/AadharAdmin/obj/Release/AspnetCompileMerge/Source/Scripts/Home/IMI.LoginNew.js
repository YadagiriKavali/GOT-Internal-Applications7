/// <reference path="../angular.min.js" />

Aadhar.controller("LoginCtrl", ['$scope', 'AadharFactory', function ($scope, AadharFactory) {

    $scope.CheckLoginFlow = function ($event) {
        if ($('#logintype').val() != "" && $('#mobileno').val() != "") {
            $event.preventDefault();
            $scope.CheckFlow();
        }
    }

    $scope.CheckFlow = function () {
        $scope.isLoading = true;
        AadharFactory.rawItems($('#hdnVerify').val(), { logintype: $('#logintype').val(), mobileno: $('#mobileno').val(), otpVal: '123456', Securitykey: $('#Securitykey').val() })
        .then(function (res) {
            if (res) {                
                var rawData = res.rawData;
                if (rawData.resCode == '000') {
                    displayMessage('success', 'OTP is sent to your mobile number successfully.');
                    $scope.dv_ValidateOtp = true;
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

    if ($('#hdnIsInValidOtp').val() != "") {         
        if ($('#hdnIsInValidOtp').val().toLowerCase() == "true") {
            $scope.dv_ValidateOtp = true;
        }
    }
}]);