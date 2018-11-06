using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TableColumnInstance
	{
		private int m_uniqueName;

		private bool m_startHidden;

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

		internal bool StartHidden
		{
			get
			{
				return this.m_startHidden;
			}
			set
			{
				this.m_startHidden = value;
			}
		}

		internal TableColumnInstance(ReportProcessing.ProcessingContext pc, TableColumn tableColumnDef, Table tableDef)
		{
			this.m_uniqueName = pc.CreateUniqueName();
			if (pc.ShowHideType != 0)
			{
				this.m_startHidden = pc.ProcessReceiver(this.m_uniqueName, tableColumnDef.Visibility, (tableDef.TableExprHost != null) ? tableDef.TableExprHost.TableColumnVisibilityHiddenExpressions : null, tableDef.ObjectType, tableDef.Name);
			}
		}

		internal TableColumnInstance()
		{
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.UniqueName, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.StartHidden, Token.Boolean));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
