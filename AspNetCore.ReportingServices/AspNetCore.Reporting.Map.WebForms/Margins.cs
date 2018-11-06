using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace AspNetCore.Reporting.Map.WebForms
{
	[TypeConverter(typeof(MarginExpandableObjectConverter))]
	[Description("Chart element margins.")]
	internal class Margins
	{
		private int top;

		private int bottom;

		private int left;

		private int right;

		internal CommonElements Common;

		[NotifyParentProperty(true)]
		[DefaultValue(0)]
		[SRCategory("CategoryAttribute_Misc")]
		[SRDescription("DescriptionAttributeMargins_Top")]
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
					throw new ArgumentException(SR.ExceptionTopMarginCannotBeNegative, "Top");
				}
				this.top = value;
				this.Invalidate();
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Misc")]
		[SRDescription("DescriptionAttributeMargins_Bottom")]
		[DefaultValue(0)]
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
					throw new ArgumentException(SR.ExceptionBottomMarginCannotBeNegative, "Bottom");
				}
				this.bottom = value;
				this.Invalidate();
			}
		}

		[DefaultValue(0)]
		[SRCategory("CategoryAttribute_Misc")]
		[RefreshProperties(RefreshProperties.All)]
		[NotifyParentProperty(true)]
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
					throw new ArgumentException(SR.ExceptionLeftMarginCannotBeNegative, "Left");
				}
				this.left = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeMargins_Right")]
		[RefreshProperties(RefreshProperties.All)]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Misc")]
		[DefaultValue(0)]
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
					throw new ArgumentException(SR.ExceptionRightMarginCannotBeNegative, "Right");
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
			return string.Format(CultureInfo.CurrentCulture, "{0:D}, {1:D}, {2:D}, {3:D}", this.Top, this.Bottom, this.Left, this.Right);
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

		public static Margins Parse(string text)
		{
			string[] array = text.Split(',');
			if (array.Length != 4)
			{
				throw new ArgumentException(SR.ExceptionValueFormatIncorrect(text));
			}
			return new Margins(int.Parse(array[0], CultureInfo.CurrentCulture), int.Parse(array[1], CultureInfo.CurrentCulture), int.Parse(array[2], CultureInfo.CurrentCulture), int.Parse(array[3], CultureInfo.CurrentCulture));
		}

		private void Invalidate()
		{
		}
	}
}
