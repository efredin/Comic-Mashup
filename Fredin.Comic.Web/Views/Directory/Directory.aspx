<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Web.Master" Inherits="Fredin.Comic.Web.ComicViewPage<Fredin.Comic.Web.Models.ViewDirectory>" %>
<%@ Import Namespace="Fredin.Util" %>
<%@ Import Namespace="Fredin.Comic.Data" %>
<%@ Import Namespace="Fredin.Comic.Web.Models" %>
<%@ Import Namespace="System.Globalization" %>

<asp:Content ID="cPageTitle" ContentPlaceHolderID="cphPageTitle" runat="server"><%: this.Model.Mode.GetDescription()%> | Comic Mashup</asp:Content>
<asp:Content ID="cTitle" ContentPlaceHolderID="cphTitle" runat="server"><%: this.Model.Mode.GetDescription()%></asp:Content>
<asp:Content ID="cCanvas" ContentPlaceHolderID="cphCanvas" runat="server">

	<div class="wideskyscraper">
		<% #if !DEBUG %>
		<iframe width='160' height='600' frameborder='no' framespacing='0' scrolling='no'  src='http://ads.lfstmedia.com/slot/slot19165?ad_size=160x600&adkey=754'></iframe>
		<% #else %>
		<div class="ad-debug"></div>
		<% #endif %>
	</div>

	<div id="directory" class="content800">
		
		<%-- Filters - Hide while in search mode --%>
		<% if(this.Model.Mode != ViewDirectory.DirectoryMode.Search) { %>
			<div id="directoryFilter" class="menu">
				<ul>
					<li>
						<a id="modeButton" href="javascript:void(0);"><%= this.Model.Mode.GetDescription() %></a>
						<div class="ui-helper-hidden menu-misc">
							<ul>
								<li><a href="<%= this.Url.Action("BestOverall", "Directory", new { period = this.Model.Period, language = this.Model.Culture.TwoLetterISOLanguageName, page = this.Model.Page }) %>">Best Overall</a></li>
								<li><a href="<%= this.Url.Action("Funniest", "Directory", new { period = this.Model.Period, language = this.Model.Culture.TwoLetterISOLanguageName, page = this.Model.Page }) %>">Funniest</a></li>
								<li><a href="<%= this.Url.Action("Smartest", "Directory", new { period = this.Model.Period, language = this.Model.Culture.TwoLetterISOLanguageName, page = this.Model.Page }) %>">Smartest</a></li>
								<li><a href="<%= this.Url.Action("MostRandom", "Directory", new { period = this.Model.Period, language = this.Model.Culture.TwoLetterISOLanguageName, page = this.Model.Page }) %>">Most Random</a></li>
							</ul>
						</div>
					</li>
					<li>
						<a id="periodButton" href="javascript:void(0);"><%= this.Model.Period.GetDescription() %></a>
						<div class="ui-helper-hidden menu-misc">
							<ul>
								<li><a href="<%= this.Url.Action(this.Model.Mode.ToString(), "Directory", new { period = ComicStat.ComicStatPeriod.AllTime, language = this.Model.Culture.TwoLetterISOLanguageName, page = this.Model.Page }) %>">All Time</a></li>
								<li><a href="<%= this.Url.Action(this.Model.Mode.ToString(), "Directory", new { period = ComicStat.ComicStatPeriod.Year, language = this.Model.Culture.TwoLetterISOLanguageName, page = this.Model.Page }) %>">This Year</a></li>
								<li><a href="<%= this.Url.Action(this.Model.Mode.ToString(), "Directory", new { period = ComicStat.ComicStatPeriod.Month, language = this.Model.Culture.TwoLetterISOLanguageName, page = this.Model.Page }) %>">This Month</a></li>
								<li><a href="<%= this.Url.Action(this.Model.Mode.ToString(), "Directory", new { period = ComicStat.ComicStatPeriod.Week, language = this.Model.Culture.TwoLetterISOLanguageName, page = this.Model.Page }) %>">This Week</a></li>
								<li><a href="<%= this.Url.Action(this.Model.Mode.ToString(), "Directory", new { period = ComicStat.ComicStatPeriod.Day, language = this.Model.Culture.TwoLetterISOLanguageName, page = this.Model.Page }) %>">Today</a></li>
							</ul>
						</div>
					</li>
					<li>
						<a id="languageButton" href="javascript:void(0);"><%= this.Model.Culture.ShortDisplayName() %></a>
						<div class="ui-helper-hidden menu-misc">
							<ul>
								<li><a href="<%= this.Url.Action(this.Model.Mode.ToString(), "Directory", new { period = this.Model.Period, language = "en", page = this.Model.Page }) %>">English</a></li>
								<li><a href="<%= this.Url.Action(this.Model.Mode.ToString(), "Directory", new { period = this.Model.Period, language = "fr", page = this.Model.Page }) %>">French</a></li>
								<li><a href="<%= this.Url.Action(this.Model.Mode.ToString(), "Directory", new { period = this.Model.Period, language = "es", page = this.Model.Page }) %>">Spanish</a></li>
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