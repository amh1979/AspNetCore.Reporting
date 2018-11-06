using AspNetCore.ReportingServices.ReportProcessing;
using System;

namespace AspNetCore.Reporting
{
	[Serializable]
	internal class StoredDataSet
	{
		public byte[] Definition
		{
			get;
			private set;
		}

		public DataSetPublishingResult PublishingResult
		{
			get;
			private set;
		}

		public StoredDataSet(byte[] dataSetDefinition, DataSetPublishingResult result)
		{
			this.Definition = dataSetDefinition;
			this.PublishingResult = result;
		}
	}
}
