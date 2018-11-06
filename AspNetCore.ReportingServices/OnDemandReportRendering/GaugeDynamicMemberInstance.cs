using AspNetCore.ReportingServices.Common;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class GaugeDynamicMemberInstance : GaugeMemberInstance, IDynamicInstance, IReportScopeInstance
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

		internal GaugeDynamicMemberInstance(GaugePanel owner, GaugeMember memberDef, InternalDynamicMemberLogic memberLogic)
			: base(owner, memberDef)
		{
			this.m_memberLogic = memberLogic;
			this.ResetContext();
		}

		public bool MoveNext()
		{
			return this.m_memberLogic.MoveNext();
		}

		public bool SetInstanceIndex(int index)
		{
			return this.m_memberLogic.SetInstanceIndex(index);
		}

		public void ResetContext()
		{
			this.m_memberLogic.ResetContext();
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

		public int GetInstanceIndex()
		{
			return this.m_memberLogic.GetInstanceIndex();
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
