using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeSortFilterEventInfo : IStorable, IPersistable, ISelfReferential
	{
		[PersistedWithinRequestOnly]
		internal class SortFilterExpressionScopeObj : IHierarchyObj, IStorable, IPersistable
		{
			private ScalableList<IReference<RuntimeDataRegionObj>> m_scopeInstances;

			private ScalableList<SortScopeValuesHolder> m_scopeValuesList;

			private BTree m_sortTree;

			private int m_currentScopeInstanceIndex = -1;

			[NonSerialized]
			private static Declaration m_declaration = SortFilterExpressionScopeObj.GetDeclaration();

			public int Depth
			{
				get
				{
					return -1;
				}
			}

			internal int CurrentScopeInstanceIndex
			{
				get
				{
					return this.m_currentScopeInstanceIndex;
				}
			}

			IReference<IHierarchyObj> IHierarchyObj.HierarchyRoot
			{
				get
				{
					return null;
				}
			}

			OnDemandProcessingContext IHierarchyObj.OdpContext
			{
				get
				{
					Global.Tracer.Assert(0 < this.m_scopeInstances.Count, "(0 < m_scopeInstances.Count)");
					return this.m_scopeInstances[0].Value().OdpContext;
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
					return 0;
				}
			}

			List<int> IHierarchyObj.SortFilterInfoIndices
			{
				get
				{
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
					return false;
				}
			}

			public int Size
			{
				get
				{
					return ItemSizes.SizeOf(this.m_scopeInstances) + ItemSizes.SizeOf(this.m_scopeValuesList) + ItemSizes.SizeOf(this.m_sortTree) + 4;
				}
			}

			internal SortFilterExpressionScopeObj()
			{
			}

			internal SortFilterExpressionScopeObj(IReference<RuntimeSortFilterEventInfo> owner, OnDemandProcessingContext odpContext, int depth)
			{
				this.m_scopeInstances = new ScalableList<IReference<RuntimeDataRegionObj>>(depth, odpContext.TablixProcessingScalabilityCache);
				this.m_scopeValuesList = new ScalableList<SortScopeValuesHolder>(depth, odpContext.TablixProcessingScalabilityCache);
			}

			IHierarchyObj IHierarchyObj.CreateHierarchyObjForSortTree()
			{
				return new SortExpressionScopeInstanceHolder(null);
			}

			ProcessingMessageList IHierarchyObj.RegisterComparisonError(string propertyName)
			{
				return ((IHierarchyObj)this).OdpContext.RegisterComparisonErrorForSortFilterEvent(propertyName);
			}

			void IHierarchyObj.NextRow(IHierarchyObj owner)
			{
				Global.Tracer.Assert(false);
			}

			public void Traverse(ProcessingStages operation, ITraversalContext traversalContext)
			{
				UserSortFilterTraversalContext userSortFilterTraversalContext = (UserSortFilterTraversalContext)traversalContext;
				userSortFilterTraversalContext.ExpressionScope = this;
				if (this.m_sortTree != null)
				{
					this.m_sortTree.Traverse(operation, userSortFilterTraversalContext.EventInfo.SortDirection, traversalContext);
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

			void IHierarchyObj.AddSortInfoIndex(int sortFilterInfoIndex, IReference<RuntimeSortFilterEventInfo> sortInfo)
			{
				Global.Tracer.Assert(false);
			}

			internal void RegisterScopeInstance(IReference<RuntimeDataRegionObj> scopeObj, List<object>[] scopeValues)
			{
				this.m_scopeInstances.Add(scopeObj);
				this.m_scopeValuesList.Add(new SortScopeValuesHolder(scopeValues));
			}

			internal void SortSEScopes(OnDemandProcessingContext odpContext, IInScopeEventSource eventSource)
			{
				this.m_sortTree = new BTree(this, odpContext, this.Depth + 1);
				for (int i = 0; i < this.m_scopeInstances.Count; i++)
				{
					IReference<RuntimeDataRegionObj> reference = this.m_scopeInstances[i];
					this.m_currentScopeInstanceIndex = i;
					using (reference.PinValue())
					{
						RuntimeDataRegionObj runtimeDataRegionObj = reference.Value();
						runtimeDataRegionObj.SetupEnvironment();
					}
					this.m_sortTree.NextRow(odpContext.ReportRuntime.EvaluateUserSortExpression(eventSource), this);
				}
			}

			internal void AddSortOrder(RuntimeSortFilterEventInfo owner, int scopeInstanceIndex, bool incrementCounter)
			{
				owner.AddSortOrder(this.m_scopeValuesList[scopeInstanceIndex].Values, incrementCounter);
			}

			public void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(SortFilterExpressionScopeObj.m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.ScopeInstances:
						writer.Write(this.m_scopeInstances);
						break;
					case MemberName.SortTree:
						writer.Write(this.m_sortTree);
						break;
					case MemberName.ScopeValuesList:
						writer.Write(this.m_scopeValuesList);
						break;
					case MemberName.CurrentScopeIndex:
						writer.Write(this.m_currentScopeInstanceIndex);
						break;
					default:
						Global.Tracer.Assert(false);
						break;
					}
				}
			}

			public void Deserialize(IntermediateFormatReader reader)
			{
				reader.RegisterDeclaration(SortFilterExpressionScopeObj.m_declaration);
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.ScopeInstances:
						this.m_scopeInstances = reader.ReadRIFObject<ScalableList<IReference<RuntimeDataRegionObj>>>();
						break;
					case MemberName.ScopeValuesList:
						this.m_scopeValuesList = reader.ReadRIFObject<ScalableList<SortScopeValuesHolder>>();
						break;
					case MemberName.SortTree:
						this.m_sortTree = (BTree)reader.ReadRIFObject();
						break;
					case MemberName.CurrentScopeIndex:
						this.m_currentScopeInstanceIndex = reader.ReadInt32();
						break;
					default:
						Global.Tracer.Assert(false);
						break;
					}
				}
			}

			public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
			{
				return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortFilterExpressionScopeObj;
			}

			internal static Declaration GetDeclaration()
			{
				if (SortFilterExpressionScopeObj.m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.ScopeInstances, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataRegionObjReference));
					list.Add(new MemberInfo(MemberName.ScopeValuesList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortScopeValuesHolder));
					list.Add(new MemberInfo(MemberName.SortTree, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTree));
					list.Add(new MemberInfo(MemberName.CurrentScopeIndex, Token.Int32));
					return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortFilterExpressionScopeObj, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
				}
				return SortFilterExpressionScopeObj.m_declaration;
			}
		}

		[PersistedWithinRequestOnly]
		internal class SortScopeValuesHolder : IStorable, IPersistable
		{
			private List<object>[] m_values;

			[NonSerialized]
			private static Declaration m_declaration = SortScopeValuesHolder.GetDeclaration();

			public List<object>[] Values
			{
				get
				{
					return this.m_values;
				}
			}

			public int Size
			{
				get
				{
					return ItemSizes.SizeOf(this.m_values);
				}
			}

			public SortScopeValuesHolder()
			{
			}

			public SortScopeValuesHolder(List<object>[] values)
			{
				this.m_values = values;
			}

			public void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(SortScopeValuesHolder.m_declaration);
				while (writer.NextMember())
				{
					MemberName memberName = writer.CurrentMember.MemberName;
					if (memberName == MemberName.Values)
					{
						writer.WriteArrayOfListsOfPrimitives(this.m_values);
					}
					else
					{
						Global.Tracer.Assert(false);
					}
				}
			}

			public void Deserialize(IntermediateFormatReader reader)
			{
				reader.RegisterDeclaration(SortScopeValuesHolder.m_declaration);
				while (reader.NextMember())
				{
					MemberName memberName = reader.CurrentMember.MemberName;
					if (memberName == MemberName.Values)
					{
						this.m_values = reader.ReadArrayOfListsOfPrimitives<object>();
					}
					else
					{
						Global.Tracer.Assert(false);
					}
				}
			}

			public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
			{
				return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortScopeValuesHolder;
			}

			internal static Declaration GetDeclaration()
			{
				if (SortScopeValuesHolder.m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.Values, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, Token.Object, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList));
					return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortScopeValuesHolder, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
				}
				return SortScopeValuesHolder.m_declaration;
			}
		}

		[PersistedWithinRequestOnly]
		internal class SortExpressionScopeInstanceHolder : IHierarchyObj, IStorable, IPersistable
		{
			private List<int> m_scopeInstanceIndices;

			[NonSerialized]
			private static Declaration m_declaration = SortExpressionScopeInstanceHolder.GetDeclaration();

			public int Depth
			{
				get
				{
					return -1;
				}
			}

			IReference<IHierarchyObj> IHierarchyObj.HierarchyRoot
			{
				get
				{
					return null;
				}
			}

			OnDemandProcessingContext IHierarchyObj.OdpContext
			{
				get
				{
					return null;
				}
			}

			BTree IHierarchyObj.SortTree
			{
				get
				{
					return null;
				}
			}

			int IHierarchyObj.ExpressionIndex
			{
				get
				{
					return -1;
				}
			}

			List<int> IHierarchyObj.SortFilterInfoIndices
			{
				get
				{
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
					return false;
				}
			}

			public int Size
			{
				get
				{
					return ItemSizes.SizeOf(this.m_scopeInstanceIndices);
				}
			}

			internal SortExpressionScopeInstanceHolder()
			{
			}

			internal SortExpressionScopeInstanceHolder(OnDemandProcessingContext odpContext)
			{
				this.m_scopeInstanceIndices = new List<int>();
			}

			IHierarchyObj IHierarchyObj.CreateHierarchyObjForSortTree()
			{
				Global.Tracer.Assert(false);
				return null;
			}

			ProcessingMessageList IHierarchyObj.RegisterComparisonError(string propertyName)
			{
				Global.Tracer.Assert(false);
				return null;
			}

			void IHierarchyObj.NextRow(IHierarchyObj owner)
			{
				this.m_scopeInstanceIndices.Add(((SortFilterExpressionScopeObj)owner).CurrentScopeInstanceIndex);
			}

			void IHierarchyObj.Traverse(ProcessingStages operation, ITraversalContext traversalContext)
			{
				UserSortFilterTraversalContext userSortFilterTraversalContext = (UserSortFilterTraversalContext)traversalContext;
				RuntimeSortFilterEventInfo eventInfo = userSortFilterTraversalContext.EventInfo;
				SortFilterExpressionScopeObj expressionScope = userSortFilterTraversalContext.ExpressionScope;
				if (eventInfo.SortDirection)
				{
					for (int i = 0; i < this.m_scopeInstanceIndices.Count; i++)
					{
						expressionScope.AddSortOrder(eventInfo, this.m_scopeInstanceIndices[i], i == this.m_scopeInstanceIndices.Count - 1);
					}
				}
				else
				{
					for (int num = this.m_scopeInstanceIndices.Count - 1; num >= 0; num--)
					{
						expressionScope.AddSortOrder(eventInfo, this.m_scopeInstanceIndices[num], num == 0);
					}
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

			void IHierarchyObj.AddSortInfoIndex(int sortFilterInfoIndex, IReference<RuntimeSortFilterEventInfo> sortInfo)
			{
				Global.Tracer.Assert(false);
			}

			void IPersistable.Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(SortExpressionScopeInstanceHolder.m_declaration);
				while (writer.NextMember())
				{
					MemberName memberName = writer.CurrentMember.MemberName;
					if (memberName == MemberName.ScopeInstanceIndices)
					{
						writer.WriteListOfPrimitives(this.m_scopeInstanceIndices);
					}
					else
					{
						Global.Tracer.Assert(false);
					}
				}
			}

			void IPersistable.Deserialize(IntermediateFormatReader reader)
			{
				reader.RegisterDeclaration(SortExpressionScopeInstanceHolder.m_declaration);
				while (reader.NextMember())
				{
					MemberName memberName = reader.CurrentMember.MemberName;
					if (memberName == MemberName.ScopeInstanceIndices)
					{
						this.m_scopeInstanceIndices = reader.ReadListOfPrimitives<int>();
					}
					else
					{
						Global.Tracer.Assert(false);
					}
				}
			}

			void IPersistable.ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IPersistable.GetObjectType()
			{
				return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortExpressionScopeInstanceHolder;
			}

			internal static Declaration GetDeclaration()
			{
				if (SortExpressionScopeInstanceHolder.m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.ScopeInstanceIndices, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int32));
					return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortExpressionScopeInstanceHolder, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
				}
				return SortExpressionScopeInstanceHolder.m_declaration;
			}
		}

		[StaticReference]
		private IInScopeEventSource m_eventSource;

		private string m_oldUniqueName;

		private List<object>[] m_sortSourceScopeInfo;

		private bool m_sortDirection;

		private IReference<IScope> m_eventSourceRowScope;

		private IReference<IScope> m_eventSourceColScope;

		private int m_eventSourceColDetailIndex = -1;

		private int m_eventSourceRowDetailIndex = -1;

		private List<IReference<RuntimeDataRegionObj>> m_detailRowScopes;

		private List<IReference<RuntimeDataRegionObj>> m_detailColScopes;

		private List<int> m_detailRowScopeIndices;

		private List<int> m_detailColScopeIndices;

		private IReference<IHierarchyObj> m_eventTarget;

		private bool m_targetSortFilterInfoAdded;

		private List<RuntimeExpressionInfo> m_groupExpressionsInSortTarget;

		private List<SortFilterExpressionScopeObj> m_sortFilterExpressionScopeObjects;

		private int m_currentSortIndex = 1;

		private int m_currentInstanceIndex;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ScopeLookupTable m_sortOrders;

		private bool m_processed;

		private int m_nullScopeCount;

		private string m_newUniqueName;

		private int m_depth;

		private Hashtable m_peerSortFilters;

		[NonSerialized]
		private IReference<RuntimeSortFilterEventInfo> m_selfReference;

		private static Declaration m_declaration = RuntimeSortFilterEventInfo.GetDeclaration();

		internal IInScopeEventSource EventSource
		{
			get
			{
				return this.m_eventSource;
			}
		}

		internal bool HasEventSourceScope
		{
			get
			{
				if (this.m_eventSourceColScope == null)
				{
					return null != this.m_eventSourceRowScope;
				}
				return true;
			}
		}

		internal bool HasDetailScopeInfo
		{
			get
			{
				if (this.m_detailColScopes == null)
				{
					return null != this.m_detailRowScopes;
				}
				return true;
			}
		}

		internal int EventSourceColDetailIndex
		{
			get
			{
				return this.m_eventSourceColDetailIndex;
			}
			set
			{
				this.m_eventSourceColDetailIndex = value;
			}
		}

		internal int EventSourceRowDetailIndex
		{
			get
			{
				return this.m_eventSourceRowDetailIndex;
			}
			set
			{
				this.m_eventSourceRowDetailIndex = value;
			}
		}

		internal List<IReference<RuntimeDataRegionObj>> DetailRowScopes
		{
			get
			{
				return this.m_detailRowScopes;
			}
			set
			{
				this.m_detailRowScopes = value;
			}
		}

		internal List<IReference<RuntimeDataRegionObj>> DetailColScopes
		{
			get
			{
				return this.m_detailColScopes;
			}
			set
			{
				this.m_detailColScopes = value;
			}
		}

		internal List<int> DetailRowScopeIndices
		{
			get
			{
				return this.m_detailRowScopeIndices;
			}
			set
			{
				this.m_detailRowScopeIndices = value;
			}
		}

		internal List<int> DetailColScopeIndices
		{
			get
			{
				return this.m_detailColScopeIndices;
			}
			set
			{
				this.m_detailColScopeIndices = value;
			}
		}

		internal bool SortDirection
		{
			get
			{
				return this.m_sortDirection;
			}
			set
			{
				this.m_sortDirection = value;
			}
		}

		internal List<object>[] SortSourceScopeInfo
		{
			get
			{
				return this.m_sortSourceScopeInfo;
			}
		}

		internal IReference<IHierarchyObj> EventTarget
		{
			get
			{
				return this.m_eventTarget;
			}
			set
			{
				this.m_eventTarget = value;
			}
		}

		internal bool TargetSortFilterInfoAdded
		{
			get
			{
				return this.m_targetSortFilterInfoAdded;
			}
			set
			{
				this.m_targetSortFilterInfoAdded = value;
			}
		}

		internal bool Processed
		{
			get
			{
				return this.m_processed;
			}
			set
			{
				this.m_processed = value;
			}
		}

		internal string OldUniqueName
		{
			get
			{
				return this.m_oldUniqueName;
			}
		}

		internal string NewUniqueName
		{
			get
			{
				return this.m_newUniqueName;
			}
			set
			{
				this.m_newUniqueName = value;
			}
		}

		internal Hashtable PeerSortFilters
		{
			get
			{
				return this.m_peerSortFilters;
			}
			set
			{
				this.m_peerSortFilters = value;
			}
		}

		internal IReference<RuntimeSortFilterEventInfo> SelfReference
		{
			get
			{
				return this.m_selfReference;
			}
		}

		public int Size
		{
			get
			{
				return ItemSizes.ReferenceSize + 4 + ItemSizes.SizeOf(this.m_sortSourceScopeInfo) + 1 + ItemSizes.SizeOf(this.m_eventSourceRowScope) + ItemSizes.SizeOf(this.m_eventSourceColScope) + 4 + 4 + ItemSizes.SizeOf(this.m_detailRowScopes) + ItemSizes.SizeOf(this.m_detailColScopes) + ItemSizes.SizeOf(this.m_detailRowScopeIndices) + ItemSizes.SizeOf(this.m_detailColScopeIndices) + ItemSizes.SizeOf(this.m_eventTarget) + 1 + ItemSizes.SizeOf(this.m_groupExpressionsInSortTarget) + ItemSizes.SizeOf(this.m_sortFilterExpressionScopeObjects) + 4 + 4 + ItemSizes.SizeOf(this.m_sortOrders) + 1 + 4 + 4 + 4 + ItemSizes.SizeOf(this.m_peerSortFilters) + 4 + ItemSizes.SizeOf(this.m_selfReference);
			}
		}

		internal RuntimeSortFilterEventInfo()
		{
		}

		internal RuntimeSortFilterEventInfo(IInScopeEventSource eventSource, string oldUniqueName, bool sortDirection, List<object>[] sortSourceScopeInfo, OnDemandProcessingContext odpContext, int depth)
		{
			this.m_depth = depth;
			odpContext.TablixProcessingScalabilityCache.AllocateAndPin(this, this.m_depth);
			this.m_eventSource = eventSource;
			this.m_oldUniqueName = oldUniqueName;
			this.m_sortDirection = sortDirection;
			this.m_sortSourceScopeInfo = sortSourceScopeInfo;
		}

		internal IReference<IScope> GetEventSourceScope(bool isColumnAxis)
		{
			if (!isColumnAxis)
			{
				return this.m_eventSourceRowScope;
			}
			return this.m_eventSourceColScope;
		}

		internal void SetEventSourceScope(bool isColumnAxis, IReference<IScope> eventSourceScope, int rowIndex)
		{
			if (isColumnAxis)
			{
				Global.Tracer.Assert(null == this.m_eventSourceColScope);
				this.m_eventSourceColScope = eventSourceScope;
				this.m_eventSourceColDetailIndex = rowIndex;
			}
			else
			{
				Global.Tracer.Assert(null == this.m_eventSourceRowScope);
				this.m_eventSourceRowScope = eventSourceScope;
				this.m_eventSourceRowDetailIndex = rowIndex;
			}
		}

		internal void UpdateEventSourceScope(bool isColumnAxis, IReference<IScope> eventSourceScope, int rootRowCount)
		{
			if (isColumnAxis)
			{
				this.m_eventSourceColScope = eventSourceScope;
				this.m_eventSourceColDetailIndex += rootRowCount;
			}
			else
			{
				this.m_eventSourceRowScope = eventSourceScope;
				this.m_eventSourceRowDetailIndex += rootRowCount;
			}
		}

		internal void AddDetailScopeInfo(bool isColumnAxis, RuntimeDataRegionObjReference dataRegionReference, int detailRowIndex)
		{
			if (this.m_detailRowScopes == null)
			{
				this.m_detailRowScopes = new List<IReference<RuntimeDataRegionObj>>();
				this.m_detailRowScopeIndices = new List<int>();
				this.m_detailColScopes = new List<IReference<RuntimeDataRegionObj>>();
				this.m_detailColScopeIndices = new List<int>();
			}
			if (isColumnAxis)
			{
				this.m_detailColScopes.Add(dataRegionReference);
				this.m_detailColScopeIndices.Add(detailRowIndex);
			}
			else
			{
				this.m_detailRowScopes.Add(dataRegionReference);
				this.m_detailRowScopeIndices.Add(detailRowIndex);
			}
		}

		internal void UpdateDetailScopeInfo(RuntimeGroupRootObj detailRoot, bool isColumnAxis, int rootRowCount, RuntimeDataRegionObjReference selfReference)
		{
			List<IReference<RuntimeDataRegionObj>> list;
			List<int> list2;
			if (isColumnAxis)
			{
				list = this.m_detailColScopes;
				list2 = this.m_detailColScopeIndices;
			}
			else
			{
				list = this.m_detailRowScopes;
				list2 = this.m_detailRowScopeIndices;
			}
			for (int i = 0; i < list.Count; i++)
			{
				if ((BaseReference)selfReference == (object)list[i])
				{
					list[i] = detailRoot.SelfReference;
					List<int> list3;
					int index;
					(list3 = list2)[index = i] = list3[index] + rootRowCount;
				}
			}
		}

		internal void RegisterSortFilterExpressionScope(ref int containerSortFilterExprScopeIndex, IReference<RuntimeDataRegionObj> scopeObj, List<object>[] scopeValues, int sortFilterInfoIndex)
		{
			if (this.m_eventTarget != null && !this.m_targetSortFilterInfoAdded)
			{
				using (this.m_eventTarget.PinValue())
				{
					this.m_eventTarget.Value().AddSortInfoIndex(sortFilterInfoIndex, this.SelfReference);
				}
			}
			SortFilterExpressionScopeObj sortFilterExpressionScopeObj = null;
			if (-1 != containerSortFilterExprScopeIndex)
			{
				sortFilterExpressionScopeObj = this.m_sortFilterExpressionScopeObjects[containerSortFilterExprScopeIndex];
			}
			else
			{
				if (this.m_sortFilterExpressionScopeObjects == null)
				{
					this.m_sortFilterExpressionScopeObjects = new List<SortFilterExpressionScopeObj>();
				}
				containerSortFilterExprScopeIndex = this.m_sortFilterExpressionScopeObjects.Count;
				sortFilterExpressionScopeObj = new SortFilterExpressionScopeObj(this.m_selfReference, scopeObj.Value().OdpContext, this.m_depth + 1);
				this.m_sortFilterExpressionScopeObjects.Add(sortFilterExpressionScopeObj);
			}
			sortFilterExpressionScopeObj.RegisterScopeInstance(scopeObj, scopeValues);
		}

		internal void PrepareForSorting(OnDemandProcessingContext odpContext)
		{
			Global.Tracer.Assert(!this.m_processed, "(!m_processed)");
			if (this.m_eventTarget != null && this.m_sortFilterExpressionScopeObjects != null)
			{
				odpContext.UserSortFilterContext.CurrentSortFilterEventSource = this.m_eventSource;
				for (int i = 0; i < this.m_sortFilterExpressionScopeObjects.Count; i++)
				{
					SortFilterExpressionScopeObj sortFilterExpressionScopeObj = this.m_sortFilterExpressionScopeObjects[i];
					sortFilterExpressionScopeObj.SortSEScopes(odpContext, this.m_eventSource);
				}
				AspNetCore.ReportingServices.ReportIntermediateFormat.GroupingList groupsInSortTarget = this.m_eventSource.UserSort.GroupsInSortTarget;
				if (groupsInSortTarget != null && 0 < groupsInSortTarget.Count)
				{
					this.m_groupExpressionsInSortTarget = new List<RuntimeExpressionInfo>();
					for (int j = 0; j < groupsInSortTarget.Count; j++)
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping grouping = groupsInSortTarget[j];
						for (int k = 0; k < grouping.GroupExpressions.Count; k++)
						{
							this.m_groupExpressionsInSortTarget.Add(new RuntimeExpressionInfo(grouping.GroupExpressions, grouping.ExprHost, null, k));
						}
					}
				}
				this.CollectSortOrders();
			}
		}

		private void CollectSortOrders()
		{
			this.m_currentSortIndex = 1;
			UserSortFilterTraversalContext traversalContext = new UserSortFilterTraversalContext(this);
			for (int i = 0; i < this.m_sortFilterExpressionScopeObjects.Count; i++)
			{
				SortFilterExpressionScopeObj sortFilterExpressionScopeObj = this.m_sortFilterExpressionScopeObjects[i];
				sortFilterExpressionScopeObj.Traverse(ProcessingStages.UserSortFilter, traversalContext);
			}
			this.m_sortFilterExpressionScopeObjects = null;
		}

		internal bool ProcessSorting(OnDemandProcessingContext odpContext)
		{
			Global.Tracer.Assert(!this.m_processed, "(!m_processed)");
			if (this.m_eventTarget == null)
			{
				return false;
			}
			using (this.m_eventTarget.PinValue())
			{
				this.m_eventTarget.Value().ProcessUserSort();
			}
			this.m_sortOrders = null;
			return true;
		}

		private void AddSortOrder(List<object>[] scopeValues, bool incrementCounter)
		{
			if (this.m_sortOrders == null)
			{
				this.m_sortOrders = new AspNetCore.ReportingServices.ReportIntermediateFormat.ScopeLookupTable();
			}
			if (scopeValues == null || scopeValues.Length == 0)
			{
				this.m_sortOrders.Add(this.m_eventSource.UserSort.GroupsInSortTarget, scopeValues, this.m_currentSortIndex);
			}
			else
			{
				int num = 0;
				for (int i = 0; i < scopeValues.Length; i++)
				{
					if (scopeValues[i] == null)
					{
						num++;
					}
				}
				if (num >= this.m_nullScopeCount)
				{
					if (num > this.m_nullScopeCount)
					{
						this.m_sortOrders.Clear();
						this.m_nullScopeCount = num;
					}
					this.m_sortOrders.Add(this.m_eventSource.UserSort.GroupsInSortTarget, scopeValues, this.m_currentSortIndex);
				}
			}
			if (incrementCounter)
			{
				this.m_currentSortIndex++;
			}
		}

		internal object GetSortOrder(AspNetCore.ReportingServices.RdlExpressions.ReportRuntime runtime)
		{
			object obj = null;
			if (this.m_eventSource.UserSort.SortExpressionScope == null)
			{
				obj = runtime.EvaluateUserSortExpression(this.m_eventSource);
			}
			else if (this.m_sortOrders == null)
			{
				obj = null;
			}
			else
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.GroupingList groupsInSortTarget = this.m_eventSource.UserSort.GroupsInSortTarget;
				if (groupsInSortTarget == null || groupsInSortTarget.Count == 0)
				{
					obj = this.m_sortOrders.LookupTable;
				}
				else
				{
					bool flag = true;
					bool flag2 = false;
					int num = 0;
					Hashtable hashtable = this.m_sortOrders.LookupTable;
					int num2 = 0;
					int num3 = 0;
					while (num3 < groupsInSortTarget.Count)
					{
						IEnumerator enumerator = hashtable.Keys.GetEnumerator();
						enumerator.MoveNext();
						num2 = (int)enumerator.Current;
						for (int i = 0; i < num2; i++)
						{
							num += groupsInSortTarget[num3++].GroupExpressions.Count;
						}
						hashtable = (Hashtable)hashtable[num2];
						if (num3 < groupsInSortTarget.Count)
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping grouping = groupsInSortTarget[num3];
							for (int j = 0; j < grouping.GroupExpressions.Count; j++)
							{
								object key = runtime.EvaluateRuntimeExpression(this.m_groupExpressionsInSortTarget[num], AspNetCore.ReportingServices.ReportProcessing.ObjectType.Grouping, grouping.Name, "GroupExpression");
								num++;
								obj = hashtable[key];
								if (obj == null)
								{
									flag = false;
									break;
								}
								if (num < this.m_groupExpressionsInSortTarget.Count)
								{
									hashtable = (Hashtable)obj;
								}
							}
							num3++;
							if (!flag)
							{
								break;
							}
							continue;
						}
						flag2 = true;
						break;
					}
					if (flag && flag2)
					{
						obj = hashtable[1];
						if (obj == null)
						{
							flag = false;
						}
					}
					if (flag)
					{
						this.m_currentInstanceIndex = this.m_currentSortIndex + 1;
					}
					else
					{
						obj = this.m_currentInstanceIndex;
					}
				}
			}
			if (obj == null)
			{
				obj = DBNull.Value;
			}
			return obj;
		}

		internal void MatchEventSource(IInScopeEventSource eventSource, string eventSourceUniqueNameString, IScope containingScope, OnDemandProcessingContext odpContext)
		{
			bool flag = false;
			if (!(containingScope is RuntimeCell))
			{
				while (containingScope != null && !(containingScope is RuntimeGroupLeafObj) && !(containingScope is RuntimeDetailObj))
				{
					IReference<IScope> outerScope = containingScope.GetOuterScope(true);
					containingScope = ((outerScope != null) ? outerScope.Value() : null);
				}
			}
			if (containingScope == null)
			{
				if (this.m_eventSource.ContainingScopes == null || this.m_eventSource.ContainingScopes.Count == 0)
				{
					flag = true;
				}
			}
			else
			{
				if (this.m_eventSourceRowScope != null && this.m_eventSourceRowScope.Value() == containingScope)
				{
					goto IL_008f;
				}
				if (this.m_eventSourceColScope != null && this.m_eventSourceColScope.Value() == containingScope)
				{
					goto IL_008f;
				}
			}
			goto IL_00f9;
			IL_008f:
			flag = true;
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion = null;
			bool flag2 = false;
			RuntimeGroupLeafObj runtimeGroupLeafObj = containingScope as RuntimeGroupLeafObj;
			if (runtimeGroupLeafObj != null && runtimeGroupLeafObj.MemberDef.Grouping.IsDetail)
			{
				dataRegion = runtimeGroupLeafObj.MemberDef.DataRegionDef;
				flag2 = runtimeGroupLeafObj.MemberDef.IsColumn;
			}
			if (dataRegion != null)
			{
				if (flag2 && dataRegion.CurrentColDetailIndex != this.m_eventSourceColDetailIndex)
				{
					flag = false;
				}
				else if (!flag2 && dataRegion.CurrentRowDetailIndex != this.m_eventSourceRowDetailIndex)
				{
					flag = false;
				}
			}
			goto IL_00f9;
			IL_00f9:
			if (flag)
			{
				if (eventSource == this.m_eventSource)
				{
					this.m_newUniqueName = eventSourceUniqueNameString;
				}
				else if (this.m_peerSortFilters != null && this.m_peerSortFilters.Contains(eventSource.ID))
				{
					this.m_peerSortFilters[eventSource.ID] = eventSourceUniqueNameString;
				}
			}
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(RuntimeSortFilterEventInfo.m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.EventSource:
				{
					int value = scalabilityCache.StoreStaticReference(this.m_eventSource);
					writer.Write(value);
					break;
				}
				case MemberName.OldUniqueName:
					writer.Write(this.m_oldUniqueName);
					break;
				case MemberName.SortSourceScopeInfo:
					writer.WriteArrayOfListsOfPrimitives(this.m_sortSourceScopeInfo);
					break;
				case MemberName.SortDirection:
					writer.Write(this.m_sortDirection);
					break;
				case MemberName.EventSourceRowScope:
					writer.Write(this.m_eventSourceRowScope);
					break;
				case MemberName.EventSourceColScope:
					writer.Write(this.m_eventSourceColScope);
					break;
				case MemberName.EventSourceColDetailIndex:
					writer.Write(this.m_eventSourceColDetailIndex);
					break;
				case MemberName.EventSourceRowDetailIndex:
					writer.Write(this.m_eventSourceRowDetailIndex);
					break;
				case MemberName.DetailRowScopes:
					writer.Write(this.m_detailRowScopes);
					break;
				case MemberName.DetailColScopes:
					writer.Write(this.m_detailColScopes);
					break;
				case MemberName.DetailRowScopeIndices:
					writer.WriteListOfPrimitives(this.m_detailRowScopeIndices);
					break;
				case MemberName.DetailColScopeIndices:
					writer.WriteListOfPrimitives(this.m_detailColScopeIndices);
					break;
				case MemberName.EventTarget:
					writer.Write(this.m_eventTarget);
					break;
				case MemberName.TargetSortFilterInfoAdded:
					writer.Write(this.m_targetSortFilterInfoAdded);
					break;
				case MemberName.GroupExpressionsInSortTarget:
					writer.Write(this.m_groupExpressionsInSortTarget);
					break;
				case MemberName.SortFilterExpressionScopeObjects:
					writer.Write(this.m_sortFilterExpressionScopeObjects);
					break;
				case MemberName.CurrentSortIndex:
					writer.Write(this.m_currentSortIndex);
					break;
				case MemberName.CurrentInstanceIndex:
					writer.Write(this.m_currentInstanceIndex);
					break;
				case MemberName.SortOrders:
					writer.Write(this.m_sortOrders);
					break;
				case MemberName.Processed:
					writer.Write(this.m_processed);
					break;
				case MemberName.NullScopeCount:
					writer.Write(this.m_nullScopeCount);
					break;
				case MemberName.NewUniqueName:
					writer.Write(this.m_newUniqueName);
					break;
				case MemberName.PeerSortFilters:
					writer.WriteInt32StringHashtable(this.m_peerSortFilters);
					break;
				case MemberName.Depth:
					writer.Write(this.m_depth);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(RuntimeSortFilterEventInfo.m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.EventSource:
				{
					int id = reader.ReadInt32();
					this.m_eventSource = (IInScopeEventSource)scalabilityCache.FetchStaticReference(id);
					break;
				}
				case MemberName.OldUniqueName:
					this.m_oldUniqueName = reader.ReadString();
					break;
				case MemberName.SortSourceScopeInfo:
					this.m_sortSourceScopeInfo = reader.ReadArrayOfListsOfPrimitives<object>();
					break;
				case MemberName.SortDirection:
					this.m_sortDirection = reader.ReadBoolean();
					break;
				case MemberName.EventSourceRowScope:
					this.m_eventSourceRowScope = (IReference<IScope>)reader.ReadRIFObject();
					break;
				case MemberName.EventSourceColScope:
					this.m_eventSourceColScope = (IReference<IScope>)reader.ReadRIFObject();
					break;
				case MemberName.EventSourceColDetailIndex:
					this.m_eventSourceColDetailIndex = reader.ReadInt32();
					break;
				case MemberName.EventSourceRowDetailIndex:
					this.m_eventSourceRowDetailIndex = reader.ReadInt32();
					break;
				case MemberName.DetailRowScopes:
					this.m_detailRowScopes = reader.ReadListOfRIFObjects<List<IReference<RuntimeDataRegionObj>>>();
					break;
				case MemberName.DetailColScopes:
					this.m_detailColScopes = reader.ReadListOfRIFObjects<List<IReference<RuntimeDataRegionObj>>>();
					break;
				case MemberName.DetailRowScopeIndices:
					this.m_detailRowScopeIndices = reader.ReadListOfPrimitives<int>();
					break;
				case MemberName.DetailColScopeIndices:
					this.m_detailColScopeIndices = reader.ReadListOfPrimitives<int>();
					break;
				case MemberName.EventTarget:
					this.m_eventTarget = (IReference<IHierarchyObj>)reader.ReadRIFObject();
					break;
				case MemberName.TargetSortFilterInfoAdded:
					this.m_targetSortFilterInfoAdded = reader.ReadBoolean();
					break;
				case MemberName.GroupExpressionsInSortTarget:
					this.m_groupExpressionsInSortTarget = reader.ReadListOfRIFObjects<List<RuntimeExpressionInfo>>();
					break;
				case MemberName.SortFilterExpressionScopeObjects:
					this.m_sortFilterExpressionScopeObjects = reader.ReadListOfRIFObjects<List<SortFilterExpressionScopeObj>>();
					break;
				case MemberName.CurrentSortIndex:
					this.m_currentSortIndex = reader.ReadInt32();
					break;
				case MemberName.CurrentInstanceIndex:
					this.m_currentInstanceIndex = reader.ReadInt32();
					break;
				case MemberName.SortOrders:
					this.m_sortOrders = (AspNetCore.ReportingServices.ReportIntermediateFormat.ScopeLookupTable)reader.ReadRIFObject();
					break;
				case MemberName.Processed:
					this.m_processed = reader.ReadBoolean();
					break;
				case MemberName.NullScopeCount:
					this.m_nullScopeCount = reader.ReadInt32();
					break;
				case MemberName.NewUniqueName:
					this.m_newUniqueName = reader.ReadString();
					break;
				case MemberName.PeerSortFilters:
					this.m_peerSortFilters = reader.ReadInt32StringHashtable();
					break;
				case MemberName.Depth:
					this.m_depth = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortFilterEventInfo;
		}

		public static Declaration GetDeclaration()
		{
			if (RuntimeSortFilterEventInfo.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.EventSource, Token.Int32));
				list.Add(new MemberInfo(MemberName.OldUniqueName, Token.String));
				list.Add(new MemberInfo(MemberName.SortSourceScopeInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList));
				list.Add(new MemberInfo(MemberName.SortDirection, Token.Boolean));
				list.Add(new MemberInfo(MemberName.EventSourceRowScope, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IScopeReference));
				list.Add(new MemberInfo(MemberName.EventSourceColScope, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IScopeReference));
				list.Add(new MemberInfo(MemberName.EventSourceRowDetailIndex, Token.Int32));
				list.Add(new MemberInfo(MemberName.EventSourceColDetailIndex, Token.Int32));
				list.Add(new MemberInfo(MemberName.DetailRowScopes, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataRegionObjReference));
				list.Add(new MemberInfo(MemberName.DetailColScopes, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataRegionObjReference));
				list.Add(new MemberInfo(MemberName.DetailRowScopeIndices, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int32));
				list.Add(new MemberInfo(MemberName.DetailColScopeIndices, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int32));
				list.Add(new MemberInfo(MemberName.EventTarget, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IHierarchyObjReference));
				list.Add(new MemberInfo(MemberName.TargetSortFilterInfoAdded, Token.Boolean));
				list.Add(new MemberInfo(MemberName.GroupExpressionsInSortTarget, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeExpressionInfo));
				list.Add(new MemberInfo(MemberName.SortFilterExpressionScopeObjects, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortFilterExpressionScopeObj));
				list.Add(new MemberInfo(MemberName.CurrentSortIndex, Token.Int32));
				list.Add(new MemberInfo(MemberName.CurrentInstanceIndex, Token.Int32));
				list.Add(new MemberInfo(MemberName.SortOrders, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScopeLookupTable));
				list.Add(new MemberInfo(MemberName.Processed, Token.Boolean));
				list.Add(new MemberInfo(MemberName.NullScopeCount, Token.Int32));
				list.Add(new MemberInfo(MemberName.NewUniqueName, Token.String));
				list.Add(new MemberInfo(MemberName.PeerSortFilters, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Int32StringHashtable));
				list.Add(new MemberInfo(MemberName.Depth, Token.Int32));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortFilterEventInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return RuntimeSortFilterEventInfo.m_declaration;
		}

		public void SetReference(IReference selfRef)
		{
			this.m_selfReference = (IReference<RuntimeSortFilterEventInfo>)selfRef;
		}
	}
}
