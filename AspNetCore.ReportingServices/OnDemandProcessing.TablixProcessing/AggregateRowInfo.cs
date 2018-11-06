using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal sealed class AggregateRowInfo
	{
		private bool[] m_aggregationFieldChecked;

		private int m_aggregationFieldCount;

		private bool m_validAggregateRow;

		internal static AggregateRowInfo CreateAndSaveAggregateInfo(OnDemandProcessingContext odpContext)
		{
			AggregateRowInfo aggregateRowInfo = new AggregateRowInfo();
			aggregateRowInfo.SaveAggregateInfo(odpContext);
			return aggregateRowInfo;
		}

		internal void SaveAggregateInfo(OnDemandProcessingContext odpContext)
		{
			FieldsImpl fieldsImpl = odpContext.ReportObjectModel.FieldsImpl;
			this.m_aggregationFieldCount = fieldsImpl.AggregationFieldCount;
			if (this.m_aggregationFieldChecked == null)
			{
				this.m_aggregationFieldChecked = new bool[fieldsImpl.Count];
			}
			for (int i = 0; i < fieldsImpl.Count; i++)
			{
				this.m_aggregationFieldChecked[i] = fieldsImpl[i].AggregationFieldChecked;
			}
			this.m_validAggregateRow = fieldsImpl.ValidAggregateRow;
		}

		internal void RestoreAggregateInfo(OnDemandProcessingContext odpContext)
		{
			FieldsImpl fieldsImpl = odpContext.ReportObjectModel.FieldsImpl;
			fieldsImpl.AggregationFieldCount = this.m_aggregationFieldCount;
			Global.Tracer.Assert(null != this.m_aggregationFieldChecked, "(null != m_aggregationFieldChecked)");
			for (int i = 0; i < fieldsImpl.Count; i++)
			{
				fieldsImpl[i].AggregationFieldChecked = this.m_aggregationFieldChecked[i];
			}
			fieldsImpl.ValidAggregateRow = this.m_validAggregateRow;
		}

		internal void CombineAggregateInfo(OnDemandProcessingContext odpContext, AggregateRowInfo updated)
		{
			FieldsImpl fieldsImpl = odpContext.ReportObjectModel.FieldsImpl;
			if (updated == null)
			{
				fieldsImpl.ValidAggregateRow = false;
			}
			else
			{
				if (!updated.m_validAggregateRow)
				{
					fieldsImpl.ValidAggregateRow = false;
				}
				for (int i = 0; i < fieldsImpl.Count; i++)
				{
					if (updated.m_aggregationFieldChecked[i])
					{
						fieldsImpl.ConsumeAggregationField(i);
					}
				}
			}
		}
	}
}
