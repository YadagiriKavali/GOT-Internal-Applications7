/// <reference path="../an-js/angular.min.js" />
var Aadhar = angular.module("AadharModule", ['ngMaterial']);

/*Accepts Numerics only */
Aadhar.directive('numericsOnly', function () {
    return {
        restrict: "A",
        compile: function (tElement, tAttrs) {
            return function (scope, element, attrs) {

                element[0].addEventListener('keydown', function (e) {
                    var code = e.keyCode ? e.keyCode : e.which ? e.which : e.charCode;
                    // Allow: backspace, delete, tab, escape, enter and .
                    if ($.inArray(code, [46, 8, 9, 27, 13, 110]) !== -1 ||
                        // Allow: Ctrl+A,Ctrl+C,Ctrl+V, Command+A
                           ((code == 65 || code == 86 || code == 67) && (e.ctrlKey === true || e.metaKey === true)) ||
                        // Allow: home, end, left, right, down, up
                        (code >= 35 && code <= 40)) {
                        // let it happen, don't do anything
                        return;
                    }
                    // Ensure that it is a number and stop the keypress
                    if ((e.shiftKey || (code < 48 || code > 57)) && (code < 96 || code > 105)) {
                        e.preventDefault();
                    }

                });
            };
        }
    };
});

/*Accepts Alphabets Only */
Aadhar.directive('charWithSpace', function () {
    return {
        restrict: "A",
        compile: function (tElement, tAttrs) {
            return function (scope, element, attrs) {
                element.bind("keypress", function (event) {
                    var keyCode = event.which || event.keyCode;
                    var keyCodeChar = String.fromCharCode(keyCode);
                    if (!keyCodeChar.match(new RegExp(/[a-zA-Z\s\b]/, "i"))) {
                        event.preventDefault();
                        return false;
                    }
                });
            };
        }
    };
});


function isHTML(str) {
    var a = document.createElement('div');
    a.innerHTML = str;

    for (var c = a.childNodes, i = c.length; i--;) {
        if (c[i].nodeType == 1) return true;
    }

    return false;
}

function displayMessage(Type, Msg) {
    bootoast.toast({
        message: Msg,
        type: Type,
        timeout: 3,
        timeoutProgress: false
    });
}

Aadhar.factory("AadharFactory", ['$http', '$q', function ($http, $q) {
    return {
        rawItems: function (_url, paramObj) {
            try {
                return $http.post(_url, JSON.stringify(paramObj))
                .then(function (res) {                     
                    if (isHTML(res.data) || res.data.code == '401') {
                        //displayMessage('danger', 'Internal Server Error');
                        window.location.href = '../Home/Logout';
                        return;
                    }
                    $('#Securitykey').val(res.data.Securitykey);
                    return res.data;
                }, function (errRes) {
                    $q.reject(errRes);
                    window.location.href = '../Home/Logout';
                    return;
                });
            } catch (e) {
                console.log(e);
            }
        }
    };
}]);