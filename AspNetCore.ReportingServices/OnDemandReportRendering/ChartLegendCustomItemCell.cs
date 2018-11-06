using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartLegendCustomItemCell : ChartObjectCollectionItem<ChartLegendCustomItemCellInstance>, IROMStyleDefinitionContainer, IROMActionOwner
	{
		private Chart m_chart;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell m_chartLegendCustomItemCellDef;

		private Style m_style;

		private ActionInfo m_actionInfo;

		private ReportEnumProperty<ChartCellType> m_cellType;

		private ReportStringProperty m_text;

		private ReportIntProperty m_cellSpan;

		private ReportStringProperty m_toolTip;

		private ReportIntProperty m_imageWidth;

		private ReportIntProperty m_imageHeight;

		private ReportIntProperty m_symbolHeight;

		private ReportIntProperty m_symbolWidth;

		private ReportEnumProperty<ChartCellAlignment> m_alignment;

		private ReportIntProperty m_topMargin;

		private ReportIntProperty m_bottomMargin;

		private ReportIntProperty m_leftMargin;

		private ReportIntProperty m_rightMargin;

		public Style Style
		{
			get
			{
				if (this.m_style == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendCustomItemCellDef.StyleClass != null)
				{
					this.m_style = new Style(this.m_chart, this.m_chart, this.m_chartLegendCustomItemCellDef, this.m_chart.RenderingContext);
				}
				return this.m_style;
			}
		}

		public string UniqueName
		{
			get
			{
				return this.m_chart.ChartDef.UniqueName + 'x' + this.m_chartLegendCustomItemCellDef.ID;
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				if (this.m_actionInfo == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendCustomItemCellDef.Action != null)
				{
					this.m_actionInfo = new ActionInfo(this.m_chart.RenderingContext, this.m_chart, this.m_chartLegendCustomItemCellDef.Action, this.m_chart.ChartDef, this.m_chart, ObjectType.Chart, this.m_chartLegendCustomItemCellDef.Name, this);
				}
				return this.m_actionInfo;
			}
		}

		public List<string> FieldsUsedInValueExpression
		{
			get
			{
				return null;
			}
		}

		public ReportEnumProperty<ChartCellType> CellType
		{
			get
			{
				if (this.m_cellType == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendCustomItemCellDef.CellType != null)
				{
					this.m_cellType = new ReportEnumProperty<ChartCellType>(this.m_chartLegendCustomItemCellDef.CellType.IsExpression, this.m_chartLegendCustomItemCellDef.CellType.OriginalText, EnumTranslator.TranslateChartCellType(this.m_chartLegendCustomItemCellDef.CellType.StringValue, null));
				}
				return this.m_cellType;
			}
		}

		public ReportStringProperty Text
		{
			get
			{
				if (this.m_text == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendCustomItemCellDef.Text != null)
				{
					this.m_text = new ReportStringProperty(this.m_chartLegendCustomItemCellDef.Text);
				}
				return this.m_text;
			}
		}

		public ReportIntProperty CellSpan
		{
			get
			{
				if (this.m_cellSpan == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendCustomItemCellDef.CellSpan != null)
				{
					this.m_cellSpan = new ReportIntProperty(this.m_chartLegendCustomItemCellDef.CellSpan.IsExpression, this.m_chartLegendCustomItemCellDef.CellSpan.OriginalText, this.m_chartLegendCustomItemCellDef.CellSpan.IntValue, 1);
				}
				return this.m_cellSpan;
			}
		}

		public ReportStringProperty ToolTip
		{
			get
			{
				if (this.m_toolTip == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendCustomItemCellDef.ToolTip != null)
				{
					this.m_toolTip = new ReportStringProperty(this.m_chartLegendCustomItemCellDef.ToolTip);
				}
				return this.m_toolTip;
			}
		}

		public ReportIntProperty ImageWidth
		{
			get
			{
				if (this.m_imageWidth == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendCustomItemCellDef.ImageWidth != null)
				{
					this.m_imageWidth = new ReportIntProperty(this.m_chartLegendCustomItemCellDef.ImageWidth.IsExpression, this.m_chartLegendCustomItemCellDef.ImageWidth.OriginalText, this.m_chartLegendCustomItemCellDef.ImageWidth.IntValue, 0);
				}
				return this.m_imageWidth;
			}
		}

		public ReportIntProperty ImageHeight
		{
			get
			{
				if (this.m_imageHeight == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendCustomItemCellDef.ImageHeight != null)
				{
					this.m_imageHeight = new ReportIntProperty(this.m_chartLegendCustomItemCellDef.ImageHeight.IsExpression, this.m_chartLegendCustomItemCellDef.ImageHeight.OriginalText, this.m_chartLegendCustomItemCellDef.ImageHeight.IntValue, 0);
				}
				return this.m_imageHeight;
			}
		}

		public ReportIntProperty SymbolHeight
		{
			get
			{
				if (this.m_symbolHeight == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendCustomItemCellDef.SymbolHeight != null)
				{
					this.m_symbolHeight = new ReportIntProperty(this.m_chartLegendCustomItemCellDef.SymbolHeight.IsExpression, this.m_chartLegendCustomItemCellDef.SymbolHeight.OriginalText, this.m_chartLegendCustomItemCellDef.SymbolHeight.IntValue, 0);
				}
				return this.m_symbolHeight;
			}
		}

		public ReportIntProperty SymbolWidth
		{
			get
			{
				if (this.m_symbolWidth == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendCustomItemCellDef.SymbolWidth != null)
				{
					this.m_symbolWidth = new ReportIntProperty(this.m_chartLegendCustomItemCellDef.SymbolWidth.IsExpression, this.m_chartLegendCustomItemCellDef.SymbolWidth.OriginalText, this.m_chartLegendCustomItemCellDef.SymbolWidth.IntValue, 0);
				}
				return this.m_symbolWidth;
			}
		}

		public ReportEnumProperty<ChartCellAlignment> Alignment
		{
			get
			{
				if (this.m_alignment == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendCustomItemCellDef.Alignment != null)
				{
					this.m_alignment = new ReportEnumProperty<ChartCellAlignment>(this.m_chartLegendCustomItemCellDef.Alignment.IsExpression, this.m_chartLegendCustomItemCellDef.Alignment.OriginalText, EnumTranslator.TranslateChartCellAlignment(this.m_chartLegendCustomItemCellDef.Alignment.StringValue, null));
				}
				return this.m_alignment;
			}
		}

		public ReportIntProperty TopMargin
		{
			get
			{
				if (this.m_topMargin == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendCustomItemCellDef.TopMargin != null)
				{
					this.m_topMargin = new ReportIntProperty(this.m_chartLegendCustomItemCellDef.TopMargin.IsExpression, this.m_chartLegendCustomItemCellDef.TopMargin.OriginalText, this.m_chartLegendCustomItemCellDef.TopMargin.IntValue, 0);
				}
				return this.m_topMargin;
			}
		}

		public ReportIntProperty BottomMargin
		{
			get
			{
				if (this.m_bottomMargin == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendCustomItemCellDef.BottomMargin != null)
				{
					this.m_bottomMargin = new ReportIntProperty(this.m_chartLegendCustomItemCellDef.BottomMargin.IsExpression, this.m_chartLegendCustomItemCellDef.BottomMargin.OriginalText, this.m_chartLegendCustomItemCellDef.BottomMargin.IntValue, 0);
				}
				return this.m_bottomMargin;
			}
		}

		public ReportIntProperty LeftMargin
		{
			get
			{
				if (this.m_leftMargin == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendCustomItemCellDef.LeftMargin != null)
				{
					this.m_leftMargin = new ReportIntProperty(this.m_chartLegendCustomItemCellDef.LeftMargin.IsExpression, this.m_chartLegendCustomItemCellDef.LeftMargin.OriginalText, this.m_chartLegendCustomItemCellDef.LeftMargin.IntValue, 0);
				}
				return this.m_leftMargin;
			}
		}

		public ReportIntProperty RightMargin
		{
			get
			{
				if (this.m_rightMargin == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendCustomItemCellDef.RightMargin != null)
				{
					this.m_rightMargin = new ReportIntProperty(this.m_chartLegendCustomItemCellDef.RightMargin.IsExpression, this.m_chartLegendCustomItemCellDef.RightMargin.OriginalText, this.m_chartLegendCustomItemCellDef.RightMargin.IntValue, 0);
				}
				return this.m_rightMargin;
			}
		}

		internal Chart ChartDef
		{
			get
			{
				return this.m_chart;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell ChartLegendCustomItemCellDef
		{
			get
			{
				return this.m_chartLegendCustomItemCellDef;
			}
		}

		public ChartLegendCustomItemCellInstance Instance
		{
			get
			{
				if (this.m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (base.m_instance == null)
				{
					base.m_instance = new ChartLegendCustomItemCellInstance(this);
				}
				return base.m_instance;
			}
		}

		internal ChartLegendCustomItemCell(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCellDef, Chart chart)
		{
			this.m_chartLegendCustomItemCellDef = chartLegendCustomItemCellDef;
			this.m_chart = chart;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			if (this.m_actionInfo != null)
			{
				this.m_actionInfo.SetNewContext();
			}
		}
	}
}
