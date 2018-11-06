using System;
using System.ComponentModel;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class SymbolFieldPropertyDescriptor : PropertyDescriptor
	{
		private Field field;

		public override Type ComponentType
		{
			get
			{
				return typeof(Symbol);
			}
		}

		public override string DisplayName
		{
			get
			{
				return this.field.Name;
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
				return this.field.IsTemporary;
			}
		}

		public override Type PropertyType
		{
			get
			{
				return this.field.Type;
			}
		}

		public SymbolFieldPropertyDescriptor(Field field)
			: base(field.Name, new Attribute[2]
			{
				new CategoryAttribute(SR.CategoryAttribute_SymbolFields),
				new DescriptionAttribute(SR.DescriptionAttributeSymbol_Fields(field.Name))
			})
		{
			this.field = field;
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override object GetValue(object component)
		{
			try
			{
				Symbol symbol = (Symbol)component;
				return symbol[this.field.Name];
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public override void ResetValue(object component)
		{
		}

		public override bool ShouldSerializeValue(object component)
		{
			return true;
		}

		public override void SetValue(object component, object value)
		{
			Symbol symbol = (Symbol)component;
			symbol[this.field.Name] = value;
		}
	}
}
