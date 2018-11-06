using System;

namespace AspNetCore.Reporting.Chart.WebForms.Utilities
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
		}

		public SerializationVisibilityAttribute(SerializationVisibility visibility)
		{
			this.visibility = visibility;
		}
	}
}
