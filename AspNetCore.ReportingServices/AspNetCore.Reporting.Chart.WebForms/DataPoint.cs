using AspNetCore.Reporting.Chart.WebForms.Design;
using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeDataPoint_DataPoint")]
	[DefaultProperty("YValues")]
	internal class DataPoint : DataPointAttributes
	{
		private double xValue;

		private double[] yValue = new double[1];

		internal PointF positionRel = PointF.Empty;

		[Bindable(true)]
		[SRDescription("DescriptionAttributeDataPoint_XValue")]
		[TypeConverter(typeof(DataPointValueConverter))]
		[Browsable(false)]
		[SRCategory("CategoryAttributeData")]
		[DefaultValue(typeof(double), "0.0")]
		public double XValue
		{
			get
			{
				return this.xValue;
			}
			set
			{
				this.xValue = value;
				base.Invalidate(false);
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(DoubleArrayConverter))]
		[Browsable(false)]
		[SerializationVisibility(SerializationVisibility.Attribute)]
		[SRCategory("CategoryAttributeData")]
		[SRDescription("DescriptionAttributeDataPoint_YValues")]
		[Bindable(true)]
		public double[] YValues
		{
			get
			{
				return this.yValue;
			}
			set
			{
				if (value == null)
				{
					for (int i = 0; i < this.yValue.Length; i++)
					{
						this.yValue[i] = 0.0;
					}
				}
				else
				{
					this.yValue = value;
				}
				base.Invalidate(false);
			}
		}

		[SRDescription("DescriptionAttributeDataPoint_Empty")]
		[Bindable(true)]
		[DefaultValue(false)]
		[SRCategory("CategoryAttributeData")]
		[Browsable(false)]
		public bool Empty
		{
			get
			{
				return base.emptyPoint;
			}
			set
			{
				base.emptyPoint = value;
				base.Invalidate(true);
			}
		}

		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Bindable(true)]
		[SRCategory("CategoryAttributeData")]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeDataPoint_Name")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Name
		{
			get
			{
				return "DataPoint";
			}
			set
			{
			}
		}

		[Browsable(false)]
		[SRDescription("DescriptionAttributeMarkerStyle4")]
		[SRCategory("CategoryAttributeMarker")]
		[RefreshProperties(RefreshProperties.All)]
		[Bindable(true)]
		public new MarkerStyle MarkerStyle
		{
			get
			{
				return base.MarkerStyle;
			}
			set
			{
				base.MarkerStyle = value;
			}
		}

		[SRCategory("CategoryAttributeMarker")]
		[SRDescription("DescriptionAttributeMarkerSize")]
		[RefreshProperties(RefreshProperties.All)]
		[Browsable(false)]
		[Bindable(true)]
		[DefaultValue(5)]
		public new int MarkerSize
		{
			get
			{
				return base.MarkerSize;
			}
			set
			{
				base.MarkerSize = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[SRDescription("DescriptionAttributeMarkerColor3")]
		[DefaultValue(typeof(Color), "")]
		[Browsable(false)]
		[SRCategory("CategoryAttributeMarker")]
		[Bindable(true)]
		public new Color MarkerColor
		{
			get
			{
				return base.MarkerColor;
			}
			set
			{
				base.MarkerColor = value;
			}
		}

		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeMarkerBorderColor")]
		[Browsable(false)]
		[SRCategory("CategoryAttributeMarker")]
		[Bindable(true)]
		[RefreshProperties(RefreshProperties.All)]
		public new Color MarkerBorderColor
		{
			get
			{
				return base.MarkerBorderColor;
			}
			set
			{
				base.MarkerBorderColor = value;
			}
		}

		[Browsable(false)]
		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeColor4")]
		public new Color Color
		{
			get
			{
				return base.Color;
			}
			set
			{
				base.Color = value;
			}
		}

		[SRDescription("DescriptionAttributeBorderColor9")]
		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		public new Color BorderColor
		{
			get
			{
				return base.BorderColor;
			}
			set
			{
				base.BorderColor = value;
			}
		}

		[DefaultValue(ChartDashStyle.Solid)]
		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeBorderStyle3")]
		public new ChartDashStyle BorderStyle
		{
			get
			{
				return base.BorderStyle;
			}
			set
			{
				base.BorderStyle = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[DefaultValue(1)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeBorderWidth8")]
		public new int BorderWidth
		{
			get
			{
				return base.BorderWidth;
			}
			set
			{
				base.BorderWidth = value;
			}
		}

		[Browsable(false)]
		[DefaultValue(GradientType.None)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeBackGradientType4")]
		public new GradientType BackGradientType
		{
			get
			{
				return base.BackGradientType;
			}
			set
			{
				base.BackGradientType = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeBackGradientEndColor7")]
		[DefaultValue(typeof(Color), "")]
		public new Color BackGradientEndColor
		{
			get
			{
				return base.BackGradientEndColor;
			}
			set
			{
				base.BackGradientEndColor = value;
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeBackHatchStyle9")]
		[DefaultValue(ChartHatchStyle.None)]
		public new ChartHatchStyle BackHatchStyle
		{
			get
			{
				return base.BackHatchStyle;
			}
			set
			{
				base.BackHatchStyle = value;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeLabelAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeFont")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt")]
		public new Font Font
		{
			get
			{
				return base.Font;
			}
			set
			{
				base.Font = value;
			}
		}

		[SRCategory("CategoryAttributeLabelAppearance")]
		[Browsable(false)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeFontColor")]
		[DefaultValue(typeof(Color), "Black")]
		public new Color FontColor
		{
			get
			{
				return base.FontColor;
			}
			set
			{
				base.FontColor = value;
			}
		}

		[DefaultValue("")]
		[Browsable(false)]
		[SRCategory("CategoryAttributeLabel")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLabel")]
		public override string Label
		{
			get
			{
				return base.Label;
			}
			set
			{
				base.Label = value;
			}
		}

		public DataPoint()
			: base(null, true)
		{
			this.yValue = new double[1];
		}

		public DataPoint(Series series)
			: base(series, true)
		{
			this.yValue = new double[series.YValuesPerPoint];
			this.xValue = 0.0;
		}

		public DataPoint(double xValue, string yValues)
			: base(null, true)
		{
			string[] array = yValues.Split(',');
			this.yValue = new double[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				this.yValue[i] = CommonElements.ParseDouble(array[i]);
			}
			this.xValue = xValue;
		}

		public DataPoint(double xValue, double yValue)
			: base(null, true)
		{
			this.yValue = new double[1];
			this.yValue[0] = yValue;
			this.xValue = xValue;
		}

		internal void SetPointAttribute(object obj, string attributeName, string format)
		{
			string text = obj as string;
			if (text == null)
			{
				double num = double.NaN;
				ChartValueTypes valueType = ChartValueTypes.Auto;
				if (obj is DateTime)
				{
					num = ((DateTime)obj).ToOADate();
					valueType = ChartValueTypes.Date;
				}
				else
				{
					num = this.ConvertValue(obj);
				}
				if (!double.IsNaN(num))
				{
					try
					{
						text = ValueConverter.FormatValue(base.series.chart, this, num, format, valueType, ChartElementType.DataPoint);
					}
					catch
					{
						text = obj.ToString();
					}
				}
				else
				{
					text = obj.ToString();
				}
			}
			if (text.Length > 0)
			{
				if (string.Compare(attributeName, "AxisLabel", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.AxisLabel = text;
				}
				else if (string.Compare(attributeName, "Tooltip", StringComparison.OrdinalIgnoreCase) == 0)
				{
					base.ToolTip = text;
				}
				else if (string.Compare(attributeName, "Href", StringComparison.OrdinalIgnoreCase) == 0)
				{
					base.Href = text;
				}
				else if (string.Compare(attributeName, "Label", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.Label = text;
				}
				else if (string.Compare(attributeName, "LegendTooltip", StringComparison.OrdinalIgnoreCase) == 0)
				{
					base.LegendToolTip = text;
				}
				else if (string.Compare(attributeName, "LegendText", StringComparison.OrdinalIgnoreCase) == 0)
				{
					base.LegendText = text;
				}
				else if (string.Compare(attributeName, "LabelToolTip", StringComparison.OrdinalIgnoreCase) == 0)
				{
					base.LabelToolTip = text;
				}
				else
				{
					base[attributeName] = text;
				}
			}
		}

		private double ConvertValue(object value)
		{
			if (value == null)
			{
				return 0.0;
			}
			if (value is double)
			{
				return (double)value;
			}
			if (value is float)
			{
				return (double)(float)value;
			}
			if (value is decimal)
			{
				return (double)(decimal)value;
			}
			if (value is int)
			{
				return (double)(int)value;
			}
			if (value is uint)
			{
				return (double)(uint)value;
			}
			if (value is long)
			{
				return (double)(long)value;
			}
			if (value is ulong)
			{
				return (double)(ulong)value;
			}
			if (value is byte)
			{
				return (double)(int)(byte)value;
			}
			if (value is sbyte)
			{
				return (double)(sbyte)value;
			}
			if (value is bool)
			{
				if (!(bool)value)
				{
					return 0.0;
				}
				return 1.0;
			}
			string text = "";
			text = value.ToString();
			return CommonElements.ParseDouble(text);
		}

		public void SetValueXY(object xValue, params object[] yValue)
		{
			this.SetValueY(yValue);
			Type type = xValue.GetType();
			if (base.series != null)
			{
				base.series.CheckSupportedTypes(type);
			}
			if (type == typeof(string))
			{
				this.AxisLabel = (string)xValue;
			}
			else if (type == typeof(DateTime))
			{
				this.xValue = ((DateTime)xValue).ToOADate();
			}
			else
			{
				this.xValue = this.ConvertValue(xValue);
			}
			if (base.series != null && xValue is DateTime)
			{
				if (base.series.XValueType == ChartValueTypes.Date)
				{
					DateTime dateTime = new DateTime(((DateTime)xValue).Year, ((DateTime)xValue).Month, ((DateTime)xValue).Day, 0, 0, 0, 0);
					this.xValue = dateTime.ToOADate();
				}
				else if (base.series.XValueType == ChartValueTypes.Time)
				{
					DateTime dateTime2 = new DateTime(1899, 12, 30, ((DateTime)xValue).Hour, ((DateTime)xValue).Minute, ((DateTime)xValue).Second, ((DateTime)xValue).Millisecond);
					this.xValue = dateTime2.ToOADate();
				}
			}
			bool flag = false;
			double[] array = this.yValue;
			foreach (double d in array)
			{
				if (double.IsNaN(d))
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				this.Empty = true;
				for (int j = 0; j < this.yValue.Length; j++)
				{
					this.yValue[j] = 0.0;
				}
			}
		}

		public void SetValueY(params object[] yValue)
		{
			if (yValue.Length != 0 && (base.series == null || yValue.Length <= base.series.YValuesPerPoint))
			{
				for (int i = 0; i < yValue.Length; i++)
				{
					if (yValue[i] == null || yValue[i] is DBNull)
					{
						yValue[i] = 0.0;
						if (i == 0)
						{
							this.Empty = true;
						}
					}
				}
				Type type = yValue[0].GetType();
				if (base.series != null)
				{
					base.series.CheckSupportedTypes(type);
				}
				if (this.yValue.Length < yValue.Length)
				{
					this.yValue = new double[yValue.Length];
				}
				if (type == typeof(string))
				{
					try
					{
						for (int j = 0; j < yValue.Length; j++)
						{
							this.yValue[j] = CommonElements.ParseDouble((string)yValue[j]);
						}
					}
					catch
					{
						if (base.series != null && base.series.chart == null && base.series.serviceContainer != null)
						{
							base.series.chart = (Chart)base.series.serviceContainer.GetService(typeof(Chart));
						}
						if (base.series != null && base.series.chart != null && base.series.chart.chartPicture.SuppressExceptions)
						{
							this.Empty = true;
							for (int k = 0; k < yValue.Length; k++)
							{
								yValue[k] = 0.0;
							}
							goto end_IL_00ee;
						}
						throw new ArgumentException(SR.ExceptionDataPointYValueStringFormat);
						end_IL_00ee:;
					}
				}
				else if (type == typeof(DateTime))
				{
					for (int l = 0; l < yValue.Length; l++)
					{
						if (yValue[l] == null || (yValue[l] is double && (double)yValue[l] == 0.0))
						{
							this.yValue[l] = DateTime.Now.ToOADate();
						}
						else
						{
							this.yValue[l] = ((DateTime)yValue[l]).ToOADate();
						}
					}
				}
				else
				{
					for (int m = 0; m < yValue.Length; m++)
					{
						this.yValue[m] = this.ConvertValue(yValue[m]);
					}
				}
				if (base.series != null)
				{
					for (int n = 0; n < yValue.Length; n++)
					{
						if (yValue[n] == null || (yValue[n] is double && (double)yValue[n] == 0.0))
						{
							if (base.series.YValueType == ChartValueTypes.Date)
							{
								this.yValue[n] = Math.Floor(this.yValue[n]);
							}
							else if (base.series.YValueType == ChartValueTypes.Time)
							{
								this.yValue[n] = this.xValue - Math.Floor(this.yValue[n]);
							}
						}
						else if (base.series.YValueType == ChartValueTypes.Date)
						{
							DateTime dateTime = (!(yValue[n] is DateTime)) ? ((!(yValue[n] is double)) ? Convert.ToDateTime(yValue[n], CultureInfo.InvariantCulture) : DateTime.FromOADate((double)yValue[n])) : ((DateTime)yValue[n]);
							DateTime dateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0);
							this.yValue[n] = dateTime2.ToOADate();
						}
						else if (base.series.YValueType == ChartValueTypes.Time)
						{
							DateTime dateTime3;
							if (yValue[n] is DateTime)
							{
								dateTime3 = (DateTime)yValue[n];
							}
							dateTime3 = ((!(yValue[n] is double)) ? Convert.ToDateTime(yValue[n], CultureInfo.InvariantCulture) : DateTime.FromOADate((double)yValue[n]));
							DateTime dateTime4 = new DateTime(1899, 12, 30, dateTime3.Hour, dateTime3.Minute, dateTime3.Second, dateTime3.Millisecond);
							this.yValue[n] = dateTime4.ToOADate();
						}
					}
				}
				return;
			}
			throw new ArgumentOutOfRangeException("yValue", SR.ExceptionDataPointYValuesSettingCountMismatch(base.series.YValuesPerPoint.ToString(CultureInfo.InvariantCulture)));
		}

		public DataPoint Clone()
		{
			DataPoint dataPoint = new DataPoint();
			dataPoint.series = null;
			dataPoint.pointAttributes = base.pointAttributes;
			dataPoint.xValue = this.XValue;
			dataPoint.yValue = new double[this.yValue.Length];
			this.yValue.CopyTo(dataPoint.yValue, 0);
			dataPoint.tempColorIsSet = base.tempColorIsSet;
			dataPoint.emptyPoint = base.emptyPoint;
			foreach (object key in base.attributes.Keys)
			{
				dataPoint.attributes.Add(key, base.attributes[key]);
			}
			((IMapAreaAttributes)dataPoint).Tag = ((IMapAreaAttributes)this).Tag;
			return dataPoint;
		}

		internal void ResizeYValueArray(int newSize)
		{
			double[] array = new double[newSize];
			if (this.yValue != null)
			{
				for (int i = 0; i < ((this.yValue.Length < newSize) ? this.yValue.Length : newSize); i++)
				{
					array[i] = this.yValue[i];
				}
			}
			this.yValue = array;
		}

		public double GetValueY(int yValueIndex)
		{
			return this.YValues[yValueIndex];
		}

		public void SetValueY(int yValueIndex, double yValue)
		{
			this.YValues[yValueIndex] = yValue;
		}

		internal double GetValueByName(string valueName)
		{
			valueName = valueName.ToUpper(CultureInfo.InvariantCulture);
			if (string.Compare(valueName, "X", StringComparison.Ordinal) == 0)
			{
				return this.XValue;
			}
			if (valueName.StartsWith("Y", StringComparison.Ordinal))
			{
				if (valueName.Length == 1)
				{
					return this.YValues[0];
				}
				int num = 0;
				try
				{
					num = int.Parse(valueName.Substring(1), CultureInfo.InvariantCulture) - 1;
				}
				catch (Exception)
				{
					throw new ArgumentException(SR.ExceptionDataPointValueNameInvalid, "valueName");
				}
				if (num < 0)
				{
					throw new ArgumentException(SR.ExceptionDataPointValueNameYIndexIsNotPositive, "valueName");
				}
				if (num >= this.YValues.Length)
				{
					throw new ArgumentException(SR.ExceptionDataPointValueNameYIndexOutOfRange, "valueName");
				}
				return this.YValues[num];
			}
			throw new ArgumentException(SR.ExceptionDataPointValueNameInvalid, "valueName");
		}

		internal string ReplaceKeywords(string strOriginal)
		{
			if (strOriginal != null && strOriginal.Length != 0)
			{
				string text = strOriginal.Replace("\\n", "\n");
				text = text.Replace("#LABEL", this.Label);
				text = text.Replace("#LEGENDTEXT", base.LegendText);
				text = text.Replace("#AXISLABEL", this.AxisLabel);
				if (base.series != null)
				{
					text = text.Replace("#INDEX", base.series.Points.IndexOf(this).ToString(CultureInfo.InvariantCulture));
					text = base.series.ReplaceKeywords(text);
					text = base.series.ReplaceOneKeyword(base.series.chart, this, ChartElementType.DataPoint, text, "#PERCENT", this.YValues[0] / base.series.GetTotalYValue(), ChartValueTypes.Double, "P");
					text = ((base.series.XValueType != ChartValueTypes.String) ? base.series.ReplaceOneKeyword(base.series.chart, this, ChartElementType.DataPoint, text, "#VALX", this.XValue, base.series.XValueType, "") : text.Replace("#VALX", this.AxisLabel));
					for (int i = this.YValues.Length; i <= 7; i++)
					{
						text = this.RemoveOneKeyword(text, "#VALY" + (i + 1), SR.FormatErrorString);
					}
					for (int j = 1; j <= this.YValues.Length; j++)
					{
						text = base.series.ReplaceOneKeyword(base.series.chart, this, ChartElementType.DataPoint, text, "#VALY" + j, this.YValues[j - 1], base.series.YValueType, "");
					}
					text = base.series.ReplaceOneKeyword(base.series.chart, this, ChartElementType.DataPoint, text, "#VALY", this.YValues[0], base.series.YValueType, "");
					text = base.series.ReplaceOneKeyword(base.series.chart, this, ChartElementType.DataPoint, text, "#VAL", this.YValues[0], base.series.YValueType, "");
				}
				return text;
			}
			return strOriginal;
		}

		private string RemoveOneKeyword(string strOriginal, string keyword, string strToReplace)
		{
			string text = strOriginal;
			int num = -1;
			while ((num = text.IndexOf(keyword, StringComparison.Ordinal)) != -1)
			{
				int num2 = num + keyword.Length;
				if (text.Length > num2 && text[num2] == '{')
				{
					int num3 = text.IndexOf('}', num2);
					if (num3 == -1)
					{
						throw new InvalidOperationException(SR.ExceptionDataSeriesKeywordFormatInvalid(text));
					}
					num2 = num3 + 1;
				}
				text = text.Remove(num, num2 - num);
				if (!string.IsNullOrEmpty(strToReplace))
				{
					text = text.Insert(num, strToReplace);
				}
			}
			return text;
		}

		public void ResetYValues()
		{
			this.YValues = null;
		}
	}
}
