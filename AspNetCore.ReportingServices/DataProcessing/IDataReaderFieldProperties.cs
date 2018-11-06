namespace AspNetCore.ReportingServices.DataProcessing
{
	internal interface IDataReaderFieldProperties
	{
		int GetPropertyCount(int fieldIndex);

		string GetPropertyName(int fieldIndex, int propertyIndex);

		object GetPropertyValue(int fieldIndex, int propertyIndex);
	}
}
