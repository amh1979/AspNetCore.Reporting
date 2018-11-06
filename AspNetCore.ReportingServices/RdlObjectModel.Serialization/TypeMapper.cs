using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Xml.Serialization;

namespace AspNetCore.ReportingServices.RdlObjectModel.Serialization
{
	internal static class TypeMapper
	{
		private static Dictionary<Type, TypeMapping> m_mappings = new Dictionary<Type, TypeMapping>();

		private static ReaderWriterLock m_lock = new ReaderWriterLock();

		public static TypeMapping GetTypeMapping(Type type)
		{
			TypeMapper.m_lock.AcquireReaderLock(-1);
			try
			{
				if (TypeMapper.m_mappings.ContainsKey(type))
				{
					return TypeMapper.m_mappings[type];
				}
			}
			finally
			{
				TypeMapper.m_lock.ReleaseReaderLock();
			}
			TypeMapping typeMapping = (!typeof(IXmlSerializable).IsAssignableFrom(type)) ? ((!TypeMapper.IsPrimitiveType(type)) ? ((!type.IsArray && !typeof(IEnumerable).IsAssignableFrom(type)) ? ((TypeMapping)TypeMapper.ImportStructMapping(type)) : ((TypeMapping)TypeMapper.ImportArrayMapping(type))) : TypeMapper.ImportPrimitiveMapping(type)) : TypeMapper.ImportSpecialMapping(type);
			TypeMapper.m_lock.AcquireWriterLock(-1);
			try
			{
				TypeMapper.m_mappings[type] = typeMapping;
				return typeMapping;
			}
			finally
			{
				TypeMapper.m_lock.ReleaseWriterLock();
			}
		}

		public static bool IsPrimitiveType(Type type)
		{
			if (!type.IsEnum && !type.IsPrimitive && type != typeof(string) && type != typeof(Guid))
			{
				return type == typeof(DateTime);
			}
			return true;
		}

		private static SpecialMapping ImportSpecialMapping(Type type)
		{
			SpecialMapping specialMapping = new SpecialMapping(type);
			IList customAttributes = type.GetCustomAttributes(typeof(XmlElementClassAttribute), true);
			foreach (XmlElementAttribute item in customAttributes)
			{
				if (item.Type != null && type != item.Type)
				{
					continue;
				}
				if (item.Namespace != null)
				{
					specialMapping.Namespace = item.Namespace;
				}
				if (item.ElementName == null)
				{
					return specialMapping;
				}
				specialMapping.Name = item.ElementName;
				return specialMapping;
			}
			return specialMapping;
		}

		private static PrimitiveMapping ImportPrimitiveMapping(Type type)
		{
			return new PrimitiveMapping(type);
		}

		private static ArrayMapping ImportArrayMapping(Type type)
		{
			ArrayMapping arrayMapping = new ArrayMapping(type);
			arrayMapping.ElementTypes = new Dictionary<string, Type>();
			if (type.IsArray)
			{
				Type type2 = arrayMapping.ItemType = type.GetElementType();
				arrayMapping.ElementTypes.Add(type2.Name, type2);
			}
			else
			{
				TypeMapper.GetCollectionElementTypes(type, arrayMapping);
			}
			return arrayMapping;
		}

