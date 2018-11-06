using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal class SubReport : ReportItem, IPersistable, IIndexedInCollection, IGloballyReferenceable, IGlobalIDOwner
	{
		internal enum Status
		{
			NotRetrieved,
			DataRetrieveFailed,
			DefinitionRetrieveFailed,
			PreFetched,
			DefinitionRetrieved,
			DataRetrieved,
			DataNotRetrieved,
			ParametersNotSpecified
		}

		internal const uint MaxSubReportLevel = 20u;

		private string m_reportName;

		private List<ParameterValue> m_parameters;

		private ExpressionInfo m_noRowsMessage;

		private bool m_mergeTransactions;

		[Reference]
		private GroupingList m_containingScopes;

		private bool m_omitBorderOnPageBreak;

		private bool m_keepTogether;

		private bool m_isTablixCellScope;

		private AspNetCore.ReportingServices.ReportPublishing.LocationFlags m_location = AspNetCore.ReportingServices.ReportPublishing.LocationFlags.None;

		private int m_indexInCollection = -1;

		[Reference]
		private ReportSection m_containingSection;

		[NonSerialized]
		private bool m_isDetailScope;

		[NonSerialized]
		private static readonly Declaration m_Declaration = SubReport.GetDeclaration();

		[NonSerialized]
		private ParameterInfoCollection m_parametersFromCatalog;

		[NonSerialized]
		private Status m_status;

		[NonSerialized]
		private Report m_report;

		[NonSerialized]
		private string m_description;

		[NonSerialized]
		private string m_reportPath;

		[NonSerialized]
		private SubreportExprHost m_exprHost;

		[NonSerialized]
		private List<SubReport> m_detailScopeSubReports;

		[NonSerialized]
		private SubReportInfo m_odpSubReportInfo;

		[NonSerialized]
		private ICatalogItemContext m_reportContext;

		[NonSerialized]
		private OnDemandProcessingContext m_odpContext;

		[NonSerialized]
		private bool m_exceededMaxLevel;

		[NonSerialized]
		private IReference<SubReportInstance> m_currentSubReportInstance;

		internal override AspNetCore.ReportingServices.ReportProcessing.ObjectType ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.Subreport;
			}
		}

		internal string OriginalCatalogPath
		{
			get
			{
				return this.m_reportPath;
			}
			set
			{
				this.m_reportPath = value;
			}
		}

		internal List<ParameterValue> Parameters
		{
			get
			{
				return this.m_parameters;
			}
			set
			{
				this.m_parameters = value;
			}
		}

		internal ExpressionInfo NoRowsMessage
		{
			get
			{
				return this.m_noRowsMessage;
			}
			set
			{
				this.m_noRowsMessage = value;
			}
		}

		internal bool MergeTransactions
		{
			get
			{
				return this.m_mergeTransactions;
			}
			set
			{
				this.m_mergeTransactions = value;
			}
		}

		internal GroupingList ContainingScopes
		{
			get
			{
				return this.m_containingScopes;
			}
			set
			{
				this.m_containingScopes = value;
			}
		}

		internal Status RetrievalStatus
		{
			get
			{
				return this.m_status;
			}
			set
			{
				this.m_status = value;
			}
		}

		internal string ReportName
		{
			get
			{
				return this.m_reportName;
			}
			set
			{
				this.m_reportName = value;
			}
		}

		internal string Description
		{
			get
			{
				return this.m_description;
			}
			set
			{
				this.m_description = value;
			}
		}

		internal Report Report
		{
			get
			{
				return this.m_report;
			}
			set
			{
				this.m_report = value;
			}
		}

		internal ICatalogItemContext ReportContext
		{
			get
			{
				return this.m_reportContext;
			}
			set
			{
				this.m_reportContext = value;
			}
		}

		internal ParameterInfoCollection ParametersFromCatalog
		{
			get
			{
				return this.m_parametersFromCatalog;
			}
			set
			{
				this.m_parametersFromCatalog = value;
			}
		}

		internal SubreportExprHost SubReportExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal bool IsTablixCellScope
		{
			get
			{
				return this.m_isTablixCellScope;
			}
		}

		internal bool IsDetailScope
		{
			get
			{
				return this.m_isDetailScope;
			}
			set
			{
				this.m_isDetailScope = value;
			}
		}

		internal List<SubReport> DetailScopeSubReports
		{
			get
			{
				return this.m_detailScopeSubReports;
			}
			set
			{
				this.m_detailScopeSubReports = value;
			}
		}

		internal SubReportInfo OdpSubReportInfo
		{
			get
			{
				return this.m_odpSubReportInfo;
			}
			set
			{
				this.m_odpSubReportInfo = value;
			}
		}

		internal bool KeepTogether
		{
			get
			{
				return this.m_keepTogether;
			}
			set
			{
				this.m_keepTogether = value;
			}
		}

		internal bool OmitBorderOnPageBreak
		{
			get
			{
				return this.m_omitBorderOnPageBreak;
			}
			set
			{
				this.m_omitBorderOnPageBreak = value;
			}
		}

		internal OnDemandProcessingContext OdpContext
		{
			get
			{
				return this.m_odpContext;
			}
			set
			{
				this.m_odpContext = value;
			}
		}

		internal bool ExceededMaxLevel
		{
			get
			{
				return this.m_exceededMaxLevel;
			}
			set
			{
				this.m_exceededMaxLevel = value;
			}
		}

		internal bool InDataRegion
		{
			get
			{
				return (this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataRegion) != (AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0;
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
				return IndexedInCollectionType.SubReport;
			}
		}

		internal IReference<SubReportInstance> CurrentSubReportInstance
		{
			get
			{
				return this.m_currentSubReportInstance;
			}
			set
			{
				this.m_currentSubReportInstance = value;
			}
		}

		internal SubReport(ReportItem parent)
			: base(parent)
		{
		}

		internal SubReport(int id, ReportItem parent)
			: base(id, parent)
		{
			this.m_parameters = new List<ParameterValue>();
		}

		protected override InstancePathItem CreateInstancePathItem()
		{
			return new InstancePathItem(InstancePathItemType.SubReport, this.IndexInCollection);
		}

		internal ReportSection GetContainingSection(OnDemandProcessingContext parentReportOdpContext)
		{
			if (this.m_containingSection == null)
			{
				this.m_containingSection = parentReportOdpContext.ReportDefinition.ReportSections[0];
			}
			return this.m_containingSection;
		}

		internal void SetContainingSection(ReportSection section)
		{
			this.m_containingSection = section;
		}

		internal override bool Initialize(InitializationContext context)
		{
			this.m_location = context.Location;
			context.ObjectType = this.ObjectType;
			context.ObjectName = base.m_name;
			if (this.InDataRegion)
			{
				context.SetDataSetHasSubReports();
				if ((context.Location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataRegionCellTopLevelItem) != 0)
				{
					this.m_isTablixCellScope = context.IsDataRegionScopedCell;
				}
				if ((AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0 < (context.Location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDetail))
				{
					this.m_isDetailScope = true;
					context.SetDataSetDetailUserSortFilter();
				}
			}
			context.SetIndexInCollection(this);
			context.ExprHostBuilder.SubreportStart(base.m_name);
			base.Initialize(context);
			if (base.m_visibility != null)
			{
				base.m_visibility.Initialize(context);
			}
			if (this.m_parameters != null)
			{
				for (int i = 0; i < this.m_parameters.Count; i++)
				{
					ParameterValue parameterValue = this.m_parameters[i];
					context.ExprHostBuilder.SubreportParameterStart();
					parameterValue.Initialize("SubreportParameter(" + parameterValue.Name + ")", context, false);
					parameterValue.ExprHostID = context.ExprHostBuilder.SubreportParameterEnd();
				}
			}
			if (this.m_noRowsMessage != null)
			{
				this.m_noRowsMessage.Initialize("NoRows", context);
				context.ExprHostBuilder.GenericNoRows(this.m_noRowsMessage);
			}
			base.ExprHostID = context.ExprHostBuilder.SubreportEnd();
			return false;
		}

		internal override void InitializeRVDirectionDependentItems(InitializationContext context)
		{
			this.m_containingScopes = context.GetContainingScopes();
			for (int i = 0; i < this.m_containingScopes.Count; i++)
			{
				this.m_containingScopes[i].SaveGroupExprValues = true;
			}
			if (this.m_isDetailScope)
			{
				this.m_containingScopes.Add(null);
			}
		}

		internal void UpdateSubReportScopes(AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing.UserSortFilterContext context)
		{
			if (this.m_containingScopes != null && 0 < this.m_containingScopes.Count && this.m_containingScopes.LastEntry == null)
			{
				if (context.DetailScopeSubReports != null)
				{
					this.m_detailScopeSubReports = context.CloneDetailScopeSubReports();
				}
				else
				{
					this.m_detailScopeSubReports = new List<SubReport>();
				}
				this.m_detailScopeSubReports.Add(this);
			}
			else
			{
				this.m_detailScopeSubReports = context.DetailScopeSubReports;
			}
			if (context.ContainingScopes != null)
			{
				if (this.m_containingScopes != null && 0 < this.m_containingScopes.Count)
				{
					this.m_containingScopes.InsertRange(0, context.ContainingScopes);
				}
				else
				{
					this.m_containingScopes = context.ContainingScopes;
				}
			}
			if (this.m_report != null && this.m_report.EventSources != null)
			{
				int count = this.m_report.EventSources.Count;
				for (int i = 0; i < count; i++)
				{
					IInScopeEventSource inScopeEventSource = this.m_report.EventSources[i];
					if (inScopeEventSource.UserSort != null)
					{
						inScopeEventSource.UserSort.DetailScopeSubReports = this.m_detailScopeSubReports;
					}
					if (this.m_containingScopes != null && 0 < this.m_containingScopes.Count)
					{
						if (inScopeEventSource.ContainingScopes != null && 0 < inScopeEventSource.ContainingScopes.Count)
						{
							inScopeEventSource.ContainingScopes.InsertRange(0, this.m_containingScopes);
						}
						else
						{
							inScopeEventSource.IsSubReportTopLevelScope = true;
							inScopeEventSource.ContainingScopes = this.m_containingScopes;
						}
					}
				}
			}
		}

		internal void UpdateSubReportEventSourceGlobalDataSetIds(SubReportInfo subReportInfo)
		{
			this.m_odpSubReportInfo = subReportInfo;
			if (this.m_report != null && this.m_report.EventSources != null)
			{
				int count = this.m_report.EventSources.Count;
				for (int i = 0; i < count; i++)
				{
					IInScopeEventSource inScopeEventSource = this.m_report.EventSources[i];
					if (inScopeEventSource.UserSort != null && -1 != subReportInfo.UserSortDataSetGlobalId)
					{
						inScopeEventSource.UserSort.SubReportDataSetGlobalId = subReportInfo.UserSortDataSetGlobalId;
					}
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Parameters, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterValue));
			list.Add(new MemberInfo(MemberName.NoRowsMessage, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MergeTransactions, Token.Boolean));
			list.Add(new MemberInfo(MemberName.ContainingScopes, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Grouping));
			list.Add(new MemberInfo(MemberName.IsTablixCellScope, Token.Boolean));
			list.Add(new MemberInfo(MemberName.ReportName, Token.String));
			list.Add(new MemberInfo(MemberName.OmitBorderOnPageBreak, Token.Boolean));
			list.Add(new MemberInfo(MemberName.KeepTogether, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Location, Token.Enum));
			list.Add(new MemberInfo(MemberName.IndexInCollection, Token.Int32));
			list.Add(new MemberInfo(MemberName.ContainingSection, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportSection, Token.Reference));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReport, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(SubReport.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Parameters:
					writer.Write(this.m_parameters);
					break;
				case MemberName.NoRowsMessage:
					writer.Write(this.m_noRowsMessage);
					break;
				case MemberName.MergeTransactions:
					writer.Write(this.m_mergeTransactions);
					break;
				case MemberName.ContainingScopes:
					writer.WriteListOfReferences(this.m_containingScopes);
					break;
				case MemberName.IsTablixCellScope:
					writer.Write(this.m_isTablixCellScope);
					break;
				case MemberName.ReportName:
					writer.Write(this.m_reportName);
					break;
				case MemberName.OmitBorderOnPageBreak:
					writer.Write(this.m_omitBorderOnPageBreak);
					break;
				case MemberName.KeepTogether:
					writer.Write(this.m_keepTogether);
					break;
				case MemberName.Location:
					writer.WriteEnum((int)this.m_location);
					break;
				case MemberName.IndexInCollection:
					writer.Write(this.m_indexInCollection);
					break;
				case MemberName.ContainingSection:
					writer.WriteReference(this.m_containingSection);
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
			reader.RegisterDeclaration(SubReport.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Parameters:
					this.m_parameters = reader.ReadGenericListOfRIFObjects<ParameterValue>();
					break;
				case MemberName.NoRowsMessage:
					this.m_noRowsMessage = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MergeTransactions:
					this.m_mergeTransactions = reader.ReadBoolean();
					break;
				case MemberName.ContainingScopes:
					if (reader.ReadListOfReferencesNoResolution(this) == 0)
					{
						this.m_containingScopes = new GroupingList();
					}
					break;
				case MemberName.IsTablixCellScope:
					this.m_isTablixCellScope = reader.ReadBoolean();
					break;
				case MemberName.ReportName:
					this.m_reportName = reader.ReadString();
					break;
				case MemberName.OmitBorderOnPageBreak:
					this.m_omitBorderOnPageBreak = reader.ReadBoolean();
					break;
				case MemberName.KeepTogether:
					this.m_keepTogether = reader.ReadBoolean();
					break;
				case MemberName.Location:
					this.m_location = (AspNetCore.ReportingServices.ReportPublishing.LocationFlags)reader.ReadEnum();
					break;
				case MemberName.IndexInCollection:
					this.m_indexInCollection = reader.ReadInt32();
					break;
				case MemberName.ContainingSection:
					this.m_containingSection = reader.ReadReference<ReportSection>(this);
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
			if (memberReferencesCollection.TryGetValue(SubReport.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					switch (item.MemberName)
					{
					case MemberName.ContainingScopes:
						if (this.m_containingScopes == null)
						{
							this.m_containingScopes = new GroupingList();
						}
						if (item.RefID != -2)
						{
							Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
							Global.Tracer.Assert(referenceableItems[item.RefID] is Grouping);
							Global.Tracer.Assert(!this.m_containingScopes.Contains((Grouping)referenceableItems[item.RefID]));
							this.m_containingScopes.Add((Grouping)referenceableItems[item.RefID]);
						}
						else
						{
							this.m_containingScopes.Add(null);
						}
						break;
					case MemberName.ContainingSection:
					{
						IReferenceable referenceable = default(IReferenceable);
						referenceableItems.TryGetValue(item.RefID, out referenceable);
						this.m_containingSection = (ReportSection)referenceable;
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReport;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			SubReport subReport = (SubReport)base.PublishClone(context);
			context.AddSubReport(subReport);
			if (this.m_reportPath != null)
			{
				subReport.m_reportPath = (string)this.m_reportPath.Clone();
			}
			if (this.m_parameters != null)
			{
				subReport.m_parameters = new List<ParameterValue>(this.m_parameters.Count);
				foreach (ParameterValue parameter in this.m_parameters)
				{
					subReport.m_parameters.Add((ParameterValue)parameter.PublishClone(context));
				}
			}
			if (this.m_noRowsMessage != null)
			{
				subReport.m_noRowsMessage = (ExpressionInfo)this.m_noRowsMessage.PublishClone(context);
			}
			return subReport;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null, "(reportExprHost != null && reportObjectModel != null)");
				this.m_exprHost = reportExprHost.SubreportHostsRemotable[base.ExprHostID];
				base.ReportItemSetExprHost(this.m_exprHost, reportObjectModel);
				if (this.m_exprHost.ParameterHostsRemotable != null)
				{
					Global.Tracer.Assert(this.m_parameters != null, "(m_parameters != null)");
					for (int num = this.m_parameters.Count - 1; num >= 0; num--)
					{
						this.m_parameters[num].SetExprHost(this.m_exprHost.ParameterHostsRemotable, reportObjectModel);
					}
				}
			}
		}

		internal string EvaulateNoRowMessage(IReportScopeInstance subReportInstance, OnDemandProcessingContext odpContext)
		{
			odpContext.SetupContext(this, subReportInstance);
			return odpContext.ReportRuntime.EvaluateSubReportNoRowsExpression(this, "Subreport", "NoRowsMessage");
		}
	}
}
