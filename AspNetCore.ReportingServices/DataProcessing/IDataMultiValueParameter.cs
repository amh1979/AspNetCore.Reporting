namespace AspNetCore.ReportingServices.DataProcessing
{
	internal interface IDataMultiValueParameter : IDataParameter
	{
		object[] Values
		{
			get;
			set;
		}
	}
}
