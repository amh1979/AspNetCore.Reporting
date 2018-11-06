using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class ViewCenter : MapObject, ICloneable
	{
		private PointF point = new PointF(0f, 0f);

		private bool defaultValues;

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeViewCenter_X")]
		[DefaultValue(50f)]
		[SRCategory("CategoryAttribute_Values")]
		[RefreshProperties(RefreshProperties.All)]
		public float X
		{
			get
			{
				return this.point.X;
			}
			set
			{
				this.point.X = value;
				this.DefaultValues = false;
				this.InvalidateDistanceScalePanel();
				this.InvalidateViewport(false);
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(50f)]
		[SRCategory("CategoryAttribute_Values")]
		[SRDescription("DescriptionAttributeViewCenter_Y")]
		[NotifyParentProperty(true)]
		public float Y
		{
			get
			{
				return this.point.Y;
			}
			set
			{
				this.point.Y = value;
				this.DefaultValues = false;
				this.InvalidateDistanceScalePanel();
				this.InvalidateViewport(false);
			}
		}

		internal bool DefaultValues
		{
			get
			{
				return this.defaultValues;
			}
			set
			{
				this.defaultValues = value;
			}
		}

		public ViewCenter()
			: this(null)
		{
		}

		internal ViewCenter(object parent)
			: base(parent)
		{
		}

		internal ViewCenter(object parent, float x, float y)
			: this(parent)
		{
			this.point.X = x;
			this.point.Y = y;
		}

		public override string ToString()
		{
			return this.point.X.ToString(CultureInfo.CurrentCulture) + ", " + this.point.Y.ToString(CultureInfo.CurrentCulture);
		}

		public PointF ToPoint()
		{
			return new PointF(this.point.X, this.point.Y);
		}

		public static implicit operator PointF(ViewCenter viewCenter)
		{
			return viewCenter.GetPointF();
		}

		public object Clone()
		{
			return new ViewCenter(this.Parent, this.X, this.Y);
		}

		internal PointF GetPointF()
		{
			return this.point;
		}
	}
}
