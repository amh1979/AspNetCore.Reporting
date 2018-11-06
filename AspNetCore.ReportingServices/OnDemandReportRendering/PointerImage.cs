using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System.Drawing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class PointerImage : BaseGaugeImage
	{
		private ReportColorProperty m_hueColor;

		private ReportDoubleProperty m_transparency;

		private ReportSizeProperty m_offsetX;

		private ReportSizeProperty m_offsetY;

		public ReportColorProperty HueColor
		{
			get
			{
				if (this.m_hueColor == null && this.PointerImageDef.HueColor != null)
				{
					ExpressionInfo hueColor = this.PointerImageDef.HueColor;
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
				if (this.m_transparency == null && this.PointerImageDef.Transparency != null)
				{
					this.m_transparency = new ReportDoubleProperty(this.PointerImageDef.Transparency);
				}
				return this.m_transparency;
			}
		}

		public ReportSizeProperty OffsetX
		{
			get
			{
				if (this.m_offsetX == null && this.PointerImageDef.OffsetX != null)
				{
					this.m_offsetX = new ReportSizeProperty(this.PointerImageDef.OffsetX);
				}
				return this.m_offsetX;
			}
		}

		public ReportSizeProperty OffsetY
		{
			get
			{
				if (this.m_offsetY == null && this.PointerImageDef.OffsetY != null)
				{
					this.m_offsetY = new ReportSizeProperty(this.PointerImageDef.OffsetY);
				}
				return this.m_offsetY;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.PointerImage PointerImageDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.PointerImage)base.m_defObject;
			}
		}

		public new PointerImageInstance Instance
		{
			get
			{
				return (PointerImageInstance)this.GetInstance();
			}
		}

		internal PointerImage(AspNetCore.ReportingServices.ReportIntermediateFormat.PointerImage defObject, GaugePanel gaugePanel)
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
				base.m_instance = new PointerImageInstance(this);
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
