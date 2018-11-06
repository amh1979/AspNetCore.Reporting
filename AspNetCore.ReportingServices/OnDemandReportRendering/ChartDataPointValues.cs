using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartDataPointValues
	{
		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPointValues m_chartDataPointValuesDef;

		private Chart m_chart;

		private ChartDataPointValuesInstance m_instance;

		private ReportVariantProperty m_x;

		private ReportVariantProperty m_y;

		private ReportVariantProperty m_size;

		private ReportVariantProperty m_high;

		private ReportVariantProperty m_low;

		private ReportVariantProperty m_start;

		private ReportVariantProperty m_end;

		private ReportVariantProperty m_mean;

		private ReportVariantProperty m_median;

		private ReportVariantProperty m_highlightX;

		private ReportVariantProperty m_highlightY;

		private ReportVariantProperty m_highlightSize;

		private ReportStringProperty m_formatX;

		private ReportStringProperty m_formatY;

		private ReportStringProperty m_formatSize;

		private ReportStringProperty m_currencyLanguageX;

		private ReportStringProperty m_currencyLanguageY;

		private ReportStringProperty m_currencyLanguageSize;

		private ChartDataPoint m_dataPoint;

		public ReportVariantProperty X
		{
			get
			{
				if (this.m_x == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						DataValue dataValue = this.GetDataValue("X");
						if (dataValue != null)
						{
							this.m_x = dataValue.Value;
						}
					}
					else if (this.m_chartDataPointValuesDef.X != null)
					{
						this.m_x = new ReportVariantProperty(this.m_chartDataPointValuesDef.X);
					}
				}
				return this.m_x;
			}
		}

		public ReportVariantProperty Y
		{
			get
			{
				if (this.m_y == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						DataValue dataValue = this.GetDataValue("Y");
						if (dataValue != null)
						{
							this.m_y = dataValue.Value;
						}
					}
					else if (this.m_chartDataPointValuesDef.Y != null)
					{
						this.m_y = new ReportVariantProperty(this.m_chartDataPointValuesDef.Y);
					}
				}
				return this.m_y;
			}
		}

		public ReportVariantProperty Size
		{
			get
			{
				if (this.m_size == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						DataValue dataValue = this.GetDataValue("Size");
						if (dataValue != null)
						{
							this.m_size = dataValue.Value;
						}
					}
					else if (this.m_chartDataPointValuesDef.Size != null)
					{
						this.m_size = new ReportVariantProperty(this.m_chartDataPointValuesDef.Size);
					}
				}
				return this.m_size;
			}
		}

		public ReportVariantProperty High
		{
			get
			{
				if (this.m_high == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						DataValue dataValue = this.GetDataValue("High");
						if (dataValue != null)
						{
							this.m_high = dataValue.Value;
						}
					}
					else if (this.m_chartDataPointValuesDef.High != null)
					{
						this.m_high = new ReportVariantProperty(this.m_chartDataPointValuesDef.High);
					}
				}
				return this.m_high;
			}
		}

		public ReportVariantProperty Low
		{
			get
			{
				if (this.m_low == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						DataValue dataValue = this.GetDataValue("Low");
						if (dataValue != null)
						{
							this.m_low = dataValue.Value;
						}
					}
					else if (this.m_chartDataPointValuesDef.Low != null)
					{
						this.m_low = new ReportVariantProperty(this.m_chartDataPointValuesDef.Low);
					}
				}
				return this.m_low;
			}
		}

		public ReportVariantProperty Start
		{
			get
			{
				if (this.m_start == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						DataValue dataValue = this.GetDataValue("Open");
						if (dataValue != null)
						{
							this.m_start = dataValue.Value;
						}
					}
					else if (this.m_chartDataPointValuesDef.Start != null)
					{
						this.m_start = new ReportVariantProperty(this.m_chartDataPointValuesDef.Start);
					}
				}
				return this.m_start;
			}
		}

		public ReportVariantProperty End
		{
			get
			{
				if (this.m_end == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						DataValue dataValue = this.GetDataValue("Close");
						if (dataValue != null)
						{
							this.m_end = dataValue.Value;
						}
					}
					else if (this.m_chartDataPointValuesDef.End != null)
					{
						this.m_end = new ReportVariantProperty(this.m_chartDataPointValuesDef.End);
					}
				}
				return this.m_end;
			}
		}

		public ReportVariantProperty Mean
		{
			get
			{
				if (this.m_mean == null && !this.m_chart.IsOldSnapshot && this.m_chartDataPointValuesDef.Mean != null)
				{
					this.m_mean = new ReportVariantProperty(this.m_chartDataPointValuesDef.Mean);
				}
				return this.m_mean;
			}
		}

		public ReportVariantProperty Median
		{
			get
			{
				if (this.m_median == null && !this.m_chart.IsOldSnapshot && this.m_chartDataPointValuesDef.Median != null)
				{
					this.m_median = new ReportVariantProperty(this.m_chartDataPointValuesDef.Median);
				}
				return this.m_median;
			}
		}

		public ReportVariantProperty HighlightX
		{
			get
			{
				if (this.m_highlightX == null && !this.m_chart.IsOldSnapshot && this.m_chartDataPointValuesDef.HighlightX != null)
				{
					this.m_highlightX = new ReportVariantProperty(this.m_chartDataPointValuesDef.HighlightX);
				}
				return this.m_highlightX;
			}
		}

		public ReportVariantProperty HighlightY
		{
			get
			{
				if (this.m_highlightY == null && !this.m_chart.IsOldSnapshot && this.m_chartDataPointValuesDef.HighlightY != null)
				{
					this.m_highlightY = new ReportVariantProperty(this.m_chartDataPointValuesDef.HighlightY);
				}
				return this.m_highlightY;
			}
		}

		public ReportVariantProperty HighlightSize
		{
			get
			{
				if (this.m_highlightSize == null && !this.m_chart.IsOldSnapshot && this.m_chartDataPointValuesDef.HighlightSize != null)
				{
					this.m_highlightSize = new ReportVariantProperty(this.m_chartDataPointValuesDef.HighlightSize);
				}
				return this.m_highlightSize;
			}
		}

		public ReportStringProperty FormatX
		{
			get
			{
				if (this.m_formatX == null && !this.m_chart.IsOldSnapshot && this.m_chartDataPointValuesDef.FormatX != null)
				{
					this.m_formatX = new ReportStringProperty(this.m_chartDataPointValuesDef.FormatX);
				}
				return this.m_formatX;
			}
		}

		public ReportStringProperty FormatY
		{
			get
			{
				if (this.m_formatY == null && !this.m_chart.IsOldSnapshot && this.m_chartDataPointValuesDef.FormatY != null)
				{
					this.m_formatY = new ReportStringProperty(this.m_chartDataPointValuesDef.FormatY);
				}
				return this.m_formatY;
			}
		}

		public ReportStringProperty FormatSize
		{
			get
			{
				if (this.m_formatSize == null && !this.m_chart.IsOldSnapshot && this.m_chartDataPointValuesDef.FormatSize != null)
				{
					this.m_formatSize = new ReportStringProperty(this.m_chartDataPointValuesDef.FormatSize);
				}
				return this.m_formatSize;
			}
		}

		public ReportStringProperty CurrencyLanguageX
		{
			get
			{
				if (this.m_currencyLanguageX == null && !this.m_chart.IsOldSnapshot && this.m_chartDataPointValuesDef.CurrencyLanguageX != null)
				{
					this.m_currencyLanguageX = new ReportStringProperty(this.m_chartDataPointValuesDef.CurrencyLanguageX);
				}
				return this.m_currencyLanguageX;
			}
		}

		public ReportStringProperty CurrencyLanguageY
		{
			get
			{
				if (this.m_currencyLanguageY == null && !this.m_chart.IsOldSnapshot && this.m_chartDataPointValuesDef.CurrencyLanguageY != null)
				{
					this.m_currencyLanguageY = new ReportStringProperty(this.m_chartDataPointValuesDef.CurrencyLanguageY);
				}
				return this.m_currencyLanguageY;
			}
		}

		public ReportStringProperty CurrencyLanguageSize
		{
			get
			{
				if (this.m_currencyLanguageSize == null && !this.m_chart.IsOldSnapshot && this.m_chartDataPointValuesDef.CurrencyLanguageSize != null)
				{
					this.m_currencyLanguageSize = new ReportStringProperty(this.m_chartDataPointValuesDef.CurrencyLanguageSize);
				}
				return this.m_currencyLanguageSize;
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

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPointValues ChartDataPointValuesDef
		{
			get
			{
				return this.m_chartDataPointValuesDef;
			}
		}

		public ChartDataPointValuesInstance Instance
		{
			get
			{
				if (this.m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new ChartDataPointValuesInstance(this);
				}
				return this.m_instance;
			}
		}

		internal ChartDataPointValues(ChartDataPoint dataPoint, AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPointValues chartDataPointValuesDef, Chart chart)
		{
			this.m_dataPoint = dataPoint;
			this.m_chartDataPointValuesDef = chartDataPointValuesDef;
			this.m_chart = chart;
		}

		internal ChartDataPointValues(ChartDataPoint dataPoint, Chart chart)
		{
			this.m_dataPoint = dataPoint;
			this.m_chart = chart;
		}

		internal DataValue GetDataValue(string propertyName)
		{
			DataValueCollection dataValues = ((ShimChartDataPoint)this.m_dataPoint).DataValues;
			try
			{
				return dataValues[propertyName];
			}
			catch (Exception e)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(e))
				{
					throw;
				}
				return null;
			}
		}

		internal void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
		}
	}
}
