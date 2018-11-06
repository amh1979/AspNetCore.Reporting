using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartDataPointValuesInstance : BaseInstance
	{
		private ChartDataPointValues m_chartDataPointValuesDef;

		private object m_x;

		private object m_y;

		private object m_size;

		private object m_high;

		private object m_low;

		private object m_start;

		private object m_end;

		private object m_mean;

		private object m_median;

		private object m_highlightX;

		private object m_highlightY;

		private object m_highlightSize;

		private string m_formatX;

		private string m_formatY;

		private string m_formatSize;

		private string m_currencyLanguageX;

		private string m_currencyLanguageY;

		private string m_currencyLanguageSize;

		private List<string> m_fieldsUsedInValues;

		private bool m_fieldsUsedInValuesEvaluated;

		public object X
		{
			get
			{
				if (this.m_x == null)
				{
					if (this.m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
					{
						DataValue dataValue = this.m_chartDataPointValuesDef.GetDataValue("X");
						if (dataValue != null)
						{
							this.m_x = dataValue.Instance.Value;
						}
					}
					else
					{
						this.m_x = this.m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateX(this.ReportScopeInstance, this.m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext).Value;
					}
				}
				return this.m_x;
			}
		}

		public object Y
		{
			get
			{
				if (this.m_y == null)
				{
					if (this.m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
					{
						DataValue dataValue = this.m_chartDataPointValuesDef.GetDataValue("Y");
						if (dataValue != null)
						{
							this.m_y = dataValue.Instance.Value;
						}
					}
					else
					{
						this.m_y = this.m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateY(this.ReportScopeInstance, this.m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext).Value;
					}
				}
				return this.m_y;
			}
		}

		public object Size
		{
			get
			{
				if (this.m_size == null)
				{
					if (this.m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
					{
						DataValue dataValue = this.m_chartDataPointValuesDef.GetDataValue("Size");
						if (dataValue != null)
						{
							this.m_size = dataValue.Instance.Value;
						}
					}
					else
					{
						this.m_size = this.m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateSize(this.ReportScopeInstance, this.m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext).Value;
					}
				}
				return this.m_size;
			}
		}

		public object High
		{
			get
			{
				if (this.m_high == null)
				{
					if (this.m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
					{
						DataValue dataValue = this.m_chartDataPointValuesDef.GetDataValue("High");
						if (dataValue != null)
						{
							this.m_high = dataValue.Instance.Value;
						}
					}
					else
					{
						this.m_high = this.m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateHigh(this.ReportScopeInstance, this.m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext).Value;
					}
				}
				return this.m_high;
			}
		}

		public object Low
		{
			get
			{
				if (this.m_low == null)
				{
					if (this.m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
					{
						DataValue dataValue = this.m_chartDataPointValuesDef.GetDataValue("Low");
						if (dataValue != null)
						{
							this.m_low = dataValue.Instance.Value;
						}
					}
					else
					{
						this.m_low = this.m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateLow(this.ReportScopeInstance, this.m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext).Value;
					}
				}
				return this.m_low;
			}
		}

		public object Start
		{
			get
			{
				if (this.m_start == null)
				{
					if (this.m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
					{
						DataValue dataValue = this.m_chartDataPointValuesDef.GetDataValue("Open");
						if (dataValue != null)
						{
							this.m_start = dataValue.Instance.Value;
						}
					}
					else
					{
						this.m_start = this.m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateStart(this.ReportScopeInstance, this.m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext).Value;
					}
				}
				return this.m_start;
			}
		}

		public object End
		{
			get
			{
				if (this.m_end == null)
				{
					if (this.m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
					{
						DataValue dataValue = this.m_chartDataPointValuesDef.GetDataValue("Close");
						if (dataValue != null)
						{
							this.m_end = dataValue.Instance.Value;
						}
					}
					else
					{
						this.m_end = this.m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateEnd(this.ReportScopeInstance, this.m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext).Value;
					}
				}
				return this.m_end;
			}
		}

		public object Mean
		{
			get
			{
				if (this.m_mean == null && !this.m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
				{
					this.m_mean = this.m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateMean(this.ReportScopeInstance, this.m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext).Value;
				}
				return this.m_mean;
			}
		}

		public object Median
		{
			get
			{
				if (this.m_median == null && !this.m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
				{
					this.m_median = this.m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateMedian(this.ReportScopeInstance, this.m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext).Value;
				}
				return this.m_median;
			}
		}

		public object HighlightX
		{
			get
			{
				if (this.m_highlightX == null && !this.m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
				{
					this.m_highlightX = this.m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateHighlightX(this.ReportScopeInstance, this.m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext).Value;
				}
				return this.m_highlightX;
			}
		}

		public object HighlightY
		{
			get
			{
				if (this.m_highlightY == null && !this.m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
				{
					this.m_highlightY = this.m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateHighlightY(this.ReportScopeInstance, this.m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext).Value;
				}
				return this.m_highlightY;
			}
		}

		public object HighlightSize
		{
			get
			{
				if (this.m_highlightSize == null && !this.m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
				{
					this.m_highlightSize = this.m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateHighlightSize(this.ReportScopeInstance, this.m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext).Value;
				}
				return this.m_highlightSize;
			}
		}

		public string FormatX
		{
			get
			{
				if (this.m_formatX == null && !this.m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
				{
					this.m_formatX = this.m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateFormatX(this.ReportScopeInstance, this.m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_formatX;
			}
		}

		public string FormatY
		{
			get
			{
				if (this.m_formatY == null && !this.m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
				{
					this.m_formatY = this.m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateFormatY(this.ReportScopeInstance, this.m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_formatY;
			}
		}

		public string FormatSize
		{
			get
			{
				if (this.m_formatSize == null && !this.m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
				{
					this.m_formatSize = this.m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateFormatSize(this.ReportScopeInstance, this.m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_formatSize;
			}
		}

		public string CurrencyLanguageX
		{
			get
			{
				if (this.m_currencyLanguageX == null && !this.m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
				{
					this.m_currencyLanguageX = this.m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateCurrencyLanguageX(this.ReportScopeInstance, this.m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_currencyLanguageX;
			}
		}

		public string CurrencyLanguageY
		{
			get
			{
				if (this.m_currencyLanguageY == null && !this.m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
				{
					this.m_currencyLanguageY = this.m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateCurrencyLanguageY(this.ReportScopeInstance, this.m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_currencyLanguageY;
			}
		}

		public string CurrencyLanguageSize
		{
			get
			{
				if (this.m_currencyLanguageSize == null && !this.m_chartDataPointValuesDef.ChartDef.IsOldSnapshot)
				{
					this.m_currencyLanguageSize = this.m_chartDataPointValuesDef.ChartDataPointValuesDef.EvaluateCurrencyLanguageSize(this.ReportScopeInstance, this.m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_currencyLanguageSize;
			}
		}

		internal ChartDataPointValuesInstance(ChartDataPointValues chartDataPointValuesDef)
			: base(chartDataPointValuesDef.ChartDataPoint)
		{
			this.m_chartDataPointValuesDef = chartDataPointValuesDef;
		}

		protected override void ResetInstanceCache()
		{
			this.m_x = null;
			this.m_y = null;
			this.m_size = null;
			this.m_high = null;
			this.m_low = null;
			this.m_start = null;
			this.m_end = null;
			this.m_mean = null;
			this.m_median = null;
			this.m_highlightX = null;
			this.m_highlightY = null;
			this.m_highlightSize = null;
			this.m_formatX = null;
			this.m_formatY = null;
			this.m_formatSize = null;
			this.m_currencyLanguageX = null;
			this.m_currencyLanguageY = null;
			this.m_currencyLanguageSize = null;
			this.m_fieldsUsedInValuesEvaluated = false;
			this.m_fieldsUsedInValues = null;
		}

		internal List<string> GetFieldsUsedInValues()
		{
			if (!this.m_fieldsUsedInValuesEvaluated)
			{
				this.m_fieldsUsedInValuesEvaluated = true;
				AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPointDef = this.m_chartDataPointValuesDef.ChartDataPoint.DataPointDef;
				if (dataPointDef.Action != null && dataPointDef.Action.TrackFieldsUsedInValueExpression)
				{
					this.m_fieldsUsedInValues = new List<string>();
					ObjectModelImpl reportObjectModel = this.m_chartDataPointValuesDef.ChartDef.RenderingContext.OdpContext.ReportObjectModel;
					reportObjectModel.ResetFieldsUsedInExpression();
					ReportVariantProperty x = this.m_chartDataPointValuesDef.X;
					if (x != null && x.IsExpression)
					{
						object x2 = this.X;
					}
					x = this.m_chartDataPointValuesDef.Y;
					if (x != null && x.IsExpression)
					{
						object y = this.Y;
					}
					x = this.m_chartDataPointValuesDef.Size;
					if (x != null && x.IsExpression)
					{
						object size = this.Size;
					}
					x = this.m_chartDataPointValuesDef.High;
					if (x != null && x.IsExpression)
					{
						object high = this.High;
					}
					x = this.m_chartDataPointValuesDef.Low;
					if (x != null && x.IsExpression)
					{
						object low = this.Low;
					}
					x = this.m_chartDataPointValuesDef.Start;
					if (x != null && x.IsExpression)
					{
						object start = this.Start;
					}
					x = this.m_chartDataPointValuesDef.End;
					if (x != null && x.IsExpression)
					{
						object end = this.End;
					}
					x = this.m_chartDataPointValuesDef.Mean;
					if (x != null && x.IsExpression)
					{
						object mean = this.Mean;
					}
					x = this.m_chartDataPointValuesDef.Median;
					if (x != null && x.IsExpression)
					{
						object median = this.Median;
					}
					reportObjectModel.AddFieldsUsedInExpression(this.m_fieldsUsedInValues);
				}
			}
			return this.m_fieldsUsedInValues;
		}
	}
}
