using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class BaseGaugeImageInstance : BaseInstance
	{
		protected BaseGaugeImage m_defObject;

		private Image.SourceType? m_source;

		private ReportColor m_transparentColor;

		private ImageDataHandler m_imageDataHandler;

		public Image.SourceType Source
		{
			get
			{
				if (!this.m_source.HasValue)
				{
					this.m_source = this.m_defObject.BaseGaugeImageDef.EvaluateSource(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_source.Value;
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
					this.m_transparentColor = new ReportColor(this.m_defObject.BaseGaugeImageDef.EvaluateTransparentColor(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext));
				}
				return this.m_transparentColor;
			}
		}

		public byte[] ImageData
		{
			get
			{
				return this.ImageHandler.ImageData;
			}
		}

		private ImageDataHandler ImageHandler
		{
			get
			{
				if (this.m_imageDataHandler == null || this.Source != this.m_imageDataHandler.Source)
				{
					this.m_imageDataHandler = ImageDataHandlerFactory.Create(this.m_defObject.GaugePanelDef, this.m_defObject);
				}
				return this.m_imageDataHandler;
			}
		}

		internal BaseGaugeImageInstance(BaseGaugeImage defObject)
			: base(defObject.GaugePanelDef)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			this.m_source = null;
			this.m_transparentColor = null;
			if (this.m_imageDataHandler != null)
			{
				this.m_imageDataHandler.ClearCache();
			}
		}
	}
}
