using AspNetCore.ReportingServices.DataExtensions;
using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Library;
using AspNetCore.ReportingServices.ReportProcessing;
using System;

namespace AspNetCore.Reporting
{
	[Serializable]
	internal class LocalExecutionSession
	{
		private ControlSnapshot __ReportSnapshot;

		private ControlSnapshot __compiledReport;

		public ControlSnapshot Snapshot
		{
			get
			{
				return this.__ReportSnapshot;
			}
			set
			{
				this.__ReportSnapshot = value;
				this.ExecutionInfo.HasSnapshot = (value != null);
			}
		}

		public ControlSnapshot CompiledReport
		{
			get
			{
				return this.__compiledReport;
			}
			set
			{
				this.__compiledReport = value;
				this.ExecutionInfo.IsCompiled = (value != null);
			}
		}

		public EventInformation EventInfo
		{
			get;
			set;
		}

		public LocalExecutionInfo ExecutionInfo
		{
			get;
			private set;
		}

		public DatasourceCredentialsCollection Credentials
		{
			get;
			private set;
		}

		internal DataSourceInfoCollection CompiledDataSources
		{
			get;
			set;
		}

		public LocalExecutionSession()
		{
			this.ExecutionInfo = new LocalExecutionInfo();
			this.Credentials = new DatasourceCredentialsCollection();
		}

		public void ResetExecution()
		{
			this.Snapshot = null;
			this.EventInfo = null;
			this.CompiledDataSources = null;
			this.ExecutionInfo.Reset();
		}

		public void SaveProcessingResult(OnDemandProcessingResult result)
		{
			if (result != null)
			{
				result.Save();
				if (result.EventInfoChanged)
				{
					this.EventInfo = result.NewEventInfo;
				}
				this.ExecutionInfo.SaveProcessingResult(result);
			}
		}
	}
}
