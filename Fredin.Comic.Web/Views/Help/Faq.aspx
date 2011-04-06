<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Web.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="cPageTitle" ContentPlaceHolderID="cphPageTitle" runat="server">FAQ | Comic Mashup</asp:Content>
<asp:Content ID="cTitle" ContentPlaceHolderID="cphTitle" runat="server">FAQ</asp:Content>
<asp:Content ID="cCanvas" ContentPlaceHolderID="cphCanvas" runat="server">
	
	<%= Html.Partial("~/Views/Shared/AdSkyscraper.ascx") %>
	<div id="faq" class="box content764">
		<p>If your question is not answered below, please <a href="<%= this.Url.Action("Contact", "Help") %>">contact us</a>.</p>
		<p>
			<ul>
				<li>
					<div class="question">Why do I only see profile pictures for some of my friends?</div>
					<div class="answer">Your friends may have high photo privacy settings, preventing Comic Mashup from reading their photos.  This privacy setting doesn’t apply if your friends also use Comic Mashup.</div>
				</li>
			</ul>
		</p>
	</div>
</asp:Content>


