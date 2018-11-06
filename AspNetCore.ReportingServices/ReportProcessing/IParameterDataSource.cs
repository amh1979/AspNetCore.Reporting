namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal interface IParameterDataSource
	{
		int DataSourceIndex
		{
			get;
		}

		int DataSetIndex
		{
			get;
		}

		int ValueFieldIndex
		{
			get;
		}

		int LabelFieldIndex
		{
			get;
		}
	}
}
