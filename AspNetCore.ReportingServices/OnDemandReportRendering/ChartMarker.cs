using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartMarker : IROMStyleDefinitionContainer
	{
		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartMarker m_markerDef;

		private Chart m_chart;

		private ChartMarkerInstance m_instance;

		private Style m_style;

		private ReportSizeProperty m_size;

		private ReportEnumProperty<ChartMarkerTypes> m_type;

		private ChartDataPoint m_dataPoint;

		private InternalChartSeries m_chartSeries;

		public Style Style
		{
			get
			{
				if (this.m_style == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_style = new Style(this.m_dataPoint.RenderDataPointDef.MarkerStyleClass, this.m_dataPoint.RenderItem.InstanceInfo.MarkerStyleAttributeValues, this.m_chart.RenderingContext);
					}
					else if (this.m_markerDef.StyleClass != null)
					{
						this.m_style = new Style(this.m_chart, this.ReportScope, this.m_markerDef, this.m_chart.RenderingContext);
					}
				}
				return this.m_style;
			}
		}

		public ReportEnumProperty<ChartMarkerTypes> Type
		{
			get
			{
				if (this.m_type == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						ChartMarkerTypes value = ChartMarkerTypes.None;
						switch (this.m_dataPoint.RenderDataPointDef.MarkerType)
						{
						case AspNetCore.ReportingServices.ReportProcessing.ChartDataPoint.MarkerTypes.Auto:
							value = ChartMarkerTypes.Auto;
							break;
						case AspNetCore.ReportingServices.ReportProcessing.ChartDataPoint.MarkerTypes.Circle:
							value = ChartMarkerTypes.Circle;
							break;
						case AspNetCore.ReportingServices.ReportProcessing.ChartDataPoint.MarkerTypes.Cross:
							value = ChartMarkerTypes.Cross;
							break;
						case AspNetCore.ReportingServices.ReportProcessing.ChartDataPoint.MarkerTypes.Diamond:
							value = ChartMarkerTypes.Diamond;
							break;
						case AspNetCore.ReportingServices.ReportProcessing.ChartDataPoint.MarkerTypes.None:
							value = ChartMarkerTypes.None;
							break;
						case AspNetCore.ReportingServices.ReportProcessing.ChartDataPoint.MarkerTypes.Square:
							value = ChartMarkerTypes.Square;
							break;
						case AspNetCore.ReportingServices.ReportProcessing.ChartDataPoint.MarkerTypes.Triangle:
							value = ChartMarkerTypes.Triangle;
							break;
						}
						this.m_type = new ReportEnumProperty<ChartMarkerTypes>(value);
					}
					else if (this.m_markerDef.Type != null)
					{
						this.m_type = new ReportEnumProperty<ChartMarkerTypes>(this.m_markerDef.Type.IsExpression, this.m_markerDef.Type.OriginalText, EnumTranslator.TranslateChartMarkerType(this.m_markerDef.Type.StringValue, null));
					}
				}
				return this.m_type;
			}
		}

		public ReportSizeProperty Size
		{
			get
			{
				if (this.m_size == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						string text = (this.m_dataPoint.RenderDataPointDef.MarkerSize == null) ? "5pt" : this.m_dataPoint.RenderDataPointDef.MarkerSize;
						this.m_size = new ReportSizeProperty(false, text, new ReportSize(text));
					}
					else if (this.m_markerDef.Size != null)
					{
						this.m_size = new ReportSizeProperty(this.m_markerDef.Size, new ReportSize("5pt"));
					}
				}
				return this.m_size;
			}
		}

		internal Chart ChartDef
		{
			get
			{
				return this.m_chart;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ChartMarker MarkerDef
		{
			get
			{
				return this.m_markerDef;
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

		public ChartMarkerInstance Instance
		{
			get
			{
				if (this.m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new ChartMarkerInstance(this);
				}
				return this.m_instance;
			}
		}

		internal ChartMarker(ChartDataPoint dataPoint, AspNetCore.ReportingServices.ReportIntermediateFormat.ChartMarker markerDef, Chart chart)
			: this(markerDef, chart)
		{
			this.m_dataPoint = dataPoint;
		}

		internal ChartMarker(InternalChartSeries chartSeries, AspNetCore.ReportingServices.ReportIntermediateFormat.ChartMarker markerDef, Chart chart)
			: this(markerDef, chart)
		{
			this.m_chartSeries = chartSeries;
		}

		internal ChartMarker(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartMarker markerDef, Chart chart)
		{
			this.m_markerDef = markerDef;
			this.m_chart = chart;
		}

		internal ChartMarker(ChartDataPoint dataPoint, Chart chart)
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
		}
	}
}
