using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimMatrixMemberVisibility : ShimMemberVisibility
	{
		private ShimMatrixMember m_owner;

		public override ReportBoolProperty Hidden
		{
			get
			{
				if (base.m_startHidden == null)
				{
					base.m_startHidden = Visibility.GetStartHidden(this.m_owner.Group.CurrentShimRenderGroup.m_visibilityDef);
				}
				return base.m_startHidden;
			}
		}

		public override string ToggleItem
		{
			get
			{
				if (this.m_owner.Group.CurrentShimRenderGroup.m_visibilityDef != null)
				{
					return this.m_owner.Group.CurrentShimRenderGroup.m_visibilityDef.Toggle;
				}
				return null;
			}
		}

		public override bool RecursiveToggleReceiver
		{
			get
			{
				if (this.m_owner.Group.CurrentShimRenderGroup.m_visibilityDef != null)
				{
					return this.m_owner.Group.CurrentShimRenderGroup.m_visibilityDef.RecursiveReceiver;
				}
				return false;
			}
		}

		public override SharedHiddenState HiddenState
		{
			get
			{
				return Visibility.GetHiddenState(this.m_owner.Group.CurrentShimRenderGroup.m_visibilityDef);
			}
		}

		public ShimMatrixMemberVisibility(ShimMatrixMember owner)
		{
			this.m_owner = owner;
		}

		internal override bool GetInstanceHidden()
		{
			return this.m_owner.Group.CurrentShimRenderGroup.Hidden;
		}

		internal override bool GetInstanceStartHidden()
		{
			if (this.m_owner.Group != null)
			{
				if (((MatrixMember)this.m_owner.Group.CurrentShimRenderGroup).InstanceInfo != null)
				{
					return ((MatrixMember)this.m_owner.Group.CurrentShimRenderGroup).InstanceInfo.StartHidden;
				}
				return this.GetInstanceHidden();
			}
			return false;
		}
	}
}
