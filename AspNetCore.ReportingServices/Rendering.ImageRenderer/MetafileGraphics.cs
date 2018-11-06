using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.Rendering.HPBProcessing;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class MetafileGraphics : Graphics
	{
		private static object m_syncRoot = new object();

		private Metafile m_metafile;

		internal MetafileGraphics(float dpiX, float dpiY)
			: base(dpiX, dpiY)
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.m_metafile != null)
			{
				this.ExecuteSync(delegate
				{
					this.m_metafile.Dispose();
				});
				this.m_metafile = null;
			}
			base.Dispose(disposing);
		}

		internal override void CacheHdc(bool createNewHdc)
		{
			base.CacheHdc(true);
		}

		internal override void ReleaseCachedHdc(bool releaseHdc)
		{
			base.ReleaseCachedHdc(true);
		}

		internal override void ExecuteSync(SynchronizedOperation synchronizedOperation)
		{
			lock (MetafileGraphics.m_syncRoot)
			{
				synchronizedOperation();
			}
		}

		internal override void Save(Stream outputStream, PaginationSettings.FormatEncoding outputFormat)
		{
			if (this.m_metafile != null)
			{
				this.ExecuteSync(delegate
				{
					this.m_metafile.Dispose();
				});
				this.m_metafile = null;
			}
			outputStream.Flush();
			IRenderStream renderStream = outputStream as IRenderStream;
			if (renderStream != null)
			{
				renderStream.Finish();
			}
		}

		internal void NewPage(Stream stream, PaginationSettings.FormatEncoding outputFormat, RectangleF metafileRectangle, int dpiX, int dpiY)
		{
			if (base.m_graphicsBase != null)
			{
				this.ReleaseCachedHdc(true);
				base.m_graphicsBase.Dispose();
				base.m_graphicsBase = null;
			}
			System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(base.m_imageBase);
			IntPtr hdc = graphics.GetHdc();
			EmfType emfType = EmfType.EmfPlusOnly;
			if (outputFormat == PaginationSettings.FormatEncoding.EMF)
			{
				emfType = EmfType.EmfPlusDual;
			}
			try
			{
				this.ExecuteSync(delegate
				{
					this.m_metafile = new Metafile(stream, hdc, metafileRectangle, MetafileFrameUnit.GdiCompatible, emfType);
					base.m_graphicsBase = System.Drawing.Graphics.FromImage(this.m_metafile);
					Graphics.SetGraphicsProperties(base.m_graphicsBase);
				});
			}
			finally
			{
				if (hdc != IntPtr.Zero)
				{
					graphics.ReleaseHdc(hdc);
				}
				if (graphics != null)
				{
					graphics.Dispose();
					graphics = null;
				}
			}
		}
	}
}
