<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Fredin.Comic.Web.Models.ClientComic>>" %>

<% if (Model.Count() > 0) { %>
	<ul class="comicList">
	<% foreach (var comic in Model) { %>
		<li class="box">
			<div class="comicList-detail">
				<span class="comicList-rate">
					<%--<% if (!String.IsNullOrWhiteSpace(comic.Stats.TopRating)) { %>
					<span title="<%: comic.Stats.TopRating %>" class="ui-icon-toggle16 ui-icon-toggle16-<%: comic.Stats.TopRating.ToLower() %>"></span>
					<% } %>--%>
				</span>
				<a class="comicList-title" href="<%: comic.ReadUrl %>"><%: comic.Title %></a><br />
				<span class="comicList-about">
					<a href="<%: comic.Author.ProfileUrl %>">by <%: comic.Author.Nickname %></a>
					on <%: comic.PublishTime.Value.ToString("dddd MMMMM d, yyyy") %>
				</span>
			</div>
			<a href="<%: comic.ReadUrl %>"><img src="<%: comic.ThumbUrl %>" alt="<%: HttpUtility.HtmlEncode(comic.Description) %>" /></a>
		</li>
	<% } %>
	</ul>
<% } else { %>
	<div class="box ui-state-highlight">No comics found matching your criteria</div>
<% } %>
