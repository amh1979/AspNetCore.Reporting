using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.Rendering.HPBProcessing;
using AspNetCore.ReportingServices.Rendering.RichText;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace AspNetCore.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class ImageWriter : WriterBase
	{
		internal const char StreamNameSeparator = '_';

		private Graphics m_graphics;

		private Dictionary<string, System.Drawing.Image> m_cachedImages = new Dictionary<string, System.Drawing.Image>();

		internal PaginationSettings.FormatEncoding OutputFormat;

		private RectangleF MetafileRectangle = RectangleF.Empty;

		private Dictionary<string, Pen> m_pens = new Dictionary<string, Pen>();

		private Dictionary<string, Brush> m_brushes = new Dictionary<string, Brush>();

		private System.Drawing.Rectangle m_bodyRect = System.Drawing.Rectangle.Empty;

		private AspNetCore.ReportingServices.Rendering.RichText.Win32.POINT m_prevViewportOrg;

		private int m_dpiX;

		private int m_dpiY;

		private int m_measureImageDpiX;

		private int m_measureImageDpiY;

		private int DEFAULT_RESOLUTION_X = 96;

		private int DYNAMIC_IMAGE_MIN_RESOLUTION_X = 300;

		private int DEFAULT_RESOLUTION_Y = 96;

		private int DYNAMIC_IMAGE_MIN_RESOLUTION_Y = 300;

		internal bool IsEmf
		{
			get
			{
				if (this.OutputFormat != PaginationSettings.FormatEncoding.EMFPLUS)
				{
					return this.OutputFormat == PaginationSettings.FormatEncoding.EMF;
				}
				return true;
			}
		}

		internal Stream OutputStream
		{
			set
			{
				base.m_outputStream = value;
			}
		}

		internal ImageWriter(Renderer renderer, Stream stream, bool disposeRenderer, CreateAndRegisterStream createAndRegisterStream, int measureImageDpiX, int measureImageDpiY)
			: base(renderer, stream, disposeRenderer, createAndRegisterStream)
		{
			this.m_measureImageDpiX = measureImageDpiX;
			this.m_measureImageDpiY = measureImageDpiY;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.m_cachedImages != null)
				{
					foreach (string key in this.m_cachedImages.Keys)
					{
						this.m_cachedImages[key].Dispose();
					}
					this.m_cachedImages = null;
				}
				if (this.m_pens != null)
				{
					foreach (string key2 in this.m_pens.Keys)
					{
						this.m_pens[key2].Dispose();
					}
					this.m_pens = null;
				}
				if (this.m_brushes != null)
				{
					foreach (string key3 in this.m_brushes.Keys)
					{
						this.m_brushes[key3].Dispose();
					}
					this.m_brushes = null;
				}
				if (this.m_graphics != null)
				{
					this.m_graphics.Dispose();
					this.m_graphics = null;
				}
			}
			base.Dispose(disposing);
		}

		~ImageWriter()
		{
			this.Dispose(false);
		}

		internal override void BeginReport(int dpiX, int dpiY)
		{
			this.m_dpiX = dpiX;
			this.m_dpiY = dpiY;
			if (!this.IsEmf)
			{
				this.m_graphics = new Graphics((float)dpiX, (float)dpiY);
			}
			else
			{
				this.m_graphics = new MetafileGraphics((float)dpiX, (float)dpiY);
			}
			base.m_commonGraphics = this.m_graphics;
		}

		internal override void BeginPage(float pageWidth, float pageHeight)
		{
			if (!this.IsEmf)
			{
				this.m_graphics.NewPage(pageWidth, pageHeight, base.m_commonGraphics.DpiX, base.m_commonGraphics.DpiY);
			}
			else
			{
				if (this.MetafileRectangle == RectangleF.Empty)
				{
					this.CalculateMetafileRectangle(pageWidth, pageHeight);
				}
				((MetafileGraphics)this.m_graphics).NewPage(base.m_outputStream, this.OutputFormat, this.MetafileRectangle, base.m_commonGraphics.DpiX, base.m_commonGraphics.DpiY);
			}
		}

		internal override void BeginPageSection(RectangleF bounds)
		{
			base.BeginPageSection(bounds);
			int dpiX = base.m_commonGraphics.DpiX;
			int dpiY = base.m_commonGraphics.DpiY;
			this.m_bodyRect = new System.Drawing.Rectangle(SharedRenderer.ConvertToPixels(bounds.X, (float)dpiX), SharedRenderer.ConvertToPixels(bounds.Y, (float)dpiY), SharedRenderer.ConvertToPixels(bounds.Width + this.HalfPixelWidthX, (float)dpiX), SharedRenderer.ConvertToPixels(bounds.Height + this.HalfPixelWidthY, (float)dpiY));
			this.m_graphics.ResetClipAndTransform(new RectangleF(bounds.Left, bounds.Top, bounds.Width + this.HalfPixelWidthX, bounds.Height + this.HalfPixelWidthY));
		}

		internal override RectangleF CalculateColumnBounds(RPLReportSection reportSection, RPLPageLayout pageLayout, RPLItemMeasurement column, int columnNumber, float top, float columnHeight, float columnWidth)
		{
			return HardPageBreakShared.CalculateColumnBounds(reportSection, pageLayout, columnNumber, top, columnHeight);
		}

		internal override RectangleF CalculateHeaderBounds(RPLReportSection section, RPLPageLayout pageLayout, float top, float width)
		{
			return HardPageBreakShared.CalculateHeaderBounds(section, pageLayout, top, width);
		}

		internal override RectangleF CalculateFooterBounds(RPLReportSection section, RPLPageLayout pageLayout, float top, float width)
		{
			return HardPageBreakShared.CalculateFooterBounds(section, pageLayout, top, width);
		}

		internal override void DrawBackgroundImage(RPLImageData imageData, RPLFormat.BackgroundRepeatTypes repeat, PointF start, RectangleF position)
		{
			System.Drawing.Image image = default(System.Drawing.Image);
			bool image2 = this.GetImage(imageData.ImageName, imageData.ImageData, imageData.ImageDataOffset, false, out image);
			if (image != null)
			{
				RectangleF destination = default(RectangleF);
				RectangleF source = default(RectangleF);
				if (repeat == RPLFormat.BackgroundRepeatTypes.Clip)
				{
					if (SharedRenderer.CalculateImageClippedUnscaledBounds((WriterBase)this, position, image.Width, image.Height, start.X, start.Y, (int?)this.m_measureImageDpiX, (int?)this.m_measureImageDpiY, out destination, out source))
					{
						this.m_graphics.DrawImage(image, destination, source);
					}
				}
				else
				{
					float num = SharedRenderer.ConvertToMillimeters(image.Width, (float)this.m_measureImageDpiX);
					float num2 = SharedRenderer.ConvertToMillimeters(image.Height, (float)this.m_measureImageDpiY);
					float num3 = position.Width;
					if (repeat == RPLFormat.BackgroundRepeatTypes.RepeatY)
					{
						num3 = num;
					}
					float num4 = position.Height;
					if (repeat == RPLFormat.BackgroundRepeatTypes.RepeatX)
					{
						num4 = num2;
					}
					for (float num5 = start.X; num5 < num3; num5 += num)
					{
						for (float num6 = start.Y; num6 < num4; num6 += num2)
						{
							if (SharedRenderer.CalculateImageClippedUnscaledBounds((WriterBase)this, position, image.Width, image.Height, num5, num6, (int?)this.m_measureImageDpiX, (int?)this.m_measureImageDpiY, out destination, out source))
							{
								this.m_graphics.DrawImage(image, destination, source);
							}
						}
					}
				}
				if (!image2)
				{
					image.Dispose();
					image = null;
				}
			}
		}

		internal override void DrawLine(Color color, float size, RPLFormat.BorderStyles style, float x1, float y1, float x2, float y2)
		{
			this.m_graphics.DrawLine(GDIPen.GetPen(this.m_pens, color, (float)this.ConvertToPixels(size), style), x1, y1, x2, y2);
		}

		internal void GetDefaultImage(out System.Drawing.Image gdiImage)
		{
			string key = "__int__InvalidImage";
			if (!this.m_cachedImages.TryGetValue(key, out gdiImage))
			{
				Bitmap bitmap = Renderer.ImageResources["InvalidImage"];
				Bitmap bitmap2 = null;
				lock (bitmap)
				{
					using (MemoryStream stream = new MemoryStream())
					{
						bitmap.Save(stream, bitmap.RawFormat);
						bitmap2 = new Bitmap(stream);
					}
				}
				bitmap2.SetResolution((float)base.m_commonGraphics.DpiX, (float)base.m_commonGraphics.DpiY);
				gdiImage = bitmap2;
				this.m_cachedImages.Add(key, gdiImage);
			}
		}

		internal override void DrawDynamicImage(string imageName, Stream imageStream, long imageDataOffset, RectangleF position)
		{
			System.Drawing.Image image = default(System.Drawing.Image);
			bool flag = this.GetImage(imageName, imageStream, imageDataOffset, true, out image);
			if (image == null)
			{
				this.GetDefaultImage(out image);
				flag = true;
			}
			RectangleF source = new RectangleF(0f, 0f, (float)image.Width, (float)image.Height);
			this.m_graphics.DrawImage(image, position, source, false);
			if (!flag)
			{
				image.Dispose();
				image = null;
			}
		}

		internal override void DrawImage(RectangleF position, RPLImage image, RPLImageProps instanceProperties, RPLImagePropsDef definitionProperties)
		{
			RPLImageData image2 = instanceProperties.Image;
			System.Drawing.Image image3 = default(System.Drawing.Image);
			bool flag = this.GetImage(image2.ImageName, image2.ImageData, image2.ImageDataOffset, false, out image3);
			RPLFormat.Sizings sizing = definitionProperties.Sizing;
			if (image3 == null)
			{
				this.GetDefaultImage(out image3);
				flag = true;
				sizing = RPLFormat.Sizings.Clip;
			}
			GDIImageProps gDIImageProps = new GDIImageProps(image3);
			RectangleF destination = default(RectangleF);
			RectangleF source = default(RectangleF);
			SharedRenderer.CalculateImageRectangle(position, gDIImageProps.Width, gDIImageProps.Height, (float)this.m_measureImageDpiX, (float)this.m_measureImageDpiY, sizing, out destination, out source);
			this.m_graphics.DrawImage(image3, destination, source);
			if (!flag)
			{
				image3.Dispose();
				image3 = null;
			}
		}

		internal override void DrawRectangle(Color color, float size, RPLFormat.BorderStyles style, RectangleF rectangle)
		{
			this.m_graphics.DrawRectangle(GDIPen.GetPen(this.m_pens, color, (float)this.ConvertToPixels(size), style), rectangle);
		}

		internal override void DrawTextRun(Win32DCSafeHandle hdc, FontCache fontCache, ReportTextBox textBox, AspNetCore.ReportingServices.Rendering.RichText.TextRun run, TypeCode typeCode, RPLFormat.TextAlignments textAlign, RPLFormat.VerticalAlignments verticalAlign, RPLFormat.WritingModes writingMode, RPLFormat.Directions direction, Point pointPosition, System.Drawing.Rectangle layoutRectangle, int lineHeight, int baselineY)
		{
			if (!string.IsNullOrEmpty(run.Text))
			{
				int x;
				int baselineY2;
				switch (writingMode)
				{
				case RPLFormat.WritingModes.Horizontal:
					x = layoutRectangle.X + pointPosition.X;
					baselineY2 = layoutRectangle.Y + baselineY;
					break;
				case RPLFormat.WritingModes.Vertical:
					x = layoutRectangle.X + (layoutRectangle.Width - baselineY);
					baselineY2 = layoutRectangle.Y + pointPosition.X;
					break;
				case RPLFormat.WritingModes.Rotate270:
					x = layoutRectangle.X + baselineY;
					baselineY2 = layoutRectangle.Y + layoutRectangle.Height - pointPosition.X;
					break;
				default:
					throw new NotSupportedException();
				}
				Underline underline = null;
				if (run.UnderlineHeight > 0)
				{
					underline = new Underline(run, hdc, fontCache, layoutRectangle, pointPosition.X, baselineY, writingMode);
				}
				if (!this.IsEmf)
				{
					AspNetCore.ReportingServices.Rendering.RichText.TextBox.DrawTextRun(run, hdc, fontCache, x, baselineY2, underline);
				}
				else
				{
					AspNetCore.ReportingServices.Rendering.RichText.TextBox.ExtDrawTextRun(run, hdc, fontCache, x, baselineY2, underline);
				}
			}
		}

		internal override void EndPage()
		{
			this.m_graphics.ReleaseCachedHdc(true);
			this.m_graphics.Save(base.m_outputStream, this.OutputFormat);
		}

		internal override void EndReport()
		{
			this.m_graphics.EndReport(this.OutputFormat);
			base.m_outputStream.Flush();
		}

		internal override void FillPolygon(Color color, PointF[] polygon)
		{
			this.m_graphics.FillPolygon(GDIBrush.GetBrush(this.m_brushes, color), polygon);
		}

		internal override void FillRectangle(Color color, RectangleF rectangle)
		{
			this.m_graphics.FillRectangle(GDIBrush.GetBrush(this.m_brushes, color), rectangle);
		}

		private void CalculateMetafileRectangle(float pageWidth, float pageHeight)
		{
			if (this.IsEmf)
			{
				Win32DCSafeHandle hdc = this.m_graphics.GetHdc();
				try
				{
					int deviceCaps = AspNetCore.ReportingServices.Rendering.RichText.Win32.GetDeviceCaps(hdc, 4);
					int deviceCaps2 = AspNetCore.ReportingServices.Rendering.RichText.Win32.GetDeviceCaps(hdc, 6);
					int deviceCaps3 = AspNetCore.ReportingServices.Rendering.RichText.Win32.GetDeviceCaps(hdc, 8);
					int deviceCaps4 = AspNetCore.ReportingServices.Rendering.RichText.Win32.GetDeviceCaps(hdc, 10);
					double num = (double)SharedRenderer.ConvertToPixels(pageWidth, (float)this.m_graphics.DpiX);
					double num2 = (double)SharedRenderer.ConvertToPixels(pageHeight, (float)this.m_graphics.DpiY);
					float width = (float)(num * (double)deviceCaps * 100.0) / (float)deviceCaps3;
					float height = (float)(num2 * (double)deviceCaps2 * 100.0) / (float)deviceCaps4;
					this.MetafileRectangle = new RectangleF(0f, 0f, width, height);
				}
				finally
				{
					this.m_graphics.ReleaseHdc();
				}
			}
		}

		private bool GetImage(string imageName, byte[] imageBytes, long imageDataOffset, bool dynamicImage, out System.Drawing.Image image)
		{
			image = null;
			if (dynamicImage || string.IsNullOrEmpty(imageName) || !this.m_cachedImages.TryGetValue(imageName, out image))
			{
				if (!SharedRenderer.GetImage(base.m_renderer.RplReport, ref imageBytes, imageDataOffset))
				{
					return false;
				}
				try
				{
					image = System.Drawing.Image.FromStream(new MemoryStream(imageBytes));
				}
				catch
				{
					return false;
				}
				this.AddImageToCache(image, dynamicImage, imageName);
			}
			if (!dynamicImage)
			{
				return !string.IsNullOrEmpty(imageName);
			}
			return false;
		}

		private bool GetImage(string imageName, Stream imageStream, long imageDataOffset, bool dynamicImage, out System.Drawing.Image image)
		{
			image = null;
			if (dynamicImage || string.IsNullOrEmpty(imageName) || !this.m_cachedImages.TryGetValue(imageName, out image))
			{
				if (imageStream == null)
				{
					imageStream = SharedRenderer.GetEmbeddedImageStream(base.m_renderer.RplReport, imageDataOffset, base.CreateAndRegisterStream, imageName);
					if (imageStream == null)
					{
						return false;
					}
				}
				if (imageStream.Position != 0 && imageStream.CanSeek)
				{
					imageStream.Position = 0L;
				}
				try
				{
					image = System.Drawing.Image.FromStream(imageStream);
				}
				catch
				{
					return false;
				}
				this.AddImageToCache(image, dynamicImage, imageName);
			}
			if (!dynamicImage)
			{
				return !string.IsNullOrEmpty(imageName);
			}
			return false;
		}

		private void AddImageToCache(System.Drawing.Image image, bool dynamicImage, string imageName)
		{
			Bitmap bitmap = image as Bitmap;
			if (bitmap != null)
			{
				this.SetResolution(bitmap, dynamicImage);
			}
			if (!dynamicImage && !string.IsNullOrEmpty(imageName))
			{
				this.m_cachedImages.Add(imageName, image);
			}
		}

		private void SetResolution(Bitmap bitmap, bool dynamicImage)
		{
			int num = this.m_dpiX;
			int num2 = this.m_dpiY;
			if (dynamicImage)
			{
				if (this.DEFAULT_RESOLUTION_X == num)
				{
					num = this.DYNAMIC_IMAGE_MIN_RESOLUTION_X;
				}
				if (this.DEFAULT_RESOLUTION_Y == num2)
				{
					num2 = this.DYNAMIC_IMAGE_MIN_RESOLUTION_Y;
				}
			}
			bitmap.SetResolution((float)num, (float)num2);
		}

		internal override void ClipTextboxRectangle(Win32DCSafeHandle hdc, RectangleF position)
		{
			if (this.m_bodyRect.X != 0 || this.m_bodyRect.Y != 0)
			{
				if (!AspNetCore.ReportingServices.Rendering.RichText.Win32.GetViewportOrgEx(hdc, out this.m_prevViewportOrg))
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					string message = string.Format(CultureInfo.InvariantCulture, ImageRendererRes.Win32ErrorInfo, "GetViewportOrgEx", lastWin32Error);
					throw new Exception(message);
				}
				if (!AspNetCore.ReportingServices.Rendering.RichText.Win32.SetViewportOrgEx(hdc, this.m_bodyRect.X, this.m_bodyRect.Y, Win32ObjectSafeHandle.Zero))
				{
					int lastWin32Error2 = Marshal.GetLastWin32Error();
					string message2 = string.Format(CultureInfo.InvariantCulture, ImageRendererRes.Win32ErrorInfo, "SetViewportOrgEx", lastWin32Error2);
					throw new Exception(message2);
				}
			}
			System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(SharedRenderer.ConvertToPixels(position.X, (float)base.m_commonGraphics.DpiX), SharedRenderer.ConvertToPixels(position.Y, (float)base.m_commonGraphics.DpiY), SharedRenderer.ConvertToPixels(position.Width, (float)base.m_commonGraphics.DpiX), SharedRenderer.ConvertToPixels(position.Height, (float)base.m_commonGraphics.DpiY));
			if (position.X < 0.0)
			{
				rectangle.Width += rectangle.X;
				rectangle.X = 0;
			}
			if (position.Y < 0.0)
			{
				rectangle.Height += rectangle.Y;
				rectangle.Y = 0;
			}
			rectangle.X += this.m_bodyRect.X;
			rectangle.Y += this.m_bodyRect.Y;
			if (rectangle.Right > this.m_bodyRect.Right)
			{
				rectangle.Width = this.m_bodyRect.Right - rectangle.Left;
			}
			if (rectangle.Bottom > this.m_bodyRect.Bottom)
			{
				rectangle.Height = this.m_bodyRect.Bottom - rectangle.Top;
			}
			Win32ObjectSafeHandle win32ObjectSafeHandle = AspNetCore.ReportingServices.Rendering.RichText.Win32.CreateRectRgn(rectangle.X, rectangle.Y, rectangle.Right, rectangle.Bottom);
			if (!win32ObjectSafeHandle.IsInvalid)
			{
				try
				{
					if (AspNetCore.ReportingServices.Rendering.RichText.Win32.SelectClipRgn(hdc, win32ObjectSafeHandle) == 0)
					{
						int lastWin32Error3 = Marshal.GetLastWin32Error();
						string message3 = string.Format(CultureInfo.InvariantCulture, ImageRendererRes.Win32ErrorInfo, "SelectClipRgn", lastWin32Error3);
						throw new Exception(message3);
					}
				}
				finally
				{
					win32ObjectSafeHandle.Close();
				}
			}
		}

		internal override void UnClipTextboxRectangle(Win32DCSafeHandle hdc)
		{
			if (AspNetCore.ReportingServices.Rendering.RichText.Win32.SelectClipRgn(hdc, Win32ObjectSafeHandle.Zero) == 0)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				string message = string.Format(CultureInfo.InvariantCulture, ImageRendererRes.Win32ErrorInfo, "SelectClipRgn", lastWin32Error);
				throw new Exception(message);
			}
			if (this.m_bodyRect.X == 0 && this.m_bodyRect.Y == 0)
			{
				return;
			}
			if (AspNetCore.ReportingServices.Rendering.RichText.Win32.SetViewportOrgEx(hdc, this.m_prevViewportOrg.x, this.m_prevViewportOrg.y, Win32ObjectSafeHandle.Zero))
			{
				return;
			}
			int lastWin32Error2 = Marshal.GetLastWin32Error();
			string message2 = string.Format(CultureInfo.InvariantCulture, ImageRendererRes.Win32ErrorInfo, "SetViewportOrgEx", lastWin32Error2);
			throw new Exception(message2);
		}

		internal static void GetScreenDpi(out int dpiX, out int dpiY)
		{
			using (Bitmap image = new Bitmap(2, 2))
			{
				using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(image))
				{
					IntPtr hdc = graphics.GetHdc();
					try
					{
						dpiX = AspNetCore.ReportingServices.Rendering.RichText.Win32.GetDeviceCaps(hdc, 88);
						dpiY = AspNetCore.ReportingServices.Rendering.RichText.Win32.GetDeviceCaps(hdc, 90);
					}
					finally
					{
						graphics.ReleaseHdc(hdc);
					}
				}
			}
		}
	}
}
