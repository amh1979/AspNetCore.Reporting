using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class IdcRelationship : Relationship
	{
		private string m_parentScope;

		[NonSerialized]
		private static readonly Declaration m_Declaration = IdcRelationship.GetDeclaration();

		internal string ParentScope
		{
			get
			{
				return this.m_parentScope;
			}
			set
			{
				this.m_parentScope = value;
			}
		}

		internal void Initialize(InitializationContext context)
		{
			if (base.m_joinConditions != null)
			{
				base.JoinConditionInitialize(base.m_relatedDataSet, context);
			}
		}

		internal bool ValidateLinearRelationship(ErrorContext errorContext, DataSet parentDataSet)
		{
			base.m_relatedDataSet = parentDataSet;
			this.m_parentScope = null;
			return true;
		}

		internal bool ValidateIntersectRelationship(ErrorContext errorContext, IRIFDataScope intersectScope, ScopeTree scopeTree)
		{
			IRIFDataScope parentRowScopeForIntersection = scopeTree.GetParentRowScopeForIntersection(intersectScope);
			IRIFDataScope parentColumnScopeForIntersection = scopeTree.GetParentColumnScopeForIntersection(intersectScope);
			if (ScopeTree.SameScope(this.m_parentScope, parentRowScopeForIntersection.Name))
			{
				base.m_relatedDataSet = parentRowScopeForIntersection.DataScopeInfo.DataSet;
				return true;
			}
			if (ScopeTree.SameScope(this.m_parentScope, parentColumnScopeForIntersection.Name))
			{
				base.m_relatedDataSet = parentColumnScopeForIntersection.DataScopeInfo.DataSet;
				return true;
			}
			IRIFDataScope parentDataRegion = scopeTree.GetParentDataRegion(intersectScope);
			errorContext.Register(ProcessingErrorCode.rsMissingIntersectionRelationshipParentScope, Severity.Error, intersectScope.DataScopeObjectType, parentDataRegion.Name, "ParentScope", parentDataRegion.DataScopeObjectType.ToString(), parentRowScopeForIntersection.Name, parentColumnScopeForIntersection.Name);
			return false;
		}

		internal void InsertAggregateIndicatorJoinCondition(Field field, int fieldIndex, Field aggregateIndicatorField, int aggregateIndicatorFieldIndex, InitializationContext context)
		{
			int num = -1;
			if (base.m_joinConditions != null)
			{
				for (int i = 0; i < base.m_joinConditions.Count; i++)
				{
					JoinCondition joinCondition = base.m_joinConditions[i];
					if (joinCondition.ForeignKeyExpression.Type == ExpressionInfo.Types.Field)
					{
						if (joinCondition.ForeignKeyExpression.FieldIndex == fieldIndex)
						{
							num = i;
						}
						else if (joinCondition.ForeignKeyExpression.FieldIndex == aggregateIndicatorFieldIndex)
						{
							return;
						}
					}
				}
			}
			bool flag = num == -1;
			string text = this.FindRelatedAggregateIndicatorFieldName(field);
			if (text != null)
			{
				ExpressionInfo primaryKey = ExpressionInfo.CreateConstExpression(flag);
				ExpressionInfo expressionInfo = new ExpressionInfo();
				expressionInfo.SetAsSimpleFieldReference(text);
				JoinCondition joinCondition2 = new JoinCondition(expressionInfo, primaryKey, SortDirection.Ascending);
				joinCondition2.Initialize(base.m_relatedDataSet, base.m_naturalJoin, context);
				if (flag)
				{
					base.AddJoinCondition(joinCondition2);
				}
				else
				{
					base.m_joinConditions.Insert(num, joinCondition2);
				}
			}
		}

		private string FindRelatedAggregateIndicatorFieldName(Field field)
		{
			Field field2 = (from f in base.m_relatedDataSet.Fields
			where f.Name == field.Name
			select f).FirstOrDefault();
			if (field2 == null)
			{
				return null;
			}
			if (!field2.HasAggregateIndicatorField)
			{
				return null;
			}
			return base.m_relatedDataSet.Fields[field2.AggregateIndicatorFieldIndex].Name;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ParentScope, Token.String));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IdcRelationship, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Relationship, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(IdcRelationship.m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.ParentScope)
				{
					writer.Write(this.m_parentScope);
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(IdcRelationship.m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.ParentScope)
				{
					this.m_parentScope = reader.ReadString();
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IdcRelationship;
		}
	}
}
