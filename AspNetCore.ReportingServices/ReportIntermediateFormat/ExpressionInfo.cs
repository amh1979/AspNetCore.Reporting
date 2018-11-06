using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ExpressionInfo : IPersistable, IStaticReferenceable
	{
		internal enum Types
		{
			Expression,
			Field,
			Aggregate,
			Constant,
			Token,
			Lookup_OneValue,
			Lookup_MultiValue,
			RdlFunction,
			ScopedFieldReference,
			Literal
		}

		private class TransformedExprSpecialFunctionInfo
		{
			internal enum SpecialFunctionType
			{
				Aggregate,
				RunningValue,
				Lookup
			}

			internal SpecialFunctionType FunctionType;

			internal int Position;

			internal string FunctionID;

			internal int IndexIntoCollection;

			internal TransformedExprSpecialFunctionInfo(int position, string functionID, SpecialFunctionType functionType, int indexIntoCollection)
			{
				this.FunctionType = functionType;
				this.Position = position;
				this.FunctionID = functionID;
				this.IndexIntoCollection = indexIntoCollection;
			}

			internal object PublishClone(AutomaticSubtotalContext context)
			{
				TransformedExprSpecialFunctionInfo transformedExprSpecialFunctionInfo = (TransformedExprSpecialFunctionInfo)base.MemberwiseClone();
				transformedExprSpecialFunctionInfo.FunctionID = (string)this.FunctionID.Clone();
				return transformedExprSpecialFunctionInfo;
			}
		}

		private Types m_type;

		private DataType m_constantType = DataType.String;

		private string m_stringValue;

		private bool m_boolValue;

		private int m_intValue;

		private DateTime m_dateTimeValue = default(DateTime);

		private DateTimeOffset? m_dateTimeOffsetValue = null;

		private double m_floatValue;

		private int m_exprHostID = -1;

		private string m_originalText;

		private bool m_inPrevious;

		private RdlFunctionInfo m_rdlFunctionInfo;

		private ScopedFieldInfo m_scopedFieldInfo;

		[NonSerialized]
		private LiteralInfo m_literalInfo;

		[NonSerialized]
		private string m_transformedExpression;

		[NonSerialized]
		private List<TransformedExprSpecialFunctionInfo> m_transformedExprAggregateInfos;

		[NonSerialized]
		private List<string> m_referencedFields;

		[NonSerialized]
		private List<string> m_referencedReportItems;

		[NonSerialized]
		private List<int> m_referencedReportItemPositionsInTransformedExpression;

		[NonSerialized]
		private List<int> m_referencedReportItemPositionsInOriginalText;

		[NonSerialized]
		private List<string> m_referencedVariables;

		[NonSerialized]
		private List<int> m_referencedVariablePositions;

		[NonSerialized]
		private List<string> m_referencedParameters;

		[NonSerialized]
		private string m_simpleParameterName;

		[NonSerialized]
		private bool m_hasDynamicParameterReference;

		[NonSerialized]
		private List<string> m_referencedDataSets;

		[NonSerialized]
		private List<string> m_referencedDataSources;

		[NonSerialized]
		private List<ScopeReference> m_referencedScopes;

		[NonSerialized]
		private List<DataAggregateInfo> m_aggregates;

		[NonSerialized]
		private List<RunningValueInfo> m_runningValues;

		[NonSerialized]
		private List<LookupInfo> m_lookups;

		[NonSerialized]
		private int m_compileTimeID = -1;

		[NonSerialized]
		private Hashtable m_referencedFieldProperties;

		[NonSerialized]
		private bool m_dynamicFieldReferences;

		[NonSerialized]
		private bool m_referencedOverallPageGlobals;

		[NonSerialized]
		private bool m_hasAnyFieldReferences;

		[NonSerialized]
		private bool m_referencedPageGlobals;

		[NonSerialized]
		private List<int> m_meDotValuePositionsInOriginalText;

		[NonSerialized]
		private List<int> m_meDotValuePositionsInTranformedExpr;

		[NonSerialized]
		private bool m_nullLevelInExpr;

		[NonSerialized]
		private int m_id = -2147483648;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ExpressionInfo.GetDeclaration();

		internal bool IsExpression
		{
			get
			{
				return this.m_type != Types.Constant;
			}
		}

		internal DataType ConstantType
		{
			get
			{
				return this.m_constantType;
			}
			set
			{
				this.m_constantType = value;
			}
		}

		internal TypeCode ConstantTypeCode
		{
			get
			{
				if (!this.IsExpression)
				{
					switch (this.m_constantType)
					{
					case DataType.Boolean:
						return TypeCode.Boolean;
					case DataType.DateTime:
						return TypeCode.DateTime;
					case DataType.Float:
						return TypeCode.Double;
					case DataType.Integer:
						return TypeCode.Int32;
					default:
						return TypeCode.String;
					}
				}
				return TypeCode.Object;
			}
		}

		internal Types Type
		{
			get
			{
				return this.m_type;
			}
			set
			{
				this.m_type = value;
			}
		}

		internal string StringValue
		{
			get
			{
				return this.m_stringValue;
			}
			set
			{
				this.m_stringValue = value;
			}
		}

		internal object Value
		{
			get
			{
				if (!this.IsExpression)
				{
					switch (this.m_constantType)
					{
					case DataType.Boolean:
						return this.m_boolValue;
					case DataType.DateTime:
						return this.GetDateTimeValue();
					case DataType.Float:
						return this.m_floatValue;
					case DataType.Integer:
						return this.m_intValue;
					case DataType.String:
						return this.m_stringValue;
					}
				}
				return null;
			}
		}

		internal bool BoolValue
		{
			get
			{
				return this.m_boolValue;
			}
			set
			{
				this.m_boolValue = value;
			}
		}

		internal int IntValue
		{
			get
			{
				return this.m_intValue;
			}
			set
			{
				this.m_intValue = value;
			}
		}

		internal double FloatValue
		{
			get
			{
				return this.m_floatValue;
			}
			set
			{
				this.m_floatValue = value;
			}
		}

		internal string OriginalText
		{
			get
			{
				return this.m_originalText;
			}
			set
			{
				this.m_originalText = value;
			}
		}

		internal string TransformedExpression
		{
			get
			{
				return this.m_transformedExpression;
			}
			set
			{
				this.m_transformedExpression = value;
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

		internal int CompileTimeID
		{
			get
			{
				return this.m_compileTimeID;
			}
			set
			{
				this.m_compileTimeID = value;
			}
		}

		internal List<DataAggregateInfo> Aggregates
		{
			get
			{
				return this.m_aggregates;
			}
		}

		internal List<RunningValueInfo> RunningValues
		{
			get
			{
				return this.m_runningValues;
			}
		}

		internal List<LookupInfo> Lookups
		{
			get
			{
				return this.m_lookups;
			}
		}

		internal bool InPrevious
		{
			get
			{
				return this.m_inPrevious;
			}
			set
			{
				this.m_inPrevious = value;
			}
		}

		internal Hashtable ReferencedFieldProperties
		{
			get
			{
				return this.m_referencedFieldProperties;
			}
		}

		internal List<string> ReferencedReportItems
		{
			get
			{
				return this.m_referencedReportItems;
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

		internal bool ReferencedOverallPageGlobals
		{
			get
			{
				return this.m_referencedOverallPageGlobals;
			}
			set
			{
				this.m_referencedOverallPageGlobals = value;
			}
		}

		internal bool ReferencedPageGlobals
		{
			get
			{
				return this.m_referencedPageGlobals;
			}
			set
			{
				this.m_referencedPageGlobals = value;
			}
		}

		internal bool MeDotValueDetected
		{
			get
			{
				if (this.m_meDotValuePositionsInOriginalText != null && this.m_meDotValuePositionsInOriginalText.Count > 0)
				{
					return true;
				}
				if (this.m_meDotValuePositionsInTranformedExpr != null)
				{
					return this.m_meDotValuePositionsInTranformedExpr.Count > 0;
				}
				return false;
			}
		}

		internal bool NullLevelDetected
		{
			get
			{
				return this.m_nullLevelInExpr;
			}
			set
			{
				this.m_nullLevelInExpr = value;
			}
		}

		internal bool HasDirectFieldReferences
		{
			get
			{
				if (this.m_referencedFields != null)
				{
					return this.m_referencedFields.Count > 0;
				}
				return false;
			}
		}

		internal bool HasAnyFieldReferences
		{
			get
			{
				if (this.m_hasAnyFieldReferences)
				{
					return true;
				}
				if (this.m_rdlFunctionInfo != null)
				{
					foreach (ExpressionInfo expression in this.m_rdlFunctionInfo.Expressions)
					{
						if (expression.HasAnyFieldReferences)
						{
							return true;
						}
					}
				}
				return false;
			}
			set
			{
				this.m_hasAnyFieldReferences = value;
			}
		}

		internal string SimpleParameterName
		{
			get
			{
				return this.m_simpleParameterName;
			}
			set
			{
				this.m_simpleParameterName = value;
			}
		}

		internal bool HasDynamicParameterReference
		{
			get
			{
				return this.m_hasDynamicParameterReference;
			}
			set
			{
				this.m_hasDynamicParameterReference = value;
			}
		}

		internal List<string> ReferencedParameters
		{
			get
			{
				return this.m_referencedParameters;
			}
		}

		internal int FieldIndex
		{
			get
			{
				return this.IntValue;
			}
		}

		internal RdlFunctionInfo RdlFunctionInfo
		{
			get
			{
				return this.m_rdlFunctionInfo;
			}
			set
			{
				this.m_rdlFunctionInfo = value;
			}
		}

		internal ScopedFieldInfo ScopedFieldInfo
		{
			get
			{
				return this.m_scopedFieldInfo;
			}
			set
			{
				this.m_scopedFieldInfo = value;
			}
		}

		internal LiteralInfo LiteralInfo
		{
			get
			{
				return this.m_literalInfo;
			}
			set
			{
				this.m_literalInfo = value;
			}
		}

		public int ID
		{
			get
			{
				return this.m_id;
			}
		}

		internal ExpressionInfo()
		{
		}

		internal static ExpressionInfo CreateConstExpression(string value)
		{
			ExpressionInfo expressionInfo = new ExpressionInfo();
			expressionInfo.Type = Types.Constant;
			expressionInfo.ConstantType = DataType.String;
			expressionInfo.OriginalText = value;
			expressionInfo.StringValue = value;
			return expressionInfo;
		}

		internal static ExpressionInfo CreateConstExpression(bool value)
		{
			ExpressionInfo expressionInfo = new ExpressionInfo();
			expressionInfo.Type = Types.Constant;
			expressionInfo.ConstantType = DataType.Boolean;
			expressionInfo.OriginalText = value.ToString(CultureInfo.InvariantCulture);
			expressionInfo.BoolValue = value;
			return expressionInfo;
		}

		internal static ExpressionInfo CreateConstExpression(int value)
		{
			ExpressionInfo expressionInfo = new ExpressionInfo();
			expressionInfo.Type = Types.Constant;
			expressionInfo.ConstantType = DataType.Integer;
			expressionInfo.OriginalText = value.ToString(CultureInfo.InvariantCulture);
			expressionInfo.IntValue = value;
			return expressionInfo;
		}

		internal static ExpressionInfo CreateEmptyExpression()
		{
			ExpressionInfo expressionInfo = new ExpressionInfo();
			expressionInfo.Type = Types.Expression;
			expressionInfo.OriginalText = null;
			expressionInfo.StringValue = null;
			return expressionInfo;
		}

		internal object GetDateTimeValue()
		{
			if (this.m_dateTimeOffsetValue.HasValue)
			{
				return this.m_dateTimeOffsetValue;
			}
			return this.m_dateTimeValue;
		}

		internal void SetDateTimeValue(DateTime dateTime)
		{
			this.m_dateTimeValue = dateTime;
		}

		internal void SetDateTimeValue(DateTimeOffset dateTimeOffset)
		{
			this.m_dateTimeOffsetValue = dateTimeOffset;
		}

		internal void Initialize(string propertyName, InitializationContext context)
		{
			this.Initialize(propertyName, context, true);
		}

		internal void Initialize(string propertyName, InitializationContext context, bool initializeDataOnError)
		{
			context.EnforceRdlSandboxContentRestrictions(this, propertyName);
			context.CheckVariableReferences(this.m_referencedVariables, propertyName);
			context.CheckFieldReferences(this.m_referencedFields, propertyName);
			context.CheckReportItemReferences(this.m_referencedReportItems, propertyName);
			context.CheckReportParameterReferences(this.m_referencedParameters, propertyName);
			context.CheckDataSetReference(this.m_referencedDataSets, propertyName);
			context.CheckDataSourceReference(this.m_referencedDataSources, propertyName);
			context.CheckScopeReferences(this.m_referencedScopes, propertyName);
			if (this.Type == Types.ScopedFieldReference)
			{
				this.ScopedFieldInfo.FieldIndex = context.ResolveScopedFieldReferenceToIndex(this.StringValue, this.ScopedFieldInfo.FieldName);
			}
			if (!context.ErrorContext.HasError || initializeDataOnError)
			{
				context.FillInFieldIndex(this);
				context.TransferAggregates(this.m_aggregates, propertyName);
				context.TransferRunningValues(this.m_runningValues, propertyName);
				context.TransferLookups(this.m_lookups, propertyName);
				context.FillInTokenIndex(this);
			}
			this.m_referencedFieldProperties = null;
			if (this.m_nullLevelInExpr && context.InRecursiveHierarchyRows && context.InRecursiveHierarchyColumns)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsLevelCallRecursiveHierarchyBothDimensions, Severity.Warning, context.ObjectType, context.ObjectName, null);
			}
			if (this.m_rdlFunctionInfo != null)
			{
				this.m_rdlFunctionInfo.Initialize(propertyName, context, initializeDataOnError);
			}
		}

		internal bool InitializeAxisExpression(string propertyName, InitializationContext context)
		{
			bool hasError = context.ErrorContext.HasError;
			context.ErrorContext.HasError = false;
			this.Initialize(propertyName, context, false);
			bool hasError2 = context.ErrorContext.HasError;
			context.ErrorContext.HasError = hasError;
			return !hasError2;
		}

		internal void AggregateInitialize(string dataSetName, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, InitializationContext context)
		{
			this.SpecialFunctionArgInitialize(dataSetName, objectType, objectName, propertyName, context, false);
		}

		private void SpecialFunctionArgInitialize(string dataSetName, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, InitializationContext context, bool isLookup)
		{
			context.EnforceRdlSandboxContentRestrictions(this, objectType, objectName, propertyName);
			context.AggregateCheckVariableReferences(this.m_referencedVariables, objectType, objectName, propertyName);
			context.AggregateCheckFieldReferences(this.m_referencedFields, dataSetName, objectType, objectName, propertyName);
			context.AggregateCheckReportItemReferences(this.m_referencedReportItems, objectType, objectName, propertyName);
			context.AggregateCheckDataSetReference(this.m_referencedDataSets, objectType, objectName, propertyName);
			context.AggregateCheckDataSourceReference(this.m_referencedDataSources, objectType, objectName, propertyName);
			context.FillInFieldIndex(this, dataSetName);
			if (!isLookup)
			{
				context.ExprHostBuilder.AggregateParamExprAdd(this);
			}
			if (this.m_inPrevious || isLookup)
			{
				context.TransferAggregates(this.m_aggregates, propertyName);
				context.TransferRunningValues(this.m_runningValues, propertyName);
			}
			else
			{
				context.TransferNestedAggregates(this.m_aggregates, propertyName);
			}
			if (!isLookup)
			{
				context.TransferLookups(this.m_lookups, objectType, objectName, propertyName, dataSetName);
			}
			if (this.m_rdlFunctionInfo != null)
			{
				this.m_rdlFunctionInfo.Initialize(propertyName, context, false);
			}
		}

		internal void LookupInitialize(string dataSetName, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, InitializationContext context)
		{
			this.SpecialFunctionArgInitialize(dataSetName, objectType, objectName, propertyName, context, true);
		}

		internal bool HasRecursiveAggregates()
		{
			if (this.m_aggregates != null)
			{
				for (int i = 0; i < this.m_aggregates.Count; i++)
				{
					if (this.m_aggregates[i].Recursive)
					{
						return true;
					}
				}
			}
			return false;
		}

		internal void GroupExpressionInitialize(InitializationContext context)
		{
			context.CheckVariableReferences(this.m_referencedVariables, "Group");
			context.CheckFieldReferences(this.m_referencedFields, "Group");
			context.CheckReportItemReferences(this.m_referencedReportItems, "Group");
			context.CheckReportParameterReferences(this.m_referencedParameters, "Group");
			context.CheckDataSetReference(this.m_referencedDataSets, "Group");
			context.CheckDataSourceReference(this.m_referencedDataSources, "Group");
			context.FillInFieldIndex(this);
			context.TransferGroupExpressionRowNumbers(this.m_runningValues);
			context.TransferLookups(this.m_lookups, "Group");
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Type, Token.Enum));
			list.Add(new MemberInfo(MemberName.StringValue, Token.String));
			list.Add(new MemberInfo(MemberName.BoolValue, Token.Boolean));
			list.Add(new MemberInfo(MemberName.IntValue, Token.Int32));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.OriginalValue, Token.String));
			list.Add(new MemberInfo(MemberName.InPrevious, Token.Boolean));
			list.Add(new MemberInfo(MemberName.DateTimeValue, Token.DateTime));
			list.Add(new MemberInfo(MemberName.FloatValue, Token.Double));
			list.Add(new MemberInfo(MemberName.ConstantType, Token.Enum));
			list.Add(new MemberInfo(MemberName.DateTimeOffsetValue, Token.Object));
			list.Add(new MemberInfo(MemberName.RdlFunctionInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RdlFunctionInfo));
			list.Add(new MemberInfo(MemberName.ScopedFieldInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScopedFieldInfo, Lifetime.AddedIn(200)));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ExpressionInfo.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Type:
					writer.WriteEnum((int)this.m_type);
					break;
				case MemberName.ConstantType:
					writer.WriteEnum((int)this.m_constantType);
					break;
				case MemberName.StringValue:
					writer.Write(this.m_stringValue);
					break;
				case MemberName.BoolValue:
					writer.Write(this.m_boolValue);
					break;
				case MemberName.IntValue:
					writer.Write(this.m_intValue);
					break;
				case MemberName.FloatValue:
					writer.Write(this.m_floatValue);
					break;
				case MemberName.DateTimeValue:
					writer.Write(this.m_dateTimeValue);
					break;
				case MemberName.DateTimeOffsetValue:
					writer.Write(this.m_dateTimeOffsetValue.HasValue ? ((object)this.m_dateTimeOffsetValue) : null);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				case MemberName.OriginalValue:
					writer.Write(this.m_originalText);
					break;
				case MemberName.InPrevious:
					writer.Write(this.m_inPrevious);
					break;
				case MemberName.RdlFunctionInfo:
					writer.Write(this.m_rdlFunctionInfo);
					break;
				case MemberName.ScopedFieldInfo:
					writer.Write(this.m_scopedFieldInfo);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ExpressionInfo.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Type:
					this.m_type = (Types)reader.ReadEnum();
					break;
				case MemberName.ConstantType:
					this.m_constantType = (DataType)reader.ReadEnum();
					break;
				case MemberName.StringValue:
					this.m_stringValue = reader.ReadString();
					break;
				case MemberName.BoolValue:
					this.m_boolValue = reader.ReadBoolean();
					break;
				case MemberName.IntValue:
					this.m_intValue = reader.ReadInt32();
					break;
				case MemberName.FloatValue:
					this.m_floatValue = reader.ReadDouble();
					break;
				case MemberName.DateTimeValue:
					this.m_dateTimeValue = reader.ReadDateTime();
					break;
				case MemberName.DateTimeOffsetValue:
				{
					object obj = reader.ReadVariant();
					if (obj != null)
					{
						this.m_dateTimeOffsetValue = (DateTimeOffset)obj;
					}
					else
					{
						this.m_dateTimeOffsetValue = null;
					}
					break;
				}
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.OriginalValue:
					this.m_originalText = reader.ReadString();
					break;
				case MemberName.InPrevious:
					this.m_inPrevious = reader.ReadBoolean();
					break;
				case MemberName.RdlFunctionInfo:
					this.m_rdlFunctionInfo = reader.ReadRIFObject<RdlFunctionInfo>();
					break;
				case MemberName.ScopedFieldInfo:
					this.m_scopedFieldInfo = (ScopedFieldInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo;
		}

		internal void SetAsSimpleFieldReference(string fieldName)
		{
			this.AddReferencedField(fieldName);
			this.HasAnyFieldReferences = true;
			this.Type = Types.Field;
			this.StringValue = fieldName;
		}

		internal void SetAsScopedFieldReference(string scopeName, string fieldName)
		{
			this.AddReferencedScope(new ScopeReference(scopeName, fieldName));
			ScopedFieldInfo scopedFieldInfo = new ScopedFieldInfo();
			scopedFieldInfo.FieldName = fieldName;
			this.Type = Types.ScopedFieldReference;
			this.StringValue = scopeName;
			this.ScopedFieldInfo = scopedFieldInfo;
		}

		internal void SetAsLiteral(LiteralInfo literal)
		{
			this.Type = Types.Literal;
			this.LiteralInfo = literal;
		}

		internal void SetAsRdlFunction(RdlFunctionInfo function)
		{
			this.Type = Types.RdlFunction;
			this.RdlFunctionInfo = function;
		}

		internal void AddReferencedField(string fieldName)
		{
			if (this.m_referencedFields == null)
			{
				this.m_referencedFields = new List<string>();
			}
			this.m_referencedFields.Add(fieldName);
		}

		internal void AddReferencedReportItemInOriginalText(string reportItemName, int index)
		{
			if (this.m_referencedReportItemPositionsInOriginalText == null)
			{
				this.m_referencedReportItemPositionsInOriginalText = new List<int>();
			}
			this.m_referencedReportItemPositionsInOriginalText.Add(index);
		}

		internal void AddReferencedReportItemInTransformedExpression(string reportItemName, int index)
		{
			if (this.m_referencedReportItems == null)
			{
				this.m_referencedReportItems = new List<string>();
			}
			if (this.m_referencedReportItemPositionsInTransformedExpression == null)
			{
				this.m_referencedReportItemPositionsInTransformedExpression = new List<int>();
			}
			this.m_referencedReportItemPositionsInTransformedExpression.Add(index);
			this.m_referencedReportItems.Add(reportItemName);
		}

		internal void AddMeDotValueInOriginalText(int index)
		{
			if (this.m_meDotValuePositionsInOriginalText == null)
			{
				this.m_meDotValuePositionsInOriginalText = new List<int>();
			}
			this.m_meDotValuePositionsInOriginalText.Add(index);
		}

		internal void AddMeDotValueInTransformedExpression(int index)
		{
			if (this.m_meDotValuePositionsInTranformedExpr == null)
			{
				this.m_meDotValuePositionsInTranformedExpr = new List<int>();
			}
			this.m_meDotValuePositionsInTranformedExpr.Add(index);
		}

		internal void AddReferencedVariable(string variableName, int index)
		{
			if (this.m_referencedVariables == null)
			{
				this.m_referencedVariables = new List<string>();
			}
			if (this.m_referencedVariablePositions == null)
			{
				this.m_referencedVariablePositions = new List<int>();
			}
			this.m_referencedVariablePositions.Add(index);
			this.m_referencedVariables.Add(variableName);
		}

		internal void AddReferencedParameter(string parameterName)
		{
			if (this.m_referencedParameters == null)
			{
				this.m_referencedParameters = new List<string>();
			}
			this.m_referencedParameters.Add(parameterName);
		}

		internal void AddReferencedDataSet(string dataSetName)
		{
			if (this.m_referencedDataSets == null)
			{
				this.m_referencedDataSets = new List<string>();
			}
			this.m_referencedDataSets.Add(dataSetName);
		}

		internal void AddReferencedDataSource(string dataSourceName)
		{
			if (this.m_referencedDataSources == null)
			{
				this.m_referencedDataSources = new List<string>();
			}
			this.m_referencedDataSources.Add(dataSourceName);
		}

		internal void AddReferencedScope(ScopeReference scopeReference)
		{
			if (this.m_referencedScopes == null)
			{
				this.m_referencedScopes = new List<ScopeReference>();
			}
			this.m_referencedScopes.Add(scopeReference);
		}

		internal void AddAggregate(DataAggregateInfo aggregate)
		{
			if (this.m_aggregates == null)
			{
				this.m_aggregates = new List<DataAggregateInfo>();
			}
			this.m_aggregates.Add(aggregate);
		}

		internal void AddRunningValue(RunningValueInfo runningValue)
		{
			if (this.m_runningValues == null)
			{
				this.m_runningValues = new List<RunningValueInfo>();
			}
			this.m_runningValues.Add(runningValue);
		}

		internal void AddLookup(LookupInfo lookup)
		{
			if (this.m_lookups == null)
			{
				this.m_lookups = new List<LookupInfo>();
			}
			this.m_lookups.Add(lookup);
		}

		internal DataAggregateInfo GetSumAggregateWithoutScope()
		{
			if (Types.Aggregate == this.m_type && this.m_aggregates != null)
			{
				Global.Tracer.Assert(1 == this.m_aggregates.Count);
				DataAggregateInfo dataAggregateInfo = this.m_aggregates[0];
				string text = default(string);
				if (DataAggregateInfo.AggregateTypes.Sum == dataAggregateInfo.AggregateType && !dataAggregateInfo.GetScope(out text))
				{
					return dataAggregateInfo;
				}
			}
			return null;
		}

		internal void AddDynamicPropertyReference(string fieldName)
		{
			Global.Tracer.Assert(null != fieldName, "(null != fieldName)");
			if (this.m_referencedFieldProperties == null)
			{
				this.m_referencedFieldProperties = new Hashtable();
			}
			else if (this.m_referencedFieldProperties.ContainsKey(fieldName))
			{
				this.m_referencedFieldProperties.Remove(fieldName);
			}
			this.m_referencedFieldProperties.Add(fieldName, null);
		}

		internal void AddStaticPropertyReference(string fieldName, string propertyName)
		{
			Global.Tracer.Assert(fieldName != null && null != propertyName, "(null != fieldName && null != propertyName)");
			if (this.m_referencedFieldProperties == null)
			{
				this.m_referencedFieldProperties = new Hashtable();
			}
			if (this.m_referencedFieldProperties.ContainsKey(fieldName))
			{
				Hashtable hashtable = this.m_referencedFieldProperties[fieldName] as Hashtable;
				if (hashtable != null)
				{
					hashtable[propertyName] = null;
				}
			}
			else
			{
				Hashtable hashtable2 = new Hashtable();
				hashtable2.Add(propertyName, null);
				this.m_referencedFieldProperties.Add(fieldName, hashtable2);
			}
		}

		internal void AddTransformedExpressionAggregateInfo(int position, string aggregateID, bool isRunningValue)
		{
			int num = 0;
			TransformedExprSpecialFunctionInfo.SpecialFunctionType funcType;
			if (isRunningValue)
			{
				num = this.m_runningValues.Count - 1;
				funcType = TransformedExprSpecialFunctionInfo.SpecialFunctionType.RunningValue;
			}
			else
			{
				num = this.m_aggregates.Count - 1;
				funcType = TransformedExprSpecialFunctionInfo.SpecialFunctionType.Aggregate;
			}
			this.AddTransformedSpecialFunctionInfo(position, aggregateID, funcType, num);
		}

		private void AddTransformedSpecialFunctionInfo(int position, string specialFunctionID, TransformedExprSpecialFunctionInfo.SpecialFunctionType funcType, int index)
		{
			if (this.m_transformedExprAggregateInfos == null)
			{
				this.m_transformedExprAggregateInfos = new List<TransformedExprSpecialFunctionInfo>();
			}
			this.m_transformedExprAggregateInfos.Add(new TransformedExprSpecialFunctionInfo(position, specialFunctionID, funcType, index));
		}

		internal void AddTransformedExpressionLookupInfo(int position, string lookupID)
		{
			this.AddTransformedSpecialFunctionInfo(position, lookupID, TransformedExprSpecialFunctionInfo.SpecialFunctionType.Lookup, this.m_lookups.Count - 1);
		}

		private int UpdateReferencedItemsCollection(ExpressionInfo meDotValueExpression, int referencedIndex, int meDotValuePositionInOriginalText, int meDotValuePositionInTransformedExpression, List<int> positionsInTransformedExpression, List<int> positionsInOriginalText, List<string> referencedValues, List<int> positionsInMeDotValueTransformedExpression, List<int> positionsInMeDotValueOriginalText, List<string> referencedMeDotValueValues)
		{
			int num = 8;
			for (int i = referencedIndex; i < positionsInTransformedExpression.Count; i++)
			{
				if (positionsInOriginalText != null && meDotValuePositionInOriginalText < positionsInOriginalText[i])
				{
					List<int> list;
					int index;
					(list = positionsInOriginalText)[index = i] = list[index] + (meDotValueExpression.OriginalText.Length - num);
				}
				if (meDotValuePositionInTransformedExpression < positionsInTransformedExpression[i])
				{
					List<int> list2;
					int index2;
					(list2 = positionsInTransformedExpression)[index2 = i] = list2[index2] + (meDotValueExpression.TransformedExpression.Length - num);
				}
				else
				{
					referencedIndex++;
				}
			}
			if (referencedMeDotValueValues != null)
			{
				for (int j = 0; j < referencedMeDotValueValues.Count; j++)
				{
					if (positionsInMeDotValueOriginalText != null)
					{
						int num2 = positionsInMeDotValueOriginalText[j];
						positionsInOriginalText.Insert(referencedIndex, meDotValuePositionInOriginalText + num2);
					}
					string item = referencedMeDotValueValues[j];
					int num3 = positionsInMeDotValueTransformedExpression[j];
					positionsInTransformedExpression.Insert(referencedIndex, meDotValuePositionInTransformedExpression + num3);
					referencedValues.Insert(referencedIndex, item);
					referencedIndex++;
				}
			}
			return referencedIndex;
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			ExpressionInfo expressionInfo = (ExpressionInfo)base.MemberwiseClone();
			if (this.RdlFunctionInfo != null)
			{
				expressionInfo.m_rdlFunctionInfo = (RdlFunctionInfo)this.m_rdlFunctionInfo.PublishClone(context);
				Global.Tracer.Assert(this.m_aggregates == null, "this.m_aggregates == null");
				Global.Tracer.Assert(this.m_runningValues == null, "this.m_aggregates == null");
				Global.Tracer.Assert(this.m_lookups == null, "this.m_aggregates == null");
				Global.Tracer.Assert(this.m_referencedReportItems == null, "this.m_aggregates == null");
				Global.Tracer.Assert(this.m_referencedReportItemPositionsInOriginalText == null, "this.m_aggregates == null");
				Global.Tracer.Assert(this.m_meDotValuePositionsInOriginalText == null, "this.m_aggregates == null");
				Global.Tracer.Assert(this.m_transformedExpression == null, "this.m_aggregates == null");
				return expressionInfo;
			}
			if (this.m_aggregates != null)
			{
				expressionInfo.m_aggregates = new List<DataAggregateInfo>(this.m_aggregates.Count);
				foreach (DataAggregateInfo aggregate in this.m_aggregates)
				{
					expressionInfo.m_aggregates.Add((DataAggregateInfo)aggregate.PublishClone(context));
				}
			}
			if (this.m_runningValues != null)
			{
				expressionInfo.m_runningValues = new List<RunningValueInfo>(this.m_runningValues.Count);
				foreach (RunningValueInfo runningValue in this.m_runningValues)
				{
					expressionInfo.m_runningValues.Add((RunningValueInfo)runningValue.PublishClone(context));
				}
			}
			if (this.m_lookups != null)
			{
				expressionInfo.m_lookups = new List<LookupInfo>(this.m_lookups.Count);
				foreach (LookupInfo lookup in this.m_lookups)
				{
					expressionInfo.m_lookups.Add((LookupInfo)lookup.PublishClone(context));
				}
			}
			if (this.m_referencedReportItems != null)
			{
				expressionInfo.m_referencedReportItems = new List<string>(this.m_referencedReportItems.Count);
				foreach (string referencedReportItem in this.m_referencedReportItems)
				{
					expressionInfo.m_referencedReportItems.Add((string)referencedReportItem.Clone());
				}
				context.AddExpressionThatReferencesReportItems(expressionInfo);
			}
			if (this.m_referencedReportItemPositionsInOriginalText != null)
			{
				expressionInfo.m_referencedReportItemPositionsInOriginalText = new List<int>(this.m_referencedReportItemPositionsInOriginalText.Count);
				foreach (int item in this.m_referencedReportItemPositionsInOriginalText)
				{
					expressionInfo.m_referencedReportItemPositionsInOriginalText.Add(item);
				}
			}
			if (this.m_meDotValuePositionsInOriginalText != null)
			{
				expressionInfo.m_meDotValuePositionsInOriginalText = new List<int>(this.m_meDotValuePositionsInOriginalText.Count);
				foreach (int item2 in this.m_meDotValuePositionsInOriginalText)
				{
					expressionInfo.m_meDotValuePositionsInOriginalText.Add(item2);
				}
			}
			if (this.m_transformedExpression != null)
			{
				StringBuilder stringBuilder = new StringBuilder(this.m_transformedExpression);
				StringBuilder stringBuilder2 = null;
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				bool flag = false;
				bool flag2 = false;
				if (this.m_transformedExprAggregateInfos != null)
				{
					expressionInfo.m_transformedExprAggregateInfos = new List<TransformedExprSpecialFunctionInfo>(this.m_transformedExprAggregateInfos.Count);
					foreach (TransformedExprSpecialFunctionInfo transformedExprAggregateInfo in this.m_transformedExprAggregateInfos)
					{
						expressionInfo.m_transformedExprAggregateInfos.Add((TransformedExprSpecialFunctionInfo)transformedExprAggregateInfo.PublishClone(context));
					}
					flag = true;
					num3 += this.m_transformedExprAggregateInfos.Count;
				}
				if (this.m_referencedVariablePositions != null)
				{
					expressionInfo.m_referencedVariablePositions = new List<int>(this.m_referencedVariablePositions.Count);
					foreach (int referencedVariablePosition in this.m_referencedVariablePositions)
					{
						expressionInfo.m_referencedVariablePositions.Add(referencedVariablePosition);
					}
					flag2 = true;
					num3 += this.m_referencedVariablePositions.Count;
				}
				if (this.m_meDotValuePositionsInTranformedExpr != null)
				{
					expressionInfo.m_meDotValuePositionsInTranformedExpr = new List<int>(this.m_meDotValuePositionsInTranformedExpr.Count);
					foreach (int item3 in this.m_meDotValuePositionsInTranformedExpr)
					{
						expressionInfo.m_meDotValuePositionsInTranformedExpr.Add(item3);
					}
				}
				if (this.m_referencedReportItemPositionsInTransformedExpression != null)
				{
					expressionInfo.m_referencedReportItemPositionsInTransformedExpression = new List<int>(this.m_referencedReportItemPositionsInTransformedExpression.Count);
					foreach (int item4 in this.m_referencedReportItemPositionsInTransformedExpression)
					{
						expressionInfo.m_referencedReportItemPositionsInTransformedExpression.Add(item4);
					}
				}
				int num4 = 11;
				int num5 = 8;
				int num6 = 0;
				int num7 = 0;
				int num8 = 0;
				int num9 = 0;
				int num10 = 0;
				int num11 = 0;
				int num12 = 0;
				int num13 = 0;
				int num14 = 0;
				int num15 = 0;
				if (flag || flag2)
				{
					stringBuilder2 = new StringBuilder();
					stringBuilder2.Append("=");
					for (int i = 0; i < num3; i++)
					{
						Global.Tracer.Assert(!flag || !flag2 || num14 >= this.m_transformedExprAggregateInfos.Count || num15 >= this.m_referencedVariablePositions.Count || this.m_transformedExprAggregateInfos[num14].Position != this.m_referencedVariablePositions[num15]);
						int num16 = 2147483647;
						int num17 = 2147483647;
						if (flag && num14 < this.m_transformedExprAggregateInfos.Count)
						{
							num16 = this.m_transformedExprAggregateInfos[num14].Position;
						}
						if (flag2 && num15 < this.m_referencedVariablePositions.Count)
						{
							num17 = this.m_referencedVariablePositions[num15];
						}
						if (num16 < num17)
						{
							num10 = this.m_transformedExprAggregateInfos[num14].Position;
							TransformedExprSpecialFunctionInfo transformedExprSpecialFunctionInfo = this.m_transformedExprAggregateInfos[num14];
							string functionID = transformedExprSpecialFunctionInfo.FunctionID;
							string text;
							int num18;
							if (transformedExprSpecialFunctionInfo.FunctionType == TransformedExprSpecialFunctionInfo.SpecialFunctionType.Lookup)
							{
								text = context.GetNewLookupID(functionID);
								num18 = num5;
							}
							else
							{
								text = context.GetNewAggregateID(functionID);
								num18 = num4;
							}
							TransformedExprSpecialFunctionInfo transformedExprSpecialFunctionInfo2 = expressionInfo.m_transformedExprAggregateInfos[num14];
							transformedExprSpecialFunctionInfo2.FunctionID = text;
							transformedExprSpecialFunctionInfo2.Position = num10 + num7;
							stringBuilder.Replace(functionID, text, num10 + num18 + num7, functionID.Length);
							stringBuilder2.Append(this.m_transformedExpression.Substring(num9, num10 - num9));
							num11 = stringBuilder2.Length;
							int indexIntoCollection = transformedExprSpecialFunctionInfo.IndexIntoCollection;
							string text2 = null;
							string text3 = null;
							switch (transformedExprSpecialFunctionInfo.FunctionType)
							{
							case TransformedExprSpecialFunctionInfo.SpecialFunctionType.Aggregate:
								text2 = this.m_aggregates[indexIntoCollection].GetAsString();
								text3 = expressionInfo.m_aggregates[indexIntoCollection].GetAsString();
								break;
							case TransformedExprSpecialFunctionInfo.SpecialFunctionType.RunningValue:
								text2 = this.m_runningValues[indexIntoCollection].GetAsString();
								text3 = expressionInfo.m_runningValues[indexIntoCollection].GetAsString();
								break;
							case TransformedExprSpecialFunctionInfo.SpecialFunctionType.Lookup:
								text2 = this.m_lookups[indexIntoCollection].GetAsString();
								text3 = expressionInfo.m_lookups[indexIntoCollection].GetAsString();
								break;
							default:
								Global.Tracer.Assert(false, "Unknown transformed item function type: {0}", transformedExprSpecialFunctionInfo.FunctionType);
								break;
							}
							stringBuilder2.Append(text3);
							num9 = num10 + num18 + functionID.Length;
							Global.Tracer.Assert(text.Length >= functionID.Length, "(newName.Length >= oldName.Length)");
							num6 = text.Length - functionID.Length;
							num8 = text3.Length - text2.Length;
							num7 += num6;
							num14++;
						}
						else if (num17 != 2147483647)
						{
							num10 = this.m_referencedVariablePositions[num15];
							string text4 = this.m_referencedVariables[num15];
							string newVariableName = context.GetNewVariableName(text4);
							expressionInfo.m_referencedVariablePositions[num15] = num10 + num7;
							stringBuilder.Replace(text4, newVariableName, num10 + num7, text4.Length);
							stringBuilder2.Append(this.m_transformedExpression.Substring(num9, num10 - num9));
							num11 = stringBuilder2.Length;
							stringBuilder2.Append(newVariableName);
							num9 = num10 + text4.Length;
							Global.Tracer.Assert(newVariableName.Length >= text4.Length, "(newName.Length >= oldName.Length)");
							num6 = newVariableName.Length - text4.Length;
							num8 = num6;
							num7 += num6;
							num15++;
						}
						if (num6 != 0)
						{
							if (this.m_meDotValuePositionsInTranformedExpr != null)
							{
								for (int j = num13; j < this.m_meDotValuePositionsInTranformedExpr.Count; j++)
								{
									if (num11 > this.m_meDotValuePositionsInTranformedExpr[j])
									{
										num13++;
									}
									else
									{
										int num19 = this.m_meDotValuePositionsInTranformedExpr[j];
										expressionInfo.m_meDotValuePositionsInTranformedExpr[j] = num19 + num6;
									}
								}
							}
							if (this.m_referencedReportItemPositionsInTransformedExpression != null)
							{
								for (int k = num2; k < this.m_referencedReportItemPositionsInTransformedExpression.Count; k++)
								{
									if (num10 > this.m_referencedReportItemPositionsInTransformedExpression[k])
									{
										num2++;
									}
									else
									{
										int num20 = this.m_referencedReportItemPositionsInTransformedExpression[k];
										expressionInfo.m_referencedReportItemPositionsInTransformedExpression[k] = num20 + num6;
									}
								}
							}
						}
						if (num8 != 0)
						{
							if (this.m_meDotValuePositionsInOriginalText != null)
							{
								for (int l = num12; l < this.m_meDotValuePositionsInOriginalText.Count; l++)
								{
									if (num11 > this.m_meDotValuePositionsInOriginalText[l])
									{
										num12++;
									}
									else
									{
										int num21 = this.m_meDotValuePositionsInOriginalText[l];
										expressionInfo.m_meDotValuePositionsInOriginalText[l] = num21 + num8;
									}
								}
							}
							if (this.m_referencedReportItemPositionsInOriginalText != null)
							{
								for (int m = num; m < this.m_referencedReportItemPositionsInOriginalText.Count; m++)
								{
									if (num11 > this.m_referencedReportItemPositionsInOriginalText[m])
									{
										num++;
									}
									else
									{
										int num22 = this.m_referencedReportItemPositionsInOriginalText[m];
										expressionInfo.m_referencedReportItemPositionsInOriginalText[m] = num22 + num8;
									}
								}
							}
						}
					}
					stringBuilder2.Append(this.m_transformedExpression.Substring(num9));
					Global.Tracer.Assert(num14 + num15 == num3, "((currentAggIDIndex + currentVariableIndex) == potentialChangeCount)");
				}
				else
				{
					stringBuilder2 = new StringBuilder(this.m_originalText);
				}
				expressionInfo.m_originalText = stringBuilder2.ToString();
				expressionInfo.m_transformedExpression = stringBuilder.ToString();
			}
			else if (expressionInfo.m_aggregates != null && expressionInfo.m_aggregates.Count > 0)
			{
				expressionInfo.m_stringValue = expressionInfo.m_aggregates[0].Name;
				expressionInfo.m_originalText = expressionInfo.m_aggregates[0].GetAsString();
			}
			else if (expressionInfo.m_runningValues != null && expressionInfo.m_runningValues.Count > 0)
			{
				expressionInfo.m_stringValue = expressionInfo.m_runningValues[0].Name;
				expressionInfo.m_originalText = expressionInfo.m_runningValues[0].GetAsString();
			}
			else if (expressionInfo.m_lookups != null && expressionInfo.m_lookups.Count > 0)
			{
				expressionInfo.m_stringValue = expressionInfo.m_lookups[0].Name;
				expressionInfo.m_originalText = expressionInfo.m_lookups[0].GetAsString();
			}
			return expressionInfo;
		}

		internal void UpdateReportItemReferences(AutomaticSubtotalContext context)
		{
			StringBuilder stringBuilder = new StringBuilder(this.m_transformedExpression);
			StringBuilder stringBuilder2 = new StringBuilder(this.m_originalText);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			for (int i = 0; i < this.m_referencedReportItemPositionsInTransformedExpression.Count; i++)
			{
				string text = this.m_referencedReportItems[i];
				string newReportItemName = context.GetNewReportItemName(text);
				num2 = 0;
				if (newReportItemName != text)
				{
					this.m_referencedReportItems[i] = newReportItemName;
					if (this.m_transformedExpression != null)
					{
						num4 = this.m_referencedReportItemPositionsInTransformedExpression[i];
						stringBuilder.Replace(text, newReportItemName, num4 + num, text.Length);
						this.m_referencedReportItemPositionsInTransformedExpression[i] = num4 + num;
					}
					num3 = this.m_referencedReportItemPositionsInOriginalText[i];
					stringBuilder2.Replace(text, newReportItemName, num3 + num, text.Length);
					this.m_referencedReportItemPositionsInOriginalText[i] = num3 + num;
					num2 = newReportItemName.Length - text.Length;
					num += num2;
					if (num2 != 0)
					{
						if (this.m_transformedExpression != null && this.m_transformedExprAggregateInfos != null)
						{
							for (int j = num5; j < this.m_transformedExprAggregateInfos.Count; j++)
							{
								if (num4 > this.m_transformedExprAggregateInfos[j].Position)
								{
									num5++;
								}
								else
								{
									int position = this.m_transformedExprAggregateInfos[j].Position;
									this.m_transformedExprAggregateInfos[j].Position = position + num2;
								}
							}
						}
						if (this.m_referencedVariablePositions != null)
						{
							for (int k = num6; k < this.m_referencedVariablePositions.Count; k++)
							{
								if (num4 > this.m_referencedVariablePositions[k])
								{
									num6++;
								}
								else
								{
									int num9 = this.m_referencedVariablePositions[k];
									this.m_referencedVariablePositions[k] = num9 + num2;
								}
							}
						}
						if (this.m_meDotValuePositionsInOriginalText != null)
						{
							for (int l = num8; l < this.m_meDotValuePositionsInOriginalText.Count; l++)
							{
								if (num3 > this.m_meDotValuePositionsInOriginalText[l])
								{
									num8++;
								}
								else
								{
									int num10 = this.m_meDotValuePositionsInOriginalText[l];
									this.m_meDotValuePositionsInOriginalText[l] = num10 + num2;
								}
							}
							for (int m = num7; m < this.m_meDotValuePositionsInTranformedExpr.Count; m++)
							{
								if (num4 > this.m_meDotValuePositionsInTranformedExpr[m])
								{
									num7++;
								}
								else
								{
									int num11 = this.m_meDotValuePositionsInTranformedExpr[m];
									this.m_meDotValuePositionsInTranformedExpr[m] = num11 + num2;
								}
							}
						}
					}
				}
			}
			this.m_transformedExpression = stringBuilder.ToString();
			this.m_originalText = stringBuilder2.ToString();
		}

		public void SetID(int id)
		{
			this.m_id = id;
		}
	}
}
