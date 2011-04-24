<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Web.Master" Inherits="Fredin.Comic.Web.ComicViewPage<Fredin.Comic.Web.Models.ViewCreate>" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>

<asp:Content ID="cCanvas" ContentPlaceHolderID="cphCanvas" runat="server">

	<% using (Html.BeginForm()) { %>

	<div class="wideskyscraper">
		<% #if !DEBUG %>
		<script type="text/javascript"><!--
			google_ad_client = "ca-pub-8738340659342677";
			/* mashup-directory */
			google_ad_slot = "7165679501";
			google_ad_width = 160;
			google_ad_height = 600;
			//-->
			</script>
			<script type="text/javascript" src="http://pagead2.googlesyndication.com/pagead/show_ads.js"></script>
		<% #else %>
		<div class="ad-debug"></div>
		<% #endif %>
	</div>

	<div id="wizardCreate" class="box content764">
		<div id="wizardContent" class="ui-wizard ui-helper-hidden">
			<div id="wizardTitle" class="ui-wizard-title">
				<a>Template<span class="caption">Step 1</span></a>
				<a>Create<span class="caption">Step 2</span></a>
				<a>Publish<span class="caption">Step 3</span></a>
			</div>
			<div id="stepTemplate" class="ui-wizard-step">
				<a class='ui-wizard-next'>Next</a>
				<a class='ui-wizard-back'>Back</a>
				<h2>Select a template for your comic</h2>
				<div class="ui-widget ui-widget-content ui-corner-all ui-wizard-navPad">
					<select id="templateSelector" name="templateSelector"></select>
				</div>
			</div>
			<div id="stepCreate" class="ui-wizard-step">
				<a class='ui-wizard-next'>Next</a>
				<a class='ui-wizard-back'>Back</a>
				<h2>Create your comic</h2>
				<div class="ui-wizard-navPad">
					<div id="comicEditor" class="ui-comic">
						<a class="ui-comic-bubbleAdd">Add Text Bubble</a>
						<div class="ui-comic-canvas"></div>
					</div>
				</div>
			</div>
			<div id="stepPublish" class="ui-wizard-step">
				<a class='ui-wizard-back'>Back</a>
				<h2>Publish your comic</h2>
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

		<div id="bubbleDialog" class="ui-helper-hidden" title="Text Bubble Selector">
			<select id="bubbleSelector"></select>
		</div>

		<div id="renderDialog" class="ui-helper-hidden" title="Photo Rendering">
			Your photo is rendering. One moment please.
		</div>

		<div id="photoDialog" class="ui-helper-hidden" title="Photo Selector">
			<div id="photoWizard" class="ui-wizard">
				<div class="ui-wizard-title">
					<a>Album<span class="caption">Step 1</span></a>
					<a>Photo<span class="caption">Step 2</span></a>
					<a>Effect<span class="caption">Step 3</span></a>
				</div>
				<div id="stepAlbum" class="ui-wizard-step">
					<h2>Select an album</h2>
					<div id="albumLoad"></div>
					<div id="albumContent" class="ui-widget ui-widget-content ui-corner-all ui-wizard-navPad">
						<select id="albumSelector" name="albumSelector"></select>
					</div>
				</div>
				<div id="stepPhoto" class="ui-wizard-step">
					<a class='ui-wizard-back'>Back</a>
					<h2>Select a photo</h2>
					<div id="photoLoad"></div>
					<div id="photoContent" class="ui-widget ui-widget-content ui-corner-all ui-wizard-navPad">
						<select id="photoSelector" name="photoSelector"></select>
					</div>
				</div>
				<div id="stepEffect" class="ui-wizard-step">
					<a class='ui-wizard-back'>Back</a>
					<h2>Select an effect</h2>
					<div class="ui-wizard-navPad">
						<div id="optionsetEffect" class="ui-optionset">
							<div class="ui-optionset-title">Effect</div>
							<div class="ui-optionset-content">
								<select id="effectSelector" name="effectSelector"></select>
							</div>
						</div>
						<div id="optionsetAmount" class="ui-optionset">
							<div class="ui-optionset-title">Effect Strength</div>
							<div class="ui-optionset-caption">How strong do you want the effect to be?</div>
							<div class="ui-optionset-content">
								<div id="buttonsetIntensity" class="ui-optionset-content">
									<input type="radio" value="0" name="intensity" id="optionAmountLow" /><label for="optionAmountLow">Low</label>
									<input type="radio" value="1" name="intensity" id="optionAmountMedium" checked="checked" /><label for="optionAmountMedium">Medium</label>
									<input type="radio" value="2" name="intensity" id="optionAmountHigh" /><label for="optionAmountHigh">High</label>
								</div>
							</div>
						</div>
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
	appOptions.bubbleDirections = <%= new JavaScriptSerializer().Serialize(this.Model.Bubbles) %>;
	appOptions.edit = true;
	Comic.Create(appOptions);
	</script>

	<script type="text/x-jqote-template" id="templateTemplate">
	<![CDATA[
	<span class="template template-size<#= this.TemplateItems.length #>">
		<img class="template-picture" src="<#= this.ThumbUrl #>" alt="" />
	</span>
	]]>
	</script>

	<script type="text/x-jqote-template" id="bubbleTemplate">
	<![CDATA[
	<span class="bubble">
		<img class="bubbleThumb" src="<#= this.ImageUrl #>" alt="" />
	</span>
	]]>
	</script>

	<script type="text/x-jqote-template" id="albumTemplate">
	<![CDATA[
	<span class="album">
		<#= this.name.wordSubstr(0, 20, '...') #>
	</span>
	]]>
	</script>

	<script type="text/x-jqote-template" id="photoTemplate">
	<![CDATA[
	<span class="photo">
		<img class="photoThumb" src="<#= this.picture #>" alt="" />
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