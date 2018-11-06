using System;
using System.ComponentModel;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class GroupRulePropertyDescriptor : PropertyDescriptor
	{
		private Field field;

		public override Type ComponentType
		{
			get
			{
				return typeof(GroupRule);
			}
		}

		public override bool IsBrowsable
		{
			get
			{
				return true;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public override Type PropertyType
		{
			get
			{
				return this.field.Type;
			}
		}

		public GroupRulePropertyDescriptor(Field field, string name, Attribute[] attrs)
			: base(name, attrs)
		{
			this.field = field;
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override object GetValue(object component)
		{
			GroupRule groupRule = (GroupRule)component;
			if (this.Name == "FromValue")
			{
				return this.field.Parse(groupRule.FromValue);
			}
			return this.field.Parse(groupRule.ToValue);
		}

		public override void SetValue(object component, object value)
		{
			GroupRule groupRule = (GroupRule)component;
			if (this.Name == "FromValue")
			{
				groupRule.FromValue = Field.ToStringInvariant(value);
			}
			else
			{
				groupRule.ToValue = Field.ToStringInvariant(value);
			}
		}

		public override void ResetValue(object component)
		{
		}

		public override bool ShouldSerializeValue(object component)
		{
			return true;
		}
	}
}
