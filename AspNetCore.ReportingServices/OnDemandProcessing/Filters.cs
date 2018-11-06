using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class Filters : IStaticReferenceable
	{
		internal enum FilterTypes
		{
			DataSetFilter,
			DataRegionFilter,
			GroupFilter
		}

		private sealed class MyTopComparer : IComparer
		{
			private readonly IDataComparer m_comparer;

			internal MyTopComparer(IDataComparer comparer)
			{
				this.m_comparer = comparer;
			}

			int IComparer.Compare(object x, object y)
			{
				object key = ((FilterKey)x).Key;
				object key2 = ((FilterKey)y).Key;
				return this.m_comparer.Compare(key2, key);
			}
		}

		private sealed class MyBottomComparer : IComparer
		{
			private readonly IDataComparer m_comparer;

			internal MyBottomComparer(IDataComparer comparer)
			{
				this.m_comparer = comparer;
			}

			int IComparer.Compare(object x, object y)
			{
				object key = ((FilterKey)x).Key;
				object key2 = ((FilterKey)y).Key;
				return this.m_comparer.Compare(key, key2);
			}
		}

		private abstract class MySortedList
		{
			protected IComparer m_comparer;

			protected ScalableList<FilterKey> m_keys;

			protected ScalableHybridList<object> m_values;

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

			internal IEnumerator<object> Instances
			{
				get
				{
					if (this.m_keys == null)
					{
						return null;
					}
					return this.m_values.GetEnumerator();
				}
			}

			internal MySortedList(IComparer comparer, Filters filters)
			{
				this.m_comparer = comparer;
				this.m_filters = filters;
			}

			internal abstract bool Add(FilterKey key, object value, FilterInfo owner);

			internal virtual void TrimInstanceSet(int maxSize, FilterInfo owner)
			{
			}

			protected int Search(FilterKey key)
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

			internal void Clear()
			{
				if (this.m_values != null)
				{
					this.m_values.Clear();
				}
				if (this.m_keys != null)
				{
					this.m_keys.Clear();
				}
			}

			protected void InitLists(int bucketSize, int initialCapacity)
			{
				OnDemandProcessingContext processingContext = this.m_filters.m_processingContext;
				processingContext.EnsureScalabilitySetup();
				this.m_keys = new ScalableList<FilterKey>(this.m_filters.m_scalabilityPriority, processingContext.TablixProcessingScalabilityCache, bucketSize, initialCapacity);
				this.m_values = new ScalableHybridList<object>(this.m_filters.m_scalabilityPriority, processingContext.TablixProcessingScalabilityCache, bucketSize, initialCapacity);
			}
		}

		internal sealed class FilterKey : IStorable, IPersistable
		{
			internal object Key;

			internal int ValueIndex;

			private static readonly Declaration m_declaration = FilterKey.GetDeclaration();

			public int Size
			{
				get
				{
					return ItemSizes.SizeOf(this.Key) + 4;
				}
			}

			public void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(FilterKey.m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.Key:
						writer.WriteVariantOrPersistable(this.Key);
						break;
					case MemberName.Value:
						writer.Write(this.ValueIndex);
						break;
					default:
						Global.Tracer.Assert(false);
						break;
					}
				}
			}

			public void Deserialize(IntermediateFormatReader reader)
			{
				reader.RegisterDeclaration(FilterKey.m_declaration);
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.Key:
						this.Key = reader.ReadVariant();
						break;
					case MemberName.Value:
						this.ValueIndex = reader.ReadInt32();
						break;
					default:
						Global.Tracer.Assert(false);
						break;
					}
				}
			}

			public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
				Global.Tracer.Assert(false, "No references to resolve");
			}

			public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
			{
				return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.FilterKey;
			}

			public static Declaration GetDeclaration()
			{
				if (FilterKey.m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.Key, Token.Object));
					list.Add(new MemberInfo(MemberName.Value, Token.Int32));
					return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.FilterKey, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
				}
				return FilterKey.m_declaration;
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

			internal override bool Add(FilterKey key, object value, FilterInfo owner)
			{
				if (base.m_keys == null)
				{
					int num = Math.Min(500, this.m_maxSize);
					base.InitLists(num, num);
				}
				int num2;
				try
				{
					num2 = base.Search(key);
				}
				catch (Exception e)
				{
					if (AsynchronousExceptionDetection.IsStoppingException(e))
					{
						throw;
					}
					throw new ReportProcessingException(base.m_filters.RegisterComparisonError());
				}
				int count = base.m_keys.Count;
				bool flag = false;
				if (count < this.m_maxSize)
				{
					flag = true;
				}
				else if (num2 < count)
				{
					flag = true;
					if (num2 < this.m_maxSize)
					{
						int num3 = this.m_maxSize - 1;
						object y = (num2 != this.m_maxSize - 1) ? base.m_keys[num3 - 1] : key;
						int num4;
						try
						{
							num4 = base.m_comparer.Compare(base.m_keys[num3], y);
						}
						catch (Exception e2)
						{
							if (AsynchronousExceptionDetection.IsStoppingException(e2))
							{
								throw;
							}
							throw new ReportProcessingException(base.m_filters.RegisterComparisonError());
						}
						if (num4 != 0)
						{
							for (int i = num3; i < count; i++)
							{
								FilterKey filterKey = base.m_keys[i];
								base.m_values.Remove(filterKey.ValueIndex);
							}
							base.m_keys.RemoveRange(num3, count - num3);
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
					catch (Exception e3)
					{
						if (AsynchronousExceptionDetection.IsStoppingException(e3))
						{
							throw;
						}
						throw new ReportProcessingException(base.m_filters.RegisterComparisonError());
					}
				}
				if (flag)
				{
					key.ValueIndex = base.m_values.Add(value);
					base.m_keys.Insert(num2, key);
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

			internal override bool Add(FilterKey key, object value, FilterInfo owner)
			{
				if (base.m_keys == null)
				{
					base.InitLists(500, 500);
				}
				int index;
				try
				{
					index = base.Search(key);
				}
				catch (Exception e)
				{
					if (AsynchronousExceptionDetection.IsStoppingException(e))
					{
						throw;
					}
					throw new ReportProcessingException(base.m_filters.RegisterComparisonError());
				}
				key.ValueIndex = base.m_values.Add(value);
				base.m_keys.Insert(index, key);
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
							FilterKey filterKey = base.m_keys[i];
							base.m_values.Remove(filterKey.ValueIndex);
						}
						base.m_keys.RemoveRange(num, count - num);
					}
					else
					{
						if (base.m_keys != null)
						{
							base.m_keys.Dispose();
						}
						if (base.m_values != null)
						{
							base.m_values.Dispose();
						}
						base.m_keys = null;
						base.m_values = null;
					}
				}
			}
		}

		private sealed class FilterInfo
		{
			private double m_percentage = -1.0;

			private AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators m_operator = AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.TopPercent;

			private MySortedList m_dataInstances;

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

			internal AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators Operator
			{
				get
				{
					return this.m_operator;
				}
				set
				{
					this.m_operator = value;
				}
			}

			internal int InstanceCount
			{
				get
				{
					return this.m_dataInstances.Count;
				}
			}

			internal IEnumerator<object> Instances
			{
				get
				{
					return this.m_dataInstances.Instances;
				}
			}

			internal FilterInfo(MySortedList dataInstances)
			{
				Global.Tracer.Assert(null != dataInstances, "(null != dataInstances)");
				this.m_dataInstances = dataInstances;
			}

			internal bool Add(object key, object dataInstance)
			{
				FilterKey filterKey = new FilterKey();
				filterKey.Key = key;
				return this.m_dataInstances.Add(filterKey, dataInstance, this);
			}

			internal void RemoveAll()
			{
				this.m_dataInstances.Clear();
			}

			internal void TrimInstanceSet(int maxSize)
			{
				this.m_dataInstances.TrimInstanceSet(maxSize, this);
			}
		}

		private FilterTypes m_filterType;

		private IReference<AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner> m_owner;

		private AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner m_ownerObj;

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.Filter> m_filters;

		private AspNetCore.ReportingServices.ReportProcessing.ObjectType m_objectType;

		private string m_objectName;

		private OnDemandProcessingContext m_processingContext;

		private int m_startFilterIndex;

		private int m_currentSpecialFilterIndex = -1;

		private FilterInfo m_filterInfo;

		private bool m_failFilters;

		private int m_scalabilityPriority;

		private int m_id = -2147483648;

		internal bool FailFilters
		{
			set
			{
				this.m_failFilters = value;
			}
		}

		int IStaticReferenceable.ID
		{
			get
			{
				return this.m_id;
			}
		}

		internal Filters(FilterTypes filterType, IReference<AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner> owner, List<AspNetCore.ReportingServices.ReportIntermediateFormat.Filter> filters, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, OnDemandProcessingContext processingContext, int scalabilityPriority)
			: this(filterType, filters, objectType, objectName, processingContext, scalabilityPriority)
		{
			this.m_owner = owner;
		}

		internal Filters(FilterTypes filterType, RuntimeParameterDataSet owner, List<AspNetCore.ReportingServices.ReportIntermediateFormat.Filter> filters, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, OnDemandProcessingContext processingContext, int scalabilityPriority)
			: this(filterType, filters, objectType, objectName, processingContext, scalabilityPriority)
		{
			this.m_ownerObj = owner;
		}

		private Filters(FilterTypes filterType, List<AspNetCore.ReportingServices.ReportIntermediateFormat.Filter> filters, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, OnDemandProcessingContext processingContext, int scalabilityPriority)
		{
			this.m_filterType = filterType;
			this.m_filters = filters;
			this.m_objectType = objectType;
			this.m_objectName = objectName;
			this.m_processingContext = processingContext;
			this.m_scalabilityPriority = scalabilityPriority;
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
			switch (fieldStatus)
			{
			case DataFieldStatus.UnSupportedDataType:
			{
				string errMessage = string.Format(CultureInfo.CurrentCulture, RPRes.Keys.GetString(ProcessingErrorCode.rsInvalidExpressionDataType.ToString()), this.m_objectType, this.m_objectName, propertyName);
				throw new ReportProcessingException(errMessage, ErrorCode.rsFilterEvaluationError);
			}
			default:
				throw new ReportProcessingException(ErrorCode.rsFilterFieldError, this.m_objectType, this.m_objectName, propertyName, AspNetCore.ReportingServices.RdlExpressions.ReportRuntime.GetErrorName(fieldStatus, null));
			case DataFieldStatus.None:
				throw new ReportProcessingException(ErrorCode.rsFilterEvaluationError, this.m_objectType, this.m_objectName, propertyName);
			}
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
					AspNetCore.ReportingServices.ReportIntermediateFormat.Filter filter = this.m_filters[num];
					if (AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.Like == filter.Operator)
					{
						AspNetCore.ReportingServices.RdlExpressions.StringResult stringResult = this.m_processingContext.ReportRuntime.EvaluateFilterStringExpression(filter, this.m_objectType, this.m_objectName);
						this.ThrowIfErrorOccurred("FilterExpression", stringResult.ErrorOccurred, stringResult.FieldStatus);
						Global.Tracer.Assert(null != filter.Values, "(null != filter.Values)");
						Global.Tracer.Assert(1 <= filter.Values.Count, "(1 <= filter.Values.Count)");
						AspNetCore.ReportingServices.RdlExpressions.StringResult stringResult2 = this.m_processingContext.ReportRuntime.EvaluateFilterStringValue(filter, 0, this.m_objectType, this.m_objectName);
						this.ThrowIfErrorOccurred("FilterValue", stringResult2.ErrorOccurred, stringResult2.FieldStatus);
						if (stringResult.Value != null && stringResult2.Value != null)
						{
							if (!stringResult.Value.Equals( stringResult2.Value,StringComparison.OrdinalIgnoreCase))
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
						AspNetCore.ReportingServices.RdlExpressions.VariantResult variantResult = this.m_processingContext.ReportRuntime.EvaluateFilterVariantExpression(filter, this.m_objectType, this.m_objectName);
						this.ThrowIfErrorOccurred("FilterExpression", variantResult.ErrorOccurred, variantResult.FieldStatus);
						object value = variantResult.Value;
						if (filter.Operator == AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.Equal || AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.NotEqual == filter.Operator || AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.GreaterThan == filter.Operator || AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.GreaterThanOrEqual == filter.Operator || AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.LessThan == filter.Operator || AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.LessThanOrEqual == filter.Operator)
						{
							object value2 = this.EvaluateFilterValue(filter);
							int num2 = 0;
							try
							{
								num2 = this.Compare(value, value2);
							}
							catch (ReportProcessingException_SpatialTypeComparisonError reportProcessingException_SpatialTypeComparisonError)
							{
								throw new ReportProcessingException(this.RegisterSpatialTypeComparisonError(reportProcessingException_SpatialTypeComparisonError.Type));
							}
							catch (ReportProcessingException_ComparisonError e)
							{
								throw new ReportProcessingException(this.RegisterComparisonError(e));
							}
							catch (Exception e2)
							{
								if (AsynchronousExceptionDetection.IsStoppingException(e2))
								{
									throw;
								}
								throw new ReportProcessingException(this.RegisterComparisonError());
							}
							if (flag)
							{
								switch (filter.Operator)
								{
								case AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.Equal:
									if (num2 != 0)
									{
										flag = false;
									}
									break;
								case AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.NotEqual:
									if (num2 == 0)
									{
										flag = false;
									}
									break;
								case AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.GreaterThan:
									if (0 >= num2)
									{
										flag = false;
									}
									break;
								case AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.GreaterThanOrEqual:
									if (0 > num2)
									{
										flag = false;
									}
									break;
								case AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.LessThan:
									if (0 <= num2)
									{
										flag = false;
									}
									break;
								case AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.LessThanOrEqual:
									if (0 < num2)
									{
										flag = false;
									}
									break;
								}
							}
						}
						else if (AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.In == filter.Operator)
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
												if (this.Compare(value, item) == 0)
												{
													flag = true;
													break;
												}
											}
										}
										else if (this.Compare(value, array[i]) == 0)
										{
											flag = true;
										}
										if (flag)
										{
											goto IL_05f4;
										}
									}
									catch (ReportProcessingException_SpatialTypeComparisonError reportProcessingException_SpatialTypeComparisonError2)
									{
										throw new ReportProcessingException(this.RegisterSpatialTypeComparisonError(reportProcessingException_SpatialTypeComparisonError2.Type));
									}
									catch (ReportProcessingException_ComparisonError e3)
									{
										throw new ReportProcessingException(this.RegisterComparisonError(e3));
									}
									catch (Exception e4)
									{
										if (AsynchronousExceptionDetection.IsStoppingException(e4))
										{
											throw;
										}
										throw new ReportProcessingException(this.RegisterComparisonError());
									}
								}
							}
						}
						else if (AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.Between == filter.Operator)
						{
							object[] array2 = this.EvaluateFilterValues(filter);
							flag = false;
							Global.Tracer.Assert(array2 != null && 2 == array2.Length, "(null != values && 2 == values.Length)");
							try
							{
								if (0 <= this.Compare(value, array2[0]) && 0 >= this.Compare(value, array2[1]))
								{
									flag = true;
								}
							}
							catch (ReportProcessingException_SpatialTypeComparisonError reportProcessingException_SpatialTypeComparisonError3)
							{
								throw new ReportProcessingException(this.RegisterSpatialTypeComparisonError(reportProcessingException_SpatialTypeComparisonError3.Type));
							}
							catch (ReportProcessingException_ComparisonError e5)
							{
								throw new ReportProcessingException(this.RegisterComparisonError(e5));
							}
							catch (RSException)
							{
								throw;
							}
							catch (Exception e6)
							{
								if (AsynchronousExceptionDetection.IsStoppingException(e6))
								{
									throw;
								}
								throw new ReportProcessingException(this.RegisterComparisonError());
							}
						}
						else if (AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.TopN == filter.Operator || AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.BottomN == filter.Operator)
						{
							if (this.m_filterInfo == null)
							{
								Global.Tracer.Assert(filter.Values != null && 1 == filter.Values.Count, "(null != filter.Values && 1 == filter.Values.Count)");
								AspNetCore.ReportingServices.RdlExpressions.IntegerResult integerResult = this.m_processingContext.ReportRuntime.EvaluateFilterIntegerValue(filter, 0, this.m_objectType, this.m_objectName);
								this.ThrowIfErrorOccurred("FilterValue", integerResult.ErrorOccurred, integerResult.FieldStatus);
								int value3 = integerResult.Value;
								IComparer comparer = (AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.TopN != filter.Operator) ? ((IComparer)new MyBottomComparer(this.m_processingContext.ProcessingComparer)) : ((IComparer)new MyTopComparer(this.m_processingContext.ProcessingComparer));
								this.InitFilterInfos(new MySortedListWithMaxSize(comparer, value3, this), num);
							}
							this.SortAndSave(value, dataInstance);
							flag = false;
							specialFilter = true;
						}
						else if (AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.TopPercent == filter.Operator || AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.BottomPercent == filter.Operator)
						{
							if (this.m_filterInfo == null)
							{
								Global.Tracer.Assert(filter.Values != null && 1 == filter.Values.Count, "(null != filter.Values && 1 == filter.Values.Count)");
								AspNetCore.ReportingServices.RdlExpressions.FloatResult floatResult = this.m_processingContext.ReportRuntime.EvaluateFilterIntegerOrFloatValue(filter, 0, this.m_objectType, this.m_objectName);
								this.ThrowIfErrorOccurred("FilterValue", floatResult.ErrorOccurred, floatResult.FieldStatus);
								double value4 = floatResult.Value;
								IComparer comparer2 = (AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.TopPercent != filter.Operator) ? ((IComparer)new MyBottomComparer(this.m_processingContext.ProcessingComparer)) : ((IComparer)new MyTopComparer(this.m_processingContext.ProcessingComparer));
								this.InitFilterInfos(new MySortedListWithoutMaxSize(comparer2, this), num);
								this.m_filterInfo.Percentage = value4;
								this.m_filterInfo.Operator = filter.Operator;
							}
							this.SortAndSave(value, dataInstance);
							flag = false;
							specialFilter = true;
						}
					}
					goto IL_05f4;
					IL_05f4:
					if (!flag)
					{
						return false;
					}
				}
			}
			return flag;
		}

		private int Compare(object value1, object value2)
		{
			return this.m_processingContext.ProcessingComparer.Compare(value1, value2);
		}

		internal void FinishReadingGroups(AggregateUpdateContext context)
		{
			this.FinishFilters(context);
		}

		internal void FinishReadingRows()
		{
			this.FinishFilters(null);
		}

		private void FinishFilters(AggregateUpdateContext context)
		{
			if (this.m_filterInfo != null)
			{
				FilterInfo filterInfo = this.m_filterInfo;
				this.m_filterInfo = null;
				this.m_startFilterIndex = this.m_currentSpecialFilterIndex + 1;
				bool flag = this.m_startFilterIndex >= this.m_filters.Count;
				this.TrimInstanceSet(filterInfo);
				IEnumerator<object> instances = filterInfo.Instances;
				if (instances != null)
				{
					try
					{
						AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner filterOwner;
						if (this.m_owner != null)
						{
							this.m_owner.PinValue();
							filterOwner = this.m_owner.Value();
						}
						else
						{
							filterOwner = this.m_ownerObj;
						}
						while (instances.MoveNext())
						{
							object current = instances.Current;
							if (FilterTypes.GroupFilter == this.m_filterType)
							{
								IReference<RuntimeGroupLeafObj> reference = (IReference<RuntimeGroupLeafObj>)current;
								using (reference.PinValue())
								{
									RuntimeGroupLeafObj runtimeGroupLeafObj = reference.Value();
									runtimeGroupLeafObj.SetupEnvironment();
									if (flag || this.PassFilters(current))
									{
										runtimeGroupLeafObj.PostFilterNextRow(context);
									}
									else
									{
										runtimeGroupLeafObj.FailFilter();
									}
								}
							}
							else
							{
								DataFieldRow dataFieldRow = (DataFieldRow)current;
								dataFieldRow.SetFields(this.m_processingContext.ReportObjectModel.FieldsImpl);
								if (flag || this.PassFilters(current))
								{
									filterOwner.PostFilterNextRow();
								}
							}
						}
					}
					finally
					{
						if (this.m_owner != null)
						{
							this.m_owner.UnPinValue();
						}
					}
				}
				filterInfo.RemoveAll();
				filterInfo = null;
				this.FinishFilters(context);
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

		private ProcessingMessageList RegisterSpatialTypeComparisonError(string type)
		{
			this.m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsCannotCompareSpatialType, Severity.Error, this.m_objectType, this.m_objectName, type);
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
						instanceCount = ((filterInfo.Operator != AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.BottomPercent) ? ((int)Math.Ceiling(num)) : ((int)Math.Floor(num)));
					}
					catch (Exception e)
					{
						if (AsynchronousExceptionDetection.IsStoppingException(e))
						{
							throw;
						}
						throw new ReportProcessingException(ErrorCode.rsFilterEvaluationError, "FilterValues");
					}
				}
				filterInfo.TrimInstanceSet(instanceCount);
			}
		}

		private object EvaluateFilterValue(AspNetCore.ReportingServices.ReportIntermediateFormat.Filter filterDef)
		{
			Global.Tracer.Assert(filterDef.Values != null, "(filterDef.Values != null)");
			Global.Tracer.Assert(filterDef.Values.Count > 0, "(filterDef.Values.Count > 0)");
			AspNetCore.ReportingServices.RdlExpressions.VariantResult variantResult = this.m_processingContext.ReportRuntime.EvaluateFilterVariantValue(filterDef, 0, this.m_objectType, this.m_objectName);
			this.ThrowIfErrorOccurred("FilterValue", variantResult.ErrorOccurred, variantResult.FieldStatus);
			return variantResult.Value;
		}

		private object[] EvaluateFilterValues(AspNetCore.ReportingServices.ReportIntermediateFormat.Filter filterDef)
		{
			if (filterDef.Values != null)
			{
				object[] array = new object[filterDef.Values.Count];
				for (int num = filterDef.Values.Count - 1; num >= 0; num--)
				{
					AspNetCore.ReportingServices.RdlExpressions.VariantResult variantResult = this.m_processingContext.ReportRuntime.EvaluateFilterVariantValue(filterDef, num, this.m_objectType, this.m_objectName);
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

		void IStaticReferenceable.SetID(int id)
		{
			this.m_id = id;
		}

		AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IStaticReferenceable.GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Filters;
		}
	}
}
