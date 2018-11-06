using AspNetCore.ReportingServices.ReportProcessing;
using System;

namespace AspNetCore.Reporting
{
	internal sealed class Warning
	{
		public string Code
		{
			get;
			private set;
		}

		public string Message
		{
			get;
			private set;
		}

		public string ObjectName
		{
			get;
			private set;
		}

		public string ObjectType
		{
			get;
			private set;
		}

		public Severity Severity
		{
			get;
			private set;
		}

		internal Warning(string code, string message, string objectName, string objectType, string severity)
		{
			this.Code = code;
			this.Message = message;
			this.ObjectName = objectName;
			this.ObjectType = objectType;
			if (string.Compare(severity, "Warning", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.Severity = Severity.Warning;
			}
			else
			{
				this.Severity = Severity.Error;
			}
		}


		internal static Warning[] FromProcessingMessageList(ProcessingMessageList processingWarnings)
		{
			if (processingWarnings == null)
			{
				return new Warning[0];
			}
			Warning[] array = new Warning[processingWarnings.Count];
			for (int i = 0; i < processingWarnings.Count; i++)
			{
				array[i] = new Warning(processingWarnings[i].Code.ToString(), processingWarnings[i].Message, processingWarnings[i].ObjectName, processingWarnings[i].ObjectType.ToString(), processingWarnings[i].Severity.ToString());
			}
			return array;
		}
	}
}
