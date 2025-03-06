<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true"
    CodeBehind="competitiontoponim.aspx.cs" Inherits="site.sections.project.competitiontoponim.competitiontoponim" EnableSessionState="True"
    MaintainScrollPositionOnPostback="True" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="/App_Themes/site/stages.css?v=0" type="text/css" />
    <link rel="stylesheet" href="/App_Themes/site/competitionspages.css?v=10" type="text/css" />
    <link rel="stylesheet" href="/App_Themes/site/competitionsresults.css?v=1" type="text/css" />
    <script type="text/javascript" src="/Scripts/voting.js?v=2"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:Panel runat="server" ID="addPanel"></asp:Panel>
    <asp:Panel runat="server" ID="votingPanels"></asp:Panel>
        
</asp:Content>