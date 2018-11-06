using System.Collections;
using System.Threading;

namespace AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public sealed class VariablesImpl : Variables
	{
		private bool m_lockAdd;

		private Hashtable m_collection;

		public override Variable this[string key]
		{
			get
			{
				if (key != null && this.m_collection != null)
				{
					Variable variable = this.m_collection[key] as Variable;
					if (variable == null)
					{
						throw new ReportProcessingException_NonExistingVariableReference(key);
					}
					return variable;
				}
				throw new ReportProcessingException_NonExistingVariableReference(key);
			}
		}

		internal Hashtable Collection
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

		internal VariablesImpl(bool lockAdd)
		{
			this.m_lockAdd = lockAdd;
			this.m_collection = new Hashtable();
		}

		internal void Add(VariableImpl variable)
		{
			try
			{
				if (this.m_lockAdd)
				{
					Monitor.Enter(this.m_collection);
				}
				this.m_collection.Add(variable.Name, variable);
			}
			finally
			{
				if (this.m_lockAdd)
				{
					Monitor.Exit(this.m_collection);
				}
			}
		}

		internal void ResetAll()
		{
			foreach (VariableImpl value in this.m_collection.Values)
			{
				value.Reset();
			}
		}
	}
}
