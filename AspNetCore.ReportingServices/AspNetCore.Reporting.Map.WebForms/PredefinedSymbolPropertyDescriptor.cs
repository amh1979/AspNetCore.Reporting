using System;
using System.ComponentModel;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class PredefinedSymbolPropertyDescriptor : PropertyDescriptor
	{
		private Field field;

		public override Type ComponentType
		{
			get
			{
				return typeof(PredefinedSymbol);
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

		public PredefinedSymbolPropertyDescriptor(Field field, string name, Attribute[] attrs)
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
			PredefinedSymbol predefinedSymbol = (PredefinedSymbol)component;
			if (this.Name == "FromValue")
			{
				return this.field.Parse(predefinedSymbol.FromValue);
			}
			return this.field.Parse(predefinedSymbol.ToValue);
		}

		public override void SetValue(object component, object value)
		{
			PredefinedSymbol predefinedSymbol = (PredefinedSymbol)component;
			if (this.Name == "FromValue")
			{
				predefinedSymbol.FromValue = Field.ToStringInvariant(value);
			}
			else
			{
				predefinedSymbol.ToValue = Field.ToStringInvariant(value);
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
