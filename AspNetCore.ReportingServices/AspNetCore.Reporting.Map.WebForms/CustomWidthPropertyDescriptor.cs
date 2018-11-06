using System;
using System.ComponentModel;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class CustomWidthPropertyDescriptor : PropertyDescriptor
	{
		private Field field;

		public override Type ComponentType
		{
			get
			{
				return typeof(CustomWidth);
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

		public CustomWidthPropertyDescriptor(Field field, string name, Attribute[] attrs)
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
			CustomWidth customWidth = (CustomWidth)component;
			if (this.Name == "FromValue")
			{
				return this.field.Parse(customWidth.FromValue);
			}
			return this.field.Parse(customWidth.ToValue);
		}

		public override void SetValue(object component, object value)
		{
			CustomWidth customWidth = (CustomWidth)component;
			if (this.Name == "FromValue")
			{
				customWidth.FromValue = Field.ToStringInvariant(value);
			}
			else
			{
				customWidth.ToValue = Field.ToStringInvariant(value);
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
