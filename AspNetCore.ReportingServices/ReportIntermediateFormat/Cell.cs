using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal abstract class Cell : IDOwner, IAggregateHolder, IRunningValueHolder, IPersistable, IIndexedInCollection, IGloballyReferenceable, IGlobalIDOwner, IRIFReportIntersectionScope, IRIFReportDataScope, IRIFReportScope, IInstancePath, IRIFDataScope
	{
		protected int m_exprHostID = -1;

		protected int m_parentRowID = -1;

		protected int m_parentColumnID = -1;

		protected int m_indexInCollection = -1;

		protected bool m_hasInnerGroupTreeHierarchy;

		[Reference]
		protected DataRegion m_dataRegionDef;

		protected List<int> m_aggregateIndexes;

		protected List<int> m_postSortAggregateIndexes;

		protected List<int> m_runningValueIndexes;

		private bool m_needToCacheDataRows;

		private byte[] m_textboxesInScope;

		private byte[] m_variablesInScope;

		private List<IInScopeEventSource> m_inScopeEventSources;

		protected bool m_inDynamicRowAndColumnContext;

		protected DataScopeInfo m_dataScopeInfo;

		[NonSerialized]
		private static readonly Declaration m_Declaration = Cell.GetDeclaration();

		[NonSerialized]
		protected IDOwner m_parentColumnIDOwner;

		[NonSerialized]
		protected List<DataAggregateInfo> m_aggregates;

		[NonSerialized]
		protected List<DataAggregateInfo> m_postSortAggregates;

		[NonSerialized]
		protected List<RunningValueInfo> m_runningValues;

		[NonSerialized]
		protected DataScopeInfo m_canonicalDataScopeInfo;

		[NonSerialized]
		private IRIFReportDataScope m_parentReportScope;

		[NonSerialized]
		private IRIFReportDataScope m_parentColumnReportScope;

		[NonSerialized]
		private IReference<IOnDemandScopeInstance> m_currentStreamingScopeInstance;

		[NonSerialized]
		private SyntheticTriangulatedCellReference m_cachedSyntheticCellReference;

		[NonSerialized]
		private IReference<IOnDemandScopeInstance> m_cachedNoRowsStreamingScopeInstance;

		string IRIFDataScope.Name
		{
			get
			{
				return null;
			}
		}

		public DataScopeInfo DataScopeInfo
		{
			get
			{
				return this.m_dataScopeInfo;
			}
		}

		public abstract AspNetCore.ReportingServices.ReportProcessing.ObjectType DataScopeObjectType
		{
			get;
		}

		internal DataScopeInfo CanonicalDataScopeInfo
		{
			get
			{
				return this.m_canonicalDataScopeInfo;
			}
			set
			{
				this.m_canonicalDataScopeInfo = value;
			}
		}

		internal int ExpressionHostID
		{
			get
			{
				return this.m_exprHostID;
			}
			set
			{
				this.m_exprHostID = value;
			}
		}

		internal int ParentRowMemberID
		{
			get
			{
				return this.m_parentRowID;
			}
		}

		internal int ParentColumnMemberID
		{
			get
			{
				return this.m_parentColumnID;
			}
		}

		internal DataRegion DataRegionDef
		{
			get
			{
				return this.m_dataRegionDef;
			}
		}

		internal List<int> AggregateIndexes
		{
			get
			{
				return this.m_aggregateIndexes;
			}
		}

		internal List<int> PostSortAggregateIndexes
		{
			get
			{
				return this.m_postSortAggregateIndexes;
			}
		}

		internal List<int> RunningValueIndexes
		{
			get
			{
				return this.m_runningValueIndexes;
			}
		}

		internal bool HasInnerGroupTreeHierarchy
		{
			get
			{
				return this.m_hasInnerGroupTreeHierarchy;
			}
		}

		internal bool SimpleGroupTreeCell
		{
			get
			{
				if (!this.m_hasInnerGroupTreeHierarchy && this.m_aggregateIndexes == null && this.m_postSortAggregateIndexes == null && this.m_runningValueIndexes == null && (this.m_dataScopeInfo == null || !this.m_dataScopeInfo.HasAggregatesOrRunningValues) && !this.m_dataRegionDef.IsMatrixIDC)
				{
					return true;
				}
				return false;
			}
		}

		internal List<DataAggregateInfo> Aggregates
		{
			get
			{
				return this.m_aggregates;
			}
		}

		internal List<DataAggregateInfo> PostSortAggregates
		{
			get
			{
				return this.m_postSortAggregates;
			}
		}

		internal List<RunningValueInfo> RunningValues
		{
			get
			{
				return this.m_runningValues;
			}
		}

		public override List<InstancePathItem> InstancePath
		{
			get
			{
				if (base.m_cachedInstancePath == null)
				{
					if (this.m_parentColumnIDOwner == null)
					{
						return base.InstancePath;
					}
					base.m_cachedInstancePath = InstancePathItem.CombineRowColPath(base.InstancePath, this.m_parentColumnIDOwner.InstancePath);
					base.m_cachedInstancePath.Add(base.InstancePathItem);
				}
				return base.m_cachedInstancePath;
			}
		}

		protected virtual bool IsDataRegionBodyCell
		{
			get
			{
				return false;
			}
		}

		public int IndexInCollection
		{
			get
			{
				return this.m_indexInCollection;
			}
			set
			{
				this.m_indexInCollection = value;
			}
		}

		public IndexedInCollectionType IndexedInCollectionType
		{
			get
			{
				return IndexedInCollectionType.Cell;
			}
		}

		internal List<IInScopeEventSource> InScopeEventSources
		{
			get
			{
				return this.m_inScopeEventSources;
			}
		}

		internal bool InDynamicRowAndColumnContext
		{
			get
			{
				return this.m_inDynamicRowAndColumnContext;
			}
		}

		internal virtual List<ReportItem> CellContentCollection
		{
			get
			{
				return null;
			}
		}

		bool IRIFReportScope.NeedToCacheDataRows
		{
			get
			{
				return this.m_needToCacheDataRows;
			}
			set
			{
				if (!this.m_needToCacheDataRows)
				{
					this.m_needToCacheDataRows = value;
				}
			}
		}

		public bool IsDataIntersectionScope
		{
			get
			{
				return this.InDynamicRowAndColumnContext;
			}
		}

		public bool IsScope
		{
			get
			{
				if (!this.IsDataIntersectionScope)
				{
					if (this.m_dataScopeInfo != null)
					{
						return this.m_dataScopeInfo.NeedsIDC;
					}
					return false;
				}
				return true;
			}
		}

		public bool IsGroup
		{
			get
			{
				return false;
			}
		}

		public bool IsColumnOuterGrouping
		{
			get
			{
				return this.DataRegionDef.ProcessingInnerGrouping == DataRegion.ProcessingInnerGroupings.Row;
			}
		}

		public IRIFReportDataScope ParentReportScope
		{
			get
			{
				if (this.IsDataIntersectionScope)
				{
					return null;
				}
				if (this.m_parentReportScope == null)
				{
					IRIFReportDataScope parentReportScope = IDOwner.FindReportDataScope(base.ParentInstancePath);
					IRIFReportDataScope iRIFReportDataScope = IDOwner.FindReportDataScope(this.m_parentColumnIDOwner);
					if (iRIFReportDataScope is ReportHierarchyNode)
					{
						this.m_parentReportScope = iRIFReportDataScope;
					}
					else
					{
						this.m_parentReportScope = parentReportScope;
					}
				}
				return this.m_parentReportScope;
			}
		}

		public IRIFReportDataScope ParentRowReportScope
		{
			get
			{
				if (this.IsDataIntersectionScope)
				{
					if (this.m_parentReportScope == null)
					{
						this.m_parentReportScope = IDOwner.FindReportDataScope(base.ParentInstancePath);
					}
					return this.m_parentReportScope;
				}
				return null;
			}
		}

		public IRIFReportDataScope ParentColumnReportScope
		{
			get
			{
				if (this.IsDataIntersectionScope)
				{
					if (this.m_parentColumnReportScope == null)
					{
						this.m_parentColumnReportScope = IDOwner.FindReportDataScope(this.m_parentColumnIDOwner);
					}
					return this.m_parentColumnReportScope;
				}
				return null;
			}
		}

		public IReference<IOnDemandScopeInstance> CurrentStreamingScopeInstance
		{
			get
			{
				return this.m_currentStreamingScopeInstance;
			}
		}

		public bool IsBoundToStreamingScopeInstance
		{
			get
			{
				return this.m_currentStreamingScopeInstance != null;
			}
		}

		protected abstract AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode ExprHostDataRegionMode
		{
			get;
		}

		internal Cell()
		{
		}

		internal Cell(int id, DataRegion dataRegion)
			: base(id)
		{
			this.m_dataRegionDef = dataRegion;
			this.m_aggregates = new List<DataAggregateInfo>();
			this.m_postSortAggregates = new List<DataAggregateInfo>();
			this.m_runningValues = new List<RunningValueInfo>();
			this.m_dataScopeInfo = new DataScopeInfo(id);
		}

		protected override InstancePathItem CreateInstancePathItem()
		{
			return new InstancePathItem(InstancePathItemType.Cell, this.IndexInCollection);
		}

		bool IRIFReportScope.VariableInScope(int sequenceIndex)
		{
			return SequenceIndex.GetBit(this.m_variablesInScope, sequenceIndex, true);
		}

		bool IRIFReportScope.TextboxInScope(int sequenceIndex)
		{
			return SequenceIndex.GetBit(this.m_textboxesInScope, sequenceIndex, true);
		}

		void IRIFReportScope.ResetTextBoxImpls(OnDemandProcessingContext context)
		{
		}

		void IRIFReportScope.AddInScopeTextBox(TextBox textbox)
		{
		}

		void IRIFReportScope.AddInScopeEventSource(IInScopeEventSource eventSource)
		{
			if (this.m_inScopeEventSources == null)
			{
				this.m_inScopeEventSources = new List<IInScopeEventSource>();
			}
			this.m_inScopeEventSources.Add(eventSource);
		}

		public bool IsSameOrChildScopeOf(IRIFReportDataScope candidateScope)
		{
			return DataScopeInfo.IsSameOrChildScope(this, candidateScope);
		}

		public bool IsChildScopeOf(IRIFReportDataScope candidateScope)
		{
			return DataScopeInfo.IsChildScopeOf(this, candidateScope);
		}

		public void BindToStreamingScopeInstance(IReference<IOnDemandMemberInstance> parentRowScopeInstance, IReference<IOnDemandMemberInstance> parentColumnScopeInstance)
		{
			if (this.m_cachedSyntheticCellReference == null)
			{
				this.m_cachedSyntheticCellReference = new SyntheticTriangulatedCellReference(parentRowScopeInstance, parentColumnScopeInstance);
			}
			else
			{
				this.m_cachedSyntheticCellReference.UpdateGroupLeafReferences(parentRowScopeInstance, parentColumnScopeInstance);
			}
			this.m_currentStreamingScopeInstance = this.m_cachedSyntheticCellReference;
		}

		public void ResetAggregates(AggregatesImpl reportOmAggregates)
		{
			this.ResetAggregates(reportOmAggregates, this.m_dataRegionDef.CellAggregates, this.m_aggregateIndexes);
			this.ResetAggregates(reportOmAggregates, this.m_dataRegionDef.CellPostSortAggregates, this.m_postSortAggregateIndexes);
			this.ResetAggregates(reportOmAggregates, this.m_dataRegionDef.CellRunningValues, this.m_runningValueIndexes);
			if (this.m_dataScopeInfo != null)
			{
				this.m_dataScopeInfo.ResetAggregates(reportOmAggregates);
			}
		}

		private void ResetAggregates<T>(AggregatesImpl reportOmAggregates, List<T> aggregateDefs, List<int> aggregateIndices) where T : DataAggregateInfo
		{
			if (aggregateDefs != null && aggregateIndices != null)
			{
				for (int i = 0; i < aggregateIndices.Count; i++)
				{
					int index = aggregateIndices[i];
					reportOmAggregates.Reset((DataAggregateInfo)(object)aggregateDefs[index]);
				}
			}
		}

		public bool HasServerAggregate(string aggregateName)
		{
			if (this.m_aggregateIndexes == null)
			{
				return false;
			}
			foreach (int aggregateIndex in this.m_aggregateIndexes)
			{
				if (DataScopeInfo.IsTargetServerAggregate(this.m_dataRegionDef.CellAggregates[aggregateIndex], aggregateName))
				{
					return true;
				}
			}
			return false;
		}

		public void BindToStreamingScopeInstance(IReference<IOnDemandScopeInstance> scopeInstance)
		{
			this.m_currentStreamingScopeInstance = scopeInstance;
		}

		public void BindToNoRowsScopeInstance(OnDemandProcessingContext odpContext)
		{
			if (this.m_cachedNoRowsStreamingScopeInstance == null)
			{
				StreamingNoRowsCellInstance scopeInstance = new StreamingNoRowsCellInstance(odpContext, this);
				this.m_cachedNoRowsStreamingScopeInstance = new SyntheticOnDemandScopeInstanceReference(scopeInstance);
			}
			this.m_currentStreamingScopeInstance = this.m_cachedNoRowsStreamingScopeInstance;
		}

		public void ClearStreamingScopeInstanceBinding()
		{
			this.m_currentStreamingScopeInstance = null;
		}

		internal void TraverseScopes(IRIFScopeVisitor visitor, int rowIndex, int colIndex)
		{
			visitor.PreVisit(this, rowIndex, colIndex);
			this.TraverseNestedScopes(visitor);
			visitor.PostVisit(this, rowIndex, colIndex);
		}

		protected virtual void TraverseNestedScopes(IRIFScopeVisitor visitor)
		{
		}

		List<DataAggregateInfo> IAggregateHolder.GetAggregateList()
		{
			return this.m_aggregates;
		}

		List<DataAggregateInfo> IAggregateHolder.GetPostSortAggregateList()
		{
			return this.m_postSortAggregates;
		}

		List<RunningValueInfo> IRunningValueHolder.GetRunningValueList()
		{
			return this.m_runningValues;
		}

		void IAggregateHolder.ClearIfEmpty()
		{
			Global.Tracer.Assert(null != this.m_aggregates, "(null != m_aggregates)");
			if (this.m_aggregates.Count == 0)
			{
				this.m_aggregates = null;
			}
			Global.Tracer.Assert(null != this.m_postSortAggregates, "(null != m_postSortAggregates)");
			if (this.m_postSortAggregates.Count == 0)
			{
				this.m_postSortAggregates = null;
			}
		}

		void IRunningValueHolder.ClearIfEmpty()
		{
			Global.Tracer.Assert(null != this.m_runningValues, "(null != m_runningValues)");
			if (this.m_runningValues.Count == 0)
			{
				this.m_runningValues = null;
			}
		}

		internal void GenerateAggregateIndexes(Dictionary<string, int> aggregateIndexMapping, Dictionary<string, int> postSortAggregateIndexMapping, Dictionary<string, int> runningValueIndexMapping)
		{
			if (this.m_aggregates != null)
			{
				Cell.GenerateAggregateIndexes<DataAggregateInfo>(this.m_aggregates, aggregateIndexMapping, ref this.m_aggregateIndexes);
			}
			if (this.m_postSortAggregates != null)
			{
				Cell.GenerateAggregateIndexes<DataAggregateInfo>(this.m_postSortAggregates, postSortAggregateIndexMapping, ref this.m_postSortAggregateIndexes);
			}
			if (this.m_runningValues != null)
			{
				Cell.GenerateAggregateIndexes<RunningValueInfo>(this.m_runningValues, runningValueIndexMapping, ref this.m_runningValueIndexes);
			}
		}

		private static void GenerateAggregateIndexes<AggregateType>(List<AggregateType> cellAggregates, Dictionary<string, int> aggregateIndexMapping, ref List<int> aggregateIndexes) where AggregateType : DataAggregateInfo
		{
			int count = cellAggregates.Count;
			if (count != 0)
			{
				aggregateIndexes = new List<int>();
				for (int i = 0; i < count; i++)
				{
					AggregateType val = cellAggregates[i];
					int item = default(int);
					if (aggregateIndexMapping.TryGetValue(val.Name, out item))
					{
						aggregateIndexes.Add(item);
					}
				}
			}
		}

		internal static bool ContainsInnerGroupTreeHierarchy(ReportItem cellContents)
		{
			if (cellContents == null)
			{
				return false;
			}
			return cellContents.IsOrContainsDataRegionOrSubReport();
		}

		internal void Initialize(int parentRowID, int parentColumnID, int rowindex, int colIndex, InitializationContext context)
		{
			bool flag = this.IsDataRegionBodyCell && context.IsDataRegionCellScope;
			if (flag)
			{
				context.RegisterIndividualCellScope(this);
				this.m_inDynamicRowAndColumnContext = context.IsDataRegionCellScope;
				if (this.DataScopeInfo.JoinInfo != null && this.DataScopeInfo.JoinInfo is IntersectJoinInfo)
				{
					this.m_dataRegionDef.IsMatrixIDC = true;
				}
			}
			else
			{
				context.RegisterNonScopeCell(this);
			}
			this.m_textboxesInScope = context.GetCurrentReferencableTextboxes();
			this.m_variablesInScope = context.GetCurrentReferencableVariables();
			this.m_parentRowID = parentRowID;
			this.m_parentColumnID = parentColumnID;
			context.SetIndexInCollection(this);
			this.StartExprHost(context);
			if (this.m_dataScopeInfo != null)
			{
				this.m_dataScopeInfo.Initialize(context, this);
			}
			this.InternalInitialize(parentRowID, parentColumnID, rowindex, colIndex, context);
			this.EndExprHost(context);
			this.m_dataScopeInfo.ValidateScopeRulesForIdc(context, this);
			if (context.EvaluateAtomicityCondition(this.HasAggregatesForAtomicityCheck(), this, AtomicityReason.Aggregates) || context.EvaluateAtomicityCondition(context.HasMultiplePeerChildScopes(this), this, AtomicityReason.PeerChildScopes))
			{
				context.FoundAtomicScope(this);
			}
			else
			{
				this.m_dataScopeInfo.IsDecomposable = true;
			}
			if (flag)
			{
				context.UnRegisterIndividualCellScope(this);
			}
			else
			{
				context.UnRegisterNonScopeCell(this);
			}
		}

		internal abstract void InternalInitialize(int parentRowID, int parentColumnID, int rowindex, int colIndex, InitializationContext context);

		protected virtual void StartExprHost(InitializationContext context)
		{
			context.ExprHostBuilder.DataCellStart(this.ExprHostDataRegionMode);
		}

		protected virtual void EndExprHost(InitializationContext context)
		{
			this.m_exprHostID = context.ExprHostBuilder.DataCellEnd(this.ExprHostDataRegionMode);
		}

		private bool HasAggregatesForAtomicityCheck()
		{
			if (!DataScopeInfo.HasNonServerAggregates(this.m_aggregates) && !DataScopeInfo.HasAggregates(this.m_postSortAggregates) && !DataScopeInfo.HasAggregates(this.m_runningValues))
			{
				return this.m_dataScopeInfo.HasAggregatesOrRunningValues;
			}
			return true;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			Cell cell = (Cell)base.PublishClone(context);
			cell.m_aggregates = new List<DataAggregateInfo>();
			cell.m_postSortAggregates = new List<DataAggregateInfo>();
			cell.m_runningValues = new List<RunningValueInfo>();
			cell.m_dataScopeInfo = this.m_dataScopeInfo.PublishClone(context, cell.ID);
			context.AddAggregateHolder(cell);
			context.AddRunningValueHolder(cell);
			if (context.CurrentDataRegionClone != null)
			{
				cell.m_dataRegionDef = context.CurrentDataRegionClone;
			}
			return cell;
		}

		internal void BaseSetExprHost(CellExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			if (this.m_dataScopeInfo != null && this.m_dataScopeInfo.JoinInfo != null && exprHost.JoinConditionExprHostsRemotable != null)
			{
				this.m_dataScopeInfo.JoinInfo.SetJoinConditionExprHost(exprHost.JoinConditionExprHostsRemotable, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.ParentRowID, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IDOwner, Token.Reference));
			list.Add(new MemberInfo(MemberName.ParentColumnID, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IDOwner, Token.Reference));
			list.Add(new MemberInfo(MemberName.IndexInCollection, Token.Int32));
			list.Add(new MemberInfo(MemberName.HasInnerGroupTreeHierarchy, Token.Boolean));
			list.Add(new MemberInfo(MemberName.DataRegionDef, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegion, Token.Reference));
			list.Add(new MemberInfo(MemberName.AggregateIndexes, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int32));
			list.Add(new MemberInfo(MemberName.PostSortAggregateIndexes, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int32));
			list.Add(new MemberInfo(MemberName.RunningValueIndexes, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int32));
			list.Add(new MemberInfo(MemberName.NeedToCacheDataRows, Token.Boolean));
			list.Add(new MemberInfo(MemberName.InScopeEventSources, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IInScopeEventSource));
			list.Add(new MemberInfo(MemberName.InDynamicRowAndColumnContext, Token.Boolean));
			list.Add(new MemberInfo(MemberName.TextboxesInScope, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Byte));
			list.Add(new MemberInfo(MemberName.VariablesInScope, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Byte));
			list.Add(new MemberInfo(MemberName.DataScopeInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataScopeInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Cell, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IDOwner, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(Cell.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				case MemberName.ParentRowID:
					writer.WriteReferenceID(this.m_parentRowID);
					break;
				case MemberName.ParentColumnID:
					writer.WriteReferenceID(this.m_parentColumnID);
					break;
				case MemberName.IndexInCollection:
					writer.Write(this.m_indexInCollection);
					break;
				case MemberName.HasInnerGroupTreeHierarchy:
					writer.Write(this.m_hasInnerGroupTreeHierarchy);
					break;
				case MemberName.DataRegionDef:
					Global.Tracer.Assert(null != this.m_dataRegionDef, "(null != m_dataRegionDef)");
					writer.WriteReference(this.m_dataRegionDef);
					break;
				case MemberName.AggregateIndexes:
					writer.WriteListOfPrimitives(this.m_aggregateIndexes);
					break;
				case MemberName.PostSortAggregateIndexes:
					writer.WriteListOfPrimitives(this.m_postSortAggregateIndexes);
					break;
				case MemberName.RunningValueIndexes:
					writer.WriteListOfPrimitives(this.m_runningValueIndexes);
					break;
				case MemberName.NeedToCacheDataRows:
					writer.Write(this.m_needToCacheDataRows);
					break;
				case MemberName.InScopeEventSources:
					writer.WriteListOfReferences(this.m_inScopeEventSources);
					break;
				case MemberName.InDynamicRowAndColumnContext:
					writer.Write(this.m_inDynamicRowAndColumnContext);
					break;
				case MemberName.TextboxesInScope:
					writer.Write(this.m_textboxesInScope);
					break;
				case MemberName.VariablesInScope:
					writer.Write(this.m_variablesInScope);
					break;
				case MemberName.DataScopeInfo:
					writer.Write(this.m_dataScopeInfo);
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
			reader.RegisterDeclaration(Cell.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.ParentRowID:
					base.m_parentIDOwner = reader.ReadReference<IDOwner>(this);
					if (base.m_parentIDOwner != null)
					{
						this.m_parentRowID = base.m_parentIDOwner.ID;
					}
					break;
				case MemberName.ParentColumnID:
					this.m_parentColumnIDOwner = reader.ReadReference<IDOwner>(this);
					if (this.m_parentColumnIDOwner != null)
					{
						this.m_parentColumnID = this.m_parentColumnIDOwner.ID;
					}
					break;
				case MemberName.IndexInCollection:
					this.m_indexInCollection = reader.ReadInt32();
					break;
				case MemberName.HasInnerGroupTreeHierarchy:
					this.m_hasInnerGroupTreeHierarchy = reader.ReadBoolean();
					break;
				case MemberName.DataRegionDef:
					this.m_dataRegionDef = reader.ReadReference<DataRegion>(this);
					break;
				case MemberName.AggregateIndexes:
					this.m_aggregateIndexes = reader.ReadListOfPrimitives<int>();
					break;
				case MemberName.PostSortAggregateIndexes:
					this.m_postSortAggregateIndexes = reader.ReadListOfPrimitives<int>();
					break;
				case MemberName.RunningValueIndexes:
					this.m_runningValueIndexes = reader.ReadListOfPrimitives<int>();
					break;
				case MemberName.NeedToCacheDataRows:
					this.m_needToCacheDataRows = reader.ReadBoolean();
					break;
				case MemberName.InScopeEventSources:
					this.m_inScopeEventSources = reader.ReadGenericListOfReferences<IInScopeEventSource>(this);
					break;
				case MemberName.InDynamicRowAndColumnContext:
					this.m_inDynamicRowAndColumnContext = reader.ReadBoolean();
					break;
				case MemberName.TextboxesInScope:
					this.m_textboxesInScope = reader.ReadByteArray();
					break;
				case MemberName.VariablesInScope:
					this.m_variablesInScope = reader.ReadByteArray();
					break;
				case MemberName.DataScopeInfo:
					this.m_dataScopeInfo = reader.ReadRIFObject<DataScopeInfo>();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(Cell.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item2 in list)
				{
					switch (item2.MemberName)
					{
					case MemberName.ParentRowID:
						Global.Tracer.Assert(referenceableItems.ContainsKey(item2.RefID), "ParentRowID");
						base.m_parentIDOwner = (IDOwner)referenceableItems[item2.RefID];
						this.m_parentRowID = item2.RefID;
						break;
					case MemberName.ParentColumnID:
						Global.Tracer.Assert(referenceableItems.ContainsKey(item2.RefID), "ParentColumnID");
						this.m_parentColumnIDOwner = (IDOwner)referenceableItems[item2.RefID];
						this.m_parentColumnID = item2.RefID;
						break;
					case MemberName.DataRegionDef:
					{
						IReferenceable referenceable2 = default(IReferenceable);
						referenceableItems.TryGetValue(item2.RefID, out referenceable2);
						Global.Tracer.Assert(referenceable2 != null && ((ReportItem)referenceable2).IsDataRegion, "DataRegionDef");
						this.m_dataRegionDef = (DataRegion)referenceable2;
						break;
					}
					case MemberName.InScopeEventSources:
					{
						IReferenceable referenceable = default(IReferenceable);
						referenceableItems.TryGetValue(item2.RefID, out referenceable);
						IInScopeEventSource item = (IInScopeEventSource)referenceable;
						if (this.m_inScopeEventSources == null)
						{
							this.m_inScopeEventSources = new List<IInScopeEventSource>();
						}
						this.m_inScopeEventSources.Add(item);
						break;
					}
					default:
						Global.Tracer.Assert(false);
						break;
					}
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Cell;
		}
	}
}
