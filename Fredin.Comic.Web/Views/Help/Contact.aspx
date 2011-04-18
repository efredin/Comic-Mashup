<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Web.Master" Inherits="Fredin.Comic.Web.ComicViewPage<Fredin.Comic.Web.Models.ViewContact>" %>


<asp:Content ID="cPageTitle" ContentPlaceHolderID="cphPageTitle" runat="server">Contact | Comic Mashup</asp:Content>
<asp:Content ID="cTitle" ContentPlaceHolderID="cphTitle" runat="server">Contact</asp:Content>
<asp:Content ID="cCanvas" ContentPlaceHolderID="cphCanvas" runat="server">
	
	<div class="wideskyscraper">
		<% #if !DEBUG %>
		<iframe width='160' height='600' frameborder='no' framespacing='0' scrolling='no'  src='http://ads.lfstmedia.com/slot/slot19167?ad_size=160x600&adkey=317'></iframe>
		<% #else %>
		<div class="ad-debug"></div>
		<% #endif %>
	</div>

	<div class="box content764">

		<p>Please complete the following form to contact us.</p>

		<div id="contact">
			<% using (Html.BeginForm()) { %>
			<div class="ui-optionset">
				<div class="ui-optionset-title">Name</div>
				<div class="ui-optionset-content"><%= this.Html.TextBoxFor(f => f.Nickname) %></div>
			</div>
			<div class="ui-optionset">
				<div class="ui-optionset-title">Email</div>
				<div class="ui-optionset-content"><%= this.Html.TextBoxFor(f => f.Email) %></div>
			</div>
			<div class="ui-optionset">
				<div class="ui-optionset-title">Message</div>
				<div class="ui-optionset-content"><%= this.Html.TextAreaFor(f => f.Message, new { cols = 4 })%></div>
			</div>
			<div class="ui-optionset">
				<div class="ui-optionset-content"><button id="contactButton" type="submit" name="contactButton">Send Message</button></div>
			</div>
			<% } %>
		</div>

	</div>
</asp:Content>

<asp:Content ID="cScript" ContentPlaceHolderID="cphScript" runat="server">
	<script type="text/javascript">
	Help.Contact(appOptions);
	</script>
</asp:Content>