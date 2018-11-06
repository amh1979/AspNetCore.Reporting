using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeDataRowSortHierarchyObj : IHierarchyObj, IStorable, IPersistable
	{
		private IReference<IHierarchyObj> m_hierarchyRoot;

		private RuntimeSortDataHolder m_dataRowHolder;

		private RuntimeExpressionInfo m_sortExpression;

		private BTree m_sortTree;

		private static readonly Declaration m_declaration = RuntimeDataRowSortHierarchyObj.GetDeclaration();

		public int Depth
		{
			get
			{
				return this.m_hierarchyRoot.Value().Depth + 1;
			}
		}

		IReference<IHierarchyObj> IHierarchyObj.HierarchyRoot
		{
			get
			{
				return this.m_hierarchyRoot;
			}
		}

		public OnDemandProcessingContext OdpContext
		{
			get
			{
				return this.m_hierarchyRoot.Value().OdpContext;
			}
		}

		BTree IHierarchyObj.SortTree
		{
			get
			{
				return this.m_sortTree;
			}
		}

		int IHierarchyObj.ExpressionIndex
		{
			get
			{
				Global.Tracer.Assert(this.m_sortExpression != null, "m_sortExpression != null");
				return this.m_sortExpression.ExpressionIndex;
			}
		}

		List<int> IHierarchyObj.SortFilterInfoIndices
		{
			get
			{
				Global.Tracer.Assert(false, "SortFilterInfoIndices should not be called on this type");
				return null;
			}
		}

		bool IHierarchyObj.IsDetail
		{
			get
			{
				return false;
			}
		}

		bool IHierarchyObj.InDataRowSortPhase
		{
			get
			{
				return true;
			}
		}

		public int Size
		{
			get
			{
				return ItemSizes.SizeOf(this.m_hierarchyRoot) + ItemSizes.SizeOf(this.m_dataRowHolder) + ItemSizes.SizeOf(this.m_sortExpression) + ItemSizes.SizeOf(this.m_sortTree);
			}
		}

		internal RuntimeDataRowSortHierarchyObj()
		{
		}

		internal RuntimeDataRowSortHierarchyObj(IHierarchyObj outerHierarchy, int depth)
		{
			this.m_hierarchyRoot = outerHierarchy.HierarchyRoot;
			int num = outerHierarchy.ExpressionIndex + 1;
			IDataRowSortOwner dataRowSortOwner = (IDataRowSortOwner)this.m_hierarchyRoot.Value();
			AspNetCore.ReportingServices.ReportIntermediateFormat.Sorting sortingDef = dataRowSortOwner.SortingDef;
			if (sortingDef.SortExpressions == null || num >= sortingDef.SortExpressions.Count)
			{
				this.m_dataRowHolder = new RuntimeSortDataHolder();
			}
			else
			{
				this.m_sortExpression = new RuntimeExpressionInfo(sortingDef.SortExpressions, sortingDef.ExprHost, sortingDef.SortDirections, num);
				this.m_sortTree = new BTree(this, this.OdpContext, depth);
			}
		}

		IHierarchyObj IHierarchyObj.CreateHierarchyObjForSortTree()
		{
			return new RuntimeDataRowSortHierarchyObj(this, this.Depth + 1);
		}

		ProcessingMessageList IHierarchyObj.RegisterComparisonError(string propertyName)
		{
			return this.m_hierarchyRoot.Value().RegisterComparisonError(propertyName);
		}

		void IHierarchyObj.NextRow(IHierarchyObj owner)
		{
			if (this.m_dataRowHolder != null)
			{
				this.m_dataRowHolder.NextRow(owner.OdpContext, owner.Depth);
			}
			else
			{
				IDataRowSortOwner dataRowSortOwner = (IDataRowSortOwner)this.m_hierarchyRoot.Value();
				object keyValue = dataRowSortOwner.EvaluateDataRowSortExpression(this.m_sortExpression);
				this.m_sortTree.NextRow(keyValue, this);
			}
		}

		void IHierarchyObj.Traverse(ProcessingStages operation, ITraversalContext traversalContext)
		{
			if (this.m_dataRowHolder != null)
			{
				this.m_dataRowHolder.Traverse(operation, traversalContext, this);
			}
			else
			{
				this.m_sortTree.Traverse(operation, this.m_sortExpression.Direction, traversalContext);
			}
		}

		void IHierarchyObj.ReadRow()
		{
			Global.Tracer.Assert(false);
		}

		void IHierarchyObj.ProcessUserSort()
		{
			Global.Tracer.Assert(false);
		}

		void IHierarchyObj.MarkSortInfoProcessed(List<IReference<RuntimeSortFilterEventInfo>> runtimeSortFilterInfo)
		{
			Global.Tracer.Assert(false);
		}

		void IHierarchyObj.AddSortInfoIndex(int sortInfoIndex, IReference<RuntimeSortFilterEventInfo> sortInfo)
		{
			Global.Tracer.Assert(false);
		}

		void IPersistable.Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(RuntimeDataRowSortHierarchyObj.m_declaration);
			PersistenceHelper persistenceHelper = writer.PersistenceHelper;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.HierarchyRoot:
					writer.Write(this.m_hierarchyRoot);
					break;
				case MemberName.DataRowHolder:
					writer.Write(this.m_dataRowHolder);
					break;
				case MemberName.Expression:
					writer.Write(this.m_sortExpression);
					break;
				case MemberName.SortTree:
					writer.Write(this.m_sortTree);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		void IPersistable.Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(RuntimeDataRowSortHierarchyObj.m_declaration);
			PersistenceHelper persistenceHelper = reader.PersistenceHelper;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.HierarchyRoot:
					this.m_hierarchyRoot = (IReference<IHierarchyObj>)reader.ReadRIFObject();
					break;
				case MemberName.DataRowHolder:
					this.m_dataRowHolder = (RuntimeSortDataHolder)reader.ReadRIFObject();
					break;
				case MemberName.Expression:
					this.m_sortExpression = (RuntimeExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SortTree:
					this.m_sortTree = (BTree)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		void IPersistable.ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IPersistable.GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataRowSortHierarchyObj;
		}

		internal static Declaration GetDeclaration()
		{
			if (RuntimeDataRowSortHierarchyObj.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.HierarchyRoot, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IHierarchyObjReference));
				list.Add(new MemberInfo(MemberName.DataRowHolder, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortDataHolder));
				list.Add(new MemberInfo(MemberName.Expression, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeExpressionInfo));
				list.Add(new MemberInfo(MemberName.SortTree, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTree));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataRowSortHierarchyObj, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return RuntimeDataRowSortHierarchyObj.m_declaration;
		}
	}
}
