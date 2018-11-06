using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.DataExtensions;
using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;


namespace AspNetCore.ReportingServices.ReportPublishing
{
    internal sealed class ReportPublishing
    {
        private bool m_static;

        private bool m_interactive;

        private int m_idCounter;

        private int m_dataSetIndexCounter;

        private RmlValidatingReader m_reader;

        private CLSUniqueNameValidator m_reportItemNames;

        private CLSUniqueNameValidator m_reportSectionNames;

        private VariableNameValidator m_variableNames;

        private ScopeNameValidator m_scopeNames;

        private string m_description;

        private List<AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport> m_subReports;

        private UserLocationFlags m_reportLocationFlags = UserLocationFlags.ReportBody;

        private UserLocationFlags m_userReferenceLocation = UserLocationFlags.None;

        private bool m_hasExternalImages;

        private bool m_hasHyperlinks;

        private List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion> m_nestedDataRegions;

        private SortedList<double, Pair<double, int>> m_headerLevelSizeList;

        private double m_firstCumulativeHeaderSize;

        private AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSection m_currentReportSection;

        private DataSourceInfoCollection m_dataSources;

        private DataSetInfoCollection m_sharedDataSetReferences;

        private bool m_hasGrouping;

        private bool m_hasSorting;

        private bool m_requiresSortingPostGrouping;

        private bool m_hasUserSort;

        private bool m_hasGroupFilters;

        private bool m_hasSpecialRecursiveAggregates;

        private bool m_subReportMergeTransactions;

        private ExprHostCompiler m_reportCT;

        private bool m_hasImageStreams;

        private bool m_hasLabels;

        private bool m_hasBookmarks;

        private List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox> m_textBoxesWithUserSortTarget = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox>();

        private List<ICreateSubtotals> m_createSubtotalsDefs = new List<ICreateSubtotals>();

        private List<AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping> m_domainScopeGroups = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping>();

        private Holder<int> m_variableSequenceIdCounter = new Holder<int>();

        private Holder<int> m_textboxSequenceIdCounter = new Holder<int>();

        private bool m_hasFilters;

        private List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet> m_dataSets = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet>();

        private bool m_parametersNotUsedInQuery = true;

        private Hashtable m_usedInQueryInfos = new Hashtable();

        private Hashtable m_reportParamUserProfile = new Hashtable();

        private Hashtable m_dataSetQueryInfo = new Hashtable();

        private ArrayList m_dynamicParameters = new ArrayList();

        private CultureInfo m_reportLanguage;

        private bool m_hasUserSortPeerScopes;

        private Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.ISortFilterScope> m_reportScopes = new Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.ISortFilterScope>();

        private StringDictionary m_dataSourceNames = new StringDictionary();

        private int m_dataRegionCount;

        private List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItemCollection> m_reportItemCollectionList = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItemCollection>();

        private List<AspNetCore.ReportingServices.ReportIntermediateFormat.IAggregateHolder> m_aggregateHolderList = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.IAggregateHolder>();

        private List<AspNetCore.ReportingServices.ReportIntermediateFormat.IRunningValueHolder> m_runningValueHolderList = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.IRunningValueHolder>();

        private AspNetCore.ReportingServices.ReportIntermediateFormat.Report m_report;

        private ParametersGridLayout m_parametersLayout;

        private PublishingErrorContext m_errorContext;

        private PublishingContextBase m_publishingContext;

        private readonly ReportUpgradeStrategy m_reportUpgradeStrategy;

        private ScopeTree m_scopeTree;

        private Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes, Dictionary<string, Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>>> m_aggregateHashByType = new Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes, Dictionary<string, Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>>>();

        private Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes, Dictionary<string, AllowNullKeyDictionary<string, Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo>>>> m_runningValueHashByType = new Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes, Dictionary<string, AllowNullKeyDictionary<string, Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo>>>>();

        private bool m_wroteAggregateHeaderInformation;

        private DataSetCore m_dataSetCore;

        internal ReportPublishing(PublishingContextBase publishingContext, PublishingErrorContext errorContext)
        {
            this.m_publishingContext = publishingContext;
            this.m_errorContext = errorContext;
        }

        internal ReportPublishing(PublishingContextBase publishingContext, PublishingErrorContext errorContext, ReportUpgradeStrategy reportUpgradeStrategy)
        {
            this.m_publishingContext = publishingContext;
            this.m_errorContext = errorContext;
            this.m_reportUpgradeStrategy = reportUpgradeStrategy;
        }

        internal AspNetCore.ReportingServices.ReportIntermediateFormat.Report CreateProgressiveIntermediateFormat(Stream definitionStream, out string reportDescription, out ParameterInfoCollection parameters, out DataSourceInfoCollection dataSources)
        {
            DataSetInfoCollection dataSetInfoCollection = null;
            ArrayList arrayList = null;
            byte[] array = null;
            this.CheckForMissingDefinition(definitionStream);
            string text = default(string);
            UserLocationFlags userLocationFlags = default(UserLocationFlags);
            bool flag = default(bool);
            bool flag2 = default(bool);
            return this.InternalCreateIntermediateFormat(definitionStream, out reportDescription, out text, out parameters, out dataSources, out dataSetInfoCollection, out userLocationFlags, out arrayList, out flag, out flag2, out array);
        }

        internal ReportIntermediateFormat.Report CreateIntermediateFormat(byte[] definition, out string description, out string language, out ParameterInfoCollection parameters, out DataSourceInfoCollection dataSources, out DataSetInfoCollection sharedDataSetReferences, out UserLocationFlags userReferenceLocation, out ArrayList dataSetsName, out bool hasExternalImages, out bool hasHyperlinks, out byte[] dataSetsHash)
        {
            this.CheckForMissingDefinition(definition);
            Stream definitionStream = new MemoryStream(definition, false);

            return this.InternalCreateIntermediateFormat(definitionStream, out description, out language, out parameters, out dataSources, out sharedDataSetReferences, out userReferenceLocation, out dataSetsName, out hasExternalImages, out hasHyperlinks, out dataSetsHash);
        }

		private void CheckForMissingDefinition(object definition)
		{
			if (definition != null)
			{
				return;
			}
			this.m_errorContext.Register(ProcessingErrorCode.rsNotAReportDefinition, Severity.Error, ReportProcessing.ObjectType.Report, null, null);
			throw new ReportPublishingException(this.m_errorContext.Messages, ReportProcessingFlags.YukonEngine);
		}

		private ReportIntermediateFormat.Report InternalCreateIntermediateFormat(Stream definitionStream, out string description, out string language, out ParameterInfoCollection parameters, out DataSourceInfoCollection dataSources, out DataSetInfoCollection sharedDataSetReferences, out UserLocationFlags userReferenceLocation, out ArrayList dataSetsName, out bool hasExternalImages, out bool hasHyperlinks, out byte[] dataSetsHash)
		{
            try 
            {
                this.m_report = null;
                ReportProcessingCompatibilityVersion.TraceCompatibilityVersion(this.m_publishingContext.Configuration);
                this.Phase1(definitionStream, out description, out language, out dataSources, out sharedDataSetReferences, out hasExternalImages, out hasHyperlinks);
                dataSetsHash = this.CreateHashForCachedDataSets();
                this.Phase2();
                Dictionary<string, int> groupingExprCountAtScope = default(Dictionary<string, int>);
                this.Phase3(out parameters, out groupingExprCountAtScope);
                this.Phase4(groupingExprCountAtScope, out dataSetsName);
                userReferenceLocation = this.m_userReferenceLocation;
                if (this.m_errorContext.HasError)
                {
                    throw new ReportPublishingException(this.m_errorContext.Messages, ReportProcessingFlags.YukonEngine);
                }
                return this.m_report;
            }
            catch (Exception ex)
            {
                throw new ReportProcessingException(ex, null);
            }
            finally
            {
                this.m_report = null;
                this.m_errorContext = null;
            }
		}

		private byte[] CreateHashForCachedDataSets()
		{
			MemoryStream memoryStream = new MemoryStream();
            try
            {
                IntermediateFormatWriter intermediateFormatWriter = new IntermediateFormatWriter(memoryStream, 0);
                List<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo>();
                list.Add(new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(MemberName.Value, Token.Object));
                Declaration declaration = new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObject, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Null, list);
                intermediateFormatWriter.RegisterDeclaration(declaration);
                intermediateFormatWriter.NextMember();
                intermediateFormatWriter.WriteVariantOrPersistable(this.m_report.Language);
                intermediateFormatWriter.WriteVariantOrPersistable(this.m_report.Code);
                if (((IExpressionHostAssemblyHolder)this.m_report).CodeModules != null)
                {
                    foreach (string codeModule in ((IExpressionHostAssemblyHolder)this.m_report).CodeModules)
                    {
                        intermediateFormatWriter.WriteVariantOrPersistable(codeModule);
                    }
                }
                else
                {
                    intermediateFormatWriter.WriteNull();
                }
                if (((IExpressionHostAssemblyHolder)this.m_report).CodeClasses != null)
                {
                    foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.CodeClass codeClass in ((IExpressionHostAssemblyHolder)this.m_report).CodeClasses)
                    {
                        intermediateFormatWriter.WriteVariantOrPersistable(codeClass);
                    }
                }
                else
                {
                    intermediateFormatWriter.WriteNull();
                }
                if (this.m_report.DataSources != null)
                {
                    foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource dataSource in this.m_report.DataSources)
                    {
                        intermediateFormatWriter.WriteVariantOrPersistable(dataSource.Name);
                        intermediateFormatWriter.WriteVariantOrPersistable(dataSource.Transaction);
                        intermediateFormatWriter.WriteVariantOrPersistable(dataSource.Type);
                        intermediateFormatWriter.WriteVariantOrPersistable(dataSource.ConnectStringExpression);
                        intermediateFormatWriter.WriteVariantOrPersistable(dataSource.IntegratedSecurity);
                        intermediateFormatWriter.WriteVariantOrPersistable(dataSource.Prompt);
                        intermediateFormatWriter.WriteVariantOrPersistable(dataSource.DataSourceReference);
                    }
                }
                else
                {
                    intermediateFormatWriter.WriteNull();
                }
                if (this.m_dataSets != null)
                {
                    foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet in this.m_dataSets)
                    {
                        intermediateFormatWriter.WriteVariantOrPersistable(dataSet.Name);
                        intermediateFormatWriter.WriteVariantOrPersistable(dataSet.Query);
                        if (dataSet.Query != null)
                        {
                            intermediateFormatWriter.WriteVariantOrPersistable(dataSet.Query.DataSourceName);
                        }
                        if (dataSet.Fields != null)
                        {
                            foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.Field field in dataSet.Fields)
                            {
                                intermediateFormatWriter.WriteVariantOrPersistable(field);
                            }
                        }
                        else
                        {
                            intermediateFormatWriter.WriteNull();
                        }
                    }
                }
                else
                {
                    intermediateFormatWriter.WriteNull();
                }
                memoryStream.Flush();
                memoryStream.Seek(0L, SeekOrigin.Begin);
                using (HashAlgorithm hashAlgorithm = SHA1.Create())
                {
                    hashAlgorithm.ComputeHash(memoryStream);
                    return hashAlgorithm.Hash;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (memoryStream != null)
                {
                    memoryStream.Close();
                    memoryStream = null;
                }
            }
		}

		private void RegisterDataRegion(AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion)
		{
			this.m_dataRegionCount++;
			this.m_aggregateHolderList.Add(dataRegion);
			this.m_runningValueHolderList.Add(dataRegion);
		}

		private int GenerateID()
		{
			return ++this.m_idCounter;
		}

		private int GenerateVariableSequenceID()
		{
			return this.m_variableSequenceIdCounter.Value++;
		}

		private int GenerateTextboxSequenceID()
		{
			return this.m_textboxSequenceIdCounter.Value++;
		}

		private void Phase1(Stream definitionStream, out string description, out string language, out DataSourceInfoCollection dataSources, out DataSetInfoCollection sharedDataSetReferences, out bool hasExternalImages, out bool hasHyperlinks)
		{
            try
            {
                Global.Tracer.Assert(this.m_reportUpgradeStrategy != null, "There is no Upgrade Strategy for this stream.");
                Stream stream = this.m_reportUpgradeStrategy.Upgrade(definitionStream);
                Pair<string, Stream> pair = default(Pair<string, Stream>);
                List<Pair<string, Stream>> list = new List<Pair<string, Stream>>();
                pair = this.GetRDLNamespaceSchemaStreamPair("http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition", "AspNetCore.ReportingServices.ReportProcessing.ReportPublishing.ReportDefinition2016.xsd");
                list.Add(pair);
                pair = this.GetRDLNamespaceSchemaStreamPair("http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition", "AspNetCore.ReportingServices.ReportProcessing.ReportPublishing.ReportDefinition2010.xsd");
                list.Add(pair);
                pair = this.GetRDLNamespaceSchemaStreamPair("http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition", "AspNetCore.ReportingServices.ReportProcessing.ReportPublishing.ReportDefinition2011.xsd");
                list.Add(pair);
                pair = this.GetRDLNamespaceSchemaStreamPair("http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition", "AspNetCore.ReportingServices.ReportProcessing.ReportPublishing.ReportDefinition2012.xsd");
                list.Add(pair);
                pair = this.GetRDLNamespaceSchemaStreamPair("http://schemas.microsoft.com/sqlserver/reporting/2013/01/reportdefinition", "AspNetCore.ReportingServices.ReportProcessing.ReportPublishing.ReportDefinition2013.xsd");
                list.Add(pair);
                pair = this.GetRDLNamespaceSchemaStreamPair("http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition/defaultfontfamily", "AspNetCore.ReportingServices.ReportProcessing.ReportPublishing.DefaultFontFamily.xsd");
                list.Add(pair);
                this.m_reader = new RmlValidatingReader(stream, list, this.m_errorContext, (RmlValidatingReader.ItemType)(this.m_publishingContext.IsRdlx ? 1 : 0));
                this.m_reportItemNames = new CLSUniqueNameValidator(ProcessingErrorCode.rsInvalidNameNotCLSCompliant, ProcessingErrorCode.rsDuplicateReportItemName, ProcessingErrorCode.rsInvalidNameLength);
                this.m_reportSectionNames = new CLSUniqueNameValidator(ProcessingErrorCode.rsInvalidNameNotCLSCompliant, ProcessingErrorCode.rsDuplicateReportSectionName, ProcessingErrorCode.rsInvalidNameLength);
                this.m_variableNames = new VariableNameValidator();
                this.m_scopeNames = new ScopeNameValidator();
                this.m_dataSources = new DataSourceInfoCollection();
                sharedDataSetReferences = null;
                this.m_sharedDataSetReferences = new DataSetInfoCollection();
                this.m_subReports = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport>();
                while (this.m_reader.Read())
                {
                    if (XmlNodeType.Element == this.m_reader.NodeType && "Report" == this.m_reader.LocalName)
                    {
                        this.m_reportCT = new ExprHostCompiler(new AspNetCore.ReportingServices.RdlExpressions.VBExpressionParser(this.m_errorContext), this.m_errorContext);
                        this.ReadReport(this.m_publishingContext.DataProtection);
                    }
                }
                if (this.m_report == null)
                {
                    this.m_errorContext.Register(ProcessingErrorCode.rsNotACurrentReportDefinition, Severity.Error, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Report, null, "Namespace", "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition");
                    throw new ReportProcessingException(this.m_errorContext.Messages);
                }
            }
            catch (XmlSchemaException e)
            {
                this.CreateInvalidReportDefinitionException(e);
            }
            catch (XmlException e2)
            {
                this.CreateInvalidReportDefinitionException(e2);
            }
            catch (ArgumentException e3)
            {
                this.CreateInvalidReportDefinitionException(e3);
            }
            catch (IndexOutOfRangeException e4)
            {
                this.CreateInvalidReportDefinitionException(e4);
            }
            catch (FormatException e5)
            {
                this.CreateInvalidReportDefinitionException(e5);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (this.m_reader != null)
                {
                    this.m_reader.Close();
                    this.m_reader = null;
                }
                description = this.m_description;
                language = null;
                if (this.m_reportLanguage != null)
                {
                    language = this.m_reportLanguage.Name;
                }
                dataSources = this.m_dataSources;
                sharedDataSetReferences = this.m_sharedDataSetReferences;
                hasExternalImages = this.m_hasExternalImages;
                hasHyperlinks = this.m_hasHyperlinks;
                this.m_description = null;
                this.m_dataSources = null;
            }
		}

		private Pair<string, Stream> GetRDLNamespaceSchemaStreamPair(string validationNamespace, string xsdResource)
		{
			Stream stream = null;
			stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(xsdResource);
			Global.Tracer.Assert(stream != null, "(schemaStream != null)");
			return new Pair<string, Stream>(validationNamespace, stream);
		}

		private void CreateInvalidReportDefinitionException(Exception e)
		{
			this.m_errorContext.Register(ProcessingErrorCode.rsInvalidReportDefinition, Severity.Error, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Report, null, null, e.Message);
			throw new ReportProcessingException(this.m_errorContext.Messages);
		}

		private void ReadReport(IDataProtection dataProtection)
		{
			ReportIntermediateFormat.Report report = new ReportIntermediateFormat.Report(this.GenerateID(), this.GenerateID());
			report.Name = "Report";
			int maxExpressionLength = -1;
			if (this.m_publishingContext.IsRdlSandboxingEnabled)
			{
				maxExpressionLength = this.m_publishingContext.Configuration.RdlSandboxing.MaxExpressionLength;
			}
			this.m_report = report;
			PublishingContextStruct context = new PublishingContextStruct(LocationFlags.None, report.ObjectType, maxExpressionLength, this.m_errorContext);
			ReportIntermediateFormat.ExpressionInfo expressionInfo = null;
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "Description":
						this.m_description = this.m_reader.ReadString();
						break;
					case "Author":
						report.Author = this.m_reader.ReadString();
						break;
					case "AutoRefresh":
						report.AutoRefreshExpression = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
						break;
					case "DataSources":
					{
						List<ReportIntermediateFormat.DataSource> list = this.ReadDataSources(context, dataProtection);
						if (report.DataSources == null)
						{
							report.DataSources = list;
						}
						else
						{
							report.DataSources.AddRange(list);
						}
						break;
					}
					case "DataSets":
						this.ReadDataSets(context);
						break;
					case "ReportParameters":
						report.Parameters = this.ReadReportParameters(context);
						break;
					case "ReportParametersLayout":
						this.m_parametersLayout = this.ReadReportParametersLayout(context, report.Parameters);
						break;
					case "CustomProperties":
						report.CustomProperties = this.ReadCustomProperties(context);
						break;
					case "Code":
					{
						string code = this.m_reader.ReadString();
						if (this.m_publishingContext.IsRdlSandboxingEnabled)
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsSandboxingCustomCodeNotAllowed, Severity.Error, context.ObjectType, context.ObjectName, "Code");
						}
						else if (this.m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.Report_Code))
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						}
						else
						{
							report.Code = code;
							this.m_reportCT.Builder.SetCustomCode();
						}
						break;
					}
					case "EmbeddedImages":
						report.EmbeddedImages = this.ReadEmbeddedImages(context);
						break;
					case "Language":
						expressionInfo = (report.Language = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.ReportLanguage, DataType.String, context));
						break;
					case "CodeModules":
					{
						List<string> codeModules = this.ReadCodeModules(context);
						if (this.m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.Report_CodeModules))
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						}
						else
						{
							((IExpressionHostAssemblyHolder)report).CodeModules = codeModules;
						}
						break;
					}
					case "Classes":
					{
						List<ReportIntermediateFormat.CodeClass> codeClasses = this.ReadClasses(context);
						if (this.m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.Report_Classes))
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						}
						else
						{
							((IExpressionHostAssemblyHolder)report).CodeClasses = codeClasses;
						}
						break;
					}
					case "DataTransform":
						report.DataTransform = this.m_reader.ReadString();
						break;
					case "DataSchema":
						report.DataSchema = this.m_reader.ReadString();
						break;
					case "DataElementName":
						report.DataElementName = this.m_reader.ReadString();
						break;
					case "DataElementStyle":
						report.DataElementStyleAttribute = this.ReadDataElementStyle();
						break;
					case "Variables":
						report.Variables = this.ReadVariables(context, false, null);
						break;
					case "DeferVariableEvaluation":
						report.DeferVariableEvaluation = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						break;
					case "ConsumeContainerWhitespace":
						report.ConsumeContainerWhitespace = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						break;
					case "ReportSections":
						report.ReportSections = this.ReadReportSections(context, report);
						break;
					case "InitialPageName":
					{
						bool flag2 = default(bool);
						report.InitialPageName = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out flag2);
						break;
					}
					case "DefaultFontFamily":
					{
						string text = this.m_reader.ReadString();
						if (this.m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.DefaultFontFamily))
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						}
						else if (!text.IsNullOrWhiteSpace())
						{
							report.DefaultFontFamily = text;
						}
						break;
					}
					}
					break;
				case XmlNodeType.EndElement:
					if ("Report" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (report.Parameters != null && this.m_parametersLayout != null)
			{
				ReportParametersGridLayoutValidator.Validate(report.Parameters, this.m_parametersLayout, this.m_errorContext);
			}
			if (expressionInfo == null)
			{
				this.m_reportLanguage = Localization.DefaultReportServerSpecificCulture;
			}
			else if (AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == expressionInfo.Type)
			{
				PublishingValidator.ValidateSpecificLanguage(expressionInfo, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Report, (string)null, "Language", (ErrorContext)context.ErrorContext, out this.m_reportLanguage);
			}
			if (this.m_interactive)
			{
				report.ShowHideType = AspNetCore.ReportingServices.ReportIntermediateFormat.Report.ShowHideTypes.Interactive;
			}
			else if (this.m_static)
			{
				report.ShowHideType = AspNetCore.ReportingServices.ReportIntermediateFormat.Report.ShowHideTypes.Static;
			}
			else
			{
				report.ShowHideType = AspNetCore.ReportingServices.ReportIntermediateFormat.Report.ShowHideTypes.None;
			}
			report.SubReports = this.m_subReports;
			report.LastID = this.m_idCounter;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSection> ReadReportSections(PublishingContextStruct context, AspNetCore.ReportingServices.ReportIntermediateFormat.Report report)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSection> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSection>();
			bool flag = false;
			if (!this.m_reader.IsEmptyElement)
			{
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "ReportSection")
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSection item = this.ReadReportSection(context, report, list.Count);
							list.Add(item);
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "ReportSections")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSection ReadReportSection(PublishingContextStruct context, AspNetCore.ReportingServices.ReportIntermediateFormat.Report report, int sectionIndex)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSection reportSection = new AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSection(sectionIndex, report, this.GenerateID(), this.GenerateID());
			context.ObjectType = AspNetCore.ReportingServices.ReportProcessing.ObjectType.ReportSection;
			string attributeLocalName = this.m_reader.GetAttributeLocalName("Name");
			if (attributeLocalName != null)
			{
				if (this.m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.ReportSectionName))
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlAttribute, Severity.Error, context.ObjectType, attributeLocalName, "Name");
				}
				reportSection.Name = attributeLocalName;
			}
			else
			{
				reportSection.Name = "ReportSection" + sectionIndex.ToString(CultureInfo.InvariantCulture);
			}
			context.ObjectName = reportSection.Name;
			this.m_reportSectionNames.Validate(context.ObjectType, context.ObjectName, context.ErrorContext);
			this.m_reportItemCollectionList.Add(reportSection.ReportItems);
			this.m_currentReportSection = reportSection;
			bool flag = false;
			if (!this.m_reader.IsEmptyElement)
			{
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Body":
							this.ReadBody(reportSection, context);
							break;
						case "Width":
							reportSection.Width = this.ReadSize();
							break;
						case "Page":
							reportSection.Page = this.ReadPage(context, reportSection, sectionIndex);
							break;
						case "DataElementName":
							reportSection.DataElementName = this.m_reader.ReadString();
							break;
						case "DataElementOutput":
							reportSection.DataElementOutput = this.ReadDataElementOutput();
							break;
						case "LayoutDirection":
							reportSection.LayoutDirection = this.ReadLayoutDirection();
							if (reportSection.LayoutDirection && this.m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.ReportSection_LayoutDirection))
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, attributeLocalName, "LayoutDirection");
							}
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "ReportSection")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return reportSection;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Page ReadPage(PublishingContextStruct context, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSection section, int sectionNum)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.Page page = new AspNetCore.ReportingServices.ReportIntermediateFormat.Page(this.GenerateID());
			this.m_aggregateHolderList.Add(page);
			bool flag = false;
			if (!this.m_reader.IsEmptyElement)
			{
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "PageHeader":
							page.PageHeader = this.ReadPageSection(true, section, context);
							if (this.m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.PageHeaderFooter))
							{
								page.PageHeader = null;
								context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, "PageHeader");
							}
							break;
						case "PageFooter":
							page.PageFooter = this.ReadPageSection(false, section, context);
							if (this.m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.PageHeaderFooter))
							{
								page.PageFooter = null;
								context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, "PageFooter");
							}
							break;
						case "PageHeight":
						{
							string pageHeight = this.ReadSize();
							if (sectionNum == 0)
							{
								page.PageHeight = pageHeight;
							}
							else
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsPagePropertyInSubsequentReportSection, Severity.Warning, context.ObjectType, context.ObjectName, "PageHeight");
							}
							break;
						}
						case "PageWidth":
						{
							string pageWidth = this.ReadSize();
							if (sectionNum == 0)
							{
								page.PageWidth = pageWidth;
							}
							else
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsPagePropertyInSubsequentReportSection, Severity.Warning, context.ObjectType, context.ObjectName, "PageWidth");
							}
							break;
						}
						case "InteractiveHeight":
						{
							string interactiveHeight = this.ReadSize();
							if (sectionNum == 0)
							{
								page.InteractiveHeight = interactiveHeight;
							}
							else
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsPagePropertyInSubsequentReportSection, Severity.Warning, context.ObjectType, context.ObjectName, "InteractiveHeight");
							}
							break;
						}
						case "InteractiveWidth":
						{
							string interactiveWidth = this.ReadSize();
							if (sectionNum == 0)
							{
								page.InteractiveWidth = interactiveWidth;
							}
							else
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsPagePropertyInSubsequentReportSection, Severity.Warning, context.ObjectType, context.ObjectName, "InteractiveWidth");
							}
							break;
						}
						case "LeftMargin":
						{
							string leftMargin = this.ReadSize();
							if (sectionNum == 0)
							{
								page.LeftMargin = leftMargin;
							}
							else
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsPagePropertyInSubsequentReportSection, Severity.Warning, context.ObjectType, context.ObjectName, "LeftMargin");
							}
							break;
						}
						case "RightMargin":
						{
							string rightMargin = this.ReadSize();
							if (sectionNum == 0)
							{
								page.RightMargin = rightMargin;
							}
							else
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsPagePropertyInSubsequentReportSection, Severity.Warning, context.ObjectType, context.ObjectName, "RightMargin");
							}
							break;
						}
						case "TopMargin":
						{
							string topMargin = this.ReadSize();
							if (sectionNum == 0)
							{
								page.TopMargin = topMargin;
							}
							else
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsPagePropertyInSubsequentReportSection, Severity.Warning, context.ObjectType, context.ObjectName, "TopMargin");
							}
							break;
						}
						case "BottomMargin":
						{
							string bottomMargin = this.ReadSize();
							if (sectionNum == 0)
							{
								page.BottomMargin = bottomMargin;
							}
							else
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsPagePropertyInSubsequentReportSection, Severity.Warning, context.ObjectType, context.ObjectName, "BottomMargin");
							}
							break;
						}
						case "Columns":
						{
							int columns = this.m_reader.ReadInteger(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
							if (PublishingValidator.ValidateColumns(columns, context.ObjectType, context.ObjectName, "Columns", context.ErrorContext, sectionNum))
							{
								page.Columns = columns;
							}
							break;
						}
						case "ColumnSpacing":
							page.ColumnSpacing = this.ReadSize();
							break;
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Body, false);
							if (sectionNum == 0)
							{
								page.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, context.ErrorContext);
							}
							else if (styleInformation.Attributes.Count > 0)
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsPagePropertyInSubsequentReportSection, Severity.Warning, context.ObjectType, context.ObjectName, "Style");
							}
							break;
						}
						}
						break;
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "Page")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return page;
		}

		private List<string> ReadCodeModules(PublishingContextStruct context)
		{
			List<string> list = new List<string>();
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "CodeModule")
					{
						list.Add(this.m_reader.ReadString());
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("CodeModules" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return list;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.CodeClass> ReadClasses(PublishingContextStruct context)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.CodeClass> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.CodeClass>();
			CLSUniqueNameValidator instanceNameValidator = new CLSUniqueNameValidator(ProcessingErrorCode.rsInvalidNameNotCLSCompliant, ProcessingErrorCode.rsDuplicateClassInstanceName, ProcessingErrorCode.rsInvalidNameLength);
			context.ObjectType = AspNetCore.ReportingServices.ReportProcessing.ObjectType.CodeClass;
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "Class")
					{
						this.ReadClass(list, instanceNameValidator, context);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("Classes" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			this.m_reportCT.Builder.SetCustomCode();
			return list;
		}

		private void ReadClass(List<AspNetCore.ReportingServices.ReportIntermediateFormat.CodeClass> codeClasses, CLSUniqueNameValidator instanceNameValidator, PublishingContextStruct context)
		{
			bool flag = false;
			AspNetCore.ReportingServices.ReportIntermediateFormat.CodeClass item = default(AspNetCore.ReportingServices.ReportIntermediateFormat.CodeClass);
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "ClassName":
						item.ClassName = this.m_reader.ReadString();
						break;
					case "InstanceName":
						item.InstanceName = this.m_reader.ReadString();
						if (!instanceNameValidator.Validate(context.ObjectType, item.InstanceName, context.ErrorContext))
						{
							item.InstanceName = null;
						}
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Class" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			codeClasses.Add(item);
		}

		private void ReadBody(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSection section, PublishingContextStruct context)
		{
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "ReportItems":
							this.ReadReportItems(null, section, section.ReportItems, context, null);
							break;
						case "Height":
							section.Height = this.ReadSize();
							break;
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Body, false);
							section.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, context.ErrorContext);
							break;
						}
						}
						break;
					case XmlNodeType.EndElement:
						if ("Body" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.PageSection ReadPageSection(bool isHeader, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSection section, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.PageSection pageSection = new AspNetCore.ReportingServices.ReportIntermediateFormat.PageSection(isHeader, this.GenerateID(), this.GenerateID(), section);
			pageSection.Name = section.Name + "." + (isHeader ? "PageHeader" : "PageFooter");
			context.Location |= LocationFlags.InPageSection;
			context.ObjectType = pageSection.ObjectType;
			context.ObjectName = pageSection.Name;
			this.m_report.HasHeadersOrFooters = true;
			this.m_reportItemCollectionList.Add(pageSection.ReportItems);
			this.m_reportLocationFlags = UserLocationFlags.ReportPageSection;
			this.m_reportCT.ResetPageSectionRefersFlags();
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "Height":
						pageSection.Height = this.ReadSize();
						break;
					case "PrintOnFirstPage":
						pageSection.PrintOnFirstPage = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						break;
					case "PrintOnLastPage":
						pageSection.PrintOnLastPage = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						break;
					case "PrintBetweenSections":
						pageSection.PrintBetweenSections = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						break;
					case "ReportItems":
						this.ReadReportItems((string)null, (AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem)pageSection, pageSection.ReportItems, context, (List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox>)null, out flag);
						break;
					case "Style":
					{
						StyleInformation styleInformation = this.ReadStyle(context, out flag2);
						styleInformation.Filter(StyleOwnerType.PageSection, false);
						pageSection.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, context.ErrorContext);
						break;
					}
					}
					break;
				case XmlNodeType.EndElement:
					if (isHeader)
					{
						if ("PageHeader" == this.m_reader.LocalName)
						{
							flag3 = true;
						}
					}
					else if ("PageFooter" == this.m_reader.LocalName)
					{
						flag3 = true;
					}
					break;
				}
			}
			while (!flag3);
			section.NeedsReportItemsOnPage |= this.m_reportCT.PageSectionRefersToReportItems;
			section.NeedsOverallTotalPages |= this.m_reportCT.PageSectionRefersToOverallTotalPages;
			section.NeedsPageBreakTotalPages |= this.m_reportCT.PageSectionRefersToTotalPages;
			this.m_reportLocationFlags = UserLocationFlags.ReportBody;
			return pageSection;
		}

		private void ReadReportItems(string propertyName, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem parent, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItemCollection parentCollection, PublishingContextStruct context, List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget, out bool computed)
		{
			computed = false;
			int num = 0;
			bool isParentTablix = parent is AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix;
			bool flag = false;
			do
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem reportItem = null;
				AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem reportItem2 = null;
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "Tablix":
						num++;
						reportItem = this.ReadTablix(parent, context);
						break;
					case "Line":
						num++;
						reportItem = this.ReadLine(parent, context);
						break;
					case "Rectangle":
						num++;
						reportItem = this.ReadRectangle(parent, context, textBoxesWithDefaultSortTarget);
						break;
					case "CustomReportItem":
						num++;
						reportItem = this.ReadCustomReportItem(parent, context, textBoxesWithDefaultSortTarget, out reportItem2);
						Global.Tracer.Assert(reportItem2 != null);
						break;
					case "Textbox":
						num++;
						reportItem = this.ReadTextbox(parent, context, textBoxesWithDefaultSortTarget);
						break;
					case "Image":
						num++;
						reportItem = this.ReadImage(parent, context);
						break;
					case "Subreport":
						num++;
						reportItem = this.ReadSubreport(parent, context);
						break;
					case "Chart":
						num++;
						reportItem = this.ReadChart(parent, context);
						break;
					case "GaugePanel":
						num++;
						reportItem = this.ReadGaugePanel(parent, context);
						break;
					case "Map":
						num++;
						reportItem = this.ReadMap(parent, context);
						break;
					}
					if (reportItem != null)
					{
						computed |= this.AddReportItemToParentCollection(reportItem, parentCollection, isParentTablix);
						if (reportItem2 != null)
						{
							computed |= this.AddReportItemToParentCollection(reportItem2, parentCollection, isParentTablix);
						}
					}
					break;
				case XmlNodeType.EndElement:
					if ("ReportItems" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private bool AddReportItemToParentCollection(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem reportItem, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItemCollection parentCollection, bool isParentTablix)
		{
			parentCollection.AddReportItem(reportItem);
			return reportItem.Computed;
		}

		private void ReadReportItems(string propertyName, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem parent, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItemCollection parentCollection, PublishingContextStruct context, List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
		{
			bool flag = default(bool);
			this.ReadReportItems(propertyName, parent, parentCollection, context, textBoxesWithDefaultSortTarget, out flag);
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadPageNameExpression(PublishingContextStruct context)
		{
			bool flag = default(bool);
			return this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out flag);
		}

		private void ReadPageBreak(IPageBreakOwner pageBreakOwner, PublishingContextStruct context)
		{
			bool flag = false;
			if (!this.m_reader.IsEmptyElement)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.PageBreak pageBreak2 = pageBreakOwner.PageBreak = new AspNetCore.ReportingServices.ReportIntermediateFormat.PageBreak();
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						bool flag2 = default(bool);
						switch (this.m_reader.LocalName)
						{
						case "BreakLocation":
							pageBreak2.BreakLocation = this.ReadPageBreakLocation();
							break;
						case "ResetPageNumber":
							pageBreak2.ResetPageNumber = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context, out flag2);
							break;
						case "Disabled":
							pageBreak2.Disabled = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context, out flag2);
							break;
						}
						break;
					}
					case XmlNodeType.EndElement:
						flag = (this.m_reader.LocalName == "PageBreak");
						break;
					}
				}
				while (!flag);
			}
		}

		private void SetSortTargetForTextBoxes(List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox> textBoxes, AspNetCore.ReportingServices.ReportIntermediateFormat.ISortFilterScope target)
		{
			if (textBoxes != null)
			{
				for (int i = 0; i < textBoxes.Count; i++)
				{
					textBoxes[i].UserSort.SetDefaultSortTarget(target);
				}
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport ReadSubreport(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem parent, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport subReport = new AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport(this.GenerateID(), parent);
			subReport.Name = this.m_reader.GetAttribute("Name");
			subReport.SetContainingSection(this.m_currentReportSection);
			context.ObjectType = subReport.ObjectType;
			context.ObjectName = subReport.Name;
			this.m_reportItemNames.Validate(context.ObjectType, context.ObjectName, context.ErrorContext);
			bool flag = true;
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsDataRegionInPageSection, Severity.Error, context.ObjectType, context.ObjectName, null);
				flag = false;
			}
			bool flag2 = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "Style":
					{
						StyleInformation styleInformation = this.ReadStyle(context);
						styleInformation.Filter(StyleOwnerType.SubReport, false);
						subReport.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, context.ErrorContext);
						break;
					}
					case "Top":
						subReport.Top = this.ReadSize();
						break;
					case "Left":
						subReport.Left = this.ReadSize();
						break;
					case "Height":
						subReport.Height = this.ReadSize();
						break;
					case "Width":
						subReport.Width = this.ReadSize();
						break;
					case "ZIndex":
						subReport.ZIndex = this.m_reader.ReadInteger(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						break;
					case "Visibility":
						subReport.Visibility = this.ReadVisibility(context);
						break;
					case "ToolTip":
						subReport.ToolTip = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
						break;
					case "DocumentMapLabel":
						subReport.DocumentMapLabel = this.ReadDocumentMapLabelExpression(this.m_reader.LocalName, context);
						break;
					case "Bookmark":
						subReport.Bookmark = this.ReadBookmarkExpression(this.m_reader.LocalName, context);
						break;
					case "CustomProperties":
						subReport.CustomProperties = this.ReadCustomProperties(context);
						break;
					case "ReportName":
						subReport.ReportName = PublishingValidator.ValidateReportName(this.m_publishingContext.CatalogContext, this.m_reader.ReadString(), context.ObjectType, context.ObjectName, "ReportName", context.ErrorContext);
						break;
					case "Parameters":
						subReport.Parameters = this.ReadParameters(context, true);
						break;
					case "NoRowsMessage":
						subReport.NoRowsMessage = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
						break;
					case "MergeTransactions":
						subReport.MergeTransactions = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						if (subReport.MergeTransactions)
						{
							this.m_subReportMergeTransactions = true;
						}
						break;
					case "DataElementName":
						subReport.DataElementName = this.m_reader.ReadString();
						break;
					case "DataElementOutput":
						subReport.DataElementOutput = this.ReadDataElementOutput();
						break;
					case "KeepTogether":
						subReport.KeepTogether = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						break;
					case "OmitBorderOnPageBreak":
						subReport.OmitBorderOnPageBreak = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Subreport" == this.m_reader.LocalName)
					{
						flag2 = true;
					}
					break;
				}
			}
			while (!flag2);
			if (this.m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.SubReports))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, "Subreport");
			}
			subReport.Computed = true;
			if (flag)
			{
				this.m_subReports.Add(subReport);
				this.m_parametersNotUsedInQuery = false;
				return subReport;
			}
			return null;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.DataValueList ReadCustomProperties(PublishingContextStruct context)
		{
			bool flag = default(bool);
			return this.ReadCustomProperties(context, out flag);
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.DataValueList ReadCustomProperties(PublishingContextStruct context, out bool computed)
		{
			bool flag = false;
			computed = false;
			int num = 0;
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataValueList dataValueList = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataValueList();
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "CustomProperty")
					{
						dataValueList.Add(this.ReadDataValue(true, true, ++num, ref computed, context));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("CustomProperties" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return dataValueList;
		}

		private DataType ReadDataTypeAttribute()
		{
			bool flag = default(bool);
			return this.ReadDataTypeAttribute(out flag);
		}

		private DataType ReadDataTypeAttribute(out bool hadExplicitDataType)
		{
			if (this.m_reader.HasAttributes)
			{
				string attribute = this.m_reader.GetAttribute("DataType");
				if (attribute != null)
				{
					hadExplicitDataType = true;
					return (DataType)Enum.Parse(typeof(DataType), attribute, false);
				}
			}
			hadExplicitDataType = false;
			return DataType.String;
		}

		private PageBreakLocation ReadPageBreakLocation()
		{
			string value = this.m_reader.ReadString();
			return (PageBreakLocation)Enum.Parse(typeof(PageBreakLocation), value, false);
		}

		private bool ReadDataElementStyle()
		{
			string strOne = this.m_reader.ReadString();
			return Validator.CompareWithInvariantCulture(strOne, "Attribute");
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem.DataElementStyles ReadDataElementStyleRDL()
		{
			string value = this.m_reader.ReadString();
			return (AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem.DataElementStyles)Enum.Parse(typeof(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem.DataElementStyles), value, false);
		}

		private DataElementOutputTypes ReadDataElementOutput()
		{
			string value = this.m_reader.ReadString();
			return (DataElementOutputTypes)Enum.Parse(typeof(DataElementOutputTypes), value, false);
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Action ReadActionInfo(PublishingContextStruct context, StyleOwnerType styleOwnerType, out bool computed)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.Action action = new AspNetCore.ReportingServices.ReportIntermediateFormat.Action();
			bool flag = false;
			bool flag2 = false;
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag3 = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context, out flag);
							styleInformation.Filter(styleOwnerType, false);
							action.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, context.ErrorContext);
							break;
						}
						case "Actions":
							this.ReadActionItemList(action, context, out flag2);
							if (action.ActionItems.Count > 1)
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsInvalidActionsCount, Severity.Error, context.ObjectType, context.ObjectName, "ActionInfo", "Action");
							}
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ActionInfo" == this.m_reader.LocalName)
						{
							flag3 = true;
						}
						break;
					}
				}
				while (!flag3);
			}
			computed = (flag || flag2);
			return action;
		}

		private void ReadActionItemList(AspNetCore.ReportingServices.ReportIntermediateFormat.Action actionInfo, PublishingContextStruct context, out bool computed)
		{
			computed = false;
			int num = -1;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "Action")
					{
						bool flag4 = default(bool);
						actionInfo.ActionItems.Add(this.ReadActionItem(context, out flag2, ref num, ref flag3, out flag4));
						actionInfo.TrackFieldsUsedInValueExpression |= flag4;
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("Actions" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			num++;
			computed = (num > 0);
			if (flag3 && actionInfo.ActionItems.Count > 1)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidActionLabel, Severity.Error, context.ObjectType, context.ObjectName, "Actions");
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ActionItem ReadActionItem(PublishingContextStruct context, out bool computed, ref int computedIndex, ref bool missingLabel, out bool hasDrillthroughParameter)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ActionItem actionItem = new AspNetCore.ReportingServices.ReportIntermediateFormat.ActionItem();
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = false;
			bool flag8 = false;
			hasDrillthroughParameter = false;
			context.PrefixPropertyName = "ActionInfo.Action.";
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag9 = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Hyperlink":
							this.m_hasHyperlinks = true;
							flag = true;
							actionItem.HyperLinkURL = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out flag5);
							break;
						case "Drillthrough":
							flag2 = true;
							this.ReadDrillthrough(context, actionItem, out flag6);
							break;
						case "BookmarkLink":
							flag3 = true;
							actionItem.BookmarkLink = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out flag7);
							break;
						case "Label":
							flag4 = true;
							actionItem.Label = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out flag8);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("Action" == this.m_reader.LocalName)
						{
							flag9 = true;
						}
						break;
					}
				}
				while (!flag9);
			}
			int num = 0;
			if (flag)
			{
				num++;
			}
			if (flag2)
			{
				num++;
				hasDrillthroughParameter = (actionItem.DrillthroughParameters != null && actionItem.DrillthroughParameters.Count > 0);
			}
			if (flag3)
			{
				num++;
			}
			if (1 != num)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidAction, Severity.Error, context.ObjectType, context.ObjectName, "Action");
			}
			if (!flag4)
			{
				missingLabel = true;
			}
			computed = (flag5 || flag6 || flag7 || flag8);
			if (computed)
			{
				computedIndex++;
				actionItem.ComputedIndex = computedIndex;
			}
			return actionItem;
		}

		private void ReadDrillthrough(PublishingContextStruct context, AspNetCore.ReportingServices.ReportIntermediateFormat.ActionItem actionItem, out bool computed)
		{
			computed = false;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			context.PrefixPropertyName = "ActionInfo.Action.Drillthrough.";
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "ReportName":
						actionItem.DrillthroughReportName = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out flag3);
						if (AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == actionItem.DrillthroughReportName.Type)
						{
							actionItem.DrillthroughReportName.StringValue = PublishingValidator.ValidateReportName(this.m_publishingContext.CatalogContext, actionItem.DrillthroughReportName.StringValue, context.ObjectType, context.ObjectName, "DrillthroughReportName", context.ErrorContext);
						}
						break;
					case "Parameters":
						actionItem.DrillthroughParameters = this.ReadParameters(context, true, false, false, out flag);
						break;
					case "BookmarkLink":
						actionItem.DrillthroughBookmarkLink = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out flag2);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Drillthrough" == this.m_reader.LocalName)
					{
						flag4 = true;
					}
					break;
				}
			}
			while (!flag4);
			computed = (flag || flag2 || flag3);
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadBookmarkExpression(PublishingContextStruct context, out bool computedBookmark)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo result = this.ReadBookmarkExpression(this.m_reader.LocalName, context, out computedBookmark);
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsBookmarkInPageSection, Severity.Warning, context.ObjectType, context.ObjectName, null);
			}
			return result;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.BandLayoutOptions ReadBandLayoutOptions(AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.BandLayoutOptions bandLayoutOptions = new AspNetCore.ReportingServices.ReportIntermediateFormat.BandLayoutOptions();
			int num = 0;
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "RowCount":
							bandLayoutOptions.RowCount = this.m_reader.ReadInteger(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
							break;
						case "ColumnCount":
							bandLayoutOptions.ColumnCount = this.m_reader.ReadInteger(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
							break;
						case "Coverflow":
							num++;
							if (bandLayoutOptions.Navigation == null)
							{
								bandLayoutOptions.Navigation = this.ReadCoverflow(tablix, context);
							}
							break;
						case "Tabstrip":
							num++;
							if (bandLayoutOptions.Navigation == null)
							{
								bandLayoutOptions.Navigation = this.ReadTabstrip(tablix, context);
							}
							break;
						case "PlayAxis":
							num++;
							if (bandLayoutOptions.Navigation == null)
							{
								bandLayoutOptions.Navigation = this.ReadPlayAxis(context);
							}
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "BandLayoutOptions")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			if (num > 1)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidBandNavigations, Severity.Error, context.ObjectType, tablix.Name, null);
			}
			return bandLayoutOptions;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.NavigationItem ReadNavigationItem(AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context, string navigationType)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.NavigationItem navigationItem = new AspNetCore.ReportingServices.ReportIntermediateFormat.NavigationItem();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "ReportItemReference":
							navigationItem.ReportItemReference = this.m_reader.ReadString();
							break;
						case "ReportItem":
						{
							List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox>();
							navigationItem.BandNavigationCell = new BandNavigationCell(this.GenerateID(), tablix);
							AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem altCellContents = default(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem);
							int? nullable = default(int?);
							int? nullable2 = default(int?);
							navigationItem.BandNavigationCell.CellContents = this.ReadCellContents((AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem)tablix, context, textBoxesWithDefaultSortTarget, false, out altCellContents, out nullable, out nullable2);
							navigationItem.BandNavigationCell.AltCellContents = altCellContents;
							break;
						}
						}
						break;
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "NavigationItem")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
				if (navigationItem.ReportItemReference != null && navigationItem.BandNavigationCell != null)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidBandNavigationItem, Severity.Error, context.ObjectType, tablix.Name, navigationType);
				}
			}
			return navigationItem;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Coverflow ReadCoverflow(AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.Coverflow coverflow = new AspNetCore.ReportingServices.ReportIntermediateFormat.Coverflow();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "NavigationItem":
							coverflow.NavigationItem = this.ReadNavigationItem(tablix, context, "Coverflow");
							break;
						case "Slider":
							coverflow.Slider = this.ReadSlider(context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "Coverflow")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return coverflow;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Tabstrip ReadTabstrip(AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.Tabstrip tabstrip = new AspNetCore.ReportingServices.ReportIntermediateFormat.Tabstrip();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "NavigationItem":
							tabstrip.NavigationItem = this.ReadNavigationItem(tablix, context, "Tabstrip");
							break;
						case "Slider":
							tabstrip.Slider = this.ReadSlider(context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "Tabstrip")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return tabstrip;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.PlayAxis ReadPlayAxis(PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.PlayAxis playAxis = new AspNetCore.ReportingServices.ReportIntermediateFormat.PlayAxis();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Slider":
							playAxis.Slider = this.ReadSlider(context);
							break;
						case "DockingOption":
							playAxis.DockingOption = this.ReadDockingOption();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "PlayAxis")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return playAxis;
		}

		private DockingOption ReadDockingOption()
		{
			string value = this.m_reader.ReadString();
			return (DockingOption)Enum.Parse(typeof(DockingOption), value, false);
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Slider ReadSlider(PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.Slider slider = new AspNetCore.ReportingServices.ReportIntermediateFormat.Slider();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Hidden":
							slider.Hidden = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
							break;
						case "LabelData":
							slider.LabelData = this.ReadLabelData(context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "Slider")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return slider;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.LabelData ReadLabelData(PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.LabelData labelData = new AspNetCore.ReportingServices.ReportIntermediateFormat.LabelData();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "DataSetName":
							labelData.DataSetName = this.m_reader.ReadString();
							break;
						case "Key":
						{
							string item = this.m_reader.ReadString();
							if (labelData.KeyFields == null)
							{
								labelData.KeyFields = new List<string>(1);
								labelData.KeyFields.Add(item);
							}
							break;
						}
						case "KeyFields":
							labelData.KeyFields = this.ReadKeyFields();
							if (this.m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.LabelData_KeyFields))
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, "KeyFields");
							}
							break;
						case "Label":
							labelData.Label = this.m_reader.ReadString();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "LabelData")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			if (labelData.KeyFields == null)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidReportDefinition, Severity.Error, context.ObjectType, context.ObjectName, "LabelData", "KeyFields");
			}
			return labelData;
		}

		private List<string> ReadKeyFields()
		{
			List<string> list = new List<string>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "Key")
						{
							list.Add(this.m_reader.ReadString());
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "KeyFields")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Chart ReadChart(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem parent, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart = new AspNetCore.ReportingServices.ReportIntermediateFormat.Chart(this.GenerateID(), parent);
			chart.Name = this.m_reader.GetAttribute("Name");
			if ((context.Location & LocationFlags.InDataRegion) != 0)
			{
				Global.Tracer.Assert(this.m_nestedDataRegions != null, "(m_nestedDataRegions != null)");
				this.m_nestedDataRegions.Add(chart);
			}
			context.Location = (context.Location | LocationFlags.InDataSet | LocationFlags.InDataRegion);
			context.ObjectType = chart.ObjectType;
			context.ObjectName = chart.Name;
			this.RegisterDataRegion(chart);
			bool flag = true;
			if (!this.m_reportItemNames.Validate(context.ObjectType, context.ObjectName, context.ErrorContext))
			{
				flag = false;
			}
			if (this.m_scopeNames.Validate(false, context.ObjectName, context.ObjectType, context.ObjectName, context.ErrorContext))
			{
				this.m_reportScopes.Add(chart.Name, chart);
			}
			else
			{
				flag = false;
			}
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsDataRegionInPageSection, Severity.Error, context.ObjectType, context.ObjectName, null);
				flag = false;
			}
			StyleInformation styleInformation = null;
			IdcRelationship relationship = null;
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag2 = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "SortExpressions":
							chart.Sorting = this.ReadSortExpressions(true, context);
							break;
						case "Style":
							styleInformation = this.ReadStyle(context);
							break;
						case "Top":
							chart.Top = this.ReadSize();
							break;
						case "Left":
							chart.Left = this.ReadSize();
							break;
						case "Height":
							chart.Height = this.ReadSize();
							break;
						case "Width":
							chart.Width = this.ReadSize();
							break;
						case "ZIndex":
							chart.ZIndex = this.m_reader.ReadInteger(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
							break;
						case "Visibility":
							chart.Visibility = this.ReadVisibility(context);
							break;
						case "ToolTip":
							chart.ToolTip = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "DocumentMapLabel":
							chart.DocumentMapLabel = this.ReadDocumentMapLabelExpression(this.m_reader.LocalName, context);
							break;
						case "Bookmark":
							chart.Bookmark = this.ReadBookmarkExpression(this.m_reader.LocalName, context);
							break;
						case "CustomProperties":
							chart.CustomProperties = this.ReadCustomProperties(context);
							break;
						case "DataElementName":
							chart.DataElementName = this.m_reader.ReadString();
							break;
						case "DataElementOutput":
							chart.DataElementOutput = this.ReadDataElementOutput();
							break;
						case "NoRowsMessage":
							chart.NoRowsMessage = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "DataSetName":
							chart.DataSetName = this.m_reader.ReadString();
							break;
						case "Relationship":
							relationship = this.ReadRelationship(context);
							break;
						case "PageBreak":
							this.ReadPageBreak(chart, context);
							break;
						case "PageName":
							chart.PageName = this.ReadPageNameExpression(context);
							break;
						case "Filters":
							chart.Filters = this.ReadFilters(AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.DataRegionFilters, context);
							break;
						case "ChartSeriesHierarchy":
							chart.SeriesMembers = this.ReadChartHierarchy(chart, context, false);
							break;
						case "ChartCategoryHierarchy":
							chart.CategoryMembers = this.ReadChartHierarchy(chart, context, true);
							break;
						case "ChartData":
						{
							bool hasDataValueAggregates = default(bool);
							this.ReadChartData(chart, context, out hasDataValueAggregates);
							chart.HasDataValueAggregates = hasDataValueAggregates;
							break;
						}
						case "ChartAreas":
							chart.ChartAreas = this.ReadChartAreas(chart, context);
							break;
						case "ChartLegends":
							chart.Legends = this.ReadChartLegends(chart, context);
							break;
						case "ChartTitles":
							chart.Titles = this.ReadChartTitles(chart, context);
							break;
						case "Palette":
							chart.Palette = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chart.Palette.IsExpression)
							{
								Validator.ValidatePalette(chart.Palette.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "PaletteHatchBehavior":
							chart.PaletteHatchBehavior = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chart.PaletteHatchBehavior.IsExpression)
							{
								Validator.ValidatePaletteHatchBehavior(chart.PaletteHatchBehavior.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "ChartCodeParameters":
							chart.CodeParameters = this.ReadChartCodeParameters(context);
							break;
						case "ChartCustomPaletteColors":
							chart.CustomPaletteColors = this.ReadChartCustomPaletteColors(chart, context);
							break;
						case "ChartBorderSkin":
							chart.BorderSkin = this.ReadChartBorderSkin(chart, context);
							break;
						case "ChartNoDataMessage":
						{
							ChartNoDataMessage chartNoDataMessage = new ChartNoDataMessage(chart);
							this.ReadChartTitle(chart, chartNoDataMessage, true, context, new DynamicImageObjectUniqueNameValidator());
							chart.NoDataMessage = chartNoDataMessage;
							break;
						}
						case "DynamicHeight":
							chart.DynamicHeight = this.ReadExpression("DynamicHeight", AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "DynamicWidth":
							chart.DynamicWidth = this.ReadExpression("DynamicWidth", AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("Chart" == this.m_reader.LocalName)
						{
							flag2 = true;
						}
						break;
					}
				}
				while (!flag2);
			}
			chart.SetColumnGroupingDirection(this.m_publishingContext.IsRdlx);
			chart.DataScopeInfo.SetRelationship(chart.DataSetName, relationship);
			if (styleInformation != null)
			{
				styleInformation.Filter(StyleOwnerType.Chart, null != chart.NoRowsMessage);
				chart.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, false, context.ErrorContext);
			}
			if (chart.CategoryMembers == null || chart.CategoryMembers.Count == 0)
			{
				this.ChartFakeStaticCategory(chart);
			}
			if (chart.SeriesMembers == null || chart.SeriesMembers.Count == 0)
			{
				this.ChartFakeStaticSeries(chart);
			}
			if (chart.StyleClass != null)
			{
				PublishingValidator.ValidateBorderColorNotTransparent(chart.ObjectType, chart.Name, chart.StyleClass, "BorderColor", context.ErrorContext);
				PublishingValidator.ValidateBorderColorNotTransparent(chart.ObjectType, chart.Name, chart.StyleClass, "BorderColorBottom", context.ErrorContext);
				PublishingValidator.ValidateBorderColorNotTransparent(chart.ObjectType, chart.Name, chart.StyleClass, "BorderColorTop", context.ErrorContext);
				PublishingValidator.ValidateBorderColorNotTransparent(chart.ObjectType, chart.Name, chart.StyleClass, "BorderColorLeft", context.ErrorContext);
				PublishingValidator.ValidateBorderColorNotTransparent(chart.ObjectType, chart.Name, chart.StyleClass, "BorderColorRight", context.ErrorContext);
			}
			chart.Computed = true;
			if (flag)
			{
				this.m_hasImageStreams = true;
				return chart;
			}
			return null;
		}

		private ChartMemberList ReadChartHierarchy(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, bool isCategoryHierarchy)
		{
			ChartMemberList chartMemberList = null;
			int num = 0;
			int num2 = 0;
			bool flag = false;
			if (!this.m_reader.IsEmptyElement)
			{
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "ChartMembers":
							chartMemberList = this.ReadChartMembers(chart, context, isCategoryHierarchy, 0, ref num, ref num2);
							break;
						case "EnableDrilldown":
							if (isCategoryHierarchy)
							{
								bool flag2 = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
								if (flag2 && this.m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.ChartHierarchy_EnableDrilldown))
								{
									context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, chart.Name, "EnableDrilldown");
								}
								chart.EnableCategoryDrilldown = flag2;
							}
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == (isCategoryHierarchy ? "ChartCategoryHierarchy" : "ChartSeriesHierarchy"))
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			if (isCategoryHierarchy)
			{
				chart.CategoryCount = num;
			}
			else
			{
				chart.SeriesCount = num;
			}
			this.SetCategoryOrSeriesSpans(chartMemberList, isCategoryHierarchy, num2 + 1);
			return chartMemberList;
		}

		private void SetCategoryOrSeriesSpans(ChartMemberList members, bool isCategoryHierarchy, int totalSpansFromLevel)
		{
			foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.ChartMember member in members)
			{
				int num;
				if (member.ChartMembers != null && member.ChartMembers.Count > 0)
				{
					num = 1;
					this.SetCategoryOrSeriesSpans(member.ChartMembers, isCategoryHierarchy, totalSpansFromLevel - 1);
				}
				else
				{
					num = totalSpansFromLevel;
				}
				if (isCategoryHierarchy)
				{
					member.RowSpan = num;
				}
				else
				{
					member.ColSpan = num;
				}
			}
		}

		private ChartMemberList ReadChartMembers(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, bool isCategoryHierarchy, int level, ref int leafNodes, ref int maxLevel)
		{
			ChartMemberList chartMemberList = new ChartMemberList();
			bool flag = false;
			if (!this.m_reader.IsEmptyElement)
			{
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "ChartMember")
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.ChartMember value = this.ReadChartMember(chart, context, isCategoryHierarchy, level, ref leafNodes, ref maxLevel);
							chartMemberList.Add(value);
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("ChartMembers" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			if (chartMemberList.Count <= 0)
			{
				return null;
			}
			return chartMemberList;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartMember ReadChartMember(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, bool isCategoryHierarchy, int level, ref int aLeafNodes, ref int maxLevel)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ChartMember chartMember = new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartMember(this.GenerateID(), chart);
			this.m_runningValueHolderList.Add(chartMember);
			chartMember.IsColumn = isCategoryHierarchy;
			chartMember.Level = level;
			maxLevel = Math.Max(maxLevel, level);
			bool flag = false;
			int num = 0;
			if (!this.m_reader.IsEmptyElement)
			{
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Group":
							chartMember.Grouping = this.ReadGrouping(chartMember, context);
							if (chartMember.Grouping.PageBreak != null && chartMember.Grouping.PageBreak.BreakLocation != 0)
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsPageBreakOnChartGroup, Severity.Warning, context.ObjectType, context.ObjectName, isCategoryHierarchy ? "CategoryGroupings" : "SeriesGroupings", chartMember.Grouping.Name.MarkAsModelInfo());
							}
							break;
						case "SortExpressions":
							chartMember.Sorting = this.ReadSortExpressions(false, context);
							break;
						case "ChartMembers":
							chartMember.ChartMembers = this.ReadChartMembers(chart, context, isCategoryHierarchy, level + 1, ref num, ref maxLevel);
							break;
						case "Label":
							chartMember.Label = this.ReadExpression("Label", AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "CustomProperties":
							chartMember.CustomProperties = this.ReadCustomProperties(context);
							break;
						case "DataElementName":
							chartMember.DataElementName = this.m_reader.ReadString();
							break;
						case "DataElementOutput":
							chartMember.DataElementOutput = this.ReadDataElementOutput();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartMember" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			if (chartMember.ChartMembers == null || chartMember.ChartMembers.Count == 0)
			{
				aLeafNodes++;
				if (isCategoryHierarchy)
				{
					chartMember.ColSpan = 1;
				}
				else
				{
					chartMember.RowSpan = 1;
				}
			}
			else
			{
				aLeafNodes += num;
				if (isCategoryHierarchy)
				{
					chartMember.ColSpan = num;
				}
				else
				{
					chartMember.RowSpan = num;
				}
			}
			this.ValidateAndProcessMemberGroupAndSort(chartMember, context);
			if (chartMember.Grouping != null)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo label = chartMember.Label;
				if ((label == null || (label.Type == AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant && label.StringValue.Length == 0)) && chartMember.Grouping.GroupExpressions != null && chartMember.Grouping.GroupExpressions.Count > 0)
				{
					chartMember.Label = chartMember.Grouping.GroupExpressions[0];
				}
			}
			else
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo label2 = chartMember.Label;
				if (label2.Type == AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant && label2.StringValue.Length == 0)
				{
					label2.StringValue = null;
				}
			}
			return chartMember;
		}

		private void ChartFakeStaticSeries(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart)
		{
			Global.Tracer.Assert(null != chart);
			Global.Tracer.Assert(!(chart.SeriesMembers != null && chart.SeriesMembers.Count != 0));
			chart.SeriesCount = 1;
			chart.SeriesMembers = new ChartMemberList(1);
			AspNetCore.ReportingServices.ReportIntermediateFormat.ChartMember chartMember = new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartMember(this.GenerateID(), chart);
			chartMember.ColSpan = 1;
			chartMember.RowSpan = 1;
			chart.SeriesMembers.Add(chartMember);
		}

		private void ChartFakeStaticCategory(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart)
		{
			Global.Tracer.Assert(null != chart);
			Global.Tracer.Assert(!(chart.CategoryMembers != null && chart.CategoryMembers.Count != 0));
			chart.CategoryCount = 1;
			chart.CategoryMembers = new ChartMemberList(1);
			AspNetCore.ReportingServices.ReportIntermediateFormat.ChartMember chartMember = new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartMember(this.GenerateID(), chart);
			chartMember.ColSpan = 1;
			chartMember.RowSpan = 1;
			chart.CategoryMembers.Add(chartMember);
		}

		private void ReadChartTitle(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, AspNetCore.ReportingServices.ReportIntermediateFormat.ChartTitle title, bool isNoDataMessage, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator namesValidator)
		{
			if (!isNoDataMessage)
			{
				title.TitleName = this.m_reader.GetAttribute("Name");
				namesValidator.Validate(Severity.Error, "ChartTitle", AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, chart.Name, title.TitleName, context.ErrorContext);
			}
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Caption":
							title.Caption = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, false);
							title.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, context.ErrorContext);
							break;
						}
						case "Position":
							title.Position = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!title.Position.IsExpression)
							{
								Validator.ValidateChartTitlePositions(title.Position.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "Hidden":
							title.Hidden = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Docking":
							title.Docking = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!title.Docking.IsExpression)
							{
								Validator.ValidateChartTitleDockings(title.Docking.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "DockToChartArea":
							title.DockToChartArea = this.m_reader.ReadString();
							break;
						case "DockOutsideChartArea":
							title.DockOutsideChartArea = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "DockOffset":
							title.DockOffset = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "ToolTip":
							title.ToolTip = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ActionInfo":
						{
							bool flag2 = default(bool);
							title.Action = this.ReadActionInfo(context, StyleOwnerType.Chart, out flag2);
							break;
						}
						case "TextOrientation":
							title.TextOrientation = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!title.TextOrientation.IsExpression)
							{
								Validator.ValidateTextOrientations(title.TextOrientation.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "ChartElementPosition":
							title.ChartElementPosition = this.ReadChartElementPosition(chart, context, "ChartElementPosition");
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if (isNoDataMessage)
						{
							if ("ChartNoDataMessage" == this.m_reader.LocalName)
							{
								flag = true;
							}
						}
						else if ("ChartTitle" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxisTitle ReadChartAxisTitle(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxisTitle chartAxisTitle = new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxisTitle(chart);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Caption":
							chartAxisTitle.Caption = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, false);
							chartAxisTitle.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, context.ErrorContext);
							break;
						}
						case "Position":
							chartAxisTitle.Position = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartAxisTitle.Position.IsExpression)
							{
								Validator.ValidateChartAxisTitlePositions(chartAxisTitle.Position.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "TextOrientation":
							chartAxisTitle.TextOrientation = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartAxisTitle.TextOrientation.IsExpression)
							{
								Validator.ValidateTextOrientations(chartAxisTitle.TextOrientation.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartAxisTitle" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartAxisTitle;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendTitle ReadChartLegendTitle(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendTitle chartLegendTitle = new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendTitle(chart);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Caption":
							chartLegendTitle.Caption = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.FilterChartLegendTitleStyle();
							chartLegendTitle.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, context.ErrorContext);
							break;
						}
						case "TitleSeparator":
							chartLegendTitle.TitleSeparator = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartLegendTitle" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartLegendTitle;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis> ReadValueAxes(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "ChartAxis")
						{
							list.Add(this.ReadAxis(chart, context, false, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "ChartValueAxes")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis> ReadCategoryAxes(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "ChartAxis")
						{
							list.Add(this.ReadAxis(chart, context, true, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "ChartCategoryAxes")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartArea> ReadChartAreas(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartArea> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartArea>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "ChartArea")
						{
							list.Add(this.ReadChartArea(chart, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "ChartAreas")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartTitle> ReadChartTitles(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator namesValidator = new DynamicImageObjectUniqueNameValidator();
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartTitle> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartTitle>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "ChartTitle")
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.ChartTitle chartTitle = new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartTitle(chart);
							this.ReadChartTitle(chart, chartTitle, false, context, namesValidator);
							list.Add(chartTitle);
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "ChartTitles")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegend> ReadChartLegends(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegend> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegend>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "ChartLegend")
						{
							list.Add(this.ReadChartLegend(chart, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "ChartLegends")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartTickMarks ReadChartTickMarks(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, bool isMajor)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ChartTickMarks chartTickMarks = new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartTickMarks(chart);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Enabled":
							chartTickMarks.Enabled = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Type":
							chartTickMarks.Type = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartTickMarks.Type.IsExpression)
							{
								Validator.ValidateChartTickMarksType(chartTickMarks.Type.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "Length":
							chartTickMarks.Length = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Interval":
							chartTickMarks.Interval = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "IntervalType":
							chartTickMarks.IntervalType = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartTickMarks.IntervalType.IsExpression)
							{
								Validator.ValidateChartIntervalType(chartTickMarks.IntervalType.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "IntervalOffset":
							chartTickMarks.IntervalOffset = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "IntervalOffsetType":
							chartTickMarks.IntervalOffsetType = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartTickMarks.IntervalOffsetType.IsExpression)
							{
								Validator.ValidateChartIntervalType(chartTickMarks.IntervalOffsetType.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, false);
							chartTickMarks.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, context.ErrorContext);
							break;
						}
						}
						break;
					case XmlNodeType.EndElement:
						if (!isMajor || !("ChartMajorTickMarks" == this.m_reader.LocalName))
						{
							if (isMajor)
							{
								break;
							}
							if (!("ChartMinorTickMarks" == this.m_reader.LocalName))
							{
								break;
							}
						}
						flag = true;
						break;
					}
				}
				while (!flag);
			}
			return chartTickMarks;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis ReadAxis(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, bool isCategory, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis = new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis(chart);
			chartAxis.AxisName = this.m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, isCategory ? "ChartCategoryAxis" : "ChartValueAxis", AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, chart.Name, chartAxis.AxisName, context.ErrorContext);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, false);
							chartAxis.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, context.ErrorContext);
							break;
						}
						case "Visible":
							chartAxis.Visible = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartAxis.Visible.IsExpression)
							{
								Validator.ValidateChartAutoBool(chartAxis.Visible.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "Margin":
							chartAxis.Margin = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartAxis.Margin.IsExpression)
							{
								Validator.ValidateChartAutoBool(chartAxis.Margin.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "Interval":
							chartAxis.Interval = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "IntervalType":
							chartAxis.IntervalType = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartAxis.IntervalType.IsExpression)
							{
								Validator.ValidateChartIntervalType(chartAxis.IntervalType.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "IntervalOffset":
							chartAxis.IntervalOffset = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "IntervalOffsetType":
							chartAxis.IntervalOffsetType = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartAxis.IntervalOffsetType.IsExpression)
							{
								Validator.ValidateChartIntervalType(chartAxis.IntervalOffsetType.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "ChartMajorTickMarks":
							chartAxis.MajorTickMarks = this.ReadChartTickMarks(chart, context, true);
							break;
						case "ChartMinorTickMarks":
							chartAxis.MinorTickMarks = this.ReadChartTickMarks(chart, context, false);
							break;
						case "MarksAlwaysAtPlotEdge":
							chartAxis.MarksAlwaysAtPlotEdge = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Reverse":
							chartAxis.Reverse = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Location":
							chartAxis.Location = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartAxis.Location.IsExpression)
							{
								Validator.ValidateChartAxisLocation(chartAxis.Location.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "Interlaced":
							chartAxis.Interlaced = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "InterlacedColor":
							chartAxis.InterlacedColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartAxis.InterlacedColor.IsExpression)
							{
								PublishingValidator.ValidateColor(chartAxis.InterlacedColor, context.ObjectType, context.ObjectName, this.m_reader.LocalName, context.ErrorContext);
							}
							break;
						case "LogScale":
							chartAxis.LogScale = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "LogBase":
							chartAxis.LogBase = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "HideLabels":
							chartAxis.HideLabels = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Angle":
							chartAxis.Angle = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Arrows":
							chartAxis.Arrows = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartAxis.Arrows.IsExpression)
							{
								Validator.ValidateChartAxisArrow(chartAxis.Arrows.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "PreventFontShrink":
							chartAxis.PreventFontShrink = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "PreventFontGrow":
							chartAxis.PreventFontGrow = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "PreventLabelOffset":
							chartAxis.PreventLabelOffset = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "PreventWordWrap":
							chartAxis.PreventWordWrap = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "AllowLabelRotation":
							chartAxis.AllowLabelRotation = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartAxis.AllowLabelRotation.IsExpression)
							{
								Validator.ValidateChartAxisLabelRotation(chartAxis.AllowLabelRotation.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "IncludeZero":
							chartAxis.IncludeZero = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "LabelsAutoFitDisabled":
							chartAxis.LabelsAutoFitDisabled = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "MinFontSize":
							chartAxis.MinFontSize = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartAxis.MinFontSize.IsExpression)
							{
								PublishingValidator.ValidateSize(chartAxis.MinFontSize, Validator.FontSizeMin, Validator.FontSizeMax, context.ObjectType, context.ObjectName, this.m_reader.LocalName, context.ErrorContext);
							}
							break;
						case "MaxFontSize":
							chartAxis.MaxFontSize = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartAxis.MaxFontSize.IsExpression)
							{
								PublishingValidator.ValidateSize(chartAxis.MaxFontSize, Validator.FontSizeMin, Validator.FontSizeMax, context.ObjectType, context.ObjectName, this.m_reader.LocalName, context.ErrorContext);
							}
							break;
						case "OffsetLabels":
							chartAxis.OffsetLabels = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "HideEndLabels":
							chartAxis.HideEndLabels = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ChartAxisScaleBreak":
							chartAxis.AxisScaleBreak = this.ReadChartAxisScaleBreak(chart, context);
							break;
						case "ChartAxisTitle":
							chartAxis.Title = this.ReadChartAxisTitle(chart, context);
							break;
						case "ChartMajorGridLines":
							chartAxis.MajorGridLines = this.ReadGridLines(chart, context, true);
							break;
						case "ChartMinorGridLines":
							chartAxis.MinorGridLines = this.ReadGridLines(chart, context, false);
							break;
						case "CrossAt":
							chartAxis.CrossAt = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Scalar":
							chartAxis.Scalar = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
							break;
						case "Minimum":
							chartAxis.Minimum = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Maximum":
							chartAxis.Maximum = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ChartStripLines":
							chartAxis.StripLines = this.ReadChartStripLines(chart, context);
							break;
						case "CustomProperties":
							chartAxis.CustomProperties = this.ReadCustomProperties(context);
							break;
						case "VariableAutoInterval":
							chartAxis.VariableAutoInterval = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "LabelInterval":
							chartAxis.LabelInterval = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "LabelIntervalType":
							chartAxis.LabelIntervalType = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartAxis.LabelIntervalType.IsExpression)
							{
								Validator.ValidateChartIntervalType(chartAxis.LabelIntervalType.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "LabelIntervalOffset":
							chartAxis.LabelIntervalOffset = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "LabelIntervalOffsetType":
							chartAxis.LabelIntervalOffsetType = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartAxis.LabelIntervalOffsetType.IsExpression)
							{
								Validator.ValidateChartIntervalType(chartAxis.LabelIntervalOffsetType.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartAxis" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartAxis;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxisScaleBreak ReadChartAxisScaleBreak(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxisScaleBreak chartAxisScaleBreak = new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxisScaleBreak(chart);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Enabled":
							chartAxisScaleBreak.Enabled = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "BreakLineType":
							chartAxisScaleBreak.BreakLineType = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartAxisScaleBreak.BreakLineType.IsExpression)
							{
								Validator.ValidateChartBreakLineType(chartAxisScaleBreak.BreakLineType.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "CollapsibleSpaceThreshold":
							chartAxisScaleBreak.CollapsibleSpaceThreshold = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "MaxNumberOfBreaks":
							chartAxisScaleBreak.MaxNumberOfBreaks = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "Spacing":
							chartAxisScaleBreak.Spacing = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "IncludeZero":
							chartAxisScaleBreak.IncludeZero = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartAxisScaleBreak.IncludeZero.IsExpression)
							{
								Validator.ValidateChartAutoBool(chartAxisScaleBreak.IncludeZero.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, false);
							chartAxisScaleBreak.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, context.ErrorContext);
							break;
						}
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartAxisScaleBreak" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartAxisScaleBreak;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartFormulaParameter> ReadChartFormulaParameters(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDerivedSeries chartDerivedSeries, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartFormulaParameter> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartFormulaParameter>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "ChartFormulaParameter")
						{
							list.Add(this.ReadChartFormulaParameter(chart, chartDerivedSeries, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "ChartFormulaParameters")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartFormulaParameter ReadChartFormulaParameter(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDerivedSeries chartDerivedSeries, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ChartFormulaParameter chartFormulaParameter = new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartFormulaParameter(chart, chartDerivedSeries);
			chartFormulaParameter.FormulaParameterName = this.m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "ChartFormulaParameter", AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, chart.Name, chartFormulaParameter.FormulaParameterName, context.ErrorContext);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Value":
							chartFormulaParameter.Value = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Source":
							chartFormulaParameter.Source = this.m_reader.ReadString();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartFormulaParameter" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartFormulaParameter;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections ReadChartNoMoveDirections(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections chartNoMoveDirections = new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections(chart, chartSeries);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Up":
							chartNoMoveDirections.Up = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Down":
							chartNoMoveDirections.Down = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Left":
							chartNoMoveDirections.Left = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Right":
							chartNoMoveDirections.Right = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "UpLeft":
							chartNoMoveDirections.UpLeft = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "UpRight":
							chartNoMoveDirections.UpRight = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "DownLeft":
							chartNoMoveDirections.DownLeft = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "DownRight":
							chartNoMoveDirections.DownRight = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartNoMoveDirections" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartNoMoveDirections;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendColumn ReadChartLegendColumn(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendColumn chartLegendColumn = new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendColumn(chart, chart.GenerateActionOwnerID());
			chartLegendColumn.LegendColumnName = this.m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "ChartLegendColumn", AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, chart.Name, chartLegendColumn.LegendColumnName, context.ErrorContext);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				bool flag2 = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, false);
							chartLegendColumn.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, context.ErrorContext);
							break;
						}
						case "ActionInfo":
							chartLegendColumn.Action = this.ReadActionInfo(context, StyleOwnerType.Chart, out flag2);
							break;
						case "ColumnType":
							chartLegendColumn.ColumnType = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartLegendColumn.ColumnType.IsExpression)
							{
								Validator.ValidateChartColumnType(chartLegendColumn.ColumnType.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "Value":
							chartLegendColumn.Value = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ToolTip":
							chartLegendColumn.ToolTip = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "MinimumWidth":
							chartLegendColumn.MinimumWidth = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartLegendColumn.MinimumWidth.IsExpression)
							{
								PublishingValidator.ValidateSize(chartLegendColumn.MinimumWidth, Validator.NormalMin, Validator.NormalMax, context.ObjectType, context.ObjectName, this.m_reader.LocalName, context.ErrorContext);
							}
							break;
						case "MaximumWidth":
							chartLegendColumn.MaximumWidth = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartLegendColumn.MaximumWidth.IsExpression)
							{
								PublishingValidator.ValidateSize(chartLegendColumn.MaximumWidth, Validator.NormalMin, Validator.NormalMax, context.ObjectType, context.ObjectName, this.m_reader.LocalName, context.ErrorContext);
							}
							break;
						case "SeriesSymbolWidth":
							chartLegendColumn.SeriesSymbolWidth = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "SeriesSymbolHeight":
							chartLegendColumn.SeriesSymbolHeight = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "Header":
							chartLegendColumn.Header = this.ReadChartLegendColumnHeader(chart, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartLegendColumn" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartLegendColumn;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartElementPosition ReadChartElementPosition(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, string chartElementPositionName)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ChartElementPosition chartElementPosition = new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartElementPosition(chart);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Top":
							chartElementPosition.Top = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Left":
							chartElementPosition.Left = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Height":
							chartElementPosition.Height = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Width":
							chartElementPosition.Width = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if (chartElementPositionName == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartElementPosition;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSmartLabel ReadChartSmartLabel(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSmartLabel chartSmartLabel = new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSmartLabel(chart, chartSeries);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "AllowOutSidePlotArea":
							chartSmartLabel.AllowOutSidePlotArea = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartSmartLabel.AllowOutSidePlotArea.IsExpression)
							{
								Validator.ValidateChartAllowOutsideChartArea(chartSmartLabel.AllowOutSidePlotArea.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "CalloutBackColor":
							chartSmartLabel.CalloutBackColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartSmartLabel.CalloutBackColor.IsExpression)
							{
								PublishingValidator.ValidateColor(chartSmartLabel.CalloutBackColor, context.ObjectType, context.ObjectName, this.m_reader.LocalName, context.ErrorContext);
							}
							break;
						case "CalloutLineAnchor":
							chartSmartLabel.CalloutLineAnchor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartSmartLabel.CalloutLineAnchor.IsExpression)
							{
								Validator.ValidateChartCalloutLineAnchor(chartSmartLabel.CalloutLineAnchor.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "CalloutLineColor":
							chartSmartLabel.CalloutLineColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartSmartLabel.CalloutLineColor.IsExpression)
							{
								PublishingValidator.ValidateColor(chartSmartLabel.CalloutLineColor, context.ObjectType, context.ObjectName, this.m_reader.LocalName, context.ErrorContext);
							}
							break;
						case "CalloutLineStyle":
							chartSmartLabel.CalloutLineStyle = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartSmartLabel.CalloutLineStyle.IsExpression)
							{
								Validator.ValidateChartCalloutLineStyle(chartSmartLabel.CalloutLineStyle.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "CalloutLineWidth":
							chartSmartLabel.CalloutLineWidth = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartSmartLabel.CalloutLineWidth.IsExpression)
							{
								PublishingValidator.ValidateSize(chartSmartLabel.CalloutLineWidth, Validator.NormalMin, Validator.NormalMax, context.ObjectType, context.ObjectName, this.m_reader.LocalName, context.ErrorContext);
							}
							break;
						case "CalloutStyle":
							chartSmartLabel.CalloutStyle = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartSmartLabel.CalloutStyle.IsExpression)
							{
								Validator.ValidateChartCalloutStyle(chartSmartLabel.CalloutStyle.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "ShowOverlapped":
							chartSmartLabel.ShowOverlapped = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "MarkerOverlapping":
							chartSmartLabel.MarkerOverlapping = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "MaxMovingDistance":
							chartSmartLabel.MaxMovingDistance = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartSmartLabel.MaxMovingDistance.IsExpression)
							{
								PublishingValidator.ValidateSize(chartSmartLabel.MaxMovingDistance, Validator.NormalMin, Validator.NormalMax, context.ObjectType, context.ObjectName, this.m_reader.LocalName, context.ErrorContext);
							}
							break;
						case "MinMovingDistance":
							chartSmartLabel.MinMovingDistance = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartSmartLabel.MinMovingDistance.IsExpression)
							{
								PublishingValidator.ValidateSize(chartSmartLabel.MinMovingDistance, Validator.NormalMin, Validator.NormalMax, context.ObjectType, context.ObjectName, this.m_reader.LocalName, context.ErrorContext);
							}
							break;
						case "Disabled":
							chartSmartLabel.Disabled = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ChartNoMoveDirections":
							chartSmartLabel.NoMoveDirections = this.ReadChartNoMoveDirections(chart, chartSeries, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartSmartLabel" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartSmartLabel;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell ReadChartLegendCustomItemCell(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell chartLegendCustomItemCell = new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell(chart, chart.GenerateActionOwnerID());
			chartLegendCustomItemCell.LegendCustomItemCellName = this.m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "ChartLegendCustomItemCell", AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, chart.Name, chartLegendCustomItemCell.LegendCustomItemCellName, context.ErrorContext);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				bool flag2 = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, false);
							chartLegendCustomItemCell.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, context.ErrorContext);
							break;
						}
						case "ActionInfo":
							chartLegendCustomItemCell.Action = this.ReadActionInfo(context, StyleOwnerType.Chart, out flag2);
							break;
						case "CellType":
							chartLegendCustomItemCell.CellType = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartLegendCustomItemCell.CellType.IsExpression)
							{
								Validator.ValidateChartCellType(chartLegendCustomItemCell.CellType.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "Text":
							chartLegendCustomItemCell.Text = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "CellSpan":
							chartLegendCustomItemCell.CellSpan = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "ToolTip":
							chartLegendCustomItemCell.ToolTip = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ImageWidth":
							chartLegendCustomItemCell.ImageWidth = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "ImageHeight":
							chartLegendCustomItemCell.ImageHeight = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "SymbolHeight":
							chartLegendCustomItemCell.SymbolHeight = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "SymbolWidth":
							chartLegendCustomItemCell.SymbolWidth = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "Alignment":
							chartLegendCustomItemCell.Alignment = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "TopMargin":
							chartLegendCustomItemCell.TopMargin = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "BottomMargin":
							chartLegendCustomItemCell.BottomMargin = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "LeftMargin":
							chartLegendCustomItemCell.LeftMargin = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "RightMargin":
							chartLegendCustomItemCell.RightMargin = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartLegendCustomItemCell" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartLegendCustomItemCell;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItem ReadChartLegendCustomItem(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItem chartLegendCustomItem = new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItem(chart, chart.GenerateActionOwnerID());
			chartLegendCustomItem.LegendCustomItemName = this.m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "ChartLegendCustomItem", AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, chart.Name, chartLegendCustomItem.LegendCustomItemName, context.ErrorContext);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				bool flag2 = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, false);
							chartLegendCustomItem.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, context.ErrorContext);
							break;
						}
						case "ActionInfo":
							chartLegendCustomItem.Action = this.ReadActionInfo(context, StyleOwnerType.Chart, out flag2);
							break;
						case "ChartMarker":
							chartLegendCustomItem.Marker = this.ReadChartMarker(chart, null, null, context);
							break;
						case "Separator":
							chartLegendCustomItem.Separator = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartLegendCustomItem.Separator.IsExpression)
							{
								Validator.ValidateChartCustomItemSeparator(chartLegendCustomItem.Separator.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "SeparatorColor":
							chartLegendCustomItem.SeparatorColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartLegendCustomItem.SeparatorColor.IsExpression)
							{
								PublishingValidator.ValidateColor(chartLegendCustomItem.SeparatorColor, context.ObjectType, context.ObjectName, this.m_reader.LocalName, context.ErrorContext);
							}
							break;
						case "ToolTip":
							chartLegendCustomItem.ToolTip = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ChartLegendCustomItemCells":
							chartLegendCustomItem.LegendCustomItemCells = this.ReadChartLegendCustomItemCells(chart, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartLegendCustomItem" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartLegendCustomItem;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell> ReadChartLegendCustomItemCells(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItemCell>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "ChartLegendCustomItemCell")
						{
							list.Add(this.ReadChartLegendCustomItemCell(chart, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "ChartLegendCustomItemCells")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegend ReadChartLegend(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegend chartLegend = new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegend(chart);
			chartLegend.LegendName = this.m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "ChartLegend", AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, chart.Name, chartLegend.LegendName, context.ErrorContext);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Hidden":
							chartLegend.Hidden = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, false);
							chartLegend.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, context.ErrorContext);
							break;
						}
						case "Position":
							chartLegend.Position = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Layout":
							chartLegend.Layout = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "DockToChartArea":
							chartLegend.DockToChartArea = this.m_reader.ReadString();
							break;
						case "DockOutsideChartArea":
							chartLegend.DockOutsideChartArea = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ChartLegendTitle":
							chartLegend.LegendTitle = this.ReadChartLegendTitle(chart, context);
							break;
						case "AutoFitTextDisabled":
							chartLegend.AutoFitTextDisabled = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "MinFontSize":
							chartLegend.MinFontSize = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartLegend.MinFontSize.IsExpression)
							{
								PublishingValidator.ValidateSize(chartLegend.MinFontSize, Validator.FontSizeMin, Validator.FontSizeMax, context.ObjectType, context.ObjectName, this.m_reader.LocalName, context.ErrorContext);
							}
							break;
						case "HeaderSeparator":
							chartLegend.HeaderSeparator = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartLegend.HeaderSeparator.IsExpression)
							{
								Validator.ValidateChartCustomItemSeparator(chartLegend.HeaderSeparator.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "HeaderSeparatorColor":
							chartLegend.HeaderSeparatorColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartLegend.HeaderSeparatorColor.IsExpression)
							{
								PublishingValidator.ValidateColor(chartLegend.HeaderSeparatorColor, context.ObjectType, context.ObjectName, this.m_reader.LocalName, context.ErrorContext);
							}
							break;
						case "ColumnSeparator":
							chartLegend.ColumnSeparator = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartLegend.ColumnSeparator.IsExpression)
							{
								Validator.ValidateChartCustomItemSeparator(chartLegend.ColumnSeparator.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "ColumnSeparatorColor":
							chartLegend.ColumnSeparatorColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartLegend.ColumnSeparatorColor.IsExpression)
							{
								PublishingValidator.ValidateColor(chartLegend.ColumnSeparatorColor, context.ObjectType, context.ObjectName, this.m_reader.LocalName, context.ErrorContext);
							}
							break;
						case "ColumnSpacing":
							chartLegend.ColumnSpacing = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "InterlacedRows":
							chartLegend.InterlacedRows = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "InterlacedRowsColor":
							chartLegend.InterlacedRowsColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartLegend.InterlacedRowsColor.IsExpression)
							{
								PublishingValidator.ValidateColor(chartLegend.InterlacedRowsColor, context.ObjectType, context.ObjectName, this.m_reader.LocalName, context.ErrorContext);
							}
							break;
						case "EquallySpacedItems":
							chartLegend.EquallySpacedItems = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Reversed":
							chartLegend.Reversed = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartLegend.Reversed.IsExpression)
							{
								Validator.ValidateChartAutoBool(chartLegend.Reversed.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "MaxAutoSize":
							chartLegend.MaxAutoSize = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "TextWrapThreshold":
							chartLegend.TextWrapThreshold = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "ChartLegendColumns":
							chartLegend.LegendColumns = this.ReadChartLegendColumns(chart, context);
							break;
						case "ChartLegendCustomItems":
							chartLegend.LegendCustomItems = this.ReadChartLegendCustomItems(chart, context);
							break;
						case "ChartElementPosition":
							chartLegend.ChartElementPosition = this.ReadChartElementPosition(chart, context, "ChartElementPosition");
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartLegend" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartLegend;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendColumn> ReadChartLegendColumns(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendColumn> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendColumn>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "ChartLegendColumn")
						{
							list.Add(this.ReadChartLegendColumn(chart, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "ChartLegendColumns")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItem> ReadChartLegendCustomItems(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItem> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendCustomItem>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "ChartLegendCustomItem")
						{
							list.Add(this.ReadChartLegendCustomItem(chart, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "ChartLegendCustomItems")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartStripLine> ReadChartStripLines(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartStripLine> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartStripLine>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "ChartStripLine")
						{
							list.Add(this.ReadChartStripLine(chart, context));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "ChartStripLines")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDerivedSeries> ReadChartDerivedSeriesCollection(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDerivedSeries> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDerivedSeries>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "ChartDerivedSeries")
						{
							list.Add(this.ReadChartDerivedSeries(chart, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "ChartDerivedSeriesCollection")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDerivedSeries ReadChartDerivedSeries(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDerivedSeries chartDerivedSeries = new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDerivedSeries(chart);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				bool flag2 = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "ChartSeries":
							chartDerivedSeries.Series = this.ReadChartSeries(chart, chartDerivedSeries, context, ref flag2, nameValidator);
							break;
						case "SourceChartSeriesName":
							chartDerivedSeries.SourceChartSeriesName = this.m_reader.ReadString();
							break;
						case "DerivedSeriesFormula":
							chartDerivedSeries.DerivedSeriesFormula = (ChartSeriesFormula)Enum.Parse(typeof(ChartSeriesFormula), this.m_reader.ReadString(), false);
							break;
						case "ChartFormulaParameters":
							chartDerivedSeries.FormulaParameters = this.ReadChartFormulaParameters(chart, chartDerivedSeries, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartDerivedSeries" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartDerivedSeries;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartStripLine ReadChartStripLine(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLine = new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartStripLine(chart, chart.GenerateActionOwnerID());
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				bool flag2 = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.FilterChartStripLineStyle();
							chartStripLine.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, context.ErrorContext);
							break;
						}
						case "ActionInfo":
							chartStripLine.Action = this.ReadActionInfo(context, StyleOwnerType.Chart, out flag2);
							break;
						case "Title":
							chartStripLine.Title = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "TitleAngle":
							chartStripLine.TitleAngle = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "ToolTip":
							chartStripLine.ToolTip = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Interval":
							chartStripLine.Interval = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "IntervalType":
							chartStripLine.IntervalType = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartStripLine.IntervalType.IsExpression)
							{
								Validator.ValidateChartIntervalType(chartStripLine.IntervalType.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "IntervalOffset":
							chartStripLine.IntervalOffset = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "IntervalOffsetType":
							chartStripLine.IntervalOffsetType = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartStripLine.IntervalOffsetType.IsExpression)
							{
								Validator.ValidateChartIntervalType(chartStripLine.IntervalOffsetType.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "StripWidth":
							chartStripLine.StripWidth = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "StripWidthType":
							chartStripLine.StripWidthType = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartStripLine.StripWidthType.IsExpression)
							{
								Validator.ValidateChartIntervalType(chartStripLine.StripWidthType.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "TextOrientation":
							chartStripLine.TextOrientation = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartStripLine.TextOrientation.IsExpression)
							{
								Validator.ValidateTextOrientations(chartStripLine.TextOrientation.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartStripLine" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartStripLine;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartGridLines ReadGridLines(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, bool isMajor)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ChartGridLines chartGridLines = new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartGridLines(chart);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, false);
							chartGridLines.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, context.ErrorContext);
							break;
						}
						case "Enabled":
							chartGridLines.Enabled = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Interval":
							chartGridLines.Interval = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "IntervalType":
							chartGridLines.IntervalType = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartGridLines.IntervalType.IsExpression)
							{
								Validator.ValidateChartIntervalType(chartGridLines.IntervalType.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "IntervalOffset":
							chartGridLines.IntervalOffset = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "IntervalOffsetType":
							chartGridLines.IntervalOffsetType = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartGridLines.IntervalOffsetType.IsExpression)
							{
								Validator.ValidateChartIntervalType(chartGridLines.IntervalOffsetType.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if (!isMajor || !("ChartMajorGridLines" == this.m_reader.LocalName))
						{
							if (isMajor)
							{
								break;
							}
							if (!("ChartMinorGridLines" == this.m_reader.LocalName))
							{
								break;
							}
						}
						flag = true;
						break;
					}
				}
				while (!flag);
			}
			return chartGridLines;
		}

		private void ReadChartData(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, out bool hasAggregates)
		{
			hasAggregates = false;
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "ChartSeriesCollection":
							chart.ChartSeriesCollection = this.ReadChartSeriesCollection(chart, context, ref hasAggregates);
							break;
						case "ChartDerivedSeriesCollection":
							chart.DerivedSeriesCollection = this.ReadChartDerivedSeriesCollection(chart, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartData" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			if (chart.DerivedSeriesCollection != null)
			{
				foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDerivedSeries item in chart.DerivedSeriesCollection)
				{
					if (item.SourceSeries == null)
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsInvalidSourceSeriesName, Severity.Error, context.ObjectType, context.ObjectName, "SourceChartSeriesName", item.SourceChartSeriesName.MarkAsPrivate());
					}
				}
			}
		}

		private ChartSeriesList ReadChartSeriesCollection(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, ref bool hasAggregates)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			ChartSeriesList chartSeriesList = new ChartSeriesList();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "ChartSeries")
						{
							chartSeriesList.Add(this.ReadChartSeries(chart, (AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDerivedSeries)null, context, ref hasAggregates, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("ChartSeriesCollection" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartSeriesList;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartArea ReadChartArea(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ChartArea chartArea = new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartArea(chart);
			chartArea.ChartAreaName = this.m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "ChartArea", AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, chart.Name, chartArea.ChartAreaName, context.ErrorContext);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, false);
							chartArea.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, context.ErrorContext);
							break;
						}
						case "ChartCategoryAxes":
							chartArea.CategoryAxes = this.ReadCategoryAxes(chart, context);
							break;
						case "ChartValueAxes":
							chartArea.ValueAxes = this.ReadValueAxes(chart, context);
							break;
						case "ChartThreeDProperties":
							chartArea.ThreeDProperties = this.ReadThreeDProperties(chart, context);
							break;
						case "Hidden":
							chartArea.Hidden = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "AlignOrientation":
							chartArea.AlignOrientation = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ChartAlignType":
							chartArea.ChartAlignType = this.ReadChartAlignType(chart, context);
							break;
						case "AlignWithChartArea":
							chartArea.AlignWithChartArea = this.m_reader.ReadString();
							break;
						case "EquallySizedAxesFont":
							chartArea.EquallySizedAxesFont = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ChartElementPosition":
							chartArea.ChartElementPosition = this.ReadChartElementPosition(chart, context, "ChartElementPosition");
							break;
						case "ChartInnerPlotPosition":
							chartArea.ChartInnerPlotPosition = this.ReadChartElementPosition(chart, context, "ChartInnerPlotPosition");
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartArea" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartArea;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAlignType ReadChartAlignType(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAlignType chartAlignType = null;
			if (!this.m_reader.IsEmptyElement)
			{
				chartAlignType = new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAlignType(chart);
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Position":
							chartAlignType.Position = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "InnerPlotPosition":
							chartAlignType.InnerPlotPosition = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "AxesView":
							chartAlignType.AxesView = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Cursor":
							chartAlignType.Cursor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartAlignType" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartAlignType;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSeries ReadChartSeries(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDerivedSeries chartDerivedSeries, PublishingContextStruct context, ref bool hasAggregates, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSeries chartSeries = new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSeries(chart, chartDerivedSeries, this.GenerateID());
			chartSeries.Name = this.m_reader.GetAttribute("Name");
			if (!string.IsNullOrEmpty(chartSeries.Name))
			{
				nameValidator.Validate(Severity.Error, "ChartSeries", AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, chart.Name, chartSeries.Name, context.ErrorContext);
			}
			if (!this.m_reader.IsEmptyElement)
			{
				string text = null;
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							if (chartDerivedSeries != null)
							{
								styleInformation.Filter(StyleOwnerType.Chart, false);
							}
							else
							{
								styleInformation.FilterChartSeriesStyle();
							}
							chartSeries.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, context.ErrorContext);
							break;
						}
						case "CustomProperties":
							chartSeries.CustomProperties = this.ReadCustomProperties(context);
							break;
						case "ChartDataPoints":
							chartSeries.DataPoints = this.ReadChartDataPoints(chart, context, ref hasAggregates);
							break;
						case "Type":
							chartSeries.Type = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartSeries.Type.IsExpression)
							{
								Validator.ValidateChartSeriesType(chartSeries.Type.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "Subtype":
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!expressionInfo.IsExpression)
							{
								Validator.ValidateChartSeriesSubtype(expressionInfo.StringValue, context.ErrorContext, context, this.m_reader.LocalName, this.m_reader.NamespaceURI);
							}
							if (text != null && RdlNamespaceComparer.Instance.Compare(this.m_reader.NamespaceURI, text) <= 0)
							{
								break;
							}
							chartSeries.Subtype = expressionInfo;
							text = this.m_reader.NamespaceURI;
							break;
						}
						case "ChartEmptyPoints":
							chartSeries.EmptyPoints = this.ReadChartEmptyPoints(chart, chartSeries, context);
							break;
						case "LegendName":
							chartSeries.LegendName = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ChartAreaName":
							chartSeries.ChartAreaName = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ValueAxisName":
							chartSeries.ValueAxisName = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "CategoryAxisName":
							chartSeries.CategoryAxisName = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Hidden":
							chartSeries.Hidden = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ChartSmartLabel":
							chartSeries.ChartSmartLabel = this.ReadChartSmartLabel(chart, chartSeries, context);
							break;
						case "ChartDataLabel":
							if (chartDerivedSeries != null)
							{
								chartSeries.DataLabel = this.ReadChartDataLabel(chart, chartSeries, null, context);
							}
							break;
						case "ChartMarker":
							if (chartDerivedSeries != null)
							{
								chartSeries.Marker = this.ReadChartMarker(chart, chartSeries, null, context);
							}
							break;
						case "ChartItemInLegend":
							chartSeries.ChartItemInLegend = this.ReadChartItemInLegend(chart, chartSeries, null, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartSeries" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartSeries;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPointList ReadChartDataPoints(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, ref bool hasAggregates)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPointList chartDataPointList = new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPointList();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "ChartDataPoint")
						{
							chartDataPointList.Add(this.ReadChartDataPoint(chart, context, ref hasAggregates));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("ChartDataPoints" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartDataPointList;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint ReadChartDataPoint(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context, ref bool hasAggregates)
		{
			context.Location |= LocationFlags.InDynamicTablixCell;
			AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint chartDataPoint = new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint(this.GenerateID(), chart);
			this.m_aggregateHolderList.Add(chartDataPoint);
			this.m_runningValueHolderList.Add(chartDataPoint);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				bool flag2 = false;
				string dataSetName = null;
				List<IdcRelationship> relationships = null;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "DataSetName":
							dataSetName = this.m_reader.ReadString();
							break;
						case "Relationships":
							relationships = this.ReadRelationships(context);
							break;
						case "ChartDataPointValues":
							chartDataPoint.DataPointValues = this.ReadChartDataPointValues(chart, chartDataPoint, context);
							break;
						case "ChartDataLabel":
							chartDataPoint.DataLabel = this.ReadChartDataLabel(chart, null, chartDataPoint, context);
							break;
						case "ActionInfo":
							chartDataPoint.Action = this.ReadActionInfo(context, StyleOwnerType.Chart, out flag2);
							break;
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, false);
							chartDataPoint.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, context.ErrorContext);
							break;
						}
						case "ChartMarker":
							chartDataPoint.Marker = this.ReadChartMarker(chart, null, chartDataPoint, context);
							break;
						case "DataElementName":
							chartDataPoint.DataElementName = this.m_reader.ReadString();
							break;
						case "DataElementOutput":
							chartDataPoint.DataElementOutput = this.ReadDataElementOutput();
							break;
						case "CustomProperties":
							chartDataPoint.CustomProperties = this.ReadCustomProperties(context);
							break;
						case "AxisLabel":
							chartDataPoint.AxisLabel = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ChartItemInLegend":
							chartDataPoint.ItemInLegend = this.ReadChartItemInLegend(chart, null, chartDataPoint, context);
							break;
						case "ToolTip":
							chartDataPoint.ToolTip = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartDataPoint" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
				chartDataPoint.DataScopeInfo.SetRelationship(dataSetName, relationships);
			}
			return chartDataPoint;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartMarker ReadChartMarker(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSeries series, AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ChartMarker chartMarker = (dataPoint == null) ? new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartMarker(chart, series) : new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartMarker(chart, dataPoint);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Type":
							chartMarker.Type = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartMarker.Type.IsExpression)
							{
								Validator.ValidateChartMarkerType(chartMarker.Type.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "Size":
							chartMarker.Size = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartMarker.Size.IsExpression)
							{
								PublishingValidator.ValidateSize(chartMarker.Size, Validator.NormalMin, Validator.NormalMax, context.ObjectType, context.ObjectName, this.m_reader.LocalName, context.ErrorContext);
							}
							break;
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, false);
							chartMarker.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, context.ErrorContext);
							break;
						}
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartMarker" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartMarker;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataLabel ReadChartDataLabel(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSeries series, AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataLabel chartDataLabel = (dataPoint == null) ? new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataLabel(chart, series) : new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataLabel(chart, dataPoint);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Visible":
							chartDataLabel.Visible = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, false);
							chartDataLabel.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, context.ErrorContext);
							break;
						}
						case "Label":
							chartDataLabel.Label = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Position":
							chartDataLabel.Position = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartDataLabel.Position.IsExpression)
							{
								Validator.ValidateChartDataLabelPosition(chartDataLabel.Position.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "Rotation":
							chartDataLabel.Rotation = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "UseValueAsLabel":
							chartDataLabel.UseValueAsLabel = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ActionInfo":
						{
							bool flag2 = default(bool);
							chartDataLabel.Action = this.ReadActionInfo(context, StyleOwnerType.Chart, out flag2);
							break;
						}
						case "ToolTip":
							chartDataLabel.ToolTip = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartDataLabel" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartDataLabel;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadChartDataPointFormatExpressionValues(string propertyName, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo result = null;
			if (this.m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.CellLevelFormatting))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, propertyName);
			}
			else
			{
				result = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
			}
			return result;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPointValues ReadChartDataPointValues(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPointValues chartDataPointValues = new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPointValues(chart, dataPoint);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "X":
							chartDataPointValues.X = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Y":
							chartDataPointValues.Y = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Size":
							chartDataPointValues.Size = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "High":
							chartDataPointValues.High = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Low":
							chartDataPointValues.Low = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Start":
							chartDataPointValues.Start = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "End":
							chartDataPointValues.End = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Mean":
							chartDataPointValues.Mean = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Median":
							chartDataPointValues.Median = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "HighlightX":
							chartDataPointValues.HighlightX = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "HighlightY":
							chartDataPointValues.HighlightY = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "HighlightSize":
							chartDataPointValues.HighlightSize = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "FormatX":
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo6 = this.ReadChartDataPointFormatExpressionValues("FormatX", context);
							if (expressionInfo6 != null)
							{
								chartDataPointValues.FormatX = expressionInfo6;
							}
							break;
						}
						case "FormatY":
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo5 = this.ReadChartDataPointFormatExpressionValues("FormatY", context);
							if (expressionInfo5 != null)
							{
								chartDataPointValues.FormatY = expressionInfo5;
							}
							break;
						}
						case "FormatSize":
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo4 = this.ReadChartDataPointFormatExpressionValues("FormatSize", context);
							if (expressionInfo4 != null)
							{
								chartDataPointValues.FormatSize = expressionInfo4;
							}
							break;
						}
						case "CurrencyLanguageX":
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo3 = this.ReadChartDataPointFormatExpressionValues("CurrencyLanguageX", context);
							if (expressionInfo3 != null)
							{
								chartDataPointValues.CurrencyLanguageX = expressionInfo3;
							}
							break;
						}
						case "CurrencyLanguageY":
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo2 = this.ReadChartDataPointFormatExpressionValues("CurrencyLanguageY", context);
							if (expressionInfo2 != null)
							{
								chartDataPointValues.CurrencyLanguageY = expressionInfo2;
							}
							break;
						}
						case "CurrencyLanguageSize":
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = this.ReadChartDataPointFormatExpressionValues("CurrencyLanguageSize", context);
							if (expressionInfo != null)
							{
								chartDataPointValues.CurrencyLanguageSize = expressionInfo;
							}
							break;
						}
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartDataPointValues" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartDataPointValues;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartThreeDProperties ReadThreeDProperties(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ChartThreeDProperties chartThreeDProperties = new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartThreeDProperties(chart);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Enabled":
							chartThreeDProperties.Enabled = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ProjectionMode":
							chartThreeDProperties.ProjectionMode = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartThreeDProperties.ProjectionMode.IsExpression)
							{
								Validator.ValidateChartThreeDProjectionMode(chartThreeDProperties.ProjectionMode.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "Rotation":
							chartThreeDProperties.Rotation = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "Inclination":
							chartThreeDProperties.Inclination = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "Perspective":
							chartThreeDProperties.Perspective = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "DepthRatio":
							chartThreeDProperties.DepthRatio = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "Shading":
							chartThreeDProperties.Shading = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartThreeDProperties.Shading.IsExpression)
							{
								Validator.ValidateChartThreeDShading(chartThreeDProperties.Shading.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						case "GapDepth":
							chartThreeDProperties.GapDepth = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "WallThickness":
							chartThreeDProperties.WallThickness = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "Clustered":
							chartThreeDProperties.Clustered = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartThreeDProperties" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartThreeDProperties;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartCustomPaletteColor> ReadChartCustomPaletteColors(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartCustomPaletteColor> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.ChartCustomPaletteColor>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "ChartCustomPaletteColor")
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.ChartCustomPaletteColor chartCustomPaletteColor = new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartCustomPaletteColor(chart);
							chartCustomPaletteColor.Color = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							list.Add(chartCustomPaletteColor);
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("ChartCustomPaletteColors" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.DataValueList ReadChartCodeParameters(PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataValueList dataValueList = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataValueList();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				bool flag2 = false;
				int num = 0;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "ChartCodeParameter")
						{
							dataValueList.Add(this.ReadDataValue(false, true, ++num, ref flag2, context));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("CustomProperties" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return dataValueList;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartItemInLegend ReadChartItemInLegend(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSeries series, AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPoint, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ChartItemInLegend chartItemInLegend = (dataPoint == null) ? new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartItemInLegend(chart, series) : new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartItemInLegend(chart, dataPoint);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				bool flag2 = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "ActionInfo":
							chartItemInLegend.Action = this.ReadActionInfo(context, StyleOwnerType.Chart, out flag2);
							break;
						case "LegendText":
							chartItemInLegend.LegendText = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ToolTip":
							chartItemInLegend.ToolTip = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Hidden":
							chartItemInLegend.Hidden = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartItemInLegend" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartItemInLegend;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartEmptyPoints ReadChartEmptyPoints(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSeries series, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ChartEmptyPoints chartEmptyPoints = new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartEmptyPoints(chart, series);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				bool flag2 = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, false);
							chartEmptyPoints.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, context.ErrorContext);
							break;
						}
						case "ActionInfo":
							chartEmptyPoints.Action = this.ReadActionInfo(context, StyleOwnerType.Chart, out flag2);
							break;
						case "ChartMarker":
							chartEmptyPoints.Marker = this.ReadChartMarker(chart, series, null, context);
							break;
						case "ChartDataLabel":
							chartEmptyPoints.DataLabel = this.ReadChartDataLabel(chart, series, null, context);
							break;
						case "AxisLabel":
							chartEmptyPoints.AxisLabel = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "CustomProperties":
							chartEmptyPoints.CustomProperties = this.ReadCustomProperties(context);
							break;
						case "ToolTip":
							chartEmptyPoints.ToolTip = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartEmptyPoints" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartEmptyPoints;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendColumnHeader ReadChartLegendColumnHeader(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendColumnHeader chartLegendColumnHeader = new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendColumnHeader(chart);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, false);
							chartLegendColumnHeader.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, context.ErrorContext);
							break;
						}
						case "Value":
							chartLegendColumnHeader.Value = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartLegendColumnHeader" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartLegendColumnHeader;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartBorderSkin ReadChartBorderSkin(AspNetCore.ReportingServices.ReportIntermediateFormat.Chart chart, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ChartBorderSkin chartBorderSkin = new AspNetCore.ReportingServices.ReportIntermediateFormat.ChartBorderSkin(chart);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, false);
							chartBorderSkin.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, context.ErrorContext);
							break;
						}
						case "ChartBorderSkinType":
							chartBorderSkin.BorderSkinType = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!chartBorderSkin.BorderSkinType.IsExpression)
							{
								Validator.ValidateChartBorderSkinType(chartBorderSkin.BorderSkinType.StringValue, context.ErrorContext, context, this.m_reader.LocalName);
							}
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ChartBorderSkin" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartBorderSkin;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel ReadGaugePanel(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem parent, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel = new AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel(this.GenerateID(), parent);
			gaugePanel.Name = this.m_reader.GetAttribute("Name");
			if ((context.Location & LocationFlags.InDataRegion) != 0)
			{
				Global.Tracer.Assert(this.m_nestedDataRegions != null, "(m_nestedDataRegions != null)");
				this.m_nestedDataRegions.Add(gaugePanel);
			}
			context.Location = (context.Location | LocationFlags.InDataSet | LocationFlags.InDataRegion);
			context.ObjectType = gaugePanel.ObjectType;
			context.ObjectName = gaugePanel.Name;
			this.RegisterDataRegion(gaugePanel);
			bool flag = true;
			if (!this.m_reportItemNames.Validate(context.ObjectType, context.ObjectName, this.m_errorContext))
			{
				flag = false;
			}
			if (this.m_scopeNames.Validate(false, context.ObjectName, context.ObjectType, context.ObjectName, this.m_errorContext))
			{
				this.m_reportScopes.Add(gaugePanel.Name, gaugePanel);
			}
			else
			{
				flag = false;
			}
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsDataRegionInPageSection, Severity.Error, context.ObjectType, context.ObjectName, null);
				flag = false;
			}
			StyleInformation styleInformation = null;
			IdcRelationship relationship = null;
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag2 = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "SortExpressions":
							gaugePanel.Sorting = this.ReadSortExpressions(true, context);
							break;
						case "Style":
							styleInformation = this.ReadStyle(context);
							break;
						case "Top":
							gaugePanel.Top = this.ReadSize();
							break;
						case "Left":
							gaugePanel.Left = this.ReadSize();
							break;
						case "Height":
							gaugePanel.Height = this.ReadSize();
							break;
						case "Width":
							gaugePanel.Width = this.ReadSize();
							break;
						case "ZIndex":
							gaugePanel.ZIndex = this.m_reader.ReadInteger(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
							break;
						case "Visibility":
							gaugePanel.Visibility = this.ReadVisibility(context);
							break;
						case "ToolTip":
							gaugePanel.ToolTip = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "DocumentMapLabel":
							gaugePanel.DocumentMapLabel = this.ReadDocumentMapLabelExpression(this.m_reader.LocalName, context);
							break;
						case "Bookmark":
							gaugePanel.Bookmark = this.ReadBookmarkExpression(this.m_reader.LocalName, context);
							break;
						case "CustomProperties":
							gaugePanel.CustomProperties = this.ReadCustomProperties(context);
							break;
						case "DataElementName":
							gaugePanel.DataElementName = this.m_reader.ReadString();
							break;
						case "DataElementOutput":
							gaugePanel.DataElementOutput = this.ReadDataElementOutput();
							break;
						case "NoRowsMessage":
							gaugePanel.NoRowsMessage = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "DataSetName":
							gaugePanel.DataSetName = this.m_reader.ReadString();
							break;
						case "Relationship":
							relationship = this.ReadRelationship(context);
							break;
						case "PageBreak":
							this.ReadPageBreak(gaugePanel, context);
							break;
						case "PageName":
							gaugePanel.PageName = this.ReadPageNameExpression(context);
							break;
						case "Filters":
							gaugePanel.Filters = this.ReadFilters(AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.DataRegionFilters, context);
							break;
						case "GaugeMember":
						{
							int num = 0;
							gaugePanel.GaugeMember = this.ReadGaugeMember(gaugePanel, context, 0, ref num);
							break;
						}
						case "LinearGauges":
							gaugePanel.LinearGauges = this.ReadLinearGauges(gaugePanel, context);
							break;
						case "RadialGauges":
							gaugePanel.RadialGauges = this.ReadRadialGauges(gaugePanel, context);
							break;
						case "NumericIndicators":
							gaugePanel.NumericIndicators = this.ReadNumericIndicators(gaugePanel, context);
							break;
						case "StateIndicators":
							gaugePanel.StateIndicators = this.ReadStateIndicators(gaugePanel, context);
							break;
						case "GaugeImages":
							gaugePanel.GaugeImages = this.ReadGaugeImages(gaugePanel, context);
							break;
						case "GaugeLabels":
							gaugePanel.GaugeLabels = this.ReadGaugeLabels(gaugePanel, context);
							break;
						case "AntiAliasing":
							gaugePanel.AntiAliasing = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!gaugePanel.AntiAliasing.IsExpression)
							{
								Validator.ValidateGaugeAntiAliasings(gaugePanel.AntiAliasing.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "AutoLayout":
							gaugePanel.AutoLayout = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "BackFrame":
							gaugePanel.BackFrame = this.ReadBackFrame(gaugePanel, context);
							break;
						case "ShadowIntensity":
							gaugePanel.ShadowIntensity = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "TextAntiAliasingQuality":
							gaugePanel.TextAntiAliasingQuality = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!gaugePanel.TextAntiAliasingQuality.IsExpression)
							{
								Validator.ValidateTextAntiAliasingQualities(gaugePanel.TextAntiAliasingQuality.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "TopImage":
							gaugePanel.TopImage = this.ReadTopImage(gaugePanel, context, "TopImage");
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("GaugePanel" == this.m_reader.LocalName)
						{
							flag2 = true;
						}
						break;
					}
				}
				while (!flag2);
			}
			gaugePanel.DataScopeInfo.SetRelationship(gaugePanel.DataSetName, relationship);
			if (styleInformation != null)
			{
				styleInformation.Filter(StyleOwnerType.GaugePanel, null != gaugePanel.NoRowsMessage);
				gaugePanel.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, false, this.m_errorContext);
			}
			if (gaugePanel.GaugeMember == null)
			{
				this.AddStaticGaugeMember(this.GenerateID(), gaugePanel);
			}
			this.AddStaticGaugeRowMember(this.GenerateID(), gaugePanel);
			this.AddGaugeRow(this.GenerateID(), this.GenerateID(), gaugePanel);
			if (gaugePanel.StyleClass != null)
			{
				PublishingValidator.ValidateBorderColorNotTransparent(gaugePanel.ObjectType, gaugePanel.Name, gaugePanel.StyleClass, "BorderColor", this.m_errorContext);
				PublishingValidator.ValidateBorderColorNotTransparent(gaugePanel.ObjectType, gaugePanel.Name, gaugePanel.StyleClass, "BorderColorBottom", this.m_errorContext);
				PublishingValidator.ValidateBorderColorNotTransparent(gaugePanel.ObjectType, gaugePanel.Name, gaugePanel.StyleClass, "BorderColorTop", this.m_errorContext);
				PublishingValidator.ValidateBorderColorNotTransparent(gaugePanel.ObjectType, gaugePanel.Name, gaugePanel.StyleClass, "BorderColorLeft", this.m_errorContext);
				PublishingValidator.ValidateBorderColorNotTransparent(gaugePanel.ObjectType, gaugePanel.Name, gaugePanel.StyleClass, "BorderColorRight", this.m_errorContext);
			}
			gaugePanel.Computed = true;
			if (flag)
			{
				this.m_hasImageStreams = true;
				return gaugePanel;
			}
			return null;
		}

		private void AddStaticGaugeMember(int ID, AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel)
		{
			gaugePanel.GaugeMember = new AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeMember(ID, gaugePanel);
			gaugePanel.GaugeMember.Level = 0;
			gaugePanel.GaugeMember.ColSpan = 1;
			gaugePanel.GaugeMember.IsColumn = true;
		}

		private void AddStaticGaugeRowMember(int ID, AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel)
		{
			gaugePanel.GaugeRowMember = new AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeMember(ID, gaugePanel);
			gaugePanel.GaugeRowMember.Level = 0;
			gaugePanel.GaugeRowMember.RowSpan = 1;
		}

		private void AddGaugeRow(int rowID, int cellID, AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel)
		{
			gaugePanel.GaugeRow = new AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeRow(rowID, gaugePanel);
			gaugePanel.GaugeRow.GaugeCell = new AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeCell(cellID, gaugePanel);
			this.m_aggregateHolderList.Add(gaugePanel.GaugeRow.GaugeCell);
			this.m_runningValueHolderList.Add(gaugePanel.GaugeRow.GaugeCell);
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeMember ReadGaugeMember(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, int level, ref int aLeafNodes)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeMember gaugeMember = new AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeMember(this.GenerateID(), gaugePanel);
			this.m_runningValueHolderList.Add(gaugeMember);
			gaugeMember.IsColumn = true;
			gaugeMember.Level = level;
			bool flag = false;
			int num = 0;
			if (!this.m_reader.IsEmptyElement)
			{
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Group":
							gaugeMember.Grouping = this.ReadGrouping(gaugeMember, context);
							if (gaugeMember.Grouping.PageBreak != null && gaugeMember.Grouping.PageBreak.BreakLocation != 0)
							{
								this.m_errorContext.Register(ProcessingErrorCode.rsPageBreakOnGaugeGroup, Severity.Warning, context.ObjectType, context.ObjectName, "Group", gaugeMember.Grouping.Name);
							}
							break;
						case "SortExpressions":
							gaugeMember.Sorting = this.ReadSortExpressions(false, context);
							break;
						case "GaugeMember":
							gaugeMember.ChildGaugeMember = this.ReadGaugeMember(gaugePanel, context, level + 1, ref num);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("GaugeMember" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			if (gaugeMember.ChildGaugeMember == null)
			{
				aLeafNodes++;
				gaugeMember.ColSpan = 1;
			}
			else
			{
				aLeafNodes += num;
				gaugeMember.ColSpan = num;
			}
			this.ValidateAndProcessMemberGroupAndSort(gaugeMember, context);
			return gaugeMember;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.TopImage ReadTopImage(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, string elementName)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.TopImage topImage = new AspNetCore.ReportingServices.ReportIntermediateFormat.TopImage(gaugePanel);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Source":
							topImage.Source = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!topImage.Source.IsExpression)
							{
								Validator.ValidateImageSourceType(topImage.Source.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "Value":
							topImage.Value = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "MIMEType":
							topImage.MIMEType = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "HueColor":
							topImage.HueColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "TransparentColor":
							topImage.TransparentColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if (elementName == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return topImage;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.PointerImage ReadPointerImage(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.PointerImage pointerImage = new AspNetCore.ReportingServices.ReportIntermediateFormat.PointerImage(gaugePanel);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Source":
							pointerImage.Source = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!pointerImage.Source.IsExpression)
							{
								Validator.ValidateImageSourceType(pointerImage.Source.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "Value":
							pointerImage.Value = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "MIMEType":
							pointerImage.MIMEType = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "TransparentColor":
							pointerImage.TransparentColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "HueColor":
							pointerImage.HueColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Transparency":
							pointerImage.Transparency = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "OffsetX":
							pointerImage.OffsetX = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "OffsetY":
							pointerImage.OffsetY = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("PointerImage" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return pointerImage;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.FrameImage ReadFrameImage(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.FrameImage frameImage = new AspNetCore.ReportingServices.ReportIntermediateFormat.FrameImage(gaugePanel);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Source":
							frameImage.Source = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!frameImage.Source.IsExpression)
							{
								Validator.ValidateImageSourceType(frameImage.Source.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "Value":
							frameImage.Value = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "MIMEType":
							frameImage.MIMEType = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "TransparentColor":
							frameImage.TransparentColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "HueColor":
							frameImage.HueColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Transparency":
							frameImage.Transparency = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "ClipImage":
							frameImage.ClipImage = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("FrameImage" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return frameImage;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.CapImage ReadCapImage(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.CapImage capImage = new AspNetCore.ReportingServices.ReportIntermediateFormat.CapImage(gaugePanel);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Source":
							capImage.Source = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Value":
							capImage.Value = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "MIMEType":
							capImage.MIMEType = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "TransparentColor":
							capImage.TransparentColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "HueColor":
							capImage.HueColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "OffsetX":
							capImage.OffsetX = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "OffsetY":
							capImage.OffsetY = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("CapImage" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return capImage;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.BackFrame ReadBackFrame(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.BackFrame backFrame = new AspNetCore.ReportingServices.ReportIntermediateFormat.BackFrame(gaugePanel);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, false);
							backFrame.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, this.m_errorContext);
							break;
						}
						case "FrameStyle":
							backFrame.FrameStyle = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!backFrame.FrameStyle.IsExpression)
							{
								Validator.ValidateGaugeFrameStyles(backFrame.FrameStyle.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "FrameShape":
							backFrame.FrameShape = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!backFrame.FrameShape.IsExpression)
							{
								Validator.ValidateGaugeFrameShapes(backFrame.FrameShape.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "FrameWidth":
							backFrame.FrameWidth = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "GlassEffect":
							backFrame.GlassEffect = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!backFrame.GlassEffect.IsExpression)
							{
								Validator.ValidateGaugeGlassEffects(backFrame.GlassEffect.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "FrameBackground":
							backFrame.FrameBackground = this.ReadFrameBackground(gaugePanel, context);
							break;
						case "FrameImage":
							backFrame.FrameImage = this.ReadFrameImage(gaugePanel, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("BackFrame" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return backFrame;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.FrameBackground ReadFrameBackground(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.FrameBackground frameBackground = new AspNetCore.ReportingServices.ReportIntermediateFormat.FrameBackground(gaugePanel);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "Style")
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, false);
							frameBackground.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, this.m_errorContext);
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("FrameBackground" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return frameBackground;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.CustomLabel ReadCustomLabel(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.CustomLabel customLabel = new AspNetCore.ReportingServices.ReportIntermediateFormat.CustomLabel(gaugePanel);
			customLabel.Name = this.m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "CustomLabel", AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, gaugePanel.Name, customLabel.Name, this.m_errorContext);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, false);
							customLabel.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, this.m_errorContext);
							break;
						}
						case "Text":
							customLabel.Text = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "AllowUpsideDown":
							customLabel.AllowUpsideDown = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "DistanceFromScale":
							customLabel.DistanceFromScale = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "FontAngle":
							customLabel.FontAngle = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Placement":
							customLabel.Placement = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!customLabel.Placement.IsExpression)
							{
								Validator.ValidateGaugeLabelPlacements(customLabel.Placement.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "RotateLabel":
							customLabel.RotateLabel = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "TickMarkStyle":
							customLabel.TickMarkStyle = this.ReadTickMarkStyle(gaugePanel, context);
							break;
						case "Value":
							customLabel.Value = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Hidden":
							customLabel.Hidden = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "UseFontPercent":
							customLabel.UseFontPercent = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("CustomLabel" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return customLabel;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.CustomLabel> ReadCustomLabels(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.CustomLabel> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.CustomLabel>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "CustomLabel")
						{
							list.Add(this.ReadCustomLabel(gaugePanel, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "CustomLabels")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.TickMarkStyle ReadTickMarkStyle(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.TickMarkStyle tickMarkStyle = new AspNetCore.ReportingServices.ReportIntermediateFormat.TickMarkStyle(gaugePanel);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, false);
							tickMarkStyle.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, this.m_errorContext);
							break;
						}
						case "DistanceFromScale":
							tickMarkStyle.DistanceFromScale = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Placement":
							tickMarkStyle.Placement = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!tickMarkStyle.Placement.IsExpression)
							{
								Validator.ValidateGaugeLabelPlacements(tickMarkStyle.Placement.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "EnableGradient":
							tickMarkStyle.EnableGradient = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "GradientDensity":
							tickMarkStyle.GradientDensity = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "TickMarkImage":
							tickMarkStyle.TickMarkImage = this.ReadTopImage(gaugePanel, context, "TickMarkImage");
							break;
						case "Length":
							tickMarkStyle.Length = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Width":
							tickMarkStyle.Width = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Shape":
							tickMarkStyle.Shape = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!tickMarkStyle.Shape.IsExpression)
							{
								Validator.ValidateGaugeTickMarkShapes(tickMarkStyle.Shape.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "Hidden":
							tickMarkStyle.Hidden = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("TickMarkStyle" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return tickMarkStyle;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeTickMarks ReadGaugeTickMarks(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, string elementName)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeTickMarks gaugeTickMarks = new AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeTickMarks(gaugePanel);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Interval":
							gaugeTickMarks.Interval = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "IntervalOffset":
							gaugeTickMarks.IntervalOffset = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, false);
							gaugeTickMarks.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, this.m_errorContext);
							break;
						}
						case "DistanceFromScale":
							gaugeTickMarks.DistanceFromScale = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Placement":
							gaugeTickMarks.Placement = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!gaugeTickMarks.Placement.IsExpression)
							{
								Validator.ValidateGaugeLabelPlacements(gaugeTickMarks.Placement.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "EnableGradient":
							gaugeTickMarks.EnableGradient = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "GradientDensity":
							gaugeTickMarks.GradientDensity = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "TickMarkImage":
							gaugeTickMarks.TickMarkImage = this.ReadTopImage(gaugePanel, context, "TickMarkImage");
							break;
						case "Length":
							gaugeTickMarks.Length = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Width":
							gaugeTickMarks.Width = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Shape":
							gaugeTickMarks.Shape = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!gaugeTickMarks.Shape.IsExpression)
							{
								Validator.ValidateGaugeTickMarkShapes(gaugeTickMarks.Shape.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "Hidden":
							gaugeTickMarks.Hidden = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if (elementName == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return gaugeTickMarks;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeImage ReadGaugeImage(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeImage gaugeImage = new AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeImage(gaugePanel, gaugePanel.GenerateActionOwnerID());
			gaugeImage.Name = this.m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "GaugeImage", AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, gaugePanel.Name, gaugeImage.Name, this.m_errorContext);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				bool flag2 = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, false);
							gaugeImage.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, this.m_errorContext);
							break;
						}
						case "ActionInfo":
							gaugeImage.Action = this.ReadActionInfo(context, StyleOwnerType.Chart, out flag2);
							break;
						case "Top":
							gaugeImage.Top = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Left":
							gaugeImage.Left = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Height":
							gaugeImage.Height = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Width":
							gaugeImage.Width = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "ZIndex":
							gaugeImage.ZIndex = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "Hidden":
							gaugeImage.Hidden = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ToolTip":
							gaugeImage.ToolTip = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ParentItem":
							gaugeImage.ParentItem = this.m_reader.ReadString();
							break;
						case "Source":
							gaugeImage.Source = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Value":
							gaugeImage.Value = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "TransparentColor":
							gaugeImage.TransparentColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("GaugeImage" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return gaugeImage;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeImage> ReadGaugeImages(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeImage> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeImage>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "GaugeImage")
						{
							list.Add(this.ReadGaugeImage(gaugePanel, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "GaugeImages")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeInputValue ReadGaugeInputValue(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, string inputValueName)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeInputValue gaugeInputValue = new AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeInputValue(gaugePanel);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Value":
							gaugeInputValue.Value = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Formula":
							gaugeInputValue.Formula = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!gaugeInputValue.Formula.IsExpression)
							{
								Validator.ValidateGaugeInputValueFormulas(gaugeInputValue.Formula.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "MinPercent":
							gaugeInputValue.MinPercent = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "MaxPercent":
							gaugeInputValue.MaxPercent = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Multiplier":
							gaugeInputValue.Multiplier = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "AddConstant":
							gaugeInputValue.AddConstant = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "DataElementName":
							gaugeInputValue.DataElementName = this.m_reader.ReadString();
							break;
						case "DataElementOutput":
							gaugeInputValue.DataElementOutput = this.ReadDataElementOutput();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if (inputValueName == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return gaugeInputValue;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeLabel ReadGaugeLabel(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeLabel gaugeLabel = new AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeLabel(gaugePanel, gaugePanel.GenerateActionOwnerID());
			gaugeLabel.Name = this.m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "GaugeLabel", AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, gaugePanel.Name, gaugeLabel.Name, this.m_errorContext);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				bool flag2 = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.FilterGaugeLabelStyle();
							gaugeLabel.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, this.m_errorContext);
							break;
						}
						case "ActionInfo":
							gaugeLabel.Action = this.ReadActionInfo(context, StyleOwnerType.Chart, out flag2);
							break;
						case "Top":
							gaugeLabel.Top = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Left":
							gaugeLabel.Left = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Height":
							gaugeLabel.Height = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Width":
							gaugeLabel.Width = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "ZIndex":
							gaugeLabel.ZIndex = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "Hidden":
							gaugeLabel.Hidden = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ToolTip":
							gaugeLabel.ToolTip = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ParentItem":
							gaugeLabel.ParentItem = this.m_reader.ReadString();
							break;
						case "Text":
							gaugeLabel.Text = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Angle":
							gaugeLabel.Angle = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "ResizeMode":
							gaugeLabel.ResizeMode = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!gaugeLabel.ResizeMode.IsExpression)
							{
								Validator.ValidateGaugeResizeModes(gaugeLabel.ResizeMode.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "TextShadowOffset":
							gaugeLabel.TextShadowOffset = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "UseFontPercent":
							gaugeLabel.UseFontPercent = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("GaugeLabel" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return gaugeLabel;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeLabel> ReadGaugeLabels(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeLabel> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeLabel>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "GaugeLabel")
						{
							list.Add(this.ReadGaugeLabel(gaugePanel, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "GaugeLabels")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.LinearGauge ReadLinearGauge(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.LinearGauge linearGauge = new AspNetCore.ReportingServices.ReportIntermediateFormat.LinearGauge(gaugePanel, gaugePanel.GenerateActionOwnerID());
			linearGauge.Name = this.m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "LinearGauge", AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, gaugePanel.Name, linearGauge.Name, this.m_errorContext);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				bool flag2 = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, false);
							linearGauge.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, this.m_errorContext);
							break;
						}
						case "ActionInfo":
							linearGauge.Action = this.ReadActionInfo(context, StyleOwnerType.Chart, out flag2);
							break;
						case "Top":
							linearGauge.Top = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Left":
							linearGauge.Left = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Height":
							linearGauge.Height = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Width":
							linearGauge.Width = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "ZIndex":
							linearGauge.ZIndex = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "Hidden":
							linearGauge.Hidden = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ToolTip":
							linearGauge.ToolTip = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ParentItem":
							linearGauge.ParentItem = this.m_reader.ReadString();
							break;
						case "BackFrame":
							linearGauge.BackFrame = this.ReadBackFrame(gaugePanel, context);
							break;
						case "ClipContent":
							linearGauge.ClipContent = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "TopImage":
							linearGauge.TopImage = this.ReadTopImage(gaugePanel, context, "TopImage");
							break;
						case "GaugeScales":
							linearGauge.GaugeScales = this.ReadLinearScales(gaugePanel, context);
							break;
						case "Orientation":
							linearGauge.Orientation = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!linearGauge.Orientation.IsExpression)
							{
								Validator.ValidateGaugeOrientations(linearGauge.Orientation.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "AspectRatio":
							linearGauge.AspectRatio = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("LinearGauge" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return linearGauge;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.LinearGauge> ReadLinearGauges(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.LinearGauge> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.LinearGauge>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "LinearGauge")
						{
							list.Add(this.ReadLinearGauge(gaugePanel, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "LinearGauges")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.LinearPointer ReadLinearPointer(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.LinearPointer linearPointer = new AspNetCore.ReportingServices.ReportIntermediateFormat.LinearPointer(gaugePanel, gaugePanel.GenerateActionOwnerID());
			linearPointer.Name = this.m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "LinearPointer", AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, gaugePanel.Name, linearPointer.Name, this.m_errorContext);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				bool flag2 = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, false);
							linearPointer.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, this.m_errorContext);
							break;
						}
						case "ActionInfo":
							linearPointer.Action = this.ReadActionInfo(context, StyleOwnerType.Chart, out flag2);
							break;
						case "GaugeInputValue":
							linearPointer.GaugeInputValue = this.ReadGaugeInputValue(gaugePanel, context, "GaugeInputValue");
							break;
						case "BarStart":
							linearPointer.BarStart = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!linearPointer.BarStart.IsExpression)
							{
								Validator.ValidateGaugeBarStarts(linearPointer.BarStart.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "DistanceFromScale":
							linearPointer.DistanceFromScale = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "PointerImage":
							linearPointer.PointerImage = this.ReadPointerImage(gaugePanel, context);
							break;
						case "MarkerLength":
							linearPointer.MarkerLength = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "MarkerStyle":
							linearPointer.MarkerStyle = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!linearPointer.MarkerStyle.IsExpression)
							{
								Validator.ValidateGaugeMarkerStyles(linearPointer.MarkerStyle.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "Placement":
							linearPointer.Placement = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!linearPointer.Placement.IsExpression)
							{
								Validator.ValidateGaugePointerPlacements(linearPointer.Placement.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "SnappingEnabled":
							linearPointer.SnappingEnabled = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "SnappingInterval":
							linearPointer.SnappingInterval = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "ToolTip":
							linearPointer.ToolTip = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Hidden":
							linearPointer.Hidden = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Width":
							linearPointer.Width = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Type":
							linearPointer.Type = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!linearPointer.Type.IsExpression)
							{
								Validator.ValidateLinearPointerTypes(linearPointer.Type.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "Thermometer":
							linearPointer.Thermometer = this.ReadThermometer(gaugePanel, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("LinearPointer" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return linearPointer;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.LinearPointer> ReadLinearPointers(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.LinearPointer> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.LinearPointer>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "LinearPointer")
						{
							list.Add(this.ReadLinearPointer(gaugePanel, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "GaugePointers")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.LinearScale ReadLinearScale(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.LinearScale linearScale = new AspNetCore.ReportingServices.ReportIntermediateFormat.LinearScale(gaugePanel, gaugePanel.GenerateActionOwnerID());
			linearScale.Name = this.m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "LinearScale", AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, gaugePanel.Name, linearScale.Name, this.m_errorContext);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				bool flag2 = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, false);
							linearScale.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, this.m_errorContext);
							break;
						}
						case "ActionInfo":
							linearScale.Action = this.ReadActionInfo(context, StyleOwnerType.Chart, out flag2);
							break;
						case "ScaleRanges":
							linearScale.ScaleRanges = this.ReadScaleRanges(gaugePanel, context);
							break;
						case "CustomLabels":
							linearScale.CustomLabels = this.ReadCustomLabels(gaugePanel, context);
							break;
						case "Interval":
							linearScale.Interval = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "IntervalOffset":
							linearScale.IntervalOffset = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Logarithmic":
							linearScale.Logarithmic = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "LogarithmicBase":
							linearScale.LogarithmicBase = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "MaximumValue":
							linearScale.MaximumValue = this.ReadGaugeInputValue(gaugePanel, context, "MaximumValue");
							break;
						case "MinimumValue":
							linearScale.MinimumValue = this.ReadGaugeInputValue(gaugePanel, context, "MinimumValue");
							break;
						case "Multiplier":
							linearScale.Multiplier = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Reversed":
							linearScale.Reversed = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "GaugeMajorTickMarks":
							linearScale.GaugeMajorTickMarks = this.ReadGaugeTickMarks(gaugePanel, context, "GaugeMajorTickMarks");
							break;
						case "GaugeMinorTickMarks":
							linearScale.GaugeMinorTickMarks = this.ReadGaugeTickMarks(gaugePanel, context, "GaugeMinorTickMarks");
							break;
						case "MaximumPin":
							linearScale.MaximumPin = this.ReadScalePin(gaugePanel, context, "MaximumPin");
							break;
						case "MinimumPin":
							linearScale.MinimumPin = this.ReadScalePin(gaugePanel, context, "MinimumPin");
							break;
						case "ScaleLabels":
							linearScale.ScaleLabels = this.ReadScaleLabels(gaugePanel, context);
							break;
						case "TickMarksOnTop":
							linearScale.TickMarksOnTop = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ToolTip":
							linearScale.ToolTip = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Hidden":
							linearScale.Hidden = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Width":
							linearScale.Width = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "GaugePointers":
							linearScale.GaugePointers = this.ReadLinearPointers(gaugePanel, context);
							break;
						case "StartMargin":
							linearScale.StartMargin = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "EndMargin":
							linearScale.EndMargin = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Position":
							linearScale.Position = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("LinearScale" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return linearScale;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.LinearScale> ReadLinearScales(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.LinearScale> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.LinearScale>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "LinearScale")
						{
							list.Add(this.ReadLinearScale(gaugePanel, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "GaugeScales")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.NumericIndicator ReadNumericIndicator(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator = new AspNetCore.ReportingServices.ReportIntermediateFormat.NumericIndicator(gaugePanel, gaugePanel.GenerateActionOwnerID());
			numericIndicator.Name = this.m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "NumericIndicator", AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, gaugePanel.Name, numericIndicator.Name, this.m_errorContext);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				bool flag2 = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, false);
							numericIndicator.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, this.m_errorContext);
							break;
						}
						case "ActionInfo":
							numericIndicator.Action = this.ReadActionInfo(context, StyleOwnerType.Chart, out flag2);
							break;
						case "Top":
							numericIndicator.Top = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Left":
							numericIndicator.Left = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Height":
							numericIndicator.Height = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Width":
							numericIndicator.Width = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "ZIndex":
							numericIndicator.ZIndex = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "Hidden":
							numericIndicator.Hidden = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ToolTip":
							numericIndicator.ToolTip = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ParentItem":
							numericIndicator.ParentItem = this.m_reader.ReadString();
							break;
						case "GaugeInputValue":
							numericIndicator.GaugeInputValue = this.ReadGaugeInputValue(gaugePanel, context, "GaugeInputValue");
							break;
						case "NumericIndicatorRanges":
							numericIndicator.NumericIndicatorRanges = this.ReadNumericIndicatorRanges(gaugePanel, context);
							break;
						case "DecimalDigitColor":
							numericIndicator.DecimalDigitColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "DigitColor":
							numericIndicator.DigitColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "UseFontPercent":
							numericIndicator.UseFontPercent = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "DecimalDigits":
							numericIndicator.DecimalDigits = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "Digits":
							numericIndicator.Digits = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "MinimumValue":
							numericIndicator.MinimumValue = this.ReadGaugeInputValue(gaugePanel, context, "MinimumValue");
							break;
						case "MaximumValue":
							numericIndicator.MaximumValue = this.ReadGaugeInputValue(gaugePanel, context, "MaximumValue");
							break;
						case "Multiplier":
							numericIndicator.Multiplier = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "NonNumericString":
							numericIndicator.NonNumericString = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "OutOfRangeString":
							numericIndicator.OutOfRangeString = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ResizeMode":
							numericIndicator.ResizeMode = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!numericIndicator.ResizeMode.IsExpression)
							{
								Validator.ValidateGaugeResizeModes(numericIndicator.ResizeMode.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "ShowDecimalPoint":
							numericIndicator.ShowDecimalPoint = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ShowLeadingZeros":
							numericIndicator.ShowLeadingZeros = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "IndicatorStyle":
							numericIndicator.IndicatorStyle = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!numericIndicator.IndicatorStyle.IsExpression)
							{
								Validator.ValidateGaugeIndicatorStyles(numericIndicator.IndicatorStyle.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "ShowSign":
							numericIndicator.ShowSign = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!numericIndicator.ShowSign.IsExpression)
							{
								Validator.ValidateGaugeShowSigns(numericIndicator.ShowSign.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "SnappingEnabled":
							numericIndicator.SnappingEnabled = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "SnappingInterval":
							numericIndicator.SnappingInterval = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "LedDimColor":
							numericIndicator.LedDimColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "SeparatorWidth":
							numericIndicator.SeparatorWidth = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "SeparatorColor":
							numericIndicator.SeparatorColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("NumericIndicator" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return numericIndicator;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.NumericIndicator> ReadNumericIndicators(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.NumericIndicator> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.NumericIndicator>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "NumericIndicator")
						{
							list.Add(this.ReadNumericIndicator(gaugePanel, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "NumericIndicators")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private NumericIndicatorRange ReadNumericIndicatorRange(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			NumericIndicatorRange numericIndicatorRange = new NumericIndicatorRange(gaugePanel);
			numericIndicatorRange.Name = this.m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "NumericIndicatorRange", AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, gaugePanel.Name, numericIndicatorRange.Name, this.m_errorContext);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadNumericIndicatorRangeElement(gaugePanel, numericIndicatorRange, context);
						break;
					case XmlNodeType.EndElement:
						if ("NumericIndicatorRange" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return numericIndicatorRange;
		}

		private void ReadNumericIndicatorRangeElement(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, NumericIndicatorRange numericIndicatorRange, PublishingContextStruct context)
		{
			string localName;
			if ((localName = this.m_reader.LocalName) != null)
			{
				if (!(localName == "StartValue"))
				{
					if (!(localName == "EndValue"))
					{
						if (!(localName == "DecimalDigitColor"))
						{
							if (localName == "DigitColor")
							{
								numericIndicatorRange.DigitColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							}
						}
						else
						{
							numericIndicatorRange.DecimalDigitColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
						}
					}
					else
					{
						numericIndicatorRange.EndValue = this.ReadGaugeInputValue(gaugePanel, context, "EndValue");
					}
				}
				else
				{
					numericIndicatorRange.StartValue = this.ReadGaugeInputValue(gaugePanel, context, "StartValue");
				}
			}
		}

		private List<NumericIndicatorRange> ReadNumericIndicatorRanges(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<NumericIndicatorRange> list = new List<NumericIndicatorRange>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "NumericIndicatorRange")
						{
							list.Add(this.ReadNumericIndicatorRange(gaugePanel, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "NumericIndicatorRanges")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.PinLabel ReadPinLabel(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.PinLabel pinLabel = new AspNetCore.ReportingServices.ReportIntermediateFormat.PinLabel(gaugePanel);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, false);
							pinLabel.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, this.m_errorContext);
							break;
						}
						case "Text":
							pinLabel.Text = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "AllowUpsideDown":
							pinLabel.AllowUpsideDown = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "DistanceFromScale":
							pinLabel.DistanceFromScale = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "FontAngle":
							pinLabel.FontAngle = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Placement":
							pinLabel.Placement = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!pinLabel.Placement.IsExpression)
							{
								Validator.ValidateGaugeLabelPlacements(pinLabel.Placement.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "RotateLabel":
							pinLabel.RotateLabel = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "UseFontPercent":
							pinLabel.UseFontPercent = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("PinLabel" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return pinLabel;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.PointerCap ReadPointerCap(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.PointerCap pointerCap = new AspNetCore.ReportingServices.ReportIntermediateFormat.PointerCap(gaugePanel);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, false);
							pointerCap.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, this.m_errorContext);
							break;
						}
						case "CapImage":
							pointerCap.CapImage = this.ReadCapImage(gaugePanel, context);
							break;
						case "OnTop":
							pointerCap.OnTop = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Reflection":
							pointerCap.Reflection = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "CapStyle":
							pointerCap.CapStyle = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!pointerCap.CapStyle.IsExpression)
							{
								Validator.ValidateGaugeCapStyles(pointerCap.CapStyle.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "Hidden":
							pointerCap.Hidden = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Width":
							pointerCap.Width = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("PointerCap" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return pointerCap;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.RadialGauge ReadRadialGauge(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.RadialGauge radialGauge = new AspNetCore.ReportingServices.ReportIntermediateFormat.RadialGauge(gaugePanel, gaugePanel.GenerateActionOwnerID());
			radialGauge.Name = this.m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "RadialGauge", AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, gaugePanel.Name, radialGauge.Name, this.m_errorContext);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				bool flag2 = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, false);
							radialGauge.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, this.m_errorContext);
							break;
						}
						case "ActionInfo":
							radialGauge.Action = this.ReadActionInfo(context, StyleOwnerType.Chart, out flag2);
							break;
						case "Top":
							radialGauge.Top = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Left":
							radialGauge.Left = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Height":
							radialGauge.Height = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Width":
							radialGauge.Width = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "ZIndex":
							radialGauge.ZIndex = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "Hidden":
							radialGauge.Hidden = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ToolTip":
							radialGauge.ToolTip = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ParentItem":
							radialGauge.ParentItem = this.m_reader.ReadString();
							break;
						case "BackFrame":
							radialGauge.BackFrame = this.ReadBackFrame(gaugePanel, context);
							break;
						case "ClipContent":
							radialGauge.ClipContent = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "TopImage":
							radialGauge.TopImage = this.ReadTopImage(gaugePanel, context, "TopImage");
							break;
						case "GaugeScales":
							radialGauge.GaugeScales = this.ReadRadialScales(gaugePanel, context);
							break;
						case "PivotX":
							radialGauge.PivotX = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "PivotY":
							radialGauge.PivotY = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "AspectRatio":
							radialGauge.AspectRatio = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("RadialGauge" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return radialGauge;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.RadialGauge> ReadRadialGauges(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.RadialGauge> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.RadialGauge>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "RadialGauge")
						{
							list.Add(this.ReadRadialGauge(gaugePanel, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "RadialGauges")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.RadialPointer ReadRadialPointer(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.RadialPointer radialPointer = new AspNetCore.ReportingServices.ReportIntermediateFormat.RadialPointer(gaugePanel, gaugePanel.GenerateActionOwnerID());
			radialPointer.Name = this.m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "RadialPointer", AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, gaugePanel.Name, radialPointer.Name, this.m_errorContext);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				bool flag2 = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, false);
							radialPointer.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, this.m_errorContext);
							break;
						}
						case "ActionInfo":
							radialPointer.Action = this.ReadActionInfo(context, StyleOwnerType.Chart, out flag2);
							break;
						case "GaugeInputValue":
							radialPointer.GaugeInputValue = this.ReadGaugeInputValue(gaugePanel, context, "GaugeInputValue");
							break;
						case "BarStart":
							radialPointer.BarStart = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!radialPointer.BarStart.IsExpression)
							{
								Validator.ValidateGaugeBarStarts(radialPointer.BarStart.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "DistanceFromScale":
							radialPointer.DistanceFromScale = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "PointerImage":
							radialPointer.PointerImage = this.ReadPointerImage(gaugePanel, context);
							break;
						case "MarkerLength":
							radialPointer.MarkerLength = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "MarkerStyle":
							radialPointer.MarkerStyle = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!radialPointer.MarkerStyle.IsExpression)
							{
								Validator.ValidateGaugeMarkerStyles(radialPointer.MarkerStyle.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "Placement":
							radialPointer.Placement = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!radialPointer.Placement.IsExpression)
							{
								Validator.ValidateGaugePointerPlacements(radialPointer.Placement.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "SnappingEnabled":
							radialPointer.SnappingEnabled = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "SnappingInterval":
							radialPointer.SnappingInterval = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "ToolTip":
							radialPointer.ToolTip = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Hidden":
							radialPointer.Hidden = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Width":
							radialPointer.Width = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Type":
							radialPointer.Type = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!radialPointer.Type.IsExpression)
							{
								Validator.ValidateRadialPointerTypes(radialPointer.Type.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "PointerCap":
							radialPointer.PointerCap = this.ReadPointerCap(gaugePanel, context);
							break;
						case "NeedleStyle":
							radialPointer.NeedleStyle = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!radialPointer.NeedleStyle.IsExpression)
							{
								Validator.ValidateRadialPointerNeedleStyles(radialPointer.NeedleStyle.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("RadialPointer" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return radialPointer;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.RadialPointer> ReadRadialPointers(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.RadialPointer> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.RadialPointer>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "RadialPointer")
						{
							list.Add(this.ReadRadialPointer(gaugePanel, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "GaugePointers")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.RadialScale ReadRadialScale(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.RadialScale radialScale = new AspNetCore.ReportingServices.ReportIntermediateFormat.RadialScale(gaugePanel, gaugePanel.GenerateActionOwnerID());
			radialScale.Name = this.m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "RadialScale", AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, gaugePanel.Name, radialScale.Name, this.m_errorContext);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				bool flag2 = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "GaugePointers":
							radialScale.GaugePointers = this.ReadRadialPointers(gaugePanel, context);
							break;
						case "Radius":
							radialScale.Radius = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "StartAngle":
							radialScale.StartAngle = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "SweepAngle":
							radialScale.SweepAngle = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, false);
							radialScale.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, this.m_errorContext);
							break;
						}
						case "ActionInfo":
							radialScale.Action = this.ReadActionInfo(context, StyleOwnerType.Chart, out flag2);
							break;
						case "ScaleRanges":
							radialScale.ScaleRanges = this.ReadScaleRanges(gaugePanel, context);
							break;
						case "CustomLabels":
							radialScale.CustomLabels = this.ReadCustomLabels(gaugePanel, context);
							break;
						case "Interval":
							radialScale.Interval = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "IntervalOffset":
							radialScale.IntervalOffset = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Logarithmic":
							radialScale.Logarithmic = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "LogarithmicBase":
							radialScale.LogarithmicBase = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "MaximumValue":
							radialScale.MaximumValue = this.ReadGaugeInputValue(gaugePanel, context, "MaximumValue");
							break;
						case "MinimumValue":
							radialScale.MinimumValue = this.ReadGaugeInputValue(gaugePanel, context, "MinimumValue");
							break;
						case "Multiplier":
							radialScale.Multiplier = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Reversed":
							radialScale.Reversed = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "GaugeMajorTickMarks":
							radialScale.GaugeMajorTickMarks = this.ReadGaugeTickMarks(gaugePanel, context, "GaugeMajorTickMarks");
							break;
						case "GaugeMinorTickMarks":
							radialScale.GaugeMinorTickMarks = this.ReadGaugeTickMarks(gaugePanel, context, "GaugeMinorTickMarks");
							break;
						case "MaximumPin":
							radialScale.MaximumPin = this.ReadScalePin(gaugePanel, context, "MaximumPin");
							break;
						case "MinimumPin":
							radialScale.MinimumPin = this.ReadScalePin(gaugePanel, context, "MinimumPin");
							break;
						case "ScaleLabels":
							radialScale.ScaleLabels = this.ReadScaleLabels(gaugePanel, context);
							break;
						case "TickMarksOnTop":
							radialScale.TickMarksOnTop = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ToolTip":
							radialScale.ToolTip = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Hidden":
							radialScale.Hidden = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Width":
							radialScale.Width = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("RadialScale" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return radialScale;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.RadialScale> ReadRadialScales(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.RadialScale> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.RadialScale>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "RadialScale")
						{
							list.Add(this.ReadRadialScale(gaugePanel, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "GaugeScales")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleLabels ReadScaleLabels(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleLabels scaleLabels = new AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleLabels(gaugePanel);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, false);
							scaleLabels.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, this.m_errorContext);
							break;
						}
						case "Interval":
							scaleLabels.Interval = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "IntervalOffset":
							scaleLabels.IntervalOffset = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "AllowUpsideDown":
							scaleLabels.AllowUpsideDown = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "DistanceFromScale":
							scaleLabels.DistanceFromScale = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "FontAngle":
							scaleLabels.FontAngle = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Placement":
							scaleLabels.Placement = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!scaleLabels.Placement.IsExpression)
							{
								Validator.ValidateGaugeLabelPlacements(scaleLabels.Placement.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "RotateLabels":
							scaleLabels.RotateLabels = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ShowEndLabels":
							scaleLabels.ShowEndLabels = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Hidden":
							scaleLabels.Hidden = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "UseFontPercent":
							scaleLabels.UseFontPercent = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ScaleLabels" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return scaleLabels;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ScalePin ReadScalePin(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, string elementName)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ScalePin scalePin = new AspNetCore.ReportingServices.ReportIntermediateFormat.ScalePin(gaugePanel);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, false);
							scalePin.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, this.m_errorContext);
							break;
						}
						case "DistanceFromScale":
							scalePin.DistanceFromScale = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Placement":
							scalePin.Placement = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!scalePin.Placement.IsExpression)
							{
								Validator.ValidateGaugeLabelPlacements(scalePin.Placement.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "EnableGradient":
							scalePin.EnableGradient = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "GradientDensity":
							scalePin.GradientDensity = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "TickMarkImage":
							scalePin.TickMarkImage = this.ReadTopImage(gaugePanel, context, "TickMarkImage");
							break;
						case "Length":
							scalePin.Length = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Width":
							scalePin.Width = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Shape":
							scalePin.Shape = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!scalePin.Shape.IsExpression)
							{
								Validator.ValidateGaugeTickMarkShapes(scalePin.Shape.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "Hidden":
							scalePin.Hidden = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "Location":
							scalePin.Location = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Enable":
							scalePin.Enable = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "PinLabel":
							scalePin.PinLabel = this.ReadPinLabel(gaugePanel, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if (elementName == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return scalePin;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleRange ReadScaleRange(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange = new AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleRange(gaugePanel, gaugePanel.GenerateActionOwnerID());
			scaleRange.Name = this.m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "ScaleRange", AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, gaugePanel.Name, scaleRange.Name, this.m_errorContext);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				bool flag2 = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, false);
							scaleRange.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, this.m_errorContext);
							break;
						}
						case "ActionInfo":
							scaleRange.Action = this.ReadActionInfo(context, StyleOwnerType.Chart, out flag2);
							break;
						case "DistanceFromScale":
							scaleRange.DistanceFromScale = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "StartValue":
							scaleRange.StartValue = this.ReadGaugeInputValue(gaugePanel, context, "StartValue");
							break;
						case "EndValue":
							scaleRange.EndValue = this.ReadGaugeInputValue(gaugePanel, context, "EndValue");
							break;
						case "StartWidth":
							scaleRange.StartWidth = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "EndWidth":
							scaleRange.EndWidth = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "InRangeBarPointerColor":
							scaleRange.InRangeBarPointerColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "InRangeLabelColor":
							scaleRange.InRangeLabelColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "InRangeTickMarksColor":
							scaleRange.InRangeTickMarksColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "BackgroundGradientType":
							scaleRange.BackgroundGradientType = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!scaleRange.BackgroundGradientType.IsExpression)
							{
								Validator.ValidateBackgroundGradientTypes(scaleRange.BackgroundGradientType.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "Placement":
							scaleRange.Placement = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!scaleRange.Placement.IsExpression)
							{
								Validator.ValidateScaleRangePlacements(scaleRange.Placement.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "ToolTip":
							scaleRange.ToolTip = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "Hidden":
							scaleRange.Hidden = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ScaleRange" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return scaleRange;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleRange> ReadScaleRanges(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleRange> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleRange>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "ScaleRange")
						{
							list.Add(this.ReadScaleRange(gaugePanel, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "ScaleRanges")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.StateIndicator ReadStateIndicator(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.StateIndicator stateIndicator = new AspNetCore.ReportingServices.ReportIntermediateFormat.StateIndicator(gaugePanel, gaugePanel.GenerateActionOwnerID());
			stateIndicator.Name = this.m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "StateIndicator", AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, gaugePanel.Name, stateIndicator.Name, this.m_errorContext);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				bool flag2 = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, false);
							stateIndicator.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, this.m_errorContext);
							break;
						}
						case "ActionInfo":
							stateIndicator.Action = this.ReadActionInfo(context, StyleOwnerType.Chart, out flag2);
							break;
						case "Top":
							stateIndicator.Top = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Left":
							stateIndicator.Left = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Height":
							stateIndicator.Height = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "Width":
							stateIndicator.Width = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "ZIndex":
							stateIndicator.ZIndex = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
							break;
						case "Hidden":
							stateIndicator.Hidden = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							break;
						case "ToolTip":
							stateIndicator.ToolTip = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "ParentItem":
							stateIndicator.ParentItem = this.m_reader.ReadString();
							break;
						case "GaugeInputValue":
							stateIndicator.GaugeInputValue = this.ReadGaugeInputValue(gaugePanel, context, "GaugeInputValue");
							break;
						case "TransformationType":
							stateIndicator.TransformationType = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!stateIndicator.TransformationType.IsExpression)
							{
								Validator.ValidateGaugeTransformationType(stateIndicator.TransformationType.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "TransformationScope":
							stateIndicator.TransformationScope = this.m_reader.ReadString();
							break;
						case "MaximumValue":
							stateIndicator.MaximumValue = this.ReadGaugeInputValue(gaugePanel, context, "MaximumValue");
							break;
						case "MinimumValue":
							stateIndicator.MinimumValue = this.ReadGaugeInputValue(gaugePanel, context, "MinimumValue");
							break;
						case "IndicatorStyle":
							stateIndicator.IndicatorStyle = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!stateIndicator.IndicatorStyle.IsExpression)
							{
								Validator.ValidateGaugeStateIndicatorStyles(stateIndicator.IndicatorStyle.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "IndicatorImage":
							stateIndicator.IndicatorImage = this.ReadIndicatorImage(gaugePanel, context);
							break;
						case "ScaleFactor":
							stateIndicator.ScaleFactor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "IndicatorStates":
							stateIndicator.IndicatorStates = this.ReadIndicatorStates(gaugePanel, context);
							break;
						case "ResizeMode":
							stateIndicator.ResizeMode = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!stateIndicator.ResizeMode.IsExpression)
							{
								Validator.ValidateGaugeResizeModes(stateIndicator.ResizeMode.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "Angle":
							stateIndicator.Angle = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "StateDataElementName":
							stateIndicator.StateDataElementName = this.m_reader.ReadString();
							break;
						case "StateDataElementOutput":
							stateIndicator.StateDataElementOutput = this.ReadDataElementOutput();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("StateIndicator" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			this.InitializeStateIndicatorMinMax(gaugePanel, stateIndicator, context);
			return stateIndicator;
		}

		private void InitializeStateIndicatorMinMax(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, AspNetCore.ReportingServices.ReportIntermediateFormat.StateIndicator stateIndicator, PublishingContextStruct context)
		{
			if (stateIndicator.TransformationType == null || Validator.IsStateIndicatorTransformationTypePercent(stateIndicator.TransformationType.StringValue) || stateIndicator.TransformationType.IsExpression)
			{
				string text = this.GenerateStateIndicatorAutoMinMaxExpression(gaugePanel, stateIndicator, false);
				if (text != null)
				{
					stateIndicator.MinimumValue = new AutoGeneratedGaugeInputValue(gaugePanel, stateIndicator.Name);
					stateIndicator.MinimumValue.Value = this.ReadExpression(text, "Value", null, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.EvaluationMode.Auto, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				}
				text = this.GenerateStateIndicatorAutoMinMaxExpression(gaugePanel, stateIndicator, true);
				if (text != null)
				{
					stateIndicator.MaximumValue = new AutoGeneratedGaugeInputValue(gaugePanel, stateIndicator.Name);
					stateIndicator.MaximumValue.Value = this.ReadExpression(text, "Value", null, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.EvaluationMode.Auto, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				}
			}
		}

		private string GenerateStateIndicatorAutoMinMaxExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, AspNetCore.ReportingServices.ReportIntermediateFormat.StateIndicator stateIndicator, bool max)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeInputValue gaugeInputValue = max ? stateIndicator.MaximumValue : stateIndicator.MinimumValue;
			if (gaugeInputValue != null && !Validator.CompareWithInvariantCulture(gaugeInputValue.Value.StringValue, "NaN"))
			{
				return null;
			}
			if (string.IsNullOrEmpty(stateIndicator.TransformationScope))
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsStateIndicatorInvalidTransformationScope, Severity.Error, gaugePanel.ObjectType, gaugePanel.Name, "TransformationScope", stateIndicator.Name);
			}
			if (stateIndicator.GaugeInputValue != null && !string.IsNullOrEmpty(stateIndicator.GaugeInputValue.Value.OriginalText))
			{
				string text = stateIndicator.GaugeInputValue.Value.OriginalText.Trim();
				if (text.StartsWith("=", StringComparison.Ordinal))
				{
					text = text.Remove(0, 1);
				}
				if (max)
				{
					return "=Max(" + text + ", \"" + stateIndicator.TransformationScope + "\")";
				}
				return "=IIF(Count(" + text + ", \"" + stateIndicator.TransformationScope + "\")=1, 0, Min(" + text + ", \"" + stateIndicator.TransformationScope + "\"))";
			}
			return null;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.StateIndicator> ReadStateIndicators(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.StateIndicator> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.StateIndicator>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "StateIndicator")
						{
							list.Add(this.ReadStateIndicator(gaugePanel, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "StateIndicators")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Thermometer ReadThermometer(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.Thermometer thermometer = new AspNetCore.ReportingServices.ReportIntermediateFormat.Thermometer(gaugePanel);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.GaugePanel, false);
							thermometer.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, this.m_errorContext);
							break;
						}
						case "BulbOffset":
							thermometer.BulbOffset = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "BulbSize":
							thermometer.BulbSize = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "ThermometerStyle":
							thermometer.ThermometerStyle = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!thermometer.ThermometerStyle.IsExpression)
							{
								Validator.ValidateGaugeThermometerStyles(thermometer.ThermometerStyle.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("Thermometer" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return thermometer;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.IndicatorImage ReadIndicatorImage(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.IndicatorImage indicatorImage = new AspNetCore.ReportingServices.ReportIntermediateFormat.IndicatorImage(gaugePanel);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadIndicatorImageElement(gaugePanel, indicatorImage, context);
						break;
					case XmlNodeType.EndElement:
						if ("IndicatorImage" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return indicatorImage;
		}

		private void ReadIndicatorImageElement(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, AspNetCore.ReportingServices.ReportIntermediateFormat.IndicatorImage indicatorImage, PublishingContextStruct context)
		{
			string localName;
			if ((localName = this.m_reader.LocalName) != null)
			{
				if (!(localName == "Source"))
				{
					if (!(localName == "Value"))
					{
						if (!(localName == "MIMEType"))
						{
							if (!(localName == "HueColor"))
							{
								if (!(localName == "TransparentColor"))
								{
									if (localName == "Transparency")
									{
										indicatorImage.Transparency = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
									}
								}
								else
								{
									indicatorImage.TransparentColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
								}
							}
							else
							{
								indicatorImage.HueColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							}
						}
						else
						{
							indicatorImage.MIMEType = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
						}
					}
					else
					{
						indicatorImage.Value = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
					}
				}
				else
				{
					indicatorImage.Source = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
					if (!indicatorImage.Source.IsExpression)
					{
						Validator.ValidateImageSourceType(indicatorImage.Source.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
					}
				}
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.IndicatorState ReadIndicatorState(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.IndicatorState indicatorState = new AspNetCore.ReportingServices.ReportIntermediateFormat.IndicatorState(gaugePanel);
			indicatorState.Name = this.m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "IndicatorState", AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel, gaugePanel.Name, indicatorState.Name, this.m_errorContext);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadIndicatorStateElement(gaugePanel, indicatorState, context);
						break;
					case XmlNodeType.EndElement:
						if ("IndicatorState" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return indicatorState;
		}

		private void ReadIndicatorStateElement(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, AspNetCore.ReportingServices.ReportIntermediateFormat.IndicatorState indicatorState, PublishingContextStruct context)
		{
			string localName;
			if ((localName = this.m_reader.LocalName) != null)
			{
				if (!(localName == "StartValue"))
				{
					if (!(localName == "EndValue"))
					{
						if (!(localName == "Color"))
						{
							if (!(localName == "ScaleFactor"))
							{
								if (!(localName == "IndicatorStyle"))
								{
									if (localName == "IndicatorImage")
									{
										indicatorState.IndicatorImage = this.ReadIndicatorImage(gaugePanel, context);
									}
								}
								else
								{
									indicatorState.IndicatorStyle = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
									if (!indicatorState.IndicatorStyle.IsExpression)
									{
										Validator.ValidateGaugeStateIndicatorStyles(indicatorState.IndicatorStyle.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
									}
								}
							}
							else
							{
								indicatorState.ScaleFactor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							}
						}
						else
						{
							indicatorState.Color = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
						}
					}
					else
					{
						indicatorState.EndValue = this.ReadGaugeInputValue(gaugePanel, context, "EndValue");
					}
				}
				else
				{
					indicatorState.StartValue = this.ReadGaugeInputValue(gaugePanel, context, "StartValue");
				}
			}
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.IndicatorState> ReadIndicatorStates(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel gaugePanel, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.IndicatorState> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.IndicatorState>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "IndicatorState")
						{
							list.Add(this.ReadIndicatorState(gaugePanel, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "IndicatorStates")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Map ReadMap(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem parent, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.Map map = new AspNetCore.ReportingServices.ReportIntermediateFormat.Map(this.GenerateID(), parent);
			map.Name = this.m_reader.GetAttribute("Name");
			context.ObjectType = map.ObjectType;
			context.ObjectName = map.Name;
			bool flag = true;
			if (!this.m_reportItemNames.Validate(context.ObjectType, context.ObjectName, this.m_errorContext))
			{
				flag = false;
			}
			StyleInformation styleInformation = null;
			double maxValue = 914.4;
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag2 = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						double num = default(double);
						string text = default(string);
						switch (this.m_reader.LocalName)
						{
						case "Style":
							styleInformation = this.ReadStyle(context);
							break;
						case "Top":
							map.Top = this.ReadSize();
							break;
						case "Left":
							map.Left = this.ReadSize();
							break;
						case "Height":
							map.Height = this.ReadSize();
							PublishingValidator.ValidateSize(map.Height, false, 0.0, maxValue, map.ObjectType, map.Name, "Height", (ErrorContext)this.m_errorContext, out num, out text);
							break;
						case "Width":
							map.Width = this.ReadSize();
							PublishingValidator.ValidateSize(map.Width, false, 0.0, maxValue, map.ObjectType, map.Name, "Width", (ErrorContext)this.m_errorContext, out num, out text);
							break;
						case "ZIndex":
							map.ZIndex = this.m_reader.ReadInteger(AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, map.Name, "ZIndex");
							break;
						case "Visibility":
							map.Visibility = this.ReadVisibility(context);
							break;
						case "ToolTip":
							map.ToolTip = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						case "DocumentMapLabel":
							map.DocumentMapLabel = this.ReadDocumentMapLabelExpression(this.m_reader.LocalName, context);
							break;
						case "Bookmark":
							map.Bookmark = this.ReadBookmarkExpression(this.m_reader.LocalName, context);
							break;
						case "CustomProperties":
							map.CustomProperties = this.ReadCustomProperties(context);
							break;
						case "DataElementName":
							map.DataElementName = this.m_reader.ReadString();
							break;
						case "DataElementOutput":
							map.DataElementOutput = this.ReadDataElementOutput();
							break;
						case "PageBreak":
							this.ReadPageBreak(map, context);
							break;
						case "PageName":
							map.PageName = this.ReadPageNameExpression(context);
							break;
						case "MapDataRegions":
							map.MapDataRegions = this.ReadMapDataRegions(map, context);
							break;
						case "MapViewport":
							map.MapViewport = this.ReadMapViewport(map, context);
							break;
						case "MapLayers":
							map.MapLayers = this.ReadMapLayers(map, context);
							break;
						case "MapLegends":
							map.MapLegends = this.ReadMapLegends(map, context);
							break;
						case "MapTitles":
							map.MapTitles = this.ReadMapTitles(map, context);
							break;
						case "MapDistanceScale":
							map.MapDistanceScale = this.ReadMapDistanceScale(map, context);
							break;
						case "MapColorScale":
							map.MapColorScale = this.ReadMapColorScale(map, context);
							break;
						case "MapBorderSkin":
							map.MapBorderSkin = this.ReadMapBorderSkin(map, context);
							break;
						case "AntiAliasing":
							map.AntiAliasing = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!map.AntiAliasing.IsExpression)
							{
								Validator.ValidateMapAntiAliasing(map.AntiAliasing.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "TextAntiAliasingQuality":
							map.TextAntiAliasingQuality = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!map.TextAntiAliasingQuality.IsExpression)
							{
								Validator.ValidateMapTextAntiAliasingQuality(map.TextAntiAliasingQuality.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
							break;
						case "ShadowIntensity":
							map.ShadowIntensity = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							break;
						case "MaximumSpatialElementCount":
							map.MaximumSpatialElementCount = this.m_reader.ReadInteger(AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, map.Name, "MaximumSpatialElementCount");
							break;
						case "MaximumTotalPointCount":
							map.MaximumTotalPointCount = this.m_reader.ReadInteger(AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, map.Name, "MaximumTotalPointCount");
							break;
						case "ActionInfo":
						{
							bool flag3 = default(bool);
							map.Action = this.ReadActionInfo(context, StyleOwnerType.Map, out flag3);
							break;
						}
						case "TileLanguage":
							map.TileLanguage = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							break;
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("Map" == this.m_reader.LocalName)
						{
							flag2 = true;
						}
						break;
					}
				}
				while (!flag2);
			}
			if (styleInformation != null)
			{
				styleInformation.Filter(StyleOwnerType.Map, false);
				map.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, false, this.m_errorContext);
			}
			if (map.StyleClass != null)
			{
				PublishingValidator.ValidateBorderColorNotTransparent(map.ObjectType, map.Name, map.StyleClass, "BorderColor", this.m_errorContext);
				PublishingValidator.ValidateBorderColorNotTransparent(map.ObjectType, map.Name, map.StyleClass, "BorderColorBottom", this.m_errorContext);
				PublishingValidator.ValidateBorderColorNotTransparent(map.ObjectType, map.Name, map.StyleClass, "BorderColorTop", this.m_errorContext);
				PublishingValidator.ValidateBorderColorNotTransparent(map.ObjectType, map.Name, map.StyleClass, "BorderColorLeft", this.m_errorContext);
				PublishingValidator.ValidateBorderColorNotTransparent(map.ObjectType, map.Name, map.StyleClass, "BorderColorRight", this.m_errorContext);
			}
			this.ValidateDataRegionReferences(map);
			map.Computed = true;
			if (flag)
			{
				this.m_hasImageStreams = true;
				return map;
			}
			return null;
		}

		private void ValidateDataRegionReferences(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapLayer> mapLayers = map.MapLayers;
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapDataRegion> mapDataRegions = map.MapDataRegions;
			if (mapLayers != null)
			{
				foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.MapLayer item in mapLayers)
				{
					if (item is AspNetCore.ReportingServices.ReportIntermediateFormat.MapVectorLayer)
					{
						string mapDataRegionName = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapVectorLayer)item).MapDataRegionName;
						if (mapDataRegionName != null && this.GetDataRegion(mapDataRegions, mapDataRegionName) == null)
						{
							this.m_errorContext.Register(ProcessingErrorCode.rsInvalidMapDataRegionName, Severity.Error, map.ObjectType, map.Name, "MapDataRegionName", mapDataRegionName);
							break;
						}
					}
				}
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapDataRegion GetDataRegion(List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapDataRegion> dataRegions, string dataRegionName)
		{
			if (dataRegions == null)
			{
				return null;
			}
			foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.MapDataRegion dataRegion in dataRegions)
			{
				if (dataRegion.Name == dataRegionName)
				{
					return dataRegion;
				}
			}
			return null;
		}

		private void AddStaticMapMember(int ID, AspNetCore.ReportingServices.ReportIntermediateFormat.MapDataRegion mapDataRegion)
		{
			mapDataRegion.MapMember = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapMember(ID, mapDataRegion);
			mapDataRegion.MapMember.Level = 0;
			mapDataRegion.MapMember.ColSpan = 1;
			mapDataRegion.MapMember.IsColumn = true;
		}

		private void AddStaticMapRowMember(int ID, AspNetCore.ReportingServices.ReportIntermediateFormat.MapDataRegion mapDataRegion)
		{
			mapDataRegion.MapRowMember = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapMember(ID, mapDataRegion);
			mapDataRegion.MapRowMember.Level = 0;
			mapDataRegion.MapRowMember.RowSpan = 1;
		}

		private void AddMapRow(int rowID, int cellID, AspNetCore.ReportingServices.ReportIntermediateFormat.MapDataRegion mapDataRegion)
		{
			mapDataRegion.MapRow = new MapRow(rowID, mapDataRegion);
			mapDataRegion.MapRow.Cell = new MapCell(cellID, mapDataRegion);
			this.m_aggregateHolderList.Add(mapDataRegion.MapRow.Cell);
			this.m_runningValueHolderList.Add(mapDataRegion.MapRow.Cell);
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapDataRegion> ReadMapDataRegions(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapDataRegion> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapDataRegion>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "MapDataRegion")
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.MapDataRegion mapDataRegion = this.ReadMapDataRegion(map, context);
							if (mapDataRegion != null)
							{
								list.Add(mapDataRegion);
							}
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "MapDataRegions")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapDataRegion ReadMapDataRegion(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem parent, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapDataRegion mapDataRegion = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapDataRegion(this.GenerateID(), parent);
			mapDataRegion.Name = this.m_reader.GetAttribute("Name");
			if ((context.Location & LocationFlags.InDataRegion) != 0)
			{
				Global.Tracer.Assert(this.m_nestedDataRegions != null, "(m_nestedDataRegions != null)");
				this.m_nestedDataRegions.Add(mapDataRegion);
			}
			context.Location = (context.Location | LocationFlags.InDataSet | LocationFlags.InDataRegion);
			context.ObjectType = mapDataRegion.ObjectType;
			context.ObjectName = mapDataRegion.Name;
			this.RegisterDataRegion(mapDataRegion);
			bool flag = true;
			if (this.m_scopeNames.Validate(false, context.ObjectName, context.ObjectType, context.ObjectName, this.m_errorContext))
			{
				this.m_reportScopes.Add(mapDataRegion.Name, mapDataRegion);
			}
			else
			{
				flag = false;
			}
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsDataRegionInPageSection, Severity.Error, context.ObjectType, context.ObjectName, null);
				flag = false;
			}
			IdcRelationship relationship = null;
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag2 = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "DataSetName":
							mapDataRegion.DataSetName = this.m_reader.ReadString();
							break;
						case "Relationship":
							relationship = this.ReadRelationship(context);
							break;
						case "Filters":
							mapDataRegion.Filters = this.ReadFilters(AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.DataRegionFilters, context);
							break;
						case "MapMember":
						{
							int num = 0;
							mapDataRegion.MapMember = this.ReadMapMember(mapDataRegion, context, 0, ref num);
							break;
						}
						}
						break;
					case XmlNodeType.EndElement:
						if ("MapDataRegion" == this.m_reader.LocalName)
						{
							flag2 = true;
						}
						break;
					}
				}
				while (!flag2);
			}
			mapDataRegion.DataScopeInfo.SetRelationship(mapDataRegion.DataSetName, relationship);
			if (mapDataRegion.MapMember == null)
			{
				this.AddStaticMapMember(this.GenerateID(), mapDataRegion);
			}
			this.AddStaticMapRowMember(this.GenerateID(), mapDataRegion);
			this.AddMapRow(this.GenerateID(), this.GenerateID(), mapDataRegion);
			mapDataRegion.Computed = true;
			if (flag)
			{
				this.m_hasImageStreams = true;
				return mapDataRegion;
			}
			return null;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapMember ReadMapMember(AspNetCore.ReportingServices.ReportIntermediateFormat.MapDataRegion mapDataRegion, PublishingContextStruct context, int level, ref int aLeafNodes)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapMember mapMember = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapMember(this.GenerateID(), mapDataRegion);
			this.m_runningValueHolderList.Add(mapMember);
			mapMember.IsColumn = true;
			mapMember.Level = level;
			bool flag = false;
			int num = 0;
			if (!this.m_reader.IsEmptyElement)
			{
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Group":
							mapMember.Grouping = this.ReadGrouping(mapMember, context);
							if (mapMember.Grouping.PageBreak != null && mapMember.Grouping.PageBreak.BreakLocation != 0)
							{
								this.m_errorContext.Register(ProcessingErrorCode.rsPageBreakOnMapGroup, Severity.Warning, context.ObjectType, context.ObjectName, "Group", mapMember.Grouping.Name.MarkAsModelInfo());
							}
							if (mapMember.Grouping.DomainScope != null)
							{
								mapMember.Grouping.DomainScope = null;
								this.m_domainScopeGroups.Remove(mapMember.Grouping);
								this.m_errorContext.Register(ProcessingErrorCode.rsInvalidGroupingDomainScopeMap, Severity.Error, context.ObjectType, context.ObjectName, "Group", mapMember.Grouping.Name.MarkAsModelInfo(), mapMember.Grouping.DomainScope.MarkAsPrivate());
							}
							break;
						case "MapMember":
							mapMember.ChildMapMember = this.ReadMapMember(mapDataRegion, context, level + 1, ref num);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("MapMember" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			if (mapMember.ChildMapMember == null)
			{
				aLeafNodes++;
				mapMember.ColSpan = 1;
			}
			else
			{
				aLeafNodes += num;
				mapMember.ColSpan = num;
			}
			return mapMember;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapLocation ReadMapLocation(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapLocation mapLocation = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapLocation(map);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapLocationElement(map, mapLocation, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapLocation" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapLocation;
		}

		private void ReadMapLocationElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapLocation mapLocation, PublishingContextStruct context)
		{
			string localName;
			if ((localName = this.m_reader.LocalName) != null)
			{
				if (!(localName == "Left"))
				{
					if (!(localName == "Top"))
					{
						if (localName == "Unit")
						{
							mapLocation.Unit = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!mapLocation.Unit.IsExpression)
							{
								Validator.ValidateUnit(mapLocation.Unit.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
						}
					}
					else
					{
						mapLocation.Top = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
					}
				}
				else
				{
					mapLocation.Left = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				}
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapSize ReadMapSize(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapSize mapSize = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapSize(map);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapSizeElement(map, mapSize, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapSize" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapSize;
		}

		private void ReadMapSizeElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapSize mapSize, PublishingContextStruct context)
		{
			string localName;
			if ((localName = this.m_reader.LocalName) != null)
			{
				if (!(localName == "Width"))
				{
					if (!(localName == "Height"))
					{
						if (localName == "Unit")
						{
							mapSize.Unit = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!mapSize.Unit.IsExpression)
							{
								Validator.ValidateUnit(mapSize.Unit.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
						}
					}
					else
					{
						mapSize.Height = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
					}
				}
				else
				{
					mapSize.Width = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				}
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapGridLines ReadMapGridLines(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context, string tagName)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapGridLines mapGridLines = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapGridLines(map);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapGridLinesElement(map, mapGridLines, context);
						break;
					case XmlNodeType.EndElement:
						if (tagName == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapGridLines;
		}

		private void ReadMapGridLinesElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapGridLines mapGridLines, PublishingContextStruct context)
		{
			string localName;
			if ((localName = this.m_reader.LocalName) != null)
			{
				if (!(localName == "Style"))
				{
					if (!(localName == "Hidden"))
					{
						if (!(localName == "Interval"))
						{
							if (!(localName == "ShowLabels"))
							{
								if (localName == "LabelPosition")
								{
									mapGridLines.LabelPosition = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
									if (!mapGridLines.LabelPosition.IsExpression)
									{
										Validator.ValidateLabelPosition(mapGridLines.LabelPosition.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
									}
								}
							}
							else
							{
								mapGridLines.ShowLabels = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
							}
						}
						else
						{
							mapGridLines.Interval = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
						}
					}
					else
					{
						mapGridLines.Hidden = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
					}
				}
				else
				{
					this.ReadMapStyle(mapGridLines, context);
				}
			}
		}

		private void ReadMapStyle(MapStyleContainer mapStyleContainer, PublishingContextStruct context)
		{
			StyleInformation styleInformation = this.ReadStyle(context);
			styleInformation.Filter(StyleOwnerType.Map, false);
			mapStyleContainer.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, this.m_errorContext);
		}

		private void ReadMapTitleStyle(AspNetCore.ReportingServices.ReportIntermediateFormat.MapTitle mapTitle, PublishingContextStruct context)
		{
			StyleInformation styleInformation = this.ReadStyle(context);
			styleInformation.FilterMapTitleStyle();
			mapTitle.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, this.m_errorContext);
		}

		private void ReadMapLegendTitleStyle(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegendTitle legendTitle, PublishingContextStruct context)
		{
			StyleInformation styleInformation = this.ReadStyle(context);
			styleInformation.FilterMapLegendTitleStyle();
			legendTitle.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, true, this.m_errorContext);
		}

		private void ReadMapSubItemElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapSubItem mapSubItem, PublishingContextStruct context)
		{
            string localName;
            switch (localName = this.m_reader.LocalName)
            {
                case "Style":
                    this.ReadMapStyle(mapSubItem, context);
                    return;
                case "MapLocation":
                    mapSubItem.MapLocation = this.ReadMapLocation(map, context);
                    return;
                case "MapSize":
                    mapSubItem.MapSize = this.ReadMapSize(map, context);
                    return;
                case "LeftMargin":
                    mapSubItem.LeftMargin = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
                    return;
                case "RightMargin":
                    mapSubItem.RightMargin = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
                    return;
                case "TopMargin":
                    mapSubItem.TopMargin = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
                    return;
                case "BottomMargin":
                    mapSubItem.BottomMargin = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
                    return;
                case "ZIndex":
                    mapSubItem.ZIndex = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
                    return;
            }
        }

		private void ReadMapDockableSubItemElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapDockableSubItem mapDockableSubItem, PublishingContextStruct context)
		{
			switch (this.m_reader.LocalName)
			{
			case "ActionInfo":
			{
				bool flag = false;
				mapDockableSubItem.Action = this.ReadActionInfo(context, StyleOwnerType.Map, out flag);
				break;
			}
			case "Position":
				mapDockableSubItem.Position = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				if (!mapDockableSubItem.Position.IsExpression)
				{
					Validator.ValidateMapPosition(mapDockableSubItem.Position.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
				}
				break;
			case "DockOutsideViewport":
				mapDockableSubItem.DockOutsideViewport = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
				break;
			case "Hidden":
				mapDockableSubItem.Hidden = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
				break;
			case "ToolTip":
				mapDockableSubItem.ToolTip = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			default:
				this.ReadMapSubItemElement(map, mapDockableSubItem, context);
				break;
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapBindingFieldPair ReadMapBindingFieldPair(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapBindingFieldPair mapBindingFieldPair = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapBindingFieldPair(map, mapVectorLayer);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapBindingFieldPairElement(map, mapBindingFieldPair, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapBindingFieldPair" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapBindingFieldPair;
		}

		private void ReadMapBindingFieldPairElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapBindingFieldPair mapBindingFieldPair, PublishingContextStruct context)
		{
			string localName;
			if ((localName = this.m_reader.LocalName) != null)
			{
				if (!(localName == "FieldName"))
				{
					if (localName == "BindingExpression")
					{
						mapBindingFieldPair.BindingExpression = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
					}
				}
				else
				{
					mapBindingFieldPair.FieldName = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				}
			}
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapBindingFieldPair> ReadMapBindingFieldPairs(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, PublishingContextStruct context)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapBindingFieldPair> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapBindingFieldPair>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "MapBindingFieldPair")
						{
							list.Add(this.ReadMapBindingFieldPair(map, mapVectorLayer, context));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "MapBindingFieldPairs")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapViewport ReadMapViewport(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapViewport mapViewport = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapViewport(map);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapViewportElement(map, mapViewport, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapViewport" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapViewport;
		}

		private void ReadMapViewportElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapViewport mapViewport, PublishingContextStruct context)
		{
			switch (this.m_reader.LocalName)
			{
			case "MapCoordinateSystem":
				mapViewport.MapCoordinateSystem = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				if (!mapViewport.MapCoordinateSystem.IsExpression)
				{
					Validator.ValidateMapCoordinateSystem(mapViewport.MapCoordinateSystem.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
				}
				break;
			case "MapProjection":
				mapViewport.MapProjection = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				if (!mapViewport.MapProjection.IsExpression)
				{
					Validator.ValidateMapProjection(mapViewport.MapProjection.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
				}
				break;
			case "ProjectionCenterX":
				mapViewport.ProjectionCenterX = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			case "ProjectionCenterY":
				mapViewport.ProjectionCenterY = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			case "MapLimits":
				mapViewport.MapLimits = this.ReadMapLimits(map, context);
				break;
			case "MapCustomView":
			case "MapDataBoundView":
			case "MapElementView":
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.MapView mapView = mapViewport.MapView;
				this.ReadMapView(map, context, ref mapView, this.m_reader.LocalName);
				mapViewport.MapView = mapView;
				break;
			}
			case "MaximumZoom":
				mapViewport.MaximumZoom = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			case "MinimumZoom":
				mapViewport.MinimumZoom = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			case "ContentMargin":
				mapViewport.ContentMargin = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "MapMeridians":
				mapViewport.MapMeridians = this.ReadMapGridLines(map, context, "MapMeridians");
				break;
			case "MapParallels":
				mapViewport.MapParallels = this.ReadMapGridLines(map, context, "MapParallels");
				break;
			case "GridUnderContent":
				mapViewport.GridUnderContent = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
				break;
			case "SimplificationResolution":
				mapViewport.SimplificationResolution = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			default:
				this.ReadMapSubItemElement(map, mapViewport, context);
				break;
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapLimits ReadMapLimits(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapLimits mapLimits = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapLimits(map);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapLimitsElement(map, mapLimits, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapLimits" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapLimits;
		}

		private void ReadMapLimitsElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapLimits mapLimits, PublishingContextStruct context)
		{
			string localName;
			if ((localName = this.m_reader.LocalName) != null)
			{
				if (!(localName == "MinimumX"))
				{
					if (!(localName == "MinimumY"))
					{
						if (!(localName == "MaximumX"))
						{
							if (!(localName == "MaximumY"))
							{
								if (localName == "LimitToData")
								{
									mapLimits.LimitToData = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
								}
							}
							else
							{
								mapLimits.MaximumY = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							}
						}
						else
						{
							mapLimits.MaximumX = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
						}
					}
					else
					{
						mapLimits.MinimumY = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
					}
				}
				else
				{
					mapLimits.MinimumX = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				}
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScale ReadMapColorScale(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScale mapColorScale = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScale(map, map.GenerateActionOwnerID());
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapColorScaleElement(map, mapColorScale, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapColorScale" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapColorScale;
		}

		private void ReadMapColorScaleElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScale mapColorScale, PublishingContextStruct context)
		{
			switch (this.m_reader.LocalName)
			{
			case "MapColorScaleTitle":
				mapColorScale.MapColorScaleTitle = this.ReadMapColorScaleTitle(map, context);
				break;
			case "TickMarkLength":
				mapColorScale.TickMarkLength = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "ColorBarBorderColor":
				mapColorScale.ColorBarBorderColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "LabelInterval":
				mapColorScale.LabelInterval = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
				break;
			case "LabelFormat":
				mapColorScale.LabelFormat = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "LabelPlacement":
				mapColorScale.LabelPlacement = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				if (!mapColorScale.LabelPlacement.IsExpression)
				{
					Validator.ValidateLabelPlacement(mapColorScale.LabelPlacement.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
				}
				break;
			case "LabelBehavior":
				mapColorScale.LabelBehavior = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				if (!mapColorScale.LabelBehavior.IsExpression)
				{
					Validator.ValidateLabelBehavior(mapColorScale.LabelBehavior.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
				}
				break;
			case "HideEndLabels":
				mapColorScale.HideEndLabels = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
				break;
			case "RangeGapColor":
				mapColorScale.RangeGapColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "NoDataText":
				mapColorScale.NoDataText = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			default:
				this.ReadMapDockableSubItemElement(map, mapColorScale, context);
				break;
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScaleTitle ReadMapColorScaleTitle(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScaleTitle mapColorScaleTitle = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScaleTitle(map);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapColorScaleTitleElement(map, mapColorScaleTitle, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapColorScaleTitle" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapColorScaleTitle;
		}

		private void ReadMapColorScaleTitleElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScaleTitle mapColorScaleTitle, PublishingContextStruct context)
		{
			string localName;
			if ((localName = this.m_reader.LocalName) != null)
			{
				if (!(localName == "Style"))
				{
					if (localName == "Caption")
					{
						mapColorScaleTitle.Caption = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
					}
				}
				else
				{
					this.ReadMapStyle(mapColorScaleTitle, context);
				}
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapDistanceScale ReadMapDistanceScale(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapDistanceScale mapDistanceScale = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapDistanceScale(map, map.GenerateActionOwnerID());
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapDistanceScaleElement(map, mapDistanceScale, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapDistanceScale" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapDistanceScale;
		}

		private void ReadMapDistanceScaleElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapDistanceScale mapDistanceScale, PublishingContextStruct context)
		{
			switch (this.m_reader.LocalName)
			{
			case "ScaleColor":
				mapDistanceScale.ScaleColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "ScaleBorderColor":
				mapDistanceScale.ScaleBorderColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			default:
				this.ReadMapDockableSubItemElement(map, mapDistanceScale, context);
				break;
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapTitle ReadMapTitle(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapTitle mapTitle = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapTitle(map, map.GenerateActionOwnerID());
			mapTitle.Name = this.m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "MapTitle", AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, map.Name, mapTitle.Name, this.m_errorContext);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapTitleElement(map, mapTitle, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapTitle" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapTitle;
		}

		private void ReadMapTitleElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapTitle mapTitle, PublishingContextStruct context)
		{
			switch (this.m_reader.LocalName)
			{
			case "Style":
				this.ReadMapTitleStyle(mapTitle, context);
				break;
			case "Text":
				mapTitle.Text = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "Angle":
				mapTitle.Angle = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			case "TextShadowOffset":
				mapTitle.TextShadowOffset = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			default:
				this.ReadMapDockableSubItemElement(map, mapTitle, context);
				break;
			}
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapTitle> ReadMapTitles(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapTitle> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapTitle>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "MapTitle")
						{
							list.Add(this.ReadMapTitle(map, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "MapTitles")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegend ReadMapLegend(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegend mapLegend = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegend(map, map.GenerateActionOwnerID());
			mapLegend.Name = this.m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "MapLegend", AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, map.Name, mapLegend.Name, this.m_errorContext);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapLegendElement(map, mapLegend, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapLegend" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapLegend;
		}

		private void ReadMapLegendElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegend mapLegend, PublishingContextStruct context)
		{
			switch (this.m_reader.LocalName)
			{
			case "Layout":
				mapLegend.Layout = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				if (!mapLegend.Layout.IsExpression)
				{
					Validator.ValidateMapLegendLayout(mapLegend.Layout.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
				}
				break;
			case "MapLegendTitle":
				mapLegend.MapLegendTitle = this.ReadMapLegendTitle(map, context);
				break;
			case "AutoFitTextDisabled":
				mapLegend.AutoFitTextDisabled = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
				break;
			case "MinFontSize":
				mapLegend.MinFontSize = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "InterlacedRows":
				mapLegend.InterlacedRows = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
				break;
			case "InterlacedRowsColor":
				mapLegend.InterlacedRowsColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "EquallySpacedItems":
				mapLegend.EquallySpacedItems = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
				break;
			case "TextWrapThreshold":
				mapLegend.TextWrapThreshold = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
				break;
			default:
				this.ReadMapDockableSubItemElement(map, mapLegend, context);
				break;
			}
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegend> ReadMapLegends(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegend> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegend>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "MapLegend")
						{
							list.Add(this.ReadMapLegend(map, context, nameValidator));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "MapLegends")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegendTitle ReadMapLegendTitle(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegendTitle mapLegendTitle = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegendTitle(map);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapLegendTitleElement(map, mapLegendTitle, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapLegendTitle" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapLegendTitle;
		}

		private void ReadMapLegendTitleElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapLegendTitle mapLegendTitle, PublishingContextStruct context)
		{
			string localName;
			if ((localName = this.m_reader.LocalName) != null)
			{
				if (!(localName == "Style"))
				{
					if (!(localName == "Caption"))
					{
						if (!(localName == "TitleSeparator"))
						{
							if (localName == "TitleSeparatorColor")
							{
								mapLegendTitle.TitleSeparatorColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							}
						}
						else
						{
							mapLegendTitle.TitleSeparator = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							if (!mapLegendTitle.TitleSeparator.IsExpression)
							{
								Validator.ValidateMapLegendTitleSeparator(mapLegendTitle.TitleSeparator.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
							}
						}
					}
					else
					{
						mapLegendTitle.Caption = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
					}
				}
				else
				{
					this.ReadMapLegendTitleStyle(mapLegendTitle, context);
				}
			}
		}

		private void ReadMapAppearanceRuleElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapAppearanceRule mapAppearanceRule, PublishingContextStruct context)
		{
            string localName;
            switch (localName = this.m_reader.LocalName)
            {
                case "DataValue":
                    mapAppearanceRule.DataValue = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
                    return;
                case "DistributionType":
                    mapAppearanceRule.DistributionType = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
                    if (!mapAppearanceRule.DistributionType.IsExpression)
                    {
                        Validator.ValidateMapRuleDistributionType(mapAppearanceRule.DistributionType.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
                        return;
                    }
                    break;
                case "BucketCount":
                    mapAppearanceRule.BucketCount = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context);
                    return;
                case "StartValue":
                    mapAppearanceRule.StartValue = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
                    return;
                case "EndValue":
                    mapAppearanceRule.EndValue = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
                    return;
                case "MapBuckets":
                    mapAppearanceRule.MapBuckets = this.ReadMapBuckets(map, context);
                    return;
                case "LegendName":
                    mapAppearanceRule.LegendName = this.m_reader.ReadString();
                    return;
                case "LegendText":
                    mapAppearanceRule.LegendText = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
                    return;
                case "DataElementName":
                    mapAppearanceRule.DataElementName = this.m_reader.ReadString();
                    return;
                case "DataElementOutput":
                    mapAppearanceRule.DataElementOutput = this.ReadDataElementOutput();
                    return;
            }
        }

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapBucket ReadMapBucket(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapBucket mapBucket = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapBucket(map);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapBucketElement(map, mapBucket, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapBucket" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapBucket;
		}

		private void ReadMapBucketElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapBucket mapBucket, PublishingContextStruct context)
		{
			string localName;
			if ((localName = this.m_reader.LocalName) != null)
			{
				if (!(localName == "StartValue"))
				{
					if (localName == "EndValue")
					{
						mapBucket.EndValue = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
					}
				}
				else
				{
					mapBucket.StartValue = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				}
			}
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapBucket> ReadMapBuckets(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapBucket> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapBucket>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "MapBucket")
						{
							list.Add(this.ReadMapBucket(map, context));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "MapBuckets")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorPaletteRule ReadMapColorPaletteRule(AspNetCore.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorPaletteRule mapColorPaletteRule = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorPaletteRule(mapVectorLayer, map);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapColorPaletteRuleElement(map, mapColorPaletteRule, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapColorPaletteRule" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapColorPaletteRule;
		}

		private void ReadMapColorPaletteRuleElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorPaletteRule mapColorPaletteRule, PublishingContextStruct context)
		{
			string localName;
			if ((localName = this.m_reader.LocalName) != null && localName == "Palette")
			{
				mapColorPaletteRule.Palette = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				if (!mapColorPaletteRule.Palette.IsExpression)
				{
					Validator.ValidateMapPalette(mapColorPaletteRule.Palette.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
				}
			}
			else
			{
				this.ReadMapColorRuleElement(map, mapColorPaletteRule, context);
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorRangeRule ReadMapColorRangeRule(AspNetCore.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorRangeRule mapColorRangeRule = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorRangeRule(mapVectorLayer, map);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapColorRangeRuleElement(map, mapColorRangeRule, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapColorRangeRule" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapColorRangeRule;
		}

		private void ReadMapColorRangeRuleElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorRangeRule mapColorRangeRule, PublishingContextStruct context)
		{
			switch (this.m_reader.LocalName)
			{
			case "StartColor":
				mapColorRangeRule.StartColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "MiddleColor":
				mapColorRangeRule.MiddleColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "EndColor":
				mapColorRangeRule.EndColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			default:
				this.ReadMapColorRuleElement(map, mapColorRangeRule, context);
				break;
			}
		}

		private void ReadMapColorRuleElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorRule mapColorRule, PublishingContextStruct context)
		{
			string localName;
			if ((localName = this.m_reader.LocalName) != null && localName == "ShowInColorScale")
			{
				mapColorRule.ShowInColorScale = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
			}
			else
			{
				this.ReadMapAppearanceRuleElement(map, mapColorRule, context);
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineRules ReadMapLineRules(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineLayer mapLineLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineRules mapLineRules = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineRules(map);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapLineRulesElement(mapLineLayer, map, mapLineRules, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapLineRules" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapLineRules;
		}

		private void ReadMapLineRulesElement(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineLayer mapLineLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineRules mapLineRules, PublishingContextStruct context)
		{
			string localName;
			if ((localName = this.m_reader.LocalName) != null)
			{
				if (!(localName == "MapSizeRule"))
				{
					if (!(localName == "MapColorPaletteRule") && !(localName == "MapColorRangeRule") && !(localName == "MapCustomColorRule"))
					{
						return;
					}
					AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorRule mapColorRule = mapLineRules.MapColorRule;
					this.ReadMapColorRule(mapLineLayer, map, context, ref mapColorRule, this.m_reader.LocalName);
					mapLineRules.MapColorRule = mapColorRule;
				}
				else
				{
					mapLineRules.MapSizeRule = this.ReadMapSizeRule(mapLineLayer, map, context);
				}
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonRules ReadMapPolygonRules(AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonLayer mapPolygonLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonRules mapPolygonRules = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonRules(map);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapPolygonRulesElement(mapPolygonLayer, map, mapPolygonRules, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapPolygonRules" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapPolygonRules;
		}

		private void ReadMapPolygonRulesElement(AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonLayer mapPolygonLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonRules mapPolygonRules, PublishingContextStruct context)
		{
			string localName;
			if ((localName = this.m_reader.LocalName) != null)
			{
				if (!(localName == "MapColorPaletteRule") && !(localName == "MapColorRangeRule") && !(localName == "MapCustomColorRule"))
				{
					return;
				}
				AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorRule mapColorRule = mapPolygonRules.MapColorRule;
				this.ReadMapColorRule(mapPolygonLayer, map, context, ref mapColorRule, this.m_reader.LocalName);
				mapPolygonRules.MapColorRule = mapColorRule;
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapSizeRule ReadMapSizeRule(AspNetCore.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapSizeRule mapSizeRule = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapSizeRule(mapVectorLayer, map);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapSizeRuleElement(map, mapSizeRule, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapSizeRule" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapSizeRule;
		}

		private void ReadMapSizeRuleElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapSizeRule mapSizeRule, PublishingContextStruct context)
		{
			switch (this.m_reader.LocalName)
			{
			case "StartSize":
				mapSizeRule.StartSize = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "EndSize":
				mapSizeRule.EndSize = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			default:
				this.ReadMapAppearanceRuleElement(map, mapSizeRule, context);
				break;
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerImage ReadMapMarkerImage(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerImage mapMarkerImage = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerImage(map);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapMarkerImageElement(map, mapMarkerImage, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapMarkerImage" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapMarkerImage;
		}

		private void ReadMapMarkerImageElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerImage mapMarkerImage, PublishingContextStruct context)
		{
			string localName;
			if ((localName = this.m_reader.LocalName) != null)
			{
				if (!(localName == "Source"))
				{
					if (!(localName == "Value"))
					{
						if (!(localName == "MIMEType"))
						{
							if (!(localName == "TransparentColor"))
							{
								if (localName == "ResizeMode")
								{
									mapMarkerImage.ResizeMode = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
									if (!mapMarkerImage.ResizeMode.IsExpression)
									{
										Validator.ValidateMapResizeMode(mapMarkerImage.ResizeMode.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
									}
								}
							}
							else
							{
								mapMarkerImage.TransparentColor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							}
						}
						else
						{
							mapMarkerImage.MIMEType = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
						}
					}
					else
					{
						mapMarkerImage.Value = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
					}
				}
				else
				{
					mapMarkerImage.Source = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
					if (!mapMarkerImage.Source.IsExpression)
					{
						Validator.ValidateImageSourceType(mapMarkerImage.Source.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
					}
				}
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarker ReadMapMarker(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarker mapMarker = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarker(map);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapMarkerElement(map, mapMarker, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapMarker" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapMarker;
		}

		private void ReadMapMarkerElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarker mapMarker, PublishingContextStruct context)
		{
			string localName;
			if ((localName = this.m_reader.LocalName) != null)
			{
				if (!(localName == "MapMarkerStyle"))
				{
					if (localName == "MapMarkerImage")
					{
						mapMarker.MapMarkerImage = this.ReadMapMarkerImage(map, context);
					}
				}
				else
				{
					mapMarker.MapMarkerStyle = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
					if (!mapMarker.MapMarkerStyle.IsExpression)
					{
						Validator.ValidateMapMarkerStyle(mapMarker.MapMarkerStyle.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
					}
				}
			}
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarker> ReadMapMarkers(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarker> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarker>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "MapMarker")
						{
							list.Add(this.ReadMapMarker(map, context));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "MapMarkers")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerRule ReadMapMarkerRule(AspNetCore.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerRule mapMarkerRule = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerRule(mapVectorLayer, map);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapMarkerRuleElement(map, mapMarkerRule, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapMarkerRule" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapMarkerRule;
		}

		private void ReadMapMarkerRuleElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerRule mapMarkerRule, PublishingContextStruct context)
		{
			string localName;
			if ((localName = this.m_reader.LocalName) != null && localName == "MapMarkers")
			{
				mapMarkerRule.MapMarkers = this.ReadMapMarkers(map, context);
			}
			else
			{
				this.ReadMapAppearanceRuleElement(map, mapMarkerRule, context);
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointRules ReadMapPointRules(AspNetCore.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context, string tagName)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointRules mapPointRules = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointRules(map);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapPointRulesElement(mapVectorLayer, map, mapPointRules, context);
						break;
					case XmlNodeType.EndElement:
						if (tagName == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapPointRules;
		}

		private void ReadMapPointRulesElement(AspNetCore.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointRules mapPointRules, PublishingContextStruct context)
		{
			string localName;
			if ((localName = this.m_reader.LocalName) != null)
			{
				if (!(localName == "MapSizeRule"))
				{
					if (!(localName == "MapColorPaletteRule") && !(localName == "MapColorRangeRule") && !(localName == "MapCustomColorRule"))
					{
						if (localName == "MapMarkerRule")
						{
							mapPointRules.MapMarkerRule = this.ReadMapMarkerRule(mapVectorLayer, map, context);
						}
					}
					else
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorRule mapColorRule = mapPointRules.MapColorRule;
						this.ReadMapColorRule(mapVectorLayer, map, context, ref mapColorRule, this.m_reader.LocalName);
						mapPointRules.MapColorRule = mapColorRule;
					}
				}
				else
				{
					mapPointRules.MapSizeRule = this.ReadMapSizeRule(mapVectorLayer, map, context);
				}
			}
		}

		private void ReadMapColorRule(AspNetCore.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context, ref AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorRule colorRule, string propertyName)
		{
			string a;
			if (colorRule != null)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsMapPropertyAlreadyDefined, Severity.Error, context.ObjectType, context.ObjectName, propertyName);
			}
			else if ((a = propertyName) != null)
			{
				if (!(a == "MapColorPaletteRule"))
				{
					if (!(a == "MapColorRangeRule"))
					{
						if (a == "MapCustomColorRule")
						{
							colorRule = this.ReadMapCustomColorRule(mapVectorLayer, map, context);
						}
					}
					else
					{
						colorRule = this.ReadMapColorRangeRule(mapVectorLayer, map, context);
					}
				}
				else
				{
					colorRule = this.ReadMapColorPaletteRule(mapVectorLayer, map, context);
				}
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomColorRule ReadMapCustomColorRule(AspNetCore.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomColorRule mapCustomColorRule = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomColorRule(mapVectorLayer, map);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapCustomColorRuleElement(map, mapCustomColorRule, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapCustomColorRule" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapCustomColorRule;
		}

		private void ReadMapCustomColorRuleElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomColorRule mapCustomColorRule, PublishingContextStruct context)
		{
			string localName;
			if ((localName = this.m_reader.LocalName) != null && localName == "MapCustomColors")
			{
				mapCustomColorRule.MapCustomColors = this.ReadMapCustomColors(map, context);
			}
			else
			{
				this.ReadMapColorRuleElement(map, mapCustomColorRule, context);
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomColor ReadMapCustomColor(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomColor mapCustomColor = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomColor(map);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapCustomColorElement(map, mapCustomColor, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapCustomColor" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapCustomColor;
		}

		private void ReadMapCustomColorElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomColor mapCustomColor, PublishingContextStruct context)
		{
			string localName;
			if ((localName = this.m_reader.LocalName) != null && localName == "Color")
			{
				mapCustomColor.Color = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
			}
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomColor> ReadMapCustomColors(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomColor> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomColor>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "MapCustomColor")
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomColor mapCustomColor = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomColor(map);
							mapCustomColor.Color = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							list.Add(mapCustomColor);
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "MapCustomColors")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineTemplate ReadMapLineTemplate(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineLayer mapLineLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineTemplate mapLineTemplate = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineTemplate(mapLineLayer, map, map.GenerateActionOwnerID());
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapLineTemplateElement(map, mapLineTemplate, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapLineTemplate" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapLineTemplate;
		}

		private void ReadMapLineTemplateElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineTemplate mapLineTemplate, PublishingContextStruct context)
		{
			switch (this.m_reader.LocalName)
			{
			case "Width":
				mapLineTemplate.Width = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "LabelPlacement":
				mapLineTemplate.LabelPlacement = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				if (!mapLineTemplate.LabelPlacement.IsExpression)
				{
					Validator.ValidateMapLineLabelPlacement(mapLineTemplate.LabelPlacement.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
				}
				break;
			default:
				this.ReadMapSpatialElementTemplateElement(map, mapLineTemplate, context);
				break;
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate ReadMapPolygonTemplate(AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonLayer mapPolygonLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate mapPolygonTemplate = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate(mapPolygonLayer, map, map.GenerateActionOwnerID());
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapPolygonTemplateElement(map, mapPolygonTemplate, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapPolygonTemplate" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapPolygonTemplate;
		}

		private void ReadMapPolygonTemplateElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate mapPolygonTemplate, PublishingContextStruct context)
		{
			switch (this.m_reader.LocalName)
			{
			case "ScaleFactor":
				mapPolygonTemplate.ScaleFactor = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			case "CenterPointOffsetX":
				mapPolygonTemplate.CenterPointOffsetX = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			case "CenterPointOffsetY":
				mapPolygonTemplate.CenterPointOffsetY = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			case "ShowLabel":
				mapPolygonTemplate.ShowLabel = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				if (!mapPolygonTemplate.ShowLabel.IsExpression)
				{
					Validator.ValidateMapAutoBool(mapPolygonTemplate.ShowLabel.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
				}
				break;
			case "LabelPlacement":
				mapPolygonTemplate.LabelPlacement = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				if (!mapPolygonTemplate.LabelPlacement.IsExpression)
				{
					Validator.ValidateMapPolygonLabelPlacement(mapPolygonTemplate.LabelPlacement.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
				}
				break;
			default:
				this.ReadMapSpatialElementTemplateElement(map, mapPolygonTemplate, context);
				break;
			}
		}

        private void ReadMapSpatialElementTemplateElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialElementTemplate mapSpatialElementTemplate, PublishingContextStruct context)
        {
            string localName;
            switch (localName = this.m_reader.LocalName)
            {
                case "Style":
                    this.ReadMapStyle(mapSpatialElementTemplate, context);
                    return;
                case "ActionInfo":

                    bool flag = false;
                    mapSpatialElementTemplate.Action = this.ReadActionInfo(context, StyleOwnerType.Map, out flag);
                    return;

                case "Hidden":
                    mapSpatialElementTemplate.Hidden = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
                    return;
                case "OffsetX":
                    mapSpatialElementTemplate.OffsetX = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
                    return;
                case "OffsetY":
                    mapSpatialElementTemplate.OffsetY = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
                    return;
                case "Label":
                    mapSpatialElementTemplate.Label = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
                    return;
                case "ToolTip":
                    mapSpatialElementTemplate.ToolTip = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
                    return;
                case "DataElementName":
                    mapSpatialElementTemplate.DataElementName = this.m_reader.ReadString();
                    return;
                case "DataElementOutput":
                    mapSpatialElementTemplate.DataElementOutput = this.ReadDataElementOutput();
                    return;
                case "DataElementLabel":
                    mapSpatialElementTemplate.DataElementLabel = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
                    return;
            }
        }

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerTemplate ReadMapMarkerTemplate(AspNetCore.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerTemplate mapMarkerTemplate = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerTemplate(mapVectorLayer, map, map.GenerateActionOwnerID());
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapMarkerTemplateElement(map, mapMarkerTemplate, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapMarkerTemplate" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapMarkerTemplate;
		}

		private void ReadMapMarkerTemplateElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerTemplate mapMarkerTemplate, PublishingContextStruct context)
		{
			string localName;
			if ((localName = this.m_reader.LocalName) != null && localName == "MapMarker")
			{
				mapMarkerTemplate.MapMarker = this.ReadMapMarker(map, context);
			}
			else
			{
				this.ReadMapPointTemplateElement(map, mapMarkerTemplate, context);
			}
		}

		private void ReadMapPointTemplateElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointTemplate mapPointTemplate, PublishingContextStruct context)
		{
			switch (this.m_reader.LocalName)
			{
			case "Size":
				mapPointTemplate.Size = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				PublishingValidator.ValidateSize(mapPointTemplate.Size, Validator.NormalMin, Validator.NormalMax, map.ObjectType, map.Name, "Size", this.m_errorContext);
				break;
			case "LabelPlacement":
				mapPointTemplate.LabelPlacement = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				if (!mapPointTemplate.LabelPlacement.IsExpression)
				{
					Validator.ValidateMapPointLabelPlacement(mapPointTemplate.LabelPlacement.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
				}
				break;
			default:
				this.ReadMapSpatialElementTemplateElement(map, mapPointTemplate, context);
				break;
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapField ReadMapField(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapField mapField = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapField(map);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapFieldElement(map, mapField, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapField" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapField;
		}

		private void ReadMapFieldElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapField mapField, PublishingContextStruct context)
		{
			string localName;
			if ((localName = this.m_reader.LocalName) != null)
			{
				if (!(localName == "Name"))
				{
					if (localName == "Value")
					{
						mapField.Value = this.m_reader.ReadString();
					}
				}
				else
				{
					mapField.Name = this.m_reader.ReadString();
				}
			}
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapField> ReadMapFields(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapField> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapField>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "MapField")
						{
							list.Add(this.ReadMapField(map, context));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "MapFields")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapLine ReadMapLine(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineLayer mapLineLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapLine mapLine = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapLine(mapLineLayer, map);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapLineElement(mapLineLayer, map, mapLine, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapLine" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapLine;
		}

		private void ReadMapLineElement(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineLayer mapLineLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapLine mapLine, PublishingContextStruct context)
		{
			switch (this.m_reader.LocalName)
			{
			case "UseCustomLineTemplate":
				mapLine.UseCustomLineTemplate = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
				break;
			case "MapLineTemplate":
				mapLine.MapLineTemplate = this.ReadMapLineTemplate(mapLineLayer, map, context);
				break;
			default:
				this.ReadMapSpatialElementElement(map, mapLine, context);
				break;
			}
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapLine> ReadMapLines(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineLayer mapLineLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapLine> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapLine>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "MapLine")
						{
							list.Add(this.ReadMapLine(mapLineLayer, map, context));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "MapLines")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygon ReadMapPolygon(AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonLayer mapPolygonLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygon mapPolygon = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygon(mapPolygonLayer, map);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapPolygonElement(mapPolygonLayer, map, mapPolygon, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapPolygon" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapPolygon;
		}

		private void ReadMapPolygonElement(AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonLayer mapPolygonLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygon mapPolygon, PublishingContextStruct context)
		{
			switch (this.m_reader.LocalName)
			{
			case "UseCustomPolygonTemplate":
				mapPolygon.UseCustomPolygonTemplate = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
				break;
			case "MapPolygonTemplate":
				mapPolygon.MapPolygonTemplate = this.ReadMapPolygonTemplate(mapPolygonLayer, map, context);
				break;
			case "UseCustomCenterPointTemplate":
				mapPolygon.UseCustomCenterPointTemplate = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
				break;
			case "MapMarkerTemplate":
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointTemplate mapCenterPointTemplate = mapPolygon.MapCenterPointTemplate;
				this.ReadMapPointTemplate(mapPolygonLayer, map, context, ref mapCenterPointTemplate, this.m_reader.LocalName);
				mapPolygon.MapCenterPointTemplate = mapCenterPointTemplate;
				break;
			}
			default:
				this.ReadMapSpatialElementElement(map, mapPolygon, context);
				break;
			}
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygon> ReadMapPolygons(AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonLayer mapPolygonLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygon> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygon>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "MapPolygon")
						{
							list.Add(this.ReadMapPolygon(mapPolygonLayer, map, context));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "MapPolygons")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private void ReadMapSpatialElementElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialElement mapSpatialElement, PublishingContextStruct context)
		{
			string localName;
			if ((localName = this.m_reader.LocalName) != null)
			{
				if (!(localName == "VectorData"))
				{
					if (localName == "MapFields")
					{
						mapSpatialElement.MapFields = this.ReadMapFields(map, context);
					}
				}
				else
				{
					mapSpatialElement.VectorData = this.m_reader.ReadString();
				}
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapPoint ReadMapPoint(AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointLayer mapPointLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapPoint mapPoint = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapPoint(mapPointLayer, map);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapPointElement(mapPointLayer, map, mapPoint, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapPoint" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapPoint;
		}

		private void ReadMapPointElement(AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointLayer mapPointLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapPoint mapPoint, PublishingContextStruct context)
		{
			switch (this.m_reader.LocalName)
			{
			case "UseCustomPointTemplate":
				mapPoint.UseCustomPointTemplate = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
				break;
			case "MapMarkerTemplate":
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointTemplate mapPointTemplate = mapPoint.MapPointTemplate;
				this.ReadMapPointTemplate(mapPointLayer, map, context, ref mapPointTemplate, this.m_reader.LocalName);
				mapPoint.MapPointTemplate = mapPointTemplate;
				break;
			}
			default:
				this.ReadMapSpatialElementElement(map, mapPoint, context);
				break;
			}
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapPoint> ReadMapPoints(AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointLayer mapPointLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapPoint> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapPoint>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "MapPoint")
						{
							list.Add(this.ReadMapPoint(mapPointLayer, map, context));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "MapPoints")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private void ReadMapPointTemplate(AspNetCore.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context, ref AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointTemplate symbolTemplate, string propertyName)
		{
			string a;
			if (symbolTemplate != null)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsMapPropertyAlreadyDefined, Severity.Error, context.ObjectType, context.ObjectName, propertyName);
			}
			else if ((a = propertyName) != null && a == "MapMarkerTemplate")
			{
				symbolTemplate = this.ReadMapMarkerTemplate(mapVectorLayer, map, context);
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapFieldDefinition ReadMapFieldDefinition(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapFieldDefinition mapFieldDefinition = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapFieldDefinition(map);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapFieldDefinitionElement(map, mapFieldDefinition, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapFieldDefinition" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapFieldDefinition;
		}

		private void ReadMapFieldDefinitionElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapFieldDefinition mapFieldDefinition, PublishingContextStruct context)
		{
			string localName;
			if ((localName = this.m_reader.LocalName) != null)
			{
				if (!(localName == "Name"))
				{
					if (localName == "DataType")
					{
						mapFieldDefinition.DataType = this.ReadDataType();
					}
				}
				else
				{
					mapFieldDefinition.Name = this.m_reader.ReadString();
				}
			}
		}

		private MapDataType ReadDataType()
		{
			return (MapDataType)Enum.Parse(typeof(MapDataType), this.m_reader.ReadString(), false);
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapFieldDefinition> ReadMapFieldDefinitions(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapFieldDefinition> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapFieldDefinition>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "MapFieldDefinition")
						{
							list.Add(this.ReadMapFieldDefinition(map, context));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "MapFieldDefinitions")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private void ReadMapLayerElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapLayer mapLayer, PublishingContextStruct context)
		{
			string localName;
			if ((localName = this.m_reader.LocalName) != null)
			{
				if (!(localName == "VisibilityMode"))
				{
					if (!(localName == "MinimumZoom"))
					{
						if (!(localName == "MaximumZoom"))
						{
							if (localName == "Transparency")
							{
								mapLayer.Transparency = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
							}
						}
						else
						{
							mapLayer.MaximumZoom = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
						}
					}
					else
					{
						mapLayer.MinimumZoom = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
					}
				}
				else
				{
					mapLayer.VisibilityMode = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
					if (!mapLayer.VisibilityMode.IsExpression)
					{
						Validator.ValidateMapVisibilityMode(mapLayer.VisibilityMode.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
					}
				}
			}
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapLayer> ReadMapLayers(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			DynamicImageObjectUniqueNameValidator nameValidator = new DynamicImageObjectUniqueNameValidator();
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapLayer> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapLayer>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "MapTileLayer":
							list.Add(this.ReadMapTileLayer(map, context, nameValidator));
							break;
						case "MapPolygonLayer":
							list.Add(this.ReadMapPolygonLayer(map, context, nameValidator));
							break;
						case "MapPointLayer":
							list.Add(this.ReadMapPointLayer(map, context, nameValidator));
							break;
						case "MapLineLayer":
							list.Add(this.ReadMapLineLayer(map, context, nameValidator));
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "MapLayers")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineLayer ReadMapLineLayer(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineLayer mapLineLayer = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineLayer(this.GenerateID(), map);
			mapLineLayer.Name = this.m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "MapLayer", AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, map.Name, mapLineLayer.Name, this.m_errorContext);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapLineLayerElement(map, mapLineLayer, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapLineLayer" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			mapLineLayer.Validate(this.m_errorContext);
			return mapLineLayer;
		}

		private void ReadMapLineLayerElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineLayer mapLineLayer, PublishingContextStruct context)
		{
			switch (this.m_reader.LocalName)
			{
			case "MapLineTemplate":
				mapLineLayer.MapLineTemplate = this.ReadMapLineTemplate(mapLineLayer, map, context);
				break;
			case "MapLineRules":
				mapLineLayer.MapLineRules = this.ReadMapLineRules(mapLineLayer, map, context);
				break;
			case "MapLines":
				mapLineLayer.MapLines = this.ReadMapLines(mapLineLayer, map, context);
				break;
			default:
				this.ReadMapVectorLayerElement(map, mapLineLayer, context);
				break;
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapShapefile ReadMapShapefile(AspNetCore.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapShapefile mapShapefile = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapShapefile(mapVectorLayer, map);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapShapefileElement(map, mapShapefile, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapShapefile" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapShapefile;
		}

		private void ReadMapShapefileElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapShapefile mapShapefile, PublishingContextStruct context)
		{
			string localName;
			if ((localName = this.m_reader.LocalName) != null)
			{
				if (!(localName == "Source"))
				{
					if (localName == "MapFieldNames")
					{
						mapShapefile.MapFieldNames = this.ReadMapFieldNames(map, context);
					}
				}
				else
				{
					mapShapefile.Source = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				}
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonLayer ReadMapPolygonLayer(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonLayer mapPolygonLayer = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonLayer(this.GenerateID(), map);
			mapPolygonLayer.Name = this.m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "MapLayer", AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, map.Name, mapPolygonLayer.Name, this.m_errorContext);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapPolygonLayerElement(map, mapPolygonLayer, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapPolygonLayer" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			mapPolygonLayer.Validate(this.m_errorContext);
			return mapPolygonLayer;
		}

		private void ReadMapPolygonLayerElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonLayer mapPolygonLayer, PublishingContextStruct context)
		{
			switch (this.m_reader.LocalName)
			{
			case "MapPolygonTemplate":
				mapPolygonLayer.MapPolygonTemplate = this.ReadMapPolygonTemplate(mapPolygonLayer, map, context);
				break;
			case "MapPolygonRules":
				mapPolygonLayer.MapPolygonRules = this.ReadMapPolygonRules(mapPolygonLayer, map, context);
				break;
			case "MapMarkerTemplate":
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointTemplate mapCenterPointTemplate = mapPolygonLayer.MapCenterPointTemplate;
				this.ReadMapPointTemplate(mapPolygonLayer, map, context, ref mapCenterPointTemplate, this.m_reader.LocalName);
				mapPolygonLayer.MapCenterPointTemplate = mapCenterPointTemplate;
				break;
			}
			case "MapCenterPointRules":
				mapPolygonLayer.MapCenterPointRules = this.ReadMapPointRules(mapPolygonLayer, map, context, "MapCenterPointRules");
				break;
			case "MapPolygons":
				mapPolygonLayer.MapPolygons = this.ReadMapPolygons(mapPolygonLayer, map, context);
				break;
			default:
				this.ReadMapVectorLayerElement(map, mapPolygonLayer, context);
				break;
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialDataRegion ReadMapSpatialDataRegion(AspNetCore.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialDataRegion mapSpatialDataRegion = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialDataRegion(mapVectorLayer, map);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapSpatialDataRegionElement(map, mapSpatialDataRegion, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapSpatialDataRegion" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapSpatialDataRegion;
		}

		private void ReadMapSpatialDataRegionElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialDataRegion mapSpatialDataRegion, PublishingContextStruct context)
		{
			string localName;
			if ((localName = this.m_reader.LocalName) != null && localName == "VectorData")
			{
				mapSpatialDataRegion.VectorData = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialDataSet ReadMapSpatialDataSet(AspNetCore.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialDataSet mapSpatialDataSet = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialDataSet(mapVectorLayer, map);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapSpatialDataSetElement(map, mapSpatialDataSet, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapSpatialDataSet" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapSpatialDataSet;
		}

		private void ReadMapSpatialDataSetElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialDataSet mapSpatialDataSet, PublishingContextStruct context)
		{
			string localName;
			if ((localName = this.m_reader.LocalName) != null)
			{
				if (!(localName == "DataSetName"))
				{
					if (!(localName == "SpatialField"))
					{
						if (localName == "MapFieldNames")
						{
							mapSpatialDataSet.MapFieldNames = this.ReadMapFieldNames(map, context);
						}
					}
					else
					{
						mapSpatialDataSet.SpatialField = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
					}
				}
				else
				{
					mapSpatialDataSet.DataSetName = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				}
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointLayer ReadMapPointLayer(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointLayer mapPointLayer = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointLayer(this.GenerateID(), map);
			mapPointLayer.Name = this.m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "MapLayer", AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, map.Name, mapPointLayer.Name, this.m_errorContext);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapPointLayerElement(map, mapPointLayer, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapPointLayer" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			mapPointLayer.Validate(this.m_errorContext);
			return mapPointLayer;
		}

		private void ReadMapPointLayerElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointLayer mapPointLayer, PublishingContextStruct context)
		{
			switch (this.m_reader.LocalName)
			{
			case "MapMarkerTemplate":
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointTemplate mapPointTemplate = mapPointLayer.MapPointTemplate;
				this.ReadMapPointTemplate(mapPointLayer, map, context, ref mapPointTemplate, this.m_reader.LocalName);
				mapPointLayer.MapPointTemplate = mapPointTemplate;
				break;
			}
			case "MapPointRules":
				mapPointLayer.MapPointRules = this.ReadMapPointRules(mapPointLayer, map, context, "MapPointRules");
				break;
			case "MapPoints":
				mapPointLayer.MapPoints = this.ReadMapPoints(mapPointLayer, map, context);
				break;
			default:
				this.ReadMapVectorLayerElement(map, mapPointLayer, context);
				break;
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapTile ReadMapTile(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapTile mapTile = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapTile(map);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapTileElement(map, mapTile, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapTile" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapTile;
		}

		private void ReadMapTileElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapTile mapTile, PublishingContextStruct context)
		{
			string localName;
			if ((localName = this.m_reader.LocalName) != null)
			{
				if (!(localName == "Name"))
				{
					if (!(localName == "TileData"))
					{
						if (localName == "MIMEType")
						{
							mapTile.MIMEType = this.m_reader.ReadString();
						}
					}
					else
					{
						mapTile.TileData = this.m_reader.ReadString();
					}
				}
				else
				{
					mapTile.Name = this.m_reader.ReadString();
				}
			}
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapTile> ReadMapTiles(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapTile> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapTile>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "MapTile")
						{
							list.Add(this.ReadMapTile(map, context));
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "MapTiles")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapTileLayer ReadMapTileLayer(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context, DynamicImageObjectUniqueNameValidator nameValidator)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapTileLayer mapTileLayer = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapTileLayer(map);
			mapTileLayer.Name = this.m_reader.GetAttribute("Name");
			nameValidator.Validate(Severity.Error, "MapLayer", AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map, map.Name, mapTileLayer.Name, this.m_errorContext);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapTileLayerElement(map, mapTileLayer, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapTileLayer" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapTileLayer;
		}

		private void ReadMapTileLayerElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapTileLayer mapTileLayer, PublishingContextStruct context)
		{
			switch (this.m_reader.LocalName)
			{
			case "ServiceUrl":
				mapTileLayer.ServiceUrl = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "TileStyle":
				mapTileLayer.TileStyle = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				if (!mapTileLayer.TileStyle.IsExpression)
				{
					Validator.ValidateMapTileStyle(mapTileLayer.TileStyle.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
				}
				break;
			case "UseSecureConnection":
				mapTileLayer.UseSecureConnection = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context);
				break;
			case "MapTiles":
				mapTileLayer.MapTiles = this.ReadMapTiles(map, context);
				break;
			default:
				this.ReadMapLayerElement(map, mapTileLayer, context);
				break;
			}
		}

		private void ReadMapVectorLayerElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, PublishingContextStruct context)
		{
			switch (this.m_reader.LocalName)
			{
			case "MapDataRegionName":
				mapVectorLayer.MapDataRegionName = this.m_reader.ReadString();
				break;
			case "MapBindingFieldPairs":
				mapVectorLayer.MapBindingFieldPairs = this.ReadMapBindingFieldPairs(map, mapVectorLayer, context);
				break;
			case "MapFieldDefinitions":
				mapVectorLayer.MapFieldDefinitions = this.ReadMapFieldDefinitions(map, context);
				break;
			case "MapShapefile":
			case "MapSpatialDataRegion":
			case "MapSpatialDataSet":
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialData mapSpatialData = mapVectorLayer.MapSpatialData;
				this.ReadMapSpatialData(mapVectorLayer, map, context, ref mapSpatialData, this.m_reader.LocalName);
				mapVectorLayer.MapSpatialData = mapSpatialData;
				break;
			}
			case "DataElementName":
				mapVectorLayer.DataElementName = this.m_reader.ReadString();
				break;
			case "DataElementOutput":
				mapVectorLayer.DataElementOutput = this.ReadDataElementOutput();
				break;
			default:
				this.ReadMapLayerElement(map, mapVectorLayer, context);
				break;
			}
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapFieldName> ReadMapFieldNames(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapFieldName> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapFieldName>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "MapFieldName")
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.MapFieldName mapFieldName = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapFieldName(map);
							mapFieldName.Name = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
							list.Add(mapFieldName);
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "MapFieldNames")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private void ReadMapSpatialData(AspNetCore.ReportingServices.ReportIntermediateFormat.MapVectorLayer mapVectorLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context, ref AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialData mapSpatialData, string propertyName)
		{
			string a;
			if (mapSpatialData != null)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsMapPropertyAlreadyDefined, Severity.Error, context.ObjectType, context.ObjectName, propertyName);
			}
			else if ((a = propertyName) != null)
			{
				if (!(a == "MapShapefile"))
				{
					if (!(a == "MapSpatialDataRegion"))
					{
						if (a == "MapSpatialDataSet")
						{
							mapSpatialData = this.ReadMapSpatialDataSet(mapVectorLayer, map, context);
						}
					}
					else
					{
						mapSpatialData = this.ReadMapSpatialDataRegion(mapVectorLayer, map, context);
					}
				}
				else
				{
					mapSpatialData = this.ReadMapShapefile(mapVectorLayer, map, context);
				}
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapBorderSkin ReadMapBorderSkin(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapBorderSkin mapBorderSkin = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapBorderSkin(map);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapBorderSkinElement(map, mapBorderSkin, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapBorderSkin" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapBorderSkin;
		}

		private void ReadMapBorderSkinElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapBorderSkin mapBorderSkin, PublishingContextStruct context)
		{
			string localName;
			if ((localName = this.m_reader.LocalName) != null)
			{
				if (!(localName == "Style"))
				{
					if (localName == "MapBorderSkinType")
					{
						mapBorderSkin.MapBorderSkinType = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
						if (!mapBorderSkin.MapBorderSkinType.IsExpression)
						{
							Validator.ValidateMapBorderSkinType(mapBorderSkin.MapBorderSkinType.StringValue, this.m_errorContext, context, this.m_reader.LocalName);
						}
					}
				}
				else
				{
					this.ReadMapStyle(mapBorderSkin, context);
				}
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomView ReadMapCustomView(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomView mapCustomView = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomView(map);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapCustomViewElement(map, mapCustomView, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapCustomView" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapCustomView;
		}

		private void ReadMapCustomViewElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomView mapCustomView, PublishingContextStruct context)
		{
			switch (this.m_reader.LocalName)
			{
			case "CenterX":
				mapCustomView.CenterX = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			case "CenterY":
				mapCustomView.CenterY = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
				break;
			default:
				this.ReadMapViewElement(map, mapCustomView, context);
				break;
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapDataBoundView ReadMapDataBoundView(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapDataBoundView mapDataBoundView = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapDataBoundView(map);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapDataBoundViewElement(map, mapDataBoundView, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapDataBoundView" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapDataBoundView;
		}

		private void ReadMapDataBoundViewElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapDataBoundView mapDataBoundView, PublishingContextStruct context)
		{
			string localName = this.m_reader.LocalName;
			this.ReadMapViewElement(map, mapDataBoundView, context);
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapElementView ReadMapElementView(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.MapElementView mapElementView = new AspNetCore.ReportingServices.ReportIntermediateFormat.MapElementView(map);
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						this.ReadMapElementViewElement(map, mapElementView, context);
						break;
					case XmlNodeType.EndElement:
						if ("MapElementView" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return mapElementView;
		}

		private void ReadMapElementViewElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapElementView mapElementView, PublishingContextStruct context)
		{
			switch (this.m_reader.LocalName)
			{
			case "LayerName":
				mapElementView.LayerName = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
				break;
			case "MapBindingFieldPairs":
				mapElementView.MapBindingFieldPairs = this.ReadMapBindingFieldPairs(map, null, context);
				break;
			default:
				this.ReadMapViewElement(map, mapElementView, context);
				break;
			}
		}

		private void ReadMapViewElement(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, AspNetCore.ReportingServices.ReportIntermediateFormat.MapView mapView, PublishingContextStruct context)
		{
			string localName;
			if ((localName = this.m_reader.LocalName) != null && localName == "Zoom")
			{
				mapView.Zoom = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context);
			}
		}

		private void ReadMapView(AspNetCore.ReportingServices.ReportIntermediateFormat.Map map, PublishingContextStruct context, ref AspNetCore.ReportingServices.ReportIntermediateFormat.MapView mapView, string propertyName)
		{
			string a;
			if (mapView != null)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsMapPropertyAlreadyDefined, Severity.Error, context.ObjectType, context.ObjectName, propertyName);
			}
			else if ((a = propertyName) != null)
			{
				if (!(a == "MapCustomView"))
				{
					if (!(a == "MapElementView"))
					{
						if (a == "MapDataBoundView")
						{
							mapView = this.ReadMapDataBoundView(map, context);
						}
					}
					else
					{
						mapView = this.ReadMapElementView(map, context);
					}
				}
				else
				{
					mapView = this.ReadMapCustomView(map, context);
				}
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.CustomReportItem ReadCustomReportItem(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem parent, PublishingContextStruct context, List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget, out AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem altReportItem)
		{
			altReportItem = null;
			AspNetCore.ReportingServices.ReportIntermediateFormat.CustomReportItem customReportItem = new AspNetCore.ReportingServices.ReportIntermediateFormat.CustomReportItem(this.GenerateID(), parent);
			customReportItem.Name = this.m_reader.GetAttribute("Name");
			context.ObjectType = customReportItem.ObjectType;
			context.ObjectName = customReportItem.Name;
			this.RegisterDataRegion(customReportItem);
			bool flag = true;
			if (!this.m_reportItemNames.Validate(context.ObjectType, context.ObjectName, context.ErrorContext))
			{
				flag = false;
			}
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsCRIInPageSection, Severity.Error, context.ObjectType, context.ObjectName, null);
				flag = false;
			}
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			if (!this.m_reader.IsEmptyElement)
			{
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context, out flag2);
							customReportItem.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, context.ErrorContext);
							break;
						}
						case "Top":
							customReportItem.Top = this.ReadSize();
							break;
						case "Left":
							customReportItem.Left = this.ReadSize();
							break;
						case "Height":
							customReportItem.Height = this.ReadSize();
							break;
						case "Width":
							customReportItem.Width = this.ReadSize();
							break;
						case "ZIndex":
							customReportItem.ZIndex = this.m_reader.ReadInteger(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
							break;
						case "Visibility":
							customReportItem.Visibility = this.ReadVisibility(context, out flag3);
							break;
						case "RepeatWith":
							customReportItem.RepeatedSibling = true;
							customReportItem.RepeatWith = this.m_reader.ReadString();
							break;
						case "Type":
							customReportItem.Type = this.m_reader.ReadString();
							break;
						case "AltReportItem":
							customReportItem.AltReportItem = this.ReadAltReportItem(parent, context, textBoxesWithDefaultSortTarget);
							break;
						case "CustomData":
							this.ReadCustomData(customReportItem, context, ref flag);
							break;
						case "CustomProperties":
							customReportItem.CustomProperties = this.ReadCustomProperties(context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("CustomReportItem" == this.m_reader.LocalName)
						{
							flag4 = true;
						}
						break;
					}
				}
				while (!flag4);
			}
			customReportItem.Computed = true;
			if (customReportItem.AltReportItem == null)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.Rectangle rectangle = new AspNetCore.ReportingServices.ReportIntermediateFormat.Rectangle(this.GenerateID(), this.GenerateID(), parent);
				rectangle.Name = customReportItem.Name + "_" + customReportItem.ID + "_" + rectangle.ID;
				this.m_reportItemNames.Validate(rectangle.ObjectType, rectangle.Name, context.ErrorContext);
				rectangle.Computed = false;
				AspNetCore.ReportingServices.ReportIntermediateFormat.Visibility visibility = new AspNetCore.ReportingServices.ReportIntermediateFormat.Visibility();
				visibility.Hidden = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(true);
				rectangle.Visibility = visibility;
				this.m_reportItemCollectionList.Add(rectangle.ReportItems);
				customReportItem.AltReportItem = rectangle;
			}
			else
			{
				customReportItem.ExplicitlyDefinedAltReportItem = true;
			}
			customReportItem.AltReportItem.Top = customReportItem.Top;
			customReportItem.AltReportItem.Left = customReportItem.Left;
			customReportItem.AltReportItem.Height = customReportItem.Height;
			customReportItem.AltReportItem.Width = customReportItem.Width;
			customReportItem.AltReportItem.ZIndex = customReportItem.ZIndex;
			if (flag)
			{
				this.m_createSubtotalsDefs.Add(customReportItem);
			}
			altReportItem = customReportItem.AltReportItem;
			if (!flag)
			{
				return null;
			}
			return customReportItem;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem ReadAltReportItem(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem parent, PublishingContextStruct context, List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem result = null;
			if (!this.m_reader.IsEmptyElement)
			{
				int num = 0;
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Line":
							result = this.ReadLine(parent, context);
							num++;
							break;
						case "Rectangle":
							result = this.ReadRectangle(parent, context, textBoxesWithDefaultSortTarget);
							num++;
							break;
						case "CustomReportItem":
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidAltReportItem, Severity.Error, context.ObjectType, context.ObjectName, "AltReportItem");
							num++;
							break;
						case "Textbox":
							result = this.ReadTextbox(parent, context, textBoxesWithDefaultSortTarget);
							num++;
							break;
						case "Image":
							result = this.ReadImage(parent, context);
							num++;
							break;
						case "Subreport":
							result = this.ReadSubreport(parent, context);
							num++;
							break;
						case "Tablix":
							result = this.ReadTablix(parent, context);
							num++;
							break;
						case "Chart":
							result = this.ReadChart(parent, context);
							num++;
							break;
						}
						if (num > 1)
						{
							result = null;
							context.ErrorContext.Register(ProcessingErrorCode.rsMultiReportItemsInCustomReportItem, Severity.Error, context.ObjectType, context.ObjectName, "AltReportItem");
						}
						break;
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "AltReportItem")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return result;
		}

		private void ReadCustomData(AspNetCore.ReportingServices.ReportIntermediateFormat.CustomReportItem crItem, PublishingContextStruct context, ref bool validName)
		{
			crItem.SetAsDataRegion();
			if ((context.Location & LocationFlags.InDataRegion) != 0)
			{
				Global.Tracer.Assert(this.m_nestedDataRegions != null);
				this.m_nestedDataRegions.Add(crItem);
			}
			context.Location = (context.Location | LocationFlags.InDataSet | LocationFlags.InDataRegion);
			if (this.m_scopeNames.Validate(false, context.ObjectName, context.ObjectType, context.ObjectName, context.ErrorContext))
			{
				this.m_reportScopes.Add(crItem.Name, crItem);
			}
			else
			{
				validName = true;
			}
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsDataRegionInPageSection, Severity.Error, context.ObjectType, context.ObjectName, null);
			}
			bool flag = false;
			IdcRelationship relationship = null;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "DataSetName":
						crItem.DataSetName = this.m_reader.ReadString();
						break;
					case "Relationship":
						relationship = this.ReadRelationship(context);
						break;
					case "DataColumnHierarchy":
						crItem.DataColumnMembers = this.ReadCustomDataHierarchy(crItem, context, true, ref validName);
						break;
					case "DataRowHierarchy":
						crItem.DataRowMembers = this.ReadCustomDataHierarchy(crItem, context, false, ref validName);
						break;
					case "DataRows":
						crItem.DataRows = this.ReadCustomDataRows(crItem, context);
						break;
					case "Filters":
						crItem.Filters = this.ReadFilters(AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.DataRegionFilters, context);
						break;
					case "SortExpressions":
						crItem.Sorting = this.ReadSortExpressions(true, context);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("CustomData" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			crItem.DataScopeInfo.SetRelationship(crItem.DataSetName, relationship);
		}

		private DataMemberList ReadCustomDataHierarchy(AspNetCore.ReportingServices.ReportIntermediateFormat.CustomReportItem crItem, PublishingContextStruct context, bool isColumnHierarchy, ref bool validName)
		{
			DataMemberList result = null;
			int num = 0;
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "DataMembers")
					{
						result = this.ReadCustomDataMembers(crItem, context, isColumnHierarchy, 0, ref num, ref validName);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if (this.m_reader.LocalName == (isColumnHierarchy ? "DataColumnHierarchy" : "DataRowHierarchy"))
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (isColumnHierarchy)
			{
				crItem.ColumnCount = num;
			}
			else
			{
				crItem.RowCount = num;
			}
			return result;
		}

		private DataMemberList ReadCustomDataMembers(AspNetCore.ReportingServices.ReportIntermediateFormat.CustomReportItem crItem, PublishingContextStruct context, bool isColumnHierarchy, int level, ref int leafNodes, ref bool validName)
		{
			DataMemberList dataMemberList = new DataMemberList();
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "DataMember")
					{
						dataMemberList.Add(this.ReadCustomDataMember(crItem, context, isColumnHierarchy, level, ref leafNodes, ref validName));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("DataMembers" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (dataMemberList.Count <= 0)
			{
				return null;
			}
			return dataMemberList;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.DataMember ReadCustomDataMember(AspNetCore.ReportingServices.ReportIntermediateFormat.CustomReportItem crItem, PublishingContextStruct context, bool isColumnHierarchy, int level, ref int aLeafNodes, ref bool validName)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataMember dataMember = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataMember(this.GenerateID(), crItem);
			this.m_runningValueHolderList.Add(dataMember);
			dataMember.IsColumn = isColumnHierarchy;
			dataMember.Level = level;
			int num = 0;
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Group":
							dataMember.Grouping = this.ReadGrouping((AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode)dataMember, context, ref validName);
							break;
						case "SortExpressions":
							dataMember.Sorting = this.ReadSortExpressions(false, context);
							break;
						case "CustomProperties":
							dataMember.CustomProperties = this.ReadCustomProperties(context);
							break;
						case "DataMembers":
							dataMember.SubMembers = this.ReadCustomDataMembers(crItem, context, isColumnHierarchy, level + 1, ref num, ref validName);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("DataMember" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			if (dataMember.SubMembers == null || dataMember.SubMembers.Count == 0)
			{
				aLeafNodes++;
				if (isColumnHierarchy)
				{
					dataMember.ColSpan = 1;
				}
				else
				{
					dataMember.RowSpan = 1;
				}
			}
			else
			{
				aLeafNodes += num;
				if (isColumnHierarchy)
				{
					dataMember.ColSpan = num;
				}
				else
				{
					dataMember.RowSpan = num;
				}
			}
			this.ValidateAndProcessMemberGroupAndSort(dataMember, context);
			return dataMember;
		}

		private CustomDataRowList ReadCustomDataRows(AspNetCore.ReportingServices.ReportIntermediateFormat.CustomReportItem crItem, PublishingContextStruct context)
		{
			CustomDataRowList customDataRowList = new CustomDataRowList();
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "DataRow")
					{
						customDataRowList.Add(this.ReadCustomDataRow(crItem, context));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("DataRows" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return customDataRowList;
		}

		private CustomDataRow ReadCustomDataRow(AspNetCore.ReportingServices.ReportIntermediateFormat.CustomReportItem crItem, PublishingContextStruct context)
		{
			CustomDataRow customDataRow = new CustomDataRow(this.GenerateID());
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "DataCell")
					{
						if (customDataRow.DataCells == null)
						{
							customDataRow.DataCells = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataCellList();
						}
						customDataRow.DataCells.Add(this.ReadCustomDataCell(crItem, context));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("DataRow" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return customDataRow;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.DataCell ReadCustomDataCell(AspNetCore.ReportingServices.ReportIntermediateFormat.CustomReportItem crItem, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataCell dataCell = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataCell(this.GenerateID(), crItem);
			this.m_aggregateHolderList.Add(dataCell);
			this.m_runningValueHolderList.Add(dataCell);
			string dataSetName = null;
			List<IdcRelationship> relationships = null;
			int num = 0;
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "DataValue":
						if (dataCell.DataValues == null)
						{
							dataCell.DataValues = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataValueList();
						}
						dataCell.DataValues.Add(this.ReadDataValue(false, false, ++num, context));
						break;
					case "DataSetName":
						dataSetName = this.m_reader.ReadString();
						break;
					case "Relationships":
						relationships = this.ReadRelationships(context);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if (this.m_reader.LocalName == "DataCell")
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			dataCell.DataScopeInfo.SetRelationship(dataSetName, relationships);
			return dataCell;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.DataValue ReadDataValue(bool isCustomProperty, bool nameRequired, int index, PublishingContextStruct context)
		{
			bool flag = false;
			return this.ReadDataValue(isCustomProperty, nameRequired, index, ref flag, context);
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.DataValue ReadDataValue(bool isCustomProperty, bool nameRequired, int index, ref bool isComputed, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataValue dataValue = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataValue();
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "Name":
						dataValue.Name = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out flag);
						break;
					case "Value":
						dataValue.Value = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out flag2);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if (!isCustomProperty || !("CustomProperty" == this.m_reader.LocalName))
					{
						if (isCustomProperty)
						{
							break;
						}
						if (!("DataValue" == this.m_reader.LocalName))
						{
							break;
						}
					}
					flag3 = true;
					break;
				}
			}
			while (!flag3);
			Global.Tracer.Assert(null != dataValue.Value);
			if (dataValue.Name == null && nameRequired)
			{
				if (isCustomProperty)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsMissingCustomPropertyName, Severity.Error, context.ObjectType, context.ObjectName, "Name", index.ToString(CultureInfo.CurrentCulture));
				}
				else
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsMissingChartDataValueName, Severity.Error, context.ObjectType, context.ObjectName, "DataValue", index.ToString(CultureInfo.CurrentCulture), "Name");
				}
			}
			isComputed = (isComputed || flag2 || flag);
			return dataValue;
		}

		private void ReadConnectionProperties(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource dataSource, PublishingContextStruct context, ref bool hasComplexParams, Dictionary<string, bool> parametersInQuery)
		{
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "DataProvider":
						dataSource.Type = this.m_reader.ReadString();
						break;
					case "ConnectString":
						Global.Tracer.Assert(AspNetCore.ReportingServices.ReportProcessing.ObjectType.DataSource == context.ObjectType);
						dataSource.ConnectStringExpression = this.ReadQueryOrParameterExpression(context, DataType.String, ref hasComplexParams, parametersInQuery);
						if (!dataSource.ConnectStringExpression.IsExpression && DataSourceInfo.HasUseridReference(dataSource.ConnectStringExpression.OriginalText))
						{
							this.SetConnectionStringUserProfileDependency();
						}
						break;
					case "IntegratedSecurity":
						dataSource.IntegratedSecurity = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						break;
					case "Prompt":
						dataSource.Prompt = this.m_reader.ReadString();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("ConnectionProperties" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private void ReadDataSets(PublishingContextStruct context)
		{
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "DataSet")
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet = this.ReadDataSet(context);
						this.m_dataSets.Add(dataSet);
						if (dataSet.IsReferenceToSharedDataSet && this.m_report.SharedDSContainer == null)
						{
							this.m_report.SharedDSContainerCollectionIndex = this.m_report.DataSourceCount;
							this.m_report.SharedDSContainer = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource(this.GenerateID(), Guid.Empty);
							if (this.m_report.DataSources == null)
							{
								this.m_report.DataSources = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource>();
							}
							this.m_report.DataSources.Add(this.m_report.SharedDSContainer);
						}
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("DataSets" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet ReadDataSet(PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet(this.GenerateID(), this.m_dataSetIndexCounter++);
			dataSet.Name = this.m_reader.GetAttribute("Name");
			context.Location |= LocationFlags.InDataSet;
			context.ObjectType = dataSet.ObjectType;
			context.ObjectName = dataSet.Name;
			if (this.m_scopeNames.Validate(false, context.ObjectName, context.ObjectType, context.ObjectName, context.ErrorContext))
			{
				this.m_reportScopes.Add(dataSet.Name, dataSet);
			}
			this.m_aggregateHolderList.Add(dataSet);
			bool isComplex = false;
			Dictionary<string, bool> referencedReportParameters = new Dictionary<string, bool>();
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "Fields":
					{
						int nonCalculatedFieldCount = default(int);
						dataSet.Fields = this.ReadFields(context, out nonCalculatedFieldCount);
						dataSet.NonCalculatedFieldCount = nonCalculatedFieldCount;
						break;
					}
					case "Query":
						dataSet.Query = this.ReadQuery(context, ref isComplex, referencedReportParameters);
						break;
					case "SharedDataSet":
						dataSet.SharedDataSetQuery = this.ReadSharedDataSetQuery(context, ref isComplex, referencedReportParameters);
						break;
					case "CaseSensitivity":
						dataSet.CaseSensitivity = this.ReadTriState();
						break;
					case "Collation":
					{
						dataSet.Collation = this.m_reader.ReadString();
						uint lCID = default(uint);
						if (DataSetValidator.ValidateCollation(dataSet.Collation, out lCID))
						{
							dataSet.LCID = lCID;
						}
						else
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidCollationName, Severity.Warning, context.ObjectType, context.ObjectName, null, dataSet.Collation.MarkAsPrivate());
						}
						break;
					}
					case "CollationCulture":
						dataSet.CollationCulture = this.m_reader.ReadString();
						this.ValidateCollationCultureAndSetLcid(dataSet, context);
						break;
					case "AccentSensitivity":
						dataSet.AccentSensitivity = this.ReadTriState();
						break;
					case "KanatypeSensitivity":
						dataSet.KanatypeSensitivity = this.ReadTriState();
						break;
					case "WidthSensitivity":
						dataSet.WidthSensitivity = this.ReadTriState();
						break;
					case "NullsAsBlanks":
						dataSet.NullsAsBlanks = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						break;
					case "Filters":
						dataSet.Filters = this.ReadFilters(AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.DataSetFilters, context, ref isComplex, referencedReportParameters);
						break;
					case "InterpretSubtotalsAsDetails":
						dataSet.InterpretSubtotalsAsDetails = this.ReadTriState();
						break;
					case "DefaultRelationships":
						dataSet.DefaultRelationships = this.ReadDefaultRelationships(context);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("DataSet" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (!string.IsNullOrEmpty(dataSet.Collation) && !string.IsNullOrEmpty(dataSet.CollationCulture))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsCollationAndCollationCultureSpecified, Severity.Error, context.ObjectType, context.ObjectName, "CollationCulture", "Collation");
			}
			this.ValidateDataSet(dataSet, context, isComplex, referencedReportParameters);
			return dataSet;
		}

		private SharedDataSetQuery ReadSharedDataSetQuery(PublishingContextStruct context, ref bool isComplex, Dictionary<string, bool> referencedReportParameters)
		{
			SharedDataSetQuery sharedDataSetQuery = new SharedDataSetQuery();
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "SharedDataSetReference":
						sharedDataSetQuery.SharedDataSetReference = this.m_reader.ReadString();
						break;
					case "QueryParameters":
						sharedDataSetQuery.Parameters = this.ReadQueryParameters(context, ref isComplex, referencedReportParameters);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("SharedDataSet" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return sharedDataSetQuery;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ReportQuery ReadQuery(PublishingContextStruct context, ref bool isComplex, Dictionary<string, bool> referencedReportParameters)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ReportQuery reportQuery = new AspNetCore.ReportingServices.ReportIntermediateFormat.ReportQuery();
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "DataSourceName":
						reportQuery.DataSourceName = this.m_reader.ReadString();
						break;
					case "CommandType":
						reportQuery.CommandType = this.ReadCommandType();
						break;
					case "CommandText":
						Global.Tracer.Assert(AspNetCore.ReportingServices.ReportProcessing.ObjectType.DataSet == context.ObjectType);
						context.ObjectType = AspNetCore.ReportingServices.ReportProcessing.ObjectType.Query;
						reportQuery.CommandText = this.ReadQueryOrParameterExpression(context, DataType.String, ref isComplex, referencedReportParameters);
						context.ObjectType = AspNetCore.ReportingServices.ReportProcessing.ObjectType.DataSet;
						break;
					case "QueryParameters":
						reportQuery.Parameters = this.ReadQueryParameters(context, ref isComplex, referencedReportParameters);
						break;
					case "Timeout":
						reportQuery.TimeOut = this.m_reader.ReadInteger(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Query" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return reportQuery;
		}

		private CommandType ReadCommandType()
		{
			string value = this.m_reader.ReadString();
			return (CommandType)Enum.Parse(typeof(CommandType), value, false);
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterValue> ReadQueryParameters(PublishingContextStruct context, ref bool hasComplexParams, Dictionary<string, bool> parametersInQuery)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterValue> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterValue>();
			bool flag = false;
			string objectName = context.ObjectName;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "QueryParameter":
						list.Add(this.ReadQueryParameter(context, ref hasComplexParams, parametersInQuery));
						break;
					case "DataSetParameter":
						list.Add(this.ReadRSDDataSetParameter(context, ref hasComplexParams, parametersInQuery));
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if (!("QueryParameters" == this.m_reader.LocalName) && !("DataSetParameters" == this.m_reader.LocalName))
					{
						break;
					}
					flag = true;
					break;
				}
			}
			while (!flag);
			context.ObjectName = objectName;
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterValue ReadQueryParameter(PublishingContextStruct context, ref bool isComplex, Dictionary<string, bool> parametersInQuery)
		{
			Global.Tracer.Assert(AspNetCore.ReportingServices.ReportProcessing.ObjectType.DataSet == context.ObjectType);
			AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterValue parameterValue = new AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterValue();
			parameterValue.Name = this.m_reader.GetAttribute("Name");
			context.ObjectType = AspNetCore.ReportingServices.ReportProcessing.ObjectType.QueryParameter;
			context.ObjectName = parameterValue.Name;
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "Value")
					{
						parameterValue.ConstantDataType = this.ReadDataTypeAttribute();
						parameterValue.Value = this.ReadQueryOrParameterExpression(context, parameterValue.ConstantDataType, ref isComplex, parametersInQuery);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("QueryParameter" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return parameterValue;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.Field> ReadFields(PublishingContextStruct context, out int calculatedFieldStartIndex)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.Field> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.Field>();
			List<string> aggregateIndicatorFieldNames = new List<string>();
			CLSUniqueNameValidator names = new CLSUniqueNameValidator(ProcessingErrorCode.rsInvalidFieldNameNotCLSCompliant, ProcessingErrorCode.rsDuplicateFieldName, ProcessingErrorCode.rsInvalidFieldNameLength);
			AspNetCore.ReportingServices.ReportIntermediateFormat.Field field = null;
			bool flag = false;
			calculatedFieldStartIndex = -1;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "Field")
					{
						string aggregateIndicatorFieldName = default(string);
						field = this.ReadField(names, context, out aggregateIndicatorFieldName);
						this.InsertField(field, aggregateIndicatorFieldName, list, aggregateIndicatorFieldNames, ref calculatedFieldStartIndex);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("Fields" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (0 > calculatedFieldStartIndex)
			{
				calculatedFieldStartIndex = list.Count;
			}
			ReportPublishing.AssignAndValidateAggregateIndicatorFieldIndex(context, list, aggregateIndicatorFieldNames);
			return list;
		}

		private static void AssignAndValidateAggregateIndicatorFieldIndex(PublishingContextStruct context, List<AspNetCore.ReportingServices.ReportIntermediateFormat.Field> fields, List<string> aggregateIndicatorFieldNames)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>(fields.Count, StringComparer.Ordinal);
			for (int i = 0; i < fields.Count; i++)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.Field field = fields[i];
				dictionary[field.Name] = i;
			}
			for (int j = 0; j < aggregateIndicatorFieldNames.Count; j++)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.Field field2 = fields[j];
				string text = aggregateIndicatorFieldNames[j];
				if (!string.IsNullOrEmpty(text))
				{
					if (field2.IsCalculatedField)
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsAggregateIndicatorFieldOnCalculatedField, Severity.Error, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Field, field2.Name, "AggregateIndicatorField", "Value", context.ObjectName.MarkAsPrivate());
					}
					int num = default(int);
					if (dictionary.TryGetValue(text, out num))
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.Field field3 = fields[num];
						if (field3.IsCalculatedField && field3.Value.Type != AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Literal && (field3.Value.Type != AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant || field3.Value.ConstantType != DataType.Boolean))
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidAggregateIndicatorField, Severity.Error, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Field, field2.Name, "AggregateIndicatorField", context.ObjectName.MarkAsPrivate());
						}
						field2.AggregateIndicatorFieldIndex = num;
					}
					else
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsInvalidAggregateIndicatorField, Severity.Error, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Field, field2.Name, "AggregateIndicatorField", context.ObjectName.MarkAsModelInfo());
					}
				}
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Field ReadField(CLSUniqueNameValidator names, PublishingContextStruct context, out string aggregateIndicatorFieldName)
		{
			Global.Tracer.Assert(AspNetCore.ReportingServices.ReportProcessing.ObjectType.DataSet == context.ObjectType || AspNetCore.ReportingServices.ReportProcessing.ObjectType.SharedDataSet == context.ObjectType);
			string objectName = context.ObjectName;
			AspNetCore.ReportingServices.ReportIntermediateFormat.Field field = new AspNetCore.ReportingServices.ReportIntermediateFormat.Field();
			context.ObjectType = AspNetCore.ReportingServices.ReportProcessing.ObjectType.Field;
			string text = null;
			aggregateIndicatorFieldName = null;
			field.Name = this.m_reader.GetAttribute("Name");
			Global.Tracer.Assert(null != field.Name, "Name is a mandatory attribute of field elements");
			context.ObjectName = field.Name;
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "DataField":
							field.DataField = this.m_reader.ReadString();
							names.Validate(field.Name, field.DataField, objectName, context.ErrorContext);
							break;
						case "AggregateIndicatorField":
							aggregateIndicatorFieldName = this.m_reader.ReadString();
							break;
						case "Value":
						{
							field.DataType = this.ReadDataTypeAttribute();
							AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.EvaluationMode mode = this.ReadEvaluationModeAttribute();
							text = this.m_reader.ReadString();
							if (text != null)
							{
								field.Value = this.ReadExpression(text, this.m_reader.LocalName, objectName, mode, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.FieldValue, field.DataType, context);
							}
							break;
						}
						}
						break;
					case XmlNodeType.EndElement:
						if ("Field" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			this.ValidateField(field, text, context, objectName);
			return field;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.Filter> ReadFilters(AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType expressionType, PublishingContextStruct context)
		{
			bool flag = false;
			Dictionary<string, bool> referencedReportParameters = new Dictionary<string, bool>();
			return this.ReadFilters(expressionType, context, ref flag, referencedReportParameters);
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.Filter> ReadFilters(AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType expressionType, PublishingContextStruct context, ref bool isComplex, Dictionary<string, bool> referencedReportParameters)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.Filter> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.Filter>();
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "Filter")
					{
						list.Add(this.ReadFilter(expressionType, context, ref isComplex, referencedReportParameters));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("Filters" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (list.Count > 0 && this.m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.Filters))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, "Filters");
			}
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Filter ReadFilter(AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType expressionType, PublishingContextStruct context, ref bool isComplex, Dictionary<string, bool> referencedReportParameters)
		{
			this.m_hasFilters = true;
			AspNetCore.ReportingServices.ReportIntermediateFormat.Filter filter = new AspNetCore.ReportingServices.ReportIntermediateFormat.Filter();
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "FilterExpression":
						filter.Expression = this.ReadFilterExpression(this.m_reader.LocalName, expressionType, DataType.String, context, ref isComplex, referencedReportParameters);
						break;
					case "Operator":
						filter.Operator = this.ReadOperator();
						break;
					case "FilterValues":
						filter.Values = this.ReadFilterValues(expressionType, context, ref isComplex, referencedReportParameters);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Filter" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			int num = (filter.Values != null) ? filter.Values.Count : 0;
			switch (filter.Operator)
			{
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.Equal:
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.Like:
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.GreaterThan:
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.GreaterThanOrEqual:
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.LessThan:
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.LessThanOrEqual:
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.NotEqual:
				this.VerifyFilterValueCount(context, filter, num, 1);
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.TopN:
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.BottomN:
				this.VerifyTopBottomFilterValue(context, filter, num, false);
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.TopPercent:
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.BottomPercent:
				this.VerifyTopBottomFilterValue(context, filter, num, true);
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators.Between:
				this.VerifyFilterValueCount(context, filter, num, 2);
				break;
			}
			if (AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.GroupingFilters == expressionType && filter.Expression.HasRecursiveAggregates())
			{
				this.m_hasSpecialRecursiveAggregates = true;
			}
			return filter;
		}

		private void VerifyTopBottomFilterValue(PublishingContextStruct context, AspNetCore.ReportingServices.ReportIntermediateFormat.Filter filter, int count, bool isPercentFilter)
		{
			if (this.VerifyFilterValueCount(context, filter, count, 1))
			{
				ExpressionInfoTypeValuePair expressionInfoTypeValuePair = filter.Values[0];
				if (!expressionInfoTypeValuePair.Value.IsExpression)
				{
					if (expressionInfoTypeValuePair.HadExplicitDataType)
					{
						if (isPercentFilter)
						{
							if (expressionInfoTypeValuePair.DataType != DataType.Integer && expressionInfoTypeValuePair.DataType != DataType.Float)
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFilterValueDataType, Severity.Error, context.ObjectType, context.ObjectName, "FilterValues", filter.Operator.ToString(), RPRes.rsDataTypeIntegerOrFloat);
							}
						}
						else if (expressionInfoTypeValuePair.DataType != DataType.Integer)
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFilterValueDataType, Severity.Error, context.ObjectType, context.ObjectName, "FilterValues", filter.Operator.ToString(), RPRes.rsDataTypeInteger);
						}
					}
					else
					{
						DataType constantType = (DataType)((!isPercentFilter) ? 9 : 13);
						AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo value = expressionInfoTypeValuePair.Value;
						AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ParseRDLConstant(value.StringValue, value, constantType, context.ErrorContext, context.ObjectType, context.ObjectName, "FilterValues");
					}
				}
			}
		}

		private bool VerifyFilterValueCount(PublishingContextStruct context, AspNetCore.ReportingServices.ReportIntermediateFormat.Filter filter, int expectedCount, int actualCount)
		{
			if (expectedCount != actualCount)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidNumberOfFilterValues, Severity.Error, context.ObjectType, context.ObjectName, "FilterValues", filter.Operator.ToString(), Convert.ToString(expectedCount, CultureInfo.InvariantCulture));
				return false;
			}
			return true;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadFilterExpression(string propertyName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType expressionType, DataType dataType, PublishingContextStruct context, ref bool isComplex, Dictionary<string, bool> referencedReportParameters)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = this.ReadExpression(this.m_reader.LocalName, expressionType, dataType, context);
			if (expressionInfo != null)
			{
				isComplex |= expressionInfo.HasDynamicParameterReference;
				if (expressionInfo.ReferencedParameters != null)
				{
					{
						foreach (string referencedParameter in expressionInfo.ReferencedParameters)
						{
							if (!string.IsNullOrEmpty(referencedParameter))
							{
								referencedReportParameters[referencedParameter] = true;
							}
						}
						return expressionInfo;
					}
				}
			}
			return expressionInfo;
		}

		private List<ExpressionInfoTypeValuePair> ReadFilterValues(AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType expressionType, PublishingContextStruct context, ref bool isComplex, Dictionary<string, bool> referencedReportParameters)
		{
			List<ExpressionInfoTypeValuePair> list = new List<ExpressionInfoTypeValuePair>();
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "FilterValue")
					{
						bool hadExplicitDataType = default(bool);
						DataType dataType = this.ReadDataTypeAttribute(out hadExplicitDataType);
						AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = this.ReadFilterExpression(this.m_reader.LocalName, expressionType, dataType, context, ref isComplex, referencedReportParameters);
						list.Add(new ExpressionInfoTypeValuePair(dataType, hadExplicitDataType, expressionInfo));
						if (AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.GroupingFilters == expressionType && expressionInfo.HasRecursiveAggregates())
						{
							this.m_hasSpecialRecursiveAggregates = true;
						}
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("FilterValues" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators ReadOperator()
		{
			string value = this.m_reader.ReadString();
			return (AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators)Enum.Parse(typeof(AspNetCore.ReportingServices.ReportIntermediateFormat.Filter.Operators), value, false);
		}

		private DataSetReference ReadDataSetReference()
		{
			string dataSet = null;
			string valueAlias = null;
			string labelAlias = null;
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "DataSetName":
						dataSet = this.m_reader.ReadString();
						break;
					case "ValueField":
						valueAlias = this.m_reader.ReadString();
						break;
					case "LabelField":
						labelAlias = this.m_reader.ReadString();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("DataSetReference" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return new DataSetReference(dataSet, valueAlias, labelAlias);
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource> ReadDataSources(PublishingContextStruct context, IDataProtection dataProtection)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource>();
			DataSourceNameValidator dataSourceNames = new DataSourceNameValidator();
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "DataSource")
					{
						list.Add(this.ReadDataSource(dataSourceNames, context, dataProtection));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("DataSources" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return list;
		}

        private AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource ReadDataSource(DataSourceNameValidator dataSourceNames, PublishingContextStruct context, IDataProtection dataProtection)
        {
            AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource dataSource = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource(this.GenerateID());
            dataSource.Name = this.m_reader.GetAttribute("Name");
            context.ObjectType = AspNetCore.ReportingServices.ReportProcessing.ObjectType.DataSource;
            context.ObjectName = dataSource.Name;
            bool flag = false;
            if (dataSourceNames.Validate(context.ObjectType, context.ObjectName, context.ErrorContext))
            {
                flag = true;
            }
            bool flag2 = false;
            bool flag3 = false;
            bool isComplex = false;
            Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
            if (!this.m_reader.IsEmptyElement)
            {
                bool flag4 = false;
                do
                {
                    this.m_reader.Read();
                    switch (this.m_reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (this.m_reader.LocalName)
                            {
                                case "Transaction":
                                    dataSource.Transaction = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
                                    break;
                                case "ConnectionProperties":
                                    flag2 = true;
                                    this.ReadConnectionProperties(dataSource, context, ref isComplex, dictionary);
                                    break;
                                case "DataSourceReference":
                                    flag3 = true;
                                    dataSource.Type = "System.Data.DataSet";
                                    this.m_reportLocationFlags = UserLocationFlags.ReportQueries;
                                    var refer = this.m_reader.ReadString();
                                    var path = System.IO.Path.GetDirectoryName(this.m_publishingContext.CatalogContext.StableItemPath);
                                    var rds = System.IO.Path.Combine(path, $"{refer}.rds");
                                    if (!System.IO.File.Exists(rds))
                                    {
                                        rds = System.IO.Path.Combine(path, $"{dataSource.Name}.rds");
                                    }
                                    if (System.IO.File.Exists(rds))
                                    {
                                        try
                                        {
                                            XmlDocument xmlDocument = new XmlDocument();
                                            xmlDocument.Load(rds);
                                            var name = xmlDocument.DocumentElement.Attributes["Name"].Value;
                                            var connectionString = xmlDocument.SelectSingleNode("RptDataSource/ConnectionProperties/ConnectString").InnerText;
                                            dataSource.ConnectStringExpression = this.ReadQueryOrParameterExpression(connectionString, refer, this.ReadEvaluationModeAttribute(), context, DataType.String, ref isComplex, dictionary);
                                        }
                                        catch (Exception ex)
                                        {
                                            throw ex;
                                        }
                                    }
                                    dataSource.DataSourceReference = refer;
                                    break;
                            }
                            break;
                        case XmlNodeType.EndElement:
                            if ("DataSource" == this.m_reader.LocalName)
                            {
                                flag4 = true;
                            }
                            break;
                    }
                }
                while (!flag4);
            }
            if (!flag3 && !flag2)
            {
                goto IL_0163;
            }
            if (flag3 && flag2)
            {
                goto IL_0163;
            }
            goto IL_018a;
        IL_0163:
            flag = false;
            context.ErrorContext.Register(ProcessingErrorCode.rsInvalidDataSource, Severity.Error, context.ObjectType, context.ObjectName, null);
            goto IL_018a;
        IL_018a:
            if (flag && !this.m_dataSourceNames.ContainsKey(dataSource.Name))
            {
                this.m_dataSourceNames.Add(dataSource.Name, null);
            }
            DataSourceInfo dataSourceInfo = null;
            if (flag2)
            {
                dataSource.IsComplex = isComplex;
                dataSource.ParameterNames = dictionary;
                bool flag5 = false;
                if (dataSource.ConnectStringExpression.Type != AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant)
                {
                    flag5 = true;
                }
                dataSourceInfo = new DataSourceInfo(dataSource.Name, dataSource.Type, flag5 ? null : dataSource.ConnectStringExpression.OriginalText, flag5, dataSource.IntegratedSecurity, dataSource.Prompt, dataProtection);
            }
            else if (flag3)
            {
                dataSourceInfo = this.CreateSharedDataSourceLink(context, dataSource);
            }
            if (dataSourceInfo != null)
            {
                if (this.m_publishingContext.ResolveTemporaryDataSourceCallback != null)
                {
                    this.m_publishingContext.ResolveTemporaryDataSourceCallback(dataSourceInfo, this.m_publishingContext.OriginalDataSources);
                }
                dataSource.ID = dataSourceInfo.ID;
                this.m_dataSources.Add(dataSourceInfo);
            }
            return dataSource;
        }

		private DataSourceInfo CreateSharedDataSourceLink(PublishingContextStruct context, AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource dataSource)
		{
            if (!string.IsNullOrEmpty(dataSource.DataSourceReference))
            {
                return new DataSourceInfo(dataSource.Name,"rds",dataSource.ConnectStringExpression.OriginalText,false,"",DataProtectionLocal.Instance);
            }
			DataSourceInfo dataSourceInfo = null;
            string text = (this.m_publishingContext.CatalogContext == null) ? dataSource.DataSourceReference :
                Path.Combine(Path.GetDirectoryName(this.m_publishingContext.CatalogContext.StableItemPath), $"{dataSource.DataSourceReference}.rds");
			if (this.m_publishingContext.CheckDataSourceCallback == null)
			{
				dataSourceInfo = new DataSourceInfo(dataSource.Name, text, Guid.Empty);
			}
			else
			{
				Guid empty = Guid.Empty;
				DataSourceInfo dataSourceInfo2 = this.m_publishingContext.CheckDataSourceCallback(text, out empty);
				if (dataSourceInfo2 == null)
				{
					dataSourceInfo = new DataSourceInfo(dataSource.Name);
					string plainString = (this.m_publishingContext.PublishingContextKind == PublishingContextKind.SharedDataSet) ? dataSource.DataSourceReference : dataSource.Name;
					context.ErrorContext.Register(ProcessingErrorCode.rsDataSourceReferenceNotPublished, Severity.Warning, context.ObjectType, context.ObjectName, (this.m_publishingContext.PublishingContextKind == PublishingContextKind.SharedDataSet) ? RPRes.rsObjectTypeSharedDataSet : RPRes.rsObjectTypeReport, plainString.MarkAsPrivate());
				}
				else
				{
					dataSourceInfo = new DataSourceInfo(dataSource.Name, text, empty, dataSourceInfo2);
				}
			}
			return dataSourceInfo;
		}

		private void ValidateCollationCultureAndSetLcid(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet, PublishingContextStruct context)
		{
			CultureInfo cultureInfo = default(CultureInfo);
			if (Validator.ValidateSpecificLanguage(dataSet.CollationCulture, out cultureInfo))
			{
				if (cultureInfo != null)
				{
					dataSet.LCID = (uint)cultureInfo.LCID;
				}
			}
			else
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidLanguage, Severity.Error, context.ObjectType, context.ObjectName, "CollationCulture", dataSet.CollationCulture);
			}
		}

		private void ValidateDataSet(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet, PublishingContextStruct context, bool isComplex, Dictionary<string, bool> referencedReportParameters)
		{
			PublishingDataSetInfo publishingDataSetInfo = new PublishingDataSetInfo(dataSet.Name, this.m_dataSets.Count, isComplex, referencedReportParameters);
			if (null == dataSet.Query == (null == dataSet.SharedDataSetQuery))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidDataSetQuery, Severity.Error, context.ObjectType, dataSet.Name, null);
			}
			else
			{
				if (!this.m_dataSetQueryInfo.ContainsKey(context.ObjectName))
				{
					this.m_dataSetQueryInfo.Add(context.ObjectName, publishingDataSetInfo);
					int num = (dataSet.Fields != null) ? dataSet.Fields.Count : 0;
					while (num > 0 && dataSet.Fields[num - 1].IsCalculatedField)
					{
						num--;
					}
					publishingDataSetInfo.CalculatedFieldIndex = num;
				}
				if (dataSet.IsReferenceToSharedDataSet)
				{
					if (this.m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.SharedDataSetReferences))
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, dataSet.Name, "SharedDataSet");
					}
					else
					{
						DataSetInfo dataSetInfo = null;
						string text = (this.m_publishingContext.CatalogContext == null) ? dataSet.SharedDataSetQuery.SharedDataSetReference : this.m_publishingContext.CatalogContext.MapUserProvidedPath(dataSet.SharedDataSetQuery.SharedDataSetReference);
						if (this.m_publishingContext.CheckDataSetCallback == null)
						{
							dataSetInfo = new DataSetInfo(dataSet.DataSetCore.Name, text);
						}
						else
						{
							Guid empty = Guid.Empty;
							if (this.m_publishingContext.CheckDataSetCallback(text, out empty))
							{
								dataSetInfo = new DataSetInfo(dataSet.DataSetCore.Name, text, empty);
							}
							else
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsDataSetReferenceNotPublished, Severity.Warning, context.ObjectType, context.ObjectName, null, text.MarkAsPrivate());
								dataSetInfo = new DataSetInfo(dataSet.DataSetCore.Name, text);
							}
						}
						if (this.m_publishingContext.ResolveTemporaryDataSetCallback != null)
						{
							this.m_publishingContext.ResolveTemporaryDataSetCallback(dataSetInfo, this.m_publishingContext.OriginalDataSets);
						}
						dataSet.DataSetCore.SetCatalogID(dataSetInfo.ID);
						this.m_sharedDataSetReferences.Add(dataSetInfo);
					}
				}
			}
		}

		private void InsertField(AspNetCore.ReportingServices.ReportIntermediateFormat.Field field, string aggregateIndicatorFieldName, List<AspNetCore.ReportingServices.ReportIntermediateFormat.Field> fields, List<string> aggregateIndicatorFieldNames, ref int calculatedFieldStartIndex)
		{
			if (field.IsCalculatedField)
			{
				if (calculatedFieldStartIndex < 0)
				{
					calculatedFieldStartIndex = fields.Count;
				}
				fields.Add(field);
				aggregateIndicatorFieldNames.Add(aggregateIndicatorFieldName);
			}
			else if (calculatedFieldStartIndex < 0)
			{
				fields.Add(field);
				aggregateIndicatorFieldNames.Add(aggregateIndicatorFieldName);
			}
			else
			{
				fields.Insert(calculatedFieldStartIndex, field);
				aggregateIndicatorFieldNames.Insert(calculatedFieldStartIndex, aggregateIndicatorFieldName);
				calculatedFieldStartIndex++;
			}
		}

		private void ValidateField(AspNetCore.ReportingServices.ReportIntermediateFormat.Field field, object fieldValue, PublishingContextStruct context, string dataSetName)
		{
			if (null != field.DataField == (null != fieldValue))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidField, Severity.Error, context.ObjectType, field.Name, null, dataSetName.MarkAsPrivate());
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Sorting ReadSortExpressions(bool isDataRowSortExpression, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.Sorting sorting = new AspNetCore.ReportingServices.ReportIntermediateFormat.Sorting(AspNetCore.ReportingServices.ReportIntermediateFormat.ConstructionPhase.Publishing);
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "SortExpression")
					{
						this.ReadSortExpression(sorting, isDataRowSortExpression, context);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("SortExpressions" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (sorting.SortExpressions == null || sorting.SortExpressions.Count == 0)
			{
				sorting = null;
			}
			else
			{
				this.m_hasSorting = true;
				sorting.ValidateNaturalSortFlags(context);
				sorting.ValidateDeferredSortFlags(context);
				if (this.m_publishingContext.IsRestrictedDataRegionSort(isDataRowSortExpression) || this.m_publishingContext.IsRestrictedGroupSort(isDataRowSortExpression, sorting))
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, "SortExpressions");
				}
				else if (sorting.NaturalSort && isDataRowSortExpression)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidNaturalSortContainer, Severity.Error, context.ObjectType, context.ObjectName, "SortExpressions");
				}
				else if (sorting.DeferredSort && isDataRowSortExpression)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidDeferredSortContainer, Severity.Error, context.ObjectType, context.ObjectName, "SortExpressions");
				}
			}
			return sorting;
		}

		private void ReadSortExpression(AspNetCore.ReportingServices.ReportIntermediateFormat.Sorting sorting, bool isDataRowSortExpression, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = null;
			bool item = true;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "Value":
						expressionInfo = this.ReadExpression("SortExpression." + this.m_reader.LocalName, (AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType)(isDataRowSortExpression ? 6 : 5), DataType.String, context);
						break;
					case "Direction":
						item = this.ReadDirection();
						break;
					case "NaturalSort":
						flag = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						break;
					case "DeferredSort":
						flag2 = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("SortExpression" == this.m_reader.LocalName)
					{
						flag3 = true;
					}
					break;
				}
			}
			while (!flag3);
			if (expressionInfo.IsExpression)
			{
				sorting.SortExpressions.Add(expressionInfo);
				sorting.SortDirections.Add(item);
				sorting.NaturalSortFlags.Add(flag);
				sorting.DeferredSortFlags.Add(flag2);
				if (expressionInfo.HasRecursiveAggregates())
				{
					this.m_hasSpecialRecursiveAggregates = true;
				}
			}
			if (flag && this.m_publishingContext.IsRestrictedNaturalGroupSort(expressionInfo))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidNaturalSortGroupExpressionNotSimpleFieldReference, Severity.Error, context.ObjectType, context.ObjectName, "SortExpressions", "NaturalSort");
			}
			if (flag2 && this.m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.DeferredSort))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, "DeferredSort");
			}
			if (flag && flag2)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsConflictingSortFlags, Severity.Error, context.ObjectType, context.ObjectName, "SortExpression");
			}
		}

		private bool ReadDirection()
		{
			string x = this.m_reader.ReadString();
			return 0 == AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.CompareWithInvariantCulture(x, "Ascending", false);
		}

		private void ValidateAndProcessMemberGroupAndSort(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode member, PublishingContextStruct context)
		{
			if (member.IsStatic)
			{
				if (member.Sorting != null)
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsInvalidSortNotAllowed, Severity.Error, context.ObjectType, context.ObjectName, "SortExpressions", "Group", member.RdlElementName);
				}
			}
			else
			{
				this.MergeGroupingAndSortingIfCompatible(member);
				if (member.Sorting != null && member.Sorting.NaturalSort && !member.Grouping.NaturalGroup)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidNaturalSortContainer, Severity.Error, context.ObjectType, context.ObjectName, "SortExpressions");
				}
			}
		}

		private bool ShouldMergeGroupingAndSorting(AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping grouping, AspNetCore.ReportingServices.ReportIntermediateFormat.Sorting sorting)
		{
			if (grouping != null && grouping.Parent == null && sorting != null && grouping.GroupExpressions != null && sorting.SortExpressions != null && sorting.ShouldApplySorting && grouping.GroupExpressions.Count == sorting.SortExpressions.Count)
			{
				for (int i = 0; i < grouping.GroupExpressions.Count; i++)
				{
					if (grouping.GroupExpressions[i].OriginalText != sorting.SortExpressions[i].OriginalText)
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		private void MergeGroupingAndSortingIfCompatible(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode member)
		{
			if (this.ShouldMergeGroupingAndSorting(member.Grouping, member.Sorting))
			{
				member.Grouping.GroupAndSort = true;
				member.Grouping.SortDirections = member.Sorting.SortDirections;
				member.Sorting = null;
			}
			if (member.Sorting != null && member.Sorting.ShouldApplySorting)
			{
				this.m_requiresSortingPostGrouping = true;
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping ReadGrouping(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode scope, PublishingContextStruct context)
		{
			bool flag = false;
			return this.ReadGrouping(scope, context, ref flag);
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping ReadGrouping(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode scope, PublishingContextStruct context, ref bool validName)
		{
			this.m_hasGrouping = true;
			AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping grouping = new AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping(this.GenerateID(), AspNetCore.ReportingServices.ReportIntermediateFormat.ConstructionPhase.Publishing);
			grouping.Name = this.m_reader.GetAttribute("Name");
			if (this.m_scopeNames.Validate(true, grouping.Name, context.ObjectType, context.ObjectName, context.ErrorContext))
			{
				this.m_reportScopes.Add(grouping.Name, grouping);
			}
			else
			{
				validName = false;
			}
			this.m_aggregateHolderList.Add(grouping);
			string dataSetName = null;
			IdcRelationship relationship = null;
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "DocumentMapLabel":
							grouping.GroupLabel = this.ReadDocumentMapLabelExpression(this.m_reader.LocalName, context);
							break;
						case "GroupExpressions":
							this.ReadGroupExpressions(grouping.GroupExpressions, context);
							break;
						case "DataSetName":
							dataSetName = this.m_reader.ReadString();
							break;
						case "Relationship":
							relationship = this.ReadRelationship(context);
							break;
						case "PageBreak":
							this.ReadPageBreak(grouping, context);
							break;
						case "PageName":
							grouping.PageName = this.ReadPageNameExpression(context);
							break;
						case "Filters":
							grouping.Filters = this.ReadFilters(AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.GroupingFilters, context);
							this.m_hasGroupFilters = true;
							break;
						case "Parent":
							grouping.Parent = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo>();
							grouping.Parent.Add(this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.GroupExpression, DataType.String, context));
							if (this.m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.GroupParent))
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Grouping, grouping.Name, "Parent");
							}
							break;
						case "DataElementName":
							grouping.DataElementName = this.m_reader.ReadString();
							break;
						case "DataElementOutput":
							grouping.DataElementOutput = this.ReadDataElementOutput();
							break;
						case "Variables":
							grouping.Variables = this.ReadVariables(context, true, grouping.Name);
							break;
						case "DomainScope":
							grouping.DomainScope = this.m_reader.ReadString();
							if (this.m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.DomainScope))
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Grouping, grouping.Name, "DomainScope");
							}
							break;
						case "NaturalGroup":
							grouping.NaturalGroup = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("Group" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			scope.DataScopeInfo.SetRelationship(dataSetName, relationship);
			if (grouping.Parent != null && 1 != grouping.GroupExpressions.Count)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidGroupingParent, Severity.Error, context.ObjectType, context.ObjectName, "Parent");
			}
			if (grouping.NaturalGroup)
			{
				if (grouping.Parent != null)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidGroupingNaturalGroupFeature, Severity.Warning, context.ObjectType, context.ObjectName, "NaturalGroup", "Parent");
					grouping.NaturalGroup = false;
				}
				if (grouping.DomainScope != null)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidGroupingNaturalGroupFeature, Severity.Warning, context.ObjectType, context.ObjectName, "NaturalGroup", "DomainScope");
					grouping.NaturalGroup = false;
				}
				foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo groupExpression in grouping.GroupExpressions)
				{
					if (this.m_publishingContext.IsRestrictedNaturalGroupSort(groupExpression))
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsInvalidNaturalSortGroupExpressionNotSimpleFieldReference, Severity.Error, context.ObjectType, context.ObjectName, "GroupExpression", "NaturalGroup");
					}
				}
			}
			if (grouping.DomainScope != null)
			{
				if (grouping.Parent != null)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidGroupingDomainScopeWithParent, Severity.Error, context.ObjectType, context.ObjectName, "DomainScope", grouping.Name.MarkAsModelInfo(), grouping.DomainScope.MarkAsPrivate());
				}
				else if (grouping.GroupExpressions.Count == 0)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidGroupingDomainScopeWithDetailGroup, Severity.Error, context.ObjectType, context.ObjectName, "DomainScope", grouping.Name.MarkAsModelInfo(), grouping.DomainScope.MarkAsPrivate());
				}
				else
				{
					this.m_domainScopeGroups.Add(grouping);
				}
			}
			return grouping;
		}

		private void ReadGroupExpressions(List<AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo> groupExpressions, PublishingContextStruct context)
		{
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "GroupExpression")
					{
						groupExpressions.Add(this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.GroupExpression, DataType.String, context));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("GroupExpressions" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private List<Variable> ReadVariables(PublishingContextStruct context, bool isGrouping, string groupName)
		{
			List<Variable> list = new List<Variable>();
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "Variable")
					{
						list.Add(this.ReadVariable(context, isGrouping, groupName));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("Variables" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (list.Count == 0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidVariableCount, Severity.Error, context.ObjectType, context.ObjectName, "Variables");
			}
			else if (this.m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.Variables))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, "Variables");
			}
			return list;
		}

		private Variable ReadVariable(PublishingContextStruct context, bool isGrouping, string groupingName)
		{
			Variable variable = new Variable();
			variable.SequenceID = this.GenerateVariableSequenceID();
			variable.Name = this.m_reader.GetAttribute("Name");
			this.m_variableNames.Validate(variable.Name, context.ObjectType, context.ObjectName, context.ErrorContext, isGrouping, groupingName);
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "Value":
						variable.DataType = this.ReadDataTypeAttribute();
						variable.Value = this.ReadExpression(variable.GetPropertyName(), (AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType)(isGrouping ? 13 : 11), variable.DataType, context);
						break;
					case "Writable":
						if ((context.Location & (LocationFlags.InDataRegion | LocationFlags.InGrouping)) != 0)
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidWritableVariable, Severity.Error, context.ObjectType, context.ObjectName, variable.Name, "Variable");
						}
						else
						{
							variable.Writable = this.m_reader.ReadBoolean(context.ObjectType, variable.Name, "Writable");
							if (variable.Writable)
							{
								this.m_userReferenceLocation |= UserLocationFlags.ReportBody;
							}
						}
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Variable" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (variable.Value == null)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsMissingExpression, Severity.Error, context.ObjectType, context.ObjectName, "Variable");
			}
			return variable;
		}

		private List<IdcRelationship> ReadRelationships(PublishingContextStruct context)
		{
			List<IdcRelationship> list = new List<IdcRelationship>();
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "Relationship")
					{
						list.Add(this.ReadRelationship(context));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("Relationships" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (list.Count == 0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsElementMustContainChildren, Severity.Error, context.ObjectType, context.ObjectName, "Relationships", "Relationship");
			}
			return list;
		}

		private IdcRelationship ReadRelationship(PublishingContextStruct context)
		{
			IdcRelationship idcRelationship = new IdcRelationship();
			bool flag = false;
			bool flag2 = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "ParentScope":
						idcRelationship.ParentScope = this.m_reader.ReadString();
						break;
					case "NaturalJoin":
						idcRelationship.NaturalJoin = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						break;
					case "JoinConditions":
						this.ReadJoinConditions(context, (Relationship)idcRelationship, out flag2);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Relationship" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (!idcRelationship.NaturalJoin && flag2)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidSortDirectionMustNotBeSpecified, Severity.Error, context.ObjectType, context.ObjectName, "JoinCondition", "SortDirection", "ParentScope", idcRelationship.ParentScope);
			}
			return idcRelationship;
		}

		private List<DefaultRelationship> ReadDefaultRelationships(PublishingContextStruct context)
		{
			List<DefaultRelationship> list = new List<DefaultRelationship>();
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "DefaultRelationship")
					{
						list.Add(this.ReadDefaultRelationship(context));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("DefaultRelationships" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (list.Count == 0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsElementMustContainChildren, Severity.Error, context.ObjectType, context.ObjectName, "DefaultRelationships", "DefaultRelationship");
			}
			return list;
		}

		private DefaultRelationship ReadDefaultRelationship(PublishingContextStruct context)
		{
			DefaultRelationship defaultRelationship = new DefaultRelationship();
			bool flag = false;
			bool flag2 = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "RelatedDataSet":
						defaultRelationship.RelatedDataSetName = this.m_reader.ReadString();
						break;
					case "NaturalJoin":
						defaultRelationship.NaturalJoin = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						break;
					case "JoinConditions":
						this.ReadJoinConditions(context, (Relationship)defaultRelationship, out flag);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("DefaultRelationship" == this.m_reader.LocalName)
					{
						flag2 = true;
					}
					break;
				}
			}
			while (!flag2);
			if (!defaultRelationship.NaturalJoin && flag)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidSortDirectionMustNotBeSpecified, Severity.Error, context.ObjectType, context.ObjectName, "JoinCondition", "SortDirection", "RelatedDataSet", defaultRelationship.RelatedDataSetName.MarkAsPrivate());
			}
			return defaultRelationship;
		}

		private void ReadJoinConditions(PublishingContextStruct context, Relationship relationship, out bool sortDirectionSpecified)
		{
			bool flag = false;
			sortDirectionSpecified = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "JoinCondition")
					{
						this.ReadJoinCondition(context, relationship, ref sortDirectionSpecified);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("JoinConditions" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (relationship.JoinConditionCount == 0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsElementMustContainChildren, Severity.Error, context.ObjectType, context.ObjectName, "JoinConditions", "JoinCondition");
			}
		}

		private void ReadJoinCondition(PublishingContextStruct context, Relationship relationship, ref bool sortDirectionSpecified)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = null;
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo2 = null;
			SortDirection direction = SortDirection.Ascending;
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "ForeignKey":
						expressionInfo = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.JoinExpression, DataType.String, context);
						break;
					case "PrimaryKey":
						expressionInfo2 = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.JoinExpression, DataType.String, context);
						break;
					case "SortDirection":
						sortDirectionSpecified = true;
						direction = this.ReadSortDirection();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("JoinCondition" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (expressionInfo == null)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsElementMustContainChild, Severity.Error, context.ObjectType, context.ObjectName, "JoinCondition", "ForeignKey");
			}
			if (expressionInfo2 == null)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsElementMustContainChild, Severity.Error, context.ObjectType, context.ObjectName, "JoinCondition", "PrimaryKey");
			}
			if (expressionInfo != null && expressionInfo2 != null)
			{
				relationship.AddJoinCondition(expressionInfo, expressionInfo2, direction);
			}
		}

		private SortDirection ReadSortDirection()
		{
			return (SortDirection)Enum.Parse(typeof(SortDirection), this.m_reader.ReadString());
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadExpression(string expression, string propertyName, string dataSetName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.EvaluationMode mode, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType expressionType, DataType constantType, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionContext context2 = context.CreateExpressionContext(expressionType, constantType, propertyName, dataSetName, this.m_publishingContext);
			if (!this.CheckUserProfileDependency())
			{
				return this.m_reportCT.ParseExpression(expression, mode, context2);
			}
			bool flag = default(bool);
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo result = this.m_reportCT.ParseExpression(expression, context2, mode, out flag);
			if (flag)
			{
				this.SetUserProfileDependency();
			}
			return result;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadExpression(string propertyName, string dataSetName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType expressionType, DataType constantType, PublishingContextStruct context, out bool userCollectionReferenced)
		{
			AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionContext context2 = context.CreateExpressionContext(expressionType, constantType, propertyName, dataSetName, this.m_publishingContext);
			AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.EvaluationMode evaluationMode = this.ReadEvaluationModeAttribute();
			return this.m_reportCT.ParseExpression(this.m_reader.ReadString(), context2, evaluationMode, out userCollectionReferenced);
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadExpression(string propertyName, string dataSetName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType expressionType, DataType constantType, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.EvaluationMode mode = this.ReadEvaluationModeAttribute();
			return this.ReadExpression(this.m_reader.ReadString(), propertyName, dataSetName, mode, expressionType, constantType, context);
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadExpression(string propertyName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType expressionType, DataType constantType, PublishingContextStruct context)
		{
			return this.ReadExpression(propertyName, null, expressionType, constantType, context);
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadExpression(string propertyName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType expressionType, DataType constantType, PublishingContextStruct context, out bool computed)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = this.ReadExpression(propertyName, expressionType, constantType, context);
			if (AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == expressionInfo.Type)
			{
				computed = false;
			}
			else
			{
				computed = true;
			}
			return expressionInfo;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadDocumentMapLabelExpression(string propertyName, PublishingContextStruct context)
		{
			bool flag = false;
			return this.ReadDocumentMapLabelExpression(propertyName, context, out flag);
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadDocumentMapLabelExpression(string propertyName, PublishingContextStruct context, out bool computed)
		{
			AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType expressionType = AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General;
			DataType constantType = DataType.String;
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = null;
			computed = false;
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				if (context.ObjectType != AspNetCore.ReportingServices.ReportProcessing.ObjectType.Tablix && context.ObjectType != AspNetCore.ReportingServices.ReportProcessing.ObjectType.Subreport && context.ObjectType != AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart && context.ObjectType != AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel && context.ObjectType != AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidReportItemInPageSection, Severity.Warning, context.ObjectType, context.ObjectName, "DocumentMapLabel");
				}
			}
			else
			{
				expressionInfo = this.ReadExpression(propertyName, expressionType, constantType, context, out computed);
				if (expressionInfo != null && (expressionInfo.Type != AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant || !string.IsNullOrEmpty(expressionInfo.StringValue)))
				{
					this.m_hasLabels = true;
				}
			}
			return expressionInfo;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadBookmarkExpression(string propertyName, PublishingContextStruct context)
		{
			bool flag = false;
			return this.ReadBookmarkExpression(propertyName, context, out flag);
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadBookmarkExpression(string propertyName, PublishingContextStruct context, out bool computed)
		{
			AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType expressionType = AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General;
			DataType constantType = DataType.String;
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = this.ReadExpression(propertyName, expressionType, constantType, context, out computed);
			if (expressionInfo != null && (expressionInfo.Type != AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant || expressionInfo.StringValue != null))
			{
				this.m_hasBookmarks = true;
			}
			return expressionInfo;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadExpression(string expression, string propertyName, string dataSetName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType expressionType, DataType constantType, PublishingContextStruct context, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.EvaluationMode evaluationMode, out bool reportParameterReferenced, out string reportParameterName)
		{
			bool flag = default(bool);
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo result = this.ReadExpression(expression, propertyName, dataSetName, expressionType, constantType, context, evaluationMode, out reportParameterReferenced, out reportParameterName, out flag);
			if (flag)
			{
				this.SetUserProfileDependency();
			}
			return result;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadExpression(string expression, string propertyName, string dataSetName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType expressionType, DataType constantType, PublishingContextStruct context, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.EvaluationMode evaluationMode, out bool reportParameterReferenced, out string reportParameterName, out bool userCollectionReferenced)
		{
			AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionContext context2 = context.CreateExpressionContext(expressionType, constantType, propertyName, dataSetName, this.m_publishingContext);
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = this.m_reportCT.ParseExpression(expression, context2, evaluationMode, out userCollectionReferenced);
			if (expressionInfo != null && expressionInfo.IsExpression)
			{
				reportParameterReferenced = true;
				reportParameterName = expressionInfo.SimpleParameterName;
			}
			else
			{
				reportParameterName = null;
				reportParameterReferenced = false;
			}
			return expressionInfo;
		}

		private bool CheckUserProfileDependency()
		{
			bool result = false;
			if (this.m_reportLocationFlags == UserLocationFlags.ReportBody)
			{
				if ((this.m_userReferenceLocation & UserLocationFlags.ReportBody) == (UserLocationFlags)0)
				{
					result = true;
				}
			}
			else if (this.m_reportLocationFlags == UserLocationFlags.ReportPageSection)
			{
				if ((this.m_userReferenceLocation & UserLocationFlags.ReportPageSection) == (UserLocationFlags)0)
				{
					result = true;
				}
			}
			else if (this.m_reportLocationFlags == UserLocationFlags.ReportQueries && (this.m_userReferenceLocation & UserLocationFlags.ReportQueries) == (UserLocationFlags)0)
			{
				result = true;
			}
			return result;
		}

		private void SetUserProfileDependency()
		{
			if (this.m_reportLocationFlags == UserLocationFlags.ReportBody)
			{
				this.m_userReferenceLocation |= UserLocationFlags.ReportBody;
			}
			else if (this.m_reportLocationFlags == UserLocationFlags.ReportPageSection)
			{
				this.m_userReferenceLocation |= UserLocationFlags.ReportPageSection;
			}
			else if (this.m_reportLocationFlags == UserLocationFlags.ReportQueries)
			{
				this.m_userReferenceLocation |= UserLocationFlags.ReportQueries;
			}
		}

		private void SetConnectionStringUserProfileDependency()
		{
			this.m_userReferenceLocation |= UserLocationFlags.ReportQueries;
		}

		private AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.EvaluationMode ReadEvaluationModeAttribute()
		{
			if (this.m_reader.HasAttributes)
			{
				string attribute = this.m_reader.GetAttribute("EvaluationMode");
				if (attribute != null)
				{
					return (AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.EvaluationMode)Enum.Parse(typeof(AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.EvaluationMode), attribute, false);
				}
			}
			return AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.EvaluationMode.Auto;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadToggleImage(PublishingContextStruct context, out bool computed)
		{
			computed = false;
			this.m_static = true;
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo result = null;
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "InitialState")
					{
						result = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context, out computed);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("ToggleImage" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return result;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Image ReadImage(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem parent, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.Image image = new AspNetCore.ReportingServices.ReportIntermediateFormat.Image(this.GenerateID(), parent);
			image.Name = this.m_reader.GetAttribute("Name");
			context.ObjectType = image.ObjectType;
			context.ObjectName = image.Name;
			bool flag = true;
			if (!this.m_reportItemNames.Validate(context.ObjectType, context.ObjectName, context.ErrorContext))
			{
				flag = false;
			}
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = false;
			bool flag8 = false;
			bool flag9 = false;
			bool flag10 = false;
			bool flag11 = false;
			bool flag12 = false;
			AspNetCore.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes? embeddingMode = null;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "Style":
					{
						StyleInformation styleInformation = this.ReadStyle(context, out flag2);
						styleInformation.Filter(StyleOwnerType.Image, false);
						image.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, context.ErrorContext);
						break;
					}
					case "ActionInfo":
						image.Action = this.ReadActionInfo(context, StyleOwnerType.Image, out flag3);
						break;
					case "Top":
						image.Top = this.ReadSize();
						break;
					case "Left":
						image.Left = this.ReadSize();
						break;
					case "Height":
						image.Height = this.ReadSize();
						break;
					case "Width":
						image.Width = this.ReadSize();
						break;
					case "ZIndex":
						image.ZIndex = this.m_reader.ReadInteger(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						break;
					case "Visibility":
						image.Visibility = this.ReadVisibility(context, out flag4);
						break;
					case "ToolTip":
						image.ToolTip = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out flag7);
						break;
					case "DocumentMapLabel":
						image.DocumentMapLabel = this.ReadDocumentMapLabelExpression(this.m_reader.LocalName, context, out flag5);
						break;
					case "Bookmark":
						image.Bookmark = this.ReadBookmarkExpression(context, out flag6);
						break;
					case "RepeatWith":
						image.RepeatedSibling = true;
						image.RepeatWith = this.m_reader.ReadString();
						break;
					case "CustomProperties":
						image.CustomProperties = this.ReadCustomProperties(context, out flag10);
						break;
					case "Source":
						image.Source = this.ReadSource();
						break;
					case "Value":
						image.Value = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out flag8);
						break;
					case "MIMEType":
						image.MIMEType = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out flag9);
						break;
					case "Sizing":
						image.Sizing = this.ReadSizing();
						break;
					case "Tag":
						if (this.m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.ImageTag))
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, "Tag");
						}
						else
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo item = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out flag11);
							if (image.Tags == null)
							{
								image.Tags = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo>(1)
								{
									item
								};
							}
						}
						break;
					case "Tags":
					{
						List<AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo> tags = this.ReadImageTagsCollection(context, ref flag11);
						if (this.m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.ImageTagsCollection))
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, "Tags");
						}
						else
						{
							image.Tags = tags;
						}
						break;
					}
					case "EmbeddingMode":
						embeddingMode = this.ReadEmbeddingMode(context);
						image.EmbeddingMode = embeddingMode.Value;
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Image" == this.m_reader.LocalName)
					{
						flag12 = true;
					}
					break;
				}
			}
			while (!flag12);
			this.ValidateImageEmbeddingMode(context, image.Source, embeddingMode);
			if (AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType.Database == image.Source)
			{
				if (image.Tags == null || !this.m_publishingContext.IsRdlx)
				{
					Global.Tracer.Assert(null != image.Value);
					if (AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == image.Value.Type)
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsBinaryConstant, Severity.Error, context.ObjectType, context.ObjectName, "Value");
					}
				}
				if (!PublishingValidator.ValidateMimeType(image.MIMEType, context.ObjectType, context.ObjectName, "MIMEType", context.ErrorContext))
				{
					image.MIMEType = null;
				}
			}
			else
			{
				if (image.Source == AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType.External && AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == image.Value.Type && image.Value.StringValue != null && image.Value.StringValue.Trim().Length == 0)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidEmptyImageReference, Severity.Error, context.ObjectType, context.ObjectName, "Value");
				}
				image.MIMEType = null;
			}
			image.Computed = (flag2 || flag3 || flag4 || flag10 || flag5 || flag6 || flag7 || flag8 || flag9 || flag11);
			this.m_hasImageStreams = true;
			if (image.Source == AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType.External)
			{
				this.m_hasExternalImages = true;
			}
			if (!flag)
			{
				return null;
			}
			return image;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo> ReadImageTagsCollection(PublishingContextStruct context, ref bool computedTag)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo>();
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "Tag")
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo item = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computedTag);
							list.Add(item);
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (this.m_reader.LocalName == "Tags")
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return list;
		}

		private AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType ReadSource()
		{
			string value = this.m_reader.ReadString();
			return (AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType)Enum.Parse(typeof(AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType), value, false);
		}

		private void ReadBackgroundImage(StyleInformation styleInfo, PublishingContextStruct context, out bool computed)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = false;
			string rdlNamespace = null;
			AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType? nullable = null;
			AspNetCore.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes? embeddingMode = null;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "Source":
					{
						nullable = this.ReadSource();
						AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression2 = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression((int)nullable.Value);
						styleInfo.AddAttribute("BackgroundImageSource", expression2);
						AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType? nullable2 = nullable;
						if (nullable2.GetValueOrDefault() == AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType.External && nullable2.HasValue)
						{
							this.m_hasExternalImages = true;
						}
						break;
					}
					case "Value":
						styleInfo.AddAttribute("BackgroundImageValue", this.ReadExpression("BackgroundImageValue", AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out flag));
						break;
					case "MIMEType":
						styleInfo.AddAttribute("BackgroundImageMIMEType", this.ReadExpression("BackgroundImageMIMEType", AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out flag2));
						break;
					case "BackgroundRepeat":
						if (this.ReadMultiNamespaceStyleAttribute(styleInfo, context, RdlFeatures.BackgroundImageFitting, false, ref rdlNamespace, out flag3))
						{
							StyleInformation.StyleInformationAttribute attributeByName = styleInfo.GetAttributeByName("BackgroundRepeat");
							if (!attributeByName.Value.IsExpression)
							{
								string stringValue = attributeByName.Value.StringValue;
								if (!Validator.ValidateBackgroundRepeatForNamespace(stringValue, rdlNamespace))
								{
									context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlPropertyValue, Severity.Error, context.ObjectType, context.ObjectName, "BackgroundRepeat", stringValue);
								}
							}
						}
						break;
					case "Transparency":
						if (this.m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.BackgroundImageTransparency))
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, "Transparency");
						}
						else
						{
							styleInfo.AddAttribute("Transparency", this.ReadExpression("Transparency", AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Float, context, out flag7));
						}
						break;
					case "TransparentColor":
						styleInfo.AddAttribute("TransparentColor", this.ReadExpression("TransparentColor", AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out flag5));
						break;
					case "Position":
						styleInfo.AddAttribute("Position", this.ReadExpression("Position", AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out flag6));
						break;
					case "EmbeddingMode":
					{
						embeddingMode = this.ReadEmbeddingMode(context);
						AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression((int)embeddingMode.Value);
						styleInfo.AddAttribute("EmbeddingMode", expression);
						break;
					}
					}
					break;
				case XmlNodeType.EndElement:
					if ("BackgroundImage" == this.m_reader.LocalName)
					{
						flag4 = true;
					}
					break;
				}
			}
			while (!flag4);
			this.ValidateImageEmbeddingMode(context, nullable, embeddingMode);
			computed = (flag || flag2 || flag3 || flag7 || flag5 || flag6);
			this.m_hasImageStreams = true;
		}

		private void ValidateImageEmbeddingMode(PublishingContextStruct context, AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType? source, AspNetCore.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes? embeddingMode)
		{
			if (source == AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType.Embedded)
			{
				AspNetCore.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes embeddingModes = embeddingMode ?? AspNetCore.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes.Inline;
				if (this.m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.Image_Embedded))
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlPropertyValue, Severity.Error, context.ObjectType, context.ObjectName, "Source", source.ToString());
				}
				if (embeddingModes == AspNetCore.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes.Inline && this.m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.EmbeddingMode_Inline))
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlPropertyValue, Severity.Error, context.ObjectType, context.ObjectName, "EmbeddingMode", embeddingModes.ToString());
				}
			}
			if (embeddingMode.HasValue && AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType.Embedded != source)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidEmbeddingModeImageProperty, Severity.Error, context.ObjectType, context.ObjectName, "EmbeddingMode");
			}
		}

		private Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo> ReadEmbeddedImages(PublishingContextStruct context)
		{
			Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo> dictionary = new Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo>();
			CLSUniqueNameValidator embeddedImageNames = new CLSUniqueNameValidator(ProcessingErrorCode.rsInvalidEmbeddedImageNameNotCLSCompliant, ProcessingErrorCode.rsDuplicateEmbeddedImageName, ProcessingErrorCode.rsInvalidEmbeddedImageNameLength);
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "EmbeddedImage")
					{
						this.ReadEmbeddedImage(dictionary, embeddedImageNames, context);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("EmbeddedImages" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return dictionary;
		}

		private void ReadEmbeddedImage(Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo> embeddedImages, CLSUniqueNameValidator embeddedImageNames, PublishingContextStruct context)
		{
			string attribute = this.m_reader.GetAttribute("Name");
			context.ObjectType = AspNetCore.ReportingServices.ReportProcessing.ObjectType.EmbeddedImage;
			context.ObjectName = attribute;
			embeddedImageNames.Validate(context.ObjectType, context.ObjectName, context.ErrorContext);
			bool flag = false;
			byte[] array = null;
			string text = null;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "MIMEType":
						text = this.m_reader.ReadString();
						if (!PublishingValidator.ValidateMimeType(text, context.ObjectType, context.ObjectName, this.m_reader.LocalName, context.ErrorContext))
						{
							text = null;
						}
						break;
					case "ImageData":
					{
						string s = this.m_reader.ReadString();
						try
						{
							array = Convert.FromBase64String(s);
						}
						catch
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidEmbeddedImage, Severity.Error, context.ObjectType, context.ObjectName, "ImageData");
						}
						break;
					}
					}
					break;
				case XmlNodeType.EndElement:
					if ("EmbeddedImage" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			string text2 = Guid.NewGuid().ToString();
			embeddedImages[attribute] = new AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo(text2, text);
			if (array != null && text != null && this.m_publishingContext.CreateChunkFactory != null)
			{
				using (Stream stream = this.m_publishingContext.CreateChunkFactory.CreateChunk(text2, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.StaticImage, text))
				{
					stream.Write(array, 0, array.Length);
				}
			}
		}

		private AspNetCore.ReportingServices.OnDemandReportRendering.Image.Sizings ReadSizing()
		{
			string value = this.m_reader.ReadString();
			return (AspNetCore.ReportingServices.OnDemandReportRendering.Image.Sizings)Enum.Parse(typeof(AspNetCore.ReportingServices.OnDemandReportRendering.Image.Sizings), value, false);
		}

		private AspNetCore.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes ReadEmbeddingMode(PublishingContextStruct context)
		{
			if (this.m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.EmbeddingMode))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, "EmbeddingMode");
			}
			string value = this.m_reader.ReadString();
			return (AspNetCore.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes)Enum.Parse(typeof(AspNetCore.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes), value, false);
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadQueryOrParameterExpression(PublishingContextStruct context, DataType dataType, ref bool isComplex, Dictionary<string, bool> parametersInQuery)
		{
			return this.ReadQueryOrParameterExpression(this.m_reader.ReadString(), this.m_reader.LocalName, this.ReadEvaluationModeAttribute(), context, dataType, ref isComplex, parametersInQuery);
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadQueryOrParameterExpression(string expression, string propertyName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.EvaluationMode evaluationMode, PublishingContextStruct context, DataType dataType, ref bool isComplex, Dictionary<string, bool> parametersInQuery)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = null;
			Global.Tracer.Assert(AspNetCore.ReportingServices.ReportProcessing.ObjectType.QueryParameter == context.ObjectType || AspNetCore.ReportingServices.ReportProcessing.ObjectType.Query == context.ObjectType || AspNetCore.ReportingServices.ReportProcessing.ObjectType.DataSource == context.ObjectType);
			this.m_reportLocationFlags = UserLocationFlags.ReportQueries;
			bool flag = default(bool);
			string text = default(string);
			expressionInfo = this.ReadExpression(expression, propertyName, context.ObjectName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.QueryParameter, dataType, context, evaluationMode, out flag, out text);
			if ((this.m_parametersNotUsedInQuery || !isComplex) && flag)
			{
				if (text == null)
				{
					this.m_parametersNotUsedInQuery = false;
					isComplex = true;
				}
				else
				{
					if (!this.m_usedInQueryInfos.Contains(text))
					{
						this.m_usedInQueryInfos.Add(text, true);
					}
					if (!isComplex)
					{
						parametersInQuery[text] = true;
					}
				}
			}
			this.m_reportLocationFlags = UserLocationFlags.ReportBody;
			return expressionInfo;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef> ReadReportParameters(PublishingContextStruct context)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef>();
			CLSUniqueNameValidator reportParameterNames = new CLSUniqueNameValidator(ProcessingErrorCode.rsInvalidNameNotCLSCompliant, ProcessingErrorCode.rsDuplicateReportParameterName, ProcessingErrorCode.rsInvalidParameterNameLength, ProcessingErrorCode.rsDuplicateCaseInsensitiveReportParameterName);
			bool flag = false;
			int num = 0;
			Hashtable parameterNames = new Hashtable();
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "ReportParameter")
					{
						list.Add(this.ReadReportParameter(reportParameterNames, parameterNames, context, num));
						num++;
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("ReportParameters" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef ReadReportParameter(CLSUniqueNameValidator reportParameterNames, Hashtable parameterNames, PublishingContextStruct context, int count)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef parameterDef = new AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef(this.GenerateID());
			parameterDef.Name = this.m_reader.GetAttribute("Name");
			context.ObjectType = AspNetCore.ReportingServices.ReportProcessing.ObjectType.ReportParameter;
			context.ObjectName = parameterDef.Name;
			reportParameterNames.Validate(context.ObjectType, context.ObjectName, context.ErrorContext);
			string type = null;
			string nullable = null;
			bool flag = false;
			string allowBlank = null;
			bool flag2 = false;
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo prompt = null;
			List<string> list = null;
			string multiValue = null;
			string usedInQuery = null;
			bool flag3 = false;
			bool flag4 = false;
			DataSetReference dataSetReference = null;
			DataSetReference dataSetReference2 = null;
			bool flag5 = false;
			bool flag6 = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "DataType":
						type = this.m_reader.ReadString();
						break;
					case "Nullable":
						nullable = this.m_reader.ReadString();
						break;
					case "DefaultValue":
						flag = true;
						list = this.ReadDefaultValue(context, parameterDef, parameterNames, ref flag3, out dataSetReference2);
						break;
					case "AllowBlank":
						allowBlank = this.m_reader.ReadString();
						break;
					case "Prompt":
						flag2 = true;
						prompt = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(this.m_reader.ReadString());
						break;
					case "ValidValues":
						flag4 = this.ReadValidValues(context, parameterDef, parameterNames, ref flag3, out dataSetReference);
						break;
					case "Hidden":
						flag5 = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						break;
					case "MultiValue":
						multiValue = this.m_reader.ReadString();
						break;
					case "UsedInQuery":
						usedInQuery = this.m_reader.ReadString();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("ReportParameter" == this.m_reader.LocalName)
					{
						flag6 = true;
					}
					break;
				}
			}
			while (!flag6);
			if (flag3 && parameterNames.Count > 0)
			{
				parameterDef.Dependencies = (Hashtable)parameterNames.Clone();
			}
			parameterDef.Parse(parameterDef.Name, list, type, nullable, prompt, null, allowBlank, multiValue, usedInQuery, flag5, context.ErrorContext, CultureInfo.InvariantCulture);
			if (parameterDef.Nullable && !flag)
			{
				parameterDef.DefaultValues = new object[1];
				parameterDef.DefaultValues[0] = null;
			}
			if (parameterDef.DataType == DataType.Boolean)
			{
				dataSetReference = null;
			}
			if (!flag2 && !flag && !flag5 && (!parameterDef.Nullable || (parameterDef.ValidValuesValueExpressions != null && !flag4)))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsMissingParameterDefault, Severity.Error, context.ObjectType, context.ObjectName, null);
			}
			if (parameterDef.Nullable && parameterDef.MultiValue)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidMultiValueParameter, Severity.Error, context.ObjectType, context.ObjectName, null);
			}
			if (!parameterDef.MultiValue && list != null && list.Count > 1)
			{
				list.RemoveRange(1, list.Count - 1);
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidDefaultValueValues, Severity.Warning, context.ObjectType, context.ObjectName, null);
			}
			if (dataSetReference2 != null || dataSetReference != null)
			{
				this.m_dynamicParameters.Add(new DynamicParameter(dataSetReference, dataSetReference2, count, flag3));
			}
			if (!parameterNames.ContainsKey(parameterDef.Name))
			{
				parameterNames.Add(parameterDef.Name, count);
			}
			return parameterDef;
		}

		private List<string> ReadDefaultValue(PublishingContextStruct context, AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef parameter, Hashtable parameterNames, ref bool isComplex, out DataSetReference defaultDataSet)
		{
			bool flag = false;
			bool flag2 = false;
			List<string> result = null;
			defaultDataSet = null;
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag3 = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "DataSetReference":
							flag = true;
							defaultDataSet = this.ReadDataSetReference();
							break;
						case "Values":
							flag2 = true;
							result = this.ReadValues(context, parameter, parameterNames, ref isComplex);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("DefaultValue" == this.m_reader.LocalName)
						{
							flag3 = true;
						}
						break;
					}
				}
				while (!flag3);
			}
			if (flag == flag2)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidDefaultValue, Severity.Error, context.ObjectType, context.ObjectName, "DefaultValue");
			}
			return result;
		}

		private List<string> ReadValues(PublishingContextStruct context, AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef parameter, Hashtable parameterNames, ref bool isComplex)
		{
			List<string> list = null;
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo> list2 = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo>();
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = null;
			bool flag = false;
			bool flag2 = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "Value")
					{
						string attribute = this.m_reader.GetAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance");
						expressionInfo = ((attribute == null || !XmlConvert.ToBoolean(attribute)) ? this.ReadParameterExpression(this.m_reader.LocalName, context, parameter, parameterNames, ref flag, ref isComplex) : AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(null));
						list2.Add(expressionInfo);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("Values" == this.m_reader.LocalName)
					{
						flag2 = true;
					}
					break;
				}
			}
			while (!flag2);
			if (flag)
			{
				parameter.DefaultExpressions = list2;
			}
			else
			{
				list = new List<string>(list2.Count);
				for (int i = 0; i < list2.Count; i++)
				{
					list.Add(list2[i].StringValue);
				}
			}
			return list;
		}

		private bool ReadValidValues(PublishingContextStruct context, AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef parameter, Hashtable parameterNames, ref bool isComplex, out DataSetReference validValueDataSet)
		{
			bool flag = false;
			bool flag2 = false;
			bool result = false;
			validValueDataSet = null;
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag3 = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "DataSetReference":
							flag = true;
							validValueDataSet = this.ReadDataSetReference();
							break;
						case "ParameterValues":
							flag2 = true;
							this.ReadParameterValues(context, parameter, parameterNames, ref isComplex, ref result);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ValidValues" == this.m_reader.LocalName)
						{
							flag3 = true;
						}
						break;
					}
				}
				while (!flag3);
			}
			if (flag == flag2)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidValidValues, Severity.Error, context.ObjectType, context.ObjectName, "ValidValues");
			}
			return result;
		}

		private void ReadParameterValues(PublishingContextStruct context, AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef parameter, Hashtable parameterNames, ref bool isComplex, ref bool containsExplicitNull)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo>();
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo> list2 = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo>();
			bool flag = isComplex;
			bool flag2 = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "ParameterValue")
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = null;
						AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo item = null;
						if (!this.m_reader.IsEmptyElement)
						{
							bool flag3 = false;
							do
							{
								this.m_reader.Read();
								switch (this.m_reader.NodeType)
								{
								case XmlNodeType.Element:
									switch (this.m_reader.LocalName)
									{
									case "Value":
										expressionInfo = this.ReadParameterExpression(this.m_reader.LocalName, context, parameter, parameterNames, ref flag, ref isComplex);
										break;
									case "Label":
										item = this.ReadParameterExpression(this.m_reader.LocalName, context, parameter, parameterNames, ref flag, ref isComplex);
										break;
									}
									break;
								case XmlNodeType.EndElement:
									if ("ParameterValue" == this.m_reader.LocalName)
									{
										flag3 = true;
									}
									break;
								}
							}
							while (!flag3);
						}
						containsExplicitNull |= (null == expressionInfo);
						list.Add(expressionInfo);
						list2.Add(item);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("ParameterValues" == this.m_reader.LocalName)
					{
						flag2 = true;
					}
					break;
				}
			}
			while (!flag2);
			parameter.ValidValuesValueExpressions = list;
			parameter.ValidValuesLabelExpressions = list2;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadParameterExpression(string propertyName, PublishingContextStruct context, AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef parameter, Hashtable parameterNames, ref bool dynamic, ref bool isComplex)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = null;
			string text = null;
			bool flag = false;
			bool flag2 = default(bool);
			if (isComplex)
			{
				dynamic = true;
				expressionInfo = this.ReadExpression(propertyName, (string)null, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.ReportParameter, DataType.String, context, out flag2);
			}
			else
			{
				bool flag3 = default(bool);
				expressionInfo = this.ReadExpression(this.m_reader.ReadString(), propertyName, (string)null, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.ReportParameter, DataType.String, context, this.ReadEvaluationModeAttribute(), out flag3, out text, out flag2);
				if (flag3)
				{
					dynamic = true;
					if (text == null)
					{
						isComplex = true;
					}
					else if (!parameterNames.ContainsKey(text))
					{
						flag = true;
					}
					else
					{
						if (parameter.Dependencies == null)
						{
							parameter.Dependencies = new Hashtable();
						}
						if (!parameter.Dependencies.ContainsKey(text))
						{
							parameter.Dependencies.Add(text, parameterNames[text]);
						}
					}
				}
			}
			if (flag2)
			{
				if (parameter.Name != null && !this.m_reportParamUserProfile.Contains(parameter.Name))
				{
					this.m_reportParamUserProfile.Add(parameter.Name, true);
				}
				this.m_userReferenceLocation |= UserLocationFlags.ReportBody;
			}
			if (flag)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidReportParameterDependency, Severity.Error, AspNetCore.ReportingServices.ReportProcessing.ObjectType.ReportParameter, parameter.Name, "ValidValues", text.MarkAsPrivate());
			}
			return expressionInfo;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterValue> ReadParameters(PublishingContextStruct context, bool doClsValidation)
		{
			bool flag = default(bool);
			return this.ReadParameters(context, false, doClsValidation, true, out flag);
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterValue> ReadParameters(PublishingContextStruct context, bool omitAllowed, bool doClsValidation, bool isSubreportParameter, out bool computed)
		{
			computed = false;
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterValue> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterValue>();
			ParameterNameValidator parameterNames = new ParameterNameValidator();
			string propertyNamePrefix = isSubreportParameter ? "SubreportParameters" : "DrillthroughParameters";
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "Parameter")
					{
						bool flag2 = default(bool);
						list.Add(this.ReadParameter(parameterNames, context, propertyNamePrefix, omitAllowed, doClsValidation, isSubreportParameter, out flag2));
						computed = (computed || flag2);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("Parameters" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterValue ReadParameter(ParameterNameValidator parameterNames, PublishingContextStruct context, string propertyNamePrefix, bool omitAllowed, bool doClsValidation, bool isSubreportParameter, out bool computed)
		{
			AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType expressionType = (AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType)(isSubreportParameter ? 12 : 0);
			computed = false;
			bool flag = false;
			bool flag2 = false;
			AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterValue parameterValue = new AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterValue();
			parameterValue.Name = this.m_reader.GetAttribute("Name");
			if (doClsValidation)
			{
				parameterNames.Validate(parameterValue.Name, context.ObjectType, context.ObjectName, context.ErrorContext);
			}
			parameterValue.Value = null;
			parameterValue.Omit = null;
			bool flag3 = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "Value":
						parameterValue.Value = this.ReadExpression(propertyNamePrefix + "." + this.m_reader.LocalName, expressionType, DataType.String, context, out flag);
						break;
					case "Omit":
						if (omitAllowed)
						{
							parameterValue.Omit = this.ReadExpression(propertyNamePrefix + "." + this.m_reader.LocalName, expressionType, DataType.Boolean, context, out flag2);
						}
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Parameter" == this.m_reader.LocalName)
					{
						flag3 = true;
					}
					break;
				}
			}
			while (!flag3);
			computed = (flag || flag2);
			return parameterValue;
		}

		private ParametersGridLayout ReadReportParametersLayout(PublishingContextStruct context, List<AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef> parameters)
		{
			ParametersGridLayout result = null;
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "GridLayoutDefinition")
					{
						result = this.ReadGridLayoutDefinition(context);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("ReportParametersLayout" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return result;
		}

		private ParametersGridLayout ReadGridLayoutDefinition(PublishingContextStruct context)
		{
			ParametersGridLayout parametersGridLayout = new ParametersGridLayout();
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "NumberOfColumns":
						parametersGridLayout.NumberOfColumns = this.m_reader.ReadInteger(AspNetCore.ReportingServices.ReportProcessing.ObjectType.ParameterLayout, "GridLayoutDefinition", "NumberOfColumns");
						break;
					case "NumberOfRows":
						parametersGridLayout.NumberOfRows = this.m_reader.ReadInteger(AspNetCore.ReportingServices.ReportProcessing.ObjectType.ParameterLayout, "GridLayoutDefinition", "NumberOfRows");
						break;
					case "CellDefinitions":
						parametersGridLayout.CellDefinitions = this.ReadCellDefinitions(context);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("GridLayoutDefinition" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return parametersGridLayout;
		}

		private ParametersGridCellDefinitionList ReadCellDefinitions(PublishingContextStruct context)
		{
			ParametersGridCellDefinitionList parametersGridCellDefinitionList = new ParametersGridCellDefinitionList();
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "CellDefinition")
					{
						parametersGridCellDefinitionList.Add(this.ReadCellDefinition(context));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("CellDefinitions" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return parametersGridCellDefinitionList;
		}

		private ParameterGridLayoutCellDefinition ReadCellDefinition(PublishingContextStruct context)
		{
			ParameterGridLayoutCellDefinition parameterGridLayoutCellDefinition = new ParameterGridLayoutCellDefinition();
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "RowIndex":
						parameterGridLayoutCellDefinition.RowIndex = this.m_reader.ReadInteger(AspNetCore.ReportingServices.ReportProcessing.ObjectType.ParameterLayout, "CellDefinition", "RowIndex");
						break;
					case "ColumnIndex":
						parameterGridLayoutCellDefinition.ColumnIndex = this.m_reader.ReadInteger(AspNetCore.ReportingServices.ReportProcessing.ObjectType.ParameterLayout, "CellDefinition", "ColumnIndex");
						break;
					case "ParameterName":
						parameterGridLayoutCellDefinition.ParameterName = this.m_reader.ReadString();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("CellDefinition" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return parameterGridLayoutCellDefinition;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox ReadTextbox(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem parent, PublishingContextStruct context, List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox textBox = new AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox(this.GenerateID(), parent);
			textBox.Name = this.m_reader.GetAttribute("Name");
			textBox.SequenceID = this.GenerateTextboxSequenceID();
			context.ObjectType = textBox.ObjectType;
			context.ObjectName = textBox.Name;
			bool flag = true;
			if (!this.m_reportItemNames.Validate(context.ObjectType, context.ObjectName, context.ErrorContext))
			{
				flag = false;
			}
			Global.Tracer.Assert(!this.m_reportCT.ValueReferenced);
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = false;
			bool flag8 = false;
			bool flag9 = false;
			bool flag10 = false;
			bool flag11 = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "Style":
					{
						if (this.m_reportCT.ValueReferenced)
						{
							textBox.ValueReferenced = true;
							this.m_reportCT.ResetValueReferencedFlag();
						}
						StyleInformation styleInformation = this.ReadStyle(context, out flag2);
						styleInformation.Filter(StyleOwnerType.TextBox, false);
						bool flag12 = default(bool);
						textBox.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, textBox.ObjectType, textBox.Name, (ErrorContext)context.ErrorContext, this.m_reportCT.ValueReferenced && !textBox.ValueReferenced, out flag12);
						if (flag12)
						{
							textBox.ValueReferenced = true;
						}
						this.m_reportCT.ResetValueReferencedFlag();
						break;
					}
					case "ActionInfo":
						textBox.Action = this.ReadActionInfo(context, StyleOwnerType.TextBox, out flag3);
						break;
					case "Top":
						textBox.Top = this.ReadSize();
						break;
					case "Left":
						textBox.Left = this.ReadSize();
						break;
					case "Height":
						textBox.Height = this.ReadSize();
						break;
					case "Width":
						textBox.Width = this.ReadSize();
						break;
					case "ZIndex":
						textBox.ZIndex = this.m_reader.ReadInteger(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						break;
					case "Visibility":
						textBox.Visibility = this.ReadVisibility(context, out flag4);
						break;
					case "ToolTip":
						textBox.ToolTip = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out flag7);
						break;
					case "DocumentMapLabel":
						textBox.DocumentMapLabel = this.ReadDocumentMapLabelExpression(this.m_reader.LocalName, context, out flag5);
						break;
					case "Bookmark":
						textBox.Bookmark = this.ReadBookmarkExpression(context, out flag6);
						break;
					case "RepeatWith":
						textBox.RepeatedSibling = true;
						textBox.RepeatWith = this.m_reader.ReadString();
						break;
					case "CustomProperties":
						textBox.CustomProperties = this.ReadCustomProperties(context, out flag10);
						break;
					case "Paragraphs":
						if (this.m_reportCT.ValueReferenced)
						{
							textBox.ValueReferenced = true;
							this.m_reportCT.ResetValueReferencedFlag();
						}
						textBox.Paragraphs = this.ReadParagraphs(context, textBox, out flag8);
						break;
					case "CanScrollVertically":
						textBox.CanScrollVertically = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						break;
					case "CanGrow":
						textBox.CanGrow = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						break;
					case "CanShrink":
						textBox.CanShrink = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						break;
					case "HideDuplicates":
					{
						string text = this.m_reader.ReadString();
						if ((context.Location & LocationFlags.InPageSection) != 0 || text == null || text.Length <= 0)
						{
							textBox.HideDuplicates = null;
						}
						else
						{
							textBox.HideDuplicates = text;
						}
						break;
					}
					case "ToggleImage":
						textBox.InitialToggleState = this.ReadToggleImage(context, out flag9);
						break;
					case "UserSort":
						this.ReadUserSort(context, textBox, textBoxesWithDefaultSortTarget);
						this.m_hasUserSort = true;
						break;
					case "DataElementName":
						textBox.DataElementName = this.m_reader.ReadString();
						break;
					case "DataElementOutput":
						textBox.DataElementOutput = this.ReadDataElementOutput();
						break;
					case "DataElementStyle":
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem.DataElementStyles dataElementStyles = this.ReadDataElementStyleRDL();
						if (AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem.DataElementStyles.Auto != dataElementStyles)
						{
							textBox.OverrideReportDataElementStyle = true;
							textBox.DataElementStyleAttribute = (AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem.DataElementStyles.Attribute == dataElementStyles);
						}
						break;
					}
					case "KeepTogether":
						textBox.KeepTogether = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Textbox" == this.m_reader.LocalName)
					{
						flag11 = true;
					}
					break;
				}
			}
			while (!flag11);
			textBox.Computed = (flag2 || flag3 || flag4 || flag10 || flag5 || flag6 || flag7 || flag8 || flag9 || textBox.UserSort != null || textBox.HideDuplicates != null);
			textBox.ValueReferenced |= this.m_reportCT.ValueReferenced;
			this.m_reportCT.ResetValueReferencedFlag();
			if (!flag)
			{
				return null;
			}
			return textBox;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.Paragraph> ReadParagraphs(PublishingContextStruct context, AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox textbox, out bool computed)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.Paragraph> paragraphs = textbox.Paragraphs;
			computed = false;
			int num = 0;
			bool flag = false;
			if (!this.m_reader.IsEmptyElement)
			{
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "Paragraph")
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.Paragraph paragraph = this.ReadParagraph(context, textbox, num, ref computed);
							paragraphs.Add(paragraph);
							if (paragraph.TextRunValueReferenced)
							{
								textbox.TextRunValueReferenced = true;
							}
							num++;
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("Paragraphs" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			if (paragraphs.Count > 0)
			{
				return paragraphs;
			}
			return null;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Paragraph ReadParagraph(PublishingContextStruct context, AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox textbox, int index, ref bool computed)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.Paragraph paragraph = new AspNetCore.ReportingServices.ReportIntermediateFormat.Paragraph(textbox, index, this.GenerateID());
			context.ObjectType = paragraph.ObjectType;
			context.ObjectName = paragraph.Name;
			bool flag = false;
			bool flag2 = false;
			if (!this.m_reader.IsEmptyElement)
			{
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "TextRuns":
							paragraph.TextRuns = this.ReadTextRuns(context, paragraph, index, ref computed);
							break;
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context, out flag2);
							computed |= flag2;
							styleInformation.Filter(StyleOwnerType.Paragraph, false);
							paragraph.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, paragraph.ObjectType, paragraph.Name, context.ErrorContext);
							break;
						}
						case "LeftIndent":
							paragraph.LeftIndent = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out flag2);
							computed |= flag2;
							if (!flag2)
							{
								PublishingValidator.ValidateSize(paragraph.LeftIndent.StringValue, paragraph.ObjectType, paragraph.Name, "LeftIndent", context.ErrorContext);
							}
							break;
						case "RightIndent":
							paragraph.RightIndent = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out flag2);
							computed |= flag2;
							if (!flag2)
							{
								PublishingValidator.ValidateSize(paragraph.RightIndent.StringValue, paragraph.ObjectType, paragraph.Name, "RightIndent", context.ErrorContext);
							}
							break;
						case "HangingIndent":
							paragraph.HangingIndent = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out flag2);
							computed |= flag2;
							if (!flag2)
							{
								double num = default(double);
								string text = default(string);
								PublishingValidator.ValidateSize(paragraph.HangingIndent.StringValue, paragraph.ObjectType, paragraph.Name, "HangingIndent", false, true, (ErrorContext)context.ErrorContext, out num, out text);
							}
							break;
						case "SpaceBefore":
							paragraph.SpaceBefore = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out flag2);
							computed |= flag2;
							if (!flag2)
							{
								PublishingValidator.ValidateSize(paragraph.SpaceBefore.StringValue, paragraph.ObjectType, paragraph.Name, "SpaceBefore", context.ErrorContext);
							}
							break;
						case "SpaceAfter":
							paragraph.SpaceAfter = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out flag2);
							computed |= flag2;
							if (!flag2)
							{
								PublishingValidator.ValidateSize(paragraph.SpaceAfter.StringValue, paragraph.ObjectType, paragraph.Name, "SpaceAfter", context.ErrorContext);
							}
							break;
						case "ListLevel":
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo2 = paragraph.ListLevel = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context, out flag2);
							computed |= flag2;
							int? nullable = default(int?);
							if (expressionInfo2 != null && !expressionInfo2.IsExpression && !Validator.ValidateParagraphListLevel(expressionInfo2.IntValue, out nullable))
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsOutOfRangeSize, Severity.Error, paragraph.ObjectType, paragraph.Name, "ListLevel", expressionInfo2.OriginalText.MarkAsPrivate(), Convert.ToString(0, CultureInfo.InvariantCulture), Convert.ToString(9, CultureInfo.InvariantCulture));
							}
							break;
						}
						case "ListStyle":
							paragraph.ListStyle = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out flag2);
							computed |= flag2;
							if (!flag2)
							{
								PublishingValidator.ValidateParagraphListStyle(paragraph.ListStyle.StringValue, paragraph.ObjectType, paragraph.Name, "ListStyle", context.ErrorContext);
							}
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("Paragraph" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return paragraph;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextRun> ReadTextRuns(PublishingContextStruct context, AspNetCore.ReportingServices.ReportIntermediateFormat.Paragraph paragraph, int paragraphIndex, ref bool computed)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextRun> textRuns = paragraph.TextRuns;
			int num = 0;
			bool flag = false;
			if (!this.m_reader.IsEmptyElement)
			{
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "TextRun")
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.TextRun textRun = this.ReadTextRun(context, paragraph, paragraphIndex, num, ref computed);
							textRuns.Add(textRun);
							if (textRun.ValueReferenced)
							{
								paragraph.TextRunValueReferenced = true;
							}
							num++;
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("TextRuns" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			if (textRuns.Count > 0)
			{
				return textRuns;
			}
			return null;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.TextRun ReadTextRun(PublishingContextStruct context, AspNetCore.ReportingServices.ReportIntermediateFormat.Paragraph paragraph, int paragraphIndex, int index, ref bool computed)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.TextRun textRun = new AspNetCore.ReportingServices.ReportIntermediateFormat.TextRun(paragraph, index, this.GenerateID());
			context.ObjectType = textRun.ObjectType;
			context.ObjectName = textRun.Name;
			bool flag = false;
			bool flag2 = false;
			if (!this.m_reader.IsEmptyElement)
			{
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "ActionInfo":
							textRun.Action = this.ReadActionInfo(context, StyleOwnerType.TextRun, out flag2);
							computed |= flag2;
							break;
						case "Style":
						{
							if (this.m_reportCT.ValueReferenced)
							{
								textRun.ValueReferenced = true;
								this.m_reportCT.ResetValueReferencedFlag();
							}
							StyleInformation styleInformation = this.ReadStyle(context, out flag2);
							computed |= flag2;
							styleInformation.Filter(StyleOwnerType.TextRun, false);
							bool flag3 = default(bool);
							textRun.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, textRun.ObjectType, textRun.Name, (ErrorContext)context.ErrorContext, this.m_reportCT.ValueReferenced && !textRun.ValueReferenced, out flag3);
							if (flag3)
							{
								textRun.ValueReferenced = true;
							}
							this.m_reportCT.ResetValueReferencedFlag();
							break;
						}
						case "Value":
							textRun.DataType = this.ReadDataTypeAttribute();
							textRun.Value = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, textRun.DataType, context, out flag2);
							computed |= flag2;
							break;
						case "Label":
							textRun.Label = this.m_reader.ReadString();
							break;
						case "ToolTip":
							textRun.ToolTip = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out flag2);
							computed |= flag2;
							break;
						case "MarkupType":
							textRun.MarkupType = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out flag2);
							computed |= flag2;
							if (!flag2)
							{
								PublishingValidator.ValidateTextRunMarkupType(textRun.MarkupType.StringValue, textRun.ObjectType, textRun.Name, "MarkupType", context.ErrorContext);
							}
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("TextRun" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			if (this.m_reportCT.ValueReferenced)
			{
				textRun.ValueReferenced = true;
				this.m_reportCT.ResetValueReferencedFlag();
			}
			return textRun;
		}

		private void ReadUserSort(PublishingContextStruct context, AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox textbox, List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
		{
			bool flag = (LocationFlags)0 != (context.Location & LocationFlags.InPageSection);
			bool flag2 = false;
			AspNetCore.ReportingServices.ReportIntermediateFormat.EndUserSort endUserSort = new AspNetCore.ReportingServices.ReportIntermediateFormat.EndUserSort();
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "SortExpression":
					{
						bool flag3 = default(bool);
						endUserSort.SortExpression = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.UserSortExpression, DataType.String, context, out flag3);
						break;
					}
					case "SortExpressionScope":
						endUserSort.SortExpressionScopeString = this.m_reader.ReadString();
						break;
					case "SortTarget":
						this.m_hasUserSortPeerScopes = true;
						endUserSort.SortTargetString = this.m_reader.ReadString();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("UserSort" == this.m_reader.LocalName)
					{
						flag2 = true;
					}
					break;
				}
			}
			while (!flag2);
			if (this.m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.UserSort))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Textbox, textbox.Name, "UserSort");
			}
			else if (flag)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidTextboxInPageSection, Severity.Error, textbox.ObjectType, textbox.Name, "UserSort");
			}
			else
			{
				textbox.UserSort = endUserSort;
				if (endUserSort.SortTargetString == null)
				{
					if (textBoxesWithDefaultSortTarget != null)
					{
						textBoxesWithDefaultSortTarget.Add(textbox);
					}
				}
				else
				{
					this.m_textBoxesWithUserSortTarget.Add(textbox);
				}
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Rectangle ReadRectangle(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem parent, PublishingContextStruct context, List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.Rectangle rectangle = new AspNetCore.ReportingServices.ReportIntermediateFormat.Rectangle(this.GenerateID(), this.GenerateID(), parent);
			rectangle.Name = this.m_reader.GetAttribute("Name");
			context.ObjectType = rectangle.ObjectType;
			context.ObjectName = rectangle.Name;
			bool flag = true;
			if (!this.m_reportItemNames.Validate(context.ObjectType, context.ObjectName, context.ErrorContext))
			{
				flag = false;
			}
			this.m_reportItemCollectionList.Add(rectangle.ReportItems);
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = false;
			bool flag8 = false;
			bool flag9 = false;
			string text = null;
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = null;
			if (!this.m_reader.IsEmptyElement)
			{
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context, out flag2);
							styleInformation.Filter(StyleOwnerType.Rectangle, false);
							rectangle.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, context.ErrorContext);
							break;
						}
						case "Top":
							rectangle.Top = this.ReadSize();
							break;
						case "Left":
							rectangle.Left = this.ReadSize();
							break;
						case "Height":
							rectangle.Height = this.ReadSize();
							break;
						case "Width":
							rectangle.Width = this.ReadSize();
							break;
						case "ZIndex":
							rectangle.ZIndex = this.m_reader.ReadInteger(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
							break;
						case "Visibility":
							rectangle.Visibility = this.ReadVisibility(context, out flag3);
							break;
						case "ToolTip":
							rectangle.ToolTip = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out flag6);
							break;
						case "DocumentMapLabel":
							expressionInfo = (rectangle.DocumentMapLabel = this.ReadDocumentMapLabelExpression(this.m_reader.LocalName, context, out flag4));
							break;
						case "LinkToChild":
							text = this.m_reader.ReadString();
							break;
						case "Bookmark":
							rectangle.Bookmark = this.ReadBookmarkExpression(context, out flag5);
							break;
						case "RepeatWith":
							rectangle.RepeatedSibling = true;
							rectangle.RepeatWith = this.m_reader.ReadString();
							break;
						case "CustomProperties":
							rectangle.CustomProperties = this.ReadCustomProperties(context, out flag8);
							break;
						case "ReportItems":
							this.ReadReportItems((string)null, (AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem)rectangle, rectangle.ReportItems, context, textBoxesWithDefaultSortTarget, out flag7);
							break;
						case "PageBreak":
							this.ReadPageBreak(rectangle, context);
							break;
						case "PageName":
						{
							bool flag10 = default(bool);
							rectangle.PageName = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out flag10);
							break;
						}
						case "KeepTogether":
							rectangle.KeepTogether = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
							break;
						case "DataElementName":
							rectangle.DataElementName = this.m_reader.ReadString();
							break;
						case "DataElementOutput":
							rectangle.DataElementOutput = this.ReadDataElementOutput();
							break;
						case "OmitBorderOnPageBreak":
							rectangle.OmitBorderOnPageBreak = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("Rectangle" == this.m_reader.LocalName)
						{
							flag9 = true;
						}
						break;
					}
				}
				while (!flag9);
			}
			rectangle.Computed = (flag2 || flag3 || flag4 || flag5 || flag7 || flag6 || flag8 || (rectangle.PageBreak != null && rectangle.PageBreak.BreakLocation != PageBreakLocation.None));
			if (expressionInfo != null && text != null)
			{
				rectangle.ReportItems.LinkToChild = text;
			}
			if (!flag)
			{
				return null;
			}
			return rectangle;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Line ReadLine(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem parent, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.Line line = new AspNetCore.ReportingServices.ReportIntermediateFormat.Line(this.GenerateID(), parent);
			line.Name = this.m_reader.GetAttribute("Name");
			context.ObjectType = line.ObjectType;
			context.ObjectName = line.Name;
			bool flag = true;
			if (!this.m_reportItemNames.Validate(context.ObjectType, context.ObjectName, context.ErrorContext))
			{
				flag = false;
			}
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = false;
			if (!this.m_reader.IsEmptyElement)
			{
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context, out flag2);
							styleInformation.Filter(StyleOwnerType.Line, false);
							line.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, context.ErrorContext);
							break;
						}
						case "Top":
							line.Top = this.ReadSize();
							break;
						case "Left":
							line.Left = this.ReadSize();
							break;
						case "Height":
							line.Height = this.ReadSize();
							break;
						case "Width":
							line.Width = this.ReadSize();
							break;
						case "ZIndex":
							line.ZIndex = this.m_reader.ReadInteger(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
							break;
						case "Visibility":
							line.Visibility = this.ReadVisibility(context, out flag3);
							break;
						case "DocumentMapLabel":
							line.DocumentMapLabel = this.ReadDocumentMapLabelExpression(this.m_reader.LocalName, context, out flag4);
							break;
						case "Bookmark":
							line.Bookmark = this.ReadBookmarkExpression(context, out flag5);
							break;
						case "RepeatWith":
							line.RepeatedSibling = true;
							line.RepeatWith = this.m_reader.ReadString();
							break;
						case "CustomProperties":
							line.CustomProperties = this.ReadCustomProperties(context, out flag6);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("Line" == this.m_reader.LocalName)
						{
							flag7 = true;
						}
						break;
					}
				}
				while (!flag7);
			}
			line.Computed = (flag2 || flag3 || flag4 || flag5 || flag6);
			if (!flag)
			{
				return null;
			}
			return line;
		}

		private StyleInformation ReadStyle(PublishingContextStruct context, out bool computed)
		{
			computed = false;
			StyleInformation styleInformation = new StyleInformation();
			if (!this.m_reader.IsEmptyElement)
			{
				string text = null;
				string text2 = null;
				string text3 = null;
				string text4 = null;
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						bool flag2 = false;
						switch (this.m_reader.LocalName)
						{
						case "Border":
							this.ReadBorderAttributes("", styleInformation, context, out flag2);
							break;
						case "TopBorder":
							this.ReadBorderAttributes("Top", styleInformation, context, out flag2);
							break;
						case "BottomBorder":
							this.ReadBorderAttributes("Bottom", styleInformation, context, out flag2);
							break;
						case "LeftBorder":
							this.ReadBorderAttributes("Left", styleInformation, context, out flag2);
							break;
						case "RightBorder":
							this.ReadBorderAttributes("Right", styleInformation, context, out flag2);
							break;
						case "BackgroundImage":
							this.ReadBackgroundImage(styleInformation, context, out flag2);
							break;
						case "FontFamily":
							this.ReadMultiNamespaceStyleAttribute(styleInformation, context, RdlFeatures.ThemeFonts, true, ref text, out flag2);
							break;
						case "BackgroundColor":
							this.ReadMultiNamespaceStyleAttribute(styleInformation, context, RdlFeatures.ThemeColors, true, ref text3, out flag2);
							break;
						case "Color":
							this.ReadMultiNamespaceStyleAttribute(styleInformation, context, RdlFeatures.ThemeColors, true, ref text2, out flag2);
							break;
						case "CurrencyLanguage":
							this.ReadMultiNamespaceStyleAttribute(styleInformation, context, RdlFeatures.CellLevelFormatting, true, ref text4, out flag2);
							break;
						case "Format":
						case "Language":
						case "Calendar":
						case "NumeralLanguage":
						case "BackgroundGradientType":
						case "BackgroundGradientEndColor":
						case "FontStyle":
						case "FontSize":
						case "FontWeight":
						case "TextDecoration":
						case "TextAlign":
						case "VerticalAlign":
						case "PaddingLeft":
						case "PaddingRight":
						case "PaddingTop":
						case "PaddingBottom":
						case "LineHeight":
						case "Direction":
						case "WritingMode":
						case "UnicodeBiDi":
						case "TextEffect":
						case "ShadowColor":
						case "ShadowOffset":
						case "BackgroundHatchType":
							styleInformation.AddAttribute(this.m_reader.LocalName, this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out flag2));
							break;
						case "NumeralVariant":
							styleInformation.AddAttribute(this.m_reader.LocalName, this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Integer, context, out flag2));
							break;
						}
						computed = (computed || flag2);
						break;
					}
					case XmlNodeType.EndElement:
						if ("Style" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return styleInformation;
		}

		private bool ReadMultiNamespaceStyleAttribute(StyleInformation styleInfo, PublishingContextStruct context, RdlFeatures feature, bool readValueType, ref string attributeNamespace, out bool computedAttribute)
		{
			computedAttribute = false;
			string localName = this.m_reader.LocalName;
			if (attributeNamespace != null && RdlNamespaceComparer.Instance.Compare(this.m_reader.NamespaceURI, attributeNamespace) <= 0)
			{
				return false;
			}
			AspNetCore.ReportingServices.ReportIntermediateFormat.ValueType valueType = AspNetCore.ReportingServices.ReportIntermediateFormat.ValueType.Constant;
			if (this.m_reader.NamespaceURI == "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition" || this.m_reader.NamespaceURI == "http://schemas.microsoft.com/sqlserver/reporting/2013/01/reportdefinition")
			{
				if (this.m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(feature))
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, context.ObjectName, localName);
				}
				if (readValueType)
				{
					string attribute = this.m_reader.GetAttribute("ValueType", this.m_reader.NamespaceURI);
					if (!string.IsNullOrEmpty(attribute))
					{
						valueType = (AspNetCore.ReportingServices.ReportIntermediateFormat.ValueType)Enum.Parse(typeof(AspNetCore.ReportingServices.ReportIntermediateFormat.ValueType), attribute);
					}
				}
			}
			if (attributeNamespace != null)
			{
				styleInfo.RemoveAttribute(localName);
			}
			attributeNamespace = this.m_reader.NamespaceURI;
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = this.ReadExpression(localName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out computedAttribute);
			if (feature == RdlFeatures.ThemeFonts && string.IsNullOrEmpty(expressionInfo.StringValue))
			{
				expressionInfo.StringValue = "Arial";
			}
			styleInfo.AddAttribute(localName, expressionInfo, valueType);
			return true;
		}

		private StyleInformation ReadStyle(PublishingContextStruct context)
		{
			bool flag = default(bool);
			return this.ReadStyle(context, out flag);
		}

		private void ReadBorderAttributes(string borderLocation, StyleInformation styleInfo, PublishingContextStruct context, out bool computed)
		{
			computed = false;
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						bool flag2 = false;
						switch (this.m_reader.LocalName)
						{
						case "Color":
							styleInfo.AddAttribute("BorderColor" + borderLocation, this.ReadExpression("BorderColor", AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out flag2));
							break;
						case "Style":
							styleInfo.AddAttribute("BorderStyle" + borderLocation, this.ReadExpression("BorderStyle", AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out flag2));
							break;
						case "Width":
							styleInfo.AddAttribute("BorderWidth" + borderLocation, this.ReadExpression("BorderWidth", AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context, out flag2));
							break;
						}
						computed = (computed || flag2);
						break;
					}
					case XmlNodeType.EndElement:
						if (borderLocation + "Border" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
		}

		private string ReadSize()
		{
			return this.m_reader.ReadString();
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo ReadSizeExpression(PublishingContextStruct context)
		{
			return this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet.TriState ReadTriState()
		{
			string value = this.m_reader.ReadString();
			return (AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet.TriState)Enum.Parse(typeof(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet.TriState), value, false);
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Visibility ReadVisibility(PublishingContextStruct context, out bool computed)
		{
			this.m_static = true;
			AspNetCore.ReportingServices.ReportIntermediateFormat.Visibility visibility = new AspNetCore.ReportingServices.ReportIntermediateFormat.Visibility();
			bool flag = false;
			bool flag2 = false;
			context.PrefixPropertyName = "Visibility.";
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag3 = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Hidden":
							visibility.Hidden = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.Boolean, context, out flag);
							break;
						case "ToggleItem":
							flag2 = true;
							if ((context.Location & LocationFlags.InPageSection) != 0)
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsToggleInPageSection, Severity.Error, context.ObjectType, context.ObjectName, "ToggleItem");
							}
							this.m_interactive = true;
							visibility.Toggle = this.m_reader.ReadString();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("Visibility" == this.m_reader.LocalName)
						{
							flag3 = true;
						}
						break;
					}
				}
				while (!flag3);
			}
			computed = (flag || flag2);
			return visibility;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Visibility ReadVisibility(PublishingContextStruct context)
		{
			bool flag = default(bool);
			return this.ReadVisibility(context, out flag);
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix ReadTablix(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem parent, PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix tablix = new AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix(this.GenerateID(), parent);
			tablix.Name = this.m_reader.GetAttribute("Name");
			bool isTopLevelDataRegion = false;
			if ((context.Location & LocationFlags.InDataRegion) == (LocationFlags)0)
			{
				isTopLevelDataRegion = true;
				this.m_nestedDataRegions = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion>();
			}
			else
			{
				Global.Tracer.Assert(this.m_nestedDataRegions != null);
				this.m_nestedDataRegions.Add(tablix);
			}
			context.Location |= (LocationFlags.InDataSet | LocationFlags.InDataRegion);
			context.ObjectType = tablix.ObjectType;
			context.ObjectName = tablix.Name;
			this.RegisterDataRegion(tablix);
			bool flag = true;
			if (!this.m_reportItemNames.Validate(context.ObjectType, context.ObjectName, context.ErrorContext))
			{
				flag = false;
			}
			if (this.m_scopeNames.Validate(false, context.ObjectName, context.ObjectType, context.ObjectName, context.ErrorContext))
			{
				this.m_reportScopes.Add(tablix.Name, tablix);
			}
			else
			{
				flag = false;
			}
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsDataRegionInPageSection, Severity.Error, context.ObjectType, context.ObjectName, null);
				flag = false;
			}
			StyleInformation styleInformation = null;
			bool flag2 = false;
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox>();
			IdcRelationship relationship = null;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "SortExpressions":
						tablix.Sorting = this.ReadSortExpressions(true, context);
						break;
					case "Style":
						styleInformation = this.ReadStyle(context);
						break;
					case "Top":
						tablix.Top = this.ReadSize();
						break;
					case "Left":
						tablix.Left = this.ReadSize();
						break;
					case "Height":
						tablix.Height = this.ReadSize();
						break;
					case "Width":
						tablix.Width = this.ReadSize();
						break;
					case "CanScroll":
						tablix.CanScroll = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						break;
					case "ZIndex":
						tablix.ZIndex = this.m_reader.ReadInteger(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						break;
					case "Visibility":
						tablix.Visibility = this.ReadVisibility(context);
						break;
					case "ToolTip":
						tablix.ToolTip = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
						break;
					case "DocumentMapLabel":
						tablix.DocumentMapLabel = this.ReadDocumentMapLabelExpression(this.m_reader.LocalName, context);
						break;
					case "Bookmark":
						tablix.Bookmark = this.ReadBookmarkExpression(this.m_reader.LocalName, context);
						break;
					case "CustomProperties":
						tablix.CustomProperties = this.ReadCustomProperties(context);
						break;
					case "DataElementName":
						tablix.DataElementName = this.m_reader.ReadString();
						break;
					case "DataElementOutput":
						tablix.DataElementOutput = this.ReadDataElementOutput();
						break;
					case "KeepTogether":
						tablix.KeepTogether = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						break;
					case "NoRowsMessage":
						tablix.NoRowsMessage = this.ReadExpression(this.m_reader.LocalName, AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.General, DataType.String, context);
						break;
					case "DataSetName":
						tablix.DataSetName = this.m_reader.ReadString();
						break;
					case "Relationship":
						relationship = this.ReadRelationship(context);
						break;
					case "PageBreak":
						this.ReadPageBreak(tablix, context);
						break;
					case "PageName":
						tablix.PageName = this.ReadPageNameExpression(context);
						break;
					case "Filters":
						tablix.Filters = this.ReadFilters(AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.DataRegionFilters, context);
						break;
					case "TablixCorner":
						this.ReadTablixCorner(tablix, context, list);
						break;
					case "TablixBody":
						this.ReadTablixBody(tablix, context, list);
						break;
					case "TablixColumnHierarchy":
						tablix.TablixColumnMembers = this.ReadTablixHierarchy(tablix, context, list, true, ref flag);
						break;
					case "TablixRowHierarchy":
						tablix.TablixRowMembers = this.ReadTablixHierarchy(tablix, context, list, false, ref flag);
						break;
					case "LayoutDirection":
						tablix.LayoutDirection = this.ReadLayoutDirection();
						break;
					case "GroupsBeforeRowHeaders":
						tablix.GroupsBeforeRowHeaders = this.m_reader.ReadInteger(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						break;
					case "RepeatColumnHeaders":
						tablix.RepeatColumnHeaders = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						break;
					case "RepeatRowHeaders":
						tablix.RepeatRowHeaders = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						break;
					case "FixedColumnHeaders":
						tablix.FixedColumnHeaders = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						break;
					case "FixedRowHeaders":
						tablix.FixedRowHeaders = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						break;
					case "OmitBorderOnPageBreak":
						tablix.OmitBorderOnPageBreak = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						break;
					case "BandLayoutOptions":
						tablix.BandLayout = this.ReadBandLayoutOptions(tablix, context);
						break;
					case "TopMargin":
						tablix.TopMargin = this.ReadSizeExpression(context);
						break;
					case "BottomMargin":
						tablix.BottomMargin = this.ReadSizeExpression(context);
						break;
					case "LeftMargin":
						tablix.LeftMargin = this.ReadSizeExpression(context);
						break;
					case "RightMargin":
						tablix.RightMargin = this.ReadSizeExpression(context);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Tablix" == this.m_reader.LocalName)
					{
						flag2 = true;
					}
					break;
				}
			}
			while (!flag2);
			tablix.InitializationData.IsTopLevelDataRegion = isTopLevelDataRegion;
			if (tablix.BandLayout != null)
			{
				tablix.ValidateBandStructure(context);
			}
			tablix.DataScopeInfo.SetRelationship(tablix.DataSetName, relationship);
			if (tablix.Height == null)
			{
				tablix.ComputeHeight = true;
			}
			if (tablix.Width == null)
			{
				tablix.ComputeWidth = true;
			}
			if (styleInformation != null)
			{
				styleInformation.Filter(StyleOwnerType.Tablix, null != tablix.NoRowsMessage);
				tablix.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Attributes, context.ObjectType, context.ObjectName, context.ErrorContext);
			}
			this.SetSortTargetForTextBoxes(list, tablix);
			tablix.Computed = true;
			if (flag)
			{
				this.m_createSubtotalsDefs.Add(tablix);
				if (tablix.Corner == null && tablix.ColumnHeaderRowCount > 0 && tablix.RowHeaderColumnCount > 0)
				{
					List<List<AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCornerCell>> list2 = new List<List<AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCornerCell>>(1);
					List<AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCornerCell> list3 = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCornerCell>(1);
					AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCornerCell tablixCornerCell = new AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCornerCell(this.GenerateID(), tablix);
					tablixCornerCell.RowSpan = tablix.ColumnHeaderRowCount;
					tablixCornerCell.ColSpan = tablix.RowHeaderColumnCount;
					list3.Add(tablixCornerCell);
					list2.Add(list3);
					tablix.Corner = list2;
				}
			}
			if (!flag)
			{
				return null;
			}
			return tablix;
		}

		private TablixMemberList ReadTablixHierarchy(AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context, List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget, bool isColumnHierarchy, ref bool validName)
		{
			TablixMemberList tablixMemberList = null;
			bool flag = false;
			int num = 0;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "TablixMembers":
					{
						bool flag3 = default(bool);
						tablixMemberList = this.ReadTablixMembers(tablix, (AspNetCore.ReportingServices.ReportIntermediateFormat.TablixMember)null, context, textBoxesWithDefaultSortTarget, isColumnHierarchy, 0, ref num, ref validName, out flag3);
						break;
					}
					case "EnableDrilldown":
					{
						bool flag2 = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						if (flag2 && this.m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.TablixHierarchy_EnableDrilldown))
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFeatureRdlElement, Severity.Error, context.ObjectType, tablix.Name, "EnableDrilldown");
						}
						if (isColumnHierarchy)
						{
							tablix.EnableColumnDrilldown = flag2;
						}
						else
						{
							tablix.EnableRowDrilldown = flag2;
						}
						break;
					}
					}
					break;
				case XmlNodeType.EndElement:
					if (this.m_reader.LocalName == (isColumnHierarchy ? "TablixColumnHierarchy" : "TablixRowHierarchy"))
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			this.ValidateHeaderSizesAndSetSpans(tablix, context, tablixMemberList, isColumnHierarchy);
			if (isColumnHierarchy)
			{
				tablix.ColumnCount = num;
			}
			else
			{
				tablix.RowCount = num;
			}
			return tablixMemberList;
		}

		private void ValidateHeaderSizesAndSetSpans(AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context, TablixMemberList members, bool isColumnHierarchy)
		{
			bool hasError = context.ErrorContext.HasError;
			context.ErrorContext.HasError = false;
			this.m_headerLevelSizeList = new SortedList<double, Pair<double, int>>(Validator.DoubleComparer.Instance);
			this.m_headerLevelSizeList.Add(0.0, new Pair<double, int>(0.0, 0));
			this.m_firstCumulativeHeaderSize = -1.0;
			int num = 0;
			int num2 = default(int);
			this.ValidateHeaderSizes(context, members, isColumnHierarchy, 0.0, 0, ref num, out num2);
			IList<Pair<double, int>> values = this.m_headerLevelSizeList.Values;
			int totalSpanCount = this.GetTotalSpanCount();
			if (isColumnHierarchy)
			{
				tablix.InitializationData.ColumnHeaderLevelSizeList = values;
				tablix.ColumnHeaderRowCount = totalSpanCount;
			}
			else
			{
				tablix.InitializationData.RowHeaderLevelSizeList = values;
				tablix.RowHeaderColumnCount = totalSpanCount;
			}
			if (!context.ErrorContext.HasError)
			{
				bool[] parentHeaderLevelHasStaticArray = new bool[totalSpanCount];
				this.SetHeaderSpans(context, members, isColumnHierarchy, 0.0, false, 0, parentHeaderLevelHasStaticArray);
			}
			context.ErrorContext.HasError |= hasError;
		}

		private int GetTotalSpanCount()
		{
			int num = -1;
			for (int i = 0; i < this.m_headerLevelSizeList.Count; i++)
			{
				num += this.m_headerLevelSizeList.Values[i].Second + 1;
			}
			return num;
		}

		private bool ValidateHeaderSizes(PublishingContextStruct context, TablixMemberList members, bool isColumnHierarchy, double ancestorHeaderSize, int consecutiveZeroHeightAncestorCount, ref int rowOrColumnNumber, out int maxConsecutiveZeroHeightDescendentCount)
		{
			bool? nullable = null;
			bool flag = false;
			maxConsecutiveZeroHeightDescendentCount = 0;
			bool flag2 = false;
			for (int i = 0; i < members.Count; i++)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.TablixMember tablixMember = members[i];
				tablixMember.ConsecutiveZeroHeightAncestorCount = consecutiveZeroHeightAncestorCount;
				flag = false;
				double num = 0.0;
				double num2 = 0.0;
				bool flag3 = false;
				if (tablixMember.TablixHeader != null)
				{
					flag = true;
					num = tablixMember.TablixHeader.SizeValue;
					num2 = ancestorHeaderSize + num;
					if (num == 0.0)
					{
						flag3 = true;
						flag2 = true;
						int second = this.m_headerLevelSizeList[num2].Second;
						this.m_headerLevelSizeList[num2] = new Pair<double, int>(num2, Math.Max(consecutiveZeroHeightAncestorCount + 1, second));
					}
					else if (!this.m_headerLevelSizeList.ContainsKey(num2))
					{
						this.m_headerLevelSizeList.Add(num2, new Pair<double, int>(num2, 0));
					}
				}
				if (tablixMember.SubMembers != null)
				{
					int val = default(int);
					flag |= this.ValidateHeaderSizes(context, tablixMember.SubMembers, isColumnHierarchy, ancestorHeaderSize + num, flag3 ? (consecutiveZeroHeightAncestorCount + 1) : 0, ref rowOrColumnNumber, out val);
					maxConsecutiveZeroHeightDescendentCount = Math.Max(val, maxConsecutiveZeroHeightDescendentCount);
					tablixMember.ConsecutiveZeroHeightDescendentCount = maxConsecutiveZeroHeightDescendentCount;
				}
				else
				{
					rowOrColumnNumber++;
				}
				if (tablixMember.IsInnerMostMemberWithHeader)
				{
					int num3 = 0;
					if (tablixMember.SubMembers != null)
					{
						num3 = ((!isColumnHierarchy) ? (tablixMember.RowSpan - 1) : (tablixMember.ColSpan - 1));
					}
					if (this.m_firstCumulativeHeaderSize == -1.0)
					{
						this.m_firstCumulativeHeaderSize = num2;
					}
					else
					{
						double first = Math.Round(this.m_firstCumulativeHeaderSize, 4);
						double second2 = Math.Round(num2, 4);
						if (Validator.CompareDoubles(first, second2) != 0)
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidTablixHeaderSize, Severity.Error, context.ObjectType, context.ObjectName, isColumnHierarchy ? "TablixColumnHierarchy" : "TablixRowHierarchy", "TablixHeader.Size", (rowOrColumnNumber - num3).ToString(CultureInfo.InvariantCulture.NumberFormat), first.ToString(CultureInfo.InvariantCulture.NumberFormat) + "mm", second2.ToString(CultureInfo.InvariantCulture.NumberFormat) + "mm", isColumnHierarchy ? "TablixColumn" : "TablixRow");
						}
					}
				}
				if (!nullable.HasValue)
				{
					nullable = flag;
				}
				if (flag != nullable.Value)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidTablixHeaders, Severity.Error, context.ObjectType, context.ObjectName, isColumnHierarchy ? "TablixColumnHierarchy" : "TablixRowHierarchy", "TablixMember", "TablixHeader", members[i].Level.ToString(CultureInfo.InvariantCulture.NumberFormat));
				}
			}
			if (flag2)
			{
				maxConsecutiveZeroHeightDescendentCount++;
			}
			else
			{
				maxConsecutiveZeroHeightDescendentCount = 0;
			}
			return flag;
		}

		private void SetHeaderSpans(PublishingContextStruct context, TablixMemberList members, bool isColumnHierarchy, double ancestorHeaderSize, bool outerConsumedInnerZeroHeightLevel, int parentHeaderLevelPlusSpans, bool[] parentHeaderLevelHasStaticArray)
		{
			for (int i = 0; i < members.Count; i++)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.TablixMember tablixMember = members[i];
				double num = 0.0;
				int num2 = 0;
				bool outerConsumedInnerZeroHeightLevel2 = false;
				if (tablixMember.TablixHeader != null)
				{
					num = tablixMember.TablixHeader.SizeValue;
					if (num == 0.0)
					{
						outerConsumedInnerZeroHeightLevel2 = outerConsumedInnerZeroHeightLevel;
						num2 = 1;
					}
					else
					{
						num2 = this.GetSpans(tablixMember, ancestorHeaderSize, num, outerConsumedInnerZeroHeightLevel, out outerConsumedInnerZeroHeightLevel2);
					}
					if (isColumnHierarchy)
					{
						tablixMember.RowSpan = num2;
					}
					else
					{
						tablixMember.ColSpan = num2;
					}
					tablixMember.HeaderLevel = parentHeaderLevelPlusSpans;
					if (parentHeaderLevelHasStaticArray != null)
					{
						parentHeaderLevelHasStaticArray[parentHeaderLevelPlusSpans] |= tablixMember.IsStatic;
					}
				}
				bool[] array = parentHeaderLevelHasStaticArray;
				if (tablixMember.HasConditionalOrToggleableVisibility)
				{
					array = (tablixMember.HeaderLevelHasStaticArray = new bool[(int)parentHeaderLevelHasStaticArray.LongLength]);
				}
				if (tablixMember.SubMembers != null)
				{
					double ancestorHeaderSize2 = ancestorHeaderSize + num;
					this.SetHeaderSpans(context, tablixMember.SubMembers, isColumnHierarchy, ancestorHeaderSize2, outerConsumedInnerZeroHeightLevel2, parentHeaderLevelPlusSpans + num2, array);
				}
				if (parentHeaderLevelHasStaticArray != null && tablixMember.HasConditionalOrToggleableVisibility)
				{
					for (int j = tablixMember.HeaderLevel + 1; j < parentHeaderLevelHasStaticArray.Length; j++)
					{
						parentHeaderLevelHasStaticArray[j] |= array[j];
					}
				}
			}
		}

		private int GetSpans(AspNetCore.ReportingServices.ReportIntermediateFormat.TablixMember member, double ancestorHeaderSize, double headerSize, bool outerConsumedInnerZeroHeightLevel, out bool consumedInnerZeroHeightLevel)
		{
			double second = ancestorHeaderSize + headerSize;
			int num = 0;
			consumedInnerZeroHeightLevel = false;
			int num2 = this.m_headerLevelSizeList.IndexOfKey(ancestorHeaderSize);
			if (!outerConsumedInnerZeroHeightLevel)
			{
				int second2 = this.m_headerLevelSizeList.Values[num2].Second;
				if (second2 > 0)
				{
					num += second2 - member.ConsecutiveZeroHeightAncestorCount;
				}
			}
			for (int i = num2 + 1; i < this.m_headerLevelSizeList.Count; i++)
			{
				double first = this.m_headerLevelSizeList.Values[i].First;
				int second2 = this.m_headerLevelSizeList.Values[i].Second;
				num++;
				if (Validator.CompareDoubles(first, second) == 0)
				{
					if (second2 > 0)
					{
						int num3 = second2 - member.ConsecutiveZeroHeightDescendentCount;
						if (num3 > 0)
						{
							consumedInnerZeroHeightLevel = true;
							num += num3;
						}
					}
					break;
				}
				num += second2;
			}
			return num;
		}

		private TablixMemberList ReadTablixMembers(AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix tablix, AspNetCore.ReportingServices.ReportIntermediateFormat.TablixMember parentMember, PublishingContextStruct context, List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget, bool isColumnHierarchy, int level, ref int leafNodes, ref bool validName, out bool innerMostMemberWithHeaderFound)
		{
			TablixMemberList tablixMemberList = new TablixMemberList();
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			int num = -1;
			bool flag4 = false;
			innerMostMemberWithHeaderFound = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "TablixMember")
					{
						num = leafNodes;
						List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox>();
						AspNetCore.ReportingServices.ReportIntermediateFormat.TablixMember tablixMember = this.ReadTablixMember(tablix, context, list, isColumnHierarchy, level, ref leafNodes, ref flag4, ref validName, out innerMostMemberWithHeaderFound);
						tablixMember.ParentMember = parentMember;
						if (level == 0)
						{
							if (flag3)
							{
								if (tablixMember.FixedData)
								{
									context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFixedDataNotContiguous, Severity.Error, context.ObjectType, context.ObjectName, "FixedData", isColumnHierarchy ? "TablixColumnHierarchy" : "TablixRowHierarchy");
								}
							}
							else if (flag2)
							{
								if (tablixMember.FixedData)
								{
									tablix.InitializationData.FixedColLength += tablixMember.ColSpan;
								}
								else
								{
									flag3 = true;
								}
							}
							else if (tablixMember.FixedData)
							{
								if (isColumnHierarchy)
								{
									tablix.InitializationData.HasFixedColData = true;
									tablix.InitializationData.FixedColStartIndex = num;
									tablix.InitializationData.FixedColLength = tablixMember.ColSpan;
								}
								else
								{
									tablix.InitializationData.HasFixedRowData = true;
								}
								flag2 = true;
							}
						}
						else
						{
							if (tablixMember.FixedData && !parentMember.FixedData)
							{
								context.ErrorContext.Register(ProcessingErrorCode.rsInvalidFixedDataInHierarchy, Severity.Warning, context.ObjectType, context.ObjectName, "FixedData", isColumnHierarchy ? "TablixColumnHierarchy" : "TablixRowHierarchy");
							}
							tablixMember.FixedData = parentMember.FixedData;
						}
						tablixMemberList.Add(tablixMember);
						if (tablixMember.Grouping != null)
						{
							this.SetSortTargetForTextBoxes(list, tablixMember.Grouping);
						}
						else
						{
							textBoxesWithDefaultSortTarget.AddRange(list);
						}
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("TablixMembers" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (flag4)
			{
				foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.TablixMember item in tablixMemberList)
				{
					item.HasStaticPeerWithHeader = true;
				}
			}
			if (tablixMemberList.Count <= 0)
			{
				return null;
			}
			return tablixMemberList;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.TablixMember ReadTablixMember(AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context, List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget, bool isColumnHierarchy, int level, ref int aLeafNodes, ref bool isStaticWithHeader, ref bool validName, out bool innerMostMemberWithHeaderFound)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.TablixMember tablixMember = new AspNetCore.ReportingServices.ReportIntermediateFormat.TablixMember(this.GenerateID(), tablix);
			this.m_runningValueHolderList.Add(tablixMember);
			tablixMember.IsColumn = isColumnHierarchy;
			tablixMember.Level = level;
			innerMostMemberWithHeaderFound = false;
			bool flag = false;
			bool flag2 = false;
			int num = 0;
			if (!this.m_reader.IsEmptyElement)
			{
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Group":
							tablixMember.Grouping = this.ReadGrouping((AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode)tablixMember, context, ref validName);
							break;
						case "SortExpressions":
							tablixMember.Sorting = this.ReadSortExpressions(false, context);
							break;
						case "TablixHeader":
							tablixMember.TablixHeader = this.ReadTablixHeader(tablix, context, textBoxesWithDefaultSortTarget);
							tablixMember.TablixHeader.SizeValue = context.ValidateSize(tablixMember.TablixHeader.Size, isColumnHierarchy ? "Height" : "Width", context.ErrorContext);
							break;
						case "TablixMembers":
							tablixMember.SubMembers = this.ReadTablixMembers(tablix, tablixMember, context, textBoxesWithDefaultSortTarget, isColumnHierarchy, level + 1, ref num, ref validName, out innerMostMemberWithHeaderFound);
							break;
						case "CustomProperties":
							tablixMember.CustomProperties = this.ReadCustomProperties(context);
							break;
						case "FixedData":
							tablixMember.FixedData = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
							break;
						case "Visibility":
							tablixMember.Visibility = this.ReadVisibility(context, out flag2);
							break;
						case "HideIfNoRows":
							tablixMember.HideIfNoRows = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
							break;
						case "KeepWithGroup":
							tablixMember.KeepWithGroup = (KeepWithGroup)Enum.Parse(typeof(KeepWithGroup), this.m_reader.ReadString());
							break;
						case "RepeatOnNewPage":
							tablixMember.RepeatOnNewPage = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
							break;
						case "DataElementName":
							tablixMember.DataElementName = this.m_reader.ReadString();
							break;
						case "DataElementOutput":
							tablixMember.DataElementOutput = this.ReadDataElementOutput();
							break;
						case "KeepTogether":
							tablixMember.KeepTogether = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
							tablixMember.KeepTogetherSpecified = true;
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("TablixMember" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			if (!innerMostMemberWithHeaderFound && tablixMember.TablixHeader != null)
			{
				tablixMember.IsInnerMostMemberWithHeader = true;
				innerMostMemberWithHeaderFound = true;
			}
			if (tablixMember.SubMembers == null || tablixMember.SubMembers.Count == 0)
			{
				aLeafNodes++;
				if (isColumnHierarchy)
				{
					tablixMember.ColSpan = 1;
				}
				else
				{
					tablixMember.RowSpan = 1;
				}
			}
			else
			{
				aLeafNodes += num;
				if (isColumnHierarchy)
				{
					tablixMember.ColSpan = num;
				}
				else
				{
					tablixMember.RowSpan = num;
				}
			}
			this.ValidateAndProcessMemberGroupAndSort(tablixMember, context);
			if (tablixMember.IsStatic && tablixMember.TablixHeader != null)
			{
				isStaticWithHeader = true;
			}
			if (isColumnHierarchy)
			{
				if (tablixMember.KeepWithGroup != 0)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidKeepWithGroupOnColumnTablixMember, Severity.Error, context.ObjectType, context.ObjectName, "TablixMember", "KeepWithGroup", "None");
				}
				if (tablixMember.RepeatOnNewPage)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidRepeatOnNewPageOnColumnTablixMember, Severity.Error, context.ObjectType, context.ObjectName, "TablixMember", "RepeatOnNewPage");
				}
			}
			return tablixMember;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.TablixHeader ReadTablixHeader(AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context, List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.TablixHeader tablixHeader = new AspNetCore.ReportingServices.ReportIntermediateFormat.TablixHeader(this.GenerateID());
			int? nullable = null;
			int? nullable2 = null;
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "Size":
						tablixHeader.Size = this.ReadSize();
						break;
					case "CellContents":
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem altCellContents = default(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem);
						tablixHeader.CellContents = this.ReadCellContents((AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem)tablix, context, textBoxesWithDefaultSortTarget, true, out altCellContents, out nullable, out nullable2);
						tablixHeader.AltCellContents = altCellContents;
						break;
					}
					}
					break;
				case XmlNodeType.EndElement:
					if ("TablixHeader" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return tablixHeader;
		}

		private void ReadTablixCorner(AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context, List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
		{
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "TablixCornerRows")
					{
						tablix.Corner = this.ReadTablixCornerRows(tablix, context, textBoxesWithDefaultSortTarget);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("TablixCorner" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private List<List<AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCornerCell>> ReadTablixCornerRows(AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context, List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
		{
			List<List<AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCornerCell>> list = new List<List<AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCornerCell>>();
			int[] array = null;
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "TablixCornerRow")
					{
						List<AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCornerCell> list2 = this.ReadTablixCornerRow(tablix, context, array, textBoxesWithDefaultSortTarget);
						if (array == null)
						{
							array = new int[list2.Count];
							for (int i = 0; i < array.Length; i++)
							{
								array[i] = 1;
							}
						}
						else if (array.Length != list2.Count)
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInconsistentNumberofCellsInRow, Severity.Error, context.ObjectType, context.ObjectName, "TablixCorner");
							int num = array.Length;
							if (num < list2.Count)
							{
								int j = 0;
								int[] array2 = new int[list2.Count];
								for (; j < num; j++)
								{
									array2[j] = array[j];
								}
								for (; j < array2.Length; j++)
								{
									array2[j] = 1;
								}
								array = array2;
							}
						}
						for (int k = 0; k < list2.Count; k++)
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCornerCell tablixCornerCell = list2[k];
							if (array[k] <= 1)
							{
								array[k] = tablixCornerCell.RowSpan;
								if (tablixCornerCell.RowSpan > 1 && tablixCornerCell.ColSpan > 1)
								{
									for (int l = 1; l < tablixCornerCell.ColSpan; l++)
									{
										k++;
										if (k == array.Length)
										{
											context.ErrorContext.Register(ProcessingErrorCode.rsInvalidTablixCornerCellSpan, Severity.Error, context.ObjectType, context.ObjectName, "ColSpan");
											break;
										}
										array[k] = tablixCornerCell.RowSpan;
									}
								}
							}
							else
							{
								array[k]--;
							}
						}
						list.Add(list2);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("TablixCornerRows" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return list;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCornerCell> ReadTablixCornerRow(AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context, int[] rowSpans, List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCornerCell> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCornerCell>();
			bool flag = false;
			int num = 1;
			int num2 = 0;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "TablixCornerCell")
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCornerCell tablixCornerCell = this.ReadTablixCornerCell(tablix, context, num > 1 || (rowSpans != null && rowSpans.Length > num2 && rowSpans[num2] > 1), textBoxesWithDefaultSortTarget);
						num = ((num > 1) ? (num - 1) : tablixCornerCell.ColSpan);
						list.Add(tablixCornerCell);
						num2++;
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("TablixCornerRow" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCornerCell ReadTablixCornerCell(AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context, bool shouldBeEmpty, List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCornerCell tablixCornerCell = new AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCornerCell(this.GenerateID(), tablix);
			bool flag = false;
			int? nullable = null;
			int? nullable2 = null;
			if (!this.m_reader.IsEmptyElement)
			{
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						string localName;
						if ((localName = this.m_reader.LocalName) != null && localName == "CellContents")
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem altCellContents = default(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem);
							tablixCornerCell.CellContents = this.ReadCellContents((AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem)tablix, context, textBoxesWithDefaultSortTarget, true, out altCellContents, out nullable2, out nullable);
							tablixCornerCell.AltCellContents = altCellContents;
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("TablixCornerCell" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			if (shouldBeEmpty)
			{
				if (tablixCornerCell.CellContents != null)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsCellContentsNotOmitted, Severity.Error, context.ObjectType, context.ObjectName, "TablixCornerCell");
				}
				if (nullable2.HasValue)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidTablixCornerCellSpan, Severity.Error, context.ObjectType, context.ObjectName, "ColSpan");
				}
				if (nullable.HasValue)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidTablixCornerCellSpan, Severity.Error, context.ObjectType, context.ObjectName, "RowSpan");
				}
			}
			else if (tablixCornerCell.CellContents == null)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsCellContentsRequired, Severity.Error, context.ObjectType, context.ObjectName, "TablixCornerCell");
			}
			else
			{
				if (!nullable.HasValue)
				{
					tablixCornerCell.RowSpan = 1;
				}
				else
				{
					int? nullable3 = nullable;
					if (nullable3.GetValueOrDefault() == 0 && nullable3.HasValue)
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsInvalidTablixCornerCellSpan, Severity.Error, context.ObjectType, context.ObjectName, "RowSpan");
					}
					else
					{
						tablixCornerCell.RowSpan = nullable.Value;
					}
				}
				if (!nullable2.HasValue)
				{
					tablixCornerCell.ColSpan = 1;
				}
				else
				{
					int? nullable4 = nullable2;
					if (nullable4.GetValueOrDefault() == 0 && nullable4.HasValue)
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsInvalidTablixCornerCellSpan, Severity.Error, context.ObjectType, context.ObjectName, "ColSpan");
					}
					else
					{
						tablixCornerCell.ColSpan = nullable2.Value;
					}
				}
			}
			return tablixCornerCell;
		}

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.TablixColumn> ReadTablixColumns(AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context, List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.TablixColumn> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.TablixColumn>();
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "TablixColumn")
					{
						list.Add(this.ReadTablixColumn(tablix, context, textBoxesWithDefaultSortTarget));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("TablixColumns" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return list;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.TablixRow ReadTablixRow(AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context, List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.TablixRow tablixRow = new AspNetCore.ReportingServices.ReportIntermediateFormat.TablixRow(this.GenerateID());
			int num = -1;
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "Height":
						tablixRow.Height = this.ReadSize();
						break;
					case "TablixCells":
						tablixRow.TablixCells = this.ReadTablixCells(tablix, context, textBoxesWithDefaultSortTarget);
						if (num > 0 && tablixRow.Cells.Count != num)
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInconsistentNumberofCellsInRow, Severity.Error, context.ObjectType, context.ObjectName, "Tablix");
						}
						num = tablixRow.Cells.Count;
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("TablixRow" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (tablixRow.Cells == null)
			{
				tablixRow.TablixCells = new TablixCellList();
			}
			return tablixRow;
		}

		private TablixCellList ReadTablixCells(AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context, List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
		{
			TablixCellList tablixCellList = new TablixCellList();
			bool flag = false;
			int num = 1;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "TablixCell")
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCell tablixCell = this.ReadTablixCell(tablix, context, num > 1, textBoxesWithDefaultSortTarget);
						num = ((num != 1) ? (num - 1) : tablixCell.ColSpan);
						tablixCellList.Add(tablixCell);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("TablixCells" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return tablixCellList;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCell ReadTablixCell(AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context, bool shouldBeEmpty, List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCell tablixCell = new AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCell(this.GenerateID(), tablix);
			this.m_aggregateHolderList.Add(tablixCell);
			this.m_runningValueHolderList.Add(tablixCell);
			bool flag = false;
			int? nullable = null;
			int? nullable2 = null;
			bool flag2 = false;
			if (!this.m_reader.IsEmptyElement)
			{
				string dataSetName = null;
				List<IdcRelationship> relationships = null;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "DataSetName":
							dataSetName = this.m_reader.ReadString();
							break;
						case "Relationships":
							relationships = this.ReadRelationships(context);
							break;
						case "CellContents":
						{
							flag2 = true;
							AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem altCellContents = default(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem);
							tablixCell.CellContents = this.ReadCellContents((AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem)tablix, context, textBoxesWithDefaultSortTarget, true, out altCellContents, out nullable, out nullable2);
							tablixCell.AltCellContents = altCellContents;
							break;
						}
						case "DataElementName":
							tablixCell.DataElementName = this.m_reader.ReadString();
							break;
						case "DataElementOutput":
							tablixCell.DataElementOutput = this.ReadDataElementOutput();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("TablixCell" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
				tablixCell.DataScopeInfo.SetRelationship(dataSetName, relationships);
			}
			if (shouldBeEmpty)
			{
				if (flag2)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsCellContentsNotOmitted, Severity.Error, context.ObjectType, context.ObjectName, "TablixCell");
				}
				if (nullable.HasValue && nullable.Value != 0)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidTablixCellCellSpan, Severity.Error, context.ObjectType, context.ObjectName, "ColSpan");
				}
			}
			else
			{
				if (!flag2)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsCellContentsRequired, Severity.Error, context.ObjectType, context.ObjectName, "TablixCell");
				}
				if (!nullable.HasValue)
				{
					tablixCell.ColSpan = 1;
				}
				else if (nullable.Value == 0)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidTablixCellCellSpan, Severity.Error, context.ObjectType, context.ObjectName, "ColSpan");
				}
				else
				{
					tablixCell.ColSpan = nullable.Value;
				}
			}
			if (nullable2.HasValue && nullable2.Value != 1)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidTablixCellRowSpan, Severity.Error, context.ObjectType, context.ObjectName, "RowSpan");
			}
			else
			{
				tablixCell.RowSpan = 1;
			}
			return tablixCell;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.TablixColumn ReadTablixColumn(AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context, List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.TablixColumn tablixColumn = new AspNetCore.ReportingServices.ReportIntermediateFormat.TablixColumn(this.GenerateID());
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "Width")
					{
						tablixColumn.Width = this.ReadSize();
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("TablixColumn" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return tablixColumn;
		}

		private TablixRowList ReadTablixRows(AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context, List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
		{
			TablixRowList tablixRowList = new TablixRowList();
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "TablixRow")
					{
						tablixRowList.Add(this.ReadTablixRow(tablix, context, textBoxesWithDefaultSortTarget));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("TablixRows" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return tablixRowList;
		}

		private void ReadTablixBody(AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix tablix, PublishingContextStruct context, List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget)
		{
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "TablixColumns":
						tablix.TablixColumns = this.ReadTablixColumns(tablix, context, textBoxesWithDefaultSortTarget);
						break;
					case "TablixRows":
						tablix.TablixRows = this.ReadTablixRows(tablix, context, textBoxesWithDefaultSortTarget);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("TablixBody" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem ReadCellContents(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem parent, PublishingContextStruct context, List<AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox> textBoxesWithDefaultSortTarget, bool readRowColSpans, out AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem altCellContents, out int? colSpan, out int? rowSpan)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem result = null;
			altCellContents = null;
			colSpan = null;
			rowSpan = null;
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "Line":
							result = this.ReadLine(parent, context);
							break;
						case "Rectangle":
							result = this.ReadRectangle(parent, context, textBoxesWithDefaultSortTarget);
							break;
						case "CustomReportItem":
							result = this.ReadCustomReportItem(parent, context, textBoxesWithDefaultSortTarget, out altCellContents);
							Global.Tracer.Assert(altCellContents != null);
							break;
						case "Textbox":
							result = this.ReadTextbox(parent, context, textBoxesWithDefaultSortTarget);
							break;
						case "Image":
							result = this.ReadImage(parent, context);
							break;
						case "Subreport":
							result = this.ReadSubreport(parent, context);
							break;
						case "Tablix":
							result = this.ReadTablix(parent, context);
							break;
						case "Chart":
							result = this.ReadChart(parent, context);
							break;
						case "GaugePanel":
							result = this.ReadGaugePanel(parent, context);
							break;
						case "Map":
							result = this.ReadMap(parent, context);
							break;
						default:
							if (readRowColSpans)
							{
								switch (this.m_reader.LocalName)
								{
								case "ColSpan":
									colSpan = this.m_reader.ReadInteger(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
									break;
								case "RowSpan":
									rowSpan = this.m_reader.ReadInteger(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
									break;
								}
							}
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if (!readRowColSpans || !(this.m_reader.LocalName == "CellContents"))
						{
							if (readRowColSpans)
							{
								break;
							}
							if (!(this.m_reader.LocalName == "ReportItem"))
							{
								break;
							}
						}
						flag = true;
						break;
					}
				}
				while (!flag);
			}
			return result;
		}

		private bool ReadLayoutDirection()
		{
			string x = this.m_reader.ReadString();
			return 0 == AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.CompareWithInvariantCulture(x, "RTL", false);
		}

		private void Phase2()
		{
			if (1 < this.m_dataSets.Count)
			{
				this.m_reportCT.ConvertFields2ComplexExpr();
			}
			else
			{
				this.m_report.OneDataSetName = ((this.m_dataSets.Count == 1) ? this.m_dataSets[0].Name : null);
			}
			if (0 < this.m_textBoxesWithUserSortTarget.Count)
			{
				for (int i = 0; i < this.m_textBoxesWithUserSortTarget.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.EndUserSort userSort = this.m_textBoxesWithUserSortTarget[i].UserSort;
					AspNetCore.ReportingServices.ReportIntermediateFormat.ISortFilterScope sortFilterScope = null;
					this.m_reportScopes.TryGetValue(userSort.SortTargetString, out sortFilterScope);
					if (sortFilterScope != null)
					{
						userSort.SetSortTarget(sortFilterScope);
					}
				}
			}
			this.m_report.LastAggregateID = this.m_reportCT.LastAggregateID;
			this.m_report.LastLookupID = this.m_reportCT.LastLookupID;
			this.MapAndValidateDataSets();
			this.CreateAutomaticSubtotalsAndDomainScopeMembers();
			this.m_report.MergeOnePass = (!this.m_hasGrouping && !this.m_hasSorting && !this.m_reportCT.BodyRefersToReportItems && !this.m_reportCT.ValueReferencedGlobal && !this.m_subReportMergeTransactions && !this.m_hasUserSort && !this.m_reportCT.AggregateOfAggregateUsed);
			this.m_report.SubReportMergeTransactions = this.m_subReportMergeTransactions;
			this.m_report.NeedPostGroupProcessing = (this.m_requiresSortingPostGrouping || this.m_hasGroupFilters || this.m_reportCT.AggregateOfAggregateUsed || this.m_domainScopeGroups.Count > 0);
			this.m_report.HasSpecialRecursiveAggregates = this.m_hasSpecialRecursiveAggregates;
			this.m_report.HasReportItemReferences = this.m_reportCT.BodyRefersToReportItems;
			this.m_report.HasImageStreams = this.m_hasImageStreams;
			this.m_report.HasBookmarks = this.m_hasBookmarks;
			this.m_report.HasLabels = this.m_hasLabels;
			this.m_report.HasUserSortFilter = this.m_hasUserSort;
			if (this.m_report.ReportSections != null)
			{
				foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSection reportSection in this.m_report.ReportSections)
				{
					reportSection.NeedsReportItemsOnPage |= (0 != reportSection.Page.PageAggregates.Count);
				}
			}
		}

		private static void RegisterDataSetWithDataSource(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet, AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource dataSource, int dataSourceIndex, Hashtable dataSetQueryInfo, bool hasDynamicParameters)
		{
			dataSet.DataSource = dataSource;
			if (dataSource.DataSets == null)
			{
				dataSource.DataSets = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet>();
			}
			if (hasDynamicParameters)
			{
				PublishingDataSetInfo publishingDataSetInfo = (PublishingDataSetInfo)dataSetQueryInfo[dataSet.Name];
				Global.Tracer.Assert(null != publishingDataSetInfo, "(null != dataSetInfo)");
				publishingDataSetInfo.DataSourceIndex = dataSourceIndex;
				publishingDataSetInfo.DataSetIndex = dataSource.DataSets.Count;
				publishingDataSetInfo.MergeFlagsFromDataSource(dataSource.IsComplex, dataSource.ParameterNames);
			}
			dataSource.DataSets.Add(dataSet);
		}

		private void MapAndValidateDataSets()
		{
			if (this.m_dataSets != null && this.m_dataSets.Count != 0)
			{
				bool hasDynamicParameters = false;
				if (this.m_dynamicParameters != null && this.m_dynamicParameters.Count > 0)
				{
					hasDynamicParameters = true;
				}
				foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet in this.m_dataSets)
				{
					bool flag = false;
					if (dataSet.IsReferenceToSharedDataSet)
					{
						if (this.m_report.SharedDSContainer != null)
						{
							flag = true;
							ReportPublishing.RegisterDataSetWithDataSource(dataSet, this.m_report.SharedDSContainer, this.m_report.SharedDSContainerCollectionIndex, this.m_dataSetQueryInfo, hasDynamicParameters);
						}
					}
					else if (dataSet.Query != null && this.m_report.DataSources != null)
					{
						for (int i = 0; i < this.m_report.DataSources.Count; i++)
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource dataSource = this.m_report.DataSources[i];
							if (dataSet.Query.DataSourceName == dataSource.Name)
							{
								flag = true;
								ReportPublishing.RegisterDataSetWithDataSource(dataSet, dataSource, i, this.m_dataSetQueryInfo, hasDynamicParameters);
								break;
							}
						}
					}
					if (!flag && dataSet.Query != null)
					{
						this.m_errorContext.Register(ProcessingErrorCode.rsInvalidDataSourceReference, Severity.Error, dataSet.ObjectType, dataSet.Name, "DataSourceName", dataSet.Query.DataSourceName.MarkAsPrivate());
					}
				}
			}
		}

		private void CreateAutomaticSubtotalsAndDomainScopeMembers()
		{
			if (!this.m_errorContext.HasError && (this.ShouldCreateAutomaticSubtotals() || this.ShouldCreateDomainScopeMembers()))
			{
				AutomaticSubtotalContext context = new AutomaticSubtotalContext(this.m_report, this.m_createSubtotalsDefs, this.m_domainScopeGroups, this.m_reportItemNames, this.m_scopeNames, this.m_variableNames, this.m_reportScopes, this.m_reportItemCollectionList, this.m_aggregateHolderList, this.m_runningValueHolderList, this.m_variableSequenceIdCounter, this.m_textboxSequenceIdCounter, this.m_report.BuildScopeTree());
				if (this.ShouldCreateAutomaticSubtotals())
				{
					for (int i = 0; i < this.m_createSubtotalsDefs.Count; i++)
					{
						ICreateSubtotals createSubtotals = this.m_createSubtotalsDefs[i];
						createSubtotals.CreateAutomaticSubtotals(context);
					}
				}
				if (this.ShouldCreateDomainScopeMembers())
				{
					foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping domainScopeGroup in this.m_domainScopeGroups)
					{
						this.CreateDomainScopeMember(domainScopeGroup, context);
					}
				}
			}
			this.m_reportItemNames = null;
			this.m_scopeNames = null;
			this.m_variableNames = null;
		}

		private bool ShouldCreateDomainScopeMembers()
		{
			if (this.m_domainScopeGroups.Count > 0)
			{
				return !this.m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.DomainScope);
			}
			return false;
		}

		private bool ShouldCreateAutomaticSubtotals()
		{
			if (this.m_createSubtotalsDefs.Count > 0)
			{
				return !this.m_publishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.AutomaticSubtotals);
			}
			return false;
		}

		private void CreateDomainScopeMember(AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping group, AutomaticSubtotalContext context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode owner = group.Owner;
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef = owner.DataRegionDef;
			if (owner.InnerDynamicMembers != null && owner.InnerDynamicMembers.Count > 0)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsInvalidGroupingDomainScopeNotLeaf, Severity.Error, dataRegionDef.ObjectType, dataRegionDef.Name, "DomainScope", group.Name.MarkAsModelInfo(), group.DomainScope.MarkAsPrivate());
			}
			else if (!this.m_reportScopes.ContainsKey(group.DomainScope))
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsInvalidGroupingDomainScope, Severity.Error, dataRegionDef.ObjectType, dataRegionDef.Name, "DomainScope", group.Name.MarkAsModelInfo(), group.DomainScope.MarkAsPrivate());
			}
			else
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.ISortFilterScope sortFilterScope = this.m_reportScopes[group.DomainScope];
				if (sortFilterScope is AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet)
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsInvalidGroupingDomainScopeDataSet, Severity.Error, dataRegionDef.ObjectType, dataRegionDef.Name, "DomainScope", group.Name.MarkAsModelInfo(), group.DomainScope.MarkAsPrivate());
				}
				else
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode reportHierarchyNode = null;
					AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion;
					if (sortFilterScope is AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping)
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping grouping = (AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping)sortFilterScope;
						if (grouping.Parent != null)
						{
							this.m_errorContext.Register(ProcessingErrorCode.rsInvalidGroupingDomainScopeTargetWithParent, Severity.Error, dataRegionDef.ObjectType, dataRegionDef.Name, "DomainScope", group.Name.MarkAsModelInfo(), group.DomainScope.MarkAsPrivate());
							return;
						}
						reportHierarchyNode = grouping.Owner;
						dataRegion = reportHierarchyNode.DataRegionDef;
					}
					else
					{
						dataRegion = (AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion)sortFilterScope;
					}
					dataRegion.CreateDomainScopeMember(reportHierarchyNode, group, context);
				}
			}
		}

		private void Phase3(out ParameterInfoCollection parameters, out Dictionary<string, int> groupingExprCountAtScope)
		{
			try
			{
				this.m_report.HasPreviousAggregates = this.m_reportCT.PreviousAggregateUsed;
				this.m_report.HasAggregatesOfAggregates = this.m_reportCT.AggregateOfAggregateUsed;
				this.m_report.HasAggregatesOfAggregatesInUserSort = this.m_reportCT.AggregateOfAggregateUsedInUserSort;
				this.m_scopeTree = ScopeTreeBuilderForDataScopeDataSet.BuildScopeTree(this.m_report, this.m_errorContext);
				this.m_reportCT.Builder.ReportStart();
				ReportIntermediateFormat.InitializationContext context = new ReportIntermediateFormat.InitializationContext(this.m_publishingContext.CatalogContext, this.m_hasFilters, this.m_dataSourceNames, this.m_dataSets, this.m_dynamicParameters, this.m_dataSetQueryInfo, this.m_errorContext, this.m_reportCT.Builder, this.m_report, this.m_reportLanguage, this.m_reportScopes, this.m_hasUserSortPeerScopes, this.m_hasUserSort, this.m_dataRegionCount, this.m_textboxSequenceIdCounter.Value, this.m_variableSequenceIdCounter.Value, this.m_publishingContext, this.m_scopeTree, false);
				this.m_report.Initialize(context);
				this.InitializeParameters(context, out parameters);
				groupingExprCountAtScope = context.GroupingExprCountAtScope;
				this.m_reportCT.Builder.ReportEnd();
				if (!this.m_errorContext.HasError)
				{
					((IExpressionHostAssemblyHolder)this.m_report).CompiledCode = this.m_reportCT.Compile(this.m_report, this.m_publishingContext.CompilationTempAppDomain, this.m_publishingContext.GenerateExpressionHostWithRefusedPermissions, this.m_publishingContext.PublishingVersioning);
				}
				int num = 0;
				for (int i = 0; i < this.m_dataSets.Count; i++)
				{
					if (!this.m_dataSets[i].UsedOnlyInParameters)
					{
						if (num == 0)
						{
							this.m_report.FirstDataSet = this.m_dataSets[i];
						}
						num++;
						if (1 < num)
						{
							this.m_report.MergeOnePass = false;
						}
					}
				}
				this.m_report.DataSetsNotOnlyUsedInParameters = num;
				this.m_report.HasLookups = context.HasLookups;
			}
			finally
			{
				this.m_reportCT = null;
			}
		}

		private void InitializeParameters(AspNetCore.ReportingServices.ReportIntermediateFormat.InitializationContext context, out ParameterInfoCollection parameters)
		{
			bool parametersNotUsedInQuery = false;
			AspNetCore.ReportingServices.ReportProcessing.ParameterInfo parameterInfo = null;
			parameters = new ParameterInfoCollection();
			parameters.ParametersLayout = this.m_parametersLayout;
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef> parameters2 = this.m_report.Parameters;
			if (parameters2 != null && parameters2.Count > 0)
			{
				context.InitializeParameters(this.m_report.Parameters, this.m_dataSets);
				for (int i = 0; i < parameters2.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef parameterDef = parameters2[i];
					if (parameterDef.UsedInQueryAsDefined == ParameterBase.UsedInQueryType.Auto)
					{
						if (this.m_parametersNotUsedInQuery)
						{
							if (this.m_usedInQueryInfos.Contains(parameterDef.Name))
							{
								parameterDef.UsedInQuery = true;
							}
							else
							{
								parameterDef.UsedInQuery = false;
								parametersNotUsedInQuery = true;
							}
						}
						else
						{
							parameterDef.UsedInQuery = true;
						}
					}
					else if (parameterDef.UsedInQueryAsDefined == ParameterBase.UsedInQueryType.False)
					{
						parametersNotUsedInQuery = true;
						parameterDef.UsedInQuery = false;
					}
					if (parameterDef.UsedInQuery && (this.m_userReferenceLocation & UserLocationFlags.ReportQueries) == (UserLocationFlags)0 && this.m_reportParamUserProfile.Contains(parameterDef.Name))
					{
						this.m_userReferenceLocation |= UserLocationFlags.ReportQueries;
					}
					parameterDef.Initialize(context);
					parameterInfo = new AspNetCore.ReportingServices.ReportProcessing.ParameterInfo(parameterDef);
					if (parameterDef.PromptExpression != null)
					{
						if (parameterDef.PromptExpression.Type == AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant)
						{
							parameterInfo.DynamicPrompt = false;
							parameterInfo.Prompt = parameterDef.PromptExpression.StringValue;
						}
						else
						{
							parameterInfo.DynamicPrompt = true;
							parameterInfo.Prompt = parameterDef.Name;
						}
					}
					if (parameterDef.Dependencies != null && parameterDef.Dependencies.Count > 0)
					{
						int num = 0;
						IDictionaryEnumerator enumerator = parameterDef.Dependencies.GetEnumerator();
						List<AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef>();
						ParameterInfoCollection parameterInfoCollection = new ParameterInfoCollection();
						while (enumerator.MoveNext())
						{
							num = (int)enumerator.Value;
							list.Add(parameters2[num]);
							parameterInfoCollection.Add(parameters[num]);
							if (parameterDef.UsedInQuery)
							{
								parameters[num].UsedInQuery = true;
							}
						}
						parameterDef.DependencyList = list;
						parameterInfo.DependencyList = parameterInfoCollection;
					}
					if (parameterDef.ValidValuesDataSource != null)
					{
						parameterInfo.DynamicValidValues = true;
					}
					else if (parameterDef.ValidValuesValueExpressions != null)
					{
						int count = parameterDef.ValidValuesValueExpressions.Count;
						for (int j = 0; j < count && !parameterInfo.DynamicValidValues; j++)
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = parameterDef.ValidValuesValueExpressions[j];
							AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo2 = parameterDef.ValidValuesLabelExpressions[j];
							if (expressionInfo != null && AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant != expressionInfo.Type)
							{
								goto IL_0251;
							}
							if (expressionInfo2 != null && AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant != expressionInfo2.Type)
							{
								goto IL_0251;
							}
							continue;
							IL_0251:
							parameterInfo.DynamicValidValues = true;
						}
						if (!parameterInfo.DynamicValidValues)
						{
							parameterInfo.ValidValues = new ValidValueList(count);
							for (int k = 0; k < count; k++)
							{
								AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo3 = parameterDef.ValidValuesValueExpressions[k];
								AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo4 = parameterDef.ValidValuesLabelExpressions[k];
								object paramValue = null;
								string paramLabel = null;
								if (expressionInfo4 != null)
								{
									paramLabel = expressionInfo4.StringValue;
								}
								if (expressionInfo3 != null)
								{
									switch (expressionInfo3.ConstantType)
									{
									case DataType.Boolean:
										paramValue = expressionInfo3.BoolValue;
										break;
									case DataType.DateTime:
										paramValue = expressionInfo3.GetDateTimeValue();
										break;
									case DataType.Float:
										paramValue = expressionInfo3.FloatValue;
										break;
									case DataType.Integer:
										paramValue = expressionInfo3.IntValue;
										break;
									case DataType.String:
										paramValue = expressionInfo3.StringValue;
										break;
									}
								}
								parameterInfo.AddValidValueExplicit(paramValue, paramLabel);
							}
						}
					}
					parameterInfo.DynamicDefaultValue = (parameterDef.DefaultDataSource != null || null != parameterDef.DefaultExpressions);
					parameterInfo.Values = parameterDef.DefaultValues;
					parameters.Add(parameterInfo);
				}
			}
			else
			{
				parametersNotUsedInQuery = true;
			}
			this.m_parametersNotUsedInQuery = parametersNotUsedInQuery;
			this.m_report.ParametersNotUsedInQuery = this.m_parametersNotUsedInQuery;
		}

		private void Phase4(Dictionary<string, int> groupingExprCountAtScope, out ArrayList dataSetsName)
		{
			this.PopulateReportItemCollections();
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion> list = default(List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion>);
			this.CompactAggregates(out list);
			this.CompactRunningValues(groupingExprCountAtScope);
			for (int i = 0; i < list.Count; i++)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion = list[i];
				bool flag = false;
				if (dataRegion.CellAggregates != null)
				{
					this.m_aggregateHashByType.Clear();
					this.CompactAggregates(dataRegion.CellAggregates, true, this.m_aggregateHashByType);
					flag = true;
				}
				if (dataRegion.CellPostSortAggregates != null)
				{
					this.m_aggregateHashByType.Clear();
					if (this.CompactAggregates(dataRegion.CellPostSortAggregates, true, this.m_aggregateHashByType))
					{
						this.m_report.HasPostSortAggregates = true;
					}
					flag = true;
				}
				if (dataRegion.CellRunningValues != null)
				{
					this.m_runningValueHashByType.Clear();
					this.CompactRunningValueList(groupingExprCountAtScope, dataRegion.CellRunningValues, true, this.m_runningValueHashByType);
					flag = true;
				}
				if (flag)
				{
					dataRegion.ConvertCellAggregatesToIndexes();
				}
			}
			dataSetsName = null;
			if (!this.m_errorContext.HasError)
			{
				for (int j = 0; j < this.m_dataSets.Count; j++)
				{
					if (this.m_publishingContext.IsRdlx)
					{
						this.m_dataSets[j].RestrictDataSetAggregates(this.m_errorContext);
					}
					if (!this.m_dataSets[j].UsedOnlyInParameters)
					{
						if (dataSetsName == null)
						{
							dataSetsName = new ArrayList();
						}
						dataSetsName.Add(this.m_dataSets[j].Name);
					}
					else
					{
						this.m_report.ClearDatasetParameterOnlyDependencies(this.m_dataSets[j].IndexInCollection);
					}
				}
				this.m_report.Phase4_DetermineFirstDatasetToProcess();
			}
		}

		private void PopulateReportItemCollections()
		{
			try
			{
				Global.Tracer.Assert(null != this.m_reportItemCollectionList);
				for (int i = 0; i < this.m_reportItemCollectionList.Count; i++)
				{
					this.m_reportItemCollectionList[i].Populate(this.m_errorContext);
				}
			}
			finally
			{
				this.m_reportItemCollectionList = null;
			}
		}

		private void CompactAggregates(out List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion> dataRegions)
		{
			dataRegions = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion>();
			try
			{
				List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo> list = null;
				for (int i = 0; i < this.m_aggregateHolderList.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.IAggregateHolder aggregateHolder = this.m_aggregateHolderList[i];
					Global.Tracer.Assert(null != aggregateHolder);
					list = aggregateHolder.GetAggregateList();
					Global.Tracer.Assert(null != list);
					this.m_aggregateHashByType.Clear();
					this.CompactAggregates(list, false, this.m_aggregateHashByType);
					list = aggregateHolder.GetPostSortAggregateList();
					this.m_aggregateHashByType.Clear();
					if (list != null && this.CompactAggregates(list, false, this.m_aggregateHashByType))
					{
						this.m_report.HasPostSortAggregates = true;
					}
					DataScopeInfo dataScopeInfo = aggregateHolder.DataScopeInfo;
					if (dataScopeInfo != null)
					{
						BucketedDataAggregateInfos aggregatesOfAggregates = dataScopeInfo.AggregatesOfAggregates;
						if (list != null)
						{
							this.m_aggregateHashByType.Clear();
							this.CompactAggregates(aggregatesOfAggregates, false, this.m_aggregateHashByType);
						}
						aggregatesOfAggregates = dataScopeInfo.PostSortAggregatesOfAggregates;
						this.m_aggregateHashByType.Clear();
						if (list != null && this.CompactAggregates(aggregatesOfAggregates, false, this.m_aggregateHashByType))
						{
							this.m_report.HasPostSortAggregates = true;
						}
						dataScopeInfo.ClearAggregatesIfEmpty();
					}
					if (aggregateHolder is AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping)
					{
						if (this.CompactAggregates(((AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping)aggregateHolder).RecursiveAggregates, false, this.m_aggregateHashByType))
						{
							this.m_report.NeedPostGroupProcessing = true;
						}
					}
					else if (aggregateHolder is AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion && ((AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion)aggregateHolder).IsDataRegion)
					{
						dataRegions.Add(aggregateHolder as AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion);
					}
					aggregateHolder.ClearIfEmpty();
				}
			}
			finally
			{
				this.m_aggregateHolderList = null;
			}
		}

		private bool CompactAggregates(BucketedDataAggregateInfos aggregates, bool nonDataRegionScopedCellAggs, Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes, Dictionary<string, Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>>> aggregateHashByType)
		{
			bool flag = false;
			foreach (AggregateBucket<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo> bucket in aggregates.Buckets)
			{
				if (bucket.Aggregates != null)
				{
					flag |= this.CompactAggregates(bucket.Aggregates, nonDataRegionScopedCellAggs, aggregateHashByType);
				}
			}
			return flag;
		}

		private bool CompactAggregates(List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo> aggregates, bool nonDataRegionScopedCellAggs, Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes, Dictionary<string, Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>>> aggregateHashByType)
		{
			Global.Tracer.Assert(null != aggregates);
			Global.Tracer.Assert(null != aggregateHashByType);
			bool result = false;
			for (int num = aggregates.Count - 1; num >= 0; num--)
			{
				result = true;
				AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo dataAggregateInfo = aggregates[num];
				Global.Tracer.Assert(null != dataAggregateInfo);
				string text = null;
				text = ((!nonDataRegionScopedCellAggs) ? "" : dataAggregateInfo.EvaluationScopeName);
				bool flag = false;
				Dictionary<string, Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>> dictionary = default(Dictionary<string, Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>>);
				if (aggregateHashByType.TryGetValue(dataAggregateInfo.AggregateType, out dictionary))
				{
					Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo> dictionary2 = default(Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>);
					if (dictionary.TryGetValue(text, out dictionary2))
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo dataAggregateInfo2 = default(AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo);
						if (dictionary2.TryGetValue(dataAggregateInfo.ExpressionTextForCompaction, out dataAggregateInfo2))
						{
							if (!dataAggregateInfo.IsAggregateOfAggregate || this.AreNestedAggregateScopesIdentical(dataAggregateInfo, dataAggregateInfo2))
							{
								if (dataAggregateInfo2.DuplicateNames == null)
								{
									dataAggregateInfo2.DuplicateNames = new List<string>();
								}
								dataAggregateInfo2.DuplicateNames.Add(dataAggregateInfo.Name);
								if (dataAggregateInfo.DuplicateNames != null)
								{
									dataAggregateInfo2.DuplicateNames.AddRange(dataAggregateInfo.DuplicateNames);
								}
								aggregates.RemoveAt(num);
								flag = true;
							}
						}
						else
						{
							dictionary2.Add(dataAggregateInfo.ExpressionTextForCompaction, dataAggregateInfo);
						}
					}
					else
					{
						dictionary2 = new Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>();
						dictionary2.Add(dataAggregateInfo.ExpressionTextForCompaction, dataAggregateInfo);
						dictionary.Add(text, dictionary2);
					}
				}
				else
				{
					Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo> dictionary3 = new Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>();
					dictionary3.Add(dataAggregateInfo.ExpressionTextForCompaction, dataAggregateInfo);
					dictionary = new Dictionary<string, Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>>();
					dictionary.Add(text, dictionary3);
					aggregateHashByType.Add(dataAggregateInfo.AggregateType, dictionary);
				}
				if (!flag)
				{
					this.ProcessPostCompactedAggOrRv(dataAggregateInfo);
				}
			}
			return result;
		}

		private bool AreNestedAggregateScopesIdentical(AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo agg1, AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo agg2)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo> nestedAggregates = agg1.PublishingInfo.NestedAggregates;
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo> nestedAggregates2 = agg2.PublishingInfo.NestedAggregates;
			Global.Tracer.Assert(nestedAggregates.Count == nestedAggregates2.Count, "Duplicate candidate has identical text but different number of nested aggs.");
			for (int i = 0; i < nestedAggregates.Count; i++)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo dataAggregateInfo = nestedAggregates[i];
				AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo dataAggregateInfo2 = nestedAggregates2[i];
				if (dataAggregateInfo.EvaluationScope != dataAggregateInfo2.EvaluationScope)
				{
					return false;
				}
				if (dataAggregateInfo.IsAggregateOfAggregate && !this.AreNestedAggregateScopesIdentical(dataAggregateInfo, dataAggregateInfo2))
				{
					return false;
				}
			}
			return true;
		}

		private void ProcessPostCompactedAggOrRv(AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo aggregate)
		{
			if (!aggregate.IsAggregateOfAggregate)
			{
				this.TraceAggregateInformation(aggregate, null);
			}
			else
			{
				IRIFDataScope evaluationScope = aggregate.EvaluationScope;
				if (evaluationScope != null)
				{
					foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo nestedAggregate in aggregate.PublishingInfo.NestedAggregates)
					{
						IRIFDataScope evaluationScope2 = nestedAggregate.EvaluationScope;
						if (evaluationScope2 != null)
						{
							if (!this.m_scopeTree.IsSameOrProperParentScope(aggregate.EvaluationScope, evaluationScope2))
							{
								this.RegisterIncompatibleAggregateScopes(aggregate.EvaluationScope, evaluationScope2, ProcessingErrorCode.rsInvalidNestedAggregateScope, aggregate);
							}
							if (this.m_scopeTree.IsSameOrProperParentScope(evaluationScope, evaluationScope2))
							{
								evaluationScope = nestedAggregate.EvaluationScope;
							}
							else if (!this.m_scopeTree.IsSameOrProperParentScope(evaluationScope2, evaluationScope))
							{
								this.RegisterIncompatibleAggregateScopes(evaluationScope, evaluationScope2, ProcessingErrorCode.rsIncompatibleNestedAggregateScopes, aggregate);
							}
						}
					}
					if (evaluationScope != null && evaluationScope.DataScopeInfo != null)
					{
						aggregate.UpdatesAtRowScope = aggregate.PublishingInfo.HasAnyFieldReferences;
						evaluationScope.DataScopeInfo.HasAggregatesToUpdateAtRowScope |= aggregate.PublishingInfo.HasAnyFieldReferences;
						aggregate.UpdateScopeID = evaluationScope.DataScopeInfo.ScopeID;
						if (!aggregate.IsRunningValue() && !aggregate.IsPostSortAggregate())
						{
							this.m_scopeTree.Traverse(this.CheckSpannedGroupFilters, aggregate.EvaluationScope, evaluationScope, false);
							this.m_report.NeedPostGroupProcessing = true;
						}
					}
					this.TraceAggregateInformation(aggregate, evaluationScope);
				}
			}
		}

		private void RegisterIncompatibleAggregateScopes(IRIFDataScope firstScope, IRIFDataScope secondScope, ProcessingErrorCode fallbackMessage, AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo aggregate)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode reportHierarchyNode = firstScope as AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode;
			AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode reportHierarchyNode2 = secondScope as AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode;
			if (reportHierarchyNode != null && reportHierarchyNode2 != null && reportHierarchyNode.DataRegionDef == reportHierarchyNode2.DataRegionDef && reportHierarchyNode.IsColumn != reportHierarchyNode2.IsColumn)
			{
				this.RegisterAggregateScopeValidationError(ProcessingErrorCode.rsNestedAggregateScopesFromDifferentAxes, aggregate);
			}
			else if (secondScope is Cell)
			{
				this.RegisterAggregateScopeValidationError(ProcessingErrorCode.rsNestedAggregateScopeRequired, aggregate);
			}
			else
			{
				this.RegisterAggregateScopeValidationError(fallbackMessage, aggregate);
			}
		}

		private void RegisterAggregateScopeValidationError(ProcessingErrorCode errorCode, AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo aggregate)
		{
			this.m_errorContext.Register(errorCode, Severity.Error, aggregate.PublishingInfo.ObjectType, aggregate.PublishingInfo.ObjectName, aggregate.PublishingInfo.PropertyName);
		}

		private void TraceAggregateInformation(AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo aggregate, IRIFDataScope commonAggregateScope)
		{
			if (Global.Tracer.TraceVerbose && aggregate.AggregateType != AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Previous)
			{
				if (!this.m_wroteAggregateHeaderInformation)
				{
					this.m_wroteAggregateHeaderInformation = true;
					Global.Tracer.Trace(TraceLevel.Verbose, "Aggregate Debugging Information (duplicate items removed):");
				}
				string scopeName = this.GetScopeName(aggregate.EvaluationScope);
				AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo runningValueInfo = aggregate as AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo;
				string text = (runningValueInfo == null) ? scopeName : runningValueInfo.Scope;
				string text2;
				if (commonAggregateScope == null)
				{
					text2 = scopeName + "-ROW";
				}
				else
				{
					text2 = this.GetScopeName(commonAggregateScope);
					if (aggregate.UpdatesAtRowScope)
					{
						text2 += "-ROW";
					}
				}
				Global.Tracer.Trace(TraceLevel.Verbose, "    Aggregate: ContainingObject:{4} ContainingProperty:{5} Text:{0} UpdateScope:{1} OutputScope:{2} ResetScope:{3}", aggregate.GetAsString().MarkAsPrivate(), text2, scopeName, text, aggregate.PublishingInfo.ObjectName.MarkAsPrivate(), aggregate.PublishingInfo.PropertyName);
			}
		}

		private string GetScopeName(IRIFDataScope dataScope)
		{
			if (dataScope == null)
			{
				return "UNKNOWN";
			}
			Cell cell = dataScope as Cell;
			if (cell != null)
			{
				return this.m_scopeTree.GetScopeName(cell);
			}
			return dataScope.Name;
		}

		private void CheckSpannedGroupFilters(IRIFDataScope dataScope)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode reportHierarchyNode = dataScope as AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode;
			if (reportHierarchyNode != null && reportHierarchyNode.Grouping != null && reportHierarchyNode.Grouping.Filters != null && reportHierarchyNode.Grouping.Filters.Count > 0)
			{
				reportHierarchyNode.DataScopeInfo.AggregatesSpanGroupFilter = true;
			}
		}

		private void CompactRunningValues(Dictionary<string, int> groupingExprCountAtScope)
		{
			try
			{
				for (int i = 0; i < this.m_runningValueHolderList.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.IRunningValueHolder runningValueHolder = this.m_runningValueHolderList[i];
					Global.Tracer.Assert(null != runningValueHolder);
					this.m_runningValueHashByType.Clear();
					this.CompactRunningValueList(groupingExprCountAtScope, runningValueHolder.GetRunningValueList(), false, this.m_runningValueHashByType);
					DataScopeInfo dataScopeInfo = runningValueHolder.DataScopeInfo;
					if (dataScopeInfo != null)
					{
						this.m_runningValueHashByType.Clear();
						this.CompactRunningValueList(groupingExprCountAtScope, dataScopeInfo.RunningValuesOfAggregates, false, this.m_runningValueHashByType);
						dataScopeInfo.ClearRunningValuesIfEmpty();
					}
					runningValueHolder.ClearIfEmpty();
				}
			}
			finally
			{
				this.m_runningValueHolderList = null;
			}
		}

		private void CompactRunningValueList(Dictionary<string, int> groupingExprCountAtScope, List<AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo> runningValueList, bool nonDataRegionScopedCellRVs, Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes, Dictionary<string, AllowNullKeyDictionary<string, Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo>>>> runningValueHashByType)
		{
			Global.Tracer.Assert(null != runningValueList);
			Global.Tracer.Assert(null != runningValueHashByType);
			for (int num = runningValueList.Count - 1; num >= 0; num--)
			{
				this.m_report.HasPostSortAggregates = true;
				AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo runningValueInfo = runningValueList[num];
				Global.Tracer.Assert(null != runningValueInfo);
				string text = null;
				text = ((!nonDataRegionScopedCellRVs) ? "" : runningValueInfo.EvaluationScopeName);
				if (runningValueInfo.AggregateType == AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Previous && runningValueInfo.Scope != null)
				{
					int totalGroupingExpressionCount = default(int);
					if (groupingExprCountAtScope.TryGetValue(runningValueInfo.Scope, out totalGroupingExpressionCount))
					{
						runningValueInfo.TotalGroupingExpressionCount = totalGroupingExpressionCount;
					}
					else
					{
						Global.Tracer.Assert(false);
					}
				}
				bool flag = false;
				Dictionary<string, AllowNullKeyDictionary<string, Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo>>> dictionary = default(Dictionary<string, AllowNullKeyDictionary<string, Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo>>>);
				if (runningValueHashByType.TryGetValue(runningValueInfo.AggregateType, out dictionary))
				{
					AllowNullKeyDictionary<string, Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo>> allowNullKeyDictionary = default(AllowNullKeyDictionary<string, Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo>>);
					if (dictionary.TryGetValue(text, out allowNullKeyDictionary))
					{
						Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo> dictionary2 = default(Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo>);
						if (allowNullKeyDictionary.TryGetValue(runningValueInfo.Scope, out dictionary2))
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo runningValueInfo2 = default(AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo);
							if (dictionary2.TryGetValue(runningValueInfo.ExpressionTextForCompaction, out runningValueInfo2))
							{
								if (runningValueInfo2.DuplicateNames == null)
								{
									runningValueInfo2.DuplicateNames = new List<string>();
								}
								runningValueInfo2.DuplicateNames.Add(runningValueInfo.Name);
								if (runningValueInfo.DuplicateNames != null)
								{
									runningValueInfo2.DuplicateNames.AddRange(runningValueInfo.DuplicateNames);
								}
								runningValueList.RemoveAt(num);
								flag = true;
							}
							else
							{
								dictionary2.Add(runningValueInfo.ExpressionTextForCompaction, runningValueInfo);
							}
						}
						else
						{
							dictionary2 = new Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo>();
							dictionary2.Add(runningValueInfo.ExpressionTextForCompaction, runningValueInfo);
							allowNullKeyDictionary.Add(runningValueInfo.Scope, dictionary2);
						}
					}
					else
					{
						Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo> dictionary3 = new Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo>();
						dictionary3.Add(runningValueInfo.ExpressionTextForCompaction, runningValueInfo);
						allowNullKeyDictionary = new AllowNullKeyDictionary<string, Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo>>();
						allowNullKeyDictionary.Add(runningValueInfo.Scope, dictionary3);
						dictionary.Add(text, allowNullKeyDictionary);
					}
				}
				else
				{
					Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo> dictionary4 = new Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo>();
					dictionary4.Add(runningValueInfo.ExpressionTextForCompaction, runningValueInfo);
					AllowNullKeyDictionary<string, Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo>> allowNullKeyDictionary2 = new AllowNullKeyDictionary<string, Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo>>();
					allowNullKeyDictionary2.Add(runningValueInfo.Scope, dictionary4);
					dictionary = new Dictionary<string, AllowNullKeyDictionary<string, Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo>>>();
					dictionary.Add(text, allowNullKeyDictionary2);
					runningValueHashByType.Add(runningValueInfo.AggregateType, dictionary);
				}
				if (!flag)
				{
					this.ProcessPostCompactedAggOrRv(runningValueInfo);
				}
			}
			runningValueHashByType.Clear();
		}

		internal DataSetPublishingResult CreateSharedDataSet(byte[] definition)
		{
			if (definition == null)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsNotASharedDataSetDefinition, Severity.Error, AspNetCore.ReportingServices.ReportProcessing.ObjectType.SharedDataSet, null, null);
				throw new DataSetPublishingException(this.m_errorContext.Messages);
			}
			this.RSDPhase1(definition);
			ParameterInfoCollection dataSetParameters = default(ParameterInfoCollection);
			this.RSDPhase3(out dataSetParameters);
			if (!this.m_errorContext.HasError)
			{
				DataSourceInfo theOnlyDataSource = this.m_dataSources.GetTheOnlyDataSource();
				DataSetDefinition dataSetDefinition = new DataSetDefinition(this.m_dataSetCore, this.m_description, theOnlyDataSource, dataSetParameters);
				this.SerializeSharedDataSetDefinition();
				if (this.m_userReferenceLocation != UserLocationFlags.None)
				{
					this.m_userReferenceLocation = UserLocationFlags.ReportQueries;
				}
				return new DataSetPublishingResult(dataSetDefinition, theOnlyDataSource, this.m_userReferenceLocation, this.m_errorContext.Messages);
			}
			throw new DataSetPublishingException(this.m_errorContext.Messages);
		}

		private void SerializeSharedDataSetDefinition()
		{
			if (this.m_dataSetCore != null && this.m_publishingContext.CreateChunkFactory != null)
			{
				Stream stream = null;
				try
				{
					stream = this.m_publishingContext.CreateChunkFactory.CreateChunk("CompiledDefinition", AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.CompiledDefinition, null);
					int compatibilityVersion = ReportProcessingCompatibilityVersion.GetCompatibilityVersion(this.m_publishingContext.Configuration);
					new IntermediateFormatWriter(stream, compatibilityVersion).Write(this.m_dataSetCore);
				}
				finally
				{
					if (stream != null)
					{
						stream.Close();
					}
				}
			}
		}

		private void RSDPhase1(byte[] definition)
		{
			try
			{
				this.m_dataSources = new DataSourceInfoCollection();
				Stream stream = new MemoryStream(definition, false);
				Pair<string, Stream> pair = default(Pair<string, Stream>);
				List<Pair<string, Stream>> list = new List<Pair<string, Stream>>();
				pair = this.GetRSDNamespaceSchemaStreamPair("http://schemas.microsoft.com/sqlserver/reporting/2010/01/shareddatasetdefinition", "AspNetCore.ReportingServices.ReportProcessing.ReportPublishing.SharedDataSetDefinition.xsd");
				list.Add(pair);
				this.m_reader = new RmlValidatingReader(stream, list, this.m_errorContext, RmlValidatingReader.ItemType.Rsd);
				while (this.m_reader.Read())
				{
					if (XmlNodeType.Element == this.m_reader.NodeType && "SharedDataSet" == this.m_reader.LocalName)
					{
						this.m_reportCT = new ExprHostCompiler(new AspNetCore.ReportingServices.RdlExpressions.VBExpressionParser(this.m_errorContext), this.m_errorContext);
						this.ReadRSDSharedDataSet();
					}
				}
				if (this.m_dataSetCore == null)
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsNotASharedDataSetDefinition, Severity.Error, AspNetCore.ReportingServices.ReportProcessing.ObjectType.SharedDataSet, null, "Namespace");
					throw new ReportProcessingException(this.m_errorContext.Messages);
				}
			}
			catch (XmlException ex)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsInvalidSharedDataSetDefinition, Severity.Error, AspNetCore.ReportingServices.ReportProcessing.ObjectType.SharedDataSet, null, null, ex.Message);
				throw new ReportProcessingException(this.m_errorContext.Messages);
			}
			finally
			{
				if (this.m_reader != null)
				{
					this.m_reader.Close();
					this.m_reader = null;
				}
			}
		}

		private Pair<string, Stream> GetRSDNamespaceSchemaStreamPair(string validationNamespace, string xsdResource)
		{
			Stream stream = null;
			stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(xsdResource);
			Global.Tracer.Assert(stream != null, "(schemaStream != null)");
			return new Pair<string, Stream>(validationNamespace, stream);
		}

		private void ReadRSDSharedDataSet()
		{
			int maxExpressionLength = -1;
			if (this.m_publishingContext.IsRdlSandboxingEnabled)
			{
				maxExpressionLength = this.m_publishingContext.Configuration.RdlSandboxing.MaxExpressionLength;
			}
			PublishingContextStruct context = new PublishingContextStruct(LocationFlags.None, AspNetCore.ReportingServices.ReportProcessing.ObjectType.SharedDataSet, maxExpressionLength, this.m_errorContext);
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "Description":
						this.m_description = this.m_reader.ReadString();
						break;
					case "DataSet":
						this.m_dataSets.Add(this.ReadRSDDataSet(context));
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("SharedDataSet" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet ReadRSDDataSet(PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet(this.GenerateID(), this.m_dataSetIndexCounter++);
			this.m_dataSetCore = dataSet.DataSetCore;
			this.m_dataSetCore.Name = this.m_reader.GetAttribute("Name");
			context.Location |= LocationFlags.InDataSet;
			context.ObjectType = AspNetCore.ReportingServices.ReportProcessing.ObjectType.SharedDataSet;
			context.ObjectName = this.m_dataSetCore.Name;
			this.m_reportScopes.Add(this.m_dataSetCore.Name, dataSet);
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "Fields":
					{
						int nonCalculatedFieldCount = default(int);
						this.m_dataSetCore.Fields = this.ReadFields(context, out nonCalculatedFieldCount);
						this.m_dataSetCore.NonCalculatedFieldCount = nonCalculatedFieldCount;
						break;
					}
					case "Query":
						this.m_dataSetCore.Query = this.ReadRSDQuery(context);
						break;
					case "CaseSensitivity":
						this.m_dataSetCore.CaseSensitivity = this.ReadTriState();
						break;
					case "Collation":
					{
						this.m_dataSetCore.Collation = this.m_reader.ReadString();
						uint lCID = default(uint);
						if (DataSetValidator.ValidateCollation(this.m_dataSetCore.Collation, out lCID))
						{
							this.m_dataSetCore.LCID = lCID;
						}
						else
						{
							this.m_errorContext.Register(ProcessingErrorCode.rsInvalidCollationName, Severity.Warning, context.ObjectType, context.ObjectName, null, this.m_dataSetCore.Collation);
						}
						break;
					}
					case "AccentSensitivity":
						this.m_dataSetCore.AccentSensitivity = this.ReadTriState();
						break;
					case "KanatypeSensitivity":
						this.m_dataSetCore.KanatypeSensitivity = this.ReadTriState();
						break;
					case "WidthSensitivity":
						this.m_dataSetCore.WidthSensitivity = this.ReadTriState();
						break;
					case "Filters":
						this.m_dataSetCore.Filters = this.ReadFilters(AspNetCore.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType.DataSetFilters, context);
						break;
					case "InterpretSubtotalsAsDetails":
						this.m_dataSetCore.InterpretSubtotalsAsDetails = this.ReadTriState();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("DataSet" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (this.m_dataSetCore.Fields == null || this.m_dataSetCore.Fields.Count == 0)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsDataSetWithoutFields, Severity.Error, context.ObjectType, context.ObjectName, null);
			}
			return dataSet;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ReportQuery ReadRSDQuery(PublishingContextStruct context)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ReportQuery reportQuery = new AspNetCore.ReportingServices.ReportIntermediateFormat.ReportQuery();
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource dataSource = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource(this.GenerateID());
			bool flag = false;
			bool flag2 = false;
			Dictionary<string, bool> parametersInQuery = null;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "DataSourceReference":
					{
						dataSource.DataSourceReference = this.m_reader.ReadString();
						AspNetCore.ReportingServices.ReportIntermediateFormat.ReportQuery reportQuery2 = reportQuery;
						AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource dataSource2 = dataSource;
						string text3 = reportQuery2.DataSourceName = (dataSource2.Name = "DataSetDataSource");
						break;
					}
					case "CommandType":
						reportQuery.CommandType = this.ReadCommandType();
						break;
					case "CommandText":
					{
						AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType = context.ObjectType;
						context.ObjectType = AspNetCore.ReportingServices.ReportProcessing.ObjectType.Query;
						reportQuery.CommandText = this.ReadQueryOrParameterExpression(context, DataType.String, ref flag2, parametersInQuery);
						context.ObjectType = objectType;
						break;
					}
					case "DataSetParameters":
						reportQuery.Parameters = this.ReadQueryParameters(context, ref flag2, parametersInQuery);
						SharedDataSetParameterNameMapper.MakeUnique(reportQuery.Parameters);
						break;
					case "Timeout":
						reportQuery.TimeOut = this.m_reader.ReadInteger(context.ObjectType, context.ObjectName, this.m_reader.LocalName);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Query" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (reportQuery.DataSourceName != null)
			{
				DataSourceInfo dataSourceInfo = this.CreateSharedDataSourceLink(context, dataSource);
				if (dataSourceInfo != null)
				{
					if (this.m_publishingContext.ResolveTemporaryDataSourceCallback != null)
					{
						this.m_publishingContext.ResolveTemporaryDataSourceCallback(dataSourceInfo, this.m_publishingContext.OriginalDataSources);
					}
					dataSource.ID = dataSourceInfo.ID;
					this.m_dataSources.Add(dataSourceInfo);
				}
			}
			return reportQuery;
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterValue ReadRSDDataSetParameter(PublishingContextStruct context, ref bool isComplex, Dictionary<string, bool> parametersInQuery)
		{
			Global.Tracer.Assert(AspNetCore.ReportingServices.ReportProcessing.ObjectType.SharedDataSet == context.ObjectType);
			DataSetParameterValue dataSetParameterValue = new DataSetParameterValue();
			dataSetParameterValue.UniqueName = this.m_reader.GetAttribute("UniqueName");
			dataSetParameterValue.Name = this.m_reader.GetAttribute("Name");
			context.ObjectType = AspNetCore.ReportingServices.ReportProcessing.ObjectType.QueryParameter;
			context.ObjectName = dataSetParameterValue.Name;
			if (!this.m_reader.IsEmptyElement)
			{
				bool flag = false;
				do
				{
					this.m_reader.Read();
					switch (this.m_reader.NodeType)
					{
					case XmlNodeType.Element:
						switch (this.m_reader.LocalName)
						{
						case "DefaultValue":
							dataSetParameterValue.ConstantDataType = this.ReadDataTypeAttribute();
							dataSetParameterValue.Value = this.ReadQueryOrParameterExpression(context, dataSetParameterValue.ConstantDataType, ref isComplex, parametersInQuery);
							break;
						case "ReadOnly":
							dataSetParameterValue.ReadOnly = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, "ReadOnly");
							break;
						case "Nullable":
							dataSetParameterValue.Nullable = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, "Nullable");
							break;
						case "OmitFromQuery":
							dataSetParameterValue.OmitFromQuery = this.m_reader.ReadBoolean(context.ObjectType, context.ObjectName, "OmitFromQuery");
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("DataSetParameter" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			if (dataSetParameterValue.ReadOnly && !dataSetParameterValue.Nullable && (dataSetParameterValue.Value == null || dataSetParameterValue.Value.OriginalText == null))
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsMissingDataSetParameterDefault, Severity.Error, context.ObjectType, context.ObjectName, null);
			}
			return dataSetParameterValue;
		}

		private void RSDPhase3(out ParameterInfoCollection parameters)
		{
			try
			{
				this.m_report = new AspNetCore.ReportingServices.ReportIntermediateFormat.Report();
				this.m_reportCT.Builder.SharedDataSetStart();
				AspNetCore.ReportingServices.ReportIntermediateFormat.InitializationContext context = new AspNetCore.ReportingServices.ReportIntermediateFormat.InitializationContext(this.m_publishingContext.CatalogContext, this.m_dataSets, this.m_errorContext, this.m_reportCT.Builder, this.m_report, this.m_reportScopes, this.m_publishingContext);
				context.RSDRegisterDataSetParameters(this.m_dataSetCore);
				context.ExprHostBuilder.DataSetStart(this.m_dataSetCore.Name);
				context.Location |= LocationFlags.InDataSet;
				context.ObjectType = AspNetCore.ReportingServices.ReportProcessing.ObjectType.SharedDataSet;
				context.ObjectName = this.m_dataSetCore.Name;
				this.m_dataSetCore.Initialize(context);
				this.InitializeDataSetParameters(context, out parameters);
				this.m_dataSetCore.ExprHostID = context.ExprHostBuilder.DataSetEnd();
				this.m_reportCT.Builder.SharedDataSetEnd();
				if (!this.m_errorContext.HasError)
				{
					((IExpressionHostAssemblyHolder)this.m_dataSetCore).CompiledCode = this.m_reportCT.Compile(this.m_dataSetCore, this.m_publishingContext.CompilationTempAppDomain, this.m_publishingContext.GenerateExpressionHostWithRefusedPermissions, this.m_publishingContext.PublishingVersioning);
				}
			}
			finally
			{
				this.m_reportCT = null;
			}
		}

		private void InitializeDataSetParameters(AspNetCore.ReportingServices.ReportIntermediateFormat.InitializationContext context, out ParameterInfoCollection parameters)
		{
			parameters = new ParameterInfoCollection();
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterValue> parameters2 = this.m_dataSetCore.Query.Parameters;
			if (parameters2 != null && parameters2.Count > 0)
			{
				foreach (DataSetParameterValue item in parameters2)
				{
					bool usedInQuery = true;
					AspNetCore.ReportingServices.ReportProcessing.ParameterInfo parameterInfo = new AspNetCore.ReportingServices.ReportProcessing.ParameterInfo(item, usedInQuery);
					parameters.Add(parameterInfo);
					if (item.Value != null && item.Value.IsExpression)
					{
						parameterInfo.DynamicDefaultValue = true;
					}
				}
			}
		}
	}
}
