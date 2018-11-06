using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AspNetCore.ReportingServices.RdlObjectModel
{
	internal class EnumProperty : PropertyDefinition, IPropertyDefinition
	{
		private object m_default;

		private IList<int> m_validIntValues;

		private IList<object> m_validValues;

		private Type m_type;

		public object Default
		{
			get
			{
				return this.m_default;
			}
		}

		public IList<object> ValidValues
		{
			get
			{
				if (this.m_validValues == null)
				{
					object[] array;
					if (this.m_validIntValues != null)
					{
						array = new object[this.m_validValues.Count];
						for (int i = 0; i < this.m_validIntValues.Count; i++)
						{
							array[i] = Enum.ToObject(this.m_type, this.m_validValues[i]);
						}
					}
					else
					{
						Array values = Enum.GetValues(this.m_type);
						array = new object[values.Length];
						values.CopyTo(array, 0);
					}
					this.m_validValues = new ReadOnlyCollection<object>(array);
				}
				return this.m_validValues;
			}
		}

		object IPropertyDefinition.Minimum
		{
			get
			{
				return null;
			}
		}

		object IPropertyDefinition.Maximum
		{
			get
			{
				return null;
			}
		}

		void IPropertyDefinition.Validate(object component, object value)
		{
			if (value is IExpression)
			{
				if (((IExpression)value).IsExpression)
				{
					return;
				}
				value = ((IExpression)value).Value;
			}
			if (value.GetType() == this.m_type)
			{
				this.Validate(component, (int)value);
				return;
			}
			throw new ArgumentException("Invalid type.");
		}

		public void Validate(object component, int value)
		{
			if (this.m_validIntValues == null)
			{
				return;
			}
			if (this.m_validIntValues.Contains(value))
			{
				return;
			}
			object value2 = Enum.ToObject(this.m_type, value);
			throw new ArgumentConstraintException(component, base.Name, value2, null, SRErrors.InvalidParam(base.Name, value2));
		}

		public EnumProperty(string name, Type enumType, object defaultValue, IList<int> validValues)
			: base(name)
		{
			this.m_type = enumType;
			this.m_default = defaultValue;
			this.m_validIntValues = validValues;
		}
	}
}
