<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Web.Master" Inherits="System.Web.Mvc.ViewPage<Fredin.Comic.Web.Models.ViewCreateWizard>" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Fredin.Comic.Web" %>
<%@ Import Namespace="Fredin.Comic.Config" %>

<asp:Content ID="cPageTitle" ContentPlaceHolderID="cphPageTitle" runat="server">Create a Comic | Comic Mashup</asp:Content>
<asp:Content ID="cTitle" ContentPlaceHolderID="cphTitle" runat="server">Create a Comic</asp:Content>

<asp:Content ID="cCanvas" ContentPlaceHolderID="cphCanvas" runat="server">

    <% using (Html.BeginForm()) { %>

	<%= Html.Partial("~/Views/Shared/AdSkyscraper.ascx") %>

	<div id="wizardCreate" class="box content764">
		<div id="wizardLoad"></div>
		<div id="wizardContent" class="ui-wizard">
			<div id="wizardTitle" class="ui-wizard-title">
				<a>Story<span class="caption">Step 1</span></a>
				<a>Comments<span class="caption">Step 2</span></a>
				<a>Template<span class="caption">Step 3</span></a>
				<a>Options<span class="caption">Step 4</span></a>
				<a>Publish<span class="caption">Step 5</span></a>
			</div>
			<div id="stepStory" class="ui-wizard-step">
				<a class='ui-wizard-next'>Next</a>
				<h2>Select a story for your comic</h2>
				Your selection will be used as the dialog for the first frame of your comic.
				<div class="ui-widget ui-widget-content ui-corner-all ui-wizard-navPad">
					<div id="storyFilter" class="selectFilter">
						<input type="radio" value=".feed-suggested" name="storyFilter" id="storyFilterSuggest" /><label for="storyFilterSuggest">Suggested</label>
						<input type="radio" value=".feed" name="storyFilter" id="storyFilterAll" /><label for="storyFilterAll">All</label>
					</div>
					<select id="storySelector" name="storySelector"></select>
				</div>
			</div>
			<div id="stepComment" class="ui-wizard-step">
				<a class='ui-wizard-next'>Next</a>
				<a class='ui-wizard-back'>Back</a>
				<h2>Select comments for your comic.</h2>
				Comments are used for additional frames in your comic, and are added in the same order you make your selection.
				<div id="commentLoad"></div>
				<div id="commentContent" class="ui-widget ui-widget-content ui-corner-all ui-wizard-navPad">
					<select id="commentSelector" name="commentSelector"></select>
				</div>
			</div>
			<div id="stepTemplate" class="ui-wizard-step">
				<a class='ui-wizard-next'>Next</a>
				<a class='ui-wizard-back'>Back</a>
				<h2>Select a template for your comic</h2>
				Based on the number of comments you've selected, these are the available comic templates available.
				<div class="ui-widget ui-widget-content ui-corner-all ui-wizard-navPad">
					<div id="templateFilter" class="selectFilter">
						<input type="radio" value=".template-size1" name="templateFilter" id="templateFilterSuggest" /><label for="templateFilterSuggest">Suggested</label>
						<input type="radio" value=".template" name="templateFilter" id="templateFilterAll" /><label for="templateFilterAll">All</label>
					</div>
					<select id="templateSelector" name="templateSelector"></select>
				</div>
			</div>
			<div id="stepOption" class="ui-wizard-step">
				<a class='ui-wizard-next'>Next</a>
				<a class='ui-wizard-back'>Back</a>
				<h2>Select the options for your comic</h2>
				Your selection will control the final appearance of yoru comic.
				<div class="ui-wizard-navPad">
					<div id="optionsetPhoto" class="ui-optionset">
						<div class="ui-optionset-title">Photo Source</div>
						<span class="ui-optionset-caption">When tagged is selected and no tags are found, another photo will be used instead.</span>
						<div id="buttonsetPhotoSource" class="ui-optionset-content">
							<input type="radio" value="Tagged" name="optionPhoto" id="optionPhotoTagged" /><label for="optionPhotoTagged">Tagged</label>
							<%--<input type="radio" value="Profile" name="optionPhoto" id="optionPhotoProfile" /><label for="optionPhotoProfile">Profile</label>--%>
							<input type="radio" value="Any" name="optionPhoto" id="optionPhotoAny" checked="checked" /><label for="optionPhotoAny">Any</label>
						</div>
					</div>
					<div id="optionsetEffect" class="ui-optionset">
						<div class="ui-optionset-title">Effect</div>
						<div class="ui-optionset-content">
							<select id="effectSelector" name="effectSelector"></select>
						</div>
					</div>
				</div>
			</div>
			<div id="stepRender" class="ui-wizard-step ui-wizard-navPad">
				<div id="renderLoad">
					Your comic is being created - This may take a moment . . .
				</div>
				<div id="renderContent">
					<a class='ui-wizard-back'>Back</a>
					<h2>Your comic has been created</h2>
					Publish your comic to share it with your friends, or refresh to make another.<br /><br />
					<a id="buttonPublish">Publish</a>
					<a id="buttonRefresh">Refresh</a>
					<div id="renderComic"></div>
				</div>
			</div>
		</div>
		<div id="publishComic" title="Comic Publisher" class="ui-helper-hidden">
			<div id="publishImage"></div>
			<div id="publishForm">
				<div id="optionSetTitle" class="ui-optionset">
					<div class="ui-optionset-title">Title</div>
					<div class="ui-optionset-content"><input type="text" name="comicTitle" id="comicTitle" value="My Comic" /></div>
				</div>
				<div id="optionSetDescription" class="ui-optionset">
					<div class="ui-optionset-title">Description</div>
					<div class="ui-optionset-content"><textarea id="comicDescription" name="comicDescription" cols="55" rows="2"></textarea></div>
				</div>
				<div id="optionSetPrivacy" class="ui-optionset">
					<div class="ui-optionset-title">Privacy</div>
					<span class="ui-optionset-caption">Controls who can read your comic.</span>
					<div id="buttonsetPrivacy" class="ui-optionset-content">
						<input type="radio" value="Friends" name="optionPrivacy" id="optionPrivacyFriends" /><label for="optionPrivacyFriends">Friends</label>
						<input type="radio" value="All" name="optionPrivacy" id="optionPrivacyAll" checked="checked" /><label for="optionPrivacyAll">Everyone</label>
					</div>
				</div>
			</div>
		</div>
	</div>

	<% } %>

