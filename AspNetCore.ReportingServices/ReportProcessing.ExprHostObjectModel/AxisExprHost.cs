using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class AxisExprHost : StyleExprHost
	{
		public ChartTitleExprHost TitleHost;

		public StyleExprHost MajorGridLinesHost;

		public StyleExprHost MinorGridLinesHost;

		protected DataValueExprHost[] CustomPropertyHosts;

		[CLSCompliant(false)]
		protected IList<DataValueExprHost> m_customPropertyHostsRemotable;

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

		public virtual object AxisMinExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object AxisMaxExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object AxisCrossAtExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object AxisMajorIntervalExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object AxisMinorIntervalExpr
		{
			get
			{
				return null;
			}
		}
	}
}
