<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true"
    CodeBehind="default.aspx.cs" Inherits="site._default" EnableSessionState="True"
    MaintainScrollPositionOnPostback="True" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="/App_Themes/site/news.css?v=2" type="text/css" />
    <link rel="stylesheet" href="/App_Themes/site/slidescroll.css?v=0" type="text/css" />
    <style type="text/css">
        div#addAnimFoto {
            margin: 60px 0 0 0;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <%-- <h2 id="about_h2" class="">О нас</h2>
    <asp:Panel runat="server" ID="about_div" CssClass="def_content"></asp:Panel>--%>
    
    <asp:Panel runat="server" ID="addPanel"></asp:Panel>
    <asp:Panel runat="server" ID="addAnimFoto"></asp:Panel>
    
</asp:Content>
