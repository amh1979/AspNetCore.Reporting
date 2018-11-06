using System.ComponentModel;

namespace AspNetCore.Reporting.Map.WebForms
{
	[TypeConverter(typeof(GroupDataBindingRuleConverter))]
	internal class GroupDataBindingRule : DataBindingRuleBase
	{
		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeGroupDataBindingRule_BindingField")]
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

		public GroupDataBindingRule()
			: this(null)
		{
		}

		internal GroupDataBindingRule(CommonElements common)
			: base(common)
		{
		}

		internal override void DataBind()
		{
			if (this.Common != null)
			{
				this.Common.MapCore.ExecuteDataBind(BindingType.Groups, this, base.DataSource, base.DataMember, this.BindingField);
			}
		}
	}
}
