namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class ShimCell : TablixCell
	{
		protected bool m_inSubtotal;

		protected string m_shimID;

		public override string ID
		{
			get
			{
				return base.DefinitionPath;
			}
		}

		public override string DataElementName
		{
			get
			{
				return null;
			}
		}

		public override DataElementOutputTypes DataElementOutput
		{
			get
			{
				return DataElementOutputTypes.ContentsOnly;
			}
		}

		internal ShimCell(Tablix owner, int rowIndex, int colIndex, bool inSubtotal)
			: base(null, owner, rowIndex, colIndex)
		{
			this.m_inSubtotal = inSubtotal;
		}
	}
}
