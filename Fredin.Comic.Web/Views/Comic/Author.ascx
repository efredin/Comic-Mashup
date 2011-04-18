<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Fredin.Comic.Web.Models.ClientComic>" %>
<%@ Import Namespace="Fredin.Util" %>
<div class="author">
	<div class="author-photo"><img src="<%: Model.Author.ThumbUrl %>" alt="" /></div>
	<div class="author-by">by <a class="authorName" href="<%: Model.Author.AuthorUrl %>"><%: Model.Author.Nickname %></a></div>
	<div class="author-date">on <%: Model.PublishTime.Value.UtcToEdmonton().ToString("dddd MMMMM d, yyyy") %></div>
</div>