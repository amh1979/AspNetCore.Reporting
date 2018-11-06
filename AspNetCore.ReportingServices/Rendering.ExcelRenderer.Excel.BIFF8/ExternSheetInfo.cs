using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal class ExternSheetInfo
	{
		internal class XTI
		{
			internal const int LENGTH = 6;

			private ushort m_firstTab;

			private ushort m_lastTab;

			private ushort m_supBookIndex;

			internal ushort SupBookIndex
			{
				get
				{
					return this.m_supBookIndex;
				}
			}

			internal ushort FirstTab
			{
				get
				{
					return this.m_firstTab;
				}
			}

			internal ushort LastTab
			{
				get
				{
					return this.m_lastTab;
				}
			}

			internal XTI(ushort supBookIndex, ushort firstTab, ushort lastTab)
			{
				this.m_firstTab = firstTab;
				this.m_lastTab = lastTab;
				this.m_supBookIndex = supBookIndex;
			}
		}

		private List<XTI> m_xtiStructures;

		internal List<XTI> XTIStructures
		{
			get
			{
				return this.m_xtiStructures;
			}
		}

		internal ExternSheetInfo()
		{
			this.m_xtiStructures = new List<XTI>();
		}

		internal int AddXTI(ushort supBookIndex, ushort firstTab, ushort lastTab)
		{
			this.m_xtiStructures.Add(new XTI(supBookIndex, firstTab, lastTab));
			return this.m_xtiStructures.Count - 1;
		}
	}
}
