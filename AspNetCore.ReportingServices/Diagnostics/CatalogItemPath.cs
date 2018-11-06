using System.Diagnostics;

namespace AspNetCore.ReportingServices.Diagnostics
{
	[DebuggerDisplay("CatalogItemPath: {m_originalValue}")]
	internal class CatalogItemPath : ItemPathBase
	{
		public static readonly CatalogItemPath Empty = new CatalogItemPath(string.Empty);

		private readonly string m_originalValue;

		public string OriginalValue
		{
			get
			{
				return this.m_originalValue;
			}
		}

		public CatalogItemPath(string value)
			: base(ItemPathBase.GetLocalPath(value))
		{
			this.m_originalValue = value;
		}

		public CatalogItemPath(string value, string editSessionID)
			: this(value)
		{
			base.m_editSessionID = editSessionID;
		}
	}
}
