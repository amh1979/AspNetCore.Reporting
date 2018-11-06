using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms.Design;

namespace AspNetCore.Reporting.Chart.WebForms.Design
{
	internal class ObjectPropertyTab : PropertyTab
	{
		private string img = "AAEAAAD/////AQAAAAAAAAAMAgAAAFRTeXN0ZW0uRHJhd2luZywgVmVyc2lvbj0xLjAuMzMwMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWIwM2Y1ZjdmMTFkNTBhM2EFAQAAABVTeXN0ZW0uRHJhd2luZy5CaXRtYXABAAAABERhdGEHAgIAAAAJAwAAAA8DAAAA9gAAAAJCTfYAAAAAAAAANgAAACgAAAAIAAAACAAAAAEAGAAAAAAAAAAAAMQOAADEDgAAAAAAAAAAAAD///////////////////////////////////9ZgABZgADzPz/zPz/zPz9AgP//////////gAD/gAD/AAD/AAD/AACKyub///////+AAACAAAAAAP8AAP8AAP9AgP////////9ZgABZgABz13hz13hz13hAgP//////////gAD/gACA/wCA/wCA/wAA//////////+AAACAAAAAAP8AAP8AAP9AgP////////////////////////////////////8L";

		public override string TabName
		{
			get
			{
				return "Selected Chart Object";
			}
		}

		public new Bitmap Bitmap
		{
			get
			{
				return new Bitmap(this.DeserializeFromBase64Text(this.img));
			}
		}

		private Image DeserializeFromBase64Text(string text)
		{
			Image image = null;
			byte[] buffer = Convert.FromBase64String(text);
			IFormatter formatter = new BinaryFormatter();
			MemoryStream memoryStream = new MemoryStream(buffer);
			image = (Image)formatter.Deserialize(memoryStream);
			memoryStream.Close();
			return image;
		}

		public override bool CanExtend(object o)
		{
			return o is Chart;
		}

		public override PropertyDescriptorCollection GetProperties(object component, Attribute[] attrs)
		{
			return this.GetProperties(null, component, attrs);
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object component, Attribute[] attrs)
		{
			Chart chart = component as Chart;
			if (chart == null)
			{
				TypeConverter converter = TypeDescriptor.GetConverter(component);
				if (converter != null)
				{
					return converter.GetProperties(context, component, attrs);
				}
				return TypeDescriptor.GetProperties(component, attrs);
			}
			PropertyDescriptorCollection propertyDescriptorCollection = (attrs != null) ? TypeDescriptor.GetProperties(component, attrs) : TypeDescriptor.GetProperties(component);
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < propertyDescriptorCollection.Count; i++)
			{
				arrayList.Add(propertyDescriptorCollection);
			}
			PropertyDescriptor[] properties = (PropertyDescriptor[])arrayList.ToArray(typeof(PropertyDescriptor));
			return new PropertyDescriptorCollection(properties);
		}
	}
}
