using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class DataCell : IReportScope, IDataRegionCell
	{
		protected int m_rowIndex;

		protected int m_columnIndex;

		protected CustomReportItem m_owner;

		protected DataValueCollection m_dataValues;

		protected DataCellInstance m_instance;

		public abstract DataValueCollection DataValues
		{
			get;
		}

		internal abstract AspNetCore.ReportingServices.ReportIntermediateFormat.DataCell DataCellDef
		{
			get;
		}

		internal abstract AspNetCore.ReportingServices.ReportRendering.DataCell RenderItem
		{
			get;
		}

		internal CustomReportItem CriDef
		{
			get
			{
				return this.m_owner;
			}
		}

		public DataCellInstance Instance
		{
			get
			{
				if (this.m_owner.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new DataCellInstance(this);
				}
				return this.m_instance;
			}
		}

		IReportScopeInstance IReportScope.ReportScopeInstance
		{
			get
			{
				return this.Instance;
			}
		}

		IRIFReportScope IReportScope.RIFReportScope
		{
			get
			{
				return this.RIFReportScope;
			}
		}

		internal virtual IRIFReportScope RIFReportScope
		{
			get
			{
				return null;
			}
		}

		internal DataCell(CustomReportItem owner, int rowIndex, int colIndex)
		{
			this.m_owner = owner;
			this.m_rowIndex = rowIndex;
			this.m_columnIndex = colIndex;
			this.m_dataValues = null;
		}

		void IDataRegionCell.SetNewContext()
		{
			this.SetNewContext();
		}

		internal virtual void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
			if (this.m_dataValues != null)
			{
				this.m_dataValues.SetNewContext();
			}
		}
	}
}
