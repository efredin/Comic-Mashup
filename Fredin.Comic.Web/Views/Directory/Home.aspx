<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Web.Master" Inherits="Fredin.Comic.Web.ComicViewPage<Fredin.Comic.Web.Models.ViewDirectory>" %>
<%@ Import Namespace="Fredin.Util" %>
<%@ Import Namespace="Fredin.Comic.Config" %>
<%@ Import Namespace="Fredin.Comic.Web" %>

<asp:Content ID="cTitle" ContentPlaceHolderID="cphTitle" runat="server">Welcome</asp:Content>
<asp:Content ID="cCanvas" ContentPlaceHolderID="cphCanvas" runat="server">

	<div id="home" class="box content734">

		<!--<div id="home-like"><fb:like href="http://www.facebook.com/apps/application.php?id=<%= ComicConfigSectionGroup.Facebook.AppId %>" layout="box_count" show_faces="false" width="55" font=""></div>-->

		<p>Create, read and share comics with your friends online.  It's fun, easy, and best of all, you don't need to draw!</p>
		<h2>How it Works</h2>
		<p>
			Comic Mashup uses your photos and status updates to generate web comics.  Just <a href="javascript:void(0);" class="button-fbLogin">connect</a> your account to get started.
			You have complete control over the appearance of your comic.  You can even add cool effects to your photots.
		</p>
		<a href="<%= this.Url.Action("CreateWizard", "Comic") %>" class="actionButton ui-state-default" id="home-buttonCreate"><span class="icon96 icon96-create"></span><br />Create</a>
		<a href="<%= this.Url.Action("BestOverall", "Directory") %>" class="actionButton ui-state-default" id="home-buttonDirectory"><span class="icon96 icon96-read"></span><br />Read</a>
		<a class="actionButton ui-state-default addthis_button" id="home-buttonShare" href="http://www.addthis.com/bookmark.php"><span class="icon96 icon96-share"></span><br />Share</a>
		<script type="text/javascript">var addthis_config = { "data_track_clickback": true };</script>
		<script type="text/javascript" src="http://s7.addthis.com/js/250/addthis_widget.js#pubid=ra-4d9a88e114b59f84"></script>
	</div>

	<div id="home-directory" class="content760">
		<h2>Latest Comics</h2>
		<%= Html.Partial("~/Views/Comic/ComicList.ascx", Model.Comics) %>
	</div>

</asp:Content>