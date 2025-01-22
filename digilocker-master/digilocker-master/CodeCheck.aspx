<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CodeCheck.aspx.cs" Inherits="CodeCheck" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
     <!-- Global js-->
    <script src="Scripts/jquery-1.12.4.min.js" type="text/javascript"></script>
    <script src="Scripts/jquery.blockUI.js" type="text/javascript"></script>
    <script src="Scripts/bootstrap.min.js" type="text/javascript"></script>
    <script src="Scripts/bootstrap.bundle.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="js/DGUpload.js" id="dlshare" data-client-url="<%=strDGURL %>"></script>
    <script>
        $(function () {
            var DGUrl = $('#dlshare').attr('data-client-url');
            window.open(DGUrl, "_self");
        });
    </script>
</body>
</html>
