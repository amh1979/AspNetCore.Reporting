namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimMemberVisibilityInstance : VisibilityInstance
	{
		private ShimMemberVisibility m_owner;

		public override bool CurrentlyHidden
		{
			get
			{
				if (!base.m_cachedCurrentlyHidden)
				{
					base.m_cachedCurrentlyHidden = true;
					base.m_currentlyHiddenValue = this.m_owner.GetInstanceHidden();
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
					base.m_startHiddenValue = this.m_owner.GetInstanceStartHidden();
				}
				return base.m_startHiddenValue;
			}
		}

		internal ShimMemberVisibilityInstance(ShimMemberVisibility owner)
			: base(null)
		{
			this.m_owner = owner;
		}
	}
}
