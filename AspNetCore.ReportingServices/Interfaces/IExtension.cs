namespace AspNetCore.ReportingServices.Interfaces
{
	internal interface IExtension
	{
		string LocalizedName
		{
			get;
		}

		void SetConfiguration(string configuration);
	}
}
