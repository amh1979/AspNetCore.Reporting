using System.Collections;
using System.Threading;

namespace AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel
{
	public sealed class DataSetsImpl : DataSets
	{
		internal const string Name = "DataSets";

		internal const string FullName = "AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel.DataSets";

		private bool m_lockAdd;

		private Hashtable m_collection;

		public override DataSet this[string key]
		{
			get
			{
				if (key != null && this.m_collection != null)
				{
					try
					{
						DataSet dataSet = this.m_collection[key] as DataSet;
						if (dataSet == null)
						{
							throw new ReportProcessingException_NonExistingDataSetReference(key);
						}
						return dataSet;
					}
					catch
					{
						throw new ReportProcessingException_NonExistingDataSetReference(key);
					}
				}
				throw new ReportProcessingException_NonExistingDataSetReference(key);
			}
		}

		internal DataSetsImpl()
		{
		}

		internal DataSetsImpl(bool lockAdd, int size)
		{
			this.m_lockAdd = lockAdd;
			this.m_collection = new Hashtable(size);
		}

		internal void Add(AspNetCore.ReportingServices.ReportProcessing.DataSet dataSetDef)
		{
			try
			{
				if (this.m_lockAdd)
				{
					Monitor.Enter(this.m_collection);
				}
				this.m_collection.Add(dataSetDef.Name, new DataSetImpl(dataSetDef));
			}
			finally
			{
				if (this.m_lockAdd)
				{
					Monitor.Exit(this.m_collection);
				}
			}
		}
	}
}
