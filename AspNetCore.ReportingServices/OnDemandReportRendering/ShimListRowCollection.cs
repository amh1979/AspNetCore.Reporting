using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimListRowCollection : TablixRowCollection
	{
		private ShimListRow m_row;

		public override TablixRow this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					return this.m_row;
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

		internal ShimListRowCollection(Tablix owner)
			: base(owner)
		{
			this.m_row = new ShimListRow(owner);
		}
	}
}
