using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class DataSetCollection : ReportElementCollectionBase<DataSet>
	{
		private DataSet[] m_collection;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Report m_reportDef;

		private RenderingContext m_rendringContext;

		public override int Count
		{
			get
			{
				return this.m_reportDef.DataSetCount;
			}
		}

		public override DataSet this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					if (this.m_collection == null)
					{
						this.m_collection = new DataSet[this.Count];
					}
					if (this.m_collection[index] == null)
					{
						this.m_collection[index] = new DataSet(this.m_reportDef.MappingDataSetIndexToDataSet[index], this.m_rendringContext);
					}
					return this.m_collection[index];
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public DataSet this[string name]
		{
			get
			{
				if (this.m_reportDef.MappingNameToDataSet.ContainsKey(name))
				{
					return ((ReportElementCollectionBase<DataSet>)this)[this.m_reportDef.MappingDataSetIndexToDataSet.IndexOf(this.m_reportDef.MappingNameToDataSet[name])];
				}
				return null;
			}
		}

		internal DataSetCollection(AspNetCore.ReportingServices.ReportIntermediateFormat.Report reportDef, RenderingContext renderingContext)
		{
			this.m_reportDef = reportDef;
			this.m_rendringContext = renderingContext;
		}

		internal void SetNewContext()
		{
			if (this.m_collection != null)
			{
				for (int i = 0; i < this.m_collection.Length; i++)
				{
					DataSet dataSet = this.m_collection[i];
					if (dataSet != null)
					{
						dataSet.SetNewContext();
					}
				}
			}
		}
	}
}
