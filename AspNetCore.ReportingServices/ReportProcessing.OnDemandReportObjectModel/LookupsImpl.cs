using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public sealed class LookupsImpl : Lookups
	{
		private Dictionary<string, LookupImpl> m_collection;

		public override Lookup this[string key]
		{
			get
			{
				LookupImpl result = null;
				if (key != null && this.m_collection != null && this.m_collection.TryGetValue(key, out result))
				{
					return result;
				}
				throw new ReportProcessingException_NonExistingLookupReference();
			}
		}

		internal LookupsImpl()
		{
		}

		internal void Add(LookupImpl lookup)
		{
			if (this.m_collection == null)
			{
				this.m_collection = new Dictionary<string, LookupImpl>();
			}
			this.m_collection.Add(lookup.Name, lookup);
		}
	}
}
