<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Web.Master" Inherits="Fredin.Comic.Web.ComicViewPage<Fredin.Comic.Web.Models.ViewRead>" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>

<asp:Content ID="cPageTitle" ContentPlaceHolderID="cphPageTitle" runat="server"><%: Model.Comic.Title %> | Comic Mashup</asp:Content>
<asp:Content ID="cTitle" ContentPlaceHolderID="cphTitle" runat="server"><%: Model.Comic.Title %></asp:Content>

<asp:Content ID="cMeta" ContentPlaceHolderID="cphMeta" runat="server">
	<meta property="og:title" content="<%: Model.Comic.Title %>" />
	<meta property="og:description" content="<%: Model.Comic.Description %>" />
	<meta property="og:type" content="article" />
	<meta property="og:image" content="<%: Model.Comic.FrameThumbUrl %>" />
	<meta property="og:url" content="<%: Model.Comic.ReadUrl %>" />
	<meta property="description" content="<%: Model.Comic.Description %>" />
	<meta property="author" content="<%: Model.Comic.Author.Nickname %>" />
</asp:Content>

<asp:Content ID="cCanvas" ContentPlaceHolderID="cphCanvas" runat="server">
	
	<div id="comicRead">
		<div class="box content734">
			
			<div id="comicInfo">
				<%: Model.Comic.Stats.Readers %> readers | 
				<a href="javascript:$.scrollTo('#comments', 200);"><span id="commentCount">0</span> comments</a>

				<%-- Tags --%>
				<div id="tags">
					<% if(this.Model.Tags.Count > 0) { %>
						<% foreach(var t in this.Model.Tags) { %>
							<img title="<%= t.Nickname %>" width="25" height="25" src="<%= String.Format("http://graph.facebook.com/{0}/picture?type=square", t.Uid) %>" />
						<% } %>
					<% } %>
				</div>
			</div>

			<%= Html.Partial("~/Views/Comic/Author.ascx", Model.Comic) %>
			<div id="comicDescription"><%: Model.Comic.Description.StripHtml() %></div>
			<img id="comic" src="<%: Model.Comic.ComicUrl %>" alt="" />

			<div id="reader">
				<div id="readerAction">
					<!-- AddThis Button BEGIN -->
					<div class="addthis_toolbox addthis_default_style addthis_32x32_style">
					<a class="addthis_button_preferred_1"></a>
					<a class="addthis_button_preferred_2"></a>
					<a class="addthis_button_preferred_3"></a>
					<a class="addthis_button_preferred_4"></a>
					<a class="addthis_button_compact"></a>
					</div>
					<script type="text/javascript">	var addthis_config = { "data_track_clickback": true };</script>
					<script type="text/javascript" src="http://s7.addthis.com/js/250/addthis_widget.js#pubid=ra-4d9a88e114b59f84"></script>
					<!-- AddThis Button END -->
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

		<div id="column1">
			<div id="comments" class="box">
				<h2>Comments</h2>
				<fb:comments numposts="10" width="400" href="<%: Model.Comic.ReadUrl %>"></fb:comments>
			</div>
		</div>

		<div id="column2">
			<div id="ad-rectangle" class="box">
				<% if ( this.SessionManager.FbCanvas) { %>
					<iframe width='300' height='250' frameborder='no' framespacing='0' scrolling='no'  src='http://ads.lfstmedia.com/fbslot/slot19152?ad_size=300x250&adkey=7a7'></iframe>
				<% } else { %>
					<iframe width='300' height='250' frameborder='no' framespacing='0' scrolling='no'  src='http://ads.lfstmedia.com/slot/slot21429?ad_size=300x250&adkey=d14'></iframe>
				<% } %>
			</div>

			<div id="navigate" class="box">
				<h2>See Also</h2>
				<ul>
					<% if (Model.Comic.CreateTime.Date > new DateTime(2011, 5, 30)) { %>
					<li><a href="<%: Model.Comic.RemixUrl %>">Remix this Comic</a></li>
					<% } %>
					<li><a href="<%: Model.Comic.Author.AuthorUrl %>">More Comics by <%: Model.Comic.Author.Nickname %></a></li>
					<li><a href="<%: this.Url.Action("Random", "Comic") %>">Random Comic</a></li>
				</ul>
			</div>

			<%-- Author Edit Control --%>
			<% if(this.Model.Comic.Uid == this.Model.Reader.Uid) { %>

			<div id="edit" class="box">
				<ul>
					<li>
						<a id="authorDelete" href="javascript:void(0);">Delete Comic</a>
						<div id="dialog-authorDelete" class="ui-helper-hidden" title="Please Confirm">
							<div id="dialog-authorDeleteMessage">Are you sure you want to delete this comic?</div>
						</div>
					</li>
				</ul>
			</div>

			<% } %>

		</div>

	</div>

</asp:Content>

<asp:Content ID="cScript" ContentPlaceHolderID="cphScript" runat="server">
	<script type="text/javascript">
	appOptions.comic = <%= new JavaScriptSerializer().Serialize(this.Model.Comic) %>;
	Comic.Read(appOptions);
	</script>
</asp:Content>

