using System;
using System.Reflection;

namespace AspNetCore.ReportingServices.RdlObjectModel.Serialization
{
	internal class PropertyMapping : MemberMapping
	{
		internal enum PropertyTypeCode
		{
			None,
			Object,
			ContainedObject,
			Boolean,
			Integer,
			Size,
			Enum,
			ValueType
		}

		private PropertyInfo m_property;

		private int m_index;

		private PropertyTypeCode m_typeCode;

		private IPropertyDefinition m_definition;

		public PropertyInfo Property
		{
			get
			{
				return this.m_property;
			}
		}

		public int Index
		{
			get
			{
				return this.m_index;
			}
			set
			{
				this.m_index = value;
			}
		}

		public PropertyTypeCode TypeCode
		{
			get
			{
				return this.m_typeCode;
			}
			set
			{
				this.m_typeCode = value;
			}
		}

		public IPropertyDefinition Definition
		{
			get
			{
				return this.m_definition;
			}
			set
			{
				this.m_definition = value;
			}
		}

		public PropertyMapping(Type propertyType, string name, string ns, PropertyInfo property)
			: base(propertyType, name, ns, !property.CanWrite)
		{
			this.m_property = property;
		}

		public override void SetValue(object obj, object value)
		{
			if (this.m_typeCode != 0)
			{
				IPropertyStore propertyStore = ((ReportObject)obj).PropertyStore;
				if (this.m_definition != null)
				{
					this.m_definition.Validate(obj, value);
				}
				switch (this.m_typeCode)
				{
				default:
					propertyStore.SetObject(this.m_index, value);
					break;
				case PropertyTypeCode.ContainedObject:
					propertyStore.SetObject(this.m_index, (IContainedObject)value);
					break;
				case PropertyTypeCode.Boolean:
					propertyStore.SetBoolean(this.m_index, (bool)value);
					break;
				case PropertyTypeCode.Integer:
				case PropertyTypeCode.Enum:
					propertyStore.SetInteger(this.m_index, (int)value);
					break;
				case PropertyTypeCode.Size:
					propertyStore.SetSize(this.m_index, (ReportSize)value);
					break;
				}
			}
			else
			{
				this.m_property.SetValue(obj, value, null);
			}
		}

		public override object GetValue(object obj)
		{
			if (this.m_typeCode != 0)
			{
				IPropertyStore propertyStore = ((ReportObject)obj).PropertyStore;
				switch (this.m_typeCode)
				{
				default:
					return propertyStore.GetObject(this.m_index);
				case PropertyTypeCode.Boolean:
					return propertyStore.GetBoolean(this.m_index);
				case PropertyTypeCode.Integer:
					return propertyStore.GetInteger(this.m_index);
				case PropertyTypeCode.Size:
					return propertyStore.GetSize(this.m_index);
				case PropertyTypeCode.Enum:
				{
					int integer = propertyStore.GetInteger(this.m_index);
					return Enum.ToObject(base.Type, integer);
				}
				case PropertyTypeCode.ValueType:
				{
					object obj2 = propertyStore.GetObject(this.m_index);
					if (obj2 == null)
					{
						obj2 = Activator.CreateInstance(base.Type);
					}
					return obj2;
				}
				}
			}
			return this.m_property.GetValue(obj, null);
		}

		public override bool HasValue(object obj)
		{
			if (this.m_typeCode != 0)
			{
				IPropertyStore propertyStore = ((ReportObject)obj).PropertyStore;
				switch (this.m_typeCode)
				{
				default:
					return propertyStore.ContainsObject(this.m_index);
				case PropertyTypeCode.Boolean:
					return propertyStore.ContainsBoolean(this.m_index);
				case PropertyTypeCode.Integer:
				case PropertyTypeCode.Enum:
					return propertyStore.ContainsInteger(this.m_index);
				case PropertyTypeCode.Size:
					return propertyStore.ContainsSize(this.m_index);
				}
			}
			return this.m_property.GetValue(obj, null) != null;
		}
	}
}
