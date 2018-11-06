using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal abstract class RuntimeDataTablixWithScopedItemsObj : RuntimeDataTablixObj, IStorable, IPersistable
	{
		private RuntimeRICollection m_dataRegionScopedItems;

		[NonSerialized]
		private static Declaration m_declaration = RuntimeDataTablixWithScopedItemsObj.GetDeclaration();

		public override int Size
		{
			get
			{
				return base.Size + ItemSizes.SizeOf(this.m_dataRegionScopedItems);
			}
		}

		internal RuntimeDataTablixWithScopedItemsObj()
		{
		}

		internal RuntimeDataTablixWithScopedItemsObj(IReference<IScope> outerScope, AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataTablixDef, ref DataActions dataAction, OnDemandProcessingContext odpContext, bool onePassProcess, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType)
			: base(outerScope, dataTablixDef, ref dataAction, odpContext, onePassProcess, objectType)
		{
		}

		protected abstract List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem> GetDataRegionScopedItems();

		protected override void ConstructRuntimeStructure(ref DataActions innerDataAction, bool onePassProcess)
		{
			this.m_dataRegionScopedItems = null;
			base.ConstructRuntimeStructure(ref innerDataAction, onePassProcess);
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem> dataRegionScopedItems = this.GetDataRegionScopedItems();
			if (dataRegionScopedItems != null)
			{
				this.m_dataRegionScopedItems = new RuntimeRICollection((IReference<IScope>)base.m_selfReference, dataRegionScopedItems, ref innerDataAction, base.m_odpContext);
			}
		}

		internal override RuntimeDataTablixObjReference GetNestedDataRegion(AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion rifDataRegion)
		{
			Global.Tracer.Assert(this.m_dataRegionScopedItems != null, "Cannot find data region.");
			return this.m_dataRegionScopedItems.GetDataRegionObj(rifDataRegion);
		}

		protected override void SendToInner()
		{
			base.SendToInner();
			if (this.m_dataRegionScopedItems != null)
			{
				this.m_dataRegionScopedItems.FirstPassNextDataRow(base.m_odpContext);
			}
		}

		protected override void CalculateRunningValuesForTopLevelStaticContents(Dictionary<string, IReference<RuntimeGroupRootObj>> groupCol, IReference<RuntimeGroupRootObj> lastGroup, AggregateUpdateContext aggContext)
		{
			if (this.m_dataRegionScopedItems != null)
			{
				this.m_dataRegionScopedItems.CalculateRunningValues(groupCol, lastGroup, aggContext);
			}
		}

		protected override void Traverse(ProcessingStages operation, ITraversalContext context)
		{
			base.Traverse(operation, context);
			if (this.m_dataRegionScopedItems != null)
			{
				this.TraverseDataRegionScopedItems(operation, context);
			}
		}

		private void TraverseDataRegionScopedItems(ProcessingStages operation, ITraversalContext context)
		{
			switch (operation)
			{
			case ProcessingStages.SortAndFilter:
				this.m_dataRegionScopedItems.SortAndFilter((AggregateUpdateContext)context);
				break;
			case ProcessingStages.UpdateAggregates:
				this.m_dataRegionScopedItems.UpdateAggregates((AggregateUpdateContext)context);
				break;
			default:
				Global.Tracer.Assert(false, "Unknown ProcessingStage for TraverseDataRegionScopedItems");
				break;
			}
		}

		protected override void CreateDataRegionScopedInstance(DataRegionInstance dataRegionInstance)
		{
			base.CreateDataRegionScopedInstance(dataRegionInstance);
			if (this.m_dataRegionScopedItems != null)
			{
				this.m_dataRegionScopedItems.CreateInstances(dataRegionInstance, base.m_odpContext, base.m_selfReference, this.GetDataRegionScopedItems());
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixWithScopedItemsObj;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(RuntimeDataTablixWithScopedItemsObj.m_declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.DataRegionScopedItems)
				{
					writer.Write(this.m_dataRegionScopedItems);
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
			reader.RegisterDeclaration(RuntimeDataTablixWithScopedItemsObj.m_declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.DataRegionScopedItems)
				{
					this.m_dataRegionScopedItems = (RuntimeRICollection)reader.ReadRIFObject();
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
			if (RuntimeDataTablixWithScopedItemsObj.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.DataRegionScopedItems, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRICollection));
				RuntimeDataTablixWithScopedItemsObj.m_declaration = new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixWithScopedItemsObj, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixObj, list);
			}
			return RuntimeDataTablixWithScopedItemsObj.m_declaration;
		}
	}
}
