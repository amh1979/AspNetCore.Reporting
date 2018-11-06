using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal class OnDemandMetadata : IReportInstanceContainer, IPersistable
	{
		private Dictionary<string, SubReportInfo> m_subReportInfoMap;

		private Dictionary<string, CommonSubReportInfo> m_commonSubReportInfoMap;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot m_reportSnapshot;

		private Dictionary<string, DataSetInstance> m_dataChunkMap = new Dictionary<string, DataSetInstance>();

		private Dictionary<string, bool[]> m_tablixProcessingComplete;

		private Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo> m_cachedExternalImages;

		private Dictionary<string, ShapefileInfo> m_cachedShapefiles;

		private string m_transparentImageChunkName;

		private long m_groupTreeRootOffset = TreePartitionManager.EmptyTreePartitionOffset;

		private TreePartitionManager m_groupTreePartitions;

		private TreePartitionManager m_lookupPartitions;

		private int m_lastAssignedGlobalID = -1;

		private Dictionary<string, UpdatedVariableValues> m_updatedVariableValues;

		[NonSerialized]
		private IReference<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance> m_reportInstance;

		[NonSerialized]
		private AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager m_odpChunkManager;

		[NonSerialized]
		private AspNetCore.ReportingServices.ReportIntermediateFormat.Report m_report;

		[NonSerialized]
		private bool m_isInitialProcessingRequest;

		[NonSerialized]
		private bool m_metaDataChanged;

		[NonSerialized]
		private List<OnDemandProcessingContext> m_odpContexts = new List<OnDemandProcessingContext>();

		[NonSerialized]
		private GroupTreeScalabilityCache m_groupTreeScalabilityCache;

		[NonSerialized]
		private LookupScalabilityCache m_lookupScalabilityCache;

		[NonSerialized]
		private GlobalIDOwnerCollection m_globalIDOwnerCollection;

		[NonSerialized]
		private static readonly Declaration m_Declaration = OnDemandMetadata.GetDeclaration();

		internal bool IsInitialProcessingRequest
		{
			get
			{
				return this.m_isInitialProcessingRequest;
			}
		}

		internal bool GroupTreeHasChanged
		{
			get
			{
				if (this.m_groupTreePartitions != null)
				{
					return this.m_groupTreePartitions.TreeHasChanged;
				}
				return false;
			}
			set
			{
				this.GroupTreePartitionManager.TreeHasChanged = value;
			}
		}

		internal bool LookupInfoHasChanged
		{
			get
			{
				if (this.m_lookupPartitions != null)
				{
					return this.m_lookupPartitions.TreeHasChanged;
				}
				return false;
			}
			set
			{
				this.LookupPartitionManager.TreeHasChanged = value;
			}
		}

		internal bool SnapshotHasChanged
		{
			get
			{
				if (!this.GroupTreeHasChanged && !this.LookupInfoHasChanged && !this.m_metaDataChanged)
				{
					return this.m_reportSnapshot.CachedDataChanged;
				}
				return true;
			}
		}

		internal bool MetadataHasChanged
		{
			get
			{
				return this.m_metaDataChanged;
			}
			set
			{
				this.m_metaDataChanged = value;
			}
		}

		internal TreePartitionManager GroupTreePartitionManager
		{
			get
			{
				if (this.m_groupTreePartitions == null)
				{
					this.m_groupTreePartitions = new TreePartitionManager();
					this.m_groupTreePartitions.TreeHasChanged = true;
				}
				return this.m_groupTreePartitions;
			}
		}

		internal TreePartitionManager LookupPartitionManager
		{
			get
			{
				if (this.m_lookupPartitions == null)
				{
					this.m_lookupPartitions = new TreePartitionManager();
					this.m_lookupPartitions.TreeHasChanged = true;
				}
				return this.m_lookupPartitions;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.Report Report
		{
			get
			{
				return this.m_report;
			}
			set
			{
				this.m_report = value;
			}
		}

		public IReference<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance> ReportInstance
		{
			get
			{
				return this.m_reportInstance;
			}
			set
			{
				this.m_reportInstance = value;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot ReportSnapshot
		{
			get
			{
				return this.m_reportSnapshot;
			}
			set
			{
				this.m_reportSnapshot = value;
				this.m_metaDataChanged = true;
			}
		}

		internal Dictionary<string, DataSetInstance> DataChunkMap
		{
			get
			{
				return this.m_dataChunkMap;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager OdpChunkManager
		{
			get
			{
				return this.m_odpChunkManager;
			}
			set
			{
				this.m_odpChunkManager = value;
			}
		}

		internal List<OnDemandProcessingContext> OdpContexts
		{
			get
			{
				return this.m_odpContexts;
			}
		}

		internal string TransparentImageChunkName
		{
			get
			{
				return this.m_transparentImageChunkName;
			}
			set
			{
				this.m_transparentImageChunkName = value;
				this.m_metaDataChanged = true;
			}
		}

		internal GroupTreeScalabilityCache GroupTreeScalabilityCache
		{
			get
			{
				return this.m_groupTreeScalabilityCache;
			}
			set
			{
				this.m_groupTreeScalabilityCache = value;
			}
		}

		internal LookupScalabilityCache LookupScalabilityCache
		{
			get
			{
				return this.m_lookupScalabilityCache;
			}
			set
			{
				this.m_lookupScalabilityCache = value;
			}
		}

		internal GlobalIDOwnerCollection GlobalIDOwnerCollection
		{
			get
			{
				return this.m_globalIDOwnerCollection;
			}
			set
			{
				this.m_globalIDOwnerCollection = value;
			}
		}

		internal long GroupTreeRootOffset
		{
			get
			{
				return this.m_groupTreeRootOffset;
			}
			set
			{
				this.m_groupTreeRootOffset = value;
				this.m_metaDataChanged = true;
			}
		}

		internal int LastAssignedGlobalID
		{
			get
			{
				return this.m_lastAssignedGlobalID;
			}
		}

		internal OnDemandMetadata()
		{
			this.m_isInitialProcessingRequest = false;
			this.m_metaDataChanged = false;
		}

		internal OnDemandMetadata(AspNetCore.ReportingServices.ReportIntermediateFormat.Report report)
		{
			this.m_report = report;
			this.m_odpChunkManager = new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager();
			this.m_isInitialProcessingRequest = true;
			this.m_metaDataChanged = true;
			this.m_tablixProcessingComplete = new Dictionary<string, bool[]>();
		}

		internal OnDemandMetadata(OnDemandMetadata metadataFromOldSnapshot, AspNetCore.ReportingServices.ReportIntermediateFormat.Report report)
		{
			this.m_isInitialProcessingRequest = true;
			this.m_metaDataChanged = true;
			this.m_report = report;
			this.m_odpChunkManager = metadataFromOldSnapshot.m_odpChunkManager;
			this.m_subReportInfoMap = metadataFromOldSnapshot.m_subReportInfoMap;
			this.m_commonSubReportInfoMap = metadataFromOldSnapshot.m_commonSubReportInfoMap;
			this.m_dataChunkMap = metadataFromOldSnapshot.m_dataChunkMap;
			this.m_lastAssignedGlobalID = metadataFromOldSnapshot.m_lastAssignedGlobalID;
			this.CommonPrepareForReprocessing();
		}

		public IReference<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance> SetReportInstance(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, OnDemandMetadata odpMetadata)
		{
			this.m_reportInstance = this.m_groupTreeScalabilityCache.AllocateAndPin(reportInstance, 0);
			return this.m_reportInstance;
		}

		internal void ResetUserSortFilterContexts()
		{
			foreach (OnDemandProcessingContext odpContext in this.m_odpContexts)
			{
				odpContext.ResetUserSortFilterContext();
			}
		}

		internal void UpdateLastAssignedGlobalID()
		{
			if (this.m_globalIDOwnerCollection != null)
			{
				int lastAssignedID = this.m_globalIDOwnerCollection.LastAssignedID;
				if (lastAssignedID > this.m_lastAssignedGlobalID)
				{
					this.m_lastAssignedGlobalID = lastAssignedID;
					this.m_metaDataChanged = true;
				}
			}
		}

		private void CommonPrepareForReprocessing()
		{
			this.m_tablixProcessingComplete = new Dictionary<string, bool[]>();
			if (this.m_dataChunkMap != null)
			{
				foreach (DataSetInstance value in this.m_dataChunkMap.Values)
				{
					value.InitializeForReprocessing();
				}
			}
		}

		internal void PrepareForCachedDataProcessing(OnDemandMetadata odpMetadata)
		{
			this.m_subReportInfoMap = odpMetadata.m_subReportInfoMap;
			this.m_commonSubReportInfoMap = odpMetadata.m_commonSubReportInfoMap;
			this.m_dataChunkMap = odpMetadata.m_dataChunkMap;
			this.CommonPrepareForReprocessing();
		}

		internal bool IsTablixProcessingComplete(OnDemandProcessingContext odpContext, int dataSetIndexInCollection)
		{
			if (this.m_tablixProcessingComplete == null)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet = odpContext.ReportDefinition.MappingDataSetIndexToDataSet[dataSetIndexInCollection];
				DataSetInstance dataSetInstance = odpContext.GetDataSetInstance(dataSet);
				if (dataSetInstance != null)
				{
					return dataSetInstance.OldSnapshotTablixProcessingComplete;
				}
				return false;
			}
			bool[] array = default(bool[]);
			if (this.m_tablixProcessingComplete.TryGetValue(this.GetUniqueIdFromContext(odpContext), out array))
			{
				return array[dataSetIndexInCollection];
			}
			return false;
		}

		internal void SetTablixProcessingComplete(OnDemandProcessingContext odpContext, int dataSetIndexInCollection)
		{
			if (this.m_tablixProcessingComplete == null)
			{
				this.m_tablixProcessingComplete = new Dictionary<string, bool[]>();
			}
			string uniqueIdFromContext = this.GetUniqueIdFromContext(odpContext);
			bool[] array = default(bool[]);
			if (!this.m_tablixProcessingComplete.TryGetValue(uniqueIdFromContext, out array))
			{
				array = new bool[odpContext.ReportDefinition.DataSetCount];
				this.m_tablixProcessingComplete[uniqueIdFromContext] = array;
			}
			array[dataSetIndexInCollection] = true;
			this.m_metaDataChanged = true;
		}

		private string GetUniqueIdFromContext(OnDemandProcessingContext odpContext)
		{
			if (odpContext.InSubreport)
			{
				string processingAbortItemUniqueIdentifier = odpContext.ProcessingAbortItemUniqueIdentifier;
				Global.Tracer.Assert(!string.IsNullOrEmpty(processingAbortItemUniqueIdentifier), "Subreport ID must not be null or empty");
				return processingAbortItemUniqueIdentifier;
			}
			return string.Empty;
		}

		internal void DisposePersistedTreeScalability()
		{
			if (this.m_groupTreeScalabilityCache != null)
			{
				this.m_groupTreeScalabilityCache.Dispose();
				this.m_groupTreeScalabilityCache = null;
			}
			if (this.m_lookupScalabilityCache != null)
			{
				this.m_lookupScalabilityCache.Dispose();
				this.m_lookupScalabilityCache = null;
			}
		}

		internal void EnsureLookupScalabilitySetup(IChunkFactory chunkFactory, int rifCompatVersion, bool prohibitSerializableValues)
		{
			if (this.m_lookupScalabilityCache == null)
			{
				bool openExisting = this.m_lookupPartitions != null;
				AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.EnsureLookupStorageSetup(this, chunkFactory, openExisting, rifCompatVersion, prohibitSerializableValues);
			}
		}

		internal SubReportInfo AddSubReportInfo(bool isTopLevelSubreport, string definitionPath, string reportPath, string originalCatalogReportPath)
		{
			this.m_metaDataChanged = true;
			if (this.m_subReportInfoMap == null)
			{
				this.m_subReportInfoMap = new Dictionary<string, SubReportInfo>(EqualityComparers.StringComparerInstance);
			}
			Global.Tracer.Assert(!this.m_subReportInfoMap.ContainsKey(definitionPath), "(!m_subReportInfoMap.ContainsKey(definitionPath))");
			SubReportInfo subReportInfo = new SubReportInfo(Guid.NewGuid());
			this.m_subReportInfoMap.Add(definitionPath, subReportInfo);
			string reportPath2 = isTopLevelSubreport ? reportPath : (definitionPath + "_" + reportPath);
			bool flag = default(bool);
			subReportInfo.CommonSubReportInfo = this.GetOrCreateCommonSubReportInfo(reportPath2, out flag);
			if (flag)
			{
				subReportInfo.CommonSubReportInfo.DefinitionUniqueName = subReportInfo.UniqueName;
				subReportInfo.CommonSubReportInfo.OriginalCatalogPath = originalCatalogReportPath;
			}
			return subReportInfo;
		}

		private CommonSubReportInfo GetOrCreateCommonSubReportInfo(string reportPath, out bool created)
		{
			created = false;
			if (this.m_commonSubReportInfoMap == null)
			{
				this.m_commonSubReportInfoMap = new Dictionary<string, CommonSubReportInfo>(EqualityComparers.StringComparerInstance);
			}
			CommonSubReportInfo commonSubReportInfo = default(CommonSubReportInfo);
			if (!this.m_commonSubReportInfoMap.TryGetValue(reportPath, out commonSubReportInfo))
			{
				created = true;
				commonSubReportInfo = new CommonSubReportInfo();
				commonSubReportInfo.ReportPath = reportPath;
				this.m_commonSubReportInfoMap.Add(reportPath, commonSubReportInfo);
			}
			return commonSubReportInfo;
		}

		internal bool TryGetSubReportInfo(bool isTopLevelSubreport, string definitionPath, string reportPath, out SubReportInfo subReportInfo)
		{
			subReportInfo = null;
			if (this.m_subReportInfoMap != null && this.m_subReportInfoMap.TryGetValue(definitionPath, out subReportInfo))
			{
				if (subReportInfo.CommonSubReportInfo == null)
				{
					string key = isTopLevelSubreport ? reportPath : (definitionPath + "_" + reportPath);
					if (this.m_commonSubReportInfoMap != null)
					{
						CommonSubReportInfo commonSubReportInfo = default(CommonSubReportInfo);
						if (this.m_commonSubReportInfoMap.TryGetValue(key, out commonSubReportInfo))
						{
							subReportInfo.CommonSubReportInfo = commonSubReportInfo;
							return true;
						}
						int length = reportPath.Length;
						foreach (string key2 in this.m_commonSubReportInfoMap.Keys)
						{
							if (key2.Length >= length)
							{
								int num = key2.LastIndexOf(reportPath, StringComparison.OrdinalIgnoreCase);
								if (num >= 0 && num + length == key2.Length)
								{
									subReportInfo.CommonSubReportInfo = this.m_commonSubReportInfoMap[key2];
									return true;
								}
							}
						}
						goto IL_00e7;
					}
					subReportInfo = null;
					return false;
				}
				goto IL_00e7;
			}
			return false;
			IL_00e7:
			return true;
		}

		internal SubReportInfo GetSubReportInfo(bool isTopLevelSubreport, string definitionPath, string reportPath)
		{
			SubReportInfo result = null;
			bool condition = this.TryGetSubReportInfo(isTopLevelSubreport, definitionPath, reportPath, out result);
			Global.Tracer.Assert(condition, "Missing expected SubReportInfo: {0}_{1}", definitionPath, reportPath.MarkAsPrivate());
			return result;
		}

		internal void AddDataChunk(string dataSetChunkName, DataSetInstance dataSetInstance)
		{
			this.m_metaDataChanged = true;
			dataSetInstance.DataChunkName = dataSetChunkName;
			lock (this.m_dataChunkMap)
			{
				this.m_dataChunkMap.Add(dataSetChunkName, dataSetInstance);
			}
		}

		internal void DeleteDataChunk(string dataSetChunkName)
		{
			this.m_metaDataChanged = true;
			lock (this.m_dataChunkMap)
			{
				this.m_dataChunkMap.Remove(dataSetChunkName);
			}
		}

		internal void AddExternalImage(string value, AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo imageInfo)
		{
			this.m_metaDataChanged = true;
			if (this.m_cachedExternalImages == null)
			{
				this.m_cachedExternalImages = new Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo>(EqualityComparers.StringComparerInstance);
			}
			this.m_cachedExternalImages.Add(value, imageInfo);
		}

		internal bool TryGetExternalImage(string value, out AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo imageInfo)
		{
			if (this.m_cachedExternalImages != null)
			{
				return this.m_cachedExternalImages.TryGetValue(value, out imageInfo);
			}
			imageInfo = null;
			return false;
		}

		internal void AddShapefile(string value, ShapefileInfo shapefileInfo)
		{
			this.m_metaDataChanged = true;
			if (this.m_cachedShapefiles == null)
			{
				this.m_cachedShapefiles = new Dictionary<string, ShapefileInfo>(EqualityComparers.StringComparerInstance);
			}
			this.m_cachedShapefiles.Add(value, shapefileInfo);
		}

		internal bool TryGetShapefile(string value, out ShapefileInfo shapefileInfo)
		{
			if (this.m_cachedShapefiles != null)
			{
				return this.m_cachedShapefiles.TryGetValue(value, out shapefileInfo);
			}
			shapefileInfo = null;
			return false;
		}

		internal bool StoreUpdatedVariableValue(OnDemandProcessingContext odpContext, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, int index, object value)
		{
			this.m_metaDataChanged = true;
			if (this.m_updatedVariableValues == null)
			{
				this.m_updatedVariableValues = new Dictionary<string, UpdatedVariableValues>();
			}
			string key = odpContext.SubReportUniqueName ?? "Report";
			UpdatedVariableValues updatedVariableValues = default(UpdatedVariableValues);
			Dictionary<int, object> dictionary;
			if (this.m_updatedVariableValues.TryGetValue(key, out updatedVariableValues))
			{
				dictionary = updatedVariableValues.VariableValues;
			}
			else
			{
				dictionary = new Dictionary<int, object>();
				updatedVariableValues = new UpdatedVariableValues();
				updatedVariableValues.VariableValues = dictionary;
				this.m_updatedVariableValues.Add(key, updatedVariableValues);
			}
			if (reportInstance != null && reportInstance.VariableValues != null)
			{
				reportInstance.VariableValues[index] = value;
			}
			dictionary[index] = value;
			return true;
		}

		internal void SetUpdatedVariableValues(OnDemandProcessingContext odpContext, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance)
		{
			if (this.m_updatedVariableValues != null)
			{
				string key = odpContext.SubReportUniqueName ?? "Report";
				UpdatedVariableValues updatedVariableValues = default(UpdatedVariableValues);
				if (this.m_updatedVariableValues.TryGetValue(key, out updatedVariableValues))
				{
					Dictionary<int, object> variableValues = updatedVariableValues.VariableValues;
					List<AspNetCore.ReportingServices.ReportIntermediateFormat.Variable> variables = odpContext.ReportDefinition.Variables;
					foreach (KeyValuePair<int, object> item in variableValues)
					{
						reportInstance.VariableValues[item.Key] = item.Value;
						AspNetCore.ReportingServices.ReportIntermediateFormat.Variable variable = variables[item.Key];
						VariableImpl cachedVariableObj = variable.GetCachedVariableObj(odpContext);
						cachedVariableObj.SetValue(item.Value, true);
					}
				}
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.CommonSubReportInfos, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringRIFObjectDictionary, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CommonSubReportInfo));
			list.Add(new MemberInfo(MemberName.SubReportInfos, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringRIFObjectDictionary, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReportInfo));
			list.Add(new MemberInfo(MemberName.ReportSnapshot, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportSnapshot));
			list.Add(new ReadOnlyMemberInfo(MemberName.GroupTreePartitionOffsets, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int64));
			list.Add(new MemberInfo(MemberName.DataChunkMap, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringRIFObjectDictionary, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSetInstance));
			list.Add(new MemberInfo(MemberName.CachedExternalImages, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringRIFObjectDictionary, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ImageInfo));
			list.Add(new MemberInfo(MemberName.TransparentImageChunkName, Token.String));
			list.Add(new MemberInfo(MemberName.GroupTreeRootOffset, Token.Int64));
			list.Add(new MemberInfo(MemberName.TablixProcessingComplete, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringBoolArrayDictionary, Token.Boolean));
			list.Add(new MemberInfo(MemberName.GroupTreePartitions, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TreePartitionManager));
			list.Add(new MemberInfo(MemberName.LookupPartitions, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TreePartitionManager));
			list.Add(new MemberInfo(MemberName.LastAssignedGlobalID, Token.Int32));
			list.Add(new MemberInfo(MemberName.CachedShapefiles, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringRIFObjectDictionary, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ShapefileInfo));
			list.Add(new MemberInfo(MemberName.UpdatedVariableValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringRIFObjectDictionary, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.UpdatedVariableValues));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.OnDemandMetadata, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(OnDemandMetadata.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.CommonSubReportInfos:
					writer.WriteStringRIFObjectDictionary(this.m_commonSubReportInfoMap);
					break;
				case MemberName.SubReportInfos:
					writer.WriteStringRIFObjectDictionary(this.m_subReportInfoMap);
					break;
				case MemberName.ReportSnapshot:
					writer.Write(this.m_reportSnapshot);
					break;
				case MemberName.DataChunkMap:
					writer.WriteStringRIFObjectDictionary(this.m_dataChunkMap);
					break;
				case MemberName.CachedExternalImages:
					writer.WriteStringRIFObjectDictionary(this.m_cachedExternalImages);
					break;
				case MemberName.CachedShapefiles:
					writer.WriteStringRIFObjectDictionary(this.m_cachedShapefiles);
					break;
				case MemberName.TransparentImageChunkName:
					writer.Write(this.m_transparentImageChunkName);
					break;
				case MemberName.GroupTreeRootOffset:
					writer.Write(this.m_groupTreeRootOffset);
					break;
				case MemberName.TablixProcessingComplete:
					writer.WriteStringBoolArrayDictionary(this.m_tablixProcessingComplete);
					break;
				case MemberName.GroupTreePartitions:
					writer.Write(this.m_groupTreePartitions);
					break;
				case MemberName.LookupPartitions:
					writer.Write(this.m_lookupPartitions);
					break;
				case MemberName.LastAssignedGlobalID:
					writer.Write(this.m_lastAssignedGlobalID);
					break;
				case MemberName.UpdatedVariableValues:
					writer.WriteStringRIFObjectDictionary(this.m_updatedVariableValues);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(OnDemandMetadata.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.CommonSubReportInfos:
					this.m_commonSubReportInfoMap = reader.ReadStringRIFObjectDictionary<CommonSubReportInfo>();
					break;
				case MemberName.SubReportInfos:
					this.m_subReportInfoMap = reader.ReadStringRIFObjectDictionary<SubReportInfo>();
					break;
				case MemberName.ReportSnapshot:
					this.m_reportSnapshot = (AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot)reader.ReadRIFObject();
					break;
				case MemberName.GroupTreePartitionOffsets:
				{
					List<long> list = reader.ReadListOfPrimitives<long>();
					if (list != null)
					{
						this.m_groupTreePartitions = new TreePartitionManager(list);
					}
					break;
				}
				case MemberName.DataChunkMap:
					this.m_dataChunkMap = reader.ReadStringRIFObjectDictionary<DataSetInstance>();
					break;
				case MemberName.CachedExternalImages:
					this.m_cachedExternalImages = reader.ReadStringRIFObjectDictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo>();
					break;
				case MemberName.CachedShapefiles:
					this.m_cachedShapefiles = reader.ReadStringRIFObjectDictionary<ShapefileInfo>();
					break;
				case MemberName.TransparentImageChunkName:
					this.m_transparentImageChunkName = reader.ReadString();
					break;
				case MemberName.GroupTreeRootOffset:
					this.m_groupTreeRootOffset = reader.ReadInt64();
					break;
				case MemberName.TablixProcessingComplete:
					this.m_tablixProcessingComplete = reader.ReadStringBoolArrayDictionary();
					break;
				case MemberName.GroupTreePartitions:
					this.m_groupTreePartitions = (TreePartitionManager)reader.ReadRIFObject();
					break;
				case MemberName.LookupPartitions:
					this.m_lookupPartitions = (TreePartitionManager)reader.ReadRIFObject();
					break;
				case MemberName.LastAssignedGlobalID:
					this.m_lastAssignedGlobalID = reader.ReadInt32();
					break;
				case MemberName.UpdatedVariableValues:
					this.m_updatedVariableValues = reader.ReadStringRIFObjectDictionary<UpdatedVariableValues>();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.OnDemandMetadata;
		}
	}
}
