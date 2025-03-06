<%@ Page Title="Архивная заявка" Language="C#" MasterPageFile="~/adm/Adm.Master" AutoEventWireup="true" 
    CodeBehind="competitiononearch.aspx.cs" Inherits="site.adm.competitiononearch" EnableSessionState="True" MaintainScrollPositionOnPostback="True" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="/App_Themes/site/competitionone.css?v=4" type="text/css" />
    <script type="text/javascript" src="/Scripts/adm_competitions_arch.js?v=4"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <span id='competitions_code' style="display: none;">325kjghksfdjgher9874354idshfdgkjsht348576sjefkhdsjkg</span>

    <asp:Panel ID="addPanel" runat="server">
        <%-- панель для добавления панели управления статистикой --%>
    </asp:Panel>

</asp:Content>
