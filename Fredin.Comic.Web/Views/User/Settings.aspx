<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Web.Master" Inherits="System.Web.Mvc.ViewPage<Fredin.Comic.Web.Models.ViewSettings>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cphPageTitle" runat="server">Settings | Comic Mashup</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cphTitle" runat="server">Settings</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="cphCanvas" runat="server">

	<div class="box content764">
		
		<% using(this.Html.BeginForm()) { %>

		<h2>Notifications</h2>
		Notifications are sent via email to your facebook email address. If you wish to change your address, please do so <a href="https://www.facebook.com/editaccount.php" target="_blank">here</a>.
		
		<p>Your current email address is <span class="setting-email"><%: this.Model.Email %></span></p>

		<div>
			Send me a notification when someone:
			<div><%: this.Html.CheckBoxFor(s => s.Engage.Comment) %><%: this.Html.LabelFor(s => s.Engage.Comment, "Comments on my comic") %></div>
			<div><%: this.Html.CheckBoxFor(s => s.Engage.Tag) %><%: this.Html.LabelFor(s => s.Engage.Comment, "I am tagged in a comic") %></div>
		</div>

		<button type="submit">Save Changes</button>

		<% } %>

	</div>

</asp:Content>
