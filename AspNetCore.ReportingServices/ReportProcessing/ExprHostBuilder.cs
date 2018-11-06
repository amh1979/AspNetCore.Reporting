using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Globalization;
using System.Security.Permissions;
using System.Text.RegularExpressions;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class ExprHostBuilder
	{
		internal enum ErrorSource
		{
			Expression,
			CodeModuleClassInstanceDecl,
			CustomCode,
			Unknown
		}

		private static class Constants
		{
			internal const string ReportObjectModelNS = "AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel";

			internal const string ExprHostObjectModelNS = "AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel";

			internal const string ReportExprHost = "ReportExprHost";

			internal const string IndexedExprHost = "IndexedExprHost";

			internal const string ReportParamExprHost = "ReportParamExprHost";

			internal const string CalcFieldExprHost = "CalcFieldExprHost";

			internal const string DataSourceExprHost = "DataSourceExprHost";

			internal const string DataSetExprHost = "DataSetExprHost";

			internal const string ReportItemExprHost = "ReportItemExprHost";

			internal const string ActionExprHost = "ActionExprHost";

			internal const string ActionInfoExprHost = "ActionInfoExprHost";

			internal const string TextBoxExprHost = "TextBoxExprHost";

			internal const string ImageExprHost = "ImageExprHost";

			internal const string ParamExprHost = "ParamExprHost";

			internal const string SubreportExprHost = "SubreportExprHost";

			internal const string ActiveXControlExprHost = "ActiveXControlExprHost";

			internal const string SortingExprHost = "SortingExprHost";

			internal const string FilterExprHost = "FilterExprHost";

			internal const string GroupingExprHost = "GroupingExprHost";

			internal const string ListExprHost = "ListExprHost";

			internal const string TableGroupExprHost = "TableGroupExprHost";

			internal const string TableExprHost = "TableExprHost";

			internal const string MatrixDynamicGroupExprHost = "MatrixDynamicGroupExprHost";

			internal const string MatrixExprHost = "MatrixExprHost";

			internal const string ChartExprHost = "ChartExprHost";

			internal const string OWCChartExprHost = "OWCChartExprHost";

			internal const string StyleExprHost = "StyleExprHost";

			internal const string AggregateParamExprHost = "AggregateParamExprHost";

			internal const string MultiChartExprHost = "MultiChartExprHost";

			internal const string ChartDynamicGroupExprHost = "ChartDynamicGroupExprHost";

			internal const string ChartDataPointExprHost = "ChartDataPointExprHost";

			internal const string ChartTitleExprHost = "ChartTitleExprHost";

			internal const string AxisExprHost = "AxisExprHost";

			internal const string DataValueExprHost = "DataValueExprHost";

			internal const string CustomReportItemExprHost = "CustomReportItemExprHost";

			internal const string DataGroupingExprHost = "DataGroupingExprHost";

			internal const string DataCellExprHost = "DataCellExprHost";

			internal const string ParametersOnlyParam = "parametersOnly";

			internal const string CustomCodeProxy = "CustomCodeProxy";

			internal const string CustomCodeProxyBase = "CustomCodeProxyBase";

			internal const string ReportObjectModelParam = "reportObjectModel";

			internal const string SetReportObjectModel = "SetReportObjectModel";

			internal const string Code = "Code";

			internal const string CodeProxyBase = "m_codeProxyBase";

			internal const string CodeParam = "code";

			internal const string Report = "Report";

			internal const string RemoteArrayWrapper = "RemoteArrayWrapper";

			internal const string LabelExpr = "LabelExpr";

			internal const string ValueExpr = "ValueExpr";

			internal const string NoRowsExpr = "NoRowsExpr";

			internal const string ParameterHosts = "m_parameterHostsRemotable";

			internal const string IndexParam = "index";

			internal const string FilterHosts = "m_filterHostsRemotable";

			internal const string SortingHost = "SortingHost";

			internal const string GroupingHost = "GroupingHost";

			internal const string SubgroupHost = "SubgroupHost";

			internal const string VisibilityHiddenExpr = "VisibilityHiddenExpr";

			internal const string SortDirectionHosts = "SortDirectionHosts";

			internal const string DataValueHosts = "m_dataValueHostsRemotable";

			internal const string CustomPropertyHosts = "m_customPropertyHostsRemotable";

			internal const string ReportLanguageExpr = "ReportLanguageExpr";

			internal const string AggregateParamHosts = "m_aggregateParamHostsRemotable";

			internal const string ReportParameterHosts = "m_reportParameterHostsRemotable";

			internal const string DataSourceHosts = "m_dataSourceHostsRemotable";

			internal const string DataSetHosts = "m_dataSetHostsRemotable";

			internal const string PageSectionHosts = "m_pageSectionHostsRemotable";

			internal const string LineHosts = "m_lineHostsRemotable";

			internal const string RectangleHosts = "m_rectangleHostsRemotable";

			internal const string TextBoxHosts = "m_textBoxHostsRemotable";

			internal const string ImageHosts = "m_imageHostsRemotable";

			internal const string SubreportHosts = "m_subreportHostsRemotable";

			internal const string ActiveXControlHosts = "m_activeXControlHostsRemotable";

			internal const string ListHosts = "m_listHostsRemotable";

			internal const string TableHosts = "m_tableHostsRemotable";

			internal const string MatrixHosts = "m_matrixHostsRemotable";

			internal const string ChartHosts = "m_chartHostsRemotable";

			internal const string OWCChartHosts = "m_OWCChartHostsRemotable";

			internal const string CustomReportItemHosts = "m_customReportItemHostsRemotable";

			internal const string ConnectStringExpr = "ConnectStringExpr";

			internal const string FieldHosts = "m_fieldHostsRemotable";

			internal const string QueryParametersHost = "QueryParametersHost";

			internal const string QueryCommandTextExpr = "QueryCommandTextExpr";

			internal const string ValidValuesHost = "ValidValuesHost";

			internal const string ValidValueLabelsHost = "ValidValueLabelsHost";

			internal const string ValidationExpressionExpr = "ValidationExpressionExpr";

			internal const string ActionInfoHost = "ActionInfoHost";

			internal const string ActionHost = "ActionHost";

			internal const string ActionItemHosts = "m_actionItemHostsRemotable";

			internal const string BookmarkExpr = "BookmarkExpr";

			internal const string ToolTipExpr = "ToolTipExpr";

			internal const string ToggleImageInitialStateExpr = "ToggleImageInitialStateExpr";

			internal const string UserSortExpressionsHost = "UserSortExpressionsHost";

			internal const string MIMETypeExpr = "MIMETypeExpr";

			internal const string OmitExpr = "OmitExpr";

			internal const string HyperlinkExpr = "HyperlinkExpr";

			internal const string DrillThroughReportNameExpr = "DrillThroughReportNameExpr";

			internal const string DrillThroughParameterHosts = "m_drillThroughParameterHostsRemotable";

			internal const string DrillThroughBookmakLinkExpr = "DrillThroughBookmarkLinkExpr";

			internal const string BookmarkLinkExpr = "BookmarkLinkExpr";

			internal const string FilterExpressionExpr = "FilterExpressionExpr";

			internal const string ParentExpressionsHost = "ParentExpressionsHost";

			internal const string SubGroupHost = "SubGroupHost";

			internal const string SubtotalHost = "SubtotalHost";

			internal const string RowGroupingsHost = "RowGroupingsHost";

			internal const string ColumnGroupingsHost = "ColumnGroupingsHost";

			internal const string SeriesGroupingsHost = "SeriesGroupingsHost";

			internal const string CategoryGroupingsHost = "CategoryGroupingsHost";

			internal const string MultiChartHost = "MultiChartHost";

			internal const string HeadingLabelExpr = "HeadingLabelExpr";

			internal const string ChartDataPointHosts = "m_chartDataPointHostsRemotable";

			internal const string DataLabelValueExpr = "DataLabelValueExpr";

			internal const string DataLabelStyleHost = "DataLabelStyleHost";

			internal const string StyleHost = "StyleHost";

			internal const string MarkerStyleHost = "MarkerStyleHost";

			internal const string TitleHost = "TitleHost";

			internal const string CaptionExpr = "CaptionExpr";

			internal const string MajorGridLinesHost = "MajorGridLinesHost";

			internal const string MinorGridLinesHost = "MinorGridLinesHost";

			internal const string StaticRowLabelsHost = "StaticRowLabelsHost";

			internal const string StaticColumnLabelsHost = "StaticColumnLabelsHost";

			internal const string CategoryAxisHost = "CategoryAxisHost";

			internal const string ValueAxisHost = "ValueAxisHost";

			internal const string LegendHost = "LegendHost";

			internal const string PlotAreaHost = "PlotAreaHost";

			internal const string AxisMinExpr = "AxisMinExpr";

			internal const string AxisMaxExpr = "AxisMaxExpr";

			internal const string AxisCrossAtExpr = "AxisCrossAtExpr";

			internal const string AxisMajorIntervalExpr = "AxisMajorIntervalExpr";

			internal const string AxisMinorIntervalExpr = "AxisMinorIntervalExpr";

			internal const string TableGroupsHost = "TableGroupsHost";

			internal const string TableRowVisibilityHiddenExpressions = "TableRowVisibilityHiddenExpressions";

			internal const string TableColumnVisibilityHiddenExpressions = "TableColumnVisibilityHiddenExpressions";

			internal const string OWCChartColumnHosts = "OWCChartColumnHosts";

			internal const string DataValueNameExpr = "DataValueNameExpr";

			internal const string DataValueValueExpr = "DataValueValueExpr";

			internal const string DataGroupingHosts = "m_dataGroupingHostsRemotable";

			internal const string DataCellHosts = "m_dataCellHostsRemotable";
		}

		private abstract class TypeDecl
		{
			internal CodeTypeDeclaration Type;

			internal string BaseTypeName;

			internal TypeDecl Parent;

			internal CodeConstructor Constructor;

			internal bool HasExpressions;

			internal CodeExpressionCollection DataValues;

			protected bool m_setCode;

			internal void NestedTypeAdd(string name, CodeTypeDeclaration nestedType)
			{
				this.ConstructorCreate();
				this.Type.Members.Add(nestedType);
				this.Constructor.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), name), this.CreateTypeCreateExpression(nestedType.Name)));
			}

			internal int NestedTypeColAdd(string name, string baseTypeName, ref CodeExpressionCollection initializers, CodeTypeDeclaration nestedType)
			{
				this.Type.Members.Add(nestedType);
				this.TypeColInit(name, baseTypeName, ref initializers);
				return initializers.Add(this.CreateTypeCreateExpression(nestedType.Name));
			}

			protected TypeDecl(string typeName, string baseTypeName, TypeDecl parent, bool setCode)
			{
				this.BaseTypeName = baseTypeName;
				this.Parent = parent;
				this.m_setCode = setCode;
				this.Type = this.CreateType(typeName, baseTypeName);
			}

			protected void ConstructorCreate()
			{
				if (this.Constructor == null)
				{
					this.Constructor = this.CreateConstructor();
					this.Type.Members.Add(this.Constructor);
				}
			}

			protected virtual CodeConstructor CreateConstructor()
			{
				CodeConstructor codeConstructor = new CodeConstructor();
				codeConstructor.Attributes = MemberAttributes.Public;
				return codeConstructor;
			}

			protected CodeAssignStatement CreateTypeColInitStatement(string name, string baseTypeName, ref CodeExpressionCollection initializers)
			{
				CodeObjectCreateExpression codeObjectCreateExpression = new CodeObjectCreateExpression();
				codeObjectCreateExpression.CreateType = new CodeTypeReference("RemoteArrayWrapper", new CodeTypeReference(baseTypeName));
				if (initializers != null)
				{
					codeObjectCreateExpression.Parameters.AddRange(initializers);
				}
				initializers = codeObjectCreateExpression.Parameters;
				return new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), name), codeObjectCreateExpression);
			}

			protected virtual CodeTypeDeclaration CreateType(string name, string baseType)
			{
				Global.Tracer.Assert(name != null);
				CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration(name);
				if (baseType != null)
				{
					codeTypeDeclaration.BaseTypes.Add(new CodeTypeReference(baseType));
				}
				codeTypeDeclaration.Attributes = (MemberAttributes)24578;
				return codeTypeDeclaration;
			}

			private void TypeColInit(string name, string baseTypeName, ref CodeExpressionCollection initializers)
			{
				this.ConstructorCreate();
				if (initializers == null)
				{
					this.Constructor.Statements.Add(this.CreateTypeColInitStatement(name, baseTypeName, ref initializers));
				}
			}

			private CodeObjectCreateExpression CreateTypeCreateExpression(string typeName)
			{
				if (this.m_setCode)
				{
					return new CodeObjectCreateExpression(typeName, new CodeArgumentReferenceExpression("Code"));
				}
				return new CodeObjectCreateExpression(typeName);
			}
		}

		private sealed class RootTypeDecl : TypeDecl
		{
			internal CodeExpressionCollection Aggregates;

			internal CodeExpressionCollection PageSections;

			internal CodeExpressionCollection ReportParameters;

			internal CodeExpressionCollection DataSources;

			internal CodeExpressionCollection DataSets;

			internal CodeExpressionCollection Lines;

			internal CodeExpressionCollection Rectangles;

			internal CodeExpressionCollection TextBoxes;

			internal CodeExpressionCollection Images;

			internal CodeExpressionCollection Subreports;

			internal CodeExpressionCollection ActiveXControls;

			internal CodeExpressionCollection Lists;

			internal CodeExpressionCollection Tables;

			internal CodeExpressionCollection Matrices;

			internal CodeExpressionCollection Charts;

			internal CodeExpressionCollection OWCCharts;

			internal CodeExpressionCollection CustomReportItems;

			internal RootTypeDecl(bool setCode)
				: base("ReportExprHostImpl", "ReportExprHost", null, setCode)
			{
			}

			protected override CodeConstructor CreateConstructor()
			{
				CodeConstructor codeConstructor = base.CreateConstructor();
				codeConstructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(bool), "parametersOnly"));
				CodeParameterDeclarationExpression value = new CodeParameterDeclarationExpression(typeof(object), "reportObjectModel");
				codeConstructor.Parameters.Add(value);
				codeConstructor.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression("reportObjectModel"));
				this.ReportParameters = new CodeExpressionCollection();
				this.DataSources = new CodeExpressionCollection();
				this.DataSets = new CodeExpressionCollection();
				return codeConstructor;
			}

			protected override CodeTypeDeclaration CreateType(string name, string baseType)
			{
				CodeTypeDeclaration codeTypeDeclaration = base.CreateType(name, baseType);
				if (base.m_setCode)
				{
					CodeMemberField codeMemberField = new CodeMemberField("CustomCodeProxy", "Code");
					codeMemberField.Attributes = (MemberAttributes)20482;
					codeTypeDeclaration.Members.Add(codeMemberField);
				}
				return codeTypeDeclaration;
			}

			internal void CompleteConstructorCreation()
			{
				if (base.HasExpressions)
				{
					if (base.Constructor == null)
					{
						base.ConstructorCreate();
					}
					else
					{
						CodeConditionStatement codeConditionStatement = new CodeConditionStatement();
						codeConditionStatement.Condition = new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("parametersOnly"), CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(true));
						if (this.ReportParameters.Count > 0)
						{
							codeConditionStatement.TrueStatements.Add(base.CreateTypeColInitStatement("m_reportParameterHostsRemotable", "ReportParamExprHost", ref this.ReportParameters));
						}
						codeConditionStatement.TrueStatements.Add(new CodeMethodReturnStatement());
						base.Constructor.Statements.Insert(0, codeConditionStatement);
						if (this.DataSources.Count > 0)
						{
							base.Constructor.Statements.Insert(0, base.CreateTypeColInitStatement("m_dataSourceHostsRemotable", "DataSourceExprHost", ref this.DataSources));
						}
						if (this.DataSets.Count > 0)
						{
							base.Constructor.Statements.Insert(0, base.CreateTypeColInitStatement("m_dataSetHostsRemotable", "DataSetExprHost", ref this.DataSets));
						}
						if (base.m_setCode)
						{
							base.Constructor.Statements.Insert(0, new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "m_codeProxyBase"), new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "Code")));
							base.Constructor.Statements.Insert(0, new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "Code"), new CodeObjectCreateExpression("CustomCodeProxy", new CodeThisReferenceExpression())));
						}
					}
				}
			}
		}

		private sealed class NonRootTypeDecl : TypeDecl
		{
			internal CodeExpressionCollection Parameters;

			internal CodeExpressionCollection Filters;

			internal CodeExpressionCollection Actions;

			internal CodeExpressionCollection Fields;

			internal CodeExpressionCollection DataPoints;

			internal CodeExpressionCollection DataGroupings;

			internal CodeExpressionCollection DataCells;

			internal ReturnStatementList IndexedExpressions;

			internal NonRootTypeDecl(string typeName, string baseTypeName, TypeDecl parent, bool setCode)
				: base(typeName, baseTypeName, parent, setCode)
			{
				if (setCode)
				{
					base.ConstructorCreate();
				}
			}

			protected override CodeConstructor CreateConstructor()
			{
				CodeConstructor codeConstructor = base.CreateConstructor();
				if (base.m_setCode)
				{
					codeConstructor.Parameters.Add(new CodeParameterDeclarationExpression("CustomCodeProxy", "code"));
					codeConstructor.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "Code"), new CodeArgumentReferenceExpression("code")));
				}
				return codeConstructor;
			}

			protected override CodeTypeDeclaration CreateType(string name, string baseType)
			{
				CodeTypeDeclaration codeTypeDeclaration = base.CreateType(string.Format(CultureInfo.InvariantCulture, "{0}_{1}", name, baseType), baseType);
				if (base.m_setCode)
				{
					CodeMemberField codeMemberField = new CodeMemberField("CustomCodeProxy", "Code");
					codeMemberField.Attributes = (MemberAttributes)20482;
					codeTypeDeclaration.Members.Add(codeMemberField);
				}
				return codeTypeDeclaration;
			}
		}

		private sealed class CustomCodeProxyDecl : TypeDecl
		{
			internal CustomCodeProxyDecl(TypeDecl parent)
				: base("CustomCodeProxy", "CustomCodeProxyBase", parent, false)
			{
				base.ConstructorCreate();
			}

			protected override CodeConstructor CreateConstructor()
			{
				CodeConstructor codeConstructor = base.CreateConstructor();
				codeConstructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IReportObjectModelProxyForCustomCode), "reportObjectModel"));
				codeConstructor.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression("reportObjectModel"));
				return codeConstructor;
			}

			internal void AddClassInstance(string className, string instanceName, int id)
			{
				string fileName = "CMCID" + id.ToString(CultureInfo.InvariantCulture) + "end";
				CodeMemberField codeMemberField = new CodeMemberField(className, "m_" + instanceName);
				codeMemberField.Attributes = (MemberAttributes)20482;
				codeMemberField.InitExpression = new CodeObjectCreateExpression(className);
				codeMemberField.LinePragma = new CodeLinePragma(fileName, 0);
				base.Type.Members.Add(codeMemberField);
				CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
				codeMemberProperty.Type = new CodeTypeReference(className);
				codeMemberProperty.Name = instanceName;
				codeMemberProperty.Attributes = (MemberAttributes)24578;
				codeMemberProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), codeMemberField.Name)));
				codeMemberProperty.LinePragma = new CodeLinePragma(fileName, 2);
				base.Type.Members.Add(codeMemberProperty);
			}

			internal void AddCode(string code)
			{
				CodeTypeMember codeTypeMember = new CodeSnippetTypeMember(code);
				codeTypeMember.LinePragma = new CodeLinePragma("CustomCode", 0);
				base.Type.Members.Add(codeTypeMember);
			}
		}

		private sealed class ReturnStatementList
		{
			private ArrayList m_list = new ArrayList();

			internal CodeMethodReturnStatement this[int index]
			{
				get
				{
					return (CodeMethodReturnStatement)this.m_list[index];
				}
			}

			internal int Count
			{
				get
				{
					return this.m_list.Count;
				}
			}

			internal int Add(CodeMethodReturnStatement retStatement)
			{
				return this.m_list.Add(retStatement);
			}
		}

		internal const string RootType = "ReportExprHostImpl";

		private const string EndSrcMarker = "end";

		private const string ExprSrcMarker = "Expr";

		private const string CustomCodeSrcMarker = "CustomCode";

		private const string CodeModuleClassInstanceDeclSrcMarker = "CMCID";

		private RootTypeDecl m_rootTypeDecl;

		private TypeDecl m_currentTypeDecl;

		private bool m_setCode;

		private static readonly Regex m_findExprNumber = new Regex("^Expr([0-9]+)end", RegexOptions.Compiled);

		private static readonly Regex m_findCodeModuleClassInstanceDeclNumber = new Regex("^CMCID([0-9]+)end", RegexOptions.Compiled);

		internal bool HasExpressions
		{
			get
			{
				if (this.m_rootTypeDecl != null)
				{
					return this.m_rootTypeDecl.HasExpressions;
				}
				return false;
			}
		}

		internal bool CustomCode
		{
			get
			{
				return this.m_setCode;
			}
		}

		internal ExprHostBuilder()
		{
		}

		internal void SetCustomCode()
		{
			this.m_setCode = true;
		}

		internal CodeCompileUnit GetExprHost(IntermediateFormatVersion version, bool refusePermissions)
		{
			Global.Tracer.Assert(this.m_rootTypeDecl != null && this.m_currentTypeDecl.Parent == null, "(m_rootTypeDecl != null && m_currentTypeDecl.Parent == null)");
			CodeCompileUnit codeCompileUnit = null;
			if (this.HasExpressions)
			{
				codeCompileUnit = new CodeCompileUnit();
				codeCompileUnit.AssemblyCustomAttributes.Add(new CodeAttributeDeclaration("System.Reflection.AssemblyVersion", new CodeAttributeArgument(new CodePrimitiveExpression(version.ToString()))));
				if (refusePermissions)
				{
					codeCompileUnit.AssemblyCustomAttributes.Add(new CodeAttributeDeclaration("System.Security.Permissions.SecurityPermission", new CodeAttributeArgument(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(SecurityAction)), "RequestMinimum")), new CodeAttributeArgument("Execution", new CodePrimitiveExpression(true))));
					codeCompileUnit.AssemblyCustomAttributes.Add(new CodeAttributeDeclaration("System.Security.Permissions.SecurityPermission", new CodeAttributeArgument(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(SecurityAction)), "RequestOptional")), new CodeAttributeArgument("Execution", new CodePrimitiveExpression(true))));
				}
				CodeNamespace codeNamespace = new CodeNamespace();
				codeCompileUnit.Namespaces.Add(codeNamespace);
				codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
				codeNamespace.Imports.Add(new CodeNamespaceImport("System.Convert"));
				codeNamespace.Imports.Add(new CodeNamespaceImport("System.Math"));
				codeNamespace.Imports.Add(new CodeNamespaceImport("Microsoft.VisualBasic"));
				codeNamespace.Imports.Add(new CodeNamespaceImport("AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel"));
				codeNamespace.Imports.Add(new CodeNamespaceImport("AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel"));
				codeNamespace.Types.Add(this.m_rootTypeDecl.Type);
			}
			this.m_rootTypeDecl = null;
			return codeCompileUnit;
		}

		internal ErrorSource ParseErrorSource(CompilerError error, out int id)
		{
			Global.Tracer.Assert(error.FileName != null, "(error.FileName != null)");
			id = -1;
			if (error.FileName.StartsWith("CustomCode", StringComparison.Ordinal))
			{
				return ErrorSource.CustomCode;
			}
			Match match = ExprHostBuilder.m_findCodeModuleClassInstanceDeclNumber.Match(error.FileName);
			if (match.Success && int.TryParse(match.Groups[1].Value, NumberStyles.Integer, (IFormatProvider)CultureInfo.InvariantCulture, out id))
			{
				return ErrorSource.CodeModuleClassInstanceDecl;
			}
			match = ExprHostBuilder.m_findExprNumber.Match(error.FileName);
			if (match.Success && int.TryParse(match.Groups[1].Value, NumberStyles.Integer, (IFormatProvider)CultureInfo.InvariantCulture, out id))
			{
				return ErrorSource.Expression;
			}
			return ErrorSource.Unknown;
		}

		internal void ReportStart()
		{
			this.m_currentTypeDecl = (this.m_rootTypeDecl = new RootTypeDecl(this.m_setCode));
		}

		internal void ReportEnd()
		{
			this.m_rootTypeDecl.CompleteConstructorCreation();
		}

		internal void ReportLanguage(ExpressionInfo expression)
		{
			this.ExpressionAdd("ReportLanguageExpr", expression);
		}

		internal void GenericLabel(ExpressionInfo expression)
		{
			this.ExpressionAdd("LabelExpr", expression);
		}

		internal void GenericValue(ExpressionInfo expression)
		{
			this.ExpressionAdd("ValueExpr", expression);
		}

		internal void GenericNoRows(ExpressionInfo expression)
		{
			this.ExpressionAdd("NoRowsExpr", expression);
		}

		internal void GenericVisibilityHidden(ExpressionInfo expression)
		{
			this.ExpressionAdd("VisibilityHiddenExpr", expression);
		}

		internal void AggregateParamExprAdd(ExpressionInfo expression)
		{
			this.AggregateStart();
			this.GenericValue(expression);
			expression.ExprHostID = this.AggregateEnd();
		}

		internal void CustomCodeProxyStart()
		{
			Global.Tracer.Assert(this.m_setCode, "(m_setCode)");
			this.m_currentTypeDecl = new CustomCodeProxyDecl(this.m_currentTypeDecl);
		}

		internal void CustomCodeProxyEnd()
		{
			this.m_rootTypeDecl.Type.Members.Add(this.m_currentTypeDecl.Type);
			this.TypeEnd(this.m_rootTypeDecl);
		}

		internal void CustomCodeClassInstance(string className, string instanceName, int id)
		{
			((CustomCodeProxyDecl)this.m_currentTypeDecl).AddClassInstance(className, instanceName, id);
		}

		internal void ReportCode(string code)
		{
			((CustomCodeProxyDecl)this.m_currentTypeDecl).AddCode(code);
		}

		internal void ReportParameterStart(string name)
		{
			this.TypeStart(name, "ReportParamExprHost");
		}

		internal int ReportParameterEnd()
		{
			this.ExprIndexerCreate();
			return this.TypeEnd((TypeDecl)this.m_rootTypeDecl, "m_reportParameterHostsRemotable", ref this.m_rootTypeDecl.ReportParameters);
		}

		internal void ReportParameterValidationExpression(ExpressionInfo expression)
		{
			this.ExpressionAdd("ValidationExpressionExpr", expression);
		}

		internal void ReportParameterDefaultValue(ExpressionInfo expression)
		{
			this.IndexedExpressionAdd(expression);
		}

		internal void ReportParameterValidValuesStart()
		{
			this.TypeStart("ReportParameterValidValues", "IndexedExprHost");
		}

		internal void ReportParameterValidValuesEnd()
		{
			this.ExprIndexerCreate();
			this.TypeEnd(this.m_currentTypeDecl.Parent, "ValidValuesHost");
		}

		internal void ReportParameterValidValue(ExpressionInfo expression)
		{
			this.IndexedExpressionAdd(expression);
		}

		internal void ReportParameterValidValueLabelsStart()
		{
			this.TypeStart("ReportParameterValidValueLabels", "IndexedExprHost");
		}

		internal void ReportParameterValidValueLabelsEnd()
		{
			this.ExprIndexerCreate();
			this.TypeEnd(this.m_currentTypeDecl.Parent, "ValidValueLabelsHost");
		}

		internal void ReportParameterValidValueLabel(ExpressionInfo expression)
		{
			this.IndexedExpressionAdd(expression);
		}

		internal void CalcFieldStart(string name)
		{
			this.TypeStart(name, "CalcFieldExprHost");
		}

		internal int CalcFieldEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_fieldHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).Fields);
		}

		internal void QueryParametersStart()
		{
			this.TypeStart("QueryParameters", "IndexedExprHost");
		}

		internal void QueryParametersEnd()
		{
			this.ExprIndexerCreate();
			this.TypeEnd(this.m_currentTypeDecl.Parent, "QueryParametersHost");
		}

		internal void QueryParameterValue(ExpressionInfo expression)
		{
			this.IndexedExpressionAdd(expression);
		}

		internal void DataSourceStart(string name)
		{
			this.TypeStart(name, "DataSourceExprHost");
		}

		internal int DataSourceEnd()
		{
			return this.TypeEnd((TypeDecl)this.m_rootTypeDecl, "m_dataSourceHostsRemotable", ref this.m_rootTypeDecl.DataSources);
		}

		internal void DataSourceConnectString(ExpressionInfo expression)
		{
			this.ExpressionAdd("ConnectStringExpr", expression);
		}

		internal void DataSetStart(string name)
		{
			this.TypeStart(name, "DataSetExprHost");
		}

		internal int DataSetEnd()
		{
			return this.TypeEnd((TypeDecl)this.m_rootTypeDecl, "m_dataSetHostsRemotable", ref this.m_rootTypeDecl.DataSets);
		}

		internal void DataSetQueryCommandText(ExpressionInfo expression)
		{
			this.ExpressionAdd("QueryCommandTextExpr", expression);
		}

		internal void PageSectionStart()
		{
			this.TypeStart(this.CreateTypeName("PageSection", this.m_rootTypeDecl.PageSections), "StyleExprHost");
		}

		internal int PageSectionEnd()
		{
			return this.TypeEnd((TypeDecl)this.m_rootTypeDecl, "m_pageSectionHostsRemotable", ref this.m_rootTypeDecl.PageSections);
		}

		internal void ParameterOmit(ExpressionInfo expression)
		{
			this.ExpressionAdd("OmitExpr", expression);
		}

		internal void StyleAttribute(string name, ExpressionInfo expression)
		{
			this.ExpressionAdd(name + "Expr", expression);
		}

		internal void ActionInfoStart()
		{
			this.TypeStart("ActionInfo", "ActionInfoExprHost");
		}

		internal void ActionInfoEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "ActionInfoHost");
		}

		internal void ActionStart()
		{
			this.TypeStart(this.CreateTypeName("Action", ((NonRootTypeDecl)this.m_currentTypeDecl).Actions), "ActionExprHost");
		}

		internal int ActionEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_actionItemHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).Actions);
		}

		internal void ActionHyperlink(ExpressionInfo expression)
		{
			this.ExpressionAdd("HyperlinkExpr", expression);
		}

		internal void ActionDrillThroughReportName(ExpressionInfo expression)
		{
			this.ExpressionAdd("DrillThroughReportNameExpr", expression);
		}

		internal void ActionDrillThroughBookmarkLink(ExpressionInfo expression)
		{
			this.ExpressionAdd("DrillThroughBookmarkLinkExpr", expression);
		}

		internal void ActionBookmarkLink(ExpressionInfo expression)
		{
			this.ExpressionAdd("BookmarkLinkExpr", expression);
		}

		internal void ActionDrillThroughParameterStart()
		{
			this.ParameterStart();
		}

		internal int ActionDrillThroughParameterEnd()
		{
			return this.ParameterEnd("m_drillThroughParameterHostsRemotable");
		}

		internal void ReportItemBookmark(ExpressionInfo expression)
		{
			this.ExpressionAdd("BookmarkExpr", expression);
		}

		internal void ReportItemToolTip(ExpressionInfo expression)
		{
			this.ExpressionAdd("ToolTipExpr", expression);
		}

		internal void LineStart(string name)
		{
			this.TypeStart(name, "ReportItemExprHost");
		}

		internal int LineEnd()
		{
			return this.ReportItemEnd("m_lineHostsRemotable", ref this.m_rootTypeDecl.Lines);
		}

		internal void RectangleStart(string name)
		{
			this.TypeStart(name, "ReportItemExprHost");
		}

		internal int RectangleEnd()
		{
			return this.ReportItemEnd("m_rectangleHostsRemotable", ref this.m_rootTypeDecl.Rectangles);
		}

		internal void TextBoxStart(string name)
		{
			this.TypeStart(name, "TextBoxExprHost");
		}

		internal int TextBoxEnd()
		{
			return this.ReportItemEnd("m_textBoxHostsRemotable", ref this.m_rootTypeDecl.TextBoxes);
		}

		internal void TextBoxToggleImageInitialState(ExpressionInfo expression)
		{
			this.ExpressionAdd("ToggleImageInitialStateExpr", expression);
		}

		internal void UserSortExpressionsStart()
		{
			this.TypeStart("UserSort", "IndexedExprHost");
		}

		internal void UserSortExpressionsEnd()
		{
			this.ExprIndexerCreate();
			this.TypeEnd(this.m_currentTypeDecl.Parent, "UserSortExpressionsHost");
		}

		internal void UserSortExpression(ExpressionInfo expression)
		{
			this.IndexedExpressionAdd(expression);
		}

		internal void ImageStart(string name)
		{
			this.TypeStart(name, "ImageExprHost");
		}

		internal int ImageEnd()
		{
			return this.ReportItemEnd("m_imageHostsRemotable", ref this.m_rootTypeDecl.Images);
		}

		internal void ImageMIMEType(ExpressionInfo expression)
		{
			this.ExpressionAdd("MIMETypeExpr", expression);
		}

		internal void SubreportStart(string name)
		{
			this.TypeStart(name, "SubreportExprHost");
		}

		internal int SubreportEnd()
		{
			return this.ReportItemEnd("m_subreportHostsRemotable", ref this.m_rootTypeDecl.Subreports);
		}

		internal void SubreportParameterStart()
		{
			this.ParameterStart();
		}

		internal int SubreportParameterEnd()
		{
			return this.ParameterEnd("m_parameterHostsRemotable");
		}

		internal void ActiveXControlStart(string name)
		{
			this.TypeStart(name, "ActiveXControlExprHost");
		}

		internal int ActiveXControlEnd()
		{
			return this.ReportItemEnd("m_activeXControlHostsRemotable", ref this.m_rootTypeDecl.ActiveXControls);
		}

		internal void ActiveXControlParameterStart()
		{
			this.ParameterStart();
		}

		internal int ActiveXControlParameterEnd()
		{
			return this.ParameterEnd("m_parameterHostsRemotable");
		}

		internal void SortingStart()
		{
			this.TypeStart("Sorting", "SortingExprHost");
		}

		internal void SortingEnd()
		{
			this.ExprIndexerCreate();
			this.TypeEnd(this.m_currentTypeDecl.Parent, "SortingHost");
		}

		internal void SortingExpression(ExpressionInfo expression)
		{
			this.IndexedExpressionAdd(expression);
		}

		internal void SortDirectionsStart()
		{
			this.TypeStart("SortDirections", "IndexedExprHost");
		}

		internal void SortDirectionsEnd()
		{
			this.ExprIndexerCreate();
			this.TypeEnd(this.m_currentTypeDecl.Parent, "SortDirectionHosts");
		}

		internal void SortDirection(ExpressionInfo expression)
		{
			this.IndexedExpressionAdd(expression);
		}

		internal void FilterStart()
		{
			this.TypeStart(this.CreateTypeName("Filter", ((NonRootTypeDecl)this.m_currentTypeDecl).Filters), "FilterExprHost");
		}

		internal int FilterEnd()
		{
			this.ExprIndexerCreate();
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_filterHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).Filters);
		}

		internal void FilterExpression(ExpressionInfo expression)
		{
			this.ExpressionAdd("FilterExpressionExpr", expression);
		}

		internal void FilterValue(ExpressionInfo expression)
		{
			this.IndexedExpressionAdd(expression);
		}

		internal void GroupingStart(string typeName)
		{
			this.TypeStart(typeName, "GroupingExprHost");
		}

		internal void GroupingEnd()
		{
			this.ExprIndexerCreate();
			this.TypeEnd(this.m_currentTypeDecl.Parent, "GroupingHost");
		}

		internal void GroupingExpression(ExpressionInfo expression)
		{
			this.IndexedExpressionAdd(expression);
		}

		internal void GroupingParentExpressionsStart()
		{
			this.TypeStart("Parent", "IndexedExprHost");
		}

		internal void GroupingParentExpressionsEnd()
		{
			this.ExprIndexerCreate();
			this.TypeEnd(this.m_currentTypeDecl.Parent, "ParentExpressionsHost");
		}

		internal void GroupingParentExpression(ExpressionInfo expression)
		{
			this.IndexedExpressionAdd(expression);
		}

		internal void ListStart(string name)
		{
			this.TypeStart(name, "ListExprHost");
		}

		internal int ListEnd()
		{
			return this.ReportItemEnd("m_listHostsRemotable", ref this.m_rootTypeDecl.Lists);
		}

		internal void MatrixDynamicGroupStart(string name)
		{
			this.TypeStart("MatrixDynamicGroup_" + name, "MatrixDynamicGroupExprHost");
		}

		internal bool MatrixDynamicGroupEnd(bool column)
		{
			switch (this.m_currentTypeDecl.Parent.BaseTypeName)
			{
			case "MatrixExprHost":
				if (column)
				{
					return this.TypeEnd(this.m_currentTypeDecl.Parent, "ColumnGroupingsHost");
				}
				return this.TypeEnd(this.m_currentTypeDecl.Parent, "RowGroupingsHost");
			case "MatrixDynamicGroupExprHost":
				return this.TypeEnd(this.m_currentTypeDecl.Parent, "SubGroupHost");
			default:
				Global.Tracer.Assert(false);
				return false;
			}
		}

		internal void SubtotalStart()
		{
			this.TypeStart("Subtotal", "StyleExprHost");
		}

		internal void SubtotalEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "SubtotalHost");
		}

		internal void MatrixStart(string name)
		{
			this.TypeStart(name, "MatrixExprHost");
		}

		internal int MatrixEnd()
		{
			return this.ReportItemEnd("m_matrixHostsRemotable", ref this.m_rootTypeDecl.Matrices);
		}

		internal void MultiChartStart()
		{
			this.TypeStart("MultiChart", "MultiChartExprHost");
		}

		internal void MultiChartEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "MultiChartHost");
		}

		internal void ChartDynamicGroupStart(string name)
		{
			this.TypeStart("ChartDynamicGroup_" + name, "ChartDynamicGroupExprHost");
		}

		internal bool ChartDynamicGroupEnd(bool column)
		{
			switch (this.m_currentTypeDecl.Parent.BaseTypeName)
			{
			case "ChartExprHost":
				if (column)
				{
					return this.TypeEnd(this.m_currentTypeDecl.Parent, "ColumnGroupingsHost");
				}
				return this.TypeEnd(this.m_currentTypeDecl.Parent, "RowGroupingsHost");
			case "ChartDynamicGroupExprHost":
				return this.TypeEnd(this.m_currentTypeDecl.Parent, "SubGroupHost");
			default:
				Global.Tracer.Assert(false);
				return false;
			}
		}

		internal void ChartHeadingLabel(ExpressionInfo expression)
		{
			this.ExpressionAdd("HeadingLabelExpr", expression);
		}

		internal void ChartDataPointStart()
		{
			this.TypeStart(this.CreateTypeName("DataPoint", ((NonRootTypeDecl)this.m_currentTypeDecl).DataPoints), "ChartDataPointExprHost");
		}

		internal int ChartDataPointEnd()
		{
			this.ExprIndexerCreate();
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_chartDataPointHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).DataPoints);
		}

		internal void ChartDataPointDataValue(ExpressionInfo expression)
		{
			this.IndexedExpressionAdd(expression);
		}

		internal void DataLabelValue(ExpressionInfo expression)
		{
			this.ExpressionAdd("DataLabelValueExpr", expression);
		}

		internal void DataLabelStyleStart()
		{
			this.StyleStart("DataLabelStyle");
		}

		internal void DataLabelStyleEnd()
		{
			this.StyleEnd("DataLabelStyleHost");
		}

		internal void DataPointStyleStart()
		{
			this.StyleStart("Style");
		}

		internal void DataPointStyleEnd()
		{
			this.StyleEnd("StyleHost");
		}

		internal void DataPointMarkerStyleStart()
		{
			this.StyleStart("DataPointMarkerStyle");
		}

		internal void DataPointMarkerStyleEnd()
		{
			this.StyleEnd("MarkerStyleHost");
		}

		internal void ChartTitleStart()
		{
			this.TypeStart("Title", "ChartTitleExprHost");
		}

		internal void ChartTitleEnd()
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, "TitleHost");
		}

		internal void ChartCaption(ExpressionInfo expression)
		{
			this.ExpressionAdd("CaptionExpr", expression);
		}

		internal void MajorGridLinesStyleStart()
		{
			this.StyleStart("MajorGridLinesStyle");
		}

		internal void MajorGridLinesStyleEnd()
		{
			this.StyleEnd("MajorGridLinesHost");
		}

		internal void MinorGridLinesStyleStart()
		{
			this.StyleStart("MinorGridLinesStyle");
		}

		internal void MinorGridLinesStyleEnd()
		{
			this.StyleEnd("MinorGridLinesHost");
		}

		internal void AxisMin(ExpressionInfo expression)
		{
			this.ExpressionAdd("AxisMinExpr", expression);
		}

		internal void AxisMax(ExpressionInfo expression)
		{
			this.ExpressionAdd("AxisMaxExpr", expression);
		}

		internal void AxisCrossAt(ExpressionInfo expression)
		{
			this.ExpressionAdd("AxisCrossAtExpr", expression);
		}

		internal void AxisMajorInterval(ExpressionInfo expression)
		{
			this.ExpressionAdd("AxisMajorIntervalExpr", expression);
		}

		internal void AxisMinorInterval(ExpressionInfo expression)
		{
			this.ExpressionAdd("AxisMinorIntervalExpr", expression);
		}

		internal void ChartStaticRowLabelsStart()
		{
			this.TypeStart("ChartStaticRowLabels", "IndexedExprHost");
		}

		internal void ChartStaticRowLabelsEnd()
		{
			this.ExprIndexerCreate();
			this.TypeEnd(this.m_currentTypeDecl.Parent, "StaticRowLabelsHost");
		}

		internal void ChartStaticColumnLabelsStart()
		{
			this.TypeStart("ChartStaticColumnLabels", "IndexedExprHost");
		}

		internal void ChartStaticColumnLabelsEnd()
		{
			this.ExprIndexerCreate();
			this.TypeEnd(this.m_currentTypeDecl.Parent, "StaticColumnLabelsHost");
		}

		internal void ChartStaticColumnRowLabel(ExpressionInfo expression)
		{
			this.IndexedExpressionAdd(expression);
		}

		internal void ChartStart(string name)
		{
			this.TypeStart(name, "ChartExprHost");
		}

		internal int ChartEnd()
		{
			return this.ReportItemEnd("m_chartHostsRemotable", ref this.m_rootTypeDecl.Charts);
		}

		internal void ChartCategoryAxisStart()
		{
			this.AxisStart("CategoryAxis");
		}

		internal void ChartCategoryAxisEnd()
		{
			this.AxisEnd("CategoryAxisHost");
		}

		internal void ChartValueAxisStart()
		{
			this.AxisStart("ValueAxis");
		}

		internal void ChartValueAxisEnd()
		{
			this.AxisEnd("ValueAxisHost");
		}

		internal void ChartLegendStart()
		{
			this.StyleStart("Legend");
		}

		internal void ChartLegendEnd()
		{
			this.StyleEnd("LegendHost");
		}

		internal void ChartPlotAreaStart()
		{
			this.StyleStart("PlotArea");
		}

		internal void ChartPlotAreaEnd()
		{
			this.StyleEnd("PlotAreaHost");
		}

		internal void TableGroupStart(string name)
		{
			this.TypeStart("TableGroup_" + name, "TableGroupExprHost");
		}

		internal bool TableGroupEnd()
		{
			switch (this.m_currentTypeDecl.Parent.BaseTypeName)
			{
			case "TableExprHost":
				return this.TypeEnd(this.m_currentTypeDecl.Parent, "TableGroupsHost");
			case "TableGroupExprHost":
				return this.TypeEnd(this.m_currentTypeDecl.Parent, "SubGroupHost");
			default:
				Global.Tracer.Assert(false);
				return false;
			}
		}

		internal void TableRowVisibilityHiddenExpressionsStart()
		{
			this.TypeStart("TableRowVisibilityHiddenExpressionsClass", "IndexedExprHost");
		}

		internal void TableRowVisibilityHiddenExpressionsEnd()
		{
			this.ExprIndexerCreate();
			this.TypeEnd(this.m_currentTypeDecl.Parent, "TableRowVisibilityHiddenExpressions");
		}

		internal void TableRowColVisibilityHiddenExpressionsExpr(ExpressionInfo expression)
		{
			this.IndexedExpressionAdd(expression);
		}

		internal void TableStart(string name)
		{
			this.TypeStart(name, "TableExprHost");
		}

		internal int TableEnd()
		{
			return this.ReportItemEnd("m_tableHostsRemotable", ref this.m_rootTypeDecl.Tables);
		}

		internal void TableColumnVisibilityHiddenExpressionsStart()
		{
			this.TypeStart("TableColumnVisibilityHiddenExpressions", "IndexedExprHost");
		}

		internal void TableColumnVisibilityHiddenExpressionsEnd()
		{
			this.ExprIndexerCreate();
			this.TypeEnd(this.m_currentTypeDecl.Parent, "TableColumnVisibilityHiddenExpressions");
		}

		internal void OWCChartStart(string name)
		{
			this.TypeStart(name, "OWCChartExprHost");
		}

		internal int OWCChartEnd()
		{
			return this.ReportItemEnd("m_OWCChartHostsRemotable", ref this.m_rootTypeDecl.OWCCharts);
		}

		internal void OWCChartColumnsStart()
		{
			this.TypeStart("OWCChartColumns", "IndexedExprHost");
		}

		internal void OWCChartColumnsEnd()
		{
			this.ExprIndexerCreate();
			this.TypeEnd(this.m_currentTypeDecl.Parent, "OWCChartColumnHosts");
		}

		internal void OWCChartColumnsValue(ExpressionInfo expression)
		{
			this.IndexedExpressionAdd(expression);
		}

		internal void DataValueStart()
		{
			this.TypeStart(this.CreateTypeName("DataValue", this.m_currentTypeDecl.DataValues), "DataValueExprHost");
		}

		internal int DataValueEnd(bool isCustomProperty)
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, isCustomProperty ? "m_customPropertyHostsRemotable" : "m_dataValueHostsRemotable", ref this.m_currentTypeDecl.Parent.DataValues);
		}

		internal void DataValueName(ExpressionInfo expression)
		{
			this.ExpressionAdd("DataValueNameExpr", expression);
		}

		internal void DataValueValue(ExpressionInfo expression)
		{
			this.ExpressionAdd("DataValueValueExpr", expression);
		}

		internal void CustomReportItemStart(string name)
		{
			this.TypeStart(name, "CustomReportItemExprHost");
		}

		internal int CustomReportItemEnd()
		{
			return this.ReportItemEnd("m_customReportItemHostsRemotable", ref this.m_rootTypeDecl.CustomReportItems);
		}

		internal void DataGroupingStart(bool column)
		{
			string template = "DataGrouping" + (column ? "Column" : "Row");
			this.TypeStart(this.CreateTypeName(template, ((NonRootTypeDecl)this.m_currentTypeDecl).DataGroupings), "DataGroupingExprHost");
		}

		internal int DataGroupingEnd(bool column)
		{
			Global.Tracer.Assert("CustomReportItemExprHost" == this.m_currentTypeDecl.Parent.BaseTypeName || "DataGroupingExprHost" == this.m_currentTypeDecl.Parent.BaseTypeName);
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_dataGroupingHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).DataGroupings);
		}

		internal void DataCellStart()
		{
			this.TypeStart(this.CreateTypeName("DataCell", ((NonRootTypeDecl)this.m_currentTypeDecl).DataCells), "DataCellExprHost");
		}

		internal int DataCellEnd()
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, "m_dataCellHostsRemotable", ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).DataCells);
		}

		private void TypeStart(string typeName, string baseType)
		{
			this.m_currentTypeDecl = new NonRootTypeDecl(typeName, baseType, this.m_currentTypeDecl, this.m_setCode);
		}

		private int TypeEnd(TypeDecl container, string name, ref CodeExpressionCollection initializers)
		{
			int result = -1;
			if (this.m_currentTypeDecl.HasExpressions)
			{
				result = container.NestedTypeColAdd(name, this.m_currentTypeDecl.BaseTypeName, ref initializers, this.m_currentTypeDecl.Type);
			}
			this.TypeEnd(container);
			return result;
		}

		private bool TypeEnd(TypeDecl container, string name)
		{
			bool hasExpressions = this.m_currentTypeDecl.HasExpressions;
			if (hasExpressions)
			{
				container.NestedTypeAdd(name, this.m_currentTypeDecl.Type);
			}
			this.TypeEnd(container);
			return hasExpressions;
		}

		private void TypeEnd(TypeDecl container)
		{
			Global.Tracer.Assert(this.m_currentTypeDecl.Parent != null && container != null, "(m_currentTypeDecl.Parent != null && container != null)");
			container.HasExpressions |= this.m_currentTypeDecl.HasExpressions;
			this.m_currentTypeDecl = this.m_currentTypeDecl.Parent;
		}

		private int ReportItemEnd(string name, ref CodeExpressionCollection initializers)
		{
			return this.TypeEnd((TypeDecl)this.m_rootTypeDecl, name, ref initializers);
		}

		private void ParameterStart()
		{
			this.TypeStart(this.CreateTypeName("Parameter", ((NonRootTypeDecl)this.m_currentTypeDecl).Parameters), "ParamExprHost");
		}

		private int ParameterEnd(string propName)
		{
			return this.TypeEnd(this.m_currentTypeDecl.Parent, propName, ref ((NonRootTypeDecl)this.m_currentTypeDecl.Parent).Parameters);
		}

		private void StyleStart(string typeName)
		{
			this.TypeStart(typeName, "StyleExprHost");
		}

		private void StyleEnd(string propName)
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, propName);
		}

		private void AxisStart(string typeName)
		{
			this.TypeStart(typeName, "AxisExprHost");
		}

		private void AxisEnd(string propName)
		{
			this.TypeEnd(this.m_currentTypeDecl.Parent, propName);
		}

		private void AggregateStart()
		{
			this.TypeStart(this.CreateTypeName("Aggregate", this.m_rootTypeDecl.Aggregates), "AggregateParamExprHost");
		}

		private int AggregateEnd()
		{
			return this.TypeEnd((TypeDecl)this.m_rootTypeDecl, "m_aggregateParamHostsRemotable", ref this.m_rootTypeDecl.Aggregates);
		}

		private string CreateTypeName(string template, CodeExpressionCollection initializers)
		{
			return template + ((initializers == null) ? "0" : initializers.Count.ToString(CultureInfo.InvariantCulture));
		}

		private void ExprIndexerCreate()
		{
			NonRootTypeDecl nonRootTypeDecl = (NonRootTypeDecl)this.m_currentTypeDecl;
			if (nonRootTypeDecl.IndexedExpressions != null)
			{
				Global.Tracer.Assert(nonRootTypeDecl.IndexedExpressions.Count > 0, "(currentTypeDecl.IndexedExpressions.Count > 0)");
				CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
				codeMemberProperty.Name = "Item";
				codeMemberProperty.Attributes = (MemberAttributes)24580;
				codeMemberProperty.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "index"));
				codeMemberProperty.Type = new CodeTypeReference(typeof(object));
				nonRootTypeDecl.Type.Members.Add(codeMemberProperty);
				int count = nonRootTypeDecl.IndexedExpressions.Count;
				if (count == 1)
				{
					codeMemberProperty.GetStatements.Add(nonRootTypeDecl.IndexedExpressions[0]);
				}
				else
				{
					CodeConditionStatement codeConditionStatement = new CodeConditionStatement();
					codeMemberProperty.GetStatements.Add(codeConditionStatement);
					int num = count - 1;
					int num2 = count - 2;
					for (int i = 0; i < num; i++)
					{
						codeConditionStatement.Condition = new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("index"), CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(i));
						codeConditionStatement.TrueStatements.Add(nonRootTypeDecl.IndexedExpressions[i]);
						if (i < num2)
						{
							CodeConditionStatement codeConditionStatement2 = new CodeConditionStatement();
							codeConditionStatement.FalseStatements.Add(codeConditionStatement2);
							codeConditionStatement = codeConditionStatement2;
						}
					}
					codeConditionStatement.FalseStatements.Add(nonRootTypeDecl.IndexedExpressions[num]);
				}
			}
		}

		private void IndexedExpressionAdd(ExpressionInfo expression)
		{
			if (expression.Type == ExpressionInfo.Types.Expression)
			{
				NonRootTypeDecl nonRootTypeDecl = (NonRootTypeDecl)this.m_currentTypeDecl;
				if (nonRootTypeDecl.IndexedExpressions == null)
				{
					nonRootTypeDecl.IndexedExpressions = new ReturnStatementList();
				}
				nonRootTypeDecl.HasExpressions = true;
				expression.ExprHostID = nonRootTypeDecl.IndexedExpressions.Add(this.CreateExprReturnStatement(expression));
			}
		}

		private void ExpressionAdd(string name, ExpressionInfo expression)
		{
			if (expression.Type == ExpressionInfo.Types.Expression)
			{
				CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
				codeMemberProperty.Name = name;
				codeMemberProperty.Type = new CodeTypeReference(typeof(object));
				codeMemberProperty.Attributes = (MemberAttributes)24580;
				codeMemberProperty.GetStatements.Add(this.CreateExprReturnStatement(expression));
				this.m_currentTypeDecl.Type.Members.Add(codeMemberProperty);
				this.m_currentTypeDecl.HasExpressions = true;
			}
		}

		private CodeMethodReturnStatement CreateExprReturnStatement(ExpressionInfo expression)
		{
			CodeMethodReturnStatement codeMethodReturnStatement = new CodeMethodReturnStatement(new CodeSnippetExpression(expression.TransformedExpression));
			codeMethodReturnStatement.LinePragma = new CodeLinePragma("Expr" + expression.CompileTimeID.ToString(CultureInfo.InvariantCulture) + "end", 0);
			return codeMethodReturnStatement;
		}
	}
}
