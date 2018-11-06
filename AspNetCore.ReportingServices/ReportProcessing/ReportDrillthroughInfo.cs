using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportDrillthroughInfo
	{
		private TokensHashtable m_rewrittenCommands;

		private DrillthroughHashtable m_drillthroughHashtable;

		internal DrillthroughHashtable DrillthroughInformation
		{
			get
			{
				return this.m_drillthroughHashtable;
			}
			set
			{
				this.m_drillthroughHashtable = value;
			}
		}

		internal TokensHashtable RewrittenCommands
		{
			get
			{
				return this.m_rewrittenCommands;
			}
			set
			{
				this.m_rewrittenCommands = value;
			}
		}

		internal int Count
		{
			get
			{
				if (this.m_drillthroughHashtable == null)
				{
					return 0;
				}
				return this.m_drillthroughHashtable.Count;
			}
		}

		internal ReportDrillthroughInfo()
		{
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.RewrittenCommands, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.TokensHashtable));
			memberInfoList.Add(new MemberInfo(MemberName.DrillthroughHashtable, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.DrillthroughHashtable));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}

		internal void AddDrillthrough(string drillthroughId, DrillthroughInformation drillthroughInfo)
		{
			if (this.m_drillthroughHashtable == null)
			{
				this.m_drillthroughHashtable = new DrillthroughHashtable();
			}
			this.m_drillthroughHashtable.Add(drillthroughId, drillthroughInfo);
		}

		internal void AddRewrittenCommand(int id, object value)
		{
			lock (this)
			{
				if (this.m_rewrittenCommands == null)
				{
					this.m_rewrittenCommands = new TokensHashtable();
				}
				if (!this.m_rewrittenCommands.ContainsKey(id))
				{
					this.m_rewrittenCommands.Add(id, value);
				}
			}
		}
	}
}
