using System.ComponentModel;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class LinearSpecialPosition : SpecialPosition
	{
		private LinearPinLabel pinLinearLabel;

		[SRDescription("DescriptionAttributeLabelStyle3")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRCategory("CategoryAppearance")]
		public LinearPinLabel LabelStyle
		{
			get
			{
				return this.pinLinearLabel;
			}
			set
			{
				this.pinLinearLabel = value;
				this.pinLinearLabel.Parent = this;
				this.Invalidate();
			}
		}

		public LinearSpecialPosition()
			: this(null)
		{
		}

		public LinearSpecialPosition(object parent)
			: base(parent)
		{
			this.pinLinearLabel = new LinearPinLabel(this);
		}
	}
}
