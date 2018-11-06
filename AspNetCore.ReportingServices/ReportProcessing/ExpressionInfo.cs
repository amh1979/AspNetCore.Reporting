using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;
using System.Collections;
using System.Text;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal class ExpressionInfo
	{
		internal enum Types
		{
			Expression,
			Field,
			Aggregate,
			Constant,
			Token
		}

		private Types m_type;

		private string m_stringValue;

		private bool m_boolValue;

		private int m_intValue;

		private int m_exprHostID = -1;

		private string m_originalText;

		[NonSerialized]
		private string m_transformedExpression;

		[NonSerialized]
		private IntList m_transformedExpressionAggregatePositions;

		[NonSerialized]
		private StringList m_transformedExpressionAggregateIDs;

		[NonSerialized]
		private StringList m_referencedFields;

		[NonSerialized]
		private StringList m_referencedReportItems;

		[NonSerialized]
		private StringList m_referencedParameters;

		[NonSerialized]
		private StringList m_referencedDataSets;

		[NonSerialized]
		private StringList m_referencedDataSources;

		[NonSerialized]
		private DataAggregateInfoList m_aggregates;

		[NonSerialized]
		private RunningValueInfoList m_runningValues;

		[NonSerialized]
		private int m_compileTimeID = -1;

		[NonSerialized]
		private Hashtable m_referencedFieldProperties;

		[NonSerialized]
		private bool m_dynamicFieldReferences;

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

		internal bool IsExpression
		{
			get
			{
				return this.m_type != Types.Constant;
			}
		}

		internal string Value
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

		internal IntList TransformedExpressionAggregatePositions
		{
			get
			{
				return this.m_transformedExpressionAggregatePositions;
			}
			set
			{
				this.m_transformedExpressionAggregatePositions = value;
			}
		}

		internal StringList TransformedExpressionAggregateIDs
		{
			get
			{
				return this.m_transformedExpressionAggregateIDs;
			}
			set
			{
				this.m_transformedExpressionAggregateIDs = value;
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

		internal DataAggregateInfoList Aggregates
		{
			get
			{
				return this.m_aggregates;
			}
		}

		internal RunningValueInfoList RunningValues
		{
			get
			{
				return this.m_runningValues;
			}
		}

		internal Hashtable ReferencedFieldProperties
		{
			get
			{
				return this.m_referencedFieldProperties;
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

		internal ExpressionInfo()
		{
		}

		internal ExpressionInfo(Types type)
		{
			this.m_type = Types.Expression;
		}

		internal void Initialize(string propertyName, InitializationContext context)
		{
			context.CheckFieldReferences(this.m_referencedFields, propertyName);
			context.CheckReportItemReferences(this.m_referencedReportItems, propertyName);
			context.CheckReportParameterReferences(this.m_referencedParameters, propertyName);
			context.CheckDataSetReference(this.m_referencedDataSets, propertyName);
			context.CheckDataSourceReference(this.m_referencedDataSources, propertyName);
			if ((LocationFlags.InMatrixCellTopLevelItem & context.Location) != 0 && this.m_referencedFields != null)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsNonAggregateInMatrixCell, Severity.Warning, context.ObjectType, context.ObjectName, propertyName);
			}
			context.FillInFieldIndex(this);
			context.TransferAggregates(this.m_aggregates, propertyName);
			context.TransferRunningValues(this.m_runningValues, propertyName);
			context.MergeFieldPropertiesIntoDataset(this);
			context.FillInTokenIndex(this);
			this.m_referencedFieldProperties = null;
		}

		internal void AggregateInitialize(string dataSetName, ObjectType objectType, string objectName, string propertyName, InitializationContext context)
		{
			context.AggregateCheckFieldReferences(this.m_referencedFields, dataSetName, objectType, objectName, propertyName);
			context.AggregateCheckReportItemReferences(this.m_referencedReportItems, objectType, objectName, propertyName);
			context.AggregateCheckDataSetReference(this.m_referencedDataSets, objectType, objectName, propertyName);
			context.AggregateCheckDataSourceReference(this.m_referencedDataSources, objectType, objectName, propertyName);
			context.MergeFieldPropertiesIntoDataset(this);
			context.FillInFieldIndex(this, dataSetName);
			context.ExprHostBuilder.AggregateParamExprAdd(this);
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
			context.CheckFieldReferences(this.m_referencedFields, "Group");
			context.CheckReportItemReferences(this.m_referencedReportItems, "Group");
			context.CheckReportParameterReferences(this.m_referencedParameters, "Group");
			context.CheckDataSetReference(this.m_referencedDataSets, "Group");
			context.CheckDataSourceReference(this.m_referencedDataSources, "Group");
			context.MergeFieldPropertiesIntoDataset(this);
			context.FillInFieldIndex(this);
			context.TransferGroupExpressionRowNumbers(this.m_runningValues);
		}

		internal ExpressionInfo DeepClone(InitializationContext context)
		{
			Global.Tracer.Assert(-1 == this.m_exprHostID);
			ExpressionInfo expressionInfo = new ExpressionInfo();
			expressionInfo.m_type = this.m_type;
			expressionInfo.m_compileTimeID = this.m_compileTimeID;
			expressionInfo.m_stringValue = this.m_stringValue;
			expressionInfo.m_boolValue = this.m_boolValue;
			expressionInfo.m_intValue = this.m_intValue;
			expressionInfo.m_originalText = this.m_originalText;
			expressionInfo.m_referencedFields = this.m_referencedFields;
			expressionInfo.m_referencedParameters = this.m_referencedParameters;
			Global.Tracer.Assert(null == this.m_referencedReportItems);
			if (this.m_aggregates != null)
			{
				int count = this.m_aggregates.Count;
				expressionInfo.m_aggregates = new DataAggregateInfoList(count);
				for (int i = 0; i < count; i++)
				{
					expressionInfo.m_aggregates.Add(this.m_aggregates[i].DeepClone(context));
				}
			}
			if (this.m_runningValues != null)
			{
				int count2 = this.m_runningValues.Count;
				expressionInfo.m_runningValues = new RunningValueInfoList(count2);
				for (int j = 0; j < count2; j++)
				{
					expressionInfo.m_runningValues.Add(this.m_runningValues[j].DeepClone(context));
				}
			}
			if (this.m_transformedExpression != null)
			{
				StringBuilder stringBuilder = new StringBuilder(this.m_transformedExpression);
				if (context.AggregateRewriteMap != null && this.m_transformedExpressionAggregateIDs != null)
				{
					Global.Tracer.Assert(this.m_transformedExpressionAggregatePositions != null && this.m_transformedExpressionAggregateIDs.Count == this.m_transformedExpressionAggregatePositions.Count && null != this.m_transformedExpression);
					int num = 11;
					for (int k = 0; k < this.m_transformedExpressionAggregateIDs.Count; k++)
					{
						string text = this.m_transformedExpressionAggregateIDs[k];
						string text2 = context.AggregateRewriteMap[text] as string;
						int num2 = this.m_transformedExpressionAggregatePositions[k];
						if (text2 != null)
						{
							Global.Tracer.Assert(text != null && text2 != null && text2.Length >= text.Length);
							Global.Tracer.Assert(this.m_transformedExpression.Length > num2 + num);
							stringBuilder.Replace(text, text2, num2 + num, text.Length);
							num += text2.Length - text.Length;
						}
					}
				}
				expressionInfo.m_transformedExpression = stringBuilder.ToString();
			}
			return expressionInfo;
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Type, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.StringValue, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.BoolValue, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.IntValue, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.OriginalValue, Token.String));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}

		internal void AddReferencedField(string fieldName)
		{
			if (this.m_referencedFields == null)
			{
				this.m_referencedFields = new StringList();
			}
			this.m_referencedFields.Add(fieldName);
		}

		internal void AddReferencedReportItem(string reportItemName)
		{
			if (this.m_referencedReportItems == null)
			{
				this.m_referencedReportItems = new StringList();
			}
			this.m_referencedReportItems.Add(reportItemName);
		}

		internal void AddReferencedParameter(string parameterName)
		{
			if (this.m_referencedParameters == null)
			{
				this.m_referencedParameters = new StringList();
			}
			this.m_referencedParameters.Add(parameterName);
		}

		internal void AddReferencedDataSet(string dataSetName)
		{
			if (this.m_referencedDataSets == null)
			{
				this.m_referencedDataSets = new StringList();
			}
			this.m_referencedDataSets.Add(dataSetName);
		}

		internal void AddReferencedDataSource(string dataSourceName)
		{
			if (this.m_referencedDataSources == null)
			{
				this.m_referencedDataSources = new StringList();
			}
			this.m_referencedDataSources.Add(dataSourceName);
		}

		internal void AddAggregate(DataAggregateInfo aggregate)
		{
			if (this.m_aggregates == null)
			{
				this.m_aggregates = new DataAggregateInfoList();
			}
			this.m_aggregates.Add(aggregate);
		}

		internal void AddRunningValue(RunningValueInfo runningValue)
		{
			if (this.m_runningValues == null)
			{
				this.m_runningValues = new RunningValueInfoList();
			}
			this.m_runningValues.Add(runningValue);
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
			Global.Tracer.Assert(null != fieldName);
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
			Global.Tracer.Assert(fieldName != null && null != propertyName);
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
	}
}
