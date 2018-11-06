using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class DrillthroughInfo
	{
		private string m_reportName;

		private DrillthroughParameters m_reportParameters;

		internal string ReportName
		{
			get
			{
				return this.m_reportName;
			}
		}

		internal DrillthroughParameters ReportParameters
		{
			get
			{
				return this.m_reportParameters;
			}
		}

		internal DrillthroughInfo(string reportName, DrillthroughParameters parameters)
		{
			this.m_reportName = reportName;
			this.m_reportParameters = parameters;
		}
	}
}
