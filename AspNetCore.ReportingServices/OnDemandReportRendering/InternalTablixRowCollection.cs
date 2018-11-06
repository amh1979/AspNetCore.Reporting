using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalTablixRowCollection : TablixRowCollection
	{
		private TablixRowList m_rowDefs;

		private TablixRow[] m_rowROMDefs;

		public override TablixRow this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					if (this.m_rowROMDefs[index] == null)
					{
						this.m_rowROMDefs[index] = new InternalTablixRow(base.m_owner, index, this.m_rowDefs[index]);
					}
					return this.m_rowROMDefs[index];
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public override int Count
		{
			get
			{
				return this.m_rowDefs.Count;
			}
		}

		internal InternalTablixRowCollection(Tablix owner, TablixRowList rowDefs)
			: base(owner)
		{
			this.m_rowDefs = rowDefs;
			this.m_rowROMDefs = new TablixRow[rowDefs.Count];
		}

		internal IDataRegionRow GetIfExists(int index)
		{
			if (this.m_rowROMDefs != null && index >= 0 && index < this.Count)
			{
				return this.m_rowROMDefs[index];
			}
			return null;
		}
	}
}
