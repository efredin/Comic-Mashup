<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Web.Master" Inherits="Fredin.Comic.Web.ComicViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cphPageTitle" runat="server">Contact | Comic Mashup</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cphTitle" runat="server">Contact</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="cphCanvas" runat="server">

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

    <div class="box content764">
		<p>Your message has been received. Thank you.</p>
	</div>

</asp:Content>


