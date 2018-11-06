using System;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ReportServerStorageException : ReportCatalogException
	{
		public bool IsSqlException
		{
			[DebuggerStepThrough]
			get
			{
				return base.InnerException is SqlException;
			}
		}

		public int SqlErrorNumber
		{
			get
			{
				if (this.IsSqlException)
				{
					return (base.InnerException as SqlException).Number;
				}
				return 0;
			}
		}

		public string SqlErrorMessage
		{
			get
			{
				if (this.IsSqlException)
				{
					return (base.InnerException as SqlException).Message;
				}
				return null;
			}
		}

		public SqlErrorCollection SqlErrors
		{
			get
			{
				if (this.IsSqlException)
				{
					return (base.InnerException as SqlException).Errors;
				}
				return null;
			}
		}

		protected override bool TraceFullException
		{
			get
			{
				return false;
			}
		}

		public ReportServerStorageException(Exception innerException)
			: this(innerException, (innerException != null) ? innerException.Message : null)
		{
		}

		public ReportServerStorageException(Exception innerException, string additionalTraceMessage)
			: base(ErrorCode.rsReportServerDatabaseError, (innerException != null && innerException is SqlTypeException) ? innerException.Message : ErrorStrings.rsReportServerDatabaseError, innerException, additionalTraceMessage)
		{
		}

		private ReportServerStorageException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
