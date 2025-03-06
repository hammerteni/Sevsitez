<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true"
    CodeBehind="sitemap.aspx.cs" Inherits="site.sections.sitemap.sitemap" EnableSessionState="True"
    MaintainScrollPositionOnPostback="True" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:Panel runat="server" ID="addPanel"></asp:Panel>

    <asp:TreeView ID="TreeView1" 
        runat="server" 
        DataSourceID="sitemapDataSource" 
        CssClass="mainStyle_sm" 
        ForeColor="#663300" 
        HoverNodeStyle-CssClass="nodeStyleHover_sm" 
        HoverNodeStyle-ForeColor="#CC3300" 
        NodeStyle-CssClass="nodeStyle_sm"
        OnTreeNodeDataBound="Data_Bound">

    </asp:TreeView>
        
</asp:Content>