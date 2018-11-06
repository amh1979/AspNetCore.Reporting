using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class DataSet : IDOwner, IAggregateHolder, ISortFilterScope
	{
		internal enum Sensitivity
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

		private string m_name;

		private DataFieldList m_fields;

		private ReportQuery m_query;

		private Sensitivity m_caseSensitivity;

		private string m_collation;

		private Sensitivity m_accentSensitivity;

		private Sensitivity m_kanatypeSensitivity;

		private Sensitivity m_widthSensitivity;

		private DataRegionList m_dataRegions;

		private DataAggregateInfoList m_aggregates;

		private FilterList m_filters;

		private bool m_usedOnlyInParameters;

		private int m_nonCalculatedFieldCount = -1;

		private int m_exprHostID = -1;

		private DataAggregateInfoList m_postSortAggregates;

		private bool m_hasDetailUserSortFilter;

		private ExpressionInfoList m_userSortExpressions;

		private bool m_dynamicFieldReferences;

		private bool? m_interpretSubtotalsAsDetails;

		private int m_recordSetSize = -1;

		private uint m_lcid = DataSetValidator.LOCALE_SYSTEM_DEFAULT;

		[NonSerialized]
		private DataSetExprHost m_exprHost;

		[NonSerialized]
		private string m_autoDetectedCollation;

		[NonSerialized]
		private bool[] m_isSortFilterTarget;

		[NonSerialized]
		private Hashtable m_referencedFieldProperties;

		[NonSerialized]
		private bool m_usedInAggregates;

		internal ObjectType ObjectType
		{
			get
			{
				return ObjectType.DataSet;
			}
		}

		internal string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				this.m_name = value;
			}
		}

		internal DataFieldList Fields
		{
			get
			{
				return this.m_fields;
			}
			set
			{
				this.m_fields = value;
			}
		}

		internal ReportQuery Query
		{
			get
			{
				return this.m_query;
			}
			set
			{
				this.m_query = value;
			}
		}

		internal Sensitivity CaseSensitivity
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

		internal string Collation
		{
			get
			{
				return this.m_collation;
			}
			set
			{
				this.m_collation = value;
			}
		}

		internal Sensitivity AccentSensitivity
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

		internal Sensitivity KanatypeSensitivity
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

		internal Sensitivity WidthSensitivity
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

		internal DataRegionList DataRegions
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

		internal DataAggregateInfoList Aggregates
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

		internal FilterList Filters
		{
			get
			{
				return this.m_filters;
			}
			set
			{
				this.m_filters = value;
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
				this.m_usedOnlyInParameters = value;
			}
		}

		internal int NonCalculatedFieldCount
		{
			get
			{
				return this.m_nonCalculatedFieldCount;
			}
			set
			{
				this.m_nonCalculatedFieldCount = value;
			}
		}

		internal int ExprHostID
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

		internal DataAggregateInfoList PostSortAggregates
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

		internal ExpressionInfoList UserSortExpressions
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
				return this.m_exprHost;
			}
		}

		internal string AutoDetectedCollation
		{
			get
			{
				return this.m_autoDetectedCollation;
			}
			set
			{
				this.m_autoDetectedCollation = value;
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
				return this.m_name;
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

		ExpressionInfoList ISortFilterScope.UserSortExpressions
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
				if (this.m_exprHost == null)
				{
					return null;
				}
				return this.m_exprHost.UserSortExpressionsHost;
			}
		}

		internal bool DynamicFieldReferences
		{
			get
			{
				return this.m_dynamicFieldReferences;
			}
			set
			{
				this.m_dynamicFieldReferences = value;
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

		internal bool InterpretSubtotalsAsDetailsIsAuto
		{
			get
			{
				return !this.m_interpretSubtotalsAsDetails.HasValue;
			}
		}

		internal bool InterpretSubtotalsAsDetails
		{
			get
			{
				if (this.m_interpretSubtotalsAsDetails.HasValue)
				{
					return this.m_interpretSubtotalsAsDetails.Value;
				}
				return false;
			}
			set
			{
				this.m_interpretSubtotalsAsDetails = value;
			}
		}

		internal DataSet(int id)
			: base(id)
		{
			this.m_fields = new DataFieldList();
			this.m_dataRegions = new DataRegionList();
			this.m_aggregates = new DataAggregateInfoList();
			this.m_postSortAggregates = new DataAggregateInfoList();
		}

		internal DataSet()
		{
		}

		internal void Initialize(InitializationContext context)
		{
			context.ObjectType = this.ObjectType;
			context.ObjectName = this.m_name;
			context.RegisterDataSet(this);
			this.InternalInitialize(context);
			context.UnRegisterDataSet(this);
		}

		private void InternalInitialize(InitializationContext context)
		{
			context.ExprHostBuilder.DataSetStart(this.m_name);
			context.Location |= LocationFlags.InDataSet;
			if (this.m_query != null)
			{
				this.m_query.Initialize(context);
			}
			if (this.m_fields != null)
			{
				int count = this.m_fields.Count;
				for (int i = 0; i < count; i++)
				{
					this.m_fields[i].Initialize(context);
				}
			}
			if (this.m_filters != null)
			{
				for (int j = 0; j < this.m_filters.Count; j++)
				{
					this.m_filters[j].Initialize(context);
				}
			}
			if (this.m_userSortExpressions != null)
			{
				context.ExprHostBuilder.UserSortExpressionsStart();
				for (int k = 0; k < this.m_userSortExpressions.Count; k++)
				{
					ExpressionInfo expression = this.m_userSortExpressions[k];
					context.ExprHostBuilder.UserSortExpression(expression);
				}
				context.ExprHostBuilder.UserSortExpressionsEnd();
			}
			this.m_exprHostID = context.ExprHostBuilder.DataSetEnd();
		}

		DataAggregateInfoList[] IAggregateHolder.GetAggregateLists()
		{
			return new DataAggregateInfoList[1]
			{
				this.m_aggregates
			};
		}

		DataAggregateInfoList[] IAggregateHolder.GetPostSortAggregateLists()
		{
			return new DataAggregateInfoList[1]
			{
				this.m_postSortAggregates
			};
		}

		void IAggregateHolder.ClearIfEmpty()
		{
			Global.Tracer.Assert(null != this.m_aggregates);
			if (this.m_aggregates.Count == 0)
			{
				this.m_aggregates = null;
			}
			Global.Tracer.Assert(null != this.m_postSortAggregates);
			if (this.m_postSortAggregates.Count == 0)
			{
				this.m_postSortAggregates = null;
			}
		}

		internal void CheckNonCalculatedFieldCount()
		{
			if (this.m_nonCalculatedFieldCount < 0 && this.m_fields != null)
			{
				int i;
				for (i = 0; i < this.m_fields.Count && !this.m_fields[i].IsCalculatedField; i++)
				{
				}
				this.m_nonCalculatedFieldCount = i;
			}
		}

		internal void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (this.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
				this.m_exprHost = reportExprHost.DataSetHostsRemotable[this.ExprHostID];
				this.m_exprHost.SetReportObjectModel(reportObjectModel);
				if (this.m_exprHost.QueryParametersHost != null)
				{
					this.m_query.SetExprHost(this.m_exprHost.QueryParametersHost, reportObjectModel);
				}
				if (this.m_exprHost.UserSortExpressionsHost != null)
				{
					this.m_exprHost.UserSortExpressionsHost.SetReportObjectModel(reportObjectModel);
				}
			}
		}

		internal bool NeedAutoDetectCollation()
		{
			if (DataSetValidator.LOCALE_SYSTEM_DEFAULT != this.m_lcid && this.m_accentSensitivity != 0 && this.m_caseSensitivity != 0 && this.m_kanatypeSensitivity != 0)
			{
				return Sensitivity.Auto == this.m_widthSensitivity;
			}
			return true;
		}

		internal void MergeCollationSettings(ErrorContext errorContext, string dataSourceType, string cultureName, bool caseSensitive, bool accentSensitive, bool kanatypeSensitive, bool widthSensitive)
		{
			if (this.NeedAutoDetectCollation())
			{
				uint lcid = DataSetValidator.LOCALE_SYSTEM_DEFAULT;
				if (cultureName != null)
				{
					try
					{
						CultureInfo cultureInfo = CultureInfo.GetCultureInfo(cultureName);
						lcid = (uint)cultureInfo.LCID;
					}
					catch (Exception)
					{
						errorContext.Register(ProcessingErrorCode.rsInvalidCollationCultureName, Severity.Warning, ObjectType.DataSet, this.m_name, dataSourceType, cultureName);
					}
				}
				if (DataSetValidator.LOCALE_SYSTEM_DEFAULT == this.m_lcid)
				{
					this.m_lcid = lcid;
				}
				this.MergeSensitivity(ref this.m_accentSensitivity, accentSensitive);
				this.MergeSensitivity(ref this.m_caseSensitivity, caseSensitive);
				this.MergeSensitivity(ref this.m_kanatypeSensitivity, kanatypeSensitive);
				this.MergeSensitivity(ref this.m_widthSensitivity, widthSensitive);
			}
		}

		private void MergeSensitivity(ref Sensitivity current, bool detectedValue)
		{
			if (current == Sensitivity.Auto)
			{
				if (detectedValue)
				{
					current = Sensitivity.True;
				}
				else
				{
					current = Sensitivity.False;
				}
			}
		}

		internal uint GetSQLSortCompareFlags()
		{
			return DataSetValidator.GetSQLSortCompareMask(Sensitivity.True == this.m_caseSensitivity, Sensitivity.True == this.m_accentSensitivity, Sensitivity.True == this.m_kanatypeSensitivity, Sensitivity.True == this.m_widthSensitivity);
		}

		internal CompareOptions GetCLRCompareOptions()
		{
			CompareOptions compareOptions = CompareOptions.None;
			if (Sensitivity.True != this.m_caseSensitivity)
			{
				compareOptions |= CompareOptions.IgnoreCase;
			}
			if (Sensitivity.True != this.m_accentSensitivity)
			{
				compareOptions |= CompareOptions.IgnoreNonSpace;
			}
			if (Sensitivity.True != this.m_kanatypeSensitivity)
			{
				compareOptions |= CompareOptions.IgnoreKanaType;
			}
			if (Sensitivity.True != this.m_widthSensitivity)
			{
				compareOptions |= CompareOptions.IgnoreWidth;
			}
			return compareOptions;
		}

		internal void MergeFieldProperties(ExpressionInfo expressionInfo)
		{
			if (!this.m_dynamicFieldReferences)
			{
				Global.Tracer.Assert(null != expressionInfo);
				if (expressionInfo.DynamicFieldReferences)
				{
					this.m_dynamicFieldReferences = true;
					this.m_referencedFieldProperties = null;
				}
				else if (expressionInfo.ReferencedFieldProperties != null && expressionInfo.ReferencedFieldProperties.Count != 0)
				{
					if (this.m_referencedFieldProperties == null)
					{
						this.m_referencedFieldProperties = new Hashtable();
					}
					IDictionaryEnumerator enumerator = expressionInfo.ReferencedFieldProperties.GetEnumerator();
					while (enumerator.MoveNext())
					{
						string key = enumerator.Entry.Key as string;
						bool flag = this.m_referencedFieldProperties.ContainsKey(key);
						Hashtable hashtable = this.m_referencedFieldProperties[key] as Hashtable;
						if (!flag || hashtable != null)
						{
							Hashtable hashtable2 = enumerator.Entry.Value as Hashtable;
							if (!flag)
							{
								this.m_referencedFieldProperties.Add(key, hashtable2);
							}
							else if (hashtable2 == null)
							{
								this.m_referencedFieldProperties[key] = null;
							}
							else
							{
								Global.Tracer.Assert(hashtable != null && null != hashtable2);
								IEnumerator enumerator2 = hashtable2.Keys.GetEnumerator();
								while (enumerator2.MoveNext())
								{
									string key2 = enumerator2.Current as string;
									if (!hashtable.ContainsKey(key2))
									{
										hashtable.Add(key2, null);
									}
								}
							}
						}
					}
				}
			}
		}

		internal void PopulateReferencedFieldProperties()
		{
			if (!this.m_dynamicFieldReferences && this.m_fields != null && this.m_referencedFieldProperties != null)
			{
				for (int i = 0; i < this.m_fields.Count; i++)
				{
					Field field = this.m_fields[i];
					if (this.m_referencedFieldProperties.ContainsKey(field.Name))
					{
						Hashtable hashtable = this.m_referencedFieldProperties[field.Name] as Hashtable;
						if (hashtable == null)
						{
							field.DynamicPropertyReferences = true;
						}
						else
						{
							IEnumerator enumerator = hashtable.Keys.GetEnumerator();
							FieldPropertyHashtable fieldPropertyHashtable = new FieldPropertyHashtable(hashtable.Count);
							while (enumerator.MoveNext())
							{
								fieldPropertyHashtable.Add(enumerator.Current as string);
							}
							field.ReferencedProperties = fieldPropertyHashtable;
						}
					}
				}
			}
		}

		internal bool IsShareable()
		{
			if (this.m_query != null && this.m_query.CommandText != null)
			{
				if (ExpressionInfo.Types.Constant == this.m_query.CommandText.Type)
				{
					if (this.m_query.Parameters == null)
					{
						return true;
					}
					int count = this.m_query.Parameters.Count;
					for (int i = 0; i < count; i++)
					{
						ExpressionInfo value = this.m_query.Parameters[i].Value;
						if (value != null && ExpressionInfo.Types.Constant != value.Type)
						{
							return false;
						}
					}
					return true;
				}
				return false;
			}
			return true;
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Name, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.Fields, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.DataFieldList));
			memberInfoList.Add(new MemberInfo(MemberName.Query, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.CaseSensitivity, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.Collation, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.AccentSensitivity, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.KanatypeSensitivity, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.WidthSensitivity, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.DataRegions, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.DataRegionList));
			memberInfoList.Add(new MemberInfo(MemberName.Aggregates, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.DataAggregateInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.Filters, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.FilterList));
			memberInfoList.Add(new MemberInfo(MemberName.RecordSetSize, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.UsedOnlyInParameters, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.NonCalculatedFieldCount, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.PostSortAggregates, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.DataAggregateInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.LCID, Token.UInt32));
			memberInfoList.Add(new MemberInfo(MemberName.HasDetailUserSortFilter, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.UserSortExpressions, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.DynamicFieldReferences, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.InterpretSubtotalsAsDetails, Token.Boolean));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.IDOwner, memberInfoList);
		}
	}
}
