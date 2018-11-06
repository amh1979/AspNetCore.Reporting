using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal abstract class RuleBase : NamedElement
	{
		internal abstract string Field
		{
			get;
			set;
		}

		public abstract string Category
		{
			get;
			set;
		}

		internal abstract DataGrouping DataGrouping
		{
			get;
			set;
		}

		public abstract string FromValue
		{
			get;
			set;
		}

		public abstract string ToValue
		{
			get;
			set;
		}

		public abstract string LegendText
		{
			get;
			set;
		}

		public abstract string ShowInLegend
		{
			get;
			set;
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public override object Tag
		{
			get
			{
				return base.Tag;
			}
			set
			{
				base.Tag = value;
			}
		}

		internal RuleBase(CommonElements common)
			: base(common)
		{
		}

		internal abstract ArrayList GetSortedValues(Field field, object fromValue, object toValue);

		internal MapCore GetMapCore()
		{
			return (MapCore)this.ParentElement;
		}

		internal bool IsValueInRange(Field field, object testValue, object fromValue, object toValue)
		{
			if (testValue != null && fromValue != null && toValue != null)
			{
				if (field.Type == typeof(string))
				{
					string text = Convert.ToString(testValue, CultureInfo.InvariantCulture);
					string a = Convert.ToString(fromValue, CultureInfo.InvariantCulture);
					string b = Convert.ToString(toValue, CultureInfo.InvariantCulture);
					if (!(a == text) && !(text == b))
					{
						goto IL_022b;
					}
					return true;
				}
				if (field.Type == typeof(int))
				{
					int num = Convert.ToInt32(testValue, CultureInfo.InvariantCulture);
					int num2 = Convert.ToInt32(fromValue, CultureInfo.InvariantCulture);
					int num3 = Convert.ToInt32(toValue, CultureInfo.InvariantCulture);
					if (num2 <= num && num <= num3)
					{
						return true;
					}
				}
				else if (field.Type == typeof(double))
				{
					double num4 = Convert.ToDouble(testValue, CultureInfo.InvariantCulture);
					double num5 = Convert.ToDouble(fromValue, CultureInfo.InvariantCulture);
					double num6 = Convert.ToDouble(toValue, CultureInfo.InvariantCulture);
					if (num5 <= num4 && num4 <= num6)
					{
						return true;
					}
				}
				else if (field.Type == typeof(decimal))
				{
					decimal num7 = Convert.ToDecimal(testValue, CultureInfo.InvariantCulture);
					decimal d = Convert.ToDecimal(fromValue, CultureInfo.InvariantCulture);
					decimal d2 = Convert.ToDecimal(toValue, CultureInfo.InvariantCulture);
					if (d <= num7 && num7 <= d2)
					{
						return true;
					}
				}
				else
				{
					if (field.Type == typeof(bool))
					{
						bool flag = Convert.ToBoolean(testValue, CultureInfo.InvariantCulture);
						bool flag2 = Convert.ToBoolean(fromValue, CultureInfo.InvariantCulture);
						bool flag3 = Convert.ToBoolean(toValue, CultureInfo.InvariantCulture);
						if (flag2 != flag && flag != flag3)
						{
							goto IL_022b;
						}
						return true;
					}
					if (field.Type == typeof(DateTime))
					{
						DateTime dateTime = Convert.ToDateTime(testValue, CultureInfo.InvariantCulture);
						DateTime t = Convert.ToDateTime(fromValue, CultureInfo.InvariantCulture);
						DateTime t2 = Convert.ToDateTime(toValue, CultureInfo.InvariantCulture);
						if (t <= dateTime && dateTime <= t2)
						{
							return true;
						}
					}
					else if (field.Type == typeof(TimeSpan))
					{
						TimeSpan timeSpan = (TimeSpan)testValue;
						TimeSpan t3 = (TimeSpan)fromValue;
						TimeSpan t4 = (TimeSpan)toValue;
						if (t3 <= timeSpan && timeSpan <= t4)
						{
							return true;
						}
					}
				}
				goto IL_022b;
			}
			return false;
			IL_022b:
			return false;
		}

		internal void GetEqualIntervals(Field field, object fromValue, object toValue, int intervalCount, ref object[] fromValues, ref object[] toValues)
		{
			fromValues = new object[intervalCount];
			toValues = new object[intervalCount];
			if (field.Type == typeof(int))
			{
				int num = (int)fromValue;
				int num2 = (int)toValue;
				int num3 = 0;
				if (fromValues.Length > 0)
				{
					num3 = (int)Math.Round((double)(num2 - num) / (double)fromValues.Length);
				}
				int num4 = num;
				for (int i = 0; i < fromValues.Length; i++)
				{
					fromValues[i] = num4;
					toValues[i] = num4 + num3;
					num4 += num3;
				}
			}
			else if (field.Type == typeof(double))
			{
				decimal num5 = (decimal)(double)fromValue;
				decimal d = (decimal)(double)toValue;
				decimal d2 = 0m;
				if (fromValues.Length > 0)
				{
					d2 = (d - num5) / (decimal)fromValues.Length;
				}
				decimal num6 = num5;
				for (int j = 0; j < fromValues.Length; j++)
				{
					fromValues[j] = (double)num6;
					toValues[j] = (double)(num6 + d2);
					num6 += d2;
				}
			}
			else if (field.Type == typeof(decimal))
			{
				decimal num7 = (decimal)fromValue;
				decimal d3 = (decimal)toValue;
				decimal d4 = 0m;
				if (fromValues.Length > 0)
				{
					d4 = (d3 - num7) / (decimal)fromValues.Length;
				}
				decimal num8 = num7;
				for (int k = 0; k < fromValues.Length; k++)
				{
					fromValues[k] = num8;
					toValues[k] = num8 + d4;
					num8 += d4;
				}
			}
			else if (field.Type == typeof(DateTime))
			{
				DateTime dateTime = (DateTime)fromValue;
				DateTime d5 = (DateTime)toValue;
				TimeSpan t = (fromValues.Length <= 0) ? new TimeSpan(0, 0, 0, 0, 0) : new TimeSpan((d5 - dateTime).Ticks / fromValues.Length);
				DateTime dateTime2 = dateTime;
				for (int l = 0; l < fromValues.Length; l++)
				{
					fromValues[l] = dateTime2;
					toValues[l] = dateTime2 + t;
					dateTime2 += t;
				}
			}
			else if (field.Type == typeof(TimeSpan))
			{
				TimeSpan timeSpan = (TimeSpan)fromValue;
				TimeSpan t2 = (TimeSpan)toValue;
				TimeSpan t3 = (fromValues.Length <= 0) ? new TimeSpan(0, 0, 0, 0, 0) : new TimeSpan((t2 - timeSpan).Ticks / fromValues.Length);
				TimeSpan timeSpan2 = timeSpan;
				for (int m = 0; m < fromValues.Length; m++)
				{
					fromValues[m] = timeSpan2;
					toValues[m] = timeSpan2 + t3;
					timeSpan2 += t3;
				}
			}
			if (toValues.Length > 0)
			{
				toValues[toValues.Length - 1] = toValue;
			}
		}

		internal void GetEqualDistributionIntervals(Field field, ArrayList sortedList, object fromValue, object toValue, int intervalCount, ref object[] fromValues, ref object[] toValues)
		{
			if (sortedList.Count == 0)
			{
				this.GetEqualIntervals(field, fromValue, toValue, intervalCount, ref fromValues, ref toValues);
			}
			else
			{
				if (intervalCount > sortedList.Count)
				{
					intervalCount = sortedList.Count;
				}
				if (intervalCount < 2)
				{
					fromValue = this.GetRoundedAverage(field, fromValue, fromValue, true);
					toValue = this.GetRoundedAverage(field, toValue, toValue, false);
					this.GetEqualIntervals(field, fromValue, toValue, intervalCount, ref fromValues, ref toValues);
				}
				else
				{
					fromValues = new object[intervalCount];
					toValues = new object[intervalCount];
					double num = (double)sortedList.Count / (double)fromValues.Length;
					double num2 = num;
					fromValues[0] = this.GetRoundedAverage(field, sortedList[0], sortedList[0], true);
					object roundedAverage = this.GetRoundedAverage(field, sortedList[(int)Math.Round(num2 - 1.0)], sortedList[(int)Math.Round(num2)], true);
					toValues[0] = roundedAverage;
					for (int i = 1; i < fromValues.Length - 1; i++)
					{
						num2 += num;
						fromValues[i] = roundedAverage;
						roundedAverage = this.GetRoundedAverage(field, sortedList[(int)Math.Round(num2 - 1.0)], sortedList[(int)Math.Round(num2)], true);
						toValues[i] = roundedAverage;
					}
					fromValues[fromValues.Length - 1] = roundedAverage;
					toValues[fromValues.Length - 1] = this.GetRoundedAverage(field, sortedList[sortedList.Count - 1], sortedList[sortedList.Count - 1], false);
					if (this.FromValue != string.Empty)
					{
						fromValues[0] = field.Parse(this.FromValue);
					}
					if (this.ToValue != string.Empty)
					{
						toValues[toValues.Length - 1] = field.Parse(this.ToValue);
					}
				}
			}
		}

		internal void GetOptimalIntervals(Field field, ArrayList sortedList, object fromValue, object toValue, int intervalCount, ref object[] fromValues, ref object[] toValues)
		{
			if (sortedList.Count == 0 || AspNetCore.Reporting.Map.WebForms.Field.ToStringInvariant(fromValue) == AspNetCore.Reporting.Map.WebForms.Field.ToStringInvariant(toValue))
			{
				this.GetEqualIntervals(field, fromValue, toValue, intervalCount, ref fromValues, ref toValues);
			}
			else
			{
				intervalCount = Math.Min(intervalCount, sortedList.Count);
				if (intervalCount < 4)
				{
					if (intervalCount < 1)
					{
						intervalCount = 1;
					}
					this.GetEqualIntervals(field, fromValue, toValue, intervalCount, ref fromValues, ref toValues);
				}
				else
				{
					if (intervalCount > sortedList.Count - 3)
					{
						intervalCount = sortedList.Count - 3;
					}
					if (intervalCount < 4)
					{
						if (intervalCount < 1)
						{
							intervalCount = 1;
						}
						this.GetEqualIntervals(field, fromValue, toValue, intervalCount, ref fromValues, ref toValues);
					}
					else
					{
						ArrayList arrayList = new ArrayList();
						for (int i = 0; i < sortedList.Count; i++)
						{
							arrayList.Add(field.ConvertToDouble(sortedList[i]));
						}
						int[] jenksBreaks = this.GetJenksBreaks(arrayList, intervalCount);
						fromValues = new object[jenksBreaks.Length];
						toValues = new object[jenksBreaks.Length];
						fromValues[0] = this.GetRoundedAverage(field, sortedList[0], sortedList[0], true);
						object roundedAverage = this.GetRoundedAverage(field, sortedList[jenksBreaks[0] - 1], sortedList[jenksBreaks[0]], true);
						toValues[0] = roundedAverage;
						for (int j = 1; j < fromValues.Length - 1; j++)
						{
							fromValues[j] = roundedAverage;
							roundedAverage = this.GetRoundedAverage(field, sortedList[jenksBreaks[j] - 1], sortedList[jenksBreaks[j]], true);
							toValues[j] = roundedAverage;
						}
						fromValues[fromValues.Length - 1] = roundedAverage;
						toValues[fromValues.Length - 1] = this.GetRoundedAverage(field, sortedList[sortedList.Count - 1], sortedList[sortedList.Count - 1], false);
						if (this.FromValue != string.Empty)
						{
							fromValues[0] = field.Parse(this.FromValue);
						}
						if (this.ToValue != string.Empty)
						{
							toValues[toValues.Length - 1] = field.Parse(this.ToValue);
						}
					}
				}
			}
		}

		private object GetRoundedAverage(Field field, object value1, object value2, bool floor)
		{
			if (field.Type == typeof(int))
			{
				int num = (int)value1;
				int num2 = (int)value2;
				double num3 = (double)num + 0.5 * (double)(num2 - num);
				if (num3 == 0.0)
				{
					return (int)num3;
				}
				int num4 = (int)Math.Log10(num3);
				double num5 = Math.Pow(10.0, (double)(num4 - 1));
				num3 /= num5;
				num3 = ((!floor) ? Math.Ceiling(num3) : Math.Floor(num3));
				num3 *= num5;
				return (int)num3;
			}
			if (field.Type == typeof(double))
			{
				double num6 = (double)value1;
				double num7 = (double)value2;
				double num8 = num6 + 0.5 * (num7 - num6);
				if (num8 == 0.0)
				{
					return num8;
				}
				int num9 = (int)Math.Log10(Math.Abs(num8));
				double num10 = Math.Pow(10.0, (double)(num9 - 1));
				num8 /= num10;
				num8 = ((!floor) ? Math.Ceiling(num8) : Math.Floor(num8));
				num8 *= num10;
				return num8;
			}
			if (field.Type == typeof(decimal))
			{
				decimal num11 = (decimal)value1;
				decimal d = (decimal)value2;
				decimal num12 = num11 + 0.5m * (d - num11);
				if (num12 == 0m)
				{
					return num12;
				}
				int num13 = (int)Math.Log10((double)Math.Abs(num12));
				decimal d2 = (decimal)Math.Pow(10.0, (double)(num13 - 1));
				num12 /= d2;
				num12 = ((!floor) ? Math.Ceiling(num12) : decimal.Floor(num12));
				num12 *= d2;
				return num12;
			}
			if (field.Type == typeof(DateTime))
			{
				DateTime dateTime = (DateTime)value1;
				DateTime d3 = (DateTime)value2;
				TimeSpan t = new TimeSpan((d3 - dateTime).Ticks / 2);
				DateTime dateTime2 = dateTime + t;
				return dateTime2;
			}
			TimeSpan timeSpan = (TimeSpan)value1;
			TimeSpan t2 = (TimeSpan)value2;
			TimeSpan t3 = new TimeSpan((t2 - timeSpan).Ticks / 2);
			TimeSpan timeSpan2 = timeSpan + t3;
			return timeSpan2;
		}

		internal int[] GetJenksBreaks(ArrayList list, int itervalCount)
		{
			int count = list.Count;
			double[] array = (double[])list.ToArray(typeof(double));
			double[][] array2 = new double[count + 1][];
			double[][] array3 = new double[count + 1][];
			for (int i = 0; i <= count; i++)
			{
				array2[i] = new double[itervalCount + 1];
				array3[i] = new double[itervalCount + 1];
			}
			for (int j = 1; j <= itervalCount; j++)
			{
				array2[1][j] = 1.0;
				array3[1][j] = 0.0;
				for (int k = 2; k <= count; k++)
				{
					array3[k][j] = 1.7976931348623157E+308;
				}
			}
			double num = 0.0;
			for (int l = 2; l <= count; l++)
			{
				double num2 = 0.0;
				double num3 = 0.0;
				double[] array4 = array2[l];
				double[] array5 = array3[l];
				for (int m = 1; m <= l; m++)
				{
					int num4 = l - m;
					double num5 = array[num4];
					num3 += num5 * num5;
					num2 += num5;
					num = num3 - num2 * num2 / (double)m;
					if (num4 != 0)
					{
						double[] array6 = array3[num4];
						for (int n = 2; n <= itervalCount; n++)
						{
							if (array5[n] >= num + array6[n - 1])
							{
								array4[n] = (double)(num4 + 1);
								array5[n] = num + array6[n - 1];
							}
						}
					}
				}
				array4[1] = 1.0;
				array5[1] = num;
			}
			ArrayList arrayList = new ArrayList();
			int num6 = count;
			for (int num7 = itervalCount; num7 >= 2; num7--)
			{
				int num8 = (int)array2[num6][num7] - 2;
				if (num8 > 0)
				{
					arrayList.Insert(0, num8);
				}
				num6 = Math.Max((int)array2[num6][num7] - 1, 0);
			}
			arrayList.Add(list.Count - 1);
			return (int[])arrayList.ToArray(typeof(int));
		}

		internal Color[] GetColors(ColoringMode coloringMode, MapColorPalette colorPalette, Color startColor, Color middleColor, Color endColor, int colorCount)
		{
			if (colorCount == 0)
			{
				return null;
			}
			if (coloringMode == ColoringMode.DistinctColors)
			{
				ColorGenerator colorGenerator = new ColorGenerator();
				return colorGenerator.GenerateColors(colorPalette, colorCount);
			}
			switch (colorCount)
			{
			case 1:
				return new Color[1]
				{
					startColor
				};
			case 2:
				return new Color[2]
				{
					startColor,
					endColor
				};
			default:
			{
				bool flag = middleColor.A == 0;
				byte a = startColor.A;
				byte a3 = middleColor.A;
				byte a2 = endColor.A;
				startColor = Color.FromArgb(255, startColor);
				middleColor = Color.FromArgb(255, middleColor);
				endColor = Color.FromArgb(255, endColor);
				Color[] array = new Color[colorCount];
				array[0] = startColor;
				array[colorCount - 1] = endColor;
				Rectangle rect = new Rectangle(0, 0, colorCount, 1);
				if (colorCount % 2 == 1)
				{
					rect.Width--;
				}
				Bitmap bitmap = new Bitmap(colorCount, 1);
				using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(rect, startColor, endColor, 0f, false))
				{
					ColorBlend colorBlend = new ColorBlend();
					if (flag)
					{
						colorBlend.Positions = new float[2]
						{
							0f,
							1f
						};
						colorBlend.Colors = new Color[2]
						{
							startColor,
							endColor
						};
					}
					else
					{
						colorBlend.Positions = new float[3]
						{
							0f,
							0.5f,
							1f
						};
						colorBlend.Colors = new Color[3]
						{
							startColor,
							middleColor,
							endColor
						};
					}
					linearGradientBrush.InterpolationColors = colorBlend;
					using (Graphics graphics = Graphics.FromImage(bitmap))
					{
						graphics.FillRectangle(linearGradientBrush, rect);
					}
				}
				float num = (float)((a2 - a) / (colorCount - 1));
				float num2 = (float)(int)a;
				for (int i = 1; i < colorCount - 1; i++)
				{
					num2 += num;
					Color pixel = bitmap.GetPixel(i, 0);
					array[i] = Color.FromArgb((int)num2, pixel);
				}
				return array;
			}
			}
		}

		internal float[] GetWidths(float startWidth, float endWidth, int count)
		{
			switch (count)
			{
			case 0:
				return null;
			case 1:
				return new float[1]
				{
					startWidth
				};
			case 2:
				return new float[2]
				{
					startWidth,
					endWidth
				};
			default:
			{
				float num = (endWidth - startWidth) / (float)count;
				float[] array = new float[count];
				float num2 = startWidth;
				for (int i = 0; i < count - 1; i++)
				{
					array[i] = num2;
					num2 += num;
					num2 = float.Parse(num2.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
				}
				array[count - 1] = endWidth;
				return array;
			}
			}
		}

		internal abstract Field GetField();
	}
}
