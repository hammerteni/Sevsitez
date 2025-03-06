
<%@ Register Assembly="Recaptcha.Web" Namespace="Recaptcha.Web.UI.Controls" TagPrefix="cc1" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true"
    CodeBehind="onenews.aspx.cs" Inherits="site.onenews" EnableSessionState="True"
    MaintainScrollPositionOnPostback="True" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="/App_Themes/site/news.css?v=0" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<%--    <cc1:RecaptchaWidget ID="Recaptcha1" runat="server" />
    <cc1:RecaptchaApiScript ID="RecaptchaApiScript1" runat="server" />
<cc1:RecaptchaWidget ID="RecaptchaWidget1" RenderApiScript="false" runat="server" />
<cc1:RecaptchaWidget ID="RecaptchaWidget2" RenderApiScript="false" runat="server" />--%>
    <asp:Panel runat="server" ID="addPanel"></asp:Panel>

</asp:Content>
