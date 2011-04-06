<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Web.Master" Inherits="System.Web.Mvc.ViewPage<System.Web.Mvc.HandleErrorInfo>" %>

<asp:Content ID="cPageTitle" ContentPlaceHolderID="cphPageTitle" runat="server">Error | Comic Mashup</asp:Content>
<asp:Content ID="cTitle" ContentPlaceHolderID="cphTitle" runat="server">Error</asp:Content>
<asp:Content ID="cCanvas" ContentPlaceHolderID="cphCanvas" runat="server">

    <h1>Error</h1>
	<%: Model.Exception.Message %>
	<% #if DEBUG %>
	<p class="debug-trace"><%: MvcHtmlString.Create(Model.Exception.ToString().Replace("\n", "<br/>")) %></p>
	<% #endif %>

</asp:Content>