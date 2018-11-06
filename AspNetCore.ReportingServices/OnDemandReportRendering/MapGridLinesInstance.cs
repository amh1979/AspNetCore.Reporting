namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapGridLinesInstance : BaseInstance
	{
		private MapGridLines m_defObject;

		private StyleInstance m_style;

		private bool? m_hidden;

		private double? m_interval;

		private bool? m_showLabels;

		private MapLabelPosition? m_labelPosition;

		public StyleInstance Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new StyleInstance(this.m_defObject, this.m_defObject.MapDef.ReportScope, this.m_defObject.MapDef.RenderingContext);
				}
				return this.m_style;
			}
		}

		public bool Hidden
		{
			get
			{
				if (!this.m_hidden.HasValue)
				{
					this.m_hidden = this.m_defObject.MapGridLinesDef.EvaluateHidden(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_hidden.Value;
			}
		}

		public double Interval
		{
			get
			{
				if (!this.m_interval.HasValue)
				{
					this.m_interval = this.m_defObject.MapGridLinesDef.EvaluateInterval(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_interval.Value;
			}
		}

		public bool ShowLabels
		{
			get
			{
				if (!this.m_showLabels.HasValue)
				{
					this.m_showLabels = this.m_defObject.MapGridLinesDef.EvaluateShowLabels(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_showLabels.Value;
			}
		}

		public MapLabelPosition LabelPosition
		{
			get
			{
				if (!this.m_labelPosition.HasValue)
				{
					this.m_labelPosition = this.m_defObject.MapGridLinesDef.EvaluateLabelPosition(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_labelPosition.Value;
			}
		}

		internal MapGridLinesInstance(MapGridLines defObject)
			: base(defObject.MapDef.ReportScope)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			this.m_hidden = null;
			this.m_interval = null;
			this.m_showLabels = null;
			this.m_labelPosition = null;
		}
	}
}
