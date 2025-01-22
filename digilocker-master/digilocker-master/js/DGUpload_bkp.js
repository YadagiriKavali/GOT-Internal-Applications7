$(function () {
    //debugger;
    var param1var = getQueryVariable("access_token");
    var param2var = getQueryVariable("code");
    if (param1var || param2var) {
        $("#content-wrapper").show();
        if (param1var != '')
            GetDocuments('', param1var);
    }
    else {
        $("#content-wrapper").show();
        var DGUrl = $('#dlshare').attr('data-client-url');
        window.open(DGUrl, "_self");
    }

    $("#rdbupload").change(function () {
        $('#tViewDtls').html('');
        var param1var = getQueryVariable("access_token");
        var param2var = getQueryVariable("code");
        if (param1var != '')
            GetDocuments('', param1var);
    });

    $("#rdbissued").change(function () {
        $('#tViewDtls').html('');
        var param1var = getQueryVariable("access_token");
        var param2var = getQueryVariable("code");
        if (param1var != '')
            GetIssuedDocuments('', param1var);
    });
});


function getQueryVariable(variable) {
    var query = window.location.href.slice(window.location.href.indexOf('#') + 1);// window.location.search.substring(1);
    var vars = query.split("&");
    for (var i = 0; i < vars.length; i++) {
        var pair = vars[i].split("=");
        if (pair[0] === variable) {
            return pair[1];
        }
    }
}

function GetDocuments(docid, accesstoken) {
    try {
        //debugger;
        $.ajax({
            type: "POST",
            url: "Handlers/DGUpload.ashx?action=getdocfiles&docid=" + docid + "&accesstoken=" + accesstoken,
            headers: { 'SESSTOKEN': $("#sesstoken").val() },
            error: function (xhr, ajaxOptions, thrownError) {
                $.unblockUI();
            },
            success: function (msg) {
                //debugger;
                if (msg.length != 0) {
                    var data = msg;
                    if (docid != '')
                        fillGrid(data, accesstoken);
                    else
                        fillDocGrid(data, accesstoken);
                }
                else {
                    emptyGrid();
                }
            },
            beforeSend: function () { processingRequest(); },
            complete: function (xhr, ststus) {
                $.unblockUI();
            }
        });
    }
    catch (e) {
        //alert(e);
        $.unblockUI();
    }
}

function GetIssuedDocuments(docid, accesstoken) {
    try {
        //debugger;
        $.ajax({
            type: "POST",
            url: "Handlers/DGUpload.ashx?action=getissueddocfiles&docid=" + docid + "&accesstoken=" + accesstoken,
            headers: { 'SESSTOKEN': $("#sesstoken").val() },
            error: function (xhr, ajaxOptions, thrownError) {
                $.unblockUI();
            },
            success: function (msg) {
                //debugger;
                if (msg.length != 0) {
                    var data = msg;
                    if (docid != '')
                        fillGrid(data, accesstoken);
                    else
                        fillDocGrid(data, accesstoken);
                }
                else {
                    emptyGrid();
                }
            },
            beforeSend: function () { processingRequest(); },
            complete: function (xhr, ststus) {
                $.unblockUI();
            }
        });
    }
    catch (e) {
        //alert(e);
        $.unblockUI();
    }
}

function fillGrid(records, accesstoken) {
    try {
        //debugger;
        var dgheaders = "Name,Size,Updated".split(',');
        var dgcolWidth = "50%,25%,25%".split(',');
        $('#tViewDtls').html('');
        $('#tViewDtls').append('<tbody></tbody>');
        if (records != null) {
            for (var j = 0; j < records.length; j++) {
                var myTr = '\n<tr>';
                var iSizeinKB = parseInt(records[j].size);
                iSizeinKB = (iSizeinKB / 1024);
                myTr += '<td valign="top" style="width:' + dgcolWidth[0] + '" onclick="GetPDF(\'' + records[j].uri + '\',\'' + accesstoken + '\',\'' + records[j].name + '\',\'' + records[j].mime + '\',\'' + records[j].issuer + '\')">' + records[j].name + '</td>';
                //myTr += '<td valign="top" style="width:' + dgcolWidth[1] + '">' + Math.round(iSizeinKB) + ' kB</td>';
                myTr += '<td valign="top" style="width:' + dgcolWidth[2] + '">' + records[j].date + '</td>';
                myTr += '</tr>';
                $('#tViewDtls tbody').append(myTr);
            }
        }
        else
            emptyGrid();

        $('#tViewDtls').dataTable({
            "jQueryUI": true,
            "destroy": true,
            "autoWidth": true,
            "bLengthChange": false,
            "sPaginationType": "full_numbers",
            "language": {
                "emptyTable": "No Details Found."
            },
            "columnDefs": [
                                { "sTitle": "Name", sWidth: "50%", "bSortable": true, "targets": 0 },
                                //{ "sTitle": "Size", sWidth: "25%", "bSortable": true, "targets": 1 },
                                { "sTitle": "Updated", sWidth: "25%", "bSortable": true, "targets": 1 }
            ]
        });
    }
    catch (e) {
        //alert(e);
    }
}

