using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class GaugeLocation : GaugeObject, ICloneable
	{
		private PointF point = new PointF(0f, 0f);

		private bool defaultValues;

		[ValidateBound(0.0, 100.0, false)]
		[SRCategory("CategoryValues")]
		[SRDescription("DescriptionAttributeGaugeLocation_X")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public float X
		{
			get
			{
				return this.point.X;
			}
			set
			{
				if (!((double)value > 100000000.0) && !((double)value < -100000000.0))
				{
					this.RemoveAutoLayout();
					this.point.X = value;
					this.DefaultValues = false;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", -100000000.0, 100000000.0));
			}
		}

		[SRDescription("DescriptionAttributeY")]
		[ValidateBound(0.0, 100.0, false)]
		[SRCategory("CategoryValues")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public float Y
		{
			get
			{
				return this.point.Y;
			}
			set
			{
				if (!((double)value > 100000000.0) && !((double)value < -100000000.0))
				{
					this.RemoveAutoLayout();
					this.point.Y = value;
					this.DefaultValues = false;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", -100000000.0, 100000000.0));
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

		public GaugeLocation()
			: this(null)
		{
		}

		internal GaugeLocation(object parent)
			: base(parent)
		{
		}

		internal GaugeLocation(object parent, float x, float y)
			: this(parent)
		{
			this.point.X = x;
			this.point.Y = y;
		}

		public override string ToString()
		{
			if (this.Parent != null && this.Parent is GaugeBase)
			{
				GaugeBase gaugeBase = (GaugeBase)this.Parent;
				if (gaugeBase.Location == this && gaugeBase.Common.GaugeContainer.AutoLayout && gaugeBase.Parent.Length == 0)
				{
					return "(AutoLayout)";
				}
			}
			return this.point.X.ToString(CultureInfo.CurrentCulture) + ", " + this.point.Y.ToString(CultureInfo.CurrentCulture);
		}

		public PointF ToPoint()
		{
			return new PointF(this.point.X, this.point.Y);
		}

		public static implicit operator PointF(GaugeLocation location)
		{
			return location.GetPointF();
		}

		public object Clone()
		{
			return new GaugeLocation(this.Parent, this.X, this.Y);
		}

		internal PointF GetPointF()
		{
			return this.point;
		}

		private void RemoveAutoLayout()
		{
			if (this.Parent != null && this.Parent is GaugeBase)
			{
				GaugeBase gaugeBase = (GaugeBase)this.Parent;
				if (gaugeBase.Location == this && gaugeBase.Parent == string.Empty && gaugeBase.Common != null && gaugeBase.Common.GaugeContainer != null && gaugeBase.Common.GaugeContainer.AutoLayout && gaugeBase.Common.GaugeCore != null && !gaugeBase.Common.GaugeCore.layoutFlag)
				{
					gaugeBase.Common.GaugeContainer.AutoLayout = false;
				}
			}
		}
	}
}
