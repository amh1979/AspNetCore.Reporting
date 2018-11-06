namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalTablixMemberVisibility : Visibility
	{
		private InternalTablixMember m_owner;

		public override ReportBoolProperty Hidden
		{
			get
			{
				if (base.m_startHidden == null)
				{
					base.m_startHidden = Visibility.GetStartHidden(this.m_owner.MemberDefinition.Visibility);
				}
				return base.m_startHidden;
			}
		}

		public override string ToggleItem
		{
			get
			{
				if (this.m_owner.MemberDefinition.Visibility != null)
				{
					return this.m_owner.MemberDefinition.Visibility.Toggle;
				}
				return null;
			}
		}

		public override SharedHiddenState HiddenState
		{
			get
			{
				return Visibility.GetHiddenState(this.m_owner.MemberDefinition.Visibility);
			}
		}

		public override bool RecursiveToggleReceiver
		{
			get
			{
				if (this.m_owner.MemberDefinition.Visibility != null)
				{
					return this.m_owner.MemberDefinition.Visibility.RecursiveReceiver;
				}
				return false;
			}
		}

		public InternalTablixMemberVisibility(InternalTablixMember owner)
		{
			this.m_owner = owner;
		}
	}
}
