using System;
using System.Collections;
using System.ComponentModel;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class ShapeConverter : CollectionItemTypeConverter
	{
		public ShapeConverter()
		{
			base.simpleType = typeof(Shape);
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value, false);
			PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
			for (int i = 0; i < properties.Count; i++)
			{
				if (properties[i].IsBrowsable)
				{
					propertyDescriptorCollection.Add(properties[i]);
				}
			}
			Shape shape = (Shape)value;
			MapCore mapCore = shape.GetMapCore();
			if (mapCore != null)
			{
				{
					foreach (Field shapeField in mapCore.ShapeFields)
					{
						ArrayList arrayList = new ArrayList();
						arrayList.Add(new CategoryAttribute(SR.CategoryAttribute_ShapeFields));
						arrayList.Add(new DescriptionAttribute(SR.DescriptionAttributeShape_Fields(shapeField.Name)));
						ShapeFieldPropertyDescriptor value2 = new ShapeFieldPropertyDescriptor(shapeField, (Attribute[])arrayList.ToArray(typeof(Attribute)));
						propertyDescriptorCollection.Add(value2);
					}
					return propertyDescriptorCollection;
				}
			}
			return propertyDescriptorCollection;
		}
	}
}
