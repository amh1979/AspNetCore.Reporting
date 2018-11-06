using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapColorScaleInstance : MapDockableSubItemInstance
	{
		private MapColorScale m_defObject;

		private ReportSize m_tickMarkLength;

		private ReportColor m_colorBarBorderColor;

		private int? m_labelInterval;

		private string m_labelFormat;

		private MapLabelPlacement? m_labelPlacement;

		private MapLabelBehavior? m_labelBehavior;

		private bool? m_hideEndLabels;

		private ReportColor m_rangeGapColor;

		private string m_noDataText;

		public ReportSize TickMarkLength
		{
			get
			{
				if (this.m_tickMarkLength == null)
				{
					this.m_tickMarkLength = new ReportSize(((AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScale)this.m_defObject.MapDockableSubItemDef).EvaluateTickMarkLength(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return this.m_tickMarkLength;
			}
		}

		public ReportColor ColorBarBorderColor
		{
			get
			{
				if (this.m_colorBarBorderColor == null)
				{
					this.m_colorBarBorderColor = new ReportColor(((AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScale)this.m_defObject.MapDockableSubItemDef).EvaluateColorBarBorderColor(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return this.m_colorBarBorderColor;
			}
		}

		public int LabelInterval
		{
			get
			{
				if (!this.m_labelInterval.HasValue)
				{
					this.m_labelInterval = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScale)this.m_defObject.MapDockableSubItemDef).EvaluateLabelInterval(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_labelInterval.Value;
			}
		}

		public string LabelFormat
		{
			get
			{
				if (this.m_labelFormat == null)
				{
					this.m_labelFormat = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScale)this.m_defObject.MapDockableSubItemDef).EvaluateLabelFormat(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_labelFormat;
			}
		}

		public MapLabelPlacement LabelPlacement
		{
			get
			{
				if (!this.m_labelPlacement.HasValue)
				{
					this.m_labelPlacement = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScale)this.m_defObject.MapDockableSubItemDef).EvaluateLabelPlacement(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_labelPlacement.Value;
			}
		}

		public MapLabelBehavior LabelBehavior
		{
			get
			{
				if (!this.m_labelBehavior.HasValue)
				{
					this.m_labelBehavior = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScale)this.m_defObject.MapDockableSubItemDef).EvaluateLabelBehavior(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_labelBehavior.Value;
			}
		}

		public bool HideEndLabels
		{
			get
			{
				if (!this.m_hideEndLabels.HasValue)
				{
					this.m_hideEndLabels = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScale)this.m_defObject.MapDockableSubItemDef).EvaluateHideEndLabels(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_hideEndLabels.Value;
			}
		}

		public ReportColor RangeGapColor
		{
			get
			{
				if (this.m_rangeGapColor == null)
				{
					this.m_rangeGapColor = new ReportColor(((AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScale)this.m_defObject.MapDockableSubItemDef).EvaluateRangeGapColor(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return this.m_rangeGapColor;
			}
		}

		public string NoDataText
		{
			get
			{
				if (this.m_noDataText == null)
				{
					this.m_noDataText = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScale)this.m_defObject.MapDockableSubItemDef).EvaluateNoDataText(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_noDataText;
			}
		}

		internal MapColorScaleInstance(MapColorScale defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_tickMarkLength = null;
			this.m_colorBarBorderColor = null;
			this.m_labelInterval = null;
			this.m_labelFormat = null;
			this.m_labelPlacement = null;
			this.m_labelBehavior = null;
			this.m_hideEndLabels = null;
			this.m_rangeGapColor = null;
			this.m_noDataText = null;
		}
	}
}
