using AspNetCore.Reporting.Chart.WebForms.Design;
using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.ComponentModel;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[TypeConverter(typeof(CustomAttributeTypeConverter))]
	internal class CustomAttributes
	{
		internal DataPointAttributes m_DataPointAttributes;

		internal virtual DataPointAttributes DataPointAttributes
		{
			get
			{
				return this.m_DataPointAttributes;
			}
			set
			{
				this.m_DataPointAttributes = value;
			}
		}

		public CustomAttributes(DataPointAttributes attributes)
		{
			this.DataPointAttributes = attributes;
		}

		internal virtual string GetUserDefinedAttributes()
		{
			return this.GetUserDefinedAttributes(true);
		}

		internal virtual string GetUserDefinedAttributes(bool userDefined)
		{
			string customAttributes = this.DataPointAttributes.CustomAttributes;
			string text = string.Empty;
			Series series = (this.DataPointAttributes is Series) ? ((Series)this.DataPointAttributes) : this.DataPointAttributes.series;
			CustomAttributeRegistry customAttributeRegistry = (CustomAttributeRegistry)series.chart.chartPicture.common.container.GetService(typeof(CustomAttributeRegistry));
			customAttributes = customAttributes.Replace("\\,", "\\x45");
			customAttributes = customAttributes.Replace("\\=", "\\x46");
			if (customAttributes.Length > 0)
			{
				string[] array = customAttributes.Split(',');
				string[] array2 = array;
				foreach (string text2 in array2)
				{
					string[] array3 = text2.Split('=');
					if (array3.Length != 2)
					{
						throw new FormatException(SR.ExceptionAttributeInvalidFormat);
					}
					array3[0] = array3[0].Trim();
					array3[1] = array3[1].Trim();
					if (array3[0].Length == 0)
					{
						throw new FormatException(SR.ExceptionAttributeInvalidFormat);
					}
					bool flag = true;
					foreach (CustomAttributeInfo registeredCustomAttribute in customAttributeRegistry.registeredCustomAttributes)
					{
						if (string.Compare(registeredCustomAttribute.Name, array3[0], StringComparison.OrdinalIgnoreCase) == 0)
						{
							flag = false;
						}
					}
					if (flag == userDefined)
					{
						if (text.Length > 0)
						{
							text += ", ";
						}
						string text3 = array3[1].Replace("\\x45", ",");
						text3 = text3.Replace("\\x46", "=");
						text = text + array3[0] + "=" + text3;
					}
				}
			}
			return text;
		}

		internal virtual void SetUserDefinedAttributes(string val)
		{
			string text = this.GetUserDefinedAttributes(false);
			if (val.Length > 0)
			{
				if (text.Length > 0)
				{
					text += ", ";
				}
				text += val;
			}
			this.DataPointAttributes.CustomAttributes = text;
		}

		public override bool Equals(object obj)
		{
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
