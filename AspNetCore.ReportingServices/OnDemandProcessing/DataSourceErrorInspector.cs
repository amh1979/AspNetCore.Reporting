using AspNetCore.ReportingServices.DataProcessing;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using System;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class DataSourceErrorInspector
	{
		private readonly IDbConnection m_connection;

		internal DataSourceErrorInspector(IDbConnection connection)
		{
			this.m_connection = connection;
		}

		internal bool TryInterpretProviderErrorCode(Exception e, out ErrorCode errorCode)
		{
			IDbErrorInspectorFactory dbErrorInspectorFactory = this.m_connection as IDbErrorInspectorFactory;
			if (dbErrorInspectorFactory != null)
			{
				IDbErrorInspector dbErrorInspector = dbErrorInspectorFactory.CreateErrorInspector();
				if (dbErrorInspector.IsQueryMemoryLimitExceeded(e))
				{
					errorCode = ErrorCode.rsQueryMemoryLimitExceeded;
					return true;
				}
				if (dbErrorInspector.IsQueryTimeout(e))
				{
					errorCode = ErrorCode.rsQueryTimeoutExceeded;
					return true;
				}
			}
			errorCode = ErrorCode.rsSuccess;
			return false;
		}

		internal bool IsOnPremiseServiceException(Exception e)
		{
			IDbErrorInspectorFactory dbErrorInspectorFactory = this.m_connection as IDbErrorInspectorFactory;
			if (dbErrorInspectorFactory != null)
			{
				IDbErrorInspector dbErrorInspector = dbErrorInspectorFactory.CreateErrorInspector();
				return dbErrorInspector.IsOnPremisesServiceException(e);
			}
			return false;
		}
	}
}
