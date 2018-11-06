namespace AspNetCore.ReportingServices.Diagnostics
{
	internal interface IRdlSandboxTypeInfo
	{
		string Namespace
		{
			get;
		}

		bool AllowNew
		{
			get;
		}

		string Name
		{
			get;
		}
	}
}
