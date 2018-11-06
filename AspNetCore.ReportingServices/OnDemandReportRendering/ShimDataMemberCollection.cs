using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimDataMemberCollection : DataMemberCollection
	{
		private bool m_isStatic;

		private bool m_isColumnMember;

		private int m_definitionStartIndex = -1;

		private int m_definitionEndIndex = -1;

		private DataGroupingCollection m_definitionGroups;

		public override DataMember this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					return (DataMember)base.m_children[index];
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public override int Count
		{
			get
			{
				return base.m_children.Length;
			}
		}

		internal ShimDataMemberCollection(IDefinitionPath parentDefinitionPath, CustomReportItem owner, bool isColumnMember, ShimDataMember parent, DataGroupingCollection definitionGroups)
			: base(parentDefinitionPath, owner)
		{
			this.m_isColumnMember = isColumnMember;
			this.m_definitionGroups = definitionGroups;
			this.m_definitionStartIndex = owner.GetCurrentMemberCellDefinitionIndex();
			if (definitionGroups[0] != null && definitionGroups[0][0] != null)
			{
				this.m_isStatic = definitionGroups[0][0].IsStatic;
			}
			int count = definitionGroups.Count;
			base.m_children = new ShimDataMember[count];
			for (int i = 0; i < count; i++)
			{
				base.m_children[i] = new ShimDataMember(this, owner, parent, i, this.m_isColumnMember, this.m_isStatic, definitionGroups[i], i);
			}
			this.m_definitionEndIndex = owner.GetCurrentMemberCellDefinitionIndex();
		}

		internal void UpdateContext()
		{
			if (base.m_children != null)
			{
				if (this.m_isColumnMember)
				{
					this.ResetContext(base.OwnerCri.RenderCri.CustomData.DataColumnGroupings);
				}
				else
				{
					this.ResetContext(base.OwnerCri.RenderCri.CustomData.DataRowGroupings);
				}
			}
		}

		internal void ResetContext(DataGroupingCollection definitionGroups)
		{
			if (base.m_children != null)
			{
				if (definitionGroups != null)
				{
					this.m_definitionGroups = definitionGroups;
				}
				if (this.m_isStatic)
				{
					for (int i = 0; i < base.m_children.Length; i++)
					{
						((ShimDataMember)base.m_children[i]).ResetContext(this.m_definitionGroups[i]);
					}
				}
			}
		}
	}
}