</asp:Content>

<asp:Content ID="cScript" ContentPlaceHolderID="cphScript" runat="server">
	<script type="text/javascript">
	appOptions.templates = <%= new JavaScriptSerializer().Serialize(this.Model.Templates) %>;
	appOptions.effects = <%= new JavaScriptSerializer().Serialize(this.Model.Effects) %>;
	Comic.CreateWizard(appOptions);
	</script>

	<script type="text/x-jqote-template" id="storyTemplate">
	<![CDATA[
	<div class="feed <#= this.css #>">
		<img class="feed-picture" src="http://graph.facebook.com/<#= this.from.id #>/picture/" alt="" />
		<div class="feed-content">
			<span class="feed-from name"><#= this.from.name #></span>
			<span class="feed-message"><#= this.message.wordSubstr(0, 79, '...') #></span><br/>
			<# if(this.icon){ #><img class="feed-icon" alt="" src="<#= this.icon #>" /><# } #>
			<span class="feed-time"><#= Date.parse8601(this.created_time).toContextual() #></span>
			<span class="feed-commentCount"> - <#= this.comments ? this.comments.count : 0 #> comment(s)</span>
		</div>
	</div>
	]]>
	</script>

	<script type="text/x-jqote-template" id="commentTemplate">
	<![CDATA[
	<div class="feed">
		<img class="feed-picture" src="http://graph.facebook.com/<#= this.from.id #>/picture/" alt="" />
		<div class="feed-content">
			<span class="feed-from name"><#= this.from.name #></span>
			<span class="feed-message"><#= this.message.wordSubstr(0, 79, '...') #></span>
		</div>
	</div>
	]]>
	</script>

	<script type="text/x-jqote-template" id="templateTemplate">
	<![CDATA[
	<span class="template template-size<#= this.TemplateItems.length #>">
		<img class="template-picture" src="<#= this.ThumbUrl #>" alt="" />
	</span>
	]]>
	</script>

	<script type="text/x-jqote-template" id="templateEffect">
	<![CDATA[
	<div class="effect">
		<img class="effect-picture" src="<#= this.ThumbUrl #>" alt="" />
		<div class="effect-title"><#= this.Title #></div>
	</div>
	]]>
	</script>
</asp:Content>