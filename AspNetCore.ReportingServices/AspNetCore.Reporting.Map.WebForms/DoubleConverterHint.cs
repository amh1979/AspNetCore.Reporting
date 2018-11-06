using System;

namespace AspNetCore.Reporting.Map.WebForms
{
	[AttributeUsage(AttributeTargets.Property)]
	internal class DoubleConverterHint : Attribute
	{
		private double bound;

		public virtual double Bound
		{
			get
			{
				return this.bound;
			}
		}

		public DoubleConverterHint(double bound)
		{
			this.bound = bound;
		}
	}
}
