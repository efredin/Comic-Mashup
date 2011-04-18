<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Web.Master" Inherits="Fredin.Comic.Web.ComicViewPage<System.Web.Mvc.HandleErrorInfo>" %>
<%@ Import Namespace="log4net" %>

<asp:Content ID="cPageTitle" ContentPlaceHolderID="cphPageTitle" runat="server">Error | Comic Mashup</asp:Content>
<asp:Content ID="cTitle" ContentPlaceHolderID="cphTitle" runat="server">Error</asp:Content>
<asp:Content ID="cCanvas" ContentPlaceHolderID="cphCanvas" runat="server">

	Something went terribly wrong. Our support team has been notified. Sorry for the inconvenience.

	<% #if DEBUG %>
	<p class="debug-trace"><%: MvcHtmlString.Create(Model.Exception.ToString().Replace("\n", "<br/>")) %></p>
	<% #endif %>
	<% LogManager.GetLogger(this.GetType()).Error("Unhandled Error", Model.Exception); %>

</asp:Content>