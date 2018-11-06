using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class SubreportExprHost : ReportItemExprHost
	{
		protected ParamExprHost[] ParameterHosts;

		[CLSCompliant(false)]
		protected IList<ParamExprHost> m_parameterHostsRemotable;

		public virtual object NoRowsExpr
		{
			get
			{
				return null;
			}
		}

		internal IList<ParamExprHost> ParameterHostsRemotable
		{
			get
			{
				if (this.m_parameterHostsRemotable == null && this.ParameterHosts != null)
				{
					this.m_parameterHostsRemotable = new RemoteArrayWrapper<ParamExprHost>(this.ParameterHosts);
				}
				return this.m_parameterHostsRemotable;
			}
		}
	}
}
