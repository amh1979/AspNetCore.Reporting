using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalFullOdpDynamicMemberLogic : InternalDynamicMemberLogic
	{
		private readonly DataRegionMember m_memberDef;

		private readonly OnDemandProcessingContext m_odpContext;

		public InternalFullOdpDynamicMemberLogic(DataRegionMember memberDef, OnDemandProcessingContext odpContext)
		{
			this.m_memberDef = memberDef;
			this.m_odpContext = odpContext;
		}

		public override void ResetContext()
		{
			base.m_isNewContext = true;
			base.m_currentContext = -1;
			this.m_memberDef.DataRegionMemberDefinition.InstanceCount = -1;
			this.m_memberDef.DataRegionMemberDefinition.InstancePathItem.ResetContext();
		}

		public override bool MoveNext()
		{
			if (!this.IsContextValid(base.m_currentContext + 1))
			{
				return false;
			}
			base.m_isNewContext = true;
			base.m_currentContext++;
			this.m_memberDef.DataRegionMemberDefinition.InstancePathItem.MoveNext();
			this.m_memberDef.SetNewContext(true);
			return true;
		}

		public override bool SetInstanceIndex(int index)
		{
			if (index < 0)
			{
				this.ResetContext();
				return true;
			}
			if (this.IsContextValid(index))
			{
				base.m_currentContext = index;
				this.m_memberDef.DataRegionMemberDefinition.InstancePathItem.SetContext(base.m_currentContext);
				this.m_memberDef.SetNewContext(true);
				base.m_isNewContext = true;
				return true;
			}
			return false;
		}

		private bool IsContextValid(int context)
		{
			if (this.m_memberDef.DataRegionMemberDefinition.InstanceCount < 0)
			{
				this.m_odpContext.SetupContext(this.m_memberDef.DataRegionMemberDefinition, this.m_memberDef.ReportScopeInstance, context);
			}
			return context < this.m_memberDef.DataRegionMemberDefinition.InstanceCount;
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
