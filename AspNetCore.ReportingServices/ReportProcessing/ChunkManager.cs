using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections;
using System.Globalization;
using System.IO;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class ChunkManager
	{
		internal enum SpecialChunkName
		{
			DocumentMap,
			ShowHideInfo,
			Bookmark,
			QuickFind,
			SortFilterEventInfo
		}

		internal sealed class InstanceInfoOwnerList : ArrayList
		{
			internal new InstanceInfoOwner this[int index]
			{
				get
				{
					return (InstanceInfoOwner)base[index];
				}
			}
		}

		internal sealed class InstanceInfoList : ArrayList
		{
			internal new InstanceInfo this[int index]
			{
				get
				{
					return (InstanceInfo)base[index];
				}
			}
		}

		internal sealed class DataChunkWriter
		{
			internal sealed class RecordRowList : ArrayList
			{
				internal new RecordRow this[int index]
				{
					get
					{
						return (RecordRow)base[index];
					}
				}

				internal RecordRowList()
				{
				}

				internal RecordRowList(int capacity)
					: base(capacity)
				{
				}
			}

			private ReportProcessing.CreateReportChunk m_createChunkCallback;

			private IChunkFactory m_createChunkFactory;

			private string m_dataSetChunkName;

			private RecordSetInfo m_recordSetInfo;

			private bool m_recordSetPopulated;

			private RecordRowList m_recordRows;

			private Stream m_chunkStream;

			private IntermediateFormatWriter m_chunkWriter;

			private ReportProcessing.CreateReportChunk m_cacheDataCallback;

			private Stream m_cacheStream;

			private IntermediateFormatWriter m_cacheWriter;

			private bool m_stopSaveOnError;

			private bool m_errorOccurred;

			private Hashtable[] m_fieldAliasPropertyNames;

			internal Hashtable[] FieldAliasPropertyNames
			{
				set
				{
					this.m_fieldAliasPropertyNames = value;
				}
			}

			internal RecordSetInfo RecordSetInfo
			{
				get
				{
					return this.m_recordSetInfo;
				}
			}

			internal bool RecordSetInfoPopulated
			{
				get
				{
					return this.m_recordSetPopulated;
				}
				set
				{
					this.m_recordSetPopulated = value;
				}
			}

			internal DataChunkWriter(DataSet dataSet, ReportProcessing.ProcessingContext context, bool readerExtensionsSupported, bool stopSaveOnError)
			{
				Global.Tracer.Assert(null != context.CreateReportChunkCallback, "(null != context.CreateReportChunkCallback)");
				this.m_dataSetChunkName = ChunkManager.GenerateDataChunkName(dataSet, context, true);
				this.m_createChunkCallback = context.CreateReportChunkCallback;
				this.m_createChunkFactory = context.CreateReportChunkFactory;
				this.m_recordSetInfo = new RecordSetInfo(readerExtensionsSupported, dataSet.GetCLRCompareOptions());
				this.m_recordRows = new RecordRowList();
				this.m_cacheDataCallback = context.CacheDataCallback;
				this.m_stopSaveOnError = stopSaveOnError;
			}

			internal DataChunkWriter(DataSet dataSet, ReportProcessing.ProcessingContext context)
			{
				Global.Tracer.Assert(null != context.CreateReportChunkCallback, "(null != context.CreateReportChunkCallback)");
				this.m_dataSetChunkName = ChunkManager.GenerateDataChunkName(dataSet, context, false);
				this.m_createChunkCallback = context.CreateReportChunkCallback;
				this.m_createChunkFactory = context.CreateReportChunkFactory;
			}

			internal bool AddRecordRow(FieldsImpl fields, int fieldCount)
			{
				return this.AddRecordRow(new RecordRow(fields, fieldCount));
			}

			internal bool AddRecordRow(RecordRow aRow)
			{
				this.CheckChunkLimit();
				if (!this.m_errorOccurred || !this.m_stopSaveOnError)
				{
					this.m_recordRows.Add(aRow);
				}
				return !this.m_errorOccurred;
			}

			internal bool FinalFlush()
			{
				this.Flush();
				if (!this.m_errorOccurred || !this.m_stopSaveOnError)
				{
					this.Close();
				}
				return !this.m_errorOccurred;
			}

			internal void Close()
			{
				if (this.m_chunkWriter != null)
				{
					this.m_chunkWriter = null;
				}
				if (this.m_chunkStream != null)
				{
					this.m_chunkStream.Close();
					this.m_chunkStream = null;
				}
				if (this.m_cacheWriter != null)
				{
					this.m_cacheWriter = null;
				}
				if (this.m_cacheStream != null)
				{
					this.m_cacheStream.Close();
					this.m_cacheStream = null;
				}
			}

			internal void CloseAndEraseChunk()
			{
				if (this.m_chunkWriter != null)
				{
					this.m_chunkWriter = null;
				}
				if (this.m_cacheWriter != null)
				{
					this.m_cacheWriter = null;
				}
				if (this.m_cacheStream != null)
				{
					this.m_cacheStream.Close();
					this.m_cacheStream = null;
				}
				if (this.m_createChunkFactory != null)
				{
					try
					{
						if (this.m_chunkStream != null)
						{
							this.m_chunkStream.Close();
							this.m_chunkStream = null;
						}
						this.m_createChunkFactory.Erase(this.m_dataSetChunkName, ReportProcessing.ReportChunkTypes.Other);
					}
					catch
					{
					}
				}
			}

			private void CheckChunkLimit()
			{
				if (this.m_recordRows.Count >= 4096)
				{
					this.Flush();
					if (this.m_errorOccurred && this.m_stopSaveOnError)
					{
						return;
					}
					this.m_recordRows = new RecordRowList();
				}
			}

			private void Flush()
			{
				if (this.m_recordRows != null)
				{
					if (this.m_createChunkCallback == null && this.m_cacheDataCallback == null)
					{
						return;
					}
					try
					{
						if (this.m_fieldAliasPropertyNames != null && !this.m_recordSetPopulated)
						{
							this.m_recordSetInfo.FieldPropertyNames = new RecordSetPropertyNamesList(this.m_fieldAliasPropertyNames.Length);
							for (int i = 0; i < this.m_fieldAliasPropertyNames.Length; i++)
							{
								RecordSetPropertyNames recordSetPropertyNames = null;
								if (this.m_fieldAliasPropertyNames[i] != null && this.m_fieldAliasPropertyNames[i].Count != 0)
								{
									recordSetPropertyNames = new RecordSetPropertyNames();
									recordSetPropertyNames.PropertyNames = new StringList(this.m_fieldAliasPropertyNames[i].Count);
									recordSetPropertyNames.PropertyNames.AddRange(this.m_fieldAliasPropertyNames[i].Values);
								}
								this.m_recordSetInfo.FieldPropertyNames.Add(recordSetPropertyNames);
							}
						}
						if (this.m_chunkStream == null && this.m_createChunkCallback != null)
						{
							this.m_chunkStream = this.m_createChunkCallback(this.m_dataSetChunkName, ReportProcessing.ReportChunkTypes.Other, null);
							this.m_chunkWriter = new IntermediateFormatWriter(this.m_chunkStream, true);
							this.m_chunkWriter.WriteRecordSetInfo(this.m_recordSetInfo);
						}
						if (this.m_cacheStream == null && this.m_cacheDataCallback != null)
						{
							this.m_cacheStream = this.m_cacheDataCallback(this.m_dataSetChunkName, ReportProcessing.ReportChunkTypes.Other, null);
							this.m_cacheWriter = new IntermediateFormatWriter(this.m_cacheStream, true);
							this.m_cacheWriter.WriteRecordSetInfo(this.m_recordSetInfo);
						}
						Global.Tracer.Assert(this.m_chunkWriter != null || null != this.m_cacheWriter, "(null != m_chunkWriter || null != m_cacheWriter)");
						for (int j = 0; j < this.m_recordRows.Count; j++)
						{
							if (this.m_chunkWriter != null && !this.m_chunkWriter.WriteRecordRow(this.m_recordRows[j], this.m_recordSetInfo.FieldPropertyNames))
							{
								this.m_errorOccurred = true;
							}
							if (this.m_errorOccurred && this.m_stopSaveOnError)
							{
								this.CloseAndEraseChunk();
								break;
							}
							if (this.m_cacheWriter != null)
							{
								this.m_cacheWriter.WriteRecordRow(this.m_recordRows[j], this.m_recordSetInfo.FieldPropertyNames);
							}
						}
						this.m_recordRows = null;
					}
					catch
					{
						this.m_chunkWriter = null;
						if (this.m_chunkStream != null)
						{
							this.m_chunkStream.Close();
							this.m_chunkStream = null;
						}
						this.m_cacheWriter = null;
						if (this.m_cacheStream != null)
						{
							this.m_cacheStream.Close();
							this.m_cacheStream = null;
						}
						throw;
					}
				}
			}
		}

		internal sealed class DataChunkReader : IDisposable
		{
			private Stream m_chunkStream;

			private IntermediateFormatReader m_chunkReader;

			private RecordSetInfo m_recordSetInfo;

			private RecordRow m_recordRow;

			private int m_recordSetSize = -1;

			private int m_currentRow;

			private long m_streamLength = -1L;

			internal bool ReaderExtensionsSupported
			{
				get
				{
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

			public bool IsAggregateRow
			{
				get
				{
					return this.m_recordRow.IsAggregateRow;
				}
			}

			public int AggregationFieldCount
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

			internal DataChunkReader(DataSet dataSet, ReportProcessing.ProcessingContext context)
			{
				this.m_currentRow = -1;
				if (context.SubReportLevel == 0)
				{
					this.m_recordSetSize = dataSet.RecordSetSize;
				}
				Global.Tracer.Assert(null != context.GetReportChunkCallback, "(null != context.GetReportChunkCallback)");
				string text = default(string);
				this.m_chunkStream = context.GetReportChunkCallback(ChunkManager.GenerateDataChunkName(dataSet, context, false), ReportProcessing.ReportChunkTypes.Other, out text);
				this.m_chunkReader = new IntermediateFormatReader(this.m_chunkStream);
				this.m_recordSetInfo = this.m_chunkReader.ReadRecordSetInfo();
				if (-1 == this.m_recordSetSize)
				{
					this.m_streamLength = this.m_chunkStream.Length;
				}
			}

			internal bool GetNextRow()
			{
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
					this.m_currentRow++;
					this.ReadNextRow();
				}
				return flag;
			}

			internal object GetFieldValue(int aliasIndex)
			{
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
					VariantList fieldPropertyValues = this.m_recordRow.RecordFields[aliasIndex].FieldPropertyValues;
					if (fieldPropertyValues != null && propertyIndex >= 0 && propertyIndex < fieldPropertyValues.Count)
					{
						return ((ArrayList)fieldPropertyValues)[propertyIndex];
					}
				}
				return null;
			}

			void IDisposable.Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}

			private void Dispose(bool disposing)
			{
				if (disposing)
				{
					if (this.m_chunkReader != null)
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
				this.m_recordRow = this.m_chunkReader.ReadRecordRow();
			}
		}

		internal sealed class PageSectionManager
		{
			private InstanceInfoList m_pageSectionInstances;

			private InstanceInfoOwnerList m_pageSectionInstanceOwners;

			internal void AddPageSectionInstance(InstanceInfo instanceInfo, InstanceInfoOwner owner)
			{
				if (this.m_pageSectionInstances == null)
				{
					this.m_pageSectionInstances = new InstanceInfoList();
					this.m_pageSectionInstanceOwners = new InstanceInfoOwnerList();
				}
				this.m_pageSectionInstances.Add(instanceInfo);
				this.m_pageSectionInstanceOwners.Add(owner);
			}

			internal void Flush(ReportSnapshot reportSnapshot, ReportProcessing.CreateReportChunk createChunkCallback)
			{
				if (this.m_pageSectionInstances != null && createChunkCallback != null && reportSnapshot != null)
				{
					Stream stream = null;
					IntermediateFormatWriter intermediateFormatWriter = null;
					try
					{
						stream = createChunkCallback("PageSectionInstances", ReportProcessing.ReportChunkTypes.Other, null);
						intermediateFormatWriter = new IntermediateFormatWriter(stream, false);
						for (int i = 0; i < this.m_pageSectionInstances.Count; i++)
						{
							long position = stream.Position;
							intermediateFormatWriter.WriteInstanceInfo(this.m_pageSectionInstances[i]);
							this.m_pageSectionInstanceOwners[i].SetOffset(position);
						}
						stream.Close();
						bool[] declarationsToWrite = intermediateFormatWriter.DeclarationsToWrite;
						stream = createChunkCallback("PageSections", ReportProcessing.ReportChunkTypes.Main, null);
						intermediateFormatWriter = new IntermediateFormatWriter(stream, declarationsToWrite, null);
						reportSnapshot.PageSectionOffsets = intermediateFormatWriter.WritePageSections(stream, reportSnapshot.PageSections);
						Global.Tracer.Assert(2 * reportSnapshot.PageSectionOffsets.Count == reportSnapshot.PageSections.Count);
						reportSnapshot.PageSections = null;
						intermediateFormatWriter = null;
						stream.Close();
						stream = null;
					}
					finally
					{
						this.m_pageSectionInstances = null;
						this.m_pageSectionInstanceOwners = null;
						intermediateFormatWriter = null;
						if (stream != null)
						{
							stream.Close();
						}
					}
				}
			}
		}

		internal abstract class SnapshotChunkManager
		{
			protected ReportProcessing.CreateReportChunk m_createChunkCallback;

			protected InstanceInfoList m_firstPageChunkInstances;

			protected InstanceInfoOwnerList m_firstPageChunkInstanceOwners;

			protected InstanceInfoList m_chunkInstances;

			protected InstanceInfoOwnerList m_chunkInstanceOwners;

			protected IntermediateFormatWriter m_chunkWriter;

			protected Stream m_chunkStream;

			private bool[] m_firstPageDeclarationsToWrite;

			private bool[] m_otherPageDeclarationsToWrite;

			protected internal SnapshotChunkManager(ReportProcessing.CreateReportChunk createChunkCallback)
			{
				this.m_createChunkCallback = createChunkCallback;
				this.m_firstPageChunkInstances = new InstanceInfoList();
				this.m_firstPageChunkInstanceOwners = new InstanceInfoOwnerList();
			}

			protected void Flush()
			{
				if (this.m_chunkInstances != null && this.m_createChunkCallback != null)
				{
					try
					{
						if (this.m_chunkStream == null)
						{
							this.m_chunkStream = this.m_createChunkCallback("OtherPages", ReportProcessing.ReportChunkTypes.Other, null);
						}
						if (this.m_chunkWriter == null)
						{
							this.m_chunkWriter = new IntermediateFormatWriter(this.m_chunkStream, false);
						}
						for (int i = 0; i < this.m_chunkInstances.Count; i++)
						{
							long position = this.m_chunkStream.Position;
							this.m_chunkWriter.WriteInstanceInfo(this.m_chunkInstances[i]);
							this.m_chunkInstanceOwners[i].SetOffset(position);
						}
						this.m_chunkInstances = null;
						this.m_chunkInstanceOwners = null;
					}
					catch
					{
						this.m_chunkWriter = null;
						if (this.m_chunkStream != null)
						{
							this.m_chunkStream.Close();
							this.m_chunkStream = null;
						}
						throw;
					}
				}
			}

			internal void FinalFlush()
			{
				this.Flush();
				if (this.m_chunkWriter != null)
				{
					this.m_otherPageDeclarationsToWrite = this.m_chunkWriter.DeclarationsToWrite;
					this.m_chunkWriter = null;
				}
				if (this.m_chunkStream != null)
				{
					this.m_chunkStream.Close();
					this.m_chunkStream = null;
				}
			}

			internal void SaveFirstPage()
			{
				if (this.m_firstPageChunkInstances != null && this.m_firstPageChunkInstances.Count != 0 && this.m_createChunkCallback != null)
				{
					Stream stream = null;
					try
					{
						stream = this.m_createChunkCallback("FirstPage", ReportProcessing.ReportChunkTypes.Main, null);
						IntermediateFormatWriter intermediateFormatWriter = new IntermediateFormatWriter(stream, false);
						for (int i = 0; i < this.m_firstPageChunkInstances.Count; i++)
						{
							long position = stream.Position;
							intermediateFormatWriter.WriteInstanceInfo(this.m_firstPageChunkInstances[i]);
							Global.Tracer.Assert(0 != position, "(0 != offset)");
							position = -position;
							this.m_firstPageChunkInstanceOwners[i].SetOffset(position);
						}
						this.m_firstPageChunkInstances = null;
						this.m_firstPageChunkInstanceOwners = null;
						this.m_firstPageDeclarationsToWrite = intermediateFormatWriter.DeclarationsToWrite;
						intermediateFormatWriter = null;
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

			internal void SaveReportSnapshot(ReportSnapshot reportSnapshot)
			{
				if (this.m_createChunkCallback != null)
				{
					Stream stream = null;
					IntermediateFormatWriter intermediateFormatWriter = null;
					try
					{
						if (reportSnapshot.HasDocumentMap)
						{
							stream = this.m_createChunkCallback("DocumentMap", ReportProcessing.ReportChunkTypes.Main, null);
							intermediateFormatWriter = new IntermediateFormatWriter(stream, true);
							intermediateFormatWriter.WriteDocumentMapNode(reportSnapshot.DocumentMap);
							reportSnapshot.DocumentMap = null;
							intermediateFormatWriter = null;
							stream.Close();
							stream = null;
						}
						if (reportSnapshot.HasBookmarks)
						{
							stream = this.m_createChunkCallback("Bookmarks", ReportProcessing.ReportChunkTypes.Main, null);
							intermediateFormatWriter = new IntermediateFormatWriter(stream, true);
							intermediateFormatWriter.WriteBookmarksHashtable(reportSnapshot.BookmarksInfo);
							reportSnapshot.BookmarksInfo = null;
							intermediateFormatWriter = null;
							stream.Close();
							stream = null;
						}
						if (reportSnapshot.DrillthroughInfo != null)
						{
							stream = this.m_createChunkCallback("Drillthrough", ReportProcessing.ReportChunkTypes.Main, null);
							intermediateFormatWriter = new IntermediateFormatWriter(stream, true);
							intermediateFormatWriter.WriteDrillthroughInfo(reportSnapshot.DrillthroughInfo);
							reportSnapshot.DrillthroughInfo = null;
							intermediateFormatWriter = null;
							stream.Close();
							stream = null;
						}
						if (reportSnapshot.ShowHideSenderInfo != null || reportSnapshot.ShowHideReceiverInfo != null)
						{
							stream = this.m_createChunkCallback("ShowHideInfo", ReportProcessing.ReportChunkTypes.Main, null);
							intermediateFormatWriter = new IntermediateFormatWriter(stream, true);
							intermediateFormatWriter.WriteSenderInformationHashtable(reportSnapshot.ShowHideSenderInfo);
							reportSnapshot.ShowHideSenderInfo = null;
							intermediateFormatWriter.WriteReceiverInformationHashtable(reportSnapshot.ShowHideReceiverInfo);
							reportSnapshot.ShowHideReceiverInfo = null;
							intermediateFormatWriter = null;
							stream.Close();
							stream = null;
						}
						if (reportSnapshot.QuickFind != null)
						{
							stream = this.m_createChunkCallback("QuickFind", ReportProcessing.ReportChunkTypes.Main, null);
							intermediateFormatWriter = new IntermediateFormatWriter(stream, true);
							intermediateFormatWriter.WriteQuickFindHashtable(reportSnapshot.QuickFind);
							reportSnapshot.QuickFind = null;
							intermediateFormatWriter = null;
							stream.Close();
							stream = null;
						}
						if (reportSnapshot.SortFilterEventInfo != null)
						{
							stream = this.m_createChunkCallback("SortFilterEventInfo", ReportProcessing.ReportChunkTypes.Main, null);
							intermediateFormatWriter = new IntermediateFormatWriter(stream, true);
							intermediateFormatWriter.WriteSortFilterEventInfoHashtable(reportSnapshot.SortFilterEventInfo);
							reportSnapshot.SortFilterEventInfo = null;
							intermediateFormatWriter = null;
							stream.Close();
							stream = null;
						}
						stream = this.m_createChunkCallback("Main", ReportProcessing.ReportChunkTypes.Main, null);
						intermediateFormatWriter = new IntermediateFormatWriter(stream, this.m_firstPageDeclarationsToWrite, this.m_otherPageDeclarationsToWrite);
						intermediateFormatWriter.WriteReportSnapshot(reportSnapshot);
					}
					finally
					{
						intermediateFormatWriter = null;
						if (stream != null)
						{
							stream.Close();
						}
					}
				}
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
		}

		internal sealed class UpgradeManager : SnapshotChunkManager
		{
			internal UpgradeManager(ReportProcessing.CreateReportChunk createChunkCallback)
				: base(createChunkCallback)
			{
			}

			internal void AddInstance(InstanceInfo instanceInfo, InstanceInfoOwner owner, long offset)
			{
				Global.Tracer.Assert(0 != offset, "(0 != offset)");
				if (offset < 0)
				{
					base.m_firstPageChunkInstances.Add(instanceInfo);
					base.m_firstPageChunkInstanceOwners.Add(owner);
				}
				else
				{
					if (base.m_chunkInstances == null)
					{
						base.m_chunkInstances = new InstanceInfoList();
						base.m_chunkInstanceOwners = new InstanceInfoOwnerList();
					}
					base.m_chunkInstances.Add(instanceInfo);
					base.m_chunkInstanceOwners.Add(owner);
				}
			}
		}

		internal sealed class ProcessingChunkManager : SnapshotChunkManager
		{
			private bool m_inFirstPage = true;

			private bool m_hasLeafNode;

			private int m_ignorePageBreaks;

			private int m_ignoreInstances;

			private int m_reportItemCollectionLevel;

			private int m_instanceCount;

			private Hashtable m_repeatSiblingLists;

			private bool m_isOnePass;

			private PageSectionManager m_pageSectionManager;

			private long m_totalInstanceCount;

			internal bool InFirstPage
			{
				get
				{
					return this.m_inFirstPage;
				}
			}

			internal long TotalCount
			{
				get
				{
					return this.m_totalInstanceCount + this.m_instanceCount;
				}
			}

			internal ProcessingChunkManager(ReportProcessing.CreateReportChunk createChunkCallback, bool isOnePass)
				: base(createChunkCallback)
			{
				this.m_isOnePass = isOnePass;
				this.m_pageSectionManager = new PageSectionManager();
			}

			internal void PageSectionFlush(ReportSnapshot reportSnapshot)
			{
				this.m_pageSectionManager.Flush(reportSnapshot, base.m_createChunkCallback);
			}

			internal void EnterIgnorePageBreakItem()
			{
				if (!this.m_isOnePass)
				{
					this.m_ignorePageBreaks++;
				}
			}

			internal void LeaveIgnorePageBreakItem()
			{
				if (!this.m_isOnePass)
				{
					this.m_ignorePageBreaks--;
					Global.Tracer.Assert(0 <= this.m_ignorePageBreaks, "(0 <= m_ignorePageBreaks)");
				}
			}

			internal void EnterIgnoreInstances()
			{
				if (!this.m_isOnePass)
				{
					this.m_ignoreInstances++;
				}
			}

			internal void LeaveIgnoreInstances()
			{
				if (!this.m_isOnePass)
				{
					this.m_ignoreInstances--;
					Global.Tracer.Assert(0 <= this.m_ignoreInstances, "(0 <= m_ignoreInstances)");
				}
			}

			internal void EnterReportItemCollection()
			{
				if (!this.m_isOnePass)
				{
					this.m_reportItemCollectionLevel++;
				}
			}

			internal void LeaveReportItemCollection()
			{
				if (!this.m_isOnePass)
				{
					if (this.m_repeatSiblingLists != null)
					{
						this.m_repeatSiblingLists.Remove(this.m_reportItemCollectionLevel);
					}
					this.m_reportItemCollectionLevel--;
					Global.Tracer.Assert(0 <= this.m_reportItemCollectionLevel, "(0 <= m_reportItemCollectionLevel)");
				}
			}

			internal void AddRepeatSiblings(DataRegion dataRegion, int index)
			{
				if (!this.m_isOnePass && this.m_inFirstPage && dataRegion.RepeatSiblings != null)
				{
					Hashtable hashtable = null;
					for (int i = 0; i < dataRegion.RepeatSiblings.Count; i++)
					{
						int num = dataRegion.RepeatSiblings[i];
						if (index <= num)
						{
							if (hashtable == null)
							{
								if (this.m_repeatSiblingLists == null)
								{
									this.m_repeatSiblingLists = new Hashtable();
								}
								else
								{
									hashtable = (Hashtable)this.m_repeatSiblingLists[this.m_reportItemCollectionLevel];
								}
								if (hashtable == null)
								{
									hashtable = new Hashtable();
									this.m_repeatSiblingLists.Add(this.m_reportItemCollectionLevel, hashtable);
								}
							}
							hashtable.Add(num, true);
						}
					}
				}
			}

			internal void CheckPageBreak(IPageBreakItem item, bool atStart)
			{
				if (!this.m_isOnePass && this.m_inFirstPage && 0 >= this.m_ignorePageBreaks && base.m_createChunkCallback != null)
				{
					if (item.IgnorePageBreaks())
					{
						if (atStart)
						{
							this.EnterIgnorePageBreakItem();
							if (item is Rectangle && ((Rectangle)item).RepeatedSibling)
							{
								this.EnterIgnoreInstances();
							}
						}
						else
						{
							this.LeaveIgnorePageBreakItem();
							if (item is Rectangle && ((Rectangle)item).RepeatedSibling)
							{
								this.LeaveIgnoreInstances();
							}
						}
					}
					else
					{
						if (!atStart || !this.m_hasLeafNode || !item.HasPageBreaks(atStart))
						{
							if (atStart)
							{
								return;
							}
							if (!item.HasPageBreaks(atStart))
							{
								return;
							}
						}
						this.m_inFirstPage = false;
					}
				}
			}

			internal void AddInstance(InstanceInfo newInstance, ReportItem reportItemDef, InstanceInfoOwner owner, int index, bool isPageSection)
			{
				if (isPageSection)
				{
					this.m_pageSectionManager.AddPageSectionInstance(newInstance, owner);
				}
				else
				{
					if (!this.m_isOnePass && reportItemDef.RepeatedSibling && !this.m_inFirstPage)
					{
						Hashtable hashtable = null;
						if (this.m_repeatSiblingLists != null)
						{
							hashtable = (Hashtable)this.m_repeatSiblingLists[this.m_reportItemCollectionLevel];
						}
						if (hashtable != null && hashtable[index] != null)
						{
							this.SyncAddInstanceToFirstPage(newInstance, owner);
							return;
						}
					}
					this.AddInstance(newInstance, owner, isPageSection);
				}
			}

			internal void AddInstance(InstanceInfo newInstance, InstanceInfoOwner owner, bool isPageSection)
			{
				if (isPageSection)
				{
					this.m_pageSectionManager.AddPageSectionInstance(newInstance, owner);
				}
				else if (this.m_isOnePass)
				{
					lock (this)
					{
						this.SyncAddInstance(newInstance, owner);
					}
				}
				else
				{
					this.SyncAddInstance(newInstance, owner);
				}
			}

			internal void AddInstance(InstanceInfo newInstance, InstanceInfoOwner owner, bool addToFirstPage, bool isPageSection)
			{
				if (isPageSection)
				{
					this.m_pageSectionManager.AddPageSectionInstance(newInstance, owner);
				}
				else if (addToFirstPage)
				{
					this.AddInstanceToFirstPage(newInstance, owner, false);
				}
				else
				{
					this.AddInstance(newInstance, owner, false);
				}
			}

			internal void AddInstanceToFirstPage(InstanceInfo newInstance, InstanceInfoOwner owner, bool isPageSection)
			{
				if (isPageSection)
				{
					this.m_pageSectionManager.AddPageSectionInstance(newInstance, owner);
				}
				else if (this.m_isOnePass)
				{
					lock (this)
					{
						this.SyncAddInstanceToFirstPage(newInstance, owner);
					}
				}
				else
				{
					this.SyncAddInstanceToFirstPage(newInstance, owner);
				}
			}

			private void SyncAddInstanceToFirstPage(InstanceInfo newInstance, InstanceInfoOwner owner)
			{
				base.m_firstPageChunkInstances.Add(newInstance);
				base.m_firstPageChunkInstanceOwners.Add(owner);
			}

			private void SyncAddInstance(InstanceInfo newInstance, InstanceInfoOwner owner)
			{
				this.CheckChunkLimit();
				if (this.m_inFirstPage)
				{
					this.SetHasLeafNodes(owner);
					this.SyncAddInstanceToFirstPage(newInstance, owner);
				}
				else
				{
					base.m_chunkInstances.Add(newInstance);
					base.m_chunkInstanceOwners.Add(owner);
				}
				if (newInstance is OWCChartInstanceInfo)
				{
					this.m_instanceCount += ((OWCChartInstanceInfo)newInstance).Size;
				}
				else
				{
					this.m_instanceCount++;
				}
			}

			private void SetHasLeafNodes(InstanceInfoOwner owner)
			{
				if (!this.m_isOnePass && !this.m_hasLeafNode && 0 >= this.m_ignoreInstances && (owner is TextBoxInstance || owner is LineInstance || owner is CheckBoxInstance || owner is ImageInstance || owner is ActiveXControlInstance || owner is OWCChartInstance))
				{
					this.m_hasLeafNode = true;
				}
			}

			private void CheckChunkLimit()
			{
				if (base.m_createChunkCallback != null)
				{
					if (this.m_inFirstPage)
					{
						if (this.m_instanceCount < 4096)
						{
							return;
						}
						this.m_inFirstPage = false;
					}
					bool flag = false;
					if (base.m_chunkInstances == null)
					{
						flag = true;
					}
					else if (this.m_instanceCount >= 4096)
					{
						base.Flush();
						flag = true;
					}
					if (flag)
					{
						this.m_totalInstanceCount += this.m_instanceCount;
						this.m_instanceCount = 0;
						base.m_chunkInstances = new InstanceInfoList();
						base.m_chunkInstanceOwners = new InstanceInfoOwnerList();
					}
				}
			}
		}

		internal sealed class EventsChunkManager
		{
			private SpecialChunkManager m_specialChunkManager;

			internal EventsChunkManager(ReportProcessing.GetReportChunk getChunkCallback)
			{
				this.m_specialChunkManager = new SpecialChunkManager(getChunkCallback, null, null, null);
			}

			internal EventsChunkManager(ReportProcessing.GetReportChunk getChunkCallback, Hashtable definitionObjects, IntermediateFormatVersion intermediateFormatVersion)
			{
				this.m_specialChunkManager = new SpecialChunkManager(getChunkCallback, null, definitionObjects, intermediateFormatVersion);
			}

			internal void Close()
			{
				this.m_specialChunkManager.Close();
			}

			internal void GetShowHideInfo(out SenderInformationHashtable senderInfo, out ReceiverInformationHashtable receiverInfo)
			{
				senderInfo = null;
				receiverInfo = null;
				IntermediateFormatReader showHideInfoReader = this.m_specialChunkManager.ShowHideInfoReader;
				if (showHideInfoReader != null)
				{
					senderInfo = showHideInfoReader.ReadSenderInformationHashtable();
					receiverInfo = showHideInfoReader.ReadReceiverInformationHashtable();
				}
			}

			internal BookmarkInformation GetBookmarkIdInfo(string bookmarkId)
			{
				if (bookmarkId == null)
				{
					return null;
				}
				IntermediateFormatReader bookmarkReader = this.m_specialChunkManager.BookmarkReader;
				if (bookmarkReader != null)
				{
					return bookmarkReader.FindBookmarkIdInfo(bookmarkId);
				}
				return null;
			}

			internal DrillthroughInformation GetDrillthroughIdInfo(string drillthroughId)
			{
				if (drillthroughId == null)
				{
					return null;
				}
				IntermediateFormatReader drillthroughReader = this.m_specialChunkManager.DrillthroughReader;
				if (drillthroughReader != null)
				{
					return drillthroughReader.FindDrillthroughIdInfo(drillthroughId);
				}
				return null;
			}

			internal int GetDocumentMapNodePage(string documentMapId)
			{
				if (documentMapId == null)
				{
					return 0;
				}
				int result = 0;
				IntermediateFormatReader documentMapReader = this.m_specialChunkManager.DocumentMapReader;
				if (documentMapReader != null)
				{
					documentMapReader.FindDocumentMapNodePage(documentMapId, ref result);
				}
				return result;
			}

			internal DocumentMapNodeInfo GetDocumentMapInfo()
			{
				IntermediateFormatReader documentMapReader = this.m_specialChunkManager.DocumentMapReader;
				if (documentMapReader != null)
				{
					return documentMapReader.ReadDocumentMapNodeInfo();
				}
				return null;
			}

			internal DocumentMapNode GetDocumentMapNode()
			{
				IntermediateFormatReader documentMapReader = this.m_specialChunkManager.DocumentMapReader;
				if (documentMapReader != null)
				{
					return documentMapReader.ReadDocumentMapNode();
				}
				return null;
			}

			internal SortFilterEventInfoHashtable GetSortFilterEventInfo()
			{
				IntermediateFormatReader sortFilterEventInfoReader = this.m_specialChunkManager.SortFilterEventInfoReader;
				if (sortFilterEventInfoReader != null)
				{
					return sortFilterEventInfoReader.ReadSortFilterEventInfoHashtable();
				}
				return null;
			}
		}

		internal sealed class SpecialChunkManager
		{
			private ReportProcessing.GetReportChunk m_getChunkCallback;

			private Hashtable m_definitionObjects;

			private Hashtable m_instanceObjects;

			private IntermediateFormatVersion m_intermediateFormatVersion;

			private Stream m_docMap;

			private bool m_hasDocMap = true;

			private Stream m_showHideInfo;

			private bool m_hasShowHideInfo = true;

			private Stream m_bookmarks;

			private bool m_hasBookmarks = true;

			private Stream m_drillthrough;

			private bool m_hasDrillthrough = true;

			private Stream m_quickFind;

			private bool m_hasQuickFind = true;

			private Stream m_sortFilterEventInfo;

			private bool m_hasSortFilterEventInfo = true;

			internal IntermediateFormatReader DocumentMapReader
			{
				get
				{
					IntermediateFormatReader result = null;
					if (this.m_hasDocMap && this.m_docMap == null)
					{
						this.m_docMap = this.GetSpecialChunkInfo("DocumentMap", ref this.m_hasDocMap);
					}
					if (this.m_docMap != null)
					{
						this.m_docMap.Position = 0L;
						result = new IntermediateFormatReader(this.m_docMap, this.m_instanceObjects);
					}
					return result;
				}
			}

			internal IntermediateFormatReader ShowHideInfoReader
			{
				get
				{
					IntermediateFormatReader result = null;
					if (this.m_hasShowHideInfo && this.m_showHideInfo == null)
					{
						this.m_showHideInfo = this.GetSpecialChunkInfo("ShowHideInfo", ref this.m_hasShowHideInfo);
					}
					if (this.m_showHideInfo != null)
					{
						this.m_showHideInfo.Position = 0L;
						result = new IntermediateFormatReader(this.m_showHideInfo, this.m_instanceObjects);
					}
					return result;
				}
			}

			internal IntermediateFormatReader BookmarkReader
			{
				get
				{
					IntermediateFormatReader result = null;
					if (this.m_hasBookmarks && this.m_bookmarks == null)
					{
						this.m_bookmarks = this.GetSpecialChunkInfo("Bookmarks", ref this.m_hasBookmarks);
					}
					if (this.m_bookmarks != null)
					{
						this.m_bookmarks.Position = 0L;
						result = new IntermediateFormatReader(this.m_bookmarks, this.m_instanceObjects);
					}
					return result;
				}
			}

			internal IntermediateFormatReader DrillthroughReader
			{
				get
				{
					IntermediateFormatReader result = null;
					if (this.m_hasDrillthrough && this.m_drillthrough == null)
					{
						this.m_drillthrough = this.GetSpecialChunkInfo("Drillthrough", ref this.m_hasDrillthrough);
					}
					if (this.m_drillthrough != null)
					{
						this.m_drillthrough.Position = 0L;
						result = new IntermediateFormatReader(this.m_drillthrough, this.m_instanceObjects);
					}
					return result;
				}
			}

			internal IntermediateFormatReader QuickFindReader
			{
				get
				{
					IntermediateFormatReader result = null;
					if (this.m_hasQuickFind && this.m_quickFind == null)
					{
						this.m_quickFind = this.GetSpecialChunkInfo("QuickFind", ref this.m_hasQuickFind);
					}
					if (this.m_quickFind != null)
					{
						this.m_quickFind.Position = 0L;
						result = new IntermediateFormatReader(this.m_quickFind, this.m_instanceObjects);
					}
					return result;
				}
			}

			internal IntermediateFormatReader SortFilterEventInfoReader
			{
				get
				{
					IntermediateFormatReader result = null;
					if (this.m_hasSortFilterEventInfo && this.m_sortFilterEventInfo == null)
					{
						this.m_sortFilterEventInfo = this.GetSpecialChunkInfo("SortFilterEventInfo", ref this.m_hasSortFilterEventInfo);
					}
					if (this.m_sortFilterEventInfo != null)
					{
						this.m_sortFilterEventInfo.Position = 0L;
						result = new IntermediateFormatReader(this.m_sortFilterEventInfo, this.m_instanceObjects, this.m_definitionObjects, this.m_intermediateFormatVersion);
					}
					return result;
				}
			}

			internal SpecialChunkManager(ReportProcessing.GetReportChunk getChunkCallback, Hashtable instanceObjects, Hashtable definitionObjects, IntermediateFormatVersion intermediateFormatVersion)
			{
				this.m_definitionObjects = definitionObjects;
				this.m_instanceObjects = instanceObjects;
				this.m_getChunkCallback = getChunkCallback;
				this.m_intermediateFormatVersion = intermediateFormatVersion;
			}

			private Stream GetSpecialChunkInfo(string chunkName, ref bool hasChunk)
			{
				string text = default(string);
				Stream stream = this.m_getChunkCallback(chunkName, ReportProcessing.ReportChunkTypes.Main, out text);
				if (stream == null)
				{
					hasChunk = false;
				}
				return stream;
			}

			internal void Close()
			{
				if (this.m_docMap != null)
				{
					this.m_docMap.Close();
					this.m_docMap = null;
					this.m_hasDocMap = true;
				}
				if (this.m_showHideInfo != null)
				{
					this.m_showHideInfo.Close();
					this.m_showHideInfo = null;
					this.m_hasShowHideInfo = true;
				}
				if (this.m_bookmarks != null)
				{
					this.m_bookmarks.Close();
					this.m_bookmarks = null;
					this.m_hasBookmarks = true;
				}
				if (this.m_drillthrough != null)
				{
					this.m_drillthrough.Close();
					this.m_drillthrough = null;
					this.m_hasDrillthrough = true;
				}
				if (this.m_quickFind != null)
				{
					this.m_quickFind.Close();
					this.m_quickFind = null;
					this.m_hasQuickFind = true;
				}
				if (this.m_sortFilterEventInfo != null)
				{
					this.m_sortFilterEventInfo.Close();
					this.m_sortFilterEventInfo = null;
					this.m_hasSortFilterEventInfo = true;
				}
			}
		}

		internal sealed class RenderingChunkManager
		{
			private ReportProcessing.GetReportChunk m_getChunkCallback;

			private SpecialChunkManager m_specialChunkManager;

			private Stream m_firstPageChunk;

			private IntermediateFormatReader m_firstPageReader;

			private Stream m_otherPageChunk;

			private IntermediateFormatReader m_otherPageReader;

			private Hashtable m_instanceObjects;

			private Hashtable m_definitionObjects;

			private IntermediateFormatReader.State m_declarationsRead;

			private IntermediateFormatVersion m_intermediateFormatVersion;

			private Stream m_specialChunk;

			private IntermediateFormatReader m_specialChunkReader;

			private Stream m_pageSectionChunk;

			private IntermediateFormatReader m_pageSectionReader;

			private Stream m_pageSectionInstanceChunk;

			private IntermediateFormatReader m_pageSectionInstanceReader;

			private IntermediateFormatVersion m_pageSectionIntermediateFormatVersion;

			private IntermediateFormatReader.State m_pageSectionDeclarationsRead;

			private int m_pageSectionLastReadPage = -1;

			internal RenderingChunkManager(ReportProcessing.GetReportChunk getChunkCallback, Hashtable instanceObjects, Hashtable definitionObjects, IntermediateFormatReader.State declarationsRead, IntermediateFormatVersion intermediateFormatVersion)
			{
				this.m_getChunkCallback = getChunkCallback;
				this.m_instanceObjects = instanceObjects;
				this.m_definitionObjects = definitionObjects;
				this.m_declarationsRead = declarationsRead;
				this.m_intermediateFormatVersion = intermediateFormatVersion;
				if (intermediateFormatVersion != null && intermediateFormatVersion.IsRS2005_WithPHFChunks)
				{
					this.m_pageSectionIntermediateFormatVersion = intermediateFormatVersion;
				}
				else
				{
					this.m_pageSectionIntermediateFormatVersion = new IntermediateFormatVersion();
				}
			}

			internal IntermediateFormatReader GetReaderForSpecialChunk(long offset)
			{
				if (this.m_specialChunkReader == null)
				{
					if (this.m_specialChunk == null)
					{
						string text = default(string);
						this.m_specialChunk = this.m_getChunkCallback("Special", ReportProcessing.ReportChunkTypes.Main, out text);
					}
					this.m_specialChunkReader = new IntermediateFormatReader(this.m_specialChunk, this.m_instanceObjects, this.m_intermediateFormatVersion);
				}
				this.m_specialChunk.Position = offset;
				return this.m_specialChunkReader;
			}

			internal IntermediateFormatReader GetSpecialChunkReader(SpecialChunkName chunkName)
			{
				if (this.m_specialChunkManager == null)
				{
					this.m_specialChunkManager = new SpecialChunkManager(this.m_getChunkCallback, this.m_instanceObjects, this.m_definitionObjects, this.m_intermediateFormatVersion);
				}
				switch (chunkName)
				{
				case SpecialChunkName.DocumentMap:
					return this.m_specialChunkManager.DocumentMapReader;
				case SpecialChunkName.Bookmark:
					return this.m_specialChunkManager.BookmarkReader;
				case SpecialChunkName.ShowHideInfo:
					return this.m_specialChunkManager.ShowHideInfoReader;
				case SpecialChunkName.QuickFind:
					return this.m_specialChunkManager.QuickFindReader;
				case SpecialChunkName.SortFilterEventInfo:
					return this.m_specialChunkManager.SortFilterEventInfoReader;
				default:
					return null;
				}
			}

			internal bool PageSectionChunkExists()
			{
				string text = default(string);
				this.m_pageSectionChunk = this.m_getChunkCallback("PageSections", ReportProcessing.ReportChunkTypes.Main, out text);
				if (this.m_pageSectionChunk != null)
				{
					this.m_pageSectionChunk.Close();
					this.m_pageSectionChunk = null;
					return true;
				}
				return false;
			}

			internal IntermediateFormatReader GetPageSectionReader(int requestedPageNumber, out int currentPageNumber)
			{
				currentPageNumber = this.m_pageSectionLastReadPage + 1;
				bool flag = null == this.m_pageSectionReader;
				if (this.m_pageSectionChunk == null)
				{
					string text = default(string);
					this.m_pageSectionChunk = this.m_getChunkCallback("PageSections", ReportProcessing.ReportChunkTypes.Main, out text);
				}
				else if (requestedPageNumber < 0 || requestedPageNumber <= this.m_pageSectionLastReadPage)
				{
					this.m_pageSectionChunk.Position = 0L;
					currentPageNumber = 0;
					flag = true;
				}
				if (flag)
				{
					this.m_pageSectionReader = new IntermediateFormatReader(this.m_pageSectionChunk, this.m_pageSectionIntermediateFormatVersion);
				}
				return this.m_pageSectionReader;
			}

			internal void SetPageSectionReaderState(IntermediateFormatReader.State declarations, int pageSectionLastReadPage)
			{
				this.m_pageSectionDeclarationsRead = declarations;
				this.m_pageSectionLastReadPage = pageSectionLastReadPage;
			}

			internal IntermediateFormatReader GetPageSectionInstanceReader(long offset)
			{
				if (this.m_pageSectionInstanceReader == null)
				{
					if (this.m_pageSectionInstanceChunk == null)
					{
						string text = default(string);
						this.m_pageSectionInstanceChunk = this.m_getChunkCallback("PageSectionInstances", ReportProcessing.ReportChunkTypes.Other, out text);
					}
					if (this.m_pageSectionInstanceChunk != null)
					{
						this.m_pageSectionInstanceReader = new IntermediateFormatReader(this.m_pageSectionInstanceChunk, this.m_pageSectionDeclarationsRead, this.m_pageSectionIntermediateFormatVersion);
					}
				}
				if (this.m_pageSectionInstanceChunk != null)
				{
					this.m_pageSectionInstanceChunk.Position = offset;
				}
				return this.m_pageSectionInstanceReader;
			}

			internal IntermediateFormatReader GetReader(long offset)
			{
				if (offset < 0)
				{
					if (this.m_firstPageReader == null)
					{
						if (this.m_firstPageChunk == null)
						{
							string text = default(string);
							this.m_firstPageChunk = this.m_getChunkCallback("FirstPage", ReportProcessing.ReportChunkTypes.Main, out text);
						}
						this.m_firstPageReader = new IntermediateFormatReader(this.m_firstPageChunk, this.m_declarationsRead, this.m_definitionObjects, this.m_intermediateFormatVersion);
					}
					this.m_firstPageChunk.Position = -offset;
					return this.m_firstPageReader;
				}
				if (this.m_otherPageReader == null)
				{
					if (this.m_otherPageChunk == null)
					{
						string text2 = default(string);
						this.m_otherPageChunk = this.m_getChunkCallback("OtherPages", ReportProcessing.ReportChunkTypes.Other, out text2);
					}
					this.m_otherPageReader = new IntermediateFormatReader(this.m_otherPageChunk, this.m_declarationsRead, this.m_definitionObjects, this.m_intermediateFormatVersion);
				}
				this.m_otherPageChunk.Position = offset;
				return this.m_otherPageReader;
			}

			internal void Close()
			{
				this.m_firstPageReader = null;
				if (this.m_firstPageChunk != null)
				{
					this.m_firstPageChunk.Close();
					this.m_firstPageChunk = null;
				}
				this.m_otherPageReader = null;
				if (this.m_otherPageChunk != null)
				{
					this.m_otherPageChunk.Close();
					this.m_otherPageChunk = null;
				}
				if (this.m_specialChunkManager != null)
				{
					this.m_specialChunkManager.Close();
					this.m_specialChunkManager = null;
				}
				this.m_specialChunkReader = null;
				if (this.m_specialChunk != null)
				{
					this.m_specialChunk.Close();
					this.m_specialChunk = null;
				}
				this.m_pageSectionReader = null;
				if (this.m_pageSectionChunk != null)
				{
					this.m_pageSectionChunk.Close();
					this.m_pageSectionChunk = null;
				}
				this.m_pageSectionInstanceReader = null;
				if (this.m_pageSectionInstanceChunk != null)
				{
					this.m_pageSectionInstanceChunk.Close();
					this.m_pageSectionInstanceChunk = null;
				}
			}
		}

		internal const string Definition = "CompiledDefinition";

		internal const string MainChunk = "Main";

		internal const string FirstPageChunk = "FirstPage";

		internal const string OtherPageChunk = "OtherPages";

		internal const string SpecialChunk = "Special";

		internal const string DocumentMap = "DocumentMap";

		internal const string ShowHideInfo = "ShowHideInfo";

		internal const string Bookmarks = "Bookmarks";

		internal const string Drillthrough = "Drillthrough";

		internal const string QuickFind = "QuickFind";

		internal const string SortFilterEventInfo = "SortFilterEventInfo";

		internal const string DataChunkPrefix = "DataChunk";

		internal const string PageSections = "PageSections";

		internal const string PageSectionInstances = "PageSectionInstances";

		internal const string Delimiter = "_";

		private const int InstancePerChunk = 4096;

		private const int RecordRowPerChunk = 4096;

		private static string GenerateDataChunkName(string dataSetName, string subReportName, bool isShareable, int reportUniqueName)
		{
			if (-1 == reportUniqueName)
			{
				return "DataChunk_" + dataSetName;
			}
			if (isShareable && subReportName != null)
			{
				return "DataChunk" + subReportName + "_" + dataSetName;
			}
			return "DataChunk" + reportUniqueName + "_" + dataSetName;
		}

		private static string GenerateDataChunkName(DataSet dataSet, ReportProcessing.ProcessingContext context, bool writeOperation)
		{
			string text = null;
			string subReportName = null;
			if (context.SubReportLevel != 0)
			{
				subReportName = context.ReportContext.StableItemPath;
			}
			if (dataSet.IsShareable())
			{
				text = (context.CachedDataChunkMapping[dataSet.ID] as string);
				if (text == null)
				{
					text = ChunkManager.GenerateDataChunkName(dataSet.Name, subReportName, true, context.DataSetUniqueName);
					if (writeOperation)
					{
						context.CachedDataChunkMapping.Add(dataSet.ID, text);
					}
				}
			}
			else
			{
				text = ChunkManager.GenerateDataChunkName(dataSet.Name, subReportName, false, context.DataSetUniqueName);
			}
			return text;
		}
	}
}