		private static void GetCollectionElementTypes(Type type, ArrayMapping mapping)
		{
			PropertyInfo propertyInfo = null;
			for (Type type2 = type; type2 != null; type2 = type2.BaseType)
			{
				MemberInfo[] defaultMembers = type.GetDefaultMembers();
				if (defaultMembers != null)
				{
					for (int i = 0; i < defaultMembers.Length; i++)
					{
						if (defaultMembers[i] is PropertyInfo)
						{
							PropertyInfo propertyInfo2 = (PropertyInfo)defaultMembers[i];
							if (propertyInfo2.CanRead)
							{
								MethodInfo getMethod = propertyInfo2.GetGetMethod();
								ParameterInfo[] parameters = getMethod.GetParameters();
								if (parameters.Length == 1 && (propertyInfo == null || propertyInfo.PropertyType == typeof(object)))
								{
									propertyInfo = propertyInfo2;
								}
							}
						}
					}
				}
				if (propertyInfo != null)
				{
					break;
				}
			}
			if (propertyInfo == null)
			{
				throw new Exception("NoDefaultAccessors");
			}
			MethodInfo method = type.GetMethod("Add", new Type[1]
			{
				propertyInfo.PropertyType
			});
			if (method == null)
			{
				throw new Exception("NoAddMethod");
			}
			mapping.ItemType = propertyInfo.PropertyType;
			IList customAttributes = propertyInfo.PropertyType.GetCustomAttributes(typeof(XmlElementClassAttribute), true);
			if (customAttributes != null && customAttributes.Count > 0)
			{
				foreach (XmlElementClassAttribute item in customAttributes)
				{
					mapping.ElementTypes.Add((item.ElementName != string.Empty) ? item.ElementName : item.Type.Name, item.Type);
				}
			}
			else
			{
				string name = propertyInfo.PropertyType.Name;
				mapping.ElementTypes.Add(name, propertyInfo.PropertyType);
			}
		}

		private static void GetMemberName(XmlAttributes attributes, ref string tagName, ref string ns)
		{
			if (attributes.XmlElements.Count > 0)
			{
				if (attributes.XmlElements[0].ElementName != null && attributes.XmlElements[0].ElementName != string.Empty)
				{
					tagName = attributes.XmlElements[0].ElementName;
				}
				if (attributes.XmlElements[0].Namespace != null && attributes.XmlElements[0].Namespace != string.Empty)
				{
					ns = attributes.XmlElements[0].Namespace;
				}
			}
		}

		private static void ImportTypeMembers(StructMapping mapping, Type type)
		{
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			PropertyInfo[] array = properties;
			foreach (PropertyInfo prop in array)
			{
				TypeMapper.ImportPropertyInfo(mapping, prop);
			}
		}

