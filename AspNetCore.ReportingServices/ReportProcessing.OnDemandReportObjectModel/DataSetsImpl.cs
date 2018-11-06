using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections;
using System.Threading;

namespace AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public sealed class DataSetsImpl : DataSets
	{
		private bool m_lockAdd;

		private Hashtable m_collection;

		public override AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel.DataSet this[string key]
		{
			get
			{
				if (key != null && this.m_collection != null)
				{
					AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel.DataSet dataSet = this.m_collection[key] as AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel.DataSet;
					if (dataSet == null)
					{
						throw new ReportProcessingException_NonExistingDataSetReference(key.MarkAsPrivate());
					}
					return dataSet;
				}
				throw new ReportProcessingException_NonExistingDataSetReference(key.MarkAsPrivate());
			}
		}

		internal DataSetsImpl(int size)
		{
			this.m_lockAdd = (size > 1);
			this.m_collection = new Hashtable(size);
		}

		internal void AddOrUpdate(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSetDef, DataSetInstance dataSetInstance, DateTime reportExecutionTime)
		{
			try
			{
				if (this.m_lockAdd)
				{
					Monitor.Enter(this.m_collection);
				}
				DataSetImpl dataSetImpl = this.m_collection[dataSetDef.Name] as DataSetImpl;
				if (dataSetImpl == null)
				{
					this.m_collection.Add(dataSetDef.Name, new DataSetImpl(dataSetDef, dataSetInstance, reportExecutionTime));
				}
				else
				{
					dataSetImpl.Update(dataSetInstance, reportExecutionTime);
				}
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
