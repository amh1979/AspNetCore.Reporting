using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System.Drawing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class IndicatorImage : BaseGaugeImage
	{
		private ReportColorProperty m_hueColor;

		private ReportDoubleProperty m_transparency;

		public ReportColorProperty HueColor
		{
			get
			{
				if (this.m_hueColor == null && this.IndicatorImageDef.HueColor != null)
				{
					ExpressionInfo hueColor = this.IndicatorImageDef.HueColor;
					if (hueColor != null)
					{
						this.m_hueColor = new ReportColorProperty(hueColor.IsExpression, this.IndicatorImageDef.HueColor.OriginalText, hueColor.IsExpression ? null : new ReportColor(hueColor.StringValue.Trim(), true), hueColor.IsExpression ? new ReportColor("", Color.Empty, true) : null);
					}
				}
				return this.m_hueColor;
			}
		}

		public ReportDoubleProperty Transparency
		{
			get
			{
				if (this.m_transparency == null && this.IndicatorImageDef.Transparency != null)
				{
					this.m_transparency = new ReportDoubleProperty(this.IndicatorImageDef.Transparency);
				}
				return this.m_transparency;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.IndicatorImage IndicatorImageDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.IndicatorImage)base.m_defObject;
			}
		}

		public new IndicatorImageInstance Instance
		{
			get
			{
				return (IndicatorImageInstance)this.GetInstance();
			}
		}

		internal IndicatorImage(AspNetCore.ReportingServices.ReportIntermediateFormat.IndicatorImage defObject, GaugePanel gaugePanel)
			: base(defObject, gaugePanel)
		{
		}

		internal override BaseGaugeImageInstance GetInstance()
		{
			if (base.m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (base.m_instance == null)
			{
				base.m_instance = new IndicatorImageInstance(this);
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
