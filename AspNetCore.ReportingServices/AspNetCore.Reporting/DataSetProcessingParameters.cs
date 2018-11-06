using AspNetCore.ReportingServices.DataProcessing;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.Reporting
{
	internal class DataSetProcessingParameters : IDataParameterCollection, IEnumerable
	{
		private List<IDataParameter> m_list;

		public DataSetProcessingParameters()
		{
			this.m_list = new List<IDataParameter>();
		}

		public int Add(IDataParameter parameter)
		{
			this.m_list.Add(parameter);
			return this.m_list.Count;
		}

		public IEnumerator GetEnumerator()
		{
			return (IEnumerator)(object)this.m_list.GetEnumerator();
		}
	}
}
