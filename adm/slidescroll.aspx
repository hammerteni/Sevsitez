<%@ Page Title="Редактор анимированного списка фотографий" Language="C#" MasterPageFile="~/adm/Adm.Master" AutoEventWireup="true" CodeBehind="slidescroll.aspx.cs" Inherits="site.adm.slidescroll" EnableSessionState="True" MaintainScrollPositionOnPostback="True" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%-- <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="True">
    </asp:ScriptManager>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            setInterval('OneAsyncPost()', 30000);
        });
        function OneAsyncPost() {
            PageMethods.ReturnOk("", getCallback);

            function getCallback(result, eventArgs) { }
        };
    </script>--%>
    <asp:Panel ID="addPanel" runat="server">
        <%-- панель для добавления содержимого страницы --%>
    </asp:Panel>
</asp:Content>
