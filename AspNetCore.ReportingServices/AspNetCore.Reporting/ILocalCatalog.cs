using AspNetCore.ReportingServices.DataExtensions;

namespace AspNetCore.Reporting
{
	internal interface ILocalCatalog
	{
		byte[] GetReportDefinition(PreviewItemContext itemContext);

		byte[] GetResource(string resourcePath, out string mimeType);

		DataSourceInfo GetDataSource(string dataSourcePath);

		DataSetInfo GetDataSet(string dataSetPath);

		void SetReportDefinition(string reportName, byte[] reportBytes);

		bool HasDirectReportDefinition(string reportName);

		void GetReportDataSourceCredentials(PreviewItemContext itemContext, string dataSourceName, out string userName, out string password);

		string GetReportDataFileInfo(PreviewItemContext itemContext, out bool isOutOfDate);

		void UpdateReportDataFileStatus(PreviewItemContext itemContext, bool isOutOfDate);
	}
}
