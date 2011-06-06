<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Web.Master" Inherits="Fredin.Comic.Web.ComicViewPage<dynamic>" %>

<asp:Content ID="cPageTitle" ContentPlaceHolderID="cphPageTitle" runat="server">FAQ | Comic Mashup</asp:Content>
<asp:Content ID="cTitle" ContentPlaceHolderID="cphTitle" runat="server">FAQ</asp:Content>
<asp:Content ID="cCanvas" ContentPlaceHolderID="cphCanvas" runat="server">

	<div id="faq" class="box content734">
		<p>If your question is not answered below, please <a href="<%= this.Url.Action("Contact", "Help") %>">contact us</a>.</p>
		<ul>
			<li>
				<div class="question">Why do no comments load when I try to create a comic?</div>
				<div class="answer">You may not have recent posts in your news feed which can be used to create a comic. Only posts created by you that also have comments will show up.</div>
			</li>
			<li>
				<div class="question">Why do I only see profile pictures for some of my friends?</div>
				<div class="answer">Your friends may have high photo privacy settings, preventing Comic Mashup from reading their photos.  This privacy setting doesn’t apply if your friends also use Comic Mashup.</div>
			</li>
		</ul>
	</div>
</asp:Content>


