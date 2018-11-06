using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class TablixCell : IDataRegionCell, IDefinitionPath, IReportScope
	{
		private Cell m_cell;

		protected Tablix m_owner;

		protected int m_rowIndex;

		protected int m_columnIndex;

		protected CellContents m_cellContents;

		protected TablixCellInstance m_instance;

		protected string m_definitionPath;

		public abstract string ID
		{
			get;
		}

		public string DefinitionPath
		{
			get
			{
				if (this.m_definitionPath == null)
				{
					this.m_definitionPath = DefinitionPathConstants.GetTablixCellDefinitionPath(this.m_owner, this.m_rowIndex, this.m_columnIndex, true);
				}
				return this.m_definitionPath;
			}
		}

		public IDefinitionPath ParentDefinitionPath
		{
			get
			{
				return this.m_owner;
			}
		}

		public abstract CellContents CellContents
		{
			get;
		}

		public abstract DataElementOutputTypes DataElementOutput
		{
			get;
		}

		public abstract string DataElementName
		{
			get;
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
				return this.m_cell;
			}
		}

		internal Cell Cell
		{
			get
			{
				return this.m_cell;
			}
		}

		public virtual TablixCellInstance Instance
		{
			get
			{
				if (this.m_owner.m_renderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new TablixCellInstance(this, this.m_owner, this.m_rowIndex, this.m_columnIndex);
				}
				return this.m_instance;
			}
		}

		internal TablixCell(Cell cell, Tablix owner, int rowIndex, int colIndex)
		{
			this.m_cell = cell;
			this.m_owner = owner;
			this.m_rowIndex = rowIndex;
			this.m_columnIndex = colIndex;
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
			if (this.m_cellContents != null)
			{
				this.m_cellContents.SetNewContext();
			}
			if (this.m_cell != null)
			{
				this.m_cell.ClearStreamingScopeInstanceBinding();
			}
		}
	}
}
