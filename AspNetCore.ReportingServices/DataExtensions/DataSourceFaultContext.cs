using AspNetCore.ReportingServices.Diagnostics.Utilities;

namespace AspNetCore.ReportingServices.DataExtensions
{
	internal sealed class DataSourceFaultContext
	{
		public readonly ErrorCode m_errorCode;

		public readonly string m_errorString;

		public ErrorCode ErrorCode
		{
			get
			{
				return this.m_errorCode;
			}
		}

		public string ErrorString
		{
			get
			{
				return this.m_errorString;
			}
		}

		internal DataSourceFaultContext(ErrorCode errorCode, string errorString)
		{
			this.m_errorCode = errorCode;
			this.m_errorString = errorString;
		}
	}
}
