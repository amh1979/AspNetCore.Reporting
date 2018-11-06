using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeCells : IStorable, IPersistable
	{
		private int m_firstCellKey = -1;

		private int m_lastCellKey = -1;

		private ScalableList<IStorable> m_collection;

		private static Declaration m_declaration = RuntimeCells.GetDeclaration();

		public int Count
		{
			get
			{
				return this.m_collection.Count;
			}
		}

		public int Size
		{
			get
			{
				return 8 + ItemSizes.SizeOf(this.m_collection);
			}
		}

		internal RuntimeCells()
		{
		}

		internal RuntimeCells(int priority, IScalabilityCache cache)
		{
			this.m_collection = new ScalableList<IStorable>(priority, cache, 200, 10);
		}

		internal void AddCell(int key, RuntimeCell cell)
		{
			this.InternalAdd(key, cell);
		}

		internal void AddCell(int key, IReference<RuntimeCell> cellRef)
		{
			this.InternalAdd(key, cellRef);
		}

		private void InternalAdd(int key, IStorable cell)
		{
			if (this.Count == 0)
			{
				this.m_firstCellKey = key;
			}
			else
			{
				IDisposable disposable = default(IDisposable);
				RuntimeCell andPinCell = this.GetAndPinCell(this.m_lastCellKey, out disposable);
				andPinCell.NextCell = key;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			this.m_lastCellKey = key;
			this.m_collection.SetValueWithExtension(key, cell);
		}

		internal RuntimeCell GetCell(int key, out RuntimeCellReference cellRef)
		{
			RuntimeCell result = null;
			cellRef = null;
			if (key < this.m_collection.Count)
			{
				IStorable storable = this.m_collection[key];
				if (storable != null)
				{
					if (this.IsCellReference(storable))
					{
						cellRef = (RuntimeCellReference)storable;
						result = cellRef.Value();
					}
					else
					{
						result = (RuntimeCell)storable;
					}
				}
			}
			return result;
		}

		internal RuntimeCell GetAndPinCell(int key, out IDisposable cleanupRef)
		{
			cleanupRef = null;
			IStorable storable = default(IStorable);
			bool flag;
			if (key < this.m_collection.Count)
			{
				cleanupRef = this.m_collection.GetAndPin(key, out storable);
				flag = (storable != null);
			}
			else
			{
				storable = null;
				flag = false;
			}
			if (flag)
			{
				if (this.IsCellReference(storable))
				{
					if (cleanupRef != null)
					{
						cleanupRef.Dispose();
					}
					IReference<RuntimeCell> reference = (IReference<RuntimeCell>)storable;
					reference.PinValue();
					cleanupRef = (IDisposable)reference;
					return reference.Value();
				}
				return (RuntimeCell)storable;
			}
			return null;
		}

		private bool IsCellReference(IStorable cellOrReference)
		{
			switch (cellOrReference.GetObjectType())
			{
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixCellReference:
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriCellReference:
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCellReference:
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataShapeIntersectionReference:
				return true;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixCell:
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriCell:
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCell:
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataShapeIntersection:
				return false;
			default:
				Global.Tracer.Assert(false, "Unexpected type in RuntimeCells collection");
				throw new InvalidOperationException();
			}
		}

		internal RuntimeCell GetOrCreateCell(AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef, IReference<RuntimeDataTablixGroupLeafObj> ownerRef, IReference<RuntimeDataTablixGroupRootObj> currOuterGroupRootRef, out IDisposable cleanupRef)
		{
			RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj = currOuterGroupRootRef.Value();
			int groupLeafIndex = dataRegionDef.OuterGroupingIndexes[runtimeDataTablixGroupRootObj.HierarchyDef.HierarchyDynamicIndex];
			return this.GetOrCreateCellByIndex(groupLeafIndex, dataRegionDef, ownerRef, runtimeDataTablixGroupRootObj, out cleanupRef);
		}

		internal RuntimeCell GetOrCreateCell(AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef, IReference<RuntimeDataTablixGroupLeafObj> ownerRef, IReference<RuntimeDataTablixGroupRootObj> currOuterGroupRootRef, int groupLeafIndex, out IDisposable cleanupRef)
		{
			RuntimeDataTablixGroupRootObj currOuterGroupRoot = currOuterGroupRootRef.Value();
			return this.GetOrCreateCellByIndex(groupLeafIndex, dataRegionDef, ownerRef, currOuterGroupRoot, out cleanupRef);
		}

		private RuntimeCell GetOrCreateCellByIndex(int groupLeafIndex, AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef, IReference<RuntimeDataTablixGroupLeafObj> ownerRef, RuntimeDataTablixGroupRootObj currOuterGroupRoot, out IDisposable cleanupRef)
		{
			RuntimeCell andPinCell = this.GetAndPinCell(groupLeafIndex, out cleanupRef);
			if (andPinCell == null)
			{
				using (ownerRef.PinValue())
				{
					RuntimeDataTablixGroupLeafObj runtimeDataTablixGroupLeafObj = ownerRef.Value();
					if (!RuntimeCell.HasOnlySimpleGroupTreeCells(currOuterGroupRoot.HierarchyDef, runtimeDataTablixGroupLeafObj.MemberDef, dataRegionDef))
					{
						runtimeDataTablixGroupLeafObj.CreateCell(this, groupLeafIndex, currOuterGroupRoot.HierarchyDef, runtimeDataTablixGroupLeafObj.MemberDef, dataRegionDef);
					}
				}
				andPinCell = this.GetAndPinCell(groupLeafIndex, out cleanupRef);
			}
			return andPinCell;
		}

		internal void SortAndFilter(AggregateUpdateContext aggContext)
		{
			this.Traverse(ProcessingStages.SortAndFilter, aggContext);
		}

		private void Traverse(ProcessingStages operation, AggregateUpdateContext context)
		{
			if (this.Count != 0)
			{
				int num = this.m_firstCellKey;
				int num2;
				do
				{
					num2 = num;
					IDisposable disposable = default(IDisposable);
					RuntimeCell andPinCell = this.GetAndPinCell(num2, out disposable);
					switch (operation)
					{
					case ProcessingStages.SortAndFilter:
						andPinCell.SortAndFilter(context);
						break;
					case ProcessingStages.UpdateAggregates:
						andPinCell.UpdateAggregates(context);
						break;
					default:
						Global.Tracer.Assert(false, "Unknown operation in Traverse");
						break;
					}
					num = andPinCell.NextCell;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
				while (num2 != this.m_lastCellKey);
			}
		}

		internal void UpdateAggregates(AggregateUpdateContext aggContext)
		{
			this.Traverse(ProcessingStages.UpdateAggregates, aggContext);
		}

		internal void CalculateRunningValues(AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef, Dictionary<string, IReference<RuntimeGroupRootObj>> groupCol, IReference<RuntimeGroupRootObj> lastGroup, IReference<RuntimeDataTablixGroupLeafObj> owner, AggregateUpdateContext aggContext)
		{
			IDisposable disposable = default(IDisposable);
			RuntimeCell orCreateCell = this.GetOrCreateCell(dataRegionDef, owner, dataRegionDef.CurrentOuterGroupRoot, out disposable);
			if (orCreateCell != null)
			{
				orCreateCell.CalculateRunningValues(groupCol, lastGroup, aggContext);
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(RuntimeCells.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.FirstCell:
					writer.Write(this.m_firstCellKey);
					break;
				case MemberName.LastCell:
					writer.Write(this.m_lastCellKey);
					break;
				case MemberName.Collection:
					writer.Write(this.m_collection);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(RuntimeCells.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.FirstCell:
					this.m_firstCellKey = reader.ReadInt32();
					break;
				case MemberName.LastCell:
					this.m_lastCellKey = reader.ReadInt32();
					break;
				case MemberName.Collection:
					this.m_collection = reader.ReadRIFObject<ScalableList<IStorable>>();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCells;
		}

		public static Declaration GetDeclaration()
		{
			if (RuntimeCells.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.FirstCell, Token.Int32));
				list.Add(new MemberInfo(MemberName.LastCell, Token.Int32));
				list.Add(new MemberInfo(MemberName.Collection, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCells, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return RuntimeCells.m_declaration;
		}
	}
}
