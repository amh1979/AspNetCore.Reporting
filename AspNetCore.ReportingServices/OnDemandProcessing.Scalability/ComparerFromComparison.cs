using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal class ComparerFromComparison<T> : IComparer
	{
		private Comparison<T> m_comparison;

		internal ComparerFromComparison(Comparison<T> comparison)
		{
			this.m_comparison = comparison;
		}

		public int Compare(object x, object y)
		{
			if (!(x is T) || !(y is T))
			{
				Global.Tracer.Assert(false, "Cannot compare other types than the comparison's types");
			}
			return this.m_comparison((T)x, (T)y);
		}
	}
}
