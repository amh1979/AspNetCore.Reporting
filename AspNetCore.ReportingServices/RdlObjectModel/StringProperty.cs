namespace AspNetCore.ReportingServices.RdlObjectModel
{
	internal class StringProperty : PropertyDefinition, IPropertyDefinition
	{
		private string m_default;

		public object Default
		{
			get
			{
				return this.m_default;
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
		}

		public StringProperty(string name, string defaultValue)
			: base(name)
		{
			this.m_default = defaultValue;
		}
	}
}
