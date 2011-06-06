<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Web.Master" Inherits="Fredin.Comic.Web.ComicViewPage<Fredin.Comic.Web.Models.ViewAuthor>" %>

<asp:Content ID="cPageTitle" ContentPlaceHolderID="cphPageTitle" runat="server"><%: Model.User.Nickname %> | Comic Mashup</asp:Content>
<asp:Content ID="cMeta" ContentPlaceHolderID="cphMeta" runat="server">
	<meta property="og:title" content="<%: Model.User.Nickname %>" />
	<meta property="og:description" content="Comics by <%: Model.User.Nickname %>" />
	<meta property="og:type" content="article" />
	<meta property="og:image" content="<%: Model.User.ThumbUrl %>" />
	<meta property="og:url" content="<%: Model.User.AuthorUrl %>" />
	<meta property="description" content="Comics by <%: Model.User.Nickname %>" />
	<meta property="author" content="<%: Model.User.Nickname %>" />
</asp:Content>

<asp:Content ID="cTitle" ContentPlaceHolderID="cphTitle" runat="server"><%: Model.User.Nickname %></asp:Content>
<asp:Content ID="cCanvas" ContentPlaceHolderID="cphCanvas" runat="server">
	
	<div id="directory-author" class="content760">
		<div id="directory-author-comics">
			<%= Html.Partial("~/Views/Comic/ComicList.ascx", Model.Comics) %>
		</div>
	</div>

</asp:Content>