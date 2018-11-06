using System.ComponentModel;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class DockablePanel : Panel
	{
		private PanelDockStyle dockStyle;

		private DockAlignment dockAlignment;

		private bool dockedInsideViewport;

		public override bool Visible
		{
			get
			{
				return base.Visible;
			}
			set
			{
				if (base.Visible != value)
				{
					base.Visible = value;
					this.InvalidateAndLayout();
				}
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[SRDescription("DescriptionAttributeDockablePanel_Dock")]
		[SRCategory("CategoryAttribute_Layout")]
		[NotifyParentProperty(true)]
		public virtual PanelDockStyle Dock
		{
			get
			{
				return this.dockStyle;
			}
			set
			{
				if (this.dockStyle != value)
				{
					this.dockStyle = value;
					this.Location.Docked = (this.dockStyle != PanelDockStyle.None);
					this.InvalidateAndLayout();
				}
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributeDockablePanel_DockAlignment")]
		public virtual DockAlignment DockAlignment
		{
			get
			{
				return this.dockAlignment;
			}
			set
			{
				if (this.dockAlignment != value)
				{
					this.dockAlignment = value;
					this.InvalidateAndLayout();
				}
			}
		}

		[SRCategory("CategoryAttribute_Layout")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeDockablePanel_DockedInsideViewport")]
		public bool DockedInsideViewport
		{
			get
			{
				return this.dockedInsideViewport;
			}
			set
			{
				if (this.dockedInsideViewport != value)
				{
					this.dockedInsideViewport = value;
					this.InvalidateAndLayout();
				}
			}
		}

		protected void ResetDock()
		{
			this.Dock = (PanelDockStyle)this.GetDefaultPropertyValue("Dock", this.Dock);
		}

		protected bool ShouldSerializeDock()
		{
			return !this.Dock.Equals(this.GetDefaultPropertyValue("Dock", this.Dock));
		}

		protected void ResetDockAlignment()
		{
			this.DockAlignment = (DockAlignment)this.GetDefaultPropertyValue("DockAlignment", this.DockAlignment);
		}

		protected bool ShouldSerializeDockAlignment()
		{
			return !this.DockAlignment.Equals(this.GetDefaultPropertyValue("DockAlignment", this.DockAlignment));
		}

		protected void ResetDockedInsideViewport()
		{
			this.DockedInsideViewport = (bool)this.GetDefaultPropertyValue("DockedInsideViewport", this.DockedInsideViewport);
		}

		protected bool ShouldSerializeDockedInsideViewport()
		{
			return !this.DockedInsideViewport.Equals(this.GetDefaultPropertyValue("DockedInsideViewport", this.DockedInsideViewport));
		}

		public DockablePanel()
			: this(null)
		{
		}

		internal DockablePanel(CommonElements common)
			: base(common)
		{
			this.Dock = (PanelDockStyle)this.GetDefaultPropertyValue("Dock", null);
			this.DockAlignment = (DockAlignment)this.GetDefaultPropertyValue("DockAlignment", null);
			this.DockedInsideViewport = (bool)this.GetDefaultPropertyValue("DockedInsideViewport", null);
		}

		internal override void SizeLocationChanged(SizeLocationChangeInfo info)
		{
			base.SizeLocationChanged(info);
			switch (info)
			{
			case SizeLocationChangeInfo.Location:
				this.Location.Docked = (this.Dock != PanelDockStyle.None);
				break;
			case SizeLocationChangeInfo.LocationUnit:
			case SizeLocationChangeInfo.Size:
			case SizeLocationChangeInfo.SizeUnit:
			case SizeLocationChangeInfo.ZOrder:
				this.InvalidateAndLayout();
				break;
			}
		}

		internal override object GetDefaultPropertyValue(string prop, object currentValue)
		{
			object obj = null;
			switch (prop)
			{
			case "Dock":
				return PanelDockStyle.None;
			case "DockAlignment":
				return DockAlignment.Near;
			case "DockedInsideViewport":
				return true;
			default:
				return base.GetDefaultPropertyValue(prop, currentValue);
			}
		}

		internal override bool IsVisible()
		{
			MapCore mapCore = base.GetMapCore();
			bool result = true;
			if (mapCore != null && this.DockedInsideViewport)
			{
				result = mapCore.Viewport.Visible;
			}
			if (base.IsVisible())
			{
				return result;
			}
			return false;
		}
	}
}
