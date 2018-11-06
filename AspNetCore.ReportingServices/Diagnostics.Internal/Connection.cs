using System.Collections.Generic;
using System.Xml.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Internal
{
    internal sealed class Connection
	{
		public long? ConnectionOpenTime
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool ConnectionOpenTimeSpecified
		{
			get
			{
				return this.ConnectionOpenTime.HasValue;
			}
		}

		public bool? ConnectionFromPool
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool ConnectionFromPoolSpecified
		{
			get
			{
				return this.ConnectionFromPool.HasValue;
			}
		}

		public ModelMetadata ModelMetadata
		{
			get;
			set;
		}

		public DataSource DataSource
		{
			get;
			set;
		}

		public List<DataSet> DataSets
		{
			get;
			set;
		}

		internal Connection()
		{
		}
	}
}
