<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<div class="wideskyscraper">
	<% #if !DEBUG %>
	<script type="text/javascript">
		google_ad_client = "ca-pub-8738340659342677";
		google_ad_slot = "7418994237";
		google_ad_width = 160;
		google_ad_height = 600;
	</script>
	<script type="text/javascript" src="http://pagead2.googlesyndication.com/pagead/show_ads.js"></script>
	<% #else %>
	<div class="ad-debug"></div>
	<% #endif %>
</div>