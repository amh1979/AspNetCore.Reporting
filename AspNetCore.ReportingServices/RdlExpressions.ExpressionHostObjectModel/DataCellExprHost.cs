using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class DataCellExprHost : CellExprHost
	{
		[CLSCompliant(false)]
		protected IList<DataValueExprHost> m_dataValueHostsRemotable;

		internal IList<DataValueExprHost> DataValueHostsRemotable
		{
			get
			{
				return this.m_dataValueHostsRemotable;
			}
		}
	}
}
