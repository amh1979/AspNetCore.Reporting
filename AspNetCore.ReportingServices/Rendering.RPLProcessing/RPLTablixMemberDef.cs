namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLTablixMemberDef
	{
		private string m_definitionPath;

		private int m_memberCellIndex;

		private int m_level;

		private byte m_state;

		public string DefinitionPath
		{
			get
			{
				return this.m_definitionPath;
			}
			set
			{
				this.m_definitionPath = value;
			}
		}

		public int MemberCellIndex
		{
			get
			{
				return this.m_memberCellIndex;
			}
			set
			{
				this.m_memberCellIndex = value;
			}
		}

		public int Level
		{
			get
			{
				return this.m_level;
			}
			set
			{
				this.m_level = value;
			}
		}

		public bool StaticHeadersTree
		{
			get
			{
				return (this.m_state & 4) > 0;
			}
		}

		public bool IsStatic
		{
			get
			{
				return (this.m_state & 2) > 0;
			}
		}

		public bool IsColumn
		{
			get
			{
				return (this.m_state & 1) > 0;
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

		internal RPLTablixMemberDef()
		{
		}

		internal RPLTablixMemberDef(string definitionPath, int memberCellIndex, byte state, int defTreeLevel)
		{
			this.m_definitionPath = definitionPath;
			this.m_memberCellIndex = memberCellIndex;
			this.m_state = state;
			this.m_level = defTreeLevel;
		}
	}
}
