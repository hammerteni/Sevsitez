<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true"
    CodeBehind="map.aspx.cs" Inherits="site.sections.project.map" EnableSessionState="True"
    MaintainScrollPositionOnPostback="True" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="/App_Themes/site/participantsmap.css?v=0" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:Panel runat="server" ID="addPanel"></asp:Panel>
    <asp:Panel runat="server" ID="addYandexMap"></asp:Panel>
    <asp:Panel runat="server" ID="addParticipantsMap"></asp:Panel>
        
</asp:Content>