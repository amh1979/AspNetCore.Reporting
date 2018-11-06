using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.RdlObjectModel
{
	[Serializable]
	internal class ArgumentConstraintException : ArgumentException
	{
		private object m_component;

		private string m_property;

		private object m_value;

		private object m_constraint;

		public object Component
		{
			get
			{
				return this.m_component;
			}
		}

		public string Property
		{
			get
			{
				return this.m_property;
			}
		}

		public object Value
		{
			get
			{
				return this.m_value;
			}
		}

		public object Constraint
		{
			get
			{
				return this.m_constraint;
			}
		}

		public ArgumentConstraintException(object component, string property, object value, object constraint, string message)
			: base(message, property)
		{
			this.m_component = component;
			this.m_property = property;
			this.m_value = value;
			this.m_constraint = constraint;
		}

		protected ArgumentConstraintException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
