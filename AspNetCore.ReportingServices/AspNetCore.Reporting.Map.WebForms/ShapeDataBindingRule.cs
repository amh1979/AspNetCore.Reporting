using System.ComponentModel;

namespace AspNetCore.Reporting.Map.WebForms
{
	[TypeConverter(typeof(ShapeDataBindingRuleConverter))]
	internal class ShapeDataBindingRule : DataBindingRuleBase
	{
		[SRDescription("DescriptionAttributeShapeDataBindingRule_BindingField")]
		[SRCategory("CategoryAttribute_Data")]
		public override string BindingField
		{
			get
			{
				return base.BindingField;
			}
			set
			{
				base.BindingField = value;
			}
		}

		public ShapeDataBindingRule()
			: this(null)
		{
		}

		internal ShapeDataBindingRule(CommonElements common)
			: base(common)
		{
		}

		internal override void DataBind()
		{
			if (this.Common != null)
			{
				this.Common.MapCore.ExecuteDataBind(BindingType.Shapes, this, base.DataSource, base.DataMember, this.BindingField);
			}
		}
	}
}
