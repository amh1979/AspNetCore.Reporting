using System;
using System.ComponentModel;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class ShapeRulePropertyDescriptor : PropertyDescriptor
	{
		private Field field;

		public override Type ComponentType
		{
			get
			{
				return typeof(ShapeRule);
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

		public ShapeRulePropertyDescriptor(Field field, string name, Attribute[] attrs)
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
			ShapeRule shapeRule = (ShapeRule)component;
			if (this.Name == "FromValue")
			{
				return this.field.Parse(shapeRule.FromValue);
			}
			return this.field.Parse(shapeRule.ToValue);
		}

		public override void SetValue(object component, object value)
		{
			ShapeRule shapeRule = (ShapeRule)component;
			if (this.Name == "FromValue")
			{
				shapeRule.FromValue = Field.ToStringInvariant(value);
			}
			else
			{
				shapeRule.ToValue = Field.ToStringInvariant(value);
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
