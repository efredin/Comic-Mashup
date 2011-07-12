using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fredin.Comic.Render;
using System.Drawing;

public partial class _Default : System.Web.UI.Page
{
	protected override void OnLoad(EventArgs e)
	{
		Bitmap imageIn = new Bitmap(@"C:\Dev\Fredin\Fredin.Comic.WebTest\in.jpg");

		RenderHelper effectHelper = new RenderHelper(imageIn.Size);
		Dictionary<string, object> parameters = new Dictionary<string, object>();
		parameters.Add("coloring", 6);
		ImageRenderData renderResult = effectHelper.RenderEffect(imageIn, ComicEffectType.Comic, parameters);

		Bitmap imageOut = new Bitmap(renderResult.RenderStream);
		imageOut.Save(@"C:\Dev\Fredin\Fredin.Comic.WebTest\out.jpg");

		base.OnLoad(e);
	}
}
