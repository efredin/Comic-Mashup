﻿<%@ Page Language="C#" MasterPageFile="~/Views/Facebook/Facebook.Master" Inherits="System.Web.Mvc.ViewPage<Fredin.Comic.Web.Models.ClientProfileTask>" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>

<asp:Content ID="cPageTitle" ContentPlaceHolderID="cphPageTitle" runat="server">Results | Comic Mashup for Profiles</asp:Content>
<asp:Content ID="cTitle" ContentPlaceHolderID="cphTitle" runat="server">Results</asp:Content>

<asp:Content ID="cCanvas" ContentPlaceHolderID="cphCanvas" runat="server">

	<div class="fbrectangle">
		<%--  Lifestreet Tag --%>
		<% #if !DEBUG %>
		<iframe width='300' height='250' frameborder='no' framespacing='0' scrolling='no'  src='http://ads.lfstmedia.com/fbslot/slot19152?ad_size=300x250&adkey=7a7'></iframe>
		<% #else %>
		<div class="ad-debug"></div>
		<% #endif %>
	</div>

	<div class="content434">
		<div id="renderLoad">
			Your photo is being created - This may take a moment . . .
		</div>
		<div id="renderContent">
			<div class="box contentIb">
				<div id="renderPhoto"></div>
				<div id="renderAction">
					<a id="renderBack" href="<%= this.Url.Action("Index", "Facebook") %>">Back</a>
					<button id="renderShare">Share</button>
				</div>
			</div>
			<p>If you think this is cool, you should check out the full <a href="http://www.comicmashup.com/" target="_top">Comic Mashup</a> app.  Head on over to <a href="http://www.comicmashup.com/" target="_top">www.comicmashup.com</a> to transform your status updates into awesome web comics!</p>
		</div>
	</div>
</asp:Content>

<asp:Content ID="cScript" ContentPlaceHolderID="cphScript" runat="server">
	<script type="text/javascript">
	appOptions.task = <%= new JavaScriptSerializer().Serialize(this.Model) %>;
	Facebook.Render(appOptions);
	</script>
</asp:Content>
