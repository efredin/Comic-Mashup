<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Web.Master" Inherits="Fredin.Comic.Web.ComicViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cphPageTitle" runat="server">Contact | Comic Mashup</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cphTitle" runat="server">Contact</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="cphCanvas" runat="server">

	<div class="wideskyscraper">
		<% #if !DEBUG %>
		<iframe width='160' height='600' frameborder='no' framespacing='0' scrolling='no'  src='http://ads.lfstmedia.com/slot/slot19167?ad_size=160x600&adkey=317'></iframe>
		<% #else %>
		<div class="ad-debug"></div>
		<% #endif %>
	</div>

    <div class="box content764">
		<p>Your message has been received. Thank you.</p>
	</div>

</asp:Content>


