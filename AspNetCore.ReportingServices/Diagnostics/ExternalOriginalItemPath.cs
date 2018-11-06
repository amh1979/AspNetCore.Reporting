namespace AspNetCore.ReportingServices.Diagnostics
{
	internal class ExternalOriginalItemPath : ExternalItemPath
	{
		public override string FullEditSessionIdentifier
		{
			get
			{
				return base.Value;
			}
		}

		public ExternalOriginalItemPath(string value, string editSessionID)
			: base(value, editSessionID)
		{
		}

		private ExternalOriginalItemPath(string value, string editSessionID, bool runChecks)
			: base(value, editSessionID, false)
		{
		}

		public new static ExternalOriginalItemPath CreateWithoutChecks(string value, string editSessionID)
		{
			return new ExternalOriginalItemPath(value, editSessionID, false);
		}
	}
}
