using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal sealed class GaugeSize : GaugeObject, ICloneable
	{
		private SizeF size = new SizeF(100f, 100f);

		private bool defaultValues;

		[ValidateBound(0.0, 100.0)]
		[SRCategory("CategoryValues")]
		[SRDescription("DescriptionAttributeGaugeSize_Width")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public float Width
		{
			get
			{
				return this.size.Width;
			}
			set
			{
				if (!((double)value > 100000000.0) && !((double)value < -100000000.0))
				{
					this.RemoveAutoLayout();
					this.size.Width = Math.Max(value, 0f);
					this.DefaultValues = false;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", -100000000.0, 100000000.0));
			}
		}

		[SRCategory("CategoryValues")]
		[ValidateBound(0.0, 100.0)]
		[SRDescription("DescriptionAttributeGaugeSize_Height")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public float Height
		{
			get
			{
				return this.size.Height;
			}
			set
			{
				if (!((double)value > 100000000.0) && !((double)value < -100000000.0))
				{
					this.RemoveAutoLayout();
					this.size.Height = Math.Max(value, 0f);
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

		public GaugeSize()
			: this(null)
		{
		}

		internal GaugeSize(object parent)
			: base(parent)
		{
		}

		internal GaugeSize(object parent, float width, float height)
			: this(parent)
		{
			this.Width = Math.Max(width, 0f);
			this.Height = Math.Max(height, 0f);
		}

		internal GaugeSize(object parent, SizeF size)
			: this(parent)
		{
			this.Width = size.Width;
			this.Height = size.Height;
		}

		public override string ToString()
		{
			if (this.Parent != null && this.Parent is GaugeBase)
			{
				GaugeBase gaugeBase = (GaugeBase)this.Parent;
				if (gaugeBase.Size == this && gaugeBase.Common.GaugeContainer.AutoLayout && gaugeBase.Parent.Length == 0)
				{
					return "(AutoLayout)";
				}
			}
			return this.size.Width.ToString(CultureInfo.CurrentCulture) + ", " + this.size.Height.ToString(CultureInfo.CurrentCulture);
		}

		public SizeF ToSize()
		{
			return new SizeF(this.size);
		}

		public static implicit operator SizeF(GaugeSize size)
		{
			return size.ToSize();
		}

		public object Clone()
		{
			return new GaugeSize(this.Parent, this.size);
		}

		private void RemoveAutoLayout()
		{
			if (this.Parent != null && this.Parent is GaugeBase)
			{
				GaugeBase gaugeBase = (GaugeBase)this.Parent;
				if (gaugeBase.Size == this && gaugeBase.Parent == string.Empty && gaugeBase.Common != null && gaugeBase.Common.GaugeContainer != null && gaugeBase.Common.GaugeContainer.AutoLayout && gaugeBase.Common.GaugeCore != null && !gaugeBase.Common.GaugeCore.layoutFlag)
				{
					gaugeBase.Common.GaugeContainer.AutoLayout = false;
				}
			}
		}
	}
}
