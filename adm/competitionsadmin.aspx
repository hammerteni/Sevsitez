<%@ Page Title="Конкурсы" Language="C#" MasterPageFile="~/adm/Adm.Master" AutoEventWireup="true" 
    CodeBehind="competitionsadmin.aspx.cs" Inherits="site.adm.competitionsadmin" EnableSessionState="True" MaintainScrollPositionOnPostback="True" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="/Scripts/adm_competitions.js?v=16"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <span id='competitions_code' style="display: none;">325kjghksfdjgher9874354idshfdgkjsht348576sjefkhdsjkg</span>

    <asp:Panel ID="addPanel" runat="server">
        <%-- панель для добавления панели управления статистикой --%>
    </asp:Panel>

</asp:Content>
