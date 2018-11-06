using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class DataRegionMemberInstance : ScopeInstance, IMemberHierarchy
	{
		private int m_memberInstanceIndexWithinScopeLevel = -1;

		private List<ScalableList<DataRegionMemberInstance>> m_children;

		private ScalableList<DataCellInstanceList> m_cells;

		[NonSerialized]
		private List<ScalableList<DataCellInstance>> m_upgradedSnapshotCells;

		private object[] m_variables;

		private int m_recursiveLevel;

		private object[] m_groupExprValues;

		private int m_parentInstanceIndex = -1;

		private bool? m_hasRecursiveChildren = null;

		[Reference]
		private ReportHierarchyNode m_memberDef;

		[NonSerialized]
		private static readonly Declaration m_Declaration = DataRegionMemberInstance.GetDeclaration();

		internal override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegionMemberInstance;
			}
		}

		internal override IRIFReportScope RIFReportScope
		{
			get
			{
				return this.m_memberDef;
			}
		}

		internal int MemberInstanceIndexWithinScopeLevel
		{
			get
			{
				return this.m_memberInstanceIndexWithinScopeLevel;
			}
		}

		internal int RecursiveLevel
		{
			get
			{
				return this.m_recursiveLevel;
			}
		}

		internal object[] GroupVariables
		{
			get
			{
				return this.m_variables;
			}
		}

		internal object[] GroupExprValues
		{
			get
			{
				return this.m_groupExprValues;
			}
		}

		internal List<ScalableList<DataRegionMemberInstance>> Children
		{
			get
			{
				return this.m_children;
			}
		}

		internal ScalableList<DataCellInstanceList> Cells
		{
			get
			{
				return this.m_cells;
			}
		}

		internal ReportHierarchyNode MemberDef
		{
			get
			{
				return this.m_memberDef;
			}
		}

		internal int RecursiveParentIndex
		{
			get
			{
				return this.m_parentInstanceIndex;
			}
			set
			{
				this.m_parentInstanceIndex = value;
			}
		}

		internal bool? HasRecursiveChildren
		{
			get
			{
				return this.m_hasRecursiveChildren;
			}
			set
			{
				this.m_hasRecursiveChildren = value;
			}
		}

		public override int Size
		{
			get
			{
				return base.Size + ItemSizes.SizeOf(this.m_children) + ItemSizes.SizeOf(this.m_cells) + ItemSizes.SizeOf(this.m_upgradedSnapshotCells) + ItemSizes.SizeOf(this.m_variables) + ItemSizes.SizeOf(this.m_recursiveLevel) + ItemSizes.SizeOf(this.m_groupExprValues) + 4 + ItemSizes.NullableBoolSize + ItemSizes.ReferenceSize;
			}
		}

		private DataRegionMemberInstance(OnDemandProcessingContext odpContext, ReportHierarchyNode memberDef, long firstRowOffset, int memberInstanceIndexWithinScopeLevel, int recursiveLevel, List<object> groupExpressionValues, object[] groupVariableValues)
			: base(firstRowOffset)
		{
			this.m_memberDef = memberDef;
			this.m_memberInstanceIndexWithinScopeLevel = memberInstanceIndexWithinScopeLevel;
			this.m_recursiveLevel = recursiveLevel;
			if (groupExpressionValues != null && groupExpressionValues.Count != 0)
			{
				this.m_groupExprValues = new object[groupExpressionValues.Count];
				for (int i = 0; i < this.m_groupExprValues.Length; i++)
				{
					object obj = groupExpressionValues[i];
					if (obj == DBNull.Value)
					{
						obj = null;
					}
					this.m_groupExprValues[i] = obj;
				}
			}
			base.StoreAggregates(odpContext, memberDef.Grouping.Aggregates);
			base.StoreAggregates(odpContext, memberDef.Grouping.RecursiveAggregates);
			base.StoreAggregates(odpContext, memberDef.Grouping.PostSortAggregates);
			base.StoreAggregates(odpContext, memberDef.RunningValues);
			if (memberDef.DataScopeInfo != null)
			{
				DataScopeInfo dataScopeInfo = memberDef.DataScopeInfo;
				base.StoreAggregates(odpContext, dataScopeInfo.AggregatesOfAggregates);
				base.StoreAggregates(odpContext, dataScopeInfo.PostSortAggregatesOfAggregates);
				base.StoreAggregates(odpContext, dataScopeInfo.RunningValuesOfAggregates);
			}
			this.m_variables = groupVariableValues;
		}

		internal DataRegionMemberInstance()
		{
		}

		internal static DataRegionMemberInstance CreateInstance(IMemberHierarchy parentInstance, OnDemandProcessingContext odpContext, ReportHierarchyNode memberDef, long firstRowOffset, int memberInstanceIndexWithinScopeLevel, int recursiveLevel, List<object> groupExpressionValues, object[] groupVariableValues, out int instanceIndex)
		{
			DataRegionMemberInstance dataRegionMemberInstance = new DataRegionMemberInstance(odpContext, memberDef, firstRowOffset, memberInstanceIndexWithinScopeLevel, recursiveLevel, groupExpressionValues, groupVariableValues);
			dataRegionMemberInstance.m_cleanupRef = parentInstance.AddMemberInstance(dataRegionMemberInstance, memberDef.IndexInCollection, (IScalabilityCache)odpContext.OdpMetadata.GroupTreeScalabilityCache, out instanceIndex);
			return dataRegionMemberInstance;
		}

		internal override void InstanceComplete()
		{
			if (this.m_cells != null)
			{
				this.m_cells.UnPinAll();
			}
			base.UnPinList(this.m_children);
			base.InstanceComplete();
		}

		IDisposable IMemberHierarchy.AddMemberInstance(DataRegionMemberInstance instance, int indexInCollection, IScalabilityCache cache, out int instanceIndex)
		{
			bool flag = false;
			if (this.m_children == null)
			{
				this.m_children = new List<ScalableList<DataRegionMemberInstance>>();
				flag = true;
			}
			ListUtils.AdjustLength(this.m_children, indexInCollection);
			ScalableList<DataRegionMemberInstance> scalableList = this.m_children[indexInCollection];
			if (flag || scalableList == null)
			{
				scalableList = new ScalableList<DataRegionMemberInstance>(0, cache, 100, 5);
				this.m_children[indexInCollection] = scalableList;
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

		internal void SetupEnvironment(OnDemandProcessingContext odpContext, int dataSetIndex)
		{
			base.SetupFields(odpContext, dataSetIndex);
			int num = 0;
			base.SetupAggregates(odpContext, this.m_memberDef.Grouping.Aggregates, ref num);
			base.SetupAggregates(odpContext, this.m_memberDef.Grouping.RecursiveAggregates, ref num);
			base.SetupAggregates(odpContext, this.m_memberDef.Grouping.PostSortAggregates, ref num);
			base.SetupAggregates(odpContext, this.m_memberDef.RunningValues, ref num);
			if (this.m_memberDef.DataScopeInfo != null)
			{
				DataScopeInfo dataScopeInfo = this.m_memberDef.DataScopeInfo;
				base.SetupAggregates(odpContext, dataScopeInfo.AggregatesOfAggregates, ref num);
				base.SetupAggregates(odpContext, dataScopeInfo.PostSortAggregatesOfAggregates, ref num);
				base.SetupAggregates(odpContext, dataScopeInfo.RunningValuesOfAggregates, ref num);
			}
			if (this.m_variables != null)
			{
				ScopeInstance.SetupVariables(odpContext, this.m_memberDef.Grouping.Variables, this.m_variables);
			}
		}

		IList<DataRegionMemberInstance> IMemberHierarchy.GetChildMemberInstances(bool isRowMember, int memberIndexInCollection)
		{
			return ScopeInstance.GetChildMemberInstances(this.m_children, memberIndexInCollection);
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
			list.Add(new MemberInfo(MemberName.ID, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportHierarchyNode, Token.GlobalReference));
			list.Add(new MemberInfo(MemberName.MemberInstanceIndexWithinScopeLevel, Token.Int32));
			list.Add(new MemberInfo(MemberName.Children, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList));
			list.Add(new ReadOnlyMemberInfo(MemberName.Cells, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList));
			list.Add(new ReadOnlyMemberInfo(MemberName.Variables, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, Token.Object));
			list.Add(new MemberInfo(MemberName.SerializableVariables, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SerializableArray, Token.Serializable));
			list.Add(new MemberInfo(MemberName.RecursiveLevel, Token.Int32));
			list.Add(new MemberInfo(MemberName.GroupExpressionValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, Token.Object));
			list.Add(new MemberInfo(MemberName.ParentInstanceIndex, Token.Int32));
			list.Add(new MemberInfo(MemberName.HasRecursiveChildren, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Nullable, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Cells2, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataCellInstanceList));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegionMemberInstance, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScopeInstance, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(DataRegionMemberInstance.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ID:
					Global.Tracer.Assert(null != this.m_memberDef, "(null != m_memberDef)");
					writer.WriteGlobalReference(this.m_memberDef);
					break;
				case MemberName.MemberInstanceIndexWithinScopeLevel:
					writer.Write7BitEncodedInt(this.m_memberInstanceIndexWithinScopeLevel);
					break;
				case MemberName.Children:
					writer.Write(this.m_children);
					break;
				case MemberName.Cells2:
					writer.Write(this.m_cells);
					break;
				case MemberName.SerializableVariables:
					writer.WriteSerializableArray(this.m_variables);
					break;
				case MemberName.RecursiveLevel:
					writer.Write7BitEncodedInt(this.m_recursiveLevel);
					break;
				case MemberName.GroupExpressionValues:
					writer.Write(this.m_groupExprValues);
					break;
				case MemberName.ParentInstanceIndex:
					writer.Write(this.m_parentInstanceIndex);
					break;
				case MemberName.HasRecursiveChildren:
					writer.Write(this.m_hasRecursiveChildren);
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
			reader.RegisterDeclaration(DataRegionMemberInstance.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ID:
					this.m_memberDef = reader.ReadGlobalReference<ReportHierarchyNode>();
					break;
				case MemberName.MemberInstanceIndexWithinScopeLevel:
					this.m_memberInstanceIndexWithinScopeLevel = reader.Read7BitEncodedInt();
					break;
				case MemberName.Children:
					this.m_children = reader.ReadGenericListOfRIFObjectsUsingNew<ScalableList<DataRegionMemberInstance>>();
					base.SetReadOnlyList(this.m_children);
					break;
				case MemberName.Cells2:
					this.m_cells = reader.ReadRIFObject<ScalableList<DataCellInstanceList>>();
					break;
				case MemberName.Cells:
					this.m_upgradedSnapshotCells = reader.ReadGenericListOfRIFObjectsUsingNew<ScalableList<DataCellInstance>>();
					base.SetReadOnlyList(this.m_upgradedSnapshotCells);
					break;
				case MemberName.Variables:
					this.m_variables = reader.ReadVariantArray();
					break;
				case MemberName.SerializableVariables:
					this.m_variables = reader.ReadSerializableArray();
					break;
				case MemberName.RecursiveLevel:
					this.m_recursiveLevel = reader.Read7BitEncodedInt();
					break;
				case MemberName.GroupExpressionValues:
					this.m_groupExprValues = reader.ReadVariantArray();
					break;
				case MemberName.ParentInstanceIndex:
					this.m_parentInstanceIndex = reader.ReadInt32();
					break;
				case MemberName.HasRecursiveChildren:
				{
					object obj = reader.ReadVariant();
					if (obj != null)
					{
						this.m_hasRecursiveChildren = (bool)obj;
					}
					break;
				}
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegionMemberInstance;
		}
	}
}
