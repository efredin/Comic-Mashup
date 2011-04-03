using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fredin.Comic.Render
{
	public enum RenderMode
	{
		/// <summary>
		/// Full size comic
		/// </summary>
		Comic,

		/// <summary>
		/// Thumbnail
		/// </summary>
		Thumb,

		/// <summary>
		/// Full first frame only
		/// </summary>
		Frame,

		/// <summary>
		/// Thumbnail first frame
		/// </summary>
		FrameThumb
	}
}