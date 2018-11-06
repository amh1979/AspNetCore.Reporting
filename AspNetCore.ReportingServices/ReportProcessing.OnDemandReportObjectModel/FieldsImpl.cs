using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public sealed class FieldsImpl : Fields
	{
		private ObjectModelImpl m_reportOM;

		private Hashtable m_nameMap;

		private bool[] m_fieldMissing;

		private bool[] m_fieldError;

		private FieldImpl[] m_collection;

		private int m_count;

		private bool m_referenced;

		private bool m_readerExtensionsSupported;

		private bool m_readerFieldProperties;

		private bool m_isAggregateRow;

		private int m_aggregationFieldCount;

		private int m_aggregationFieldCountForDetailRow;

		private bool m_noRows;

		private long m_streamOffset = DataFieldRow.UnInitializedStreamOffset;

		private bool m_validAggregateRow;

		private bool m_addRowIndex;

		private bool m_needsInlineSetup;

		public override AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel.Field this[string key]
		{
			get
			{
				if (key == null)
				{
					throw new ReportProcessingException_NonExistingFieldReference(key.MarkAsPrivate());
				}
				this.ValidateFieldCollection();
				try
				{
					int index = (int)this.m_nameMap[key];
					return this.CheckedGetFieldByIndex(index);
				}
				catch (Exception e)
				{
					if (AsynchronousExceptionDetection.IsStoppingException(e))
					{
						throw;
					}
					throw new ReportProcessingException_NonExistingFieldReference(key.MarkAsPrivate());
				}
			}
		}

		internal bool IsCollectionInitialized
		{
			get
			{
				return null != this.m_collection;
			}
		}

		internal FieldImpl this[int index]
		{
			get
			{
				this.ValidateFieldCollection();
				return this.CheckedGetFieldByIndex(index);
			}
			set
			{
				Global.Tracer.Assert(null != this.m_collection, "(null != m_collection)");
				this.m_collection[index] = value;
			}
		}

		internal int Count
		{
			get
			{
				return this.m_count - (this.m_addRowIndex ? 1 : 0);
			}
		}

		internal int CountWithRowIndex
		{
			get
			{
				return this.m_count;
			}
		}

		internal bool ReaderExtensionsSupported
		{
			get
			{
				return this.m_readerExtensionsSupported;
			}
			set
			{
				this.m_readerExtensionsSupported = value;
			}
		}

		internal bool ReaderFieldProperties
		{
			get
			{
				return this.m_readerFieldProperties;
			}
			set
			{
				this.m_readerFieldProperties = value;
			}
		}

		internal bool IsAggregateRow
		{
			get
			{
				return this.m_isAggregateRow;
			}
			set
			{
				this.m_isAggregateRow = value;
			}
		}

		internal int AggregationFieldCount
		{
			get
			{
				return this.m_aggregationFieldCount;
			}
			set
			{
				this.m_aggregationFieldCount = value;
			}
		}

		internal int AggregationFieldCountForDetailRow
		{
			set
			{
				this.m_aggregationFieldCountForDetailRow = value;
			}
		}

		internal bool ValidAggregateRow
		{
			get
			{
				return this.m_validAggregateRow;
			}
			set
			{
				this.m_validAggregateRow = value;
			}
		}

		internal bool AddRowIndex
		{
			get
			{
				return this.m_addRowIndex;
			}
		}

		internal bool NeedsInlineSetup
		{
			get
			{
				return this.m_needsInlineSetup;
			}
			set
			{
				this.m_needsInlineSetup = value;
			}
		}

		internal long StreamOffset
		{
			get
			{
				return this.m_streamOffset;
			}
		}

		internal FieldsImpl(ObjectModelImpl reportOM, int size, bool addRowIndex, bool noRows)
		{
			this.m_reportOM = reportOM;
			if (addRowIndex)
			{
				this.m_collection = new FieldImpl[size + 1];
			}
			else
			{
				this.m_collection = new FieldImpl[size];
			}
			this.m_nameMap = new Hashtable(size);
			this.m_fieldMissing = null;
			this.m_count = 0;
			this.m_referenced = false;
			this.m_readerExtensionsSupported = false;
			this.m_isAggregateRow = false;
			this.m_aggregationFieldCount = size;
			this.m_aggregationFieldCountForDetailRow = size;
			this.m_noRows = noRows;
			this.m_validAggregateRow = true;
			this.m_addRowIndex = addRowIndex;
		}

		internal FieldsImpl(ObjectModelImpl reportOM)
		{
			this.m_reportOM = reportOM;
			this.m_collection = null;
			this.m_nameMap = null;
			this.m_fieldMissing = null;
			this.m_count = 0;
			this.m_referenced = false;
			this.m_readerExtensionsSupported = false;
			this.m_isAggregateRow = false;
			this.m_aggregationFieldCount = 0;
			this.m_aggregationFieldCountForDetailRow = 0;
			this.m_noRows = true;
			this.m_validAggregateRow = true;
			this.m_addRowIndex = false;
		}

		internal FieldImpl GetFieldByIndex(int index)
		{
			return this.m_collection[index];
		}

		private FieldImpl CheckedGetFieldByIndex(int index)
		{
			try
			{
				if (this.m_collection[index] == null || this.m_collection[index].IsCalculatedField)
				{
					this.m_reportOM.PerformPendingFieldValueUpdate();
				}
				return this.m_collection[index];
			}
			catch (Exception e)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(e))
				{
					throw;
				}
				throw new ReportProcessingException_NonExistingFieldReference();
			}
		}

		internal void Add(string name, FieldImpl field)
		{
			Global.Tracer.Assert(null != this.m_collection, "(null != m_collection)");
			Global.Tracer.Assert(null != this.m_nameMap, "(null != m_nameMap)");
			Global.Tracer.Assert(this.m_count < this.m_collection.Length, "(m_count < m_collection.Length)");
			this.m_nameMap.Add(name, this.m_count);
			this.m_collection[this.m_count] = field;
			this.m_count++;
		}

		internal void AddRowIndexField()
		{
			Global.Tracer.Assert(null != this.m_collection, "(null != m_collection)");
			Global.Tracer.Assert(this.m_count < this.m_collection.Length, "(m_count < m_collection.Length)");
			this.m_collection[this.m_count] = null;
			this.m_count++;
		}

		internal void SetFieldIsMissing(int index)
		{
			if (this.m_fieldMissing == null)
			{
				this.m_fieldMissing = new bool[this.m_collection.Length];
			}
			this.m_fieldMissing[index] = true;
		}

		internal bool IsFieldMissing(int index)
		{
			if (this.m_fieldMissing == null)
			{
				return false;
			}
			return this.m_fieldMissing[index];
		}

		internal void SetFieldErrorRegistered(int index)
		{
			if (this.m_fieldError == null)
			{
				this.m_fieldError = new bool[this.m_collection.Length];
			}
			this.m_fieldError[index] = true;
		}

		internal bool IsFieldErrorRegistered(int index)
		{
			if (this.m_fieldError == null)
			{
				return false;
			}
			return this.m_fieldError[index];
		}

		internal void NewRow()
		{
			this.NewRow(DataFieldRow.UnInitializedStreamOffset);
		}

		internal void NewRow(long streamOffset)
		{
			this.m_noRows = false;
			this.m_validAggregateRow = true;
			this.m_streamOffset = streamOffset;
			if (this.m_referenced)
			{
				this.m_collection = new FieldImpl[this.m_count];
				this.m_referenced = false;
			}
		}

		internal void SetRowIndex(int rowIndex)
		{
			Global.Tracer.Assert(this.m_addRowIndex, "(m_addRowIndex)");
			Global.Tracer.Assert(this.m_count > 0, "(m_count > 0)");
			this.m_collection[this.m_count - 1] = new FieldImpl(this.m_reportOM, rowIndex, false, null);
		}

		internal void SetFields(FieldImpl[] fields, long streamOffset)
		{
			bool flag = this.m_referenced || streamOffset == DataFieldRow.UnInitializedStreamOffset || this.m_streamOffset != streamOffset;
			this.NewRow(streamOffset);
			if (this.m_collection == null)
			{
				Global.Tracer.Assert(false, "Invalid FieldsImpl.  m_collection should not be null.");
			}
			if (fields == null)
			{
				for (int i = 0; i < this.m_count; i++)
				{
					FieldImpl fieldImpl = this.m_collection[i];
					AspNetCore.ReportingServices.ReportIntermediateFormat.Field fieldDef = (fieldImpl == null) ? null : fieldImpl.FieldDef;
					this.m_collection[i] = new FieldImpl(this.m_reportOM, null, false, fieldDef);
				}
			}
			else if (flag)
			{
				if (fields.Length != this.m_count)
				{
					Global.Tracer.Assert(false, "Wrong number of fields during ReportOM update.  Usually this means the data set is wrong.  Expected: {0}.  Actual: {1}", this.m_count, fields.Length);
				}
				for (int j = 0; j < this.m_count; j++)
				{
					this.m_collection[j] = fields[j];
				}
				this.m_isAggregateRow = false;
				this.m_aggregationFieldCount = this.m_aggregationFieldCountForDetailRow;
			}
		}

		internal void SetFields(FieldImpl[] fields, long streamOffset, bool isAggregateRow, int aggregationFieldCount, bool validAggregateRow)
		{
			this.SetFields(fields, streamOffset);
			this.m_isAggregateRow = isAggregateRow;
			this.m_aggregationFieldCount = aggregationFieldCount;
			this.m_validAggregateRow = validAggregateRow;
		}

		internal FieldImpl[] GetAndSaveFields()
		{
			Global.Tracer.Assert(null != this.m_collection, "(null != m_collection)");
			this.m_referenced = true;
			return this.m_collection;
		}

		internal FieldImpl[] GetFields()
		{
			return this.m_collection;
		}

		internal int GetRowIndex()
		{
			return (int)this.m_collection[this.m_count - 1].Value;
		}

		internal void Clone(FieldsImpl fields)
		{
			if (fields != null)
			{
				this.m_collection = fields.m_collection;
				this.m_nameMap = fields.m_nameMap;
				this.m_count = fields.m_count;
				this.m_referenced = fields.m_referenced;
				this.m_noRows = fields.m_noRows;
				this.m_fieldMissing = fields.m_fieldMissing;
			}
		}

		private bool ValidateFieldCollection()
		{
			if (this.m_needsInlineSetup)
			{
				this.m_needsInlineSetup = false;
				this.m_reportOM.OdpContext.PrepareFieldsCollectionForDirectFields();
			}
			if (this.m_nameMap != null && this.m_collection != null)
			{
				if (this.m_noRows)
				{
					throw new ReportProcessingException_NoRowsFieldAccess();
				}
				return true;
			}
			throw new ReportProcessingException_NonExistingFieldReference();
		}

		internal void ResetFieldsUsedInExpression()
		{
			if (this.m_collection != null)
			{
				for (int i = 0; i < this.m_collection.Length; i++)
				{
					FieldImpl fieldImpl = this.m_collection[i];
					if (fieldImpl != null)
					{
						fieldImpl.UsedInExpression = false;
					}
				}
			}
		}

		internal void AddFieldsUsedInExpression(List<string> fieldsUsedInValueExpression)
		{
			if (this.m_collection != null)
			{
				for (int i = 0; i < this.m_collection.Length; i++)
				{
					FieldImpl fieldImpl = this.m_collection[i];
					if (fieldImpl != null && fieldImpl.UsedInExpression && fieldImpl.FieldDef != null && fieldImpl.FieldDef.DataField != null)
					{
						fieldsUsedInValueExpression.Add(fieldImpl.FieldDef.DataField);
					}
				}
			}
		}

		internal void ConsumeAggregationField(int fieldIndex)
		{
			FieldImpl fieldImpl = this[fieldIndex];
			if (!fieldImpl.AggregationFieldChecked && fieldImpl.IsAggregationField)
			{
				fieldImpl.AggregationFieldChecked = true;
				this.m_aggregationFieldCount--;
			}
		}
	}
}
