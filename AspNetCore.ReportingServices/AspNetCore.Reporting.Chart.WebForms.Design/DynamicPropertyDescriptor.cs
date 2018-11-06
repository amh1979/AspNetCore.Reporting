using System;
using System.ComponentModel;

namespace AspNetCore.Reporting.Chart.WebForms.Design
{
	internal class DynamicPropertyDescriptor : PropertyDescriptor
	{
		private PropertyDescriptor basePropertyDescriptor;

		private string displayName = string.Empty;

		public override Type ComponentType
		{
			get
			{
				return this.basePropertyDescriptor.ComponentType;
			}
		}

		public override string DisplayName
		{
			get
			{
				if (this.displayName.Length > 0)
				{
					return this.displayName;
				}
				return this.basePropertyDescriptor.DisplayName;
			}
		}

		public override bool IsBrowsable
		{
			get
			{
				return this.basePropertyDescriptor.IsBrowsable;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return this.basePropertyDescriptor.IsReadOnly;
			}
		}

		public override Type PropertyType
		{
			get
			{
				return this.basePropertyDescriptor.PropertyType;
			}
		}

		public DynamicPropertyDescriptor(PropertyDescriptor basePropertyDescriptor, string displayName)
			: base(basePropertyDescriptor)
		{
			this.displayName = displayName;
			this.basePropertyDescriptor = basePropertyDescriptor;
		}

		public override bool CanResetValue(object component)
		{
			return this.basePropertyDescriptor.CanResetValue(component);
		}

		public override object GetValue(object component)
		{
			return this.basePropertyDescriptor.GetValue(component);
		}

		public override void ResetValue(object component)
		{
			this.basePropertyDescriptor.ResetValue(component);
		}

		public override bool ShouldSerializeValue(object component)
		{
			return this.basePropertyDescriptor.ShouldSerializeValue(component);
		}

		public override void SetValue(object component, object value)
		{
			this.basePropertyDescriptor.SetValue(component, value);
		}
	}
}
