/// <reference path="../angular.min.js" />
Aadhar.controller("AdminIndexCtrl", ['$scope', 'AadharFactory', function ($scope, AadharFactory) {
    var HistoryTable;
    $scope.isLoading = true;
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
                    $scope.LoadHistoryData("");
                } else {
                    displayMessage('danger', rawData.resDesc);
                    $scope.isLoading = false;
                }
            } else {
                $scope.isLoading = false;
            }
        });
    }

    $scope.onChangeDistricts = function (districtid) {
        if (districtid) {
            $scope.isLoading = true;
            AadharFactory.rawItems($('#hdnGetCenters').val(), { 'districtid': districtid, Securitykey: $('#Securitykey').val() })
            .then(function (res) {
                if (res) {
                    var rawData = res.rawData;
                    if (rawData.resCode == '000') {
                        if ($scope.optCenters != null && $scope.optCenters.length > 0) {
                            $scope.optCenters = [];
                            delete $scope.center;
                            setTimeout(function () {
                                $('.basic_center').select2();
                            }, 200);
                        }

                        $scope.optCenters = rawData.centers;
                        $scope.isLoading = false;
                    } else {
                        displayMessage('danger', rawData.resDesc);
                        $scope.isLoading = false;
                    }
                } else {
                    $scope.isLoading = false;
                }
            });
        } else {
            displayMessage('danger', 'Please select Districts');
        }
    }
    
    $scope.LoadHistoryData = function (slotDateRange) {
        AadharFactory.rawItems($('#hndGetTransData').val(), { 'centerid': $scope.center, 'districtname': $('#districtId option:selected').text(), 'status': $scope.status, 'slotdate': slotDateRange, Securitykey: $('#Securitykey').val() })
        .then(function (res) {
           
            if (res) {
                var rawData = res.rawData;
                if (rawData.resCode == '000') {                    
                    LoadHistory(rawData.transDet);
                } else {
                    HistoryTable = $('#TransHistory').DataTable({
                        "destroy": true,
                        "lengthChange": false,
                        "bFilter": false,
                        "bPaginate": false,
                        "bInfo": false,
                        "data":[],
                        "language": {
                            "emptyTable": 'No data found.'
                        }
                    });                     
                    $scope.isLoading = false;
                }
            } else {
                $scope.isLoading = false;
            }
        });
    }

    $scope.SearchData = function () {
        $scope.LoadHistoryData($('#SlotDateRange').val());
    }

    function LoadHistory(resData) {
        HistoryTable = $('#TransHistory').DataTable({
            "destroy": true,
            "lengthChange": false,
            dom: 'Bfrtip',
            buttons: [
                {
                    extend: 'excelHtml5',
                    text: '<i class="fa fa-file-excel-o"></i>',
                    title: 'AADHAR TRANSACTIONS REPORTS'
                }
            ],
            "data": resData,
            "columns": [
                { "data": "mgovid" },
                { "data": "slotdate" },
                { "data": "slottime" },
                { "data": "district" },
                { "data": "center" },
                { "data": "name" },
                { "data": "status" }
            ]
        });

        $scope.isLoading = false;
    }
    $scope.LoadDistricts();
}]);