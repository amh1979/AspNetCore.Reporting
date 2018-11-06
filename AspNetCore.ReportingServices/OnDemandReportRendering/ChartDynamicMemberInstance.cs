using AspNetCore.ReportingServices.Common;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartDynamicMemberInstance : ChartMemberInstance, IDynamicInstance, IReportScopeInstance
	{
		private readonly InternalDynamicMemberLogic m_memberLogic;

		string IReportScopeInstance.UniqueName
		{
			get
			{
				return base.m_memberDef.UniqueName;
			}
		}

		bool IReportScopeInstance.IsNewContext
		{
			get
			{
				return this.m_memberLogic.IsNewContext;
			}
			set
			{
				this.m_memberLogic.IsNewContext = value;
			}
		}

		IReportScope IReportScopeInstance.ReportScope
		{
			get
			{
				return base.m_reportScope;
			}
		}

		internal ChartDynamicMemberInstance(Chart owner, ChartMember memberDef, InternalDynamicMemberLogic memberLogic)
			: base(owner, memberDef)
		{
			this.m_memberLogic = memberLogic;
			this.ResetContext();
		}

		void IDynamicInstance.ResetContext()
		{
			this.ResetContext();
		}

		bool IDynamicInstance.MoveNext()
		{
			return this.MoveNext();
		}

		int IDynamicInstance.GetInstanceIndex()
		{
			return this.GetInstanceIndex();
		}

		bool IDynamicInstance.SetInstanceIndex(int index)
		{
			return this.SetInstanceIndex(index);
		}

		ScopeID IDynamicInstance.GetScopeID()
		{
			return this.GetScopeID();
		}

		void IDynamicInstance.SetScopeID(ScopeID scopeID)
		{
			this.SetScopeID(scopeID);
		}

		public void ResetContext()
		{
			this.m_memberLogic.ResetContext();
		}

		public bool MoveNext()
		{
			return this.m_memberLogic.MoveNext();
		}

		public int GetInstanceIndex()
		{
			return this.m_memberLogic.GetInstanceIndex();
		}

		public bool SetInstanceIndex(int index)
		{
			return this.m_memberLogic.SetInstanceIndex(index);
		}

		internal ScopeID GetScopeID()
		{
			return this.m_memberLogic.GetScopeID();
		}

		internal void SetScopeID(ScopeID scopeID)
		{
			this.m_memberLogic.SetScopeID(scopeID);
		}
	}
}
