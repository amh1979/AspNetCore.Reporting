namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLTablixMemberCell : RPLTablixCell
	{
		private string m_uniqueName;

		private string m_groupLabel;

		private int m_recursiveToggleLevel = -1;

		private byte m_state;

		private int m_rowSpan = 1;

		private RPLTablixMemberDef m_memberDef;

		public string GroupLabel
		{
			get
			{
				return this.m_groupLabel;
			}
			set
			{
				this.m_groupLabel = value;
			}
		}

		public int RecursiveToggleLevel
		{
			get
			{
				return this.m_recursiveToggleLevel;
			}
			set
			{
				this.m_recursiveToggleLevel = value;
			}
		}

		public bool IsRecursiveToggle
		{
			get
			{
				return this.m_recursiveToggleLevel >= 0;
			}
		}

		public string UniqueName
		{
			get
			{
				return this.m_uniqueName;
			}
			set
			{
				this.m_uniqueName = value;
			}
		}

		public bool HasToggle
		{
			get
			{
				return (this.m_state & 1) > 0;
			}
		}

		public bool ToggleCollapse
		{
			get
			{
				return (this.m_state & 2) > 0;
			}
		}

		public bool IsInnerMost
		{
			get
			{
				return (this.m_state & 4) > 0;
			}
		}

		public override int RowSpan
		{
			get
			{
				return this.m_rowSpan;
			}
			set
			{
				this.m_rowSpan = value;
			}
		}

		public RPLTablixMemberDef TablixMemberDef
		{
			get
			{
				return this.m_memberDef;
			}
			set
			{
				this.m_memberDef = value;
			}
		}

		internal byte State
		{
			get
			{
				return this.m_state;
			}
			set
			{
				this.m_state = value;
			}
		}

		internal RPLTablixMemberCell()
		{
		}

		internal RPLTablixMemberCell(RPLItem element, byte elementState, int rowSpan, int colSpan)
			: base(element, elementState)
		{
			this.m_rowSpan = rowSpan;
			base.m_colSpan = colSpan;
		}
	}
}
