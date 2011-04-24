<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Web.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="cPageTitle" ContentPlaceHolderID="cphPageTitle" runat="server">Comic Not Found | Comic Mashup</asp:Content>
<asp:Content ID="cTitle" ContentPlaceHolderID="cphTitle" runat="server">Comic Not Found</asp:Content>

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

    <p>The comic you requested could not be found, or you are not authorized to view it.</p>
</asp:Content>