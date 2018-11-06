using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartDataLabel : IROMStyleDefinitionContainer, IROMActionOwner
	{
		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataLabel m_chartDataLabelDef;

		private Chart m_chart;

		private ChartDataLabelInstance m_instance;

		private Style m_style;

		private ReportStringProperty m_value;

		private ChartDataPoint m_dataPoint;

		private InternalChartSeries m_chartSeries;

		private ReportBoolProperty m_visible;

		private ReportEnumProperty<ChartDataLabelPositions> m_position;

		private ReportIntProperty m_rotation;

		private ActionInfo m_actionInfo;

		private ReportBoolProperty m_useValueAsLabel;

		private ReportStringProperty m_toolTip;

		public Style Style
		{
			get
			{
				if (this.m_style == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_style = new Style(this.m_dataPoint.RenderDataPointDef.DataLabel.StyleClass, this.m_dataPoint.RenderItem.InstanceInfo.DataLabelStyleAttributeValues, this.m_chart.RenderingContext);
					}
					else if (this.m_chartDataLabelDef.StyleClass != null)
					{
						this.m_style = new Style(this.m_chart, this.ReportScope, this.m_chartDataLabelDef, this.m_chart.RenderingContext);
					}
				}
				return this.m_style;
			}
		}

		public string UniqueName
		{
			get
			{
				return this.InstancePath.UniqueName + 'x' + "DataLabel";
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				if (this.m_actionInfo == null && !this.m_chart.IsOldSnapshot && this.m_chartDataLabelDef.Action != null)
				{
					this.m_actionInfo = new ActionInfo(this.m_chart.RenderingContext, this.ReportScope, this.m_chartDataLabelDef.Action, this.InstancePath, this.m_chart, ObjectType.Chart, this.m_chartDataLabelDef.Name, this);
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

		public ReportStringProperty Label
		{
			get
			{
				if (this.m_value == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_value = new ReportStringProperty(this.m_dataPoint.RenderDataPointDef.DataLabel.Value);
					}
					else if (this.m_chartDataLabelDef.Label != null)
					{
						this.m_value = new ReportStringProperty(this.m_chartDataLabelDef.Label);
					}
				}
				return this.m_value;
			}
		}

		public ReportBoolProperty UseValueAsLabel
		{
			get
			{
				if (this.m_useValueAsLabel == null && !this.m_chart.IsOldSnapshot && this.m_chartDataLabelDef.UseValueAsLabel != null)
				{
					this.m_useValueAsLabel = new ReportBoolProperty(this.m_chartDataLabelDef.UseValueAsLabel);
				}
				return this.m_useValueAsLabel;
			}
		}

		public ReportBoolProperty Visible
		{
			get
			{
				if (this.m_visible == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_visible = new ReportBoolProperty(this.m_dataPoint.RenderDataPointDef.DataLabel.Visible);
					}
					else if (this.m_chartDataLabelDef.Visible != null)
					{
						this.m_visible = new ReportBoolProperty(this.m_chartDataLabelDef.Visible);
					}
				}
				return this.m_visible;
			}
		}

		public ReportIntProperty Rotation
		{
			get
			{
				if (this.m_rotation == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_rotation = new ReportIntProperty(this.m_dataPoint.RenderDataPointDef.DataLabel.Rotation);
					}
					else if (this.m_chartDataLabelDef.Rotation != null)
					{
						this.m_rotation = new ReportIntProperty(this.m_chartDataLabelDef.Rotation);
					}
				}
				return this.m_rotation;
			}
		}

		public ReportStringProperty ToolTip
		{
			get
			{
				if (this.m_toolTip == null && !this.m_chart.IsOldSnapshot && this.m_chartDataLabelDef.ToolTip != null)
				{
					this.m_toolTip = new ReportStringProperty(this.m_chartDataLabelDef.ToolTip);
				}
				return this.m_toolTip;
			}
		}

		public ReportEnumProperty<ChartDataLabelPositions> Position
		{
			get
			{
				if (this.m_chart.IsOldSnapshot)
				{
					ChartDataLabelPositions value = ChartDataLabelPositions.Auto;
					switch (this.m_dataPoint.RenderDataPointDef.DataLabel.Position)
					{
					case AspNetCore.ReportingServices.ReportProcessing.ChartDataLabel.Positions.Auto:
						value = ChartDataLabelPositions.Auto;
						break;
					case AspNetCore.ReportingServices.ReportProcessing.ChartDataLabel.Positions.Bottom:
						value = ChartDataLabelPositions.Bottom;
						break;
					case AspNetCore.ReportingServices.ReportProcessing.ChartDataLabel.Positions.BottomLeft:
						value = ChartDataLabelPositions.BottomLeft;
						break;
					case AspNetCore.ReportingServices.ReportProcessing.ChartDataLabel.Positions.BottomRight:
						value = ChartDataLabelPositions.BottomRight;
						break;
					case AspNetCore.ReportingServices.ReportProcessing.ChartDataLabel.Positions.Center:
						value = ChartDataLabelPositions.Center;
						break;
					case AspNetCore.ReportingServices.ReportProcessing.ChartDataLabel.Positions.Left:
						value = ChartDataLabelPositions.Left;
						break;
					case AspNetCore.ReportingServices.ReportProcessing.ChartDataLabel.Positions.Right:
						value = ChartDataLabelPositions.Right;
						break;
					case AspNetCore.ReportingServices.ReportProcessing.ChartDataLabel.Positions.Top:
						value = ChartDataLabelPositions.Top;
						break;
					case AspNetCore.ReportingServices.ReportProcessing.ChartDataLabel.Positions.TopLeft:
						value = ChartDataLabelPositions.TopLeft;
						break;
					case AspNetCore.ReportingServices.ReportProcessing.ChartDataLabel.Positions.TopRight:
						value = ChartDataLabelPositions.TopRight;
						break;
					}
					this.m_position = new ReportEnumProperty<ChartDataLabelPositions>(value);
				}
				else if (this.m_chartDataLabelDef.Position != null)
				{
					this.m_position = new ReportEnumProperty<ChartDataLabelPositions>(this.m_chartDataLabelDef.Position.IsExpression, this.m_chartDataLabelDef.Position.OriginalText, EnumTranslator.TranslateChartDataLabelPosition(this.m_chartDataLabelDef.Position.StringValue, null));
				}
				return this.m_position;
			}
		}

		internal IReportScope ReportScope
		{
			get
			{
				if (this.m_dataPoint != null)
				{
					return this.m_dataPoint;
				}
				if (this.m_chartSeries != null)
				{
					return this.m_chartSeries.ReportScope;
				}
				return this.m_chart;
			}
		}

		private IInstancePath InstancePath
		{
			get
			{
				if (this.m_dataPoint != null)
				{
					return this.m_dataPoint.DataPointDef;
				}
				if (this.m_chartSeries != null)
				{
					return this.m_chartSeries.ChartSeriesDef;
				}
				return this.m_chart.ChartDef;
			}
		}

		internal Chart ChartDef
		{
			get
			{
				return this.m_chart;
			}
		}

		internal ChartDataPoint ChartDataPoint
		{
			get
			{
				return this.m_dataPoint;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataLabel ChartDataLabelDef
		{
			get
			{
				return this.m_chartDataLabelDef;
			}
		}

		public ChartDataLabelInstance Instance
		{
			get
			{
				if (this.m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new ChartDataLabelInstance(this);
				}
				return this.m_instance;
			}
		}

		internal ChartDataLabel(ChartDataPoint dataPoint, AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataLabel chartDataLabelDef, Chart chart)
		{
			this.m_dataPoint = dataPoint;
			this.m_chartDataLabelDef = chartDataLabelDef;
			this.m_chart = chart;
		}

		internal ChartDataLabel(InternalChartSeries series, AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataLabel chartDataLabelDef, Chart chart)
		{
			this.m_chartSeries = series;
			this.m_chartDataLabelDef = chartDataLabelDef;
			this.m_chart = chart;
		}

		internal ChartDataLabel(ChartDataPoint dataPoint, Chart chart)
		{
			this.m_dataPoint = dataPoint;
			this.m_chart = chart;
		}

		internal void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
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
