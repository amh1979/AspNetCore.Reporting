using System;
using System.Globalization;

namespace AspNetCore.ReportingServices.RdlObjectModel
{
	internal class ComparablePropertyDefinition<T> : PropertyDefinition<T>, IPropertyDefinition where T : struct, IComparable
	{
		private T? m_minimum;

		private T? m_maximum;

		public T? Minimum
		{
			get
			{
				return this.m_minimum;
			}
		}

		public T? Maximum
		{
			get
			{
				return this.m_maximum;
			}
		}

		object IPropertyDefinition.Default
		{
			get
			{
				return base.Default;
			}
		}

		object IPropertyDefinition.Minimum
		{
			get
			{
				return this.Minimum;
			}
		}

		object IPropertyDefinition.Maximum
		{
			get
			{
				return this.Maximum;
			}
		}

		void IPropertyDefinition.Validate(object component, object value)
		{
			if (value is T)
			{
				this.Validate(component, (T)value);
				return;
			}
			if (value is ReportExpression<T>)
			{
				this.Validate(component, (ReportExpression<T>)value);
				return;
			}
			if (value is string)
			{
				this.Validate(component, (string)value);
				return;
			}
			throw new ArgumentException("Invalid type.");
		}

		public ComparablePropertyDefinition(string name, T? defaultValue)
			: base(name, defaultValue)
		{
		}

		public ComparablePropertyDefinition(string name, T? defaultValue, T? minimum, T? maximum)
			: this(name, defaultValue)
		{
			this.m_minimum = minimum;
			this.m_maximum = maximum;
		}

		public void Constrain(ref T value)
		{
			if (this.Minimum.HasValue && this.Minimum.Value.CompareTo(value) > 0)
			{
				value = this.Minimum.Value;
			}
			else if (this.Maximum.HasValue && this.Maximum.Value.CompareTo(value) < 0)
			{
				value = this.Maximum.Value;
			}
		}

		public void Validate(object component, T value)
		{
			if (this.Minimum.HasValue && this.Minimum.Value.CompareTo(value) > 0)
			{
				throw new ArgumentTooSmallException(component, base.Name, value, this.Minimum);
			}
			if (!this.Maximum.HasValue)
			{
				return;
			}
			if (this.Maximum.Value.CompareTo(value) >= 0)
			{
				return;
			}
			throw new ArgumentTooLargeException(component, base.Name, value, this.Maximum);
		}

		public void Validate(object component, ReportExpression<T> value)
		{
			if (!value.IsExpression)
			{
				this.Validate(component, value.Value);
			}
		}

		public void Validate(object component, string value)
		{
			this.Validate(component, new ReportExpression<T>(value, CultureInfo.InvariantCulture));
		}
	}
}
