using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalShimDynamicMemberLogic : InternalDynamicMemberLogic
	{
		private readonly IShimDataRegionMember m_shimMember;

		public InternalShimDynamicMemberLogic(IShimDataRegionMember shimMember)
		{
			this.m_shimMember = shimMember;
		}

		public override void ResetContext()
		{
			base.m_isNewContext = true;
			base.m_currentContext = -1;
			this.m_shimMember.ResetContext();
		}

		public override bool MoveNext()
		{
			if (!this.m_shimMember.SetNewContext(base.m_currentContext + 1))
			{
				return false;
			}
			base.m_currentContext++;
			return true;
		}

		public override bool SetInstanceIndex(int index)
		{
			if (index < 0)
			{
				this.ResetContext();
				return true;
			}
			if (this.m_shimMember.SetNewContext(index))
			{
				base.m_currentContext = index;
				return true;
			}
			return false;
		}

		internal override ScopeID GetScopeID()
		{
			throw new RenderingObjectModelException(ProcessingErrorCode.rsNotSupportedInStreamingMode, "GetScopeID");
		}

		internal override ScopeID GetLastScopeID()
		{
			throw new RenderingObjectModelException(ProcessingErrorCode.rsNotSupportedInStreamingMode, "GetLastScopeID");
		}

		internal override void SetScopeID(ScopeID scopeID)
		{
			throw new RenderingObjectModelException(ProcessingErrorCode.rsNotSupportedInStreamingMode, "SetScopeID");
		}
	}
}
