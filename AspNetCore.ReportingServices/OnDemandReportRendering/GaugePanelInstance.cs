using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System.IO;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugePanelInstance : DynamicImageInstance, IDynamicImageInstance
	{
		private GaugeAntiAliasings? m_antiAliasing;

		private bool? m_autoLayout;

		private double? m_shadowIntensity;

		private TextAntiAliasingQualities? m_textAntiAliasingQuality;

		public GaugeAntiAliasings AntiAliasing
		{
			get
			{
				if (!this.m_antiAliasing.HasValue)
				{
					this.m_antiAliasing = ((AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel)base.m_reportElementDef.ReportItemDef).EvaluateAntiAliasing(this, base.m_reportElementDef.RenderingContext.OdpContext);
				}
				return this.m_antiAliasing.Value;
			}
		}

		public bool AutoLayout
		{
			get
			{
				if (!this.m_autoLayout.HasValue)
				{
					this.m_autoLayout = ((AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel)base.m_reportElementDef.ReportItemDef).EvaluateAutoLayout(this, base.m_reportElementDef.RenderingContext.OdpContext);
				}
				return this.m_autoLayout.Value;
			}
		}

		public double ShadowIntensity
		{
			get
			{
				if (!this.m_shadowIntensity.HasValue)
				{
					this.m_shadowIntensity = ((AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel)base.m_reportElementDef.ReportItemDef).EvaluateShadowIntensity(this, base.m_reportElementDef.RenderingContext.OdpContext);
				}
				return this.m_shadowIntensity.Value;
			}
		}

		public TextAntiAliasingQualities TextAntiAliasingQuality
		{
			get
			{
				if (!this.m_textAntiAliasingQuality.HasValue)
				{
					this.m_textAntiAliasingQuality = ((AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel)base.m_reportElementDef.ReportItemDef).EvaluateTextAntiAliasingQuality(this, base.m_reportElementDef.RenderingContext.OdpContext);
				}
				return this.m_textAntiAliasingQuality.Value;
			}
		}

		internal GaugePanelInstance(GaugePanel reportItemDef)
			: base(reportItemDef)
		{
		}

		protected override void GetImage(ImageType type, out ActionInfoWithDynamicImageMapCollection actionImageMaps, out Stream image)
		{
			using (IGaugeMapper gaugeMapper = GaugeMapperFactory.CreateGaugeMapperInstance((GaugePanel)base.m_reportElementDef, base.GetDefaultFontFamily()))
			{
				gaugeMapper.DpiX = base.m_dpiX;
				gaugeMapper.DpiY = base.m_dpiY;
				gaugeMapper.WidthOverride = base.m_widthOverride;
				gaugeMapper.HeightOverride = base.m_heightOverride;
				gaugeMapper.RenderGaugePanel();
				image = gaugeMapper.GetImage(type);
				actionImageMaps = gaugeMapper.GetImageMaps();
			}
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_antiAliasing = null;
			this.m_autoLayout = null;
			this.m_shadowIntensity = null;
			this.m_textAntiAliasingQuality = null;
		}

		public Stream GetCoreXml()
		{
			Stream stream = null;
			using (IGaugeMapper gaugeMapper = GaugeMapperFactory.CreateGaugeMapperInstance((GaugePanel)base.m_reportElementDef, base.GetDefaultFontFamily()))
			{
				gaugeMapper.DpiX = base.m_dpiX;
				gaugeMapper.DpiY = base.m_dpiY;
				gaugeMapper.WidthOverride = base.m_widthOverride;
				gaugeMapper.HeightOverride = base.m_heightOverride;
				gaugeMapper.RenderGaugePanel();
				return gaugeMapper.GetCoreXml();
			}
		}
	}
}
