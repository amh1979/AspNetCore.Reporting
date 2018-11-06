using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace AspNetCore.Reporting.Chart.WebForms.Utilities
{
	internal class BinaryFormatSerializer : SerializerBase
	{
		public override void Serialize(object objectToSerialize, object destination)
		{
			if (objectToSerialize == null)
			{
				throw new ArgumentNullException("objectToSerialize");
			}
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			if (destination is string)
			{
				this.Serialize(objectToSerialize, (string)destination);
				return;
			}
			if (destination is Stream)
			{
				this.Serialize(objectToSerialize, (Stream)destination);
				return;
			}
			if (destination is BinaryWriter)
			{
				this.Serialize(objectToSerialize, (BinaryWriter)destination);
				return;
			}
			throw new ArgumentException(SR.ExceptionChartSerializerDestinationObjectInvalid, "destination");
		}

		public void Serialize(object objectToSerialize, string fileName)
		{
			FileStream fileStream = new FileStream(fileName, FileMode.Create);
			this.Serialize(objectToSerialize, new BinaryWriter(fileStream));
			fileStream.Close();
		}

		public void Serialize(object objectToSerialize, Stream stream)
		{
			this.Serialize(objectToSerialize, new BinaryWriter(stream));
		}

		public void Serialize(object objectToSerialize, BinaryWriter writer)
		{
			if (objectToSerialize == null)
			{
				throw new ArgumentNullException("objectToSerialize");
			}
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			base.binaryFormatVersion = 300;
			char[] chars = new char[15]
			{
				'D',
				'C',
				'B',
				'F',
				'3',
				'0',
				'0',
				'\0',
				'\0',
				'\0',
				'\0',
				'\0',
				'\0',
				'\0',
				'\0'
			};
			writer.Write(chars);
			this.SerializeObject(objectToSerialize, null, base.GetObjectName(objectToSerialize), writer);
			writer.Flush();
			writer.Seek(0, SeekOrigin.Begin);
		}

		protected virtual void SerializeObject(object objectToSerialize, object parent, string elementName, BinaryWriter writer)
		{
			if (objectToSerialize != null)
			{
				if (parent != null)
				{
					PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(parent)[elementName];
					if (propertyDescriptor != null)
					{
						SerializationVisibilityAttribute serializationVisibilityAttribute = (SerializationVisibilityAttribute)propertyDescriptor.Attributes[typeof(SerializationVisibilityAttribute)];
						if (serializationVisibilityAttribute != null && serializationVisibilityAttribute.Visibility == SerializationVisibility.Hidden)
						{
							return;
						}
					}
				}
				if (objectToSerialize is ICollection)
				{
					this.SerializeCollection(objectToSerialize, elementName, writer);
				}
				else
				{
					writer.Write(SerializerBase.GetStringHashCode(elementName));
					long num = writer.Seek(0, SeekOrigin.Current);
					ArrayList arrayList = new ArrayList();
					PropertyInfo[] properties = objectToSerialize.GetType().GetProperties();
					if (properties != null)
					{
						PropertyInfo[] array = properties;
						foreach (PropertyInfo propertyInfo in array)
						{
							if (!base.IsChartBaseProperty(objectToSerialize, parent, propertyInfo))
							{
								if (propertyInfo.CanRead && propertyInfo.PropertyType.GetInterface("ICollection", true) != null && !base.SerializeICollAsAtribute(propertyInfo, objectToSerialize))
								{
									bool flag = base.IsSerializableContent(propertyInfo.Name, objectToSerialize);
									if (flag && objectToSerialize != null)
									{
										PropertyDescriptor propertyDescriptor2 = TypeDescriptor.GetProperties(objectToSerialize)[propertyInfo.Name];
										if (propertyDescriptor2 != null)
										{
											SerializationVisibilityAttribute serializationVisibilityAttribute2 = (SerializationVisibilityAttribute)propertyDescriptor2.Attributes[typeof(SerializationVisibilityAttribute)];
											if (serializationVisibilityAttribute2 != null && serializationVisibilityAttribute2.Visibility == SerializationVisibility.Hidden)
											{
												flag = false;
											}
										}
									}
									MethodInfo method = objectToSerialize.GetType().GetMethod("ShouldSerialize" + propertyInfo.Name);
									if (flag && method != null)
									{
										object obj = method.Invoke(objectToSerialize, null);
										if (obj is bool && !(bool)obj)
										{
											flag = false;
										}
									}
									if (flag)
									{
										arrayList.Add(propertyInfo.Name);
										this.SerializeCollection(propertyInfo.GetValue(objectToSerialize, null), propertyInfo.Name, writer);
									}
								}
								else if (propertyInfo.CanRead && propertyInfo.CanWrite && !(propertyInfo.Name == "Item"))
								{
									bool flag2 = true;
									MethodInfo method2 = objectToSerialize.GetType().GetMethod("ShouldSerialize" + propertyInfo.Name);
									if (method2 != null)
									{
										object obj2 = method2.Invoke(objectToSerialize, null);
										if (obj2 is bool && !(bool)obj2)
										{
											flag2 = false;
										}
									}
									if (flag2)
									{
										if (base.ShouldSerializeAsAttribute(propertyInfo, objectToSerialize))
										{
											if (base.IsSerializableContent(propertyInfo.Name, objectToSerialize))
											{
												this.SerializeProperty(propertyInfo.GetValue(objectToSerialize, null), objectToSerialize, propertyInfo.Name, writer);
											}
										}
										else
										{
											this.SerializeObject(propertyInfo.GetValue(objectToSerialize, null), objectToSerialize, propertyInfo.Name, writer);
										}
									}
									arrayList.Add(propertyInfo.Name);
								}
							}
						}
						this.CheckPropertiesID(arrayList);
					}
					if (writer.Seek(0, SeekOrigin.Current) == num)
					{
						writer.Seek(-2, SeekOrigin.Current);
						writer.Write((short)0);
						writer.Seek(-2, SeekOrigin.Current);
					}
					else
					{
						writer.Write((short)0);
					}
				}
			}
		}

		protected virtual void SerializeCollection(object objectToSerialize, string elementName, BinaryWriter writer)
		{
			if (objectToSerialize is ICollection)
			{
				writer.Write(SerializerBase.GetStringHashCode(elementName));
				long num = writer.Seek(0, SeekOrigin.Current);
				foreach (object item in (ICollection)objectToSerialize)
				{
					this.SerializeObject(item, objectToSerialize, base.GetObjectName(item), writer);
				}
				if (writer.Seek(0, SeekOrigin.Current) == num)
				{
					writer.Seek(-2, SeekOrigin.Current);
					writer.Write((short)0);
					writer.Seek(-2, SeekOrigin.Current);
				}
				else
				{
					writer.Write((short)0);
				}
			}
		}

		protected virtual void SerializeProperty(object objectToSerialize, object parent, string elementName, BinaryWriter writer)
		{
			if (objectToSerialize != null && parent != null)
			{
				PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(parent)[elementName];
				if (propertyDescriptor != null)
				{
					DefaultValueAttribute defaultValueAttribute = (DefaultValueAttribute)propertyDescriptor.Attributes[typeof(DefaultValueAttribute)];
					if (defaultValueAttribute != null && objectToSerialize.Equals(defaultValueAttribute.Value))
					{
						return;
					}
					SerializationVisibilityAttribute serializationVisibilityAttribute = (SerializationVisibilityAttribute)propertyDescriptor.Attributes[typeof(SerializationVisibilityAttribute)];
					if (serializationVisibilityAttribute != null && serializationVisibilityAttribute.Visibility == SerializationVisibility.Hidden)
					{
						return;
					}
				}
				this.WritePropertyValue(objectToSerialize, parent, elementName, writer);
			}
		}

		protected void WritePropertyValue(object obj, object parent, string elementName, BinaryWriter writer)
		{
			writer.Write(SerializerBase.GetStringHashCode(elementName));
			if (obj is bool)
			{
				writer.Write((bool)obj);
				return;
			}
			if (obj is double)
			{
				writer.Write((double)obj);
				return;
			}
			if (obj is string)
			{
				writer.Write((string)obj);
				return;
			}
			if (obj is int)
			{
				writer.Write((int)obj);
				return;
			}
			if (obj is long)
			{
				writer.Write((long)obj);
				return;
			}
			if (obj is float)
			{
				writer.Write((float)obj);
				return;
			}
			if (obj.GetType().IsEnum)
			{
				string value = ((Enum)obj).ToString();
				writer.Write(value);
				return;
			}
			if (obj is byte)
			{
				writer.Write((byte)obj);
				return;
			}
			if (obj is Font)
			{
				writer.Write(SerializerBase.FontToString((Font)obj));
				return;
			}
			if (obj is Color)
			{
				writer.Write(((Color)obj).ToArgb());
				return;
			}
			if (obj is DateTime)
			{
				writer.Write(((DateTime)obj).Ticks);
				return;
			}
			if (obj is Size)
			{
				writer.Write(((Size)obj).Width);
				writer.Write(((Size)obj).Height);
				return;
			}
			if (obj is double[])
			{
				double[] array = (double[])obj;
				writer.Write(array.Length);
				double[] array2 = array;
				foreach (double value2 in array2)
				{
					writer.Write(value2);
				}
				return;
			}
			if (obj is Color[])
			{
				Color[] array3 = (Color[])obj;
				writer.Write(array3.Length);
				Color[] array4 = array3;
				for (int j = 0; j < array4.Length; j++)
				{
					Color color = array4[j];
					writer.Write(color.ToArgb());
				}
				return;
			}
			if (obj is Image)
			{
				MemoryStream memoryStream = new MemoryStream();
				((Image)obj).Save(memoryStream, ((Image)obj).RawFormat);
				int value3 = (int)memoryStream.Seek(0L, SeekOrigin.End);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				writer.Write(value3);
				writer.Write(memoryStream.ToArray());
				memoryStream.Close();
				return;
			}
			if (obj is Margins)
			{
				writer.Write(((Margins)obj).Top);
				writer.Write(((Margins)obj).Bottom);
				writer.Write(((Margins)obj).Left);
				writer.Write(((Margins)obj).Right);
				return;
			}
			throw new InvalidOperationException(SR.ExceptionChartSerializerBinaryTypeUnsupported(obj.GetType().ToString()));
		}

		public void CheckPropertiesID(ArrayList propNames)
		{
		}

		public override void Deserialize(object objectToDeserialize, object source)
		{
			if (objectToDeserialize == null)
			{
				throw new ArgumentNullException("objectToDeserialize");
			}
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (source is string)
			{
				this.Deserialize(objectToDeserialize, (string)source);
				return;
			}
			if (source is Stream)
			{
				this.Deserialize(objectToDeserialize, (Stream)source);
				return;
			}
			if (source is BinaryWriter)
			{
				this.Deserialize(objectToDeserialize, (BinaryWriter)source);
				return;
			}
			throw new ArgumentException(SR.ExceptionChartSerializerSourceObjectInvalid, "source");
		}

		public void Deserialize(object objectToDeserialize, string fileName)
		{
			FileStream fileStream = new FileStream(fileName, FileMode.Open);
			this.Deserialize(objectToDeserialize, new BinaryReader(fileStream));
			fileStream.Close();
		}

		public void Deserialize(object objectToDeserialize, Stream stream)
		{
			this.Deserialize(objectToDeserialize, new BinaryReader(stream));
		}

		public void Deserialize(object objectToDeserialize, BinaryReader reader)
		{
			if (objectToDeserialize == null)
			{
				throw new ArgumentNullException("objectToDeserialize");
			}
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (base.IgnoreUnknownAttributes)
			{
				throw new InvalidOperationException(SR.ExceptionChartSerializerBinaryIgnoreUnknownAttributesUnsupported);
			}
			char[] array = reader.ReadChars(15);
			if (array[0] == 'D' && array[1] == 'C' && array[2] == 'B' && array[3] == 'F')
			{
				base.binaryFormatVersion = int.Parse(new string(array, 4, 3), CultureInfo.InvariantCulture);
				base.ReadHashID(reader, false);
				if (base.ResetWhenLoading)
				{
					this.ResetObjectProperties(objectToDeserialize);
				}
				this.DeserializeObject(objectToDeserialize, null, base.GetObjectName(objectToDeserialize), reader);
				return;
			}
			throw new InvalidOperationException(SR.ExceptionChartSerializerBinaryFromatInvalid);
		}

		protected virtual int DeserializeObject(object objectToDeserialize, object parent, string elementName, BinaryReader reader)
		{
			int num = 0;
			if (objectToDeserialize == null)
			{
				return num;
			}
			Type[] array = null;
			int num2 = 0;
			if (objectToDeserialize is IList)
			{
				short num3 = 0;
				while ((num3 = base.ReadHashID(reader, true)) != 0)
				{
					string itemTypeName = string.Empty;
					PropertyInfo property = objectToDeserialize.GetType().GetProperty("Item");
					if (property != null)
					{
						Assembly assembly = property.PropertyType.Assembly;
						if (assembly != null)
						{
							if (array == null)
							{
								array = assembly.GetExportedTypes();
							}
							Type[] array2 = array;
							foreach (Type type in array2)
							{
								if (type.IsSubclassOf(property.PropertyType) && SerializerBase.GetStringHashCode(type.Name) == num3)
								{
									itemTypeName = type.Name;
								}
							}
						}
					}
					string text = null;
					bool flag = false;
					object listNewItem = base.GetListNewItem((IList)objectToDeserialize, itemTypeName, ref text, ref flag);
					long offset = reader.BaseStream.Seek(0L, SeekOrigin.Current);
					int num4 = this.DeserializeObject(listNewItem, objectToDeserialize, "", reader);
					if (num4 > 0 || flag)
					{
						bool flag2 = true;
						PropertyInfo property2 = listNewItem.GetType().GetProperty("Name");
						if (property2 != null)
						{
							object obj = null;
							try
							{
								text = (string)property2.GetValue(listNewItem, null);
								if (text != null && text.Length > 0)
								{
									bool flag3 = false;
									obj = base.GetListNewItem((IList)objectToDeserialize, itemTypeName, ref text, ref flag3);
									if (text == null)
									{
										obj = null;
									}
								}
							}
							catch (Exception)
							{
							}
							if (obj != null)
							{
								flag2 = false;
								reader.BaseStream.Seek(offset, SeekOrigin.Begin);
								num4 = this.DeserializeObject(obj, objectToDeserialize, "", reader);
								((IList)objectToDeserialize).Remove(obj);
								((IList)objectToDeserialize).Insert(num2++, obj);
							}
						}
						if (flag2)
						{
							((IList)objectToDeserialize).Insert(num2++, listNewItem);
						}
					}
					num += num4;
				}
				return num;
			}
			PropertyInfo[] properties = objectToDeserialize.GetType().GetProperties();
			if (properties == null)
			{
				return num;
			}
			PropertyInfo propertyInfo = null;
			while ((propertyInfo = this.ReadPropertyInfo(objectToDeserialize, parent, properties, reader)) != null)
			{
				if (base.ShouldSerializeAsAttribute(propertyInfo, objectToDeserialize))
				{
					if (this.SetPropertyValue(objectToDeserialize, propertyInfo, reader))
					{
						num++;
					}
				}
				else
				{
					PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(objectToDeserialize)[propertyInfo.Name];
					if (propertyDescriptor != null)
					{
						object value = propertyDescriptor.GetValue(objectToDeserialize);
						num += this.DeserializeObject(value, objectToDeserialize, propertyInfo.Name, reader);
					}
					else if (!base.IgnoreUnknownAttributes)
					{
						throw new InvalidOperationException(SR.ExceptionChartSerializerPropertyNameUnknown(propertyInfo.Name, objectToDeserialize.GetType().ToString()));
					}
				}
			}
			return num;
		}

		private bool SetPropertyValue(object obj, PropertyInfo pi, BinaryReader reader)
		{
			object obj2;
			if (pi != null)
			{
				obj2 = null;
				if (pi.PropertyType == typeof(bool))
				{
					obj2 = reader.ReadBoolean();
					goto IL_02de;
				}
				if (pi.PropertyType == typeof(double))
				{
					obj2 = reader.ReadDouble();
					goto IL_02de;
				}
				if (pi.PropertyType == typeof(string))
				{
					obj2 = reader.ReadString();
					goto IL_02de;
				}
				if (pi.PropertyType == typeof(int))
				{
					obj2 = reader.ReadInt32();
					goto IL_02de;
				}
				if (pi.PropertyType == typeof(long))
				{
					obj2 = reader.ReadInt64();
					goto IL_02de;
				}
				if (pi.PropertyType == typeof(float))
				{
					obj2 = reader.ReadSingle();
					goto IL_02de;
				}
				if (pi.PropertyType.IsEnum)
				{
					obj2 = Enum.Parse(pi.PropertyType, reader.ReadString());
					goto IL_02de;
				}
				if (pi.PropertyType == typeof(byte))
				{
					obj2 = reader.ReadByte();
					goto IL_02de;
				}
				if (pi.PropertyType == typeof(Font))
				{
					obj2 = SerializerBase.FontFromString(reader.ReadString());
					goto IL_02de;
				}
				if (pi.PropertyType == typeof(Color))
				{
					obj2 = Color.FromArgb(reader.ReadInt32());
					goto IL_02de;
				}
				if (pi.PropertyType == typeof(DateTime))
				{
					obj2 = new DateTime(reader.ReadInt64());
					goto IL_02de;
				}
				if (pi.PropertyType == typeof(Size))
				{
					obj2 = new Size(reader.ReadInt32(), reader.ReadInt32());
					goto IL_02de;
				}
				if (pi.PropertyType == typeof(Margins))
				{
					obj2 = new Margins(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
					goto IL_02de;
				}
				if (pi.PropertyType == typeof(double[]))
				{
					double[] array = new double[reader.ReadInt32()];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = reader.ReadDouble();
					}
					obj2 = array;
					goto IL_02de;
				}
				if (pi.PropertyType == typeof(Color[]))
				{
					Color[] array2 = new Color[reader.ReadInt32()];
					for (int j = 0; j < array2.Length; j++)
					{
						array2[j] = Color.FromArgb(reader.ReadInt32());
					}
					obj2 = array2;
					goto IL_02de;
				}
				if (pi.PropertyType == typeof(Image))
				{
					int num = reader.ReadInt32();
					MemoryStream memoryStream = new MemoryStream(num + 10);
					memoryStream.Write(reader.ReadBytes(num), 0, num);
					obj2 = new Bitmap(Image.FromStream(memoryStream));
					memoryStream.Close();
					goto IL_02de;
				}
				throw new InvalidOperationException(SR.ExceptionChartSerializerBinaryTypeUnsupported(obj.GetType().ToString()));
			}
			goto IL_02f8;
			IL_02de:
			if (base.IsSerializableContent(pi.Name, obj))
			{
				pi.SetValue(obj, obj2, null);
				return true;
			}
			goto IL_02f8;
			IL_02f8:
			return false;
		}

		private PropertyInfo ReadPropertyInfo(object objectToDeserialize, object parent, PropertyInfo[] properties, BinaryReader reader)
		{
			short num = base.ReadHashID(reader, false);
			if (num == 0)
			{
				return null;
			}
			int num2 = 0;
			PropertyInfo result;
			while (true)
			{
				if (num2 < properties.Length)
				{
					PropertyInfo propertyInfo = properties[num2];
					if (!base.IsChartBaseProperty(objectToDeserialize, parent, propertyInfo))
					{
						if (propertyInfo.CanRead && propertyInfo.PropertyType.GetInterface("ICollection", true) != null)
						{
							if (SerializerBase.GetStringHashCode(propertyInfo.Name) == num)
							{
								result = propertyInfo;
								break;
							}
						}
						else if (propertyInfo.CanRead && propertyInfo.CanWrite && !(propertyInfo.Name == "Item") && SerializerBase.GetStringHashCode(propertyInfo.Name) == num)
						{
							return propertyInfo;
						}
					}
					num2++;
					continue;
				}
				throw new InvalidOperationException(SR.ExceptionChartSerializerPropertyNotFound);
			}
			return result;
		}
	}
}
