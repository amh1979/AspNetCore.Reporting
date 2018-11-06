using System;
using System.ComponentModel;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class PathRulePropertyDescriptor : PropertyDescriptor
	{
		private Field field;

		public override Type ComponentType
		{
			get
			{
				return typeof(PathRule);
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

		public PathRulePropertyDescriptor(Field field, string name, Attribute[] attrs)
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
			PathRule pathRule = (PathRule)component;
			if (this.Name == "FromValue")
			{
				return this.field.Parse(pathRule.FromValue);
			}
			return this.field.Parse(pathRule.ToValue);
		}

		public override void SetValue(object component, object value)
		{
			PathRule pathRule = (PathRule)component;
			if (this.Name == "FromValue")
			{
				pathRule.FromValue = Field.ToStringInvariant(value);
			}
			else
			{
				pathRule.ToValue = Field.ToStringInvariant(value);
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
