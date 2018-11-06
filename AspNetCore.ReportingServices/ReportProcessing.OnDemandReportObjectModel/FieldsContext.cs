using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public sealed class FieldsContext
	{
		private FieldsImpl m_fields;

		private DataSetCore m_dataSet;

		private DataSetInstance m_dataSetInstance;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader m_dataReader;

		private bool m_allFieldsCleared;

		private bool m_pendingFieldValueUpdate;

		private long m_lastRowOffset;

		private FieldImpl[] m_noRowsFields;

		internal bool AllFieldsCleared
		{
			get
			{
				return this.m_allFieldsCleared;
			}
		}

		internal bool PendingFieldValueUpdate
		{
			get
			{
				return this.m_pendingFieldValueUpdate;
			}
		}

		internal long LastRowOffset
		{
			get
			{
				return this.m_lastRowOffset;
			}
		}

		internal DataSetCore DataSet
		{
			get
			{
				return this.m_dataSet;
			}
		}

		internal DataSetInstance DataSetInstance
		{
			get
			{
				return this.m_dataSetInstance;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader DataReader
		{
			get
			{
				return this.m_dataReader;
			}
		}

		internal FieldsImpl Fields
		{
			get
			{
				return this.m_fields;
			}
		}

		internal FieldsContext(ObjectModelImpl reportOM)
			: this(reportOM, null)
		{
		}

		internal FieldsContext(ObjectModelImpl reportOM, DataSetCore dataSet)
		{
			this.Initialize(reportOM, new FieldsImpl(reportOM), dataSet, null, null, true, false, DataFieldRow.UnInitializedStreamOffset);
		}

		internal FieldsContext(ObjectModelImpl reportOM, DataSetCore dataSet, bool addRowIndex, bool noRows)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.Field> fields = dataSet.Fields;
			int num = (fields != null) ? fields.Count : 0;
			FieldsImpl fieldsImpl = new FieldsImpl(reportOM, num, addRowIndex, noRows);
			this.Initialize(reportOM, fieldsImpl, dataSet, null, null, true, false, DataFieldRow.UnInitializedStreamOffset);
			for (int i = 0; i < num; i++)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.Field field = fields[i];
				if (dataSet.ExprHost != null)
				{
					field.SetExprHost(dataSet.ExprHost, reportOM);
				}
				fieldsImpl.Add(field.Name, null);
			}
			if (addRowIndex)
			{
				fieldsImpl.AddRowIndexField();
			}
		}

		private void Initialize(ObjectModelImpl reportOM, FieldsImpl fields, DataSetCore dataSet, DataSetInstance dataSetInstance, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader dataReader, bool allFieldsCleared, bool pendingFieldValueUpdate, long lastRowOffset)
		{
			this.m_fields = fields;
			this.m_dataSet = dataSet;
			this.m_dataSetInstance = dataSetInstance;
			this.m_dataReader = dataReader;
			this.m_allFieldsCleared = allFieldsCleared;
			this.m_pendingFieldValueUpdate = pendingFieldValueUpdate;
			this.m_lastRowOffset = lastRowOffset;
			this.AttachToDataSetCache(reportOM);
		}

		internal void AttachToDataSetCache(ObjectModelImpl reportOM)
		{
			if (this.m_dataSet != null && reportOM.UseDataSetFieldsCache)
			{
				this.m_dataSet.FieldsContext = this;
			}
		}

		internal void ResetFieldFlags()
		{
			this.m_pendingFieldValueUpdate = false;
			this.m_allFieldsCleared = true;
		}

		internal void UpdateDataSetInfo(DataSetInstance dataSetInstance, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader dataChunkReader)
		{
			this.m_dataSetInstance = dataSetInstance;
			this.m_dataReader = dataChunkReader;
		}

		internal void CreateNoRows()
		{
			if (this.m_noRowsFields == null)
			{
				this.m_fields.SetFields(null, DataFieldRow.UnInitializedStreamOffset);
				this.m_noRowsFields = this.m_fields.GetAndSaveFields();
			}
			else
			{
				this.m_fields.SetFields(this.m_noRowsFields, DataFieldRow.UnInitializedStreamOffset);
			}
			this.ResetFieldFlags();
		}

		internal void CreateNullFieldValues()
		{
			int count = this.m_fields.Count;
			for (int i = 0; i < count; i++)
			{
				FieldImpl fieldByIndex = this.m_fields.GetFieldByIndex(i);
				if (fieldByIndex != null)
				{
					fieldByIndex.UpdateValue(null, false, DataFieldStatus.None, null);
				}
			}
			this.ResetFieldFlags();
		}

		internal void PerformPendingFieldValueUpdate(ObjectModelImpl reportOM, bool useDataSetFieldsCache)
		{
			if (this.m_pendingFieldValueUpdate)
			{
				this.m_pendingFieldValueUpdate = false;
				this.UpdateFieldValues(reportOM, useDataSetFieldsCache, this.m_lastRowOffset);
			}
		}

		internal void RegisterOnDemandFieldValueUpdate(long firstRowOffsetInScope, DataSetInstance dataSetInstance, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader dataReader)
		{
			this.m_pendingFieldValueUpdate = true;
			this.m_lastRowOffset = firstRowOffsetInScope;
			this.m_dataSetInstance = dataSetInstance;
			this.m_dataReader = dataReader;
		}

		internal void UpdateFieldValues(ObjectModelImpl reportOM, bool useDataSetFieldsCache, long firstRowOffsetInScope)
		{
			if (!this.m_dataReader.ReadOneRowAtPosition(firstRowOffsetInScope) && !this.m_allFieldsCleared)
			{
				return;
			}
			this.UpdateFieldValues(reportOM, useDataSetFieldsCache, true, this.m_dataReader.RecordRow, this.m_dataSetInstance, this.m_dataReader.ReaderExtensionsSupported);
		}

		internal void UpdateFieldValues(ObjectModelImpl reportOM, bool useDataSetFieldsCache, bool reuseFieldObjects, AspNetCore.ReportingServices.ReportIntermediateFormat.RecordRow row, DataSetInstance dataSetInstance, bool readerExtensionsSupported)
		{
			Global.Tracer.Assert(null != row, "Empty data row / no data reader");
			if (this.m_dataSetInstance != dataSetInstance)
			{
				this.m_dataSetInstance = dataSetInstance;
				this.m_dataSet = dataSetInstance.DataSetDef.DataSetCore;
				if (this.m_dataSet.FieldsContext != null && useDataSetFieldsCache)
				{
					this.m_fields = this.m_dataSet.FieldsContext.Fields;
				}
				else
				{
					reuseFieldObjects = false;
				}
				this.m_dataReader = null;
				this.m_lastRowOffset = DataFieldRow.UnInitializedStreamOffset;
				this.m_pendingFieldValueUpdate = false;
			}
			this.m_allFieldsCleared = false;
			FieldInfo[] fieldInfos = dataSetInstance.FieldInfos;
			if (this.m_fields.ReaderExtensionsSupported && this.m_dataSet.InterpretSubtotalsAsDetails == AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet.TriState.False)
			{
				this.m_fields.IsAggregateRow = row.IsAggregateRow;
				this.m_fields.AggregationFieldCount = row.AggregationFieldCount;
				if (!row.IsAggregateRow)
				{
					this.m_fields.AggregationFieldCountForDetailRow = row.AggregationFieldCount;
				}
			}
			int num = 0;
			int count = this.m_dataSet.Fields.Count;
			int num2 = row.RecordFields.Length;
			for (num = 0; num < num2; num++)
			{
				FieldImpl fieldImpl = reuseFieldObjects ? this.m_fields.GetFieldByIndex(num) : null;
				AspNetCore.ReportingServices.ReportIntermediateFormat.Field fieldDef = this.m_dataSet.Fields[num];
				AspNetCore.ReportingServices.ReportIntermediateFormat.RecordField recordField = row.RecordFields[num];
				if (recordField == null)
				{
					if (!reuseFieldObjects || fieldImpl == null)
					{
						fieldImpl = new FieldImpl(reportOM, DataFieldStatus.IsMissing, null, fieldDef);
					}
					else
					{
						fieldImpl.UpdateValue(null, false, DataFieldStatus.IsMissing, null);
					}
				}
				else if (recordField.FieldStatus == DataFieldStatus.None)
				{
					if (!reuseFieldObjects || fieldImpl == null)
					{
						fieldImpl = new FieldImpl(reportOM, recordField.FieldValue, recordField.IsAggregationField, fieldDef);
					}
					else
					{
						fieldImpl.UpdateValue(recordField.FieldValue, recordField.IsAggregationField, DataFieldStatus.None, null);
					}
				}
				else if (!reuseFieldObjects || fieldImpl == null)
				{
					fieldImpl = new FieldImpl(reportOM, recordField.FieldStatus, ReportRuntime.GetErrorName(recordField.FieldStatus, null), fieldDef);
				}
				else
				{
					fieldImpl.UpdateValue(null, false, recordField.FieldStatus, ReportRuntime.GetErrorName(recordField.FieldStatus, null));
				}
				if (recordField != null && fieldInfos != null)
				{
					FieldInfo fieldInfo = fieldInfos[num];
					if (fieldInfo != null && fieldInfo.PropertyCount != 0 && recordField.FieldPropertyValues != null)
					{
						for (int i = 0; i < fieldInfo.PropertyCount; i++)
						{
							fieldImpl.SetProperty(fieldInfo.PropertyNames[i], recordField.FieldPropertyValues[i]);
						}
					}
				}
				this.m_fields[num] = fieldImpl;
			}
			if (num < count)
			{
				if (!reuseFieldObjects && reportOM.OdpContext.ReportRuntime.ReportExprHost != null)
				{
					this.m_dataSet.SetExprHost(reportOM.OdpContext.ReportRuntime.ReportExprHost, reportOM);
				}
				for (; num < count; num++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.Field fieldDef2 = this.m_dataSet.Fields[num];
					FieldImpl fieldImpl2 = reuseFieldObjects ? this.m_fields.GetFieldByIndex(num) : null;
					if (reuseFieldObjects && fieldImpl2 != null)
					{
						if (!fieldImpl2.ResetCalculatedField())
						{
							this.CreateAndInitializeCalculatedFieldWrapper(reportOM, readerExtensionsSupported, this.m_dataSet, num, fieldDef2);
						}
					}
					else
					{
						this.CreateAndInitializeCalculatedFieldWrapper(reportOM, readerExtensionsSupported, this.m_dataSet, num, fieldDef2);
					}
				}
			}
		}

		private void CreateAndInitializeCalculatedFieldWrapper(ObjectModelImpl reportOM, bool readerExtensionsSupported, DataSetCore dataSet, int fieldIndex, AspNetCore.ReportingServices.ReportIntermediateFormat.Field fieldDef)
		{
			CalculatedFieldWrapperImpl value = new CalculatedFieldWrapperImpl(fieldDef, reportOM.OdpContext.ReportRuntime);
			bool isAggregationField = (byte)((!readerExtensionsSupported) ? 1 : 0) != 0;
			if (dataSet.InterpretSubtotalsAsDetails == AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet.TriState.True)
			{
				isAggregationField = true;
			}
			this.m_fields[fieldIndex] = new FieldImpl(reportOM, value, isAggregationField, fieldDef);
			if (dataSet.ExprHost != null && fieldDef.ExprHost == null)
			{
				fieldDef.SetExprHost(dataSet.ExprHost, reportOM);
			}
		}
	}
}
