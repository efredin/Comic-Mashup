using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;
using AForge.Imaging.Filters;

namespace Fredin.Comic.Render
{
	public class ComicGenerator
	{
		public enum ImageAlign
		{
			Center,
			Top
		}

		public int Width { get; private set; }
		public int Height { get; private set; }
		public Bitmap ComicImage { get; private set; }

		public static Font ComicFont { get; set; }
		private static PrivateFontCollection CustomFont { get; set; }

		static ComicGenerator()
		{
			CustomFont = new PrivateFontCollection();
			unsafe
			{
				Stream fontStream = typeof(ComicGenerator).Assembly.GetManifestResourceStream("Fredin.Comic.Image.smackattackbb_reg.ttf");
				byte[] fontBuffer = new byte[fontStream.Length];
				fontStream.Read(fontBuffer, 0, fontBuffer.Length);
				fontStream.Close();

				IntPtr fontPtr = Marshal.AllocHGlobal(fontBuffer.Length);
				Marshal.Copy(fontBuffer, 0, fontPtr, fontBuffer.Length);

				CustomFont.AddMemoryFont(fontPtr, fontBuffer.Length);

				Marshal.FreeHGlobal(fontPtr);
			}

			ComicFont = new Font(CustomFont.Families[0], 8, FontStyle.Regular, GraphicsUnit.Point);
		}

		public ComicGenerator(int width, int height)
		{
			this.Width = width;
			this.Height = height;
			this.ComicImage = new Bitmap(this.Width, this.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

			using (Graphics thumbGraphics = Graphics.FromImage(this.ComicImage))
			{
				thumbGraphics.FillRectangle(Brushes.Black, 0, 0, this.Width, this.Height);
			}
		}

		public Bitmap GenerateThumb(int width)
		{
			int height = Convert.ToInt32(Convert.ToDouble(this.ComicImage.Height) / Convert.ToDouble(this.ComicImage.Width) * Convert.ToDouble(width));
			ResizeBicubic filterResize = new ResizeBicubic(width, height);
			return filterResize.Apply(this.ComicImage);
		}

		public void AddFitImage(Bitmap image, int width, int height, int x, int y)
		{
			this.AddFitImage(image, width, height, x, y, ImageAlign.Top);
		}

		public void AddFitImage(Bitmap image, int width, int height, int x, int y, ImageAlign alignment)
		{
			// Resize and crop image to fit in the max bounds
			Bitmap fitImage = GetFitImage(image, width, height, x, y, alignment);

			using (Graphics thumbGraphics = Graphics.FromImage(this.ComicImage))
			{
				thumbGraphics.DrawImage(fitImage, x, y, fitImage.Width, fitImage.Height);
			}
		}

		public static Bitmap GetFitImage(Bitmap image, int width, int height, int x, int y, ImageAlign alignment)
		{
			return CropImage(new Size(width, height), FitImage(new Size(width, height), image), alignment);
		}

		public void AddScaleImage(Bitmap image, int width, int height, int x, int y)
		{
			using (Graphics thumbGraphics = Graphics.FromImage(this.ComicImage))
			{
				ImageAttributes attr = new ImageAttributes();
				attr.SetColorKey(Color.Transparent, Color.Transparent);

				Rectangle destination = new Rectangle(x, y, width, height);
				thumbGraphics.DrawImage(image, destination, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
			}
		}

		public void AddText(string text, RectangleF layoutRectangle)
		{
			using (Graphics thumbGraphics = Graphics.FromImage(this.ComicImage))
			{
				StringFormat textFormat = new StringFormat();
				textFormat.Alignment = StringAlignment.Center;

				// AntiAliasing
				thumbGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
				thumbGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

				//layoutRectangle.Width += 5; // Tollerance for measure error

				thumbGraphics.DrawString(text, ComicFont, Brushes.Black, layoutRectangle, textFormat);
			}
		}

		public SizeF MeasureText(string text)
		{
			return this.MeasureText(text, 2000);
		}

		public SizeF MeasureText(string text, int maxWidth)
		{
			SizeF result = default(SizeF);
		
			using (Graphics thumbGraphics = Graphics.FromImage(this.ComicImage))
			{
				StringFormat textFormat = new StringFormat();
				textFormat.Alignment = StringAlignment.Center;

				// AntiAliasing
				thumbGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
				thumbGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

				result = thumbGraphics.MeasureString(text, ComicFont, maxWidth, textFormat);
			}

			return result;
		}

		public static Rectangle GetCropImageSize(Size inputSize, Size cropSize, ImageAlign alignment)
		{
			int x = Convert.ToInt32(Convert.ToDouble(inputSize.Width - cropSize.Width) / 2d);
			int y = 0;

			if (alignment == ImageAlign.Center)
			{
				y = Convert.ToInt32(Convert.ToDouble(inputSize.Height - cropSize.Height) / 2d);
			}

			return new Rectangle(x, y, cropSize.Width, cropSize.Height);
		}

		public static Bitmap CropImage(Size cropSize, Bitmap inputBitmap, ImageAlign alignment)
		{
			Crop imageCrop = new Crop(GetCropImageSize(new Size(inputBitmap.Width, inputBitmap.Height), cropSize, alignment));
			return imageCrop.Apply(inputBitmap);
		}

		public static Size GetFitImageSize(Size inputSize, Size targetSize)
		{
			// Calculate item ratios
			double imageRatio = Convert.ToDouble(inputSize.Width) / Convert.ToDouble(inputSize.Height);
			double scaleRatio = Convert.ToDouble(targetSize.Width) / Convert.ToDouble(targetSize.Height);

			Size resize = new Size();

			// Scale raw image to fit shortest dimension
			if (imageRatio >= scaleRatio)
			{
				resize.Height = targetSize.Height;
				resize.Width = Convert.ToInt32(imageRatio * Convert.ToDouble(resize.Height));
			}
			else
			{
				resize.Width = targetSize.Width;
				resize.Height = Convert.ToInt32(Convert.ToDouble(inputSize.Height) / Convert.ToDouble(inputSize.Width) * Convert.ToDouble(resize.Width));
			}

			return resize;
		}

		public static Bitmap FitImage(Size targetSize, Bitmap inputBitmap)
		{
			Size resize = GetFitImageSize(new Size(inputBitmap.Width, inputBitmap.Height), targetSize);

			ResizeBicubic filterResize = new ResizeBicubic(resize.Width, resize.Height);
			return filterResize.Apply(inputBitmap);
		}
	}
}
