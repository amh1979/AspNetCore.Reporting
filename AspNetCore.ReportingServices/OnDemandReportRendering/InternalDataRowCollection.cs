using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalDataRowCollection : DataRowCollection
	{
		private CustomDataRowList m_dataRowDefs;

		public override DataRow this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					if (base.m_cachedDataRows == null)
					{
						base.m_cachedDataRows = new DataRow[this.Count];
					}
					if (base.m_cachedDataRows[index] == null)
					{
						base.m_cachedDataRows[index] = new InternalDataRow(base.m_owner, index, this.m_dataRowDefs[index]);
					}
					return base.m_cachedDataRows[index];
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public override int Count
		{
			get
			{
				return this.m_dataRowDefs.Count;
			}
		}

		internal InternalDataRowCollection(CustomReportItem owner, CustomDataRowList dataRowDefs)
			: base(owner)
		{
			this.m_dataRowDefs = dataRowDefs;
		}
	}
}