function fillDocGrid(records, accesstoken) {
    try {
        //debugger;
        var dgheaders = "Name,Size,Updated".split(',');
        var dgcolWidth = "50%,25%,25%".split(',');
        $('#tViewDtls').html('');
        $('#tViewDtls').append('<tbody></tbody>');
        if (records != null) {
            for (var j = 0; j < records.length; j++) {
                var myTr = '\n<tr>';
                var iSizeinKB = parseInt(records[j].size);
                iSizeinKB = (iSizeinKB / 1024);
                var fileextn = records[j].mime.split('/');
                if (records[j].type == "dir") {
                    myTr += '<td valign="top" style="width:' + dgcolWidth[0] + '" onclick="GetDocuments(' + records[j].id + ',\'' + accesstoken + '\')">' + records[j].name + '</td>';
                }
                else if (records[j].type == "file") {
                    myTr += '<td valign="top" style="width:' + dgcolWidth[0] + '" onclick="GetPDF(\'' + records[j].uri + '\',\'' + accesstoken + '\',\'' + records[j].name + '\',\'' + records[j].mime + '\',\'' + records[j].issuer + '\')">' + records[j].name + '</td>';
                }
                //myTr += '<td valign="top" style="width:' + dgcolWidth[1] + '">' + Math.round(iSizeinKB) + ' kB</td>';
                myTr += '<td valign="top" style="width:' + dgcolWidth[2] + '">' + records[j].date + '</td>';
                myTr += '</tr>';
                $('#tViewDtls tbody').append(myTr);
            }
        }
        else
            emptyGrid();

        $('#tViewDtls').dataTable({
            "jQueryUI": true,
            "destroy": true,
            "autoWidth": true,
            "bLengthChange": false,
            "sPaginationType": "full_numbers",
            "language": {
                "emptyTable": "No Details Found."
            },
            "columnDefs": [
                                { "sTitle": "Name", sWidth: "50%", "bSortable": true, "targets": 0 },
                                //{ "sTitle": "Size", sWidth: "25%", "bSortable": true, "targets": 1 },
                                { "sTitle": "Updated", sWidth: "25%", "bSortable": true, "targets": 1 }
            ]
        });
    }
    catch (e) {
        //alert(e);
    }
}


// No Records Founs
function emptyGrid() {
    try {
        $('#tViewDtls').html('');
        $('#tViewDtls').append('<tbody></tbody>');
        $('#tViewDtls').dataTable({
            "jQueryUI": true,
            "destroy": true,
            "autoWidth": true,
            "bLengthChange": false,
            "sPaginationType": "full_numbers",
            "language": {
                "emptyTable": "No Details Found."
            },
            "columnDefs": [
                                { "sTitle": "Name", sWidth: "50%", "bSortable": true, "targets": 0 },
                                //{ "sTitle": "Size", sWidth: "25%", "bSortable": true, "targets": 1 },
                                { "sTitle": "Updated", sWidth: "25%", "bSortable": true, "targets": 1 }
            ]
        });
    }
    catch (e) {
    }
}

function GetPDF(uri, accesstoken, filename, mimetype, issuer) {
    //$.ajax({
    //    type: "POST",
    //    beforeSend: function (request) {
    //        request.setRequestHeader("Authorization", "Bearer " + accesstoken);
    //    },
    //    url: "https://developers.digitallocker.gov.in/public/oauth2/1/file/" + uri,
    //    processData: false,
    //    success: function (msg) {
    //        $("#results").append("The result =" + StringifyPretty(msg));
    //    }
    //});
    //window.open("https://developers.digitallocker.gov.in/public/oauth2/1/file/" + uri, "_blank");
    try {
        //debugger;
        $.ajax({
            type: "POST",
            url: "Handlers/DGUpload.ashx?action=downloadpdffiles&uri=" + uri + "&accesstoken=" + accesstoken + "&name=" + filename + "&mimetype=" + mimetype + "&issuer=" + issuer,
            headers: { 'SESSTOKEN': $("#sesstoken").val() },
            error: function (xhr, ajaxOptions, thrownError) {
                $.unblockUI();
            },
            success: function (msg) {
                //debugger;
                if (msg.length != 0) {
                    window.location.href = "https://mgov.telangana.gov.in/DGDownloads/" + msg;
                    //PostURLJson(msg);
                }
                else {
                    emptyGrid();
                }
            },
            beforeSend: function () { processingRequest(); },
            complete: function (xhr, ststus) {
                $.unblockUI();
            }
        });
    }
    catch (e) {
        alert(e);
        $.unblockUI();
    }
}
function processingRequest() {
    $.blockUI({
        message: "<img src='Images/busy.gif' alt='' />",
        css: { width: '32px', height: '32px', top: '45%', left: '45%' }
    });
}


function PostURLJson(msg) {
    try {
        var $form = $("<form/>").attr("id", "data_form")
                                .attr("action", "DigiLockerUpload.aspx")
                                .attr("method", "post");
        $("body").append($form);
        //Append the values to be send
        AddParameter($form, "pdfjsonresp", encodeURIComponent(msg));
        //Send the Form
        $form[0].submit();
    }
    catch (e) {

    }
}
function AddParameter(form, name, value) {
    var $input = $("<input />").attr("type", "hidden")
                                .attr("name", name)
                                .attr("value", value);
    form.append($input);
}