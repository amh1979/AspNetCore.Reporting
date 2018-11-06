using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class NonComputedUniqueNames
	{
		private int m_uniqueName;

		private NonComputedUniqueNames[] m_childrenUniqueNames;

		internal int UniqueName
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

		internal NonComputedUniqueNames[] ChildrenUniqueNames
		{
			get
			{
				return this.m_childrenUniqueNames;
			}
			set
			{
				this.m_childrenUniqueNames = value;
			}
		}

		private NonComputedUniqueNames(int uniqueName, NonComputedUniqueNames[] childrenUniqueNames)
		{
			this.m_uniqueName = uniqueName;
			this.m_childrenUniqueNames = childrenUniqueNames;
		}

		internal NonComputedUniqueNames()
		{
		}

		internal static NonComputedUniqueNames[] CreateNonComputedUniqueNames(ReportProcessing.ProcessingContext pc, ReportItemCollection reportItemsDef)
		{
			if (reportItemsDef != null && pc != null)
			{
				ReportItemList nonComputedReportItems = reportItemsDef.NonComputedReportItems;
				if (nonComputedReportItems == null)
				{
					return null;
				}
				if (nonComputedReportItems.Count == 0)
				{
					return null;
				}
				NonComputedUniqueNames[] array = new NonComputedUniqueNames[nonComputedReportItems.Count];
				for (int i = 0; i < nonComputedReportItems.Count; i++)
				{
					array[i] = NonComputedUniqueNames.CreateNonComputedUniqueNames(pc, nonComputedReportItems[i]);
				}
				return array;
			}
			return null;
		}

		internal static NonComputedUniqueNames CreateNonComputedUniqueNames(ReportProcessing.ProcessingContext pc, ReportItem reportItemDef)
		{
			if (reportItemDef != null && pc != null)
			{
				NonComputedUniqueNames[] childrenUniqueNames = null;
				if (reportItemDef is Rectangle)
				{
					childrenUniqueNames = NonComputedUniqueNames.CreateNonComputedUniqueNames(pc, ((Rectangle)reportItemDef).ReportItems);
				}
				return new NonComputedUniqueNames(pc.CreateUniqueName(), childrenUniqueNames);
			}
			return null;
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.UniqueName, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.ChildrenUniqueNames, Token.Array, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.NonComputedUniqueNames));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
