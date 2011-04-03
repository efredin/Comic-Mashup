<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Web.Master" Inherits="System.Web.Mvc.ViewPage<Fredin.Comic.Web.Models.ViewProfile>" %>

<asp:Content ID="cPageTitle" ContentPlaceHolderID="cphPageTitle" runat="server"><%: Model.User.Nickname %> | Comic Mashup</asp:Content>
<asp:Content ID="cHead" ContentPlaceHolderID="cphHead" runat="server">
	<meta property="og:title" content="<%: Model.User.Nickname %>" />
	<meta property="og:description" content="Comics by <%: Model.User.Nickname %>" />
	<meta property="og:type" content="article" />
	<meta property="og:image" content="<%: Model.User.ThumbUrl %>" />
	<meta property="og:url" content="<%: Model.User.ProfileUrl %>" />
</asp:Content>

<asp:Content ID="cTitle" ContentPlaceHolderID="cphTitle" runat="server"><%: Model.User.Nickname %></asp:Content>
<asp:Content ID="cCanvas" ContentPlaceHolderID="cphCanvas" runat="server">
	
	<%= Html.Partial("~/Views/Shared/AdSkyscraper.ascx") %>

	<div id="profile">

		

		<div id="profile-comics">
			<%= Html.Partial("~/Views/Comic/ComicList.ascx", Model.Comics) %>
		</div>

		<div class="columnRight">
			<p class="wideskyscraper">Ad Here</p>
		</div>

	</div>

	<script type="text/javascript">
	appOptions.menuSelected = 2;
	</script>

</asp:Content>