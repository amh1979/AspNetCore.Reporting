namespace AspNetCore.ReportingServices.Diagnostics.Internal
{
    internal sealed class ExternalImageCategory
	{
		public string Count
		{
			get;
			set;
		}

		public string ByteCount
		{
			get;
			set;
		}

		public string ResourceFetchTime
		{
			get;
			set;
		}

		internal ExternalImageCategory()
		{
		}
	}
}
