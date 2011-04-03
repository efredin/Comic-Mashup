<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Web/Views/Web.Master" Inherits="System.Web.Mvc.ViewPage<Fredin.Comic.Web.Areas.Web.Models.LoginModel>" %>

<asp:Content ID="cCanvas" ContentPlaceHolderID="cphCanvas" runat="server">
	<p>
		Please login to SocialPIX to continue.  If you don't have an account you may create one, or use facebook connect.
	</p>

	<div id="login-login" class="column columnLeft">
		<% using (Html.BeginForm()) { %>
			<fieldset>
				<legend>Login</legend>

				<div class="label">Email</div>
				<div class="field">
					<%= Html.TextBox("email") %>
					<%= Html.ValidationMessage("email")%>
				</div>

				<div class="label">Password</div>
				<div class="field">
					<%= Html.Password("password") %>
					<%= Html.ValidationMessage("password") %>
				</div>

				<button type="submit" value="Login">Login</button>

				<fb:login-button v="2" onlogin="top.location.href='';"><fb:intl>Connect with Facebook</fb:intl></fb:login-button>
			</fieldset>
		<% } %>

	</div>

	<div id="login-create" class="column columnRight">
		<% using (Html.BeginForm()) { %>
			<fieldset>
				<legend>Create an Account</legend>

				<%--<div class="label">Name</div>
				<div class="field"><asp:TextBox runat="server" ID="txtCreateName" ValidationGroup="create" /></div>

				<div class="label">Email</div>
				<div class="field"><asp:TextBox runat="server" ID="txtCreateEmail" /><span id="invalidEmail" class="invalid hide"></span></div>

				<div class="label">Password</div>
				<div class="field">
					<asp:TextBox runat="server" ID="txtCreatePassword" TextMode="Password" /><span id="invalidPassword" class="invalid hide"></span>
				</div>

				<div class="label">Confirm Password</div>
				<div class="field">
					<asp:TextBox runat="server" ID="txtCreatePassword2" TextMode="Password" />
				</div>

				<button type="submit" value="Create">Create Account</button>
	--%>
			</fieldset>
		<% } %>
	</div>

</asp:Content>
