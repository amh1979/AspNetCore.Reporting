using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;
using System.Drawing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartLegendCustomItem : ChartObjectCollectionItem<ChartLegendCustomItemInstance>, IROMStyleDefinitionContainer, IROMActionOwner
	{
		private Chart m_chart;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItem m_chartLegendCustomItemDef;

		private Style m_style;

		private ActionInfo m_actionInfo;

		private ChartMarker m_marker;

		private ReportEnumProperty<ChartSeparators> m_separator;

		private ReportColorProperty m_separatorColor;

		private ReportStringProperty m_toolTip;

		private ChartLegendCustomItemCellCollection m_chartLegendCustomItemCells;

		public Style Style
		{
			get
			{
				if (this.m_style == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendCustomItemDef.StyleClass != null)
				{
					this.m_style = new Style(this.m_chart, this.m_chart, this.m_chartLegendCustomItemDef, this.m_chart.RenderingContext);
				}
				return this.m_style;
			}
		}

		public string UniqueName
		{
			get
			{
				return this.m_chart.ChartDef.UniqueName + 'x' + this.m_chartLegendCustomItemDef.ID;
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				if (this.m_actionInfo == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendCustomItemDef.Action != null)
				{
					this.m_actionInfo = new ActionInfo(this.m_chart.RenderingContext, this.m_chart, this.m_chartLegendCustomItemDef.Action, this.m_chart.ChartDef, this.m_chart, ObjectType.Chart, this.m_chartLegendCustomItemDef.Name, this);
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

		public ChartMarker Marker
		{
			get
			{
				if (this.m_marker == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendCustomItemDef.Marker != null)
				{
					this.m_marker = new ChartMarker(this.m_chartLegendCustomItemDef.Marker, this.m_chart);
				}
				return this.m_marker;
			}
		}

		public ReportEnumProperty<ChartSeparators> Separator
		{
			get
			{
				if (this.m_separator == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendCustomItemDef.Separator != null)
				{
					this.m_separator = new ReportEnumProperty<ChartSeparators>(this.m_chartLegendCustomItemDef.Separator.IsExpression, this.m_chartLegendCustomItemDef.Separator.OriginalText, EnumTranslator.TranslateChartSeparator(this.m_chartLegendCustomItemDef.Separator.StringValue, null));
				}
				return this.m_separator;
			}
		}

		public ReportColorProperty SeparatorColor
		{
			get
			{
				if (this.m_separatorColor == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendCustomItemDef.SeparatorColor != null)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo separatorColor = this.m_chartLegendCustomItemDef.SeparatorColor;
					this.m_separatorColor = new ReportColorProperty(separatorColor.IsExpression, separatorColor.OriginalText, separatorColor.IsExpression ? null : new ReportColor(separatorColor.StringValue.Trim(), true), separatorColor.IsExpression ? new ReportColor("", Color.Empty, true) : null);
				}
				return this.m_separatorColor;
			}
		}

		public ReportStringProperty ToolTip
		{
			get
			{
				if (this.m_toolTip == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendCustomItemDef.ToolTip != null)
				{
					this.m_toolTip = new ReportStringProperty(this.m_chartLegendCustomItemDef.ToolTip);
				}
				return this.m_toolTip;
			}
		}

		public ChartLegendCustomItemCellCollection LegendCustomItemCells
		{
			get
			{
				if (this.m_chartLegendCustomItemCells == null && !this.m_chart.IsOldSnapshot && this.ChartLegendCustomItemDef.LegendCustomItemCells != null)
				{
					this.m_chartLegendCustomItemCells = new ChartLegendCustomItemCellCollection(this, this.m_chart);
				}
				return this.m_chartLegendCustomItemCells;
			}
		}

		internal Chart ChartDef
		{
			get
			{
				return this.m_chart;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItem ChartLegendCustomItemDef
		{
			get
			{
				return this.m_chartLegendCustomItemDef;
			}
		}

		public ChartLegendCustomItemInstance Instance
		{
			get
			{
				if (this.m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (base.m_instance == null)
				{
					base.m_instance = new ChartLegendCustomItemInstance(this);
				}
				return base.m_instance;
			}
		}

		internal ChartLegendCustomItem(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItem chartLegendCustomItemDef, Chart chart)
		{
			this.m_chartLegendCustomItemDef = chartLegendCustomItemDef;
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
			if (this.m_marker != null)
			{
				this.m_marker.SetNewContext();
			}
			if (this.m_chartLegendCustomItemCells != null)
			{
				this.m_chartLegendCustomItemCells.SetNewContext();
			}
		}
	}
}
