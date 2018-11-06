using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeRICollection : IStorable, IPersistable
	{
		private List<RuntimeDataTablixObjReference> m_dataRegionObjs;

		private static Declaration m_declaration = RuntimeRICollection.GetDeclaration();

		public int Size
		{
			get
			{
				return ItemSizes.SizeOf(this.m_dataRegionObjs);
			}
		}

		internal RuntimeRICollection()
		{
		}

		internal RuntimeRICollection(int capacity)
		{
			this.m_dataRegionObjs = new List<RuntimeDataTablixObjReference>(capacity);
		}

		internal RuntimeRICollection(IReference<IScope> owner, List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem> reportItems, ref DataActions dataAction, OnDemandProcessingContext odpContext)
		{
			this.m_dataRegionObjs = new List<RuntimeDataTablixObjReference>();
			this.AddItems(owner, reportItems, ref dataAction, odpContext);
		}

		internal RuntimeRICollection(IReference<IScope> outerScope, List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion> dataRegionDefs, OnDemandProcessingContext odpContext, bool onePass)
		{
			this.m_dataRegionObjs = new List<RuntimeDataTablixObjReference>(dataRegionDefs.Count);
			DataActions dataActions = DataActions.None;
			for (int i = 0; i < dataRegionDefs.Count; i++)
			{
				this.CreateDataRegions(outerScope, dataRegionDefs[i], odpContext, onePass, ref dataActions);
			}
		}

		public void AddItems(IReference<IScope> owner, List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem> reportItems, ref DataActions dataAction, OnDemandProcessingContext odpContext)
		{
			if (reportItems != null && reportItems.Count > 0)
			{
				this.CreateDataRegions(owner, reportItems, odpContext, false, ref dataAction);
			}
		}

		private void CreateDataRegions(IReference<IScope> owner, List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem> computedRIs, OnDemandProcessingContext odpContext, bool onePass, ref DataActions dataAction)
		{
			if (computedRIs != null)
			{
				for (int i = 0; i < computedRIs.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem reportItem = computedRIs[i];
					this.CreateDataRegions(owner, reportItem, odpContext, onePass, ref dataAction);
				}
			}
		}

		private void CreateDataRegions(IReference<IScope> owner, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem reportItem, OnDemandProcessingContext odpContext, bool onePass, ref DataActions dataAction)
		{
			RuntimeDataTablixObj runtimeDataTablixObj = null;
			switch (reportItem.ObjectType)
			{
			case AspNetCore.ReportingServices.ReportProcessing.ObjectType.Rectangle:
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItemCollection reportItems = ((AspNetCore.ReportingServices.ReportIntermediateFormat.Rectangle)reportItem).ReportItems;
				if (reportItems != null && reportItems.ComputedReportItems != null)
				{
					this.CreateDataRegions(owner, reportItems.ComputedReportItems, odpContext, onePass, ref dataAction);
				}
				break;
			}
			case AspNetCore.ReportingServices.ReportProcessing.ObjectType.Tablix:
				runtimeDataTablixObj = new RuntimeTablixObj(owner, (AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix)reportItem, ref dataAction, odpContext, onePass);
				break;
			case AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart:
				runtimeDataTablixObj = new RuntimeChartObj(owner, (AspNetCore.ReportingServices.ReportIntermediateFormat.Chart)reportItem, ref dataAction, odpContext, onePass);
				break;
			case AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel:
				runtimeDataTablixObj = new RuntimeGaugePanelObj(owner, (GaugePanel)reportItem, ref dataAction, odpContext, onePass);
				break;
			case AspNetCore.ReportingServices.ReportProcessing.ObjectType.MapDataRegion:
				runtimeDataTablixObj = new RuntimeMapDataRegionObj(owner, (MapDataRegion)reportItem, ref dataAction, odpContext, onePass);
				break;
			case AspNetCore.ReportingServices.ReportProcessing.ObjectType.CustomReportItem:
				if (reportItem.IsDataRegion)
				{
					runtimeDataTablixObj = new RuntimeCriObj(owner, (AspNetCore.ReportingServices.ReportIntermediateFormat.CustomReportItem)reportItem, ref dataAction, odpContext, onePass);
				}
				break;
			case AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map:
			{
				List<MapDataRegion> mapDataRegions = ((Map)reportItem).MapDataRegions;
				if (mapDataRegions != null)
				{
					this.CreateMapDataRegions(owner, mapDataRegions, odpContext, onePass, ref dataAction);
				}
				break;
			}
			}
			if (runtimeDataTablixObj != null)
			{
				this.AddDataRegion(runtimeDataTablixObj, (AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion)reportItem);
			}
		}

		private void CreateMapDataRegions(IReference<IScope> owner, List<MapDataRegion> mapDataRegions, OnDemandProcessingContext odpContext, bool onePass, ref DataActions dataAction)
		{
			RuntimeDataTablixObj runtimeDataTablixObj = null;
			foreach (MapDataRegion mapDataRegion in mapDataRegions)
			{
				runtimeDataTablixObj = new RuntimeMapDataRegionObj(owner, mapDataRegion, ref dataAction, odpContext, onePass);
				this.AddDataRegion(runtimeDataTablixObj, mapDataRegion);
			}
		}

		private void AddDataRegion(RuntimeDataTablixObj dataRegion, AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef)
		{
			RuntimeDataTablixObjReference runtimeDataTablixObjReference = (RuntimeDataTablixObjReference)dataRegion.SelfReference;
			runtimeDataTablixObjReference.UnPinValue();
			int indexInCollection = dataRegionDef.IndexInCollection;
			ListUtils.AdjustLength(this.m_dataRegionObjs, indexInCollection);
			this.m_dataRegionObjs[indexInCollection] = runtimeDataTablixObjReference;
		}

		internal void FirstPassNextDataRow(OnDemandProcessingContext odpContext)
		{
			AggregateRowInfo aggregateRowInfo = AggregateRowInfo.CreateAndSaveAggregateInfo(odpContext);
			for (int i = 0; i < this.m_dataRegionObjs.Count; i++)
			{
				RuntimeDataRegionObjReference runtimeDataRegionObjReference = this.m_dataRegionObjs[i];
				if ((BaseReference)runtimeDataRegionObjReference != (object)null)
				{
					using (runtimeDataRegionObjReference.PinValue())
					{
						runtimeDataRegionObjReference.Value().NextRow();
					}
					aggregateRowInfo.RestoreAggregateInfo(odpContext);
				}
			}
		}

		internal void SortAndFilter(AggregateUpdateContext aggContext)
		{
			this.Traverse(ProcessingStages.SortAndFilter, aggContext);
		}

		private void Traverse(ProcessingStages operation, AggregateUpdateContext context)
		{
			for (int i = 0; i < this.m_dataRegionObjs.Count; i++)
			{
				RuntimeDataRegionObjReference runtimeDataRegionObjReference = this.m_dataRegionObjs[i];
				if ((BaseReference)runtimeDataRegionObjReference != (object)null)
				{
					using (runtimeDataRegionObjReference.PinValue())
					{
						switch (operation)
						{
						case ProcessingStages.SortAndFilter:
							runtimeDataRegionObjReference.Value().SortAndFilter(context);
							break;
						case ProcessingStages.UpdateAggregates:
							runtimeDataRegionObjReference.Value().UpdateAggregates(context);
							break;
						default:
							Global.Tracer.Assert(false, "Unknown ProcessingStage in Traverse");
							break;
						}
					}
				}
			}
		}

		internal void UpdateAggregates(AggregateUpdateContext aggContext)
		{
			this.Traverse(ProcessingStages.UpdateAggregates, aggContext);
		}

		internal void CalculateRunningValues(Dictionary<string, IReference<RuntimeGroupRootObj>> groupCollection, IReference<RuntimeGroupRootObj> lastGroup, AggregateUpdateContext aggContext)
		{
			for (int i = 0; i < this.m_dataRegionObjs.Count; i++)
			{
				RuntimeDataRegionObjReference runtimeDataRegionObjReference = this.m_dataRegionObjs[i];
				if ((BaseReference)runtimeDataRegionObjReference != (object)null)
				{
					using (runtimeDataRegionObjReference.PinValue())
					{
						runtimeDataRegionObjReference.Value().CalculateRunningValues(groupCollection, lastGroup, aggContext);
					}
				}
			}
		}

		internal static void StoreRunningValues(AggregatesImpl globalRVCol, List<AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo> runningValues, ref AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[] runningValueValues)
		{
			if (runningValues != null && 0 < runningValues.Count)
			{
				if (runningValueValues == null)
				{
					runningValueValues = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[runningValues.Count];
				}
				for (int i = 0; i < runningValues.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo runningValueInfo = runningValues[i];
					AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj = globalRVCol.GetAggregateObj(runningValueInfo.Name);
					if (aggregateObj != null)
					{
						runningValueValues[i] = aggregateObj.AggregateResult();
					}
				}
			}
			else
			{
				runningValueValues = null;
			}
		}

		internal void CreateAllDataRegionInstances(ScopeInstance parentInstance, OnDemandProcessingContext odpContext, IReference<IScope> owner)
		{
			for (int i = 0; i < this.m_dataRegionObjs.Count; i++)
			{
				RuntimeRICollection.CreateDataRegionInstance(parentInstance, odpContext, this.m_dataRegionObjs[i]);
			}
			this.m_dataRegionObjs = null;
		}

		internal void CreateInstances(ScopeInstance parentInstance, OnDemandProcessingContext odpContext, IReference<IScope> owner, List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem> reportItems)
		{
			if (reportItems != null)
			{
				for (int i = 0; i < reportItems.Count; i++)
				{
					this.CreateInstance(parentInstance, reportItems[i], odpContext, owner);
				}
			}
		}

		private static void CreateDataRegionInstance(ScopeInstance parentInstance, OnDemandProcessingContext odpContext, RuntimeDataRegionObjReference dataRegionObjRef)
		{
			if (!((BaseReference)dataRegionObjRef == (object)null))
			{
				using (dataRegionObjRef.PinValue())
				{
					RuntimeDataTablixObj runtimeDataTablixObj = (RuntimeDataTablixObj)dataRegionObjRef.Value();
					AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef = runtimeDataTablixObj.DataRegionDef;
					runtimeDataTablixObj.SetupEnvironment();
					IReference<DataRegionInstance> reference = DataRegionInstance.CreateInstance(parentInstance, odpContext.OdpMetadata, dataRegionDef, odpContext.CurrentDataSetIndex);
					DataRegionInstance dataRegionInstance = reference.Value();
					runtimeDataTablixObj.CreateInstances(dataRegionInstance);
					dataRegionInstance.InstanceComplete();
					dataRegionDef.RuntimeDataRegionObj = null;
				}
			}
		}

		public static void MergeDataProcessingItems(List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem> candidateItems, ref List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem> results)
		{
			if (candidateItems != null)
			{
				for (int i = 0; i < candidateItems.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem item = candidateItems[i];
					RuntimeRICollection.MergeDataProcessingItem(item, ref results);
				}
			}
		}

		public static void MergeDataProcessingItem(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem item, ref List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem> results)
		{
			if (item != null)
			{
				if (item.IsDataRegion)
				{
					RuntimeRICollection.AddItem(item, ref results);
				}
				else
				{
					switch (item.ObjectType)
					{
					case AspNetCore.ReportingServices.ReportProcessing.ObjectType.Rectangle:
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.Rectangle rectangle = (AspNetCore.ReportingServices.ReportIntermediateFormat.Rectangle)item;
						RuntimeRICollection.MergeDataProcessingItems(rectangle.ReportItems.ComputedReportItems, ref results);
						break;
					}
					case AspNetCore.ReportingServices.ReportProcessing.ObjectType.Subreport:
					case AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map:
						RuntimeRICollection.AddItem(item, ref results);
						break;
					}
				}
			}
		}

		private static void AddItem(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem item, ref List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem> results)
		{
			if (results == null)
			{
				results = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem>();
			}
			results.Add(item);
		}

		private void CreateInstance(ScopeInstance parentInstance, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem reportItem, OnDemandProcessingContext odpContext, IReference<IScope> owner)
		{
			if (reportItem != null)
			{
				if (reportItem.IsDataRegion)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion = (AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion)reportItem;
					RuntimeDataRegionObjReference dataRegionObjRef = this.m_dataRegionObjs[dataRegion.IndexInCollection];
					RuntimeRICollection.CreateDataRegionInstance(parentInstance, odpContext, dataRegionObjRef);
				}
				else
				{
					switch (reportItem.ObjectType)
					{
					case AspNetCore.ReportingServices.ReportProcessing.ObjectType.Subreport:
						this.CreateSubReportInstance((AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport)reportItem, parentInstance, odpContext, owner);
						break;
					case AspNetCore.ReportingServices.ReportProcessing.ObjectType.Rectangle:
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.Rectangle rectangle = (AspNetCore.ReportingServices.ReportIntermediateFormat.Rectangle)reportItem;
						this.CreateInstances(parentInstance, odpContext, owner, rectangle.ReportItems.ComputedReportItems);
						break;
					}
					case AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map:
					{
						Map map = (Map)reportItem;
						List<MapDataRegion> mapDataRegions = map.MapDataRegions;
						for (int i = 0; i < mapDataRegions.Count; i++)
						{
							this.CreateInstance(parentInstance, mapDataRegions[i], odpContext, owner);
						}
						break;
					}
					}
				}
			}
		}

		private void CreateSubReportInstance(AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport subReport, ScopeInstance parentInstance, OnDemandProcessingContext odpContext, IReference<IScope> owner)
		{
			if (!subReport.ExceededMaxLevel)
			{
				IReference<AspNetCore.ReportingServices.ReportIntermediateFormat.SubReportInstance> reference2 = subReport.CurrentSubReportInstance = AspNetCore.ReportingServices.ReportIntermediateFormat.SubReportInstance.CreateInstance(parentInstance, subReport, odpContext.OdpMetadata);
				subReport.OdpContext.UserSortFilterContext.CurrentContainingScope = owner;
				odpContext.LastTablixProcessingReportScope = parentInstance.RIFReportScope;
				if (SubReportInitializer.InitializeSubReport(subReport))
				{
					IReference<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance> reportInstance = reference2.Value().ReportInstance;
					Merge.PreProcessTablixes(subReport.Report, subReport.OdpContext, !odpContext.ReprocessSnapshot);
					if (subReport.Report.HasSubReports)
					{
						SubReportInitializer.InitializeSubReports(subReport.Report, reportInstance.Value(), subReport.OdpContext, false, true);
					}
				}
				if (reference2 != null)
				{
					reference2.Value().InstanceComplete();
				}
				odpContext.EnsureCultureIsSetOnCurrentThread();
			}
		}

		public RuntimeDataTablixObjReference GetDataRegionObj(AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion rifDataRegion)
		{
			int indexInCollection = rifDataRegion.IndexInCollection;
			return this.m_dataRegionObjs[indexInCollection];
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(RuntimeRICollection.m_declaration);
			PersistenceHelper persistenceHelper = writer.PersistenceHelper;
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.DataRegionObjs)
				{
					writer.Write(this.m_dataRegionObjs);
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(RuntimeRICollection.m_declaration);
			PersistenceHelper persistenceHelper = reader.PersistenceHelper;
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.DataRegionObjs)
				{
					this.m_dataRegionObjs = reader.ReadListOfRIFObjects<List<RuntimeDataTablixObjReference>>();
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRICollection;
		}

		public static Declaration GetDeclaration()
		{
			if (RuntimeRICollection.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.DataRegionObjs, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixObjReference));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRICollection, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return RuntimeRICollection.m_declaration;
		}
	}
}
