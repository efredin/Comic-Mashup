using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Fredin.Comic.Image
{
	public static class BitmapExtension
	{
		public static Bitmap ConvertFormat(this Bitmap source, PixelFormat format)
		{
			Bitmap result = new Bitmap(source.Width, source.Height, format);
			using (Graphics g = Graphics.FromImage(result))
			{
				g.DrawImage(source, 0, 0, source.Width, source.Height);
			}
			return result;
		}
	}
}
