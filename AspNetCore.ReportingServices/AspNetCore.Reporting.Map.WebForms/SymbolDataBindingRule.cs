using System.Collections.Specialized;
using System.ComponentModel;

namespace AspNetCore.Reporting.Map.WebForms
{
	[TypeConverter(typeof(SymbolDataBindingRuleConverter))]
	internal class SymbolDataBindingRule : DataBindingRuleBase
	{
		private string category = "";

		private string xCoordinateField;

		private string yCoordinateField;

		private string parentShapeField;

		[SRDescription("DescriptionAttributeSymbolDataBindingRule_BindingField")]
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

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeSymbolDataBindingRule_Category")]
		[DefaultValue("")]
		public string Category
		{
			get
			{
				return this.category;
			}
			set
			{
				this.category = value;
			}
		}

		[TypeConverter(typeof(CoordinateFieldConverter))]
		[SRDescription("DescriptionAttributeSymbolDataBindingRule_XCoordinateField")]
		[DefaultValue("")]
		[SRCategory("CategoryAttribute_Data")]
		public string XCoordinateField
		{
			get
			{
				return this.xCoordinateField;
			}
			set
			{
				this.xCoordinateField = value;
			}
		}

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeSymbolDataBindingRule_YCoordinateField")]
		[DefaultValue("")]
		[TypeConverter(typeof(CoordinateFieldConverter))]
		public string YCoordinateField
		{
			get
			{
				return this.yCoordinateField;
			}
			set
			{
				this.yCoordinateField = value;
			}
		}

		[TypeConverter(typeof(CoordinateFieldConverter))]
		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeSymbolDataBindingRule_ParentShapeField")]
		[DefaultValue("")]
		public string ParentShapeField
		{
			get
			{
				return this.parentShapeField;
			}
			set
			{
				this.parentShapeField = value;
			}
		}

		public SymbolDataBindingRule()
			: this(null)
		{
		}

		internal SymbolDataBindingRule(CommonElements common)
			: base(common)
		{
		}

		internal override void DataBind()
		{
			if (this.Common != null)
			{
				this.Common.MapCore.ExecuteDataBind(BindingType.Symbols, this, base.DataSource, base.DataMember, this.BindingField, this.Category, this.ParentShapeField, this.XCoordinateField, this.YCoordinateField);
			}
		}

		internal override void UpdateDataFields(string dataMember, int dataMemberIndex, StringCollection dataFields)
		{
			base.UpdateDataFields(dataMember, dataMemberIndex, dataFields);
			if (!(base.DataMember == dataMember))
			{
				if (!string.IsNullOrEmpty(base.DataMember))
				{
					return;
				}
				if (dataMemberIndex != 0)
				{
					return;
				}
			}
			if (!dataFields.Contains(this.XCoordinateField))
			{
				this.XCoordinateField = "";
			}
			if (!dataFields.Contains(this.YCoordinateField))
			{
				this.YCoordinateField = "";
			}
			if (!dataFields.Contains(this.ParentShapeField))
			{
				this.ParentShapeField = "";
			}
		}
	}
}
