<%@ Page Title="Редактор новости" Language="C#" MasterPageFile="~/adm/Adm.Master" AutoEventWireup="true" CodeBehind="newsedit.aspx.cs" Inherits="site.adm.newsedit" EnableSessionState="True" MaintainScrollPositionOnPostback="True" ValidateRequest="False" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
    <script src="../Scripts/ckeditor/ckeditor.js" type="text/javascript"></script>
    
    <script src="../Scripts/filemanager/fm.js?v=1" type="text/javascript"></script>
    <link rel="stylesheet" href="../Scripts/filemanager/fm.css?v=1" type="text/css" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Panel ID="addPanel" runat="server">
        <%-- панель для добавления содержимого страницы --%>
    </asp:Panel>
</asp:Content>
