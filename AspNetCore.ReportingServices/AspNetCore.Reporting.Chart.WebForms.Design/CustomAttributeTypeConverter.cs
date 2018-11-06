using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms.Design
{
	internal class CustomAttributeTypeConverter : TypeConverter
	{
		protected class CustomAttributesPropertyDescriptor : SimplePropertyDescriptor
		{
			private string name = string.Empty;

			private CustomAttributeInfo customAttributeInfo;

			internal CustomAttributesPropertyDescriptor(Type componentType, string name, Type propertyType, Attribute[] attributes, CustomAttributeInfo customAttributeInfo)
				: base(componentType, name, propertyType, attributes)
			{
				this.name = name;
				this.customAttributeInfo = customAttributeInfo;
			}

			internal CustomAttributeInfo GetCustomAttributeInfo()
			{
				return this.customAttributeInfo;
			}

			public override object GetValue(object component)
			{
				CustomAttributes customAttributes = component as CustomAttributes;
				if (this.name == "UserDefined")
				{
					return customAttributes.GetUserDefinedAttributes();
				}
				object obj = null;
				string text = customAttributes.DataPointAttributes[this.name];
				if (this.customAttributeInfo != null)
				{
					if (text != null && text.Length != 0)
					{
						return this.GetValueFromString(text);
					}
					return this.GetValueFromString(this.customAttributeInfo.DefaultValue);
				}
				return text;
			}

			public override void SetValue(object component, object value)
			{
				this.ValidateValue(this.name, value);
				string stringFromValue = this.GetStringFromValue(value);
				CustomAttributes customAttributes = component as CustomAttributes;
				if (this.name == "UserDefined")
				{
					customAttributes.SetUserDefinedAttributes(stringFromValue);
				}
				else
				{
					bool flag = true;
					if (this.IsDefaultValue(stringFromValue) && (!(customAttributes.DataPointAttributes is DataPoint) || !((DataPoint)customAttributes.DataPointAttributes).series.IsAttributeSet(this.name)) && customAttributes.DataPointAttributes.IsAttributeSet(this.name))
					{
						customAttributes.DataPointAttributes.DeleteAttribute(this.name);
						flag = false;
					}
					if (flag)
					{
						customAttributes.DataPointAttributes[this.name] = stringFromValue;
					}
				}
				customAttributes.DataPointAttributes.CustomAttributes = customAttributes.DataPointAttributes.CustomAttributes;
				if (component is IChangeTracking)
				{
					((IChangeTracking)component).AcceptChanges();
				}
			}

			public bool IsDefaultValue(string val)
			{
				string stringFromValue = this.GetStringFromValue(this.customAttributeInfo.DefaultValue);
				return string.Compare(val, stringFromValue, StringComparison.Ordinal) == 0;
			}

			public virtual object GetValueFromString(object obj)
			{
				object result = null;
				if (obj != null)
				{
					if (this.customAttributeInfo.ValueType == obj.GetType())
					{
						return obj;
					}
					if (obj is string)
					{
						string text = (string)obj;
						if (this.customAttributeInfo.ValueType == typeof(string))
						{
							result = text;
							goto IL_0172;
						}
						if (this.customAttributeInfo.ValueType == typeof(float))
						{
							result = float.Parse(text, CultureInfo.InvariantCulture);
							goto IL_0172;
						}
						if (this.customAttributeInfo.ValueType == typeof(double))
						{
							result = double.Parse(text, CultureInfo.InvariantCulture);
							goto IL_0172;
						}
						if (this.customAttributeInfo.ValueType == typeof(int))
						{
							result = int.Parse(text, CultureInfo.InvariantCulture);
							goto IL_0172;
						}
						if (this.customAttributeInfo.ValueType == typeof(bool))
						{
							result = bool.Parse(text);
							goto IL_0172;
						}
						if (this.customAttributeInfo.ValueType == typeof(Color))
						{
							ColorConverter colorConverter = new ColorConverter();
							result = (Color)colorConverter.ConvertFromString(null, CultureInfo.InvariantCulture, text);
							goto IL_0172;
						}
						if (this.customAttributeInfo.ValueType.IsEnum)
						{
							result = Enum.Parse(this.customAttributeInfo.ValueType, text, true);
							goto IL_0172;
						}
						throw new InvalidOperationException(SR.ExceptionCustomAttributeTypeUnsupported(this.customAttributeInfo.ValueType.ToString()));
					}
				}
				goto IL_0172;
				IL_0172:
				return result;
			}

			public string GetStringFromValue(object value)
			{
				if (value is Color)
				{
					ColorConverter colorConverter = new ColorConverter();
					return colorConverter.ConvertToString(null, CultureInfo.InvariantCulture, value);
				}
				if (value is float)
				{
					return ((float)value).ToString(CultureInfo.InvariantCulture);
				}
				if (value is double)
				{
					return ((double)value).ToString(CultureInfo.InvariantCulture);
				}
				if (value is int)
				{
					return ((int)value).ToString(CultureInfo.InvariantCulture);
				}
				if (value is bool)
				{
					return ((bool)value).ToString();
				}
				return value.ToString();
			}

			public virtual void ValidateValue(string attrName, object value)
			{
				bool flag;
				if (this.customAttributeInfo != null)
				{
					flag = false;
					if (this.customAttributeInfo.MaxValue != null)
					{
						if (value.GetType() != this.customAttributeInfo.MaxValue.GetType())
						{
							throw new InvalidOperationException(SR.ExceptionCustomAttributeTypeOrMaximumPossibleValueInvalid(attrName));
						}
						if (value is float)
						{
							if ((float)value > (float)this.customAttributeInfo.MaxValue)
							{
								flag = true;
							}
							goto IL_00b7;
						}
						if (value is double)
						{
							if ((double)value > (double)this.customAttributeInfo.MaxValue)
							{
								flag = true;
							}
							goto IL_00b7;
						}
						if (value is int)
						{
							if ((int)value > (int)this.customAttributeInfo.MaxValue)
							{
								flag = true;
							}
							goto IL_00b7;
						}
						throw new InvalidOperationException(SR.ExceptionCustomAttributeTypeOrMinimumPossibleValueUnsupported(attrName));
					}
					goto IL_00b7;
				}
				return;
				IL_0163:
				if (!flag)
				{
					return;
				}
				if (this.customAttributeInfo.MaxValue != null && this.customAttributeInfo.MinValue != null)
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeMustBeInRange(attrName, this.customAttributeInfo.MinValue.ToString(), this.customAttributeInfo.MaxValue.ToString()));
				}
				if (this.customAttributeInfo.MinValue != null)
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeMustBeBiggerThenValue(attrName, this.customAttributeInfo.MinValue.ToString()));
				}
				if (this.customAttributeInfo.MaxValue == null)
				{
					return;
				}
				throw new InvalidOperationException(SR.ExceptionCustomAttributeMustBeMoreThenValue(attrName, this.customAttributeInfo.MaxValue.ToString()));
				IL_00b7:
				if (this.customAttributeInfo.MinValue != null)
				{
					if (value.GetType() != this.customAttributeInfo.MinValue.GetType())
					{
						throw new InvalidOperationException(SR.ExceptionCustomAttributeTypeOrMinimumPossibleValueInvalid(attrName));
					}
					if (value is float)
					{
						if ((float)value < (float)this.customAttributeInfo.MinValue)
						{
							flag = true;
						}
						goto IL_0163;
					}
					if (value is double)
					{
						if ((double)value < (double)this.customAttributeInfo.MinValue)
						{
							flag = true;
						}
						goto IL_0163;
					}
					if (value is int)
					{
						if ((int)value < (int)this.customAttributeInfo.MinValue)
						{
							flag = true;
						}
						goto IL_0163;
					}
					throw new InvalidOperationException(SR.ExceptionCustomAttributeTypeOrMinimumPossibleValueUnsupported(attrName));
				}
				goto IL_0163;
			}
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(CustomAttributes))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				return ((CustomAttributes)value).DataPointAttributes.CustomAttributes;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string && context != null && context.Instance != null)
			{
				if (context.Instance is DataPointAttributes)
				{
					((DataPointAttributes)context.Instance).CustomAttributes = (string)value;
					return new CustomAttributes((DataPointAttributes)context.Instance);
				}
				if (context.Instance is CustomAttributes)
				{
					return new CustomAttributes(((CustomAttributes)context.Instance).DataPointAttributes);
				}
				if (context.Instance is IDataPointAttributesProvider)
				{
					return new CustomAttributes(((IDataPointAttributesProvider)context.Instance).DataPointAttributes);
				}
				if (context.Instance is Array)
				{
					DataPointAttributes dataPointAttributes = null;
					foreach (object item in (Array)context.Instance)
					{
						if (item is DataPointAttributes)
						{
							dataPointAttributes = (DataPointAttributes)item;
							dataPointAttributes.CustomAttributes = (string)value;
						}
					}
					if (dataPointAttributes != null)
					{
						return new CustomAttributes(dataPointAttributes);
					}
				}
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object obj, Attribute[] attributes)
		{
			PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
			CustomAttributes customAttributes = obj as CustomAttributes;
			if (customAttributes != null && context != null)
			{
				Series series = (customAttributes.DataPointAttributes is Series) ? ((Series)customAttributes.DataPointAttributes) : customAttributes.DataPointAttributes.series;
				if (series != null && series.chart != null && series.chart.chartPicture != null && series.chart.chartPicture.common != null)
				{
					CustomAttributeRegistry customAttributeRegistry = (CustomAttributeRegistry)series.chart.chartPicture.common.container.GetService(typeof(CustomAttributeRegistry));
					foreach (CustomAttributeInfo registeredCustomAttribute in customAttributeRegistry.registeredCustomAttributes)
					{
						if (this.IsApplicableCustomAttribute(registeredCustomAttribute, context.Instance))
						{
							Attribute[] propertyAttributes = this.GetPropertyAttributes(registeredCustomAttribute);
							CustomAttributesPropertyDescriptor value = new CustomAttributesPropertyDescriptor(typeof(CustomAttributes), registeredCustomAttribute.Name, registeredCustomAttribute.ValueType, propertyAttributes, registeredCustomAttribute);
							propertyDescriptorCollection.Add(value);
						}
					}
					Attribute[] attributes2 = new Attribute[3]
					{
						new NotifyParentPropertyAttribute(true),
						new RefreshPropertiesAttribute(RefreshProperties.All),
						new DescriptionAttribute(SR.DescriptionAttributeUserDefined)
					};
					CustomAttributesPropertyDescriptor value2 = new CustomAttributesPropertyDescriptor(typeof(CustomAttributes), "UserDefined", typeof(string), attributes2, null);
					propertyDescriptorCollection.Add(value2);
				}
			}
			return propertyDescriptorCollection;
		}

		private bool IsApplicableCustomAttribute(CustomAttributeInfo attrInfo, object obj)
		{
			if (obj is CustomAttributes)
			{
				obj = ((CustomAttributes)obj).DataPointAttributes;
			}
			if (this.IsDataPoint(obj) && attrInfo.AppliesToDataPoint)
			{
				goto IL_0037;
			}
			if (!this.IsDataPoint(obj) && attrInfo.AppliesToSeries)
			{
				goto IL_0037;
			}
			goto IL_00a6;
			IL_00a6:
			return false;
			IL_0037:
			if (this.Is3DChartType(obj) && attrInfo.AppliesTo3D)
			{
				goto IL_0059;
			}
			if (!this.Is3DChartType(obj) && attrInfo.AppliesTo2D)
			{
				goto IL_0059;
			}
			goto IL_00a6;
			IL_0059:
			SeriesChartType[] selectedChartTypes = this.GetSelectedChartTypes(obj);
			SeriesChartType[] array = selectedChartTypes;
			foreach (SeriesChartType seriesChartType in array)
			{
				SeriesChartType[] appliesToChartType = attrInfo.AppliesToChartType;
				foreach (SeriesChartType seriesChartType2 in appliesToChartType)
				{
					if (seriesChartType2 == seriesChartType)
					{
						return true;
					}
				}
			}
			goto IL_00a6;
		}

		private bool IsDataPoint(object obj)
		{
			if (obj is Series)
			{
				return false;
			}
			if (obj is Array && ((Array)obj).Length > 0 && ((Array)obj).GetValue(0) is Series)
			{
				return false;
			}
			return true;
		}

		private bool Is3DChartType(object obj)
		{
			Series[] selectedSeries = this.GetSelectedSeries(obj);
			Series[] array = selectedSeries;
			foreach (Series series in array)
			{
				ChartArea chartArea = series.chart.ChartAreas[series.ChartArea];
				if (chartArea.Area3DStyle.Enable3D)
				{
					return true;
				}
			}
			return false;
		}

		private Series[] GetSelectedSeries(object obj)
		{
			Series[] array = new Series[0];
			if (obj is Array && ((Array)obj).Length > 0)
			{
				if (((Array)obj).GetValue(0) is Series)
				{
					array = new Series[((Array)obj).Length];
					((Array)obj).CopyTo(array, 0);
				}
				else if (((Array)obj).GetValue(0) is DataPointAttributes)
				{
					array = new Series[1]
					{
						((DataPointAttributes)((Array)obj).GetValue(0)).series
					};
				}
			}
			else if (obj is Series)
			{
				array = new Series[1]
				{
					(Series)obj
				};
			}
			else if (obj is DataPointAttributes)
			{
				array = new Series[1]
				{
					((DataPointAttributes)obj).series
				};
			}
			return array;
		}

		private SeriesChartType[] GetSelectedChartTypes(object obj)
		{
			Series[] selectedSeries = this.GetSelectedSeries(obj);
			int num = 0;
			SeriesChartType[] array = new SeriesChartType[selectedSeries.Length];
			Series[] array2 = selectedSeries;
			foreach (Series series in array2)
			{
				array[num++] = series.ChartType;
			}
			return array;
		}

		private Attribute[] GetPropertyAttributes(CustomAttributeInfo attrInfo)
		{
			DefaultValueAttribute defaultValueAttribute = null;
			if (attrInfo.DefaultValue.GetType() == attrInfo.ValueType)
			{
				defaultValueAttribute = new DefaultValueAttribute(attrInfo.DefaultValue);
				goto IL_0054;
			}
			if (attrInfo.DefaultValue is string)
			{
				defaultValueAttribute = new DefaultValueAttribute(attrInfo.ValueType, (string)attrInfo.DefaultValue);
				goto IL_0054;
			}
			throw new InvalidOperationException(SR.ExceptionCustomAttributeDefaultValueTypeInvalid);
			IL_0054:
			ArrayList arrayList = new ArrayList();
			arrayList.Add(new NotifyParentPropertyAttribute(true));
			arrayList.Add(new RefreshPropertiesAttribute(RefreshProperties.All));
			arrayList.Add(new DescriptionAttribute(attrInfo.Description));
			arrayList.Add(defaultValueAttribute);
			int num = 0;
			Attribute[] array = new Attribute[arrayList.Count];
			foreach (Attribute item in arrayList)
			{
				array[num++] = item;
			}
			return array;
		}
	}
}
