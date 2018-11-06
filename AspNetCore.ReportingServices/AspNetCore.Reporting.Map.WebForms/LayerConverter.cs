using System;
using System.ComponentModel;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class LayerConverter : CollectionItemTypeConverter
	{
		public LayerConverter()
		{
			base.simpleType = typeof(Layer);
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			if (context != null)
			{
				Layer layer = (Layer)value;
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value, new Attribute[1]
				{
					new BrowsableAttribute(true)
				});
				if (layer.Visibility != LayerVisibility.ZoomBased)
				{
					PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
					{
						foreach (PropertyDescriptor item in properties)
						{
							if (item.Name == "VisibleToZoom" || item.Name == "VisibleFromZoom" || item.Name == "LabelVisibleFromZoom")
							{
								propertyDescriptorCollection.Add(TypeDescriptor.CreateProperty(value.GetType(), item, new ReadOnlyAttribute(true)));
							}
							else
							{
								propertyDescriptorCollection.Add(item);
							}
						}
						return propertyDescriptorCollection;
					}
				}
				return properties;
			}
			return base.GetProperties(context, value, attributes);
		}
	}
}
