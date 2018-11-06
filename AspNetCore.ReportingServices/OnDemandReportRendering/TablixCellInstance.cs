namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class TablixCellInstance : BaseInstance, IReportScopeInstance
	{
		private TablixCell m_cellDef;

		private Tablix m_owner;

		private int m_rowIndex;

		private int m_columnIndex;

		private bool m_isNewContext = true;

		string IReportScopeInstance.UniqueName
		{
			get
			{
				return this.m_cellDef.Cell.UniqueName;
			}
		}

		bool IReportScopeInstance.IsNewContext
		{
			get
			{
				return this.m_isNewContext;
			}
			set
			{
				this.m_isNewContext = value;
			}
		}

		IReportScope IReportScopeInstance.ReportScope
		{
			get
			{
				return base.m_reportScope;
			}
		}

		internal TablixCellInstance(TablixCell cellDef, Tablix owner, int rowIndex, int colIndex)
			: base(cellDef)
		{
			this.m_cellDef = cellDef;
			this.m_owner = owner;
			this.m_rowIndex = rowIndex;
			this.m_columnIndex = colIndex;
		}

		internal override void SetNewContext()
		{
			if (!this.m_isNewContext)
			{
				this.m_isNewContext = true;
				base.SetNewContext();
			}
		}

		protected override void ResetInstanceCache()
		{
		}
	}
}
