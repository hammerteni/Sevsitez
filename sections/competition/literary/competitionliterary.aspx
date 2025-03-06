<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true"
    CodeBehind="competitionliterary.aspx.cs" Inherits="site.sections.project.competitionliterary.competitionliterary" EnableSessionState="True"
    MaintainScrollPositionOnPostback="True" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="/App_Themes/site/competitionspages.css?v=5" type="text/css" />
    <script type="text/javascript" src="/Scripts/voting.js?v=1"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <a class="aCompetitionReg" href="reg.aspx">Отправить заявку на участие</a>

    <asp:Panel runat="server" ID="addPanel"></asp:Panel>
    <asp:Panel runat="server" ID="votingPanels"></asp:Panel>
        
</asp:Content>