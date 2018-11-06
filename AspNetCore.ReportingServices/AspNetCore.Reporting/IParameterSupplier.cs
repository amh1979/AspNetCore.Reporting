using System.Collections.Generic;

namespace AspNetCore.Reporting
{
	internal interface IParameterSupplier
	{
		bool IsReadyForConnection
		{
			get;
		}

		bool IsQueryExecutionAllowed
		{
			get;
		}

		ReportParameterInfoCollection GetParameters();

		ParametersPaneLayout GetParametersPaneLayout();

		void SetParameters(IEnumerable<ReportParameter> parameters);

		ReportDataSourceInfoCollection GetDataSources(out bool allCredentialsSatisfied);

		void SetDataSourceCredentials(DataSourceCredentialsCollection credentials);
	}
}
