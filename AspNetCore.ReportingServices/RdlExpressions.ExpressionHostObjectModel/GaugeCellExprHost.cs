using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class GaugeCellExprHost : DataCellExprHost
	{
		[CLSCompliant(false)]
		protected IList<GaugeInputValueExprHost> m_gaugeInputValueHostsRemotable;

		internal IList<GaugeInputValueExprHost> GaugeInputValueHostsRemotable
		{
			get
			{
				return this.m_gaugeInputValueHostsRemotable;
			}
		}
	}
}
