<%@ Page Title="Редактор текстовых страниц" Language="C#" MasterPageFile="~/adm/Adm.Master" AutoEventWireup="true" CodeBehind="pageseditor.aspx.cs" 
    Inherits="site.adm.pageseditor" EnableSessionState="True" MaintainScrollPositionOnPostback="True" ValidateRequest="False" EnableViewState="True" %>
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
