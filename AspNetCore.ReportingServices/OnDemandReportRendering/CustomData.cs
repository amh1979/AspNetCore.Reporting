using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class CustomData : IReportScopeInstance
	{
		private CustomReportItem m_owner;

		private DataHierarchy m_columns;

		private DataHierarchy m_rows;

		private DataRowCollection m_rowCollection;

		private bool m_isNewContext = true;

		public string DataSetName
		{
			get
			{
				if (this.m_owner.IsOldSnapshot)
				{
					return this.m_owner.RenderCri.CriDefinition.DataSetName;
				}
				return ((AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion)this.m_owner.ReportItemDef).DataSetName;
			}
		}

		public DataHierarchy DataColumnHierarchy
		{
			get
			{
				if (this.m_columns == null)
				{
					if (this.m_owner.IsOldSnapshot)
					{
						if (this.m_owner.RenderCri.CustomData.DataColumnGroupings != null)
						{
							this.m_columns = new DataHierarchy(this.m_owner, true);
						}
					}
					else if (this.m_owner.CriDef.DataColumnMembers != null)
					{
						this.m_columns = new DataHierarchy(this.m_owner, true);
					}
				}
				return this.m_columns;
			}
		}

		public DataHierarchy DataRowHierarchy
		{
			get
			{
				if (this.m_rows == null)
				{
					if (this.m_owner.IsOldSnapshot)
					{
						if (this.m_owner.RenderCri.CustomData.DataRowGroupings != null)
						{
							this.m_rows = new DataHierarchy(this.m_owner, false);
						}
					}
					else if (this.m_owner.CriDef.DataRowMembers != null)
					{
						this.m_rows = new DataHierarchy(this.m_owner, false);
					}
				}
				return this.m_rows;
			}
		}

		internal bool HasDataRowCollection
		{
			get
			{
				return this.m_rowCollection != null;
			}
		}

		public DataRowCollection RowCollection
		{
			get
			{
				if (this.m_rowCollection == null)
				{
					if (this.m_owner.IsOldSnapshot)
					{
						if (this.m_owner.RenderCri.CustomData.DataCells != null)
						{
							this.m_rowCollection = new ShimDataRowCollection(this.m_owner);
						}
					}
					else if (this.m_owner.CriDef.DataRows != null)
					{
						this.m_rowCollection = new InternalDataRowCollection(this.m_owner, this.m_owner.CriDef.DataRows);
					}
				}
				return this.m_rowCollection;
			}
		}

		string IReportScopeInstance.UniqueName
		{
			get
			{
				return this.m_owner.InstanceUniqueName;
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
				return this.m_owner.ReportScope;
			}
		}

		internal CustomData(CustomReportItem owner)
		{
			this.m_owner = owner;
		}

		internal void SetNewContext()
		{
			this.m_isNewContext = true;
			if (this.m_rows != null)
			{
				this.m_rows.SetNewContext();
			}
			if (this.m_columns != null)
			{
				this.m_columns.SetNewContext();
			}
		}
	}
}
