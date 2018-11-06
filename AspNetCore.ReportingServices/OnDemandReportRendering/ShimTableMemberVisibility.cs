using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimTableMemberVisibility : ShimMemberVisibility
	{
		internal enum Mode
		{
			StaticColumn,
			StaticRow,
			TableGroup,
			TableDetails
		}

		private ShimTableMember m_owner;

		private Mode m_mode;

		public override ReportBoolProperty Hidden
		{
			get
			{
				if (base.m_startHidden == null)
				{
					base.m_startHidden = Visibility.GetStartHidden(this.GetVisibilityDefinition());
				}
				return base.m_startHidden;
			}
		}

		public override string ToggleItem
		{
			get
			{
				AspNetCore.ReportingServices.ReportProcessing.Visibility visibilityDefinition = this.GetVisibilityDefinition();
				if (visibilityDefinition != null)
				{
					return visibilityDefinition.Toggle;
				}
				return null;
			}
		}

		public override bool RecursiveToggleReceiver
		{
			get
			{
				AspNetCore.ReportingServices.ReportProcessing.Visibility visibilityDefinition = this.GetVisibilityDefinition();
				if (visibilityDefinition != null)
				{
					return visibilityDefinition.RecursiveReceiver;
				}
				return false;
			}
		}

		public override SharedHiddenState HiddenState
		{
			get
			{
				return Visibility.GetHiddenState(this.GetVisibilityDefinition());
			}
		}

		public ShimTableMemberVisibility(ShimTableMember owner, Mode mode)
		{
			this.m_owner = owner;
			this.m_mode = mode;
		}

		private AspNetCore.ReportingServices.ReportProcessing.Visibility GetVisibilityDefinition()
		{
			switch (this.m_mode)
			{
			case Mode.StaticColumn:
				return this.m_owner.RenderTableColumn.ColumnDefinition.Visibility;
			case Mode.StaticRow:
				return this.m_owner.RenderTableRow.m_rowDef.Visibility;
			case Mode.TableDetails:
				return this.m_owner.RenderTableDetails.DetailDefinition.Visibility;
			case Mode.TableGroup:
				return this.m_owner.RenderTableGroup.m_visibilityDef;
			default:
				return null;
			}
		}

		internal override bool GetInstanceHidden()
		{
			switch (this.m_mode)
			{
			case Mode.StaticColumn:
				return this.m_owner.RenderTableColumn.Hidden;
			case Mode.StaticRow:
				return this.m_owner.RenderTableRow.Hidden;
			case Mode.TableDetails:
				return this.m_owner.RenderTableDetails[this.m_owner.Group.CurrentRenderGroupIndex].Hidden;
			case Mode.TableGroup:
				return this.m_owner.RenderTableGroup.Hidden;
			default:
				return false;
			}
		}

		internal override bool GetInstanceStartHidden()
		{
			switch (this.m_mode)
			{
			case Mode.StaticColumn:
				if (this.m_owner.RenderTableColumn.ColumnInstance == null)
				{
					return this.GetInstanceHidden();
				}
				return this.m_owner.RenderTableColumn.ColumnInstance.StartHidden;
			case Mode.StaticRow:
				if (this.m_owner.RenderTableRow.InstanceInfo == null)
				{
					return this.GetInstanceHidden();
				}
				return this.m_owner.RenderTableRow.InstanceInfo.StartHidden;
			case Mode.TableDetails:
			{
				TableDetailRowCollection tableDetailRowCollection = this.m_owner.RenderTableDetails[this.m_owner.Group.CurrentRenderGroupIndex];
				if (tableDetailRowCollection.InstanceInfo != null)
				{
					return tableDetailRowCollection.InstanceInfo.StartHidden;
				}
				return this.GetInstanceHidden();
			}
			case Mode.TableGroup:
				if (this.m_owner.RenderTableGroup.InstanceInfo == null)
				{
					return this.GetInstanceHidden();
				}
				return this.m_owner.RenderTableGroup.InstanceInfo.StartHidden;
			default:
				return false;
			}
		}
	}
}
