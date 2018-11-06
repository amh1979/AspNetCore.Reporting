using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class SubreportExprHost : ReportItemExprHost
	{
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
				return this.m_parameterHostsRemotable;
			}
		}
	}
}
