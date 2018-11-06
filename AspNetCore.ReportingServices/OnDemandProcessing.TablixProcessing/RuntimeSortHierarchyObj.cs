using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeSortHierarchyObj : IHierarchyObj, IStorable, IPersistable
	{
		[PersistedWithinRequestOnly]
		internal class SortHierarchyStructure : IStorable, IPersistable
		{
			internal IReference<RuntimeSortFilterEventInfo> SortInfo;

			internal int SortIndex;

			internal BTree SortTree;

			[NonSerialized]
			private static Declaration m_declaration = SortHierarchyStructure.GetDeclaration();

			public int Size
			{
				get
				{
					return ItemSizes.SizeOf(this.SortInfo) + 4 + ItemSizes.SizeOf(this.SortTree);
				}
			}

			internal SortHierarchyStructure()
			{
			}

			internal SortHierarchyStructure(IHierarchyObj owner, int sortIndex, List<IReference<RuntimeSortFilterEventInfo>> sortInfoList, List<int> sortInfoIndices)
			{
				this.SortIndex = sortIndex;
				this.SortInfo = sortInfoList[sortInfoIndices[sortIndex]];
				this.SortTree = new BTree(owner, owner.OdpContext, owner.Depth);
			}

			public void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(SortHierarchyStructure.m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.SortInfo:
						writer.Write(this.SortInfo);
						break;
					case MemberName.SortIndex:
						writer.Write(this.SortIndex);
						break;
					case MemberName.SortTree:
						writer.Write(this.SortTree);
						break;
					default:
						Global.Tracer.Assert(false);
						break;
					}
				}
			}

			public void Deserialize(IntermediateFormatReader reader)
			{
				reader.RegisterDeclaration(SortHierarchyStructure.m_declaration);
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.SortInfo:
						this.SortInfo = (IReference<RuntimeSortFilterEventInfo>)reader.ReadRIFObject();
						break;
					case MemberName.SortIndex:
						this.SortIndex = reader.ReadInt32();
						break;
					case MemberName.SortTree:
						this.SortTree = (BTree)reader.ReadRIFObject();
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
				return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortHierarchyStruct;
			}

			public static Declaration GetDeclaration()
			{
				if (SortHierarchyStructure.m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.SortInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortFilterEventInfoReference));
					list.Add(new MemberInfo(MemberName.SortIndex, Token.Int32));
					list.Add(new MemberInfo(MemberName.SortTree, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTreeNode));
					return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortHierarchyStruct, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
				}
				return SortHierarchyStructure.m_declaration;
			}
		}

		private IReference<IHierarchyObj> m_hierarchyRoot;

		private OnDemandProcessingContext m_odpContext;

		private SortHierarchyStructure m_sortHierarchyStruct;

		private IReference<ISortDataHolder> m_dataHolder;

		private RuntimeSortDataHolder m_dataRowHolder;

		private static readonly Declaration m_declaration = RuntimeSortHierarchyObj.GetDeclaration();

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

		OnDemandProcessingContext IHierarchyObj.OdpContext
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
				if (this.m_sortHierarchyStruct != null)
				{
					return this.m_sortHierarchyStruct.SortTree;
				}
				return null;
			}
		}

		int IHierarchyObj.ExpressionIndex
		{
			get
			{
				if (this.m_sortHierarchyStruct != null)
				{
					return this.m_sortHierarchyStruct.SortIndex;
				}
				return -1;
			}
		}

		List<int> IHierarchyObj.SortFilterInfoIndices
		{
			get
			{
				return this.m_hierarchyRoot.Value().SortFilterInfoIndices;
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
				return ItemSizes.SizeOf(this.m_hierarchyRoot) + ItemSizes.ReferenceSize + ItemSizes.SizeOf(this.m_sortHierarchyStruct) + ItemSizes.SizeOf(this.m_dataHolder) + ItemSizes.SizeOf(this.m_dataRowHolder);
			}
		}

		internal RuntimeSortHierarchyObj()
		{
		}

		internal RuntimeSortHierarchyObj(IHierarchyObj outerHierarchy, int depth)
		{
			this.m_hierarchyRoot = outerHierarchy.HierarchyRoot;
			this.m_odpContext = this.m_hierarchyRoot.Value().OdpContext;
			List<int> sortFilterInfoIndices = this.m_hierarchyRoot.Value().SortFilterInfoIndices;
			int num = outerHierarchy.ExpressionIndex + 1;
			if (sortFilterInfoIndices == null || num >= sortFilterInfoIndices.Count)
			{
				RuntimeDataTablixGroupRootObjReference runtimeDataTablixGroupRootObjReference = this.m_hierarchyRoot as RuntimeDataTablixGroupRootObjReference;
				if ((BaseReference)null != (object)runtimeDataTablixGroupRootObjReference)
				{
					using (runtimeDataTablixGroupRootObjReference.PinValue())
					{
						RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj = runtimeDataTablixGroupRootObjReference.Value();
						this.m_dataHolder = (IReference<ISortDataHolder>)runtimeDataTablixGroupRootObj.CreateGroupLeaf();
						if (!runtimeDataTablixGroupRootObj.HasParent)
						{
							runtimeDataTablixGroupRootObj.AddChildWithNoParent((RuntimeGroupLeafObjReference)this.m_dataHolder);
						}
					}
				}
				else
				{
					this.m_dataRowHolder = new RuntimeSortDataHolder();
				}
			}
			else
			{
				this.m_sortHierarchyStruct = new SortHierarchyStructure(this, num, this.m_odpContext.RuntimeSortFilterInfo, sortFilterInfoIndices);
			}
		}

		IHierarchyObj IHierarchyObj.CreateHierarchyObjForSortTree()
		{
			return new RuntimeSortHierarchyObj(this, this.Depth + 1);
		}

		ProcessingMessageList IHierarchyObj.RegisterComparisonError(string propertyName)
		{
			return this.m_odpContext.RegisterComparisonErrorForSortFilterEvent(propertyName);
		}

		void IHierarchyObj.NextRow(IHierarchyObj owner)
		{
			if (this.m_dataHolder != null)
			{
				using (this.m_dataHolder.PinValue())
				{
					this.m_dataHolder.Value().NextRow();
				}
			}
			else if (this.m_sortHierarchyStruct != null)
			{
				IReference<RuntimeSortFilterEventInfo> sortInfo = this.m_sortHierarchyStruct.SortInfo;
				object sortOrder = default(object);
				using (sortInfo.PinValue())
				{
					sortOrder = sortInfo.Value().GetSortOrder(this.m_odpContext.ReportRuntime);
				}
				this.m_sortHierarchyStruct.SortTree.NextRow(sortOrder, this);
			}
			else if (this.m_dataRowHolder != null)
			{
				this.m_dataRowHolder.NextRow(this.m_odpContext, this.Depth + 1);
			}
		}

		void IHierarchyObj.Traverse(ProcessingStages operation, ITraversalContext traversalContext)
		{
			if (this.m_sortHierarchyStruct != null)
			{
				bool ascending = true;
				RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = this.m_sortHierarchyStruct.SortInfo.Value();
				if (runtimeSortFilterEventInfo.EventSource.UserSort.SortExpressionScope == null)
				{
					ascending = runtimeSortFilterEventInfo.SortDirection;
				}
				this.m_sortHierarchyStruct.SortTree.Traverse(operation, ascending, traversalContext);
			}
			if (this.m_dataHolder != null)
			{
				using (this.m_dataHolder.PinValue())
				{
					this.m_dataHolder.Value().Traverse(operation, traversalContext);
				}
			}
			if (this.m_dataRowHolder != null)
			{
				using (this.m_hierarchyRoot.PinValue())
				{
					this.m_dataRowHolder.Traverse(operation, traversalContext, this.m_hierarchyRoot.Value());
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

		void IHierarchyObj.AddSortInfoIndex(int sortInfoIndex, IReference<RuntimeSortFilterEventInfo> sortInfo)
		{
			Global.Tracer.Assert(false);
		}

		void IPersistable.Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(RuntimeSortHierarchyObj.m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.HierarchyRoot:
					writer.Write(this.m_hierarchyRoot);
					break;
				case MemberName.OdpContext:
				{
					int value = scalabilityCache.StoreStaticReference(this.m_odpContext);
					writer.Write(value);
					break;
				}
				case MemberName.SortHierarchyStruct:
					writer.Write(this.m_sortHierarchyStruct);
					break;
				case MemberName.DataHolder:
					writer.Write(this.m_dataHolder);
					break;
				case MemberName.DataRowHolder:
					writer.Write(this.m_dataRowHolder);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		void IPersistable.Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(RuntimeSortHierarchyObj.m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.HierarchyRoot:
					this.m_hierarchyRoot = (IReference<IHierarchyObj>)reader.ReadRIFObject();
					break;
				case MemberName.OdpContext:
				{
					int id = reader.ReadInt32();
					this.m_odpContext = (OnDemandProcessingContext)scalabilityCache.FetchStaticReference(id);
					break;
				}
				case MemberName.SortHierarchyStruct:
					this.m_sortHierarchyStruct = (SortHierarchyStructure)reader.ReadRIFObject();
					break;
				case MemberName.DataHolder:
					this.m_dataHolder = (IReference<ISortDataHolder>)reader.ReadRIFObject();
					break;
				case MemberName.DataRowHolder:
					this.m_dataRowHolder = (RuntimeSortDataHolder)reader.ReadRIFObject();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortHierarchyObj;
		}

		internal static Declaration GetDeclaration()
		{
			if (RuntimeSortHierarchyObj.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.HierarchyRoot, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IHierarchyObjReference));
				list.Add(new MemberInfo(MemberName.OdpContext, Token.Int32));
				list.Add(new MemberInfo(MemberName.SortHierarchyStruct, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortHierarchyStruct));
				list.Add(new MemberInfo(MemberName.DataHolder, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ISortDataHolderReference));
				list.Add(new MemberInfo(MemberName.DataRowHolder, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortDataHolder));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortHierarchyObj, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return RuntimeSortHierarchyObj.m_declaration;
		}
	}
}
