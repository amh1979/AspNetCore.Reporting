using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalTablixMemberVisibilityInstance : VisibilityInstance
	{
		private InternalTablixMember m_owner;

		public override bool CurrentlyHidden
		{
			get
			{
				if (!base.m_cachedCurrentlyHidden)
				{
					base.m_cachedCurrentlyHidden = true;
					AspNetCore.ReportingServices.ReportIntermediateFormat.TablixMember memberDefinition = this.m_owner.MemberDefinition;
					ToggleCascadeDirection direction = (ToggleCascadeDirection)((!memberDefinition.IsColumn) ? 1 : 2);
					base.m_currentlyHiddenValue = memberDefinition.ComputeHidden(this.m_owner.OwnerTablix.RenderingContext, direction);
				}
				return base.m_currentlyHiddenValue;
			}
		}

		public override bool StartHidden
		{
			get
			{
				if (!base.m_cachedStartHidden)
				{
					base.m_cachedStartHidden = true;
					if (this.m_owner.MemberDefinition.Visibility == null || this.m_owner.MemberDefinition.Visibility.Hidden == null)
					{
						base.m_startHiddenValue = false;
					}
					else
					{
						base.m_startHiddenValue = this.m_owner.MemberDefinition.ComputeStartHidden(this.m_owner.OwnerTablix.RenderingContext);
					}
				}
				return base.m_startHiddenValue;
			}
		}

		internal InternalTablixMemberVisibilityInstance(InternalTablixMember owner)
			: base(owner.ReportScope)
		{
			this.m_owner = owner;
		}
	}
}
