<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Fredin.Comic.Web.Models.ViewDirectory>" %>
<%@ Import Namespace="Fredin.Util" %>
<%@ Import Namespace="Fredin.Comic.Data" %>
<%@ Import Namespace="Fredin.Comic.Web.Models" %>

<div id="directoryPaging">
	<% if(this.Model.Page > 1) { %>
		<a id="pagerButtonBack" class="pagerButton" href="<%= this.Url.Action(this.Model.Mode.ToString(), "Directory", new { period = this.Model.Period, page = this.Model.Page - 1 }) %>">Previous</a>
	<% } %>
	<% for(int p = this.Model.Page - 2; p <= this.Model.Page + 2; p++) { %>
		<% if(p >= 1 && p <= this.Model.MaxPage) { %>
			<a class="pagerButtonNum <%= this.Model.Page == p ? "ui-state-active" : String.Empty %>" 
				href="<%= this.Url.Action(this.Model.Mode.ToString(), "Directory", new { period = this.Model.Period, page = p }) %>"><%= p %></a>
		<% } %>
	<% } %>
	<% if(this.Model.Page < this.Model.MaxPage) { %>
		<a id="pagerButtonNext" class="pagerButton" href="<%= this.Url.Action(this.Model.Mode.ToString(), "Directory", new { period = this.Model.Period, page = this.Model.Page + 1 }) %>">Next</a>
	<% } %>
</div>