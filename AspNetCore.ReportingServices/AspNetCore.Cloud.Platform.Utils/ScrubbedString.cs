namespace Microsoft.Cloud.Platform.Utils
{
	internal sealed class ScrubbedString : IContainsPrivateInformation
	{
		private readonly string m_plainString;

		public ScrubbedString(string plainString)
		{
			this.m_plainString = plainString;
		}

		public string ToPrivateString()
		{
			return this.m_plainString;
		}

		public string ToInternalString()
		{
			return this.m_plainString;
		}

		public string ToOriginalString()
		{
			return this.m_plainString;
		}
	}
}
