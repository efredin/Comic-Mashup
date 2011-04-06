<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Web.Master" Inherits="System.Web.Mvc.ViewPage<Fredin.Comic.Web.Models.ViewDirectory>" %>
<%@ Import Namespace="Fredin.Util" %>

<asp:Content ID="cPageTitle" ContentPlaceHolderID="cphPageTitle" runat="server">Comic Mashup</asp:Content>
<asp:Content ID="cTitle" ContentPlaceHolderID="cphTitle" runat="server">Welcome</asp:Content>
<asp:Content ID="cCanvas" ContentPlaceHolderID="cphCanvas" runat="server">

	<%= Html.Partial("~/Views/Shared/AdSkyscraper.ascx") %>

	<div id="home" class="box content764">
		<p>Create, read and share web comics with your online profile.  It's easy and fun.</p>
		<a href="<%= this.Url.Action("CreateWizard", "Comic") %>" class="actionButton ui-state-default" id="home-buttonCreate"><span class="icon96 icon96-create"></span><br />Create</a>
		<a href="<%= this.Url.Action("BestOverall", "Directory") %>" class="actionButton ui-state-default" id="home-buttonDirectory"><span class="icon96 icon96-read"></span><br />Read</a>
		<a class="actionButton ui-state-default addthis_button" id="home-buttonShare" href="http://www.addthis.com/bookmark.php"><span class="icon96 icon96-share"></span><br />Share</a>
		<script type="text/javascript">var addthis_config = { "data_track_clickback": true };</script>
		<script type="text/javascript" src="http://s7.addthis.com/js/250/addthis_widget.js#pubid=ra-4d9a88e114b59f84"></script>
	</div>

	<div id="home-directory" class="content800">
		<h2>Latest Comics</h2>
		<%= Html.Partial("~/Views/Comic/ComicList.ascx", Model.Comics) %>
	</div>

</asp:Content>