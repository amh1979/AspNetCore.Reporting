using System;
using System.ComponentModel;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class FieldConverter : CollectionItemTypeConverter
	{
		public FieldConverter()
		{
			base.simpleType = typeof(Field);
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			if (context != null)
			{
				Field field = (Field)value;
				if (field.IsTemporary)
				{
					PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value, new Attribute[1]
					{
						new BrowsableAttribute(true)
					});
					PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
					{
						foreach (PropertyDescriptor item in properties)
						{
							propertyDescriptorCollection.Add(TypeDescriptor.CreateProperty(value.GetType(), item, new ReadOnlyAttribute(true)));
						}
						return propertyDescriptorCollection;
					}
				}
			}
			return TypeDescriptor.GetProperties(value, new Attribute[1]
			{
				new BrowsableAttribute(true)
			});
		}
	}
}
