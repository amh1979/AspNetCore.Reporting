using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System.Drawing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class CapImage : BaseGaugeImage
	{
		private ReportColorProperty m_hueColor;

		private ReportSizeProperty m_offsetX;

		private ReportSizeProperty m_offsetY;

		public ReportColorProperty HueColor
		{
			get
			{
				if (this.m_hueColor == null && this.CapImageDef.HueColor != null)
				{
					ExpressionInfo hueColor = this.CapImageDef.HueColor;
					if (hueColor != null)
					{
						this.m_hueColor = new ReportColorProperty(hueColor.IsExpression, hueColor.OriginalText, hueColor.IsExpression ? null : new ReportColor(hueColor.StringValue.Trim(), true), hueColor.IsExpression ? new ReportColor("", Color.Empty, true) : null);
					}
				}
				return this.m_hueColor;
			}
		}

		public ReportSizeProperty OffsetX
		{
			get
			{
				if (this.m_offsetX == null && this.CapImageDef.OffsetX != null)
				{
					this.m_offsetX = new ReportSizeProperty(this.CapImageDef.OffsetX);
				}
				return this.m_offsetX;
			}
		}

		public ReportSizeProperty OffsetY
		{
			get
			{
				if (this.m_offsetY == null && this.CapImageDef.OffsetY != null)
				{
					this.m_offsetY = new ReportSizeProperty(this.CapImageDef.OffsetY);
				}
				return this.m_offsetY;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.CapImage CapImageDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.CapImage)base.m_defObject;
			}
		}

		public new CapImageInstance Instance
		{
			get
			{
				return (CapImageInstance)this.GetInstance();
			}
		}

		internal CapImage(AspNetCore.ReportingServices.ReportIntermediateFormat.CapImage defObject, GaugePanel gaugePanel)
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
				base.m_instance = new CapImageInstance(this);
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
