using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace AspNetCore.Reporting.Chart.WebForms.Utilities
{
	internal abstract class SerializerBase
	{
		private class ItemInfo
		{
			public string name = "";

			public bool any;

			public bool startsWith;

			public bool endsWith;
		}

		private bool ignoreUnknown;

		private bool templateMode;

		private bool resetWhenLoading = true;

		private string serializableContent = "";

		private string nonSerializableContent = "";

		protected int binaryFormatVersion = 300;

		internal static FontConverter fontConverter = new FontConverter();

		internal static ColorConverter colorConverter = new ColorConverter();

		internal static SizeConverter sizeConverter = new SizeConverter();

		internal static ArrayConverter arrayConverter = new ArrayConverter();

		protected static CaseInsensitiveHashCodeProvider hashCodeProvider = CaseInsensitiveHashCodeProvider.Default;

		private ArrayList serializableContentList;

		private ArrayList nonSerializableContentList;

		public bool IgnoreUnknownAttributes
		{
			get
			{
				return this.ignoreUnknown;
			}
			set
			{
				this.ignoreUnknown = value;
			}
		}

		public bool TemplateMode
		{
			get
			{
				return this.templateMode;
			}
			set
			{
				this.templateMode = value;
			}
		}

		public bool ResetWhenLoading
		{
			get
			{
				return this.resetWhenLoading;
			}
			set
			{
				this.resetWhenLoading = value;
			}
		}

		public string SerializableContent
		{
			get
			{
				return this.serializableContent;
			}
			set
			{
				this.serializableContent = value;
				this.serializableContentList = null;
			}
		}

		public string NonSerializableContent
		{
			get
			{
				return this.nonSerializableContent;
			}
			set
			{
				this.nonSerializableContent = value;
				this.nonSerializableContentList = null;
			}
		}

		public virtual void ResetObjectProperties(object objectToReset)
		{
			this.ResetObjectProperties(objectToReset, null, this.GetObjectName(objectToReset));
		}

		protected virtual void ResetObjectProperties(object objectToReset, object parent, string elementName)
		{
			if (objectToReset != null)
			{
				if (objectToReset is IList && this.IsSerializableContent(elementName, parent))
				{
					((IList)objectToReset).Clear();
				}
				else
				{
					PropertyInfo[] properties = objectToReset.GetType().GetProperties();
					if (properties != null)
					{
						PropertyInfo[] array = properties;
						foreach (PropertyInfo propertyInfo in array)
						{
							PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(objectToReset)[propertyInfo.Name];
							if (propertyDescriptor != null)
							{
								SerializationVisibilityAttribute serializationVisibilityAttribute = (SerializationVisibilityAttribute)propertyDescriptor.Attributes[typeof(SerializationVisibilityAttribute)];
								if (serializationVisibilityAttribute != null && serializationVisibilityAttribute.Visibility == SerializationVisibility.Hidden)
								{
									continue;
								}
							}
							bool flag = this.IsSerializableContent(propertyInfo.Name, objectToReset);
							if (!this.IsChartBaseProperty(objectToReset, parent, propertyInfo))
							{
								if (propertyInfo.CanRead && propertyInfo.PropertyType.GetInterface("IList", true) != null)
								{
									if (flag)
									{
										bool flag2 = false;
										MethodInfo method = objectToReset.GetType().GetMethod("Reset" + propertyInfo.Name);
										if (method != null)
										{
											method.Invoke(objectToReset, null);
											flag2 = true;
										}
										if (!flag2)
										{
											((IList)propertyInfo.GetValue(objectToReset, null)).Clear();
										}
									}
									else
									{
										foreach (object item in (IList)propertyInfo.GetValue(objectToReset, null))
										{
											this.ResetObjectProperties(item, objectToReset, this.GetObjectName(item));
										}
									}
								}
								else if (propertyInfo.CanRead && propertyInfo.CanWrite && !(propertyInfo.Name == "Item"))
								{
									if (this.ShouldSerializeAsAttribute(propertyInfo, objectToReset))
									{
										if (flag && propertyDescriptor != null)
										{
											object value = propertyInfo.GetValue(objectToReset, null);
											DefaultValueAttribute defaultValueAttribute = (DefaultValueAttribute)propertyDescriptor.Attributes[typeof(DefaultValueAttribute)];
											if (defaultValueAttribute != null)
											{
												if (value == null)
												{
													if (defaultValueAttribute.Value != null)
													{
														propertyDescriptor.SetValue(objectToReset, defaultValueAttribute.Value);
													}
												}
												else if (!value.Equals(defaultValueAttribute.Value))
												{
													propertyDescriptor.SetValue(objectToReset, defaultValueAttribute.Value);
												}
											}
											else
											{
												MethodInfo method2 = objectToReset.GetType().GetMethod("Reset" + propertyInfo.Name);
												if (method2 != null)
												{
													method2.Invoke(objectToReset, null);
												}
											}
										}
									}
									else
									{
										this.ResetObjectProperties(propertyInfo.GetValue(objectToReset, null), objectToReset, propertyInfo.Name);
									}
								}
							}
						}
					}
				}
			}
		}

		public abstract void Serialize(object objectToSerialize, object destination);

		public abstract void Deserialize(object objectToDeserialize, object source);

		internal static string FontToString(Font font)
		{
			string text = SerializerBase.fontConverter.ConvertToInvariantString(font);
			if (font.GdiCharSet != 1)
			{
				text = text + ", GdiCharSet=" + font.GdiCharSet.ToString(CultureInfo.InvariantCulture);
			}
			if (font.GdiVerticalFont)
			{
				text += ", GdiVerticalFont";
			}
			return text;
		}

		internal static Font FontFromString(string fontString)
		{
			string text = fontString;
			byte b = 1;
			bool flag = false;
			int num = fontString.IndexOf(", GdiCharSet=", StringComparison.Ordinal);
			if (num >= 0)
			{
				string text2 = fontString.Substring(num + 13);
				int num2 = text2.IndexOf(",", StringComparison.Ordinal);
				if (num2 >= 0)
				{
					text2 = text2.Substring(0, num2);
				}
				b = (byte)int.Parse(text2, CultureInfo.InvariantCulture);
				if (text.Length > num)
				{
					text = text.Substring(0, num);
				}
			}
			num = fontString.IndexOf(", GdiVerticalFont", StringComparison.Ordinal);
			if (num >= 0)
			{
				flag = true;
				if (text.Length > num)
				{
					text = text.Substring(0, num);
				}
			}
			Font font = (Font)SerializerBase.fontConverter.ConvertFromInvariantString(text);
			if (!flag && b == 1)
			{
				return font;
			}
			Font result = new Font(font.Name, font.SizeInPoints, font.Style, GraphicsUnit.Point, b, flag);
			font.Dispose();
			return result;
		}

		internal static short GetStringHashCode(string str)
		{
			return (short)(SerializerBase.hashCodeProvider.GetHashCode(str) + str.Length * 2);
		}

		protected short ReadHashID(BinaryReader reader, bool isCollectionMember)
		{
			if (this.binaryFormatVersion <= 200)
			{
				return this.MapHashValueVersion200(reader.ReadInt16());
			}
			return reader.ReadInt16();
		}

		protected short MapHashValueVersion200(short oldHashValue)
		{
			if (oldHashValue == 0)
			{
				return 0;
			}
			int num = Array.IndexOf(StringHashCodeConverter.hashCodes200, oldHashValue);
			if (num < 0)
			{
				throw new InvalidOperationException(SR.ExceptionChartSerializerPropertyIdUnknown);
			}
			return StringHashCodeConverter.hashCodes300[num];
		}

		protected bool IsChartBaseProperty(object objectToSerialize, object parent, PropertyInfo pi)
		{
			bool result = false;
			if (parent == null)
			{
				Type type = objectToSerialize.GetType();
				while (type != null)
				{
					if (pi.DeclaringType == type)
					{
						result = false;
					}
					else
					{
						if (!(type.FullName == "AspNetCore.Reporting.Chart.WinForms.Chart") && !(type.FullName == "AspNetCore.Reporting.Chart.WebForms.Chart") && !(type.FullName == "Dundas.Charting.Wizard.WinControl.Chart") && !(type.FullName == "Dundas.Olap.WebUIControls.Chart"))
						{
							type = type.BaseType;
							continue;
						}
						result = true;
					}
					break;
				}
			}
			return result;
		}

		internal static string ImageToString(Image image)
		{
			MemoryStream memoryStream = new MemoryStream();
			image.Save(memoryStream, ImageFormat.Png);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			StringBuilder stringBuilder = new StringBuilder();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder, CultureInfo.InvariantCulture));
			byte[] array = memoryStream.ToArray();
			xmlTextWriter.WriteBase64(array, 0, array.Length);
			xmlTextWriter.Close();
			memoryStream.Close();
			return stringBuilder.ToString();
		}

		protected static Image ImageFromString(string data)
		{
			byte[] array = new byte[1000];
			MemoryStream memoryStream = new MemoryStream();
			XmlTextReader xmlTextReader = new XmlTextReader(new StringReader("<base64>" + data + "</base64>"));
			xmlTextReader.Read();
			int num = 0;
			while ((num = xmlTextReader.ReadBase64(array, 0, 1000)) > 0)
			{
				memoryStream.Write(array, 0, num);
			}
			xmlTextReader.Read();
			memoryStream.Seek(0L, SeekOrigin.Begin);
			Image result = new Bitmap(Image.FromStream(memoryStream));
			xmlTextReader.Close();
			memoryStream.Close();
			return result;
		}

		internal string GetObjectName(object obj)
		{
			string text = obj.GetType().ToString();
			return text.Substring(text.LastIndexOf('.') + 1);
		}

		protected object GetListNewItem(IList list, string itemTypeName, ref string itemName, ref bool reusedObject)
		{
			Type type = null;
			if (itemTypeName.Length > 0)
			{
				string str = "AspNetCore.Reporting.Chart.WebForms.";
				type = Type.GetType(str + itemTypeName, false, true);
			}
			reusedObject = false;
			PropertyInfo property = list.GetType().GetProperty("Item");
			MethodInfo method = list.GetType().GetMethod("GetIndex", new Type[1]
			{
				typeof(string)
			});
			ConstructorInfo constructorInfo = null;
			if (property != null)
			{
				if (itemName != null && itemName.Length > 0)
				{
					bool flag = false;
					if (method != null)
					{
						try
						{
							int num = -1;
							object obj = method.Invoke(list, new object[1]
							{
								itemName
							});
							if (obj is int)
							{
								num = (int)obj;
								flag = true;
							}
							if (num != -1)
							{
								object obj2 = list[num];
								if (obj2 != null)
								{
									list.Remove(obj2);
									reusedObject = true;
									return obj2;
								}
							}
						}
						catch (Exception)
						{
						}
					}
					if (!flag)
					{
						try
						{
							object value = property.GetValue(list, new object[1]
							{
								itemName
							});
							if (value != null)
							{
								list.Remove(value);
								reusedObject = true;
								return value;
							}
						}
						catch (Exception)
						{
						}
					}
					itemName = null;
				}
				constructorInfo = ((type == null) ? property.PropertyType.GetConstructor(Type.EmptyTypes) : type.GetConstructor(Type.EmptyTypes));
				if (constructorInfo == null)
				{
					throw new InvalidOperationException(SR.ExceptionChartSerializerDefaultConstructorUndefined(property.PropertyType.ToString()));
				}
				return constructorInfo.Invoke(null);
			}
			return null;
		}

		protected bool ShouldSerializeAsAttribute(PropertyInfo pi, object parent)
		{
			if (parent != null)
			{
				PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(parent)[pi.Name];
				if (propertyDescriptor != null)
				{
					SerializationVisibilityAttribute serializationVisibilityAttribute = (SerializationVisibilityAttribute)propertyDescriptor.Attributes[typeof(SerializationVisibilityAttribute)];
					if (serializationVisibilityAttribute != null)
					{
						if (serializationVisibilityAttribute.Visibility == SerializationVisibility.Attribute)
						{
							return true;
						}
						if (serializationVisibilityAttribute.Visibility == SerializationVisibility.Element)
						{
							return false;
						}
					}
				}
			}
			if (!pi.PropertyType.IsClass)
			{
				return true;
			}
			if (pi.PropertyType != typeof(string) && pi.PropertyType != typeof(Font) && pi.PropertyType != typeof(Color) && pi.PropertyType != typeof(Image))
			{
				return false;
			}
			return true;
		}

		protected bool SerializeICollAsAtribute(PropertyInfo pi, object objectToSerialize)
		{
			if (objectToSerialize != null)
			{
				PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(objectToSerialize)[pi.Name];
				if (propertyDescriptor != null)
				{
					SerializationVisibilityAttribute serializationVisibilityAttribute = (SerializationVisibilityAttribute)propertyDescriptor.Attributes[typeof(SerializationVisibilityAttribute)];
					if (serializationVisibilityAttribute != null && serializationVisibilityAttribute.Visibility == SerializationVisibility.Attribute)
					{
						return true;
					}
				}
			}
			return false;
		}

		internal bool IsSerializableContent(string propertyName, object parent)
		{
			bool flag = true;
			if (this.serializableContent.Length > 0 || this.nonSerializableContent.Length > 0)
			{
				int num = 0;
				int num2 = 0;
				string objectName = this.GetObjectName(parent);
				flag = this.IsPropertyInList(this.GetSerializableContentList(), objectName, propertyName, out num, out num2);
				if (flag)
				{
					int num3 = 0;
					int num4 = 0;
					if (this.IsPropertyInList(this.GetNonSerializableContentList(), objectName, propertyName, out num3, out num4) && num3 + num4 > num + num2)
					{
						flag = false;
					}
				}
			}
			return flag;
		}

		private bool IsPropertyInList(ArrayList contentList, string className, string propertyName, out int classFitType, out int propertyFitType)
		{
			classFitType = 0;
			propertyFitType = 0;
			if (contentList != null)
			{
				for (int i = 0; i < contentList.Count; i += 2)
				{
					classFitType = 0;
					propertyFitType = 0;
					if (this.NameMatchMask((ItemInfo)contentList[i], className, out classFitType) && this.NameMatchMask((ItemInfo)contentList[i + 1], propertyName, out propertyFitType))
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool NameMatchMask(ItemInfo itemInfo, string objectName, out int type)
		{
			type = 0;
			if (itemInfo.any)
			{
				type = 1;
				return true;
			}
			if (itemInfo.endsWith && itemInfo.name.Length <= objectName.Length && objectName.Substring(0, itemInfo.name.Length) == itemInfo.name)
			{
				type = 2;
				return true;
			}
			if (itemInfo.startsWith && itemInfo.name.Length <= objectName.Length && objectName.Substring(objectName.Length - itemInfo.name.Length, itemInfo.name.Length) == itemInfo.name)
			{
				type = 2;
				return true;
			}
			if (itemInfo.name == objectName)
			{
				type = 3;
				return true;
			}
			return false;
		}

		private ArrayList GetSerializableContentList()
		{
			if (this.serializableContentList == null)
			{
				this.serializableContentList = new ArrayList();
				this.FillContentList(this.serializableContentList, (this.SerializableContent.Length > 0) ? this.SerializableContent : "*.*");
			}
			return this.serializableContentList;
		}

		private ArrayList GetNonSerializableContentList()
		{
			if (this.nonSerializableContentList == null)
			{
				this.nonSerializableContentList = new ArrayList();
				this.FillContentList(this.nonSerializableContentList, this.NonSerializableContent);
			}
			return this.nonSerializableContentList;
		}

		private void FillContentList(ArrayList list, string content)
		{
			if (content.Length <= 0)
			{
				return;
			}
			string[] array = content.Split(',');
			string[] array2 = array;
			int num = 0;
			while (true)
			{
				if (num < array2.Length)
				{
					string text = array2[num];
					ItemInfo itemInfo = new ItemInfo();
					ItemInfo itemInfo2 = new ItemInfo();
					int num2 = text.IndexOf('.');
					if (num2 == -1)
					{
						throw new ArgumentException(SR.ExceptionChartSerializerContentStringFormatInvalid);
					}
					itemInfo.name = text.Substring(0, num2).Trim();
					itemInfo2.name = text.Substring(num2 + 1).Trim();
					if (itemInfo.name.Length == 0)
					{
						throw new ArgumentException(SR.ExceptionChartSerializerClassNameUndefined);
					}
					if (itemInfo2.name.Length == 0)
					{
						throw new ArgumentException(SR.ExceptionChartSerializerPropertyNameUndefined);
					}
					if (itemInfo2.name.IndexOf('.') == -1)
					{
						this.CheckWildCars(itemInfo);
						this.CheckWildCars(itemInfo2);
						list.Add(itemInfo);
						list.Add(itemInfo2);
						num++;
						continue;
					}
					break;
				}
				return;
			}
			throw new ArgumentException(SR.ExceptionChartSerializerContentStringFormatInvalid);
		}

		private void CheckWildCars(ItemInfo info)
		{
			if (info.name == "*")
			{
				info.any = true;
			}
			else if (info.name[info.name.Length - 1] == '*')
			{
				info.endsWith = true;
				info.name = info.name.TrimEnd('*');
			}
			else if (info.name[0] == '*')
			{
				info.startsWith = true;
				info.name = info.name.TrimStart('*');
			}
		}
	}
}
