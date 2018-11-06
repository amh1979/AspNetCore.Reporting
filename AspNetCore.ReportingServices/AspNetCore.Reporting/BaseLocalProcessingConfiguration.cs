using AspNetCore.ReportingServices.Diagnostics;

namespace AspNetCore.Reporting
{
	internal abstract class BaseLocalProcessingConfiguration : IConfiguration
	{
		private bool m_showSubReportErrorDetails;

		public bool ShowSubreportErrorDetails
		{
			get
			{
				return this.m_showSubReportErrorDetails;
			}
			set
			{
				this.m_showSubReportErrorDetails = value;
			}
		}

		public IRdlSandboxConfig RdlSandboxing
		{
			get
			{
				return null;
			}
		}

		public abstract IMapTileServerConfiguration MapTileServerConfiguration
		{
			get;
		}

		public ProcessingUpgradeState UpgradeState
		{
			get
			{
				return ProcessingUpgradeState.CurrentVersion;
			}
		}

		public bool ProhibitSerializableValues
		{
			get
			{
				return false;
			}
		}
	}
}
