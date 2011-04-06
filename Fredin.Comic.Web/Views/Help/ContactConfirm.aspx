<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Web.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cphPageTitle" runat="server">Contact | Comic Mashup</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cphTitle" runat="server">Contact</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="cphCanvas" runat="server">

	<%= Html.Partial("~/Views/Shared/AdSkyscraper.ascx") %>
    <div class="box box764">
		<p>Your message has been received. Thank you.</p>
	</div>

</asp:Content>


