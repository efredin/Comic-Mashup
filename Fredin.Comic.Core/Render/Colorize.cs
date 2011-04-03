using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

using AForge.Imaging;
using AForge.Imaging.Filters;

namespace Fredin.Comic.Render
{
	public class Colorize : BaseInPlaceFilter
	{
		#region [Member]

		private double _toleranceFactor = 0.30;
		private double _toleranceR = 0;
		private double _toleranceG = 0;
		private double _toleranceB = 0;
		private List<PixelBlob> _blobs;
		private List<PixelData> _pixels;
		private Dictionary<PixelFormat, PixelFormat> _formatTransalations;

		#endregion

		#region [Property]

		public override Dictionary<PixelFormat, PixelFormat> FormatTransalations
		{
			get { return this._formatTransalations; }
		}

		public double ToleranceFactor
		{
			get { return this._toleranceFactor; }
			set { this._toleranceFactor = value; }
		}

		private double ToleranceR
		{
			get { return this._toleranceR; }
			set { this._toleranceR = value; }
		}

		private double ToleranceG
		{
			get { return this._toleranceG; }
			set { this._toleranceG = value; }
		}

		private double ToleranceB
		{
			get { return this._toleranceB; }
			set { this._toleranceB = value; }
		}

		private List<PixelData> Pixels
		{
			get { return this._pixels; }
		}

		private List<PixelBlob> Blobs
		{
			get { return this._blobs; }
		}

		#endregion

		#region [Method]

		public Colorize()
		{
			this._blobs = new List<PixelBlob>();
			this._pixels = new List<PixelData>();

			this._formatTransalations = new Dictionary<PixelFormat, PixelFormat>();
			this.FormatTransalations.Add(PixelFormat.Format24bppRgb, PixelFormat.Format24bppRgb);
			this.FormatTransalations.Add(PixelFormat.Format32bppRgb, PixelFormat.Format32bppRgb);
			this.FormatTransalations.Add(PixelFormat.Format32bppArgb, PixelFormat.Format32bppArgb);
		}

		protected override unsafe void ProcessFilter(UnmanagedImage image)
		{
			//TODO: Empty image / 0 pixels / division by 0

			PixelData data;
			//IntPtr scan0 = image.Scan0;
			//int stride = image.Stride;
			int offset = (image.Stride - image.Width * 3);

			// Calc running total of pixel values for standard deviation
			double totalR = 0;
			double totalG = 0;
			double totalB = 0;

			byte* ptr = (byte*)image.ImageData.ToPointer();
			for (int y = 0; y < image.Height; y++)
			{
				for (int x = 0; x < image.Width; x++, ptr += 3)
				{
					// Get pixel data for all channels
					data = new PixelData();
					data.Red = ptr[RGB.R];
					data.Green = ptr[RGB.G];
					data.Blue = ptr[RGB.B];
					data.X = x;
					data.Y = y;
					data.Ptr = ptr;
					this.Pixels.Add(data);

					totalR += data.Red;
					totalG += data.Green;
					totalB += data.Blue;
				}

				ptr += offset;
			}

			// Total variances of pixel channels for standard deviation
			double varianceR = 0;
			double varianceG = 0;
			double varianceB = 0;
			double averageR = totalR / this.Pixels.Count;
			double averageG = totalG / this.Pixels.Count;
			double averageB = totalB / this.Pixels.Count;

			for (int p = 0; p < this.Pixels.Count; p++)
			{
				varianceR += Math.Pow(this.Pixels[p].Red - averageR, 2);
				varianceG += Math.Pow(this.Pixels[p].Green - averageG, 2);
				varianceB += Math.Pow(this.Pixels[p].Blue - averageB, 2);
			}

			// Calculate standard deviation of each channel
			double deviationR = Math.Sqrt(varianceR / this.Pixels.Count);
			double deviationG = Math.Sqrt(varianceG / this.Pixels.Count);
			double deviationB = Math.Sqrt(varianceB / this.Pixels.Count);

			this.ToleranceR = deviationR * this.ToleranceFactor;
			this.ToleranceG = deviationG * this.ToleranceFactor;
			this.ToleranceB = deviationB * this.ToleranceFactor;

			// Generate blobs from the image
			for (int p = 0; p < this.Pixels.Count; p++)
			{
				// Attempt to find a blob to belong to
				PixelBlob blob = this.FindBlob(this.Pixels[p]);
				if (blob == null)
				{
					// No blob found -- create a new one
					blob = new PixelBlob();
					this.Blobs.Add(blob);
				}

				// Add the current pixel to the blob
				blob.AddPixel(this.Pixels[p]);
			}

			// Traverse pixels again, this time to set them to blob averages
			for (int b = 0; b < this.Blobs.Count; b++)
			{
				PixelBlob currentBlob = this.Blobs[b];

				// Y pixels
				foreach (PixelDataList pixel in currentBlob.Pixels)
				{
					// X pixels
					foreach (PixelData pixelData in pixel)
					{
						pixelData.Ptr[RGB.R] = currentBlob.AverageR;
						pixelData.Ptr[RGB.G] = currentBlob.AverageG;
						pixelData.Ptr[RGB.B] = currentBlob.AverageB;
					}
				}
			}
		}

