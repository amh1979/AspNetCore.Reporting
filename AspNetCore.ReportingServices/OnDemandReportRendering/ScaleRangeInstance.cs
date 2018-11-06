namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ScaleRangeInstance : BaseInstance
	{
		private ScaleRange m_defObject;

		private StyleInstance m_style;

		private double? m_distanceFromScale;

		private double? m_startWidth;

		private double? m_endWidth;

		private ReportColor m_inRangeBarPointerColor;

		private ReportColor m_inRangeLabelColor;

		private ReportColor m_inRangeTickMarksColor;

		private BackgroundGradientTypes? m_backgroundGradientType;

		private ScaleRangePlacements? m_placement;

		private string m_toolTip;

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
					this.m_distanceFromScale = this.m_defObject.ScaleRangeDef.EvaluateDistanceFromScale(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_distanceFromScale.Value;
			}
		}

		public double StartWidth
		{
			get
			{
				if (!this.m_startWidth.HasValue)
				{
					this.m_startWidth = this.m_defObject.ScaleRangeDef.EvaluateStartWidth(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_startWidth.Value;
			}
		}

		public double EndWidth
		{
			get
			{
				if (!this.m_endWidth.HasValue)
				{
					this.m_endWidth = this.m_defObject.ScaleRangeDef.EvaluateEndWidth(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_endWidth.Value;
			}
		}

		public ReportColor InRangeBarPointerColor
		{
			get
			{
				if (this.m_inRangeBarPointerColor == null)
				{
					this.m_inRangeBarPointerColor = new ReportColor(this.m_defObject.ScaleRangeDef.EvaluateInRangeBarPointerColor(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext), true);
				}
				return this.m_inRangeBarPointerColor;
			}
		}

		public ReportColor InRangeLabelColor
		{
			get
			{
				if (this.m_inRangeLabelColor == null)
				{
					this.m_inRangeLabelColor = new ReportColor(this.m_defObject.ScaleRangeDef.EvaluateInRangeLabelColor(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext), true);
				}
				return this.m_inRangeLabelColor;
			}
		}

		public ReportColor InRangeTickMarksColor
		{
			get
			{
				if (this.m_inRangeTickMarksColor == null)
				{
					this.m_inRangeTickMarksColor = new ReportColor(this.m_defObject.ScaleRangeDef.EvaluateInRangeTickMarksColor(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext));
				}
				return this.m_inRangeTickMarksColor;
			}
		}

		public BackgroundGradientTypes BackgroundGradientType
		{
			get
			{
				if (!this.m_backgroundGradientType.HasValue)
				{
					this.m_backgroundGradientType = this.m_defObject.ScaleRangeDef.EvaluateBackgroundGradientType(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_backgroundGradientType.Value;
			}
		}

		public ScaleRangePlacements Placement
		{
			get
			{
				if (!this.m_placement.HasValue)
				{
					this.m_placement = this.m_defObject.ScaleRangeDef.EvaluatePlacement(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_placement.Value;
			}
		}

		public string ToolTip
		{
			get
			{
				if (this.m_toolTip == null)
				{
					this.m_toolTip = this.m_defObject.ScaleRangeDef.EvaluateToolTip(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_toolTip;
			}
		}

		public bool Hidden
		{
			get
			{
				if (!this.m_hidden.HasValue)
				{
					this.m_hidden = this.m_defObject.ScaleRangeDef.EvaluateHidden(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_hidden.Value;
			}
		}

		internal ScaleRangeInstance(ScaleRange defObject)
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
			this.m_startWidth = null;
			this.m_endWidth = null;
			this.m_inRangeBarPointerColor = null;
			this.m_inRangeLabelColor = null;
			this.m_inRangeTickMarksColor = null;
			this.m_placement = null;
			this.m_toolTip = null;
			this.m_hidden = null;
			this.m_backgroundGradientType = null;
		}
	}
}
