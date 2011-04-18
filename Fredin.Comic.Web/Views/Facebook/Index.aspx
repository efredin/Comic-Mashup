<%@ Page Language="C#" MasterPageFile="~/Views/Facebook/Facebook.Master" Inherits="Fredin.Comic.Web.ComicViewPage<Fredin.Comic.Web.Models.ViewFacebook>" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>

<asp:Content ID="cPageTitle" ContentPlaceHolderID="cphPageTitle" runat="server">Profile Photo | Comic Mashup for Profiles</asp:Content>
<asp:Content ID="cTitle" ContentPlaceHolderID="cphTitle" runat="server">Profile Photo</asp:Content>

<asp:Content ID="cCanvas" ContentPlaceHolderID="cphCanvas" runat="server">

	<p>Transform your profile picture into something new and exciting!</p>

	<div class="fbrectangle">
		<%--  Lifestreet Tag --%>
		<% #if !DEBUG %>
		<iframe width='300' height='250' frameborder='no' framespacing='0' scrolling='no'  src='http://ads.lfstmedia.com/fbslot/slot19152?ad_size=300x250&adkey=7a7'></iframe>
		<% #else %>
		<div class="ad-debug"></div>
		<% #endif %>
	</div>

	<div class="content434">
		<% using (this.Html.BeginForm("QueueRender", "Facebook")) { %>
			<div id="optionsetEffect" class="ui-optionset">
				<div class="ui-optionset-title">Effect</div>
				<div class="ui-optionset-caption">Choose the effect you want applied to your profile picture.</div>
				<div class="ui-optionset-content">
					<select id="effectSelector" name="effect"></select>
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
			<button type="button" id="buttonRender">Draw Photo</button>
		<% } %>
	</div>

</asp:Content>

<asp:Content ID="cScript" ContentPlaceHolderID="cphScript" runat="server">
	<script type="text/javascript">
	appOptions.effects = <%= new JavaScriptSerializer().Serialize(this.Model.Effects) %>;
	Facebook.Index(appOptions);
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
