using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class LinearJoinInfo : JoinInfo
	{
		[Reference]
		private DataSet m_parentDataSet;

		[NonSerialized]
		private static readonly Declaration m_Declaration = LinearJoinInfo.GetDeclaration();

		internal DataSet ParentDataSet
		{
			get
			{
				return this.m_parentDataSet;
			}
		}

		public LinearJoinInfo()
		{
		}

		public LinearJoinInfo(IdcRelationship relationship)
			: base(relationship)
		{
		}

		internal override bool ValidateRelationships(ScopeTree scopeTree, ErrorContext errorContext, DataSet ourDataSet, ParentDataSetContainer parentDataSets, IRIFReportDataScope currentScope)
		{
			Global.Tracer.Assert(parentDataSets != null && parentDataSets.Count == 1, "LinearJoinInfo can only be used with exactly one parent data set");
			this.m_parentDataSet = parentDataSets.ParentDataSet;
			if (DataSet.AreEqualById(ourDataSet, this.m_parentDataSet))
			{
				return false;
			}
			bool flag = false;
			if (base.m_relationships != null)
			{
				foreach (IdcRelationship relationship in base.m_relationships)
				{
					flag |= relationship.ValidateLinearRelationship(errorContext, this.m_parentDataSet);
				}
			}
			if (!flag && !ourDataSet.HasDefaultRelationship(this.m_parentDataSet))
			{
				this.RegisterInvalidInnerDataSetNameError(errorContext, ourDataSet, currentScope);
				return false;
			}
			Relationship activeRelationship = this.GetActiveRelationship(ourDataSet);
			if (activeRelationship == null)
			{
				this.RegisterInvalidInnerDataSetNameError(errorContext, ourDataSet, currentScope);
				return false;
			}
			if (activeRelationship.IsCrossJoin && (!activeRelationship.NaturalJoin || LinearJoinInfo.ScopeHasParentGroups(currentScope, scopeTree)))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidNaturalCrossJoin, Severity.Error, currentScope.DataScopeObjectType, currentScope.Name, "JoinConditions");
				return false;
			}
			return true;
		}

		private static bool ScopeHasParentGroups(IRIFReportDataScope currentScope, ScopeTree scopeTree)
		{
			ScopeTree.DirectedScopeTreeVisitor visitor = delegate(IRIFDataScope candidate)
			{
				if (candidate == currentScope)
				{
					return true;
				}
				if (candidate.DataScopeObjectType != AspNetCore.ReportingServices.ReportProcessing.ObjectType.DataShapeMember && candidate.DataScopeObjectType != AspNetCore.ReportingServices.ReportProcessing.ObjectType.Grouping)
				{
					return true;
				}
				return false;
			};
			return !scopeTree.Traverse(visitor, currentScope);
		}

		private void RegisterInvalidInnerDataSetNameError(ErrorContext errorContext, DataSet ourDataSet, IRIFReportDataScope currentScope)
		{
			Severity severity = Severity.Error;
			DataRegion dataRegion = currentScope as DataRegion;
			if (dataRegion != null)
			{
				severity = Severity.Warning;
			}
			errorContext.Register(ProcessingErrorCode.rsInvalidInnerDataSetName, severity, currentScope.DataScopeObjectType, currentScope.Name, "DataSetName", this.m_parentDataSet.Name.MarkAsPrivate(), ourDataSet.Name.MarkAsPrivate());
		}

		internal Relationship GetActiveRelationship(DataSet ourDataSet)
		{
			return base.GetActiveRelationship(ourDataSet, this.m_parentDataSet);
		}

		internal override void CheckContainerJoinForNaturalJoin(IRIFDataScope startScope, ErrorContext errorContext, IRIFDataScope scope)
		{
			base.CheckContainerRelationshipForNaturalJoin(startScope, errorContext, scope, this.GetActiveRelationship(scope.DataScopeInfo.DataSet));
		}

		internal override void ValidateScopeRulesForIdcNaturalJoin(InitializationContext context, IRIFDataScope scope)
		{
			Relationship activeRelationship = this.GetActiveRelationship(scope.DataScopeInfo.DataSet);
			base.ValidateScopeRulesForIdcNaturalJoin(context, scope, activeRelationship);
		}

		internal override void AddMappedFieldIndices(List<int> parentFieldIndices, DataSet parentDataSet, DataSet ourDataSet, List<int> ourFieldIndices)
		{
			Global.Tracer.Assert(DataSet.AreEqualById(this.m_parentDataSet, parentDataSet), "Invalid parent data set");
			Relationship activeRelationship = this.GetActiveRelationship(ourDataSet);
			JoinInfo.AddMappedFieldIndices(activeRelationship, parentFieldIndices, ourFieldIndices);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ParentDataSet, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSet, Token.Reference));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LinearJoinInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.JoinInfo, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(LinearJoinInfo.m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.ParentDataSet)
				{
					writer.WriteReference(this.m_parentDataSet);
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
			reader.RegisterDeclaration(LinearJoinInfo.m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.ParentDataSet)
				{
					this.m_parentDataSet = reader.ReadReference<DataSet>(this);
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(LinearJoinInfo.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					MemberName memberName = item.MemberName;
					if (memberName == MemberName.ParentDataSet)
					{
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						Global.Tracer.Assert(referenceableItems[item.RefID] is DataSet);
						this.m_parentDataSet = (DataSet)referenceableItems[item.RefID];
					}
					else
					{
						Global.Tracer.Assert(false);
					}
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LinearJoinInfo;
		}
	}
}
