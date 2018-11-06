using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimListMemberVisibility : ShimMemberVisibility
	{
		private ShimListMember m_owner;

		public override ReportBoolProperty Hidden
		{
			get
			{
				if (base.m_startHidden == null && this.m_owner.Group != null)
				{
					base.m_startHidden = Visibility.GetStartHidden(this.m_owner.OwnerTablix.RenderList.ReportItemDef.Visibility);
				}
				return base.m_startHidden;
			}
		}

		public override string ToggleItem
		{
			get
			{
				if (this.m_owner.Group != null && this.m_owner.OwnerTablix.RenderList.ReportItemDef.Visibility != null)
				{
					return this.m_owner.OwnerTablix.RenderList.ReportItemDef.Visibility.Toggle;
				}
				return null;
			}
		}

		public override bool RecursiveToggleReceiver
		{
			get
			{
				if (this.m_owner.Group != null && this.m_owner.OwnerTablix.RenderList.ReportItemDef.Visibility != null)
				{
					return this.m_owner.OwnerTablix.RenderList.ReportItemDef.Visibility.RecursiveReceiver;
				}
				return false;
			}
		}

		public override SharedHiddenState HiddenState
		{
			get
			{
				if (this.m_owner.Group != null)
				{
					return Visibility.GetHiddenState(this.m_owner.OwnerTablix.RenderList.ReportItemDef.Visibility);
				}
				return SharedHiddenState.Never;
			}
		}

		public ShimListMemberVisibility(ShimListMember owner)
		{
			this.m_owner = owner;
		}

		internal override bool GetInstanceHidden()
		{
			if (this.m_owner.Group != null)
			{
				return this.m_owner.Group.CurrentShimRenderGroup.Hidden;
			}
			return false;
		}

		internal override bool GetInstanceStartHidden()
		{
			if (this.m_owner.Group != null)
			{
				if (((ListContent)this.m_owner.Group.CurrentShimRenderGroup).InstanceInfo != null)
				{
					return ((ListContent)this.m_owner.Group.CurrentShimRenderGroup).InstanceInfo.StartHidden;
				}
				return this.GetInstanceHidden();
			}
			return false;
		}
	}
}
