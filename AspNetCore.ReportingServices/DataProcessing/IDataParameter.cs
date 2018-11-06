namespace AspNetCore.ReportingServices.DataProcessing
{
	internal interface IDataParameter
	{
		string ParameterName
		{
			get;
			set;
		}

		object Value
		{
			get;
			set;
		}
	}
}
