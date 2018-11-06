namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class PointerCapInstance : BaseInstance
	{
		private PointerCap m_defObject;

		private StyleInstance m_style;

		private bool? m_onTop;

		private bool? m_reflection;

		private GaugeCapStyles? m_capStyle;

		private bool? m_hidden;

		private double? m_width;

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

		public bool OnTop
		{
			get
			{
				if (!this.m_onTop.HasValue)
				{
					this.m_onTop = this.m_defObject.PointerCapDef.EvaluateOnTop(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_onTop.Value;
			}
		}

		public bool Reflection
		{
			get
			{
				if (!this.m_reflection.HasValue)
				{
					this.m_reflection = this.m_defObject.PointerCapDef.EvaluateReflection(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_reflection.Value;
			}
		}

		public GaugeCapStyles CapStyle
		{
			get
			{
				if (!this.m_capStyle.HasValue)
				{
					this.m_capStyle = this.m_defObject.PointerCapDef.EvaluateCapStyle(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_capStyle.Value;
			}
		}

		public bool Hidden
		{
			get
			{
				if (!this.m_hidden.HasValue)
				{
					this.m_hidden = this.m_defObject.PointerCapDef.EvaluateHidden(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_hidden.Value;
			}
		}

		public double Width
		{
			get
			{
				if (!this.m_width.HasValue)
				{
					this.m_width = this.m_defObject.PointerCapDef.EvaluateWidth(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_width.Value;
			}
		}

		internal PointerCapInstance(PointerCap defObject)
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
			this.m_onTop = null;
			this.m_reflection = null;
			this.m_capStyle = null;
			this.m_hidden = null;
			this.m_width = null;
		}
	}
}
