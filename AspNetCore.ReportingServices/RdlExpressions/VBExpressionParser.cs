using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace AspNetCore.ReportingServices.RdlExpressions
{
	internal sealed class VBExpressionParser : ExpressionParser
	{
		private sealed class ParserState
		{
			internal bool BodyRefersToReportItems
			{
				get;
				set;
			}

			internal bool PageSectionRefersToReportItems
			{
				get;
				set;
			}

			internal bool PageSectionRefersToOverallTotalPages
			{
				get;
				set;
			}

			internal bool PageSectionRefersToTotalPages
			{
				get;
				set;
			}

			internal int NumberOfAggregates
			{
				get;
				set;
			}

			internal int NumberOfRunningValues
			{
				get;
				set;
			}

			internal int NumberOfLookups
			{
				get;
				set;
			}

			internal int LastID
			{
				get
				{
					return this.NumberOfAggregates + this.NumberOfRunningValues;
				}
			}

			internal int LastLookupID
			{
				get
				{
					return this.NumberOfLookups;
				}
			}

			internal bool PreviousAggregateUsed
			{
				get;
				set;
			}

			internal bool AggregateOfAggregatesUsed
			{
				get;
				set;
			}

			internal bool AggregateOfAggregatesUsedInUserSort
			{
				get;
				set;
			}

			internal ParserState Save()
			{
				return (ParserState)base.MemberwiseClone();
			}
		}

		private sealed class ReportRegularExpressions
		{
			internal Regex NonConstant;

			internal Regex FieldDetection;

			internal Regex ReportItemsDetection;

			internal Regex ParametersDetection;

			internal Regex RenderFormatAnyDetection;

			internal Regex OverallPageGlobalsDetection;

			internal Regex PageGlobalsDetection;

			internal Regex OverallTotalPagesDetection;

			internal Regex TotalPagesDetection;

			internal Regex AggregatesDetection;

			internal Regex UserDetection;

			internal Regex DataSetsDetection;

			internal Regex DataSourcesDetection;

			internal Regex VariablesDetection;

			internal Regex MeDotValueExpression;

			internal Regex MeDotValueDetection;

			internal Regex IllegalCharacterDetection;

			internal Regex LineTerminatorDetection;

			internal Regex FieldOnly;

			internal Regex ParameterOnly;

			internal Regex StringLiteralOnly;

			internal Regex NothingOnly;

			internal Regex InScopeOrLevel;

			internal Regex InScope;

			internal Regex Level;

			internal Regex CreateDrillthroughContext;

			internal Regex ReportItemName;

			internal Regex FieldName;

			internal Regex ParameterName;

			internal Regex DynamicParameterReference;

			internal Regex DataSetName;

			internal Regex DataSourceName;

			internal Regex SpecialFunction;

			internal Regex Arguments;

			internal Regex DynamicFieldReference;

			internal Regex DynamicFieldPropertyReference;

			internal Regex StaticFieldPropertyReference;

			internal Regex RewrittenCommandText;

			internal Regex SimpleDynamicFieldReference;

			internal Regex SimpleDynamicReportItemReference;

			internal Regex SimpleDynamicVariableReference;

			internal Regex VariableName;

			internal Regex RenderFormatPropertyName;

			internal Regex HasLevelWithNoScope;

			internal Regex RdlFunction;

			internal Regex ScopedFieldReferenceOnly;

			internal Regex ScopesDetection;

			internal Regex SimpleDynamicScopeReference;

			internal Regex ScopeName;

			internal Regex DictionaryOpWithIdentifier;

			internal Regex IndexerWithIdentifier;

			internal static readonly ReportRegularExpressions Value = new ReportRegularExpressions();

			private ReportRegularExpressions()
			{
				RegexOptions options = RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline;
				this.NonConstant = new Regex("^\\s*=", options);
				string text = Regex.Escape("-+()#,:&*/\\^<=>");
				string text2 = Regex.Escape("!");
				string text3 = Regex.Escape(".");
				string text4 = "[" + text2 + text3 + "]";
				string text5 = "[" + text + "\\s]";
				string text6 = "(^|" + text5 + ")";
				string text7 = "($|[" + text + text2 + text3 + "\\s])";
				string text8 = "($|[" + text + text3 + "\\s])";
				string text9 = ReportRegularExpressions.CaseInsensitive("Fields");
				string text10 = ReportRegularExpressions.CaseInsensitive("Value");
				string text11 = ReportRegularExpressions.CaseInsensitive("Scopes");
				string text12 = ReportRegularExpressions.CaseInsensitive("ReportItems");
				string text13 = ReportRegularExpressions.CaseInsensitive("Parameters");
				string text14 = ReportRegularExpressions.CaseInsensitive("Globals");
				string text15 = ReportRegularExpressions.CaseInsensitive("RenderFormat");
				string text16 = ReportRegularExpressions.CaseInsensitive("OverallTotalPages");
				string text17 = ReportRegularExpressions.CaseInsensitive("TotalPages");
				string text18 = ReportRegularExpressions.CaseInsensitive("DataSets");
				string text19 = ReportRegularExpressions.CaseInsensitive("DataSources");
				string text20 = ReportRegularExpressions.CaseInsensitive("Variables");
				string text21 = ReportRegularExpressions.CaseInsensitive("Me");
				string text22 = ReportRegularExpressions.CaseInsensitive("Item");
				string text23 = ReportRegularExpressions.CaseInsensitive("InScope");
				string text24 = ReportRegularExpressions.CaseInsensitive("Level");
				this.FieldDetection = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>" + text9 + ")" + text7, options);
				this.ScopesDetection = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>" + text11 + ")" + text7, options);
				this.ReportItemsDetection = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>" + text12 + ")" + text7, options);
				this.ParametersDetection = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>" + text13 + ")" + text7, options);
				this.RenderFormatAnyDetection = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>(" + text14 + text4 + text15 + text7 + "))", options);
				this.OverallPageGlobalsDetection = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>(" + text14 + text4 + ReportRegularExpressions.CaseInsensitive("OverallPageNumber") + ")|(" + text14 + text4 + text16 + "))" + text7, options);
				this.PageGlobalsDetection = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>(" + text14 + text4 + ReportRegularExpressions.CaseInsensitive("PageNumber") + ")|(" + text14 + text4 + text17 + ")|(" + text14 + text4 + ReportRegularExpressions.CaseInsensitive("PageName") + "))" + text7, options);
				this.OverallTotalPagesDetection = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>(" + text14 + text4 + text16 + text7 + ")|(" + text14 + text5 + "))", options);
				this.TotalPagesDetection = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>(" + text14 + text4 + text17 + text7 + ")|(" + text14 + text5 + "))", options);
				this.AggregatesDetection = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>" + ReportRegularExpressions.CaseInsensitive("Aggregates") + ")" + text7, options);
				this.UserDetection = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>" + ReportRegularExpressions.CaseInsensitive("User") + ")" + text7, options);
				this.DataSetsDetection = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>" + text18 + ")" + text7, options);
				this.DataSourcesDetection = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>" + text19 + ")" + text7, options);
				this.VariablesDetection = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>" + text20 + ")" + text7, options);
				this.MeDotValueDetection = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>(" + text21 + text3 + ")?" + text10 + ")" + text7, options);
				this.MeDotValueExpression = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<medotvalue>(" + text21 + text3 + ")?" + text10 + ")*" + text7, options);
				string text25 = Regex.Escape(":");
				string text26 = Regex.Escape("#");
				string text27 = "(" + text26 + "[^" + text26 + "]*" + text26 + ")";
				string text28 = Regex.Escape(":=");
				this.LineTerminatorDetection = new Regex("(?<detected>(\\u000D\\u000A)|([\\u000D\\u000A\\u2028\\u2029]))", options);
				this.IllegalCharacterDetection = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text27 + "|" + text28 + "|(?<detected>" + text25 + ")", options);
				string text29 = "[\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Pc}][\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Pc}\\p{Nd}\\p{Mn}\\p{Mc}\\p{Cf}]*";
				string str = text12 + text2 + "(?<reportitemname>" + text29 + ")";
				string text30 = text9 + text2 + "(?<fieldname>" + text29 + ")";
				string text31 = text13 + text2 + "(?<parametername>" + text29 + ")";
				string text32 = text18 + text2 + "(?<datasetname>" + text29 + ")";
				string str2 = text19 + text2 + "(?<datasourcename>" + text29 + ")";
				string str3 = text20 + text2 + "(?<variablename>" + text29 + ")";
				string text33 = text11 + text2 + "(?<scopename>" + text29 + ")";
				string str4 = text14 + text4 + text15 + text4 + "(?<propertyname>" + text29 + ")";
				this.SimpleDynamicReportItemReference = new Regex(text6 + "(?<detected>(" + text12 + "(" + text3 + text22 + ")?" + Regex.Escape("(") + "[ \t]*" + Regex.Escape("\"") + "(?<reportitemname>" + text29 + ")" + Regex.Escape("\"") + "[ \t]*" + Regex.Escape(")") + "))", options);
				this.SimpleDynamicVariableReference = new Regex(text6 + "(?<detected>(" + text20 + "(" + text3 + text22 + ")?" + Regex.Escape("(") + "[ \t]*" + Regex.Escape("\"") + "(?<variablename>" + text29 + ")" + Regex.Escape("\"") + "[ \t]*" + Regex.Escape(")") + "))", options);
				this.SimpleDynamicFieldReference = new Regex(text6 + "(?<detected>(" + text9 + "(" + text3 + text22 + ")?" + Regex.Escape("(") + "[ \t]*" + Regex.Escape("\"") + "(?<fieldname>" + text29 + ")" + Regex.Escape("\"") + "[ \t]*" + Regex.Escape(")") + "))", options);
				this.DynamicFieldReference = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>(" + text9 + "(" + text3 + text22 + ")?" + Regex.Escape("(") + "))", options);
				this.DynamicFieldPropertyReference = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + text30 + Regex.Escape("("), options);
				this.StaticFieldPropertyReference = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + text30 + text3 + "(?<propertyname>" + text29 + ")", options);
				this.FieldOnly = new Regex("^\\s*" + text30 + text3 + text10 + "\\s*$", options);
				string text34 = "(?<hasfields>" + text3 + text9 + ")?";
				this.SimpleDynamicScopeReference = new Regex(text6 + "(?<detected>(" + text11 + "(" + text3 + text22 + ")?" + Regex.Escape("(") + "[ \t]*" + Regex.Escape("\"") + "(?<scopename>" + text29 + ")" + Regex.Escape("\"") + "[ \t]*" + Regex.Escape(")") + ")" + text34 + ")", options);
				this.ScopeName = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + text33 + text34, options);
				string text35 = "(?<fieldname>" + text29 + ")";
				this.DictionaryOpWithIdentifier = new Regex("\\G" + text2 + text35, options);
				this.IndexerWithIdentifier = new Regex("\\G(" + text3 + text22 + ")?" + Regex.Escape("(") + "[ \t]*" + Regex.Escape("\"") + text35 + Regex.Escape("\"") + "[ \t]*" + Regex.Escape(")"), options);
				this.ScopedFieldReferenceOnly = new Regex("^\\s*" + text33 + text3 + text30 + text3 + text10 + "\\s*$", options);
				this.RewrittenCommandText = new Regex("^\\s*" + text32 + text3 + ReportRegularExpressions.CaseInsensitive("RewrittenCommandText") + "\\s*$", options);
				this.ParameterOnly = new Regex("^\\s*" + text31 + text3 + text10 + "\\s*$", options);
				this.StringLiteralOnly = new Regex("^\\s*\"(?<string>((\"\")|[^\"])*)\"\\s*$", options);
				this.NothingOnly = new Regex("^\\s*" + ReportRegularExpressions.CaseInsensitive("Nothing") + "\\s*$", options);
				this.InScopeOrLevel = new Regex("((\"((\"\")|[^\"])*\")|('.*)|" + text6 + ")*(" + text23 + "|" + text24 + ")\\s*\\(", options);
				string str5 = "(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>";
				string str6 = ")\\s*\\(";
				this.InScope = new Regex(str5 + text23 + str6, options);
				this.Level = new Regex(str5 + text24 + str6, options);
				this.CreateDrillthroughContext = new Regex(str5 + ReportRegularExpressions.CaseInsensitive("CreateDrillthroughContext") + str6, options);
				this.ReportItemName = new Regex(text6 + str, options);
				this.FieldName = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + text30, options);
				this.ParameterName = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + text31, options);
				this.DynamicParameterReference = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + "(?<detected>" + text13 + text8 + ")", options);
				this.DataSetName = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + text32, options);
				this.DataSourceName = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + str2, options);
				this.VariableName = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + str3, options);
				this.RenderFormatPropertyName = new Regex("(\"((\"\")|[^\"])*\")|('.*)|" + text6 + str4, options);
				this.SpecialFunction = new Regex("(\"((\"\")|[^\"])*\")|('.*)|(?<prefix>" + text6 + ")(?<sfname>" + ReportRegularExpressions.CaseInsensitive("RunningValue") + "|" + ReportRegularExpressions.CaseInsensitive("RowNumber") + "|" + ReportRegularExpressions.CaseInsensitive("Lookup") + "|" + ReportRegularExpressions.CaseInsensitive("LookupSet") + "|" + ReportRegularExpressions.CaseInsensitive("MultiLookup") + "|" + ReportRegularExpressions.CaseInsensitive("First") + "|" + ReportRegularExpressions.CaseInsensitive("Last") + "|" + ReportRegularExpressions.CaseInsensitive("Previous") + "|" + ReportRegularExpressions.CaseInsensitive("Sum") + "|" + ReportRegularExpressions.CaseInsensitive("Avg") + "|" + ReportRegularExpressions.CaseInsensitive("Max") + "|" + ReportRegularExpressions.CaseInsensitive("Min") + "|" + ReportRegularExpressions.CaseInsensitive("CountDistinct") + "|" + ReportRegularExpressions.CaseInsensitive("Count") + "|" + ReportRegularExpressions.CaseInsensitive("CountRows") + "|" + ReportRegularExpressions.CaseInsensitive("StDevP") + "|" + ReportRegularExpressions.CaseInsensitive("VarP") + "|" + ReportRegularExpressions.CaseInsensitive("StDev") + "|" + ReportRegularExpressions.CaseInsensitive("Var") + "|" + ReportRegularExpressions.CaseInsensitive("Aggregate") + "|" + ReportRegularExpressions.CaseInsensitive("Union") + ")\\s*\\(", options);
				this.RdlFunction = new Regex("^\\s*(?<functionName>" + ReportRegularExpressions.CaseInsensitive("MinValue") + "|" + ReportRegularExpressions.CaseInsensitive("MaxValue") + ")\\s*\\(", options);
				string text36 = Regex.Escape("(");
				string text37 = Regex.Escape(")");
				string text38 = Regex.Escape(",");
				string text39 = Regex.Escape("{");
				string text40 = Regex.Escape("}");
				this.Arguments = new Regex("(\"((\"\")|[^\"])*\")|('.*)|(?<openParen>" + text36 + ")|(?<closeParen>" + text37 + ")|(?<openCurly>" + text39 + ")|(?<closeCurly>" + text40 + ")|(?<comma>" + text38 + ")", options);
				this.HasLevelWithNoScope = new Regex(text24 + "\\s*" + text36 + "\\s*" + text37);
			}

			private static string CaseInsensitive(string input)
			{
				StringBuilder stringBuilder = new StringBuilder(input.Length * 4);
				foreach (char c in input)
				{
					stringBuilder.Append("[");
					stringBuilder.Append(char.ToUpperInvariant(c));
					stringBuilder.Append(char.ToLowerInvariant(c));
					stringBuilder.Append("]");
				}
				return stringBuilder.ToString();
			}
		}

		private const string RunningValue = "RunningValue";

		private const string RowNumber = "RowNumber";

		private const string Previous = "Previous";

		private const string Lookup = "Lookup";

		private const string LookupSet = "LookupSet";

		private const string MultiLookup = "MultiLookup";

		private const string Fields = "Fields";

		private const string ReportItems = "ReportItems";

		private const string Parameters = "Parameters";

		private const string Globals = "Globals";

		private const string User = "User";

		private const string Aggregates = "Aggregates";

		private const string DataSets = "DataSets";

		private const string DataSources = "DataSources";

		private const string Variables = "Variables";

		private const string MinValue = "MinValue";

		private const string MaxValue = "MaxValue";

		private const string Scopes = "Scopes";

		private const string Star = "*";

		private ReportRegularExpressions m_regexes;

		private ExpressionContext m_context;

		private ParserState m_state = new ParserState();

		internal override bool BodyRefersToReportItems
		{
			get
			{
				return this.m_state.BodyRefersToReportItems;
			}
		}

		internal override bool PageSectionRefersToReportItems
		{
			get
			{
				return this.m_state.PageSectionRefersToReportItems;
			}
		}

		internal override bool PageSectionRefersToOverallTotalPages
		{
			get
			{
				return this.m_state.PageSectionRefersToOverallTotalPages;
			}
		}

		internal override bool PageSectionRefersToTotalPages
		{
			get
			{
				return this.m_state.PageSectionRefersToTotalPages;
			}
		}

		internal override int NumberOfAggregates
		{
			get
			{
				return this.m_state.NumberOfAggregates;
			}
		}

		internal override int LastID
		{
			get
			{
				return this.m_state.LastID;
			}
		}

		internal override int LastLookupID
		{
			get
			{
				return this.m_state.LastLookupID;
			}
		}

		internal override bool PreviousAggregateUsed
		{
			get
			{
				return this.m_state.PreviousAggregateUsed;
			}
		}

		internal override bool AggregateOfAggregatesUsed
		{
			get
			{
				return this.m_state.AggregateOfAggregatesUsed;
			}
		}

		internal override bool AggregateOfAggregatesUsedInUserSort
		{
			get
			{
				return this.m_state.AggregateOfAggregatesUsedInUserSort;
			}
		}

		internal VBExpressionParser(ErrorContext errorContext)
			: base(errorContext)
		{
			this.m_regexes = ReportRegularExpressions.Value;
		}

		internal override CodeDomProvider GetCodeCompiler()
		{
			return new VBExpressionCodeProvider();
		}

		internal override string GetCompilerArguments()
		{
			return "/optimize+";
		}

		internal override AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo ParseExpression(string expression, ExpressionContext context, EvaluationMode evaluationMode)
		{
			Global.Tracer.Assert(null != expression, "(null != expression)");
			string text = default(string);
			return this.Lex(expression, context, evaluationMode, out text);
		}

		internal override AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo ParseExpression(string expression, ExpressionContext context, EvaluationMode evaluationMode, out bool userCollectionReferenced)
		{
			string expression2 = default(string);
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = this.Lex(expression, context, evaluationMode, out expression2);
			userCollectionReferenced = false;
			if (expressionInfo.Type == AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Expression)
			{
				userCollectionReferenced = this.DetectUserReference(expression2);
				expressionInfo.SimpleParameterName = this.GetSimpleParameterReferenceName(expression2);
			}
			return expressionInfo;
		}

		internal override void ConvertField2ComplexExpr(ref AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo info)
		{
			Global.Tracer.Assert(info.Type == AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Field, "(info.Type == ExpressionInfo.Types.Field)");
			info.Type = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Expression;
			info.TransformedExpression = "Fields!" + info.StringValue + ".Value";
		}

		internal override void ResetPageSectionRefersFlags()
		{
			this.m_state.PageSectionRefersToReportItems = false;
			this.m_state.PageSectionRefersToOverallTotalPages = false;
			this.m_state.PageSectionRefersToTotalPages = false;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo Lex(string expression, ExpressionContext context, EvaluationMode evaluationMode, out string vbExpression)
		{
			vbExpression = null;
			this.m_context = context;
			if (context.MaxExpressionLength != -1 && expression != null && expression.Length > context.MaxExpressionLength)
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsSandboxingExpressionExceedsMaximumLength, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, Convert.ToString(context.MaxExpressionLength, CultureInfo.InvariantCulture));
			}
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = new AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo();
			expressionInfo.OriginalText = expression;
			bool flag = evaluationMode == EvaluationMode.Constant;
			if (!flag)
			{
				Match match = this.m_regexes.NonConstant.Match(expression);
				if (match.Success)
				{
					vbExpression = expression.Substring(match.Length);
				}
				else
				{
					flag = true;
				}
			}
			if (flag)
			{
				ExpressionParser.ParseRDLConstant(expression, expressionInfo, context.ConstantType, base.m_errorContext, context.ObjectType, context.ObjectName, context.PropertyName);
			}
			else
			{
				GrammarFlags grammarFlags = (GrammarFlags)(((this.m_context.Location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InPageSection) == (AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0) ? (ExpressionParser.ExpressionType2Restrictions(this.m_context.ExpressionType) | Restrictions.InBody) : (ExpressionParser.ExpressionType2Restrictions(this.m_context.ExpressionType) | Restrictions.InPageSection));
				if (this.m_context.ObjectType == ObjectType.Paragraph)
				{
					grammarFlags |= GrammarFlags.DenyMeDotValue;
				}
				Match match2 = this.m_regexes.HasLevelWithNoScope.Match(expression);
				if (match2.Success)
				{
					expressionInfo.NullLevelDetected = true;
				}
				this.VBLex(vbExpression, false, grammarFlags, expressionInfo);
			}
			if (this.m_state.AggregateOfAggregatesUsed && context.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.AggregatesOfAggregates))
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlExpressionAggregatesOfAggregates, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
			}
			return expressionInfo;
		}

		private string GetSimpleParameterReferenceName(string expression)
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

		private void VBLex(string expression, bool isParameter, GrammarFlags grammarFlags, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo)
		{
			ParserState state = this.m_state.Save();
			if (!this.TryVBLex(expression, isParameter, grammarFlags, expressionInfo, true))
			{
				this.m_state = state;
				this.TryVBLex(expression, isParameter, grammarFlags, expressionInfo, false);
			}
			if (expressionInfo.Type == AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Expression && this.m_context.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.ComplexExpression))
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsInvalidComplexExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
			}
		}

		private bool TryVBLex(string expression, bool isParameter, GrammarFlags grammarFlags, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo, bool parseSpecialFunctions)
		{
			if ((grammarFlags & GrammarFlags.DenyFields) == (GrammarFlags)0)
			{
				Match match = this.m_regexes.FieldOnly.Match(expression);
				if (match.Success)
				{
					string asSimpleFieldReference = match.Result("${fieldname}");
					expressionInfo.SetAsSimpleFieldReference(asSimpleFieldReference);
					return true;
				}
			}
			if ((grammarFlags & GrammarFlags.DenyDataSets) == (GrammarFlags)0)
			{
				Match match2 = this.m_regexes.RewrittenCommandText.Match(expression);
				if (match2.Success)
				{
					string text = match2.Result("${datasetname}");
					expressionInfo.AddReferencedDataSet(text);
					expressionInfo.Type = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Token;
					expressionInfo.StringValue = text;
					return true;
				}
			}
			if ((grammarFlags & GrammarFlags.DenyFields) == (GrammarFlags)0 && (grammarFlags & GrammarFlags.DenyScopes) == (GrammarFlags)0 && !this.m_context.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.ScopesCollection))
			{
				Match match3 = this.m_regexes.ScopedFieldReferenceOnly.Match(expression);
				if (match3.Success)
				{
					string scopeName = match3.Result("${scopename}");
					string fieldName = match3.Result("${fieldname}");
					expressionInfo.SetAsScopedFieldReference(scopeName, fieldName);
					return true;
				}
			}
			this.EnforceRestrictions(ref expression, isParameter, grammarFlags, expressionInfo);
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			bool flag = false;
			while (num < expression.Length)
			{
				if (parseSpecialFunctions)
				{
					Match match4 = this.m_regexes.RdlFunction.Match(expression, num);
					if (match4.Success)
					{
						string text2 = match4.Result("${functionName}");
						if (string.IsNullOrEmpty(text2))
						{
							return false;
						}
						num = match4.Length;
						if (!this.ParseRdlFunction(expressionInfo, num, text2, expression, grammarFlags, out num) && expression.Substring(num).Trim().Length <= 0)
						{
							return true;
						}
						return false;
					}
					parseSpecialFunctions = false;
				}
				Match match5 = this.m_regexes.SpecialFunction.Match(expression, num);
				if (!match5.Success)
				{
					stringBuilder.Append(expression.Substring(num));
					num = expression.Length;
				}
				else
				{
					stringBuilder.Append(expression.Substring(num, match5.Index - num));
					string text3 = match5.Result("${sfname}");
					if (string.IsNullOrEmpty(text3))
					{
						stringBuilder.Append(match5.Value);
						num = match5.Index + match5.Length;
					}
					else
					{
						stringBuilder.Append(match5.Result("${prefix}"));
						num = match5.Index + match5.Length;
						bool isRunningValue = false;
						AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types types;
						string text4 = default(string);
						if (this.AreEqualOrdinalIgnoreCase(text3, "Lookup"))
						{
							types = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Lookup_OneValue;
							LookupInfo lookup = this.GetLookup(num, text3, expression, isParameter, grammarFlags, LookupType.Lookup, out num, out text4);
							expressionInfo.AddLookup(lookup);
						}
						else if (this.AreEqualOrdinalIgnoreCase(text3, "LookupSet"))
						{
							types = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Lookup_MultiValue;
							LookupInfo lookup2 = this.GetLookup(num, text3, expression, isParameter, grammarFlags, LookupType.LookupSet, out num, out text4);
							expressionInfo.AddLookup(lookup2);
						}
						else if (this.AreEqualOrdinalIgnoreCase(text3, "MultiLookup"))
						{
							types = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Lookup_MultiValue;
							LookupInfo lookup3 = this.GetLookup(num, text3, expression, isParameter, grammarFlags, LookupType.MultiLookup, out num, out text4);
							expressionInfo.AddLookup(lookup3);
						}
						else
						{
							text4 = this.CreateAggregateID();
							this.m_state.NumberOfAggregates++;
							types = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Aggregate;
							if (this.AreEqualOrdinalIgnoreCase(text3, "Previous"))
							{
								AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo runningValueInfo = default(AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo);
								this.GetPreviousAggregate(num, text3, expression, isParameter, grammarFlags, out num, out runningValueInfo);
								runningValueInfo.Name = text4;
								expressionInfo.AddRunningValue(runningValueInfo);
								isRunningValue = true;
							}
							else if (this.AreEqualOrdinalIgnoreCase(text3, "RunningValue"))
							{
								AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo runningValueInfo2 = default(AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo);
								this.GetRunningValue(num, text3, expression, isParameter, grammarFlags, out num, out runningValueInfo2);
								runningValueInfo2.Name = text4;
								expressionInfo.AddRunningValue(runningValueInfo2);
								isRunningValue = true;
							}
							else if (this.AreEqualOrdinalIgnoreCase(text3, "RowNumber"))
							{
								AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo runningValueInfo3 = default(AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo);
								this.GetRowNumber(num, text3, expression, isParameter, grammarFlags, out num, out runningValueInfo3);
								runningValueInfo3.Name = text4;
								expressionInfo.AddRunningValue(runningValueInfo3);
								isRunningValue = true;
							}
							else
							{
								AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo dataAggregateInfo = default(AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo);
								this.GetAggregate(num, text3, expression, isParameter, grammarFlags, out num, out dataAggregateInfo);
								dataAggregateInfo.Name = text4;
								expressionInfo.AddAggregate(dataAggregateInfo);
								isRunningValue = false;
							}
						}
						if (!flag)
						{
							flag = true;
							string text5 = stringBuilder.ToString();
							if (text5.Trim().Length == 0 && expression.Substring(num).Trim().Length == 0)
							{
								expressionInfo.Type = types;
								expressionInfo.StringValue = text4;
								return true;
							}
						}
						switch (types)
						{
						case AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Aggregate:
							expressionInfo.AddTransformedExpressionAggregateInfo(stringBuilder.Length, text4, isRunningValue);
							stringBuilder.Append("Aggregates!");
							stringBuilder.Append(text4);
							break;
						case AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Lookup_OneValue:
						case AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Lookup_MultiValue:
							expressionInfo.AddTransformedExpressionLookupInfo(stringBuilder.Length, text4);
							stringBuilder.Append("Lookups!");
							stringBuilder.Append(text4);
							if (types == AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Lookup_OneValue)
							{
								stringBuilder.Append(".Value");
							}
							else
							{
								stringBuilder.Append(".Values");
							}
							break;
						default:
							Global.Tracer.Assert(false, "Unexpected special function type {0}", types);
							break;
						}
					}
				}
			}
			string text6 = stringBuilder.ToString();
			this.GetReferencedFieldNames(text6, expressionInfo);
			this.GetReferencedParameterNames(text6, expressionInfo);
			this.GetReferencedDataSetNames(text6, expressionInfo);
			this.GetReferencedDataSourceNames(text6, expressionInfo);
			this.GetReferencedVariableNames(text6, expressionInfo);
			this.GetReferencedScopesAndScopedFields(text6, expressionInfo);
			this.GetReferencedReportItemNames(text6, expressionInfo);
			this.GetReferencedReportItemNames(expressionInfo);
			this.GetMeDotValueReferences(text6, expressionInfo);
			this.GetMeDotValueReferences(expressionInfo);
			expressionInfo.Type = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Expression;
			expressionInfo.TransformedExpression = text6;
			if ((this.m_context.ObjectType == ObjectType.Textbox || this.m_context.ObjectType == ObjectType.TextRun) && expressionInfo.MeDotValueDetected)
			{
				base.SetValueReferenced();
			}
			if (expressionInfo.Type == AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Expression && this.Detected(text6, this.m_regexes.ScopesDetection))
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsScopeReferenceInComplexExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
			}
			return true;
		}

		internal override AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo CreateScopedFirstAggregate(string fieldName, string scopeName)
		{
			string text = this.CreateAggregateID();
			this.m_state.NumberOfAggregates++;
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = new AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo();
			expressionInfo.SetAsSimpleFieldReference(fieldName);
			expressionInfo.OriginalText = fieldName;
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo dataAggregateInfo = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo();
			dataAggregateInfo.AggregateType = AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.First;
			dataAggregateInfo.Name = text;
			dataAggregateInfo.Expressions = new AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo[1]
			{
				expressionInfo
			};
			dataAggregateInfo.SetScope(scopeName);
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo2 = new AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo();
			expressionInfo2.AddAggregate(dataAggregateInfo);
			expressionInfo2.HasAnyFieldReferences = true;
			expressionInfo2.Type = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Aggregate;
			expressionInfo2.StringValue = text;
			return expressionInfo2;
		}

		private bool AreEqualOrdinalIgnoreCase(string str1, string str2)
		{
			return string.Equals(str1, str2, StringComparison.OrdinalIgnoreCase);
		}

		private void EnforceRestrictions(ref string expression, bool isParameter, GrammarFlags grammarFlags, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo)
		{
			if ((grammarFlags & GrammarFlags.DenyRenderFormatAll) != 0 && this.Detected(expression, this.m_regexes.RenderFormatAnyDetection))
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsInvalidRenderFormatUsage, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
			}
			if (this.m_context.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.InScope) && this.Detected(expression, this.m_regexes.InScope))
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, "InScope");
			}
			if (this.m_context.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.Level) && this.Detected(expression, this.m_regexes.Level))
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, "Level");
			}
			if (this.m_context.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.CreateDrillthroughContext) && this.Detected(expression, this.m_regexes.CreateDrillthroughContext))
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, "CreateDrillthroughContext");
			}
			bool flag = this.Detected(expression, this.m_regexes.ScopesDetection);
			if (flag && this.m_context.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.ScopesCollection))
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, "Scopes");
			}
			if (flag && (grammarFlags & GrammarFlags.DenyScopes) != 0)
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsInvalidScopeCollectionReference, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
			}
			if ((grammarFlags & GrammarFlags.DenyFields) != 0 && this.Detected(expression, this.m_regexes.FieldDetection))
			{
				if ((this.m_context.Location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0)
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
					Global.Tracer.Assert(ExpressionType.ReportParameter == this.m_context.ExpressionType, "(ExpressionType.ReportParameter == m_context.ExpressionType)");
					base.m_errorContext.Register(ProcessingErrorCode.rsFieldInReportParameterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
			}
			if ((grammarFlags & GrammarFlags.DenyVariables) != 0 && this.Detected(expression, this.m_regexes.VariablesDetection))
			{
				if (this.m_context.InPrevious)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsVariableInPreviousAggregate, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (isParameter)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsAggregateofVariable, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (this.m_context.InLookup)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsLookupOfVariable, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.QueryParameter == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsVariableInQueryParameterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, this.m_context.DataSetName);
				}
				else if (ExpressionType.ReportParameter == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsVariableInReportParameterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.ReportLanguage == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsVariableInReportLanguageExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.GroupExpression == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsVariableInGroupExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.DataRegionSortExpression == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsVariableInDataRowSortExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.DataRegionFilters == this.m_context.ExpressionType || ExpressionType.DataSetFilters == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsVariableInDataRegionOrDataSetFilterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.JoinExpression == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsVariableInJoinExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else
				{
					Global.Tracer.Assert(ExpressionType.FieldValue == this.m_context.ExpressionType, "(ExpressionType.FieldValue == m_context.ExpressionType)");
					base.m_errorContext.Register(ProcessingErrorCode.rsVariableInCalculatedFieldExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
			}
			int num = this.NumberOfTimesDetected(expression, this.m_regexes.ReportItemsDetection);
			if ((grammarFlags & GrammarFlags.DenyReportItems) != 0 && 0 < num)
			{
				if (isParameter)
				{
					Global.Tracer.Assert((AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0 == (this.m_context.Location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InPageSection), "(0 == (m_context.Location & LocationFlags.InPageSection))");
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
				else if (ExpressionType.VariableValue == this.m_context.ExpressionType || ExpressionType.GroupVariableValue == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsReportItemInVariableExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.SortExpression == this.m_context.ExpressionType || ExpressionType.UserSortExpression == this.m_context.ExpressionType || ExpressionType.DataRegionSortExpression == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsReportItemInSortExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.JoinExpression == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsReportItemInJoinExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (this.m_context.InLookup)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsReportItemInLookupDestinationOrResult, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else
				{
					Global.Tracer.Assert(false, "Unknown ExpressionType: {0} denying ReportItems.", this.m_context.ExpressionType);
				}
			}
			if ((this.m_context.Location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0 && 1 < num)
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsMultiReportItemsInPageSectionExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
			}
			if (0 < num)
			{
				if ((this.m_context.Location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0)
				{
					this.m_state.PageSectionRefersToReportItems = true;
				}
				else
				{
					this.m_state.BodyRefersToReportItems = true;
				}
			}
			if ((this.m_context.Location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0 && this.Detected(expression, this.m_regexes.OverallTotalPagesDetection))
			{
				this.m_state.PageSectionRefersToOverallTotalPages = true;
			}
			if ((this.m_context.Location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0 && this.Detected(expression, this.m_regexes.TotalPagesDetection))
			{
				this.m_state.PageSectionRefersToTotalPages = true;
			}
			if (this.Detected(expression, this.m_regexes.OverallPageGlobalsDetection))
			{
				expressionInfo.ReferencedOverallPageGlobals = true;
				if ((grammarFlags & GrammarFlags.DenyOverallPageGlobals) != 0)
				{
					Global.Tracer.Assert((AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0 == (this.m_context.Location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InPageSection), "(0 == (m_context.Location & LocationFlags.InPageSection))");
					base.m_errorContext.Register(ProcessingErrorCode.rsOverallPageNumberInBody, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
			}
			if (this.Detected(expression, this.m_regexes.PageGlobalsDetection))
			{
				expressionInfo.ReferencedPageGlobals = true;
				if ((grammarFlags & GrammarFlags.DenyPageGlobals) != 0)
				{
					Global.Tracer.Assert((AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0 == (this.m_context.Location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InPageSection), "(0 == (m_context.Location & LocationFlags.InPageSection))");
					base.m_errorContext.Register(ProcessingErrorCode.rsPageNumberInBody, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
			}
			if (this.Detected(expression, this.m_regexes.AggregatesDetection))
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsGlobalNotDefined, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
			}
			if ((grammarFlags & GrammarFlags.DenyDataSets) != 0 && this.Detected(expression, this.m_regexes.DataSetsDetection))
			{
				if ((this.m_context.Location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0)
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
					Global.Tracer.Assert(ExpressionType.ReportParameter == this.m_context.ExpressionType, "(ExpressionType.ReportParameter == m_context.ExpressionType)");
					base.m_errorContext.Register(ProcessingErrorCode.rsDataSetInReportParameterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
			}
			if ((grammarFlags & GrammarFlags.DenyDataSources) != 0 && this.Detected(expression, this.m_regexes.DataSourcesDetection))
			{
				if ((this.m_context.Location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0)
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
					Global.Tracer.Assert(ExpressionType.ReportParameter == this.m_context.ExpressionType, "(ExpressionType.ReportParameter == m_context.ExpressionType)");
					base.m_errorContext.Register(ProcessingErrorCode.rsDataSourceInReportParameterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
			}
			if ((grammarFlags & GrammarFlags.DenyMeDotValue) != 0 && this.Detected(expression, this.m_regexes.MeDotValueDetection))
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsInvalidMeDotValueInExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
			}
			this.RemoveLineTerminators(ref expression, this.m_regexes.LineTerminatorDetection);
			if (this.Detected(expression, this.m_regexes.IllegalCharacterDetection))
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsInvalidCharacterInExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
			}
		}

		private void GetMeDotValueReferences(string strTransformedExpression, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo)
		{
			this.GetMeDotValueReferences(strTransformedExpression, expressionInfo, true);
		}

		private void GetMeDotValueReferences(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo)
		{
			this.GetMeDotValueReferences(expressionInfo.OriginalText, expressionInfo, false);
		}

		private void GetMeDotValueReferences(string expression, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo, bool inTransformedExpression)
		{
			MatchCollection matchCollection = this.m_regexes.MeDotValueExpression.Matches(expression);
			for (int i = 0; i < matchCollection.Count; i++)
			{
				Group group = matchCollection[i].Groups["medotvalue"];
				if (group.Value != null && group.Value.Length > 0)
				{
					if (inTransformedExpression)
					{
						expressionInfo.AddMeDotValueInTransformedExpression(group.Index);
					}
					else
					{
						expressionInfo.AddMeDotValueInOriginalText(group.Index);
					}
				}
			}
		}

		private void GetReferencedReportItemNames(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo)
		{
			this.GetReferencedReportItemNames(expressionInfo.OriginalText, expressionInfo, false);
		}

		private void GetReferencedReportItemNames(string expression, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo)
		{
			this.GetReferencedReportItemNames(expression, expressionInfo, true);
		}

		private void GetReferencedReportItemNames(string expression, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo, bool inTransformedExpression)
		{
			MatchCollection matchCollection = this.m_regexes.ReportItemName.Matches(expression);
			for (int i = 0; i < matchCollection.Count; i++)
			{
				Group group = matchCollection[i].Groups["reportitemname"];
				if (group.Value != null && group.Value.Length > 0)
				{
					if (inTransformedExpression)
					{
						expressionInfo.AddReferencedReportItemInTransformedExpression(group.Value, group.Index);
					}
					else
					{
						expressionInfo.AddReferencedReportItemInOriginalText(group.Value, group.Index);
					}
				}
			}
			matchCollection = this.m_regexes.SimpleDynamicReportItemReference.Matches(expression);
			for (int j = 0; j < matchCollection.Count; j++)
			{
				Group group2 = matchCollection[j].Groups["reportitemname"];
				if (group2.Value != null)
				{
					if (inTransformedExpression)
					{
						expressionInfo.AddReferencedReportItemInTransformedExpression(group2.Value, group2.Index);
					}
					else
					{
						expressionInfo.AddReferencedReportItemInOriginalText(group2.Value, group2.Index);
					}
				}
			}
		}

		private void GetReferencedVariableNames(string expression, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo)
		{
			MatchCollection matchCollection = this.m_regexes.VariableName.Matches(expression);
			for (int i = 0; i < matchCollection.Count; i++)
			{
				Group group = matchCollection[i].Groups["variablename"];
				if (group.Value != null && group.Value.Length > 0)
				{
					expressionInfo.AddReferencedVariable(group.Value, group.Index);
				}
			}
			matchCollection = this.m_regexes.SimpleDynamicVariableReference.Matches(expression);
			for (int j = 0; j < matchCollection.Count; j++)
			{
				Group group2 = matchCollection[j].Groups["variablename"];
				if (group2.Value != null)
				{
					expressionInfo.AddReferencedVariable(group2.Value, group2.Index);
				}
			}
		}

		private void GetReferencedFieldNames(string expression, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo)
		{
			if (this.Detected(expression, this.m_regexes.FieldDetection))
			{
				expressionInfo.HasAnyFieldReferences = true;
			}
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
			matchCollection2 = this.m_regexes.SimpleDynamicFieldReference.Matches(expression);
			for (int l = 0; l < matchCollection2.Count; l++)
			{
				Group group = matchCollection2[l].Groups["fieldname"];
				if (group.Value != null && group.Value.Length > 0)
				{
					expressionInfo.AddReferencedField(group.Value);
				}
			}
		}

		private void GetReferencedScopesAndScopedFields(string expression, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo)
		{
			MatchCollection matchCollection = this.m_regexes.ScopeName.Matches(expression);
			for (int i = 0; i < matchCollection.Count; i++)
			{
				this.HandleMatchedScopeReference(expression, expressionInfo, matchCollection[i]);
			}
			matchCollection = this.m_regexes.SimpleDynamicScopeReference.Matches(expression);
			for (int j = 0; j < matchCollection.Count; j++)
			{
				this.HandleMatchedScopeReference(expression, expressionInfo, matchCollection[j]);
			}
		}

		private void HandleMatchedScopeReference(string expression, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo, Match match)
		{
			Group group = match.Groups["scopename"];
			string value = group.Value;
			if (group.Success && !string.IsNullOrEmpty(value))
			{
				string text = null;
				if (match.Groups["hasfields"].Success)
				{
					int startat = match.Index + match.Length;
					Match match2 = this.m_regexes.DictionaryOpWithIdentifier.Match(expression, startat);
					if (!match2.Success)
					{
						match2 = this.m_regexes.IndexerWithIdentifier.Match(expression, startat);
					}
					if (match2.Success)
					{
						Group group2 = match2.Groups["fieldname"];
						if (group2.Success)
						{
							text = group2.Value;
						}
					}
				}
				ScopeReference scopeReference = null;
				scopeReference = (string.IsNullOrEmpty(text) ? new ScopeReference(value) : new ScopeReference(value, text));
				expressionInfo.AddReferencedScope(scopeReference);
			}
		}

		private void GetReferencedParameterNames(string expression, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo)
		{
			if (this.Detected(expression, this.m_regexes.DynamicParameterReference))
			{
				expressionInfo.HasDynamicParameterReference = true;
			}
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

		private bool HasRenderFormatNonIsInteractiveReference(string expression, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo, out string referencedRenderFormatProperty)
		{
			referencedRenderFormatProperty = null;
			MatchCollection matchCollection = this.m_regexes.RenderFormatPropertyName.Matches(expression);
			for (int i = 0; i < matchCollection.Count; i++)
			{
				referencedRenderFormatProperty = matchCollection[i].Result("${propertyname}");
				if (!string.IsNullOrEmpty(referencedRenderFormatProperty) && AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.CompareWithInvariantCulture("IsInteractive", referencedRenderFormatProperty, true) != 0)
				{
					return true;
				}
			}
			return false;
		}

		private void GetReferencedDataSetNames(string expression, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo)
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

		private void GetReferencedDataSourceNames(string expression, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo)
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

		private void GetRunningValue(int currentPos, string functionName, string expression, bool isParameter, GrammarFlags grammarFlags, out int newPos, out AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo runningValue)
		{
			if (this.m_context.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.RunningValue))
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, functionName);
			}
			if ((grammarFlags & GrammarFlags.DenyRunningValue) != 0)
			{
				if (this.m_context.InPrevious)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsRunningValueInPreviousAggregate, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (isParameter)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsRunningValueInAggregateExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.DataSetFilters == this.m_context.ExpressionType || ExpressionType.DataRegionFilters == this.m_context.ExpressionType || ExpressionType.GroupingFilters == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsRunningValueInFilterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.GroupExpression == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsRunningValueInGroupExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if ((this.m_context.Location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0)
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
				else if (ExpressionType.VariableValue == this.m_context.ExpressionType || ExpressionType.GroupVariableValue == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsRunningValueInVariableExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.FieldValue == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsAggregateInCalculatedFieldExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.JoinExpression == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsRunningValueInJoinExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else
				{
					Global.Tracer.Assert(ExpressionType.SortExpression == this.m_context.ExpressionType || ExpressionType.UserSortExpression == this.m_context.ExpressionType || ExpressionType.DataRegionSortExpression == this.m_context.ExpressionType, "(SortExpression == m_context.ExpressionType)");
					base.m_errorContext.Register(ProcessingErrorCode.rsRunningValueInSortExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
			}
			List<string> list = default(List<string>);
			this.GetArguments(currentPos, expression, out newPos, out list);
			if (3 != list.Count)
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsWrongNumberOfParameters, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, functionName);
			}
			runningValue = new AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo();
			if (2 <= list.Count)
			{
				bool flag;
				try
				{
					runningValue.AggregateType = (AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes)Enum.Parse(typeof(AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes), list[1], true);
					flag = (AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Aggregate != runningValue.AggregateType && AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Previous != runningValue.AggregateType && AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.CountRows != runningValue.AggregateType);
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
				if (AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Count == runningValue.AggregateType && "*" == list[0].Trim())
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsCountStarRVNotSupported, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else
				{
					runningValue.Expressions = new AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo[1];
					runningValue.Expressions[0] = this.GetParameterExpression(runningValue, list[0], grammarFlags);
				}
			}
			if (3 <= list.Count)
			{
				runningValue.Scope = this.GetAggregateScope(list[2], true);
			}
			this.DetectAggregateFieldReferences(runningValue);
			this.m_state.NumberOfRunningValues++;
		}

		private void GetPreviousAggregate(int currentPos, string functionName, string expression, bool isParameter, GrammarFlags grammarFlags, out int newPos, out AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo runningValue)
		{
			if (this.m_context.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.Previous))
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, functionName);
			}
			if ((grammarFlags & GrammarFlags.DenyPrevious) != 0)
			{
				if (this.m_context.InPrevious)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsPreviousInPreviousAggregate, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (this.m_context.InLookup)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsPreviousInLookupDestinationOrResult, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (isParameter)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsPreviousInAggregateExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.DataSetFilters == this.m_context.ExpressionType || ExpressionType.DataRegionFilters == this.m_context.ExpressionType || ExpressionType.GroupingFilters == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsPreviousAggregateInFilterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.GroupExpression == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsPreviousAggregateInGroupExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if ((this.m_context.Location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0)
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
				else if (ExpressionType.FieldValue == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsAggregateInCalculatedFieldExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.VariableValue == this.m_context.ExpressionType || ExpressionType.GroupVariableValue == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsPreviousAggregateInVariableExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.JoinExpression == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsPreviousAggregateInJoinExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else
				{
					Global.Tracer.Assert(ExpressionType.SortExpression == this.m_context.ExpressionType || ExpressionType.UserSortExpression == this.m_context.ExpressionType || ExpressionType.DataRegionSortExpression == this.m_context.ExpressionType, "(SortExpression == m_context.ExpressionType)");
					base.m_errorContext.Register(ProcessingErrorCode.rsPreviousAggregateInSortExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
			}
			this.m_state.PreviousAggregateUsed = true;
			this.m_context.InPrevious = true;
			this.m_state.NumberOfRunningValues++;
			runningValue = new AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo();
			runningValue.AggregateType = AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Previous;
			List<string> list = default(List<string>);
			this.GetArguments(currentPos, expression, out newPos, out list);
			if (list.Count == 1 || list.Count == 2)
			{
				runningValue.Expressions = new AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo[1];
				AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo parameterExpression = this.GetParameterExpression(runningValue, list[0], grammarFlags);
				parameterExpression.InPrevious = true;
				runningValue.Expressions[0] = parameterExpression;
				if (this.HasInScopeOrLevel(list[0]))
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsInScopeOrLevelInPreviousAggregate, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				if (list.Count == 2)
				{
					runningValue.Scope = this.GetAggregateScope(list[1], false);
				}
			}
			else
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsWrongNumberOfParameters, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, functionName);
			}
			this.m_context.InPrevious = false;
		}

		private bool HasInScopeOrLevel(string expression)
		{
			Match match = this.m_regexes.InScopeOrLevel.Match(expression);
			return match.Success;
		}

		private void GetRowNumber(int currentPos, string functionName, string expression, bool isParameter, GrammarFlags grammarFlags, out int newPos, out AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo rowNumber)
		{
			if (this.m_context.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.RowNumber))
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, functionName);
			}
			if ((grammarFlags & GrammarFlags.DenyRowNumber) != 0)
			{
				if (this.m_context.InPrevious)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsRowNumberInPreviousAggregate, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				if (this.m_context.InLookup)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsRowNumberInLookupDestinationOrResult, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (isParameter)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsAggregateofAggregate, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.DataSetFilters == this.m_context.ExpressionType || ExpressionType.DataRegionFilters == this.m_context.ExpressionType || ExpressionType.GroupingFilters == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsRowNumberInFilterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if ((this.m_context.Location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0)
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
				else if (ExpressionType.VariableValue == this.m_context.ExpressionType || ExpressionType.GroupVariableValue == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsRowNumberInVariableExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.FieldValue == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsAggregateInCalculatedFieldExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else
				{
					Global.Tracer.Assert(ExpressionType.SortExpression == this.m_context.ExpressionType || ExpressionType.UserSortExpression == this.m_context.ExpressionType || ExpressionType.DataRegionSortExpression == this.m_context.ExpressionType, "(SortExpression == m_context.ExpressionType)");
					base.m_errorContext.Register(ProcessingErrorCode.rsRowNumberInSortExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
			}
			List<string> list = default(List<string>);
			this.GetArguments(currentPos, expression, out newPos, out list);
			if (1 != list.Count)
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsWrongNumberOfParameters, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, functionName);
			}
			rowNumber = new AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo();
			rowNumber.AggregateType = AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.CountRows;
			rowNumber.Expressions = new AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo[0];
			if (1 <= list.Count)
			{
				rowNumber.Scope = this.GetAggregateScope(list[0], true);
			}
			this.m_state.NumberOfRunningValues++;
		}

		private string GetAggregateScope(string expression, bool allowNothing)
		{
			return this.GetScope(expression, allowNothing, ProcessingErrorCode.rsInvalidAggregateScope);
		}

		private string GetLookupScope(string expression)
		{
			return this.GetScope(expression, false, ProcessingErrorCode.rsInvalidLookupScope);
		}

		private string GetScope(string expression, bool allowNothing, ProcessingErrorCode errorCode)
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
			base.m_errorContext.Register(errorCode, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
			return null;
		}

		private bool IsRecursive(string flag)
		{
			RecursiveFlags recursiveFlags = RecursiveFlags.Simple;
			try
			{
				recursiveFlags = (RecursiveFlags)Enum.Parse(typeof(RecursiveFlags), flag, true);
			}
			catch (Exception)
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsInvalidAggregateRecursiveFlag, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
			}
			if (RecursiveFlags.Recursive == recursiveFlags)
			{
				return true;
			}
			return false;
		}

		private LookupInfo GetLookup(int currentPos, string functionName, string expression, bool isParameter, GrammarFlags grammarFlags, LookupType lookupType, out int newPos, out string lookupID)
		{
			lookupID = this.CreateLookupID();
			if (this.m_context.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.Lookup))
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, functionName);
			}
			if ((grammarFlags & GrammarFlags.DenyLookups) != 0)
			{
				if (this.m_context.InLookup)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsNestedLookups, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.DataSetFilters == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsLookupInFilterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
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
				else if (ExpressionType.ReportParameter == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsAggregateInReportParameterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else
				{
					Global.Tracer.Assert(false, "Unknown ExpressionType for Lookup restriction: {0}", this.m_context.ExpressionType);
				}
			}
			this.m_context.InLookup = true;
			List<string> list = default(List<string>);
			this.GetArguments(currentPos, expression, out newPos, out list);
			if (list.Count != 4)
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsWrongNumberOfParameters, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, functionName);
			}
			LookupInfo lookupInfo = new LookupInfo();
			lookupInfo.LookupType = lookupType;
			lookupInfo.Name = lookupID;
			lookupInfo.DestinationInfo = new LookupDestinationInfo();
			lookupInfo.DestinationInfo.IsMultiValue = (lookupType == LookupType.LookupSet);
			if (list.Count > 3)
			{
				lookupInfo.DestinationInfo.Scope = this.GetLookupScope(list[3]);
			}
			if (list.Count > 2)
			{
				lookupInfo.ResultExpr = this.GetLookupParameterExpr(list[2], GrammarFlags.DenyAggregates | GrammarFlags.DenyRunningValue | GrammarFlags.DenyRowNumber | GrammarFlags.DenyReportItems | GrammarFlags.DenyOverallPageGlobals | GrammarFlags.DenyPrevious | GrammarFlags.DenyVariables | GrammarFlags.DenyLookups | GrammarFlags.DenyPageGlobals | GrammarFlags.DenyRenderFormatAll | GrammarFlags.DenyScopes, isParameter);
			}
			if (list.Count > 1)
			{
				lookupInfo.DestinationInfo.DestinationExpr = this.GetLookupParameterExpr(list[1], GrammarFlags.DenyAggregates | GrammarFlags.DenyRunningValue | GrammarFlags.DenyRowNumber | GrammarFlags.DenyReportItems | GrammarFlags.DenyOverallPageGlobals | GrammarFlags.DenyPrevious | GrammarFlags.DenyVariables | GrammarFlags.DenyLookups | GrammarFlags.DenyPageGlobals | GrammarFlags.DenyRenderFormatAll | GrammarFlags.DenyScopes, isParameter);
			}
			if (list.Count > 0)
			{
				lookupInfo.SourceExpr = this.GetLookupParameterExpr(list[0], grammarFlags | (GrammarFlags.DenyVariables | GrammarFlags.DenyLookups | GrammarFlags.DenyRenderFormatAll), isParameter);
			}
			this.m_state.NumberOfLookups++;
			return lookupInfo;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo GetLookupParameterExpr(string parameterExpression, GrammarFlags grammarFlags, bool isParameter)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = new AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo();
			expressionInfo.OriginalText = parameterExpression;
			if (this.m_context.InAggregate)
			{
				grammarFlags |= GrammarFlags.DenyAggregates;
			}
			this.VBLex(parameterExpression, isParameter, grammarFlags, expressionInfo);
			return expressionInfo;
		}

		private bool ParseRdlFunction(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo, int currentPos, string functionName, string expression, GrammarFlags grammarFlags, out int newPos)
		{
			newPos = currentPos;
			RdlFunctionInfo rdlFunctionInfo = new RdlFunctionInfo();
			List<string> list = default(List<string>);
			this.GetArguments(currentPos, expression, out newPos, out list);
			if (list.Count < 2)
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsWrongNumberOfParameters, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, functionName);
			}
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo> list2 = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo>(list.Count);
			foreach (string item in list)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo2 = new AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo();
				list2.Add(expressionInfo2);
				expressionInfo2.OriginalText = item;
				this.VBLex(item, false, grammarFlags, expressionInfo2);
				if (expressionInfo2.Type == AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Expression)
				{
					return true;
				}
			}
			expressionInfo.SetAsRdlFunction(rdlFunctionInfo);
			rdlFunctionInfo.SetFunctionType(functionName);
			rdlFunctionInfo.Expressions = list2;
			return false;
		}

		private void GetAggregate(int currentPos, string functionName, string expression, bool isParameter, GrammarFlags grammarFlags, out int newPos, out AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo aggregate)
		{
			if ((grammarFlags & GrammarFlags.DenyAggregates) != 0)
			{
				if (isParameter)
				{
					if (this.m_context.InLookup)
					{
						base.m_errorContext.Register(ProcessingErrorCode.rsNestedAggregateViaLookup, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
					}
					else if ((this.m_context.Location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0)
					{
						base.m_errorContext.Register(ProcessingErrorCode.rsNestedAggregateInPageSection, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
					}
					else
					{
						base.m_errorContext.Register(ProcessingErrorCode.rsAggregateofAggregate, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
					}
				}
				else if (ExpressionType.DataSetFilters == this.m_context.ExpressionType || ExpressionType.DataRegionFilters == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsAggregateInFilterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.GroupExpression == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsAggregateInGroupExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.DataRegionSortExpression == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsAggregateInDataRowSortExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
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
				else if (ExpressionType.ReportParameter == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsAggregateInReportParameterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.JoinExpression == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsAggregateInJoinExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (this.m_context.InLookup)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsAggregateInLookupDestinationOrResult, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else
				{
					Global.Tracer.Assert(this.m_context.InPrevious, "(m_context.InPrevious)");
				}
			}
			bool inPrevious = this.m_context.InPrevious;
			this.m_context.InPrevious = false;
			List<string> list = default(List<string>);
			this.GetArguments(currentPos, expression, out newPos, out list);
			if (list.Count > 3)
			{
				base.m_errorContext.Register(ProcessingErrorCode.rsWrongNumberOfParameters, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, functionName);
			}
			aggregate = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo();
			aggregate.AggregateType = (AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes)Enum.Parse(typeof(AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes), functionName, true);
			if ((grammarFlags & GrammarFlags.DenyPostSortAggregate) != 0 && aggregate.IsPostSortAggregate())
			{
				if (this.m_context.InAggregate)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsPostSortAggregateInAggregateExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.GroupingFilters == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsPostSortAggregateInGroupFilterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.SortExpression == this.m_context.ExpressionType || ExpressionType.UserSortExpression == this.m_context.ExpressionType || ExpressionType.DataRegionFilters == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsPostSortAggregateInSortExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.VariableValue == this.m_context.ExpressionType || ExpressionType.GroupVariableValue == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsPostSortAggregateInVariableExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
			}
			if (AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.CountRows == aggregate.AggregateType)
			{
				aggregate.AggregateType = AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.CountRows;
				aggregate.Expressions = new AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo[0];
				if (1 <= list.Count)
				{
					aggregate.SetScope(this.GetAggregateScope(list[0], false));
				}
				if (2 <= list.Count)
				{
					if (this.m_context.InAggregate)
					{
						base.m_errorContext.Register(ProcessingErrorCode.rsInvalidNestedRecursiveAggregate, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
					}
					else
					{
						aggregate.Recursive = this.IsRecursive(list[1]);
					}
				}
				if (3 <= list.Count)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsWrongNumberOfParameters, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName, functionName);
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
					if (AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Count == aggregate.AggregateType && "*" == list[0].Trim())
					{
						base.m_errorContext.Register(ProcessingErrorCode.rsCountStarNotSupported, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
					}
					else
					{
						aggregate.Expressions = new AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo[1];
						aggregate.Expressions[0] = this.GetParameterExpression(aggregate, list[0], grammarFlags);
						if (AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Aggregate == aggregate.AggregateType)
						{
							if (AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Field != aggregate.Expressions[0].Type)
							{
								base.m_errorContext.Register(ProcessingErrorCode.rsInvalidCustomAggregateExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
							}
							if (aggregate.AggregateType == AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Aggregate && inPrevious)
							{
								base.m_errorContext.Register(ProcessingErrorCode.rsAggregateInPreviousAggregate, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
							}
						}
					}
				}
				if (2 <= list.Count)
				{
					aggregate.SetScope(this.GetAggregateScope(list[1], false));
				}
				if (3 <= list.Count)
				{
					if (aggregate.IsPostSortAggregate() || AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Aggregate == aggregate.AggregateType || inPrevious)
					{
						base.m_errorContext.Register(ProcessingErrorCode.rsInvalidRecursiveAggregate, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
					}
					else if (this.m_context.InAggregate)
					{
						base.m_errorContext.Register(ProcessingErrorCode.rsInvalidNestedRecursiveAggregate, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
					}
					else if (aggregate.IsAggregateOfAggregate)
					{
						base.m_errorContext.Register(ProcessingErrorCode.rsRecursiveAggregateOfAggregate, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
					}
					else
					{
						aggregate.Recursive = this.IsRecursive(list[2]);
					}
				}
			}
			if (this.m_context.OuterAggregate != null)
			{
				this.m_context.OuterAggregate.AddNestedAggregate(aggregate);
			}
			this.m_state.AggregateOfAggregatesUsed |= aggregate.IsAggregateOfAggregate;
			if (aggregate.IsAggregateOfAggregate && this.m_context.ExpressionType == ExpressionType.UserSortExpression)
			{
				this.m_state.AggregateOfAggregatesUsedInUserSort = true;
			}
			this.DetectAggregateFieldReferences(aggregate);
			if ((grammarFlags & GrammarFlags.DenyAggregatesOfAggregates) != 0 && aggregate.IsAggregateOfAggregate)
			{
				if (ExpressionType.GroupingFilters == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsNestedAggregateInFilterExpression, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
				else if (ExpressionType.GroupVariableValue == this.m_context.ExpressionType)
				{
					base.m_errorContext.Register(ProcessingErrorCode.rsNestedAggregateInGroupVariable, Severity.Error, this.m_context.ObjectType, this.m_context.ObjectName, this.m_context.PropertyName);
				}
			}
		}

		private void DetectAggregateFieldReferences(AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo aggregate)
		{
			if (aggregate.Expressions != null && aggregate.Expressions.Length > 0)
			{
				int num = 0;
				while (true)
				{
					if (num < aggregate.Expressions.Length)
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = aggregate.Expressions[num];
						if (!expressionInfo.HasAnyFieldReferences)
						{
							if (expressionInfo.Lookups != null)
							{
								foreach (LookupInfo lookup in expressionInfo.Lookups)
								{
									if (lookup.SourceExpr.HasAnyFieldReferences)
									{
										aggregate.PublishingInfo.HasAnyFieldReferences = true;
										break;
									}
								}
							}
							num++;
							continue;
						}
						break;
					}
					return;
				}
				aggregate.PublishingInfo.HasAnyFieldReferences = true;
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo GetParameterExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo outerAggregate, string parameterExpression, GrammarFlags grammarFlags)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = new AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo();
			expressionInfo.OriginalText = parameterExpression;
			grammarFlags = (((this.m_context.Location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InPageSection) == (AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0) ? (this.m_context.InPrevious ? (grammarFlags | (GrammarFlags.DenyRowNumber | GrammarFlags.DenyReportItems | GrammarFlags.DenyPrevious | GrammarFlags.DenyVariables | GrammarFlags.DenyScopes)) : (grammarFlags | (GrammarFlags.DenyRunningValue | GrammarFlags.DenyRowNumber | GrammarFlags.DenyReportItems | GrammarFlags.DenyPostSortAggregate | GrammarFlags.DenyPrevious | GrammarFlags.DenyVariables | GrammarFlags.DenyScopes))) : (grammarFlags | (GrammarFlags.DenyAggregates | GrammarFlags.DenyRunningValue | GrammarFlags.DenyRowNumber | GrammarFlags.DenyPostSortAggregate | GrammarFlags.DenyPrevious | GrammarFlags.DenyVariables | GrammarFlags.DenyScopes)));
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo outerAggregate2 = this.m_context.OuterAggregate;
			this.m_context.OuterAggregate = outerAggregate;
			this.VBLex(parameterExpression, true, grammarFlags, expressionInfo);
			this.m_context.OuterAggregate = outerAggregate2;
			return expressionInfo;
		}

		private void GetArguments(int currentPos, string expression, out int newPos, out List<string> arguments)
		{
			int num = 1;
			int num2 = 0;
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
					string text5 = match.Result("${openCurly}");
					string text6 = match.Result("${closeCurly}");
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
					else if (text5 != null && text5.Length != 0)
					{
						num2++;
						text += expression.Substring(currentPos, match.Index - currentPos + match.Length);
					}
					else if (text6 != null && text6.Length != 0)
					{
						num2--;
						text += expression.Substring(currentPos, match.Index - currentPos + match.Length);
					}
					else if (text4 != null && text4.Length != 0)
					{
						if (1 == num && num2 == 0)
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
			return "Aggregate" + this.m_state.LastID;
		}

		private string CreateLookupID()
		{
			return "Lookup" + this.m_state.LastLookupID;
		}
	}
}
