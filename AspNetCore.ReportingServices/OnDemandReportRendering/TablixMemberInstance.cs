namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class TablixMemberInstance : BaseInstance
	{
		protected Tablix m_owner;

		protected TablixMember m_memberDef;

		protected VisibilityInstance m_visibility;

		public virtual VisibilityInstance Visibility
		{
			get
			{
				if (this.m_visibility == null && this.m_memberDef.Visibility != null)
				{
					if (this.m_owner.IsOldSnapshot)
					{
						this.m_visibility = new ShimMemberVisibilityInstance((ShimMemberVisibility)this.m_memberDef.Visibility);
					}
					else
					{
						this.m_visibility = new InternalTablixMemberVisibilityInstance((InternalTablixMember)this.m_memberDef);
					}
				}
				return this.m_visibility;
			}
		}

		internal TablixMemberInstance(Tablix owner, TablixMember memberDef)
			: base(memberDef.ReportScope)
		{
			this.m_owner = owner;
			this.m_memberDef = memberDef;
		}

		protected override void ResetInstanceCache()
		{
			if (this.m_visibility != null)
			{
				this.m_visibility.SetNewContext();
			}
		}
	}
}
