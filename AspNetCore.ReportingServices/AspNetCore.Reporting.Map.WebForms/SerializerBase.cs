using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal abstract class SerializerBase
	{
		private bool ignoreUnknown;

		private bool templateMode;

		private bool resetWhenLoading = true;

		private string serializableContent = "";

		private string nonSerializableContent = "";

		internal static FontConverter fontConverter = new FontConverter();

		internal static CursorConverter cursorConverter = new CursorConverter();

		internal static ColorConverter colorConverter = new ColorConverter();

		internal static SizeConverter sizeConverter = new SizeConverter();

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

		protected void ResetObjectProperties(object objectToReset, object parent, string elementName)
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
							if (!this.IsMapBaseProperty(objectToReset, parent, propertyInfo))
							{
								if (propertyInfo.CanRead && propertyInfo.PropertyType.GetInterface("IList", true) != null)
								{
									if (flag)
									{
										((IList)propertyInfo.GetValue(objectToReset, null)).Clear();
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
												if (!value.Equals(defaultValueAttribute.Value))
												{
													propertyDescriptor.SetValue(objectToReset, defaultValueAttribute.Value);
												}
											}
											else
											{
												MethodInfo method = objectToReset.GetType().GetMethod("Reset" + propertyInfo.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
												if (method != null)
												{
													method.Invoke(objectToReset, null);
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

		protected bool IsMapBaseProperty(object objectToSerialize, object parent, PropertyInfo pi)
		{
			bool result = false;
			if (parent == null)
			{
				Type type = objectToSerialize.GetType();
				while (type != null)
				{
					if (pi.DeclaringType != type)
					{
						type = type.BaseType;
						continue;
					}
					result = false;
					break;
				}
			}
			return result;
		}

		protected string ImageToString(Image image)
		{
			MemoryStream memoryStream = new MemoryStream();
			image.Save(memoryStream, ImageFormat.Png);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			byte[] inArray = memoryStream.ToArray();
			memoryStream.Close();
			return Convert.ToBase64String(inArray);
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

		internal static string StreamToString(Stream stream)
		{
			stream.Seek(0L, SeekOrigin.Begin);
			byte[] array = new byte[stream.Length];
			stream.Read(array, 0, array.Length);
			return Convert.ToBase64String(array);
		}

		internal static Stream StreamFromString(string data)
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
			xmlTextReader.Close();
			return memoryStream;
		}

		protected string GetObjectName(object obj)
		{
			string text = obj.GetType().ToString();
			return text.Substring(text.LastIndexOf('.') + 1);
		}

		protected object GetListNewItem(IList list, string itemTypeName, ref string itemName, ref bool reusedObject)
		{
			Type type = null;
			if (!string.IsNullOrEmpty(itemTypeName))
			{
				string str = "AspNetCore.Reporting.Map.WebForms.";
				type = Type.GetType(str + itemTypeName, false, true);
			}
			reusedObject = false;
			PropertyInfo property = list.GetType().GetProperty("Item");
			ConstructorInfo constructorInfo = null;
			if (property != null)
			{
				if (!string.IsNullOrEmpty(itemName))
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
					itemName = null;
				}
				constructorInfo = ((type == null) ? property.PropertyType.GetConstructor(Type.EmptyTypes) : type.GetConstructor(Type.EmptyTypes));
				if (constructorInfo == null)
				{
					throw new InvalidOperationException("Can't deserialize property. Can't find default public constructor for type \"" + property.PropertyType + "\".");
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
			if (pi.PropertyType != typeof(string) && pi.PropertyType != typeof(Font) && pi.PropertyType != typeof(CultureInfo) && pi.PropertyType != typeof(Color) && pi.PropertyType != typeof(Image) && pi.PropertyType != typeof(ShapeData) && pi.PropertyType != typeof(Stream) && pi.PropertyType != typeof(Type) && pi.PropertyType != typeof(DateTime) && pi.PropertyType != typeof(Margins) && pi.PropertyType != typeof(MapCoordinate))
			{
				return false;
			}
			return true;
		}

		protected bool IsSerializableContent(string propertyName, object parent)
		{
			bool flag = true;
			if (!string.IsNullOrEmpty(this.serializableContent) || !string.IsNullOrEmpty(this.nonSerializableContent))
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
				this.FillContentList(this.serializableContentList, this.SerializableContent);
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
			if (string.IsNullOrEmpty(content))
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
						throw new ArgumentException("Invalid format of the serializable content string.");
					}
					itemInfo.name = text.Substring(0, num2).Trim();
					itemInfo2.name = text.Substring(num2 + 1).Trim();
					if (string.IsNullOrEmpty(itemInfo.name))
					{
						throw new ArgumentException("Invalid format of the serializable content string. Class name not set.");
					}
					if (string.IsNullOrEmpty(itemInfo2.name))
					{
						throw new ArgumentException("Invalid format of the serializable content string. Property name not set.");
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
			throw new ArgumentException("Invalid format of the serializable content string.");
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

		internal static object ReadConvertValue(object obj, string propertyValue, BinaryReader binaryReader)
		{
			if (binaryReader == null)
			{
				if (obj is string)
				{
					return propertyValue;
				}
				if (obj is byte)
				{
					if (!string.IsNullOrEmpty(propertyValue))
					{
						return (byte)propertyValue[0];
					}
					return (byte)0;
				}
				if (obj is bool)
				{
					return bool.Parse(propertyValue);
				}
				if (obj is long)
				{
					return long.Parse(propertyValue, CultureInfo.InvariantCulture);
				}
				if (obj is int)
				{
					return int.Parse(propertyValue, CultureInfo.InvariantCulture);
				}
				if (obj is float)
				{
					return float.Parse(propertyValue, CultureInfo.InvariantCulture);
				}
				if (obj is double)
				{
					if (string.Compare(propertyValue, "Auto", StringComparison.OrdinalIgnoreCase) == 0)
					{
						return double.NaN;
					}
					return double.Parse(propertyValue, CultureInfo.InvariantCulture);
				}
				if (obj is Font)
				{
					return (Font)SerializerBase.fontConverter.ConvertFromString(null, CultureInfo.InvariantCulture, propertyValue);
				}
				if (obj is CultureInfo)
				{
					return CultureInfo.GetCultureInfoByIetfLanguageTag(propertyValue);
				}
                if (obj is Cursor)
                {
                    return (Cursor)SerializerBase.cursorConverter.ConvertFromString(null, CultureInfo.InvariantCulture, propertyValue);
                }
                if (obj is Color)
				{
					return (Color)SerializerBase.colorConverter.ConvertFromString(null, CultureInfo.InvariantCulture, propertyValue);
				}
				if (obj is Enum)
				{
					return Enum.Parse(obj.GetType(), propertyValue);
				}
				if (obj is Image)
				{
					return SerializerBase.ImageFromString(propertyValue);
				}
				if (obj is ShapeData)
				{
					return ShapeData.ShapeDataFromString(propertyValue);
				}
				if (obj is Stream)
				{
					return SerializerBase.StreamFromString(propertyValue);
				}
				if (obj is double[])
				{
					string[] array = propertyValue.Split(',');
					double[] array2 = new double[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						array2[i] = double.Parse(array[i], CultureInfo.InvariantCulture);
					}
					return array2;
				}
				if (obj is Size)
				{
					return (Size)SerializerBase.sizeConverter.ConvertFromString(null, CultureInfo.InvariantCulture, propertyValue);
				}
				if (obj is DateTime)
				{
					return XmlConvert.ToDateTime(propertyValue);
				}
				if (obj is Margins)
				{
					return Margins.Parse(propertyValue);
				}
				if (obj is MapCoordinate)
				{
					return MapCoordinate.Parse(propertyValue);
				}
				if (obj is Type)
				{
					return Type.GetType(propertyValue);
				}
				throw new InvalidOperationException("Serializer do not support objects of type \"" + obj.GetType().ToString() + "\"");
			}
			if (obj is bool)
			{
				return binaryReader.ReadBoolean();
			}
			if (obj is double)
			{
				return binaryReader.ReadDouble();
			}
			if (obj is string)
			{
				return binaryReader.ReadString();
			}
			if (obj is int)
			{
				return binaryReader.ReadInt32();
			}
			if (obj is long)
			{
				return binaryReader.ReadInt64();
			}
			if (obj is float)
			{
				return binaryReader.ReadSingle();
			}
			if (obj is Enum)
			{
				return Enum.Parse(obj.GetType(), binaryReader.ReadString());
			}
			if (obj is byte)
			{
				return binaryReader.ReadByte();
			}
			if (obj is Font)
			{
				return SerializerBase.fontConverter.ConvertFromString(null, CultureInfo.InvariantCulture, binaryReader.ReadString());
			}
			if (obj is CultureInfo)
			{
				return CultureInfo.GetCultureInfoByIetfLanguageTag(binaryReader.ReadString());
			}
            if (obj is Cursor)
            {
                return SerializerBase.cursorConverter.ConvertFromString(null, CultureInfo.InvariantCulture, binaryReader.ReadString());
            }
            if (obj is Color)
			{
				return Color.FromArgb(binaryReader.ReadInt32());
			}
			if (obj is DateTime)
			{
				return DateTime.FromOADate(binaryReader.ReadDouble());
			}
			if (obj is Margins)
			{
				return new Margins(binaryReader.ReadInt32(), binaryReader.ReadInt32(), binaryReader.ReadInt32(), binaryReader.ReadInt32());
			}
			if (obj is MapCoordinate)
			{
				return new MapCoordinate(binaryReader.ReadDouble());
			}
			if (obj is Type)
			{
				return Type.GetType(binaryReader.ReadString());
			}
			if (obj is Size)
			{
				return new Size(binaryReader.ReadInt32(), binaryReader.ReadInt32());
			}
			if (obj is double[])
			{
				double[] array3 = new double[binaryReader.ReadInt32()];
				for (int j = 0; j < array3.Length; j++)
				{
					array3[j] = binaryReader.ReadDouble();
				}
				return array3;
			}
			if (obj is Image)
			{
				int num = binaryReader.ReadInt32();
				MemoryStream memoryStream = new MemoryStream(num + 10);
				memoryStream.Write(binaryReader.ReadBytes(num), 0, num);
				return new Bitmap(Image.FromStream(memoryStream));
			}
			throw new InvalidOperationException("Serializer do not support objects of type \"" + obj.GetType().ToString() + "\"");
		}
	}
}
