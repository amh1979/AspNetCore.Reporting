using System.ComponentModel;
using System.Drawing;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(NumericRangeConverter))]
	internal class NumericRange : Range
	{
		private const double DEFAULT_START_VALUE = 7000.0;

		private const double DEFAULT_END_VALUE = 10000.0;

		private Color digitColor = Color.Red;

		private Color decimalColor = Color.Red;

		[SRDescription("DescriptionAttributeName13")]
		[SRCategory("CategoryMisc")]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[DefaultValue(7000.0)]
		[SRDescription("DescriptionAttributeStartValue")]
		public override double StartValue
		{
			get
			{
				return base.StartValue;
			}
			set
			{
				base.StartValue = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[DefaultValue(10000.0)]
		[SRDescription("DescriptionAttributeEndValue3")]
		public override double EndValue
		{
			get
			{
				return base.EndValue;
			}
			set
			{
				base.EndValue = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeNumericRange_DigitColor")]
		[DefaultValue(typeof(Color), "Red")]
		[SRCategory("CategoryAppearance")]
		public Color DigitColor
		{
			get
			{
				return this.digitColor;
			}
			set
			{
				this.digitColor = value;
				this.Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeNumericRange_DecimalColor")]
		[DefaultValue(typeof(Color), "Red")]
		[SRCategory("CategoryAppearance")]
		public Color DecimalColor
		{
			get
			{
				return this.decimalColor;
			}
			set
			{
				this.decimalColor = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeInRangeTimeout")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(0.0)]
		[SRCategory("CategoryBehavior")]
		public override double InRangeTimeout
		{
			get
			{
				return base.InRangeTimeout;
			}
			set
			{
				base.InRangeTimeout = value;
			}
		}

		[SRDescription("DescriptionAttributeInRangeTimeoutType")]
		[DefaultValue(PeriodType.Seconds)]
		[SRCategory("CategoryBehavior")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override PeriodType InRangeTimeoutType
		{
			get
			{
				return base.InRangeTimeoutType;
			}
			set
			{
				base.InRangeTimeoutType = value;
			}
		}

		public NumericRange()
			: base(7000.0, 10000.0)
		{
		}

		public override string ToString()
		{
			return this.Name;
		}

		internal override void OnAdded()
		{
			NumericIndicator numericIndicator = (NumericIndicator)this.ParentElement;
			((IValueConsumer)numericIndicator.Data).Refresh();
			base.OnAdded();
		}
	}
}
