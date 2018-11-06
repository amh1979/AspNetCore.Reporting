namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class GaugePointerInstance : BaseInstance
	{
		private GaugePointer m_defObject;

		private StyleInstance m_style;

		private GaugeBarStarts? m_barStart;

		private double? m_distanceFromScale;

		private double? m_markerLength;

		private GaugeMarkerStyles? m_markerStyle;

		private GaugePointerPlacements? m_placement;

		private bool? m_snappingEnabled;

		private double? m_snappingInterval;

		private string m_toolTip;

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

		public GaugeBarStarts BarStart
		{
			get
			{
				if (!this.m_barStart.HasValue)
				{
					this.m_barStart = this.m_defObject.GaugePointerDef.EvaluateBarStart(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_barStart.Value;
			}
		}

		public double DistanceFromScale
		{
			get
			{
				if (!this.m_distanceFromScale.HasValue)
				{
					this.m_distanceFromScale = this.m_defObject.GaugePointerDef.EvaluateDistanceFromScale(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_distanceFromScale.Value;
			}
		}

		public double MarkerLength
		{
			get
			{
				if (!this.m_markerLength.HasValue)
				{
					this.m_markerLength = this.m_defObject.GaugePointerDef.EvaluateMarkerLength(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_markerLength.Value;
			}
		}

		public GaugeMarkerStyles MarkerStyle
		{
			get
			{
				if (!this.m_markerStyle.HasValue)
				{
					this.m_markerStyle = this.m_defObject.GaugePointerDef.EvaluateMarkerStyle(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_markerStyle.Value;
			}
		}

		public GaugePointerPlacements Placement
		{
			get
			{
				if (!this.m_placement.HasValue)
				{
					this.m_placement = this.m_defObject.GaugePointerDef.EvaluatePlacement(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_placement.Value;
			}
		}

		public bool SnappingEnabled
		{
			get
			{
				if (!this.m_snappingEnabled.HasValue)
				{
					this.m_snappingEnabled = this.m_defObject.GaugePointerDef.EvaluateSnappingEnabled(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_snappingEnabled.Value;
			}
		}

		public double SnappingInterval
		{
			get
			{
				if (!this.m_snappingInterval.HasValue)
				{
					this.m_snappingInterval = this.m_defObject.GaugePointerDef.EvaluateSnappingInterval(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_snappingInterval.Value;
			}
		}

		public string ToolTip
		{
			get
			{
				if (this.m_toolTip == null)
				{
					this.m_toolTip = this.m_defObject.GaugePointerDef.EvaluateToolTip(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
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
					this.m_hidden = this.m_defObject.GaugePointerDef.EvaluateHidden(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
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
					this.m_width = this.m_defObject.GaugePointerDef.EvaluateWidth(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_width.Value;
			}
		}

		internal GaugePointerInstance(GaugePointer defObject)
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
			this.m_barStart = null;
			this.m_distanceFromScale = null;
			this.m_markerLength = null;
			this.m_markerStyle = null;
			this.m_placement = null;
			this.m_snappingEnabled = null;
			this.m_snappingInterval = null;
			this.m_toolTip = null;
			this.m_hidden = null;
			this.m_width = null;
		}
	}
}
