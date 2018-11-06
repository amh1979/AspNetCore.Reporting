using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal class ReportInstance : ScopeInstance
	{
		private bool m_noRows;

		private string m_language;

		private object[] m_variables;

		[NonSerialized]
		private DataSetInstance[] m_dataSetInstances;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ReportInstance.GetDeclaration();

		internal override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportInstance;
			}
		}

		internal bool NoRows
		{
			get
			{
				return this.m_noRows;
			}
			set
			{
				this.m_noRows = value;
			}
		}

		internal string Language
		{
			get
			{
				return this.m_language;
			}
			set
			{
				if (!base.IsReadOnly)
				{
					this.m_language = value;
				}
			}
		}

		internal object[] VariableValues
		{
			get
			{
				return this.m_variables;
			}
		}

		internal ReportInstance(OnDemandProcessingContext odpContext, Report reportDef, ParameterInfoCollection parameters)
		{
			int count = reportDef.MappingNameToDataSet.Count;
			this.m_dataSetInstances = new DataSetInstance[count];
			List<DataRegion> topLevelDataRegions = reportDef.TopLevelDataRegions;
			if (topLevelDataRegions != null)
			{
				OnDemandMetadata odpMetadata = odpContext.OdpMetadata;
				GroupTreeScalabilityCache groupTreeScalabilityCache = odpMetadata.GroupTreeScalabilityCache;
				int count2 = topLevelDataRegions.Count;
				base.m_dataRegionInstances = new List<IReference<DataRegionInstance>>(count2);
				for (int i = 0; i < count2; i++)
				{
					base.m_dataRegionInstances.Add(groupTreeScalabilityCache.AllocateEmptyTreePartition<DataRegionInstance>(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegionInstanceReference));
				}
			}
		}

		internal ReportInstance()
		{
		}

		internal bool IsMissingExpectedDataChunk(OnDemandProcessingContext odpContext)
		{
			List<DataSet> mappingDataSetIndexToDataSet = odpContext.ReportDefinition.MappingDataSetIndexToDataSet;
			for (int i = 0; i < mappingDataSetIndexToDataSet.Count; i++)
			{
				DataSet dataSet = mappingDataSetIndexToDataSet[i];
				if (!dataSet.UsedOnlyInParameters && this.GetDataSetInstance(dataSet, odpContext) == null)
				{
					return true;
				}
			}
			return false;
		}

		internal DataSetInstance GetDataSetInstance(DataSet dataSet, OnDemandProcessingContext odpContext)
		{
			if (this.m_dataSetInstances == null)
			{
				this.InitDataSetInstances(odpContext);
			}
			int indexInCollection = dataSet.IndexInCollection;
			if (this.m_dataSetInstances[indexInCollection] == null)
			{
				this.m_dataSetInstances[indexInCollection] = odpContext.GetDataSetInstance(dataSet);
			}
			return this.m_dataSetInstances[indexInCollection];
		}

		internal DataSetInstance GetDataSetInstance(int dataSetIndexInCollection, OnDemandProcessingContext odpContext)
		{
			if (this.m_dataSetInstances == null)
			{
				this.InitDataSetInstances(odpContext);
			}
			if (this.m_dataSetInstances[dataSetIndexInCollection] == null)
			{
				DataSet dataSet = odpContext.ReportDefinition.MappingDataSetIndexToDataSet[dataSetIndexInCollection];
				this.m_dataSetInstances[dataSetIndexInCollection] = odpContext.GetDataSetInstance(dataSet);
			}
			return this.m_dataSetInstances[dataSetIndexInCollection];
		}

		internal void SetDataSetInstance(DataSetInstance dataSetInstance)
		{
			this.m_dataSetInstances[dataSetInstance.DataSetDef.IndexInCollection] = dataSetInstance;
		}

		private void InitDataSetInstances(OnDemandProcessingContext odpContext)
		{
			this.m_dataSetInstances = new DataSetInstance[odpContext.ReportDefinition.MappingDataSetIndexToDataSet.Count];
		}

		internal IEnumerator GetCachedDataSetInstances()
		{
			return this.m_dataSetInstances.GetEnumerator();
		}

		internal void InitializeFromSnapshot(OnDemandProcessingContext odpContext)
		{
			if (!odpContext.ReprocessSnapshot)
			{
				int num = 0;
				if (this.m_dataSetInstances == null && odpContext.ReportDefinition.MappingNameToDataSet != null)
				{
					num = odpContext.ReportDefinition.MappingNameToDataSet.Count;
				}
				this.m_dataSetInstances = new DataSetInstance[num];
			}
			Report reportDefinition = odpContext.ReportDefinition;
			this.m_noRows = (reportDefinition.DataSetsNotOnlyUsedInParameters > 0);
			List<DataSource> dataSources = reportDefinition.DataSources;
			for (int i = 0; i < dataSources.Count; i++)
			{
				List<DataSet> dataSets = dataSources[i].DataSets;
				if (dataSets != null)
				{
					for (int j = 0; j < dataSets.Count; j++)
					{
						DataSet dataSet = dataSets[j];
						if (dataSet != null)
						{
							DataSetInstance dataSetInstance = this.GetDataSetInstance(dataSet, odpContext);
							if (dataSetInstance != null && this.m_noRows && !dataSetInstance.NoRows)
							{
								this.m_noRows = false;
							}
						}
					}
				}
			}
		}

		internal override void AddChildScope(IReference<ScopeInstance> child, int indexInCollection)
		{
			base.AddChildScope(child, indexInCollection);
		}

		internal IReference<DataRegionInstance> GetTopLevelDataRegionReference(int indexInCollection)
		{
			return base.m_dataRegionInstances[indexInCollection];
		}

		internal void SetupEnvironment(OnDemandProcessingContext odpContext)
		{
			if (this.m_dataSetInstances == null)
			{
				this.InitDataSetInstances(odpContext);
			}
			for (int i = 0; i < this.m_dataSetInstances.Length; i++)
			{
				DataSetInstance dataSetInstance = this.GetDataSetInstance(i, odpContext);
				if (dataSetInstance != null)
				{
					dataSetInstance.SetupDataSetLevelAggregates(odpContext);
				}
			}
			if (this.m_variables != null)
			{
				ScopeInstance.SetupVariables(odpContext, odpContext.ReportDefinition.Variables, this.m_variables);
			}
		}

		internal void CalculateAndStoreReportVariables(OnDemandProcessingContext odpContext)
		{
			if (odpContext.ReportDefinition.Variables != null && this.m_variables == null)
			{
				ScopeInstance.CalculateVariables(odpContext, odpContext.ReportDefinition.Variables, out this.m_variables);
			}
		}

		internal void ResetReportVariables(OnDemandProcessingContext odpContext)
		{
			if (odpContext.ReportDefinition.Variables != null)
			{
				ScopeInstance.ResetVariables(odpContext, odpContext.ReportDefinition.Variables);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.NoRows, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Language, Token.String));
			list.Add(new ReadOnlyMemberInfo(MemberName.Variables, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, Token.Object));
			list.Add(new MemberInfo(MemberName.SerializableVariables, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SerializableArray, Token.Serializable));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportInstance, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScopeInstance, list);
		}

		internal static IReference<ReportInstance> CreateInstance(IReportInstanceContainer reportInstanceContainer, OnDemandProcessingContext odpContext, Report reportDef, ParameterInfoCollection parameters)
		{
			ReportInstance reportInstance = new ReportInstance(odpContext, reportDef, parameters);
			IReference<ReportInstance> reference = reportInstanceContainer.SetReportInstance(reportInstance, odpContext.OdpMetadata);
			reportInstance.m_cleanupRef = (IDisposable)reference;
			return reference;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ReportInstance.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.NoRows:
					writer.Write(this.m_noRows);
					break;
				case MemberName.Language:
					writer.Write(this.m_language);
					break;
				case MemberName.SerializableVariables:
					writer.WriteSerializableArray(this.m_variables);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(ReportInstance.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.NoRows:
					this.m_noRows = reader.ReadBoolean();
					break;
				case MemberName.Language:
					this.m_language = reader.ReadString();
					break;
				case MemberName.Variables:
					this.m_variables = reader.ReadVariantArray();
					break;
				case MemberName.SerializableVariables:
					this.m_variables = reader.ReadSerializableArray();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportInstance;
		}
	}
}
