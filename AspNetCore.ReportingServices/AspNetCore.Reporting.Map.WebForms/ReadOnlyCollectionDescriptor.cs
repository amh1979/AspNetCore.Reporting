using System;
using System.ComponentModel;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class ReadOnlyCollectionDescriptor : PropertyDescriptor
	{
		public override Type ComponentType
		{
			get
			{
				return typeof(RuleBase);
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
				return true;
			}
		}

		public override Type PropertyType
		{
			get
			{
				return typeof(string);
			}
		}

		public ReadOnlyCollectionDescriptor(string name, Attribute[] attrs)
			: base(name, attrs)
		{
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override object GetValue(object component)
		{
			return "(Collection)";
		}

		public override void SetValue(object component, object value)
		{
		}

		public override void ResetValue(object component)
		{
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}
	}
}
