<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<ul>
	<li>
		<a href="<%= this.Url.Action("BestOverall", "Directory") %>">Browse</a>
		<div class="ui-helper-hidden menu-directory">
			<ul>
				<li><a href="<%= this.Url.Action("BestOverall", "Directory") %>">Best Overall</a></li>
				<li><a href="<%= this.Url.Action("Funniest", "Directory") %>">Funniest</a></li>
				<li><a href="<%= this.Url.Action("Smartest", "Directory") %>">Smartest</a></li>
				<li><a href="<%= this.Url.Action("MostRandom", "Directory") %>">Most Random</a></li>
				<li><a href="<%= this.Url.Action("Newest", "Directory") %>">Newest</a></li>
			</ul>
		</div>
	</li>
	<li>
		<a href="<%= this.Url.Content("~/Directory/Author") %>">My Comics</a>
	</li>
	<li>
		<a href="<%= this.Url.Action("CreateWizard", "Comic") %>">Create</a>
		<div class="ui-helper-hidden menu-directory">
			<ul>
				<li><a href="<%= this.Url.Action("CreateWizard", "Comic") %>">Mashup Wizard</a></li>
				<li><a href="<%= this.Url.Action("Create", "Comic") %>">Advanced Mashup</a></li>
				<li><a href="<%= this.Url.Action("Index", "Facebook") %>">Profile Photo</a></li>
			</ul>
		</div>
	</li>
<%--<li>
		<a href="<%= this.Url.Action("Settings", "User") %>">Settings</a>
	</li>--%>
	<li>
		<a href="<%= this.Url.Action("Faq", "Help") %>">Help</a>
		<div class="ui-helper-hidden menu-help">
			<ul>
				<li><a href="<%= this.Url.Action("Faq", "Help") %>">FAQ</a></li>
				<li><a href="<%= this.Url.Action("Contact", "Help") %>">Contact</a></li>
			</ul>
		</div>
	</li>
</ul>