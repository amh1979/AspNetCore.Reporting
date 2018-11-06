using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartDerivedSeriesExprHost : StyleExprHost
	{
		public ChartSeriesExprHost ChartSeriesHost;

		[CLSCompliant(false)]
		protected IList<ChartFormulaParameterExprHost> m_formulaParametersHostsRemotable;

		internal IList<ChartFormulaParameterExprHost> ChartFormulaParametersHostsRemotable
		{
			get
			{
				return this.m_formulaParametersHostsRemotable;
			}
		}
	}
}
