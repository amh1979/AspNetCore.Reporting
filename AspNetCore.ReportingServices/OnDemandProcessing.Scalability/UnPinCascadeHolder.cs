using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class UnPinCascadeHolder : IDisposable
	{
		private List<IDisposable> m_cleanupRefs;

		internal UnPinCascadeHolder()
		{
			this.m_cleanupRefs = new List<IDisposable>();
		}

		internal void AddCleanupRef(IDisposable cleanupRef)
		{
			this.m_cleanupRefs.Add(cleanupRef);
		}

		public void Dispose()
		{
			for (int i = 0; i < this.m_cleanupRefs.Count; i++)
			{
				this.m_cleanupRefs[i].Dispose();
			}
		}
	}
}
