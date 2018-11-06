namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class TickMarkStyleInstance : BaseInstance
	{
		protected TickMarkStyle m_defObject;

		private StyleInstance m_style;

		private double? m_distanceFromScale;

		private GaugeLabelPlacements? m_placement;

		private bool? m_enableGradient;

		private double? m_gradientDensity;

		private double? m_length;

		private double? m_width;

		private GaugeTickMarkShapes? m_shape;

		private bool? m_hidden;

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

		public double DistanceFromScale
		{
			get
			{
				if (!this.m_distanceFromScale.HasValue)
				{
					this.m_distanceFromScale = this.m_defObject.TickMarkStyleDef.EvaluateDistanceFromScale(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_distanceFromScale.Value;
			}
		}

		public GaugeLabelPlacements Placement
		{
			get
			{
				if (!this.m_placement.HasValue)
				{
					this.m_placement = this.m_defObject.TickMarkStyleDef.EvaluatePlacement(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_placement.Value;
			}
		}

		public bool EnableGradient
		{
			get
			{
				if (!this.m_enableGradient.HasValue)
				{
					this.m_enableGradient = this.m_defObject.TickMarkStyleDef.EvaluateEnableGradient(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_enableGradient.Value;
			}
		}

		public double GradientDensity
		{
			get
			{
				if (!this.m_gradientDensity.HasValue)
				{
					this.m_gradientDensity = this.m_defObject.TickMarkStyleDef.EvaluateGradientDensity(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_gradientDensity.Value;
			}
		}

		public double Length
		{
			get
			{
				if (!this.m_length.HasValue)
				{
					this.m_length = this.m_defObject.TickMarkStyleDef.EvaluateLength(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_length.Value;
			}
		}

		public double Width
		{
			get
			{
				if (!this.m_width.HasValue)
				{
					this.m_width = this.m_defObject.TickMarkStyleDef.EvaluateWidth(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_width.Value;
			}
		}

		public GaugeTickMarkShapes Shape
		{
			get
			{
				if (!this.m_shape.HasValue)
				{
					this.m_shape = this.m_defObject.TickMarkStyleDef.EvaluateShape(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_shape.Value;
			}
		}

		public bool Hidden
		{
			get
			{
				if (!this.m_hidden.HasValue)
				{
					this.m_hidden = this.m_defObject.TickMarkStyleDef.EvaluateHidden(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_hidden.Value;
			}
		}

		internal TickMarkStyleInstance(TickMarkStyle defObject)
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
			this.m_distanceFromScale = null;
			this.m_placement = null;
			this.m_enableGradient = null;
			this.m_gradientDensity = null;
			this.m_length = null;
			this.m_width = null;
			this.m_shape = null;
			this.m_hidden = null;
		}
	}
}
