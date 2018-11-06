using System;
using System.ComponentModel;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class PathWidthRulePropertyDescriptor : PropertyDescriptor
	{
		private Field field;

		public override Type ComponentType
		{
			get
			{
				return typeof(PathWidthRule);
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

		public PathWidthRulePropertyDescriptor(Field field, string name, Attribute[] attrs)
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
			PathWidthRule pathWidthRule = (PathWidthRule)component;
			if (this.Name == "FromValue")
			{
				return this.field.Parse(pathWidthRule.FromValue);
			}
			return this.field.Parse(pathWidthRule.ToValue);
		}

		public override void SetValue(object component, object value)
		{
			PathWidthRule pathWidthRule = (PathWidthRule)component;
			if (this.Name == "FromValue")
			{
				pathWidthRule.FromValue = Field.ToStringInvariant(value);
			}
			else
			{
				pathWidthRule.ToValue = Field.ToStringInvariant(value);
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
