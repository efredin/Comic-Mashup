<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Web.Master" Inherits="System.Web.Mvc.ViewPage<Fredin.Comic.Web.Models.ViewRead>" %>
<%@ Import Namespace="Fredin.Comic.Web" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>

<asp:Content ID="cPageTitle" ContentPlaceHolderID="cphPageTitle" runat="server"><%: Model.Comic.Title %> | Comic Mashup</asp:Content>
<asp:Content ID="cTitle" ContentPlaceHolderID="cphTitle" runat="server"><%: Model.Comic.Title %></asp:Content>

<asp:Content ID="cHead" ContentPlaceHolderID="cphHead" runat="server">
	<meta property="og:title" content="<%: Model.Comic.Title %>" />
	<meta property="og:description" content="<%: Model.Comic.Description %>" />
	<meta property="og:type" content="article" />
	<meta property="og:image" content="<%: Model.Comic.FrameThumbUrl %>" />
	<meta property="og:url" content="<%: Model.Comic.ReadUrl %>" />
</asp:Content>

<asp:Content ID="cCanvas" ContentPlaceHolderID="cphCanvas" runat="server">

	<%= Html.Partial("~/Views/Shared/AdSkyscraper.ascx") %>

	<div id="comicRead">
		<div class="box">
			
			<span id="comicInfo">
				<%: Model.Comic.Stats.Readers %> reads | 
				<a href="javascript:$.scrollTo('#comments', 200);"><fb:comments-count href="<%: Model.Comic.ReadUrl %>"></fb:comments-count> comments</a>
			</span>

			<%= Html.Partial("~/Views/Comic/Author.ascx", Model.Comic) %>
			<div id="comicDescription"><%: HttpUtility.HtmlEncode(Model.Comic.Description) %></div>
			<img id="comic" src="<%: Model.Comic.ComicUrl %>" alt="" />

			<div id="reader">
				<div id="readerAction">
					<a id="readerFb" title="Post to Wall"><span class="ui-icon-toggle32 ui-icon-toggle32-fb"></span></a>
				</div>
				<div id="readerRate">
					<%: Html.CheckBox("readerFunny", Model.Reader.IsFunny) %>
					<label for="readerFunny" title="Funny"><span class="ui-icon-toggle32 ui-icon-toggle32-funny"></span><br /><span class="readerCount"><%: Model.Comic.Stats.Funny > 0 ? Model.Comic.Stats.Funny.ToString() : "-" %></span></label>
					<%: Html.CheckBox("readerSmart", Model.Reader.IsSmart) %>
					<label for="readerSmart" title="Smart"><span class="ui-icon-toggle32 ui-icon-toggle32-smart"></span><br /><span class="readerCount"><%: Model.Comic.Stats.Smart > 0 ? Model.Comic.Stats.Smart.ToString() : "-" %></span></label>
					<%: Html.CheckBox("readerRandom", Model.Reader.IsRandom) %>
					<label for="readerRandom" title="Random"><span class="ui-icon-toggle32 ui-icon-toggle32-random"></span><br /><span class="readerCount"><%: Model.Comic.Stats.Random > 0 ? Model.Comic.Stats.Random.ToString() : "-" %></span></label>
					<span id="readerVote">Vote for this comic<br /><br /></span>
				</div>
			</div>
		</div>

		<div id="comments" class="box">
			<h2>Comments</h2>
			<fb:comments numposts="10" width="522" href="<%: Model.Comic.ReadUrl %>"></fb:comments>
		</div>

		<div id="navigate" class="box">
			<h2>See Also</h2>
			<ul>
				<li><a href="<%: Model.Comic.Author.ProfileUrl %>">More Comics by <%: Model.Comic.Author.Nickname %></a></li>
				<li><a href="<%: this.Url.Action("Random", "Comic") %>">Random Comic</a></li>
			</ul>
		</div>

	</div>

</asp:Content>

<asp:Content ID="cScript" ContentPlaceHolderID="cphScript" runat="server">
	<script type="text/javascript">
	appOptions.comic = <%= new JavaScriptSerializer().Serialize(this.Model.Comic) %>;
	appOptions.menuSelected = 0;
	Comic.Read(appOptions);
	</script>
</asp:Content>

