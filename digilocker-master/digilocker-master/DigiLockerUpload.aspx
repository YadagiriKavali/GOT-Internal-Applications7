<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DigiLockerUpload.aspx.cs" Inherits="DigiLockerUpload" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd" >

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Files - DigiLocker</title>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, minimum-scale=1.0, maximum-scale=1.0">
    <meta name="apple-mobile-web-app-capable" content="yes">
    <meta name="apple-mobile-web-app-status-bar-style" content="black">
    <meta name="apple-mobile-web-app-title" content="Digilocker">
    <meta name="mobile-web-app-capable" content="yes">
    <link rel="shortcut icon" type="image/png" href="https://devservices.digitallocker.gov.in/core/img/favicon.png" />
    <link rel="apple-touch-icon-precomposed" href="https://devservices.digitallocker.gov.in/core/img/favicon-touch.png" />

    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <meta name="description" content="">
    <meta name="author" content="">
    <link rel="shortcut icon" href="assets/images/mobileone.ico">
    <!--iphone mainfest st-->
    <link rel="icon" sizes="192x192" href="assets/images/launcher-icon-4x.png" />
    <link rel="apple-touch-icon" href="assets/images/launcher-icon-4x.png" />
    <meta name="theme-color" content="#ed1c24">
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <meta name="mobile-web-app-capable" content="yes" />
    <!--iphone mainfest end-->
    <link rel="shortcut icon" href="assets/images/favicon.ico">
    <!-- Global css -->
    <link href="assets/global/css/bootstrap.min.css" rel="stylesheet" type="text/css">
    <link href="assets/global/font-awesome/fontawesome.min.css" rel="stylesheet" type="text/css">
    <link href="https://use.fontawesome.com/releases/v5.0.10/css/all.css" rel="stylesheet">
    <!-- Menu css -->
    <link rel="stylesheet" href="assets/css/sidenav.min.css" type="text/css">
    <!-- Slider css -->
    <link href="assets/css/swiper.min.css" rel="stylesheet" type="text/css">
    <!-- Custom css -->
    <link href="assets/css/custom.css" rel="stylesheet" type="text/css">
    <link href="assets/css/roboto.css" rel="stylesheet" type="text/css">
    <link href="assets/css/jquery.auto-complete.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/datepicker.css" rel="stylesheet" />
    <link href="assets/global/simple-line-icons/simple-line-icons.min.css" rel="stylesheet"
        type="text/css">
    <!-- page top scrolling -->
    <style>
        .footer {
            position: absolute;
            bottom: 0;
            width: 100%;
            height: 60px;
            line-height: 20px;
        }
    </style>
</head>
<body dir="top">
    <form id="formdgupload" method="post" runat="server" autocomplete="off">
        <div class="bmtc">
            <!-- Main Container -->
            <main role="main">
                <div class="container">                    
                    <div class="par-sec1">                        
                        <div class="row marzero marbtm_20"> 
                            <!--left colm st-->
                            <div class="default-primary-color text-white col-12 p-3 ">
                        <h3 class="col-12 p-0 text-white h3">Documents</h3>
                       <div class="form-group m-0">
                                                <div class="form-check form-check-inline ">
                                                    <input class="form-check-input" name="inlineRadioOptions" id="rdbupload" value="option1" checked="" type="radio" />
                                                    <label class="form-check-label" for="inlineRadio1">Uploaded</label>
                                                </div>    
                                                <div class="form-check form-check-inline">
                                                    <input class="form-check-input" name="inlineRadioOptions" id="rdbissued" value="option2" type="radio" />
                                                    <label class="form-check-label" for="inlineRadio2">Issued</label>
                                                </div>                                               
                                                
                                            </div>

                    </div>
                          <%--  <div class="default-primary-color notice_top text-primary-color">
                                <p class="page-section-header">Uploaded Documents</p>
                            This is where you can upload your own documents &amp; certificates. You can also <strong> <a target="_blank" href="https://digilocker.gov.in/assets/img/esignbrochure1.5.pdf" style="color: #fff;">eSign</a></strong> them.
                            </div>--%>
                            
                            <div class="col-12 p-0 mt-2 bg-white">
                                <div class="table-responsive">
                                    <div class="table table-striped table-hover" id="tViewDtls" name="tViewDtls" runat="server">
                                    </div>
                                    <div class="table table-striped table-hover hide" id="tIssuesDtls" name="tIssuesDtls" runat="server">
                                    </div>
                                </div>
                            </div>                               
                        </div>
                    </div>

                    
                </div>
                <input type="hidden" id="hdnlang" value="0" />
                <input type="hidden" id="hdnchkvalue" value="0" />
                <input type="hidden" id="hdnisalreadyenrolled" />

                <input type="hidden" id="hdn_accessToken" value="<%=AccessToken%>" />
                <input type="hidden" id="hdn_code" value="<%=AuthCode%>" />
                <input type="hidden" id="hdn_RefreshToken" value="<%=reFreshToken%>" />
                <input type="hidden" id="hdn_ExpireTime" value="<%=expireTime%>" />
                <!-- Footer -->                
            </main>
            <!--top scrolling-->
            <div id="back-top">
                <a href="#" class="scrollup">
                    <img src="../assets/images/arrow-up-new1.svg" alt="top scrolling"></a>
            </div>
        </div>
        <footer class="pt-3 pb-3 text-center text-small footer">
                    <div class="container">
                        
                    </div>
                </footer>
    </form>
    <!-- Global js-->
    <script src="Scripts/jquery-1.12.4.min.js" type="text/javascript"></script>
    <script src="Scripts/jquery.blockUI.js" type="text/javascript"></script>
    <script src="Scripts/bootstrap.min.js" type="text/javascript"></script>
    <script src="Scripts/bootstrap.bundle.min.js" type="text/javascript"></script>

    <%--<script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>--%>
    <%--<script type="text/javascript" src="js/DGUpload.js" id="dlshare" data-client-url="<%=strDGURL %>"></script>--%>
    <script type="text/javascript" src="js/DGUpload.js" ></script>
</body>
</html>
