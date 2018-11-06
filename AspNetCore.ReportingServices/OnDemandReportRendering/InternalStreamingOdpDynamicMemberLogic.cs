using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalStreamingOdpDynamicMemberLogic : InternalStreamingOdpDynamicMemberLogicBase
	{
		public InternalStreamingOdpDynamicMemberLogic(DataRegionMember memberDef, OnDemandProcessingContext odpContext)
			: base(memberDef, odpContext)
		{
		}

		public override bool MoveNext()
		{
			base.ResetScopeID();
			return base.MoveNextCore(null);
		}

		internal bool RomBasedRestart(ScopeID targetScopeID)
		{
			if (targetScopeID == (ScopeID)null)
			{
				return false;
			}
			try
			{
				IEqualityComparer<object> processingComparer = base.m_odpContext.ProcessingComparer;
				bool result = true;
				while (!targetScopeID.Equals(this.GetScopeID(), processingComparer) && (result = this.MoveNext()))
				{
				}
				return result;
			}
			catch (Exception)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsRombasedRestartFailedTypeMismatch, base.m_memberDef.Group.Name);
			}
		}

		internal override void SetScopeID(ScopeID scopeID)
		{
			if (base.m_grouping.IsDetail)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsDetailGroupsNotSupportedInStreamingMode, "SetScopeID");
			}
			if (scopeID == (ScopeID)null)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidScopeID, "SetScopeID");
			}
			if (base.m_odpContext.QueryRestartInfo.TryAddScopeID(scopeID, base.m_memberDef.DataRegionMemberDefinition, this))
			{
				return;
			}
			throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidScopeIDOrder, base.m_grouping.Name, "SetScopeID");
		}
	}
}
