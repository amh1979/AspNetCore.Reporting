using System;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[AttributeUsage(AttributeTargets.All)]
	internal sealed class SerializationVisibilityAttribute : Attribute
	{
		private SerializationVisibility visibility = SerializationVisibility.Attribute;

		public SerializationVisibility Visibility
		{
			get
			{
				return this.visibility;
			}
			set
			{
				this.visibility = value;
			}
		}

		public SerializationVisibilityAttribute(SerializationVisibility visibility)
		{
			this.visibility = visibility;
		}
	}
}
