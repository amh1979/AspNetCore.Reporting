namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class ChartSeries : ReportElementCollectionBase<ChartDataPoint>, IDataRegionRow, IROMStyleDefinitionContainer
	{
		protected Chart m_chart;

		protected int m_seriesIndex;

		protected ChartDataPoint[] m_chartDataPoints;

		public abstract string Name
		{
			get;
		}

		public abstract Style Style
		{
			get;
		}

		internal abstract ActionInfo ActionInfo
		{
			get;
		}

		public abstract ReportEnumProperty<ChartSeriesType> Type
		{
			get;
		}

		public abstract ReportEnumProperty<ChartSeriesSubtype> Subtype
		{
			get;
		}

		public abstract ChartEmptyPoints EmptyPoints
		{
			get;
		}

		public abstract ChartSmartLabel SmartLabel
		{
			get;
		}

		public abstract ReportStringProperty LegendName
		{
			get;
		}

		internal abstract ReportStringProperty LegendText
		{
			get;
		}

		internal abstract ReportBoolProperty HideInLegend
		{
			get;
		}

		public abstract ReportStringProperty ChartAreaName
		{
			get;
		}

		public abstract ReportStringProperty ValueAxisName
		{
			get;
		}

		public abstract ReportStringProperty CategoryAxisName
		{
			get;
		}

		public abstract CustomPropertyCollection CustomProperties
		{
			get;
		}

		public abstract ChartDataLabel DataLabel
		{
			get;
		}

		public abstract ChartMarker Marker
		{
			get;
		}

		internal abstract ReportStringProperty ToolTip
		{
			get;
		}

		public abstract ReportBoolProperty Hidden
		{
			get;
		}

		public abstract ChartItemInLegend ChartItemInLegend
		{
			get;
		}

		public abstract ChartSeriesInstance Instance
		{
			get;
		}

		internal ChartSeries(Chart chart, int seriesIndex)
		{
			this.m_chart = chart;
			this.m_seriesIndex = seriesIndex;
		}

		IDataRegionCell IDataRegionRow.GetIfExists(int categoryIndex)
		{
			if (this.m_chartDataPoints != null && categoryIndex >= 0 && categoryIndex < this.Count)
			{
				return this.m_chartDataPoints[categoryIndex];
			}
			return null;
		}

		internal abstract void SetNewContext();
	}
}
