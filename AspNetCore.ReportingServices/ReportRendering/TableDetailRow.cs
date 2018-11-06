using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class TableDetailRow : TableRow
	{
		private TableDetailRowCollection m_detail;

		public override bool Hidden
		{
			get
			{
				if (this.m_detail.Hidden)
				{
					return true;
				}
				return base.Hidden;
			}
		}

		public override TextBox ToggleParent
		{
			get
			{
				TextBox textBox = null;
				if (this.m_detail.DetailInstance != null)
				{
					textBox = base.m_owner.RenderingContext.GetToggleParent(this.m_detail.DetailInstance.UniqueName);
				}
				if (textBox == null)
				{
					return base.ToggleParent;
				}
				return textBox;
			}
		}

		public override bool HasToggle
		{
			get
			{
				TableDetail tableDetail = ((AspNetCore.ReportingServices.ReportProcessing.Table)base.m_owner.ReportItemDef).TableDetail;
				if (Visibility.HasToggle(tableDetail.Visibility))
				{
					return true;
				}
				return base.HasToggle;
			}
		}

		public override string ToggleItem
		{
			get
			{
				TableDetail tableDetail = ((AspNetCore.ReportingServices.ReportProcessing.Table)base.m_owner.ReportItemDef).TableDetail;
				string text = null;
				if (tableDetail.Visibility != null)
				{
					text = tableDetail.Visibility.Toggle;
				}
				if (text == null)
				{
					text = base.ToggleItem;
				}
				return text;
			}
		}

		public override SharedHiddenState SharedHidden
		{
			get
			{
				TableDetail tableDetail = ((AspNetCore.ReportingServices.ReportProcessing.Table)base.m_owner.ReportItemDef).TableDetail;
				SharedHiddenState sharedHidden = Visibility.GetSharedHidden(tableDetail.Visibility);
				if (sharedHidden == SharedHiddenState.Always)
				{
					return SharedHiddenState.Always;
				}
				SharedHiddenState sharedHidden2 = base.SharedHidden;
				if (SharedHiddenState.Never == sharedHidden)
				{
					return sharedHidden2;
				}
				if (sharedHidden2 == SharedHiddenState.Always)
				{
					return SharedHiddenState.Always;
				}
				return SharedHiddenState.Sometimes;
			}
		}

		public override bool IsToggleChild
		{
			get
			{
				bool flag = false;
				if (this.m_detail.DetailInstance != null)
				{
					flag = base.m_owner.RenderingContext.IsToggleChild(this.m_detail.DetailInstance.UniqueName);
				}
				if (flag)
				{
					return true;
				}
				return base.IsToggleChild;
			}
		}

		internal TableDetailRow(Table owner, AspNetCore.ReportingServices.ReportProcessing.TableRow rowDef, TableRowInstance rowInstance, TableDetailRowCollection detail)
			: base(owner, rowDef, rowInstance)
		{
			this.m_detail = detail;
		}
	}
}
