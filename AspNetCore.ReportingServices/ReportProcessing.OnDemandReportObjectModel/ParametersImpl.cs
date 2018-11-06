using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public sealed class ParametersImpl : Parameters
	{
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
					catch (Exception e)
					{
						if (AsynchronousExceptionDetection.IsStoppingException(e))
						{
							throw;
						}
						throw new ReportProcessingException_NonExistingParameterReference(key);
					}
				}
				throw new ReportProcessingException_NonExistingParameterReference(key);
			}
		}

		internal ParameterImpl[] Collection
		{
			get
			{
				return this.m_collection;
			}
			set
			{
				this.m_collection = value;
			}
		}

		internal Hashtable NameMap
		{
			get
			{
				return this.m_nameMap;
			}
			set
			{
				this.m_nameMap = value;
			}
		}

		internal int Count
		{
			get
			{
				return this.m_count;
			}
			set
			{
				this.m_count = value;
			}
		}

		internal ParametersImpl()
		{
		}

		internal ParametersImpl(int size)
		{
			this.m_collection = new ParameterImpl[size];
			this.m_nameMap = new Hashtable(size);
			this.m_count = 0;
		}

		internal ParametersImpl(ParametersImpl copy)
		{
			this.m_count = copy.m_count;
			if (copy.m_collection != null)
			{
				this.m_collection = new ParameterImpl[this.m_count];
				Array.Copy(copy.m_collection, this.m_collection, this.m_count);
			}
			if (copy.m_nameMap != null)
			{
				this.m_nameMap = (Hashtable)copy.m_nameMap.Clone();
			}
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

		internal void Clear()
		{
			if (this.m_nameMap != null)
			{
				this.m_nameMap.Clear();
			}
			if (this.m_collection != null)
			{
				this.m_collection = new ParameterImpl[this.m_collection.Length];
			}
			this.m_count = 0;
		}
	}
}
