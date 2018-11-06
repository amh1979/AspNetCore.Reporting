using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.DataExtensions;
using AspNetCore.ReportingServices.DataProcessing;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal abstract class RuntimeDataSet : RuntimeLiveQueryExecutor
	{
		protected DataSetInstance m_dataSetInstance;

		protected IProcessingDataReader m_dataReader;

		protected int m_dataRowsRead;

		private bool m_allDataRowsRead;

		private readonly bool m_processRetrievedData = true;

		private readonly DataSetQueryRestartPosition m_restartPosition;

		internal virtual bool ProcessFromLiveDataReader
		{
			get
			{
				return false;
			}
		}

		internal bool NoRows
		{
			get
			{
				return this.m_dataRowsRead <= 0;
			}
		}

		internal int NumRowsRead
		{
			get
			{
				return this.m_dataRowsRead;
			}
		}

		internal bool UsedOnlyInParameters
		{
			get
			{
				return base.m_dataSet.UsedOnlyInParameters;
			}
		}

		protected virtual bool WritesDataChunk
		{
			get
			{
				return false;
			}
		}

		protected bool ProcessRetrievedData
		{
			get
			{
				return this.m_processRetrievedData;
			}
		}

		protected bool HasServerAggregateMetadata
		{
			get
			{
				if (!base.m_dataSet.HasAggregateIndicatorFields)
				{
					if (this.m_dataReader != null)
					{
						return this.m_dataReader.ReaderExtensionsSupported;
					}
					return false;
				}
				return true;
			}
		}

		protected virtual bool ShouldCancelCommandDuringCleanup
		{
			get
			{
				return false;
			}
		}

		internal RuntimeDataSet(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource dataSource, AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet, DataSetInstance dataSetInstance, OnDemandProcessingContext odpContext, bool processRetrievedData)
			: base(dataSource, dataSet, odpContext)
		{
			this.m_dataSetInstance = dataSetInstance;
			this.m_processRetrievedData = processRetrievedData;
			if (base.m_odpContext.QueryRestartInfo == null)
			{
				this.m_restartPosition = null;
			}
			else
			{
				this.m_restartPosition = base.m_odpContext.QueryRestartInfo.GetRestartPositionForDataSet(base.m_dataSet);
			}
		}

		internal void InitProcessingParams(IDbConnection conn, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.TransactionInfo transInfo)
		{
			base.m_dataSourceConnection = conn;
			base.m_transInfo = transInfo;
		}

		protected virtual void InitializeDataSet()
		{
			base.m_odpContext.EnsureCultureIsSetOnCurrentThread();
			if (DataSetValidator.LOCALE_SYSTEM_DEFAULT == base.m_dataSet.LCID)
			{
				if (base.m_odpContext.ShouldExecuteLiveQueries)
				{
					base.m_dataSet.LCID = DataSetValidator.LCIDfromRDLCollation(base.m_dataSet.Collation);
				}
				else
				{
					base.m_dataSet.LCID = this.m_dataSetInstance.LCID;
				}
			}
			base.m_isConnectionOwner = false;
			this.InitRuntime();
		}

		private void InitRuntime()
		{
			Global.Tracer.Assert(base.m_odpContext.ReportObjectModel != null && base.m_odpContext.ReportRuntime != null);
			if (base.m_odpContext.ReportRuntime.ReportExprHost != null)
			{
				base.m_dataSet.SetExprHost(base.m_odpContext.ReportRuntime.ReportExprHost, base.m_odpContext.ReportObjectModel);
			}
		}

		protected virtual void TeardownDataSet()
		{
			base.m_odpContext.CheckAndThrowIfAborted();
			if (this.NoRows)
			{
				base.m_dataSet.MarkDataRegionsAsNoRows();
			}
		}

		protected virtual void FinalCleanup()
		{
			this.CleanupDataReader();
			base.CloseConnection();
			if (base.m_odpContext.ExecutionLogContext != null)
			{
				base.m_odpContext.ExecutionLogContext.AddDataSetMetrics(base.m_dataSet.Name, base.m_executionMetrics);
			}
		}

		protected virtual void CleanupForException()
		{
			if (base.m_transInfo != null)
			{
				base.m_transInfo.RollbackRequired = true;
			}
		}

		protected virtual void CleanupDataReader()
		{
			base.m_executionMetrics.AddRowCount(this.m_dataRowsRead);
			if (base.m_odpContext.DataSetRetrievalComplete != null)
			{
				base.m_odpContext.DataSetRetrievalComplete[base.m_dataSet.IndexInCollection] = true;
			}
			if (base.m_dataSet.IsReferenceToSharedDataSet)
			{
				this.m_dataSetInstance.RecordSetSize = this.NumRowsRead;
			}
			this.CleanupCommandAndDataReader();
		}

		protected abstract void InitializeBeforeProcessingRows(bool aReaderExtensionsSupported);

		protected void PopulateFieldsWithReaderFlags()
		{
			if (this.m_dataReader != null)
			{
				base.m_odpContext.ReportObjectModel.FieldsImpl.ReaderExtensionsSupported = this.HasServerAggregateMetadata;
				base.m_odpContext.ReportObjectModel.FieldsImpl.ReaderFieldProperties = this.m_dataReader.ReaderFieldProperties;
			}
		}

		protected virtual void CleanupProcess()
		{
			if (!base.m_dataSet.IsReferenceToSharedDataSet)
			{
				this.CleanupCommandAndDataReader();
			}
		}

		private void CleanupCommandAndDataReader()
		{
			if (base.m_dataSet.IsReferenceToSharedDataSet && this.ProcessFromLiveDataReader)
			{
				return;
			}
			try
			{
				if (this.ShouldCancelCommandDuringCleanup && !this.m_allDataRowsRead)
				{
					base.CancelCommand();
				}
				this.DisposeDataReader();
			}
			finally
			{
				base.DisposeCommand();
			}
		}

		private void DisposeDataReader()
		{
			base.DisposeDataExtensionObject<IProcessingDataReader>(ref this.m_dataReader, "data reader", (DataProcessingMetrics.MetricType?)DataProcessingMetrics.MetricType.DisposeDataReader);
		}

		protected abstract void ProcessExtendedPropertyMappings();

		protected virtual void InitializeBeforeFirstRow(bool hasRows)
		{
			if (hasRows && !base.m_dataSet.IsReferenceToSharedDataSet)
			{
				this.MapExtendedProperties();
				this.ProcessExtendedPropertyMappings();
			}
		}

		protected AspNetCore.ReportingServices.ReportIntermediateFormat.RecordRow ReadOneRow(out int rowIndex)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.RecordRow result = null;
			rowIndex = -1;
			if (this.m_allDataRowsRead)
			{
				return result;
			}
			do
			{
				bool flag = this.m_dataReader != null && this.m_dataReader.GetNextRow();
				if (this.m_dataRowsRead == 0)
				{
					this.InitializeBeforeFirstRow(flag);
				}
				if (flag)
				{
					base.m_odpContext.CheckAndThrowIfAborted();
					result = this.ReadRow();
					rowIndex = this.m_dataRowsRead;
					this.IncrementRowCounterAndTrace();
				}
				else
				{
					result = null;
					this.m_allDataRowsRead = true;
				}
			}
			while (!this.m_allDataRowsRead && this.m_restartPosition != null && this.m_restartPosition.ShouldSkip(base.m_odpContext, result));
			if (this.m_restartPosition != null)
			{
				this.m_restartPosition.DisableRowSkipping(result);
			}
			return result;
		}

		protected void IncrementRowCounterAndTrace()
		{
			this.m_dataRowsRead++;
			if (Global.Tracer.TraceVerbose && this.m_dataRowsRead % 100000 == 0)
			{
				Global.Tracer.Trace(TraceLevel.Verbose, "Read data row: {0}", this.m_dataRowsRead);
			}
		}

		private void MapExtendedProperties()
		{
			if (this.m_dataReader.ReaderFieldProperties)
			{
				int count = base.m_dataSet.Fields.Count;
				for (int i = 0; i < count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.Field field = base.m_dataSet.Fields[i];
					if (!field.IsCalculatedField)
					{
						try
						{
							int propertyCount = this.m_dataReader.GetPropertyCount(i);
							List<int> list = new List<int>();
							List<string> list2 = new List<string>();
							for (int j = 0; j < propertyCount; j++)
							{
								string text = null;
								try
								{
									text = this.m_dataReader.GetPropertyName(i, j);
									list.Add(j);
									list2.Add(text);
								}
								catch (ReportProcessingException_FieldError reportProcessingException_FieldError)
								{
									base.m_odpContext.ErrorContext.Register(ProcessingErrorCode.rsErrorReadingFieldProperty, Severity.Warning, AspNetCore.ReportingServices.ReportProcessing.ObjectType.DataSet, base.m_dataSet.Name, "FieldExtendedProperty", field.Name.MarkAsModelInfo(), text.MarkAsModelInfo(), reportProcessingException_FieldError.Message);
								}
							}
							if (list.Count > 0)
							{
								if (this.m_dataSetInstance.FieldInfos == null)
								{
									this.m_dataSetInstance.FieldInfos = new FieldInfo[count];
								}
								this.m_dataSetInstance.FieldInfos[i] = new FieldInfo(list, list2);
							}
						}
						catch (ReportProcessingException_FieldError aException)
						{
							this.HandleFieldError(aException, i, field.Name);
						}
					}
				}
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.RecordRow ReadRow()
		{
			int count = base.m_dataSet.Fields.Count;
			AspNetCore.ReportingServices.ReportIntermediateFormat.RecordRow underlyingRecordRowObject = this.m_dataReader.GetUnderlyingRecordRowObject();
			if (underlyingRecordRowObject != null)
			{
				return underlyingRecordRowObject;
			}
			base.m_executionMetrics.StartTotalTimer();
			underlyingRecordRowObject = this.ConstructRecordRow();
			base.m_executionMetrics.RecordTotalTimerMeasurement();
			return underlyingRecordRowObject;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.RecordRow ConstructRecordRow()
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.RecordRow recordRow = new AspNetCore.ReportingServices.ReportIntermediateFormat.RecordRow();
			bool flag = this.m_dataReader.ReaderExtensionsSupported && !base.m_dataSet.HasAggregateIndicatorFields;
			bool flag2 = this.HasServerAggregateMetadata && (base.m_dataSet.InterpretSubtotalsAsDetails == AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet.TriState.False || (base.m_odpContext.IsSharedDataSetExecutionOnly && base.m_dataSet.InterpretSubtotalsAsDetails == AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet.TriState.Auto));
			AspNetCore.ReportingServices.ReportIntermediateFormat.RecordField[] array2 = recordRow.RecordFields = new AspNetCore.ReportingServices.ReportIntermediateFormat.RecordField[base.m_dataSet.NonCalculatedFieldCount];
			for (int i = 0; i < array2.Length; i++)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.Field field = base.m_dataSet.Fields[i];
				if (!this.m_dataSetInstance.IsFieldMissing(i))
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.RecordField recordField = new AspNetCore.ReportingServices.ReportIntermediateFormat.RecordField();
					try
					{
						array2[i] = recordField;
						recordField.FieldValue = this.m_dataReader.GetColumn(i);
						if (flag2)
						{
							if (flag)
							{
								recordField.IsAggregationField = this.m_dataReader.IsAggregationField(i);
							}
						}
						else
						{
							recordField.IsAggregationField = true;
						}
						recordField.FieldStatus = DataFieldStatus.None;
					}
					catch (ReportProcessingException_FieldError aException)
					{
						recordField = (array2[i] = this.HandleFieldError(aException, i, field.Name));
						if (recordField != null && !flag2)
						{
							recordField.IsAggregationField = true;
						}
					}
					this.ReadExtendedPropertiesForRecordField(i, field, recordField);
				}
				else
				{
					array2[i] = null;
				}
			}
			if (flag2)
			{
				if (flag)
				{
					recordRow.IsAggregateRow = this.m_dataReader.IsAggregateRow;
					recordRow.AggregationFieldCount = this.m_dataReader.AggregationFieldCount;
				}
				else
				{
					this.PopulateServerAggregateInformationFromIndicatorFields(recordRow);
				}
			}
			else
			{
				recordRow.AggregationFieldCount = base.m_dataSet.Fields.Count;
			}
			return recordRow;
		}

		private void PopulateServerAggregateInformationFromIndicatorFields(AspNetCore.ReportingServices.ReportIntermediateFormat.RecordRow recordRow)
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < recordRow.RecordFields.Length; i++)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.RecordField recordField = recordRow.RecordFields[i];
				AspNetCore.ReportingServices.ReportIntermediateFormat.Field field = base.m_dataSet.Fields[i];
				if (recordField != null && field.HasAggregateIndicatorField)
				{
					num++;
					AspNetCore.ReportingServices.ReportIntermediateFormat.Field field2 = base.m_dataSet.Fields[field.AggregateIndicatorFieldIndex];
					bool flag = false;
					bool flag2;
					if (field2.IsCalculatedField)
					{
						if (field2.Value.Type == AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant)
						{
							flag = field2.Value.BoolValue;
							flag2 = false;
						}
						else
						{
							flag2 = !AspNetCore.ReportingServices.RdlExpressions.ReportRuntime.TryProcessObjectToBoolean(field2.Value.LiteralInfo.Value, out flag);
						}
					}
					else
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.RecordField recordField2 = recordRow.RecordFields[field.AggregateIndicatorFieldIndex];
						flag2 = (recordField2 == null || recordField2.FieldStatus != 0 || !AspNetCore.ReportingServices.RdlExpressions.ReportRuntime.TryProcessObjectToBoolean(recordField2.FieldValue, out flag));
					}
					if (flag2)
					{
						base.m_odpContext.ErrorContext.Register(ProcessingErrorCode.rsMissingOrInvalidAggregateIndicatorFieldValue, Severity.Warning, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Field, field2.Name, "AggregateIndicatorField", base.m_dataSet.Name.MarkAsPrivate(), field.Name.MarkAsModelInfo());
					}
					else if (flag)
					{
						num2++;
						recordRow.IsAggregateRow = true;
					}
					recordField.IsAggregationField = !flag;
				}
			}
			recordRow.AggregationFieldCount = num - num2;
		}

		private void ReadExtendedPropertiesForRecordField(int fieldIndex, AspNetCore.ReportingServices.ReportIntermediateFormat.Field fieldDef, AspNetCore.ReportingServices.ReportIntermediateFormat.RecordField field)
		{
			if (this.m_dataReader.ReaderFieldProperties && this.m_dataSetInstance.GetFieldPropertyCount(fieldIndex) > 0)
			{
				FieldInfo orCreateFieldInfo = this.m_dataSetInstance.GetOrCreateFieldInfo(fieldIndex);
				field.FieldPropertyValues = new List<object>(orCreateFieldInfo.PropertyCount);
				for (int i = 0; i < orCreateFieldInfo.PropertyCount; i++)
				{
					int propertyIndex = orCreateFieldInfo.PropertyReaderIndices[i];
					string modelInfo = orCreateFieldInfo.PropertyNames[i];
					try
					{
						object propertyValue = this.m_dataReader.GetPropertyValue(fieldIndex, propertyIndex);
						field.FieldPropertyValues.Add(propertyValue);
					}
					catch (ReportProcessingException_FieldError reportProcessingException_FieldError)
					{
						if (!orCreateFieldInfo.IsPropertyErrorRegistered(i))
						{
							base.m_odpContext.ErrorContext.Register(ProcessingErrorCode.rsErrorReadingFieldProperty, Severity.Warning, AspNetCore.ReportingServices.ReportProcessing.ObjectType.DataSet, base.m_dataSet.Name, "FieldExtendedProperty", fieldDef.Name.MarkAsModelInfo(), modelInfo.MarkAsModelInfo(), reportProcessingException_FieldError.Message);
							orCreateFieldInfo.SetPropertyErrorRegistered(i);
						}
						field.FieldPropertyValues.Add(null);
					}
				}
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.RecordField HandleFieldError(ReportProcessingException_FieldError aException, int aFieldIndex, string aFieldName)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.RecordField recordField = null;
			bool flag = false;
			FieldInfo orCreateFieldInfo = this.m_dataSetInstance.GetOrCreateFieldInfo(aFieldIndex);
			if (this.m_dataRowsRead == 0 && DataFieldStatus.UnSupportedDataType != aException.Status && DataFieldStatus.Overflow != aException.Status)
			{
				orCreateFieldInfo.Missing = true;
				recordField = null;
				flag = true;
				base.m_odpContext.ErrorContext.Register(ProcessingErrorCode.rsMissingFieldInDataSet, Severity.Warning, AspNetCore.ReportingServices.ReportProcessing.ObjectType.DataSet, base.m_dataSet.Name, "Field", aFieldName.MarkAsModelInfo());
			}
			if (!flag)
			{
				recordField = new AspNetCore.ReportingServices.ReportIntermediateFormat.RecordField();
				recordField.FieldStatus = aException.Status;
				recordField.IsAggregationField = false;
				recordField.FieldValue = null;
			}
			if (!orCreateFieldInfo.ErrorRegistered)
			{
				orCreateFieldInfo.ErrorRegistered = true;
				if (DataFieldStatus.UnSupportedDataType == aException.Status)
				{
					if (!base.m_odpContext.ProcessReportParameters)
					{
						base.m_odpContext.ErrorSavingSnapshotData = true;
					}
					base.m_odpContext.ErrorContext.Register(ProcessingErrorCode.rsDataSetFieldTypeNotSupported, Severity.Warning, AspNetCore.ReportingServices.ReportProcessing.ObjectType.DataSet, base.m_dataSet.Name, "Field", aFieldName.MarkAsModelInfo());
				}
				else
				{
					base.m_odpContext.ErrorContext.Register(ProcessingErrorCode.rsErrorReadingDataSetField, Severity.Warning, AspNetCore.ReportingServices.ReportProcessing.ObjectType.DataSet, base.m_dataSet.Name, "Field", aFieldName.MarkAsModelInfo(), aException.Message);
				}
			}
			return recordField;
		}

		protected void InitializeAndRunLiveQuery()
		{
			if (base.m_dataSourceConnection == null)
			{
				base.m_isConnectionOwner = true;
			}
			bool readerExtensionsSupported = this.RunDataSetQuery();
			this.InitializeToProcessData(readerExtensionsSupported);
		}

		private void InitializeToProcessData(bool readerExtensionsSupported)
		{
			if (this.m_processRetrievedData)
			{
				this.InitializeBeforeProcessingRows(readerExtensionsSupported);
				base.m_odpContext.CheckAndThrowIfAborted();
			}
		}

		protected void InitializeAndRunFromExistingQuery(ExecutedQuery query)
		{
			bool readerExtensionsSupported = this.RunFromExistingQuery(query);
			this.InitializeToProcessData(readerExtensionsSupported);
		}

		private bool RunFromExistingQuery(ExecutedQuery query)
		{
			if (this.m_dataSetInstance != null)
			{
				this.m_dataSetInstance.SetQueryExecutionTime(query.QueryExecutionTimestamp);
				this.m_dataSetInstance.CommandText = query.CommandText;
			}
			bool result = this.TakeOwnershipFromExistingQuery(query);
			if (!base.m_odpContext.IsSharedDataSetExecutionOnly && this.m_dataSetInstance != null)
			{
				this.m_dataSetInstance.SaveCollationSettings(base.m_dataSet);
				this.UpdateReportOMDataSet();
			}
			return result;
		}

		private bool TakeOwnershipFromExistingQuery(ExecutedQuery query)
		{
			IDataReader dataReader = null;
			try
			{
				base.m_executionMetrics.Add(query.ExecutionMetrics);
				base.m_executionMetrics.CommandText = query.ExecutionMetrics.CommandText;
				query.ReleaseOwnership(ref base.m_command, ref base.m_commandWrappedForCancel, ref dataReader);
				this.ExtractRewrittenCommandText(base.m_command);
				this.StoreDataReader(dataReader, query.ErrorInspector);
				return RuntimeDataSet.ReaderExtensionsSupported(dataReader);
			}
			catch (RSException)
			{
				this.EagerInlineReaderCleanup(ref dataReader);
				throw;
			}
			catch (Exception e)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(e))
				{
					throw;
				}
				this.EagerInlineReaderCleanup(ref dataReader);
				throw;
			}
		}

		private bool RunDataSetQuery()
		{
			bool result = false;
			if (this.m_dataSetInstance != null)
			{
				this.m_dataSetInstance.SetQueryExecutionTime(DateTime.Now);
			}
			if (base.m_dataSet.Query == null)
			{
				return result;
			}
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterValue> parameters = base.m_dataSet.Query.Parameters;
			object[] array = new object[(parameters != null) ? parameters.Count : 0];
			for (int i = 0; i < array.Length; i++)
			{
				if (base.m_odpContext.IsSharedDataSetExecutionOnly)
				{
					DataSetParameterValue dataSetParameterValue = parameters[i] as DataSetParameterValue;
					if (!dataSetParameterValue.OmitFromQuery)
					{
						array[i] = ((Parameters)base.m_odpContext.ReportObjectModel.ParametersImpl)[dataSetParameterValue.UniqueName].Value;
					}
				}
				else
				{
					array[i] = parameters[i].EvaluateQueryParameterValue(base.m_odpContext, base.m_dataSet.ExprHost);
				}
			}
			base.m_odpContext.CheckAndThrowIfAborted();
			base.m_executionMetrics.StartTotalTimer();
			try
			{
				result = this.RunEmbeddedQuery(parameters, array);
			}
			finally
			{
				base.m_executionMetrics.RecordTotalTimerMeasurement();
			}
			if (!base.m_odpContext.IsSharedDataSetExecutionOnly && this.m_dataSetInstance != null)
			{
				this.m_dataSetInstance.SaveCollationSettings(base.m_dataSet);
				this.UpdateReportOMDataSet();
			}
			return result;
		}

		private void UpdateReportOMDataSet()
		{
			AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.DataSetsImpl dataSetsImpl = base.m_odpContext.ReportObjectModel.DataSetsImpl;
			AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.DataSetImpl dataSetImpl = (AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.DataSetImpl)((DataSets)dataSetsImpl)[base.m_dataSet.Name];
			dataSetImpl.Update(this.m_dataSetInstance, base.m_odpContext.ExecutionTime);
		}

		protected bool ProcessSharedDataSetReference()
		{
			DataSetInfo dataSetInfo = null;
			if (base.m_odpContext.SharedDataSetReferences != null)
			{
				if (Guid.Empty != base.m_dataSet.DataSetCore.CatalogID)
				{
					dataSetInfo = base.m_odpContext.SharedDataSetReferences.GetByID(base.m_dataSet.DataSetCore.CatalogID);
				}
				if (dataSetInfo == null)
				{
					dataSetInfo = base.m_odpContext.SharedDataSetReferences.GetByName(base.m_dataSet.DataSetCore.Name, base.m_odpContext.ReportContext);
				}
			}
			if (dataSetInfo == null)
			{
				throw new ReportProcessingException(ErrorCode.rsInvalidSharedDataSetReference, base.m_dataSet.Name.MarkAsPrivate(), base.m_dataSet.SharedDataSetQuery.SharedDataSetReference);
			}
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterValue> parameters = base.m_dataSet.SharedDataSetQuery.Parameters;
			SharedDataSetParameterNameMapper.MakeUnique(parameters);
			ParameterInfoCollection parameterInfoCollection = new ParameterInfoCollection();
			object[] array = new object[(parameters != null) ? parameters.Count : 0];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = parameters[i].EvaluateQueryParameterValue(base.m_odpContext, base.m_dataSet.ExprHost);
				if (base.m_dataSet.IsReferenceToSharedDataSet)
				{
					ParameterInfo parameterInfo = new ParameterInfo(parameters[i]);
					parameterInfo.Name = parameters[i].UniqueName;
					parameterInfo.SetValuesFromQueryParameter(array[i]);
					parameterInfo.DataType = DataType.Object;
					parameterInfoCollection.Add(parameterInfo);
				}
			}
			base.m_odpContext.CheckAndThrowIfAborted();
			base.m_executionMetrics.StartTotalTimer();
			try
			{
				this.GetSharedDataSetChunkAndProcess(true, dataSetInfo, parameterInfoCollection);
			}
			finally
			{
				base.m_executionMetrics.RecordTotalTimerMeasurement();
			}
			if (!base.m_odpContext.IsSharedDataSetExecutionOnly && this.m_dataSetInstance != null)
			{
				this.m_dataSetInstance.SaveCollationSettings(base.m_dataSet);
				this.UpdateReportOMDataSet();
			}
			return false;
		}

		private void GetSharedDataSetChunkAndProcess(bool processAsIRowConsumer, DataSetInfo dataSetInfo, ParameterInfoCollection datasetParameterCollection)
		{
			Global.Tracer.Assert(base.m_odpContext.ExternalProcessingContext != null && null != base.m_odpContext.ExternalProcessingContext.DataSetExecute, "Missing handler for shared dataset reference execution");
			string text = null;
			if (!base.m_odpContext.ProcessReportParameters)
			{
				text = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.GenerateDataChunkName(base.m_odpContext, base.m_dataSet.ID, base.m_odpContext.InSubreport);
			}
			IRowConsumer originalRequest = processAsIRowConsumer ? ((IRowConsumer)this) : null;
			bool originalRequestNeedsDataChunk = !processAsIRowConsumer || this.WritesDataChunk;
			base.m_odpContext.ExternalProcessingContext.DataSetExecute.Process(dataSetInfo, text, originalRequestNeedsDataChunk, originalRequest, datasetParameterCollection, base.m_odpContext.ExternalProcessingContext);
			if (processAsIRowConsumer)
			{
				if (!base.m_odpContext.ProcessReportParameters)
				{
					base.m_odpContext.OdpMetadata.AddDataChunk(text, this.m_dataSetInstance);
				}
			}
			else
			{
				this.m_dataReader = new ProcessingDataReader(this.m_dataSetInstance, base.m_dataSet, base.m_odpContext, true);
			}
		}

		private bool RunEmbeddedQuery(List<AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterValue> queryParams, object[] paramValues)
		{
			Global.Tracer.Assert(base.m_odpContext.StateManager.ExecutedQueryCache == null, "When query execution caching is enabled, new queries must not be run outside query prefetch.");
			IDataReader reader = base.RunLiveQuery(queryParams, paramValues);
			return RuntimeDataSet.ReaderExtensionsSupported(reader);
		}

		protected override void StoreDataReader(IDataReader reader, DataSourceErrorInspector errorInspector)
		{
			bool readerExtensionsSupportedLocal = RuntimeDataSet.ReaderExtensionsSupported(reader);
			if (reader.FieldCount > 0 || base.m_odpContext.IsSharedDataSetExecutionOnly)
			{
				this.CreateProcessingDataReader(reader, errorInspector, readerExtensionsSupportedLocal);
			}
			else
			{
				this.EagerInlineReaderCleanup(ref reader);
				base.DisposeCommand();
			}
		}

		private static bool ReaderExtensionsSupported(IDataReader reader)
		{
			return reader is IDataReaderExtension;
		}

		protected override void SetRestartPosition(IDbCommand command)
		{
			if (base.m_odpContext.StreamingMode && !(command is IRestartable))
			{
				throw new ReportProcessingException(ErrorCode.rsInvalidDataExtension);
			}
			try
			{
				if (this.m_restartPosition != null && command is IRestartable)
				{
					List<ScopeValueFieldName> queryRestartPosition = this.m_restartPosition.GetQueryRestartPosition(base.m_dataSet);
					if (queryRestartPosition != null)
					{
						IDataParameter[] startAtParameters = ((IRestartable)command).StartAt(queryRestartPosition);
						if (base.m_odpContext.UseVerboseExecutionLogging)
						{
							base.m_executionMetrics.SetStartAtParameters(startAtParameters);
						}
					}
				}
			}
			catch (Exception innerException)
			{
				throw new ReportProcessingException(ErrorCode.rsErrorSettingStartAt, innerException, base.m_dataSet.Name.MarkAsPrivate());
			}
		}

		protected override void ExtractRewrittenCommandText(IDbCommand command)
		{
			if (command is IDbCommandRewriter && this.m_dataSetInstance != null)
			{
				this.m_dataSetInstance.RewrittenCommandText = ((IDbCommandRewriter)command).RewrittenCommandText;
			}
		}

		protected override void StoreCommandText(string commandText)
		{
			this.m_dataSetInstance.CommandText = commandText;
		}

		private void CreateProcessingDataReader(IDataReader reader, DataSourceErrorInspector errorInspector, bool readerExtensionsSupportedLocal)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.Field> fields = base.m_dataSet.Fields;
			int num = 0;
			if (fields != null)
			{
				num = ((!base.m_odpContext.IsSharedDataSetExecutionOnly) ? base.m_dataSet.NonCalculatedFieldCount : base.m_dataSet.Fields.Count);
			}
			string[] array = new string[num];
			string[] array2 = new string[num];
			for (int i = 0; i < num; i++)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.Field field = fields[i];
				array[i] = field.DataField;
				array2[i] = field.Name;
			}
			base.m_executionMetrics.StartTimer(DataProcessingMetrics.MetricType.DataReaderMapping);
			this.m_dataReader = new ProcessingDataReader(base.m_odpContext, this.m_dataSetInstance, base.m_dataSet.Name, reader, readerExtensionsSupportedLocal || base.m_dataSet.HasAggregateIndicatorFields, array2, array, errorInspector);
			base.m_executionMetrics.RecordTimerMeasurement(DataProcessingMetrics.MetricType.DataReaderMapping);
		}

		protected override void EagerInlineReaderCleanup(ref IDataReader reader)
		{
			if (this.m_dataReader != null)
			{
				reader = null;
				this.DisposeDataReader();
			}
			else
			{
				base.DisposeDataExtensionObject<IDataReader>(ref reader, "data reader");
			}
		}

		internal virtual void EraseDataChunk()
		{
		}

		protected static void EraseDataChunk(OnDemandProcessingContext odpContext, DataSetInstance dataSetInstance, ref AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkWriter dataChunkWriter)
		{
			if (dataChunkWriter == null)
			{
				dataChunkWriter = new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkWriter(dataSetInstance, odpContext);
			}
			dataChunkWriter.CloseAndEraseChunk();
			dataChunkWriter = null;
		}
	}
}
