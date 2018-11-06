using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public interface IMemberNode
	{
		GroupExprHost GroupHost
		{
			get;
		}

		SortExprHost SortHost
		{
			get;
		}

		IList<DataValueExprHost> CustomPropertyHostsRemotable
		{
			get;
		}

		IList<IMemberNode> MemberTreeHostsRemotable
		{
			get;
		}

		IList<JoinConditionExprHost> JoinConditionExprHostsRemotable
		{
			get;
		}
	}
}
