using System.ComponentModel;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class CircularSpecialPosition : SpecialPosition
	{
		private CircularPinLabel pinCircularLabel;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeLabelStyle3")]
		public CircularPinLabel LabelStyle
		{
			get
			{
				return this.pinCircularLabel;
			}
			set
			{
				this.pinCircularLabel = value;
				this.pinCircularLabel.Parent = this;
				this.Invalidate();
			}
		}

		public CircularSpecialPosition()
			: this(null)
		{
		}

		public CircularSpecialPosition(object parent)
			: base(parent)
		{
			this.pinCircularLabel = new CircularPinLabel(this);
		}
	}
}
