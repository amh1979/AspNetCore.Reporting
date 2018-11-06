using AspNetCore.ReportingServices.Library;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Security.Permissions;

namespace AspNetCore.Reporting
{
	[Serializable]
	[PermissionSet(SecurityAction.Demand, Unrestricted = true)]
	internal class StoredReport : IDisposable
	{
		public ControlSnapshot Snapshot
		{
			get;
			private set;
		}

		public PublishingResult PublishingResult
		{
			get;
			private set;
		}

		public bool GeneratedExpressionHostWithRefusedPermissions
		{
			get;
			private set;
		}

		public StoredReport(PublishingResult publishingResult, ControlSnapshot snapshot, bool generatedExpressionHostWithRefusedPermissions)
		{
			this.PublishingResult = publishingResult;
			this.Snapshot = snapshot;
			this.GeneratedExpressionHostWithRefusedPermissions = generatedExpressionHostWithRefusedPermissions;
		}

		public void Dispose()
		{
			if (this.Snapshot != null)
			{
				this.Snapshot.Dispose();
				this.Snapshot = null;
			}
			GC.SuppressFinalize(this);
		}
	}
}
