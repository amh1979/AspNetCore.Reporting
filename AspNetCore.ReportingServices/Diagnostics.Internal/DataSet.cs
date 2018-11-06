using System.Collections.Generic;
using System.Xml.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Internal
{
    internal sealed class DataSet
	{
		public string Name
		{
			get;
			set;
		}

		public string CommandText
		{
			get;
			set;
		}

		public List<QueryParameter> QueryParameters
		{
			get;
			set;
		}

		public long? RowsRead
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool RowsReadSpecified
		{
			get
			{
				return this.RowsRead.HasValue;
			}
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

		public long? QueryPrepareAndExecutionTime
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool QueryPrepareAndExecutionTimeSpecified
		{
			get
			{
				return this.QueryPrepareAndExecutionTime.HasValue;
			}
		}

		public long? ExecuteReaderTime
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool ExecuteReaderTimeSpecified
		{
			get
			{
				return this.ExecuteReaderTime.HasValue;
			}
		}

		public long? DataReaderMappingTime
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool DataReaderMappingTimeSpecified
		{
			get
			{
				return this.DataReaderMappingTime.HasValue;
			}
		}

		public long? DisposeDataReaderTime
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool DisposeDataReaderTimeSpecified
		{
			get
			{
				return this.DisposeDataReaderTime.HasValue;
			}
		}

		public string CancelCommandTime
		{
			get;
			set;
		}

		internal DataSet()
		{
		}
	}
}
