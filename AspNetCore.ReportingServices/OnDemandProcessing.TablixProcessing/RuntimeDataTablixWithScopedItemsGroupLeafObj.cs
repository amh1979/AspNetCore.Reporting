using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal abstract class RuntimeDataTablixWithScopedItemsGroupLeafObj : RuntimeDataTablixGroupLeafObj
	{
		private RuntimeRICollection m_groupScopedItems;

		[NonSerialized]
		private static Declaration m_declaration = RuntimeDataTablixWithScopedItemsGroupLeafObj.GetDeclaration();

		public override int Size
		{
			get
			{
				return base.Size + ItemSizes.SizeOf(this.m_groupScopedItems);
			}
		}

		internal RuntimeDataTablixWithScopedItemsGroupLeafObj()
		{
		}

		internal RuntimeDataTablixWithScopedItemsGroupLeafObj(RuntimeDataTablixGroupRootObjReference groupRootRef, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType)
			: base(groupRootRef, objectType)
		{
		}

		protected abstract List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem> GetGroupScopedContents(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode member);

		protected abstract List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem> GetCellContents(Cell cell);

		protected abstract RuntimeCell CreateCell(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode outerGroupingMember, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode innerGroupingMember);

		protected override void InitializeGroupScopedItems(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode member, ref DataActions innerDataAction)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem> groupScopedContents = this.GetGroupScopedContents(member);
			if (groupScopedContents != null)
			{
				if (this.m_groupScopedItems == null)
				{
					this.m_groupScopedItems = new RuntimeRICollection(groupScopedContents.Count);
				}
				this.m_groupScopedItems.AddItems((IReference<IScope>)base.m_selfReference, groupScopedContents, ref innerDataAction, base.m_odpContext);
			}
		}

		protected override void ConstructOutermostCellContents(Cell cell)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem> cellContents = this.GetCellContents(cell);
			if (cellContents != null)
			{
				DataActions dataActions = DataActions.None;
				if (this.m_groupScopedItems == null)
				{
					this.m_groupScopedItems = new RuntimeRICollection(cellContents.Count);
				}
				this.m_groupScopedItems.AddItems(base.m_selfReference, cellContents, ref dataActions, base.m_odpContext);
			}
		}

		internal override void CreateCell(RuntimeCells cellsCollection, int collectionKey, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode outerGroupingMember, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode innerGroupingMember, AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef)
		{
			RuntimeCell runtimeCell = this.CreateCell(outerGroupingMember, innerGroupingMember);
			if ((BaseReference)runtimeCell.SelfReference == (object)null)
			{
				cellsCollection.AddCell(collectionKey, runtimeCell);
			}
			else
			{
				IReference<RuntimeCell> selfReference = runtimeCell.SelfReference;
				selfReference.UnPinValue();
				cellsCollection.AddCell(collectionKey, selfReference);
			}
		}

		protected override void SendToInner()
		{
			base.SendToInner();
			if (!base.IsOuterGrouping && base.m_odpContext.PeerOuterGroupProcessing)
			{
				return;
			}
			if (this.m_groupScopedItems != null)
			{
				this.m_groupScopedItems.FirstPassNextDataRow(base.m_odpContext);
			}
		}

		protected override void TraverseStaticContents(ProcessingStages operation, AggregateUpdateContext context)
		{
			if (this.m_groupScopedItems != null)
			{
				switch (operation)
				{
				case ProcessingStages.SortAndFilter:
					this.m_groupScopedItems.SortAndFilter(context);
					break;
				case ProcessingStages.UpdateAggregates:
					this.m_groupScopedItems.UpdateAggregates(context);
					break;
				default:
					Global.Tracer.Assert(false, "Unknown operation in TraverseStaticContents.");
					break;
				}
			}
		}

		protected override void CalculateRunningValuesForStaticContents(AggregateUpdateContext aggContext)
		{
			if (base.m_processHeading)
			{
				RuntimeDataTablixGroupRootObjReference runtimeDataTablixGroupRootObjReference = (RuntimeDataTablixGroupRootObjReference)base.m_hierarchyRoot;
				using (runtimeDataTablixGroupRootObjReference.PinValue())
				{
					RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj = runtimeDataTablixGroupRootObjReference.Value();
					Dictionary<string, IReference<RuntimeGroupRootObj>> groupCollection = runtimeDataTablixGroupRootObj.GroupCollection;
					RuntimeGroupRootObjReference lastGroup = runtimeDataTablixGroupRootObjReference;
					if (this.m_groupScopedItems != null)
					{
						this.m_groupScopedItems.CalculateRunningValues(groupCollection, lastGroup, aggContext);
					}
				}
			}
		}

		protected override void CreateInstanceHeadingContents()
		{
			if (base.MemberDef.InScopeEventSources != null)
			{
				UserSortFilterContext.ProcessEventSources(base.m_odpContext, this, base.MemberDef.InScopeEventSources);
			}
			if (this.m_groupScopedItems != null)
			{
				List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem> groupScopedContents = this.GetGroupScopedContents(base.MemberDef);
				if (groupScopedContents != null)
				{
					this.m_groupScopedItems.CreateInstances(base.m_memberInstance, base.m_odpContext, base.m_selfReference, groupScopedContents);
				}
			}
		}

		protected override void CreateOutermostStaticCellContents(Cell cell, DataCellInstance cellInstance)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem> cellContents = this.GetCellContents(cell);
			if (this.m_groupScopedItems != null && cellContents != null)
			{
				this.m_groupScopedItems.CreateInstances(cellInstance, base.m_odpContext, base.m_selfReference, cellContents);
			}
		}

		internal override RuntimeDataTablixObjReference GetNestedDataRegion(AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion rifDataRegion)
		{
			Global.Tracer.Assert(this.m_groupScopedItems != null, "Cannot find data region.");
			return this.m_groupScopedItems.GetDataRegionObj(rifDataRegion);
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixWithScopedItemsGroupLeafObj;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(RuntimeDataTablixWithScopedItemsGroupLeafObj.m_declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.GroupScopedItems)
				{
					writer.Write(this.m_groupScopedItems);
				}
				else
				{
					Global.Tracer.Assert(false, "Unsupported member name: " + writer.CurrentMember.MemberName + ".");
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(RuntimeDataTablixWithScopedItemsGroupLeafObj.m_declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.GroupScopedItems)
				{
					this.m_groupScopedItems = (RuntimeRICollection)reader.ReadRIFObject();
				}
				else
				{
					Global.Tracer.Assert(false, "Unsupported member name: " + reader.CurrentMember.MemberName + ".");
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public new static Declaration GetDeclaration()
		{
			if (RuntimeDataTablixWithScopedItemsGroupLeafObj.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.GroupScopedItems, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRICollection));
				RuntimeDataTablixWithScopedItemsGroupLeafObj.m_declaration = new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixWithScopedItemsGroupLeafObj, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupLeafObj, list);
			}
			return RuntimeDataTablixWithScopedItemsGroupLeafObj.m_declaration;
		}
	}
}
