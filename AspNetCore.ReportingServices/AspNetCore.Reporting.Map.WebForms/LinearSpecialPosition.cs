using System.ComponentModel;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class LinearSpecialPosition : SpecialPosition
	{
		private LinearPinLabel pinLinearLabel;

		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearSpecialPosition_LabelStyle")]
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
