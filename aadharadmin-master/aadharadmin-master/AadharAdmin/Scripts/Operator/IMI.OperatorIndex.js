/// <reference path="../angular.min.js" />
Aadhar.controller("OperIndexCtrl", ['$scope', 'AadharFactory', function ($scope, AadharFactory) {
    var HistoryTable;

    var modal = $('.modal').modal();
    $scope.isLoading = true;
    $scope.LoadHistoryData = function (slotDateRange) {
        AadharFactory.rawItems($('#hndGetTransData').val(), { 'slotdate': slotDateRange, Securitykey: $('#Securitykey').val() })
        .then(function (res) {
            if (res) {
                var rawData = res.rawData;
                if (rawData.resCode == '000') {
                    LoadHistory(rawData.TransDet);
                } else {
                    $('#TransHistory').DataTable({
                        "destroy": true,
                        "lengthChange": false,
                        "bFilter": false,
                        "bPaginate": false,
                        "data": [],
                        "bInfo": false,
                        "language": {
                            "emptyTable": 'No data found.'
                        }
                    });
                    displayMessage('danger', rawData.resDesc);
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
                { "data": "status" },
                {
                    "data": "action",
                    render: function (data, type, row) {
                        if (row.statusid == '1' && row.ActionStatus == '1') {
                            return '<a class="waves-effect waves-light btn auth modal-trigger open-ModalDialog" href="#modal1" data-slotdate=' + row.slotdate + ' data-mgovid=' + row.mgovid + '>Authenticate</a>';
                        } else {
                            return '';
                        }
                    }
                }
            ]
        });

        $scope.isLoading = false;
    }

    $('#TransHistory').on('click', 'tbody td a.auth', function () {
        var mgovid = $(this).attr('data-mgovid');
        var SlotDate = $(this).attr('data-slotdate');
        $('#mgovid').html(mgovid); $('#hdnSlotDate').val(SlotDate);
    });

    $scope.AuthPassCode = function () {
        if ($scope.usrPassCode != null && $scope.usrPassCode != "") {
            $scope.isLoading = true;
            AadharFactory.rawItems($('#hndUserSlotAuth').val(), { 'UserPassCode': $scope.usrPassCode, 'mGovId': $('#mgovid').html(), 'slotdate': $('#hdnSlotDate').val(), Securitykey: $('#Securitykey').val() })
            .then(function (res) {
                if (res) {
                    var rawData = res.rawData;
                    if (rawData.resCode == '000') {
                        displayMessage('success', "User is Authenticate Successfully.");
                        //$scope.isLoading = false;
                        //$scope.LoadHistoryData($('#SlotDateRange').val());
                        //$scope.usrPassCode = "";
                        //$('#btnClose').trigger('click');
                        setTimeout(function () {
                            window.location.reload();
                        }, 350);
                    } else {
                        displayMessage('danger', rawData.resDesc);
                        $scope.isLoading = false;
                        if (rawData.resCode == '608') {                           
                           $scope.usrPassCode = '';
                            setTimeout(function () {
                                $scope.LoadHistoryData($('#SlotDateRange').val());
                            }, 350);
                            $('#btnClose').trigger('click');
                        }
                    }
                } else {
                    $scope.isLoading = false;
                }
            });
        } else {
            $('input[type="tel"]').focus();
        }
    }

    $scope.LoadHistoryData("");

    $scope.checkTogal = function () {
        $scope.usrPassCode = '';        
        $('#btnClose').trigger('click');        
    }
   
}]);