		private static void ImportPropertyInfo(StructMapping mapping, PropertyInfo prop)
		{
			Type type = prop.PropertyType;
			bool flag = false;
			if (type.IsGenericType)
			{
				Type genericTypeDefinition = type.GetGenericTypeDefinition();
				if (genericTypeDefinition == typeof(Nullable<>))
				{
					flag = true;
					type = type.GetGenericArguments()[0];
				}
			}
			bool flag2 = false;
			XmlAttributes xmlAttributes = new XmlAttributes();
			object[] customAttributes = type.GetCustomAttributes(true);
			object[] customAttributes2 = prop.GetCustomAttributes(true);
			bool flag3 = false;
			int num = customAttributes.Length;
			Array.Resize(ref customAttributes, num + customAttributes2.Length);
			customAttributes2.CopyTo(customAttributes, num);
			object[] array = customAttributes;
			foreach (object obj in array)
			{
				Type type2 = obj.GetType();
				if (type2 == typeof(XmlIgnoreAttribute))
				{
					return;
				}
				if (typeof(DefaultValueAttribute).IsAssignableFrom(type2))
				{
					xmlAttributes.XmlDefaultValue = ((DefaultValueAttribute)obj).Value;
				}
				else if (typeof(XmlElementAttribute).IsAssignableFrom(type2))
				{
					XmlElementAttribute xmlElementAttribute = (XmlElementAttribute)obj;
					xmlAttributes.XmlElements.Add(xmlElementAttribute);
					if (xmlElementAttribute.Type != null)
					{
						if (string.IsNullOrEmpty(xmlElementAttribute.ElementName))
						{
							type = xmlElementAttribute.Type;
						}
						else
						{
							flag2 = true;
						}
					}
				}
				else if (type2 == typeof(XmlArrayItemAttribute))
				{
					XmlArrayItemAttribute xmlArrayItemAttribute = (XmlArrayItemAttribute)obj;
					int j;
					for (j = 0; j < xmlAttributes.XmlArrayItems.Count && xmlAttributes.XmlArrayItems[j].NestingLevel <= xmlArrayItemAttribute.NestingLevel; j++)
					{
					}
					xmlAttributes.XmlArrayItems.Insert(j, xmlArrayItemAttribute);
				}
				else if (typeof(XmlAttributeAttribute).IsAssignableFrom(type2))
				{
					xmlAttributes.XmlAttribute = (XmlAttributeAttribute)obj;
				}
				else if (type2 == typeof(ValidValuesAttribute) || type2 == typeof(ValidEnumValuesAttribute))
				{
					flag3 = true;
				}
			}
			string name = prop.Name;
			string empty = string.Empty;
			if (!flag2)
			{
				TypeMapper.GetMemberName(xmlAttributes, ref name, ref empty);
			}
			if (mapping.GetElement(name, empty) == null && mapping.GetAttribute(name, empty) == null)
			{
				PropertyMapping propertyMapping = new PropertyMapping(type, name, empty, prop);
				propertyMapping.XmlAttributes = xmlAttributes;
				mapping.Members.Add(propertyMapping);
				if (xmlAttributes.XmlAttribute != null)
				{
					if (xmlAttributes.XmlAttribute is XmlChildAttributeAttribute)
					{
						mapping.AddChildAttribute(propertyMapping);
					}
					else
					{
						mapping.Attributes[name, empty] = propertyMapping;
					}
				}
				else
				{
					mapping.Elements[name, empty] = propertyMapping;
					if (flag2)
					{
						mapping.AddUseTypeInfo(name, empty);
					}
				}
				Type declaringType = prop.DeclaringType;
				if (declaringType.IsSubclassOf(typeof(ReportObject)))
				{
					Type type3 = declaringType.Assembly.GetType(declaringType.FullName + "+Definition+Properties", false);
					FieldInfo field;
					if (type3 != null && type3.IsEnum && (field = type3.GetField(prop.Name)) != null)
					{
						propertyMapping.Index = (int)field.GetRawConstantValue();
						propertyMapping.TypeCode = PropertyMapping.PropertyTypeCode.Object;
						if (flag)
						{
							propertyMapping.TypeCode = PropertyMapping.PropertyTypeCode.Object;
						}
						else if (type.IsSubclassOf(typeof(IContainedObject)))
						{
							propertyMapping.TypeCode = PropertyMapping.PropertyTypeCode.ContainedObject;
						}
						else if (type == typeof(bool))
						{
							propertyMapping.TypeCode = PropertyMapping.PropertyTypeCode.Boolean;
						}
						else if (type == typeof(int))
						{
							propertyMapping.TypeCode = PropertyMapping.PropertyTypeCode.Integer;
						}
						else if (type == typeof(ReportSize))
						{
							propertyMapping.TypeCode = PropertyMapping.PropertyTypeCode.Size;
						}
						else if (type.IsEnum)
						{
							propertyMapping.TypeCode = PropertyMapping.PropertyTypeCode.Enum;
						}
						else if (type.IsValueType)
						{
							propertyMapping.TypeCode = PropertyMapping.PropertyTypeCode.ValueType;
						}
						if (flag3)
						{
							type3 = declaringType.Assembly.GetType(declaringType.FullName + "+Definition", false);
							propertyMapping.Definition = (IPropertyDefinition)type3.InvokeMember("GetProperty", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod, null, null, new object[1]
							{
								propertyMapping.Index
							}, CultureInfo.InvariantCulture);
						}
					}
				}
			}
		}

		private static StructMapping ImportStructMapping(Type type)
		{
			StructMapping structMapping = new StructMapping(type);
			IList customAttributes = type.GetCustomAttributes(typeof(XmlElementClassAttribute), true);
			foreach (XmlElementAttribute item in customAttributes)
			{
				if (item.Type != null && type != item.Type)
				{
					continue;
				}
				if (item.Namespace != null)
				{
					structMapping.Namespace = item.Namespace;
				}
				if (item.ElementName == null)
				{
					break;
				}
				structMapping.Name = item.ElementName;
				break;
			}
			TypeMapper.ImportTypeMembers(structMapping, type);
			structMapping.ResolveChildAttributes();
			return structMapping;
		}
	}
}
