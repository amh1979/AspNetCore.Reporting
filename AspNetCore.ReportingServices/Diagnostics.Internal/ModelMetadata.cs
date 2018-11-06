using System.Xml.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Internal
{
    internal sealed class ModelMetadata
	{
		public string VersionRequested
		{
			get;
			set;
		}

		public string PerspectiveName
		{
			get;
			set;
		}

		public long? TotalTimeDataRetrieval
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool TotalTimeDataRetrievalSpecified
		{
			get
			{
				return this.TotalTimeDataRetrieval.HasValue;
			}
		}

		public long? ByteCount
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool ByteCountSpecified
		{
			get
			{
				return this.ByteCount.HasValue;
			}
		}

		public long? TimeDataModelParsing
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool TimeDataModelParsingSpecified
		{
			get
			{
				return this.TimeDataModelParsing.HasValue;
			}
		}

		internal ModelMetadata()
		{
		}
	}
}
