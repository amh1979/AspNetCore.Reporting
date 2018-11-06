using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class DataSetCore : IPersistable, IExpressionHostAssemblyHolder
	{
		private string m_name;

		private List<Field> m_fields;

		private ReportQuery m_query;

		private SharedDataSetQuery m_sharedDataSetQuery;

		private string m_collation;

		private string m_collationCulture;

		private uint m_lcid = DataSetValidator.LOCALE_SYSTEM_DEFAULT;

		private DataSet.TriState m_caseSensitivity;

		private DataSet.TriState m_accentSensitivity;

		private DataSet.TriState m_kanatypeSensitivity;

		private DataSet.TriState m_widthSensitivity;

		private bool m_nullsAsBlanks;

		[NonSerialized]
		private bool m_useOrdinalStringKeyGeneration;

		private List<Filter> m_filters;

		private DataSet.TriState m_interpretSubtotalsAsDetails;

		private Guid m_catalogID = Guid.Empty;

		private int m_nonCalculatedFieldCount = -1;

		private byte[] m_compiledCode;

		private bool m_compiledCodeGeneratedWithRefusedPermissions;

		private int m_exprHostID = -1;

		private Guid m_exprHostAssemblyId = Guid.Empty;

		[NonSerialized]
		private bool? m_cachedUsesAggregateIndicatorFields = null;

		[NonSerialized]
		private DataSetExprHost m_exprHost;

		[NonSerialized]
		private FieldsContext m_fieldsContext;

		[NonSerialized]
		private static readonly Declaration m_Declaration = DataSetCore.GetDeclaration();

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

		internal List<Field> Fields
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

		internal bool HasAggregateIndicatorFields
		{
			get
			{
				if (!this.m_cachedUsesAggregateIndicatorFields.HasValue)
				{
					this.m_cachedUsesAggregateIndicatorFields = false;
					if (this.m_fields != null)
					{
						this.m_cachedUsesAggregateIndicatorFields = this.m_fields.Any((Field field) => field.HasAggregateIndicatorField);
					}
				}
				return this.m_cachedUsesAggregateIndicatorFields.Value;
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

		internal SharedDataSetQuery SharedDataSetQuery
		{
			get
			{
				return this.m_sharedDataSetQuery;
			}
			set
			{
				this.m_sharedDataSetQuery = value;
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

		internal string CollationCulture
		{
			get
			{
				return this.m_collationCulture;
			}
			set
			{
				this.m_collationCulture = value;
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

		internal bool NullsAsBlanks
		{
			get
			{
				return this.m_nullsAsBlanks;
			}
			set
			{
				this.m_nullsAsBlanks = value;
			}
		}

		internal bool UseOrdinalStringKeyGeneration
		{
			get
			{
				return this.m_useOrdinalStringKeyGeneration;
			}
			set
			{
				this.m_useOrdinalStringKeyGeneration = value;
			}
		}

		internal List<Filter> Filters
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

		internal DataSet.TriState InterpretSubtotalsAsDetails
		{
			get
			{
				return this.m_interpretSubtotalsAsDetails;
			}
			set
			{
				this.m_interpretSubtotalsAsDetails = value;
			}
		}

		internal int NonCalculatedFieldCount
		{
			get
			{
				if (this.m_nonCalculatedFieldCount < 0 && this.m_fields != null)
				{
					int i;
					for (i = 0; i < this.m_fields.Count && !this.m_fields[i].IsCalculatedField; i++)
					{
					}
					this.m_nonCalculatedFieldCount = i;
				}
				return this.m_nonCalculatedFieldCount;
			}
			set
			{
				this.m_nonCalculatedFieldCount = value;
			}
		}

		internal Guid CatalogID
		{
			get
			{
				return this.m_catalogID;
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

		internal DataSetExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
			set
			{
				this.m_exprHost = value;
			}
		}

		internal FieldsContext FieldsContext
		{
			get
			{
				return this.m_fieldsContext;
			}
			set
			{
				this.m_fieldsContext = value;
			}
		}

		AspNetCore.ReportingServices.ReportProcessing.ObjectType IExpressionHostAssemblyHolder.ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.SharedDataSet;
			}
		}

		string IExpressionHostAssemblyHolder.ExprHostAssemblyName
		{
			get
			{
				if (this.m_exprHostAssemblyId == Guid.Empty)
				{
					this.m_exprHostAssemblyId = Guid.NewGuid();
					this.m_exprHostID = 0;
				}
				return "expression_host_RSD_" + this.m_exprHostAssemblyId.ToString().Replace("-", "");
			}
		}

		byte[] IExpressionHostAssemblyHolder.CompiledCode
		{
			get
			{
				return this.m_compiledCode;
			}
			set
			{
				this.m_compiledCode = value;
			}
		}

		bool IExpressionHostAssemblyHolder.CompiledCodeGeneratedWithRefusedPermissions
		{
			get
			{
				return this.m_compiledCodeGeneratedWithRefusedPermissions;
			}
			set
			{
				this.m_compiledCodeGeneratedWithRefusedPermissions = value;
			}
		}

		List<string> IExpressionHostAssemblyHolder.CodeModules
		{
			get
			{
				return null;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		List<CodeClass> IExpressionHostAssemblyHolder.CodeClasses
		{
			get
			{
				return null;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		internal DataSetCore()
		{
		}

		internal CultureInfo CreateCultureInfoFromLcid()
		{
			return new CultureInfo((int)this.LCID, false);
		}

		internal void SetCatalogID(Guid id)
		{
			if (this.m_catalogID == Guid.Empty)
			{
				this.m_catalogID = id;
			}
		}

		internal void Initialize(InitializationContext context)
		{
			if (this.m_query != null)
			{
				this.m_query.Initialize(context);
			}
			else if (this.m_sharedDataSetQuery != null)
			{
				this.m_sharedDataSetQuery.Initialize(context);
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
		}

		internal CompareOptions GetCLRCompareOptions()
		{
			CompareOptions compareOptions = CompareOptions.None;
			if (DataSet.TriState.True != this.m_caseSensitivity)
			{
				compareOptions |= CompareOptions.IgnoreCase;
			}
			if (DataSet.TriState.True != this.m_accentSensitivity)
			{
				compareOptions |= CompareOptions.IgnoreNonSpace;
			}
			if (DataSet.TriState.True != this.m_kanatypeSensitivity)
			{
				compareOptions |= CompareOptions.IgnoreKanaType;
			}
			if (DataSet.TriState.True != this.m_widthSensitivity)
			{
				compareOptions |= CompareOptions.IgnoreWidth;
			}
			return compareOptions;
		}

		internal bool NeedAutoDetectCollation()
		{
			if (DataSetValidator.LOCALE_SYSTEM_DEFAULT != this.m_lcid && this.m_accentSensitivity != 0 && this.m_caseSensitivity != 0 && this.m_kanatypeSensitivity != 0)
			{
				return DataSet.TriState.Auto == this.m_widthSensitivity;
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
						if (errorContext != null)
						{
							errorContext.Register(ProcessingErrorCode.rsInvalidCollationCultureName, Severity.Warning, AspNetCore.ReportingServices.ReportProcessing.ObjectType.DataSet, this.m_name, dataSourceType, cultureName);
						}
					}
				}
				if (DataSetValidator.LOCALE_SYSTEM_DEFAULT == this.m_lcid)
				{
					this.m_lcid = lcid;
				}
				this.m_accentSensitivity = this.MergeSensitivity(this.m_accentSensitivity, accentSensitive);
				this.m_caseSensitivity = this.MergeSensitivity(this.m_caseSensitivity, caseSensitive);
				this.m_kanatypeSensitivity = this.MergeSensitivity(this.m_kanatypeSensitivity, kanatypeSensitive);
				this.m_widthSensitivity = this.MergeSensitivity(this.m_widthSensitivity, widthSensitive);
			}
		}

		private DataSet.TriState MergeSensitivity(DataSet.TriState current, bool detectedValue)
		{
			if (current != 0)
			{
				return current;
			}
			if (detectedValue)
			{
				return DataSet.TriState.True;
			}
			return DataSet.TriState.False;
		}

		internal bool HasCalculatedFields()
		{
			if (this.m_fields != null)
			{
				return this.NonCalculatedFieldCount != this.m_fields.Count;
			}
			return false;
		}

		internal void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (this.m_exprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null, "(reportExprHost != null && reportObjectModel != null)");
				this.m_exprHost = reportExprHost.DataSetHostsRemotable[this.m_exprHostID];
				this.m_exprHost.SetReportObjectModel(reportObjectModel);
				if (this.m_exprHost.QueryParametersHost != null)
				{
					if (this.m_query != null)
					{
						this.m_query.SetExprHost(this.m_exprHost.QueryParametersHost, reportObjectModel);
					}
					else
					{
						this.m_sharedDataSetQuery.SetExprHost(this.m_exprHost.QueryParametersHost, reportObjectModel);
					}
				}
				if (this.m_exprHost.UserSortExpressionsHost != null)
				{
					this.m_exprHost.UserSortExpressionsHost.SetReportObjectModel(reportObjectModel);
				}
			}
		}

		internal void SetFilterExprHost(ObjectModelImpl reportObjectModel)
		{
			if (this.m_filters != null && this.m_exprHost != null)
			{
				for (int i = 0; i < this.m_filters.Count; i++)
				{
					this.m_filters[i].SetExprHost(this.m_exprHost.FilterHostsRemotable, reportObjectModel);
				}
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Fields, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Field));
			list.Add(new MemberInfo(MemberName.Query, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportQuery));
			list.Add(new MemberInfo(MemberName.SharedDataSetQuery, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SharedDataSetQuery));
			list.Add(new MemberInfo(MemberName.Collation, Token.String));
			list.Add(new MemberInfo(MemberName.LCID, Token.UInt32));
			list.Add(new MemberInfo(MemberName.CaseSensitivity, Token.Enum));
			list.Add(new MemberInfo(MemberName.AccentSensitivity, Token.Enum));
			list.Add(new MemberInfo(MemberName.KanatypeSensitivity, Token.Enum));
			list.Add(new MemberInfo(MemberName.WidthSensitivity, Token.Enum));
			list.Add(new MemberInfo(MemberName.Filters, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Filter));
			list.Add(new MemberInfo(MemberName.InterpretSubtotalsAsDetails, Token.Enum));
			list.Add(new MemberInfo(MemberName.CatalogID, Token.Guid));
			list.Add(new MemberInfo(MemberName.NonCalculatedFieldCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.CompiledCode, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Byte));
			list.Add(new MemberInfo(MemberName.CompiledCodeGeneratedWithRefusedPermissions, Token.Boolean));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.ExprHostAssemblyID, Token.Guid));
			list.Add(new MemberInfo(MemberName.NullsAsBlanks, Token.Boolean));
			list.Add(new MemberInfo(MemberName.CollationCulture, Token.String));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSetCore, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(DataSetCore.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Name:
					writer.Write(this.m_name);
					break;
				case MemberName.Fields:
					writer.Write(this.m_fields);
					break;
				case MemberName.Query:
					writer.Write(this.m_query);
					break;
				case MemberName.SharedDataSetQuery:
					writer.Write(this.m_sharedDataSetQuery);
					break;
				case MemberName.Collation:
					writer.Write(this.m_collation);
					break;
				case MemberName.CollationCulture:
					writer.Write(this.m_collationCulture);
					break;
				case MemberName.LCID:
					writer.Write(this.m_lcid);
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
				case MemberName.Filters:
					writer.Write(this.m_filters);
					break;
				case MemberName.InterpretSubtotalsAsDetails:
					writer.WriteEnum((int)this.m_interpretSubtotalsAsDetails);
					break;
				case MemberName.CatalogID:
					writer.Write(this.m_catalogID);
					break;
				case MemberName.NonCalculatedFieldCount:
					writer.Write(this.m_nonCalculatedFieldCount);
					break;
				case MemberName.CompiledCode:
					writer.Write(this.m_compiledCode);
					break;
				case MemberName.CompiledCodeGeneratedWithRefusedPermissions:
					writer.Write(this.m_compiledCodeGeneratedWithRefusedPermissions);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				case MemberName.ExprHostAssemblyID:
					writer.Write(this.m_exprHostAssemblyId);
					break;
				case MemberName.NullsAsBlanks:
					writer.Write(this.m_nullsAsBlanks);
					break;
				default:
					Global.Tracer.Assert(false, string.Empty);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(DataSetCore.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Name:
					this.m_name = reader.ReadString();
					break;
				case MemberName.Fields:
					this.m_fields = reader.ReadGenericListOfRIFObjects<Field>();
					break;
				case MemberName.Query:
					this.m_query = (ReportQuery)reader.ReadRIFObject();
					break;
				case MemberName.SharedDataSetQuery:
					this.m_sharedDataSetQuery = (SharedDataSetQuery)reader.ReadRIFObject();
					break;
				case MemberName.Collation:
					this.m_collation = reader.ReadString();
					break;
				case MemberName.CollationCulture:
					this.m_collationCulture = reader.ReadString();
					break;
				case MemberName.LCID:
					this.m_lcid = reader.ReadUInt32();
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
				case MemberName.Filters:
					this.m_filters = reader.ReadGenericListOfRIFObjects<Filter>();
					break;
				case MemberName.InterpretSubtotalsAsDetails:
					this.m_interpretSubtotalsAsDetails = (DataSet.TriState)reader.ReadEnum();
					break;
				case MemberName.CatalogID:
					this.m_catalogID = reader.ReadGuid();
					break;
				case MemberName.NonCalculatedFieldCount:
					this.m_nonCalculatedFieldCount = reader.ReadInt32();
					break;
				case MemberName.CompiledCode:
					this.m_compiledCode = reader.ReadByteArray();
					break;
				case MemberName.CompiledCodeGeneratedWithRefusedPermissions:
					this.m_compiledCodeGeneratedWithRefusedPermissions = reader.ReadBoolean();
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.ExprHostAssemblyID:
					this.m_exprHostAssemblyId = reader.ReadGuid();
					break;
				case MemberName.NullsAsBlanks:
					this.m_nullsAsBlanks = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(false, string.Empty);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false, string.Empty);
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSetCore;
		}
	}
}
