using Microsoft.VisualBasic;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class VBExpressionParser : ExpressionParser
	{
		private sealed class ReportRegularExpressions
		{
			internal Regex NonConstant;

			internal Regex FieldDetection;

			internal Regex ReportItemsDetection;

			internal Regex ParametersDetection;

			internal Regex PageGlobalsDetection;

			internal Regex AggregatesDetection;

			internal Regex UserDetection;

			internal Regex DataSetsDetection;

			internal Regex DataSourcesDetection;

			internal Regex MeDotValueDetection;

			internal Regex IllegalCharacterDetection;

			internal Regex LineTerminatorDetection;

			internal Regex FieldOnly;

			internal Regex ParameterOnly;

			internal Regex StringLiteralOnly;

			internal Regex NothingOnly;

			internal Regex ReportItemName;

			internal Regex FieldName;

			internal Regex ParameterName;

			internal Regex DataSetName;

			internal Regex DataSourceName;

			internal Regex SpecialFunction;

			internal Regex Arguments;

			internal Regex DynamicFieldReference;

			internal Regex DynamicFieldPropertyReference;

			internal Regex StaticFieldPropertyReference;

			internal Regex RewrittenCommandText;

			internal Regex ExtendedPropertyName;

			internal Regex FieldWithExtendedProperty;

			internal static readonly ReportRegularExpressions Value = new ReportRegularExpressions();

			private ReportRegularExpressions()
			{
				RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline;
				this.NonConstant = new Regex("^\\s*=", options);
				string text = Regex.Escape("-+()#,:&*/\\^<=>");
				string text2 = Regex.Escape("!");
				string text3 = Regex.Escape(".");
				string text4 = "[" + text2 + text3 + "]";
				string text5 = "(^|[" + text + "\\s])";
				string text6 = "($|[" + text + text2 + text3 + "\\s])";
				this.FieldDetection = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + "(?<detected>Fields)" + text6, options);
				this.ReportItemsDetection = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + "(?<detected>ReportItems)" + text6, options);
				this.ParametersDetection = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + "(?<detected>Parameters)" + text6, options);
				this.PageGlobalsDetection = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + "(?<detected>(Globals" + text4 + "PageNumber)|(Globals" + text4 + "TotalPages))" + text6, options);
				this.AggregatesDetection = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + "(?<detected>Aggregates)" + text6, options);
				this.UserDetection = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + "(?<detected>User)" + text6, options);
				this.DataSetsDetection = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + "(?<detected>DataSets)" + text6, options);
				this.DataSourcesDetection = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + "(?<detected>DataSources)" + text6, options);
				this.MeDotValueDetection = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + "(?<detected>(?:Me.)?Value)" + text6, options);
				string text7 = Regex.Escape(":");
				string text8 = Regex.Escape("#");
				string text9 = "(" + text8 + "[^" + text8 + "]*" + text8 + ")";
				string text10 = Regex.Escape(":=");
				this.LineTerminatorDetection = new Regex("(?<detected>(\\u000D\\u000A)|([\\u000D\\u000A\\u2028\\u2029]))", options);
				this.IllegalCharacterDetection = new Regex("(\"((\"\")|[^\"])*\")|" + text9 + "|" + text10 + "|(?<detected>" + text7 + ")", options);
				string text11 = "[\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Pc}][\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Pc}\\p{Nd}\\p{Mn}\\p{Mc}\\p{Cf}]*";
				string str = "ReportItems" + text2 + "(?<reportitemname>" + text11 + ")";
				string text12 = "Fields" + text2 + "(?<fieldname>" + text11 + ")";
				string text13 = "Parameters" + text2 + "(?<parametername>" + text11 + ")";
				string text14 = "DataSets" + text2 + "(?<datasetname>" + text11 + ")";
				string str2 = "DataSources" + text2 + "(?<datasourcename>" + text11 + ")";
				string text15 = "Fields((" + text2 + "(?<fieldname>" + text11 + "))|((" + text3 + "Item)?" + Regex.Escape("(") + "\"(?<fieldname>" + text11 + ")\"" + Regex.Escape(")") + "))";
				this.ExtendedPropertyName = new Regex("(Value|IsMissing|UniqueName|BackgroundColor|Color|FontFamily|Fontsize|FontWeight|FontStyle|TextDecoration|FormattedValue|Key|LevelNumber|ParentUniqueName)", options);
				string text16 = "(" + text3 + "Properties)?" + Regex.Escape("(") + "\"(?<propertyname>" + text11 + ")\"" + Regex.Escape(")");
				this.FieldWithExtendedProperty = new Regex("^\\s*" + text15 + "((" + text3 + this.ExtendedPropertyName + ")|(" + text16 + "))\\s*$", options);
				this.DynamicFieldReference = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + "(?<detected>(Fields(" + text3 + "Item)?" + Regex.Escape("(") + "))", options);
				this.DynamicFieldPropertyReference = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + text12 + "(" + text3 + "Properties)?" + Regex.Escape("("), options);
				this.StaticFieldPropertyReference = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + text12 + text3 + "(?<propertyname>" + text11 + ")", options);
				this.FieldOnly = new Regex("^\\s*" + text12 + text3 + "Value\\s*$", options);
				this.RewrittenCommandText = new Regex("^\\s*" + text14 + text3 + "RewrittenCommandText\\s*$", options);
				this.ParameterOnly = new Regex("^\\s*" + text13 + text3 + "Value\\s*$", options);
				this.StringLiteralOnly = new Regex("^\\s*\"(?<string>((\"\")|[^\"])*)\"\\s*$", options);
				this.NothingOnly = new Regex("^\\s*Nothing\\s*$", options);
				this.ReportItemName = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + str, options);
				this.FieldName = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + text12, options);
				this.ParameterName = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + text13, options);
				this.DataSetName = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + text14, options);
				this.DataSourceName = new Regex("(\"((\"\")|[^\"])*\")|" + text5 + str2, options);
				this.SpecialFunction = new Regex("(\"((\"\")|[^\"])*\")|(?<prefix>" + text5 + ")(?<sfname>RunningValue|RowNumber|First|Last|Previous|Sum|Avg|Max|Min|CountDistinct|Count|CountRows|StDevP|VarP|StDev|Var|Aggregate)\\s*\\(", options);
				string text17 = Regex.Escape("(");
				string text18 = Regex.Escape(")");
				string text19 = Regex.Escape(",");
				this.Arguments = new Regex("(\"((\"\")|[^\"])*\")|(?<openParen>" + text17 + ")|(?<closeParen>" + text18 + ")|(?<comma>" + text19 + ")", options);
			}
		}

		private const string RunningValue = "RunningValue";

		private const string RowNumber = "RowNumber";

		private const string Previous = "Previous";

		private const string Star = "*";

		private ReportRegularExpressions m_regexes;

		private int m_numberOfAggregates;

		private int m_numberOfRunningValues;

		private bool m_bodyRefersToReportItems;

		private bool m_pageSectionRefersToReportItems;

		private ExpressionContext m_context;

		internal override bool BodyRefersToReportItems
		{
			get
			{
				return this.m_bodyRefersToReportItems;
			}
		}

		internal override bool PageSectionRefersToReportItems
		{
			get
			{
				return this.m_pageSectionRefersToReportItems;
			}
		}

		internal override int NumberOfAggregates
		{
			get
			{
				return this.m_numberOfAggregates;
			}
		}

		internal override int LastID
		{
			get
			{
				return this.m_numberOfAggregates + this.m_numberOfRunningValues;
			}
		}

		internal VBExpressionParser(ErrorContext errorContext)
			: base(errorContext)
		{
			this.m_regexes = ReportRegularExpressions.Value;
			this.m_numberOfAggregates = 0;
			this.m_numberOfRunningValues = 0;
			this.m_bodyRefersToReportItems = false;
			this.m_pageSectionRefersToReportItems = false;
		}

		internal override CodeDomProvider GetCodeCompiler()
		{
           
			return new VBCodeProvider();
		}

		internal override string GetCompilerArguments()
		{
			return "/optimize+";
		}

		internal override ExpressionInfo ParseExpression(string expression, ExpressionContext context)
		{
			Global.Tracer.Assert(null != expression);
			string text = default(string);
			return this.Lex(expression, context, out text);
		}

		internal override ExpressionInfo ParseExpression(string expression, ExpressionContext context, DetectionFlags flag, out bool reportParameterReferenced, out string reportParameterName, out bool userCollectionReferenced)
		{
			string expression2 = default(string);
			ExpressionInfo expressionInfo = this.Lex(expression, context, out expression2);
			reportParameterReferenced = false;
			reportParameterName = null;
			userCollectionReferenced = false;
			if (expressionInfo.Type == ExpressionInfo.Types.Expression)
			{
				if ((flag & DetectionFlags.ParameterReference) != 0)
				{
					reportParameterReferenced = true;
					reportParameterName = this.GetReferencedReportParameters(expression2);
				}
				if ((flag & DetectionFlags.UserReference) != 0)
				{
					userCollectionReferenced = this.DetectUserReference(expression2);
				}
			}
			return expressionInfo;
		}

		internal override ExpressionInfo ParseExpression(string expression, ExpressionContext context, out bool userCollectionReferenced)
		{
			string expression2 = default(string);
			ExpressionInfo expressionInfo = this.Lex(expression, context, out expression2);
			userCollectionReferenced = false;
			if (expressionInfo.Type == ExpressionInfo.Types.Expression)
			{
				userCollectionReferenced = this.DetectUserReference(expression2);
			}
			return expressionInfo;
		}

		internal override void ConvertField2ComplexExpr(ref ExpressionInfo info)
		{
			Global.Tracer.Assert(info.Type == ExpressionInfo.Types.Field);
			info.Type = ExpressionInfo.Types.Expression;
			info.TransformedExpression = "Fields!" + info.Value + ".Value";
		}

		private ExpressionInfo Lex(string expression, ExpressionContext context, out string vbExpression)
		{
			vbExpression = null;
			this.m_context = context;
			ExpressionInfo expressionInfo = context.ParseExtended ? new ExpressionInfoExtended() : new ExpressionInfo();
			expressionInfo.OriginalText = expression;
			Match match = this.m_regexes.NonConstant.Match(expression);
			if (!match.Success)
			{
				expressionInfo.Type = ExpressionInfo.Types.Constant;
				switch (context.ConstantType)
				{
				case ConstantType.String:
					expressionInfo.Value = expression;
					break;
				case ConstantType.Boolean:
				{
					bool boolValue;
					try
					{
						boolValue = XmlConvert.ToBoolean(expression);
					}
					catch
					{
						boolValue = false;
						base.m_errorContext.Register(ProcessingErrorCode.rsInvalidBooleanConstant, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, expression);
					}
					expressionInfo.BoolValue = boolValue;
					break;
				}
				case ConstantType.Integer:
				{
					int intValue;
					try
					{
						intValue = XmlConvert.ToInt32(expression);
					}
					catch
					{
						intValue = 0;
						base.m_errorContext.Register(ProcessingErrorCode.rsInvalidIntegerConstant, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, expression);
					}
					expressionInfo.IntValue = intValue;
					break;
				}
				default:
					Global.Tracer.Assert(false);
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
			}
			else
			{
				GrammarFlags grammarFlags = (GrammarFlags)(((this.m_context.Location & LocationFlags.InPageSection) == (LocationFlags)0) ? (ExpressionParser.ExpressionType2Restrictions(this.m_context.ExpressionType) | Restrictions.InBody) : (ExpressionParser.ExpressionType2Restrictions(this.m_context.ExpressionType) | Restrictions.InPageSection));
				vbExpression = expression.Substring(match.Length);
				this.VBLex(vbExpression, false, grammarFlags, expressionInfo);
			}
			return expressionInfo;
		}

		private string GetReferencedReportParameters(string expression)
		{
			string result = null;
			Match match = this.m_regexes.ParameterOnly.Match(expression);
			if (match.Success)
			{
				result = match.Result("${parametername}");
			}
			return result;
		}

		private bool DetectUserReference(string expression)
		{
			return this.Detected(expression, this.m_regexes.UserDetection);
		}

		private void VBLex(string expression, bool isParameter, GrammarFlags grammarFlags, ExpressionInfo expressionInfo)
		{
			if ((grammarFlags & GrammarFlags.DenyFields) == (GrammarFlags)0)
			{
				Match match = this.m_regexes.FieldOnly.Match(expression);
				if (match.Success)
				{
					string text = match.Result("${fieldname}");
					expressionInfo.AddReferencedField(text);
					expressionInfo.Type = ExpressionInfo.Types.Field;
					expressionInfo.Value = text;
					return;
				}
				if (this.m_context.ParseExtended)
				{
					match = this.m_regexes.FieldWithExtendedProperty.Match(expression);
					if (match.Success)
					{
						((ExpressionInfoExtended)expressionInfo).IsExtendedSimpleFieldReference = true;
					}
				}
			}
			if ((grammarFlags & GrammarFlags.DenyDataSets) == (GrammarFlags)0)
			{
				Match match2 = this.m_regexes.RewrittenCommandText.Match(expression);
				if (match2.Success)
				{
					string text2 = match2.Result("${datasetname}");
					expressionInfo.AddReferencedDataSet(text2);
					expressionInfo.Type = ExpressionInfo.Types.Token;
					expressionInfo.Value = text2;
					return;
				}
			}
			this.EnforceRestrictions(ref expression, isParameter, grammarFlags);
			string text3 = string.Empty;
			int num = 0;
			bool flag = false;
			while (num < expression.Length)
			{
				Match match3 = this.m_regexes.SpecialFunction.Match(expression, num);
				if (!match3.Success)
				{
					text3 += expression.Substring(num);
					num = expression.Length;
				}
				else
				{
					text3 += expression.Substring(num, match3.Index - num);
					string text4 = match3.Result("${sfname}");
					if (text4 == null || text4.Length == 0)
					{
						text3 += match3.Value;
						num = match3.Index + match3.Length;
					}
					else
					{
						text3 += match3.Result("${prefix}");
						num = match3.Index + match3.Length;
						string text5 = this.CreateAggregateID();
						if (string.Compare(text4, "Previous", StringComparison.OrdinalIgnoreCase) == 0)
						{
							RunningValueInfo runningValueInfo = default(RunningValueInfo);
							this.GetPreviousAggregate(num, text4, expression, isParameter, grammarFlags, out num, out runningValueInfo);
							runningValueInfo.Name = text5;
							expressionInfo.AddRunningValue(runningValueInfo);
						}
						else if (string.Compare(text4, "RunningValue", StringComparison.OrdinalIgnoreCase) == 0)
						{
							RunningValueInfo runningValueInfo2 = default(RunningValueInfo);
							this.GetRunningValue(num, text4, expression, isParameter, grammarFlags, out num, out runningValueInfo2);
							runningValueInfo2.Name = text5;
							expressionInfo.AddRunningValue(runningValueInfo2);
						}
						else if (string.Compare(text4, "RowNumber", StringComparison.OrdinalIgnoreCase) == 0)
						{
							RunningValueInfo runningValueInfo3 = default(RunningValueInfo);
							this.GetRowNumber(num, text4, expression, isParameter, grammarFlags, out num, out runningValueInfo3);
							runningValueInfo3.Name = text5;
							expressionInfo.AddRunningValue(runningValueInfo3);
						}
						else
						{
							DataAggregateInfo dataAggregateInfo = default(DataAggregateInfo);
							this.GetAggregate(num, text4, expression, isParameter, grammarFlags, out num, out dataAggregateInfo);
							dataAggregateInfo.Name = text5;
							expressionInfo.AddAggregate(dataAggregateInfo);
						}
						if (!flag)
						{
							flag = true;
							if (text3.Trim().Length == 0 && expression.Substring(num).Trim().Length == 0)
							{
								expressionInfo.Type = ExpressionInfo.Types.Aggregate;
								expressionInfo.Value = text5;
								return;
							}
						}
						if (expressionInfo.TransformedExpressionAggregatePositions == null)
						{
							expressionInfo.TransformedExpressionAggregatePositions = new IntList();
							expressionInfo.TransformedExpressionAggregateIDs = new StringList();
						}
						expressionInfo.TransformedExpressionAggregatePositions.Add(text3.Length);
						expressionInfo.TransformedExpressionAggregateIDs.Add(text5);
						text3 = text3 + "Aggregates!" + text5;
					}
				}
			}
			this.GetReferencedFieldNames(text3, expressionInfo);
			this.GetReferencedReportItemNames(text3, expressionInfo);
			this.GetReferencedParameterNames(text3, expressionInfo);
			this.GetReferencedDataSetNames(text3, expressionInfo);
			this.GetReferencedDataSourceNames(text3, expressionInfo);
			expressionInfo.Type = ExpressionInfo.Types.Expression;
			expressionInfo.TransformedExpression = text3;
			if (this.m_context.ObjectType == ObjectType.Textbox && this.Detected(expressionInfo.TransformedExpression, this.m_regexes.MeDotValueDetection))
			{
				base.SetValueReferenced();
			}
		}

		private void EnforceRestrictions(ref string expression, bool isParameter, GrammarFlags grammarFlags)
		{
			if ((grammarFlags & GrammarFlags.DenyFields) != 0 && this.Detected(expression, this.m_regexes.FieldDetection))
			{
				if ((this.m_context.Location & LocationFlags.InPageSection) != 0)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsFieldInPageSectionExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.QueryParameter == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsFieldInQueryParameterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, this.m_context.DataSetName);
				}
				else if (ExpressionType.ReportLanguage == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsFieldInReportLanguageExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else
				{
					Global.Tracer.Assert(ExpressionType.ReportParameter == this.m_context.ExpressionType);
					base.m_errorContext.Register(ProcessingErrorCode.rsFieldInReportParameterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
			}
			int num = this.NumberOfTimesDetected(expression, this.m_regexes.ReportItemsDetection);
			if ((grammarFlags & GrammarFlags.DenyReportItems) != 0 && 0 < num)
			{
				if (isParameter)
				{
					Global.Tracer.Assert((LocationFlags)0 == (this.m_context.Location & LocationFlags.InPageSection));
					base.m_errorContext.Register(ProcessingErrorCode.rsAggregateReportItemInBody, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.DataSetFilters == this.m_context.ExpressionType || ExpressionType.DataRegionFilters == this.m_context.ExpressionType || ExpressionType.GroupingFilters == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsReportItemInFilterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.GroupExpression == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsReportItemInGroupExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.QueryParameter == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsReportItemInQueryParameterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, this.m_context.DataSetName);
				}
				else if (ExpressionType.ReportParameter == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsReportItemInReportParameterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.ReportLanguage == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsReportItemInReportLanguageExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else
				{
					Global.Tracer.Assert(ExpressionType.SortExpression == this.m_context.ExpressionType);
					base.m_errorContext.Register(ProcessingErrorCode.rsReportItemInSortExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
			}
			if ((this.m_context.Location & LocationFlags.InPageSection) != 0 && 1 < num)
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsMultiReportItemsInPageSectionExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
			}
			if (0 < num)
			{
				if ((this.m_context.Location & LocationFlags.InPageSection) != 0)
				{
					this.m_pageSectionRefersToReportItems = true;
				}
				else
				{
					this.m_bodyRefersToReportItems = true;
				}
			}
			if ((grammarFlags & GrammarFlags.DenyPageGlobals) != 0 && this.Detected(expression, this.m_regexes.PageGlobalsDetection))
			{
				Global.Tracer.Assert((LocationFlags)0 == (this.m_context.Location & LocationFlags.InPageSection));
				base.m_errorContext.Register(ProcessingErrorCode.rsPageNumberInBody, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
			}
			if (this.Detected(expression, this.m_regexes.AggregatesDetection))
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsGlobalNotDefined, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
			}
			if ((grammarFlags & GrammarFlags.DenyDataSets) != 0 && this.Detected(expression, this.m_regexes.DataSetsDetection))
			{
				if ((this.m_context.Location & LocationFlags.InPageSection) != 0)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsDataSetInPageSectionExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.QueryParameter == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsDataSetInQueryParameterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, this.m_context.DataSetName);
				}
				else if (ExpressionType.ReportLanguage == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsDataSetInReportLanguageExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else
				{
					Global.Tracer.Assert(ExpressionType.ReportParameter == this.m_context.ExpressionType);
					base.m_errorContext.Register(ProcessingErrorCode.rsDataSetInReportParameterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
			}
			if ((grammarFlags & GrammarFlags.DenyDataSources) != 0 && this.Detected(expression, this.m_regexes.DataSourcesDetection))
			{
				if ((this.m_context.Location & LocationFlags.InPageSection) != 0)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsDataSourceInPageSectionExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.QueryParameter == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsDataSourceInQueryParameterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, this.m_context.DataSetName);
				}
				else if (ExpressionType.ReportLanguage == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsDataSourceInReportLanguageExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else
				{
					Global.Tracer.Assert(ExpressionType.ReportParameter == this.m_context.ExpressionType);
					base.m_errorContext.Register(ProcessingErrorCode.rsDataSourceInReportParameterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
			}
			this.RemoveLineTerminators(ref expression, this.m_regexes.LineTerminatorDetection);
			if (this.Detected(expression, this.m_regexes.IllegalCharacterDetection))
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsInvalidCharacterInExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
			}
		}

		private void GetReferencedReportItemNames(string expression, ExpressionInfo expressionInfo)
		{
			MatchCollection matchCollection = this.m_regexes.ReportItemName.Matches(expression);
			for (int i = 0; i < matchCollection.Count; i++)
			{
				string text = matchCollection[i].Result("${reportitemname}");
				if (text != null && text.Length != 0)
				{
					expressionInfo.AddReferencedReportItem(text);
				}
			}
		}

		private void GetReferencedFieldNames(string expression, ExpressionInfo expressionInfo)
		{
			if (this.Detected(expression, this.m_regexes.DynamicFieldReference))
			{
				expressionInfo.DynamicFieldReferences = true;
			}
			else
			{
				MatchCollection matchCollection = this.m_regexes.DynamicFieldPropertyReference.Matches(expression);
				for (int i = 0; i < matchCollection.Count; i++)
				{
					string text = matchCollection[i].Result("${fieldname}");
					if (text != null && text.Length != 0)
					{
						expressionInfo.AddDynamicPropertyReference(text);
					}
				}
				matchCollection = this.m_regexes.StaticFieldPropertyReference.Matches(expression);
				for (int j = 0; j < matchCollection.Count; j++)
				{
					string text2 = matchCollection[j].Result("${fieldname}");
					string text3 = matchCollection[j].Result("${propertyname}");
					if (text2 != null && text2.Length != 0 && text3 != null && text3.Length != 0)
					{
						expressionInfo.AddStaticPropertyReference(text2, text3);
					}
				}
			}
			MatchCollection matchCollection2 = this.m_regexes.FieldName.Matches(expression);
			for (int k = 0; k < matchCollection2.Count; k++)
			{
				string text4 = matchCollection2[k].Result("${fieldname}");
				if (text4 != null && text4.Length != 0)
				{
					expressionInfo.AddReferencedField(text4);
				}
			}
		}

		private void GetReferencedParameterNames(string expression, ExpressionInfo expressionInfo)
		{
			MatchCollection matchCollection = this.m_regexes.ParameterName.Matches(expression);
			for (int i = 0; i < matchCollection.Count; i++)
			{
				string text = matchCollection[i].Result("${parametername}");
				if (text != null && text.Length != 0)
				{
					expressionInfo.AddReferencedParameter(text);
				}
			}
		}

		private void GetReferencedDataSetNames(string expression, ExpressionInfo expressionInfo)
		{
			MatchCollection matchCollection = this.m_regexes.DataSetName.Matches(expression);
			for (int i = 0; i < matchCollection.Count; i++)
			{
				string text = matchCollection[i].Result("${datasetname}");
				if (text != null && text.Length != 0)
				{
					expressionInfo.AddReferencedDataSet(text);
				}
			}
		}

		private void GetReferencedDataSourceNames(string expression, ExpressionInfo expressionInfo)
		{
			MatchCollection matchCollection = this.m_regexes.DataSourceName.Matches(expression);
			for (int i = 0; i < matchCollection.Count; i++)
			{
				string text = matchCollection[i].Result("${datasourcename}");
				if (text != null && text.Length != 0)
				{
					expressionInfo.AddReferencedDataSource(text);
				}
			}
		}

		private bool Detected(string expression, Regex detectionRegex)
		{
			return this.NumberOfTimesDetected(expression, detectionRegex) != 0;
		}

		private int NumberOfTimesDetected(string expression, Regex detectionRegex)
		{
			int num = 0;
			MatchCollection matchCollection = detectionRegex.Matches(expression);
			for (int i = 0; i < matchCollection.Count; i++)
			{
				string text = matchCollection[i].Result("${detected}");
				if (text != null && text.Length != 0)
				{
					num++;
				}
			}
			return num;
		}

		private void RemoveLineTerminators(ref string expression, Regex detectionRegex)
		{
			if (expression != null)
			{
				StringBuilder stringBuilder = new StringBuilder(expression, expression.Length);
				MatchCollection matchCollection = detectionRegex.Matches(expression);
				for (int num = matchCollection.Count - 1; num >= 0; num--)
				{
					string text = matchCollection[num].Result("${detected}");
					if (text != null && text.Length != 0)
					{
						stringBuilder.Remove(matchCollection[num].Index, matchCollection[num].Length);
					}
				}
				if (matchCollection.Count != 0)
				{
					expression = stringBuilder.ToString();
				}
			}
		}

		private void GetRunningValue(int currentPos, string functionName, string expression, bool isParameter, GrammarFlags grammarFlags, out int newPos, out RunningValueInfo runningValue)
		{
			if ((grammarFlags & GrammarFlags.DenyRunningValue) != 0)
			{
				if (isParameter)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsAggregateofAggregate, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.DataSetFilters == this.m_context.ExpressionType || ExpressionType.DataRegionFilters == this.m_context.ExpressionType || ExpressionType.GroupingFilters == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsRunningValueInFilterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.GroupExpression == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsRunningValueInGroupExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if ((this.m_context.Location & LocationFlags.InPageSection) != 0)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsRunningValueInPageSectionExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.QueryParameter == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsRunningValueInQueryParameterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, this.m_context.DataSetName);
				}
				else if (ExpressionType.ReportParameter == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsRunningValueInReportParameterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.ReportLanguage == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsRunningValueInReportLanguageExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else
				{
					Global.Tracer.Assert(ExpressionType.SortExpression == this.m_context.ExpressionType);
					base.m_errorContext.Register(ProcessingErrorCode.rsRunningValueInSortExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
			}
			List<string> list = default(List<string>);
			this.GetArguments(currentPos, expression, out newPos, out list);
			if (3 != list.Count)
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsWrongNumberOfParameters, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, functionName);
			}
			runningValue = new RunningValueInfo();
			if (2 <= list.Count)
			{
				bool flag;
				try
				{
					runningValue.AggregateType = (DataAggregateInfo.AggregateTypes)Enum.Parse(typeof(DataAggregateInfo.AggregateTypes), list[1], true);
					flag = (DataAggregateInfo.AggregateTypes.Aggregate != runningValue.AggregateType && DataAggregateInfo.AggregateTypes.Previous != runningValue.AggregateType && DataAggregateInfo.AggregateTypes.CountRows != runningValue.AggregateType);
				}
				catch (ArgumentException)
				{
					flag = false;
				}
				if (!flag)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsInvalidRunningValueAggregate, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
			}
			if (1 <= list.Count)
			{
				if (DataAggregateInfo.AggregateTypes.Count == runningValue.AggregateType && "*" == list[0].Trim())
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsCountStarRVNotSupported, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else
				{
					runningValue.Expressions = new ExpressionInfo[1];
					runningValue.Expressions[0] = this.GetParameterExpression(list[0], grammarFlags);
				}
			}
			if (3 <= list.Count)
			{
				runningValue.Scope = this.GetScope(list[2], true);
			}
			this.m_numberOfRunningValues++;
		}

		private void GetPreviousAggregate(int currentPos, string functionName, string expression, bool isParameter, GrammarFlags grammarFlags, out int newPos, out RunningValueInfo runningValue)
		{
			if ((grammarFlags & GrammarFlags.DenyPrevious) != 0)
			{
				if (isParameter)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsAggregateofAggregate, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.DataSetFilters == this.m_context.ExpressionType || ExpressionType.DataRegionFilters == this.m_context.ExpressionType || ExpressionType.GroupingFilters == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsPreviousAggregateInFilterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.GroupExpression == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsPreviousAggregateInGroupExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if ((this.m_context.Location & LocationFlags.InPageSection) != 0)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsPreviousAggregateInPageSectionExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.QueryParameter == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsPreviousAggregateInQueryParameterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, this.m_context.DataSetName);
				}
				else if (ExpressionType.ReportParameter == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsPreviousAggregateInReportParameterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.ReportLanguage == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsPreviousAggregateInReportLanguageExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else
				{
					Global.Tracer.Assert(ExpressionType.SortExpression == this.m_context.ExpressionType);
					base.m_errorContext.Register(ProcessingErrorCode.rsPreviousAggregateInSortExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
			}
			List<string> list = default(List<string>);
			this.GetArguments(currentPos, expression, out newPos, out list);
			if (1 != list.Count)
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsWrongNumberOfParameters, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, functionName);
			}
			runningValue = new RunningValueInfo();
			runningValue.AggregateType = DataAggregateInfo.AggregateTypes.Previous;
			if (1 <= list.Count)
			{
				runningValue.Expressions = new ExpressionInfo[1];
				runningValue.Expressions[0] = this.GetParameterExpression(list[0], grammarFlags);
			}
			this.m_numberOfRunningValues++;
		}

		private void GetRowNumber(int currentPos, string functionName, string expression, bool isParameter, GrammarFlags grammarFlags, out int newPos, out RunningValueInfo rowNumber)
		{
			if ((grammarFlags & GrammarFlags.DenyRowNumber) != 0)
			{
				if (isParameter)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsAggregateofAggregate, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.DataSetFilters == this.m_context.ExpressionType || ExpressionType.DataRegionFilters == this.m_context.ExpressionType || ExpressionType.GroupingFilters == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsRowNumberInFilterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if ((this.m_context.Location & LocationFlags.InPageSection) != 0)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsRowNumberInPageSectionExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.QueryParameter == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsRowNumberInQueryParameterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, this.m_context.DataSetName);
				}
				else if (ExpressionType.ReportParameter == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsRowNumberInReportParameterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.ReportLanguage == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsRowNumberInReportLanguageExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else
				{
					Global.Tracer.Assert(ExpressionType.SortExpression == this.m_context.ExpressionType);
					base.m_errorContext.Register(ProcessingErrorCode.rsRowNumberInSortExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
			}
			List<string> list = default(List<string>);
			this.GetArguments(currentPos, expression, out newPos, out list);
			if (1 != list.Count)
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsWrongNumberOfParameters, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, functionName);
			}
			rowNumber = new RunningValueInfo();
			rowNumber.AggregateType = DataAggregateInfo.AggregateTypes.CountRows;
			rowNumber.Expressions = new ExpressionInfo[0];
			if (1 <= list.Count)
			{
				rowNumber.Scope = this.GetScope(list[0], true);
			}
			this.m_numberOfRunningValues++;
		}

		private string GetScope(string expression, bool allowNothing)
		{
			Match match = this.m_regexes.NothingOnly.Match(expression);
			if (match.Success)
			{
				if (allowNothing)
				{
					return null;
				}
			}
			else
			{
				Match match2 = this.m_regexes.StringLiteralOnly.Match(expression);
				if (match2.Success)
				{
					return match2.Result("${string}");
				}
			}
			base.m_errorContext.Register(ProcessingErrorCode.rsInvalidAggregateScope, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
			return null;
		}

		private bool IsRecursive(string flag)
		{
			RecursiveFlags recursiveFlags = RecursiveFlags.Simple;
			try
			{
				recursiveFlags = (RecursiveFlags)Enum.Parse(typeof(RecursiveFlags), flag, true);
			}
			catch
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsInvalidAggregateRecursiveFlag, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
			}
			if (RecursiveFlags.Recursive == recursiveFlags)
			{
				return true;
			}
			return false;
		}

		private void GetAggregate(int currentPos, string functionName, string expression, bool isParameter, GrammarFlags grammarFlags, out int newPos, out DataAggregateInfo aggregate)
		{
			if ((grammarFlags & GrammarFlags.DenyAggregates) != 0)
			{
				if (isParameter)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsAggregateofAggregate, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.DataSetFilters == this.m_context.ExpressionType || ExpressionType.DataRegionFilters == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsAggregateInFilterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.GroupExpression == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsAggregateInGroupExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.QueryParameter == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsAggregateInQueryParameterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, this.m_context.DataSetName);
				}
				else if (ExpressionType.ReportLanguage == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsAggregateInReportLanguageExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.FieldValue == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsAggregateInCalculatedFieldExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else
				{
					Global.Tracer.Assert(ExpressionType.ReportParameter == this.m_context.ExpressionType);
					base.m_errorContext.Register(ProcessingErrorCode.rsAggregateInReportParameterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
			}
			List<string> list = default(List<string>);
			this.GetArguments(currentPos, expression, out newPos, out list);
			if (list.Count != 0 && 1 != list.Count && 2 != list.Count && 3 != list.Count)
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsWrongNumberOfParameters, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, functionName);
			}
			aggregate = new DataAggregateInfo();
			aggregate.AggregateType = (DataAggregateInfo.AggregateTypes)Enum.Parse(typeof(DataAggregateInfo.AggregateTypes), functionName, true);
			if ((grammarFlags & GrammarFlags.DenyPostSortAggregate) != 0 && aggregate.IsPostSortAggregate())
			{
				if (ExpressionType.GroupingFilters == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsPostSortAggregateInGroupFilterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.SortExpression == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsPostSortAggregateInSortExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
			}
			if (DataAggregateInfo.AggregateTypes.CountRows == aggregate.AggregateType)
			{
				aggregate.AggregateType = DataAggregateInfo.AggregateTypes.CountRows;
				aggregate.Expressions = new ExpressionInfo[0];
				if (1 == list.Count)
				{
					aggregate.SetScope(this.GetScope(list[0], false));
				}
				else if (2 == list.Count)
				{
					aggregate.Recursive = this.IsRecursive(list[1]);
				}
				else if (list.Count != 0)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsWrongNumberOfParameters, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, functionName);
				}
				if (DataAggregateInfo.AggregateTypes.CountRows == aggregate.AggregateType && (this.m_context.Location & LocationFlags.InPageSection) != 0)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsCountRowsInPageSectionExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
			}
			else
			{
				if (list.Count == 0)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsWrongNumberOfParameters, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, functionName);
				}
				else if (1 <= list.Count)
				{
					if (DataAggregateInfo.AggregateTypes.Count == aggregate.AggregateType && "*" == list[0].Trim())
					{
						base.m_errorContext.Register(ProcessingErrorCode.rsCountStarNotSupported, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
					}
					else
					{
						aggregate.Expressions = new ExpressionInfo[1];
						aggregate.Expressions[0] = this.GetParameterExpression(list[0], grammarFlags);
						if (DataAggregateInfo.AggregateTypes.Aggregate == aggregate.AggregateType && ExpressionInfo.Types.Field != aggregate.Expressions[0].Type)
						{
							base.m_errorContext.Register(ProcessingErrorCode.rsInvalidCustomAggregateExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
						}
					}
				}
				if (2 <= list.Count)
				{
					aggregate.SetScope(this.GetScope(list[1], false));
				}
				if (3 <= list.Count)
				{
					if (aggregate.IsPostSortAggregate() || DataAggregateInfo.AggregateTypes.Aggregate == aggregate.AggregateType)
					{
						base.m_errorContext.Register(ProcessingErrorCode.rsInvalidRecursiveAggregate, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
					}
					else
					{
						aggregate.Recursive = this.IsRecursive(list[2]);
					}
				}
			}
			this.m_numberOfAggregates++;
		}

		private ExpressionInfo GetParameterExpression(string parameterExpression, GrammarFlags grammarFlags)
		{
			ExpressionInfo expressionInfo = new ExpressionInfo();
			expressionInfo.OriginalText = parameterExpression;
			grammarFlags = (((this.m_context.Location & LocationFlags.InPageSection) == (LocationFlags)0) ? (grammarFlags | (GrammarFlags.DenyAggregates | GrammarFlags.DenyRunningValue | GrammarFlags.DenyRowNumber | GrammarFlags.DenyReportItems | GrammarFlags.DenyPrevious)) : (grammarFlags | (GrammarFlags.DenyAggregates | GrammarFlags.DenyRunningValue | GrammarFlags.DenyRowNumber | GrammarFlags.DenyPrevious)));
			this.VBLex(parameterExpression, true, grammarFlags, expressionInfo);
			return expressionInfo;
		}

		private void GetArguments(int currentPos, string expression, out int newPos, out List<string> arguments)
		{
			int num = 1;
			arguments = new List<string>();
			string text = string.Empty;
			while (0 < num && currentPos < expression.Length)
			{
				Match match = this.m_regexes.Arguments.Match(expression, currentPos);
				if (!match.Success)
				{
					text += expression.Substring(currentPos);
					currentPos = expression.Length;
				}
				else
				{
					string text2 = match.Result("${openParen}");
					string text3 = match.Result("${closeParen}");
					string text4 = match.Result("${comma}");
					if (text2 != null && text2.Length != 0)
					{
						num++;
						text += expression.Substring(currentPos, match.Index - currentPos + match.Length);
					}
					else if (text3 != null && text3.Length != 0)
					{
						num--;
						if (num == 0)
						{
							text += expression.Substring(currentPos, match.Index - currentPos);
							if (text.Trim().Length != 0)
							{
								arguments.Add(text);
								text = string.Empty;
							}
						}
						else
						{
							text += expression.Substring(currentPos, match.Index - currentPos + match.Length);
						}
					}
					else if (text4 != null && text4.Length != 0)
					{
						if (1 == num)
						{
							text += expression.Substring(currentPos, match.Index - currentPos);
							arguments.Add(text);
							text = string.Empty;
						}
						else
						{
							text += expression.Substring(currentPos, match.Index - currentPos + match.Length);
						}
					}
					else
					{
						text += expression.Substring(currentPos, match.Index - currentPos + match.Length);
					}
					currentPos = match.Index + match.Length;
				}
			}
			if (num > 0)
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsExpressionMissingCloseParen, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				if (text.Trim().Length != 0)
				{
					arguments.Add(text);
					text = string.Empty;
				}
			}
			newPos = currentPos;
		}

		private string CreateAggregateID()
		{
			return "Aggregate" + (this.m_numberOfAggregates + this.m_numberOfRunningValues);
		}
	}
}
