using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal class RuntimeHierarchyObj : RuntimeDataRegionObj, IHierarchyObj, IStorable, IPersistable
	{
		protected RuntimeGroupingObj m_grouping;

		protected RuntimeExpressionInfo m_expression;

		protected RuntimeHierarchyObjReference m_hierarchyRoot;

		protected List<IReference<RuntimeHierarchyObj>> m_hierarchyObjs;

		private static Declaration m_declaration = RuntimeHierarchyObj.GetDeclaration();

		internal List<IReference<RuntimeHierarchyObj>> HierarchyObjs
		{
			get
			{
				return this.m_hierarchyObjs;
			}
		}

		protected override IReference<IScope> OuterScope
		{
			get
			{
				Global.Tracer.Assert(false);
				return null;
			}
		}

		protected virtual IReference<IHierarchyObj> HierarchyRoot
		{
			get
			{
				return this.m_hierarchyRoot;
			}
		}

		internal RuntimeGroupingObj Grouping
		{
			get
			{
				return this.m_grouping;
			}
		}

		protected virtual BTree SortTree
		{
			get
			{
				return this.m_grouping.Tree;
			}
		}

		protected virtual int ExpressionIndex
		{
			get
			{
				if (this.m_expression != null)
				{
					return this.m_expression.ExpressionIndex;
				}
				return 0;
			}
		}

		protected virtual List<DataFieldRow> SortDataRows
		{
			get
			{
				Global.Tracer.Assert(false);
				return null;
			}
		}

		protected virtual List<int> SortFilterInfoIndices
		{
			get
			{
				Global.Tracer.Assert(false);
				return null;
			}
		}

		protected virtual bool IsDetail
		{
			get
			{
				return false;
			}
		}

		IReference<IHierarchyObj> IHierarchyObj.HierarchyRoot
		{
			get
			{
				return this.HierarchyRoot;
			}
		}

		OnDemandProcessingContext IHierarchyObj.OdpContext
		{
			get
			{
				return base.m_odpContext;
			}
		}

		BTree IHierarchyObj.SortTree
		{
			get
			{
				return this.SortTree;
			}
		}

		int IHierarchyObj.ExpressionIndex
		{
			get
			{
				return this.ExpressionIndex;
			}
		}

		List<int> IHierarchyObj.SortFilterInfoIndices
		{
			get
			{
				return this.SortFilterInfoIndices;
			}
		}

		bool IHierarchyObj.IsDetail
		{
			get
			{
				return this.IsDetail;
			}
		}

		bool IHierarchyObj.InDataRowSortPhase
		{
			get
			{
				return false;
			}
		}

		public override int Size
		{
			get
			{
				return base.Size + ItemSizes.SizeOf(this.m_grouping) + ItemSizes.SizeOf(this.m_expression) + ItemSizes.SizeOf(this.m_hierarchyRoot) + ItemSizes.SizeOf(this.m_hierarchyObjs);
			}
		}

		internal RuntimeHierarchyObj()
		{
		}

		protected RuntimeHierarchyObj(OnDemandProcessingContext odpContext, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, int level)
			: base(odpContext, objectType, level)
		{
		}

		internal RuntimeHierarchyObj(RuntimeHierarchyObj outerHierarchy, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, int level)
			: base(outerHierarchy.OdpContext, objectType, level)
		{
			if (outerHierarchy.m_expression != null)
			{
				this.ConstructorHelper(outerHierarchy.m_expression.ExpressionIndex + 1, outerHierarchy.m_hierarchyRoot);
			}
			else
			{
				this.ConstructorHelper(-1, outerHierarchy.m_hierarchyRoot);
			}
		}

		ProcessingMessageList IHierarchyObj.RegisterComparisonError(string propertyName)
		{
			return this.RegisterComparisonError(propertyName);
		}

		void IHierarchyObj.NextRow(IHierarchyObj owner)
		{
			this.NextRow();
		}

		void IHierarchyObj.Traverse(ProcessingStages operation, ITraversalContext traversalContext)
		{
			switch (operation)
			{
			case ProcessingStages.SortAndFilter:
				this.SortAndFilter((AggregateUpdateContext)traversalContext);
				break;
			case ProcessingStages.RunningValues:
				this.CalculateRunningValues((AggregateUpdateContext)traversalContext);
				break;
			case ProcessingStages.CreateGroupTree:
				this.CreateInstances((CreateInstancesTraversalContext)traversalContext);
				break;
			case ProcessingStages.UpdateAggregates:
				this.UpdateAggregates((AggregateUpdateContext)traversalContext);
				break;
			default:
				Global.Tracer.Assert(false);
				break;
			}
		}

		void IHierarchyObj.ReadRow()
		{
			this.ReadRow(DataActions.UserSort, null);
		}

		void IHierarchyObj.ProcessUserSort()
		{
			this.ProcessUserSort();
		}

		void IHierarchyObj.MarkSortInfoProcessed(List<IReference<RuntimeSortFilterEventInfo>> runtimeSortFilterInfo)
		{
			this.MarkSortInfoProcessed(runtimeSortFilterInfo);
		}

		void IHierarchyObj.AddSortInfoIndex(int sortInfoIndex, IReference<RuntimeSortFilterEventInfo> sortInfo)
		{
			this.AddSortInfoIndex(sortInfoIndex, sortInfo);
		}

		private void ConstructorHelper(int exprIndex, RuntimeHierarchyObjReference hierarchyRoot)
		{
			this.m_hierarchyRoot = hierarchyRoot;
			using (this.m_hierarchyRoot.PinValue())
			{
				RuntimeGroupRootObj runtimeGroupRootObj = this.m_hierarchyRoot.Value() as RuntimeGroupRootObj;
				Global.Tracer.Assert(null != runtimeGroupRootObj, "(null != groupRoot)");
				List<AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo> list;
				IndexedExprHost expressionsHost;
				List<bool> directions;
				if (ProcessingStages.Grouping == runtimeGroupRootObj.ProcessingStage)
				{
					list = runtimeGroupRootObj.GroupExpressions;
					expressionsHost = runtimeGroupRootObj.GroupExpressionHost;
					directions = runtimeGroupRootObj.GroupDirections;
				}
				else
				{
					Global.Tracer.Assert(ProcessingStages.SortAndFilter == runtimeGroupRootObj.ProcessingStage, "(ProcessingStages.SortAndFilter == groupRoot.ProcessingStage)");
					list = runtimeGroupRootObj.SortExpressions;
					expressionsHost = runtimeGroupRootObj.SortExpressionHost;
					directions = runtimeGroupRootObj.SortDirections;
				}
				if (exprIndex == -1 || exprIndex >= list.Count)
				{
					this.m_hierarchyObjs = new List<IReference<RuntimeHierarchyObj>>();
					RuntimeGroupLeafObjReference runtimeGroupLeafObjReference = null;
					IScalabilityCache tablixProcessingScalabilityCache = base.m_odpContext.TablixProcessingScalabilityCache;
					if (ProcessingStages.Grouping == runtimeGroupRootObj.ProcessingStage)
					{
						runtimeGroupLeafObjReference = runtimeGroupRootObj.CreateGroupLeaf();
						if (!runtimeGroupRootObj.HasParent)
						{
							runtimeGroupRootObj.AddChildWithNoParent(runtimeGroupLeafObjReference);
						}
					}
					if ((BaseReference)null != (object)runtimeGroupLeafObjReference)
					{
						this.m_hierarchyObjs.Add(runtimeGroupLeafObjReference);
					}
				}
				else
				{
					this.m_expression = new RuntimeExpressionInfo(list, expressionsHost, directions, exprIndex);
					this.m_grouping = RuntimeGroupingObj.CreateGroupingObj(runtimeGroupRootObj.GroupingType, this, base.m_objectType);
				}
			}
		}

		internal ProcessingMessageList RegisterComparisonError(string propertyName)
		{
			return this.RegisterComparisonError(propertyName, null);
		}

		internal ProcessingMessageList RegisterComparisonError(string propertyName, ReportProcessingException_ComparisonError e)
		{
			AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType = default(AspNetCore.ReportingServices.ReportProcessing.ObjectType);
			string name = default(string);
			using (this.m_hierarchyRoot.PinValue())
			{
				RuntimeGroupRootObj runtimeGroupRootObj = (RuntimeGroupRootObj)this.m_hierarchyRoot.Value();
				objectType = runtimeGroupRootObj.HierarchyDef.DataRegionDef.ObjectType;
				name = runtimeGroupRootObj.HierarchyDef.DataRegionDef.Name;
			}
			return base.m_odpContext.RegisterComparisonError(e, objectType, name, propertyName);
		}

		internal ProcessingMessageList RegisterSpatialTypeComparisonError(string type)
		{
			AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType = default(AspNetCore.ReportingServices.ReportProcessing.ObjectType);
			string name = default(string);
			using (this.m_hierarchyRoot.PinValue())
			{
				RuntimeGroupRootObj runtimeGroupRootObj = (RuntimeGroupRootObj)this.m_hierarchyRoot.Value();
				objectType = runtimeGroupRootObj.HierarchyDef.DataRegionDef.ObjectType;
				name = runtimeGroupRootObj.HierarchyDef.DataRegionDef.Name;
			}
			return base.m_odpContext.RegisterSpatialTypeComparisonError(objectType, name, type);
		}

		internal override void NextRow()
		{
			bool flag = true;
			RuntimeGroupRootObj runtimeGroupRootObj = null;
			using (this.m_hierarchyRoot.PinValue())
			{
				if (this.m_hierarchyRoot is RuntimeGroupRootObjReference)
				{
					runtimeGroupRootObj = (RuntimeGroupRootObj)this.m_hierarchyRoot.Value();
					if (ProcessingStages.SortAndFilter == runtimeGroupRootObj.ProcessingStage)
					{
						flag = false;
					}
				}
				if (this.m_hierarchyObjs != null)
				{
					if (flag)
					{
						IReference<RuntimeHierarchyObj> reference = this.m_hierarchyObjs[0];
						Global.Tracer.Assert(null != reference, "(null != hierarchyObj)");
						using (reference.PinValue())
						{
							reference.Value().NextRow();
						}
					}
					else if (runtimeGroupRootObj != null)
					{
						RuntimeGroupLeafObjReference lastChild = runtimeGroupRootObj.LastChild;
						Global.Tracer.Assert((BaseReference)null != (object)lastChild, "(null != groupLastChild)");
						this.m_hierarchyObjs.Add(lastChild);
					}
				}
				else if (this.m_grouping != null)
				{
					AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType = runtimeGroupRootObj.HierarchyDef.DataRegionDef.ObjectType;
					string name = runtimeGroupRootObj.HierarchyDef.DataRegionDef.Name;
					string propertyName = "GroupExpression";
					DomainScopeContext domainScopeContext = base.OdpContext.DomainScopeContext;
					DomainScopeContext.DomainScopeInfo domainScopeInfo = null;
					if (domainScopeContext != null)
					{
						domainScopeInfo = domainScopeContext.CurrentDomainScope;
					}
					object obj;
					if (domainScopeInfo != null)
					{
						domainScopeInfo.MoveNext();
						obj = domainScopeInfo.CurrentKey;
					}
					else
					{
						obj = ((this.m_expression != null) ? base.m_odpContext.ReportRuntime.EvaluateRuntimeExpression(this.m_expression, objectType, name, propertyName) : ((object)base.m_odpContext.ReportObjectModel.FieldsImpl.GetRowIndex()));
					}
					if (runtimeGroupRootObj != null && flag)
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping grouping = runtimeGroupRootObj.HierarchyDef.Grouping;
						if (runtimeGroupRootObj.SaveGroupExprValues)
						{
							grouping.CurrentGroupExpressionValues.Add(obj);
						}
						this.MatchSortFilterScope(runtimeGroupRootObj.SelfReference, grouping, obj, this.m_expression.ExpressionIndex);
					}
					this.m_grouping.NextRow(obj);
					if (domainScopeInfo != null)
					{
						domainScopeInfo.MovePrevious();
					}
				}
			}
		}

		internal override bool SortAndFilter(AggregateUpdateContext aggContext)
		{
			this.Traverse(ProcessingStages.SortAndFilter, aggContext);
			return true;
		}

		public override void UpdateAggregates(AggregateUpdateContext aggContext)
		{
			this.Traverse(ProcessingStages.UpdateAggregates, aggContext);
		}

		private void Traverse(ProcessingStages operation, AggregateUpdateContext aggContext)
		{
			if (this.m_grouping != null)
			{
				this.m_grouping.Traverse(ProcessingStages.SortAndFilter, true, aggContext);
			}
			if (this.m_hierarchyObjs != null)
			{
				for (int i = 0; i < this.m_hierarchyObjs.Count; i++)
				{
					IReference<RuntimeHierarchyObj> reference = this.m_hierarchyObjs[i];
					using (reference.PinValue())
					{
						switch (operation)
						{
						case ProcessingStages.SortAndFilter:
							reference.Value().SortAndFilter(aggContext);
							break;
						case ProcessingStages.UpdateAggregates:
							reference.Value().UpdateAggregates(aggContext);
							break;
						}
					}
				}
			}
		}

		internal virtual void CalculateRunningValues(AggregateUpdateContext aggContext)
		{
			if (this.m_grouping != null)
			{
				this.m_grouping.Traverse(ProcessingStages.RunningValues, this.m_expression == null || this.m_expression.Direction, aggContext);
			}
			if (this.m_hierarchyObjs != null)
			{
				bool flag = true;
				for (int i = 0; i < this.m_hierarchyObjs.Count; i++)
				{
					IReference<RuntimeHierarchyObj> reference = this.m_hierarchyObjs[i];
					using (reference.PinValue())
					{
						RuntimeHierarchyObj runtimeHierarchyObj = reference.Value();
						if (!flag || runtimeHierarchyObj is RuntimeGroupLeafObj)
						{
							((RuntimeGroupLeafObj)runtimeHierarchyObj).TraverseAllLeafNodes(ProcessingStages.RunningValues, aggContext);
							flag = false;
						}
					}
				}
			}
		}

		internal override void CalculateRunningValues(Dictionary<string, IReference<RuntimeGroupRootObj>> groupCol, IReference<RuntimeGroupRootObj> lastGroup, AggregateUpdateContext aggContext)
		{
			Global.Tracer.Assert(false);
		}

		internal override void CalculatePreviousAggregates()
		{
			Global.Tracer.Assert(false);
		}

		internal void CreateInstances(CreateInstancesTraversalContext traversalContext)
		{
			if (this.m_grouping != null)
			{
				this.m_grouping.Traverse(ProcessingStages.CreateGroupTree, this.m_expression == null || this.m_expression.Direction, traversalContext);
			}
			if (this.m_hierarchyObjs != null)
			{
				bool flag = true;
				for (int i = 0; i < this.m_hierarchyObjs.Count; i++)
				{
					IReference<RuntimeHierarchyObj> reference = this.m_hierarchyObjs[i];
					using (reference.PinValue())
					{
						RuntimeHierarchyObj runtimeHierarchyObj = reference.Value();
						if (!flag || runtimeHierarchyObj is RuntimeGroupLeafObj)
						{
							((RuntimeGroupLeafObj)runtimeHierarchyObj).TraverseAllLeafNodes(ProcessingStages.CreateGroupTree, traversalContext);
							flag = false;
						}
						else
						{
							((RuntimeDetailObj)runtimeHierarchyObj).CreateInstance(traversalContext);
						}
					}
				}
			}
		}

		internal virtual void CreateInstance(CreateInstancesTraversalContext traversalContext)
		{
			Global.Tracer.Assert(false);
		}

		public override void SetupEnvironment()
		{
		}

		public override void ReadRow(DataActions dataAction, ITraversalContext context)
		{
			Global.Tracer.Assert(false);
		}

		internal override bool InScope(string scope)
		{
			Global.Tracer.Assert(false);
			return false;
		}

		public virtual IHierarchyObj CreateHierarchyObjForSortTree()
		{
			return new RuntimeHierarchyObj(this, base.m_objectType, base.m_depth + 1);
		}

		protected void MatchSortFilterScope(IReference<IScope> outerScope, AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping groupDef, object groupExprValue, int groupExprIndex)
		{
			if (base.m_odpContext.RuntimeSortFilterInfo != null && groupDef.SortFilterScopeInfo != null)
			{
				List<IReference<RuntimeSortFilterEventInfo>> runtimeSortFilterInfo = base.m_odpContext.RuntimeSortFilterInfo;
				for (int i = 0; i < runtimeSortFilterInfo.Count; i++)
				{
					List<object> list = groupDef.SortFilterScopeInfo[i];
					if (list != null && outerScope.Value().TargetScopeMatched(i, false))
					{
						if (base.m_odpContext.ProcessingComparer.Compare(list[groupExprIndex], groupExprValue) != 0)
						{
							groupDef.SortFilterScopeMatched[i] = false;
						}
					}
					else
					{
						groupDef.SortFilterScopeMatched[i] = false;
					}
				}
			}
		}

		protected virtual void ProcessUserSort()
		{
			Global.Tracer.Assert(false);
		}

		protected virtual void MarkSortInfoProcessed(List<IReference<RuntimeSortFilterEventInfo>> runtimeSortFilterInfo)
		{
			Global.Tracer.Assert(false);
		}

		protected virtual void AddSortInfoIndex(int sortInfoIndex, IReference<RuntimeSortFilterEventInfo> sortInfo)
		{
			Global.Tracer.Assert(false);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(RuntimeHierarchyObj.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Grouping:
					writer.Write(this.m_grouping);
					break;
				case MemberName.Expression:
					writer.Write(this.m_expression);
					break;
				case MemberName.HierarchyRoot:
					writer.Write(this.m_hierarchyRoot);
					break;
				case MemberName.HierarchyObjs:
					writer.Write(this.m_hierarchyObjs);
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
			reader.RegisterDeclaration(RuntimeHierarchyObj.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Grouping:
					this.m_grouping = (RuntimeGroupingObj)reader.ReadRIFObject();
					if (this.m_grouping != null)
					{
						this.m_grouping.SetOwner(this);
					}
					break;
				case MemberName.Expression:
					this.m_expression = (RuntimeExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.HierarchyRoot:
					this.m_hierarchyRoot = (RuntimeHierarchyObjReference)reader.ReadRIFObject();
					break;
				case MemberName.HierarchyObjs:
					this.m_hierarchyObjs = reader.ReadListOfRIFObjects<List<IReference<RuntimeHierarchyObj>>>();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeHierarchyObj;
		}

		public new static Declaration GetDeclaration()
		{
			if (RuntimeHierarchyObj.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Grouping, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObj));
				list.Add(new MemberInfo(MemberName.Expression, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeExpressionInfo));
				list.Add(new MemberInfo(MemberName.HierarchyRoot, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeHierarchyObjReference));
				list.Add(new MemberInfo(MemberName.HierarchyObjs, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeHierarchyObjReference));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeHierarchyObj, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataRegionObj, list);
			}
			return RuntimeHierarchyObj.m_declaration;
		}
	}
}
