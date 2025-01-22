/// <reference path="../angular.min.js" />
Aadhar.controller("ManageCtrl", ['$scope', 'AadharFactory', function ($scope, AadharFactory) {
    $('.basic_select2').select2(); $('.basic_center').select2(); $('.hour_select2').select2();
    $('.timeSlot_select2').select2();
    $scope.changeType = function (type) {
        if (type != null && type != "") {
            if (type == 'DISABLE') {
                //disable
                $scope.dv_Enable = false;
            } else {
                //Enable
                $scope.dv_Enable = true;
            }
            $scope.dv_CatVal = true;
            $scope.LoadDistricts();
        } else {
            $scope.dv_CatVal = false;
        }
    }

    $scope.onChangeTypeVal = function (typeVal) {
        if (typeVal) {

            if (typeVal == 'DISTRICT') {
                $scope.dv_district = true;

                $scope.dv_center = false; $scope.dv_hour_slot = false; $scope.dv_slot_date = false; $scope.dv_time_slot = false;
                $scope.center = ""; $scope.slotdate = ""; $scope.timeslot = "";                
            } else if (typeVal == 'CENTER') {
                $scope.dv_district = true; $scope.dv_center = true;

                $scope.dv_hour_slot = false; $scope.dv_slot_date = false; $scope.dv_time_slot = false;
                $scope.slotdate = ""; $scope.timeslot = "";
            } else if (typeVal == 'DATE') {
                $scope.dv_district = true; $scope.dv_center = true;

                $scope.dv_slot_date = true;

                $scope.dv_hour_slot = false; $scope.dv_time_slot = false;
                $scope.timeslot = "";
                $scope.LoadDistricts();
            } else if (typeVal == 'HOUR') {
                $scope.dv_district = true; $scope.dv_center = true; $scope.dv_hour_slot = true; $scope.dv_slot_date = true;

                $scope.dv_time_slot = false;
            }
            else if (typeVal == 'SLOT') {
                $scope.LoadSlotTiems();
                $scope.dv_district = true; $scope.dv_center = true; $scope.dv_hour_slot = true; $scope.dv_slot_date = true; $scope.dv_time_slot = true;
            } else {
                displayMessage('info', 'No Value Type is defined');
            }
        }
    }

    $scope.LoadDistricts = function () {
        $scope.isLoading = true;
        AadharFactory.rawItems($('#hdnGetDistricts').val(), { Securitykey: $('#Securitykey').val() })
        .then(function (res) {
            if (res) {
                var rawData = res.rawData;
                if (rawData.resCode == '000') {                     
                    $scope.optDistrict = rawData.districts;
                    $scope.isLoading = false;
                    $scope.LoadSlotDates(); setFormation();
                } else {
                    displayMessage('danger', rawData.resDesc);
                    $scope.isLoading = false;
                }
            } else {
                $scope.isLoading = false;
            }
        });
    }

    function formatState(state) {
        if (!state.element) return;
        var os = $(state.element).attr('ng-class');                  
        return $('<span ng-class="' + os + '">' + state.text + '</span>');
    }

    function setFormation() {
        setTimeout(function () {             
            $('.basic_select2').select2({
                templateResult: formatState
            });
        }, 1000);
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
                        format_center();                       

                    } else {
                        displayMessage('danger', rawData.resDesc);
                        $scope.isLoading = false;
                    }
                } else {
                    $scope.isLoading = false;
                }
            });
        }
    }

    function format_center() {
        setTimeout(function () {             
            $('.basic_center').select2({
                templateResult: formatState
            });
        }, 300);
    }

    var selectAllClicked = false;
    $scope.onChangeCenter = function (center) {
        if (center) {
            if (center[0] == 'all') {
                $('.basic_center > option').prop("selected", true);
                $('.basic_center').select2().select2();
                selectAllClicked = true;
            } else if (center[0] != 'all' && center.length > 1 && selectAllClicked) {
                $(".basic_center > option").prop("selected", false);
                $('.basic_center').select2().select2();
                selectAllClicked = false;
            }


            //don't delete
            if ($scope.slotdate != null && $scope.slotdate != "") {
                $scope.LoadSlotTiems();
            } else {
                if ($scope.optType == 'DISABLE') {
                    $scope.GetBookedSlotsCount();
                } else {
                    $scope.dv_NoOfSlots = false;
                }
            }
        }
    }

    $scope.LoadSlotDates = function () {
        if ($scope.optType) {
            $scope.isLoading = true;
            AadharFactory.rawItems($('#hdnGetSlotDates').val(), { 'typevalue': $scope.optType, Securitykey: $('#Securitykey').val() })
            .then(function (res) {
                if (res) {
                    var rawData = res.rawData;
                    if (rawData.resCode == '000') {
                        LoadSlotDates(rawData.slotdates, $scope.optType);
                        $scope.SlotDates = rawData.slotdates;
                        $scope.isLoading = false; 
                    } else {
                        //displayMessage('danger', "slot dates not avaliable.");
                        LoadSlotDates('');

                        $scope.isLoading = false;
                    }
                } else {
                    $scope.isLoading = false;
                }
            });
        } else {
            displayMessage('info', 'Please select Category Type.');
        }
    }

    function LoadSlotDates(arrayDates, typeVal) {
        var disabelDates = [];
        for (var i = 0; i < arrayDates.length; i++) {
            disabelDates.push(arrayDates[i].date);
        }
        var isDisable = false;
        if (typeVal == 'DISABLE') {
            isDisable = true;
        }

        var d = new Date();
        var year = d.getFullYear();
        var month = d.getMonth();
        var day = d.getDate();
        var endDt = new Date(year, month + 2, day);

        if (disabelDates.length > 0) {
            $('#slotDatePicker').datepicker({
                startDate: new Date(),
                endDate: endDt,
                daysOfWeekDisabled: [0],
                format: "dd-mm-yyyy",
                autoclose: true
                //beforeShowDay: function (date) {
                //    //var dt_ddmmyyyy = date.getDate() + '-' + (date.getMonth() + 1) + '-' + date.getFullYear();
                //    var dd = date.getDate();
                //    if (date.getDate() <= 9) { dd = '0' + date.getDate(); }

                //    var mm = (date.getMonth() + 1);
                //    if ((date.getMonth() + 1) <= 9) { mm = '0' + (date.getMonth() + 1); }

                //    var yyyy = date.getFullYear();
                //    if (date.getFullYear() <= 9) { mm = '0' + date.getFullYear(); }
                //    var dt_ddmmyyyy = dd + '-' + mm + '-' + yyyy;
                //    //return (disabelDates.indexOf(dt_ddmmyyyy) != -1);

                //    var itemVal = disabelDates.indexOf(dt_ddmmyyyy);
                //    //if (isDisable) {
                //    //    if ((disabelDates.indexOf(dt_ddmmyyyy) != -1)) {
                //    //        return false;
                //    //    }
                //    //    else {
                //    //        return true;
                //    //    }
                //    //    //return true;
                //    //} else 

                //    {
                //        if ((disabelDates.indexOf(dt_ddmmyyyy) != -1)) {
                //            return false;
                //        }
                //        else {
                //            return true;
                //        }
                //    }

                //}
            });
        } else {
            $('#slotDatePicker').datepicker({
                startDate: new Date(),
                endDate: endDt,
                daysOfWeekDisabled: [0],
                format: "dd-mm-yyyy",
                autoclose: true
            });
        }

    }

    $scope.onSlotDateChange = function (slotdt) {
        if (slotdt && $scope.center != null && $scope.center != "") {
            $scope.LoadSlotTiems();

            if ($scope.optHours != null && $scope.optHours.length > 0) {
                $scope.optHours = [];
                $scope.hourslot = "";
                setTimeout(function () {                     
                    $('.hour_select2').select2();
                }, 150);
            }
           

            if ($scope.optSlots != null && $scope.optSlots.length > 0) {
                $scope.optSlots = [];
                $scope.timeslot = "";
                setTimeout(function () {
                    $('.timeSlot_select2').select2();
                }, 150);
            }

        }
    }

    $scope.LoadSlotTiems = function (type) {
        if ($scope.center != null && $scope.center != "" && $scope.slotdate != null && $scope.slotdate != "") {
            $scope.isLoading = true;
            var centerData;
            if ($scope.center == 'all') {
                centerData = "1";
            } else {
                centerData = $scope.center;
            }             
            AadharFactory.rawItems($('#hdnGetSlotTimes').val(), { 'centerid': centerData.toString(), 'slotdate': $scope.slotdate, Securitykey: $('#Securitykey').val() })
            .then(function (res) {
                if (res) {
                    var rawData = res.rawData;
                    var Hourid = []; var HourSlots = [];
                    if (rawData.resCode == '000') {

                        $scope.optHours = rawData.time_slots_hour;
                       
                        $scope.isLoading = false;
                        if ($scope.optType == 'DISABLE') {
                            $scope.GetBookedSlotsCount();
                        } else {
                            $scope.dv_NoOfSlots = false;
                        }
                        format_hours();
                    } else {
                        //if ($scope.typeVal == 'SLOT' || $scope.typeVal == 'HOUR') {
                        //    if ($scope.typeVal == 'HOUR') {
                        //        displayMessage('danger', "Hours doesn't avaliable for this selected date.");
                        //    }
                        //    else {
                        //        displayMessage('danger', "Time slots doesn't avaliable for this selected date.");
                        //    }
                        //}
                        var TempHours = JSON.parse('{"code":"0","Securitykey":"b2380c590d9b4b0ab69f437a23630cf9","desc":"success","rawData":{"payable_amount":"10","time_slots_hour":[{"hour_id":1,"hour":"10:00 - 11:00","status":1,"time_slots_minutes":[{"id":"1","slot":"10:00 - 10:10","status":"1"},{"id":"2","slot":"10:10 - 10:20","status":"1"},{"id":"3","slot":"10:20 - 10:30","status":"1"},{"id":"4","slot":"10:30 - 10:40","status":"1"},{"id":"5","slot":"10:40 - 10:50","status":"1"},{"id":"6","slot":"10:50 - 11:00","status":"1"}]},{"hour_id":2,"hour":"11:00 - 12:00","status":1,"time_slots_minutes":[{"id":"1","slot":"11:00 - 11:10","status":"1"},{"id":"2","slot":"11:10 - 11:20","status":"1"},{"id":"3","slot":"11:20 - 11:30","status":"1"},{"id":"4","slot":"11:30 - 11:40","status":"1"},{"id":"5","slot":"11:40 - 11:50","status":"1"},{"id":"6","slot":"11:50 - 12:00","status":"1"}]},{"hour_id":3,"hour":"12:00 - 01:00","status":1,"time_slots_minutes":[{"id":"1","slot":"12:00 - 12:10","status":"1"},{"id":"2","slot":"12:10 - 12:20","status":"1"},{"id":"3","slot":"12:20 - 12:30","status":"1"},{"id":"4","slot":"12:30 - 12:40","status":"1"},{"id":"5","slot":"12:40 - 12:50","status":"0"},{"id":"6","slot":"12:50 - 01:00","status":"0"}]},{"hour_id":4,"hour":"01:00 - 02:00","status":0,"time_slots_minutes":[{"id":"1","slot":"01:00 - 01:10","status":"0"},{"id":"2","slot":"01:10 - 01:20","status":"0"},{"id":"3","slot":"01:20 - 01:30","status":"0"},{"id":"4","slot":"01:30 - 01:40","status":"0"},{"id":"5","slot":"01:40 - 01:50","status":"0"},{"id":"6","slot":"01:50 - 02:00","status":"0"}]},{"hour_id":5,"hour":"02:00 - 03:00","status":1,"time_slots_minutes":[{"id":"1","slot":"02:00 - 02:10","status":"0"},{"id":"2","slot":"02:10 - 02:20","status":"0"},{"id":"3","slot":"02:20 - 02:30","status":"0"},{"id":"4","slot":"02:30 - 02:40","status":"0"},{"id":"5","slot":"02:40 - 02:50","status":"0"},{"id":"6","slot":"02:50 - 03:00","status":"1"}]},{"hour_id":6,"hour":"03:00 - 04:00","status":1,"time_slots_minutes":[{"id":"1","slot":"03:00 - 03:10","status":"1"},{"id":"2","slot":"03:10 - 03:20","status":"1"},{"id":"3","slot":"03:20 - 03:30","status":"1"},{"id":"4","slot":"03:30 - 03:40","status":"1"},{"id":"5","slot":"03:40 - 03:50","status":"1"},{"id":"6","slot":"03:50 - 04:00","status":"1"}]},{"hour_id":7,"hour":"04:00 - 05:00","status":1,"time_slots_minutes":[{"id":"1","slot":"04:00 - 04:10","status":"1"},{"id":"2","slot":"04:10 - 04:20","status":"1"},{"id":"3","slot":"04:20 - 04:30","status":"1"},{"id":"4","slot":"04:30 - 04:40","status":"1"},{"id":"5","slot":"04:40 - 04:50","status":"1"},{"id":"6","slot":"04:50 - 05:00","status":"1"}]},{"hour_id":8,"hour":"05:00 - 06:00","status":1,"time_slots_minutes":[{"id":"1","slot":"05:00 - 05:10","status":"1"},{"id":"2","slot":"05:10 - 05:20","status":"1"},{"id":"3","slot":"05:20 - 05:30","status":"1"},{"id":"4","slot":"05:30 - 05:40","status":"1"},{"id":"5","slot":"05:40 - 05:50","status":"1"},{"id":"6","slot":"05:50 - 06:00","status":"1"}]}],"resCode":"000","resDesc":"success"}}');
                        $scope.optHours = TempHours.rawData.time_slots_hour;
                        
                        //$scope.dv_hour_slot = false; $scope.dv_time_slot = false;
                        $scope.isLoading = false;
                        format_hours();
                    }
                } else {
                    $scope.isLoading = false;
                }
            });
        } 
    }

    function format_hours() {
        setTimeout(function () {             
            $('.hour_select2').select2({
                templateResult: formatState
            });
        }, 500);
    }

    $scope.onChangeHour = function (hrid) {
        if (hrid) {
            if ($scope.optSlots != null && $scope.optSlots.length > 0) {
                $scope.optSlots = [];
                $scope.timeslot = "";
                setTimeout(function () {
                    $('.timeSlot_select2').select2();
                }, 150);
            }

            var SlotList = $scope.optHours;//slotTimeData[0].time_slots_hour;
            if (SlotList != null) {
                for (let i = 0; i < SlotList.length; i++) {
                    if (SlotList[i].hour_id == parseInt(hrid)) {
                        $scope.optSlots = SlotList[i].time_slots_minutes;
                        break;
                    }
                }
            } else {
                var timeSlotData = JSON.parse('{"time_slots_hour":[{"hour_id":1,"hour":"10:00 - 11:00","status":1,"time_slots_minutes":[{"id":"1","slot":"10:00 - 10:10","status":"1"},{"id":"2","slot":"10:10 - 10:20","status":"1"},{"id":"3","slot":"10:20 - 10:30","status":"1"},{"id":"4","slot":"10:30 - 10:40","status":"1"},{"id":"5","slot":"10:40 - 10:50","status":"1"},{"id":"6","slot":"10:50 - 11:00","status":"1"}]},{"hour_id":2,"hour":"11:00 - 12:00","status":1,"time_slots_minutes":[{"id":"1","slot":"11:00 - 11:10","status":"1"},{"id":"2","slot":"11:10 - 11:20","status":"1"},{"id":"3","slot":"11:20 - 11:30","status":"1"},{"id":"4","slot":"11:30 - 11:40","status":"1"},{"id":"5","slot":"11:40 - 11:50","status":"1"},{"id":"6","slot":"11:50 - 12:00","status":"1"}]},{"hour_id":3,"hour":"12:00 - 01:00","status":1,"time_slots_minutes":[{"id":"1","slot":"12:00 - 12:10","status":"1"},{"id":"2","slot":"12:10 - 12:20","status":"1"},{"id":"3","slot":"12:20 - 12:30","status":"1"},{"id":"4","slot":"12:30 - 12:40","status":"1"},{"id":"5","slot":"12:40 - 12:50","status":"0"},{"id":"6","slot":"12:50 - 01:00","status":"0"}]},{"hour_id":4,"hour":"01:00 - 02:00","status":0,"time_slots_minutes":[{"id":"1","slot":"01:00 - 01:10","status":"0"},{"id":"2","slot":"01:10 - 01:20","status":"0"},{"id":"3","slot":"01:20 - 01:30","status":"0"},{"id":"4","slot":"01:30 - 01:40","status":"0"},{"id":"5","slot":"01:40 - 01:50","status":"0"},{"id":"6","slot":"01:50 - 02:00","status":"0"}]},{"hour_id":5,"hour":"02:00 - 03:00","status":1,"time_slots_minutes":[{"id":"1","slot":"02:00 - 02:10","status":"0"},{"id":"2","slot":"02:10 - 02:20","status":"0"},{"id":"3","slot":"02:20 - 02:30","status":"0"},{"id":"4","slot":"02:30 - 02:40","status":"0"},{"id":"5","slot":"02:40 - 02:50","status":"0"},{"id":"6","slot":"02:50 - 03:00","status":"1"}]},{"hour_id":6,"hour":"03:00 - 04:00","status":1,"time_slots_minutes":[{"id":"1","slot":"03:00 - 03:10","status":"1"},{"id":"2","slot":"03:10 - 03:20","status":"1"},{"id":"3","slot":"03:20 - 03:30","status":"1"},{"id":"4","slot":"03:30 - 03:40","status":"1"},{"id":"5","slot":"03:40 - 03:50","status":"1"},{"id":"6","slot":"03:50 - 04:00","status":"1"}]},{"hour_id":7,"hour":"04:00 - 05:00","status":1,"time_slots_minutes":[{"id":"1","slot":"04:00 - 04:10","status":"1"},{"id":"2","slot":"04:10 - 04:20","status":"1"},{"id":"3","slot":"04:20 - 04:30","status":"1"},{"id":"4","slot":"04:30 - 04:40","status":"1"},{"id":"5","slot":"04:40 - 04:50","status":"1"},{"id":"6","slot":"04:50 - 05:00","status":"1"}]},{"hour_id":8,"hour":"05:00 - 06:00","status":1,"time_slots_minutes":[{"id":"1","slot":"05:00 - 05:10","status":"1"},{"id":"2","slot":"05:10 - 05:20","status":"1"},{"id":"3","slot":"05:20 - 05:30","status":"1"},{"id":"4","slot":"05:30 - 05:40","status":"1"},{"id":"5","slot":"05:40 - 05:50","status":"1"},{"id":"6","slot":"05:50 - 06:00","status":"1"}]}]}');

                for (var hrw = 0; hrw < timeSlotData.time_slots_hour.length; hrw++) {
                    if (timeSlotData.time_slots_hour[hrw].hour_id == hrid) {
                        $scope.optSlots = timeSlotData.time_slots_hour[hrw].time_slots_minutes;
                        break;
                    }
                }
            }

            if ($scope.optType == 'DISABLE') {
                $scope.GetBookedSlotsCount();
            } else {
                $scope.dv_NoOfSlots = false;
            }

            format_timeslots();

        }
    }
         
    function format_timeslots() {
        setTimeout(function () {             
            $('.timeSlot_select2').select2({
                templateResult: formatState
            });
        }, 500);
    }
    $scope.onChangeSlot = function (timeslot) {
        if (timeslot) {
            if ($scope.optType == 'DISABLE') {
                $scope.GetBookedSlotsCount();
            } else {
                $scope.dv_NoOfSlots = false;
            }
        }
    }

    $scope.DisableSubmit = function () {
        if ($scope.ValidateForms()) {
            if (confirm('Are you sure do you want to update this change??')) {
                if ($scope.optType == 'DISABLE') {
                    $scope.SubmitForms();
                } else {
                    displayMessage('danger', 'please select category');
                }
            }
        }
    }

    $scope.EnableSubmit = function () {
        if ($scope.ValidateForms()) {
            if (confirm('Are you sure do you want to update this change??')) {
                if ($scope.optType == 'ENABLE') {
                    $scope.SubmitForms();
                } else {
                    displayMessage('danger', 'please select category');
                }
            }
        }
    }

    $scope.ValidateForms = function () {

        if ($scope.dv_district && $scope.dv_center && $scope.dv_slot_date && $scope.dv_hour_slot && $scope.dv_time_slot) {
            if ($scope.optType == null || $scope.optType == "") {
                displayMessage('danger', 'Please select action');
                return false;
            } else if ($scope.typeVal == null || $scope.typeVal == "") {
                displayMessage('danger', 'Please select category');
                return false;
            } else if ($scope.district == null || $scope.district == "") {
                displayMessage('danger', 'Please select district');
                return false;
            } else if ($scope.center == null || $scope.center == "") {
                displayMessage('danger', 'Please select centers');
                return false;
            } else if ($scope.slotdate == null || $scope.slotdate == "") {
                displayMessage('danger', 'Please select slot date');
                return false;
            } else if ($scope.hourslot == null || $scope.hourslot == "") {
                displayMessage('danger', 'Please select hour');
                return false;
            } else if ($scope.timeslot == null || $scope.timeslot == "") {
                displayMessage('danger', 'Please select time slot');
                return false;
            } else {
                return true;
            }
        } else if ($scope.dv_district && $scope.dv_center && $scope.dv_slot_date && $scope.dv_hour_slot) {
            if ($scope.optType == null || $scope.optType == "") {
                displayMessage('danger', 'Please select action');
                return false;
            } else if ($scope.typeVal == null || $scope.typeVal == "") {
                displayMessage('danger', 'Please select category');
                return false;
            } else if ($scope.district == null || $scope.district == "") {
                displayMessage('danger', 'Please select district');
                return false;
            } else if ($scope.center == null || $scope.center == "") {
                displayMessage('danger', 'Please select centers');
                return false;
            } else if ($scope.slotdate == null || $scope.slotdate == "") {
                displayMessage('danger', 'Please select slot date');
                return false;
            } else if ($scope.hourslot == null || $scope.hourslot == "") {
                displayMessage('danger', 'Please select hour');
                return false;
            } else {
                return true;
            }
        } else if ($scope.dv_district && $scope.dv_center && $scope.dv_slot_date) {
            if ($scope.optType == null || $scope.optType == "") {
                displayMessage('danger', 'Please select action');
                return false;
            } else if ($scope.typeVal == null || $scope.typeVal == "") {
                displayMessage('danger', 'Please select category');
                return false;
            } else if ($scope.district == null || $scope.district == "") {
                displayMessage('danger', 'Please select district');
                return false;
            } else if ($scope.center == null || $scope.center == "") {
                displayMessage('danger', 'Please select centers');
                return false;
            } else if ($scope.slotdate == null || $scope.slotdate == "") {
                displayMessage('danger', 'Please select slot date');
                return false;
            } else {
                return true;
            }
        } else if ($scope.dv_district && $scope.dv_center) {
            if ($scope.optType == null || $scope.optType == "") {
                displayMessage('danger', 'Please select action');
                return false;
            } else if ($scope.typeVal == null || $scope.typeVal == "") {
                displayMessage('danger', 'Please select category');
                return false;
            } else if ($scope.district == null || $scope.district == "") {
                displayMessage('danger', 'Please select district');
                return false;
            } else if ($scope.center == null || $scope.center == "") {
                displayMessage('danger', 'Please select centers');
                return false;
            } else {
                return true;
            }
        } else if ($scope.dv_district) {
            if ($scope.optType == null || $scope.optType == "") {
                displayMessage('danger', 'Please select action');
                return false;
            } else if ($scope.typeVal == null || $scope.typeVal == "") {
                displayMessage('danger', 'Please select category');
                return false;
            } else if ($scope.district == null || $scope.district == "") {
                displayMessage('danger', 'Please select district');
                return false;
            } else {
                return true;
            }
        }
    }

    $scope.SubmitForms = function () {
        $scope.isLoading = true;
        AadharFactory.rawItems($('#hdnAdminSubmit').val(), { actiontype: $scope.optType, actionvalue: $scope.typeVal, districts: $('#slotDistricts option:selected').text(), centerid: $('#slotCenters').val().toString(), slotdate: $scope.slotdate, hourid: $scope.hourslot, slotid: $scope.timeslot, Securitykey: $('#Securitykey').val() })
        .then(function (res) {
            if (res) {
                var rawData = res.rawData;
                if (rawData.resCode == '000') {
                    displayMessage('success', 'Changed successfully.');
                    setTimeout(function () {
                        window.location.reload();
                    }, 500);
                } else {
                    displayMessage('danger', rawData.resDesc);
                    $scope.isLoading = false;
                }
            } else {
                $scope.isLoading = false;
            }
        });
    }
    $scope.NoOfBookedSlots = "0";
    $scope.GetBookedSlotsCount = function () {
        $scope.isLoading = true;
        AadharFactory.rawItems($('#hdnBookedSlotsCount').val(), { districts: $('#slotDistricts option:selected').text(), centerid: $('#slotCenters').val().toString(), slotdate: $scope.slotdate, hourid: $scope.hourslot, slotid: $scope.timeslot, Securitykey: $('#Securitykey').val() })
       .then(function (res) {
           if (res) {
               var rawData = res.rawData;
               if (rawData.resCode == '000') {
                   // $scope.optDistrict = rawData.districts;
                   $scope.NoOfBookedSlots = rawData.resDesc;
                   $scope.isLoading = false; $scope.dv_NoOfSlots = true;
               } else {
                   displayMessage('danger', rawData.resDesc);
                   $scope.isLoading = false;
               }
           } else {
               $scope.isLoading = false;
           }
       });
    }

    $scope.reSet = function () {
        $scope.isLoading = true;
        setTimeout(function () {
            window.location.reload();
        }, 500);
    }
  
}]);