		/// <summary>
		/// Forward-only blob search
		/// </summary>
		private PixelBlob FindBlob(PixelData data)
		{
			PixelBlob blob = null;
			PixelBlob currentBlob = null;
			PixelDataList pixels = null;
			bool isNeighbor = false;

			for (int b = this.Blobs.Count - 1; b >= 0 && blob == null; b--)
			{
				currentBlob = this.Blobs[b];
				pixels = null;
				isNeighbor = false;

				if (currentBlob.Pixels.Count > 0)
				{
					pixels = currentBlob.Pixels[currentBlob.Pixels.Count - 1];

					// Same Y-Index as the current pixel
					if (pixels.Y == data.Y)
					{
						if (Math.Abs(pixels[pixels.Count - 1].X - data.X) <= 1)
						{
							isNeighbor = true;
						}
					}
					// Previous Y-index as the current pixel
					else if (pixels.Y == (data.Y - 1))
					{
						// Iterate through the y position
						for (int x = 0; x < pixels.Count && !isNeighbor; x++)
						{
							if (Math.Abs(pixels[x].X - data.X) == 0)
							{
								isNeighbor = true;
							}
						}
					}

					// If the current blob neighbors the current pixel. Check the threshold
					if (isNeighbor && Math.Abs(data.Red - currentBlob.AverageR) <= this.ToleranceR && Math.Abs(data.Green - currentBlob.AverageG) <= this.ToleranceG && Math.Abs(data.Blue - currentBlob.AverageB) <= this.ToleranceB)
					{
						blob = currentBlob;
					}
				}
			}

			return blob;
		}

		#endregion

		#region [Sub Class]

		private class PixelBlob
		{
			#region [Member]

			List<PixelDataList> _pixels;
			private PixelData _lastPixel;

			private long _totalR;
			private long _totalG;
			private long _totalB;
			private int _xMin;
			private int _xMax;
			private int _yMin;
			private int _yMax;
			private int _staticCount = 0;

			#endregion

			#region [Property]

			private PixelData LastPixel
			{
				get { return this._lastPixel; }
				set { this._lastPixel = value; }
			}

			public List<PixelDataList> Pixels
			{
				get { return this._pixels; }
			}

			/// <summary>
			/// Sum of all Red channel pixel values
			/// </summary>
			public long TotalR
			{
				get { return this._totalR; }
			}

			/// <summary>
			/// Sum of all Green channel pixel values
			/// </summary>
			public long TotalG
			{
				get { return this._totalG; }
			}

			/// <summary>
			/// Sum of all Blue channel pixel values
			/// </summary>
			public long TotalB
			{
				get { return this._totalB; }
			}

			public byte AverageR
			{
				get { return Convert.ToByte(this.TotalR / this._staticCount); }
			}

			public byte AverageG
			{
				get { return Convert.ToByte(this.TotalG / this._staticCount); }
			}

			public byte AverageB
			{
				get { return Convert.ToByte(this.TotalB / this._staticCount); }
			}

			public int XMax
			{
				get { return this._xMax; }
			}

			public int XMin
			{
				get { return this._xMin; }
			}

			public int YMax
			{
				get { return this._yMax; }
			}

			public int YMin
			{
				get { return this._yMin; }
			}

			#endregion

			#region [Method]

			public PixelBlob()
			{
				this._pixels = new List<PixelDataList>();
			}

			public void AddPixel(PixelData data)
			{
				// Add to pixels
				if (this.Pixels.Count == 0 || this.Pixels[this.Pixels.Count - 1].Y != data.Y)
				{
					PixelDataList yData = new PixelDataList();
					yData.Y = data.Y;
					this.Pixels.Add(yData);
				}
				this.Pixels[this.Pixels.Count - 1].Add(data);

				// Backwards blob reference for the pixel
				data.Blob = this;
				this._staticCount++;

				this._totalR += data.Red;
				this._totalG += data.Green;
				this._totalB += data.Blue;

				// Check for new blob x range
				if (this.XMin > data.X)
				{
					this._xMin = data.X;
				}
				else if (this.XMax < data.X)
				{
					this._xMax = data.X;
				}

				// Check for new blob y range
				if (this.YMin > data.Y)
				{
					this._yMin = data.Y;
				}
				else if (this.YMax < data.Y)
				{
					this._yMax = data.Y;
				}
			}

			#endregion
		}

		private unsafe class PixelData : RGB
		{
			public int X;
			public int Y;
			public PixelBlob Blob;
			public byte* Ptr;
		}

		private class PixelDataList : List<PixelData>
		{
			private int _y;

			public int Y
			{
				get { return this._y; }
				set { this._y = value; }
			}

			public PixelDataList()
				: base()
			{
			}
		}

		#endregion
	}
}