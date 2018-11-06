using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimListRow : TablixRow
	{
		private ReportSize m_height;

		private ShimListCell m_cell;

		public override TablixCell this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					return this.m_cell;
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public override int Count
		{
			get
			{
				return 1;
			}
		}

		public override ReportSize Height
		{
			get
			{
				if (this.m_height == null)
				{
					this.m_height = new ReportSize(base.m_owner.RenderList.Height);
				}
				return this.m_height;
			}
		}

		internal ShimListRow(Tablix owner)
			: base(owner, 0)
		{
			this.m_cell = new ShimListCell(owner);
		}

		internal void UpdateCells(ListContent renderContents)
		{
			this.m_cell.SetCellContents(renderContents);
		}
	}
}
