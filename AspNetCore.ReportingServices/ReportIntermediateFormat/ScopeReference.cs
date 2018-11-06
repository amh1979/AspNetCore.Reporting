namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class ScopeReference
	{
		private readonly string m_scopeName;

		private readonly string m_fieldName;

		public string ScopeName
		{
			get
			{
				return this.m_scopeName;
			}
		}

		public string FieldName
		{
			get
			{
				return this.m_fieldName;
			}
		}

		public bool HasFieldName
		{
			get
			{
				return this.m_fieldName != null;
			}
		}

		public ScopeReference(string scopeName)
			: this(scopeName, null)
		{
		}

		public ScopeReference(string scopeName, string fieldName)
		{
			this.m_scopeName = scopeName;
			this.m_fieldName = fieldName;
		}
	}
}
