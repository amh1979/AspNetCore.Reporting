using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal class DataAggregateInfo
	{
		internal enum AggregateTypes
		{
			First,
			Last,
			Sum,
			Avg,
			Max,
			Min,
			CountDistinct,
			CountRows,
			Count,
			StDev,
			Var,
			StDevP,
			VarP,
			Aggregate,
			Previous
		}

		private string m_name;

		private AggregateTypes m_aggregateType;

		private ExpressionInfo[] m_expressions;

		private StringList m_duplicateNames;

		[NonSerialized]
		private string m_scope;

		[NonSerialized]
		private bool m_hasScope;

		[NonSerialized]
		private bool m_recursive;

		[NonSerialized]
		private bool m_isCopied;

		[NonSerialized]
		private AggregateParamExprHost[] m_expressionHosts;

		[NonSerialized]
		private bool m_exprHostInitialized;

		[NonSerialized]
		private ObjectModelImpl m_exprHostReportObjectModel;

		[NonSerialized]
		private bool m_suppressExceptions;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		internal string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				this.m_name = value;
			}
		}

		internal AggregateTypes AggregateType
		{
			get
			{
				return this.m_aggregateType;
			}
			set
			{
				this.m_aggregateType = value;
			}
		}

		internal ExpressionInfo[] Expressions
		{
			get
			{
				return this.m_expressions;
			}
			set
			{
				this.m_expressions = value;
			}
		}

		internal StringList DuplicateNames
		{
			get
			{
				return this.m_duplicateNames;
			}
			set
			{
				this.m_duplicateNames = value;
			}
		}

		internal string ExpressionText
		{
			get
			{
				if (this.m_expressions != null && 1 == this.m_expressions.Length)
				{
					return this.m_expressions[0].OriginalText;
				}
				return string.Empty;
			}
		}

		internal AggregateParamExprHost[] ExpressionHosts
		{
			get
			{
				return this.m_expressionHosts;
			}
		}

		internal bool ExprHostInitialized
		{
			get
			{
				return this.m_exprHostInitialized;
			}
			set
			{
				this.m_exprHostInitialized = value;
			}
		}

		internal bool Recursive
		{
			get
			{
				return this.m_recursive;
			}
			set
			{
				this.m_recursive = value;
			}
		}

		internal bool IsCopied
		{
			get
			{
				return this.m_isCopied;
			}
			set
			{
				this.m_isCopied = value;
			}
		}

		internal bool SuppressExceptions
		{
			get
			{
				return this.m_suppressExceptions;
			}
		}

		internal List<string> FieldsUsedInValueExpression
		{
			get
			{
				return this.m_fieldsUsedInValueExpression;
			}
			set
			{
				this.m_fieldsUsedInValueExpression = value;
			}
		}

		internal DataAggregateInfo DeepClone(InitializationContext context)
		{
			DataAggregateInfo dataAggregateInfo = new DataAggregateInfo();
			this.DeepCloneInternal(dataAggregateInfo, context);
			return dataAggregateInfo;
		}

		protected void DeepCloneInternal(DataAggregateInfo clone, InitializationContext context)
		{
			clone.m_name = context.GenerateAggregateID(this.m_name);
			clone.m_aggregateType = this.m_aggregateType;
			if (this.m_expressions != null)
			{
				int num = this.m_expressions.Length;
				clone.m_expressions = new ExpressionInfo[num];
				for (int i = 0; i < num; i++)
				{
					clone.m_expressions[i] = this.m_expressions[i].DeepClone(context);
				}
			}
			Global.Tracer.Assert(null == this.m_duplicateNames);
			clone.m_recursive = this.m_recursive;
			clone.m_isCopied = false;
			clone.m_suppressExceptions = true;
			if (this.m_hasScope)
			{
				clone.SetScope(context.EscalateScope(this.m_scope));
			}
		}

		internal void SetScope(string scope)
		{
			this.m_hasScope = true;
			this.m_scope = scope;
		}

		internal bool GetScope(out string scope)
		{
			scope = this.m_scope;
			return this.m_hasScope;
		}

		internal void SetExprHosts(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
			if (!this.m_exprHostInitialized)
			{
				for (int i = 0; i < this.m_expressions.Length; i++)
				{
					ExpressionInfo expressionInfo = this.m_expressions[i];
					if (expressionInfo.ExprHostID >= 0)
					{
						if (this.m_expressionHosts == null)
						{
							this.m_expressionHosts = new AggregateParamExprHost[this.m_expressions.Length];
						}
						AggregateParamExprHost aggregateParamExprHost = reportExprHost.AggregateParamHostsRemotable[expressionInfo.ExprHostID];
						aggregateParamExprHost.SetReportObjectModel(reportObjectModel);
						this.m_expressionHosts[i] = aggregateParamExprHost;
					}
				}
				this.m_exprHostInitialized = true;
				this.m_exprHostReportObjectModel = reportObjectModel;
			}
			else if (this.m_exprHostReportObjectModel != reportObjectModel && this.m_expressionHosts != null)
			{
				for (int j = 0; j < this.m_expressionHosts.Length; j++)
				{
					if (this.m_expressionHosts[j] != null)
					{
						this.m_expressionHosts[j].SetReportObjectModel(reportObjectModel);
					}
				}
				this.m_exprHostReportObjectModel = reportObjectModel;
			}
		}

		internal bool IsPostSortAggregate()
		{
			if (this.m_aggregateType != 0 && AggregateTypes.Last != this.m_aggregateType && AggregateTypes.Previous != this.m_aggregateType)
			{
				return false;
			}
			return true;
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Name, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.AggregateType, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.Expressions, Token.Array, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.DuplicateNames, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.StringList));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
