using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace AspNetCore.Reporting.Gauge.WebForms
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
							if (!this.IsGaugeBaseProperty(objectToReset, parent, propertyInfo))
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

		protected bool IsGaugeBaseProperty(object objectToSerialize, object parent, PropertyInfo pi)
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

		protected string GetObjectName(object obj)
		{
			string text = obj.GetType().ToString();
			return text.Substring(text.LastIndexOf('.') + 1);
		}

		protected object GetListNewItem(IList list, string itemTypeName, ref string itemName, ref bool reusedObject)
		{
			Type type = null;
			if (itemTypeName.Length > 0)
			{
				string str = "AspNetCore.Reporting.Gauge.WebForms.";
				type = Type.GetType(str + itemTypeName, false, true);
			}
			reusedObject = false;
			PropertyInfo property = list.GetType().GetProperty("Item");
			ConstructorInfo constructorInfo = null;
			if (property != null)
			{
				if (itemName != null && itemName.Length > 0)
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
					throw new InvalidOperationException(Utils.SRGetStr("ExceptionSerializerInvalidConstructor", property.PropertyType));
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
			if (pi.PropertyType != typeof(string) && pi.PropertyType != typeof(Font) && pi.PropertyType != typeof(Cursor) && pi.PropertyType != typeof(Color) && pi.PropertyType != typeof(Image))
			{
				return false;
			}
			return true;
		}

		protected bool IsSerializableContent(string propertyName, object parent)
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
						throw new ArgumentException(Utils.SRGetStr("ExceptionSerializerInvalidContentFormat"));
					}
					itemInfo.name = text.Substring(0, num2).Trim();
					itemInfo2.name = text.Substring(num2 + 1).Trim();
					if (itemInfo.name.Length == 0)
					{
						throw new ArgumentException(Utils.SRGetStr("ExceptionSerializerInvalidClassName"));
					}
					if (itemInfo2.name.Length == 0)
					{
						throw new ArgumentException(Utils.SRGetStr("ExceptionSerializerInvalidPropertyName"));
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
			throw new ArgumentException(Utils.SRGetStr("ExceptionSerializerInvalidContentFormat"));
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
