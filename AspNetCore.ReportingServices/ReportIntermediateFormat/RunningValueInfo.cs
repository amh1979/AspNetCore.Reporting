using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class RunningValueInfo : DataAggregateInfo, IPersistable
	{
		private string m_scope;

		private int m_totalGroupingExpressionCount;

		private bool m_isScopedInEvaluationScope;

		[NonSerialized]
		private static readonly Declaration m_Declaration = RunningValueInfo.GetDeclaration();

		internal override bool MustCopyAggregateResult
		{
			get
			{
				return true;
			}
		}

		internal string Scope
		{
			get
			{
				return this.m_scope;
			}
			set
			{
				this.m_scope = value;
			}
		}

		internal int TotalGroupingExpressionCount
		{
			get
			{
				return this.m_totalGroupingExpressionCount;
			}
			set
			{
				this.m_totalGroupingExpressionCount = value;
			}
		}

		internal bool IsScopedInEvaluationScope
		{
			get
			{
				return this.m_isScopedInEvaluationScope;
			}
			set
			{
				this.m_isScopedInEvaluationScope = value;
			}
		}

		internal bool HasDirectFieldReferences
		{
			get
			{
				bool result = false;
				if (base.Expressions != null && base.Expressions.Length > 0)
				{
					for (int i = 0; i < base.Expressions.Length; i++)
					{
						ExpressionInfo expressionInfo = base.Expressions[i];
						if (expressionInfo.HasDirectFieldReferences)
						{
							return true;
						}
					}
				}
				return result;
			}
		}

		internal override bool IsRunningValue()
		{
			return true;
		}

		public override object PublishClone(AutomaticSubtotalContext context)
		{
			RunningValueInfo runningValueInfo = (RunningValueInfo)base.PublishClone(context);
			runningValueInfo.m_scope = context.GetNewScopeName(this.m_scope);
			return runningValueInfo;
		}

		internal DataAggregateInfo GetAsAggregate()
		{
			DataAggregateInfo dataAggregateInfo = null;
			if (base.AggregateType != AggregateTypes.Previous)
			{
				dataAggregateInfo = new DataAggregateInfo();
				dataAggregateInfo.AggregateType = base.AggregateType;
				dataAggregateInfo.Expressions = base.Expressions;
				dataAggregateInfo.SetScope(this.m_scope);
			}
			return dataAggregateInfo;
		}

		internal override string GetAsString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			switch (base.AggregateType)
			{
			case AggregateTypes.CountRows:
				stringBuilder.Append("RowNumber(");
				if (this.m_scope != null)
				{
					stringBuilder.Append("\"");
					stringBuilder.Append(this.m_scope);
					stringBuilder.Append("\"");
				}
				break;
			case AggregateTypes.Previous:
				stringBuilder.Append("Previous(");
				if (base.Expressions != null)
				{
					for (int j = 0; j < base.Expressions.Length; j++)
					{
						stringBuilder.Append(base.Expressions[j].OriginalText);
					}
					if (this.m_scope != null)
					{
						stringBuilder.Append(", \"");
						stringBuilder.Append(this.m_scope);
						stringBuilder.Append("\"");
					}
				}
				break;
			default:
				stringBuilder.Append("RunningValue(");
				if (base.Expressions != null)
				{
					for (int i = 0; i < base.Expressions.Length; i++)
					{
						stringBuilder.Append(base.Expressions[i].OriginalText);
					}
				}
				stringBuilder.Append(", ");
				stringBuilder.Append(base.AggregateType.ToString());
				if (this.m_scope != null)
				{
					stringBuilder.Append(", \"");
					stringBuilder.Append(this.m_scope);
					stringBuilder.Append("\"");
				}
				break;
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		internal void Initialize(InitializationContext context, string dataSetName, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			if (base.Expressions != null && base.Expressions.Length > 0)
			{
				for (int i = 0; i < base.Expressions.Length; i++)
				{
					ExpressionInfo expressionInfo = base.Expressions[i];
					if (base.AggregateType == AggregateTypes.Previous && this.m_scope != null && expressionInfo.Aggregates != null)
					{
						foreach (DataAggregateInfo aggregate in expressionInfo.Aggregates)
						{
							string childScope = default(string);
							if (aggregate.GetScope(out childScope) && !context.IsSameOrChildScope(this.m_scope, childScope))
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsInvalidScopeInInnerAggregateOfPreviousAggregate, Severity.Error, objectType, objectName, propertyName);
							}
						}
					}
					expressionInfo.AggregateInitialize(dataSetName, objectType, objectName, propertyName, context);
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Scope, Token.String));
			list.Add(new MemberInfo(MemberName.TotalGroupingExpressionCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.IsScopedInEvaluationScope, Token.Boolean));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RunningValueInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateInfo, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(RunningValueInfo.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Scope:
					writer.Write(this.m_scope);
					break;
				case MemberName.TotalGroupingExpressionCount:
					writer.Write(this.m_totalGroupingExpressionCount);
					break;
				case MemberName.IsScopedInEvaluationScope:
					writer.Write(this.m_isScopedInEvaluationScope);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(RunningValueInfo.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Scope:
					this.m_scope = reader.ReadString();
					break;
				case MemberName.TotalGroupingExpressionCount:
					this.m_totalGroupingExpressionCount = reader.ReadInt32();
					break;
				case MemberName.IsScopedInEvaluationScope:
					this.m_isScopedInEvaluationScope = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RunningValueInfo;
		}
	}
}
