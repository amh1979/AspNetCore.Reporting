using AspNetCore.ReportingServices.ReportProcessing;
using System;

namespace AspNetCore.Reporting
{
	[Serializable]
	internal class LocalExecutionInfo
	{
		public ParameterInfoCollection ReportParameters
		{
			get;
			set;
		}

		public PageProperties PageProperties
		{
			get;
			set;
		}

		public int AutoRefreshInterval
		{
			get;
			private set;
		}

		public int TotalPages
		{
			get;
			private set;
		}

		public PaginationMode PaginationMode
		{
			get;
			private set;
		}

		public bool HasDocMap
		{
			get;
			private set;
		}

		public bool HasSnapshot
		{
			get;
			set;
		}

		public bool IsCompiled
		{
			get;
			set;
		}

		public LocalExecutionInfo()
		{
			this.Reset();
		}

		public void Reset()
		{
			this.TotalPages = 0;
			this.HasDocMap = false;
			this.PaginationMode = PaginationMode.Progressive;
		}

		public void SaveProcessingResult(OnDemandProcessingResult result)
		{
			this.HasDocMap = result.HasDocumentMap;
			this.AutoRefreshInterval = result.AutoRefresh;
			if (result.NumberOfPages != 0)
			{
				this.TotalPages = result.NumberOfPages;
				this.PaginationMode = result.UpdatedPaginationMode;
			}
		}
	}
}
