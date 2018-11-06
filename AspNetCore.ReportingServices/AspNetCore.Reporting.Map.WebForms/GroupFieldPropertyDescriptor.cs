using System;
using System.ComponentModel;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class GroupFieldPropertyDescriptor : PropertyDescriptor
	{
		private Field field;

		public override Type ComponentType
		{
			get
			{
				return typeof(Group);
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

		public GroupFieldPropertyDescriptor(Field field, Attribute[] attributes)
			: base(field.Name, attributes)
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
				Group group = (Group)component;
				return group[this.field.Name];
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
			Group group = (Group)component;
			group[this.field.Name] = value;
		}
	}
}
