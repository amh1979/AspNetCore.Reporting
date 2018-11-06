using AspNetCore.ReportingServices.DataExtensions;
using AspNetCore.ReportingServices.DataProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class ProcessingDataReader : IProcessingDataReader, IDisposable
	{
		private RecordSetInfo m_recordSetInfo;

		private MappingDataReader m_dataSourceDataReader;

		private ChunkManager.DataChunkReader m_dataSnapshotReader;

		public RecordSetInfo RecordSetInfo
		{
			get
			{
				return this.m_recordSetInfo;
			}
		}

		public bool ReaderExtensionsSupported
		{
			get
			{
				if (this.m_dataSourceDataReader != null)
				{
					return this.m_dataSourceDataReader.ReaderExtensionsSupported;
				}
				return this.m_dataSnapshotReader.ReaderExtensionsSupported;
			}
		}

		public bool ReaderFieldProperties
		{
			get
			{
				if (this.m_dataSourceDataReader != null)
				{
					return this.m_dataSourceDataReader.ReaderFieldProperties;
				}
				return this.m_dataSnapshotReader.ReaderFieldProperties;
			}
		}

		public bool IsAggregateRow
		{
			get
			{
				if (this.m_dataSourceDataReader != null)
				{
					return this.m_dataSourceDataReader.IsAggregateRow;
				}
				return this.m_dataSnapshotReader.IsAggregateRow;
			}
		}

		public int AggregationFieldCount
		{
			get
			{
				if (this.m_dataSourceDataReader != null)
				{
					return this.m_dataSourceDataReader.AggregationFieldCount;
				}
				return this.m_dataSnapshotReader.AggregationFieldCount;
			}
		}

		internal ProcessingDataReader(OnDemandProcessingContext odpContext, DataSetInstance dataSetInstance, string dataSetName, IDataReader sourceReader, bool hasServerAggregateMetadata, string[] aliases, string[] names, DataSourceErrorInspector errorInspector)
		{
			this.m_recordSetInfo = new RecordSetInfo(hasServerAggregateMetadata, odpContext.IsSharedDataSetExecutionOnly, dataSetInstance, odpContext.ExecutionTime);
			this.m_dataSourceDataReader = new MappingDataReader(dataSetName, sourceReader, aliases, names, errorInspector);
		}

		internal ProcessingDataReader(DataSetInstance dataSetInstance, DataSet dataSet, OnDemandProcessingContext odpContext, bool overrideWithSharedDataSetChunkSettings)
		{
			if (odpContext.IsSharedDataSetExecutionOnly)
			{
				this.m_dataSnapshotReader = new ChunkManager.DataChunkReader(dataSetInstance, odpContext, odpContext.ExternalDataSetContext.CachedDataChunkName);
			}
			else
			{
				this.m_dataSnapshotReader = odpContext.GetDataChunkReader(dataSet.IndexInCollection);
			}
			this.m_recordSetInfo = this.m_dataSnapshotReader.RecordSetInfo;
			this.m_dataSnapshotReader.MoveToFirstRow();
			if (overrideWithSharedDataSetChunkSettings)
			{
				this.OverrideWithDataReaderSettings(odpContext, dataSetInstance, dataSet.DataSetCore);
			}
			else
			{
				this.OverrideDataCacheCompareOptions(ref odpContext);
			}
		}

		public void Dispose()
		{
			if (this.m_dataSourceDataReader != null)
			{
				((IDisposable)this.m_dataSourceDataReader).Dispose();
			}
			else
			{
				((IDisposable)this.m_dataSnapshotReader).Dispose();
			}
		}

		public void OverrideDataCacheCompareOptions(ref OnDemandProcessingContext context)
		{
			if (this.m_dataSnapshotReader != null)
			{
				if (!context.ProcessWithCachedData && !context.SnapshotProcessing)
				{
					return;
				}
				if (this.m_dataSnapshotReader.ValidCompareOptions)
				{
					context.ClrCompareOptions = this.m_dataSnapshotReader.CompareOptions;
				}
			}
		}

		public bool GetNextRow()
		{
			if (this.m_dataSourceDataReader != null)
			{
				return this.m_dataSourceDataReader.GetNextRow();
			}
			return this.m_dataSnapshotReader.GetNextRow();
		}

		public RecordRow GetUnderlyingRecordRowObject()
		{
			if (this.m_dataSnapshotReader != null)
			{
				return this.m_dataSnapshotReader.RecordRow;
			}
			return null;
		}

		public object GetColumn(int aliasIndex)
		{
			object obj = null;
			obj = ((this.m_dataSourceDataReader == null) ? this.m_dataSnapshotReader.GetFieldValue(aliasIndex) : this.m_dataSourceDataReader.GetFieldValue(aliasIndex));
			if (obj is DBNull)
			{
				return null;
			}
			return obj;
		}

		public bool IsAggregationField(int aliasIndex)
		{
			if (this.m_dataSourceDataReader != null)
			{
				return this.m_dataSourceDataReader.IsAggregationField(aliasIndex);
			}
			return this.m_dataSnapshotReader.IsAggregationField(aliasIndex);
		}

		public int GetPropertyCount(int aliasIndex)
		{
			if (this.m_dataSourceDataReader != null)
			{
				return this.m_dataSourceDataReader.GetPropertyCount(aliasIndex);
			}
			if (this.m_dataSnapshotReader != null && this.m_dataSnapshotReader.FieldPropertyNames != null && this.m_dataSnapshotReader.FieldPropertyNames[aliasIndex] != null)
			{
				List<string> propertyNames = this.m_dataSnapshotReader.FieldPropertyNames.GetPropertyNames(aliasIndex);
				if (propertyNames != null)
				{
					return propertyNames.Count;
				}
			}
			return 0;
		}

		public string GetPropertyName(int aliasIndex, int propertyIndex)
		{
			if (this.m_dataSourceDataReader != null)
			{
				return this.m_dataSourceDataReader.GetPropertyName(aliasIndex, propertyIndex);
			}
			if (this.m_dataSnapshotReader != null && this.m_dataSnapshotReader.FieldPropertyNames != null)
			{
				return this.m_dataSnapshotReader.FieldPropertyNames.GetPropertyName(aliasIndex, propertyIndex);
			}
			return null;
		}

		public object GetPropertyValue(int aliasIndex, int propertyIndex)
		{
			object obj = null;
			if (this.m_dataSourceDataReader != null)
			{
				obj = this.m_dataSourceDataReader.GetPropertyValue(aliasIndex, propertyIndex);
			}
			else if (this.m_dataSnapshotReader != null)
			{
				obj = this.m_dataSnapshotReader.GetPropertyValue(aliasIndex, propertyIndex);
			}
			if (obj is DBNull)
			{
				return null;
			}
			return obj;
		}

		public void OverrideWithDataReaderSettings(OnDemandProcessingContext odpContext, DataSetInstance dataSetInstance, DataSetCore dataSetCore)
		{
			ChunkManager.DataChunkReader.OverrideWithDataReaderSettings(this.m_recordSetInfo, odpContext, dataSetInstance, dataSetCore);
		}

		public void GetDataReaderMappingForRowConsumer(DataSetInstance dataSetInstance, out bool mappingIdentical, out int[] mappingDataSetFieldIndexesToDataChunk)
		{
			mappingIdentical = true;
			mappingDataSetFieldIndexesToDataChunk = null;
			ChunkManager.DataChunkReader.CreateDataChunkFieldMapping(dataSetInstance, this.m_recordSetInfo, false, out mappingIdentical, out mappingDataSetFieldIndexesToDataChunk);
		}
	}
}
