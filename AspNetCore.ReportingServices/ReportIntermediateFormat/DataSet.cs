using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class DataSet : IDOwner, IAggregateHolder, ISortFilterScope, IReferenceable, IPersistable, IGloballyReferenceable, IGlobalIDOwner, IRIFDataScope
	{
		internal enum TriState
		{
			Auto,
			True,
			False
		}

		internal const uint CompareFlag_Default = 0u;

		internal const uint CompareFlag_IgnoreCase = 1u;

		internal const uint CompareFlag_IgnoreNonSpace = 2u;

		internal const uint CompareFlag_IgnoreKanatype = 65536u;

		internal const uint CompareFlag_IgnoreWidth = 131072u;

		private DataSetCore m_dataSetCore;

		[Reference]
		private List<DataRegion> m_dataRegions;

		private List<DataAggregateInfo> m_aggregates;

		private List<LookupInfo> m_lookups;

		private List<LookupDestinationInfo> m_lookupDestinationInfos;

		private bool m_usedOnlyInParameters;

		private List<DataAggregateInfo> m_postSortAggregates;

		private bool m_hasDetailUserSortFilter;

		private List<ExpressionInfo> m_userSortExpressions;

		private bool m_hasSubReports;

		private int m_indexInCollection = -1;

		private bool m_hasScopeWithCustomAggregates;

		[Reference]
		private DataSource m_dataSource;

		private bool m_allowIncrementalProcessing = true;

		private List<DefaultRelationship> m_defaultRelationships;

		[NonSerialized]
		private bool m_usedOnlyInParametersSet;

		[NonSerialized]
		private bool[] m_isSortFilterTarget;

		[NonSerialized]
		private bool m_usedInAggregates;

		[NonSerialized]
		private bool? m_hasSameDataSetLookups;

		[NonSerialized]
		private static readonly Declaration m_Declaration = DataSet.GetDeclaration();

		internal AspNetCore.ReportingServices.ReportProcessing.ObjectType ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.DataSet;
			}
		}

		internal DataSetCore DataSetCore
		{
			get
			{
				return this.m_dataSetCore;
			}
			set
			{
				this.m_dataSetCore = value;
			}
		}

		public string Name
		{
			get
			{
				return this.m_dataSetCore.Name;
			}
			set
			{
				this.m_dataSetCore.Name = value;
			}
		}

		internal List<Field> Fields
		{
			get
			{
				return this.m_dataSetCore.Fields;
			}
			set
			{
				this.m_dataSetCore.Fields = value;
			}
		}

		internal bool HasAggregateIndicatorFields
		{
			get
			{
				return this.m_dataSetCore.HasAggregateIndicatorFields;
			}
		}

		internal ReportQuery Query
		{
			get
			{
				return this.m_dataSetCore.Query;
			}
			set
			{
				this.m_dataSetCore.Query = value;
			}
		}

		internal SharedDataSetQuery SharedDataSetQuery
		{
			get
			{
				return this.m_dataSetCore.SharedDataSetQuery;
			}
			set
			{
				this.m_dataSetCore.SharedDataSetQuery = value;
			}
		}

		internal bool IsReferenceToSharedDataSet
		{
			get
			{
				return null != this.m_dataSetCore.SharedDataSetQuery;
			}
		}

		internal TriState CaseSensitivity
		{
			get
			{
				return this.m_dataSetCore.CaseSensitivity;
			}
			set
			{
				this.m_dataSetCore.CaseSensitivity = value;
			}
		}

		internal string Collation
		{
			get
			{
				return this.m_dataSetCore.Collation;
			}
			set
			{
				this.m_dataSetCore.Collation = value;
			}
		}

		internal string CollationCulture
		{
			get
			{
				return this.m_dataSetCore.CollationCulture;
			}
			set
			{
				this.m_dataSetCore.CollationCulture = value;
			}
		}

		internal TriState AccentSensitivity
		{
			get
			{
				return this.m_dataSetCore.AccentSensitivity;
			}
			set
			{
				this.m_dataSetCore.AccentSensitivity = value;
			}
		}

		internal TriState KanatypeSensitivity
		{
			get
			{
				return this.m_dataSetCore.KanatypeSensitivity;
			}
			set
			{
				this.m_dataSetCore.KanatypeSensitivity = value;
			}
		}

		internal TriState WidthSensitivity
		{
			get
			{
				return this.m_dataSetCore.WidthSensitivity;
			}
			set
			{
				this.m_dataSetCore.WidthSensitivity = value;
			}
		}

		internal bool NullsAsBlanks
		{
			get
			{
				return this.m_dataSetCore.NullsAsBlanks;
			}
			set
			{
				this.m_dataSetCore.NullsAsBlanks = value;
			}
		}

		internal bool UseOrdinalStringKeyGeneration
		{
			get
			{
				return this.m_dataSetCore.UseOrdinalStringKeyGeneration;
			}
			set
			{
				this.m_dataSetCore.UseOrdinalStringKeyGeneration = value;
			}
		}

		internal List<Filter> Filters
		{
			get
			{
				return this.m_dataSetCore.Filters;
			}
			set
			{
				this.m_dataSetCore.Filters = value;
			}
		}

		internal List<DataRegion> DataRegions
		{
			get
			{
				return this.m_dataRegions;
			}
			set
			{
				this.m_dataRegions = value;
			}
		}

		internal List<DataAggregateInfo> Aggregates
		{
			get
			{
				return this.m_aggregates;
			}
			set
			{
				this.m_aggregates = value;
			}
		}

		internal List<LookupInfo> Lookups
		{
			get
			{
				return this.m_lookups;
			}
			set
			{
				this.m_lookups = value;
			}
		}

		internal List<LookupDestinationInfo> LookupDestinationInfos
		{
			get
			{
				return this.m_lookupDestinationInfos;
			}
			set
			{
				this.m_lookupDestinationInfos = value;
			}
		}

		internal bool HasLookups
		{
			get
			{
				return this.m_lookupDestinationInfos != null;
			}
		}

		internal bool HasSameDataSetLookups
		{
			get
			{
				if (!this.m_hasSameDataSetLookups.HasValue)
				{
					this.m_hasSameDataSetLookups = false;
					if (this.m_lookupDestinationInfos != null)
					{
						int num = 0;
						while (num < this.m_lookupDestinationInfos.Count)
						{
							if (!this.m_lookupDestinationInfos[num].UsedInSameDataSetTablixProcessing)
							{
								num++;
								continue;
							}
							this.m_hasSameDataSetLookups = true;
							break;
						}
					}
				}
				return this.m_hasSameDataSetLookups.Value;
			}
		}

		internal bool UsedOnlyInParametersSet
		{
			get
			{
				return this.m_usedOnlyInParametersSet;
			}
		}

		internal bool UsedOnlyInParameters
		{
			get
			{
				return this.m_usedOnlyInParameters;
			}
			set
			{
				if (!this.m_usedOnlyInParametersSet)
				{
					this.m_usedOnlyInParameters = value;
					this.m_usedOnlyInParametersSet = true;
				}
			}
		}

		internal int NonCalculatedFieldCount
		{
			get
			{
				return this.m_dataSetCore.NonCalculatedFieldCount;
			}
			set
			{
				this.m_dataSetCore.NonCalculatedFieldCount = value;
			}
		}

		internal List<DataAggregateInfo> PostSortAggregates
		{
			get
			{
				return this.m_postSortAggregates;
			}
			set
			{
				this.m_postSortAggregates = value;
			}
		}

		internal uint LCID
		{
			get
			{
				return this.m_dataSetCore.LCID;
			}
			set
			{
				this.m_dataSetCore.LCID = value;
			}
		}

		internal bool HasDetailUserSortFilter
		{
			get
			{
				return this.m_hasDetailUserSortFilter;
			}
			set
			{
				this.m_hasDetailUserSortFilter = value;
			}
		}

		internal List<ExpressionInfo> UserSortExpressions
		{
			get
			{
				return this.m_userSortExpressions;
			}
			set
			{
				this.m_userSortExpressions = value;
			}
		}

		internal DataSetExprHost ExprHost
		{
			get
			{
				return this.m_dataSetCore.ExprHost;
			}
		}

		internal bool[] IsSortFilterTarget
		{
			get
			{
				return this.m_isSortFilterTarget;
			}
			set
			{
				this.m_isSortFilterTarget = value;
			}
		}

		int ISortFilterScope.ID
		{
			get
			{
				return base.m_ID;
			}
		}

		string ISortFilterScope.ScopeName
		{
			get
			{
				return this.m_dataSetCore.Name;
			}
		}

		bool[] ISortFilterScope.IsSortFilterTarget
		{
			get
			{
				return this.m_isSortFilterTarget;
			}
			set
			{
				this.m_isSortFilterTarget = value;
			}
		}

		bool[] ISortFilterScope.IsSortFilterExpressionScope
		{
			get
			{
				return null;
			}
			set
			{
				Global.Tracer.Assert(false, string.Empty);
			}
		}

		List<ExpressionInfo> ISortFilterScope.UserSortExpressions
		{
			get
			{
				return this.m_userSortExpressions;
			}
			set
			{
				this.m_userSortExpressions = value;
			}
		}

		IndexedExprHost ISortFilterScope.UserSortExpressionsHost
		{
			get
			{
				if (this.m_dataSetCore.ExprHost == null)
				{
					return null;
				}
				return this.m_dataSetCore.ExprHost.UserSortExpressionsHost;
			}
		}

		internal bool UsedInAggregates
		{
			get
			{
				return this.m_usedInAggregates;
			}
			set
			{
				this.m_usedInAggregates = value;
			}
		}

		internal bool HasScopeWithCustomAggregates
		{
			get
			{
				return this.m_hasScopeWithCustomAggregates;
			}
			set
			{
				this.m_hasScopeWithCustomAggregates = value;
			}
		}

		internal TriState InterpretSubtotalsAsDetails
		{
			get
			{
				return this.m_dataSetCore.InterpretSubtotalsAsDetails;
			}
			set
			{
				this.m_dataSetCore.InterpretSubtotalsAsDetails = value;
			}
		}

		internal bool HasSubReports
		{
			get
			{
				return this.m_hasSubReports;
			}
			set
			{
				this.m_hasSubReports = value;
			}
		}

		internal int IndexInCollection
		{
			get
			{
				return this.m_indexInCollection;
			}
		}

		internal DataSource DataSource
		{
			get
			{
				return this.m_dataSource;
			}
			set
			{
				this.m_dataSource = value;
			}
		}

		internal List<DefaultRelationship> DefaultRelationships
		{
			get
			{
				return this.m_defaultRelationships;
			}
			set
			{
				this.m_defaultRelationships = value;
			}
		}

		internal bool AllowIncrementalProcessing
		{
			get
			{
				return this.m_allowIncrementalProcessing;
			}
			set
			{
				this.m_allowIncrementalProcessing = value;
			}
		}

		public DataScopeInfo DataScopeInfo
		{
			get
			{
				return null;
			}
		}

		AspNetCore.ReportingServices.ReportProcessing.ObjectType IRIFDataScope.DataScopeObjectType
		{
			get
			{
				return this.ObjectType;
			}
		}

		internal DataSet(int id, int indexCounter)
			: base(id)
		{
			this.m_indexInCollection = indexCounter;
			this.m_dataRegions = new List<DataRegion>();
			this.m_aggregates = new List<DataAggregateInfo>();
			this.m_postSortAggregates = new List<DataAggregateInfo>();
			this.m_dataSetCore = new DataSetCore();
			this.m_dataSetCore.Fields = new List<Field>();
		}

		internal DataSet()
		{
			this.m_dataSetCore = new DataSetCore();
		}

		internal DataSet(DataSetCore dataSetCore)
		{
			this.m_dataSetCore = dataSetCore;
		}

		internal CultureInfo CreateCultureInfoFromLcid()
		{
			return this.m_dataSetCore.CreateCultureInfoFromLcid();
		}

		internal void Initialize(InitializationContext context)
		{
			context.ObjectType = this.ObjectType;
			context.ObjectName = this.m_dataSetCore.Name;
			context.RegisterDataSet(this);
			this.InternalInitialize(context);
			context.UnRegisterDataSet(this);
		}

		private void InternalInitialize(InitializationContext context)
		{
			context.ExprHostBuilder.DataSetStart(this.m_dataSetCore.Name);
			context.Location |= AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataSet;
			this.m_dataSetCore.Initialize(context);
			if (this.m_defaultRelationships != null)
			{
				foreach (DefaultRelationship defaultRelationship in this.m_defaultRelationships)
				{
					defaultRelationship.Initialize(this, context);
				}
			}
			if (this.m_userSortExpressions != null)
			{
				context.ExprHostBuilder.UserSortExpressionsStart();
				foreach (ExpressionInfo userSortExpression in this.m_userSortExpressions)
				{
					context.ExprHostBuilder.UserSortExpression(userSortExpression);
				}
				context.ExprHostBuilder.UserSortExpressionsEnd();
			}
			this.m_dataSetCore.ExprHostID = context.ExprHostBuilder.DataSetEnd();
		}

		internal void BindAndValidateDefaultRelationships(ErrorContext errorContext, Report report)
		{
			if (this.m_defaultRelationships != null)
			{
				List<string> list = new List<string>(this.m_defaultRelationships.Count);
				foreach (DefaultRelationship defaultRelationship in this.m_defaultRelationships)
				{
					defaultRelationship.BindAndValidate(this, errorContext, report);
					if (list.Contains(defaultRelationship.RelatedDataSetName))
					{
						errorContext.Register(ProcessingErrorCode.rsInvalidDefaultRelationshipDuplicateRelatedDataset, Severity.Error, AspNetCore.ReportingServices.ReportProcessing.ObjectType.DataSet, this.Name, "DefaultRelationship", "RelatedDataSet", defaultRelationship.RelatedDataSetName.MarkAsPrivate());
					}
					else
					{
						list.Add(defaultRelationship.RelatedDataSetName);
					}
				}
			}
		}

		internal void CheckCircularDefaultRelationshipReference(InitializationContext context)
		{
			HashSet<int> visitedDataSetIds = new HashSet<int>();
			this.CheckCircularDefaultRelationshipReference(context, this, visitedDataSetIds);
		}

		private void CheckCircularDefaultRelationshipReference(InitializationContext context, DataSet dataSet, HashSet<int> visitedDataSetIds)
		{
			visitedDataSetIds.Add(base.ID);
			if (this.m_defaultRelationships != null)
			{
				foreach (DefaultRelationship defaultRelationship in this.m_defaultRelationships)
				{
					if (defaultRelationship.RelatedDataSet != null && !defaultRelationship.IsCrossJoin)
					{
						if (visitedDataSetIds.Contains(defaultRelationship.RelatedDataSet.ID))
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidDefaultRelationshipCircularReference, Severity.Error, dataSet.ObjectType, dataSet.Name, "DefaultRelationship", this.Name.MarkAsPrivate());
						}
						else
						{
							defaultRelationship.RelatedDataSet.CheckCircularDefaultRelationshipReference(context, dataSet, visitedDataSetIds);
						}
					}
				}
			}
			visitedDataSetIds.Remove(base.ID);
		}

		internal bool HasDefaultRelationship(DataSet parentDataSet)
		{
			return this.GetDefaultRelationship(parentDataSet) != null;
		}

		internal DefaultRelationship GetDefaultRelationship(DataSet parentDataSet)
		{
			return JoinInfo.FindActiveRelationship(this.m_defaultRelationships, parentDataSet);
		}

		internal void DetermineDecomposability(InitializationContext context)
		{
			if (!context.EvaluateAtomicityCondition(this.m_dataSetCore.Filters != null, this, AtomicityReason.Filters) && !context.EvaluateAtomicityCondition(this.HasAggregatesForAtomicityCheck(), this, AtomicityReason.Aggregates) && !context.EvaluateAtomicityCondition(this.HasLookups, this, AtomicityReason.Lookups) && !context.EvaluateAtomicityCondition(this.m_dataRegions.Count > 1, this, AtomicityReason.PeerChildScopes))
			{
				return;
			}
			this.m_allowIncrementalProcessing = false;
		}

		public static bool AreEqualById(DataSet dataSet1, DataSet dataSet2)
		{
			if (dataSet1 == null && dataSet2 == null)
			{
				return true;
			}
			if (dataSet1 != null && dataSet2 != null)
			{
				return dataSet1.ID == dataSet2.ID;
			}
			return false;
		}

		private bool HasAggregatesForAtomicityCheck()
		{
			if (!DataScopeInfo.HasNonServerAggregates(this.m_aggregates))
			{
				return DataScopeInfo.HasAggregates(this.m_postSortAggregates);
			}
			return true;
		}

		List<DataAggregateInfo> IAggregateHolder.GetAggregateList()
		{
			return this.m_aggregates;
		}

		List<DataAggregateInfo> IAggregateHolder.GetPostSortAggregateList()
		{
			return this.m_postSortAggregates;
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

		internal void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			this.m_dataSetCore.SetExprHost(reportExprHost, reportObjectModel);
			if (this.m_lookups != null && reportExprHost.LookupExprHostsRemotable != null)
			{
				for (int i = 0; i < this.m_lookups.Count; i++)
				{
					this.m_lookups[i].SetExprHost(reportExprHost, reportObjectModel);
				}
			}
			if (this.m_lookupDestinationInfos != null && reportExprHost.LookupDestExprHostsRemotable != null)
			{
				for (int j = 0; j < this.m_lookupDestinationInfos.Count; j++)
				{
					this.m_lookupDestinationInfos[j].SetExprHost(reportExprHost, reportObjectModel);
				}
			}
			if (this.m_defaultRelationships != null && this.ExprHost != null)
			{
				foreach (DefaultRelationship defaultRelationship in this.m_defaultRelationships)
				{
					defaultRelationship.SetExprHost(this.ExprHost.JoinConditionExprHostsRemotable, reportObjectModel);
				}
			}
		}

		internal void SetFilterExprHost(ObjectModelImpl reportObjectModel)
		{
			this.m_dataSetCore.SetFilterExprHost(reportObjectModel);
		}

		internal void SetupRuntimeEnvironment(OnDemandProcessingContext odpContext)
		{
			odpContext.SetComparisonInformation(this.m_dataSetCore);
		}

		internal bool NeedAutoDetectCollation()
		{
			return this.m_dataSetCore.NeedAutoDetectCollation();
		}

		internal void MergeCollationSettings(ErrorContext errorContext, string dataSourceType, string cultureName, bool caseSensitive, bool accentSensitive, bool kanatypeSensitive, bool widthSensitive)
		{
			this.m_dataSetCore.MergeCollationSettings(errorContext, dataSourceType, cultureName, caseSensitive, accentSensitive, kanatypeSensitive, widthSensitive);
		}

		internal void MarkDataRegionsAsNoRows()
		{
			if (this.m_dataRegions != null)
			{
				foreach (DataRegion dataRegion in this.m_dataRegions)
				{
					dataRegion.NoRows = true;
				}
			}
		}

		internal CompareOptions GetCLRCompareOptions()
		{
			return this.m_dataSetCore.GetCLRCompareOptions();
		}

		internal void ClearDataRegionStreamingScopeInstances()
		{
			if (this.m_dataRegions != null)
			{
				foreach (DataRegion dataRegion in this.m_dataRegions)
				{
					dataRegion.ClearStreamingScopeInstanceBinding();
				}
			}
		}

		internal void RestrictDataSetAggregates(PublishingErrorContext m_errorContext)
		{
			if (!this.m_usedOnlyInParameters && this.m_usedInAggregates && (this.m_dataRegions == null || this.m_dataRegions.Count == 0))
			{
				if (this.m_defaultRelationships != null)
				{
					m_errorContext.Register(ProcessingErrorCode.rsDefaultRelationshipIgnored, Severity.Warning, this.ObjectType, this.Name, "DefaultRelationship");
				}
				if (this.m_aggregates != null)
				{
					foreach (DataAggregateInfo aggregate in this.m_aggregates)
					{
						if (aggregate.AggregateType != 0)
						{
							m_errorContext.Register(ProcessingErrorCode.rsInvalidDataSetScopedAggregate, Severity.Error, this.ObjectType, this.Name, aggregate.AggregateType.ToString());
						}
					}
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new ReadOnlyMemberInfo(MemberName.Name, Token.String));
			list.Add(new ReadOnlyMemberInfo(MemberName.Fields, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Field));
			list.Add(new ReadOnlyMemberInfo(MemberName.Query, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportQuery));
			list.Add(new ReadOnlyMemberInfo(MemberName.CaseSensitivity, Token.Enum));
			list.Add(new ReadOnlyMemberInfo(MemberName.Collation, Token.String));
			list.Add(new ReadOnlyMemberInfo(MemberName.AccentSensitivity, Token.Enum));
			list.Add(new ReadOnlyMemberInfo(MemberName.KanatypeSensitivity, Token.Enum));
			list.Add(new ReadOnlyMemberInfo(MemberName.WidthSensitivity, Token.Enum));
			list.Add(new MemberInfo(MemberName.DataRegions, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegion));
			list.Add(new MemberInfo(MemberName.Aggregates, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateInfo));
			list.Add(new ReadOnlyMemberInfo(MemberName.Filters, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Filter));
			list.Add(new MemberInfo(MemberName.UsedOnlyInParameters, Token.Boolean));
			list.Add(new ReadOnlyMemberInfo(MemberName.NonCalculatedFieldCount, Token.Int32));
			list.Add(new ReadOnlyMemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.PostSortAggregates, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateInfo));
			list.Add(new ReadOnlyMemberInfo(MemberName.LCID, Token.UInt32));
			list.Add(new MemberInfo(MemberName.HasDetailUserSortFilter, Token.Boolean));
			list.Add(new MemberInfo(MemberName.UserSortExpressions, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new ReadOnlyMemberInfo(MemberName.InterpretSubtotalsAsDetails, Token.Enum));
			list.Add(new MemberInfo(MemberName.HasSubReports, Token.Boolean));
			list.Add(new MemberInfo(MemberName.IndexInCollection, Token.Int32));
			list.Add(new MemberInfo(MemberName.DataSource, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSource, Token.Reference));
			list.Add(new MemberInfo(MemberName.Lookups, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupInfo));
			list.Add(new MemberInfo(MemberName.LookupDestinations, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupDestinationInfo));
			list.Add(new MemberInfo(MemberName.DataSetCore, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSetCore, Token.Reference));
			list.Add(new MemberInfo(MemberName.AllowIncrementalProcessing, Token.Boolean));
			list.Add(new MemberInfo(MemberName.DefaultRelationships, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DefaultRelationship));
			list.Add(new MemberInfo(MemberName.HasScopeWithCustomAggregates, Token.Boolean));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSet, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IDOwner, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(DataSet.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.DataSetCore:
					writer.Write(this.m_dataSetCore);
					break;
				case MemberName.DataRegions:
					writer.WriteListOfReferences(this.m_dataRegions);
					break;
				case MemberName.Aggregates:
					writer.Write(this.m_aggregates);
					break;
				case MemberName.UsedOnlyInParameters:
					writer.Write(this.m_usedOnlyInParameters);
					break;
				case MemberName.PostSortAggregates:
					writer.Write(this.m_postSortAggregates);
					break;
				case MemberName.HasDetailUserSortFilter:
					writer.Write(this.m_hasDetailUserSortFilter);
					break;
				case MemberName.UserSortExpressions:
					writer.Write(this.m_userSortExpressions);
					break;
				case MemberName.HasSubReports:
					writer.Write(this.m_hasSubReports);
					break;
				case MemberName.IndexInCollection:
					writer.Write(this.m_indexInCollection);
					break;
				case MemberName.DataSource:
					writer.WriteReference(this.m_dataSource);
					break;
				case MemberName.Lookups:
					writer.Write(this.m_lookups);
					break;
				case MemberName.LookupDestinations:
					writer.Write(this.m_lookupDestinationInfos);
					break;
				case MemberName.AllowIncrementalProcessing:
					writer.Write(this.m_allowIncrementalProcessing);
					break;
				case MemberName.DefaultRelationships:
					writer.Write(this.m_defaultRelationships);
					break;
				case MemberName.HasScopeWithCustomAggregates:
					writer.Write(this.m_hasScopeWithCustomAggregates);
					break;
				default:
					Global.Tracer.Assert(false, string.Empty);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(DataSet.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Name:
					this.m_dataSetCore.Name = reader.ReadString();
					break;
				case MemberName.Fields:
					this.m_dataSetCore.Fields = reader.ReadGenericListOfRIFObjects<Field>();
					break;
				case MemberName.Query:
					this.m_dataSetCore.Query = (ReportQuery)reader.ReadRIFObject();
					break;
				case MemberName.CaseSensitivity:
					this.m_dataSetCore.CaseSensitivity = (TriState)reader.ReadEnum();
					break;
				case MemberName.Collation:
					this.m_dataSetCore.Collation = reader.ReadString();
					break;
				case MemberName.AccentSensitivity:
					this.m_dataSetCore.AccentSensitivity = (TriState)reader.ReadEnum();
					break;
				case MemberName.KanatypeSensitivity:
					this.m_dataSetCore.KanatypeSensitivity = (TriState)reader.ReadEnum();
					break;
				case MemberName.WidthSensitivity:
					this.m_dataSetCore.WidthSensitivity = (TriState)reader.ReadEnum();
					break;
				case MemberName.DataRegions:
					this.m_dataRegions = reader.ReadGenericListOfReferences<DataRegion>(this);
					break;
				case MemberName.Aggregates:
					this.m_aggregates = reader.ReadGenericListOfRIFObjects<DataAggregateInfo>();
					break;
				case MemberName.Filters:
					this.m_dataSetCore.Filters = reader.ReadGenericListOfRIFObjects<Filter>();
					break;
				case MemberName.UsedOnlyInParameters:
					this.m_usedOnlyInParameters = reader.ReadBoolean();
					break;
				case MemberName.NonCalculatedFieldCount:
					this.m_dataSetCore.NonCalculatedFieldCount = reader.ReadInt32();
					break;
				case MemberName.ExprHostID:
					this.m_dataSetCore.ExprHostID = reader.ReadInt32();
					break;
				case MemberName.PostSortAggregates:
					this.m_postSortAggregates = reader.ReadGenericListOfRIFObjects<DataAggregateInfo>();
					break;
				case MemberName.LCID:
					this.m_dataSetCore.LCID = reader.ReadUInt32();
					break;
				case MemberName.HasDetailUserSortFilter:
					this.m_hasDetailUserSortFilter = reader.ReadBoolean();
					break;
				case MemberName.UserSortExpressions:
					this.m_userSortExpressions = reader.ReadGenericListOfRIFObjects<ExpressionInfo>();
					break;
				case MemberName.InterpretSubtotalsAsDetails:
					this.m_dataSetCore.InterpretSubtotalsAsDetails = (TriState)reader.ReadEnum();
					break;
				case MemberName.HasSubReports:
					this.m_hasSubReports = reader.ReadBoolean();
					break;
				case MemberName.IndexInCollection:
					this.m_indexInCollection = reader.ReadInt32();
					break;
				case MemberName.DataSource:
					this.m_dataSource = reader.ReadReference<DataSource>(this);
					break;
				case MemberName.Lookups:
					this.m_lookups = reader.ReadGenericListOfRIFObjects<LookupInfo>();
					break;
				case MemberName.LookupDestinations:
					this.m_lookupDestinationInfos = reader.ReadGenericListOfRIFObjects<LookupDestinationInfo>();
					break;
				case MemberName.DataSetCore:
					this.m_dataSetCore = (DataSetCore)reader.ReadRIFObject();
					break;
				case MemberName.AllowIncrementalProcessing:
					this.m_allowIncrementalProcessing = reader.ReadBoolean();
					break;
				case MemberName.DefaultRelationships:
					this.m_defaultRelationships = reader.ReadGenericListOfRIFObjects<DefaultRelationship>();
					break;
				case MemberName.HasScopeWithCustomAggregates:
					this.m_hasScopeWithCustomAggregates = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(false, string.Empty);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(DataSet.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					switch (item.MemberName)
					{
					case MemberName.DataRegions:
						if (this.m_dataRegions == null)
						{
							this.m_dataRegions = new List<DataRegion>();
						}
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						Global.Tracer.Assert(((ReportItem)referenceableItems[item.RefID]).IsDataRegion);
						Global.Tracer.Assert(!this.m_dataRegions.Contains((DataRegion)referenceableItems[item.RefID]));
						this.m_dataRegions.Add((DataRegion)referenceableItems[item.RefID]);
						break;
					case MemberName.DataSource:
						Global.Tracer.Assert(this.m_dataSource == null);
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						Global.Tracer.Assert(referenceableItems[item.RefID] is DataSource);
						this.m_dataSource = (DataSource)referenceableItems[item.RefID];
						break;
					default:
						Global.Tracer.Assert(false, string.Empty);
						break;
					}
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSet;
		}
	}
}
