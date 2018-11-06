using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class Filters
	{
		internal enum FilterTypes
		{
			DataSetFilter,
			DataRegionFilter,
			GroupFilter
		}

		private sealed class MyTopComparer : IComparer
		{
			private CompareInfo m_compareInfo;

			private CompareOptions m_compareOptions;

			internal MyTopComparer(CompareInfo compareInfo, CompareOptions compareOptions)
			{
				this.m_compareInfo = compareInfo;
				this.m_compareOptions = compareOptions;
			}

			int IComparer.Compare(object x, object y)
			{
				return ReportProcessing.CompareTo(y, x, this.m_compareInfo, this.m_compareOptions);
			}
		}

		private sealed class MyBottomComparer : IComparer
		{
			private CompareInfo m_compareInfo;

			private CompareOptions m_compareOptions;

			internal MyBottomComparer(CompareInfo compareInfo, CompareOptions compareOptions)
			{
				this.m_compareInfo = compareInfo;
				this.m_compareOptions = compareOptions;
			}

			int IComparer.Compare(object x, object y)
			{
				return ReportProcessing.CompareTo(x, y, this.m_compareInfo, this.m_compareOptions);
			}
		}

		private abstract class MySortedList
		{
			protected IComparer m_comparer;

			protected ArrayList m_keys;

			protected ArrayList m_values;

			protected Filters m_filters;

			internal int Count
			{
				get
				{
					if (this.m_keys == null)
					{
						return 0;
					}
					return this.m_keys.Count;
				}
			}

			internal MySortedList(IComparer comparer, Filters filters)
			{
				this.m_comparer = comparer;
				this.m_filters = filters;
			}

			internal abstract bool Add(object key, object value, FilterInfo owner);

			internal virtual void TrimInstanceSet(int maxSize, FilterInfo owner)
			{
			}

			protected int Search(object key)
			{
				Global.Tracer.Assert(null != this.m_keys, "(null != m_keys)");
				int num = this.m_keys.BinarySearch(key, this.m_comparer);
				if (num < 0)
				{
					num = ~num;
				}
				else
				{
					for (num++; num < this.m_keys.Count && this.m_comparer.Compare(this.m_keys[num - 1], this.m_keys[num]) == 0; num++)
					{
					}
				}
				return num;
			}
		}

		private sealed class MySortedListWithMaxSize : MySortedList
		{
			private int m_maxSize;

			internal MySortedListWithMaxSize(IComparer comparer, int maxSize, Filters filters)
				: base(comparer, filters)
			{
				if (0 > maxSize)
				{
					this.m_maxSize = 0;
				}
				else
				{
					this.m_maxSize = maxSize;
				}
			}

			internal override bool Add(object key, object value, FilterInfo owner)
			{
				if (base.m_keys == null)
				{
					base.m_keys = new ArrayList(Math.Min(1000, this.m_maxSize));
					base.m_values = new ArrayList(Math.Min(1000, this.m_maxSize));
				}
				int num;
				try
				{
					num = base.Search(key);
				}
				catch
				{
					throw new ReportProcessingException(base.m_filters.RegisterComparisonError());
				}
				int count = base.m_keys.Count;
				bool flag = false;
				if (count < this.m_maxSize)
				{
					flag = true;
				}
				else if (num < count)
				{
					flag = true;
					if (num < this.m_maxSize)
					{
						int num2 = this.m_maxSize - 1;
						object y = (num != this.m_maxSize - 1) ? base.m_keys[num2 - 1] : key;
						int num3;
						try
						{
							num3 = base.m_comparer.Compare(base.m_keys[num2], y);
						}
						catch
						{
							throw new ReportProcessingException(base.m_filters.RegisterComparisonError());
						}
						if (num3 != 0)
						{
							for (int i = num2; i < count; i++)
							{
								owner.Remove((DataInstanceInfo)base.m_values[i]);
							}
							base.m_keys.RemoveRange(num2, count - num2);
							base.m_values.RemoveRange(num2, count - num2);
						}
					}
				}
				else if (count > 0)
				{
					try
					{
						if (base.m_comparer.Compare(base.m_keys[count - 1], key) == 0)
						{
							flag = true;
						}
					}
					catch
					{
						throw new ReportProcessingException(base.m_filters.RegisterComparisonError());
					}
				}
				if (flag)
				{
					base.m_keys.Insert(num, key);
					base.m_values.Insert(num, value);
				}
				return flag;
			}
		}

		private sealed class MySortedListWithoutMaxSize : MySortedList
		{
			internal MySortedListWithoutMaxSize(IComparer comparer, Filters filters)
				: base(comparer, filters)
			{
			}

			internal override bool Add(object key, object value, FilterInfo owner)
			{
				if (base.m_keys == null)
				{
					base.m_keys = new ArrayList();
					base.m_values = new ArrayList();
				}
				int index;
				try
				{
					index = base.Search(key);
				}
				catch
				{
					throw new ReportProcessingException(base.m_filters.RegisterComparisonError());
				}
				base.m_keys.Insert(index, key);
				base.m_values.Insert(index, value);
				return true;
			}

			internal override void TrimInstanceSet(int maxSize, FilterInfo owner)
			{
				int count = base.Count;
				int num = count;
				if (count > maxSize)
				{
					if (0 < maxSize)
					{
						for (num = maxSize; num < count && base.m_comparer.Compare(base.m_keys[num - 1], base.m_keys[num]) == 0; num++)
						{
						}
						for (int i = num; i < count; i++)
						{
							owner.Remove((DataInstanceInfo)base.m_values[i]);
						}
						base.m_keys.RemoveRange(num, count - num);
						base.m_values.RemoveRange(num, count - num);
					}
					else
					{
						owner.RemoveAll();
						base.m_keys = null;
						base.m_values = null;
					}
				}
			}
		}

		private sealed class FilterInfo
		{
			private double m_percentage = -1.0;

			private MySortedList m_dataInstances;

			private DataInstanceInfo m_firstInstance;

			private DataInstanceInfo m_currentInstance;

			internal double Percentage
			{
				get
				{
					return this.m_percentage;
				}
				set
				{
					this.m_percentage = value;
				}
			}

			internal int InstanceCount
			{
				get
				{
					return this.m_dataInstances.Count;
				}
			}

			internal DataInstanceInfo FirstInstance
			{
				get
				{
					return this.m_firstInstance;
				}
			}

			internal FilterInfo(MySortedList dataInstances)
			{
				Global.Tracer.Assert(null != dataInstances, "(null != dataInstances)");
				this.m_dataInstances = dataInstances;
			}

			internal bool Add(object key, object dataInstance)
			{
				DataInstanceInfo dataInstanceInfo = new DataInstanceInfo();
				dataInstanceInfo.DataInstance = dataInstance;
				bool flag = this.m_dataInstances.Add(key, dataInstanceInfo, this);
				if (flag)
				{
					if (this.m_firstInstance == null)
					{
						this.m_firstInstance = dataInstanceInfo;
					}
					if (this.m_currentInstance != null)
					{
						this.m_currentInstance.NextInstance = dataInstanceInfo;
					}
					dataInstanceInfo.PrevInstance = this.m_currentInstance;
					dataInstanceInfo.NextInstance = null;
					this.m_currentInstance = dataInstanceInfo;
				}
				return flag;
			}

			internal void Remove(DataInstanceInfo instance)
			{
				if (instance.NextInstance != null)
				{
					instance.NextInstance.PrevInstance = instance.PrevInstance;
				}
				else
				{
					Global.Tracer.Assert(instance == this.m_currentInstance, "(instance == m_currentInstance)");
					this.m_currentInstance = instance.PrevInstance;
				}
				if (instance.PrevInstance != null)
				{
					instance.PrevInstance.NextInstance = instance.NextInstance;
				}
				else
				{
					Global.Tracer.Assert(instance == this.m_firstInstance, "(instance == m_firstInstance)");
					this.m_firstInstance = instance.NextInstance;
				}
			}

			internal void RemoveAll()
			{
				this.m_firstInstance = null;
				this.m_currentInstance = null;
			}

			internal void TrimInstanceSet(int maxSize)
			{
				this.m_dataInstances.TrimInstanceSet(maxSize, this);
			}
		}

		private sealed class DataInstanceInfo
		{
			internal object DataInstance;

			internal DataInstanceInfo PrevInstance;

			internal DataInstanceInfo NextInstance;
		}

		private FilterTypes m_filterType;

		private ReportProcessing.IFilterOwner m_owner;

		private FilterList m_filters;

		private ObjectType m_objectType;

		private string m_objectName;

		private ReportProcessing.ProcessingContext m_processingContext;

		private int m_startFilterIndex;

		private int m_currentSpecialFilterIndex = -1;

		private FilterInfo m_filterInfo;

		private bool m_failFilters;

		internal bool FailFilters
		{
			set
			{
				this.m_failFilters = value;
			}
		}

		internal Filters(FilterTypes filterType, ReportProcessing.IFilterOwner owner, FilterList filters, ObjectType objectType, string objectName, ReportProcessing.ProcessingContext processingContext)
		{
			this.m_filterType = filterType;
			this.m_owner = owner;
			this.m_filters = filters;
			this.m_objectType = objectType;
			this.m_objectName = objectName;
			this.m_processingContext = processingContext;
		}

		internal bool PassFilters(object dataInstance)
		{
			bool flag = default(bool);
			return this.PassFilters(dataInstance, out flag);
		}

		private void ThrowIfErrorOccurred(string propertyName, bool errorOccurred, DataFieldStatus fieldStatus)
		{
			if (!errorOccurred)
			{
				return;
			}
			if (fieldStatus != 0)
			{
				throw new ReportProcessingException(ErrorCode.rsFilterFieldError, this.m_objectType, this.m_objectName, propertyName, ReportRuntime.GetErrorName(fieldStatus, null));
			}
			throw new ReportProcessingException(ErrorCode.rsFilterEvaluationError, this.m_objectType, this.m_objectName, propertyName);
		}

		internal bool PassFilters(object dataInstance, out bool specialFilter)
		{
			bool flag = true;
			specialFilter = false;
			if (this.m_failFilters)
			{
				return false;
			}
			if (this.m_filters != null)
			{
				for (int num = this.m_startFilterIndex; num < this.m_filters.Count; num++)
				{
					Filter filter = this.m_filters[num];
					if (Filter.Operators.Like == filter.Operator)
					{
						StringResult stringResult = this.m_processingContext.ReportRuntime.EvaluateFilterStringExpression(filter, this.m_objectType, this.m_objectName);
						this.ThrowIfErrorOccurred("FilterExpression", stringResult.ErrorOccurred, stringResult.FieldStatus);
						Global.Tracer.Assert(null != filter.Values);
						Global.Tracer.Assert(1 <= filter.Values.Count);
						StringResult stringResult2 = this.m_processingContext.ReportRuntime.EvaluateFilterStringValue(filter, 0, this.m_objectType, this.m_objectName);
						this.ThrowIfErrorOccurred("FilterValue", stringResult2.ErrorOccurred, stringResult2.FieldStatus);
                        if (stringResult.Value != null && stringResult2.Value != null)
                        {
                            if (!stringResult.Value.Equals(stringResult2.Value, StringComparison.OrdinalIgnoreCase))
                            {
                                flag = false;
                            }
                        }
                        else if (stringResult.Value != null || stringResult2.Value != null)
                        {
                            flag = false;
                        }
					}
					else
					{
						VariantResult variantResult = this.m_processingContext.ReportRuntime.EvaluateFilterVariantExpression(filter, this.m_objectType, this.m_objectName);
						this.ThrowIfErrorOccurred("FilterExpression", variantResult.ErrorOccurred, variantResult.FieldStatus);
						object value = variantResult.Value;
						if (filter.Operator == Filter.Operators.Equal || Filter.Operators.NotEqual == filter.Operator || Filter.Operators.GreaterThan == filter.Operator || Filter.Operators.GreaterThanOrEqual == filter.Operator || Filter.Operators.LessThan == filter.Operator || Filter.Operators.LessThanOrEqual == filter.Operator)
						{
							object y = this.EvaluateFilterValue(filter);
							int num2 = 0;
							try
							{
								num2 = ReportProcessing.CompareTo(value, y, this.m_processingContext.CompareInfo, this.m_processingContext.ClrCompareOptions);
							}
							catch (ReportProcessingException_ComparisonError e)
							{
								throw new ReportProcessingException(this.RegisterComparisonError(e));
							}
							catch
							{
								throw new ReportProcessingException(this.RegisterComparisonError());
							}
							if (flag)
							{
								switch (filter.Operator)
								{
								case Filter.Operators.Equal:
									if (num2 != 0)
									{
										flag = false;
									}
									break;
								case Filter.Operators.NotEqual:
									if (num2 == 0)
									{
										flag = false;
									}
									break;
								case Filter.Operators.GreaterThan:
									if (0 >= num2)
									{
										flag = false;
									}
									break;
								case Filter.Operators.GreaterThanOrEqual:
									if (0 > num2)
									{
										flag = false;
									}
									break;
								case Filter.Operators.LessThan:
									if (0 <= num2)
									{
										flag = false;
									}
									break;
								case Filter.Operators.LessThanOrEqual:
									if (0 < num2)
									{
										flag = false;
									}
									break;
								}
							}
						}
						else if (Filter.Operators.In == filter.Operator)
						{
							object[] array = this.EvaluateFilterValues(filter);
							flag = false;
							if (array != null)
							{
								for (int i = 0; i < array.Length; i++)
								{
									try
									{
										if (array[i] is ICollection)
										{
											foreach (object item in (ICollection)array[i])
											{
												if (ReportProcessing.CompareTo(value, item, this.m_processingContext.CompareInfo, this.m_processingContext.ClrCompareOptions) == 0)
												{
													flag = true;
													break;
												}
											}
										}
										else if (ReportProcessing.CompareTo(value, array[i], this.m_processingContext.CompareInfo, this.m_processingContext.ClrCompareOptions) == 0)
										{
											flag = true;
										}
										if (flag)
										{
											goto IL_05f9;
										}
									}
									catch (ReportProcessingException_ComparisonError e2)
									{
										throw new ReportProcessingException(this.RegisterComparisonError(e2));
									}
									catch
									{
										throw new ReportProcessingException(this.RegisterComparisonError());
									}
								}
							}
						}
						else if (Filter.Operators.Between == filter.Operator)
						{
							object[] array2 = this.EvaluateFilterValues(filter);
							flag = false;
							Global.Tracer.Assert(array2 != null && 2 == array2.Length);
							try
							{
								if (0 <= ReportProcessing.CompareTo(value, array2[0], this.m_processingContext.CompareInfo, this.m_processingContext.ClrCompareOptions) && 0 >= ReportProcessing.CompareTo(value, array2[1], this.m_processingContext.CompareInfo, this.m_processingContext.ClrCompareOptions))
								{
									flag = true;
								}
							}
							catch (ReportProcessingException_ComparisonError e3)
							{
								throw new ReportProcessingException(this.RegisterComparisonError(e3));
							}
							catch
							{
								throw new ReportProcessingException(this.RegisterComparisonError());
							}
						}
						else if (Filter.Operators.TopN == filter.Operator || Filter.Operators.BottomN == filter.Operator)
						{
							if (this.m_filterInfo == null)
							{
								Global.Tracer.Assert(filter.Values != null && 1 == filter.Values.Count);
								IntegerResult integerResult = this.m_processingContext.ReportRuntime.EvaluateFilterIntegerValue(filter, 0, this.m_objectType, this.m_objectName);
								this.ThrowIfErrorOccurred("FilterValue", integerResult.ErrorOccurred, integerResult.FieldStatus);
								int value2 = integerResult.Value;
								IComparer comparer = (Filter.Operators.TopN != filter.Operator) ? ((IComparer)new MyBottomComparer(this.m_processingContext.CompareInfo, this.m_processingContext.ClrCompareOptions)) : ((IComparer)new MyTopComparer(this.m_processingContext.CompareInfo, this.m_processingContext.ClrCompareOptions));
								this.InitFilterInfos(new MySortedListWithMaxSize(comparer, value2, this), num);
							}
							this.SortAndSave(value, dataInstance);
							flag = false;
							specialFilter = true;
						}
						else if (Filter.Operators.TopPercent == filter.Operator || Filter.Operators.BottomPercent == filter.Operator)
						{
							if (this.m_filterInfo == null)
							{
								Global.Tracer.Assert(filter.Values != null && 1 == filter.Values.Count);
								FloatResult floatResult = this.m_processingContext.ReportRuntime.EvaluateFilterIntegerOrFloatValue(filter, 0, this.m_objectType, this.m_objectName);
								this.ThrowIfErrorOccurred("FilterValue", floatResult.ErrorOccurred, floatResult.FieldStatus);
								double value3 = floatResult.Value;
								IComparer comparer2 = (Filter.Operators.TopPercent != filter.Operator) ? ((IComparer)new MyBottomComparer(this.m_processingContext.CompareInfo, this.m_processingContext.ClrCompareOptions)) : ((IComparer)new MyTopComparer(this.m_processingContext.CompareInfo, this.m_processingContext.ClrCompareOptions));
								this.InitFilterInfos(new MySortedListWithoutMaxSize(comparer2, this), num);
								this.m_filterInfo.Percentage = value3;
							}
							this.SortAndSave(value, dataInstance);
							flag = false;
							specialFilter = true;
						}
					}
					goto IL_05f9;
					IL_05f9:
					if (!flag)
					{
						return false;
					}
				}
			}
			return flag;
		}

		internal void FinishReadingRows()
		{
			if (this.m_filterInfo != null)
			{
				FilterInfo filterInfo = this.m_filterInfo;
				this.m_filterInfo = null;
				this.m_startFilterIndex = this.m_currentSpecialFilterIndex + 1;
				bool flag = this.m_startFilterIndex >= this.m_filters.Count;
				this.TrimInstanceSet(filterInfo);
				for (DataInstanceInfo dataInstanceInfo = filterInfo.FirstInstance; dataInstanceInfo != null; dataInstanceInfo = dataInstanceInfo.NextInstance)
				{
					object dataInstance = dataInstanceInfo.DataInstance;
					if (FilterTypes.GroupFilter == this.m_filterType)
					{
						ReportProcessing.RuntimeGroupLeafObj runtimeGroupLeafObj = (ReportProcessing.RuntimeGroupLeafObj)dataInstance;
						runtimeGroupLeafObj.SetupEnvironment();
						if (flag || this.PassFilters(dataInstance))
						{
							runtimeGroupLeafObj.PostFilterNextRow();
						}
						else
						{
							runtimeGroupLeafObj.FailFilter();
						}
					}
					else
					{
						FieldImpl[] fields = (FieldImpl[])dataInstance;
						this.m_processingContext.ReportObjectModel.FieldsImpl.SetFields(fields);
						if (flag || this.PassFilters(dataInstance))
						{
							this.m_owner.PostFilterNextRow();
						}
					}
				}
				filterInfo = null;
				this.FinishReadingRows();
			}
		}

		private ProcessingMessageList RegisterComparisonError()
		{
			return this.RegisterComparisonError(null);
		}

		private ProcessingMessageList RegisterComparisonError(ReportProcessingException_ComparisonError e)
		{
			if (e == null)
			{
				this.m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsComparisonError, Severity.Error, this.m_objectType, this.m_objectName, "FilterExpression");
			}
			else
			{
				this.m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsComparisonTypeError, Severity.Error, this.m_objectType, this.m_objectName, "FilterExpression", e.TypeX, e.TypeY);
			}
			return this.m_processingContext.ErrorContext.Messages;
		}

		private void TrimInstanceSet(FilterInfo filterInfo)
		{
			if (-1.0 != filterInfo.Percentage)
			{
				int instanceCount = filterInfo.InstanceCount;
				double num = (double)instanceCount * filterInfo.Percentage / 100.0;
				if (num <= 0.0)
				{
					instanceCount = 0;
				}
				else
				{
					try
					{
						instanceCount = Convert.ToInt32(num);
					}
					catch
					{
						throw new ReportProcessingException(ErrorCode.rsFilterEvaluationError, "FilterValues");
					}
				}
				filterInfo.TrimInstanceSet(instanceCount);
			}
		}

		private object EvaluateFilterValue(Filter filterDef)
		{
			Global.Tracer.Assert(filterDef.Values != null, "(filterDef.Values != null)");
			Global.Tracer.Assert(filterDef.Values.Count > 0, "(filterDef.Values.Count > 0)");
			VariantResult variantResult = this.m_processingContext.ReportRuntime.EvaluateFilterVariantValue(filterDef, 0, this.m_objectType, this.m_objectName);
			this.ThrowIfErrorOccurred("FilterValue", variantResult.ErrorOccurred, variantResult.FieldStatus);
			return variantResult.Value;
		}

		private object[] EvaluateFilterValues(Filter filterDef)
		{
			if (filterDef.Values != null)
			{
				object[] array = new object[filterDef.Values.Count];
				for (int num = filterDef.Values.Count - 1; num >= 0; num--)
				{
					VariantResult variantResult = this.m_processingContext.ReportRuntime.EvaluateFilterVariantValue(filterDef, num, this.m_objectType, this.m_objectName);
					this.ThrowIfErrorOccurred("FilterValues", variantResult.ErrorOccurred, variantResult.FieldStatus);
					array[num] = variantResult.Value;
				}
				return array;
			}
			return null;
		}

		private void SortAndSave(object key, object dataInstance)
		{
			if (this.m_filterInfo.Add(key, dataInstance) && FilterTypes.GroupFilter != this.m_filterType)
			{
				this.m_processingContext.ReportObjectModel.FieldsImpl.GetAndSaveFields();
			}
		}

		private void InitFilterInfos(MySortedList dataInstanceList, int currentFilterIndex)
		{
			this.m_filterInfo = new FilterInfo(dataInstanceList);
			if (-1 == this.m_currentSpecialFilterIndex && FilterTypes.DataRegionFilter == this.m_filterType)
			{
				this.m_processingContext.AddSpecialDataRegionFilters(this);
			}
			this.m_currentSpecialFilterIndex = currentFilterIndex;
		}
	}
}
