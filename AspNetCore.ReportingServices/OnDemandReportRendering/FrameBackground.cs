using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class FrameBackground : IROMStyleDefinitionContainer
	{
		private GaugePanel m_gaugePanel;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.FrameBackground m_defObject;

		private FrameBackgroundInstance m_instance;

		private Style m_style;

		public Style Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new Style(this.m_gaugePanel, this.m_gaugePanel, this.m_defObject, this.m_gaugePanel.RenderingContext);
				}
				return this.m_style;
			}
		}

		internal GaugePanel GaugePanelDef
		{
			get
			{
				return this.m_gaugePanel;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.FrameBackground FrameBackgroundDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		public FrameBackgroundInstance Instance
		{
			get
			{
				if (this.m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new FrameBackgroundInstance(this);
				}
				return this.m_instance;
			}
		}

		internal FrameBackground(AspNetCore.ReportingServices.ReportIntermediateFormat.FrameBackground defObject, GaugePanel gaugePanel)
		{
			this.m_defObject = defObject;
			this.m_gaugePanel = gaugePanel;
		}

		internal void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
		}
	}
}
