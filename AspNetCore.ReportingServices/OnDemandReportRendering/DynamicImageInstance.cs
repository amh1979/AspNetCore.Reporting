using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class DynamicImageInstance : DataRegionInstance
	{
		internal enum ImageType
		{
			PNG,
			EMF
		}

		protected float m_dpiX = 96f;

		protected float m_dpiY = 96f;

		protected double? m_widthOverride = null;

		protected double? m_heightOverride = null;

		protected virtual int WidthInPixels
		{
			get
			{
				return MappingHelper.ToIntPixels(((ReportItem)base.m_reportElementDef).Width, this.m_dpiX);
			}
		}

		protected virtual int HeightInPixels
		{
			get
			{
				return MappingHelper.ToIntPixels(((ReportItem)base.m_reportElementDef).Height, this.m_dpiX);
			}
		}

		internal DynamicImageInstance(DataRegion reportItemDef)
			: base(reportItemDef)
		{
		}

		public virtual void SetDpi(int xDpi, int yDpi)
		{
			this.m_dpiX = (float)xDpi;
			this.m_dpiY = (float)yDpi;
		}

		public void SetSize(double width, double height)
		{
			this.m_widthOverride = width;
			this.m_heightOverride = height;
		}

		public Stream GetImage()
		{
			bool flag = default(bool);
			return this.GetImage(ImageType.PNG, out flag);
		}

		public Stream GetImage(ImageType type)
		{
			bool flag = default(bool);
			return this.GetImage(type, out flag);
		}

		public Stream GetImage(out ActionInfoWithDynamicImageMapCollection actionImageMaps)
		{
			return this.GetImage(ImageType.PNG, out actionImageMaps);
		}

		public virtual Stream GetImage(ImageType type, out ActionInfoWithDynamicImageMapCollection actionImageMaps)
		{
			try
			{
				Stream result = default(Stream);
				this.GetImage(type, out actionImageMaps, out result);
				return result;
			}
			catch (Exception exception)
			{
				actionImageMaps = null;
				return this.CreateExceptionImage(exception);
			}
		}

		protected virtual Stream GetImage(ImageType type, out bool hasImageMap)
		{
			ActionInfoWithDynamicImageMapCollection actionInfoWithDynamicImageMapCollection = default(ActionInfoWithDynamicImageMapCollection);
			Stream image = this.GetImage(type, out actionInfoWithDynamicImageMapCollection);
			hasImageMap = (actionInfoWithDynamicImageMapCollection != null);
			return image;
		}

		protected MemoryStream CreateExceptionImage(Exception exception)
		{
			return DynamicImageInstance.CreateExceptionImage(exception, this.WidthInPixels, this.HeightInPixels, this.m_dpiX, this.m_dpiY);
		}

		internal static MemoryStream CreateExceptionImage(Exception exception, int width, int height, float dpiX, float dpiY)
		{
			Bitmap bitmap = null;
			Graphics graphics = null;
			Brush brush = null;
			Brush brush2 = null;
			Pen pen = null;
			Pen pen2 = null;
			Font font = null;
			MemoryStream memoryStream = new MemoryStream();
			try
			{
				bitmap = new Bitmap(width, height);
				bitmap.SetResolution(dpiX, dpiY);
				graphics = Graphics.FromImage(bitmap);
				brush = new SolidBrush(Color.White);
				graphics.FillRectangle(brush, 0, 0, width, height);
				float num = (float)MappingHelper.ToPixels(new ReportSize("1pt"), dpiX);
				float num2 = (float)MappingHelper.ToPixels(new ReportSize("1pt"), dpiY);
				pen = new Pen(Color.Black, num);
				pen2 = new Pen(Color.Black, num2);
				graphics.DrawLine(pen, num, num2, (float)width - num, num2);
				graphics.DrawLine(pen2, (float)width - num, num2, (float)width - num, (float)height - num2);
				graphics.DrawLine(pen, (float)width - num, (float)height - num2, num, (float)height - num2);
				graphics.DrawLine(pen2, num, (float)height - num2, num, num2);
				brush2 = new SolidBrush(Color.Black);
				font = MappingHelper.GetDefaultFont();
				graphics.DrawString(DynamicImageInstance.GetInnerMostException(exception).Message, font, brush2, new RectangleF(num, num2, (float)width - num, (float)height - num2));
				bitmap.Save(memoryStream, ImageFormat.Png);
				return memoryStream;
			}
			catch (Exception ex)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex))
				{
					throw;
				}
				Global.Tracer.Trace(TraceLevel.Verbose, ex.Message);
				return null;
			}
			finally
			{
				if (brush != null)
				{
					brush.Dispose();
					brush = null;
				}
				if (pen != null)
				{
					pen.Dispose();
					pen = null;
				}
				if (pen2 != null)
				{
					pen2.Dispose();
					pen2 = null;
				}
				if (brush2 != null)
				{
					brush2.Dispose();
					brush2 = null;
				}
				if (font != null)
				{
					font.Dispose();
					font = null;
				}
				if (graphics != null)
				{
					graphics.Dispose();
					graphics = null;
				}
				if (bitmap != null)
				{
					bitmap.Dispose();
					bitmap = null;
				}
			}
		}

		protected abstract void GetImage(ImageType type, out ActionInfoWithDynamicImageMapCollection actionImageMaps, out Stream image);

		private static Exception GetInnerMostException(Exception exception)
		{
			Exception ex = exception;
			while (ex.InnerException != null)
			{
				ex = ex.InnerException;
			}
			return ex;
		}
	}
}
