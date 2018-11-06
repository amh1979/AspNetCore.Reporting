namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class BackFrameInstance : BaseInstance
	{
		private BackFrame m_defObject;

		private StyleInstance m_style;

		private GaugeFrameStyles? m_frameStyle;

		private GaugeFrameShapes? m_frameShape;

		private double? m_frameWidth;

		private GaugeGlassEffects? m_glassEffect;

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

		public GaugeFrameStyles FrameStyle
		{
			get
			{
				if (!this.m_frameStyle.HasValue)
				{
					this.m_frameStyle = this.m_defObject.BackFrameDef.EvaluateFrameStyle(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_frameStyle.Value;
			}
		}

		public GaugeFrameShapes FrameShape
		{
			get
			{
				if (!this.m_frameShape.HasValue)
				{
					this.m_frameShape = this.m_defObject.BackFrameDef.EvaluateFrameShape(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_frameShape.Value;
			}
		}

		public double FrameWidth
		{
			get
			{
				if (!this.m_frameWidth.HasValue)
				{
					this.m_frameWidth = this.m_defObject.BackFrameDef.EvaluateFrameWidth(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_frameWidth.Value;
			}
		}

		public GaugeGlassEffects GlassEffect
		{
			get
			{
				if (!this.m_glassEffect.HasValue)
				{
					this.m_glassEffect = this.m_defObject.BackFrameDef.EvaluateGlassEffect(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_glassEffect.Value;
			}
		}

		internal BackFrameInstance(BackFrame defObject)
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
			this.m_frameStyle = null;
			this.m_frameShape = null;
			this.m_glassEffect = null;
			this.m_frameWidth = null;
		}
	}
}
