using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class SizeCollection
	{
		private Matrix m_owner;

		private bool m_widthsCollection;

		private ReportSizeCollection m_reportSizeCollection;

		public ReportSize this[int index]
		{
			get
			{
				ReportSize reportSize;
				ReportSizeCollection reportSizeCollection;
				if (index >= 0 && index < this.Count)
				{
					reportSize = null;
					if (this.m_reportSizeCollection != null && this.m_reportSizeCollection[index] != null)
					{
						reportSize = this.m_reportSizeCollection[index];
					}
					if (reportSize == null)
					{
						AspNetCore.ReportingServices.ReportProcessing.Matrix matrix = (AspNetCore.ReportingServices.ReportProcessing.Matrix)this.m_owner.ReportItemDef;
						MatrixInstance matrixInstance = (MatrixInstance)this.m_owner.ReportItemInstance;
						reportSizeCollection = null;
						reportSizeCollection = ((!this.m_widthsCollection) ? matrix.CellHeightsForRendering : matrix.CellWidthsForRendering);
						Global.Tracer.Assert(null != reportSizeCollection);
						if (this.m_owner.NoRows || matrixInstance == null || matrixInstance.Cells.Count == 0)
						{
							reportSize = reportSizeCollection[index];
						}
						else
						{
							if (this.m_widthsCollection && matrix.StaticColumns == null)
							{
								goto IL_0107;
							}
							if (!this.m_widthsCollection && matrix.StaticRows == null)
							{
								goto IL_0107;
							}
							bool cacheState = this.m_owner.RenderingContext.CacheState;
							this.m_owner.RenderingContext.CacheState = true;
							MatrixCellCollection cellCollection = this.m_owner.CellCollection;
							MatrixCell matrixCell = null;
							matrixCell = ((!this.m_widthsCollection) ? cellCollection[index, 0] : cellCollection[0, index]);
							reportSize = reportSizeCollection[matrixCell.ColumnIndex];
							this.m_owner.RenderingContext.CacheState = cacheState;
						}
						goto IL_0184;
					}
					goto IL_01bc;
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
				IL_01bc:
				return reportSize;
				IL_0107:
				reportSize = reportSizeCollection[0];
				goto IL_0184;
				IL_0184:
				if (this.m_owner.RenderingContext.CacheState)
				{
					if (this.m_reportSizeCollection == null)
					{
						this.m_reportSizeCollection = new ReportSizeCollection(this.Count);
					}
					this.m_reportSizeCollection[index] = reportSize;
				}
				goto IL_01bc;
			}
		}

		public int Count
		{
			get
			{
				MatrixInstance matrixInstance = (MatrixInstance)this.m_owner.ReportItemInstance;
				if (!this.m_owner.NoRows && matrixInstance != null && matrixInstance.Cells.Count != 0)
				{
					if (this.m_widthsCollection)
					{
						return this.m_owner.CellColumns;
					}
					return this.m_owner.CellRows;
				}
				AspNetCore.ReportingServices.ReportProcessing.Matrix matrix = (AspNetCore.ReportingServices.ReportProcessing.Matrix)this.m_owner.ReportItemDef;
				ReportSizeCollection reportSizeCollection = null;
				reportSizeCollection = ((!this.m_widthsCollection) ? matrix.CellHeightsForRendering : matrix.CellWidthsForRendering);
				Global.Tracer.Assert(null != reportSizeCollection);
				return reportSizeCollection.Count;
			}
		}

		internal SizeCollection(Matrix owner, bool widthsCollection)
		{
			this.m_owner = owner;
			this.m_widthsCollection = widthsCollection;
		}
	}
}
