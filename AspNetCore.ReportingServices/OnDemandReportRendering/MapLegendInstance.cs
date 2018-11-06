using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLegendInstance : MapDockableSubItemInstance
	{
		private MapLegend m_defObject;

		private MapLegendLayout? m_layout;

		private bool? m_autoFitTextDisabled;

		private ReportSize m_minFontSize;

		private bool? m_interlacedRows;

		private ReportColor m_interlacedRowsColor;

		private bool? m_equallySpacedItems;

		private int? m_textWrapThreshold;

		public MapLegendLayout Layout
		{
			get
			{
				if (!this.m_layout.HasValue)
				{
					this.m_layout = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegend)this.m_defObject.MapDockableSubItemDef).EvaluateLayout(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_layout.Value;
			}
		}

		public bool AutoFitTextDisabled
		{
			get
			{
				if (!this.m_autoFitTextDisabled.HasValue)
				{
					this.m_autoFitTextDisabled = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegend)this.m_defObject.MapDockableSubItemDef).EvaluateAutoFitTextDisabled(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_autoFitTextDisabled.Value;
			}
		}

		public ReportSize MinFontSize
		{
			get
			{
				if (this.m_minFontSize == null)
				{
					this.m_minFontSize = new ReportSize(((AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegend)this.m_defObject.MapDockableSubItemDef).EvaluateMinFontSize(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return this.m_minFontSize;
			}
		}

		public bool InterlacedRows
		{
			get
			{
				if (!this.m_interlacedRows.HasValue)
				{
					this.m_interlacedRows = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegend)this.m_defObject.MapDockableSubItemDef).EvaluateInterlacedRows(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_interlacedRows.Value;
			}
		}

		public ReportColor InterlacedRowsColor
		{
			get
			{
				if (this.m_interlacedRowsColor == null)
				{
					this.m_interlacedRowsColor = new ReportColor(((AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegend)this.m_defObject.MapDockableSubItemDef).EvaluateInterlacedRowsColor(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return this.m_interlacedRowsColor;
			}
		}

		public bool EquallySpacedItems
		{
			get
			{
				if (!this.m_equallySpacedItems.HasValue)
				{
					this.m_equallySpacedItems = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegend)this.m_defObject.MapDockableSubItemDef).EvaluateEquallySpacedItems(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_equallySpacedItems.Value;
			}
		}

		public int TextWrapThreshold
		{
			get
			{
				if (!this.m_textWrapThreshold.HasValue)
				{
					this.m_textWrapThreshold = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegend)this.m_defObject.MapDockableSubItemDef).EvaluateTextWrapThreshold(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_textWrapThreshold.Value;
			}
		}

		internal MapLegendInstance(MapLegend defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_layout = null;
			this.m_autoFitTextDisabled = null;
			this.m_minFontSize = null;
			this.m_interlacedRows = null;
			this.m_interlacedRowsColor = null;
			this.m_equallySpacedItems = null;
			this.m_textWrapThreshold = null;
		}
	}
}
