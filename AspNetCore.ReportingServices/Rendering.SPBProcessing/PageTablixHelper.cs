using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal class PageTablixHelper : PageItemHelper
	{
		private int m_levelForRepeat;

		private List<int> m_tablixCreateState;

		private List<int> m_membersInstanceIndex;

		private bool m_ignoreTotalsOnLastLevel;

		internal int LevelForRepeat
		{
			get
			{
				return this.m_levelForRepeat;
			}
			set
			{
				this.m_levelForRepeat = value;
			}
		}

		internal bool IgnoreTotalsOnLastLevel
		{
			get
			{
				return this.m_ignoreTotalsOnLastLevel;
			}
			set
			{
				this.m_ignoreTotalsOnLastLevel = value;
			}
		}

		internal List<int> TablixCreateState
		{
			get
			{
				return this.m_tablixCreateState;
			}
			set
			{
				this.m_tablixCreateState = value;
			}
		}

		internal List<int> MembersInstanceIndex
		{
			get
			{
				return this.m_membersInstanceIndex;
			}
			set
			{
				this.m_membersInstanceIndex = value;
			}
		}

		internal PageTablixHelper(byte type)
			: base(type)
		{
		}
	}
}
