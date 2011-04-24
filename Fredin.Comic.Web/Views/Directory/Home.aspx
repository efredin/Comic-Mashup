<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Web.Master" Inherits="Fredin.Comic.Web.ComicViewPage<Fredin.Comic.Web.Models.ViewDirectory>" %>
<%@ Import Namespace="Fredin.Util" %>
<%@ Import Namespace="Fredin.Comic.Config" %>
<%@ Import Namespace="Fredin.Comic.Web" %>

<asp:Content ID="cTitle" ContentPlaceHolderID="cphTitle" runat="server">Welcome</asp:Content>
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

	<div id="home" class="box content764">

		<div id="home-like"><fb:like href="http://www.facebook.com/apps/application.php?id=<%= ComicConfigSectionGroup.Facebook.AppId %>" layout="box_count" show_faces="false" width="55" font=""></div>

		<p>Create, read and share web comics with your online profile.  It's fun and easy!</p>
		<h2>How it Works</h2>
		<p>
			Comic Mashup uses your social profile to generate fun web comics. 
			<a href="javascript:void(0);" class="button-fbLogin">Connect your facebook</a> account to get started. 
			Then, share your comics with your family and friends on facebook.
			The more visitors you send to your comic, the more votes it will get.
		</p>
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