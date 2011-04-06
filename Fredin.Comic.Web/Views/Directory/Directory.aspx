﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Web.Master" Inherits="System.Web.Mvc.ViewPage<Fredin.Comic.Web.Models.ViewDirectory>" %>
<%@ Import Namespace="Fredin.Util" %>
<%@ Import Namespace="Fredin.Comic.Data" %>
<%@ Import Namespace="Fredin.Comic.Web.Models" %>

<asp:Content ID="cPageTitle" ContentPlaceHolderID="cphPageTitle" runat="server"><%: this.Model.Mode.GetDescription()%> | Comic Mashup</asp:Content>
<asp:Content ID="cTitle" ContentPlaceHolderID="cphTitle" runat="server"><%: this.Model.Mode.GetDescription()%></asp:Content>
<asp:Content ID="cCanvas" ContentPlaceHolderID="cphCanvas" runat="server">

	<%= Html.Partial("~/Views/Shared/AdSkyscraper.ascx") %>

	<div id="directory" class="content800">
		
		<%-- Filters - Hide while in search mode --%>
		<% if(this.Model.Mode != ViewDirectory.DirectoryMode.Search) { %>
			<div id="directoryFilter" class="menu">
				<ul>
					<li>
						<a id="modeButton" href="javascript:void(0);"><%: this.Model.Mode.GetDescription() %></a>
						<div class="ui-helper-hidden menu-misc">
							<ul>
								<li><a href="<%= this.Url.Action("BestOverall", "Directory", new { period = this.Model.Period, page = this.Model.Page }) %>">Best Overall</a></li>
								<li><a href="<%= this.Url.Action("Funniest", "Directory", new { period = this.Model.Period, page = this.Model.Page }) %>">Funniest</a></li>
								<li><a href="<%= this.Url.Action("Smartest", "Directory", new { period = this.Model.Period, page = this.Model.Page }) %>">Smartest</a></li>
								<li><a href="<%= this.Url.Action("MostRandom", "Directory", new { period = this.Model.Period, page = this.Model.Page }) %>">Most Random</a></li>
							</ul>
						</div>
					</li>
					<li>
						<a id="periodButton" href="javascript:void(0);"><%: this.Model.Period.GetDescription() %></a>
						<div class="ui-helper-hidden menu-misc">
							<ul>
								<li><a href="<%= this.Url.Action(this.Model.Mode.ToString(), "Directory", new { period = ComicStat.ComicStatPeriod.AllTime, page = this.Model.Page }) %>">All Time</a></li>
								<li><a href="<%= this.Url.Action(this.Model.Mode.ToString(), "Directory", new { period = ComicStat.ComicStatPeriod.Year, page = this.Model.Page }) %>">This Year</a></li>
								<li><a href="<%= this.Url.Action(this.Model.Mode.ToString(), "Directory", new { period = ComicStat.ComicStatPeriod.Month, page = this.Model.Page }) %>">This Month</a></li>
								<li><a href="<%= this.Url.Action(this.Model.Mode.ToString(), "Directory", new { period = ComicStat.ComicStatPeriod.Week, page = this.Model.Page }) %>">This Week</a></li>
								<li><a href="<%= this.Url.Action(this.Model.Mode.ToString(), "Directory", new { period = ComicStat.ComicStatPeriod.Day, page = this.Model.Page }) %>">Today</a></li>
							</ul>
						</div>
					</li>
				</ul>
			</div>
		<% } %>

		<%= Html.Partial("~/Views/Comic/ComicList.ascx", this.Model.Comics) %>
		
		<%-- Paging --%>
		<%= Html.Partial("~/Views/Directory/Paging.ascx", this.Model) %>

	</div>

</asp:Content>

<asp:Content ID="cScript" ContentPlaceHolderID="cphScript" runat="server">
	<script type="text/javascript">
	Directory.Directory(appOptions);
	</script>
</asp:Content>