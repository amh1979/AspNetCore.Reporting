using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System.Drawing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class TopImage : BaseGaugeImage
	{
		private ReportColorProperty m_hueColor;

		public ReportColorProperty HueColor
		{
			get
			{
				if (this.m_hueColor == null)
				{
					ExpressionInfo hueColor = this.TopImageDef.HueColor;
					if (hueColor != null)
					{
						this.m_hueColor = new ReportColorProperty(hueColor.IsExpression, hueColor.OriginalText, hueColor.IsExpression ? null : new ReportColor(hueColor.StringValue.Trim(), true), hueColor.IsExpression ? new ReportColor("", Color.Empty, true) : null);
					}
				}
				return this.m_hueColor;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.TopImage TopImageDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.TopImage)base.m_defObject;
			}
		}

		public new TopImageInstance Instance
		{
			get
			{
				return (TopImageInstance)this.GetInstance();
			}
		}

		internal TopImage(AspNetCore.ReportingServices.ReportIntermediateFormat.TopImage defObject, GaugePanel gaugePanel)
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
				base.m_instance = new TopImageInstance(this);
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
