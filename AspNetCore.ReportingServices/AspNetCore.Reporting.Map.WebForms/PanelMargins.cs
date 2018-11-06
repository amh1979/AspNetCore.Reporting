using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace AspNetCore.Reporting.Map.WebForms
{
	[TypeConverter(typeof(PanelMarginsConverter))]
	internal class PanelMargins
	{
		private Panel owner;

		private int top;

		private int bottom;

		private int right;

		private int left;

		private bool all = true;

		[Browsable(false)]
		internal Panel Owner
		{
			get
			{
				return this.owner;
			}
			set
			{
				this.owner = value;
			}
		}

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributePanelMargins_Top")]
		[RefreshProperties(RefreshProperties.All)]
		public int Top
		{
			get
			{
				return this.top;
			}
			set
			{
				if (this.top != value)
				{
					this.top = value;
					this.SyncPropeties();
					this.NotifyOwner();
				}
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[SRDescription("DescriptionAttributePanelMargins_Bottom")]
		[NotifyParentProperty(true)]
		public int Bottom
		{
			get
			{
				return this.bottom;
			}
			set
			{
				if (this.bottom != value)
				{
					this.bottom = value;
					this.SyncPropeties();
					this.NotifyOwner();
				}
			}
		}

		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		[SRDescription("DescriptionAttributePanelMargins_Right")]
		public int Right
		{
			get
			{
				return this.right;
			}
			set
			{
				if (this.right != value)
				{
					this.right = value;
					this.SyncPropeties();
					this.NotifyOwner();
				}
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributePanelMargins_Left")]
		public int Left
		{
			get
			{
				return this.left;
			}
			set
			{
				if (this.left != value)
				{
					this.left = value;
					this.SyncPropeties();
					this.NotifyOwner();
				}
			}
		}

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributePanelMargins_All")]
		[RefreshProperties(RefreshProperties.All)]
		public int All
		{
			get
			{
				if (!this.all)
				{
					return -1;
				}
				return this.left;
			}
			set
			{
				if (value >= 0)
				{
					this.all = true;
					this.top = value;
					this.bottom = value;
					this.left = value;
					this.right = value;
					this.NotifyOwner();
				}
			}
		}

		private PanelMargins DefaultMargins
		{
			get
			{
				IDefaultValueProvider defaultValueProvider = this.Owner;
				if (defaultValueProvider == null)
				{
					return new PanelMargins(0, 0, 0, 0);
				}
				return (PanelMargins)defaultValueProvider.GetDefaultValue("Margins", this);
			}
		}

		internal PanelMargins(Panel owner)
			: this(owner, 0, 0, 0, 0)
		{
		}

		internal PanelMargins(int left, int top, int right, int bottom)
			: this(null, left, top, right, bottom)
		{
		}

		internal PanelMargins(Panel owner, int left, int top, int right, int bottom)
		{
			this.owner = owner;
			this.left = left;
			this.top = top;
			this.right = right;
			this.bottom = bottom;
			this.SyncPropeties();
		}

		internal PanelMargins(PanelMargins margins)
			: this(margins.Owner, margins.Left, margins.Top, margins.Right, margins.Bottom)
		{
		}

		protected void ResetTop()
		{
			this.Top = this.DefaultMargins.Top;
		}

		protected bool ShouldSerializeTop()
		{
			if (!this.all && this.top != this.DefaultMargins.Top)
			{
				return true;
			}
			return false;
		}

		protected void ResetBottom()
		{
			this.Bottom = this.DefaultMargins.Bottom;
		}

		protected bool ShouldSerializeBottom()
		{
			if (!this.all && this.bottom != this.DefaultMargins.Bottom)
			{
				return true;
			}
			return false;
		}

		protected void ResetRight()
		{
			this.Right = this.DefaultMargins.Right;
		}

		protected bool ShouldSerializeRight()
		{
			if (!this.all && this.Right != this.DefaultMargins.Right)
			{
				return true;
			}
			return false;
		}

		protected void ResetLeft()
		{
			this.Left = this.DefaultMargins.Left;
		}

		protected bool ShouldSerializeLeft()
		{
			if (!this.all && this.left != this.DefaultMargins.Left)
			{
				return true;
			}
			return false;
		}

		protected void ResetAll()
		{
			this.All = this.DefaultMargins.All;
		}

		protected bool ShouldSerializeAll()
		{
			if (this.all && this.left != this.DefaultMargins.All)
			{
				return true;
			}
			return false;
		}

		private void SyncPropeties()
		{
			if (this.top == this.bottom && this.left == this.right && this.top == this.left)
			{
				this.all = true;
			}
			else
			{
				this.all = false;
			}
		}

		private void NotifyOwner()
		{
			if (this.owner != null)
			{
				this.owner.Invalidate();
				this.owner.SizeLocationChanged(SizeLocationChangeInfo.Margins);
			}
		}

		public RectangleF AdjustRectangle(RectangleF rect)
		{
			rect.X = (float)this.Left;
			rect.Y = (float)this.Top;
			rect.Width -= (float)(this.Left + this.Right);
			rect.Height -= (float)(this.Top + this.Bottom);
			return rect;
		}

		public Rectangle AdjustRectangle(Rectangle rect)
		{
			rect.X = this.Left;
			rect.Y = this.Top;
			rect.Width -= this.Left + this.Right;
			rect.Height -= this.Top + this.Bottom;
			return rect;
		}

		public override bool Equals(object obj)
		{
			if (obj is PanelMargins)
			{
				return (PanelMargins)obj == this;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "Left: {0}, Top: {0}, Right: {0}, Bottom: {0}", this.Left.ToString(CultureInfo.CurrentCulture), this.Top.ToString(CultureInfo.CurrentCulture), this.Right.ToString(CultureInfo.CurrentCulture), this.Bottom.ToString(CultureInfo.CurrentCulture));
		}

		internal bool IsEmpty()
		{
			return this.All == 0;
		}
	}
}
