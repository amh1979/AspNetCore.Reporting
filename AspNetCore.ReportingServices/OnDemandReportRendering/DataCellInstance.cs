using System.Globalization;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class DataCellInstance : BaseInstance, IReportScopeInstance
	{
		private DataCell m_dataCellDef;

		private bool m_isNewContext = true;

		string IReportScopeInstance.UniqueName
		{
			get
			{
				if (this.m_dataCellDef.CriDef.IsOldSnapshot)
				{
					return this.m_dataCellDef.CriDef.ID + 'i' + this.m_dataCellDef.RenderItem.RowIndex.ToString(CultureInfo.InvariantCulture) + 'x' + this.m_dataCellDef.RenderItem.ColumnIndex.ToString(CultureInfo.InvariantCulture);
				}
				return this.m_dataCellDef.DataCellDef.UniqueName;
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

		internal DataCellInstance(DataCell dataCellDef)
			: base(dataCellDef)
		{
			this.m_dataCellDef = dataCellDef;
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
