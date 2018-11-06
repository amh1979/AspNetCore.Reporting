using System;
using System.Globalization;
using System.Reflection;

namespace AspNetCore.ReportingServices.RdlObjectModel
{
	[AttributeUsage(AttributeTargets.Property)]
	internal sealed class ValidValuesAttribute : Attribute
	{
		private object m_minimum;

		private object m_maximum;

		public object Minimum
		{
			get
			{
				return this.m_minimum;
			}
			set
			{
				this.m_minimum = value;
			}
		}

		public object Maximum
		{
			get
			{
				return this.m_maximum;
			}
			set
			{
				this.m_maximum = value;
			}
		}

		public ValidValuesAttribute(int minimum, int maximum)
		{
			this.m_minimum = minimum;
			this.m_maximum = maximum;
		}

		public ValidValuesAttribute(double minimum, double maximum)
		{
			this.m_minimum = minimum;
			this.m_maximum = maximum;
		}

		public ValidValuesAttribute(string minimumField, string maximumField)
		{
			this.m_minimum = typeof(Constants).InvokeMember(minimumField, BindingFlags.GetField, null, null, null, CultureInfo.InvariantCulture);
			this.m_maximum = typeof(Constants).InvokeMember(maximumField, BindingFlags.GetField, null, null, null, CultureInfo.InvariantCulture);
		}
	}
}
