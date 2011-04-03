using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Fredin.Comic.Render
{
	public enum ComicEffectType
	{
		None,

		[Description("Comic")]
		Comic,

		[Description("Color Sketch")]
		ColorSketch,

		[Description("Pencil Sketch")]
		PencilSketch
	}
}
