<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Web.Master" Inherits="System.Web.Mvc.ViewPage<Fredin.Comic.Web.Models.ViewDirectory>" %>
<%@ Import Namespace="Fredin.Util" %>

<asp:Content ID="cPageTitle" ContentPlaceHolderID="cphPageTitle" runat="server"><%: Model.Mode.GetDescription() %> | Comic Mashup</asp:Content>
<asp:Content ID="cTitle" ContentPlaceHolderID="cphTitle" runat="server"><%: Model.Mode.GetDescription() %></asp:Content>
<asp:Content ID="cCanvas" ContentPlaceHolderID="cphCanvas" runat="server">

	<%= Html.Partial("~/Views/Shared/AdSkyscraper.ascx") %>

	<div id="directory">
		<%= Html.Partial("~/Views/Comic/ComicList.ascx", Model.Comics) %>
	</div>

    <script type="text/javascript">
    	appOptions.menuSelected = 0;
	</script>

</asp:Content>