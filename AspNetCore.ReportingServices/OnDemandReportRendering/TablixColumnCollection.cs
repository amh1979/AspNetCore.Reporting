using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class TablixColumnCollection : ReportElementCollectionBase<TablixColumn>
	{
		private Tablix m_owner;

		private TablixColumn[] m_columns;

		public override TablixColumn this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					if (this.m_columns == null)
					{
						this.m_columns = new TablixColumn[this.Count];
					}
					if (this.m_columns[index] == null)
					{
						this.m_columns[index] = new TablixColumn(this.m_owner, index);
					}
					return this.m_columns[index];
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public override int Count
		{
			get
			{
				int result = 0;
				if (this.m_owner.IsOldSnapshot)
				{
					switch (this.m_owner.SnapshotTablixType)
					{
					case DataRegion.Type.List:
						result = 1;
						break;
					case DataRegion.Type.Table:
						result = this.m_owner.RenderTable.Columns.Count;
						break;
					case DataRegion.Type.Matrix:
						result = this.m_owner.MatrixColDefinitionMapping.Length;
						break;
					}
				}
				else
				{
					result = this.m_owner.TablixDef.TablixColumns.Count;
				}
				return result;
			}
		}

		internal TablixColumnCollection(Tablix owner)
		{
			this.m_owner = owner;
		}
	}
}
