using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.HPBProcessing;
using AspNetCore.ReportingServices.Rendering.RichText;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace AspNetCore.ReportingServices.Rendering.ImageRenderer
{
	internal class Graphics : GraphicsBase
	{
		private EncoderParameters m_encoderParameters;

		private Bitmap m_firstImage;

		protected Win32ObjectSafeHandle m_hBitmap;

		protected Win32DCSafeHandle m_hdcBitmap;

		private static ImageCodecInfo[] m_encoders = Graphics.GetGdiImageEncoders();

		internal Graphics(float dpiX, float dpiY)
			: base(dpiX, dpiY)
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.m_firstImage != null)
				{
					this.m_firstImage.Dispose();
					this.m_firstImage = null;
				}
				if (this.m_encoderParameters != null)
				{
					this.m_encoderParameters.Dispose();
					this.m_encoderParameters = null;
				}
			}
			if (this.m_hdcBitmap != null)
			{
				this.m_hdcBitmap.Close();
				this.m_hdcBitmap = null;
			}
			if (this.m_hBitmap != null)
			{
				this.m_hBitmap.Close();
				this.m_hBitmap = null;
			}
			base.Dispose(disposing);
		}

		internal virtual void Save(Stream outputStream, PaginationSettings.FormatEncoding outputFormat)
		{
			Bitmap bitmap = null;
			bool flag = true;
			try
			{
				bitmap = System.Drawing.Image.FromHbitmap(this.m_hBitmap.Handle);
				switch (outputFormat)
				{
				case PaginationSettings.FormatEncoding.BMP:
					bitmap.Save(outputStream, ImageFormat.Bmp);
					break;
				case PaginationSettings.FormatEncoding.GIF:
					bitmap.Save(outputStream, ImageFormat.Gif);
					break;
				case PaginationSettings.FormatEncoding.JPEG:
					bitmap.Save(outputStream, ImageFormat.Jpeg);
					break;
				case PaginationSettings.FormatEncoding.PNG:
					bitmap.Save(outputStream, ImageFormat.Png);
					break;
				case PaginationSettings.FormatEncoding.TIFF:
					if (this.m_firstImage == null)
					{
						this.m_firstImage = bitmap;
						flag = false;
						this.m_encoderParameters = new EncoderParameters(2);
						this.m_encoderParameters.Param[0] = new EncoderParameter(Encoder.SaveFlag, 18L);
						this.m_encoderParameters.Param[1] = new EncoderParameter(Encoder.ColorDepth, 24L);
						this.m_firstImage.Save(outputStream, Graphics.GetEncoderInfo("image/tiff"), this.m_encoderParameters);
						EncoderParameter encoderParameter = this.m_encoderParameters.Param[0];
						this.m_encoderParameters.Param[0] = new EncoderParameter(Encoder.SaveFlag, 23L);
						if (encoderParameter != null)
						{
							encoderParameter.Dispose();
							encoderParameter = null;
						}
					}
					else
					{
						this.m_firstImage.SaveAdd(bitmap, this.m_encoderParameters);
					}
					break;
				}
				outputStream.Flush();
			}
			finally
			{
				if (flag && bitmap != null)
				{
					bitmap.Dispose();
					bitmap = null;
				}
			}
		}

		internal void NewPage(float pageWidth, float pageHeight, int dpiX, int dpiY)
		{
			if (base.m_graphicsBase != null)
			{
				this.ReleaseCachedHdc(true);
				base.m_graphicsBase.Dispose();
				base.m_graphicsBase = null;
			}
			if (this.m_hdcBitmap != null)
			{
				this.m_hdcBitmap.Close();
				this.m_hdcBitmap = null;
			}
			if (this.m_hBitmap != null)
			{
				this.m_hBitmap.Close();
				this.m_hBitmap = null;
			}
			IntPtr intPtr = IntPtr.Zero;
			try
			{
				intPtr = AspNetCore.ReportingServices.Rendering.RichText.Win32.GetDC(IntPtr.Zero);
				this.HandleError(intPtr);
				this.m_hdcBitmap = AspNetCore.ReportingServices.Rendering.RichText.Win32.CreateCompatibleDC(intPtr);
				this.HandleError(this.m_hdcBitmap);
				AspNetCore.ReportingServices.Rendering.RichText.Win32.BITMAPINFOHEADER bITMAPINFOHEADER = new AspNetCore.ReportingServices.Rendering.RichText.Win32.BITMAPINFOHEADER(base.ConvertToPixels(pageWidth), base.ConvertToPixels(pageHeight), base.DpiX, base.DpiY);
				IntPtr zero = IntPtr.Zero;
				this.m_hBitmap = AspNetCore.ReportingServices.Rendering.RichText.Win32.CreateDIBSection(this.m_hdcBitmap, ref bITMAPINFOHEADER, 0u, ref zero, IntPtr.Zero, 0u);
				this.HandleError(this.m_hBitmap);
				AspNetCore.ReportingServices.Rendering.RichText.Win32.SelectObject(this.m_hdcBitmap, this.m_hBitmap);
				base.m_graphicsBase = System.Drawing.Graphics.FromHdc(this.m_hdcBitmap.Handle);
				Graphics.SetGraphicsProperties(base.m_graphicsBase);
				base.m_graphicsBase.Clear(Color.White);
			}
			catch (Exception)
			{
				if (this.m_hdcBitmap != null)
				{
					this.m_hdcBitmap.Close();
					this.m_hdcBitmap = null;
				}
				if (this.m_hBitmap != null)
				{
					this.m_hBitmap.Close();
					this.m_hBitmap = null;
				}
				throw;
			}
			finally
			{
				if (IntPtr.Zero != intPtr)
				{
					AspNetCore.ReportingServices.Rendering.RichText.Win32.ReleaseDC(IntPtr.Zero, intPtr);
				}
			}
		}

		private void HandleError(Win32DCSafeHandle handle)
		{
			if (!handle.IsInvalid)
			{
				return;
			}
			throw new ReportRenderingException(Marshal.GetExceptionForHR(Marshal.GetLastWin32Error()));
		}

		private void HandleError(Win32ObjectSafeHandle handle)
		{
			if (!handle.IsInvalid)
			{
				return;
			}
			throw new ReportRenderingException(Marshal.GetExceptionForHR(Marshal.GetLastWin32Error()));
		}

		private void HandleError(IntPtr handle)
		{
			if (!(IntPtr.Zero == handle))
			{
				return;
			}
			throw new ReportRenderingException(Marshal.GetExceptionForHR(Marshal.GetLastWin32Error()));
		}

		internal void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
		{
			this.ReleaseCachedHdc(true);
			this.ExecuteSync(delegate
			{
				base.m_graphicsBase.DrawLine(pen, base.ConvertToPixels(x1), base.ConvertToPixels(y1), base.ConvertToPixels(x2), base.ConvertToPixels(y2));
			});
		}

		internal void DrawImage(System.Drawing.Image image, RectangleF destination, RectangleF source)
		{
			this.DrawImage(image, destination, source, true);
		}

		internal void DrawImage(System.Drawing.Image image, RectangleF destination, RectangleF source, bool tile)
		{
			this.ReleaseCachedHdc(true);
			this.ExecuteSync(delegate
			{
				ImageAttributes imageAttributes = null;
				try
				{
					if (tile)
					{
						imageAttributes = new ImageAttributes();
						imageAttributes.SetWrapMode(WrapMode.Tile);
					}
					PointF[] destPoints = new PointF[3]
					{
						new PointF((float)base.ConvertToPixels(destination.Location.X), (float)base.ConvertToPixels(destination.Location.Y)),
						new PointF((float)base.ConvertToPixels(destination.Location.X + destination.Width), (float)base.ConvertToPixels(destination.Location.Y)),
						new PointF((float)base.ConvertToPixels(destination.Location.X), (float)base.ConvertToPixels(destination.Location.Y + destination.Height))
					};
					base.m_graphicsBase.DrawImage(image, destPoints, source, GraphicsUnit.Pixel, imageAttributes);
				}
				finally
				{
					if (imageAttributes != null)
					{
						imageAttributes.Dispose();
						imageAttributes = null;
					}
				}
			});
		}

		internal void DrawRectangle(Pen pen, RectangleF rectangle)
		{
			this.ReleaseCachedHdc(true);
			this.ExecuteSync(delegate
			{
				base.m_graphicsBase.DrawRectangle(pen, base.ConvertToPixels(rectangle.X), base.ConvertToPixels(rectangle.Y), base.ConvertToPixels(rectangle.Width), base.ConvertToPixels(rectangle.Height));
			});
		}

		internal void FillPolygon(Brush brush, PointF[] polygon)
		{
			this.ReleaseCachedHdc(true);
			this.ExecuteSync(delegate
			{
				Point[] array = new Point[polygon.Length];
				for (int i = 0; i < polygon.Length; i++)
				{
					PointF pointF = polygon[i];
					array[i].X = base.ConvertToPixels(pointF.X);
					array[i].Y = base.ConvertToPixels(pointF.Y);
				}
				base.m_graphicsBase.FillPolygon(brush, array);
			});
		}

		internal void FillRectangle(Brush brush, RectangleF rectangle)
		{
			this.ReleaseCachedHdc(true);
			this.ExecuteSync(delegate
			{
				base.m_graphicsBase.FillRectangle(brush, base.ConvertToPixels(rectangle.X), base.ConvertToPixels(rectangle.Y), base.ConvertToPixels(rectangle.Width), base.ConvertToPixels(rectangle.Height));
			});
		}

		internal void ResetClipAndTransform(RectangleF bounds)
		{
			this.ReleaseCachedHdc(true);
			this.ExecuteSync(delegate
			{
				base.m_graphicsBase.ResetClip();
				base.m_graphicsBase.ResetTransform();
				System.Drawing.Rectangle clip = new System.Drawing.Rectangle(base.ConvertToPixels(bounds.X), base.ConvertToPixels(bounds.Y), base.ConvertToPixels(bounds.Width), base.ConvertToPixels(bounds.Height));
				base.m_graphicsBase.SetClip(clip);
				using (Matrix matrix = new Matrix())
				{
					matrix.Translate((float)clip.Left, (float)clip.Top);
					base.m_graphicsBase.Transform = matrix;
				}
			});
		}

		internal void RotateTransform(float angle)
		{
			this.ReleaseCachedHdc(true);
			this.ExecuteSync(delegate
			{
				base.m_graphicsBase.RotateTransform(angle);
			});
		}

		internal void EndReport(PaginationSettings.FormatEncoding outputFormat)
		{
			if (outputFormat == PaginationSettings.FormatEncoding.TIFF)
			{
				EncoderParameter encoderParameter = this.m_encoderParameters.Param[0];
				this.m_encoderParameters.Param[0] = new EncoderParameter(Encoder.SaveFlag, 20L);
				if (encoderParameter != null)
				{
					encoderParameter.Dispose();
					encoderParameter = null;
				}
				this.m_firstImage.SaveAdd(this.m_encoderParameters);
			}
		}

		protected static void SetGraphicsProperties(System.Drawing.Graphics graphics)
		{
			graphics.CompositingMode = CompositingMode.SourceOver;
			graphics.PageUnit = GraphicsUnit.Pixel;
			graphics.PixelOffsetMode = PixelOffsetMode.Default;
			graphics.SmoothingMode = SmoothingMode.Default;
			graphics.TextRenderingHint = TextRenderingHint.SystemDefault;
		}

		private static ImageCodecInfo GetEncoderInfo(string mimeType)
		{
			if (Graphics.m_encoders == null)
			{
				return null;
			}
			for (int i = 0; i < Graphics.m_encoders.Length; i++)
			{
				if (Graphics.m_encoders[i].MimeType == mimeType)
				{
					return Graphics.m_encoders[i];
				}
			}
			return null;
		}

		private static ImageCodecInfo[] GetGdiImageEncoders()
		{
			ImageCodecInfo[] array = null;
			if (Graphics.m_encoders == null)
			{
				return ImageCodecInfo.GetImageEncoders();
			}
			return Graphics.m_encoders;
		}
	}
}
