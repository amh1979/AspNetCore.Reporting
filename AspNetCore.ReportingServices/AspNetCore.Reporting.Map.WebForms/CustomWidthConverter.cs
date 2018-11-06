using System;
using System.ComponentModel;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class CustomWidthConverter : CollectionItemTypeConverter
	{
		public CustomWidthConverter()
		{
			base.simpleType = typeof(CustomWidth);
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		private Field GetField(CustomWidth customWidth)
		{
			if (customWidth == null)
			{
				return null;
			}
			RuleBase rule = customWidth.GetRule();
			if (rule == null)
			{
				return null;
			}
			return rule.GetField();
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value, false);
			PathRule pathRule = (PathRule)value;
			Field field = this.GetField((CustomWidth)value);
			PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
			for (int i = 0; i < properties.Count; i++)
			{
				if (properties[i].IsBrowsable)
				{
					if (field != null && (properties[i].Name == "FromValue" || properties[i].Name == "ToValue"))
					{
						Attribute[] array = new Attribute[properties[i].Attributes.Count];
						properties[i].Attributes.CopyTo(array, 0);
						CustomWidthPropertyDescriptor value2 = new CustomWidthPropertyDescriptor(field, properties[i].Name, array);
						propertyDescriptorCollection.Add(value2);
					}
					else if (properties[i].Name == "LegendText" || (properties[i].Name.EndsWith("InLegend", StringComparison.Ordinal) && pathRule.ShowInLegend == "(none)"))
					{
						propertyDescriptorCollection.Add(TypeDescriptor.CreateProperty(value.GetType(), properties[i], new ReadOnlyAttribute(true)));
					}
					else
					{
						propertyDescriptorCollection.Add(properties[i]);
					}
				}
			}
			return propertyDescriptorCollection;
		}
	}
}
