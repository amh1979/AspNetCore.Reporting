using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapMarkerImageInstance : BaseInstance
	{
		private MapMarkerImage m_defObject;

		private Image.SourceType? m_source;

		private ReportColor m_transparentColor;

		private MapResizeMode? m_resizeMode;

		private ImageDataHandler m_imageDataHandler;

		public Image.SourceType Source
		{
			get
			{
				if (!this.m_source.HasValue)
				{
					this.m_source = this.m_defObject.MapMarkerImageDef.EvaluateSource(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_source.Value;
			}
		}

		public byte[] ImageData
		{
			get
			{
				return this.ImageHandler.ImageData;
			}
		}

		public string MIMEType
		{
			get
			{
				return this.ImageHandler.MIMEType;
			}
		}

		public ReportColor TransparentColor
		{
			get
			{
				if (this.m_transparentColor == null)
				{
					this.m_transparentColor = new ReportColor(this.m_defObject.MapMarkerImageDef.EvaluateTransparentColor(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return this.m_transparentColor;
			}
		}

		public MapResizeMode ResizeMode
		{
			get
			{
				if (!this.m_resizeMode.HasValue)
				{
					this.m_resizeMode = this.m_defObject.MapMarkerImageDef.EvaluateResizeMode(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_resizeMode.Value;
			}
		}

		private ImageDataHandler ImageHandler
		{
			get
			{
				if (this.m_imageDataHandler == null || this.Source != this.m_imageDataHandler.Source)
				{
					this.m_imageDataHandler = ImageDataHandlerFactory.Create(this.m_defObject.MapDef, this.m_defObject);
				}
				return this.m_imageDataHandler;
			}
		}

		internal MapMarkerImageInstance(MapMarkerImage defObject)
			: base(defObject.MapDef.ReportScope)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			this.m_source = null;
			this.m_transparentColor = null;
			this.m_resizeMode = null;
			if (this.m_imageDataHandler != null)
			{
				this.m_imageDataHandler.ClearCache();
			}
		}
	}
}
