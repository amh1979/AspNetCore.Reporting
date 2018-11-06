using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class DataCellInstance : ScopeInstance
	{
		[NonSerialized]
		private Cell m_cellDef;

		[NonSerialized]
		private static readonly Declaration m_Declaration = DataCellInstance.GetDeclaration();

		internal override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataCellInstance;
			}
		}

		internal override IRIFReportScope RIFReportScope
		{
			get
			{
				return this.m_cellDef;
			}
		}

		internal Cell CellDef
		{
			get
			{
				return this.m_cellDef;
			}
		}

		public override int Size
		{
			get
			{
				return base.Size + ItemSizes.ReferenceSize;
			}
		}

		private DataCellInstance(OnDemandProcessingContext odpContext, Cell cellDef, DataAggregateObjResult[] runningValueValues, DataAggregateObjResult[] runningValueOfAggregateValues, long firstRowOffset)
			: base(firstRowOffset)
		{
			this.m_cellDef = cellDef;
			DataRegion dataRegionDef = this.m_cellDef.DataRegionDef;
			if (cellDef.AggregateIndexes != null)
			{
				base.StoreAggregates(odpContext, dataRegionDef.CellAggregates, cellDef.AggregateIndexes);
			}
			if (cellDef.PostSortAggregateIndexes != null)
			{
				base.StoreAggregates(odpContext, dataRegionDef.CellPostSortAggregates, cellDef.PostSortAggregateIndexes);
			}
			if (runningValueValues == null)
			{
				if (cellDef.RunningValueIndexes != null)
				{
					base.StoreAggregates(odpContext, dataRegionDef.CellRunningValues, cellDef.RunningValueIndexes);
				}
			}
			else if (runningValueValues != null)
			{
				base.StoreAggregates(runningValueValues);
			}
			if (cellDef.DataScopeInfo != null)
			{
				DataScopeInfo dataScopeInfo = cellDef.DataScopeInfo;
				if (dataScopeInfo.AggregatesOfAggregates != null)
				{
					base.StoreAggregates(odpContext, dataScopeInfo.AggregatesOfAggregates);
				}
				if (dataScopeInfo.PostSortAggregatesOfAggregates != null)
				{
					base.StoreAggregates(odpContext, dataScopeInfo.PostSortAggregatesOfAggregates);
				}
				if (runningValueOfAggregateValues != null)
				{
					base.StoreAggregates(runningValueOfAggregateValues);
				}
			}
		}

		internal DataCellInstance()
		{
		}

		internal static DataCellInstance CreateInstance(IMemberHierarchy dataRegionOrRowMemberInstance, OnDemandProcessingContext odpContext, Cell cellDef, long firstRowOffset, int columnMemberSequenceId)
		{
			return DataCellInstance.CreateInstance(dataRegionOrRowMemberInstance, odpContext, cellDef, null, null, firstRowOffset, columnMemberSequenceId);
		}

		internal static DataCellInstance CreateInstance(IMemberHierarchy dataRegionOrRowMemberInstance, OnDemandProcessingContext odpContext, Cell cellDef, DataAggregateObjResult[] runningValueValues, DataAggregateObjResult[] runningValueOfAggregateValues, long firstRowOffset, int columnMemberSequenceId)
		{
			DataCellInstance dataCellInstance = new DataCellInstance(odpContext, cellDef, runningValueValues, runningValueOfAggregateValues, firstRowOffset);
			dataCellInstance.m_cleanupRef = dataRegionOrRowMemberInstance.AddCellInstance(columnMemberSequenceId, cellDef.IndexInCollection, dataCellInstance, odpContext.OdpMetadata.GroupTreeScalabilityCache);
			return dataCellInstance;
		}

		internal void SetupEnvironment(OnDemandProcessingContext odpContext, int dataSetIndex)
		{
			base.SetupFields(odpContext, dataSetIndex);
			DataRegion dataRegionDef = this.m_cellDef.DataRegionDef;
			int num = 0;
			if (this.m_cellDef.AggregateIndexes != null)
			{
				base.SetupAggregates(odpContext, dataRegionDef.CellAggregates, this.m_cellDef.AggregateIndexes, ref num);
			}
			if (this.m_cellDef.PostSortAggregateIndexes != null)
			{
				base.SetupAggregates(odpContext, dataRegionDef.CellPostSortAggregates, this.m_cellDef.PostSortAggregateIndexes, ref num);
			}
			if (this.m_cellDef.RunningValueIndexes != null)
			{
				base.SetupAggregates(odpContext, dataRegionDef.CellRunningValues, this.m_cellDef.RunningValueIndexes, ref num);
			}
			if (this.m_cellDef.DataScopeInfo != null)
			{
				DataScopeInfo dataScopeInfo = this.m_cellDef.DataScopeInfo;
				if (dataScopeInfo.AggregatesOfAggregates != null)
				{
					base.SetupAggregates(odpContext, dataScopeInfo.AggregatesOfAggregates, ref num);
				}
				if (dataScopeInfo.PostSortAggregatesOfAggregates != null)
				{
					base.SetupAggregates(odpContext, dataScopeInfo.PostSortAggregatesOfAggregates, ref num);
				}
				if (dataScopeInfo.RunningValuesOfAggregates != null)
				{
					base.SetupAggregates(odpContext, dataScopeInfo.RunningValuesOfAggregates, ref num);
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ID, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Cell, Token.GlobalReference));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataCellInstance, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScopeInstance, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(DataCellInstance.m_Declaration);
			while (writer.NextMember())
			{
				if (writer.CurrentMember.MemberName == MemberName.ID)
				{
					Global.Tracer.Assert(null != this.m_cellDef, "(null != m_cellDef)");
					writer.WriteGlobalReference(this.m_cellDef);
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(DataCellInstance.m_Declaration);
			while (reader.NextMember())
			{
				if (reader.CurrentMember.MemberName == MemberName.ID)
				{
					this.m_cellDef = reader.ReadGlobalReference<Cell>();
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataCellInstance;
		}
	}
}
