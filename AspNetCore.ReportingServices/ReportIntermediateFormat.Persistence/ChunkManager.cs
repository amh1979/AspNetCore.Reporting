using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal sealed class ChunkManager
	{
		internal sealed class DataChunkWriter : PersistenceHelper
		{
			private static List<Declaration> m_DataChunkDeclarations = DataChunkWriter.GetDataChunkDeclarations();

			private IChunkFactory m_reportChunkFactory;

			private string m_dataSetChunkName;

			private RecordSetInfo m_recordSetInfo;

			private IntermediateFormatWriter? m_chunkWriter = null;

			private Stream m_chunkStream;

			private OnDemandProcessingContext m_odpContext;

			internal DataChunkWriter(RecordSetInfo recordSetInfo, DataSetInstance dataSetInstance, OnDemandProcessingContext odpContext)
			{
				Global.Tracer.Assert(null != odpContext.ChunkFactory, "(null != context.ChunkFactory)");
				this.m_reportChunkFactory = odpContext.ChunkFactory;
				this.m_recordSetInfo = recordSetInfo;
				this.m_odpContext = odpContext;
				if (odpContext.IsSharedDataSetExecutionOnly)
				{
					this.m_dataSetChunkName = (odpContext.ExternalDataSetContext.TargetChunkNameInSnapshot ?? "SharedDataSet");
				}
				else
				{
					this.m_dataSetChunkName = ChunkManager.GenerateDataChunkName(dataSetInstance, odpContext);
					odpContext.OdpMetadata.AddDataChunk(this.m_dataSetChunkName, dataSetInstance);
				}
			}

			internal DataChunkWriter(DataSetInstance dataSetInstance, OnDemandProcessingContext context)
			{
				Global.Tracer.Assert(null != context.ChunkFactory, "(null != context.ChunkFactory)");
				this.m_odpContext = context;
				this.m_dataSetChunkName = ChunkManager.GenerateDataChunkName(dataSetInstance, context);
				this.m_reportChunkFactory = context.ChunkFactory;
			}

			internal void Close()
			{
				this.m_chunkWriter = null;
				if (this.m_chunkStream != null)
				{
					this.m_chunkStream.Close();
					this.m_chunkStream = null;
				}
			}

			internal void CloseAndEraseChunk()
			{
				this.Close();
				if (this.m_reportChunkFactory != null)
				{
					this.m_reportChunkFactory.Erase(this.m_dataSetChunkName, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Data);
					if (!this.m_odpContext.IsSharedDataSetExecutionOnly)
					{
						this.m_odpContext.OdpMetadata.DeleteDataChunk(this.m_dataSetChunkName);
					}
				}
			}

			internal void CreateDataChunkAndWriteHeader(RecordSetInfo recordSetInfo)
			{
				if (this.m_chunkStream == null)
				{
					this.m_recordSetInfo = recordSetInfo;
					this.m_chunkStream = this.m_reportChunkFactory.CreateChunk(this.m_dataSetChunkName, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Data, null);
					this.m_chunkWriter = new IntermediateFormatWriter(this.m_chunkStream, DataChunkWriter.m_DataChunkDeclarations, this, this.m_odpContext.GetActiveCompatibilityVersion(), this.m_odpContext.ProhibitSerializableValues);
					this.m_chunkWriter.Value.Write(this.m_recordSetInfo);
				}
			}

			internal void WriteRecordRow(RecordRow recordRow)
			{
				try
				{
					if (this.m_chunkStream == null)
					{
						this.CreateDataChunkAndWriteHeader(this.m_recordSetInfo);
					}
					recordRow.StreamPosition = this.m_chunkStream.Position;
					this.m_chunkWriter.Value.Write(recordRow);
				}
				catch (Exception)
				{
					this.Close();
					throw;
				}
			}

			private static List<Declaration> GetDataChunkDeclarations()
			{
				List<Declaration> list = new List<Declaration>(4);
				list.Add(RecordSetInfo.GetDeclaration());
				list.Add(RecordRow.GetDeclaration());
				list.Add(RecordField.GetDeclaration());
				list.Add(RecordSetPropertyNames.GetDeclaration());
				return list;
			}
		}

		internal sealed class DataChunkReader : IRecordRowReader, IDisposable
		{
			private Stream m_chunkStream;

			private IntermediateFormatReader? m_chunkReader;

			private RecordSetInfo m_recordSetInfo;

			private RecordRow m_recordRow;

			private int m_recordSetSize = -1;

			private int m_currentRow = -1;

			private long m_streamLength = -1L;

			private long m_previousStreamOffset = -1L;

			private long m_firstRow = -1L;

			private int[] m_mappingDataSetFieldIndexesToDataChunk;

			private bool m_mappingIdentical;

			internal bool ReaderExtensionsSupported
			{
				get
				{
					if (this.m_chunkStream == null)
					{
						return false;
					}
					return this.m_recordSetInfo.ReaderExtensionsSupported;
				}
			}

			internal bool ReaderFieldProperties
			{
				get
				{
					if (this.m_recordSetInfo != null)
					{
						return null != this.m_recordSetInfo.FieldPropertyNames;
					}
					return false;
				}
			}

			internal bool ValidCompareOptions
			{
				get
				{
					if (this.m_chunkStream == null)
					{
						return false;
					}
					return this.m_recordSetInfo.ValidCompareOptions;
				}
			}

			internal CompareOptions CompareOptions
			{
				get
				{
					return this.m_recordSetInfo.CompareOptions;
				}
			}

			internal RecordSetInfo RecordSetInfo
			{
				get
				{
					return this.m_recordSetInfo;
				}
			}

			public RecordRow RecordRow
			{
				get
				{
					return this.m_recordRow;
				}
			}

			internal bool IsAggregateRow
			{
				get
				{
					return this.m_recordRow.IsAggregateRow;
				}
			}

			internal int AggregationFieldCount
			{
				get
				{
					return this.m_recordRow.AggregationFieldCount;
				}
			}

			internal RecordSetPropertyNamesList FieldPropertyNames
			{
				get
				{
					return this.m_recordSetInfo.FieldPropertyNames;
				}
			}

			internal DataChunkReader(DataSetInstance dataSetInstance, OnDemandProcessingContext context, string chunkName)
			{
				this.m_recordSetSize = dataSetInstance.RecordSetSize;
				Global.Tracer.Assert(context.ChunkFactory != null && !string.IsNullOrEmpty(chunkName), "null != context.ChunkFactory && !String.IsNullOrEmpty(chunkName)");
				string text = default(string);
				this.m_chunkStream = context.ChunkFactory.GetChunk(chunkName, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Data, ChunkMode.Open, out text);
				Global.Tracer.Assert(this.m_chunkStream != null, "Missing Expected DataChunk with name: {0}", chunkName);
				this.m_chunkReader = new IntermediateFormatReader(this.m_chunkStream, (IRIFObjectCreator)(object)default(DataReaderRIFObjectCreator));
				this.m_recordSetInfo = (RecordSetInfo)this.m_chunkReader.Value.ReadRIFObject();
				if (context.IsSharedDataSetExecutionOnly || dataSetInstance.DataSetDef.IsReferenceToSharedDataSet)
				{
					DataChunkReader.CreateDataChunkFieldMapping(dataSetInstance, this.m_recordSetInfo, context.IsSharedDataSetExecutionOnly, out this.m_mappingIdentical, out this.m_mappingDataSetFieldIndexesToDataChunk);
				}
				this.m_firstRow = this.m_chunkStream.Position;
				if (-1 == this.m_recordSetSize)
				{
					this.m_streamLength = this.m_chunkStream.Length;
					Global.Tracer.Assert(this.m_streamLength >= this.m_firstRow, "(m_streamLength >= m_firstRow)");
				}
			}

			internal static void OverrideWithDataReaderSettings(RecordSetInfo recordSetInfo, OnDemandProcessingContext odpContext, DataSetInstance dataSetInstance, DataSetCore dataSetCore)
			{
				if (recordSetInfo != null)
				{
					dataSetCore.MergeCollationSettings(null, null, recordSetInfo.CultureName, (recordSetInfo.CompareOptions & CompareOptions.IgnoreCase) == CompareOptions.None, (recordSetInfo.CompareOptions & CompareOptions.IgnoreNonSpace) == CompareOptions.None, (recordSetInfo.CompareOptions & CompareOptions.IgnoreKanaType) == CompareOptions.None, (recordSetInfo.CompareOptions & CompareOptions.IgnoreWidth) == CompareOptions.None);
					odpContext.SetComparisonInformation(dataSetCore);
					odpContext.ReportObjectModel.FieldsImpl.ReaderExtensionsSupported = recordSetInfo.ReaderExtensionsSupported;
					odpContext.ReportObjectModel.FieldsImpl.ReaderFieldProperties = (recordSetInfo != null && null != recordSetInfo.FieldPropertyNames);
					dataSetInstance.CommandText = recordSetInfo.CommandText;
					dataSetInstance.RewrittenCommandText = recordSetInfo.RewrittenCommandText;
					dataSetInstance.SetQueryExecutionTime(recordSetInfo.ExecutionTime);
				}
			}

			internal static void CreateDataChunkFieldMapping(DataSetInstance currentDataSetInstance, RecordSetInfo recordSetInfo, bool isSharedDataSetExecutionReader, out bool mappingIdentical, out int[] mappingDataSetFieldIndexesToDataChunk)
			{
				mappingDataSetFieldIndexesToDataChunk = null;
				mappingIdentical = true;
				string[] fieldNames = recordSetInfo.FieldNames;
				RecordSetPropertyNamesList fieldPropertyNames = recordSetInfo.FieldPropertyNames;
				List<Field> fields = currentDataSetInstance.DataSetDef.Fields;
				bool flag = isSharedDataSetExecutionReader;
				if (fieldNames != null && fields != null)
				{
					int num = flag ? currentDataSetInstance.DataSetDef.Fields.Count : currentDataSetInstance.DataSetDef.NonCalculatedFieldCount;
					if (fieldPropertyNames != null && fieldPropertyNames.Count > 0)
					{
						currentDataSetInstance.FieldInfos = new FieldInfo[num];
					}
					mappingIdentical = (fieldNames.Length == num);
					Dictionary<string, int> dictionary = new Dictionary<string, int>(StringComparer.Ordinal);
					for (int i = 0; i < fieldNames.Length; i++)
					{
						dictionary.Add(fieldNames[i], i);
					}
					int count = fields.Count;
					int num2 = 0;
					mappingDataSetFieldIndexesToDataChunk = new int[num];
					for (int j = 0; j < count; j++)
					{
						if (!fields[j].IsCalculatedField || flag)
						{
							string key = fields[j].DataField;
							if (isSharedDataSetExecutionReader || fields[j].IsCalculatedField)
							{
								key = fields[j].Name;
							}
							int num3 = default(int);
							if (dictionary.TryGetValue(key, out num3))
							{
								mappingDataSetFieldIndexesToDataChunk[num2] = num3;
								if (fieldPropertyNames != null && num3 < fieldPropertyNames.Count && fieldPropertyNames[num3] != null)
								{
									List<string> propertyNames = fieldPropertyNames.GetPropertyNames(num3);
									if (propertyNames != null)
									{
										currentDataSetInstance.FieldInfos[num2] = new FieldInfo(DataChunkReader.CreateSequentialIndexList(propertyNames.Count), propertyNames);
									}
								}
								if (num2 != num3)
								{
									mappingIdentical = false;
								}
							}
							else
							{
								mappingDataSetFieldIndexesToDataChunk[num2] = -1;
								mappingIdentical = false;
							}
							num2++;
						}
					}
				}
			}

			private static List<int> CreateSequentialIndexList(int capacity)
			{
				List<int> list = new List<int>(capacity);
				for (int i = 0; i < capacity; i++)
				{
					list.Add(i);
				}
				return list;
			}

			public bool MoveToFirstRow()
			{
				if (this.m_chunkStream != null && this.m_chunkStream.CanSeek)
				{
					this.m_chunkReader.Value.Seek(this.m_firstRow, SeekOrigin.Begin);
					this.m_currentRow = -1;
					this.m_previousStreamOffset = -1L;
					this.m_recordRow = null;
					return true;
				}
				return false;
			}

			internal void ResetCachedStreamOffset()
			{
				this.m_previousStreamOffset = -1L;
			}

			public bool GetNextRow()
			{
				if (this.m_chunkStream == null)
				{
					return false;
				}
				bool flag = false;
				if (-1 == this.m_recordSetSize)
				{
					long position = this.m_chunkStream.Position;
					if (position < this.m_streamLength - 1)
					{
						flag = true;
					}
				}
				else if (this.m_currentRow < this.m_recordSetSize - 1)
				{
					flag = true;
				}
				if (flag)
				{
					this.m_previousStreamOffset = this.m_chunkStream.Position;
					this.m_currentRow++;
					this.ReadNextRow();
				}
				return flag;
			}

			internal bool ReadOneRowAtPosition(long offset)
			{
				if (this.m_chunkStream == null)
				{
					return false;
				}
				if (this.m_previousStreamOffset == offset)
				{
					return false;
				}
				this.m_previousStreamOffset = offset;
				this.m_chunkReader.Value.Seek(offset, SeekOrigin.Begin);
				this.ReadNextRow();
				return true;
			}

			internal object GetFieldValue(int aliasIndex)
			{
				object obj = null;
				if (this.m_recordRow.RecordFields[aliasIndex] == null)
				{
					throw new ReportProcessingException_FieldError(DataFieldStatus.IsMissing, null);
				}
				return this.m_recordRow.GetFieldValue(aliasIndex);
			}

			internal bool IsAggregationField(int aliasIndex)
			{
				return this.m_recordRow.IsAggregationField(aliasIndex);
			}

			internal object GetPropertyValue(int aliasIndex, int propertyIndex)
			{
				if (this.m_recordSetInfo.FieldPropertyNames != null && this.m_recordRow.RecordFields[aliasIndex] != null)
				{
					List<object> fieldPropertyValues = this.m_recordRow.RecordFields[aliasIndex].FieldPropertyValues;
					if (fieldPropertyValues != null && propertyIndex >= 0 && propertyIndex < fieldPropertyValues.Count)
					{
						return fieldPropertyValues[propertyIndex];
					}
				}
				return null;
			}

			internal int GetPropertyCount(int aliasIndex)
			{
				if (this.m_recordSetInfo.FieldPropertyNames != null && this.m_recordRow.RecordFields[aliasIndex] != null && this.m_recordRow.RecordFields[aliasIndex].FieldPropertyValues != null)
				{
					return this.m_recordRow.RecordFields[aliasIndex].FieldPropertyValues.Count;
				}
				return 0;
			}

			internal string GetPropertyName(int aliasIndex, int propertyIndex)
			{
				if (this.m_recordSetInfo.FieldPropertyNames != null && this.m_recordSetInfo.FieldPropertyNames[aliasIndex] != null)
				{
					return this.m_recordSetInfo.FieldPropertyNames[aliasIndex].PropertyNames[propertyIndex];
				}
				return null;
			}

			public void Close()
			{
				this.Dispose(true);
			}

			public void Dispose()
			{
				this.Dispose(true);
			}

			private void Dispose(bool disposing)
			{
				if (disposing)
				{
					if (this.m_chunkReader.HasValue)
					{
						this.m_chunkReader = null;
					}
					if (this.m_chunkStream != null)
					{
						this.m_chunkStream.Close();
						this.m_chunkStream = null;
					}
				}
				this.m_recordRow = null;
				this.m_recordSetInfo = null;
			}

			private void ReadNextRow()
			{
				this.m_recordRow = (RecordRow)this.m_chunkReader.Value.ReadRIFObject();
				if (!this.m_mappingIdentical)
				{
					this.m_recordRow.ApplyFieldMapping(this.m_mappingDataSetFieldIndexesToDataChunk);
				}
			}
		}

		internal sealed class OnDemandProcessingManager
		{
			private static List<Declaration> m_ChunkDeclarations;

			private OnDemandProcessingContext m_odpContext;

			internal OnDemandProcessingManager()
			{
			}

			internal void SetOdpContext(OnDemandProcessingContext odpContext)
			{
				this.m_odpContext = odpContext;
			}

			internal static GlobalIDOwnerCollection DeserializeOdpReportSnapshot(ProcessingContext pc, IChunkFactory originalSnapshotChunks, ProcessingErrorContext errorContext, bool fetchSubreports, bool deserializeGroupTree, IConfiguration configuration, ref OnDemandMetadata odpMetadata, out Report report)
			{
				GlobalIDOwnerCollection globalIDOwnerCollection = new GlobalIDOwnerCollection();
				report = AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.DeserializeKatmaiReport(pc.ChunkFactory, true, globalIDOwnerCollection);
				IChunkFactory chunkFactory = originalSnapshotChunks ?? pc.ChunkFactory;
				if (odpMetadata == null)
				{
					odpMetadata = OnDemandProcessingManager.DeserializeOnDemandMetadata(chunkFactory, globalIDOwnerCollection);
				}
				if (pc.Parameters != null)
				{
					pc.Parameters.StoreLabels();
				}
				if (fetchSubreports)
				{
					AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.FetchSubReports(report, pc.ChunkFactory, errorContext, odpMetadata, pc.ReportContext, pc.OnDemandSubReportCallback, 0, true, false, globalIDOwnerCollection, pc.QueryParameters);
					if (deserializeGroupTree)
					{
						OnDemandProcessingManager.DeserializeGroupTree(report, chunkFactory, globalIDOwnerCollection, configuration, ref odpMetadata);
					}
				}
				odpMetadata.GlobalIDOwnerCollection = globalIDOwnerCollection;
				return globalIDOwnerCollection;
			}

			internal static void DeserializeGroupTree(Report report, IChunkFactory chunkFactory, GlobalIDOwnerCollection globalIDOwnerCollection, IConfiguration configuration, ref OnDemandMetadata odpMetadata)
			{
				bool prohibitSerializableValues = configuration != null && configuration.ProhibitSerializableValues;
				OnDemandProcessingManager.EnsureGroupTreeStorageSetup(odpMetadata, chunkFactory, globalIDOwnerCollection, true, ReportProcessingCompatibilityVersion.GetCompatibilityVersion(configuration), prohibitSerializableValues);
				IStorage storage = odpMetadata.GroupTreeScalabilityCache.Storage;
				GroupTreePartition groupTreePartition = (GroupTreePartition)storage.Retrieve(odpMetadata.GroupTreeRootOffset);
				Global.Tracer.Assert(groupTreePartition.TopLevelScopeInstances[0].GetObjectType() == ObjectType.ReportInstanceReference, "GroupTree root partition did not contain a ReportInstance");
				odpMetadata.ReportInstance = (groupTreePartition.TopLevelScopeInstances[0] as IReference<ReportInstance>);
				odpMetadata.Report = report;
				odpMetadata.ReportSnapshot.Report = report;
			}

			internal static void EnsureGroupTreeStorageSetup(OnDemandMetadata odpMetadata, IChunkFactory chunkFactory, GlobalIDOwnerCollection globalIDOwnerCollection, bool openExisting, int rifCompatVersion, bool prohibitSerializableValues)
			{
				if (odpMetadata.GroupTreeScalabilityCache == null)
				{
					IStreamHandler streamHandler = OnDemandProcessingManager.BuildChunkStreamHandler("GroupTree", AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Main, chunkFactory, openExisting);
					IStorage storage = new RIFAppendOnlyStorage(streamHandler, (IScalabilityObjectCreator)(object)default(GroupTreeRIFObjectCreator), GroupTreeReferenceCreator.Instance, globalIDOwnerCollection, openExisting, rifCompatVersion, prohibitSerializableValues);
					odpMetadata.GroupTreeScalabilityCache = new GroupTreeScalabilityCache(odpMetadata.GroupTreePartitionManager, storage);
				}
			}

			internal static void EnsureLookupStorageSetup(OnDemandMetadata odpMetadata, IChunkFactory chunkFactory, bool openExisting, int rifCompatVersion, bool prohibitSerializableValues)
			{
				if (odpMetadata.LookupScalabilityCache == null)
				{
					new AppendOnlySpaceManager();
					IStreamHandler streamHandler = OnDemandProcessingManager.BuildChunkStreamHandler("LookupInfo", AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.LookupInfo, chunkFactory, openExisting);
					IStorage storage = new RIFAppendOnlyStorage(streamHandler, (IScalabilityObjectCreator)(object)default(LookupRIFObjectCreator), LookupReferenceCreator.Instance, null, openExisting, rifCompatVersion, prohibitSerializableValues);
					odpMetadata.LookupScalabilityCache = new LookupScalabilityCache(odpMetadata.LookupPartitionManager, storage);
				}
			}

			private static IStreamHandler BuildChunkStreamHandler(string chunkName, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes chunkType, IChunkFactory chunkFactory, bool openExisting)
			{
				return new ChunkFactoryStreamHandler(chunkName, chunkType, chunkFactory, openExisting);
			}

			internal static void PreparePartitionedTreesForAsyncSerialization(OnDemandProcessingContext odpContext)
			{
				OnDemandProcessingManager.PreparePartitionedTreeForAsyncSerialization(odpContext.OdpMetadata.GroupTreeScalabilityCache, odpContext, "GroupTree", AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Main);
				OnDemandProcessingManager.PreparePartitionedTreeForAsyncSerialization(odpContext.OdpMetadata.LookupScalabilityCache, odpContext, "LookupInfo", AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.LookupInfo);
			}

			private static void PreparePartitionedTreeForAsyncSerialization(PartitionedTreeScalabilityCache scaleCache, OnDemandProcessingContext odpContext, string chunkName, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes chunkType)
			{
				if (scaleCache != null)
				{
					RIFAppendOnlyStorage rIFAppendOnlyStorage = scaleCache.Storage as RIFAppendOnlyStorage;
					if (rIFAppendOnlyStorage != null)
					{
						IStreamHandler streamHandler = OnDemandProcessingManager.BuildChunkStreamHandler(chunkName, chunkType, odpContext.ChunkFactory, rIFAppendOnlyStorage.FromExistingStream);
						rIFAppendOnlyStorage.Reset(streamHandler);
					}
					scaleCache.PrepareForFlush();
				}
			}

			internal static void PreparePartitionedTreesForSyncSerialization(OnDemandProcessingContext odpContext)
			{
				OnDemandMetadata odpMetadata = odpContext.OdpMetadata;
				if (odpMetadata.GroupTreeScalabilityCache != null)
				{
					odpMetadata.GroupTreeScalabilityCache.PrepareForFlush();
				}
				if (odpMetadata.LookupScalabilityCache != null)
				{
					odpMetadata.LookupScalabilityCache.PrepareForFlush();
				}
			}

			internal static OnDemandMetadata DeserializeOnDemandMetadata(IChunkFactory chunkFactory, GlobalIDOwnerCollection globalIDOwnerCollection)
			{
				Stream stream = null;
				try
				{
					string text = default(string);
					stream = chunkFactory.GetChunk("Metadata", AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Main, ChunkMode.Open, out text);
					IntermediateFormatReader intermediateFormatReader = new IntermediateFormatReader(stream, (IRIFObjectCreator)(object)default(GroupTreeRIFObjectCreator), globalIDOwnerCollection);
					OnDemandMetadata onDemandMetadata = (OnDemandMetadata)intermediateFormatReader.ReadRIFObject();
					Global.Tracer.Assert(null != onDemandMetadata, "(null != odpMetadata)");
					stream.Close();
					stream = null;
					onDemandMetadata.OdpChunkManager = new OnDemandProcessingManager();
					return onDemandMetadata;
				}
				finally
				{
					if (stream != null)
					{
						stream.Close();
					}
				}
			}

			internal void SerializeSnapshot()
			{
				Global.Tracer.Assert(null != this.m_odpContext, "OnDemandProcessingContext is unavailable");
				OnDemandMetadata odpMetadata = this.m_odpContext.OdpMetadata;
				if (odpMetadata.SnapshotHasChanged)
				{
					try
					{
						IReference<ReportInstance> reportInstance = odpMetadata.ReportInstance;
						Global.Tracer.Assert(reportInstance != null, "Missing GroupTreeRoot");
						if (odpMetadata.IsInitialProcessingRequest)
						{
							reportInstance.UnPinValue();
						}
						if (odpMetadata.GroupTreeHasChanged || odpMetadata.IsInitialProcessingRequest)
						{
							GroupTreeScalabilityCache groupTreeScalabilityCache = this.m_odpContext.OdpMetadata.GroupTreeScalabilityCache;
							groupTreeScalabilityCache.Flush();
							if (odpMetadata.IsInitialProcessingRequest)
							{
								GroupTreePartition groupTreePartition = new GroupTreePartition();
								groupTreePartition.AddTopLevelScopeInstance((IReference<ScopeInstance>)reportInstance);
								long groupTreeRootOffset = groupTreeScalabilityCache.Storage.Allocate(groupTreePartition);
								groupTreeScalabilityCache.Storage.Flush();
								odpMetadata.GroupTreeRootOffset = groupTreeRootOffset;
							}
						}
						if (odpMetadata.LookupInfoHasChanged)
						{
							LookupScalabilityCache lookupScalabilityCache = this.m_odpContext.OdpMetadata.LookupScalabilityCache;
							lookupScalabilityCache.Flush();
						}
						OnDemandProcessingManager.SerializeMetadata(this.m_odpContext.ChunkFactory, this.m_odpContext.OdpMetadata, this.m_odpContext.GetActiveCompatibilityVersion(), this.m_odpContext.ProhibitSerializableValues);
						OnDemandProcessingManager.SerializeSortFilterEventInfo(this.m_odpContext);
					}
					finally
					{
						if (odpMetadata != null)
						{
							odpMetadata.DisposePersistedTreeScalability();
						}
					}
				}
			}

			internal static void SerializeMetadata(IChunkFactory chunkFactory, OnDemandMetadata odpMetadata, int compatVersion, bool prohibitSerializableValues)
			{
				odpMetadata.UpdateLastAssignedGlobalID();
				using (Stream str = chunkFactory.CreateChunk("Metadata", AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Main, null))
				{
					new IntermediateFormatWriter(str, compatVersion, prohibitSerializableValues).Write(odpMetadata);
				}
			}

			private static void SerializeSortFilterEventInfo(OnDemandProcessingContext odpContext)
			{
				ReportSnapshot reportSnapshot = odpContext.OdpMetadata.ReportSnapshot;
				if (reportSnapshot != null && reportSnapshot.SortFilterEventInfo != null)
				{
					Stream stream = null;
					try
					{
						string text = default(string);
						stream = odpContext.ChunkFactory.GetChunk("SortFilterEventInfo", AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Interactivity, ChunkMode.OpenOrCreate, out text);
						stream.Seek(0L, SeekOrigin.End);
						new IntermediateFormatWriter(stream, odpContext.GetActiveCompatibilityVersion(), odpContext.ProhibitSerializableValues).Write(reportSnapshot.SortFilterEventInfo);
						reportSnapshot.SortFilterEventInfo = null;
					}
					finally
					{
						if (stream != null)
						{
							stream.Close();
						}
					}
				}
			}

			internal static SortFilterEventInfoMap DeserializeSortFilterEventInfo(IChunkFactory originalSnapshotChunks, GlobalIDOwnerCollection globalIDOwnerCollection)
			{
				Stream stream = null;
				SortFilterEventInfoMap sortFilterEventInfoMap = null;
				try
				{
					string text = default(string);
					stream = originalSnapshotChunks.GetChunk("SortFilterEventInfo", AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Interactivity, ChunkMode.Open, out text);
					if (stream != null)
					{
						IntermediateFormatReader intermediateFormatReader;
						do
						{
							intermediateFormatReader = new IntermediateFormatReader(stream, new ProcessingRIFObjectCreator(null, null), globalIDOwnerCollection);
							SortFilterEventInfoMap sortFilterEventInfoMap2 = (SortFilterEventInfoMap)intermediateFormatReader.ReadRIFObject();
							Global.Tracer.Assert(null != sortFilterEventInfoMap2, "(null != newInfo)");
							if (sortFilterEventInfoMap == null)
							{
								sortFilterEventInfoMap = sortFilterEventInfoMap2;
							}
							else
							{
								sortFilterEventInfoMap.Merge(sortFilterEventInfoMap2);
							}
						}
						while (!intermediateFormatReader.EOS);
						return sortFilterEventInfoMap;
					}
					return sortFilterEventInfoMap;
				}
				finally
				{
					if (stream != null)
					{
						stream.Close();
					}
				}
			}

			internal static List<Declaration> GetChunkDeclarations()
			{
				if (OnDemandProcessingManager.m_ChunkDeclarations == null)
				{
					List<Declaration> list = new List<Declaration>(21);
					list.Add(ScopeInstance.GetDeclaration());
					list.Add(ReportInstance.GetDeclaration());
					list.Add(DataSetInstance.GetDeclaration());
					list.Add(DataRegionInstance.GetDeclaration());
					list.Add(DataRegionMemberInstance.GetDeclaration());
					list.Add(DataCellInstance.GetDeclaration());
					list.Add(DataAggregateObjResult.GetDeclaration());
					list.Add(SubReportInstance.GetDeclaration());
					list.Add(GroupTreePartition.GetDeclaration());
					list.Add(ReportSnapshot.GetDeclaration());
					list.Add(ParametersImplWrapper.GetDeclaration());
					list.Add(ParameterImplWrapper.GetDeclaration());
					list.Add(SubReportInfo.GetDeclaration());
					list.Add(ParameterInfo.GetNewDeclaration());
					list.Add(ParameterInfoCollection.GetDeclaration());
					list.Add(ParameterBase.GetNewDeclaration());
					list.Add(ValidValue.GetNewDeclaration());
					list.Add(FieldInfo.GetDeclaration());
					list.Add(TreePartitionManager.GetDeclaration());
					list.Add(LookupObjResult.GetDeclaration());
					list.Add(DataCellInstanceList.GetDeclaration());
					return list;
				}
				return OnDemandProcessingManager.m_ChunkDeclarations;
			}

			internal static Stream OpenExistingDocumentMapStream(OnDemandMetadata odpMetadata, ICatalogItemContext reportContext, IChunkFactory chunkFactory)
			{
				Stream stream = null;
				if (!odpMetadata.ReportSnapshot.CanUseExistingDocumentMapChunk(reportContext))
				{
					return null;
				}
				string text = default(string);
				return chunkFactory.GetChunk("DocumentMap", AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Interactivity, ChunkMode.Open, out text);
			}
		}

		internal const string Definition = "CompiledDefinition";

		internal const string DocumentMap = "DocumentMap";

		internal const string ShowHideInfo = "ShowHideInfo";

		internal const string Bookmarks = "Bookmarks";

		internal const string Drillthrough = "Drillthrough";

		internal const string QuickFind = "QuickFind";

		internal const string SortFilterEventInfo = "SortFilterEventInfo";

		internal const string DataChunkPrefix = "DataChunk";

		internal const string GroupTree = "GroupTree";

		internal const string LookupInfo = "LookupInfo";

		internal const string Metadata = "Metadata";

		internal const string SharedDataSet = "SharedDataSet";

		internal const char Delimiter = 'x';

		internal static string GenerateDataChunkName(OnDemandProcessingContext context, int dataSetID, bool isInSubReport)
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.Append("DataChunk");
			stringBuilder.Append('x');
			if (isInSubReport)
			{
				stringBuilder.Append(context.SubReportUniqueName);
				stringBuilder.Append('x');
				stringBuilder.Append(context.SubReportDataChunkNameModifier);
				stringBuilder.Append('x');
			}
			stringBuilder.Append(dataSetID.ToString(CultureInfo.InvariantCulture));
			return stringBuilder.ToString();
		}

		internal static string GenerateLegacySharedSubReportDataChunkName(OnDemandProcessingContext context, int dataSetID)
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.Append("DataChunk");
			stringBuilder.Append('x');
			stringBuilder.Append(context.SubReportUniqueName);
			stringBuilder.Append('x');
			stringBuilder.Append(dataSetID.ToString(CultureInfo.InvariantCulture));
			return stringBuilder.ToString();
		}

		private static string GenerateDataChunkName(DataSetInstance dataSetInstance, OnDemandProcessingContext context)
		{
			string text = null;
			DataSet dataSetDef = dataSetInstance.DataSetDef;
			if (context.InSubreport)
			{
				return ChunkManager.GenerateDataChunkName(context, dataSetDef.ID, true);
			}
			return ChunkManager.GenerateDataChunkName(null, dataSetDef.ID, false);
		}

		internal static void SerializeReport(Report report, Stream stream, IConfiguration configuration)
		{
			int compatibilityVersion = ReportProcessingCompatibilityVersion.GetCompatibilityVersion(configuration);
			new IntermediateFormatWriter(stream, compatibilityVersion).Write(report);
		}

		internal static Report DeserializeReport(bool keepReferences, GlobalIDOwnerCollection globalIDOwnerCollection, IDOwner parentIDOwner, ReportItem parentReportItem, Stream stream)
		{
			IntermediateFormatReader intermediateFormatReader = new IntermediateFormatReader(stream, new ProcessingRIFObjectCreator(parentIDOwner, parentReportItem), globalIDOwnerCollection);
			Report report = (Report)intermediateFormatReader.ReadRIFObject();
			report.ReportOrDescendentHasUserSortFilter = report.HasUserSortFilter;
			if (!keepReferences)
			{
				intermediateFormatReader.ClearReferences();
			}
			return report;
		}
	}
}
