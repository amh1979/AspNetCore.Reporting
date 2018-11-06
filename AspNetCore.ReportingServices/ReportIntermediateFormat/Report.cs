using AspNetCore.ReportingServices.OnDemandProcessing;
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
	internal sealed class Report : ReportItem, IPersistable, IRIFReportScope, IInstancePath, IGloballyReferenceable, IGlobalIDOwner, IExpressionHostAssemblyHolder
	{
		internal enum ShowHideTypes
		{
			None,
			Static,
			Interactive
		}

		private bool m_consumeContainerWhitespace;

		private Guid m_reportVersion = Guid.Empty;

		private string m_author;

		private int m_autoRefresh = -1;

		private Dictionary<string, ImageInfo> m_embeddedImages;

		private List<DataSource> m_dataSources;

		private List<Variable> m_variables;

		private bool m_deferVariableEvaluation;

		private byte[] m_exprCompiledCode;

		private bool m_exprCompiledCodeGeneratedWithRefusedPermissions;

		private bool m_mergeOnePass;

		private bool m_subReportMergeTransactions;

		private bool m_needPostGroupProcessing;

		private bool m_hasPostSortAggregates;

		private bool m_hasAggregatesOfAggregates;

		private bool m_hasAggregatesOfAggregatesInUserSort;

		private bool m_hasReportItemReferences;

		private ShowHideTypes m_showHideType;

		private int m_lastID;

		[Reference]
		private List<SubReport> m_subReports;

		private bool m_hasImageStreams;

		private bool m_hasLabels;

		private bool m_hasBookmarks;

		private bool m_parametersNotUsedInQuery;

		private List<ParameterDef> m_parameters;

		private string m_oneDataSetName;

		private List<string> m_codeModules;

		private List<CodeClass> m_codeClasses;

		private bool m_hasSpecialRecursiveAggregates;

		private ExpressionInfo m_language;

		private string m_dataTransform;

		private string m_dataSchema;

		private bool m_dataElementStyleAttribute = true;

		private string m_code;

		private bool m_hasUserSortFilter;

		private bool m_hasHeadersOrFooters;

		private bool m_hasPreviousAggregates;

		private InScopeSortFilterHashtable m_nonDetailSortFiltersInScope;

		private InScopeSortFilterHashtable m_detailSortFiltersInScope;

		private List<DataRegion> m_topLevelDataRegions;

		[Reference]
		private DataSet m_firstDataSet;

		[Reference]
		private DataRegion m_topLeftDataRegion;

		private int m_dataSetsNotOnlyUsedInParameters;

		private List<IInScopeEventSource> m_inScopeEventSources;

		private List<IInScopeEventSource> m_eventSources;

		private List<ReportHierarchyNode> m_groupsWithVariables;

		private byte[] m_flattenedDatasetDependencyMatrix;

		private int m_firstDataSetIndexToProcess = -1;

		private byte[] m_variablesInScope;

		private bool m_hasLookups;

		private List<ReportSection> m_reportSections;

		private ExpressionInfo m_autoRefreshExpression;

		private ExpressionInfo m_initialPageName;

		private int m_sharedDSContainerCollectionIndex = -1;

		private int m_dataPipelineCount;

		private string m_defaultFontFamily;

		[NonSerialized]
		private DataSource m_sharedDSContainer;

		[NonSerialized]
		private int m_lastAggregateID = -1;

		[NonSerialized]
		private int m_lastLookupID = -1;

		[NonSerialized]
		private double m_topLeftDataRegionAbsTop;

		[NonSerialized]
		private double m_topLeftDataRegionAbsLeft;

		[NonSerialized]
		private static readonly Declaration m_Declaration = Report.GetDeclaration();

		[NonSerialized]
		private ReportExprHost m_exprHost;

		[NonSerialized]
		private Dictionary<string, DataSet> m_mappingNameToDataSet;

		[NonSerialized]
		private List<int> m_mappingDataSetIndexToDataSourceIndex;

		[NonSerialized]
		private List<DataSet> m_mappingDataSetIndexToDataSet;

		[NonSerialized]
		private bool m_reportOrDescendentHasUserSortFilter;

		internal override AspNetCore.ReportingServices.ReportProcessing.ObjectType ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.Report;
			}
		}

		internal override string DataElementNameDefault
		{
			get
			{
				return "Report";
			}
		}

		internal bool ConsumeContainerWhitespace
		{
			get
			{
				return this.m_consumeContainerWhitespace;
			}
			set
			{
				this.m_consumeContainerWhitespace = value;
			}
		}

		internal string Author
		{
			get
			{
				return this.m_author;
			}
			set
			{
				this.m_author = value;
			}
		}

		internal string DefaultFontFamily
		{
			get
			{
				return this.m_defaultFontFamily;
			}
			set
			{
				this.m_defaultFontFamily = value;
			}
		}

		internal ExpressionInfo AutoRefreshExpression
		{
			get
			{
				return this.m_autoRefreshExpression;
			}
			set
			{
				this.m_autoRefreshExpression = value;
			}
		}

		internal Dictionary<string, ImageInfo> EmbeddedImages
		{
			get
			{
				return this.m_embeddedImages;
			}
			set
			{
				this.m_embeddedImages = value;
			}
		}

		internal List<DataSource> DataSources
		{
			get
			{
				return this.m_dataSources;
			}
			set
			{
				this.m_dataSources = value;
			}
		}

		internal int DataSourceCount
		{
			get
			{
				if (this.m_dataSources != null)
				{
					return this.m_dataSources.Count;
				}
				return 0;
			}
		}

		internal int DataSetCount
		{
			get
			{
				if (this.MappingNameToDataSet != null)
				{
					return this.MappingNameToDataSet.Count;
				}
				return 0;
			}
		}

		internal int DataPipelineCount
		{
			get
			{
				return this.m_dataPipelineCount;
			}
			set
			{
				this.m_dataPipelineCount = value;
			}
		}

		internal DataSource SharedDSContainer
		{
			get
			{
				if (this.m_sharedDSContainer == null && this.m_sharedDSContainerCollectionIndex >= 0 && this.m_dataSources != null)
				{
					this.m_sharedDSContainer = this.m_dataSources[this.m_sharedDSContainerCollectionIndex];
				}
				return this.m_sharedDSContainer;
			}
			set
			{
				this.m_sharedDSContainer = value;
			}
		}

		internal int SharedDSContainerCollectionIndex
		{
			get
			{
				return this.m_sharedDSContainerCollectionIndex;
			}
			set
			{
				this.m_sharedDSContainerCollectionIndex = value;
			}
		}

		internal bool HasSharedDataSetReferences
		{
			get
			{
				return -1 != this.m_sharedDSContainerCollectionIndex;
			}
		}

		internal bool MergeOnePass
		{
			get
			{
				return this.m_mergeOnePass;
			}
			set
			{
				this.m_mergeOnePass = value;
			}
		}

		internal bool SubReportMergeTransactions
		{
			get
			{
				return this.m_subReportMergeTransactions;
			}
			set
			{
				this.m_subReportMergeTransactions = value;
			}
		}

		internal bool NeedPostGroupProcessing
		{
			get
			{
				if (!this.m_needPostGroupProcessing)
				{
					return this.HasVariables;
				}
				return true;
			}
			set
			{
				this.m_needPostGroupProcessing = value;
			}
		}

		internal bool HasPostSortAggregates
		{
			get
			{
				return this.m_hasPostSortAggregates;
			}
			set
			{
				this.m_hasPostSortAggregates = value;
			}
		}

		internal bool HasAggregatesOfAggregates
		{
			get
			{
				return this.m_hasAggregatesOfAggregates;
			}
			set
			{
				this.m_hasAggregatesOfAggregates = value;
			}
		}

		internal bool HasAggregatesOfAggregatesInUserSort
		{
			get
			{
				return this.m_hasAggregatesOfAggregatesInUserSort;
			}
			set
			{
				this.m_hasAggregatesOfAggregatesInUserSort = value;
			}
		}

		internal bool HasReportItemReferences
		{
			get
			{
				return this.m_hasReportItemReferences;
			}
			set
			{
				this.m_hasReportItemReferences = value;
			}
		}

		internal int DataSetsNotOnlyUsedInParameters
		{
			get
			{
				return this.m_dataSetsNotOnlyUsedInParameters;
			}
			set
			{
				this.m_dataSetsNotOnlyUsedInParameters = value;
			}
		}

		internal ShowHideTypes ShowHideType
		{
			get
			{
				return this.m_showHideType;
			}
			set
			{
				this.m_showHideType = value;
			}
		}

		internal bool ParametersNotUsedInQuery
		{
			get
			{
				return this.m_parametersNotUsedInQuery;
			}
			set
			{
				this.m_parametersNotUsedInQuery = value;
			}
		}

		internal int LastID
		{
			get
			{
				return this.m_lastID;
			}
			set
			{
				this.m_lastID = value;
			}
		}

		internal List<SubReport> SubReports
		{
			get
			{
				return this.m_subReports;
			}
			set
			{
				this.m_subReports = value;
			}
		}

		internal bool HasImageStreams
		{
			get
			{
				return this.m_hasImageStreams;
			}
			set
			{
				this.m_hasImageStreams = value;
			}
		}

		internal bool HasLabels
		{
			get
			{
				return this.m_hasLabels;
			}
			set
			{
				this.m_hasLabels = value;
			}
		}

		internal bool HasBookmarks
		{
			get
			{
				return this.m_hasBookmarks;
			}
			set
			{
				this.m_hasBookmarks = value;
			}
		}

		internal bool HasHeadersOrFooters
		{
			get
			{
				return this.m_hasHeadersOrFooters;
			}
			set
			{
				this.m_hasHeadersOrFooters = value;
			}
		}

		internal List<ParameterDef> Parameters
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

		internal string OneDataSetName
		{
			get
			{
				return this.m_oneDataSetName;
			}
			set
			{
				this.m_oneDataSetName = value;
			}
		}

		internal bool HasSpecialRecursiveAggregates
		{
			get
			{
				return this.m_hasSpecialRecursiveAggregates;
			}
			set
			{
				this.m_hasSpecialRecursiveAggregates = value;
			}
		}

		internal bool HasPreviousAggregates
		{
			get
			{
				return this.m_hasPreviousAggregates;
			}
			set
			{
				this.m_hasPreviousAggregates = value;
			}
		}

		internal bool HasVariables
		{
			get
			{
				if (this.m_variables == null)
				{
					return null != this.m_groupsWithVariables;
				}
				return true;
			}
		}

		internal bool HasLookups
		{
			get
			{
				return this.m_hasLookups;
			}
			set
			{
				this.m_hasLookups = value;
			}
		}

		internal ExpressionInfo Language
		{
			get
			{
				return this.m_language;
			}
			set
			{
				this.m_language = value;
			}
		}

		internal ReportExprHost ReportExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal string DataTransform
		{
			get
			{
				return this.m_dataTransform;
			}
			set
			{
				this.m_dataTransform = value;
			}
		}

		internal string DataSchema
		{
			get
			{
				return this.m_dataSchema;
			}
			set
			{
				this.m_dataSchema = value;
			}
		}

		internal bool DataElementStyleAttribute
		{
			get
			{
				return this.m_dataElementStyleAttribute;
			}
			set
			{
				this.m_dataElementStyleAttribute = value;
			}
		}

		internal string Code
		{
			get
			{
				return this.m_code;
			}
			set
			{
				this.m_code = value;
			}
		}

		internal bool HasUserSortFilter
		{
			get
			{
				return this.m_hasUserSortFilter;
			}
			set
			{
				this.m_hasUserSortFilter = value;
			}
		}

		internal bool ReportOrDescendentHasUserSortFilter
		{
			get
			{
				return this.m_reportOrDescendentHasUserSortFilter;
			}
			set
			{
				this.m_reportOrDescendentHasUserSortFilter = value;
			}
		}

		internal InScopeSortFilterHashtable NonDetailSortFiltersInScope
		{
			get
			{
				return this.m_nonDetailSortFiltersInScope;
			}
			set
			{
				this.m_nonDetailSortFiltersInScope = value;
			}
		}

		internal InScopeSortFilterHashtable DetailSortFiltersInScope
		{
			get
			{
				return this.m_detailSortFiltersInScope;
			}
			set
			{
				this.m_detailSortFiltersInScope = value;
			}
		}

		internal int LastAggregateID
		{
			get
			{
				return this.m_lastAggregateID;
			}
			set
			{
				this.m_lastAggregateID = value;
			}
		}

		internal int LastLookupID
		{
			get
			{
				return this.m_lastLookupID;
			}
			set
			{
				this.m_lastLookupID = value;
			}
		}

		internal List<Variable> Variables
		{
			get
			{
				return this.m_variables;
			}
			set
			{
				this.m_variables = value;
			}
		}

		internal bool DeferVariableEvaluation
		{
			get
			{
				return this.m_deferVariableEvaluation;
			}
			set
			{
				this.m_deferVariableEvaluation = value;
			}
		}

		internal bool HasSubReports
		{
			get
			{
				if (this.m_subReports != null)
				{
					return this.m_subReports.Count > 0;
				}
				return false;
			}
		}

		internal Dictionary<string, DataSet> MappingNameToDataSet
		{
			get
			{
				if (this.m_mappingNameToDataSet == null)
				{
					this.GenerateDataSetMappings();
				}
				return this.m_mappingNameToDataSet;
			}
		}

		internal List<int> MappingDataSetIndexToDataSourceIndex
		{
			get
			{
				if (this.m_mappingDataSetIndexToDataSourceIndex == null)
				{
					this.GenerateDataSetMappings();
				}
				return this.m_mappingDataSetIndexToDataSourceIndex;
			}
		}

		internal List<DataSet> MappingDataSetIndexToDataSet
		{
			get
			{
				if (this.m_mappingDataSetIndexToDataSet == null)
				{
					this.GenerateDataSetMappings();
				}
				return this.m_mappingDataSetIndexToDataSet;
			}
		}

		internal List<DataRegion> TopLevelDataRegions
		{
			get
			{
				return this.m_topLevelDataRegions;
			}
			set
			{
				this.m_topLevelDataRegions = value;
			}
		}

		internal DataSet FirstDataSet
		{
			get
			{
				return this.m_firstDataSet;
			}
			set
			{
				this.m_firstDataSet = value;
			}
		}

		internal int FirstDataSetIndexToProcess
		{
			get
			{
				return this.m_firstDataSetIndexToProcess;
			}
		}

		internal List<IInScopeEventSource> InScopeEventSources
		{
			get
			{
				return this.m_inScopeEventSources;
			}
		}

		internal List<IInScopeEventSource> EventSources
		{
			get
			{
				return this.m_eventSources;
			}
		}

		internal List<ReportHierarchyNode> GroupsWithVariables
		{
			get
			{
				return this.m_groupsWithVariables;
			}
		}

		internal List<ReportSection> ReportSections
		{
			get
			{
				return this.m_reportSections;
			}
			set
			{
				this.m_reportSections = value;
			}
		}

		internal ExpressionInfo InitialPageName
		{
			get
			{
				return this.m_initialPageName;
			}
			set
			{
				this.m_initialPageName = value;
			}
		}

		bool IRIFReportScope.NeedToCacheDataRows
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		AspNetCore.ReportingServices.ReportProcessing.ObjectType IExpressionHostAssemblyHolder.ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.Report;
			}
		}

		string IExpressionHostAssemblyHolder.ExprHostAssemblyName
		{
			get
			{
				return $"Compiler_{this.m_reportVersion.ToString().Replace("-", "")}";
			}
		}

		List<string> IExpressionHostAssemblyHolder.CodeModules
		{
			get
			{
				return this.m_codeModules;
			}
			set
			{
				this.m_codeModules = value;
			}
		}

		List<CodeClass> IExpressionHostAssemblyHolder.CodeClasses
		{
			get
			{
				return this.m_codeClasses;
			}
			set
			{
				this.m_codeClasses = value;
			}
		}

		byte[] IExpressionHostAssemblyHolder.CompiledCode
		{
			get
			{
				return this.m_exprCompiledCode;
			}
			set
			{
				this.m_exprCompiledCode = value;
			}
		}

		bool IExpressionHostAssemblyHolder.CompiledCodeGeneratedWithRefusedPermissions
		{
			get
			{
				return this.m_exprCompiledCodeGeneratedWithRefusedPermissions;
			}
			set
			{
				this.m_exprCompiledCodeGeneratedWithRefusedPermissions = value;
			}
		}

		internal Report()
			: base(null)
		{
		}

		internal Report(int id, int idForReportItems)
			: base(id, null)
		{
			this.m_reportVersion = Guid.NewGuid();
			base.m_height = "11in";
			base.m_width = "8.5in";
			this.m_dataSources = new List<DataSource>();
			this.m_exprCompiledCode = new byte[0];
		}

		internal Report(ReportItem parent)
			: base(parent)
		{
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.Location = AspNetCore.ReportingServices.ReportPublishing.LocationFlags.None;
			context.ObjectType = this.ObjectType;
			context.ObjectName = null;
			if (this.m_variables != null && this.m_variables.Count != 0)
			{
				context.RegisterVariables(this.m_variables);
				context.ExprHostBuilder.VariableValuesStart();
				for (int i = 0; i < this.m_variables.Count; i++)
				{
					Variable variable = this.m_variables[i];
					variable.Initialize(context);
					context.ExprHostBuilder.VariableValueExpression(variable.Value);
				}
				context.ExprHostBuilder.VariableValuesEnd();
			}
			this.AllocateDatasetDependencyMatrix();
			base.Initialize(context);
			if (this.m_language != null)
			{
				this.m_language.Initialize("Language", context);
				context.ExprHostBuilder.ReportLanguage(this.m_language);
			}
			if (this.m_autoRefreshExpression != null)
			{
				this.m_autoRefreshExpression.Initialize("AutoRefresh", context);
				context.ExprHostBuilder.ReportAutoRefresh(this.m_autoRefreshExpression);
			}
			context.ReportDataElementStyleAttribute = this.m_dataElementStyleAttribute;
			if (this.m_dataSources != null)
			{
				for (int j = 0; j < this.m_dataSources.Count; j++)
				{
					Global.Tracer.Assert(null != this.m_dataSources[j], "(null != m_dataSources[i])");
					this.m_dataSources[j].Initialize(context);
				}
			}
			this.m_variablesInScope = context.GetCurrentReferencableVariables();
			if (this.m_reportSections != null)
			{
				for (int k = 0; k < this.m_reportSections.Count; k++)
				{
					this.m_reportSections[k].Initialize(context);
				}
			}
			if (context.ExprHostBuilder.CustomCode)
			{
				context.ExprHostBuilder.CustomCodeProxyStart();
				if (this.m_codeClasses != null && this.m_codeClasses.Count > 0)
				{
					for (int num = this.m_codeClasses.Count - 1; num >= 0; num--)
					{
						CodeClass codeClass = this.m_codeClasses[num];
						context.EnforceRdlSandboxContentRestrictions(codeClass);
						context.ExprHostBuilder.CustomCodeClassInstance(codeClass.ClassName, codeClass.InstanceName, num);
					}
				}
				if (this.m_code != null && this.m_code.Length > 0)
				{
					context.ExprHostBuilder.ReportCode(this.m_code);
				}
				context.ExprHostBuilder.CustomCodeProxyEnd();
			}
			if (this.m_initialPageName != null)
			{
				this.m_initialPageName.Initialize("InitialPageName", context);
				context.ExprHostBuilder.ReportInitialPageName(this.m_initialPageName);
			}
			if (this.m_variables != null)
			{
				foreach (Variable variable2 in this.m_variables)
				{
					context.UnregisterVariable(variable2);
				}
			}
			if (this.m_dataSources != null)
			{
				foreach (DataSource dataSource in this.m_dataSources)
				{
					dataSource.DetermineDecomposability(context);
				}
			}
			return false;
		}

		internal void BindAndValidateDataSetDefaultRelationships(ErrorContext errorContext)
		{
			foreach (DataSet item in this.MappingDataSetIndexToDataSet)
			{
				if (item != null)
				{
					item.BindAndValidateDefaultRelationships(errorContext, this);
				}
			}
		}

		internal ScopeTree BuildScopeTree()
		{
			return ScopeTreeBuilder.BuildScopeTree(this);
		}

		internal override void TraverseScopes(IRIFScopeVisitor visitor)
		{
			if (this.m_reportSections != null)
			{
				foreach (ReportSection reportSection in this.m_reportSections)
				{
					reportSection.TraverseScopes(visitor);
				}
			}
		}

		internal void UpdateTopLeftDataRegion(InitializationContext context, DataRegion dataRegion)
		{
			if (this.m_topLeftDataRegion != null && !(this.m_topLeftDataRegionAbsTop > context.CurrentAbsoluteTop))
			{
				if (0.0 != Math.Round(this.m_topLeftDataRegionAbsTop - context.CurrentAbsoluteTop, 10))
				{
					return;
				}
				if (!(this.m_topLeftDataRegionAbsLeft > context.CurrentAbsoluteLeft))
				{
					return;
				}
			}
			this.m_topLeftDataRegion = dataRegion;
			this.m_topLeftDataRegionAbsTop = context.CurrentAbsoluteTop;
			this.m_topLeftDataRegionAbsLeft = context.CurrentAbsoluteLeft;
		}

		bool IRIFReportScope.TextboxInScope(int sequenceIndex)
		{
			return false;
		}

		void IRIFReportScope.AddInScopeTextBox(TextBox textbox)
		{
			Global.Tracer.Assert(false);
		}

		void IRIFReportScope.ResetTextBoxImpls(OnDemandProcessingContext context)
		{
		}

		bool IRIFReportScope.VariableInScope(int sequenceIndex)
		{
			return SequenceIndex.GetBit(this.m_variablesInScope, sequenceIndex, true);
		}

		void IRIFReportScope.AddInScopeEventSource(IInScopeEventSource eventSource)
		{
			if (this.m_inScopeEventSources == null)
			{
				this.m_inScopeEventSources = new List<IInScopeEventSource>();
			}
			this.m_inScopeEventSources.Add(eventSource);
		}

		internal void AddEventSource(IInScopeEventSource eventSource)
		{
			if (this.m_eventSources == null)
			{
				this.m_eventSources = new List<IInScopeEventSource>();
			}
			this.m_eventSources.Add(eventSource);
		}

		internal void AddGroupWithVariables(ReportHierarchyNode node)
		{
			if (this.m_groupsWithVariables == null)
			{
				this.m_groupsWithVariables = new List<ReportHierarchyNode>();
			}
			this.m_groupsWithVariables.Add(node);
		}

		private void AllocateDatasetDependencyMatrix()
		{
			if (this.MappingNameToDataSet != null)
			{
				int dataSetCount = this.DataSetCount;
				if (dataSetCount < 10000)
				{
					this.m_flattenedDatasetDependencyMatrix = new byte[(int)Math.Ceiling((double)(dataSetCount * dataSetCount) / 8.0)];
				}
			}
		}

		private void CalculateOffsetAndMask(int datasetIndex, int referencedDatasetIndex, out int byteOffset, out byte bitMask)
		{
			int dataSetCount = this.DataSetCount;
			byteOffset = dataSetCount * datasetIndex + referencedDatasetIndex;
			byte b = (byte)(byteOffset % 8);
			bitMask = (byte)(SequenceIndex.BitMask001 << (int)b);
			byteOffset >>= 3;
		}

		internal void SetDatasetDependency(int datasetIndex, int referencedDatasetIndex, bool clearDependency)
		{
			if (this.m_flattenedDatasetDependencyMatrix != null)
			{
				int num = default(int);
				byte b = default(byte);
				this.CalculateOffsetAndMask(datasetIndex, referencedDatasetIndex, out num, out b);
				if (clearDependency)
				{
					b = (byte)(b ^ SequenceIndex.BitMask255);
					this.m_flattenedDatasetDependencyMatrix[num] &= b;
				}
				else
				{
					this.m_flattenedDatasetDependencyMatrix[num] |= b;
				}
			}
		}

		internal bool HasDatasetDependency(int datasetIndex, int referencedDatasetIndex)
		{
			if (this.m_flattenedDatasetDependencyMatrix == null)
			{
				return false;
			}
			int num = default(int);
			byte b = default(byte);
			this.CalculateOffsetAndMask(datasetIndex, referencedDatasetIndex, out num, out b);
			return (this.m_flattenedDatasetDependencyMatrix[num] & b) > 0;
		}

		internal void ClearDatasetParameterOnlyDependencies(int datasetIndex)
		{
			if (this.m_flattenedDatasetDependencyMatrix != null)
			{
				int dataSetCount = this.DataSetCount;
				for (int i = 0; i < dataSetCount; i++)
				{
					this.SetDatasetDependency(i, datasetIndex, true);
					this.SetDatasetDependency(datasetIndex, i, true);
				}
			}
		}

		internal int CalculateDatasetRootIndex(int suggestedRootIndex, bool[] exclusionList, int unprocessedDataSetCount)
		{
			if (this.m_flattenedDatasetDependencyMatrix == null)
			{
				return suggestedRootIndex;
			}
			int dataSetCount = this.DataSetCount;
			if (exclusionList == null)
			{
				exclusionList = new bool[dataSetCount];
				unprocessedDataSetCount = dataSetCount;
			}
			if (!exclusionList[suggestedRootIndex])
			{
				exclusionList[suggestedRootIndex] = true;
				unprocessedDataSetCount--;
			}
			int num = -1;
			while (++num < dataSetCount && unprocessedDataSetCount > 0)
			{
				if (!exclusionList[num] && this.HasDatasetDependency(suggestedRootIndex, num))
				{
					suggestedRootIndex = num;
					exclusionList[num] = true;
					unprocessedDataSetCount--;
					num = -1;
				}
			}
			return suggestedRootIndex;
		}

		internal void Phase4_DetermineFirstDatasetToProcess()
		{
			if (this.m_topLeftDataRegion == null)
			{
				this.m_firstDataSetIndexToProcess = 0;
			}
			else
			{
				int indexInCollection = this.m_topLeftDataRegion.GetDataSet(this).IndexInCollection;
				this.m_firstDataSetIndexToProcess = this.CalculateDatasetRootIndex(indexInCollection, null, -1);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ReportVersion, Token.Guid));
			list.Add(new MemberInfo(MemberName.Author, Token.String));
			list.Add(new MemberInfo(MemberName.AutoRefresh, Token.Int32));
			list.Add(new MemberInfo(MemberName.EmbeddedImages, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringRIFObjectDictionary, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ImageInfo));
			list.Add(new ReadOnlyMemberInfo(MemberName.Page, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Page));
			list.Add(new ReadOnlyMemberInfo(MemberName.ReportItems, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItemCollection));
			list.Add(new MemberInfo(MemberName.DataSources, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSource));
			list.Add(new ReadOnlyMemberInfo(MemberName.PageAggregates, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateInfo));
			list.Add(new MemberInfo(MemberName.CompiledCode, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Byte));
			list.Add(new MemberInfo(MemberName.MergeOnePass, Token.Boolean));
			list.Add(new ReadOnlyMemberInfo(MemberName.PageMergeOnePass, Token.Boolean));
			list.Add(new MemberInfo(MemberName.SubReportMergeTransactions, Token.Boolean));
			list.Add(new MemberInfo(MemberName.NeedPostGroupProcessing, Token.Boolean));
			list.Add(new MemberInfo(MemberName.HasPostSortAggregates, Token.Boolean));
			list.Add(new MemberInfo(MemberName.HasReportItemReferences, Token.Boolean));
			list.Add(new MemberInfo(MemberName.ShowHideType, Token.Enum));
			list.Add(new MemberInfo(MemberName.LastID, Token.Int32));
			list.Add(new MemberInfo(MemberName.SubReports, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReport));
			list.Add(new MemberInfo(MemberName.HasImageStreams, Token.Boolean));
			list.Add(new MemberInfo(MemberName.HasLabels, Token.Boolean));
			list.Add(new MemberInfo(MemberName.HasBookmarks, Token.Boolean));
			list.Add(new MemberInfo(MemberName.ParametersNotUsedInQuery, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Parameters, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterDef));
			list.Add(new MemberInfo(MemberName.DataSetName, Token.String));
			list.Add(new MemberInfo(MemberName.CodeModules, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.String));
			list.Add(new MemberInfo(MemberName.CodeClasses, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CodeClass));
			list.Add(new MemberInfo(MemberName.HasSpecialRecursiveAggregates, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Language, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DataTransform, Token.String));
			list.Add(new MemberInfo(MemberName.DataSchema, Token.String));
			list.Add(new MemberInfo(MemberName.DataElementStyleAttribute, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Code, Token.String));
			list.Add(new MemberInfo(MemberName.HasUserSortFilter, Token.Boolean));
			list.Add(new MemberInfo(MemberName.CompiledCodeGeneratedWithRefusedPermissions, Token.Boolean));
			list.Add(new MemberInfo(MemberName.NonDetailSortFiltersInScope, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Int32PrimitiveListHashtable));
			list.Add(new MemberInfo(MemberName.DetailSortFiltersInScope, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Int32PrimitiveListHashtable));
			list.Add(new MemberInfo(MemberName.Variables, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Variable));
			list.Add(new MemberInfo(MemberName.DeferVariableEvaluation, Token.Boolean));
			list.Add(new MemberInfo(MemberName.DataRegions, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegion));
			list.Add(new MemberInfo(MemberName.FirstDataSet, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSet, Token.Reference));
			list.Add(new MemberInfo(MemberName.TopLeftDataRegion, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegion, Token.Reference));
			list.Add(new MemberInfo(MemberName.DataSetsNotOnlyUsedInParameters, Token.Int32));
			list.Add(new MemberInfo(MemberName.HasPreviousAggregates, Token.Boolean));
			list.Add(new ReadOnlyMemberInfo(MemberName.InScopeTextBoxesInBody, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TextBox));
			list.Add(new ReadOnlyMemberInfo(MemberName.InScopeTextBoxesInPage, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TextBox));
			list.Add(new MemberInfo(MemberName.InScopeEventSources, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IInScopeEventSource));
			list.Add(new MemberInfo(MemberName.EventSources, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IInScopeEventSource));
			list.Add(new MemberInfo(MemberName.GroupsWithVariables, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportHierarchyNode));
			list.Add(new MemberInfo(MemberName.ConsumeContainerWhitespace, Token.Boolean));
			list.Add(new MemberInfo(MemberName.FlattenedDatasetDependencyMatrix, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Byte));
			list.Add(new MemberInfo(MemberName.FirstDataSetIndexToProcess, Token.Int32));
			list.Add(new ReadOnlyMemberInfo(MemberName.TextboxesInScope, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Byte));
			list.Add(new MemberInfo(MemberName.VariablesInScope, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Byte));
			list.Add(new MemberInfo(MemberName.HasLookups, Token.Boolean));
			list.Add(new MemberInfo(MemberName.ReportSections, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportSection));
			list.Add(new MemberInfo(MemberName.HasHeadersOrFooters, Token.Boolean));
			list.Add(new MemberInfo(MemberName.AutoRefreshExpression, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.InitialPageName, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.HasAggregatesOfAggregates, Token.Boolean));
			list.Add(new MemberInfo(MemberName.HasAggregatesOfAggregatesInUserSort, Token.Boolean));
			list.Add(new MemberInfo(MemberName.SharedDSContainerCollectionIndex, Token.Int32));
			list.Add(new MemberInfo(MemberName.DataPipelineCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.DefaultFontFamily, Token.String));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Report, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(Report.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ReportVersion:
					writer.Write(this.m_reportVersion);
					break;
				case MemberName.Author:
					writer.Write(this.m_author);
					break;
				case MemberName.AutoRefresh:
					writer.Write(this.m_autoRefresh);
					break;
				case MemberName.AutoRefreshExpression:
					writer.Write(this.m_autoRefreshExpression);
					break;
				case MemberName.EmbeddedImages:
					writer.WriteStringRIFObjectDictionary(this.m_embeddedImages);
					break;
				case MemberName.DataSources:
					writer.Write(this.m_dataSources);
					break;
				case MemberName.CompiledCode:
					writer.Write(this.m_exprCompiledCode);
					break;
				case MemberName.MergeOnePass:
					writer.Write(this.m_mergeOnePass);
					break;
				case MemberName.SubReportMergeTransactions:
					writer.Write(this.m_subReportMergeTransactions);
					break;
				case MemberName.NeedPostGroupProcessing:
					writer.Write(this.m_needPostGroupProcessing);
					break;
				case MemberName.HasPostSortAggregates:
					writer.Write(this.m_hasPostSortAggregates);
					break;
				case MemberName.HasReportItemReferences:
					writer.Write(this.m_hasReportItemReferences);
					break;
				case MemberName.ShowHideType:
					writer.WriteEnum((int)this.m_showHideType);
					break;
				case MemberName.LastID:
					writer.Write(this.m_lastID);
					break;
				case MemberName.SubReports:
					writer.WriteListOfReferences(this.m_subReports);
					break;
				case MemberName.HasImageStreams:
					writer.Write(this.m_hasImageStreams);
					break;
				case MemberName.HasLabels:
					writer.Write(this.m_hasLabels);
					break;
				case MemberName.HasBookmarks:
					writer.Write(this.m_hasBookmarks);
					break;
				case MemberName.ParametersNotUsedInQuery:
					writer.Write(this.m_parametersNotUsedInQuery);
					break;
				case MemberName.Parameters:
					writer.Write(this.m_parameters);
					break;
				case MemberName.DataSetName:
					writer.Write(this.m_oneDataSetName);
					break;
				case MemberName.CodeModules:
					writer.WriteListOfPrimitives(this.m_codeModules);
					break;
				case MemberName.CodeClasses:
					writer.Write(this.m_codeClasses);
					break;
				case MemberName.HasSpecialRecursiveAggregates:
					writer.Write(this.m_hasSpecialRecursiveAggregates);
					break;
				case MemberName.Language:
					writer.Write(this.m_language);
					break;
				case MemberName.DataTransform:
					writer.Write(this.m_dataTransform);
					break;
				case MemberName.DataSchema:
					writer.Write(this.m_dataSchema);
					break;
				case MemberName.DataElementStyleAttribute:
					writer.Write(this.m_dataElementStyleAttribute);
					break;
				case MemberName.Code:
					writer.Write(this.m_code);
					break;
				case MemberName.HasUserSortFilter:
					writer.Write(this.m_hasUserSortFilter);
					break;
				case MemberName.CompiledCodeGeneratedWithRefusedPermissions:
					writer.Write(this.m_exprCompiledCodeGeneratedWithRefusedPermissions);
					break;
				case MemberName.NonDetailSortFiltersInScope:
					writer.WriteInt32PrimitiveListHashtable<int>(this.m_nonDetailSortFiltersInScope);
					break;
				case MemberName.DetailSortFiltersInScope:
					writer.WriteInt32PrimitiveListHashtable<int>(this.m_detailSortFiltersInScope);
					break;
				case MemberName.Variables:
					writer.Write(this.m_variables);
					break;
				case MemberName.DeferVariableEvaluation:
					writer.Write(this.m_deferVariableEvaluation);
					break;
				case MemberName.DataRegions:
					writer.WriteListOfReferences(this.m_topLevelDataRegions);
					break;
				case MemberName.FirstDataSet:
					writer.WriteReference(this.m_firstDataSet);
					break;
				case MemberName.TopLeftDataRegion:
					writer.WriteReference(this.m_topLeftDataRegion);
					break;
				case MemberName.DataSetsNotOnlyUsedInParameters:
					writer.Write(this.m_dataSetsNotOnlyUsedInParameters);
					break;
				case MemberName.HasPreviousAggregates:
					writer.Write(this.m_hasPreviousAggregates);
					break;
				case MemberName.InScopeEventSources:
					writer.WriteListOfReferences(this.m_inScopeEventSources);
					break;
				case MemberName.EventSources:
					writer.WriteListOfReferences(this.m_eventSources);
					break;
				case MemberName.GroupsWithVariables:
					writer.WriteListOfReferences(this.m_groupsWithVariables);
					break;
				case MemberName.ConsumeContainerWhitespace:
					writer.Write(this.m_consumeContainerWhitespace);
					break;
				case MemberName.FlattenedDatasetDependencyMatrix:
					writer.Write(this.m_flattenedDatasetDependencyMatrix);
					break;
				case MemberName.FirstDataSetIndexToProcess:
					writer.Write(this.m_firstDataSetIndexToProcess);
					break;
				case MemberName.VariablesInScope:
					writer.Write(this.m_variablesInScope);
					break;
				case MemberName.HasLookups:
					writer.Write(this.m_hasLookups);
					break;
				case MemberName.ReportSections:
					writer.Write(this.m_reportSections);
					break;
				case MemberName.HasHeadersOrFooters:
					writer.Write(this.m_hasHeadersOrFooters);
					break;
				case MemberName.InitialPageName:
					writer.Write(this.m_initialPageName);
					break;
				case MemberName.HasAggregatesOfAggregates:
					writer.Write(this.m_hasAggregatesOfAggregates);
					break;
				case MemberName.HasAggregatesOfAggregatesInUserSort:
					writer.Write(this.m_hasAggregatesOfAggregatesInUserSort);
					break;
				case MemberName.SharedDSContainerCollectionIndex:
					writer.Write(this.m_sharedDSContainerCollectionIndex);
					break;
				case MemberName.DataPipelineCount:
					writer.Write(this.m_dataPipelineCount);
					break;
				case MemberName.DefaultFontFamily:
					writer.Write(this.m_defaultFontFamily);
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
			reader.RegisterDeclaration(Report.m_Declaration);
			ReportSection reportSection = null;
			byte[] textboxesInScope = null;
			List<TextBox> inScopeTextBoxes = null;
			List<DataAggregateInfo> pageAggregates = null;
			if (reader.IntermediateFormatVersion.CompareTo(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.IntermediateFormatVersion.BIRefresh) < 0)
			{
				reportSection = new ReportSection(0);
				reportSection.Name = "ReportSection0";
				reportSection.Width = base.Width;
				reportSection.WidthValue = base.WidthValue;
				reportSection.DataElementName = reportSection.DataElementNameDefault;
				reportSection.DataElementOutput = reportSection.DataElementOutputDefault;
				reportSection.ExprHostID = 0;
				reportSection.ParentInstancePath = this;
				reportSection.Height = base.Height;
				reportSection.HeightValue = base.HeightValue;
				reportSection.StyleClass = base.StyleClass;
				this.m_reportSections = new List<ReportSection>();
				this.m_reportSections.Add(reportSection);
			}
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ReportVersion:
					this.m_reportVersion = reader.ReadGuid();
					break;
				case MemberName.Author:
					this.m_author = reader.ReadString();
					break;
				case MemberName.AutoRefresh:
					this.m_autoRefresh = reader.ReadInt32();
					break;
				case MemberName.AutoRefreshExpression:
					this.m_autoRefreshExpression = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.EmbeddedImages:
					this.m_embeddedImages = reader.ReadStringRIFObjectDictionary<ImageInfo>();
					break;
				case MemberName.Page:
				{
					Page page2 = reportSection.Page = (Page)reader.ReadRIFObject();
					reportSection.Page.ExprHostID = 0;
					bool flag = page2.UpgradedSnapshotPageHeaderEvaluation || page2.UpgradedSnapshotPageFooterEvaluation;
					reportSection.NeedsOverallTotalPages |= flag;
					reportSection.NeedsReportItemsOnPage |= flag;
					if (page2.PageHeader == null && page2.PageFooter == null)
					{
						break;
					}
					this.m_hasHeadersOrFooters = true;
					break;
				}
				case MemberName.ReportItems:
					reportSection.ReportItems = (ReportItemCollection)reader.ReadRIFObject();
					break;
				case MemberName.DataSources:
					this.m_dataSources = reader.ReadGenericListOfRIFObjects<DataSource>();
					break;
				case MemberName.PageAggregates:
					pageAggregates = reader.ReadGenericListOfRIFObjects<DataAggregateInfo>();
					break;
				case MemberName.CompiledCode:
					this.m_exprCompiledCode = reader.ReadByteArray();
					break;
				case MemberName.MergeOnePass:
					this.m_mergeOnePass = reader.ReadBoolean();
					break;
				case MemberName.PageMergeOnePass:
					reportSection.NeedsReportItemsOnPage |= !reader.ReadBoolean();
					break;
				case MemberName.SubReportMergeTransactions:
					this.m_subReportMergeTransactions = reader.ReadBoolean();
					break;
				case MemberName.NeedPostGroupProcessing:
					this.m_needPostGroupProcessing = reader.ReadBoolean();
					break;
				case MemberName.HasPostSortAggregates:
					this.m_hasPostSortAggregates = reader.ReadBoolean();
					break;
				case MemberName.HasReportItemReferences:
					this.m_hasReportItemReferences = reader.ReadBoolean();
					break;
				case MemberName.ShowHideType:
					this.m_showHideType = (ShowHideTypes)reader.ReadEnum();
					break;
				case MemberName.LastID:
					this.m_lastID = reader.ReadInt32();
					break;
				case MemberName.SubReports:
					this.m_subReports = reader.ReadGenericListOfReferences<SubReport>(this);
					break;
				case MemberName.HasImageStreams:
					this.m_hasImageStreams = reader.ReadBoolean();
					break;
				case MemberName.HasLabels:
					this.m_hasLabels = reader.ReadBoolean();
					break;
				case MemberName.HasBookmarks:
					this.m_hasBookmarks = reader.ReadBoolean();
					break;
				case MemberName.ParametersNotUsedInQuery:
					this.m_parametersNotUsedInQuery = reader.ReadBoolean();
					break;
				case MemberName.Parameters:
					this.m_parameters = reader.ReadGenericListOfRIFObjects<ParameterDef>();
					break;
				case MemberName.DataSetName:
					this.m_oneDataSetName = reader.ReadString();
					break;
				case MemberName.CodeModules:
					this.m_codeModules = reader.ReadListOfPrimitives<string>();
					break;
				case MemberName.CodeClasses:
					this.m_codeClasses = reader.ReadGenericListOfRIFObjects<CodeClass>();
					break;
				case MemberName.HasSpecialRecursiveAggregates:
					this.m_hasSpecialRecursiveAggregates = reader.ReadBoolean();
					break;
				case MemberName.Language:
					this.m_language = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DataTransform:
					this.m_dataTransform = reader.ReadString();
					break;
				case MemberName.DataSchema:
					this.m_dataSchema = reader.ReadString();
					break;
				case MemberName.DataElementStyleAttribute:
					this.m_dataElementStyleAttribute = reader.ReadBoolean();
					break;
				case MemberName.Code:
					this.m_code = reader.ReadString();
					break;
				case MemberName.HasUserSortFilter:
					this.m_hasUserSortFilter = reader.ReadBoolean();
					break;
				case MemberName.CompiledCodeGeneratedWithRefusedPermissions:
					this.m_exprCompiledCodeGeneratedWithRefusedPermissions = reader.ReadBoolean();
					break;
				case MemberName.NonDetailSortFiltersInScope:
					this.m_nonDetailSortFiltersInScope = reader.ReadInt32PrimitiveListHashtable<InScopeSortFilterHashtable, int>();
					break;
				case MemberName.DetailSortFiltersInScope:
					this.m_detailSortFiltersInScope = reader.ReadInt32PrimitiveListHashtable<InScopeSortFilterHashtable, int>();
					break;
				case MemberName.Variables:
					this.m_variables = reader.ReadGenericListOfRIFObjects<Variable>();
					break;
				case MemberName.DeferVariableEvaluation:
					this.m_deferVariableEvaluation = reader.ReadBoolean();
					break;
				case MemberName.DataRegions:
					this.m_topLevelDataRegions = reader.ReadGenericListOfReferences<DataRegion>(this);
					break;
				case MemberName.FirstDataSet:
					this.m_firstDataSet = reader.ReadReference<DataSet>(this);
					break;
				case MemberName.TopLeftDataRegion:
					this.m_topLeftDataRegion = reader.ReadReference<DataRegion>(this);
					break;
				case MemberName.DataSetsNotOnlyUsedInParameters:
					this.m_dataSetsNotOnlyUsedInParameters = reader.ReadInt32();
					break;
				case MemberName.HasPreviousAggregates:
					this.m_hasPreviousAggregates = reader.ReadBoolean();
					break;
				case MemberName.InScopeTextBoxesInBody:
					reportSection.SetInScopeTextBoxes(reader.ReadGenericListOfReferences<TextBox>(this));
					break;
				case MemberName.InScopeTextBoxesInPage:
					inScopeTextBoxes = reader.ReadGenericListOfReferences<TextBox>(this);
					break;
				case MemberName.InScopeEventSources:
					this.m_inScopeEventSources = reader.ReadGenericListOfReferences<IInScopeEventSource>(this);
					break;
				case MemberName.EventSources:
					this.m_eventSources = reader.ReadGenericListOfReferences<IInScopeEventSource>(this);
					break;
				case MemberName.GroupsWithVariables:
					this.m_groupsWithVariables = reader.ReadGenericListOfReferences<ReportHierarchyNode>(this);
					break;
				case MemberName.ConsumeContainerWhitespace:
					this.m_consumeContainerWhitespace = reader.ReadBoolean();
					break;
				case MemberName.FlattenedDatasetDependencyMatrix:
					this.m_flattenedDatasetDependencyMatrix = reader.ReadByteArray();
					break;
				case MemberName.FirstDataSetIndexToProcess:
					this.m_firstDataSetIndexToProcess = reader.ReadInt32();
					break;
				case MemberName.TextboxesInScope:
				{
					byte[] textboxesInScope2 = reader.ReadByteArray();
					reportSection.SetTextboxesInScope(textboxesInScope2);
					textboxesInScope = null;
					break;
				}
				case MemberName.VariablesInScope:
					this.m_variablesInScope = reader.ReadByteArray();
					break;
				case MemberName.HasLookups:
					this.m_hasLookups = reader.ReadBoolean();
					break;
				case MemberName.ReportSections:
					this.m_reportSections = reader.ReadGenericListOfRIFObjects<ReportSection>();
					break;
				case MemberName.HasHeadersOrFooters:
					this.m_hasHeadersOrFooters = reader.ReadBoolean();
					break;
				case MemberName.InitialPageName:
					this.m_initialPageName = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.HasAggregatesOfAggregates:
					this.m_hasAggregatesOfAggregates = reader.ReadBoolean();
					break;
				case MemberName.HasAggregatesOfAggregatesInUserSort:
					this.m_hasAggregatesOfAggregatesInUserSort = reader.ReadBoolean();
					break;
				case MemberName.SharedDSContainerCollectionIndex:
					this.m_sharedDSContainerCollectionIndex = reader.ReadInt32();
					break;
				case MemberName.DataPipelineCount:
					this.m_dataPipelineCount = reader.ReadInt32();
					break;
				case MemberName.DefaultFontFamily:
					this.m_defaultFontFamily = reader.ReadString();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
			if (reportSection != null)
			{
				reportSection.ID = ++this.m_lastID;
				reportSection.GlobalID = reportSection.ReportItems.GlobalID * -1;
				reportSection.Page.SetTextboxesInScope(textboxesInScope);
				reportSection.Page.SetInScopeTextBoxes(inScopeTextBoxes);
				reportSection.Page.PageAggregates = pageAggregates;
			}
			if (base.m_name == null)
			{
				base.m_name = "Report";
			}
			reader.ResolveReferences();
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(Report.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item4 in list)
				{
					switch (item4.MemberName)
					{
					case MemberName.SubReports:
					{
						if (this.m_subReports == null)
						{
							this.m_subReports = new List<SubReport>();
						}
						IReferenceable referenceable5 = default(IReferenceable);
						referenceableItems.TryGetValue(item4.RefID, out referenceable5);
						Global.Tracer.Assert(referenceable5 != null && referenceable5 is SubReport && !this.m_subReports.Contains((SubReport)referenceable5));
						this.m_subReports.Add((SubReport)referenceable5);
						break;
					}
					case MemberName.DataRegions:
					{
						if (this.m_topLevelDataRegions == null)
						{
							this.m_topLevelDataRegions = new List<DataRegion>();
						}
						IReferenceable referenceable6 = default(IReferenceable);
						referenceableItems.TryGetValue(item4.RefID, out referenceable6);
						Global.Tracer.Assert(referenceable6 != null && ((ReportItem)referenceable6).IsDataRegion && !this.m_topLevelDataRegions.Contains((DataRegion)referenceable6));
						this.m_topLevelDataRegions.Add((DataRegion)referenceable6);
						break;
					}
					case MemberName.FirstDataSet:
					{
						IReferenceable referenceable8 = default(IReferenceable);
						referenceableItems.TryGetValue(item4.RefID, out referenceable8);
						Global.Tracer.Assert(referenceable8 != null && referenceable8 is DataSet);
						this.m_firstDataSet = (DataSet)referenceable8;
						break;
					}
					case MemberName.TopLeftDataRegion:
					{
						IReferenceable referenceable2 = default(IReferenceable);
						referenceableItems.TryGetValue(item4.RefID, out referenceable2);
						Global.Tracer.Assert(referenceable2 != null && ((ReportItem)referenceable2).IsDataRegion);
						this.m_topLeftDataRegion = (DataRegion)referenceable2;
						break;
					}
					case MemberName.InScopeTextBoxesInBody:
					{
						Global.Tracer.Assert(this.m_reportSections != null && this.m_reportSections.Count == 1, "Expected single section");
						ReportSection reportSection2 = this.m_reportSections[0];
						IReferenceable referenceable9 = default(IReferenceable);
						referenceableItems.TryGetValue(item4.RefID, out referenceable9);
						TextBox textbox2 = (TextBox)referenceable9;
						reportSection2.AddInScopeTextBox(textbox2);
						break;
					}
					case MemberName.InScopeTextBoxesInPage:
					{
						Global.Tracer.Assert(this.m_reportSections != null && this.m_reportSections.Count == 1, "Expected single section");
						ReportSection reportSection = this.m_reportSections[0];
						IReferenceable referenceable3 = default(IReferenceable);
						referenceableItems.TryGetValue(item4.RefID, out referenceable3);
						TextBox textbox = (TextBox)referenceable3;
						reportSection.Page.AddInScopeTextBox(textbox);
						break;
					}
					case MemberName.InScopeEventSources:
					{
						if (this.m_inScopeEventSources == null)
						{
							this.m_inScopeEventSources = new List<IInScopeEventSource>();
						}
						IReferenceable referenceable7 = default(IReferenceable);
						referenceableItems.TryGetValue(item4.RefID, out referenceable7);
						IInScopeEventSource item3 = (IInScopeEventSource)referenceable7;
						this.m_inScopeEventSources.Add(item3);
						break;
					}
					case MemberName.EventSources:
					{
						if (this.m_eventSources == null)
						{
							this.m_eventSources = new List<IInScopeEventSource>();
						}
						IReferenceable referenceable4 = default(IReferenceable);
						referenceableItems.TryGetValue(item4.RefID, out referenceable4);
						IInScopeEventSource item2 = (IInScopeEventSource)referenceable4;
						this.m_eventSources.Add(item2);
						break;
					}
					case MemberName.GroupsWithVariables:
					{
						if (this.m_groupsWithVariables == null)
						{
							this.m_groupsWithVariables = new List<ReportHierarchyNode>();
						}
						IReferenceable referenceable = default(IReferenceable);
						referenceableItems.TryGetValue(item4.RefID, out referenceable);
						ReportHierarchyNode item = (ReportHierarchyNode)referenceable;
						this.m_groupsWithVariables.Add(item);
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Report;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			this.m_exprHost = reportExprHost;
			base.ReportItemSetExprHost(this.m_exprHost, reportObjectModel);
			if (reportExprHost.VariableValueHosts != null)
			{
				reportExprHost.VariableValueHosts.SetReportObjectModel(reportObjectModel);
			}
			for (int i = 0; i < this.m_reportSections.Count; i++)
			{
				this.m_reportSections[i].SetExprHost(reportExprHost, reportObjectModel);
			}
			if (reportExprHost.LookupExprHostsRemotable == null && reportExprHost.LookupDestExprHostsRemotable == null && reportExprHost.DataSetHostsRemotable == null)
			{
				return;
			}
			if (this.m_dataSources != null)
			{
				for (int j = 0; j < this.m_dataSources.Count; j++)
				{
					DataSource dataSource = this.m_dataSources[j];
					if (dataSource.DataSets != null)
					{
						for (int k = 0; k < dataSource.DataSets.Count; k++)
						{
							DataSet dataSet = dataSource.DataSets[k];
							dataSet.SetExprHost(reportExprHost, reportObjectModel);
						}
					}
				}
			}
		}

		internal int EvaluateAutoRefresh(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			if (this.m_autoRefreshExpression == null)
			{
				return Math.Max(0, this.m_autoRefresh);
			}
			if (this.m_autoRefresh < 0)
			{
				if (!this.m_autoRefreshExpression.IsExpression)
				{
					this.m_autoRefresh = this.m_autoRefreshExpression.IntValue;
				}
				else
				{
					context.SetupContext(this, romInstance);
					this.m_autoRefresh = Math.Max(0, context.ReportRuntime.EvaluateReportAutoRefreshExpression(this));
				}
			}
			return this.m_autoRefresh;
		}

		internal string EvaluateInitialPageName(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, instance);
			return context.ReportRuntime.EvaluateInitialPageNameExpression(this);
		}

		internal void RegisterDataSetScopedAggregates(OnDemandProcessingContext odpContext)
		{
			int count = this.MappingDataSetIndexToDataSet.Count;
			for (int i = 0; i < count; i++)
			{
				odpContext.RuntimeInitializeAggregates(this.MappingDataSetIndexToDataSet[i].Aggregates);
				odpContext.RuntimeInitializeAggregates(this.MappingDataSetIndexToDataSet[i].PostSortAggregates);
			}
		}

		private void GenerateDataSetMappings()
		{
			if (this.m_mappingNameToDataSet == null)
			{
				this.m_mappingNameToDataSet = new Dictionary<string, DataSet>();
				this.m_mappingDataSetIndexToDataSourceIndex = new List<int>();
				this.m_mappingDataSetIndexToDataSet = new List<DataSet>();
				int num = (this.m_dataSources != null) ? this.m_dataSources.Count : 0;
				for (int i = 0; i < num; i++)
				{
					DataSource dataSource = this.m_dataSources[i];
					int num2 = (dataSource.DataSets != null) ? dataSource.DataSets.Count : 0;
					for (int j = 0; j < num2; j++)
					{
						DataSet dataSet = dataSource.DataSets[j];
						this.AddDataSetMapping(i, dataSet);
					}
				}
			}
		}

		private void AddDataSetMapping(int dataSourceIndex, DataSet dataSet)
		{
			if (!this.m_mappingNameToDataSet.ContainsKey(dataSet.Name))
			{
				this.m_mappingNameToDataSet.Add(dataSet.Name, dataSet);
				for (int i = this.m_mappingDataSetIndexToDataSourceIndex.Count; i <= dataSet.IndexInCollection; i++)
				{
					this.m_mappingDataSetIndexToDataSourceIndex.Add(-1);
					this.m_mappingDataSetIndexToDataSet.Add(null);
				}
				this.m_mappingDataSetIndexToDataSourceIndex[dataSet.IndexInCollection] = dataSourceIndex;
				this.m_mappingDataSetIndexToDataSet[dataSet.IndexInCollection] = dataSet;
			}
		}
	}
}
