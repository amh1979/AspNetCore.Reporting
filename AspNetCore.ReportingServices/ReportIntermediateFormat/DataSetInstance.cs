using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class DataSetInstance : ScopeInstance
	{
		private int m_recordSetSize = -1;

		private string m_rewrittenCommandText;

		private string m_commandText;

		private DateTime m_executionTime = DateTime.MinValue;

		private FieldInfo[] m_fieldInfos;

		private uint m_lcid;

		private DataSet.TriState m_caseSensitivity;

		private DataSet.TriState m_accentSensitivity;

		private DataSet.TriState m_kanatypeSensitivity;

		private DataSet.TriState m_widthSensitivity;

		[NonSerialized]
		private bool m_oldSnapshotTablixProcessingComplete;

		private string m_dataChunkName;

		private List<LookupObjResult> m_lookupResults;

		[NonSerialized]
		private CompareInfo m_cachedCompareInfo;

		[NonSerialized]
		private CompareOptions m_cachedCompareOptions;

		[NonSerialized]
		private static readonly Declaration m_Declaration = DataSetInstance.GetDeclaration();

		[NonSerialized]
		private DataSet m_dataSetDef;

		internal override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSetInstance;
			}
		}

		internal DataSet DataSetDef
		{
			get
			{
				return this.m_dataSetDef;
			}
			set
			{
				this.m_dataSetDef = value;
			}
		}

		internal int RecordSetSize
		{
			get
			{
				return this.m_recordSetSize;
			}
			set
			{
				this.m_recordSetSize = value;
			}
		}

		internal bool NoRows
		{
			get
			{
				return this.m_recordSetSize <= 0;
			}
		}

		internal FieldInfo[] FieldInfos
		{
			get
			{
				return this.m_fieldInfos;
			}
			set
			{
				this.m_fieldInfos = value;
			}
		}

		internal string RewrittenCommandText
		{
			get
			{
				return this.m_rewrittenCommandText;
			}
			set
			{
				this.m_rewrittenCommandText = value;
			}
		}

		internal string CommandText
		{
			get
			{
				return this.m_commandText;
			}
			set
			{
				this.m_commandText = value;
			}
		}

		internal DataSet.TriState CaseSensitivity
		{
			get
			{
				return this.m_caseSensitivity;
			}
			set
			{
				this.m_caseSensitivity = value;
			}
		}

		internal DataSet.TriState AccentSensitivity
		{
			get
			{
				return this.m_accentSensitivity;
			}
			set
			{
				this.m_accentSensitivity = value;
			}
		}

		internal DataSet.TriState KanatypeSensitivity
		{
			get
			{
				return this.m_kanatypeSensitivity;
			}
			set
			{
				this.m_kanatypeSensitivity = value;
			}
		}

		internal DataSet.TriState WidthSensitivity
		{
			get
			{
				return this.m_widthSensitivity;
			}
			set
			{
				this.m_widthSensitivity = value;
			}
		}

		internal uint LCID
		{
			get
			{
				return this.m_lcid;
			}
			set
			{
				this.m_lcid = value;
			}
		}

		internal List<LookupObjResult> LookupResults
		{
			get
			{
				return this.m_lookupResults;
			}
			set
			{
				this.m_lookupResults = value;
			}
		}

		internal bool OldSnapshotTablixProcessingComplete
		{
			get
			{
				return this.m_oldSnapshotTablixProcessingComplete;
			}
			set
			{
				this.m_oldSnapshotTablixProcessingComplete = value;
			}
		}

		internal string DataChunkName
		{
			get
			{
				return this.m_dataChunkName;
			}
			set
			{
				this.m_dataChunkName = value;
			}
		}

		internal CompareInfo CompareInfo
		{
			get
			{
				if (this.m_cachedCompareInfo == null)
				{
					this.CreateCompareInfo();
				}
				return this.m_cachedCompareInfo;
			}
		}

		internal CompareOptions ClrCompareOptions
		{
			get
			{
				if (this.m_cachedCompareInfo == null)
				{
					this.CreateCompareInfo();
				}
				return this.m_cachedCompareOptions;
			}
		}

		internal DataSetInstance(DataSet dataSetDef)
		{
			this.m_dataSetDef = dataSetDef;
		}

		internal DataSetInstance()
		{
		}

		internal void InitializeForReprocessing()
		{
			this.m_oldSnapshotTablixProcessingComplete = false;
			base.m_aggregateValues = null;
			this.m_lookupResults = null;
			base.m_firstRowOffset = -1L;
		}

		internal override void AddChildScope(IReference<ScopeInstance> child, int indexInCollection)
		{
			Global.Tracer.Assert(false);
		}

		internal void SetupEnvironment(OnDemandProcessingContext odpContext, bool newDataSetDefinition)
		{
			if (newDataSetDefinition)
			{
				odpContext.SetupFieldsForNewDataSet(this.m_dataSetDef, this, false, this.NoRows);
			}
			if (!this.NoRows)
			{
				if (base.m_firstRowOffset == DataFieldRow.UnInitializedStreamOffset)
				{
					odpContext.ReportObjectModel.CreateNoRows();
				}
				else
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader dataChunkReader = odpContext.GetDataChunkReader(this.m_dataSetDef.IndexInCollection);
					dataChunkReader.ReadOneRowAtPosition(base.m_firstRowOffset);
					odpContext.ReportObjectModel.FieldsImpl.NewRow(base.m_firstRowOffset);
					odpContext.ReportObjectModel.UpdateFieldValues(!newDataSetDefinition, dataChunkReader.RecordRow, this, dataChunkReader.ReaderExtensionsSupported);
				}
			}
		}

		internal void SetupDataSetLevelAggregates(OnDemandProcessingContext odpContext)
		{
			int num = 0;
			base.SetupAggregates(odpContext, this.m_dataSetDef.Aggregates, ref num);
			base.SetupAggregates(odpContext, this.m_dataSetDef.PostSortAggregates, ref num);
		}

		internal void SetupCollationSettings(OnDemandProcessingContext odpContext)
		{
			odpContext.SetComparisonInformation(this.CompareInfo, this.ClrCompareOptions, this.m_dataSetDef.NullsAsBlanks, this.m_dataSetDef.UseOrdinalStringKeyGeneration);
		}

		internal void SaveCollationSettings(DataSet dataSet)
		{
			this.LCID = dataSet.LCID;
			this.CaseSensitivity = dataSet.CaseSensitivity;
			this.WidthSensitivity = dataSet.WidthSensitivity;
			this.AccentSensitivity = dataSet.AccentSensitivity;
			this.KanatypeSensitivity = dataSet.KanatypeSensitivity;
		}

		private void CreateCompareInfo()
		{
			if (this.m_dataSetDef.NeedAutoDetectCollation())
			{
				this.m_dataSetDef.LCID = this.m_lcid;
				this.m_dataSetDef.CaseSensitivity = this.m_caseSensitivity;
				this.m_dataSetDef.AccentSensitivity = this.m_accentSensitivity;
				this.m_dataSetDef.KanatypeSensitivity = this.m_kanatypeSensitivity;
				this.m_dataSetDef.WidthSensitivity = this.m_widthSensitivity;
			}
			this.m_cachedCompareInfo = this.m_dataSetDef.CreateCultureInfoFromLcid().CompareInfo;
			this.m_cachedCompareOptions = this.m_dataSetDef.GetCLRCompareOptions();
		}

		internal IDataComparer CreateProcessingComparer(OnDemandProcessingContext odpContext)
		{
			if (odpContext.ContextMode == OnDemandProcessingContext.Mode.Streaming)
			{
				return new CommonDataComparer(this.CompareInfo, this.ClrCompareOptions, this.m_dataSetDef.NullsAsBlanks);
			}
			return new AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ProcessingComparer(this.CompareInfo, this.ClrCompareOptions, this.m_dataSetDef.NullsAsBlanks);
		}

		internal DateTime GetQueryExecutionTime(DateTime reportExecutionTime)
		{
			if (!(this.m_executionTime == DateTime.MinValue))
			{
				return this.m_executionTime;
			}
			return reportExecutionTime;
		}

		internal void SetQueryExecutionTime(DateTime queryExecutionTime)
		{
			this.m_executionTime = queryExecutionTime;
		}

		internal FieldInfo GetOrCreateFieldInfo(int aIndex)
		{
			if (this.m_fieldInfos == null)
			{
				this.m_fieldInfos = new FieldInfo[this.m_dataSetDef.NonCalculatedFieldCount];
			}
			if (this.m_fieldInfos[aIndex] == null)
			{
				this.m_fieldInfos[aIndex] = new FieldInfo();
			}
			return this.m_fieldInfos[aIndex];
		}

		internal bool IsFieldMissing(int index)
		{
			if (this.m_fieldInfos != null && this.m_fieldInfos[index] != null)
			{
				return this.m_fieldInfos[index].Missing;
			}
			return false;
		}

		internal int GetFieldPropertyCount(int index)
		{
			if (this.m_fieldInfos != null && this.m_fieldInfos[index] != null)
			{
				return this.m_fieldInfos[index].PropertyCount;
			}
			return 0;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.RecordSetSize, Token.Int32));
			list.Add(new MemberInfo(MemberName.CommandText, Token.String));
			list.Add(new MemberInfo(MemberName.RewrittenCommandText, Token.String));
			list.Add(new MemberInfo(MemberName.Fields, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.FieldInfo));
			list.Add(new MemberInfo(MemberName.CaseSensitivity, Token.Enum));
			list.Add(new MemberInfo(MemberName.AccentSensitivity, Token.Enum));
			list.Add(new MemberInfo(MemberName.KanatypeSensitivity, Token.Enum));
			list.Add(new MemberInfo(MemberName.WidthSensitivity, Token.Enum));
			list.Add(new MemberInfo(MemberName.LCID, Token.UInt32));
			list.Add(new ReadOnlyMemberInfo(MemberName.TablixProcessingComplete, Token.Boolean));
			list.Add(new MemberInfo(MemberName.DataChunkName, Token.String));
			list.Add(new MemberInfo(MemberName.LookupResults, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupObjResult));
			list.Add(new MemberInfo(MemberName.ExecutionTime, Token.DateTime));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSetInstance, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScopeInstance, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(DataSetInstance.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.RecordSetSize:
					writer.Write(this.m_recordSetSize);
					break;
				case MemberName.CommandText:
					writer.Write(this.m_commandText);
					break;
				case MemberName.RewrittenCommandText:
					writer.Write(this.m_rewrittenCommandText);
					break;
				case MemberName.Fields:
					writer.Write(this.m_fieldInfos);
					break;
				case MemberName.CaseSensitivity:
					writer.WriteEnum((int)this.m_caseSensitivity);
					break;
				case MemberName.AccentSensitivity:
					writer.WriteEnum((int)this.m_accentSensitivity);
					break;
				case MemberName.KanatypeSensitivity:
					writer.WriteEnum((int)this.m_kanatypeSensitivity);
					break;
				case MemberName.WidthSensitivity:
					writer.WriteEnum((int)this.m_widthSensitivity);
					break;
				case MemberName.LCID:
					writer.Write(this.m_lcid);
					break;
				case MemberName.DataChunkName:
					writer.Write(this.m_dataChunkName);
					break;
				case MemberName.LookupResults:
					writer.Write(this.m_lookupResults);
					break;
				case MemberName.ExecutionTime:
					writer.Write(this.m_executionTime);
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
			reader.RegisterDeclaration(DataSetInstance.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.RecordSetSize:
					this.m_recordSetSize = reader.ReadInt32();
					break;
				case MemberName.CommandText:
					this.m_commandText = reader.ReadString();
					break;
				case MemberName.RewrittenCommandText:
					this.m_rewrittenCommandText = reader.ReadString();
					break;
				case MemberName.Fields:
					this.m_fieldInfos = reader.ReadArrayOfRIFObjects<FieldInfo>();
					break;
				case MemberName.CaseSensitivity:
					this.m_caseSensitivity = (DataSet.TriState)reader.ReadEnum();
					break;
				case MemberName.AccentSensitivity:
					this.m_accentSensitivity = (DataSet.TriState)reader.ReadEnum();
					break;
				case MemberName.KanatypeSensitivity:
					this.m_kanatypeSensitivity = (DataSet.TriState)reader.ReadEnum();
					break;
				case MemberName.WidthSensitivity:
					this.m_widthSensitivity = (DataSet.TriState)reader.ReadEnum();
					break;
				case MemberName.LCID:
					this.m_lcid = reader.ReadUInt32();
					break;
				case MemberName.TablixProcessingComplete:
					this.m_oldSnapshotTablixProcessingComplete = reader.ReadBoolean();
					break;
				case MemberName.DataChunkName:
					this.m_dataChunkName = reader.ReadString();
					break;
				case MemberName.LookupResults:
					this.m_lookupResults = reader.ReadListOfRIFObjects<List<LookupObjResult>>();
					break;
				case MemberName.ExecutionTime:
					this.m_executionTime = reader.ReadDateTime();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSetInstance;
		}
	}
}
