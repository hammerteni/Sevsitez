<%@ Page Title="Редактор новостной ленты" Language="C#" MasterPageFile="~/adm/Adm.Master" AutoEventWireup="true" CodeBehind="news.aspx.cs" Inherits="site.adm.news" EnableSessionState="True" MaintainScrollPositionOnPostback="True" ValidateRequest="False" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Panel ID="addPanel" runat="server">
        <%-- панель для добавления содержимого страницы --%>
    </asp:Panel>
</asp:Content>
