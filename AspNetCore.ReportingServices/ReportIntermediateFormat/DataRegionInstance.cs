using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class DataRegionInstance : ScopeInstance, IMemberHierarchy
	{
		private int m_dataSetIndexInCollection = -1;

		private List<ScalableList<DataRegionMemberInstance>> m_rowMembers;

		private List<ScalableList<DataRegionMemberInstance>> m_columnMembers;

		private ScalableList<DataCellInstanceList> m_cells;

		[NonSerialized]
		private List<ScalableList<DataCellInstance>> m_upgradedSnapshotCells;

		[Reference]
		private DataRegion m_dataRegionDef;

		[NonSerialized]
		private static readonly Declaration m_Declaration = DataRegionInstance.GetDeclaration();

		internal override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegionInstance;
			}
		}

		internal override IRIFReportScope RIFReportScope
		{
			get
			{
				return this.m_dataRegionDef;
			}
		}

		internal DataRegion DataRegionDef
		{
			get
			{
				return this.m_dataRegionDef;
			}
		}

		internal bool NoRows
		{
			get
			{
				return base.m_firstRowOffset <= 0;
			}
		}

		internal int DataSetIndexInCollection
		{
			get
			{
				return this.m_dataSetIndexInCollection;
			}
		}

		internal List<ScalableList<DataRegionMemberInstance>> TopLevelRowMembers
		{
			get
			{
				return this.m_rowMembers;
			}
		}

		internal List<ScalableList<DataRegionMemberInstance>> TopLevelColumnMembers
		{
			get
			{
				return this.m_columnMembers;
			}
		}

		internal ScalableList<DataCellInstanceList> Cells
		{
			get
			{
				return this.m_cells;
			}
		}

		public override int Size
		{
			get
			{
				return base.Size + 4 + ItemSizes.SizeOf(this.m_rowMembers) + ItemSizes.SizeOf(this.m_columnMembers) + ItemSizes.SizeOf(this.m_cells) + ItemSizes.SizeOf(this.m_upgradedSnapshotCells) + ItemSizes.ReferenceSize;
			}
		}

		private DataRegionInstance(DataRegion dataRegionDef, int dataSetIndex)
		{
			this.m_dataRegionDef = dataRegionDef;
			this.m_dataSetIndexInCollection = dataSetIndex;
		}

		internal DataRegionInstance()
		{
		}

		internal static IReference<DataRegionInstance> CreateInstance(ScopeInstance parentInstance, OnDemandMetadata odpMetadata, DataRegion dataRegionDef, int dataSetIndex)
		{
			DataRegionInstance dataRegionInstance = new DataRegionInstance(dataRegionDef, dataSetIndex);
			GroupTreeScalabilityCache groupTreeScalabilityCache = odpMetadata.GroupTreeScalabilityCache;
			IReference<DataRegionInstance> reference;
			if (parentInstance.ObjectType == AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportInstance)
			{
				ReportInstance reportInstance = (ReportInstance)parentInstance;
				reference = reportInstance.GetTopLevelDataRegionReference(dataRegionDef.IndexInCollection);
				groupTreeScalabilityCache.SetTreePartitionContentsAndPin(reference, dataRegionInstance);
			}
			else
			{
				reference = groupTreeScalabilityCache.AllocateAndPin(dataRegionInstance, 0);
				parentInstance.AddChildScope((IReference<ScopeInstance>)reference, dataRegionDef.IndexInCollection);
			}
			dataRegionInstance.m_cleanupRef = (IDisposable)reference;
			return reference;
		}

		internal override void InstanceComplete()
		{
			if (this.m_cells != null)
			{
				this.m_cells.UnPinAll();
			}
			base.UnPinList(this.m_rowMembers);
			base.UnPinList(this.m_columnMembers);
			base.InstanceComplete();
		}

		IDisposable IMemberHierarchy.AddMemberInstance(DataRegionMemberInstance instance, int indexInCollection, IScalabilityCache cache, out int instanceIndex)
		{
			List<ScalableList<DataRegionMemberInstance>> list = instance.MemberDef.IsColumn ? this.m_columnMembers : this.m_rowMembers;
			bool flag = false;
			if (list == null)
			{
				flag = true;
				list = new List<ScalableList<DataRegionMemberInstance>>();
				if (instance.MemberDef.IsColumn)
				{
					this.m_columnMembers = list;
				}
				else
				{
					this.m_rowMembers = list;
				}
			}
			ListUtils.AdjustLength(list, indexInCollection);
			ScalableList<DataRegionMemberInstance> scalableList = list[indexInCollection];
			if (flag || scalableList == null)
			{
				scalableList = (list[indexInCollection] = new ScalableList<DataRegionMemberInstance>(0, cache, 100, 5));
			}
			instanceIndex = scalableList.Count;
			return scalableList.AddAndPin(instance);
		}

		IDisposable IMemberHierarchy.AddCellInstance(int columnMemberSequenceId, int cellIndexInCollection, DataCellInstance cellInstance, IScalabilityCache cache)
		{
			if (this.m_cells == null)
			{
				this.m_cells = new ScalableList<DataCellInstanceList>(0, cache, 100, 5, true);
			}
			return DataRegionInstance.AddCellInstance(this.m_cells, columnMemberSequenceId, cellIndexInCollection, cellInstance, cache);
		}

		internal static IDisposable AddCellInstance(ScalableList<DataCellInstanceList> cells, int columnMemberSequenceId, int cellIndexInCollection, DataCellInstance cellInstance, IScalabilityCache cache)
		{
			ScopeInstance.AdjustLength(cells, columnMemberSequenceId);
			DataCellInstanceList dataCellInstanceList = default(DataCellInstanceList);
			IDisposable andPin = cells.GetAndPin(columnMemberSequenceId, out dataCellInstanceList);
			if (dataCellInstanceList == null)
			{
				dataCellInstanceList = (cells[columnMemberSequenceId] = new DataCellInstanceList());
			}
			ListUtils.AdjustLength(dataCellInstanceList, cellIndexInCollection);
			((List<DataCellInstance>)dataCellInstanceList)[cellIndexInCollection] = cellInstance;
			return andPin;
		}

		internal void SetupEnvironment(OnDemandProcessingContext odpContext)
		{
			base.SetupFields(odpContext, this.m_dataSetIndexInCollection);
			int num = 0;
			base.SetupAggregates(odpContext, this.m_dataRegionDef.Aggregates, ref num);
			base.SetupAggregates(odpContext, this.m_dataRegionDef.PostSortAggregates, ref num);
			base.SetupAggregates(odpContext, this.m_dataRegionDef.RunningValues, ref num);
			if (this.m_dataRegionDef.DataScopeInfo != null)
			{
				DataScopeInfo dataScopeInfo = this.m_dataRegionDef.DataScopeInfo;
				base.SetupAggregates(odpContext, dataScopeInfo.AggregatesOfAggregates, ref num);
				base.SetupAggregates(odpContext, dataScopeInfo.PostSortAggregatesOfAggregates, ref num);
				base.SetupAggregates(odpContext, dataScopeInfo.RunningValuesOfAggregates, ref num);
			}
		}

		IList<DataRegionMemberInstance> IMemberHierarchy.GetChildMemberInstances(bool isRowMember, int memberIndexInCollection)
		{
			return ScopeInstance.GetChildMemberInstances(isRowMember ? this.m_rowMembers : this.m_columnMembers, memberIndexInCollection);
		}

		IList<DataCellInstance> IMemberHierarchy.GetCellInstances(int columnMemberSequenceId)
		{
			if (this.m_cells != null && columnMemberSequenceId < this.m_cells.Count)
			{
				return this.m_cells[columnMemberSequenceId];
			}
			if (this.m_upgradedSnapshotCells != null && columnMemberSequenceId < this.m_upgradedSnapshotCells.Count)
			{
				return this.m_upgradedSnapshotCells[columnMemberSequenceId];
			}
			return null;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ID, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegion, Token.GlobalReference));
			list.Add(new MemberInfo(MemberName.DataSetIndexInCollection, Token.Int32));
			list.Add(new MemberInfo(MemberName.RowMembers, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList));
			list.Add(new MemberInfo(MemberName.ColumnMembers, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList));
			list.Add(new ReadOnlyMemberInfo(MemberName.Cells, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList));
			list.Add(new MemberInfo(MemberName.Cells2, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataCellInstanceList));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegionInstance, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScopeInstance, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(DataRegionInstance.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ID:
					Global.Tracer.Assert(null != this.m_dataRegionDef, "(null != m_dataRegionDef)");
					writer.WriteGlobalReference(this.m_dataRegionDef);
					break;
				case MemberName.DataSetIndexInCollection:
					writer.Write7BitEncodedInt(this.m_dataSetIndexInCollection);
					break;
				case MemberName.RowMembers:
					writer.Write(this.m_rowMembers);
					break;
				case MemberName.ColumnMembers:
					writer.Write(this.m_columnMembers);
					break;
				case MemberName.Cells2:
					writer.Write(this.m_cells);
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
			reader.RegisterDeclaration(DataRegionInstance.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ID:
					this.m_dataRegionDef = reader.ReadGlobalReference<DataRegion>();
					break;
				case MemberName.DataSetIndexInCollection:
					this.m_dataSetIndexInCollection = reader.Read7BitEncodedInt();
					break;
				case MemberName.RowMembers:
					this.m_rowMembers = reader.ReadGenericListOfRIFObjectsUsingNew<ScalableList<DataRegionMemberInstance>>();
					base.SetReadOnlyList(this.m_rowMembers);
					break;
				case MemberName.ColumnMembers:
					this.m_columnMembers = reader.ReadGenericListOfRIFObjectsUsingNew<ScalableList<DataRegionMemberInstance>>();
					base.SetReadOnlyList(this.m_columnMembers);
					break;
				case MemberName.Cells2:
					this.m_cells = reader.ReadRIFObject<ScalableList<DataCellInstanceList>>();
					break;
				case MemberName.Cells:
					this.m_upgradedSnapshotCells = reader.ReadGenericListOfRIFObjectsUsingNew<ScalableList<DataCellInstance>>();
					base.SetReadOnlyList(this.m_upgradedSnapshotCells);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegionInstance;
		}
	}
}
