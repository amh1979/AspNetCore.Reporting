using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class IntersectJoinInfo : JoinInfo
	{
		[Reference]
		private DataSet m_rowParentDataSet;

		[Reference]
		private DataSet m_columnParentDataSet;

		[NonSerialized]
		private static readonly Declaration m_Declaration = IntersectJoinInfo.GetDeclaration();

		internal DataSet RowParentDataSet
		{
			get
			{
				return this.m_rowParentDataSet;
			}
		}

		internal DataSet ColumnParentDataSet
		{
			get
			{
				return this.m_columnParentDataSet;
			}
		}

		public IntersectJoinInfo()
		{
		}

		public IntersectJoinInfo(List<IdcRelationship> relationships)
			: base(relationships)
		{
		}

		internal Relationship GetActiveRowRelationship(DataSet ourDataSet)
		{
			return base.GetActiveRelationship(ourDataSet, this.m_rowParentDataSet);
		}

		internal Relationship GetActiveColumnRelationship(DataSet ourDataSet)
		{
			return base.GetActiveRelationship(ourDataSet, this.m_columnParentDataSet);
		}

		internal override bool ValidateRelationships(ScopeTree scopeTree, ErrorContext errorContext, DataSet ourDataSet, ParentDataSetContainer parentDataSets, IRIFReportDataScope currentScope)
		{
			Global.Tracer.Assert(parentDataSets != null, "IntersectJoinInfo can only be used with one or two parent data sets");
			if (parentDataSets.Count == 1)
			{
				DataRegion parentDataRegion = scopeTree.GetParentDataRegion(currentScope);
				errorContext.Register(ProcessingErrorCode.rsUnexpectedCellDataSetName, Severity.Error, currentScope.DataScopeObjectType, parentDataRegion.Name, "DataSetName", parentDataRegion.ObjectType.ToString());
				return false;
			}
			if (parentDataSets.AreAllSameDataSet() && DataSet.AreEqualById(parentDataSets.RowParentDataSet, ourDataSet))
			{
				return false;
			}
			this.m_rowParentDataSet = parentDataSets.RowParentDataSet;
			this.m_columnParentDataSet = parentDataSets.ColumnParentDataSet;
			if (this.m_rowParentDataSet != null && this.m_columnParentDataSet != null)
			{
				bool flag = false;
				bool flag2 = false;
				if (base.m_relationships != null)
				{
					foreach (IdcRelationship relationship in base.m_relationships)
					{
						if (!relationship.ValidateIntersectRelationship(errorContext, currentScope, scopeTree))
						{
							return false;
						}
						this.CheckRelationshipDataSetBinding(scopeTree, errorContext, currentScope, relationship, this.m_rowParentDataSet, ref flag);
						this.CheckRelationshipDataSetBinding(scopeTree, errorContext, currentScope, relationship, this.m_columnParentDataSet, ref flag2);
					}
				}
				flag = this.HasRelationshipOrDefaultForDataSet(scopeTree, errorContext, currentScope, ourDataSet, this.m_rowParentDataSet, flag);
				flag2 = this.HasRelationshipOrDefaultForDataSet(scopeTree, errorContext, currentScope, ourDataSet, this.m_columnParentDataSet, flag2);
				if (flag && flag2)
				{
					DataRegion parentDataRegion2 = scopeTree.GetParentDataRegion(currentScope);
					if (this.ValidateCellBoundTotheSameDataSetAsParentScpoe(this.m_columnParentDataSet, this.m_rowParentDataSet, ourDataSet, parentDataRegion2.IsColumnGroupingSwitched))
					{
						IRIFDataScope parentDataRegion3 = scopeTree.GetParentDataRegion(currentScope);
						IRIFDataScope parentRowScopeForIntersection = scopeTree.GetParentRowScopeForIntersection(currentScope);
						IRIFDataScope parentColumnScopeForIntersection = scopeTree.GetParentColumnScopeForIntersection(currentScope);
						if (parentDataRegion2.IsColumnGroupingSwitched)
						{
							errorContext.Register(ProcessingErrorCode.rsInvalidIntersectionNaturalJoin, Severity.Error, currentScope.DataScopeObjectType, parentDataRegion3.Name, "ParentScope", parentDataRegion3.DataScopeObjectType.ToString(), parentColumnScopeForIntersection.Name, parentRowScopeForIntersection.Name);
							return false;
						}
						errorContext.Register(ProcessingErrorCode.rsInvalidIntersectionNaturalJoin, Severity.Error, currentScope.DataScopeObjectType, parentDataRegion3.Name, "ParentScope", parentDataRegion3.DataScopeObjectType.ToString(), parentRowScopeForIntersection.Name, parentColumnScopeForIntersection.Name);
						return false;
					}
					return true;
				}
				return false;
			}
			return false;
		}

		private bool ValidateCellBoundTotheSameDataSetAsParentScpoe(DataSet columnParentDataSet, DataSet rowParentDataSet, DataSet ourDataSet, bool isColumnGroupingSwitched)
		{
			if (isColumnGroupingSwitched)
			{
				if (DataSet.AreEqualById(this.m_rowParentDataSet, ourDataSet))
				{
					return this.GetActiveColumnRelationship(ourDataSet).NaturalJoin;
				}
				return false;
			}
			if (DataSet.AreEqualById(this.m_columnParentDataSet, ourDataSet))
			{
				return this.GetActiveRowRelationship(ourDataSet).NaturalJoin;
			}
			return false;
		}

		private bool HasRelationshipOrDefaultForDataSet(ScopeTree scopeTree, ErrorContext errorContext, IRIFReportDataScope currentScope, DataSet ourDataSet, DataSet parentDataSet, bool hasValidRelationship)
		{
			if (DataSet.AreEqualById(parentDataSet, ourDataSet))
			{
				return true;
			}
			if (!hasValidRelationship && !ourDataSet.HasDefaultRelationship(parentDataSet))
			{
				IntersectJoinInfo.RegisterInvalidCellDataSetNameError(scopeTree, errorContext, currentScope, ourDataSet, parentDataSet);
				return false;
			}
			Relationship activeRelationship = base.GetActiveRelationship(ourDataSet, parentDataSet);
			if (activeRelationship == null)
			{
				IntersectJoinInfo.RegisterInvalidCellDataSetNameError(scopeTree, errorContext, currentScope, ourDataSet, parentDataSet);
				return false;
			}
			if (activeRelationship.IsCrossJoin)
			{
				DataRegion parentDataRegion = scopeTree.GetParentDataRegion(currentScope);
				errorContext.Register(ProcessingErrorCode.rsInvalidIntersectionNaturalCrossJoin, Severity.Error, currentScope.DataScopeObjectType, parentDataRegion.Name, "JoinConditions", parentDataRegion.ObjectType.ToString());
				return false;
			}
			return true;
		}

		private static void RegisterInvalidCellDataSetNameError(ScopeTree scopeTree, ErrorContext errorContext, IRIFReportDataScope currentScope, DataSet ourDataSet, DataSet parentDataSet)
		{
			DataRegion parentDataRegion = scopeTree.GetParentDataRegion(currentScope);
			errorContext.Register(ProcessingErrorCode.rsInvalidCellDataSetName, Severity.Error, currentScope.DataScopeObjectType, parentDataRegion.Name, "DataSetName", parentDataSet.Name.MarkAsPrivate(), ourDataSet.Name.MarkAsPrivate(), parentDataRegion.ObjectType.ToString());
		}

		private void CheckRelationshipDataSetBinding(ScopeTree scopeTree, ErrorContext errorContext, IRIFReportDataScope currentScope, IdcRelationship relationship, DataSet parentDataSetCandidate, ref bool dataSetAlreadyHasRelationship)
		{
			if (DataSet.AreEqualById(relationship.RelatedDataSet, parentDataSetCandidate))
			{
				if (dataSetAlreadyHasRelationship)
				{
					IRIFDataScope parentDataRegion = scopeTree.GetParentDataRegion(currentScope);
					IRIFDataScope parentRowScopeForIntersection = scopeTree.GetParentRowScopeForIntersection(currentScope);
					IRIFDataScope parentColumnScopeForIntersection = scopeTree.GetParentColumnScopeForIntersection(currentScope);
					errorContext.Register(ProcessingErrorCode.rsInvalidRelationshipDuplicateParentScope, Severity.Error, currentScope.DataScopeObjectType, parentDataRegion.Name, "ParentScope", parentDataRegion.DataScopeObjectType.ToString(), parentRowScopeForIntersection.Name, parentColumnScopeForIntersection.Name);
				}
				dataSetAlreadyHasRelationship = true;
			}
		}

		internal override void CheckContainerJoinForNaturalJoin(IRIFDataScope startScope, ErrorContext errorContext, IRIFDataScope scope)
		{
			DataSet dataSet = scope.DataScopeInfo.DataSet;
			base.CheckContainerRelationshipForNaturalJoin(startScope, errorContext, scope, this.GetActiveRowRelationship(dataSet));
			base.CheckContainerRelationshipForNaturalJoin(startScope, errorContext, scope, this.GetActiveColumnRelationship(dataSet));
		}

		internal override void ValidateScopeRulesForIdcNaturalJoin(InitializationContext context, IRIFDataScope scope)
		{
			DataSet dataSet = scope.DataScopeInfo.DataSet;
			base.ValidateScopeRulesForIdcNaturalJoin(context, context.ScopeTree.GetParentRowScopeForIntersection(scope), this.GetActiveRowRelationship(dataSet));
			base.ValidateScopeRulesForIdcNaturalJoin(context, context.ScopeTree.GetParentColumnScopeForIntersection(scope), this.GetActiveColumnRelationship(dataSet));
		}

		internal override void AddMappedFieldIndices(List<int> parentFieldIndices, DataSet parentDataSet, DataSet ourDataSet, List<int> ourFieldIndices)
		{
			Relationship relationship;
			if (DataSet.AreEqualById(this.m_rowParentDataSet, parentDataSet))
			{
				relationship = this.GetActiveRowRelationship(ourDataSet);
			}
			else if (DataSet.AreEqualById(this.m_columnParentDataSet, parentDataSet))
			{
				relationship = this.GetActiveColumnRelationship(ourDataSet);
			}
			else
			{
				Global.Tracer.Assert(false, "Invalid parent data set");
				relationship = null;
			}
			JoinInfo.AddMappedFieldIndices(relationship, parentFieldIndices, ourFieldIndices);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.RowParentDataSet, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSet, Token.Reference));
			list.Add(new MemberInfo(MemberName.ColumnParentDataSet, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSet, Token.Reference));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IntersectJoinInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.JoinInfo, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(IntersectJoinInfo.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.RowParentDataSet:
					writer.WriteReference(this.m_rowParentDataSet);
					break;
				case MemberName.ColumnParentDataSet:
					writer.WriteReference(this.m_columnParentDataSet);
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
			reader.RegisterDeclaration(IntersectJoinInfo.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.RowParentDataSet:
					this.m_rowParentDataSet = reader.ReadReference<DataSet>(this);
					break;
				case MemberName.ColumnParentDataSet:
					this.m_columnParentDataSet = reader.ReadReference<DataSet>(this);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(IntersectJoinInfo.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					switch (item.MemberName)
					{
					case MemberName.RowParentDataSet:
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						Global.Tracer.Assert(referenceableItems[item.RefID] is DataSet);
						this.m_rowParentDataSet = (DataSet)referenceableItems[item.RefID];
						break;
					case MemberName.ColumnParentDataSet:
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						Global.Tracer.Assert(referenceableItems[item.RefID] is DataSet);
						this.m_columnParentDataSet = (DataSet)referenceableItems[item.RefID];
						break;
					default:
						Global.Tracer.Assert(false);
						break;
					}
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IntersectJoinInfo;
		}
	}
}
