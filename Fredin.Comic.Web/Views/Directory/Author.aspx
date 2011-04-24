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
	
	<div class="wideskyscraper">
		<% #if !DEBUG %>
		<script type="text/javascript"><!--
			google_ad_client = "ca-pub-8738340659342677";
			/* mashup-directory */
			google_ad_slot = "7165679501";
			google_ad_width = 160;
			google_ad_height = 600;
			//-->
			</script>
			<script type="text/javascript" src="http://pagead2.googlesyndication.com/pagead/show_ads.js"></script>
		<% #else %>
		<div class="ad-debug"></div>
		<% #endif %>
	</div>

	<div id="directory-author" class="content800">
		<div id="directory-author-comics">
			<%= Html.Partial("~/Views/Comic/ComicList.ascx", Model.Comics) %>
		</div>
	</div>

</asp:Content>