<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Web.Master" Inherits="System.Web.Mvc.ViewPage<Fredin.Comic.Web.Models.ViewSettings>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cphPageTitle" runat="server">Settings | Comic Mashup</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cphTitle" runat="server">Settings</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="cphCanvas" runat="server">

	<div id="settings" class="box content734">
		
		<% using(this.Html.BeginForm()) { %>

		<% if(!String.IsNullOrWhiteSpace(this.Model.Feedback)) { %>
			<div class="ui-state-highlight"><%= this.Model.Feedback %></div>
		<% } %>

		<h2>Notifications</h2>
		<div id="optionsetEmail" class="ui-optionset">
			<div class="ui-optionset-title">Email</div>
			<span class="ui-optionset-caption">Your current email address.</span>
			<div class="ui-optionset-content"><%: this.Html.TextBoxFor(s => s.Email, new { @class = "email" })%></div>
		</div>

		<div id="optionsetSubscribe" class="ui-optionset">
			<div class="ui-optionset-content">
				<%: this.Html.CheckBoxFor(s => s.Engage.Subscribe)%><%: this.Html.LabelFor(s => s.Engage.Subscribe, "Send me notifications") %>
				<div id="engage-setting">
					<ul>
						<li><%: this.Html.CheckBoxFor(s => s.Engage.ComicCreate) %><%: this.Html.LabelFor(s => s.Engage.ComicCreate, "When I publish a comic")%></li>
						<li><%: this.Html.CheckBoxFor(s => s.Engage.ComicRemix) %><%: this.Html.LabelFor(s => s.Engage.ComicRemix, "When my comic is remixed")%></li>
						<li><%: this.Html.CheckBoxFor(s => s.Engage.Comment) %><%: this.Html.LabelFor(s => s.Engage.Comment, "When my comic receives a comment") %></li>
					</ul>
				</div>
			</div>
		</div>

		<button type="submit">Save Changes</button>

		<% } %>

	</div>

</asp:Content>
<asp:Content ID="cScript" ContentPlaceHolderID="cphScript" runat="server">
	<script type="text/javascript">
		User.Settings(appOptions);
	</script>
</asp:Content>