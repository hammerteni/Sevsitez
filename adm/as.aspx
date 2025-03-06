<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="as.aspx.cs" Inherits="site.adm._as" EnableSessionState="True" MaintainScrollPositionOnPostback="True" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Вход в консоль управления</title>
    <link href="~/App_Themes/site/cssreset.css" rel="stylesheet" type="text/css" />
    <link href="~/App_Themes/site/admconsole.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="/Scripts/jquery-2.1.4.min.js"></script>
    <script type="text/javascript" src="/Scripts/site.js?v=11"></script>
</head>
<body>
    <form id="form1" runat="server">
    
    <%-- появляющаяся информационная строка --%>
    <div id="divWarnWrapper" runat="server" class="divWarnWrapper" Visible="False">
        <div class="divAttentionLeft">!</div>
        <div class="divAttentionContent">
            <asp:Label ID="attentionContent" runat="server" Text="">
                <%-- содержимое добавляется из кода --%>
            </asp:Label>
        </div><div class="divClose"><a href="" onclick="return warnVisible();">закрыть</a></div>
        <div class="divAttentionRight">!</div>
    </div>
    
    <%-- появляющееся на весь экран окно на полупрозрачном фоне(для авторизации) --%>
    <div id="divWindowWrapper" runat="server" class="divWindowWrapper" Visible="True">
        <div id="divWindow" runat="server" class="divWindow divAuth">
            <%-- <div class="divWindowClose"><a href="" onclick="windowVisible('divWindowWrapper'); return false;">закрыть</a></div> --%>
            <div id="divWindowContent" runat="server" class="divWindowContent">
                <%-- содержимое добавляется из кода --%>
            </div>
        </div>
    </div>

    </form>
</body>
</html>
