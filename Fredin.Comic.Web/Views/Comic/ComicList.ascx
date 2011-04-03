<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Fredin.Comic.Web.Models.ClientComic>>" %>

<div class="comicList">
<% foreach (var comic in Model) { %>
	<div class="comicList-item box">
		<div class="comicList-detail">
			<span class="comicList-rate">
				<% if (!String.IsNullOrWhiteSpace(comic.Stats.TopRating)) { %>
				<span title="<%: comic.Stats.TopRating %>" class="ui-icon-toggle16 ui-icon-toggle16-<%: comic.Stats.TopRating.ToLower() %>"></span>
				<% } %>
			</span>
			<a class="comicList-title" href="<%: comic.ReadUrl %>"><%: comic.Title %></a><br />
			<span class="comicList-about">
				<a href="<%: comic.Author.ProfileUrl %>">by <%: comic.Author.Nickname %></a>
				on <%: comic.PublishTime.Value.ToString("dddd MMMMM d, yyyy") %>
			</span>
		</div>
		<a href="<%: comic.ReadUrl %>"><img src="<%: comic.ThumbUrl %>" alt="<%: HttpUtility.HtmlEncode(comic.Description) %>" /></a>
	</div>
<% } %>
</div>