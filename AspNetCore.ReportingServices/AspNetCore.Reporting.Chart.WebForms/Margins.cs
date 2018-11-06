using AspNetCore.Reporting.Chart.WebForms.Design;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[TypeConverter(typeof(MarginExpandableObjectConverter))]
	[SRDescription("DescriptionAttributeMargins_Margins")]
	internal class Margins
	{
		private int top;

		private int bottom;

		private int left;

		private int right;

		internal CommonElements Common;

		[SRDescription("DescriptionAttributeMargins_Top")]
		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(0)]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public int Top
		{
			get
			{
				return this.top;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException(SR.ExceptionMarginTopIsNegative, "value");
				}
				this.top = value;
				this.Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeMargins_Bottom")]
		[RefreshProperties(RefreshProperties.All)]
		public int Bottom
		{
			get
			{
				return this.bottom;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException(SR.ExceptionMarginBottomIsNegative, "value");
				}
				this.bottom = value;
				this.Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(0)]
		[RefreshProperties(RefreshProperties.All)]
		[SRDescription("DescriptionAttributeMargins_Left")]
		public int Left
		{
			get
			{
				return this.left;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException(SR.ExceptionMarginLeftIsNegative, "value");
				}
				this.left = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeMargins_Right")]
		[DefaultValue(0)]
		[SRCategory("CategoryAttributeMisc")]
		[RefreshProperties(RefreshProperties.All)]
		[NotifyParentProperty(true)]
		public int Right
		{
			get
			{
				return this.right;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException(SR.ExceptionMarginRightIsNegative, "value");
				}
				this.right = value;
				this.Invalidate();
			}
		}

		public Margins()
		{
		}

		public Margins(int top, int bottom, int left, int right)
		{
			this.top = top;
			this.bottom = bottom;
			this.left = left;
			this.right = right;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0:D}, {1:D}, {2:D}, {3:D}", this.Top, this.Bottom, this.Left, this.Right);
		}

		public override bool Equals(object obj)
		{
			Margins margins = obj as Margins;
			if (margins != null && this.Top == margins.Top && this.Bottom == margins.Bottom && this.Left == margins.Left && this.Right == margins.Right)
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.Top.GetHashCode() + this.Bottom.GetHashCode() + this.Left.GetHashCode() + this.Right.GetHashCode();
		}

		public bool IsEmpty()
		{
			if (this.Top == 0 && this.Bottom == 0 && this.Left == 0)
			{
				return this.Right == 0;
			}
			return false;
		}

		public RectangleF ToRectangleF()
		{
			return new RectangleF((float)this.Left, (float)this.Top, (float)this.Right, (float)this.Bottom);
		}

		private void Invalidate()
		{
		}
	}
}
