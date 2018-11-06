namespace AspNetCore.ReportingServices.DataProcessing
{
	internal class ScopeValueFieldName
	{
		private readonly string m_fieldName;

		private readonly object m_value;

		internal object ScopeValue
		{
			get
			{
				return this.m_value;
			}
		}

		internal string FieldName
		{
			get
			{
				return this.m_fieldName;
			}
		}

		internal ScopeValueFieldName(string fieldName, object value)
		{
			this.m_fieldName = fieldName;
			this.m_value = value;
		}
	}
}
