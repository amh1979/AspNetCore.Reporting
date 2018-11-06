using System;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[AttributeUsage(AttributeTargets.Property)]
	internal sealed class ValidateBound : Attribute
	{
		private double minimum;

		private double maximum;

		private bool required = true;

		internal double Minimum
		{
			get
			{
				return this.minimum;
			}
		}

		internal double Maximum
		{
			get
			{
				return this.maximum;
			}
		}

		internal bool Required
		{
			get
			{
				return this.required;
			}
		}

		internal ValidateBound(double minimum, double maximum)
		{
			this.minimum = minimum;
			this.maximum = maximum;
		}

		internal ValidateBound(double minimum, double maximum, bool required)
		{
			this.minimum = minimum;
			this.maximum = maximum;
			this.required = required;
		}
	}
}
