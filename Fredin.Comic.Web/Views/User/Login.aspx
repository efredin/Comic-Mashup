<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Web.Master" Inherits="Fredin.Comic.Web.ComicViewPage<dynamic>" %>
<%@Import Namespace="Fredin.Comic.Web" %>

<asp:Content ID="cPageTitle" ContentPlaceHolderID="cphPageTitle" runat="server">Login | Comic Mashup</asp:Content>
<asp:Content ID="cTitle" ContentPlaceHolderID="cphTitle" runat="server">Login</asp:Content>
<asp:Content ID="cCanvas" ContentPlaceHolderID="cphCanvas" runat="server">

	<div class="box content800">
		<p>Please login to continue.</p>
		<a href="javascript:void(0);" class="button-fbLogin fb_button fb_button_medium">
			<span class="fb_button_text">Connect with Facebook</span>
		</a>
	</div>

</asp:Content>

<asp:Content ID="cScript" ContentPlaceHolderID="cphScript" runat="server">
	<script type="text/javascript">
	Application.create(
	{
		onConnect: function(uid)
		{
			// redirect to original request url
			if(document.referrer != '' && document.referrer != '<%= this.Url.Action("Login", "User") %>')
			{
				document.location = document.referrer;
			}
			else
			{
				document.location = '<%= this.Url.Content("~/") %>';
			}
		}
	}, appOptions);

	</script>
</asp:Content>