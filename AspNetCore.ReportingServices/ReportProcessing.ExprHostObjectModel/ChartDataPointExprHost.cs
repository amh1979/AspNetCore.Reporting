using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class ChartDataPointExprHost : IndexedExprHost
	{
		public StyleExprHost DataLabelStyleHost;

		public ActionExprHost ActionHost;

		public StyleExprHost StyleHost;

		public StyleExprHost MarkerStyleHost;

		public ActionInfoExprHost ActionInfoHost;

		protected DataValueExprHost[] CustomPropertyHosts;

		[CLSCompliant(false)]
		protected IList<DataValueExprHost> m_customPropertyHostsRemotable;

		public virtual object DataLabelValueExpr
		{
			get
			{
				return null;
			}
		}

		internal IList<DataValueExprHost> CustomPropertyHostsRemotable
		{
			get
			{
				if (this.m_customPropertyHostsRemotable == null && this.CustomPropertyHosts != null)
				{
					this.m_customPropertyHostsRemotable = new RemoteArrayWrapper<DataValueExprHost>(this.CustomPropertyHosts);
				}
				return this.m_customPropertyHostsRemotable;
			}
		}
	}
}
