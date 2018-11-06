using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class TablixBody
	{
		private Tablix m_owner;

		private TablixRowCollection m_rowCollection;

		private TablixColumnCollection m_columnCollection;

		private bool? m_ignoreCellPageBreaks = null;

		internal bool HasRowCollection
		{
			get
			{
				return null != this.m_rowCollection;
			}
		}

		public TablixRowCollection RowCollection
		{
			get
			{
				if (this.m_rowCollection == null)
				{
					if (this.m_owner.IsOldSnapshot)
					{
						switch (this.m_owner.SnapshotTablixType)
						{
						case DataRegion.Type.List:
							this.m_rowCollection = new ShimListRowCollection(this.m_owner);
							break;
						case DataRegion.Type.Table:
							this.m_rowCollection = new ShimTableRowCollection(this.m_owner);
							break;
						case DataRegion.Type.Matrix:
							this.m_rowCollection = new ShimMatrixRowCollection(this.m_owner);
							break;
						}
					}
					else
					{
						this.m_rowCollection = new InternalTablixRowCollection(this.m_owner, this.m_owner.TablixDef.TablixRows);
					}
				}
				return this.m_rowCollection;
			}
		}

		public TablixColumnCollection ColumnCollection
		{
			get
			{
				if (this.m_columnCollection == null)
				{
					this.m_columnCollection = new TablixColumnCollection(this.m_owner);
				}
				return this.m_columnCollection;
			}
		}

		public bool IgnoreCellPageBreaks
		{
			get
			{
				if (!this.m_ignoreCellPageBreaks.HasValue)
				{
					if (this.m_owner.IsOldSnapshot)
					{
						this.m_ignoreCellPageBreaks = (DataRegion.Type.List != this.m_owner.SnapshotTablixType);
					}
					else
					{
						this.m_ignoreCellPageBreaks = true;
						AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix tablixDef = this.m_owner.TablixDef;
						if (tablixDef.ColumnCount == 1 && tablixDef.ColumnMembers[0].IsStatic && ((AspNetCore.ReportingServices.ReportIntermediateFormat.TablixMember)tablixDef.ColumnMembers[0]).TablixHeader == null)
						{
							TablixMemberList members = (TablixMemberList)tablixDef.RowMembers;
							this.m_ignoreCellPageBreaks = this.HasHeader(members);
						}
					}
				}
				return this.m_ignoreCellPageBreaks.Value;
			}
		}

		internal TablixBody(Tablix owner)
		{
			this.m_owner = owner;
		}

		private bool HasHeader(TablixMemberList members)
		{
			if (members != null)
			{
				for (int i = 0; i < members.Count; i++)
				{
					if (members[i].TablixHeader != null || this.HasHeader(members[i].SubMembers))
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
