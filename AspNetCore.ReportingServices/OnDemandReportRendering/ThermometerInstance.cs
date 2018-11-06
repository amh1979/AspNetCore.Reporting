namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ThermometerInstance : BaseInstance
	{
		private Thermometer m_defObject;

		private StyleInstance m_style;

		private double? m_bulbOffset;

		private double? m_bulbSize;

		private GaugeThermometerStyles? m_thermometerStyle;

		public StyleInstance Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new StyleInstance(this.m_defObject, this.m_defObject.GaugePanelDef, this.m_defObject.GaugePanelDef.RenderingContext);
				}
				return this.m_style;
			}
		}

		public double BulbOffset
		{
			get
			{
				if (!this.m_bulbOffset.HasValue)
				{
					this.m_bulbOffset = this.m_defObject.ThermometerDef.EvaluateBulbOffset(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_bulbOffset.Value;
			}
		}

		public double BulbSize
		{
			get
			{
				if (!this.m_bulbSize.HasValue)
				{
					this.m_bulbSize = this.m_defObject.ThermometerDef.EvaluateBulbSize(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_bulbSize.Value;
			}
		}

		public GaugeThermometerStyles ThermometerStyle
		{
			get
			{
				if (!this.m_thermometerStyle.HasValue)
				{
					this.m_thermometerStyle = this.m_defObject.ThermometerDef.EvaluateThermometerStyle(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_thermometerStyle.Value;
			}
		}

		internal ThermometerInstance(Thermometer defObject)
			: base(defObject.GaugePanelDef)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			this.m_bulbOffset = null;
			this.m_bulbSize = null;
			this.m_thermometerStyle = null;
		}
	}
}
