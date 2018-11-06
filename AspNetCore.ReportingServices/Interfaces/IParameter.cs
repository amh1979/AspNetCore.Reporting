namespace AspNetCore.ReportingServices.Interfaces
{
	internal interface IParameter
	{
		string Name
		{
			get;
		}

		bool IsMultiValue
		{
			get;
		}

		object[] Values
		{
			get;
		}
	}
}
