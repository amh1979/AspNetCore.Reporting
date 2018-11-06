using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal abstract class RuntimeCellWithContents : RuntimeCell
	{
		private RuntimeRICollection m_cellContents;

		[NonSerialized]
		private static Declaration m_declaration = RuntimeCellWithContents.GetDeclaration();

		public override int Size
		{
			get
			{
				return base.Size + ItemSizes.SizeOf(this.m_cellContents);
			}
		}

		internal RuntimeCellWithContents()
		{
		}

		internal RuntimeCellWithContents(RuntimeDataTablixGroupLeafObjReference owner, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode outerGroupingMember, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode innerGroupingMember, bool innermost)
			: base(owner, outerGroupingMember, innerGroupingMember, innermost)
		{
		}

		protected abstract List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem> GetCellContents(Cell cell);

		protected override void ConstructCellContents(Cell cell, ref DataActions dataAction)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem> cellContents = this.GetCellContents(cell);
			if (cellContents != null)
			{
				base.m_owner.Value().OdpContext.TablixProcessingScalabilityCache.AllocateAndPin((RuntimeCell)this, base.m_outerGroupDynamicIndex);
				if (this.m_cellContents == null)
				{
					this.m_cellContents = new RuntimeRICollection(cellContents.Count);
				}
				this.m_cellContents.AddItems((IReference<IScope>)base.m_selfReference, cellContents, ref dataAction, base.m_owner.Value().OdpContext);
			}
		}

		internal override bool NextRow()
		{
			bool result = base.NextRow();
			if (this.m_cellContents != null)
			{
				OnDemandProcessingContext odpContext = base.m_owner.Value().OdpContext;
				this.m_cellContents.FirstPassNextDataRow(odpContext);
			}
			return result;
		}

		protected override void TraverseCellContents(ProcessingStages operation, AggregateUpdateContext context)
		{
			if (this.m_cellContents != null)
			{
				switch (operation)
				{
				case ProcessingStages.SortAndFilter:
					this.m_cellContents.SortAndFilter(context);
					break;
				case ProcessingStages.UpdateAggregates:
					this.m_cellContents.UpdateAggregates(context);
					break;
				default:
					Global.Tracer.Assert(false, "Invalid operation for TraverseCellContents.");
					break;
				}
			}
		}

		protected override void CalculateInnerRunningValues(Dictionary<string, IReference<RuntimeGroupRootObj>> groupCol, IReference<RuntimeGroupRootObj> lastGroup, AggregateUpdateContext aggContext)
		{
			if (this.m_cellContents != null)
			{
				this.m_cellContents.CalculateRunningValues(groupCol, lastGroup, aggContext);
			}
		}

		protected override void CreateInstanceCellContents(Cell cell, DataCellInstance cellInstance, OnDemandProcessingContext odpContext)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem> cellContents = this.GetCellContents(cell);
			if (cellContents != null && this.m_cellContents != null)
			{
				this.m_cellContents.CreateInstances(cellInstance, odpContext, base.m_selfReference, cellContents);
			}
		}

		public override IOnDemandMemberOwnerInstanceReference GetDataRegionInstance(AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion rifDataRegion)
		{
			Global.Tracer.Assert(this.m_cellContents != null, "Cannot find data region.");
			return this.m_cellContents.GetDataRegionObj(rifDataRegion);
		}

		public override IReference<IDataCorrelation> GetIdcReceiver(IRIFReportDataScope scope)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion = scope as AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion;
			Global.Tracer.Assert(dataRegion != null, "Invalid scope.");
			return this.m_cellContents.GetDataRegionObj(dataRegion);
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCellWithContents;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(RuntimeCellWithContents.m_declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.CellContents)
				{
					writer.Write(this.m_cellContents);
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
			reader.RegisterDeclaration(RuntimeCellWithContents.m_declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.CellContents)
				{
					this.m_cellContents = (RuntimeRICollection)reader.ReadRIFObject();
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

		internal new static Declaration GetDeclaration()
		{
			if (RuntimeCellWithContents.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.CellContents, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRICollection));
				RuntimeCellWithContents.m_declaration = new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCellWithContents, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCell, list);
			}
			return RuntimeCellWithContents.m_declaration;
		}
	}
}
