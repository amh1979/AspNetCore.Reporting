using System;
using System.ComponentModel;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class SymbolRulePropertyDescriptor : PropertyDescriptor
	{
		private Field field;

		public override Type ComponentType
		{
			get
			{
				return typeof(SymbolRule);
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

		public SymbolRulePropertyDescriptor(Field field, string name, Attribute[] attrs)
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
			SymbolRule symbolRule = (SymbolRule)component;
			if (this.Name == "FromValue")
			{
				return this.field.Parse(symbolRule.FromValue);
			}
			return this.field.Parse(symbolRule.ToValue);
		}

		public override void SetValue(object component, object value)
		{
			SymbolRule symbolRule = (SymbolRule)component;
			if (this.Name == "FromValue")
			{
				symbolRule.FromValue = Field.ToStringInvariant(value);
			}
			else
			{
				symbolRule.ToValue = Field.ToStringInvariant(value);
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
