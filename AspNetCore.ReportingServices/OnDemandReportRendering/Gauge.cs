using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class Gauge : GaugePanelItem
	{
		private BackFrame m_backFrame;

		private ReportBoolProperty m_clipContent;

		private TopImage m_topImage;

		private ReportDoubleProperty m_aspectRatio;

		public BackFrame BackFrame
		{
			get
			{
				if (this.m_backFrame == null && this.GaugeDef.BackFrame != null)
				{
					this.m_backFrame = new BackFrame(this.GaugeDef.BackFrame, base.m_gaugePanel);
				}
				return this.m_backFrame;
			}
		}

		public ReportBoolProperty ClipContent
		{
			get
			{
				if (this.m_clipContent == null && this.GaugeDef.ClipContent != null)
				{
					this.m_clipContent = new ReportBoolProperty(this.GaugeDef.ClipContent);
				}
				return this.m_clipContent;
			}
		}

		public TopImage TopImage
		{
			get
			{
				if (this.m_topImage == null && this.GaugeDef.TopImage != null)
				{
					this.m_topImage = new TopImage(this.GaugeDef.TopImage, base.m_gaugePanel);
				}
				return this.m_topImage;
			}
		}

		public ReportDoubleProperty AspectRatio
		{
			get
			{
				if (this.m_aspectRatio == null && this.GaugeDef.AspectRatio != null)
				{
					this.m_aspectRatio = new ReportDoubleProperty(this.GaugeDef.AspectRatio);
				}
				return this.m_aspectRatio;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.Gauge GaugeDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.Gauge)base.m_defObject;
			}
		}

		public new GaugeInstance Instance
		{
			get
			{
				return (GaugeInstance)this.GetInstance();
			}
		}

		internal Gauge(AspNetCore.ReportingServices.ReportIntermediateFormat.Gauge defObject, GaugePanel gaugePanel)
			: base(defObject, gaugePanel)
		{
			base.m_defObject = defObject;
			base.m_gaugePanel = gaugePanel;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
			if (this.m_backFrame != null)
			{
				this.m_backFrame.SetNewContext();
			}
			if (this.m_topImage != null)
			{
				this.m_topImage.SetNewContext();
			}
		}
	}
}
