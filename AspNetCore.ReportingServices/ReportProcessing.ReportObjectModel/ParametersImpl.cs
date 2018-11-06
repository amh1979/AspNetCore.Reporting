using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel
{
	public sealed class ParametersImpl : Parameters
	{
		internal const string Name = "Parameters";

		internal const string FullName = "AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel.Parameters";

		private Hashtable m_nameMap;

		private ParameterImpl[] m_collection;

		private int m_count;

		public override Parameter this[string key]
		{
			get
			{
				if (key != null && this.m_nameMap != null && this.m_collection != null)
				{
					try
					{
						return this.m_collection[(int)this.m_nameMap[key]];
					}
					catch
					{
						throw new ReportProcessingException_NonExistingParameterReference(key);
					}
				}
				throw new ReportProcessingException_NonExistingParameterReference(key);
			}
		}

		internal ParametersImpl(int size)
		{
			this.m_collection = new ParameterImpl[size];
			this.m_nameMap = new Hashtable(size);
			this.m_count = 0;
		}

		internal void Add(string name, ParameterImpl parameter)
		{
			Global.Tracer.Assert(null != this.m_collection, "(null != m_collection)");
			Global.Tracer.Assert(null != this.m_nameMap, "(null != m_nameMap)");
			Global.Tracer.Assert(this.m_count < this.m_collection.Length, "(m_count < m_collection.Length)");
			this.m_nameMap.Add(name, this.m_count);
			this.m_collection[this.m_count] = parameter;
			this.m_count++;
		}
	}
}
