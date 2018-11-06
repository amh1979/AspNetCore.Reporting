using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal abstract class ScopeInstance : IStorable, IPersistable
	{
		protected long m_firstRowOffset = DataFieldRow.UnInitializedStreamOffset;

		protected List<IReference<DataRegionInstance>> m_dataRegionInstances;

		protected List<IReference<SubReportInstance>> m_subReportInstances;

		protected List<DataAggregateObjResult> m_aggregateValues;

		[NonSerialized]
		private int m_serializationDataRegionIndexInCollection = -1;

		[NonSerialized]
		protected IDisposable m_cleanupRef;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ScopeInstance.GetDeclaration();

		internal abstract AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType ObjectType
		{
			get;
		}

		internal virtual IRIFReportScope RIFReportScope
		{
			get
			{
				return null;
			}
		}

		internal long FirstRowOffset
		{
			get
			{
				return this.m_firstRowOffset;
			}
			set
			{
				this.m_firstRowOffset = value;
			}
		}

		internal List<IReference<DataRegionInstance>> DataRegionInstances
		{
			get
			{
				return this.m_dataRegionInstances;
			}
		}

		internal List<IReference<SubReportInstance>> SubreportInstances
		{
			get
			{
				return this.m_subReportInstances;
			}
		}

		internal List<DataAggregateObjResult> AggregateValues
		{
			get
			{
				return this.m_aggregateValues;
			}
		}

		internal bool IsReadOnly
		{
			get
			{
				return this.m_cleanupRef == null;
			}
		}

		public virtual int Size
		{
			get
			{
				return 8 + ItemSizes.SizeOf(this.m_dataRegionInstances) + ItemSizes.SizeOf(this.m_subReportInstances) + ItemSizes.SizeOf(this.m_aggregateValues) + 4;
			}
		}

		protected ScopeInstance()
		{
		}

		protected ScopeInstance(long firstRowOffset)
		{
			this.m_firstRowOffset = firstRowOffset;
		}

		internal virtual void InstanceComplete()
		{
			this.m_cleanupRef.Dispose();
			this.m_cleanupRef = null;
		}

		protected void UnPinList<T>(List<ScalableList<T>> listOfLists)
		{
			if (listOfLists != null)
			{
				int count = listOfLists.Count;
				for (int i = 0; i < count; i++)
				{
					ScalableList<T> scalableList = listOfLists[i];
					if (scalableList != null)
					{
						scalableList.UnPinAll();
					}
				}
			}
		}

		protected void SetReadOnlyList<T>(List<ScalableList<T>> listOfLists)
		{
			if (listOfLists != null)
			{
				int count = listOfLists.Count;
				for (int i = 0; i < count; i++)
				{
					ScalableList<T> scalableList = listOfLists[i];
					if (scalableList != null)
					{
						scalableList.SetReadOnly();
					}
				}
			}
		}

		protected static void AdjustLength<T>(ScalableList<T> instances, int indexInCollection) where T : class
		{
			int count = instances.Count;
			for (int i = count; i <= indexInCollection; i++)
			{
				instances.Add((T)null);
			}
		}

		protected static IDisposable AddAndPinItemAt<T>(ScalableList<T> list, int index, T item) where T : class
		{
			int count = list.Count;
			for (int i = count; i < index; i++)
			{
				list.Add((T)null);
			}
			return list.AddAndPin(item);
		}

		internal virtual void AddChildScope(IReference<ScopeInstance> childRef, int indexInCollection)
		{
			switch (childRef.Value().ObjectType)
			{
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegionInstance:
				if (this.m_dataRegionInstances == null)
				{
					this.m_dataRegionInstances = new List<IReference<DataRegionInstance>>();
				}
				ListUtils.AdjustLength(this.m_dataRegionInstances, indexInCollection);
				Global.Tracer.Assert(null == this.m_dataRegionInstances[indexInCollection], "(null == m_dataRegionInstances[indexInCollection])");
				this.m_dataRegionInstances[indexInCollection] = (childRef as IReference<DataRegionInstance>);
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReportInstance:
				if (this.m_subReportInstances == null)
				{
					this.m_subReportInstances = new List<IReference<SubReportInstance>>();
				}
				ListUtils.AdjustLength(this.m_subReportInstances, indexInCollection);
				Global.Tracer.Assert(null == this.m_subReportInstances[indexInCollection], "(null == m_subReportInstances[indexInCollection])");
				this.m_subReportInstances[indexInCollection] = (childRef as IReference<SubReportInstance>);
				break;
			default:
				Global.Tracer.Assert(false, childRef.Value().ToString());
				break;
			}
		}

		internal void StoreAggregates<AggregateType>(OnDemandProcessingContext odpContext, List<AggregateType> aggregateDefs) where AggregateType : DataAggregateInfo
		{
			if (aggregateDefs != null)
			{
				int count = aggregateDefs.Count;
				if (this.m_aggregateValues == null)
				{
					this.m_aggregateValues = new List<DataAggregateObjResult>();
				}
				for (int i = 0; i < count; i++)
				{
					ScopeInstance.StoreAggregate<AggregateType>(odpContext, aggregateDefs[i], ref this.m_aggregateValues);
				}
			}
		}

		internal void StoreAggregates<AggregateType>(OnDemandProcessingContext odpContext, BucketedAggregatesCollection<AggregateType> aggregateDefs) where AggregateType : DataAggregateInfo
		{
			if (aggregateDefs != null && !aggregateDefs.IsEmpty)
			{
				if (this.m_aggregateValues == null)
				{
					this.m_aggregateValues = new List<DataAggregateObjResult>();
				}
				foreach (AggregateType aggregateDef in aggregateDefs)
				{
					ScopeInstance.StoreAggregate<AggregateType>(odpContext, aggregateDef, ref this.m_aggregateValues);
				}
			}
		}

		internal void StoreAggregates(DataAggregateObjResult[] aggregateObjResults)
		{
			if (aggregateObjResults != null)
			{
				int num = aggregateObjResults.Length;
				if (this.m_aggregateValues == null)
				{
					this.m_aggregateValues = new List<DataAggregateObjResult>();
				}
				for (int i = 0; i < num; i++)
				{
					this.m_aggregateValues.Add(aggregateObjResults[i]);
				}
			}
		}

		internal void StoreAggregates<AggregateType>(OnDemandProcessingContext odpContext, List<AggregateType> aggregateDefs, List<int> aggregateIndexes) where AggregateType : DataAggregateInfo
		{
			if (aggregateIndexes != null)
			{
				int count = aggregateIndexes.Count;
				if (this.m_aggregateValues == null)
				{
					this.m_aggregateValues = new List<DataAggregateObjResult>();
				}
				for (int i = 0; i < count; i++)
				{
					int index = aggregateIndexes[i];
					ScopeInstance.StoreAggregate<AggregateType>(odpContext, aggregateDefs[index], ref this.m_aggregateValues);
				}
			}
		}

		private static void StoreAggregate<AggregateType>(OnDemandProcessingContext odpContext, AggregateType aggregateDef, ref List<DataAggregateObjResult> aggregateValues) where AggregateType : DataAggregateInfo
		{
			DataAggregateObj aggregateObj = odpContext.ReportObjectModel.AggregatesImpl.GetAggregateObj(aggregateDef.Name);
			DataAggregateObjResult item = aggregateObj.AggregateResult();
			aggregateValues.Add(item);
		}

		protected static IList<DataRegionMemberInstance> GetChildMemberInstances(List<ScalableList<DataRegionMemberInstance>> members, int memberIndexInCollection)
		{
			if (members != null && members.Count > memberIndexInCollection)
			{
				return members[memberIndexInCollection];
			}
			return null;
		}

		internal void SetupFields(OnDemandProcessingContext odpContext, int dataSetIndex)
		{
			DataSetInstance dataSetInstance = odpContext.CurrentReportInstance.GetDataSetInstance(dataSetIndex, odpContext);
			this.SetupFields(odpContext, dataSetInstance);
		}

		internal void SetupFields(OnDemandProcessingContext odpContext, DataSetInstance dataSetInstance)
		{
			if (!dataSetInstance.NoRows)
			{
				if (0 < this.m_firstRowOffset)
				{
					odpContext.ReportObjectModel.RegisterOnDemandFieldValueUpdate(this.m_firstRowOffset, dataSetInstance, odpContext.GetDataChunkReader(dataSetInstance.DataSetDef.IndexInCollection));
				}
				else
				{
					odpContext.ReportObjectModel.ResetFieldValues();
				}
			}
		}

		internal void SetupAggregates<AggregateType>(OnDemandProcessingContext odpContext, List<AggregateType> aggregateDefs, ref int aggregateValueOffset) where AggregateType : DataAggregateInfo
		{
			if (this.m_aggregateValues != null && aggregateDefs != null)
			{
				int count = aggregateDefs.Count;
				for (int i = 0; i < count; i++)
				{
					ScopeInstance.SetupAggregate(odpContext, aggregateDefs[i], this.m_aggregateValues[aggregateValueOffset]);
					aggregateValueOffset++;
				}
			}
		}

		internal void SetupAggregates<AggregateType>(OnDemandProcessingContext odpContext, BucketedAggregatesCollection<AggregateType> aggregateDefs, ref int aggregateValueOffset) where AggregateType : DataAggregateInfo
		{
			if (this.m_aggregateValues != null && aggregateDefs != null)
			{
				foreach (AggregateType aggregateDef in aggregateDefs)
				{
					ScopeInstance.SetupAggregate(odpContext, aggregateDef, this.m_aggregateValues[aggregateValueOffset]);
					aggregateValueOffset++;
				}
			}
		}

		internal void SetupAggregates<AggregateType>(OnDemandProcessingContext odpContext, List<AggregateType> aggregateDefs, List<int> aggregateIndexes, ref int aggregateValueOffset) where AggregateType : DataAggregateInfo
		{
			int num = (aggregateIndexes != null) ? aggregateIndexes.Count : 0;
			for (int i = 0; i < num; i++)
			{
				int index = aggregateIndexes[i];
				ScopeInstance.SetupAggregate(odpContext, aggregateDefs[index], this.m_aggregateValues[aggregateValueOffset]);
				aggregateValueOffset++;
			}
		}

		private static void SetupAggregate<AggregateType>(OnDemandProcessingContext odpContext, AggregateType aggregateDef, DataAggregateObjResult aggregateObj) where AggregateType : DataAggregateInfo
		{
			odpContext.ReportObjectModel.AggregatesImpl.Set(aggregateDef.Name, (DataAggregateInfo)(object)aggregateDef, aggregateDef.DuplicateNames, aggregateObj);
		}

		internal static void CalculateVariables(OnDemandProcessingContext odpContext, List<Variable> variableDefs, out object[] variableValues)
		{
			variableValues = null;
			if (variableDefs != null && variableDefs.Count != 0)
			{
				int count = variableDefs.Count;
				variableValues = new object[count];
				for (int i = 0; i < count; i++)
				{
					Variable variable = variableDefs[i];
					VariableImpl cachedVariableObj = variable.GetCachedVariableObj(odpContext);
					variableValues[i] = cachedVariableObj.GetResult();
				}
			}
		}

		internal static void ResetVariables(OnDemandProcessingContext odpContext, List<Variable> variableDefs)
		{
			if (variableDefs != null)
			{
				for (int i = 0; i < variableDefs.Count; i++)
				{
					Variable variable = variableDefs[i];
					VariableImpl cachedVariableObj = variable.GetCachedVariableObj(odpContext);
					cachedVariableObj.Reset();
				}
			}
		}

		internal static void SetupVariables(OnDemandProcessingContext odpContext, List<Variable> variableDefs, object[] variableValues)
		{
			if (variableDefs != null)
			{
				for (int i = 0; i < variableValues.Length; i++)
				{
					Variable variable = variableDefs[i];
					VariableImpl cachedVariableObj = variable.GetCachedVariableObj(odpContext);
					cachedVariableObj.SetValue(variableValues[i], true);
				}
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.FirstRowIndex, Token.Int64));
			list.Add(new MemberInfo(MemberName.DataRegionInstances, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegionInstanceReference));
			list.Add(new MemberInfo(MemberName.SubReportInstances, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReportInstanceReference));
			list.Add(new MemberInfo(MemberName.AggregateValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObjResult));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScopeInstance, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ScopeInstance.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.FirstRowIndex:
					writer.Write(this.m_firstRowOffset);
					break;
				case MemberName.DataRegionInstances:
					if (this.m_serializationDataRegionIndexInCollection < 0 || this.m_dataRegionInstances == null)
					{
						writer.Write(this.m_dataRegionInstances);
					}
					else
					{
						List<IReference<DataRegionInstance>> list = new List<IReference<DataRegionInstance>>(this.m_dataRegionInstances.Count);
						ListUtils.AdjustLength(list, this.m_dataRegionInstances.Count - 1);
						list[this.m_serializationDataRegionIndexInCollection] = this.m_dataRegionInstances[this.m_serializationDataRegionIndexInCollection];
						writer.Write(list);
					}
					break;
				case MemberName.SubReportInstances:
					writer.Write(this.m_subReportInstances);
					break;
				case MemberName.AggregateValues:
					writer.Write(this.m_aggregateValues);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ScopeInstance.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.FirstRowIndex:
					this.m_firstRowOffset = reader.ReadInt64();
					break;
				case MemberName.DataRegionInstances:
					this.m_dataRegionInstances = reader.ReadGenericListOfRIFObjects<IReference<DataRegionInstance>>();
					break;
				case MemberName.SubReportInstances:
					this.m_subReportInstances = reader.ReadGenericListOfRIFObjects<IReference<SubReportInstance>>();
					break;
				case MemberName.AggregateValues:
					this.m_aggregateValues = reader.ReadGenericListOfRIFObjects<DataAggregateObjResult>();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public virtual AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScopeInstance;
		}
	}
}
