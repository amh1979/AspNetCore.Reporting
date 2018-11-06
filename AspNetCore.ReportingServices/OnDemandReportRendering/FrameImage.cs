using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System.Drawing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class FrameImage : BaseGaugeImage
	{
		private ReportColorProperty m_hueColor;

		private ReportDoubleProperty m_transparency;

		private ReportBoolProperty m_clipImage;

		public ReportColorProperty HueColor
		{
			get
			{
				if (this.m_hueColor == null && this.FrameImageDef.HueColor != null)
				{
					ExpressionInfo hueColor = this.FrameImageDef.HueColor;
					if (hueColor != null)
					{
						this.m_hueColor = new ReportColorProperty(hueColor.IsExpression, hueColor.OriginalText, hueColor.IsExpression ? null : new ReportColor(hueColor.StringValue.Trim(), true), hueColor.IsExpression ? new ReportColor("", Color.Empty, true) : null);
					}
				}
				return this.m_hueColor;
			}
		}

		public ReportDoubleProperty Transparency
		{
			get
			{
				if (this.m_transparency == null && this.FrameImageDef.Transparency != null)
				{
					this.m_transparency = new ReportDoubleProperty(this.FrameImageDef.Transparency);
				}
				return this.m_transparency;
			}
		}

		public ReportBoolProperty ClipImage
		{
			get
			{
				if (this.m_clipImage == null && this.FrameImageDef.ClipImage != null)
				{
					this.m_clipImage = new ReportBoolProperty(this.FrameImageDef.ClipImage);
				}
				return this.m_clipImage;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.FrameImage FrameImageDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.FrameImage)base.m_defObject;
			}
		}

		public new FrameImageInstance Instance
		{
			get
			{
				return (FrameImageInstance)this.GetInstance();
			}
		}

		internal FrameImage(AspNetCore.ReportingServices.ReportIntermediateFormat.FrameImage defObject, GaugePanel gaugePanel)
			: base(defObject, gaugePanel)
		{
			base.m_defObject = defObject;
			base.m_gaugePanel = gaugePanel;
		}

		internal override BaseGaugeImageInstance GetInstance()
		{
			if (base.m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (base.m_instance == null)
			{
				base.m_instance = new FrameImageInstance(this);
			}
			return base.m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
		}
	}
}
