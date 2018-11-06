using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimBackgroundImageInstance : BackgroundImageInstance
	{
		private readonly AspNetCore.ReportingServices.ReportRendering.BackgroundImage m_renderImage;

		private readonly BackgroundRepeatTypes m_backgroundRepeat;

		private readonly BackgroundImage m_backgroundImageDef;

		public override byte[] ImageData
		{
			get
			{
				return this.m_renderImage.ImageData;
			}
		}

		public override string StreamName
		{
			get
			{
				return this.m_renderImage.StreamName;
			}
		}

		public override string MIMEType
		{
			get
			{
				return this.m_renderImage.MIMEType;
			}
		}

		public override BackgroundRepeatTypes BackgroundRepeat
		{
			get
			{
				return this.m_backgroundRepeat;
			}
		}

		public override Positions Position
		{
			get
			{
				return this.m_backgroundImageDef.Position.Value;
			}
		}

		public override ReportColor TransparentColor
		{
			get
			{
				return this.m_backgroundImageDef.TransparentColor.Value;
			}
		}

		internal ShimBackgroundImageInstance(BackgroundImage backgroundImageDef, AspNetCore.ReportingServices.ReportRendering.BackgroundImage renderImage, string backgroundRepeat)
			: base(null)
		{
			this.m_backgroundImageDef = backgroundImageDef;
			this.m_renderImage = renderImage;
			this.m_backgroundRepeat = StyleTranslator.TranslateBackgroundRepeat(backgroundRepeat, null, this.m_backgroundImageDef.StyleDef.IsDynamicImageStyle);
		}

		protected override void ResetInstanceCache()
		{
		}
	}
}
