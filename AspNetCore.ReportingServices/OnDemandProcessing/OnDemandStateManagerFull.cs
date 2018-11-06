using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class OnDemandStateManagerFull : OnDemandStateManager
	{
		private IReportScopeInstance m_lastROMInstance;

		private IInstancePath m_lastRIFObject;

		private IRIFReportScope m_lastTablixProcessingReportScope;

		private List<InstancePathItem> m_lastInstancePath;

		private readonly List<PairObj<string, object>> m_specialLastGroupingValues = new List<PairObj<string, object>>();

		private bool m_lastInScopeResult;

		private int m_lastRecursiveLevel;

		private bool m_inRecursiveRowHierarchy;

		private bool m_inRecursiveColumnHierarchy;

		internal override IReportScopeInstance LastROMInstance
		{
			get
			{
				return this.m_lastROMInstance;
			}
		}

		internal override IInstancePath LastRIFObject
		{
			get
			{
				return this.m_lastRIFObject;
			}
			set
			{
				this.m_lastRIFObject = value;
			}
		}

		internal override IRIFReportScope LastTablixProcessingReportScope
		{
			get
			{
				return this.m_lastTablixProcessingReportScope;
			}
			set
			{
				this.m_lastTablixProcessingReportScope = value;
			}
		}

		internal override QueryRestartInfo QueryRestartInfo
		{
			get
			{
				return null;
			}
		}

		internal override ExecutedQueryCache ExecutedQueryCache
		{
			get
			{
				return null;
			}
		}

		public OnDemandStateManagerFull(OnDemandProcessingContext odpContext)
			: base(odpContext)
		{
		}

		internal override ExecutedQueryCache SetupExecutedQueryCache()
		{
			return this.ExecutedQueryCache;
		}

		internal override Dictionary<string, object> GetCurrentSpecialGroupingValues()
		{
			int num = (this.m_lastInstancePath != null) ? this.m_lastInstancePath.Count : 0;
			Dictionary<string, object> dictionary = new Dictionary<string, object>(num, StringComparer.Ordinal);
			for (int i = 0; i < num; i++)
			{
				PairObj<string, object> pairObj = this.m_specialLastGroupingValues[i];
				if (pairObj != null && !dictionary.ContainsKey(pairObj.First))
				{
					dictionary.Add(pairObj.First, pairObj.Second);
				}
			}
			return dictionary;
		}

		internal override bool CalculateAggregate(string aggregateName)
		{
			OnDemandProcessingContext odpWorkerContextForTablixProcessing = base.GetOdpWorkerContextForTablixProcessing();
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo dataAggregateInfo = default(AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo);
			odpWorkerContextForTablixProcessing.ReportAggregates.TryGetValue(aggregateName, out dataAggregateInfo);
			if (dataAggregateInfo == null)
			{
				return false;
			}
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet = base.m_odpContext.ReportDefinition.MappingDataSetIndexToDataSet[dataAggregateInfo.DataSetIndexInCollection];
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataSetInstance dataSetInstance = odpWorkerContextForTablixProcessing.GetDataSetInstance(dataSet);
			if (dataSetInstance != null)
			{
				bool flag = odpWorkerContextForTablixProcessing.IsTablixProcessingComplete(dataSet.IndexInCollection);
				if (!flag)
				{
					if (odpWorkerContextForTablixProcessing.IsTablixProcessingMode)
					{
						return false;
					}
					OnDemandStateManagerFull onDemandStateManagerFull = (OnDemandStateManagerFull)odpWorkerContextForTablixProcessing.StateManager;
					onDemandStateManagerFull.PerformOnDemandTablixProcessingWithContextRestore(dataSet);
				}
				if (flag || base.m_odpContext.IsPageHeaderFooter)
				{
					dataSetInstance.SetupDataSetLevelAggregates(base.m_odpContext);
				}
				return true;
			}
			return false;
		}

		internal override bool CalculateLookup(LookupInfo lookup)
		{
			OnDemandProcessingContext odpWorkerContextForTablixProcessing = base.GetOdpWorkerContextForTablixProcessing();
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet = base.m_odpContext.ReportDefinition.MappingDataSetIndexToDataSet[lookup.DataSetIndexInCollection];
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataSetInstance dataSetInstance = odpWorkerContextForTablixProcessing.GetDataSetInstance(dataSet);
			if (dataSetInstance != null)
			{
				if (!odpWorkerContextForTablixProcessing.IsTablixProcessingComplete(dataSet.IndexInCollection))
				{
					if (odpWorkerContextForTablixProcessing.IsTablixProcessingMode)
					{
						return false;
					}
					OnDemandStateManagerFull onDemandStateManagerFull = (OnDemandStateManagerFull)odpWorkerContextForTablixProcessing.StateManager;
					onDemandStateManagerFull.PerformOnDemandTablixProcessingWithContextRestore(dataSet);
				}
				return true;
			}
			return false;
		}

		internal override bool PrepareFieldsCollectionForDirectFields()
		{
			if (base.m_odpContext.IsPageHeaderFooter && base.m_odpContext.ReportDefinition.DataSetsNotOnlyUsedInParameters == 1)
			{
				OnDemandProcessingContext parentContext = base.m_odpContext.ParentContext;
				AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet firstDataSet = base.m_odpContext.ReportDefinition.FirstDataSet;
				AspNetCore.ReportingServices.ReportIntermediateFormat.DataSetInstance dataSetInstance = parentContext.GetDataSetInstance(firstDataSet);
				if (dataSetInstance != null)
				{
					if (!parentContext.IsTablixProcessingComplete(firstDataSet.IndexInCollection))
					{
						OnDemandStateManagerFull onDemandStateManagerFull = (OnDemandStateManagerFull)parentContext.StateManager;
						onDemandStateManagerFull.PerformOnDemandTablixProcessing(firstDataSet);
					}
					if (!dataSetInstance.NoRows)
					{
						dataSetInstance.SetupEnvironment(base.m_odpContext, false);
						AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader dataChunkReader = parentContext.GetDataChunkReader(firstDataSet.IndexInCollection);
						dataChunkReader.ResetCachedStreamOffset();
						return true;
					}
				}
			}
			return false;
		}

		internal override void EvaluateScopedFieldReference(string scopeName, int fieldIndex, ref AspNetCore.ReportingServices.RdlExpressions.VariantResult result)
		{
			Global.Tracer.Assert(false, "Scoped field references are not supported in Full ODP mode.");
			throw new NotImplementedException();
		}

		internal override int RecursiveLevel(string scopeName)
		{
			if (base.m_odpContext.IsTablixProcessingMode)
			{
				return base.m_odpContext.ReportRuntime.RecursiveLevel(scopeName);
			}
			if (scopeName == null)
			{
				if (this.m_inRecursiveRowHierarchy && this.m_inRecursiveColumnHierarchy)
				{
					return 0;
				}
				return this.m_lastRecursiveLevel;
			}
			this.m_lastRecursiveLevel = 0;
			this.SetupObjectModels(OnDemandMode.InScope, false, -1, scopeName);
			if (this.m_lastInScopeResult)
			{
				return this.m_lastRecursiveLevel;
			}
			return 0;
		}

		internal override bool InScope(string scopeName)
		{
			this.m_lastInScopeResult = false;
			if (base.m_odpContext.IsTablixProcessingMode)
			{
				if (base.m_odpContext.ReportRuntime.CurrentScope == null)
				{
					return false;
				}
				return base.m_odpContext.ReportRuntime.CurrentScope.InScope(scopeName);
			}
			if (this.m_lastInstancePath != null && scopeName != null)
			{
				this.SetupObjectModels(OnDemandMode.InScope, false, -1, scopeName);
			}
			return this.m_lastInScopeResult;
		}

		internal override void ResetOnDemandState()
		{
			this.m_lastInstancePath = null;
			this.m_lastRecursiveLevel = 0;
			this.m_lastInScopeResult = false;
			this.m_lastTablixProcessingReportScope = null;
		}

		private bool InScopeCompare(string scope1, string scope2)
		{
			this.m_lastInScopeResult = (0 == string.CompareOrdinal(scope1, scope2));
			return this.m_lastInScopeResult;
		}

		internal override void RestoreContext(IInstancePath originalObject)
		{
			if (originalObject != null && base.m_odpContext.ReportRuntime.ContextUpdated && !InstancePathItem.IsSameScopePath(originalObject, this.m_lastRIFObject))
			{
				this.SetupContext(originalObject, null, -1);
			}
		}

		internal override void SetupContext(IInstancePath rifObject, IReportScopeInstance romInstance)
		{
			this.SetupContext(rifObject, romInstance, -1);
		}

		internal override void SetupContext(IInstancePath rifObject, IReportScopeInstance romInstance, int moveNextInstanceIndex)
		{
			bool flag = false;
			bool needDeepCopyPath = false;
			if (romInstance == null)
			{
				flag = true;
				this.m_lastRIFObject = rifObject;
				needDeepCopyPath = true;
			}
			else if (romInstance.IsNewContext || this.m_lastROMInstance == null || this.m_lastRIFObject == null || 0 <= moveNextInstanceIndex)
			{
				flag = true;
				romInstance.IsNewContext = false;
				this.m_lastROMInstance = romInstance;
				this.m_lastRIFObject = rifObject;
				needDeepCopyPath = true;
			}
			else if (this.m_lastROMInstance.Equals(romInstance))
			{
				if (!this.m_lastRIFObject.Equals(rifObject) && (this.m_lastRIFObject.InstancePathItem.Type == InstancePathItemType.SubReport || rifObject.InstancePathItem.Type == InstancePathItemType.SubReport))
				{
					flag = true;
				}
				this.m_lastRIFObject = rifObject;
			}
			else if (this.m_lastRIFObject.Equals(rifObject))
			{
				this.m_lastROMInstance = romInstance;
			}
			else if (InstancePathItem.IsSamePath(this.m_lastInstancePath, rifObject.InstancePath))
			{
				this.m_lastROMInstance = romInstance;
				this.m_lastRIFObject = rifObject;
			}
			else
			{
				flag = true;
				this.m_lastROMInstance = romInstance;
				this.m_lastRIFObject = rifObject;
				needDeepCopyPath = true;
			}
			if (flag)
			{
				this.SetupObjectModels(OnDemandMode.FullSetup, needDeepCopyPath, moveNextInstanceIndex, null);
				base.m_odpContext.ReportRuntime.ContextUpdated = true;
			}
		}

		private static void UpdateThreadCultureWithAssert(CultureInfo newCulture)
		{
			Thread.CurrentThread.CurrentCulture = newCulture;
		}

		private void SetupObjectModels(OnDemandMode mode, bool needDeepCopyPath, int moveNextInstanceIndex, string scopeName)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegionInstance dataRegionInstance = null;
			IMemberHierarchy memberHierarchy = null;
			int num = -1;
			ScopeInstance scopeInstance = base.m_odpContext.CurrentReportInstance;
			List<InstancePathItem> lastInstancePath = this.m_lastInstancePath;
			List<InstancePathItem> list = null;
			int num2 = 0;
			AspNetCore.ReportingServices.ReportIntermediateFormat.Report reportDefinition = base.m_odpContext.ReportDefinition;
			ObjectModelImpl reportObjectModel = base.m_odpContext.ReportObjectModel;
			bool flag = false;
			bool flag2 = false;
			int num3 = 0;
			try
			{
				if (this.m_lastRIFObject.InstancePath != null)
				{
					list = this.m_lastRIFObject.InstancePath;
					num2 = list.Count;
				}
				if (mode != OnDemandMode.InScope)
				{
					base.m_odpContext.EnsureCultureIsSetOnCurrentThread();
				}
				if (mode != OnDemandMode.InScope || 1 != reportDefinition.DataSetsNotOnlyUsedInParameters || !this.InScopeCompare(reportDefinition.FirstDataSet.Name, scopeName))
				{
					int num4 = 0;
					if (base.m_odpContext.InSubreport)
					{
						num4 = InstancePathItem.GetParentReportIndex(this.m_lastRIFObject.InstancePath, this.m_lastRIFObject.InstancePathItem.Type == InstancePathItemType.SubReport);
					}
					bool flag3 = default(bool);
					int sharedPathIndex = InstancePathItem.GetSharedPathIndex(num4, lastInstancePath, list, reportObjectModel.AllFieldsCleared, out flag3);
					for (int i = this.m_specialLastGroupingValues.Count; i < num4; i++)
					{
						this.m_specialLastGroupingValues.Add(null);
					}
					for (int j = num4; j < num2; j++)
					{
						InstancePathItem instancePathItem = list[j];
						bool flag4 = false;
						if (mode != OnDemandMode.InScope)
						{
							flag4 = (j <= sharedPathIndex);
						}
						if (!flag4 && mode == OnDemandMode.FullSetup)
						{
							if (this.m_specialLastGroupingValues.Count < num2)
							{
								this.m_specialLastGroupingValues.Add(null);
							}
							else
							{
								this.m_specialLastGroupingValues[j] = null;
							}
						}
						switch (instancePathItem.Type)
						{
						case InstancePathItemType.SubReport:
							if (scopeInstance.SubreportInstances != null && instancePathItem.IndexInCollection < scopeInstance.SubreportInstances.Count)
							{
								IReference<AspNetCore.ReportingServices.ReportIntermediateFormat.SubReportInstance> reference = scopeInstance.SubreportInstances[instancePathItem.IndexInCollection];
								using (reference.PinValue())
								{
									AspNetCore.ReportingServices.ReportIntermediateFormat.SubReportInstance subReportInstance = reference.Value();
									subReportInstance.SubReportDef.CurrentSubReportInstance = reference;
									if (mode != OnDemandMode.InScope && !subReportInstance.Initialized)
									{
										if (base.m_odpContext.IsTablixProcessingMode || base.m_odpContext.IsTopLevelSubReportProcessing)
										{
											return;
										}
										SubReportInitializer.InitializeSubReport(subReportInstance.SubReportDef);
										reference.PinValue();
									}
									Global.Tracer.Assert(j == num2 - 1, "SubReport not last in instance path.");
								}
							}
							goto default;
						case InstancePathItemType.DataRegion:
							if (scopeInstance is AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance && (scopeInstance.DataRegionInstances == null || scopeInstance.DataRegionInstances.Count <= instancePathItem.IndexInCollection || scopeInstance.DataRegionInstances[instancePathItem.IndexInCollection] == null || scopeInstance.DataRegionInstances[instancePathItem.IndexInCollection].Value() == null))
							{
								Global.Tracer.Assert(instancePathItem.IndexInCollection < reportDefinition.TopLevelDataRegions.Count, "(newItem.IndexInCollection < m_reportDefinition.TopLevelDataRegions.Count)");
								AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion = reportDefinition.TopLevelDataRegions[instancePathItem.IndexInCollection];
								AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet = dataRegion.GetDataSet(reportDefinition);
								if (mode == OnDemandMode.InScope && this.InScopeCompare(dataSet.Name, scopeName))
								{
									return;
								}
								this.PerformOnDemandTablixProcessing(dataSet);
							}
							scopeInstance = scopeInstance.DataRegionInstances[instancePathItem.IndexInCollection].Value();
							flag = (this.m_inRecursiveColumnHierarchy = false);
							flag2 = (this.m_inRecursiveRowHierarchy = false);
							num = -1;
							dataRegionInstance = (scopeInstance as AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegionInstance);
							memberHierarchy = dataRegionInstance;
							if (mode == OnDemandMode.InScope && this.InScopeCompare(dataRegionInstance.DataRegionDef.Name, scopeName))
							{
								return;
							}
							if (dataRegionInstance.DataSetIndexInCollection >= 0 && base.m_odpContext.CurrentDataSetIndex != dataRegionInstance.DataSetIndexInCollection && mode != OnDemandMode.InScope)
							{
								if (!flag4)
								{
									AspNetCore.ReportingServices.ReportIntermediateFormat.DataSetInstance dataSetInstance = base.m_odpContext.CurrentReportInstance.GetDataSetInstance(dataRegionInstance.DataSetIndexInCollection, base.m_odpContext);
									if (dataSetInstance != null)
									{
										dataSetInstance.SetupEnvironment(base.m_odpContext, true);
										num3 = 0;
									}
								}
								else
								{
									num3 = j + 1;
								}
							}
							if (mode != OnDemandMode.InScope)
							{
								if (!flag4)
								{
									dataRegionInstance.SetupEnvironment(base.m_odpContext);
									num3 = 0;
									if (!dataRegionInstance.NoRows)
									{
										dataRegionInstance.DataRegionDef.NoRows = false;
										goto default;
									}
									dataRegionInstance.DataRegionDef.NoRows = true;
									dataRegionInstance.DataRegionDef.ResetTopLevelDynamicMemberInstanceCount();
									return;
								}
								num3 = j + 1;
							}
							goto default;
						case InstancePathItemType.ColumnMemberInstanceIndexTopMost:
							scopeInstance = dataRegionInstance;
							goto default;
						case InstancePathItemType.Cell:
						{
							if (-1 == num)
							{
								num = 0;
							}
							IList<AspNetCore.ReportingServices.ReportIntermediateFormat.DataCellInstance> cellInstances = memberHierarchy.GetCellInstances(num);
							if (cellInstances == null)
							{
								if (flag2 && flag)
								{
									reportObjectModel.ResetFieldValues();
								}
							}
							else if (cellInstances.Count > instancePathItem.IndexInCollection)
							{
								AspNetCore.ReportingServices.ReportIntermediateFormat.DataCellInstance dataCellInstance = cellInstances[instancePathItem.IndexInCollection];
								if (dataCellInstance != null)
								{
									scopeInstance = dataCellInstance;
									if (!flag4)
									{
										dataCellInstance.SetupEnvironment(base.m_odpContext, base.m_odpContext.CurrentDataSetIndex);
										num3 = 0;
									}
									else
									{
										num3 = j + 1;
									}
								}
							}
							goto default;
						}
						default:
							if (instancePathItem.IsDynamicMember)
							{
								IList<DataRegionMemberInstance> childMemberInstances = ((IMemberHierarchy)scopeInstance).GetChildMemberInstances(instancePathItem.Type == InstancePathItemType.RowMemberInstanceIndex, instancePathItem.IndexInCollection);
								if (childMemberInstances == null)
								{
									reportObjectModel.ResetFieldValues();
									return;
								}
								int num5 = (j != num2 - 1 || moveNextInstanceIndex < 0 || moveNextInstanceIndex >= childMemberInstances.Count) ? ((instancePathItem.InstanceIndex >= 0) ? instancePathItem.InstanceIndex : 0) : moveNextInstanceIndex;
								if (num5 >= childMemberInstances.Count)
								{
									instancePathItem.ResetContext();
									num5 = 0;
								}
								DataRegionMemberInstance dataRegionMemberInstance = childMemberInstances[num5];
								if (mode == OnDemandMode.FullSetup)
								{
									dataRegionMemberInstance.MemberDef.InstanceCount = childMemberInstances.Count;
									dataRegionMemberInstance.MemberDef.CurrentMemberIndex = num5;
								}
								scopeInstance = dataRegionMemberInstance;
								this.m_lastRecursiveLevel = dataRegionMemberInstance.RecursiveLevel;
								AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode memberDef = dataRegionMemberInstance.MemberDef;
								if (mode == OnDemandMode.InScope && this.InScopeCompare(memberDef.Grouping.Name, scopeName))
								{
									return;
								}
								if (instancePathItem.Type == InstancePathItemType.RowMemberInstanceIndex)
								{
									memberHierarchy = dataRegionMemberInstance;
									flag2 = true;
								}
								else
								{
									num = dataRegionMemberInstance.MemberInstanceIndexWithinScopeLevel;
									flag = true;
								}
								if (mode == OnDemandMode.FullSetup && !flag4)
								{
									dataRegionMemberInstance.SetupEnvironment(base.m_odpContext, base.m_odpContext.CurrentDataSetIndex);
									num3 = 0;
									AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping grouping = memberDef.Grouping;
									if (grouping.Parent != null)
									{
										if (memberDef.IsColumn)
										{
											this.m_inRecursiveColumnHierarchy = true;
										}
										else
										{
											this.m_inRecursiveRowHierarchy = true;
										}
										if (memberDef.IsTablixMember)
										{
											memberDef.SetMemberInstances(childMemberInstances);
											memberDef.SetRecursiveParentIndex(dataRegionMemberInstance.RecursiveParentIndex);
											memberDef.SetInstanceHasRecursiveChildren(dataRegionMemberInstance.HasRecursiveChildren);
										}
									}
									else if (memberDef.IsColumn)
									{
										this.m_inRecursiveColumnHierarchy = false;
									}
									else
									{
										this.m_inRecursiveRowHierarchy = false;
									}
									grouping.RecursiveLevel = this.m_lastRecursiveLevel;
									grouping.SetGroupInstanceExpressionValues(dataRegionMemberInstance.GroupExprValues);
									if (mode == OnDemandMode.FullSetup && grouping != null && grouping.GroupExpressions != null && grouping.GroupExpressions.Count > 0)
									{
										AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = grouping.GroupExpressions[0];
										if (expressionInfo.Type == AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Field)
										{
											AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet2 = memberDef.DataRegionDef.GetDataSet(reportDefinition);
											AspNetCore.ReportingServices.ReportIntermediateFormat.Field field = dataSet2.Fields[expressionInfo.IntValue];
											if (field.DataField != null)
											{
												string dataField = field.DataField;
												object second = dataRegionMemberInstance.GroupExprValues[0];
												PairObj<string, object> pairObj = this.m_specialLastGroupingValues[j];
												if (pairObj == null)
												{
													pairObj = new PairObj<string, object>(dataField, second);
													this.m_specialLastGroupingValues[j] = pairObj;
												}
												else
												{
													pairObj.First = dataField;
													pairObj.Second = second;
												}
											}
										}
									}
								}
								else
								{
									num3 = j + 1;
								}
							}
							break;
						case InstancePathItemType.None:
							break;
						}
					}
					if (mode == OnDemandMode.FullSetup && !flag3 && scopeInstance != null && num3 > 0)
					{
						while (num3 < this.m_lastInstancePath.Count)
						{
							if (!this.m_lastInstancePath[num3].IsScope)
							{
								num3++;
								continue;
							}
							scopeInstance.SetupFields(base.m_odpContext, base.m_odpContext.CurrentDataSetIndex);
							break;
						}
					}
					if (mode == OnDemandMode.FullSetup && !base.m_odpContext.IsTablixProcessingMode && base.m_odpContext.CurrentReportInstance != null && dataRegionInstance == null && reportDefinition.DataSetsNotOnlyUsedInParameters == 1)
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet firstDataSet = reportDefinition.FirstDataSet;
						AspNetCore.ReportingServices.ReportIntermediateFormat.DataSetInstance dataSetInstance2 = base.m_odpContext.CurrentReportInstance.GetDataSetInstance(firstDataSet, base.m_odpContext);
						if (dataSetInstance2 != null)
						{
							bool flag5 = true;
							if (!base.m_odpContext.IsTablixProcessingComplete(firstDataSet.IndexInCollection))
							{
								this.PerformOnDemandTablixProcessing(firstDataSet);
								flag5 = false;
							}
							if (base.m_odpContext.CurrentOdpDataSetInstance == dataSetInstance2)
							{
								flag5 = false;
							}
							if (flag5)
							{
								dataSetInstance2.SetupEnvironment(base.m_odpContext, true);
							}
							else if (!dataSetInstance2.NoRows)
							{
								dataSetInstance2.SetupFields(base.m_odpContext, dataSetInstance2);
							}
						}
					}
				}
			}
			finally
			{
				if (needDeepCopyPath)
				{
					InstancePathItem.DeepCopyPath(list, ref this.m_lastInstancePath);
				}
			}
		}

		private void PerformOnDemandTablixProcessing(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet)
		{
			Merge.TablixDataProcessing(base.m_odpContext, dataSet);
			base.m_odpContext.ReportObjectModel.ResetFieldValues();
		}

		private void PerformOnDemandTablixProcessingWithContextRestore(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet)
		{
			Global.Tracer.Assert(!base.m_odpContext.IsTablixProcessingMode, "Nested calls of tablix data processing are not supported");
			IInstancePath lastRIFObject = this.m_lastRIFObject;
			this.PerformOnDemandTablixProcessing(dataSet);
			this.RestoreContext(lastRIFObject);
		}

		internal override IRecordRowReader CreateSequentialDataReader(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet, out AspNetCore.ReportingServices.ReportIntermediateFormat.DataSetInstance dataSetInstance)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance currentReportInstance = base.m_odpContext.CurrentReportInstance;
			dataSetInstance = currentReportInstance.GetDataSetInstance(dataSet, base.m_odpContext);
			AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader dataChunkReader = null;
			if (!dataSetInstance.NoRows)
			{
				dataChunkReader = new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader(dataSetInstance, base.m_odpContext, dataSetInstance.DataChunkName);
				base.RegisterDisposableDataReaderOrIdcDataManager(dataChunkReader);
			}
			return dataChunkReader;
		}

		internal override void BindNextMemberInstance(IInstancePath rifObject, IReportScopeInstance romInstance, int moveNextInstanceIndex)
		{
			Global.Tracer.Assert(false, "This method is not valid for this StateManager type.");
			throw new InvalidOperationException("This method is not valid for this StateManager type.");
		}

		internal override bool ShouldStopPipelineAdvance(bool rowAccepted)
		{
			return true;
		}

		internal override void CreatedScopeInstance(IRIFReportDataScope scope)
		{
		}

		internal override bool ProcessOneRow(IRIFReportDataScope scope)
		{
			Global.Tracer.Assert(false, "This method is not valid for this StateManager type.");
			throw new InvalidOperationException("This method is not valid for this StateManager type.");
		}

		internal override bool CheckForPrematureServerAggregate(string aggregateName)
		{
			return false;
		}
	}
}
