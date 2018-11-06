using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class RowInstance
	{
		private AspNetCore.ReportingServices.ReportIntermediateFormat.RecordRow m_recordRow;

		private FieldInstance[] m_fields;

		private readonly FieldInfo[] m_fieldInfos;

		public FieldInstance this[int fieldIndex]
		{
			get
			{
				if (fieldIndex >= 0 && fieldIndex < this.m_recordRow.RecordFields.Length)
				{
					if (this.m_fields == null)
					{
						this.m_fields = new FieldInstance[this.m_recordRow.RecordFields.Length];
					}
					if (this.m_fields[fieldIndex] == null)
					{
						this.m_fields[fieldIndex] = new FieldInstance((this.m_fieldInfos != null) ? this.m_fieldInfos[fieldIndex] : null, this.m_recordRow.RecordFields[fieldIndex]);
					}
					return this.m_fields[fieldIndex];
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, fieldIndex, 0, this.m_recordRow.RecordFields.Length);
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

		internal RowInstance(FieldInfo[] fieldInfos, AspNetCore.ReportingServices.ReportIntermediateFormat.RecordRow recordRow)
		{
			this.m_recordRow = recordRow;
			this.m_fieldInfos = fieldInfos;
		}

		internal void UpdateRecordRow(AspNetCore.ReportingServices.ReportIntermediateFormat.RecordRow recordRow)
		{
			this.m_recordRow = recordRow;
			if (this.m_fields != null)
			{
				for (int i = 0; i < this.m_fields.Length; i++)
				{
					FieldInstance fieldInstance = this.m_fields[i];
					if (fieldInstance != null)
					{
						fieldInstance.UpdateRecordField(this.m_recordRow.RecordFields[i]);
					}
				}
			}
		}
	}
}
