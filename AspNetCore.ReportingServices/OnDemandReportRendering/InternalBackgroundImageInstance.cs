using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalBackgroundImageInstance : BackgroundImageInstance
	{
		private bool m_backgroundRepeatEvaluated;

		private BackgroundRepeatTypes m_backgroundRepeat = Style.DefaultEnumBackgroundRepeatType;

		private bool m_positionEvaluated;

		private Positions m_position;

		private bool m_transparentColorEvaluated;

		private ReportColor m_transparentColor;

		private readonly ImageDataHandler m_imageDataHandler;

		private readonly BackgroundImage m_backgroundImageDef;

		public override byte[] ImageData
		{
			get
			{
				return this.m_imageDataHandler.ImageData;
			}
		}

		public override string StreamName
		{
			get
			{
				return this.m_imageDataHandler.StreamName;
			}
		}

		public override string MIMEType
		{
			get
			{
				return this.m_imageDataHandler.MIMEType;
			}
		}

		public override BackgroundRepeatTypes BackgroundRepeat
		{
			get
			{
				if (!this.m_backgroundRepeatEvaluated)
				{
					this.m_backgroundRepeatEvaluated = true;
					this.m_backgroundRepeat = (BackgroundRepeatTypes)this.m_backgroundImageDef.StyleDef.EvaluateInstanceStyleEnum(StyleAttributeNames.BackgroundImageRepeat);
				}
				return this.m_backgroundRepeat;
			}
		}

		public override Positions Position
		{
			get
			{
				if (!this.m_positionEvaluated)
				{
					this.m_positionEvaluated = true;
					this.m_position = (Positions)this.m_backgroundImageDef.StyleDef.EvaluateInstanceStyleEnum(StyleAttributeNames.Position);
				}
				return this.m_position;
			}
		}

		public override ReportColor TransparentColor
		{
			get
			{
				if (!this.m_transparentColorEvaluated)
				{
					this.m_transparentColorEvaluated = true;
					this.m_transparentColor = this.m_backgroundImageDef.StyleDef.EvaluateInstanceReportColor(StyleAttributeNames.TransparentColor);
				}
				return this.m_transparentColor;
			}
		}

		internal InternalBackgroundImageInstance(BackgroundImage backgroundImageDef)
			: base(backgroundImageDef.StyleDef.ReportScope)
		{
			this.m_backgroundImageDef = backgroundImageDef;
			this.m_imageDataHandler = ImageDataHandlerFactory.Create(this.m_backgroundImageDef.StyleDef.ReportElement, backgroundImageDef);
		}

		protected override void ResetInstanceCache()
		{
			this.m_backgroundRepeatEvaluated = false;
			this.m_positionEvaluated = false;
			this.m_transparentColorEvaluated = false;
			this.m_transparentColor = null;
			this.m_imageDataHandler.ClearCache();
		}
	}
}
