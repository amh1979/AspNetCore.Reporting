namespace AspNetCore.Reporting.Chart.WebForms.Utilities
{
	internal class KeywordInfo
	{
		public string Name = string.Empty;

		public string Keyword = string.Empty;

		public string KeywordAliases = string.Empty;

		public string Description = string.Empty;

		public string AppliesToTypes = string.Empty;

		public string AppliesToProperties = string.Empty;

		public bool SupportsFormatting;

		public bool SupportsValueIndex;

		public KeywordInfo(string name, string keyword, string keywordAliases, string description, string appliesToTypes, string appliesToProperties, bool supportsFormatting, bool supportsValueIndex)
		{
			this.Name = name;
			this.Keyword = keyword;
			this.KeywordAliases = keywordAliases;
			this.Description = description;
			this.AppliesToTypes = appliesToTypes;
			this.AppliesToProperties = appliesToProperties;
			this.SupportsFormatting = supportsFormatting;
			this.SupportsValueIndex = supportsValueIndex;
		}

		public override string ToString()
		{
			return this.Name;
		}
	}
}
