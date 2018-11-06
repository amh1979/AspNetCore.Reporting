using System.Collections.Generic;
using System.Xml.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Internal
{
    internal sealed class DataShape
	{
		public string ID
		{
			get;
			set;
		}

		public long? TimeDataRetrieval
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool TimeDataRetrievalSpecified
		{
			get
			{
				return this.TimeDataRetrieval.HasValue;
			}
		}

		public long? TimeQueryTranslation
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool TimeQueryTranslationSpecified
		{
			get
			{
				return this.TimeQueryTranslation.HasValue;
			}
		}

		public long? TimeProcessing
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool TimeProcessingSpecified
		{
			get
			{
				return this.TimeProcessing.HasValue;
			}
		}

		public long? TimeRendering
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool TimeRenderingSpecified
		{
			get
			{
				return this.TimeRendering.HasValue;
			}
		}

		public ScaleTimeCategory ScalabilityTime
		{
			get;
			set;
		}

		public EstimatedMemoryUsageKBCategory EstimatedMemoryUsageKB
		{
			get;
			set;
		}

		public List<Connection> Connections
		{
			get;
			set;
		}

		public string QueryPattern
		{
			get;
			set;
		}

		public string QueryPatternReasons
		{
			get;
			set;
		}

		internal DataShape()
		{
		}
	}
}
