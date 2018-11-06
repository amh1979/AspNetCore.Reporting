namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportItemVisibility : Visibility
	{
		private ReportItem m_owner;

		public override ReportBoolProperty Hidden
		{
			get
			{
				if (base.m_startHidden == null)
				{
					if (this.m_owner.IsOldSnapshot)
					{
						base.m_startHidden = Visibility.GetStartHidden(this.m_owner.RenderReportItem.ReportItemDef.Visibility);
					}
					else
					{
						base.m_startHidden = Visibility.GetStartHidden(this.m_owner.ReportItemDef.Visibility);
					}
				}
				return base.m_startHidden;
			}
		}

		public override string ToggleItem
		{
			get
			{
				if (this.m_owner.IsOldSnapshot)
				{
					if (this.m_owner.RenderReportItem.ReportItemDef.Visibility != null)
					{
						return this.m_owner.RenderReportItem.ReportItemDef.Visibility.Toggle;
					}
				}
				else if (this.m_owner.ReportItemDef.Visibility != null)
				{
					return this.m_owner.ReportItemDef.Visibility.Toggle;
				}
				return null;
			}
		}

		public override SharedHiddenState HiddenState
		{
			get
			{
				if (this.m_owner.IsOldSnapshot)
				{
					return Visibility.GetHiddenState(this.m_owner.RenderReportItem.ReportItemDef.Visibility);
				}
				return Visibility.GetHiddenState(this.m_owner.ReportItemDef.Visibility);
			}
		}

		public override bool RecursiveToggleReceiver
		{
			get
			{
				if (this.m_owner.IsOldSnapshot)
				{
					if (this.m_owner.RenderReportItem.ReportItemDef.Visibility != null)
					{
						return this.m_owner.RenderReportItem.ReportItemDef.Visibility.RecursiveReceiver;
					}
				}
				else if (this.m_owner.ReportItemDef.Visibility != null)
				{
					return this.m_owner.ReportItemDef.Visibility.RecursiveReceiver;
				}
				return false;
			}
		}

		public ReportItemVisibility(ReportItem owner)
		{
			this.m_owner = owner;
		}
	}
}
