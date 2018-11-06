using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeDataPointCollection_DataPointCollection")]
	internal class DataPointCollection : IList, ICollection, IEnumerable
	{
		internal ArrayList array = new ArrayList();

		internal Series series;

		public DataPoint this[int index]
		{
			get
			{
				return (DataPoint)this.array[index];
			}
			set
			{
				this.DataPointInit(ref value);
				this.array[index] = value;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this.array[index];
			}
			set
			{
				this.array[index] = value;
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return this.array.IsFixedSize;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.array.IsReadOnly;
			}
		}

		public int Count
		{
			get
			{
				return this.array.Count;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return this.array.IsSynchronized;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this.array.SyncRoot;
			}
		}

		private DataPointCollection()
		{
			this.series = null;
		}

		public DataPointCollection(Series series)
		{
			this.series = series;
		}

		internal void DataPointInit(ref DataPoint dataPoint)
		{
			DataPointCollection.DataPointInit(this.series, ref dataPoint);
		}

		internal static void DataPointInit(Series series, ref DataPoint dataPoint)
		{
			dataPoint.series = series;
			if (dataPoint.AxisLabel.Length > 0 && series != null)
			{
				series.noLabelsInPoints = false;
			}
		}

		internal static void ParsePointFieldsParameter(string otherFields, ref string[] otherAttributeNames, ref string[] otherFieldNames, ref string[] otherValueFormat)
		{
			if (otherFields == null)
			{
				return;
			}
			if (otherFields.Length <= 0)
			{
				return;
			}
			otherAttributeNames = otherFields.Replace(",,", "\n").Split(',');
			otherFieldNames = new string[otherAttributeNames.Length];
			otherValueFormat = new string[otherAttributeNames.Length];
			int num = 0;
			while (true)
			{
				if (num < otherAttributeNames.Length)
				{
					int num2 = otherAttributeNames[num].IndexOf('=');
					if (num2 <= 0)
					{
						break;
					}
					otherFieldNames[num] = otherAttributeNames[num].Substring(num2 + 1);
					otherAttributeNames[num] = otherAttributeNames[num].Substring(0, num2);
					int num3 = otherFieldNames[num].IndexOf('{');
					if (num3 > 0 && otherFieldNames[num][otherFieldNames[num].Length - 1] == '}')
					{
						otherValueFormat[num] = otherFieldNames[num].Substring(num3 + 1);
						otherValueFormat[num] = otherValueFormat[num].Trim('{', '}');
						otherFieldNames[num] = otherFieldNames[num].Substring(0, num3);
					}
					otherAttributeNames[num] = otherAttributeNames[num].Trim().Replace("\n", ",");
					otherFieldNames[num] = otherFieldNames[num].Trim().Replace("\n", ",");
					num++;
					continue;
				}
				return;
			}
			throw new ArgumentException(SR.ExceptionParameterFormatInvalid, "otherFields");
		}

		public void DataBind(IEnumerable dataSource, string xField, string yFields, string otherFields)
		{
			this.series.TraceWrite("ChartDataBinding", SR.TraceMessagePopulatingSeriesDataPoints(this.series.Name));
			string[] array = null;
			if (yFields != null)
			{
				array = yFields.Replace(",,", "\n").Split(',');
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = array[i].Replace("\n", ",");
				}
			}
			string[] array2 = null;
			string[] array3 = null;
			string[] array4 = null;
			DataPointCollection.ParsePointFieldsParameter(otherFields, ref array2, ref array3, ref array4);
			if (dataSource == null)
			{
				throw new ArgumentNullException("dataSource", SR.ExceptionDataPointInsertionNoDataSource);
			}
			if (dataSource is string)
			{
				throw new ArgumentException(SR.ExceptionDataBindSeriesToString, "dataSource");
			}
			if (array != null && array.GetLength(0) <= this.series.YValuesPerPoint)
			{
				this.array.Clear();
				IEnumerator dataSourceEnumerator = DataPointCollection.GetDataSourceEnumerator(dataSource);
				if (dataSourceEnumerator.GetType() != typeof(DbEnumerator))
				{
					dataSourceEnumerator.Reset();
				}
				bool flag = true;
				object[] array5 = new object[array.Length];
				object obj = null;
				bool flag2 = true;
				do
				{
					if (flag)
					{
						flag = dataSourceEnumerator.MoveNext();
					}
					if (flag2)
					{
						flag2 = false;
						DataPointCollection.AutoDetectValuesType(this.series, dataSourceEnumerator, xField, dataSourceEnumerator, array[0]);
					}
					if (flag)
					{
						DataPoint dataPoint = new DataPoint(this.series);
						bool flag3 = false;
						if (xField.Length > 0)
						{
							obj = DataPointCollection.ConvertEnumerationItem(dataSourceEnumerator.Current, xField);
							if (DataPointCollection.IsEmptyValue(obj))
							{
								flag3 = true;
								obj = 0.0;
							}
						}
						if (array.Length == 0)
						{
							array5[0] = DataPointCollection.ConvertEnumerationItem(dataSourceEnumerator.Current, null);
							if (DataPointCollection.IsEmptyValue(array5[0]))
							{
								flag3 = true;
								array5[0] = 0.0;
							}
						}
						else
						{
							for (int j = 0; j < array.Length; j++)
							{
								array5[j] = DataPointCollection.ConvertEnumerationItem(dataSourceEnumerator.Current, array[j]);
								if (DataPointCollection.IsEmptyValue(array5[j]))
								{
									flag3 = true;
									array5[j] = 0.0;
								}
							}
						}
						if (array2 != null && array2.Length > 0)
						{
							for (int k = 0; k < array3.Length; k++)
							{
								object obj2 = DataPointCollection.ConvertEnumerationItem(dataSourceEnumerator.Current, array3[k]);
								if (!DataPointCollection.IsEmptyValue(obj2))
								{
									dataPoint.SetPointAttribute(obj2, array2[k], array4[k]);
								}
							}
						}
						if (flag3)
						{
							if (obj != null)
							{
								dataPoint.SetValueXY(obj, array5);
							}
							else
							{
								dataPoint.SetValueXY(0, array5);
							}
							this.DataPointInit(ref dataPoint);
							dataPoint.Empty = true;
							this.array.Add(dataPoint);
						}
						else
						{
							if (obj != null)
							{
								dataPoint.SetValueXY(obj, array5);
							}
							else
							{
								dataPoint.SetValueXY(0, array5);
							}
							this.DataPointInit(ref dataPoint);
							this.array.Add(dataPoint);
						}
					}
				}
				while (flag);
				this.series.TraceWrite("ChartDataBinding", SR.TraceMessageSeriesPopulatedWithDataPoints(this.series.Name, this.array.Count.ToString(CultureInfo.CurrentCulture)));
				return;
			}
			throw new ArgumentOutOfRangeException("yFields", SR.ExceptionDataPointYValuesCountMismatch(this.series.YValuesPerPoint.ToString(CultureInfo.InvariantCulture)));
		}

		public void DataBindY(params IEnumerable[] yValue)
		{
			this.DataBindXY(null, yValue);
		}

		public void DataBindXY(IEnumerable xValue, params IEnumerable[] yValues)
		{
			this.series.TraceWrite("ChartDataBinding", SR.TraceMessagePopulatingSeriesDataPoints(this.series.Name));
			for (int i = 0; i < yValues.Length; i++)
			{
				if (yValues[i] is string)
				{
					throw new ArgumentException(SR.ExceptionDataBindYValuesToString, "yValues");
				}
			}
			if (yValues != null && yValues.GetLength(0) != 0)
			{
				if (yValues.GetLength(0) > this.series.YValuesPerPoint)
				{
					throw new ArgumentOutOfRangeException("yValues", SR.ExceptionDataPointYValuesBindingCountMismatch(this.series.YValuesPerPoint.ToString(CultureInfo.InvariantCulture)));
				}
				this.array.Clear();
				IEnumerator enumerator = null;
				IEnumerator[] array = new IEnumerator[yValues.GetLength(0)];
				if (xValue != null)
				{
					if (xValue is string)
					{
						throw new ArgumentException(SR.ExceptionDataBindXValuesToString, "xValue");
					}
					enumerator = DataPointCollection.GetDataSourceEnumerator(xValue);
					if (enumerator.GetType() != typeof(DbEnumerator))
					{
						enumerator.Reset();
					}
				}
				for (int j = 0; j < yValues.Length; j++)
				{
					array[j] = DataPointCollection.GetDataSourceEnumerator(yValues[j]);
					if (array[j].GetType() != typeof(DbEnumerator))
					{
						array[j].Reset();
					}
				}
				bool flag = false;
				bool flag2 = true;
				object[] array2 = new object[this.series.YValuesPerPoint];
				object obj = null;
				bool flag3 = true;
				do
				{
					flag2 = true;
					for (int k = 0; k < yValues.Length; k++)
					{
						if (flag2)
						{
							flag2 = array[k].MoveNext();
						}
					}
					if (xValue != null)
					{
						flag = enumerator.MoveNext();
						if (flag2 && !flag)
						{
							throw new ArgumentOutOfRangeException("xValue", SR.ExceptionDataPointInsertionXValuesQtyIsLessYValues);
						}
					}
					if (flag3)
					{
						flag3 = false;
						DataPointCollection.AutoDetectValuesType(this.series, enumerator, null, array[0], null);
					}
					if (flag || flag2)
					{
						DataPoint dataPoint = new DataPoint(this.series);
						bool flag4 = false;
						if (flag)
						{
							obj = DataPointCollection.ConvertEnumerationItem(enumerator.Current, null);
							if (obj is DBNull || obj == null)
							{
								flag4 = true;
								obj = 0.0;
							}
						}
						for (int l = 0; l < yValues.Length; l++)
						{
							array2[l] = DataPointCollection.ConvertEnumerationItem(array[l].Current, null);
							if (array2[l] is DBNull || array2[l] == null)
							{
								flag4 = true;
								array2[l] = 0.0;
							}
						}
						if (flag4)
						{
							if (obj != null)
							{
								dataPoint.SetValueXY(obj, array2);
							}
							else
							{
								dataPoint.SetValueXY(0, array2);
							}
							this.DataPointInit(ref dataPoint);
							dataPoint.Empty = true;
							this.array.Add(dataPoint);
						}
						else
						{
							if (obj != null)
							{
								dataPoint.SetValueXY(obj, array2);
							}
							else
							{
								dataPoint.SetValueXY(0, array2);
							}
							this.DataPointInit(ref dataPoint);
							this.array.Add(dataPoint);
						}
					}
				}
				while (flag || flag2);
				this.series.TraceWrite("ChartDataBinding", SR.TraceMessageSeriesPopulatedWithDataPoints(this.series.Name, this.array.Count.ToString(CultureInfo.CurrentCulture)));
				return;
			}
			throw new ArgumentNullException("yValues", SR.ExceptionDataPointBindingYValueNotSpecified);
		}

		public void DataBindY(IEnumerable yValue, string yFields)
		{
			this.DataBindXY(null, null, yValue, yFields);
		}

		public void DataBindXY(IEnumerable xValue, string xField, IEnumerable yValue, string yFields)
		{
			this.series.TraceWrite("ChartDataBinding", SR.TraceMessagePopulatingSeriesDataPoints(this.series.Name));
			string[] array = null;
			if (yFields != null)
			{
				array = yFields.Replace(",,", "\n").Split(',');
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = array[i].Replace("\n", ",");
				}
			}
			if (yValue == null)
			{
				throw new ArgumentNullException("yValue", SR.ExceptionDataPointInsertionYValueNotSpecified);
			}
			if (yValue is string)
			{
				throw new ArgumentException(SR.ExceptionDataBindYValuesToString, "yValue");
			}
			if (xValue is string)
			{
				throw new ArgumentException(SR.ExceptionDataBindXValuesToString, "xValue");
			}
			if (array != null && array.GetLength(0) <= this.series.YValuesPerPoint)
			{
				this.array.Clear();
				IEnumerator enumerator = null;
				IEnumerator dataSourceEnumerator = DataPointCollection.GetDataSourceEnumerator(yValue);
				if (dataSourceEnumerator.GetType() != typeof(DbEnumerator))
				{
					dataSourceEnumerator.Reset();
				}
				if (xValue != null)
				{
					if (xValue != yValue)
					{
						enumerator = DataPointCollection.GetDataSourceEnumerator(xValue);
						if (enumerator.GetType() != typeof(DbEnumerator))
						{
							enumerator.Reset();
						}
					}
					else
					{
						enumerator = dataSourceEnumerator;
					}
				}
				bool flag = false;
				bool flag2 = true;
				object[] array2 = new object[array.Length];
				object obj = null;
				bool flag3 = true;
				do
				{
					if (flag2)
					{
						flag2 = dataSourceEnumerator.MoveNext();
					}
					if (xValue != null)
					{
						if (xValue != yValue)
						{
							flag = enumerator.MoveNext();
							if (flag2 && !flag)
							{
								throw new ArgumentOutOfRangeException("xValue", SR.ExceptionDataPointInsertionXValuesQtyIsLessYValues);
							}
						}
						else
						{
							flag = flag2;
						}
					}
					if (flag3)
					{
						flag3 = false;
						DataPointCollection.AutoDetectValuesType(this.series, enumerator, xField, dataSourceEnumerator, array[0]);
					}
					if (flag || flag2)
					{
						DataPoint dataPoint = new DataPoint(this.series);
						bool flag4 = false;
						if (flag)
						{
							obj = DataPointCollection.ConvertEnumerationItem(enumerator.Current, xField);
							if (DataPointCollection.IsEmptyValue(obj))
							{
								flag4 = true;
								obj = 0.0;
							}
						}
						if (array.Length == 0)
						{
							array2[0] = DataPointCollection.ConvertEnumerationItem(dataSourceEnumerator.Current, null);
							if (DataPointCollection.IsEmptyValue(array2[0]))
							{
								flag4 = true;
								array2[0] = 0.0;
							}
						}
						else
						{
							for (int j = 0; j < array.Length; j++)
							{
								array2[j] = DataPointCollection.ConvertEnumerationItem(dataSourceEnumerator.Current, array[j]);
								if (DataPointCollection.IsEmptyValue(array2[j]))
								{
									flag4 = true;
									array2[j] = 0.0;
								}
							}
						}
						if (flag4)
						{
							if (obj != null)
							{
								dataPoint.SetValueXY(obj, array2);
							}
							else
							{
								dataPoint.SetValueXY(0, array2);
							}
							this.DataPointInit(ref dataPoint);
							dataPoint.Empty = true;
							this.array.Add(dataPoint);
						}
						else
						{
							if (obj != null)
							{
								dataPoint.SetValueXY(obj, array2);
							}
							else
							{
								dataPoint.SetValueXY(0, array2);
							}
							this.DataPointInit(ref dataPoint);
							this.array.Add(dataPoint);
						}
					}
				}
				while (flag || flag2);
				this.series.TraceWrite("ChartDataBinding", SR.TraceMessageSeriesPopulatedWithDataPoints(this.series.Name, this.array.Count.ToString(CultureInfo.CurrentCulture)));
				return;
			}
			throw new ArgumentOutOfRangeException("yValue", SR.ExceptionDataPointYValuesCountMismatch(this.series.YValuesPerPoint.ToString(CultureInfo.InvariantCulture)));
		}

		internal static bool IsEmptyValue(object val)
		{
			if (!(val is DBNull) && val != null)
			{
				if (val is double && double.IsNaN((double)val))
				{
					return true;
				}
				if (val is float && float.IsNaN((float)val))
				{
					return true;
				}
				return false;
			}
			return true;
		}

		public int AddY(double yValue)
		{
			DataPoint dataPoint = new DataPoint(this.series);
			dataPoint.SetValueY(yValue);
			this.DataPointInit(ref dataPoint);
			return this.array.Add(dataPoint);
		}

		public int AddY(params object[] yValue)
		{
			if (this.series.YValueType == ChartValueTypes.Auto && yValue.Length > 0 && yValue[0] != null)
			{
				if (yValue[0] is DateTime)
				{
					this.series.YValueType = ChartValueTypes.DateTime;
					this.series.autoYValueType = true;
				}
				else if (yValue[0] is DateTimeOffset)
				{
					this.series.YValueType = ChartValueTypes.DateTimeOffset;
					this.series.autoYValueType = true;
				}
			}
			DataPoint dataPoint = new DataPoint(this.series);
			dataPoint.SetValueY(yValue);
			this.DataPointInit(ref dataPoint);
			return this.array.Add(dataPoint);
		}

		public int AddXY(double xValue, double yValue)
		{
			DataPoint dataPoint = new DataPoint(this.series);
			dataPoint.SetValueXY(xValue, yValue);
			this.DataPointInit(ref dataPoint);
			return this.array.Add(dataPoint);
		}

		public int AddXY(object xValue, params object[] yValue)
		{
			if (this.series.XValueType == ChartValueTypes.Auto)
			{
				if (xValue is DateTime)
				{
					this.series.XValueType = ChartValueTypes.DateTime;
				}
				if (xValue is DateTimeOffset)
				{
					this.series.XValueType = ChartValueTypes.DateTimeOffset;
				}
				if (xValue is string)
				{
					this.series.XValueType = ChartValueTypes.String;
				}
				this.series.autoXValueType = true;
			}
			if (this.series.YValueType == ChartValueTypes.Auto && yValue.Length > 0 && yValue[0] != null)
			{
				if (yValue[0] is DateTime)
				{
					this.series.YValueType = ChartValueTypes.DateTime;
					this.series.autoYValueType = true;
				}
				else if (yValue[0] is DateTimeOffset)
				{
					this.series.YValueType = ChartValueTypes.DateTimeOffset;
					this.series.autoYValueType = true;
				}
			}
			DataPoint dataPoint = new DataPoint(this.series);
			dataPoint.SetValueXY(xValue, yValue);
			this.DataPointInit(ref dataPoint);
			return this.array.Add(dataPoint);
		}

		public void InsertXY(int index, object xValue, params object[] yValue)
		{
			DataPoint dataPoint = new DataPoint(this.series);
			dataPoint.SetValueXY(xValue, yValue);
			this.DataPointInit(ref dataPoint);
			this.array.Insert(index, dataPoint);
		}

		public void InsertY(int index, params object[] yValue)
		{
			DataPoint dataPoint = new DataPoint(this.series);
			dataPoint.SetValueY(yValue);
			this.DataPointInit(ref dataPoint);
			this.array.Insert(index, dataPoint);
		}

		internal static IEnumerator GetDataSourceEnumerator(IEnumerable dataSource)
		{
			if (dataSource is DataView)
			{
				DataView dataView = (DataView)dataSource;
				return dataView.GetEnumerator();
			}
			if (dataSource is DataSet)
			{
				DataSet dataSet = (DataSet)dataSource;
				if (dataSet.Tables.Count > 0)
				{
					return dataSet.Tables[0].Rows.GetEnumerator();
				}
			}
			return dataSource.GetEnumerator();
		}

		internal static object ConvertEnumerationItem(object item, string fieldName)
		{
			object result = item;
			if (item is DataRow)
			{
				if (fieldName != null && fieldName.Length > 0)
				{
					bool flag = true;
					if (((DataRow)item).Table.Columns.Contains(fieldName))
					{
						result = ((DataRow)item)[fieldName];
						flag = false;
					}
					else
					{
						try
						{
							int num = int.Parse(fieldName, CultureInfo.InvariantCulture);
							if (num < ((DataRow)item).Table.Columns.Count && num >= 0)
							{
								result = ((DataRow)item)[num];
								flag = false;
							}
						}
						catch
						{
						}
					}
					if (flag)
					{
						throw new ArgumentException(SR.ExceptionColumnNameNotFound(fieldName));
					}
				}
				else
				{
					result = ((DataRow)item)[0];
				}
			}
			else if (item is DataRowView)
			{
				if (fieldName != null && fieldName.Length > 0)
				{
					bool flag2 = true;
					if (((DataRowView)item).DataView.Table.Columns.Contains(fieldName))
					{
						result = ((DataRowView)item)[fieldName];
						flag2 = false;
					}
					else
					{
						try
						{
							int num2 = int.Parse(fieldName, CultureInfo.InvariantCulture);
							if (num2 < ((DataRowView)item).DataView.Table.Columns.Count && num2 >= 0)
							{
								result = ((DataRowView)item)[num2];
								flag2 = false;
							}
						}
						catch
						{
						}
					}
					if (flag2)
					{
						throw new ArgumentException(SR.ExceptionColumnNameNotFound(fieldName));
					}
				}
				else
				{
					result = ((DataRowView)item)[0];
				}
			}
			else if (item is DbDataRecord)
			{
				if (fieldName != null && fieldName.Length > 0)
				{
					bool flag3 = true;
					if (!char.IsNumber(fieldName, 0))
					{
						try
						{
							result = ((DbDataRecord)item)[fieldName];
							flag3 = false;
						}
						catch (Exception)
						{
						}
					}
					if (flag3)
					{
						try
						{
							int i = int.Parse(fieldName, CultureInfo.InvariantCulture);
							result = ((DbDataRecord)item)[i];
							flag3 = false;
						}
						catch
						{
						}
					}
					if (flag3)
					{
						throw new ArgumentException(SR.ExceptionColumnNameNotFound(fieldName));
					}
				}
				else
				{
					result = ((DbDataRecord)item)[0];
				}
			}
			else if (fieldName != null && fieldName.Length > 0)
			{
				PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(item).Find(fieldName, true);
				if (propertyDescriptor != null)
				{
					result = propertyDescriptor.GetValue(item);
					return result ?? null;
				}
			}
			return result;
		}

		internal static void AutoDetectValuesType(Series series, IEnumerator xEnumerator, string xField, IEnumerator yEnumerator, string yField)
		{
			if (series.XValueType == ChartValueTypes.Auto)
			{
				series.XValueType = DataPointCollection.GetValueType(xEnumerator, xField);
				if (series.XValueType != 0)
				{
					series.autoXValueType = true;
				}
			}
			if (series.YValueType == ChartValueTypes.Auto)
			{
				series.YValueType = DataPointCollection.GetValueType(yEnumerator, yField);
				if (series.YValueType != 0)
				{
					series.autoYValueType = true;
				}
			}
		}

		private static ChartValueTypes GetValueType(IEnumerator enumerator, string field)
		{
			ChartValueTypes result = ChartValueTypes.Auto;
			Type type = null;
			if (enumerator == null)
			{
				return result;
			}
			try
			{
				if (enumerator.Current == null)
				{
					return result;
				}
			}
			catch (Exception)
			{
				return result;
			}
			if (enumerator.Current is DataRow)
			{
				if (field != null && field.Length > 0)
				{
					bool flag = true;
					if (((DataRow)enumerator.Current).Table.Columns.Contains(field))
					{
						type = ((DataRow)enumerator.Current).Table.Columns[field].DataType;
						flag = false;
					}
					if (flag)
					{
						try
						{
							int index = int.Parse(field, CultureInfo.InvariantCulture);
							type = ((DataRow)enumerator.Current).Table.Columns[index].DataType;
							flag = false;
						}
						catch
						{
						}
					}
					if (flag)
					{
						throw new ArgumentException(SR.ExceptionColumnNameNotFound(field));
					}
				}
				else if (((DataRow)enumerator.Current).Table.Columns.Count > 0)
				{
					type = ((DataRow)enumerator.Current).Table.Columns[0].DataType;
				}
			}
			else if (enumerator.Current is DataRowView)
			{
				if (field != null && field.Length > 0)
				{
					bool flag2 = true;
					if (((DataRowView)enumerator.Current).DataView.Table.Columns.Contains(field))
					{
						type = ((DataRowView)enumerator.Current).DataView.Table.Columns[field].DataType;
						flag2 = false;
					}
					if (flag2)
					{
						try
						{
							int index2 = int.Parse(field, CultureInfo.InvariantCulture);
							type = ((DataRowView)enumerator.Current).DataView.Table.Columns[index2].DataType;
							flag2 = false;
						}
						catch
						{
						}
					}
					if (flag2)
					{
						throw new ArgumentException(SR.ExceptionColumnNameNotFound(field));
					}
				}
				else if (((DataRowView)enumerator.Current).DataView.Table.Columns.Count > 0)
				{
					type = ((DataRowView)enumerator.Current).DataView.Table.Columns[0].DataType;
				}
			}
			else if (enumerator.Current is DbDataRecord)
			{
				if (field != null && field.Length > 0)
				{
					bool flag3 = true;
					int num = 0;
					if (!char.IsNumber(field, 0))
					{
						try
						{
							num = ((DbDataRecord)enumerator.Current).GetOrdinal(field);
							type = ((DbDataRecord)enumerator.Current).GetFieldType(num);
							flag3 = false;
						}
						catch
						{
						}
					}
					if (flag3)
					{
						try
						{
							num = int.Parse(field, CultureInfo.InvariantCulture);
							type = ((DbDataRecord)enumerator.Current).GetFieldType(num);
							flag3 = false;
						}
						catch
						{
						}
					}
					if (flag3)
					{
						throw new ArgumentException(SR.ExceptionColumnNameNotFound(field));
					}
				}
				else if (((DbDataRecord)enumerator.Current).FieldCount > 0)
				{
					type = ((DbDataRecord)enumerator.Current).GetFieldType(0);
				}
			}
			else
			{
				if (field != null && field.Length > 0)
				{
					PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(enumerator.Current).Find(field, true);
					if (propertyDescriptor != null)
					{
						type = propertyDescriptor.PropertyType;
					}
				}
				if (type == null)
				{
					type = enumerator.Current.GetType();
				}
			}
			if (type != null)
			{
				if (type == typeof(DateTime))
				{
					result = ChartValueTypes.DateTime;
				}
				else if (type == typeof(DateTimeOffset))
				{
					result = ChartValueTypes.DateTimeOffset;
				}
				else if (type == typeof(TimeSpan))
				{
					result = ChartValueTypes.Time;
				}
				else if (type == typeof(double))
				{
					result = ChartValueTypes.Double;
				}
				else if (type == typeof(int))
				{
					result = ChartValueTypes.Int;
				}
				else if (type == typeof(long))
				{
					result = ChartValueTypes.Long;
				}
				else if (type == typeof(float))
				{
					result = ChartValueTypes.Single;
				}
				else if (type == typeof(string))
				{
					result = ChartValueTypes.String;
				}
				else if (type == typeof(uint))
				{
					result = ChartValueTypes.UInt;
				}
				else if (type == typeof(ulong))
				{
					result = ChartValueTypes.ULong;
				}
			}
			return result;
		}

		public DataPoint FindValue(double valueToFind, string useValue, ref int startFromIndex)
		{
			while (startFromIndex < this.Count)
			{
				if (this[startFromIndex].GetValueByName(useValue) == valueToFind)
				{
					return this[startFromIndex];
				}
				startFromIndex++;
			}
			startFromIndex = -1;
			return null;
		}

		public DataPoint FindValue(double valueToFind, string useValue)
		{
			int num = 0;
			return this.FindValue(valueToFind, useValue, ref num);
		}

		public DataPoint FindValue(double valueToFind)
		{
			int num = 0;
			return this.FindValue(valueToFind, "Y", ref num);
		}

		public DataPoint FindMaxValue(string useValue, ref int startFromIndex)
		{
			double num = -1.7976931348623157E+308;
			int num2 = -1;
			for (num2 = 0; num2 < this.Count; num2++)
			{
				if ((!this[num2].Empty || !useValue.StartsWith("Y", StringComparison.OrdinalIgnoreCase)) && num < this[num2].GetValueByName(useValue))
				{
					num = this[num2].GetValueByName(useValue);
				}
			}
			for (num2 = startFromIndex; num2 < this.Count; num2++)
			{
				if (this[num2].GetValueByName(useValue) == num)
				{
					startFromIndex = num2;
					return this[num2];
				}
			}
			startFromIndex = -1;
			return null;
		}

		public DataPoint FindMaxValue(string useValue)
		{
			int num = 0;
			return this.FindMaxValue(useValue, ref num);
		}

		public DataPoint FindMaxValue()
		{
			int num = 0;
			return this.FindMaxValue("Y", ref num);
		}

		public DataPoint FindMinValue(string useValue, ref int startFromIndex)
		{
			double num = 1.7976931348623157E+308;
			int num2 = -1;
			for (num2 = 0; num2 < this.Count; num2++)
			{
				if ((!this[num2].Empty || !useValue.StartsWith("Y", StringComparison.OrdinalIgnoreCase)) && num > this[num2].GetValueByName(useValue))
				{
					num = this[num2].GetValueByName(useValue);
				}
			}
			for (num2 = startFromIndex; num2 < this.Count; num2++)
			{
				if (this[num2].GetValueByName(useValue) == num)
				{
					startFromIndex = num2;
					return this[num2];
				}
			}
			startFromIndex = -1;
			return null;
		}

		public DataPoint FindMinValue(string useValue)
		{
			int num = 0;
			return this.FindMinValue(useValue, ref num);
		}

		public DataPoint FindMinValue()
		{
			int num = 0;
			return this.FindMinValue("Y", ref num);
		}

		public void Clear()
		{
			this.array.Clear();
		}

		bool IList.Contains(object value)
		{
			return this.array.Contains(value);
		}

		public bool Contains(DataPoint value)
		{
			return this.array.Contains(value);
		}

		int IList.IndexOf(object value)
		{
			return this.array.IndexOf(value);
		}

		public int IndexOf(DataPoint value)
		{
			return this.array.IndexOf(value);
		}

		void IList.Remove(object value)
		{
			this.array.Remove(value);
		}

		public void Remove(DataPoint value)
		{
			this.array.Remove(value);
		}

		public void RemoveAt(int index)
		{
			this.array.RemoveAt(index);
		}

		public void RemoveRange(int index, int count)
		{
			this.array.RemoveRange(index, count);
		}

		public int Add(DataPoint value)
		{
			return this.Add((object)value);
		}

		public int Add(object value)
		{
			if (value is DataPoint)
			{
				DataPoint dataPoint = (DataPoint)value;
				this.DataPointInit(ref dataPoint);
				return this.array.Add(value);
			}
			DataPoint dataPoint2 = new DataPoint(this.series);
			this.DataPointInit(ref dataPoint2);
			dataPoint2.SetValueY(value);
			return this.array.Add(dataPoint2);
		}

		public void Insert(int index, DataPoint value)
		{
			this.Insert(index, (object)value);
		}

		public void Insert(int index, object value)
		{
			if (value is DataPoint)
			{
				DataPoint dataPoint = (DataPoint)value;
				this.DataPointInit(ref dataPoint);
				this.array.Insert(index, value);
			}
			else
			{
				DataPoint dataPoint2 = new DataPoint(this.series);
				this.DataPointInit(ref dataPoint2);
				dataPoint2.SetValueY(value);
				this.array.Insert(index, dataPoint2);
			}
		}

		public void CopyTo(Array array, int index)
		{
			this.array.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return this.array.GetEnumerator();
		}
	}
}
