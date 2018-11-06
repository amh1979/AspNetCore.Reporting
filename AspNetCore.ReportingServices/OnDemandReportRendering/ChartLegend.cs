using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Drawing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartLegend : ChartObjectCollectionItem<ChartLegendInstance>, IROMStyleDefinitionContainer
	{
		private Chart m_chart;

		private Legend m_renderLegendDef;

		private object[] m_styleValues;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegend m_legendDef;

		private Style m_style;

		private ChartLegendCustomItemCollection m_chartLegendCustomItems;

		private ChartLegendColumnCollection m_chartLegendColumns;

		private ChartLegendTitle m_legendTitle;

		private ReportEnumProperty<ChartLegendLayouts> m_layout;

		private ReportEnumProperty<ChartLegendPositions> m_position;

		private ReportBoolProperty m_hidden;

		private ReportBoolProperty m_dockOutsideChartArea;

		private ReportBoolProperty m_autoFitTextDisabled;

		private ReportSizeProperty m_minFontSize;

		private ReportEnumProperty<ChartSeparators> m_headerSeparator;

		private ReportColorProperty m_headerSeparatorColor;

		private ReportEnumProperty<ChartSeparators> m_columnSeparator;

		private ReportColorProperty m_columnSeparatorColor;

		private ReportIntProperty m_columnSpacing;

		private ReportBoolProperty m_interlacedRows;

		private ReportColorProperty m_interlacedRowsColor;

		private ReportBoolProperty m_equallySpacedItems;

		private ReportEnumProperty<ChartAutoBool> m_reversed;

		private ReportIntProperty m_maxAutoSize;

		private ReportIntProperty m_textWrapThreshold;

		private ChartElementPosition m_chartElementPosition;

		public string Name
		{
			get
			{
				if (this.m_chart.IsOldSnapshot)
				{
					return "Default";
				}
				return this.m_legendDef.LegendName;
			}
		}

		public ReportBoolProperty Hidden
		{
			get
			{
				if (this.m_hidden == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_hidden = new ReportBoolProperty(!this.m_renderLegendDef.Visible);
					}
					else
					{
						this.m_hidden = new ReportBoolProperty(this.m_legendDef.Hidden);
					}
				}
				return this.m_hidden;
			}
		}

		public Style Style
		{
			get
			{
				if (this.m_style == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_style = new Style(this.m_renderLegendDef.StyleClass, this.m_styleValues, this.m_chart.RenderingContext);
					}
					else if (this.m_legendDef.StyleClass != null)
					{
						this.m_style = new Style(this.m_chart, this.m_chart, this.m_legendDef, this.m_chart.RenderingContext);
					}
				}
				return this.m_style;
			}
		}

		public ReportEnumProperty<ChartLegendPositions> Position
		{
			get
			{
				if (this.m_position == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						ChartLegendPositions value = ChartLegendPositions.TopRight;
						switch (this.m_renderLegendDef.Position)
						{
						case Legend.Positions.BottomCenter:
							value = ChartLegendPositions.BottomCenter;
							break;
						case Legend.Positions.BottomLeft:
							value = ChartLegendPositions.BottomLeft;
							break;
						case Legend.Positions.BottomRight:
							value = ChartLegendPositions.BottomRight;
							break;
						case Legend.Positions.LeftBottom:
							value = ChartLegendPositions.LeftBottom;
							break;
						case Legend.Positions.LeftCenter:
							value = ChartLegendPositions.LeftCenter;
							break;
						case Legend.Positions.LeftTop:
							value = ChartLegendPositions.LeftTop;
							break;
						case Legend.Positions.RightBottom:
							value = ChartLegendPositions.RightBottom;
							break;
						case Legend.Positions.RightCenter:
							value = ChartLegendPositions.RightCenter;
							break;
						case Legend.Positions.RightTop:
							value = ChartLegendPositions.RightTop;
							break;
						case Legend.Positions.TopCenter:
							value = ChartLegendPositions.TopCenter;
							break;
						case Legend.Positions.TopLeft:
							value = ChartLegendPositions.TopLeft;
							break;
						case Legend.Positions.TopRight:
							value = ChartLegendPositions.TopRight;
							break;
						}
						this.m_position = new ReportEnumProperty<ChartLegendPositions>(value);
					}
					else if (this.m_legendDef.Position != null)
					{
						this.m_position = new ReportEnumProperty<ChartLegendPositions>(this.m_legendDef.Position.IsExpression, this.m_legendDef.Position.OriginalText, EnumTranslator.TranslateChartLegendPositions(this.m_legendDef.Position.StringValue, null));
					}
				}
				return this.m_position;
			}
		}

		public ReportEnumProperty<ChartLegendLayouts> Layout
		{
			get
			{
				if (this.m_layout == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						ChartLegendLayouts value = ChartLegendLayouts.AutoTable;
						switch (this.m_renderLegendDef.Layout)
						{
						case Legend.LegendLayout.Column:
							value = ChartLegendLayouts.Column;
							break;
						case Legend.LegendLayout.Row:
							value = ChartLegendLayouts.Row;
							break;
						case Legend.LegendLayout.Table:
							value = ChartLegendLayouts.AutoTable;
							break;
						}
						this.m_layout = new ReportEnumProperty<ChartLegendLayouts>(value);
					}
					else if (this.m_legendDef.Layout != null)
					{
						this.m_layout = new ReportEnumProperty<ChartLegendLayouts>(this.m_legendDef.Layout.IsExpression, this.m_legendDef.Layout.OriginalText, EnumTranslator.TranslateChartLegendLayout(this.m_legendDef.Layout.StringValue, null));
					}
				}
				return this.m_layout;
			}
		}

		public ReportBoolProperty DockOutsideChartArea
		{
			get
			{
				if (this.m_dockOutsideChartArea == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_dockOutsideChartArea = new ReportBoolProperty(!this.m_renderLegendDef.InsidePlotArea);
					}
					else if (this.m_legendDef.DockOutsideChartArea != null)
					{
						this.m_dockOutsideChartArea = new ReportBoolProperty(this.m_legendDef.DockOutsideChartArea);
					}
				}
				return this.m_dockOutsideChartArea;
			}
		}

		public ChartLegendCustomItemCollection LegendCustomItems
		{
			get
			{
				if (this.m_chartLegendCustomItems == null && !this.m_chart.IsOldSnapshot && this.ChartLegendDef.LegendCustomItems != null)
				{
					this.m_chartLegendCustomItems = new ChartLegendCustomItemCollection(this, this.m_chart);
				}
				return this.m_chartLegendCustomItems;
			}
		}

		public ChartLegendColumnCollection LegendColumns
		{
			get
			{
				if (this.m_chartLegendColumns == null && !this.m_chart.IsOldSnapshot && this.ChartLegendDef.LegendColumns != null)
				{
					this.m_chartLegendColumns = new ChartLegendColumnCollection(this, this.m_chart);
				}
				return this.m_chartLegendColumns;
			}
		}

		public ChartLegendTitle LegendTitle
		{
			get
			{
				if (this.m_legendTitle == null && !this.m_chart.IsOldSnapshot && this.m_legendDef.LegendTitle != null)
				{
					this.m_legendTitle = new ChartLegendTitle(this.m_legendDef.LegendTitle, this.m_chart);
				}
				return this.m_legendTitle;
			}
		}

		public string DockToChartArea
		{
			get
			{
				if (this.m_chart.IsOldSnapshot)
				{
					if (!this.DockOutsideChartArea.Value)
					{
						return "Default";
					}
					return null;
				}
				return this.m_legendDef.DockToChartArea;
			}
		}

		public ChartElementPosition ChartElementPosition
		{
			get
			{
				if (this.m_chartElementPosition == null && !this.m_chart.IsOldSnapshot && this.m_legendDef.ChartElementPosition != null)
				{
					this.m_chartElementPosition = new ChartElementPosition(this.m_legendDef.ChartElementPosition, this.m_chart);
				}
				return this.m_chartElementPosition;
			}
		}

		public ReportBoolProperty AutoFitTextDisabled
		{
			get
			{
				if (this.m_autoFitTextDisabled == null && !this.m_chart.IsOldSnapshot && this.m_legendDef.AutoFitTextDisabled != null)
				{
					this.m_autoFitTextDisabled = new ReportBoolProperty(this.m_legendDef.AutoFitTextDisabled);
				}
				return this.m_autoFitTextDisabled;
			}
		}

		public ReportSizeProperty MinFontSize
		{
			get
			{
				if (this.m_minFontSize == null && !this.m_chart.IsOldSnapshot && this.m_legendDef.MinFontSize != null)
				{
					this.m_minFontSize = new ReportSizeProperty(this.m_legendDef.MinFontSize);
				}
				return this.m_minFontSize;
			}
		}

		public ReportEnumProperty<ChartSeparators> HeaderSeparator
		{
			get
			{
				if (this.m_headerSeparator == null && !this.m_chart.IsOldSnapshot && this.m_legendDef.HeaderSeparator != null)
				{
					this.m_headerSeparator = new ReportEnumProperty<ChartSeparators>(this.m_legendDef.HeaderSeparator.IsExpression, this.m_legendDef.HeaderSeparator.OriginalText, EnumTranslator.TranslateChartSeparator(this.m_legendDef.HeaderSeparator.StringValue, null));
				}
				return this.m_headerSeparator;
			}
		}

		public ReportColorProperty HeaderSeparatorColor
		{
			get
			{
				if (this.m_headerSeparatorColor == null && !this.m_chart.IsOldSnapshot && this.m_legendDef.HeaderSeparatorColor != null)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo headerSeparatorColor = this.m_legendDef.HeaderSeparatorColor;
					this.m_headerSeparatorColor = new ReportColorProperty(headerSeparatorColor.IsExpression, headerSeparatorColor.OriginalText, headerSeparatorColor.IsExpression ? null : new ReportColor(headerSeparatorColor.StringValue.Trim(), true), headerSeparatorColor.IsExpression ? new ReportColor("", Color.Empty, true) : null);
				}
				return this.m_headerSeparatorColor;
			}
		}

		public ReportEnumProperty<ChartSeparators> ColumnSeparator
		{
			get
			{
				if (this.m_columnSeparator == null && !this.m_chart.IsOldSnapshot && this.m_legendDef.ColumnSeparator != null)
				{
					this.m_columnSeparator = new ReportEnumProperty<ChartSeparators>(this.m_legendDef.ColumnSeparator.IsExpression, this.m_legendDef.ColumnSeparator.OriginalText, EnumTranslator.TranslateChartSeparator(this.m_legendDef.ColumnSeparator.StringValue, null));
				}
				return this.m_columnSeparator;
			}
		}

		public ReportColorProperty ColumnSeparatorColor
		{
			get
			{
				if (this.m_columnSeparatorColor == null && !this.m_chart.IsOldSnapshot && this.m_legendDef.ColumnSeparatorColor != null)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo columnSeparatorColor = this.m_legendDef.ColumnSeparatorColor;
					this.m_columnSeparatorColor = new ReportColorProperty(columnSeparatorColor.IsExpression, columnSeparatorColor.OriginalText, columnSeparatorColor.IsExpression ? null : new ReportColor(columnSeparatorColor.StringValue.Trim(), true), columnSeparatorColor.IsExpression ? new ReportColor("", Color.Empty, true) : null);
				}
				return this.m_columnSeparatorColor;
			}
		}

		public ReportIntProperty ColumnSpacing
		{
			get
			{
				if (this.m_columnSpacing == null && !this.m_chart.IsOldSnapshot && this.m_legendDef.ColumnSpacing != null)
				{
					this.m_columnSpacing = new ReportIntProperty(this.m_legendDef.ColumnSpacing.IsExpression, this.m_legendDef.ColumnSpacing.OriginalText, this.m_legendDef.ColumnSpacing.IntValue, 50);
				}
				return this.m_columnSpacing;
			}
		}

		public ReportBoolProperty InterlacedRows
		{
			get
			{
				if (this.m_interlacedRows == null && !this.m_chart.IsOldSnapshot && this.m_legendDef.InterlacedRows != null)
				{
					this.m_interlacedRows = new ReportBoolProperty(this.m_legendDef.InterlacedRows);
				}
				return this.m_interlacedRows;
			}
		}

		public ReportColorProperty InterlacedRowsColor
		{
			get
			{
				if (this.m_interlacedRowsColor == null && !this.m_chart.IsOldSnapshot && this.m_legendDef.InterlacedRowsColor != null)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo interlacedRowsColor = this.m_legendDef.InterlacedRowsColor;
					this.m_interlacedRowsColor = new ReportColorProperty(interlacedRowsColor.IsExpression, interlacedRowsColor.OriginalText, interlacedRowsColor.IsExpression ? null : new ReportColor(interlacedRowsColor.StringValue.Trim(), true), interlacedRowsColor.IsExpression ? new ReportColor("", Color.Empty, true) : null);
				}
				return this.m_interlacedRowsColor;
			}
		}

		public ReportBoolProperty EquallySpacedItems
		{
			get
			{
				if (this.m_equallySpacedItems == null && !this.m_chart.IsOldSnapshot && this.m_legendDef.EquallySpacedItems != null)
				{
					this.m_equallySpacedItems = new ReportBoolProperty(this.m_legendDef.EquallySpacedItems);
				}
				return this.m_equallySpacedItems;
			}
		}

		public ReportEnumProperty<ChartAutoBool> Reversed
		{
			get
			{
				if (this.m_reversed == null && !this.m_chart.IsOldSnapshot && this.m_legendDef.Reversed != null)
				{
					this.m_reversed = new ReportEnumProperty<ChartAutoBool>(this.m_legendDef.Reversed.IsExpression, this.m_legendDef.Reversed.OriginalText, EnumTranslator.TranslateChartAutoBool(this.m_legendDef.Reversed.StringValue, null));
				}
				return this.m_reversed;
			}
		}

		public ReportIntProperty MaxAutoSize
		{
			get
			{
				if (this.m_maxAutoSize == null && !this.m_chart.IsOldSnapshot && this.m_legendDef.MaxAutoSize != null)
				{
					this.m_maxAutoSize = new ReportIntProperty(this.m_legendDef.MaxAutoSize.IsExpression, this.m_legendDef.MaxAutoSize.OriginalText, this.m_legendDef.MaxAutoSize.IntValue, 50);
				}
				return this.m_maxAutoSize;
			}
		}

		public ReportIntProperty TextWrapThreshold
		{
			get
			{
				if (this.m_textWrapThreshold == null && !this.m_chart.IsOldSnapshot && this.m_legendDef.TextWrapThreshold != null)
				{
					this.m_textWrapThreshold = new ReportIntProperty(this.m_legendDef.TextWrapThreshold.IsExpression, this.m_legendDef.TextWrapThreshold.OriginalText, this.m_legendDef.TextWrapThreshold.IntValue, 25);
				}
				return this.m_textWrapThreshold;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegend ChartLegendDef
		{
			get
			{
				return this.m_legendDef;
			}
		}

		internal Chart ChartDef
		{
			get
			{
				return this.m_chart;
			}
		}

		public ChartLegendInstance Instance
		{
			get
			{
				if (this.m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (base.m_instance == null)
				{
					base.m_instance = new ChartLegendInstance(this);
				}
				return base.m_instance;
			}
		}

		internal ChartLegend(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegend legendDef, Chart chart)
		{
			this.m_chart = chart;
			this.m_legendDef = legendDef;
		}

		internal ChartLegend(Legend renderLegendDef, object[] styleValues, Chart chart)
		{
			this.m_chart = chart;
			this.m_renderLegendDef = renderLegendDef;
			this.m_styleValues = styleValues;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			if (this.m_chartLegendCustomItems != null)
			{
				this.m_chartLegendCustomItems.SetNewContext();
			}
			if (this.m_chartLegendColumns != null)
			{
				this.m_chartLegendColumns.SetNewContext();
			}
			if (this.m_legendTitle != null)
			{
				this.m_legendTitle.SetNewContext();
			}
			if (this.m_chartElementPosition != null)
			{
				this.m_chartElementPosition.SetNewContext();
			}
		}
	}
}
