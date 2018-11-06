using AspNetCore.ReportingServices.DataExtensions;
using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.ReportPublishing;
using AspNetCore.ReportingServices.ReportRendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class ReportPublishing
	{
		private enum StyleOwnerType
		{
			Line = 1,
			Rectangle,
			Checkbox,
			Image,
			ActiveXControl,
			List,
			Matrix,
			Table,
			OWCChart,
			Body,
			Chart,
			Textbox,
			SubReport,
			Subtotal,
			PageSection
		}

		private struct PublishingContextStruct
		{
			private LocationFlags m_location;

			private ObjectType m_objectType;

			private string m_objectName;

			internal LocationFlags Location
			{
				get
				{
					return this.m_location;
				}
				set
				{
					this.m_location = value;
				}
			}

			internal ObjectType ObjectType
			{
				get
				{
					return this.m_objectType;
				}
				set
				{
					this.m_objectType = value;
				}
			}

			internal string ObjectName
			{
				get
				{
					return this.m_objectName;
				}
				set
				{
					this.m_objectName = value;
				}
			}

			internal PublishingContextStruct(LocationFlags location, ObjectType objectType, string objectName)
			{
				this.m_location = location;
				this.m_objectType = objectType;
				this.m_objectName = objectName;
			}

			internal ExpressionParser.ExpressionContext CreateExpressionContext(ExpressionParser.ExpressionType expressionType, ExpressionParser.ConstantType constantType, string propertyName, string dataSetName)
			{
				return new ExpressionParser.ExpressionContext(expressionType, constantType, this.m_location, this.m_objectType, this.m_objectName, propertyName, dataSetName, false);
			}

			internal ExpressionParser.ExpressionContext CreateExpressionContext(ExpressionParser.ExpressionType expressionType, ExpressionParser.ConstantType constantType, string propertyName, string dataSetName, bool parseExtended)
			{
				return new ExpressionParser.ExpressionContext(expressionType, constantType, this.m_location, this.m_objectType, this.m_objectName, propertyName, dataSetName, parseExtended);
			}
		}

		private sealed class StyleInformation
		{
			private StringList m_names = new StringList();

			private ExpressionInfoList m_values = new ExpressionInfoList();

			private static Hashtable StyleNameIndexes;

			private static bool[,] AllowStyleAttributeByType;

			internal StringList Names
			{
				get
				{
					return this.m_names;
				}
			}

			internal ExpressionInfoList Values
			{
				get
				{
					return this.m_values;
				}
			}

			static StyleInformation()
			{
				StyleInformation.AllowStyleAttributeByType = new bool[43, 12]
				{
					{
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true
					},
					{
						true,
						false,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true
					},
					{
						true,
						false,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true
					},
					{
						true,
						false,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true
					},
					{
						true,
						false,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true
					},
					{
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true
					},
					{
						true,
						false,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true
					},
					{
						true,
						false,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true
					},
					{
						true,
						false,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true
					},
					{
						true,
						false,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true
					},
					{
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true
					},
					{
						true,
						false,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true
					},
					{
						true,
						false,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true
					},
					{
						true,
						false,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true
					},
					{
						true,
						false,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true,
						true
					},
					{
						true,
						false,
						true,
						false,
						false,
						false,
						true,
						true,
						true,
						false,
						true,
						true
					},
					{
						true,
						false,
						true,
						false,
						false,
						false,
						true,
						true,
						true,
						false,
						true,
						true
					},
					{
						true,
						false,
						true,
						false,
						false,
						false,
						true,
						true,
						true,
						false,
						true,
						true
					},
					{
						true,
						false,
						true,
						false,
						false,
						false,
						true,
						true,
						true,
						false,
						true,
						true
					},
					{
						true,
						false,
						true,
						false,
						false,
						false,
						true,
						true,
						true,
						false,
						true,
						true
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						true
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						true
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						true
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						true
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						true
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						true
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						true
					},
					{
						true,
						false,
						false,
						false,
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false
					},
					{
						true,
						false,
						false,
						false,
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false
					},
					{
						true,
						false,
						false,
						false,
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false
					},
					{
						true,
						false,
						false,
						false,
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						true
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						true
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						true
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						true
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						true
					},
					{
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						true
					},
					{
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						true
					},
					{
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						false,
						true
					}
				};
				StyleInformation.StyleNameIndexes = new Hashtable();
				StyleInformation.StyleNameIndexes.Add("BorderColor", 0);
				StyleInformation.StyleNameIndexes.Add("BorderColorLeft", 1);
				StyleInformation.StyleNameIndexes.Add("BorderColorRight", 2);
				StyleInformation.StyleNameIndexes.Add("BorderColorTop", 3);
				StyleInformation.StyleNameIndexes.Add("BorderColorBottom", 4);
				StyleInformation.StyleNameIndexes.Add("BorderStyle", 5);
				StyleInformation.StyleNameIndexes.Add("BorderStyleLeft", 6);
				StyleInformation.StyleNameIndexes.Add("BorderStyleRight", 7);
				StyleInformation.StyleNameIndexes.Add("BorderStyleTop", 8);
				StyleInformation.StyleNameIndexes.Add("BorderStyleBottom", 9);
				StyleInformation.StyleNameIndexes.Add("BorderWidth", 10);
				StyleInformation.StyleNameIndexes.Add("BorderWidthLeft", 11);
				StyleInformation.StyleNameIndexes.Add("BorderWidthRight", 12);
				StyleInformation.StyleNameIndexes.Add("BorderWidthTop", 13);
				StyleInformation.StyleNameIndexes.Add("BorderWidthBottom", 14);
				StyleInformation.StyleNameIndexes.Add("BackgroundColor", 15);
				StyleInformation.StyleNameIndexes.Add("BackgroundImageSource", 16);
				StyleInformation.StyleNameIndexes.Add("BackgroundImageValue", 17);
				StyleInformation.StyleNameIndexes.Add("BackgroundImageMIMEType", 18);
				StyleInformation.StyleNameIndexes.Add("BackgroundRepeat", 19);
				StyleInformation.StyleNameIndexes.Add("FontStyle", 20);
				StyleInformation.StyleNameIndexes.Add("FontFamily", 21);
				StyleInformation.StyleNameIndexes.Add("FontSize", 22);
				StyleInformation.StyleNameIndexes.Add("FontWeight", 23);
				StyleInformation.StyleNameIndexes.Add("Format", 24);
				StyleInformation.StyleNameIndexes.Add("TextDecoration", 25);
				StyleInformation.StyleNameIndexes.Add("TextAlign", 26);
				StyleInformation.StyleNameIndexes.Add("VerticalAlign", 27);
				StyleInformation.StyleNameIndexes.Add("Color", 28);
				StyleInformation.StyleNameIndexes.Add("PaddingLeft", 29);
				StyleInformation.StyleNameIndexes.Add("PaddingRight", 30);
				StyleInformation.StyleNameIndexes.Add("PaddingTop", 31);
				StyleInformation.StyleNameIndexes.Add("PaddingBottom", 32);
				StyleInformation.StyleNameIndexes.Add("LineHeight", 33);
				StyleInformation.StyleNameIndexes.Add("Direction", 34);
				StyleInformation.StyleNameIndexes.Add("Language", 35);
				StyleInformation.StyleNameIndexes.Add("UnicodeBiDi", 36);
				StyleInformation.StyleNameIndexes.Add("Calendar", 37);
				StyleInformation.StyleNameIndexes.Add("NumeralLanguage", 38);
				StyleInformation.StyleNameIndexes.Add("NumeralVariant", 39);
				StyleInformation.StyleNameIndexes.Add("WritingMode", 40);
				StyleInformation.StyleNameIndexes.Add("BackgroundGradientType", 41);
				StyleInformation.StyleNameIndexes.Add("BackgroundGradientEndColor", 42);
			}

			internal void AddAttribute(string name, ExpressionInfo expression)
			{
				Global.Tracer.Assert(null != name);
				Global.Tracer.Assert(null != expression);
				this.m_names.Add(name);
				this.m_values.Add(expression);
			}

			internal void Filter(StyleOwnerType ownerType, bool hasNoRows)
			{
				Global.Tracer.Assert(this.m_names.Count == this.m_values.Count);
				int ownerType2 = this.MapStyleOwnerTypeToIndex(ownerType, hasNoRows);
				for (int num = this.m_names.Count - 1; num >= 0; num--)
				{
					if (!this.Allow(this.MapStyleNameToIndex(this.m_names[num]), ownerType2))
					{
						this.m_names.RemoveAt(num);
						this.m_values.RemoveAt(num);
					}
				}
			}

			private int MapStyleOwnerTypeToIndex(StyleOwnerType ownerType, bool hasNoRows)
			{
				if (hasNoRows)
				{
					return 0;
				}
				switch (ownerType)
				{
				case StyleOwnerType.PageSection:
					return 2;
				case StyleOwnerType.Textbox:
				case StyleOwnerType.SubReport:
				case StyleOwnerType.Subtotal:
					return 0;
				default:
					return (int)ownerType;
				}
			}

			private int MapStyleNameToIndex(string name)
			{
				return (int)StyleInformation.StyleNameIndexes[name];
			}

			private bool Allow(int styleName, int ownerType)
			{
				return StyleInformation.AllowStyleAttributeByType[styleName, ownerType];
			}
		}

		private sealed class RmlValidatingReader : RDLValidatingReader
		{
			internal enum CustomFlags
			{
				None,
				InCustomElement,
				AfterCustomElement
			}

			private const string XsdResourceID = "AspNetCore.ReportingServices.ReportProcessing.ReportDefinition.xsd";

			private CustomFlags m_custom;

			private PublishingErrorContext m_errorContext;

			private string m_targetRDLNamespace;

			private RmlValidatingReader(XmlTextReader textReader, PublishingErrorContext errorContext, string targetRDLNamespace)
				: base(textReader, targetRDLNamespace)
			{
				base.Schemas.Add(XmlSchema.Read(Assembly.GetExecutingAssembly().GetManifestResourceStream("AspNetCore.ReportingServices.ReportProcessing.ReportDefinition.xsd"), null));
				base.ValidationEventHandler += this.ValidationCallBack;
				base.ValidationType = ValidationType.Schema;
				this.m_errorContext = errorContext;
				this.m_targetRDLNamespace = targetRDLNamespace;
			}

			public override bool Read()
			{
				try
				{
					if (CustomFlags.AfterCustomElement != this.m_custom)
					{
						base.Read();
						string text = default(string);
						if (!base.Validate(out text))
						{
							this.m_errorContext.Register(ProcessingErrorCode.rsInvalidReportDefinition, Severity.Error, ObjectType.Report, null, null, text);
							throw new ReportProcessingException(this.m_errorContext.Messages);
						}
					}
					else
					{
						this.m_custom = CustomFlags.None;
					}
					if (CustomFlags.InCustomElement != this.m_custom)
					{
						while (!base.EOF && XmlNodeType.Element == base.NodeType && this.m_targetRDLNamespace != base.NamespaceURI)
						{
							this.Skip();
						}
					}
					return !base.EOF;
				}
				catch (ArgumentException ex)
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsInvalidReportDefinition, Severity.Error, ObjectType.Report, null, null, ex.Message);
					throw new ReportProcessingException(this.m_errorContext.Messages);
				}
			}

			public override string ReadString()
			{
				if (base.IsEmptyElement)
				{
					return string.Empty;
				}
				return base.ReadString();
			}

			internal static RmlValidatingReader CreateReader(XmlTextReader upgradedRDLReader, PublishingErrorContext errorContext, string targetRDLNamespace)
			{
				Global.Tracer.Assert(null != upgradedRDLReader);
				upgradedRDLReader.WhitespaceHandling = WhitespaceHandling.None;
				upgradedRDLReader.XmlResolver = new XmlNullResolver();
				return new RmlValidatingReader(upgradedRDLReader, errorContext, targetRDLNamespace);
			}

			internal bool ReadBoolean()
			{
				if (base.IsEmptyElement)
				{
					Global.Tracer.Assert(false);
					return false;
				}
				return XmlConvert.ToBoolean(base.ReadString());
			}

			internal int ReadInteger()
			{
				if (base.IsEmptyElement)
				{
					Global.Tracer.Assert(false);
					return 0;
				}
				return XmlConvert.ToInt32(base.ReadString());
			}

			internal string ReadCustomXml()
			{
				Global.Tracer.Assert(CustomFlags.None == this.m_custom);
				if (base.IsEmptyElement)
				{
					return string.Empty;
				}
				this.m_custom = CustomFlags.InCustomElement;
				string result = base.ReadInnerXml();
				this.m_custom = CustomFlags.AfterCustomElement;
				return result;
			}

			private void ValidationCallBack(object sender, ValidationEventArgs args)
			{
				if (ReportProcessing.CompareWithInvariantCulture(this.m_targetRDLNamespace, base.NamespaceURI, false) == 0)
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsInvalidReportDefinition, Severity.Error, ObjectType.Report, null, null, args.Message);
					throw new ReportProcessingException(this.m_errorContext.Messages);
				}
				XmlNodeType nodeType = base.NodeType;
			}
		}

		private sealed class XmlNullResolver : XmlUrlResolver
		{
			public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
			{
				throw new XmlException("Can't resolve URI reference.", null);
			}
		}

		private sealed class AllowNullKeyHashtable
		{
			private Hashtable m_hashtable = new Hashtable();

			private object m_nullValue;

			internal object this[string name]
			{
				get
				{
					if (name == null)
					{
						return this.m_nullValue;
					}
					return this.m_hashtable[name];
				}
				set
				{
					if (name == null)
					{
						this.m_nullValue = value;
					}
					else
					{
						this.m_hashtable[name] = value;
					}
				}
			}
		}

		private bool m_static;

		private bool m_interactive;

		private int m_idCounter;

		private RmlValidatingReader m_reader;

		private CLSUniqueNameValidator m_reportItemNames;

		private ScopeNameValidator m_scopeNames;

		private ImageStreamNames m_imageStreamNames;

		private ICatalogItemContext m_reportContext;

		private ReportProcessing.CreateReportChunk m_createChunkCallback;

		private ReportProcessing.CheckSharedDataSource m_checkDataSourceCallback;

		private string m_description;

		private DataSourceInfoCollection m_dataSources;

		private SubReportList m_subReports;

		private UserLocationFlags m_reportLocationFlags = UserLocationFlags.ReportBody;

		private UserLocationFlags m_userReferenceLocation = UserLocationFlags.None;

		private bool m_hasExternalImages;

		private bool m_hasHyperlinks;

		private bool m_pageSectionDrillthroughs;

		private bool m_hasGrouping;

		private bool m_hasSorting;

		private bool m_hasUserSort;

		private bool m_hasGroupFilters;

		private bool m_hasSpecialRecursiveAggregates;

		private bool m_aggregateInDetailSections;

		private bool m_subReportMergeTransactions;

		private ReportCompileTime m_reportCT;

		private bool m_hasImageStreams;

		private bool m_hasLabels;

		private bool m_hasBookmarks;

		private TextBoxList m_textBoxesWithUserSortTarget = new TextBoxList();

		private bool m_hasFilters;

		private DataSetList m_dataSets = new DataSetList();

		private bool m_parametersNotUsedInQuery = true;

		private Hashtable m_usedInQueryInfos = new Hashtable();

		private Hashtable m_reportParamUserProfile = new Hashtable();

		private Hashtable m_dataSetQueryInfo = new Hashtable();

		private ArrayList m_dynamicParameters = new ArrayList();

		private CultureInfo m_reportLanguage;

		private bool m_hasUserSortPeerScopes;

		private Hashtable m_reportScopes = new Hashtable();

		private StringDictionary m_dataSourceNames = new StringDictionary();

		private int m_dataRegionCount;

		private ArrayList m_reportItemCollectionList = new ArrayList();

		private ArrayList m_aggregateHolderList = new ArrayList();

		private ArrayList m_runningValueHolderList = new ArrayList();

		private string m_targetRDLNamespace;

		private Report m_report;

		private PublishingErrorContext m_errorContext;

		internal Report CreateIntermediateFormat(ICatalogItemContext reportContext, byte[] definition, ReportProcessing.CreateReportChunk createChunkCallback, ReportProcessing.CheckSharedDataSource checkDataSourceCallback, ReportProcessing.ResolveTemporaryDataSource resolveTemporaryDataSourceCallback, DataSourceInfoCollection originalDataSources, PublishingErrorContext errorContext, AppDomain compilationTempAppDomain, bool generateExpressionHostWithRefusedPermissions, IDataProtection dataProtection, out string description, out string language, out ParameterInfoCollection parameters, out DataSourceInfoCollection dataSources, out UserLocationFlags userReferenceLocation, out ArrayList dataSetsName, out bool hasExternalImages, out bool hasHyperlinks)
		{
			try
			{
				this.m_report = null;
				this.m_errorContext = errorContext;
				if (definition == null)
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsNotAReportDefinition, Severity.Error, ObjectType.Report, null, null);
					throw new ReportProcessingException(this.m_errorContext.Messages);
				}
				this.Phase1(reportContext, definition, createChunkCallback, checkDataSourceCallback, resolveTemporaryDataSourceCallback, originalDataSources, dataProtection, out description, out language, out dataSources, out userReferenceLocation, out hasExternalImages, out hasHyperlinks);
				this.Phase2();
				this.Phase3(reportContext, out parameters, compilationTempAppDomain, generateExpressionHostWithRefusedPermissions);
				this.Phase4();
				if (this.m_errorContext.HasError)
				{
					throw new ReportProcessingException(this.m_errorContext.Messages);
				}
				ReportPublishing.CalculateChildrenPostions(this.m_report);
				ReportPublishing.CalculateChildrenDependencies(this.m_report);
				dataSetsName = null;
				for (int i = 0; i < this.m_dataSets.Count; i++)
				{
					if (!this.m_dataSets[i].UsedOnlyInParameters)
					{
						if (dataSetsName == null)
						{
							dataSetsName = new ArrayList();
						}
						dataSetsName.Add(this.m_dataSets[i].Name);
					}
				}
				return this.m_report;
			}
			finally
			{
				this.m_report = null;
				this.m_errorContext = null;
			}
		}

		private int GenerateID()
		{
			return ++this.m_idCounter;
		}

		private void Phase1(ICatalogItemContext reportContext, byte[] definition, ReportProcessing.CreateReportChunk createChunkCallback, ReportProcessing.CheckSharedDataSource checkDataSourceCallback, ReportProcessing.ResolveTemporaryDataSource resolveTemporaryDataSourceCallback, DataSourceInfoCollection originalDataSources, IDataProtection dataProtection, out string description, out string language, out DataSourceInfoCollection dataSources, out UserLocationFlags userReferenceLocation, out bool hasExternalImages, out bool hasHyperlinks)
		{
			try
			{
				XmlTextReader xmlTextReader = new XmlTextReader(new MemoryStream(definition, false));
				XmlUtil.ApplyDtdDosDefense(xmlTextReader);
				this.m_reader = RmlValidatingReader.CreateReader(xmlTextReader, this.m_errorContext, "http://schemas.microsoft.com/sqlserver/reporting/2005/01/reportdefinition");
				this.m_reportItemNames = new CLSUniqueNameValidator(ProcessingErrorCode.rsInvalidNameNotCLSCompliant, ProcessingErrorCode.rsDuplicateReportItemName);
				this.m_scopeNames = new ScopeNameValidator();
				this.m_imageStreamNames = new ImageStreamNames();
				this.m_reportContext = reportContext;
				this.m_createChunkCallback = createChunkCallback;
				this.m_checkDataSourceCallback = checkDataSourceCallback;
				this.m_dataSources = new DataSourceInfoCollection();
				this.m_subReports = new SubReportList();
				while (this.m_reader.Read())
				{
					if (XmlNodeType.Element == this.m_reader.NodeType && "Report" == this.m_reader.LocalName)
					{
						this.m_reportCT = new ReportCompileTime(new VBExpressionParser(this.m_errorContext), this.m_errorContext);
						this.m_report = this.ReadReport(resolveTemporaryDataSourceCallback, originalDataSources, dataProtection);
					}
				}
				if (this.m_report == null)
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsNotACurrentReportDefinition, Severity.Error, ObjectType.Report, null, "Namespace", this.m_targetRDLNamespace);
					throw new ReportProcessingException(this.m_errorContext.Messages);
				}
			}
			catch (XmlException ex)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsInvalidReportDefinition, Severity.Error, ObjectType.Report, null, null, ex.Message);
				throw new ReportProcessingException(this.m_errorContext.Messages);
			}
			finally
			{
				if (this.m_reader != null)
				{
					this.m_reader.Close();
					this.m_reader = null;
				}
				this.m_reportItemNames = null;
				this.m_scopeNames = null;
				this.m_imageStreamNames = null;
				this.m_reportContext = null;
				this.m_createChunkCallback = null;
				this.m_checkDataSourceCallback = null;
				description = this.m_description;
				language = null;
				if (this.m_reportLanguage != null)
				{
					language = this.m_reportLanguage.Name;
				}
				dataSources = this.m_dataSources;
				userReferenceLocation = this.m_userReferenceLocation;
				hasExternalImages = this.m_hasExternalImages;
				hasHyperlinks = this.m_hasHyperlinks;
				this.m_description = null;
				this.m_dataSources = null;
			}
		}

		private Report ReadReport(ReportProcessing.ResolveTemporaryDataSource resolveTemporaryDataSourceCallback, DataSourceInfoCollection originalDataSources, IDataProtection dataProtection)
		{
			Report report = new Report(this.GenerateID(), this.GenerateID());
			PublishingContextStruct context = new PublishingContextStruct(LocationFlags.None, report.ObjectType, null);
			ExpressionInfo expressionInfo = null;
			this.m_reportItemCollectionList.Add(report.ReportItems);
			this.m_aggregateHolderList.Add(report);
			this.m_runningValueHolderList.Add(report.ReportItems);
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
						report.AutoRefresh = this.m_reader.ReadInteger();
						break;
					case "DataSources":
						report.DataSources = this.ReadDataSources(context, resolveTemporaryDataSourceCallback, originalDataSources, dataProtection);
						break;
					case "DataSets":
						this.ReadDataSets(context);
						break;
					case "Body":
						this.ReadBody(report, context);
						break;
					case "ReportParameters":
						report.Parameters = this.ReadReportParameters(context);
						break;
					case "Custom":
						report.Custom = this.m_reader.ReadCustomXml();
						break;
					case "CustomProperties":
						report.CustomProperties = this.ReadCustomProperties(context);
						break;
					case "Code":
						report.Code = this.m_reader.ReadString();
						this.m_reportCT.Builder.SetCustomCode();
						break;
					case "Width":
						report.Width = this.ReadSize();
						break;
					case "PageHeader":
						report.PageHeader = this.ReadPageSection(true, report, context);
						break;
					case "PageFooter":
						report.PageFooter = this.ReadPageSection(false, report, context);
						break;
					case "PageHeight":
						report.PageHeight = this.ReadSize();
						break;
					case "PageWidth":
						report.PageWidth = this.ReadSize();
						break;
					case "InteractiveHeight":
						report.InteractiveHeight = this.ReadSize();
						break;
					case "InteractiveWidth":
						report.InteractiveWidth = this.ReadSize();
						break;
					case "LeftMargin":
						report.LeftMargin = this.ReadSize();
						break;
					case "RightMargin":
						report.RightMargin = this.ReadSize();
						break;
					case "TopMargin":
						report.TopMargin = this.ReadSize();
						break;
					case "BottomMargin":
						report.BottomMargin = this.ReadSize();
						break;
					case "EmbeddedImages":
						report.EmbeddedImages = this.ReadEmbeddedImages(context);
						break;
					case "Language":
						expressionInfo = (report.Language = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.ReportLanguage, ExpressionParser.ConstantType.String, context));
						break;
					case "CodeModules":
						report.CodeModules = this.ReadCodeModules(context);
						break;
					case "Classes":
						report.CodeClasses = this.ReadClasses(context);
						break;
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
			if (expressionInfo == null)
			{
				this.m_reportLanguage = Localization.DefaultReportServerSpecificCulture;
			}
			else if (ExpressionInfo.Types.Constant == expressionInfo.Type)
			{
				PublishingValidator.ValidateSpecificLanguage(expressionInfo, ObjectType.Report, (string)null, "Language", (ErrorContext)this.m_errorContext, out this.m_reportLanguage);
			}
			if (this.m_interactive)
			{
				report.ShowHideType = Report.ShowHideTypes.Interactive;
			}
			else if (this.m_static)
			{
				report.ShowHideType = Report.ShowHideTypes.Static;
			}
			else
			{
				report.ShowHideType = Report.ShowHideTypes.None;
			}
			report.ImageStreamNames = this.m_imageStreamNames;
			report.SubReports = this.m_subReports;
			report.BodyID = this.GenerateID();
			report.LastID = this.m_idCounter;
			return report;
		}

		private EmbeddedImageHashtable ReadEmbeddedImages(PublishingContextStruct context)
		{
			EmbeddedImageHashtable embeddedImageHashtable = new EmbeddedImageHashtable();
			CLSUniqueNameValidator embeddedImageNames = new CLSUniqueNameValidator(ProcessingErrorCode.rsInvalidNameNotCLSCompliant, ProcessingErrorCode.rsDuplicateEmbeddedImageName);
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
						this.ReadEmbeddedImage(embeddedImageHashtable, embeddedImageNames, context);
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
			return embeddedImageHashtable;
		}

		private void ReadEmbeddedImage(EmbeddedImageHashtable embeddedImages, CLSUniqueNameValidator embeddedImageNames, PublishingContextStruct context)
		{
			string attribute = this.m_reader.GetAttribute("Name");
			context.ObjectType = ObjectType.EmbeddedImage;
			context.ObjectName = attribute;
			embeddedImageNames.Validate(context.ObjectType, context.ObjectName, this.m_errorContext);
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
						if (!PublishingValidator.ValidateMimeType(text, context.ObjectType, context.ObjectName, this.m_reader.LocalName, this.m_errorContext))
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
							this.m_errorContext.Register(ProcessingErrorCode.rsInvalidEmbeddedImage, Severity.Error, context.ObjectType, context.ObjectName, "ImageData");
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
			embeddedImages.Add(attribute, new ImageInfo(text2, text));
			if (array != null && text != null && this.m_createChunkCallback != null)
			{
				using (Stream stream = this.m_createChunkCallback(text2, ReportProcessing.ReportChunkTypes.Image, text))
				{
					stream.Write(array, 0, array.Length);
				}
			}
		}

		private DataSourceList ReadDataSources(PublishingContextStruct context, ReportProcessing.ResolveTemporaryDataSource resolveTemporaryDataSourceCallback, DataSourceInfoCollection originalDataSources, IDataProtection dataProtection)
		{
			DataSourceList dataSourceList = new DataSourceList();
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
						dataSourceList.Add(this.ReadDataSource(dataSourceNames, context, resolveTemporaryDataSourceCallback, originalDataSources, dataProtection));
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
			return dataSourceList;
		}

		private DataSource ReadDataSource(DataSourceNameValidator dataSourceNames, PublishingContextStruct context, ReportProcessing.ResolveTemporaryDataSource resolveTemporaryDataSourceCallback, DataSourceInfoCollection originalDataSources, IDataProtection dataProtection)
		{
			DataSource dataSource = new DataSource();
			dataSource.Name = this.m_reader.GetAttribute("Name");
			context.ObjectType = ObjectType.DataSource;
			context.ObjectName = dataSource.Name;
			bool flag = false;
			if (dataSourceNames.Validate(context.ObjectType, context.ObjectName, this.m_errorContext))
			{
				flag = true;
			}
			bool flag2 = false;
			bool flag3 = false;
			bool isComplex = false;
			StringList parameterNames = null;
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
							dataSource.Transaction = this.m_reader.ReadBoolean();
							break;
						case "ConnectionProperties":
							flag2 = true;
							this.ReadConnectionProperties(dataSource, context, ref isComplex, ref parameterNames);
							break;
						case "DataSourceReference":
							flag3 = true;
							dataSource.DataSourceReference = this.m_reader.ReadString();
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
				goto IL_0139;
			}
			if (flag3 && flag2)
			{
				goto IL_0139;
			}
			goto IL_015f;
			IL_015f:
			if (flag && !this.m_dataSourceNames.ContainsKey(dataSource.Name))
			{
				this.m_dataSourceNames.Add(dataSource.Name, null);
			}
			DataSourceInfo dataSourceInfo = null;
			if (flag2)
			{
				dataSource.IsComplex = isComplex;
				dataSource.ParameterNames = parameterNames;
				bool flag5 = false;
				if (dataSource.ConnectStringExpression.Type != ExpressionInfo.Types.Constant)
				{
					flag5 = true;
				}
				dataSourceInfo = new DataSourceInfo(dataSource.Name, dataSource.Type, flag5 ? null : dataSource.ConnectStringExpression.OriginalText, flag5, dataSource.IntegratedSecurity, dataSource.Prompt, dataProtection);
			}
			else if (flag3)
			{
				string text = (this.m_reportContext == null) ? dataSource.DataSourceReference : this.m_reportContext.MapUserProvidedPath(dataSource.DataSourceReference);
				if (this.m_checkDataSourceCallback == null)
				{
					dataSourceInfo = new DataSourceInfo(dataSource.Name, text, Guid.Empty);
				}
				else
				{
					Guid empty = Guid.Empty;
					DataSourceInfo dataSourceInfo2 = this.m_checkDataSourceCallback(text, out empty);
					if (dataSourceInfo2 == null)
					{
						dataSourceInfo = new DataSourceInfo(dataSource.Name);
						this.m_errorContext.Register(ProcessingErrorCode.rsDataSourceReferenceNotPublished, Severity.Warning, context.ObjectType, context.ObjectName, "Report", dataSource.Name);
					}
					else
					{
						dataSourceInfo = new DataSourceInfo(dataSource.Name, text, empty, dataSourceInfo2);
					}
				}
			}
			if (dataSourceInfo != null)
			{
				if (resolveTemporaryDataSourceCallback != null)
				{
					resolveTemporaryDataSourceCallback(dataSourceInfo, originalDataSources);
				}
				dataSource.ID = dataSourceInfo.ID;
				this.m_dataSources.Add(dataSourceInfo);
			}
			return dataSource;
			IL_0139:
			flag = false;
			this.m_errorContext.Register(ProcessingErrorCode.rsInvalidDataSource, Severity.Error, context.ObjectType, context.ObjectName, null);
			goto IL_015f;
		}

		private StringList ReadCodeModules(PublishingContextStruct context)
		{
			StringList stringList = new StringList();
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
						stringList.Add(this.m_reader.ReadString());
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
			return stringList;
		}

		private CodeClassList ReadClasses(PublishingContextStruct context)
		{
			CodeClassList codeClassList = new CodeClassList();
			CLSUniqueNameValidator instanceNameValidator = new CLSUniqueNameValidator(ProcessingErrorCode.rsInvalidNameNotCLSCompliant, ProcessingErrorCode.rsDuplicateClassInstanceName);
			context.ObjectType = ObjectType.CodeClass;
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
						this.ReadClass(codeClassList, instanceNameValidator, context);
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
			return codeClassList;
		}

		private void ReadClass(CodeClassList codeClasses, CLSUniqueNameValidator instanceNameValidator, PublishingContextStruct context)
		{
			bool flag = false;
			CodeClass codeClass = default(CodeClass);
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "ClassName":
						codeClass.ClassName = this.m_reader.ReadString();
						break;
					case "InstanceName":
						codeClass.InstanceName = this.m_reader.ReadString();
						if (!instanceNameValidator.Validate(context.ObjectType, codeClass.InstanceName, this.m_errorContext))
						{
							codeClass.InstanceName = null;
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
			codeClasses.Add(codeClass);
		}

		private void ReadConnectionProperties(DataSource dataSource, PublishingContextStruct context, ref bool hasComplexParams, ref StringList parametersInQuery)
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
						Global.Tracer.Assert(ObjectType.DataSource == context.ObjectType);
						dataSource.ConnectStringExpression = this.ReadQueryOrParameterExpression(context, ref hasComplexParams, ref parametersInQuery);
						break;
					case "IntegratedSecurity":
						dataSource.IntegratedSecurity = this.m_reader.ReadBoolean();
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
						this.m_dataSets.Add(this.ReadDataSet(context));
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

		private DataSet ReadDataSet(PublishingContextStruct context)
		{
			DataSet dataSet = new DataSet(this.GenerateID());
			YukonDataSetInfo yukonDataSetInfo = null;
			dataSet.Name = this.m_reader.GetAttribute("Name");
			context.Location |= LocationFlags.InDataSet;
			context.ObjectType = dataSet.ObjectType;
			context.ObjectName = dataSet.Name;
			if (this.m_scopeNames.Validate(false, context.ObjectName, context.ObjectType, context.ObjectName, this.m_errorContext))
			{
				this.m_reportScopes.Add(dataSet.Name, dataSet);
			}
			this.m_aggregateHolderList.Add(dataSet);
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
						dataSet.Query = this.ReadQuery(context, out yukonDataSetInfo);
						break;
					case "CaseSensitivity":
						dataSet.CaseSensitivity = this.ReadSensitivity();
						break;
					case "Collation":
					{
						dataSet.Collation = this.m_reader.ReadString();
						uint lCID = default(uint);
						if (DataSetValidator.ValidateCollation(dataSet.Collation, out lCID))
						{
							dataSet.LCID = lCID;
						}
						break;
					}
					case "AccentSensitivity":
						dataSet.AccentSensitivity = this.ReadSensitivity();
						break;
					case "KanatypeSensitivity":
						dataSet.KanatypeSensitivity = this.ReadSensitivity();
						break;
					case "WidthSensitivity":
						dataSet.WidthSensitivity = this.ReadSensitivity();
						break;
					case "Filters":
						dataSet.Filters = this.ReadFilters(ExpressionParser.ExpressionType.DataSetFilters, context);
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
			if (yukonDataSetInfo != null && !this.m_dataSetQueryInfo.ContainsKey(context.ObjectName))
			{
				this.m_dataSetQueryInfo.Add(context.ObjectName, yukonDataSetInfo);
				int num = (dataSet.Fields != null) ? dataSet.Fields.Count : 0;
				while (num > 0 && dataSet.Fields[num - 1].IsCalculatedField)
				{
					num--;
				}
				yukonDataSetInfo.CalculatedFieldIndex = num;
			}
			return dataSet;
		}

		private ReportQuery ReadQuery(PublishingContextStruct context, out YukonDataSetInfo queryDataSetInfo)
		{
			ReportQuery reportQuery = new ReportQuery();
			bool flag = false;
			bool isComplex = false;
			StringList parameterNames = null;
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
						Global.Tracer.Assert(ObjectType.DataSet == context.ObjectType);
						context.ObjectType = ObjectType.Query;
						reportQuery.CommandText = this.ReadQueryOrParameterExpression(context, ref isComplex, ref parameterNames);
						context.ObjectType = ObjectType.DataSet;
						break;
					case "QueryParameters":
						reportQuery.Parameters = this.ReadQueryParameters(context, ref isComplex, ref parameterNames);
						break;
					case "Timeout":
						reportQuery.TimeOut = this.m_reader.ReadInteger();
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
			queryDataSetInfo = new YukonDataSetInfo(this.m_dataSets.Count, isComplex, parameterNames);
			return reportQuery;
		}

		private ExpressionInfo ReadQueryOrParameterExpression(PublishingContextStruct context, ref bool isComplex, ref StringList parametersInQuery)
		{
			ExpressionInfo expressionInfo = null;
			Global.Tracer.Assert(ObjectType.QueryParameter == context.ObjectType || ObjectType.Query == context.ObjectType || ObjectType.DataSource == context.ObjectType);
			ExpressionParser.DetectionFlags detectionFlags = (ExpressionParser.DetectionFlags)0;
			if (this.m_parametersNotUsedInQuery || !isComplex)
			{
				detectionFlags |= ExpressionParser.DetectionFlags.ParameterReference;
			}
			this.m_reportLocationFlags = UserLocationFlags.ReportQueries;
			bool flag = default(bool);
			string text = default(string);
			expressionInfo = this.ReadExpression(this.m_reader.LocalName, context.ObjectName, ExpressionParser.ExpressionType.QueryParameter, ExpressionParser.ConstantType.String, context, detectionFlags, out flag, out text);
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
						if (parametersInQuery == null)
						{
							parametersInQuery = new StringList();
						}
						parametersInQuery.Add(text);
					}
				}
			}
			this.m_reportLocationFlags = UserLocationFlags.ReportBody;
			return expressionInfo;
		}

		private ParameterValueList ReadQueryParameters(PublishingContextStruct context, ref bool hasComplexParams, ref StringList parametersInQuery)
		{
			ParameterValueList parameterValueList = new ParameterValueList();
			bool flag = false;
			string objectName = context.ObjectName;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "QueryParameter")
					{
						parameterValueList.Add(this.ReadQueryParameter(context, ref hasComplexParams, ref parametersInQuery));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("QueryParameters" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			context.ObjectName = objectName;
			return parameterValueList;
		}

		private ParameterValue ReadQueryParameter(PublishingContextStruct context, ref bool isComplex, ref StringList parametersInQuery)
		{
			Global.Tracer.Assert(ObjectType.DataSet == context.ObjectType);
			ParameterValue parameterValue = new ParameterValue();
			parameterValue.Name = this.m_reader.GetAttribute("Name");
			context.ObjectType = ObjectType.QueryParameter;
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
						parameterValue.Value = this.ReadQueryOrParameterExpression(context, ref isComplex, ref parametersInQuery);
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

		private DataFieldList ReadFields(PublishingContextStruct context, out int calculatedFieldStartIndex)
		{
			DataFieldList dataFieldList = new DataFieldList();
			CLSUniqueNameValidator names = new CLSUniqueNameValidator(ProcessingErrorCode.rsInvalidFieldNameNotCLSCompliant, ProcessingErrorCode.rsDuplicateFieldName);
			Field field = null;
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
						field = this.ReadField(names, context);
						if (field.IsCalculatedField)
						{
							if (calculatedFieldStartIndex < 0)
							{
								calculatedFieldStartIndex = dataFieldList.Count;
							}
							dataFieldList.Add(field);
						}
						else if (calculatedFieldStartIndex < 0)
						{
							dataFieldList.Add(field);
						}
						else
						{
							dataFieldList.Insert(calculatedFieldStartIndex, field);
							calculatedFieldStartIndex++;
						}
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
				calculatedFieldStartIndex = dataFieldList.Count;
			}
			return dataFieldList;
		}

		private Field ReadField(CLSUniqueNameValidator names, PublishingContextStruct context)
		{
			Global.Tracer.Assert(ObjectType.DataSet == context.ObjectType);
			string objectName = context.ObjectName;
			Field field = new Field();
			context.ObjectType = ObjectType.Field;
			string text = null;
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
							names.Validate(field.Name, field.DataField, objectName, this.m_errorContext);
							break;
						case "Value":
							text = this.m_reader.ReadString();
							if (text != null)
							{
								context.ObjectName = text;
								field.Value = this.ReadExpression(true, text, this.m_reader.LocalName, objectName, ExpressionParser.ExpressionType.FieldValue, ExpressionParser.ConstantType.String, context);
							}
							break;
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
			if (null != field.DataField == (null != text))
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsInvalidField, Severity.Error, context.ObjectType, field.Name, null, objectName);
			}
			return field;
		}

		private FilterList ReadFilters(ExpressionParser.ExpressionType expressionType, PublishingContextStruct context)
		{
			FilterList filterList = new FilterList();
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
						filterList.Add(this.ReadFilter(expressionType, context));
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
			return filterList;
		}

		private Filter ReadFilter(ExpressionParser.ExpressionType expressionType, PublishingContextStruct context)
		{
			this.m_hasFilters = true;
			Filter filter = new Filter();
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
						filter.Expression = this.ReadExpression(this.m_reader.LocalName, expressionType, ExpressionParser.ConstantType.String, context);
						break;
					case "Operator":
						filter.Operator = this.ReadOperator();
						break;
					case "FilterValues":
						filter.Values = this.ReadFilterValues(expressionType, context);
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
			case Filter.Operators.Equal:
			case Filter.Operators.Like:
			case Filter.Operators.GreaterThan:
			case Filter.Operators.GreaterThanOrEqual:
			case Filter.Operators.LessThan:
			case Filter.Operators.LessThanOrEqual:
			case Filter.Operators.TopN:
			case Filter.Operators.BottomN:
			case Filter.Operators.TopPercent:
			case Filter.Operators.BottomPercent:
			case Filter.Operators.NotEqual:
				if (1 != num)
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsInvalidNumberOfFilterValues, Severity.Error, context.ObjectType, context.ObjectName, "FilterValues", filter.Operator.ToString(), Convert.ToString(1, CultureInfo.InvariantCulture));
				}
				break;
			case Filter.Operators.Between:
				if (2 != num)
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsInvalidNumberOfFilterValues, Severity.Error, context.ObjectType, context.ObjectName, "FilterValues", filter.Operator.ToString(), Convert.ToString(2, CultureInfo.InvariantCulture));
				}
				break;
			}
			if (ExpressionParser.ExpressionType.GroupingFilters == expressionType && filter.Expression.HasRecursiveAggregates())
			{
				this.m_hasSpecialRecursiveAggregates = true;
			}
			return filter;
		}

		private ExpressionInfoList ReadFilterValues(ExpressionParser.ExpressionType expressionType, PublishingContextStruct context)
		{
			ExpressionInfoList expressionInfoList = new ExpressionInfoList();
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
						ExpressionInfo expressionInfo = this.ReadExpression(this.m_reader.LocalName, expressionType, ExpressionParser.ConstantType.String, context);
						expressionInfoList.Add(expressionInfo);
						if (ExpressionParser.ExpressionType.GroupingFilters == expressionType && expressionInfo.HasRecursiveAggregates())
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
			return expressionInfoList;
		}

		private void ReadBody(Report report, PublishingContextStruct context)
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
							this.ReadReportItems(null, report, report.ReportItems, context, null);
							break;
						case "Height":
							report.Height = this.ReadSize();
							break;
						case "Columns":
						{
							int columns = this.m_reader.ReadInteger();
							if (PublishingValidator.ValidateColumns(columns, context.ObjectType, context.ObjectName, "Columns", this.m_errorContext))
							{
								report.Columns = columns;
							}
							break;
						}
						case "ColumnSpacing":
							report.ColumnSpacing = this.ReadSize();
							break;
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Body, false);
							report.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, this.m_errorContext);
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

		private ParameterDefList ReadReportParameters(PublishingContextStruct context)
		{
			ParameterDefList parameterDefList = new ParameterDefList();
			CLSUniqueNameValidator reportParameterNames = new CLSUniqueNameValidator(ProcessingErrorCode.rsInvalidNameNotCLSCompliant, ProcessingErrorCode.rsDuplicateReportParameterName);
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
						parameterDefList.Add(this.ReadReportParameter(reportParameterNames, parameterNames, context, num));
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
			return parameterDefList;
		}

		private ParameterDef ReadReportParameter(CLSUniqueNameValidator reportParameterNames, Hashtable parameterNames, PublishingContextStruct context, int count)
		{
			ParameterDef parameterDef = new ParameterDef();
			parameterDef.Name = this.m_reader.GetAttribute("Name");
			context.ObjectType = ObjectType.ReportParameter;
			context.ObjectName = parameterDef.Name;
			reportParameterNames.Validate(context.ObjectType, context.ObjectName, this.m_errorContext);
			string type = null;
			string nullable = null;
			bool flag = false;
			string allowBlank = null;
			bool flag2 = false;
			string prompt = null;
			List<string> list = null;
			string multiValue = null;
			string usedInQuery = null;
			bool isComplex = false;
			bool flag3 = false;
			DataSetReference dataSetReference = null;
			DataSetReference dataSetReference2 = null;
			bool flag4 = false;
			bool flag5 = false;
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
						list = this.ReadDefaultValue(context, parameterDef, parameterNames, ref isComplex, out dataSetReference2);
						break;
					case "AllowBlank":
						allowBlank = this.m_reader.ReadString();
						break;
					case "Prompt":
						flag2 = true;
						prompt = this.m_reader.ReadString();
						break;
					case "ValidValues":
						flag3 = this.ReadValidValues(context, parameterDef, parameterNames, ref isComplex, out dataSetReference);
						break;
					case "Hidden":
						flag4 = this.m_reader.ReadBoolean();
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
						flag5 = true;
					}
					break;
				}
			}
			while (!flag5);
			parameterDef.Parse(parameterDef.Name, list, type, nullable, prompt, null, allowBlank, multiValue, usedInQuery, flag4, this.m_errorContext, CultureInfo.InvariantCulture);
			if (parameterDef.Nullable && !flag)
			{
				parameterDef.DefaultValues = new object[1];
				parameterDef.DefaultValues[0] = null;
			}
			if (parameterDef.DataType == DataType.Boolean)
			{
				dataSetReference = null;
			}
			if (!flag2 && !flag && !flag4 && (!parameterDef.Nullable || (parameterDef.ValidValuesValueExpressions != null && !flag3)))
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsMissingParameterDefault, Severity.Error, context.ObjectType, context.ObjectName, null);
			}
			if (parameterDef.Nullable && parameterDef.MultiValue)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsInvalidMultiValueParameter, Severity.Error, context.ObjectType, context.ObjectName, null);
			}
			if (!parameterDef.MultiValue && list != null && list.Count > 1)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsInvalidDefaultValue, Severity.Error, context.ObjectType, context.ObjectName, null);
			}
			if (dataSetReference2 != null || dataSetReference != null)
			{
				this.m_dynamicParameters.Add(new DynamicParameter(dataSetReference, dataSetReference2, count, isComplex));
			}
			if (!parameterNames.ContainsKey(parameterDef.Name))
			{
				parameterNames.Add(parameterDef.Name, count);
			}
			return parameterDef;
		}

		private List<string> ReadDefaultValue(PublishingContextStruct context, ParameterDef parameter, Hashtable parameterNames, ref bool isComplex, out DataSetReference defaultDataSet)
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
							result = this.ReadValues(context, parameter, parameterNames, out isComplex);
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
			if (!flag && !flag2)
			{
				goto IL_00b8;
			}
			if (flag && flag2)
			{
				goto IL_00b8;
			}
			goto IL_00e0;
			IL_00e0:
			return result;
			IL_00b8:
			this.m_errorContext.Register(ProcessingErrorCode.rsInvalidDefaultValue, Severity.Error, context.ObjectType, context.ObjectName, "DefaultValue");
			goto IL_00e0;
		}

		private List<string> ReadValues(PublishingContextStruct context, ParameterDef parameter, Hashtable parameterNames, out bool isComplex)
		{
			List<string> list = null;
			ExpressionInfoList expressionInfoList = new ExpressionInfoList();
			ExpressionInfo expressionInfo = null;
			bool flag = false;
			Hashtable dependencies = null;
			bool flag2 = false;
			isComplex = false;
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
						expressionInfo = this.ReadParameterExpression(this.m_reader.LocalName, context, parameter, parameterNames, ref dependencies, ref flag, ref isComplex);
						expressionInfoList.Add(expressionInfo);
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
			if (isComplex && parameterNames.Count > 0)
			{
				dependencies = (Hashtable)parameterNames.Clone();
			}
			if (flag)
			{
				parameter.DefaultExpressions = expressionInfoList;
			}
			else
			{
				list = new List<string>(expressionInfoList.Count);
				for (int i = 0; i < expressionInfoList.Count; i++)
				{
					list.Add(expressionInfoList[i].Value);
				}
			}
			parameter.Dependencies = dependencies;
			return list;
		}

		private bool ReadValidValues(PublishingContextStruct context, ParameterDef parameter, Hashtable parameterNames, ref bool isComplex, out DataSetReference validValueDataSet)
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
			if (!flag && !flag2)
			{
				goto IL_00b9;
			}
			if (flag && flag2)
			{
				goto IL_00b9;
			}
			goto IL_00e4;
			IL_00e4:
			return result;
			IL_00b9:
			this.m_errorContext.Register(ProcessingErrorCode.rsInvalidValidValues, Severity.Error, context.ObjectType, context.ObjectName, "ValidValues");
			goto IL_00e4;
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

		private void ReadParameterValues(PublishingContextStruct context, ParameterDef parameter, Hashtable parameterNames, ref bool isComplex, ref bool containsExplicitNull)
		{
			ExpressionInfoList expressionInfoList = new ExpressionInfoList();
			ExpressionInfoList expressionInfoList2 = new ExpressionInfoList();
			Hashtable dependencies = null;
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
						ExpressionInfo expressionInfo = null;
						ExpressionInfo value = null;
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
										expressionInfo = this.ReadParameterExpression(this.m_reader.LocalName, context, parameter, parameterNames, ref dependencies, ref flag, ref isComplex);
										break;
									case "Label":
										value = this.ReadParameterExpression(this.m_reader.LocalName, context, parameter, parameterNames, ref dependencies, ref flag, ref isComplex);
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
						expressionInfoList.Add(expressionInfo);
						expressionInfoList2.Add(value);
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
			if (isComplex && parameterNames.Count > 0)
			{
				dependencies = (Hashtable)parameterNames.Clone();
			}
			parameter.ValidValuesValueExpressions = expressionInfoList;
			parameter.ValidValuesLabelExpressions = expressionInfoList2;
			parameter.Dependencies = dependencies;
		}

		private ExpressionInfo ReadParameterExpression(string propertyName, PublishingContextStruct context, ParameterDef parameter, Hashtable parameterNames, ref Hashtable dependencies, ref bool dynamic, ref bool isComplex)
		{
			ExpressionInfo expressionInfo = null;
			string text = null;
			bool flag = false;
			bool flag2 = default(bool);
			if (isComplex)
			{
				dynamic = true;
				expressionInfo = this.ReadExpression(propertyName, (string)null, ExpressionParser.ExpressionType.ReportParameter, ExpressionParser.ConstantType.String, context, out flag2);
			}
			else
			{
				ExpressionParser.DetectionFlags detectionFlags = ExpressionParser.DetectionFlags.ParameterReference;
				detectionFlags |= ExpressionParser.DetectionFlags.UserReference;
				bool flag3 = default(bool);
				expressionInfo = this.ReadExpression(propertyName, (string)null, ExpressionParser.ExpressionType.ReportParameter, ExpressionParser.ConstantType.String, context, detectionFlags, out flag3, out text, out flag2);
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
						if (dependencies == null)
						{
							dependencies = new Hashtable();
						}
						dependencies.Add(text, parameterNames[text]);
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
				this.m_errorContext.Register(ProcessingErrorCode.rsInvalidReportParameterDependency, Severity.Error, ObjectType.ReportParameter, parameter.Name, "ValidValues", text);
			}
			return expressionInfo;
		}

		private ParameterValueList ReadParameters(PublishingContextStruct context, bool doClsValidation)
		{
			bool flag = default(bool);
			return this.ReadParameters(context, false, doClsValidation, out flag);
		}

		private ParameterValueList ReadParameters(PublishingContextStruct context, bool omitAllowed, bool doClsValidation, out bool computed)
		{
			computed = false;
			ParameterValueList parameterValueList = new ParameterValueList();
			ParameterNameValidator parameterNames = new ParameterNameValidator();
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
						parameterValueList.Add(this.ReadParameter(parameterNames, context, omitAllowed, doClsValidation, out flag2));
						computed |= flag2;
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
			return parameterValueList;
		}

		private ParameterValue ReadParameter(ParameterNameValidator parameterNames, PublishingContextStruct context, bool omitAllowed, bool doClsValidation, out bool computed)
		{
			computed = false;
			bool flag = false;
			bool flag2 = false;
			ParameterValue parameterValue = new ParameterValue();
			parameterValue.Name = this.m_reader.GetAttribute("Name");
			if (doClsValidation)
			{
				parameterNames.Validate(parameterValue.Name, context.ObjectType, context.ObjectName, this.m_errorContext);
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
						parameterValue.Value = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out flag);
						break;
					case "Omit":
						if (omitAllowed)
						{
							parameterValue.Omit = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.Boolean, context, out flag2);
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
			computed = (flag | flag2);
			return parameterValue;
		}

		private PageSection ReadPageSection(bool isHeader, Report report, PublishingContextStruct context)
		{
			PageSection pageSection = new PageSection(isHeader, this.GenerateID(), this.GenerateID(), report);
			context.Location |= LocationFlags.InPageSection;
			context.ObjectType = pageSection.ObjectType;
			context.ObjectName = null;
			this.m_reportItemCollectionList.Add(pageSection.ReportItems);
			this.m_runningValueHolderList.Add(pageSection.ReportItems);
			this.m_reportLocationFlags = UserLocationFlags.ReportPageSection;
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
						pageSection.PrintOnFirstPage = this.m_reader.ReadBoolean();
						break;
					case "PrintOnLastPage":
						pageSection.PrintOnLastPage = this.m_reader.ReadBoolean();
						break;
					case "ReportItems":
						this.ReadReportItems((string)null, (ReportItem)pageSection, pageSection.ReportItems, context, (TextBoxList)null, out flag);
						break;
					case "Style":
					{
						StyleInformation styleInformation = this.ReadStyle(context, out flag2);
						styleInformation.Filter(StyleOwnerType.PageSection, false);
						pageSection.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, this.m_errorContext);
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
			pageSection.PostProcessEvaluate = (flag | flag2 | this.m_pageSectionDrillthroughs);
			this.m_pageSectionDrillthroughs = false;
			this.m_reportLocationFlags = UserLocationFlags.ReportBody;
			return pageSection;
		}

		private void ReadReportItems(string propertyName, ReportItem parent, ReportItemCollection parentCollection, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget, out bool computed)
		{
			computed = false;
			int num = 0;
			bool flag = parent is Matrix;
			bool flag2 = parent is Table;
			bool flag3 = parent is CustomReportItem;
			bool flag4 = false;
			do
			{
				ReportItem reportItem = null;
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
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
						if (!flag3)
						{
							reportItem = this.ReadCustomReportItem(parent, context, textBoxesWithDefaultSortTarget);
						}
						else
						{
							this.m_errorContext.Register(ProcessingErrorCode.rsInvalidAltReportItem, Severity.Error, context.ObjectType, context.ObjectName, propertyName);
						}
						break;
					case "Checkbox":
						num++;
						reportItem = this.ReadCheckbox(parent, context);
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
					case "ActiveXControl":
						num++;
						reportItem = this.ReadActiveXControl(parent, context);
						break;
					case "List":
						num++;
						reportItem = this.ReadList(parent, context);
						break;
					case "Matrix":
						num++;
						reportItem = this.ReadMatrix(parent, context);
						break;
					case "Table":
						num++;
						reportItem = this.ReadTable(parent, context);
						break;
					case "OWCChart":
						num++;
						reportItem = this.ReadOWCChart(parent, context);
						break;
					case "Chart":
						num++;
						reportItem = this.ReadChart(parent, context);
						break;
					}
					if (flag && num > 1)
					{
						this.m_errorContext.Register(ProcessingErrorCode.rsMultiReportItemsInMatrixSection, Severity.Error, context.ObjectType, context.ObjectName, propertyName);
					}
					if (flag && (LocationFlags.InMatrixSubtotal & context.Location) != 0 && reportItem != null && !(reportItem is TextBox))
					{
						this.m_errorContext.Register(ProcessingErrorCode.rsInvalidMatrixSubtotalReportItem, Severity.Error, context.ObjectType, context.ObjectName, "Subtotal");
					}
					if (flag2 && num > 1)
					{
						this.m_errorContext.Register(ProcessingErrorCode.rsMultiReportItemsInTableCell, Severity.Error, context.ObjectType, context.ObjectName, propertyName);
					}
					if (flag3 && num > 1)
					{
						this.m_errorContext.Register(ProcessingErrorCode.rsMultiReportItemsInCustomReportItem, Severity.Error, context.ObjectType, context.ObjectName, propertyName);
					}
					if (reportItem != null)
					{
						computed |= reportItem.Computed;
						parentCollection.AddReportItem(reportItem);
						if (flag || flag2)
						{
							reportItem.IsFullSize = true;
						}
						if (flag3)
						{
							if (!(parent.Parent is Matrix) && !(parent.Parent is Table))
							{
								break;
							}
							reportItem.IsFullSize = true;
						}
					}
					break;
				case XmlNodeType.EndElement:
					if (!("ReportItems" == this.m_reader.LocalName))
					{
						if (!flag3)
						{
							break;
						}
						if (!("AltReportItem" == this.m_reader.LocalName))
						{
							break;
						}
					}
					flag4 = true;
					break;
				}
			}
			while (!flag4);
		}

		private void ReadReportItems(string propertyName, ReportItem parent, ReportItemCollection parentCollection, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			bool flag = default(bool);
			this.ReadReportItems(propertyName, parent, parentCollection, context, textBoxesWithDefaultSortTarget, out flag);
		}

		private CustomReportItem ReadCustomReportItem(ReportItem parent, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			CustomReportItem customReportItem = new CustomReportItem(this.GenerateID(), this.GenerateID(), parent);
			customReportItem.Name = this.m_reader.GetAttribute("Name");
			context.ObjectType = customReportItem.ObjectType;
			context.ObjectName = customReportItem.Name;
			this.m_dataRegionCount++;
			this.m_reportItemNames.Validate(context.ObjectType, context.ObjectName, this.m_errorContext);
			bool flag = true;
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsCRIInPageSection, Severity.Error, context.ObjectType, context.ObjectName, null);
				flag = false;
			}
			this.m_reportItemCollectionList.Add(customReportItem.AltReportItem);
			this.m_aggregateHolderList.Add(customReportItem);
			this.m_runningValueHolderList.Add(customReportItem);
			this.m_runningValueHolderList.Add(customReportItem.AltReportItem);
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = false;
			bool flag8 = false;
			ExpressionInfo expressionInfo = null;
			ExpressionInfo expressionInfo2 = null;
			TextBoxList textBoxList = new TextBoxList();
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
							customReportItem.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, this.m_errorContext);
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
							customReportItem.ZIndex = this.m_reader.ReadInteger();
							break;
						case "Visibility":
							customReportItem.Visibility = this.ReadVisibility(context, out flag3);
							break;
						case "Label":
							expressionInfo = (customReportItem.Label = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out flag6));
							break;
						case "Bookmark":
							expressionInfo2 = (customReportItem.Bookmark = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out flag7));
							break;
						case "RepeatWith":
							customReportItem.RepeatedSibling = true;
							customReportItem.RepeatWith = this.m_reader.ReadString();
							break;
						case "Type":
							customReportItem.Type = this.m_reader.ReadString();
							break;
						case "AltReportItem":
							this.ReadReportItems("AltReportItem", (ReportItem)customReportItem, customReportItem.AltReportItem, context, textBoxList, out flag4);
							Global.Tracer.Assert(1 <= customReportItem.AltReportItem.Count);
							break;
						case "CustomData":
							this.ReadCustomData(customReportItem, context);
							break;
						case "CustomProperties":
							customReportItem.CustomProperties = this.ReadCustomProperties(context, out flag5);
							break;
						case "DataElementName":
							customReportItem.DataElementName = this.m_reader.ReadString();
							break;
						case "DataElementOutput":
							customReportItem.DataElementOutputRDL = this.ReadDataElementOutputRDL();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("CustomReportItem" == this.m_reader.LocalName)
						{
							flag8 = true;
						}
						break;
					}
				}
				while (!flag8);
			}
			customReportItem.Computed = true;
			if (!flag6 && expressionInfo != null && expressionInfo.Value != null)
			{
				this.m_hasLabels = true;
			}
			if (!flag7 && expressionInfo2 != null && expressionInfo2.Value != null)
			{
				this.m_hasBookmarks = true;
			}
			Global.Tracer.Assert(null != customReportItem.AltReportItem);
			if (customReportItem.AltReportItem.Count == 0)
			{
				Rectangle rectangle = new Rectangle(this.GenerateID(), this.GenerateID(), parent);
				rectangle.Name = customReportItem.Name + "_" + customReportItem.ID + "_" + rectangle.ID;
				this.m_reportItemNames.Validate(rectangle.ObjectType, rectangle.Name, this.m_errorContext);
				rectangle.Computed = false;
				Visibility visibility = new Visibility();
				ExpressionParser.ExpressionContext context2 = context.CreateExpressionContext(ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.Boolean, "Hidden", null);
				bool flag9 = default(bool);
				visibility.Hidden = this.m_reportCT.ParseExpression("true", context2, out flag9);
				Global.Tracer.Assert(!flag9);
				rectangle.Visibility = visibility;
				this.m_reportItemCollectionList.Add(rectangle.ReportItems);
				if (parent is Matrix || parent is Table)
				{
					rectangle.IsFullSize = true;
				}
				customReportItem.AltReportItem.AddReportItem(rectangle);
			}
			if (customReportItem.DataSetName != null)
			{
				this.SetSortTargetForTextBoxes(textBoxList, customReportItem);
			}
			else if (textBoxesWithDefaultSortTarget != null)
			{
				textBoxesWithDefaultSortTarget.AddRange(textBoxList);
			}
			if (!flag)
			{
				return null;
			}
			return customReportItem;
		}

		private void ReadCustomData(CustomReportItem crItem, PublishingContextStruct context)
		{
			LocationFlags location = context.Location;
			context.Location = (context.Location | LocationFlags.InDataSet | LocationFlags.InDataRegion);
			if (this.m_scopeNames.Validate(false, context.ObjectName, context.ObjectType, context.ObjectName, this.m_errorContext))
			{
				this.m_reportScopes.Add(crItem.Name, crItem);
			}
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsDataRegionInPageSection, Severity.Error, context.ObjectType, context.ObjectName, null);
			}
			if ((context.Location & LocationFlags.InDetail) != 0)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsDataRegionInTableDetailRow, Severity.Error, context.ObjectType, context.ObjectName, null);
			}
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
						crItem.DataSetName = this.m_reader.ReadString();
						break;
					case "DataColumnGroupings":
						crItem.Columns = this.ReadCustomDataColumnOrRowGroupings(true, crItem, context);
						break;
					case "DataRowGroupings":
						crItem.Rows = this.ReadCustomDataColumnOrRowGroupings(false, crItem, context);
						break;
					case "DataRows":
						crItem.DataRowCells = this.ReadCustomDataRows(crItem, context);
						break;
					case "Filters":
						crItem.Filters = this.ReadFilters(ExpressionParser.ExpressionType.DataRegionFilters, context);
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
		}

		private CustomReportItemHeadingList ReadCustomDataColumnOrRowGroupings(bool isColumn, CustomReportItem crItem, PublishingContextStruct context)
		{
			CustomReportItemHeadingList customReportItemHeadingList = new CustomReportItemHeadingList();
			int num = -1;
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "DataGroupings")
					{
						num = this.ReadCustomDataGroupings(isColumn, crItem, customReportItemHeadingList, context);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if (!isColumn || !("DataColumnGroupings" == this.m_reader.LocalName))
					{
						if (isColumn)
						{
							break;
						}
						if (!("DataRowGroupings" == this.m_reader.LocalName))
						{
							break;
						}
					}
					flag = true;
					break;
				}
			}
			while (!flag);
			if (isColumn)
			{
				crItem.ExpectedColumns = num;
			}
			else
			{
				crItem.ExpectedRows = num;
			}
			return customReportItemHeadingList;
		}

		private int ReadCustomDataGroupings(bool isColumn, CustomReportItem crItem, CustomReportItemHeadingList crGroupingList, PublishingContextStruct context)
		{
			bool flag = false;
			int num = 0;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "DataGrouping")
					{
						int num2 = default(int);
						crGroupingList.Add(this.ReadCustomDataGrouping(isColumn, crItem, context, out num2));
						num += num2;
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("DataGroupings" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return num;
		}

		private CustomReportItemHeading ReadCustomDataGrouping(bool isColumn, CustomReportItem crItem, PublishingContextStruct context, out int groupingLeafs)
		{
			CustomReportItemHeading customReportItemHeading = new CustomReportItemHeading(this.GenerateID(), crItem);
			this.m_runningValueHolderList.Add(customReportItemHeading);
			customReportItemHeading.IsColumn = isColumn;
			groupingLeafs = 1;
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
						case "Static":
							customReportItemHeading.Static = this.m_reader.ReadBoolean();
							break;
						case "Grouping":
							customReportItemHeading.Grouping = this.ReadGrouping(context);
							break;
						case "Sorting":
							customReportItemHeading.Sorting = this.ReadSorting(context);
							break;
						case "Subtotal":
							customReportItemHeading.Subtotal = this.m_reader.ReadBoolean();
							break;
						case "CustomProperties":
							customReportItemHeading.CustomProperties = this.ReadCustomProperties(context);
							break;
						case "DataGroupings":
							customReportItemHeading.InnerHeadings = new CustomReportItemHeadingList();
							groupingLeafs = this.ReadCustomDataGroupings(isColumn, crItem, customReportItemHeading.InnerHeadings, context);
							customReportItemHeading.HeadingSpan = groupingLeafs;
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("DataGrouping" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			if (this.CanMergeGroupingAndSorting(customReportItemHeading.Grouping, customReportItemHeading.Sorting))
			{
				customReportItemHeading.Grouping.GroupAndSort = true;
				customReportItemHeading.Grouping.SortDirections = customReportItemHeading.Sorting.SortDirections;
				customReportItemHeading.Sorting = null;
			}
			if (customReportItemHeading.Sorting != null)
			{
				this.m_hasSorting = true;
			}
			return customReportItemHeading;
		}

		private DataCellsList ReadCustomDataRows(CustomReportItem crItem, PublishingContextStruct context)
		{
			DataCellsList dataCellsList = new DataCellsList();
			bool flag = false;
			int num = 0;
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
						dataCellsList.Add(this.ReadCustomDataRow(crItem, num++, context));
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
			return dataCellsList;
		}

		private DataCellList ReadCustomDataRow(CustomReportItem crItem, int rowIndex, PublishingContextStruct context)
		{
			DataCellList dataCellList = new DataCellList();
			bool flag = false;
			int num = 0;
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
						dataCellList.Add(this.ReadCustomDataCell(crItem, rowIndex, num++, context));
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
			return dataCellList;
		}

		private DataValueCRIList ReadCustomDataCell(CustomReportItem crItem, int rowIndex, int columnIndex, PublishingContextStruct context)
		{
			DataValueCRIList dataValueCRIList = new DataValueCRIList();
			dataValueCRIList.RDLRowIndex = rowIndex;
			dataValueCRIList.RDLColumnIndex = columnIndex;
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
					if ((localName = this.m_reader.LocalName) != null && localName == "DataValue")
					{
						dataValueCRIList.Add(this.ReadDataValue(false, ++num, context));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("DataCell" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return dataValueCRIList;
		}

		private Line ReadLine(ReportItem parent, PublishingContextStruct context)
		{
			Line line = new Line(this.GenerateID(), parent);
			line.Name = this.m_reader.GetAttribute("Name");
			context.ObjectType = line.ObjectType;
			context.ObjectName = line.Name;
			this.m_reportItemNames.Validate(context.ObjectType, context.ObjectName, this.m_errorContext);
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			ExpressionInfo expressionInfo = null;
			ExpressionInfo expressionInfo2 = null;
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
							StyleInformation styleInformation = this.ReadStyle(context, out flag);
							styleInformation.Filter(StyleOwnerType.Line, false);
							line.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, this.m_errorContext);
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
							line.ZIndex = this.m_reader.ReadInteger();
							break;
						case "Visibility":
							line.Visibility = this.ReadVisibility(context, out flag2);
							break;
						case "Label":
							expressionInfo = (line.Label = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out flag3));
							break;
						case "Bookmark":
							expressionInfo2 = (line.Bookmark = this.ReadBookmarkExpression(context, out flag4));
							break;
						case "RepeatWith":
							line.RepeatedSibling = true;
							line.RepeatWith = this.m_reader.ReadString();
							break;
						case "Custom":
							line.Custom = this.m_reader.ReadCustomXml();
							break;
						case "CustomProperties":
							line.CustomProperties = this.ReadCustomProperties(context, out flag5);
							break;
						case "DataElementName":
							line.DataElementName = this.m_reader.ReadString();
							break;
						case "DataElementOutput":
							line.DataElementOutputRDL = this.ReadDataElementOutputRDL();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("Line" == this.m_reader.LocalName)
						{
							flag6 = true;
						}
						break;
					}
				}
				while (!flag6);
			}
			line.Computed = (flag | flag2 | flag3 | flag4 | flag5);
			if (!flag3 && expressionInfo != null && expressionInfo.Value != null)
			{
				this.m_hasLabels = true;
			}
			if (!flag4 && expressionInfo2 != null && expressionInfo2.Value != null)
			{
				this.m_hasBookmarks = true;
			}
			return line;
		}

		private Rectangle ReadRectangle(ReportItem parent, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			Rectangle rectangle = new Rectangle(this.GenerateID(), this.GenerateID(), parent);
			rectangle.Name = this.m_reader.GetAttribute("Name");
			context.ObjectType = rectangle.ObjectType;
			context.ObjectName = rectangle.Name;
			this.m_reportItemNames.Validate(context.ObjectType, context.ObjectName, this.m_errorContext);
			this.m_reportItemCollectionList.Add(rectangle.ReportItems);
			this.m_runningValueHolderList.Add(rectangle.ReportItems);
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = false;
			bool flag8 = false;
			string text = null;
			ExpressionInfo expressionInfo = null;
			ExpressionInfo expressionInfo2 = null;
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
							StyleInformation styleInformation = this.ReadStyle(context, out flag);
							styleInformation.Filter(StyleOwnerType.Rectangle, false);
							rectangle.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, this.m_errorContext);
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
							rectangle.ZIndex = this.m_reader.ReadInteger();
							break;
						case "Visibility":
							rectangle.Visibility = this.ReadVisibility(context, out flag2);
							break;
						case "ToolTip":
							rectangle.ToolTip = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out flag5);
							break;
						case "Label":
							expressionInfo = (rectangle.Label = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out flag3));
							break;
						case "LinkToChild":
							text = this.m_reader.ReadString();
							break;
						case "Bookmark":
							expressionInfo2 = (rectangle.Bookmark = this.ReadBookmarkExpression(context, out flag4));
							break;
						case "RepeatWith":
							rectangle.RepeatedSibling = true;
							rectangle.RepeatWith = this.m_reader.ReadString();
							break;
						case "Custom":
							rectangle.Custom = this.m_reader.ReadCustomXml();
							break;
						case "CustomProperties":
							rectangle.CustomProperties = this.ReadCustomProperties(context, out flag7);
							break;
						case "ReportItems":
							this.ReadReportItems((string)null, (ReportItem)rectangle, rectangle.ReportItems, context, textBoxesWithDefaultSortTarget, out flag6);
							break;
						case "PageBreakAtStart":
							rectangle.PageBreakAtStart = this.m_reader.ReadBoolean();
							break;
						case "PageBreakAtEnd":
							rectangle.PageBreakAtEnd = this.m_reader.ReadBoolean();
							break;
						case "DataElementName":
							rectangle.DataElementName = this.m_reader.ReadString();
							break;
						case "DataElementOutput":
							rectangle.DataElementOutputRDL = this.ReadDataElementOutputRDL();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("Rectangle" == this.m_reader.LocalName)
						{
							flag8 = true;
						}
						break;
					}
				}
				while (!flag8);
			}
			rectangle.Computed = (flag | flag2 | flag3 | flag4 | flag6 | flag5 | flag7 | rectangle.PageBreakAtStart | rectangle.PageBreakAtEnd);
			if (!flag3 && expressionInfo != null && expressionInfo.Value != null)
			{
				this.m_hasLabels = true;
			}
			if (!flag4 && expressionInfo2 != null && expressionInfo2.Value != null)
			{
				this.m_hasBookmarks = true;
			}
			if (expressionInfo != null && text != null)
			{
				rectangle.ReportItems.LinkToChild = text;
			}
			return rectangle;
		}

		private CheckBox ReadCheckbox(ReportItem parent, PublishingContextStruct context)
		{
			CheckBox checkBox = new CheckBox(this.GenerateID(), parent);
			checkBox.Name = this.m_reader.GetAttribute("Name");
			context.ObjectType = checkBox.ObjectType;
			context.ObjectName = checkBox.Name;
			this.m_reportItemNames.Validate(context.ObjectType, context.ObjectName, this.m_errorContext);
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = false;
			bool flag8 = false;
			ExpressionInfo expressionInfo = null;
			ExpressionInfo expressionInfo2 = null;
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
							StyleInformation styleInformation = this.ReadStyle(context, out flag);
							styleInformation.Filter(StyleOwnerType.Checkbox, false);
							checkBox.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, this.m_errorContext);
							break;
						}
						case "Top":
							checkBox.Top = this.ReadSize();
							break;
						case "Left":
							checkBox.Left = this.ReadSize();
							break;
						case "ZIndex":
							checkBox.ZIndex = this.m_reader.ReadInteger();
							break;
						case "Visibility":
							checkBox.Visibility = this.ReadVisibility(context, out flag2);
							break;
						case "ToolTip":
							checkBox.ToolTip = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out flag5);
							break;
						case "Label":
							expressionInfo = (checkBox.Label = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out flag3));
							break;
						case "Bookmark":
							expressionInfo2 = (checkBox.Bookmark = this.ReadBookmarkExpression(context, out flag4));
							break;
						case "RepeatWith":
							checkBox.RepeatedSibling = true;
							checkBox.RepeatWith = this.m_reader.ReadString();
							break;
						case "Custom":
							checkBox.Custom = this.m_reader.ReadCustomXml();
							break;
						case "CustomProperties":
							checkBox.CustomProperties = this.ReadCustomProperties(context, out flag7);
							break;
						case "Value":
							checkBox.Value = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.Boolean, context, out flag6);
							break;
						case "HideDuplicates":
						{
							string text = this.m_reader.ReadString();
							if ((context.Location & LocationFlags.InPageSection) != 0 || text == null || text.Length <= 0)
							{
								checkBox.HideDuplicates = null;
							}
							else
							{
								checkBox.HideDuplicates = text;
							}
							break;
						}
						case "DataElementName":
							checkBox.DataElementName = this.m_reader.ReadString();
							break;
						case "DataElementOutput":
							checkBox.DataElementOutputRDL = this.ReadDataElementOutputRDL();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("Checkbox" == this.m_reader.LocalName)
						{
							flag8 = true;
						}
						break;
					}
				}
				while (!flag8);
			}
			checkBox.Computed = (flag | flag2 | flag3 | flag5 | flag4 | flag6 | flag7 | checkBox.HideDuplicates != null);
			if (!flag3 && expressionInfo != null && expressionInfo.Value != null)
			{
				this.m_hasLabels = true;
			}
			if (!flag4 && expressionInfo2 != null && expressionInfo2.Value != null)
			{
				this.m_hasBookmarks = true;
			}
			return checkBox;
		}

		private TextBox ReadTextbox(ReportItem parent, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			TextBox textBox = new TextBox(this.GenerateID(), parent);
			textBox.Name = this.m_reader.GetAttribute("Name");
			context.ObjectType = textBox.ObjectType;
			context.ObjectName = textBox.Name;
			this.m_reportItemNames.Validate(context.ObjectType, context.ObjectName, this.m_errorContext);
			Global.Tracer.Assert(!this.m_reportCT.ValueReferenced);
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = false;
			bool flag8 = false;
			bool flag9 = false;
			bool flag10 = false;
			ExpressionInfo expressionInfo = null;
			ExpressionInfo expressionInfo2 = null;
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
						styleInformation.Filter(StyleOwnerType.Textbox, false);
						textBox.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, this.m_errorContext);
						break;
					}
					case "Action":
					{
						int num = -1;
						bool flag11 = false;
						ActionItem actionItem = this.ReadActionItem(context, out flag2, ref num, ref flag11);
						textBox.Action = new Action(actionItem, flag2);
						break;
					}
					case "ActionInfo":
						textBox.Action = this.ReadAction(context, StyleOwnerType.Textbox, out flag2);
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
						textBox.ZIndex = this.m_reader.ReadInteger();
						break;
					case "Visibility":
						textBox.Visibility = this.ReadVisibility(context, out flag3);
						break;
					case "ToolTip":
						textBox.ToolTip = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out flag6);
						break;
					case "Label":
						expressionInfo = (textBox.Label = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out flag4));
						break;
					case "Bookmark":
						expressionInfo2 = (textBox.Bookmark = this.ReadBookmarkExpression(context, out flag5));
						break;
					case "RepeatWith":
						textBox.RepeatedSibling = true;
						textBox.RepeatWith = this.m_reader.ReadString();
						break;
					case "Custom":
						textBox.Custom = this.m_reader.ReadCustomXml();
						break;
					case "CustomProperties":
						textBox.CustomProperties = this.ReadCustomProperties(context, out flag9);
						break;
					case "Value":
					{
						ExpressionInfo expressionInfo3 = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out flag7);
						if (expressionInfo3 != null)
						{
							textBox.Value = expressionInfo3;
							if (expressionInfo3.Type != ExpressionInfo.Types.Constant)
							{
								textBox.Formula = textBox.Value.OriginalText;
							}
						}
						break;
					}
					case "CanGrow":
						textBox.CanGrow = this.m_reader.ReadBoolean();
						break;
					case "CanShrink":
						textBox.CanShrink = this.m_reader.ReadBoolean();
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
						textBox.InitialToggleState = this.ReadToggleImage(context, out flag8);
						break;
					case "UserSort":
						this.ReadUserSort(context, textBox, textBoxesWithDefaultSortTarget);
						this.m_hasUserSort = true;
						break;
					case "DataElementName":
						textBox.DataElementName = this.m_reader.ReadString();
						break;
					case "DataElementOutput":
						textBox.DataElementOutputRDL = this.ReadDataElementOutputRDL();
						break;
					case "DataElementStyle":
					{
						ReportItem.DataElementStylesRDL dataElementStylesRDL = this.ReadDataElementStyleRDL();
						if (ReportItem.DataElementStylesRDL.Auto != dataElementStylesRDL)
						{
							textBox.OverrideReportDataElementStyle = true;
							Global.Tracer.Assert(dataElementStylesRDL == ReportItem.DataElementStylesRDL.AttributeNormal || ReportItem.DataElementStylesRDL.ElementNormal == dataElementStylesRDL);
							textBox.DataElementStyleAttribute = (ReportItem.DataElementStylesRDL.AttributeNormal == dataElementStylesRDL);
						}
						break;
					}
					}
					break;
				case XmlNodeType.EndElement:
					if ("Textbox" == this.m_reader.LocalName)
					{
						flag10 = true;
					}
					break;
				}
			}
			while (!flag10);
			textBox.Computed = (flag | flag2 | flag3 | flag9 | flag4 | flag5 | flag6 | flag7 | flag8 | textBox.UserSort != null | textBox.HideDuplicates != null);
			textBox.ValueReferenced = this.m_reportCT.ValueReferenced;
			this.m_reportCT.ResetValueReferencedFlag();
			if (!flag4 && expressionInfo != null && expressionInfo.Value != null)
			{
				this.m_hasLabels = true;
			}
			if (!flag5 && expressionInfo2 != null && expressionInfo2.Value != null)
			{
				this.m_hasBookmarks = true;
			}
			return textBox;
		}

		private void ReadUserSort(PublishingContextStruct context, TextBox textbox, TextBoxList textBoxesWithDefaultSortTarget)
		{
			bool flag = (LocationFlags)0 != (context.Location & LocationFlags.InPageSection);
			bool flag2 = false;
			EndUserSort endUserSort = new EndUserSort();
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
						endUserSort.SortExpression = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.SortExpression, ExpressionParser.ConstantType.String, context, out flag3);
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
			if (flag)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsInvalidTextboxInPageSection, Severity.Error, textbox.ObjectType, textbox.Name, "UserSort");
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

		private void SetSortTargetForTextBoxes(TextBoxList textBoxes, ISortFilterScope target)
		{
			if (textBoxes != null)
			{
				for (int i = 0; i < textBoxes.Count; i++)
				{
					textBoxes[i].UserSort.SetSortTarget(target);
				}
			}
		}

		private ExpressionInfo ReadToggleImage(PublishingContextStruct context, out bool computed)
		{
			computed = false;
			this.m_static = true;
			ExpressionInfo result = null;
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
						result = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.Boolean, context, out computed);
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

		private Image ReadImage(ReportItem parent, PublishingContextStruct context)
		{
			Image image = new Image(this.GenerateID(), parent);
			image.Name = this.m_reader.GetAttribute("Name");
			context.ObjectType = image.ObjectType;
			context.ObjectName = image.Name;
			this.m_reportItemNames.Validate(context.ObjectType, context.ObjectName, this.m_errorContext);
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = false;
			bool flag8 = false;
			bool flag9 = false;
			bool flag10 = false;
			ExpressionInfo expressionInfo = null;
			ExpressionInfo expressionInfo2 = null;
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
						styleInformation.Filter(StyleOwnerType.Image, false);
						image.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, this.m_errorContext);
						break;
					}
					case "Action":
					{
						int num = -1;
						bool flag11 = false;
						ActionItem actionItem = this.ReadActionItem(context, out flag2, ref num, ref flag11);
						image.Action = new Action(actionItem, flag2);
						break;
					}
					case "ActionInfo":
						image.Action = this.ReadAction(context, StyleOwnerType.Image, out flag2);
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
						image.ZIndex = this.m_reader.ReadInteger();
						break;
					case "Visibility":
						image.Visibility = this.ReadVisibility(context, out flag3);
						break;
					case "ToolTip":
						image.ToolTip = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out flag6);
						break;
					case "Label":
						expressionInfo = (image.Label = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out flag4));
						break;
					case "Bookmark":
						expressionInfo2 = (image.Bookmark = this.ReadBookmarkExpression(context, out flag5));
						break;
					case "RepeatWith":
						image.RepeatedSibling = true;
						image.RepeatWith = this.m_reader.ReadString();
						break;
					case "Custom":
						image.Custom = this.m_reader.ReadCustomXml();
						break;
					case "CustomProperties":
						image.CustomProperties = this.ReadCustomProperties(context, out flag9);
						break;
					case "Source":
						image.Source = this.ReadSource();
						break;
					case "Value":
						image.Value = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out flag7);
						break;
					case "MIMEType":
						image.MIMEType = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out flag8);
						break;
					case "Sizing":
						image.Sizing = this.ReadSizing();
						break;
					case "DataElementName":
						image.DataElementName = this.m_reader.ReadString();
						break;
					case "DataElementOutput":
						image.DataElementOutputRDL = this.ReadDataElementOutputRDL();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Image" == this.m_reader.LocalName)
					{
						flag10 = true;
					}
					break;
				}
			}
			while (!flag10);
			if (Image.SourceType.Database == image.Source)
			{
				Global.Tracer.Assert(null != image.Value);
				if (ExpressionInfo.Types.Constant == image.Value.Type)
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsBinaryConstant, Severity.Error, context.ObjectType, context.ObjectName, "Value");
				}
				if (!PublishingValidator.ValidateMimeType(image.MIMEType, context.ObjectType, context.ObjectName, "MIMEType", this.m_errorContext))
				{
					image.MIMEType = null;
				}
			}
			else
			{
				if (image.Source == Image.SourceType.External && ExpressionInfo.Types.Constant == image.Value.Type && image.Value.Value != null && image.Value.Value.Trim().Length == 0)
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsInvalidEmptyImageReference, Severity.Error, context.ObjectType, context.ObjectName, "Value");
				}
				image.MIMEType = null;
				if (image.Source == Image.SourceType.External && !flag7)
				{
					this.m_imageStreamNames[image.Value.Value] = new ImageInfo(image.Name, null);
				}
			}
			image.Computed = (flag | flag2 | flag3 | flag9 | flag4 | flag5 | flag6 | flag7 | flag8);
			this.m_hasImageStreams = true;
			if (!flag4 && expressionInfo != null && expressionInfo.Value != null)
			{
				this.m_hasLabels = true;
			}
			if (!flag5 && expressionInfo2 != null && expressionInfo2.Value != null)
			{
				this.m_hasBookmarks = true;
			}
			if (image.Source == Image.SourceType.External)
			{
				this.m_hasExternalImages = true;
			}
			return image;
		}

		private SubReport ReadSubreport(ReportItem parent, PublishingContextStruct context)
		{
			SubReport subReport = new SubReport(this.GenerateID(), parent);
			subReport.Name = this.m_reader.GetAttribute("Name");
			context.ObjectType = subReport.ObjectType;
			context.ObjectName = subReport.Name;
			this.m_reportItemNames.Validate(context.ObjectType, context.ObjectName, this.m_errorContext);
			bool flag = true;
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsDataRegionInPageSection, Severity.Error, context.ObjectType, context.ObjectName, null);
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
						subReport.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, this.m_errorContext);
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
						subReport.ZIndex = this.m_reader.ReadInteger();
						break;
					case "Visibility":
						subReport.Visibility = this.ReadVisibility(context);
						break;
					case "ToolTip":
						subReport.ToolTip = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "Label":
						subReport.Label = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "Bookmark":
						subReport.Bookmark = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "RepeatWith":
						subReport.RepeatedSibling = true;
						subReport.RepeatWith = this.m_reader.ReadString();
						break;
					case "Custom":
						subReport.Custom = this.m_reader.ReadCustomXml();
						break;
					case "CustomProperties":
						subReport.CustomProperties = this.ReadCustomProperties(context);
						break;
					case "ReportName":
						subReport.ReportPath = PublishingValidator.ValidateReportName(this.m_reportContext, this.m_reader.ReadString(), context.ObjectType, context.ObjectName, "ReportName", this.m_errorContext);
						break;
					case "Parameters":
						subReport.Parameters = this.ReadParameters(context, true);
						break;
					case "NoRows":
						subReport.NoRows = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "MergeTransactions":
						subReport.MergeTransactions = this.m_reader.ReadBoolean();
						if (subReport.MergeTransactions)
						{
							this.m_subReportMergeTransactions = true;
						}
						break;
					case "DataElementName":
						subReport.DataElementName = this.m_reader.ReadString();
						break;
					case "DataElementOutput":
						subReport.DataElementOutputRDL = this.ReadDataElementOutputRDL();
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
			subReport.Computed = true;
			if (flag)
			{
				this.m_subReports.Add(subReport);
				this.m_parametersNotUsedInQuery = false;
				return subReport;
			}
			return null;
		}

		private ActiveXControl ReadActiveXControl(ReportItem parent, PublishingContextStruct context)
		{
			ActiveXControl activeXControl = new ActiveXControl(this.GenerateID(), parent);
			activeXControl.Name = this.m_reader.GetAttribute("Name");
			context.ObjectType = activeXControl.ObjectType;
			context.ObjectName = activeXControl.Name;
			this.m_reportItemNames.Validate(context.ObjectType, context.ObjectName, this.m_errorContext);
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = false;
			bool flag8 = false;
			ExpressionInfo expressionInfo = null;
			ExpressionInfo expressionInfo2 = null;
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
						styleInformation.Filter(StyleOwnerType.ActiveXControl, false);
						activeXControl.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, this.m_errorContext);
						break;
					}
					case "Top":
						activeXControl.Top = this.ReadSize();
						break;
					case "Left":
						activeXControl.Left = this.ReadSize();
						break;
					case "Height":
						activeXControl.Height = this.ReadSize();
						break;
					case "Width":
						activeXControl.Width = this.ReadSize();
						break;
					case "ZIndex":
						activeXControl.ZIndex = this.m_reader.ReadInteger();
						break;
					case "Visibility":
						activeXControl.Visibility = this.ReadVisibility(context, out flag2);
						break;
					case "ToolTip":
						activeXControl.ToolTip = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out flag5);
						break;
					case "Label":
						expressionInfo = (activeXControl.Label = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out flag3));
						break;
					case "Bookmark":
						expressionInfo2 = (activeXControl.Bookmark = this.ReadBookmarkExpression(context, out flag4));
						break;
					case "RepeatWith":
						activeXControl.RepeatedSibling = true;
						activeXControl.RepeatWith = this.m_reader.ReadString();
						break;
					case "Custom":
						activeXControl.Custom = this.m_reader.ReadCustomXml();
						break;
					case "CustomProperties":
						activeXControl.CustomProperties = this.ReadCustomProperties(context, out flag7);
						break;
					case "ClassID":
						activeXControl.ClassID = this.m_reader.ReadString();
						break;
					case "CodeBase":
						activeXControl.CodeBase = this.m_reader.ReadString();
						break;
					case "Parameters":
						activeXControl.Parameters = this.ReadParameters(context, false, true, out flag6);
						break;
					case "DataElementName":
						activeXControl.DataElementName = this.m_reader.ReadString();
						break;
					case "DataElementOutput":
						activeXControl.DataElementOutputRDL = this.ReadDataElementOutputRDL();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("ActiveXControl" == this.m_reader.LocalName)
					{
						flag8 = true;
					}
					break;
				}
			}
			while (!flag8);
			activeXControl.Computed = (flag | flag2 | flag3 | flag7 | flag4 | flag5 | flag6);
			if (!flag3 && expressionInfo != null && expressionInfo.Value != null)
			{
				this.m_hasLabels = true;
			}
			if (!flag4 && expressionInfo2 != null && expressionInfo2.Value != null)
			{
				this.m_hasBookmarks = true;
			}
			return activeXControl;
		}

		private ExpressionInfo ReadBookmarkExpression(PublishingContextStruct context, out bool computedBookmark)
		{
			ExpressionInfo result = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out computedBookmark);
			if ((context.Location & LocationFlags.InPageSection) > (LocationFlags)0)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsBookmarkInPageSection, Severity.Warning, context.ObjectType, context.ObjectName, null);
			}
			return result;
		}

		private List ReadList(ReportItem parent, PublishingContextStruct context)
		{
			List list = new List(this.GenerateID(), this.GenerateID(), this.GenerateID(), parent);
			list.Name = this.m_reader.GetAttribute("Name");
			bool flag = (context.Location & LocationFlags.InDataRegion) == (LocationFlags)0;
			context.Location = (context.Location | LocationFlags.InDataSet | LocationFlags.InDataRegion);
			context.ObjectType = list.ObjectType;
			context.ObjectName = list.Name;
			TextBoxList textBoxList = new TextBoxList();
			this.m_dataRegionCount++;
			this.m_reportItemNames.Validate(context.ObjectType, context.ObjectName, this.m_errorContext);
			if (this.m_scopeNames.Validate(false, context.ObjectName, context.ObjectType, context.ObjectName, this.m_errorContext))
			{
				this.m_reportScopes.Add(list.Name, list);
			}
			bool flag2 = true;
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsDataRegionInPageSection, Severity.Error, context.ObjectType, context.ObjectName, null);
				flag2 = false;
			}
			if ((context.Location & LocationFlags.InDetail) != 0)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsDataRegionInTableDetailRow, Severity.Error, context.ObjectType, context.ObjectName, null);
				flag2 = false;
			}
			this.m_reportItemCollectionList.Add(list.ReportItems);
			this.m_aggregateHolderList.Add(list);
			this.m_runningValueHolderList.Add(list.ReportItems);
			StyleInformation styleInformation = null;
			int numberOfAggregates = this.m_reportCT.NumberOfAggregates;
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
							styleInformation = this.ReadStyle(context);
							break;
						case "Top":
							list.Top = this.ReadSize();
							break;
						case "Left":
							list.Left = this.ReadSize();
							break;
						case "Height":
							list.Height = this.ReadSize();
							break;
						case "Width":
							list.Width = this.ReadSize();
							break;
						case "ZIndex":
							list.ZIndex = this.m_reader.ReadInteger();
							break;
						case "Visibility":
							list.Visibility = this.ReadVisibility(context);
							break;
						case "ToolTip":
							list.ToolTip = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
							break;
						case "Label":
							list.Label = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
							break;
						case "Bookmark":
							list.Bookmark = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
							break;
						case "RepeatWith":
							list.RepeatedSibling = true;
							list.RepeatWith = this.m_reader.ReadString();
							break;
						case "Custom":
							list.Custom = this.m_reader.ReadCustomXml();
							break;
						case "CustomProperties":
							list.CustomProperties = this.ReadCustomProperties(context);
							break;
						case "KeepTogether":
							list.KeepTogether = this.m_reader.ReadBoolean();
							break;
						case "NoRows":
							list.NoRows = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
							break;
						case "DataSetName":
						{
							string dataSetName = this.m_reader.ReadString();
							if (flag)
							{
								list.DataSetName = dataSetName;
							}
							break;
						}
						case "PageBreakAtStart":
							list.PageBreakAtStart = this.m_reader.ReadBoolean();
							break;
						case "PageBreakAtEnd":
							list.PageBreakAtEnd = this.m_reader.ReadBoolean();
							break;
						case "Filters":
							list.Filters = this.ReadFilters(ExpressionParser.ExpressionType.DataRegionFilters, context);
							break;
						case "Grouping":
							list.Grouping = this.ReadGrouping(context);
							break;
						case "Sorting":
							list.Sorting = this.ReadSorting(context);
							break;
						case "ReportItems":
							this.ReadReportItems(null, list, list.ReportItems, context, textBoxList);
							break;
						case "FillPage":
							list.FillPage = this.m_reader.ReadBoolean();
							break;
						case "DataElementName":
							list.DataElementName = this.m_reader.ReadString();
							break;
						case "DataElementOutput":
							list.DataElementOutputRDL = this.ReadDataElementOutputRDL();
							break;
						case "DataInstanceName":
							list.DataInstanceName = this.m_reader.ReadString();
							break;
						case "DataInstanceElementOutput":
							list.DataInstanceElementOutput = this.ReadDataElementOutput();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("List" == this.m_reader.LocalName)
						{
							flag3 = true;
						}
						break;
					}
				}
				while (!flag3);
			}
			if (list.Grouping == null)
			{
				if (this.m_reportCT.NumberOfAggregates > numberOfAggregates)
				{
					this.m_aggregateInDetailSections = true;
				}
				this.SetSortTargetForTextBoxes(textBoxList, list);
			}
			else
			{
				this.SetSortTargetForTextBoxes(textBoxList, list.Grouping);
			}
			if (this.CanMergeGroupingAndSorting(list.Grouping, list.Sorting))
			{
				list.Grouping.GroupAndSort = true;
				list.Grouping.SortDirections = list.Sorting.SortDirections;
				list.Sorting = null;
			}
			if (list.Sorting != null)
			{
				this.m_hasSorting = true;
			}
			if (styleInformation != null)
			{
				styleInformation.Filter(StyleOwnerType.List, null != list.NoRows);
				list.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, this.m_errorContext);
			}
			list.Computed = true;
			if (!flag2)
			{
				return null;
			}
			return list;
		}

		private Matrix ReadMatrix(ReportItem parent, PublishingContextStruct context)
		{
			Matrix matrix = new Matrix(this.GenerateID(), this.GenerateID(), this.GenerateID(), parent);
			matrix.Name = this.m_reader.GetAttribute("Name");
			bool flag = (context.Location & LocationFlags.InDataRegion) == (LocationFlags)0;
			context.Location = (context.Location | LocationFlags.InDataSet | LocationFlags.InDataRegion);
			context.ObjectType = matrix.ObjectType;
			context.ObjectName = matrix.Name;
			this.m_dataRegionCount++;
			this.m_reportItemNames.Validate(context.ObjectType, context.ObjectName, this.m_errorContext);
			if (this.m_scopeNames.Validate(false, context.ObjectName, context.ObjectType, context.ObjectName, this.m_errorContext))
			{
				this.m_reportScopes.Add(matrix.Name, matrix);
			}
			bool flag2 = true;
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsDataRegionInPageSection, Severity.Error, context.ObjectType, context.ObjectName, null);
				flag2 = false;
			}
			if ((context.Location & LocationFlags.InDetail) != 0)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsDataRegionInTableDetailRow, Severity.Error, context.ObjectType, context.ObjectName, null);
				flag2 = false;
			}
			this.m_reportItemCollectionList.Add(matrix.CornerReportItems);
			this.m_reportItemCollectionList.Add(matrix.CellReportItems);
			this.m_aggregateHolderList.Add(matrix);
			this.m_runningValueHolderList.Add(matrix);
			this.m_runningValueHolderList.Add(matrix.CornerReportItems);
			this.m_runningValueHolderList.Add(matrix.CellReportItems);
			StyleInformation styleInformation = null;
			bool flag3 = false;
			TextBoxList textBoxList = new TextBoxList();
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "Style":
						styleInformation = this.ReadStyle(context);
						break;
					case "Top":
						matrix.Top = this.ReadSize();
						break;
					case "Left":
						matrix.Left = this.ReadSize();
						break;
					case "Height":
						matrix.Height = this.ReadSize();
						break;
					case "Width":
						matrix.Width = this.ReadSize();
						break;
					case "ZIndex":
						matrix.ZIndex = this.m_reader.ReadInteger();
						break;
					case "Visibility":
						matrix.Visibility = this.ReadVisibility(context);
						break;
					case "ToolTip":
						matrix.ToolTip = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "Label":
						matrix.Label = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "Bookmark":
						matrix.Bookmark = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "RepeatWith":
						matrix.RepeatedSibling = true;
						matrix.RepeatWith = this.m_reader.ReadString();
						break;
					case "Custom":
						matrix.Custom = this.m_reader.ReadCustomXml();
						break;
					case "CustomProperties":
						matrix.CustomProperties = this.ReadCustomProperties(context);
						break;
					case "DataElementName":
						matrix.DataElementName = this.m_reader.ReadString();
						break;
					case "DataElementOutput":
						matrix.DataElementOutputRDL = this.ReadDataElementOutputRDL();
						break;
					case "CellDataElementName":
						matrix.CellDataElementName = this.m_reader.ReadString();
						break;
					case "CellDataElementOutput":
						matrix.CellDataElementOutput = this.ReadDataElementOutput();
						break;
					case "KeepTogether":
						matrix.KeepTogether = this.m_reader.ReadBoolean();
						break;
					case "NoRows":
						matrix.NoRows = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "DataSetName":
					{
						string dataSetName = this.m_reader.ReadString();
						if (flag)
						{
							matrix.DataSetName = dataSetName;
						}
						break;
					}
					case "PageBreakAtStart":
						matrix.PageBreakAtStart = this.m_reader.ReadBoolean();
						break;
					case "PageBreakAtEnd":
						matrix.PageBreakAtEnd = this.m_reader.ReadBoolean();
						break;
					case "Filters":
						matrix.Filters = this.ReadFilters(ExpressionParser.ExpressionType.DataRegionFilters, context);
						break;
					case "Corner":
						this.ReadCorner(matrix, context, textBoxList);
						break;
					case "ColumnGroupings":
						this.ReadColumnGroupings(matrix, context, textBoxList);
						break;
					case "RowGroupings":
						this.ReadRowGroupings(matrix, context, textBoxList);
						break;
					case "MatrixRows":
						matrix.MatrixRows = this.ReadMatrixRows(matrix, context, textBoxList);
						break;
					case "MatrixColumns":
						matrix.MatrixColumns = this.ReadMatrixColumns();
						break;
					case "LayoutDirection":
						matrix.LayoutDirection = this.ReadLayoutDirection();
						break;
					case "GroupsBeforeRowHeaders":
						matrix.GroupsBeforeRowHeaders = this.m_reader.ReadInteger();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Matrix" == this.m_reader.LocalName)
					{
						flag3 = true;
					}
					break;
				}
			}
			while (!flag3);
			matrix.CalculatePropagatedFlags();
			if (!flag && (matrix.RowGroupingFixedHeader || matrix.ColumnGroupingFixedHeader))
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsFixedHeadersInInnerDataRegion, Severity.Error, context.ObjectType, context.ObjectName, "FixedHeader");
			}
			if (styleInformation != null)
			{
				styleInformation.Filter(StyleOwnerType.Matrix, null != matrix.NoRows);
				matrix.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, this.m_errorContext);
			}
			this.SetSortTargetForTextBoxes(textBoxList, matrix);
			matrix.Computed = true;
			if (!flag2)
			{
				return null;
			}
			return matrix;
		}

		private void ReadCorner(Matrix matrix, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
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
					if ((localName = this.m_reader.LocalName) != null && localName == "ReportItems")
					{
						this.ReadReportItems("Corner", matrix, matrix.CornerReportItems, context, textBoxesWithDefaultSortTarget);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("Corner" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private void ReadColumnGroupings(Matrix matrix, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			MatrixHeading matrixHeading = null;
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "ColumnGrouping")
					{
						TextBoxList textBoxList = new TextBoxList();
						bool flag2 = true;
						MatrixHeading matrixHeading2 = this.ReadColumnOrRowGrouping(true, matrix, context, textBoxList);
						if (matrixHeading != null)
						{
							matrixHeading.SubHeading = matrixHeading2;
							if (matrixHeading.Grouping != null)
							{
								this.SetSortTargetForTextBoxes(textBoxList, matrixHeading.Grouping);
								flag2 = false;
							}
						}
						else
						{
							matrix.Columns = matrixHeading2;
						}
						if (flag2)
						{
							textBoxesWithDefaultSortTarget.AddRange(textBoxList);
						}
						matrixHeading = matrixHeading2;
						matrix.ColumnCount++;
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("ColumnGroupings" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private MatrixHeading ReadColumnOrRowGrouping(bool isColumn, Matrix matrix, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			MatrixHeading matrixHeading = new MatrixHeading(this.GenerateID(), this.GenerateID(), matrix);
			this.m_reportItemCollectionList.Add(matrixHeading.ReportItems);
			this.m_runningValueHolderList.Add(matrixHeading.ReportItems);
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
					case "FixedHeader":
						if (isColumn)
						{
							matrix.ColumnGroupingFixedHeader = this.m_reader.ReadBoolean();
						}
						else
						{
							matrix.RowGroupingFixedHeader = this.m_reader.ReadBoolean();
						}
						break;
					case "Height":
						Global.Tracer.Assert(isColumn);
						matrixHeading.Size = this.ReadSize();
						break;
					case "Width":
						Global.Tracer.Assert(!isColumn);
						matrixHeading.Size = this.ReadSize();
						break;
					case "DynamicColumns":
						flag = true;
						Global.Tracer.Assert(isColumn);
						this.ReadDynamicColumnsOrRows(isColumn, matrix, matrixHeading, context, textBoxesWithDefaultSortTarget);
						break;
					case "DynamicRows":
						flag = true;
						Global.Tracer.Assert(!isColumn);
						this.ReadDynamicColumnsOrRows(isColumn, matrix, matrixHeading, context, textBoxesWithDefaultSortTarget);
						break;
					case "StaticColumns":
						flag2 = true;
						Global.Tracer.Assert(isColumn);
						this.ReadStaticColumnsOrRows(isColumn, matrix, matrixHeading, context, textBoxesWithDefaultSortTarget);
						break;
					case "StaticRows":
						flag2 = true;
						Global.Tracer.Assert(!isColumn);
						this.ReadStaticColumnsOrRows(isColumn, matrix, matrixHeading, context, textBoxesWithDefaultSortTarget);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if (!isColumn || !("ColumnGrouping" == this.m_reader.LocalName))
					{
						if (isColumn)
						{
							break;
						}
						if (!("RowGrouping" == this.m_reader.LocalName))
						{
							break;
						}
					}
					flag3 = true;
					break;
				}
			}
			while (!flag3);
			if (flag == flag2)
			{
				if (isColumn)
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsInvalidColumnGrouping, Severity.Error, context.ObjectType, context.ObjectName, "ColumnGrouping");
				}
				else
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsInvalidRowGrouping, Severity.Error, context.ObjectType, context.ObjectName, "RowGrouping");
				}
			}
			if (isColumn && matrixHeading.Grouping != null && (matrixHeading.Grouping.PageBreakAtStart || matrixHeading.Grouping.PageBreakAtEnd))
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsPageBreakOnMatrixColumnGroup, Severity.Warning, context.ObjectType, context.ObjectName, "ColumnGrouping", matrixHeading.Grouping.Name);
			}
			return matrixHeading;
		}

		private void ReadRowGroupings(Matrix matrix, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			MatrixHeading matrixHeading = null;
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "RowGrouping")
					{
						TextBoxList textBoxList = new TextBoxList();
						bool flag2 = true;
						MatrixHeading matrixHeading2 = this.ReadColumnOrRowGrouping(false, matrix, context, textBoxList);
						if (matrixHeading != null)
						{
							matrixHeading.SubHeading = matrixHeading2;
							if (matrixHeading.Grouping != null)
							{
								this.SetSortTargetForTextBoxes(textBoxList, matrixHeading.Grouping);
								flag2 = false;
							}
						}
						else
						{
							matrix.Rows = matrixHeading2;
						}
						if (flag2)
						{
							textBoxesWithDefaultSortTarget.AddRange(textBoxList);
						}
						matrixHeading = matrixHeading2;
						matrix.RowCount++;
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("RowGroupings" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private void ReadDynamicColumnsOrRows(bool isColumns, Matrix matrix, MatrixHeading heading, PublishingContextStruct context, TextBoxList subtotalTextBoxesWithDefaultSortTarget)
		{
			bool flag = false;
			TextBoxList textBoxList = new TextBoxList();
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "Grouping":
						heading.Grouping = this.ReadGrouping(context);
						break;
					case "Sorting":
						heading.Sorting = this.ReadSorting(context);
						break;
					case "Subtotal":
						heading.Subtotal = this.ReadSubtotal(matrix, context, subtotalTextBoxesWithDefaultSortTarget);
						break;
					case "ReportItems":
						this.ReadReportItems(isColumns ? "DynamicColumns" : "DynamicRows", matrix, heading.ReportItems, context, textBoxList);
						break;
					case "Visibility":
						heading.Visibility = this.ReadVisibility(context);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if (!("DynamicColumns" == this.m_reader.LocalName) && !("DynamicRows" == this.m_reader.LocalName))
					{
						break;
					}
					flag = true;
					break;
				}
			}
			while (!flag);
			if (this.CanMergeGroupingAndSorting(heading.Grouping, heading.Sorting))
			{
				heading.Grouping.GroupAndSort = true;
				heading.Grouping.SortDirections = heading.Sorting.SortDirections;
				heading.Sorting = null;
			}
			if (heading.Sorting != null)
			{
				this.m_hasSorting = true;
			}
			if (heading.Subtotal == null && heading.Visibility != null)
			{
				heading.Subtotal = new Subtotal(this.GenerateID(), this.GenerateID(), true);
				this.m_reportItemCollectionList.Add(heading.Subtotal.ReportItems);
				this.m_runningValueHolderList.Add(heading.Subtotal.ReportItems);
			}
			Global.Tracer.Assert(null != heading.Grouping);
			this.SetSortTargetForTextBoxes(textBoxList, heading.Grouping);
		}

		private void ReadStaticColumnsOrRows(bool isColumn, Matrix matrix, MatrixHeading heading, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			if (isColumn)
			{
				if (matrix.StaticColumns == null)
				{
					matrix.StaticColumns = heading;
				}
				else
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsMultiStaticColumnsOrRows, Severity.Error, context.ObjectType, context.ObjectName, "StaticColumns");
				}
			}
			else if (matrix.StaticRows == null)
			{
				matrix.StaticRows = heading;
			}
			else
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsMultiStaticColumnsOrRows, Severity.Error, context.ObjectType, context.ObjectName, "StaticRows");
			}
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "StaticColumn":
						Global.Tracer.Assert(isColumn);
						this.ReadStaticColumn(matrix, heading, context, textBoxesWithDefaultSortTarget);
						heading.NumberOfStatics++;
						break;
					case "StaticRow":
						Global.Tracer.Assert(!isColumn);
						this.ReadStaticRow(matrix, heading, context, textBoxesWithDefaultSortTarget);
						heading.NumberOfStatics++;
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if (!("StaticColumns" == this.m_reader.LocalName) && !("StaticRows" == this.m_reader.LocalName))
					{
						break;
					}
					flag = true;
					break;
				}
			}
			while (!flag);
			heading.IDs = new IntList();
			for (int i = 0; i < heading.ReportItems.Count; i++)
			{
				heading.IDs.Add(this.GenerateID());
			}
		}

		private void ReadStaticColumn(Matrix matrix, MatrixHeading heading, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
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
					if ((localName = this.m_reader.LocalName) != null && localName == "ReportItems")
					{
						this.ReadReportItems("StaticColumn", matrix, heading.ReportItems, context, textBoxesWithDefaultSortTarget);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("StaticColumn" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private void ReadStaticRow(Matrix matrix, MatrixHeading heading, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
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
					if ((localName = this.m_reader.LocalName) != null && localName == "ReportItems")
					{
						this.ReadReportItems("StaticRow", matrix, heading.ReportItems, context, textBoxesWithDefaultSortTarget);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("StaticRow" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private Subtotal ReadSubtotal(Matrix matrix, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			bool computed = false;
			Subtotal subtotal = new Subtotal(this.GenerateID(), this.GenerateID(), false);
			this.m_reportItemCollectionList.Add(subtotal.ReportItems);
			this.m_runningValueHolderList.Add(subtotal.ReportItems);
			context.Location |= LocationFlags.InMatrixSubtotal;
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
						this.ReadReportItems("Subtotal", matrix, subtotal.ReportItems, context, textBoxesWithDefaultSortTarget);
						break;
					case "Style":
					{
						StyleInformation styleInformation = this.ReadStyle(context, out computed);
						styleInformation.Filter(StyleOwnerType.Subtotal, false);
						subtotal.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, this.m_errorContext);
						subtotal.Computed = computed;
						break;
					}
					case "Position":
						subtotal.Position = this.ReadPosition();
						break;
					case "DataElementName":
						subtotal.DataElementName = this.m_reader.ReadString();
						break;
					case "DataElementOutput":
						subtotal.DataElementOutput = this.ReadDataElementOutput();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Subtotal" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return subtotal;
		}

		private MatrixRowList ReadMatrixRows(Matrix matrix, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			MatrixRowList matrixRowList = new MatrixRowList();
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "MatrixRow")
					{
						matrixRowList.Add(this.ReadMatrixRow(matrix, context, textBoxesWithDefaultSortTarget));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("MatrixRows" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			matrix.CellIDs = new IntList();
			for (int i = 0; i < matrix.CellReportItems.Count; i++)
			{
				matrix.CellIDs.Add(this.GenerateID());
			}
			return matrixRowList;
		}

		private MatrixRow ReadMatrixRow(Matrix matrix, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			MatrixRow matrixRow = new MatrixRow();
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
						matrixRow.Height = this.ReadSize();
						break;
					case "MatrixCells":
						matrixRow.NumberOfMatrixCells = this.ReadMatrixCells(matrix, context, textBoxesWithDefaultSortTarget);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("MatrixRow" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return matrixRow;
		}

		private int ReadMatrixCells(Matrix matrix, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
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
					if ((localName = this.m_reader.LocalName) != null && localName == "MatrixCell")
					{
						this.ReadMatrixCell(matrix, context, textBoxesWithDefaultSortTarget);
						num++;
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("MatrixCells" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return num;
		}

		private void ReadMatrixCell(Matrix matrix, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			context.Location |= LocationFlags.InMatrixCell;
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "ReportItems")
					{
						this.ReadReportItems("MatrixCell", matrix, matrix.CellReportItems, context, textBoxesWithDefaultSortTarget);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("MatrixCell" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private MatrixColumnList ReadMatrixColumns()
		{
			MatrixColumnList matrixColumnList = new MatrixColumnList();
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "MatrixColumn")
					{
						matrixColumnList.Add(this.ReadMatrixColumn());
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("MatrixColumns" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return matrixColumnList;
		}

		private MatrixColumn ReadMatrixColumn()
		{
			MatrixColumn matrixColumn = new MatrixColumn();
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
						matrixColumn.Width = this.ReadSize();
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("MatrixColumn" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return matrixColumn;
		}

		private Chart ReadChart(ReportItem parent, PublishingContextStruct context)
		{
			Chart chart = new Chart(this.GenerateID(), parent);
			chart.Name = this.m_reader.GetAttribute("Name");
			bool flag = (context.Location & LocationFlags.InDataRegion) == (LocationFlags)0;
			context.Location = (context.Location | LocationFlags.InDataSet | LocationFlags.InDataRegion);
			context.ObjectType = chart.ObjectType;
			context.ObjectName = chart.Name;
			this.m_dataRegionCount++;
			this.m_reportItemNames.Validate(context.ObjectType, context.ObjectName, this.m_errorContext);
			if (this.m_scopeNames.Validate(false, context.ObjectName, context.ObjectType, context.ObjectName, this.m_errorContext))
			{
				this.m_reportScopes.Add(chart.Name, chart);
			}
			bool flag2 = true;
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsDataRegionInPageSection, Severity.Error, context.ObjectType, context.ObjectName, null);
				flag2 = false;
			}
			this.m_aggregateHolderList.Add(chart);
			this.m_runningValueHolderList.Add(chart);
			StyleInformation styleInformation = null;
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
							chart.ZIndex = this.m_reader.ReadInteger();
							break;
						case "Visibility":
							chart.Visibility = this.ReadVisibility(context);
							break;
						case "ToolTip":
							chart.ToolTip = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
							break;
						case "Label":
							chart.Label = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
							break;
						case "Bookmark":
							chart.Bookmark = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
							break;
						case "RepeatWith":
							chart.RepeatedSibling = true;
							chart.RepeatWith = this.m_reader.ReadString();
							break;
						case "Custom":
							chart.Custom = this.m_reader.ReadCustomXml();
							break;
						case "CustomProperties":
							chart.CustomProperties = this.ReadCustomProperties(context);
							break;
						case "DataElementName":
							chart.DataElementName = this.m_reader.ReadString();
							break;
						case "DataElementOutput":
							chart.DataElementOutputRDL = this.ReadDataElementOutputRDL();
							break;
						case "ChartElementOutput":
							chart.CellDataElementOutput = this.ReadDataElementOutput();
							break;
						case "KeepTogether":
							chart.KeepTogether = this.m_reader.ReadBoolean();
							break;
						case "NoRows":
							chart.NoRows = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
							break;
						case "DataSetName":
						{
							string dataSetName = this.m_reader.ReadString();
							if (flag)
							{
								chart.DataSetName = dataSetName;
							}
							break;
						}
						case "PageBreakAtStart":
							chart.PageBreakAtStart = this.m_reader.ReadBoolean();
							break;
						case "PageBreakAtEnd":
							chart.PageBreakAtEnd = this.m_reader.ReadBoolean();
							break;
						case "Filters":
							chart.Filters = this.ReadFilters(ExpressionParser.ExpressionType.DataRegionFilters, context);
							break;
						case "Type":
							chart.Type = this.ReadChartType();
							break;
						case "Subtype":
							chart.SubType = this.ReadChartSubType();
							break;
						case "SeriesGroupings":
							this.ReadSeriesGroupings(chart, context);
							break;
						case "CategoryGroupings":
							this.ReadCategoryGroupings(chart, context);
							break;
						case "ChartData":
							this.ReadChartData(chart, context);
							break;
						case "Legend":
							chart.Legend = this.ReadLegend(context);
							break;
						case "CategoryAxis":
							chart.CategoryAxis = this.ReadCategoryOrValueAxis(chart, context);
							break;
						case "ValueAxis":
							chart.ValueAxis = this.ReadCategoryOrValueAxis(chart, context);
							break;
						case "MultiChart":
							chart.MultiChart = this.ReadMultiChart(chart, context);
							break;
						case "Title":
							chart.Title = this.ReadChartTitle(context);
							break;
						case "PointWidth":
							chart.PointWidth = this.m_reader.ReadInteger();
							break;
						case "Palette":
							chart.Palette = this.ReadChartPalette();
							break;
						case "ThreeDProperties":
							chart.ThreeDProperties = this.ReadThreeDProperties(chart, context);
							break;
						case "PlotArea":
							chart.PlotArea = this.ReadPlotArea(chart, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("Chart" == this.m_reader.LocalName)
						{
							flag3 = true;
						}
						break;
					}
				}
				while (!flag3);
			}
			if (!chart.IsValidChartSubType())
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsInvalidChartSubType, Severity.Error, context.ObjectType, context.ObjectName, null, Enum.GetName(typeof(Chart.ChartTypes), chart.Type), Enum.GetName(typeof(Chart.ChartSubTypes), chart.SubType));
			}
			if (Chart.ChartTypes.Pie == chart.Type || Chart.ChartTypes.Doughnut == chart.Type)
			{
				chart.CategoryAxis = null;
				chart.ValueAxis = null;
				if (chart.Rows != null)
				{
					if (chart.StaticRows != null && chart.StaticColumns != null)
					{
						this.m_errorContext.Register(ProcessingErrorCode.rsInvalidChartGroupings, Severity.Error, context.ObjectType, context.ObjectName, null);
					}
					else
					{
						ChartHeading chartHeading = chart.Columns;
						while (chartHeading != null && chartHeading.SubHeading != null)
						{
							chartHeading = chartHeading.SubHeading;
						}
						if (chartHeading == null)
						{
							chart.Columns = chart.Rows;
						}
						else
						{
							chartHeading.SubHeading = chart.Rows;
						}
						if (chart.StaticRows != null)
						{
							chart.StaticColumns = chart.StaticRows;
							chart.StaticRows = null;
						}
						Global.Tracer.Assert(null != chart.NumberOfSeriesDataPoints);
						int num = 0;
						for (int i = 0; i < chart.NumberOfSeriesDataPoints.Count; i++)
						{
							num += chart.NumberOfSeriesDataPoints[i];
						}
						chart.NumberOfSeriesDataPoints = new IntList(1);
						chart.NumberOfSeriesDataPoints.Add(num);
						chart.ColumnCount += chart.RowCount;
						chart.RowCount = 0;
						chart.Rows = null;
					}
				}
			}
			if (styleInformation != null)
			{
				styleInformation.Filter(StyleOwnerType.Chart, null != chart.NoRows);
				chart.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, this.m_errorContext);
			}
			if (chart.Columns == null)
			{
				if ((chart.Type != Chart.ChartTypes.Bubble && chart.Type != Chart.ChartTypes.Scatter) || chart.HasDataValueAggregates)
				{
					this.ChartFakeStaticCategory(chart);
					goto IL_090b;
				}
				this.ChartAddRowNumberCategory(chart, context);
			}
			goto IL_090b;
			IL_090b:
			if (chart.Rows == null)
			{
				this.ChartFakeStaticSeries(chart);
			}
			chart.Computed = true;
			if (flag2)
			{
				this.m_hasImageStreams = true;
				return chart;
			}
			return null;
		}

		private void ReadCategoryGroupings(Chart chart, PublishingContextStruct context)
		{
			ChartHeading chartHeading = null;
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "CategoryGrouping")
					{
						ChartHeading chartHeading2 = this.ReadCategoryOrSeriesGrouping(true, chart, context);
						if (chartHeading != null)
						{
							chartHeading.SubHeading = chartHeading2;
						}
						else
						{
							chart.Columns = chartHeading2;
						}
						chartHeading = chartHeading2;
						chart.ColumnCount++;
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("CategoryGroupings" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private void ChartAddRowNumberCategory(Chart chart, PublishingContextStruct context)
		{
			Global.Tracer.Assert(null != chart);
			Global.Tracer.Assert(0 == chart.ColumnCount);
			chart.ColumnCount++;
			chart.Columns = new ChartHeading(this.GenerateID(), chart);
			this.m_hasGrouping = true;
			Grouping grouping = new Grouping(ConstructionPhase.Publishing);
			grouping.Name = "0_" + chart.Name + "_AutoGenerated_RowNumber_Category";
			if (this.m_scopeNames.Validate(true, grouping.Name, context.ObjectType, context.ObjectName, this.m_errorContext, false))
			{
				this.m_reportScopes.Add(grouping.Name, grouping);
			}
			this.m_aggregateHolderList.Add(grouping);
			chart.Columns.Grouping = grouping;
			this.m_runningValueHolderList.Add(chart.Columns);
			ExpressionParser.ExpressionContext context2 = context.CreateExpressionContext(ExpressionParser.ExpressionType.GroupExpression, ExpressionParser.ConstantType.String, "CategoryGrouping", null);
			bool flag = default(bool);
			grouping.GroupExpressions.Add(this.m_reportCT.ParseExpression("=RowNumber(\"" + chart.Name + "\")", context2, out flag));
			Global.Tracer.Assert(!flag);
		}

		private void ChartFakeStaticSeries(Chart chart)
		{
			Global.Tracer.Assert(null != chart);
			Global.Tracer.Assert(0 == chart.RowCount);
			chart.RowCount++;
			chart.Rows = new ChartHeading(this.GenerateID(), chart);
			chart.Rows.NumberOfStatics++;
			chart.StaticRows = chart.Rows;
		}

		private void ChartFakeStaticCategory(Chart chart)
		{
			Global.Tracer.Assert(null != chart);
			Global.Tracer.Assert(0 == chart.ColumnCount);
			chart.ColumnCount++;
			chart.Columns = new ChartHeading(this.GenerateID(), chart);
			chart.Columns.NumberOfStatics++;
			chart.StaticColumns = chart.Columns;
		}

		private ChartHeading ReadCategoryOrSeriesGrouping(bool isCategory, Chart chart, PublishingContextStruct context)
		{
			ChartHeading chartHeading = new ChartHeading(this.GenerateID(), chart);
			this.m_runningValueHolderList.Add(chartHeading);
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
						case "DynamicCategories":
							flag = true;
							Global.Tracer.Assert(isCategory);
							this.ReadDynamicCategoriesOrSeries(isCategory, chart, chartHeading, context);
							break;
						case "DynamicSeries":
							flag = true;
							Global.Tracer.Assert(!isCategory);
							this.ReadDynamicCategoriesOrSeries(isCategory, chart, chartHeading, context);
							break;
						case "StaticCategories":
							flag2 = true;
							Global.Tracer.Assert(isCategory);
							this.ReadStaticCategoriesOrSeries(isCategory, chart, chartHeading, context);
							break;
						case "StaticSeries":
							flag2 = true;
							Global.Tracer.Assert(!isCategory);
							this.ReadStaticCategoriesOrSeries(isCategory, chart, chartHeading, context);
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if (!isCategory || !("CategoryGrouping" == this.m_reader.LocalName))
						{
							if (isCategory)
							{
								break;
							}
							if (!("SeriesGrouping" == this.m_reader.LocalName))
							{
								break;
							}
						}
						flag3 = true;
						break;
					}
				}
				while (!flag3);
			}
			if (flag == flag2)
			{
				if (isCategory)
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsInvalidCategoryGrouping, Severity.Error, context.ObjectType, context.ObjectName, "CategoryGrouping");
				}
				else
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsInvalidSeriesGrouping, Severity.Error, context.ObjectType, context.ObjectName, "SeriesGrouping");
				}
			}
			if (chartHeading.Grouping != null && (chartHeading.Grouping.PageBreakAtStart || chartHeading.Grouping.PageBreakAtEnd))
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsPageBreakOnChartGroup, Severity.Warning, context.ObjectType, context.ObjectName, isCategory ? "CategoryGroupings" : "SeriesGroupings", chartHeading.Grouping.Name);
			}
			return chartHeading;
		}

		private void ReadSeriesGroupings(Chart chart, PublishingContextStruct context)
		{
			ChartHeading chartHeading = null;
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "SeriesGrouping")
					{
						ChartHeading chartHeading2 = this.ReadCategoryOrSeriesGrouping(false, chart, context);
						if (chartHeading != null)
						{
							chartHeading.SubHeading = chartHeading2;
						}
						else
						{
							chart.Rows = chartHeading2;
						}
						chartHeading = chartHeading2;
						chart.RowCount++;
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("SeriesGroupings" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private void ReadDynamicCategoriesOrSeries(bool isCategory, Chart chart, ChartHeading heading, PublishingContextStruct context)
		{
			bool flag = false;
			ExpressionInfo expressionInfo = null;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "Grouping":
						heading.Grouping = this.ReadGrouping(context);
						break;
					case "Sorting":
						heading.Sorting = this.ReadSorting(context);
						break;
					case "Label":
						expressionInfo = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if (!isCategory || !("DynamicCategories" == this.m_reader.LocalName))
					{
						if (isCategory)
						{
							break;
						}
						if (!("DynamicSeries" == this.m_reader.LocalName))
						{
							break;
						}
					}
					flag = true;
					break;
				}
			}
			while (!flag);
			if (this.CanMergeGroupingAndSorting(heading.Grouping, heading.Sorting))
			{
				heading.Grouping.GroupAndSort = true;
				heading.Grouping.SortDirections = heading.Sorting.SortDirections;
				heading.Sorting = null;
			}
			if (heading.Sorting != null)
			{
				this.m_hasSorting = true;
			}
			if (expressionInfo != null && (ExpressionInfo.Types.Constant != expressionInfo.Type || expressionInfo.Value.Length != 0))
			{
				if (heading.Labels == null)
				{
					heading.Labels = new ExpressionInfoList();
				}
				heading.Labels.Add(expressionInfo);
			}
			else
			{
				if (heading.Labels == null)
				{
					heading.Labels = new ExpressionInfoList();
				}
				Global.Tracer.Assert(heading.Grouping.GroupExpressions != null && null != heading.Grouping.GroupExpressions[0]);
				heading.Labels.Add(heading.Grouping.GroupExpressions[0]);
			}
		}

		private void ReadStaticCategoriesOrSeries(bool isColumn, Chart chart, ChartHeading heading, PublishingContextStruct context)
		{
			if (isColumn)
			{
				if (chart.StaticColumns == null)
				{
					chart.StaticColumns = heading;
				}
				else
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsMultiStaticCategoriesOrSeries, Severity.Error, context.ObjectType, context.ObjectName, "StaticCategories");
				}
			}
			else if (chart.StaticRows == null)
			{
				chart.StaticRows = heading;
			}
			else
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsMultiStaticCategoriesOrSeries, Severity.Error, context.ObjectType, context.ObjectName, "StaticSeries");
			}
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "StaticMember")
					{
						this.ReadStaticMember(chart, heading, context);
						heading.NumberOfStatics++;
					}
					break;
				}
				case XmlNodeType.EndElement:
					if (!("StaticCategories" == this.m_reader.LocalName) && !("StaticSeries" == this.m_reader.LocalName))
					{
						break;
					}
					flag = true;
					break;
				}
			}
			while (!flag);
		}

		private void ReadStaticMember(Chart chart, ChartHeading heading, PublishingContextStruct context)
		{
			bool flag = false;
			ExpressionInfo expressionInfo = null;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "Label")
					{
						expressionInfo = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						if (expressionInfo != null)
						{
							if (heading.Labels == null)
							{
								heading.Labels = new ExpressionInfoList();
							}
							if (ExpressionInfo.Types.Constant == expressionInfo.Type && expressionInfo.Value.Length == 0)
							{
								expressionInfo.Value = null;
							}
							heading.Labels.Add(expressionInfo);
						}
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("StaticMember" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private ChartTitle ReadChartTitle(PublishingContextStruct context)
		{
			ChartTitle chartTitle = new ChartTitle();
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
							chartTitle.Caption = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
							break;
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Textbox, false);
							chartTitle.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, this.m_errorContext);
							break;
						}
						case "Position":
							chartTitle.Position = this.ReadChartTitlePosition();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("Title" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return chartTitle;
		}

		private Axis ReadCategoryOrValueAxis(Chart chart, PublishingContextStruct context)
		{
			Axis result = null;
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
						if ((localName = this.m_reader.LocalName) != null && localName == "Axis")
						{
							result = this.ReadAxis(chart, context);
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (!("CategoryAxis" == this.m_reader.LocalName) && !("ValueAxis" == this.m_reader.LocalName))
						{
							break;
						}
						flag = true;
						break;
					}
				}
				while (!flag);
			}
			return result;
		}

		private Axis ReadAxis(Chart chart, PublishingContextStruct context)
		{
			Axis axis = new Axis();
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
							axis.Visible = this.m_reader.ReadBoolean();
							break;
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Textbox, false);
							axis.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, this.m_errorContext);
							break;
						}
						case "Title":
							axis.Title = this.ReadChartTitle(context);
							break;
						case "Margin":
							axis.Margin = this.m_reader.ReadBoolean();
							break;
						case "MajorTickMarks":
							axis.MajorTickMarks = this.ReadAxisTickMarks();
							break;
						case "MinorTickMarks":
							axis.MinorTickMarks = this.ReadAxisTickMarks();
							break;
						case "MajorGridLines":
							axis.MajorGridLines = this.ReadGridLines(context);
							break;
						case "MinorGridLines":
							axis.MinorGridLines = this.ReadGridLines(context);
							break;
						case "MajorInterval":
							axis.MajorInterval = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
							break;
						case "MinorInterval":
							axis.MinorInterval = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
							break;
						case "Reverse":
							axis.Reverse = this.m_reader.ReadBoolean();
							break;
						case "CrossAt":
							axis.AutoCrossAt = false;
							axis.CrossAt = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
							break;
						case "Interlaced":
							axis.Interlaced = this.m_reader.ReadBoolean();
							break;
						case "Scalar":
							axis.Scalar = this.m_reader.ReadBoolean();
							break;
						case "Min":
							axis.AutoScaleMin = false;
							axis.Min = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
							break;
						case "Max":
							axis.AutoScaleMax = false;
							axis.Max = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
							break;
						case "LogScale":
							axis.LogScale = this.m_reader.ReadBoolean();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("Axis" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return axis;
		}

		private Legend ReadLegend(PublishingContextStruct context)
		{
			Legend legend = new Legend();
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
							legend.Visible = this.m_reader.ReadBoolean();
							break;
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, false);
							legend.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, this.m_errorContext);
							break;
						}
						case "Position":
							legend.Position = this.ReadLegendPosition();
							break;
						case "Layout":
							legend.Layout = this.ReadLegendLayout();
							break;
						case "InsidePlotArea":
							legend.InsidePlotArea = this.m_reader.ReadBoolean();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("Legend" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return legend;
		}

		private GridLines ReadGridLines(PublishingContextStruct context)
		{
			GridLines gridLines = new GridLines();
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
							gridLines.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, this.m_errorContext);
							break;
						}
						case "ShowGridLines":
							gridLines.ShowGridLines = this.m_reader.ReadBoolean();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if (!("MajorGridLines" == this.m_reader.LocalName) && !("MinorGridLines" == this.m_reader.LocalName))
						{
							break;
						}
						flag = true;
						break;
					}
				}
				while (!flag);
			}
			return gridLines;
		}

		private int ReadChartData(Chart chart, PublishingContextStruct context)
		{
			if (!this.m_reader.IsEmptyElement)
			{
				chart.NumberOfSeriesDataPoints = new IntList();
				chart.SeriesPlotType = new BoolList();
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
						if ((localName = this.m_reader.LocalName) != null && localName == "ChartSeries")
						{
							this.ReadChartSeries(chart, context);
							num++;
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("ChartData" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
				return num;
			}
			return 0;
		}

		private void ReadChartSeries(Chart chart, PublishingContextStruct context)
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
					case "DataPoints":
						chart.NumberOfSeriesDataPoints.Add(this.ReadChartDataPoints(chart, context));
						break;
					case "PlotType":
						if (this.ReadPlotType())
						{
							chart.SeriesPlotType.Add(true);
							chart.HasSeriesPlotTypeLine = true;
						}
						else
						{
							chart.SeriesPlotType.Add(false);
						}
						flag2 = true;
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
			if (!flag2)
			{
				chart.SeriesPlotType.Add(false);
			}
		}

		private int ReadChartDataPoints(Chart chart, PublishingContextStruct context)
		{
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
					if ((localName = this.m_reader.LocalName) != null && localName == "DataPoint")
					{
						chart.ChartDataPoints.Add(this.ReadChartDataPoint(chart, context));
						num++;
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("DataPoints" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return num;
		}

		private ChartDataPoint ReadChartDataPoint(Chart chart, PublishingContextStruct context)
		{
			context.Location |= LocationFlags.InMatrixCell;
			ChartDataPoint chartDataPoint = new ChartDataPoint();
			bool flag = false;
			bool computed = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "DataValues":
					{
						bool flag3 = default(bool);
						this.ReadChartDataValues(chartDataPoint, context, out flag3);
						if (flag3)
						{
							chart.HasDataValueAggregates = true;
						}
						break;
					}
					case "DataLabel":
						chartDataPoint.DataLabel = this.ReadChartDataLabel(context);
						break;
					case "Action":
					{
						int num = -1;
						bool flag2 = false;
						ActionItem actionItem = this.ReadActionItem(context, out computed, ref num, ref flag2);
						chartDataPoint.Action = new Action(actionItem, computed);
						break;
					}
					case "ActionInfo":
						chartDataPoint.Action = this.ReadAction(context, StyleOwnerType.Chart, out computed);
						break;
					case "Style":
					{
						StyleInformation styleInformation = this.ReadStyle(context);
						styleInformation.Filter(StyleOwnerType.Chart, false);
						chartDataPoint.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, this.m_errorContext);
						break;
					}
					case "Marker":
						this.ReadDataPointMarker(chartDataPoint, context);
						break;
					case "DataElementName":
						chartDataPoint.DataElementName = this.m_reader.ReadString();
						break;
					case "DataElementOutput":
						chartDataPoint.DataElementOutput = this.ReadDataElementOutput();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("DataPoint" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			Global.Tracer.Assert(null != chart.ChartDataPoints);
			return chartDataPoint;
		}

		private void ReadDataPointMarker(ChartDataPoint dataPoint, PublishingContextStruct context)
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
						case "Type":
							dataPoint.MarkerType = this.ReadMarkerType();
							break;
						case "Size":
							dataPoint.MarkerSize = this.ReadSize();
							break;
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Chart, false);
							dataPoint.MarkerStyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, this.m_errorContext);
							break;
						}
						}
						break;
					case XmlNodeType.EndElement:
						if ("Marker" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
		}

		private void ReadChartDataValues(ChartDataPoint dataPoint, PublishingContextStruct context, out bool hasAggregates)
		{
			hasAggregates = false;
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "DataValue")
					{
						dataPoint.DataValues.Add(this.ReadChartDataValue(context, ref hasAggregates));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("DataValues" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private ExpressionInfo ReadChartDataValue(PublishingContextStruct context, ref bool hasAggregates)
		{
			ExpressionInfo expressionInfo = null;
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
						expressionInfo = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						if (!hasAggregates)
						{
							if (expressionInfo.Aggregates == null && expressionInfo.RunningValues == null)
							{
								break;
							}
							hasAggregates = true;
						}
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("DataValue" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return expressionInfo;
		}

		private ChartDataLabel ReadChartDataLabel(PublishingContextStruct context)
		{
			ChartDataLabel chartDataLabel = new ChartDataLabel();
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
							chartDataLabel.Visible = this.m_reader.ReadBoolean();
							break;
						case "Style":
						{
							StyleInformation styleInformation = this.ReadStyle(context);
							styleInformation.Filter(StyleOwnerType.Textbox, false);
							chartDataLabel.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, this.m_errorContext);
							break;
						}
						case "Value":
							chartDataLabel.Value = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
							break;
						case "Position":
							chartDataLabel.Position = this.ReadDataLabelPosition();
							break;
						case "Rotation":
							chartDataLabel.Rotation = this.m_reader.ReadInteger();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("DataLabel" == this.m_reader.LocalName)
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

		private MultiChart ReadMultiChart(Chart chart, PublishingContextStruct context)
		{
			MultiChart multiChart = new MultiChart();
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
						case "Grouping":
							multiChart.Grouping = this.ReadGrouping(context);
							break;
						case "Layout":
							multiChart.Layout = this.ReadLayout();
							break;
						case "MaxCount":
							multiChart.MaxCount = this.m_reader.ReadInteger();
							break;
						case "SyncScale":
							multiChart.SyncScale = this.m_reader.ReadBoolean();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("MultiChart" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return multiChart;
		}

		private PlotArea ReadPlotArea(Chart chart, PublishingContextStruct context)
		{
			PlotArea plotArea = new PlotArea();
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
							styleInformation.Filter(StyleOwnerType.Chart, false);
							plotArea.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, this.m_errorContext);
						}
						break;
					}
					case XmlNodeType.EndElement:
						if ("PlotArea" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return plotArea;
		}

		private ThreeDProperties ReadThreeDProperties(Chart chart, PublishingContextStruct context)
		{
			ThreeDProperties threeDProperties = new ThreeDProperties();
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
							threeDProperties.Enabled = this.m_reader.ReadBoolean();
							break;
						case "ProjectionMode":
							threeDProperties.PerspectiveProjectionMode = this.ReadProjectionMode();
							break;
						case "Rotation":
							threeDProperties.Rotation = this.m_reader.ReadInteger();
							break;
						case "Inclination":
							threeDProperties.Inclination = this.m_reader.ReadInteger();
							break;
						case "Perspective":
							threeDProperties.Perspective = this.m_reader.ReadInteger();
							break;
						case "HeightRatio":
							threeDProperties.HeightRatio = this.m_reader.ReadInteger();
							break;
						case "DepthRatio":
							threeDProperties.DepthRatio = this.m_reader.ReadInteger();
							break;
						case "Shading":
							threeDProperties.Shading = this.ReadShading();
							break;
						case "GapDepth":
							threeDProperties.GapDepth = this.m_reader.ReadInteger();
							break;
						case "WallThickness":
							threeDProperties.WallThickness = this.m_reader.ReadInteger();
							break;
						case "DrawingStyle":
							threeDProperties.DrawingStyleCube = this.ReadDrawingStyle();
							break;
						case "Clustered":
							threeDProperties.Clustered = this.m_reader.ReadBoolean();
							break;
						}
						break;
					case XmlNodeType.EndElement:
						if ("ThreeDProperties" == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
			return threeDProperties;
		}

		private Table ReadTable(ReportItem parent, PublishingContextStruct context)
		{
			Table table = new Table(this.GenerateID(), parent);
			table.Name = this.m_reader.GetAttribute("Name");
			bool flag = (context.Location & LocationFlags.InDataRegion) == (LocationFlags)0;
			context.Location = (context.Location | LocationFlags.InDataSet | LocationFlags.InDataRegion);
			context.ObjectType = table.ObjectType;
			context.ObjectName = table.Name;
			this.m_dataRegionCount++;
			this.m_reportItemNames.Validate(context.ObjectType, context.ObjectName, this.m_errorContext);
			if (this.m_scopeNames.Validate(false, context.ObjectName, context.ObjectType, context.ObjectName, this.m_errorContext))
			{
				this.m_reportScopes.Add(table.Name, table);
			}
			bool flag2 = true;
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsDataRegionInPageSection, Severity.Error, context.ObjectType, context.ObjectName, null);
				flag2 = false;
			}
			if ((context.Location & LocationFlags.InDetail) != 0)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsDataRegionInTableDetailRow, Severity.Error, context.ObjectType, context.ObjectName, null);
				flag2 = false;
			}
			this.m_aggregateHolderList.Add(table);
			this.m_runningValueHolderList.Add(table);
			StyleInformation styleInformation = null;
			bool flag3 = false;
			TextBoxList textBoxList = new TextBoxList();
			TextBoxList textBoxList2 = new TextBoxList();
			TableGroup tableGroup = null;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "Style":
						styleInformation = this.ReadStyle(context);
						break;
					case "Top":
						table.Top = this.ReadSize();
						break;
					case "Left":
						table.Left = this.ReadSize();
						break;
					case "ZIndex":
						table.ZIndex = this.m_reader.ReadInteger();
						break;
					case "Visibility":
						table.Visibility = this.ReadVisibility(context);
						break;
					case "ToolTip":
						table.ToolTip = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "Label":
						table.Label = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "Bookmark":
						table.Bookmark = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "RepeatWith":
						table.RepeatedSibling = true;
						table.RepeatWith = this.m_reader.ReadString();
						break;
					case "Custom":
						table.Custom = this.m_reader.ReadCustomXml();
						break;
					case "CustomProperties":
						table.CustomProperties = this.ReadCustomProperties(context);
						break;
					case "KeepTogether":
						table.KeepTogether = this.m_reader.ReadBoolean();
						break;
					case "NoRows":
						table.NoRows = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "DataSetName":
					{
						string dataSetName = this.m_reader.ReadString();
						if (flag)
						{
							table.DataSetName = dataSetName;
						}
						break;
					}
					case "PageBreakAtStart":
						table.PageBreakAtStart = this.m_reader.ReadBoolean();
						break;
					case "PageBreakAtEnd":
						table.PageBreakAtEnd = this.m_reader.ReadBoolean();
						break;
					case "Filters":
						table.Filters = this.ReadFilters(ExpressionParser.ExpressionType.DataRegionFilters, context);
						break;
					case "TableColumns":
						table.TableColumns = this.ReadTableColumns(context, table);
						break;
					case "Header":
					{
						TableRowList headerRows = default(TableRowList);
						bool headerRepeatOnNewPage = default(bool);
						this.ReadHeaderOrFooter(table, context, textBoxList, true, out headerRows, out headerRepeatOnNewPage);
						table.HeaderRows = headerRows;
						table.HeaderRepeatOnNewPage = headerRepeatOnNewPage;
						break;
					}
					case "TableGroups":
						tableGroup = this.ReadTableGroups(table, context);
						break;
					case "Details":
					{
						TableDetail tableDetail = default(TableDetail);
						TableGroup tableGroup2 = default(TableGroup);
						this.ReadDetails(table, context, textBoxList2, out tableDetail, out tableGroup2);
						if (tableGroup2 != null)
						{
							table.DetailGroup = tableGroup2;
						}
						else
						{
							table.TableDetail = tableDetail;
						}
						break;
					}
					case "Footer":
					{
						TableRowList footerRows = default(TableRowList);
						bool footerRepeatOnNewPage = default(bool);
						this.ReadHeaderOrFooter(table, context, textBoxList, false, out footerRows, out footerRepeatOnNewPage);
						table.FooterRows = footerRows;
						table.FooterRepeatOnNewPage = footerRepeatOnNewPage;
						break;
					}
					case "FillPage":
						table.FillPage = this.m_reader.ReadBoolean();
						break;
					case "DataElementName":
						table.DataElementName = this.m_reader.ReadString();
						break;
					case "DataElementOutput":
						table.DataElementOutputRDL = this.ReadDataElementOutputRDL();
						break;
					case "DetailDataElementName":
						table.DetailDataElementName = this.m_reader.ReadString();
						break;
					case "DetailDataCollectionName":
						table.DetailDataCollectionName = this.m_reader.ReadString();
						break;
					case "DetailDataElementOutput":
						table.DetailDataElementOutput = this.ReadDataElementOutput();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Table" == this.m_reader.LocalName)
					{
						flag3 = true;
					}
					break;
				}
			}
			while (!flag3);
			if (!flag && (table.FixedHeader || table.HasFixedColumnHeaders))
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsFixedHeadersInInnerDataRegion, Severity.Error, context.ObjectType, context.ObjectName, "FixedHeader");
			}
			table.CalculatePropagatedFlags();
			if (styleInformation != null)
			{
				styleInformation.Filter(StyleOwnerType.Table, null != table.NoRows);
				table.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, this.m_errorContext);
			}
			this.SetSortTargetForTextBoxes(textBoxList, table);
			ISortFilterScope target = (ISortFilterScope)((table.DetailGroup == null) ? ((tableGroup == null) ? ((object)table) : ((object)tableGroup.Grouping)) : table.DetailGroup.Grouping);
			this.SetSortTargetForTextBoxes(textBoxList2, target);
			table.Computed = true;
			if (!flag2)
			{
				return null;
			}
			return table;
		}

		private void ReadHeaderOrFooter(Table parent, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget, bool allowFixedHeaders, out TableRowList tableRows, out bool repeatOnNewPage)
		{
			tableRows = null;
			repeatOnNewPage = false;
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "FixedHeader":
					{
						bool fixedHeader = this.m_reader.ReadBoolean();
						if (allowFixedHeaders)
						{
							parent.FixedHeader = fixedHeader;
						}
						else
						{
							this.m_errorContext.Register(ProcessingErrorCode.rsCantMakeTableGroupHeadersFixed, Severity.Error, context.ObjectType, context.ObjectName, "FixedHeader");
						}
						break;
					}
					case "TableRows":
						tableRows = this.ReadTableRowList(parent, context, textBoxesWithDefaultSortTarget);
						break;
					case "RepeatOnNewPage":
						repeatOnNewPage = this.m_reader.ReadBoolean();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if (!("Header" == this.m_reader.LocalName) && !("Footer" == this.m_reader.LocalName))
					{
						break;
					}
					flag = true;
					break;
				}
			}
			while (!flag);
		}

		private TableRowList ReadTableRowList(Table parent, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			TableRowList tableRowList = new TableRowList();
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "TableRow")
					{
						tableRowList.Add(this.ReadTableRow(parent, context, textBoxesWithDefaultSortTarget));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("TableRows" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return tableRowList;
		}

		private TableRow ReadTableRow(Table parent, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			TableRow tableRow = new TableRow(this.GenerateID(), this.GenerateID());
			this.m_reportItemCollectionList.Add(tableRow.ReportItems);
			this.m_runningValueHolderList.Add(tableRow.ReportItems);
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "TableCells":
						this.ReadTableCells(parent, tableRow.ReportItems, tableRow.ColSpans, context, textBoxesWithDefaultSortTarget);
						break;
					case "Height":
						tableRow.Height = this.m_reader.ReadString();
						break;
					case "Visibility":
						tableRow.Visibility = this.ReadVisibility(context);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("TableRow" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			tableRow.IDs = new IntList();
			for (int i = 0; i < tableRow.ReportItems.Count; i++)
			{
				tableRow.IDs.Add(this.GenerateID());
			}
			return tableRow;
		}

		private void ReadDetails(Table parent, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget, out TableDetail tableDetail, out TableGroup detailGroup)
		{
			context.Location |= LocationFlags.InDetail;
			tableDetail = null;
			detailGroup = null;
			int numberOfAggregates = this.m_reportCT.NumberOfAggregates;
			bool flag = false;
			Grouping grouping = null;
			TableRowList tableRowList = null;
			Sorting sorting = null;
			Visibility visibility = null;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "TableRows":
						tableRowList = this.ReadTableRowList(parent, context, textBoxesWithDefaultSortTarget);
						break;
					case "Grouping":
						grouping = this.ReadGrouping(context);
						break;
					case "Sorting":
						sorting = this.ReadSorting(context);
						break;
					case "Visibility":
						visibility = this.ReadVisibility(context);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Details" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (grouping != null)
			{
				detailGroup = new TableGroup(this.GenerateID(), parent);
				this.m_runningValueHolderList.Add(detailGroup);
				detailGroup.Grouping = grouping;
				detailGroup.HeaderRows = tableRowList;
				detailGroup.Visibility = visibility;
				if (sorting != null)
				{
					if (this.CanMergeGroupingAndSorting(grouping, sorting))
					{
						detailGroup.Grouping.GroupAndSort = true;
						detailGroup.Grouping.SortDirections = sorting.SortDirections;
						detailGroup.Sorting = null;
						sorting = null;
					}
					else
					{
						detailGroup.Sorting = sorting;
					}
				}
			}
			else
			{
				tableDetail = new TableDetail(this.GenerateID());
				this.m_runningValueHolderList.Add(tableDetail);
				tableDetail.DetailRows = tableRowList;
				tableDetail.Sorting = sorting;
				tableDetail.Visibility = visibility;
			}
			if (sorting != null)
			{
				this.m_hasSorting = true;
			}
			if (this.m_reportCT.NumberOfAggregates > numberOfAggregates)
			{
				this.m_aggregateInDetailSections = true;
			}
		}

		private void ReadTableCells(Table parent, ReportItemCollection reportItems, IntList colSpans, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
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
					if ((localName = this.m_reader.LocalName) != null && localName == "TableCell")
					{
						this.ReadTableCell(parent, reportItems, colSpans, context, textBoxesWithDefaultSortTarget);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("TableCells" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
		}

		private void ReadTableCell(Table parent, ReportItemCollection reportItems, IntList colSpans, PublishingContextStruct context, TextBoxList textBoxesWithDefaultSortTarget)
		{
			int num = 1;
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
						this.ReadReportItems("TableCell", parent, reportItems, context, textBoxesWithDefaultSortTarget);
						break;
					case "ColSpan":
						num = this.m_reader.ReadInteger();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("TableCell" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			colSpans.Add(num);
		}

		private TableColumnList ReadTableColumns(PublishingContextStruct context, Table table)
		{
			TableColumnList tableColumnList = new TableColumnList();
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
					if ((localName = this.m_reader.LocalName) != null && localName == "TableColumn")
					{
						TableColumn tableColumn = this.ReadTableColumn(context);
						tableColumnList.Add(tableColumn);
						if (tableColumn.FixedHeader)
						{
							flag2 = true;
						}
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("TableColumns" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (flag2)
			{
				bool flag3 = false;
				bool flag4 = false;
				bool flag5 = false;
				for (int i = 0; i < tableColumnList.Count; i++)
				{
					TableColumn tableColumn2 = tableColumnList[i];
					if (tableColumn2.FixedHeader)
					{
						if (flag4)
						{
							flag5 = true;
							break;
						}
						flag3 = true;
					}
					else if (flag3)
					{
						flag4 = true;
					}
				}
				if (!flag5 && !tableColumnList[0].FixedHeader && !tableColumnList[tableColumnList.Count - 1].FixedHeader)
				{
					flag5 = true;
				}
				if (!flag5 && !flag4 && tableColumnList[0].FixedHeader && tableColumnList[tableColumnList.Count - 1].FixedHeader)
				{
					flag5 = true;
				}
				if (flag5)
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsInvalidFixedTableColumnHeaderSpacing, Severity.Error, context.ObjectType, context.ObjectName, "FixedHeader");
				}
				table.HasFixedColumnHeaders = true;
			}
			return tableColumnList;
		}

		private TableColumn ReadTableColumn(PublishingContextStruct context)
		{
			TableColumn tableColumn = new TableColumn();
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "FixedHeader":
						tableColumn.FixedHeader = this.m_reader.ReadBoolean();
						break;
					case "Width":
						tableColumn.Width = this.ReadSize();
						break;
					case "Visibility":
						tableColumn.Visibility = this.ReadVisibility(context);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("TableColumn" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return tableColumn;
		}

		private TableGroup ReadTableGroups(Table table, PublishingContextStruct context)
		{
			TableGroup tableGroup = null;
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "TableGroup")
					{
						TableGroup tableGroup2 = this.ReadTableGroup(table, context);
						if (tableGroup != null)
						{
							tableGroup.SubGroup = tableGroup2;
						}
						else
						{
							table.TableGroups = tableGroup2;
						}
						tableGroup = tableGroup2;
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("TableGroups" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return tableGroup;
		}

		private TableGroup ReadTableGroup(Table table, PublishingContextStruct context)
		{
			TableGroup tableGroup = new TableGroup(this.GenerateID(), table);
			this.m_runningValueHolderList.Add(tableGroup);
			bool flag = false;
			TextBoxList textBoxList = new TextBoxList();
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "Grouping":
						tableGroup.Grouping = this.ReadGrouping(context);
						break;
					case "Sorting":
						tableGroup.Sorting = this.ReadSorting(context);
						break;
					case "Header":
					{
						TableRowList headerRows = default(TableRowList);
						bool headerRepeatOnNewPage = default(bool);
						this.ReadHeaderOrFooter(table, context, textBoxList, false, out headerRows, out headerRepeatOnNewPage);
						tableGroup.HeaderRows = headerRows;
						tableGroup.HeaderRepeatOnNewPage = headerRepeatOnNewPage;
						break;
					}
					case "Footer":
					{
						TableRowList footerRows = default(TableRowList);
						bool footerRepeatOnNewPage = default(bool);
						this.ReadHeaderOrFooter(table, context, textBoxList, false, out footerRows, out footerRepeatOnNewPage);
						tableGroup.FooterRows = footerRows;
						tableGroup.FooterRepeatOnNewPage = footerRepeatOnNewPage;
						break;
					}
					case "Visibility":
						tableGroup.Visibility = this.ReadVisibility(context);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("TableGroup" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (this.CanMergeGroupingAndSorting(tableGroup.Grouping, tableGroup.Sorting))
			{
				tableGroup.Grouping.GroupAndSort = true;
				tableGroup.Grouping.SortDirections = tableGroup.Sorting.SortDirections;
				tableGroup.Sorting = null;
			}
			if (tableGroup.Sorting != null)
			{
				this.m_hasSorting = true;
			}
			Global.Tracer.Assert(null != tableGroup.Grouping);
			this.SetSortTargetForTextBoxes(textBoxList, tableGroup.Grouping);
			return tableGroup;
		}

		private OWCChart ReadOWCChart(ReportItem parent, PublishingContextStruct context)
		{
			OWCChart oWCChart = new OWCChart(this.GenerateID(), parent);
			oWCChart.Name = this.m_reader.GetAttribute("Name");
			bool flag = (context.Location & LocationFlags.InDataRegion) == (LocationFlags)0;
			context.Location = (context.Location | LocationFlags.InDataSet | LocationFlags.InDataRegion);
			context.ObjectType = oWCChart.ObjectType;
			context.ObjectName = oWCChart.Name;
			this.m_dataRegionCount++;
			this.m_reportItemNames.Validate(context.ObjectType, context.ObjectName, this.m_errorContext);
			if (this.m_scopeNames.Validate(false, context.ObjectName, context.ObjectType, context.ObjectName, this.m_errorContext))
			{
				this.m_reportScopes.Add(oWCChart.Name, oWCChart);
			}
			bool flag2 = true;
			if ((context.Location & LocationFlags.InPageSection) != 0)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsDataRegionInPageSection, Severity.Error, context.ObjectType, context.ObjectName, null);
				flag2 = false;
			}
			if ((context.Location & LocationFlags.InDetail) != 0)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsDataRegionInTableDetailRow, Severity.Error, context.ObjectType, context.ObjectName, null);
				flag2 = false;
			}
			this.m_aggregateHolderList.Add(oWCChart);
			this.m_runningValueHolderList.Add(oWCChart);
			StyleInformation styleInformation = null;
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
						styleInformation = this.ReadStyle(context);
						break;
					case "Top":
						oWCChart.Top = this.ReadSize();
						break;
					case "Left":
						oWCChart.Left = this.ReadSize();
						break;
					case "Height":
						oWCChart.Height = this.ReadSize();
						break;
					case "Width":
						oWCChart.Width = this.ReadSize();
						break;
					case "ZIndex":
						oWCChart.ZIndex = this.m_reader.ReadInteger();
						break;
					case "Visibility":
						oWCChart.Visibility = this.ReadVisibility(context);
						break;
					case "ToolTip":
						oWCChart.ToolTip = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "Label":
						oWCChart.Label = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "Bookmark":
						oWCChart.Bookmark = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "RepeatWith":
						oWCChart.RepeatedSibling = true;
						oWCChart.RepeatWith = this.m_reader.ReadString();
						break;
					case "Custom":
						oWCChart.Custom = this.m_reader.ReadCustomXml();
						break;
					case "CustomProperties":
						oWCChart.CustomProperties = this.ReadCustomProperties(context);
						break;
					case "KeepTogether":
						oWCChart.KeepTogether = this.m_reader.ReadBoolean();
						break;
					case "NoRows":
						oWCChart.NoRows = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "DataSetName":
					{
						string dataSetName = this.m_reader.ReadString();
						if (flag)
						{
							oWCChart.DataSetName = dataSetName;
						}
						break;
					}
					case "PageBreakAtStart":
						oWCChart.PageBreakAtStart = this.m_reader.ReadBoolean();
						break;
					case "PageBreakAtEnd":
						oWCChart.PageBreakAtEnd = this.m_reader.ReadBoolean();
						break;
					case "Filters":
						oWCChart.Filters = this.ReadFilters(ExpressionParser.ExpressionType.DataRegionFilters, context);
						break;
					case "OWCColumns":
						oWCChart.ChartData = this.ReadChartColumns(context);
						break;
					case "OWCDefinition":
						oWCChart.ChartDefinition = this.m_reader.ReadString();
						break;
					case "DataElementName":
						oWCChart.DataElementName = this.m_reader.ReadString();
						break;
					case "DataElementOutput":
						oWCChart.DataElementOutputRDL = this.ReadDataElementOutputRDL();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("OWCChart" == this.m_reader.LocalName)
					{
						flag3 = true;
					}
					break;
				}
			}
			while (!flag3);
			if (styleInformation != null)
			{
				styleInformation.Filter(StyleOwnerType.OWCChart, null != oWCChart.NoRows);
				oWCChart.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, this.m_errorContext);
			}
			oWCChart.Computed = true;
			if (flag2)
			{
				this.m_hasImageStreams = true;
				return oWCChart;
			}
			return null;
		}

		private ChartColumnList ReadChartColumns(PublishingContextStruct context)
		{
			ChartColumnList chartColumnList = new ChartColumnList();
			CLSUniqueNameValidator chartColumnNames = new CLSUniqueNameValidator(ProcessingErrorCode.rsInvalidChartColumnNameNotCLSCompliant, ProcessingErrorCode.rsDuplicateItemName);
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "OWCColumn")
					{
						chartColumnList.Add(this.ReadChartColumn(chartColumnNames, context));
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("OWCColumns" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return chartColumnList;
		}

		private ChartColumn ReadChartColumn(CLSUniqueNameValidator chartColumnNames, PublishingContextStruct context)
		{
			ChartColumn chartColumn = new ChartColumn();
			chartColumn.Name = this.m_reader.GetAttribute("Name");
			chartColumnNames.Validate(chartColumn.Name, context.ObjectType, context.ObjectName, this.m_errorContext);
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
						chartColumn.Value = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("OWCColumn" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return chartColumn;
		}

		private Sorting ReadSorting(PublishingContextStruct context)
		{
			Sorting sorting = new Sorting(ConstructionPhase.Publishing);
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
				{
					string localName;
					if ((localName = this.m_reader.LocalName) != null && localName == "SortBy")
					{
						this.ReadSortBy(sorting, context);
					}
					break;
				}
				case XmlNodeType.EndElement:
					if ("Sorting" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			return sorting;
		}

		private void ReadSortBy(Sorting sorting, PublishingContextStruct context)
		{
			ExpressionInfo expressionInfo = null;
			bool flag = true;
			bool flag2 = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "SortExpression":
						expressionInfo = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.SortExpression, ExpressionParser.ConstantType.String, context);
						break;
					case "Direction":
						flag = this.ReadDirection();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("SortBy" == this.m_reader.LocalName)
					{
						flag2 = true;
					}
					break;
				}
			}
			while (!flag2);
			sorting.SortExpressions.Add(expressionInfo);
			sorting.SortDirections.Add(flag);
			if (expressionInfo.HasRecursiveAggregates())
			{
				this.m_hasSpecialRecursiveAggregates = true;
			}
		}

		private bool CanMergeGroupingAndSorting(Grouping grouping, Sorting sorting)
		{
			if (grouping != null && grouping.Parent == null && sorting != null && grouping.GroupExpressions != null && sorting.SortExpressions != null && grouping.GroupExpressions.Count == sorting.SortExpressions.Count)
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

		private Grouping ReadGrouping(PublishingContextStruct context)
		{
			this.m_hasGrouping = true;
			Grouping grouping = new Grouping(ConstructionPhase.Publishing);
			grouping.Name = this.m_reader.GetAttribute("Name");
			if (this.m_scopeNames.Validate(true, grouping.Name, context.ObjectType, context.ObjectName, this.m_errorContext))
			{
				this.m_reportScopes.Add(grouping.Name, grouping);
			}
			this.m_aggregateHolderList.Add(grouping);
			bool flag = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "Label":
						grouping.GroupLabel = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context);
						break;
					case "GroupExpressions":
						this.ReadGroupExpressions(grouping, context);
						break;
					case "PageBreakAtStart":
						grouping.PageBreakAtStart = this.m_reader.ReadBoolean();
						break;
					case "PageBreakAtEnd":
						grouping.PageBreakAtEnd = this.m_reader.ReadBoolean();
						break;
					case "Custom":
						grouping.Custom = this.m_reader.ReadCustomXml();
						break;
					case "CustomProperties":
						grouping.CustomProperties = this.ReadCustomProperties(context);
						break;
					case "Filters":
						grouping.Filters = this.ReadFilters(ExpressionParser.ExpressionType.GroupingFilters, context);
						this.m_hasGroupFilters = true;
						break;
					case "Parent":
						grouping.Parent = new ExpressionInfoList();
						grouping.Parent.Add(this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.GroupExpression, ExpressionParser.ConstantType.String, context));
						break;
					case "DataElementName":
						grouping.DataElementName = this.m_reader.ReadString();
						break;
					case "DataCollectionName":
						grouping.DataCollectionName = this.m_reader.ReadString();
						break;
					case "DataElementOutput":
						grouping.DataElementOutput = this.ReadDataElementOutput();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					if ("Grouping" == this.m_reader.LocalName)
					{
						flag = true;
					}
					break;
				}
			}
			while (!flag);
			if (grouping.Parent != null && 1 != grouping.GroupExpressions.Count)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsInvalidGroupingParent, Severity.Error, context.ObjectType, context.ObjectName, "Parent");
			}
			return grouping;
		}

		private void ReadGroupExpressions(Grouping grouping, PublishingContextStruct context)
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
						grouping.GroupExpressions.Add(this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.GroupExpression, ExpressionParser.ConstantType.String, context));
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

		private Action ReadAction(PublishingContextStruct context, StyleOwnerType styleOwnerType, out bool computed)
		{
			Action action = new Action();
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
							action.StyleClass = PublishingValidator.ValidateAndCreateStyle(styleInformation.Names, styleInformation.Values, context.ObjectType, context.ObjectName, this.m_errorContext);
							break;
						}
						case "Actions":
							this.ReadActionItemList(action, context);
							if (action.ComputedActionItemsCount > 0)
							{
								flag2 = true;
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
			computed = (flag | flag2);
			return action;
		}

		private void ReadActionItemList(Action actionInfo, PublishingContextStruct context)
		{
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
						actionInfo.ActionItems.Add(this.ReadActionItem(context, out flag2, ref num, ref flag3));
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
			num = (actionInfo.ComputedActionItemsCount = num + 1);
			if (flag3 && actionInfo.ActionItems.Count > 1)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsInvalidActionLabel, Severity.Error, context.ObjectType, context.ObjectName, "Actions");
			}
		}

		private ActionItem ReadActionItem(PublishingContextStruct context, out bool computed, ref int computedIndex, ref bool missingLabel)
		{
			ActionItem actionItem = new ActionItem();
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = false;
			bool flag8 = false;
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
							actionItem.HyperLinkURL = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out flag5);
							break;
						case "Drillthrough":
							flag2 = true;
							this.ReadDrillthrough(context, actionItem, out flag6);
							break;
						case "BookmarkLink":
							flag3 = true;
							actionItem.BookmarkLink = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out flag7);
							break;
						case "Label":
							flag4 = true;
							actionItem.Label = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out flag8);
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
				if ((context.Location & LocationFlags.InPageSection) > (LocationFlags)0)
				{
					this.m_pageSectionDrillthroughs = true;
				}
			}
			if (flag3)
			{
				num++;
			}
			if (1 != num)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsInvalidAction, Severity.Error, context.ObjectType, context.ObjectName, "Action");
			}
			if (!flag4)
			{
				missingLabel = true;
			}
			computed = (flag5 | flag6 | flag7 | flag8);
			if (computed)
			{
				computedIndex++;
				actionItem.ComputedIndex = computedIndex;
			}
			return actionItem;
		}

		private void ReadDrillthrough(PublishingContextStruct context, ActionItem actionItem, out bool computed)
		{
			computed = false;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			do
			{
				this.m_reader.Read();
				switch (this.m_reader.NodeType)
				{
				case XmlNodeType.Element:
					switch (this.m_reader.LocalName)
					{
					case "ReportName":
						actionItem.DrillthroughReportName = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out flag3);
						if (ExpressionInfo.Types.Constant == actionItem.DrillthroughReportName.Type)
						{
							actionItem.DrillthroughReportName.Value = PublishingValidator.ValidateReportName(this.m_reportContext, actionItem.DrillthroughReportName.Value, context.ObjectType, context.ObjectName, "DrillthroughReportName", this.m_errorContext);
						}
						break;
					case "Parameters":
						actionItem.DrillthroughParameters = this.ReadParameters(context, true, false, out flag);
						break;
					case "BookmarkLink":
						actionItem.DrillthroughBookmarkLink = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out flag2);
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
			computed = (flag | flag2 | flag3);
		}

		private Visibility ReadVisibility(PublishingContextStruct context, out bool computed)
		{
			this.m_static = true;
			Visibility visibility = new Visibility();
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
						case "Hidden":
							visibility.Hidden = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.Boolean, context, out flag);
							break;
						case "ToggleItem":
							flag2 = true;
							if ((context.Location & LocationFlags.InPageSection) != 0)
							{
								this.m_errorContext.Register(ProcessingErrorCode.rsToggleInPageSection, Severity.Error, context.ObjectType, context.ObjectName, "ToggleItem");
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
			computed = (flag | flag2);
			return visibility;
		}

		private Visibility ReadVisibility(PublishingContextStruct context)
		{
			bool flag = default(bool);
			return this.ReadVisibility(context, out flag);
		}

		private DataValueList ReadCustomProperties(PublishingContextStruct context)
		{
			bool flag = default(bool);
			return this.ReadCustomProperties(context, out flag);
		}

		private DataValueList ReadCustomProperties(PublishingContextStruct context, out bool computed)
		{
			bool flag = false;
			computed = false;
			int num = 0;
			DataValueList dataValueList = new DataValueList();
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
						dataValueList.Add(this.ReadDataValue(true, ++num, ref computed, context));
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

		private DataValue ReadDataValue(bool isCustomProperty, int index, PublishingContextStruct context)
		{
			bool flag = false;
			return this.ReadDataValue(isCustomProperty, index, ref flag, context);
		}

		private DataValue ReadDataValue(bool isCustomProperty, int index, ref bool isComputed, PublishingContextStruct context)
		{
			DataValue dataValue = new DataValue();
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
						dataValue.Name = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out flag);
						break;
					case "Value":
						dataValue.Value = this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out flag2);
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
			if (dataValue.Name == null && isCustomProperty)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsMissingCustomPropertyName, Severity.Error, context.ObjectType, context.ObjectName, "Name", index.ToString(CultureInfo.CurrentCulture));
			}
			isComputed |= (flag2 | flag);
			return dataValue;
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

		private ExpressionInfo ReadExpression(string expression, string propertyName, string dataSetName, ExpressionParser.ExpressionType expressionType, ExpressionParser.ConstantType constantType, PublishingContextStruct context)
		{
			return this.ReadExpression(false, expression, propertyName, dataSetName, expressionType, constantType, context);
		}

		private ExpressionInfo ReadExpression(bool parseExtended, string expression, string propertyName, string dataSetName, ExpressionParser.ExpressionType expressionType, ExpressionParser.ConstantType constantType, PublishingContextStruct context)
		{
			ExpressionParser.ExpressionContext context2 = context.CreateExpressionContext(expressionType, constantType, propertyName, dataSetName, parseExtended);
			if (!this.CheckUserProfileDependency())
			{
				return this.m_reportCT.ParseExpression(expression, context2);
			}
			bool flag = default(bool);
			ExpressionInfo result = this.m_reportCT.ParseExpression(expression, context2, out flag);
			if (flag)
			{
				this.SetUserProfileDependency();
			}
			return result;
		}

		private ExpressionInfo ReadExpression(string propertyName, string dataSetName, ExpressionParser.ExpressionType expressionType, ExpressionParser.ConstantType constantType, PublishingContextStruct context, out bool userCollectionReferenced)
		{
			ExpressionParser.ExpressionContext context2 = context.CreateExpressionContext(expressionType, constantType, propertyName, dataSetName);
			return this.m_reportCT.ParseExpression(this.m_reader.ReadString(), context2, out userCollectionReferenced);
		}

		private ExpressionInfo ReadExpression(string propertyName, string dataSetName, ExpressionParser.ExpressionType expressionType, ExpressionParser.ConstantType constantType, PublishingContextStruct context)
		{
			return this.ReadExpression(this.m_reader.ReadString(), propertyName, dataSetName, expressionType, constantType, context);
		}

		private ExpressionInfo ReadExpression(string propertyName, ExpressionParser.ExpressionType expressionType, ExpressionParser.ConstantType constantType, PublishingContextStruct context)
		{
			return this.ReadExpression(propertyName, null, expressionType, constantType, context);
		}

		private ExpressionInfo ReadExpression(string propertyName, ExpressionParser.ExpressionType expressionType, ExpressionParser.ConstantType constantType, PublishingContextStruct context, out bool computed)
		{
			ExpressionInfo expressionInfo = this.ReadExpression(propertyName, expressionType, constantType, context);
			if (ExpressionInfo.Types.Constant == expressionInfo.Type)
			{
				computed = false;
			}
			else
			{
				computed = true;
			}
			return expressionInfo;
		}

		private ExpressionInfo ReadExpression(string propertyName, string dataSetName, ExpressionParser.ExpressionType expressionType, ExpressionParser.ConstantType constantType, PublishingContextStruct context, ExpressionParser.DetectionFlags flag, out bool reportParameterReferenced, out string reportParameterName)
		{
			ExpressionParser.ExpressionContext context2 = context.CreateExpressionContext(expressionType, constantType, propertyName, dataSetName);
			if (this.CheckUserProfileDependency())
			{
				flag |= ExpressionParser.DetectionFlags.UserReference;
			}
			bool flag2 = default(bool);
			ExpressionInfo result = this.m_reportCT.ParseExpression(this.m_reader.ReadString(), context2, flag, out reportParameterReferenced, out reportParameterName, out flag2);
			if (flag2)
			{
				this.SetUserProfileDependency();
			}
			return result;
		}

		private ExpressionInfo ReadExpression(string propertyName, string dataSetName, ExpressionParser.ExpressionType expressionType, ExpressionParser.ConstantType constantType, PublishingContextStruct context, ExpressionParser.DetectionFlags flag, out bool reportParameterReferenced, out string reportParameterName, out bool userCollectionReferenced)
		{
			ExpressionParser.ExpressionContext context2 = context.CreateExpressionContext(expressionType, constantType, propertyName, dataSetName);
			return this.m_reportCT.ParseExpression(this.m_reader.ReadString(), context2, flag, out reportParameterReferenced, out reportParameterName, out userCollectionReferenced);
		}

		private DataSet.Sensitivity ReadSensitivity()
		{
			string value = this.m_reader.ReadString();
			return (DataSet.Sensitivity)Enum.Parse(typeof(DataSet.Sensitivity), value, false);
		}

		private CommandType ReadCommandType()
		{
			string value = this.m_reader.ReadString();
			return (CommandType)Enum.Parse(typeof(CommandType), value, false);
		}

		private Filter.Operators ReadOperator()
		{
			string value = this.m_reader.ReadString();
			return (Filter.Operators)Enum.Parse(typeof(Filter.Operators), value, false);
		}

		private bool ReadDirection()
		{
			string x = this.m_reader.ReadString();
			return 0 == ReportProcessing.CompareWithInvariantCulture(x, "Ascending", false);
		}

		private bool ReadLayoutDirection()
		{
			string x = this.m_reader.ReadString();
			return 0 == ReportProcessing.CompareWithInvariantCulture(x, "RTL", false);
		}

		private bool ReadProjectionMode()
		{
			string x = this.m_reader.ReadString();
			return 0 == ReportProcessing.CompareWithInvariantCulture(x, "Perspective", false);
		}

		private bool ReadDrawingStyle()
		{
			string x = this.m_reader.ReadString();
			return 0 == ReportProcessing.CompareWithInvariantCulture(x, "Cube", false);
		}

		private Image.SourceType ReadSource()
		{
			string value = this.m_reader.ReadString();
			return (Image.SourceType)Enum.Parse(typeof(Image.SourceType), value, false);
		}

		private Image.Sizings ReadSizing()
		{
			string value = this.m_reader.ReadString();
			return (Image.Sizings)Enum.Parse(typeof(Image.Sizings), value, false);
		}

		private bool ReadDataElementStyle()
		{
			string x = this.m_reader.ReadString();
			return 0 == ReportProcessing.CompareWithInvariantCulture(x, "AttributeNormal", false);
		}

		private ReportItem.DataElementStylesRDL ReadDataElementStyleRDL()
		{
			string value = this.m_reader.ReadString();
			return (ReportItem.DataElementStylesRDL)Enum.Parse(typeof(ReportItem.DataElementStylesRDL), value, false);
		}

		private ReportItem.DataElementOutputTypesRDL ReadDataElementOutputRDL()
		{
			string value = this.m_reader.ReadString();
			return (ReportItem.DataElementOutputTypesRDL)Enum.Parse(typeof(ReportItem.DataElementOutputTypesRDL), value, false);
		}

		private DataElementOutputTypes ReadDataElementOutput()
		{
			string value = this.m_reader.ReadString();
			return (DataElementOutputTypes)Enum.Parse(typeof(DataElementOutputTypes), value, false);
		}

		private Subtotal.PositionType ReadPosition()
		{
			string value = this.m_reader.ReadString();
			return (Subtotal.PositionType)Enum.Parse(typeof(Subtotal.PositionType), value, false);
		}

		private ChartDataLabel.Positions ReadDataLabelPosition()
		{
			string value = this.m_reader.ReadString();
			return (ChartDataLabel.Positions)Enum.Parse(typeof(ChartDataLabel.Positions), value, false);
		}

		private Chart.ChartTypes ReadChartType()
		{
			string value = this.m_reader.ReadString();
			return (Chart.ChartTypes)Enum.Parse(typeof(Chart.ChartTypes), value, false);
		}

		private Chart.ChartSubTypes ReadChartSubType()
		{
			string value = this.m_reader.ReadString();
			return (Chart.ChartSubTypes)Enum.Parse(typeof(Chart.ChartSubTypes), value, false);
		}

		private Chart.ChartPalette ReadChartPalette()
		{
			string value = this.m_reader.ReadString();
			return (Chart.ChartPalette)Enum.Parse(typeof(Chart.ChartPalette), value, false);
		}

		private ChartTitle.Positions ReadChartTitlePosition()
		{
			string value = this.m_reader.ReadString();
			return (ChartTitle.Positions)Enum.Parse(typeof(ChartTitle.Positions), value, false);
		}

		private Legend.Positions ReadLegendPosition()
		{
			string value = this.m_reader.ReadString();
			return (Legend.Positions)Enum.Parse(typeof(Legend.Positions), value, false);
		}

		private Legend.LegendLayout ReadLegendLayout()
		{
			string value = this.m_reader.ReadString();
			return (Legend.LegendLayout)Enum.Parse(typeof(Legend.LegendLayout), value, false);
		}

		private MultiChart.Layouts ReadLayout()
		{
			string value = this.m_reader.ReadString();
			return (MultiChart.Layouts)Enum.Parse(typeof(MultiChart.Layouts), value, false);
		}

		private Axis.TickMarks ReadAxisTickMarks()
		{
			string value = this.m_reader.ReadString();
			return (Axis.TickMarks)Enum.Parse(typeof(Axis.TickMarks), value, false);
		}

		private ThreeDProperties.ShadingTypes ReadShading()
		{
			string value = this.m_reader.ReadString();
			return (ThreeDProperties.ShadingTypes)Enum.Parse(typeof(ThreeDProperties.ShadingTypes), value, false);
		}

		private ChartDataPoint.MarkerTypes ReadMarkerType()
		{
			string value = this.m_reader.ReadString();
			return (ChartDataPoint.MarkerTypes)Enum.Parse(typeof(ChartDataPoint.MarkerTypes), value, false);
		}

		private bool ReadPlotType()
		{
			string x = this.m_reader.ReadString();
			return 0 == ReportProcessing.CompareWithInvariantCulture(x, "Line", false);
		}

		private StyleInformation ReadStyle(PublishingContextStruct context, out bool computed)
		{
			computed = false;
			StyleInformation styleInformation = new StyleInformation();
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
						case "BorderColor":
						case "BorderStyle":
						case "BorderWidth":
							this.ReadBorderAttribute(this.m_reader.LocalName, styleInformation, context, out flag2);
							break;
						case "BackgroundImage":
							this.ReadBackgroundImage(styleInformation, context, out flag2);
							break;
						case "Format":
						case "Language":
						case "Calendar":
						case "NumeralLanguage":
						case "BackgroundColor":
						case "BackgroundGradientType":
						case "BackgroundGradientEndColor":
						case "FontStyle":
						case "FontFamily":
						case "FontSize":
						case "FontWeight":
						case "TextDecoration":
						case "TextAlign":
						case "VerticalAlign":
						case "Color":
						case "PaddingLeft":
						case "PaddingRight":
						case "PaddingTop":
						case "PaddingBottom":
						case "LineHeight":
						case "Direction":
						case "WritingMode":
						case "UnicodeBiDi":
							styleInformation.AddAttribute(this.m_reader.LocalName, this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out flag2));
							break;
						case "NumeralVariant":
							styleInformation.AddAttribute(this.m_reader.LocalName, this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.Integer, context, out flag2));
							break;
						}
						computed |= flag2;
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

		private StyleInformation ReadStyle(PublishingContextStruct context)
		{
			bool flag = default(bool);
			return this.ReadStyle(context, out flag);
		}

		private void ReadBorderAttribute(string borderAttribute, StyleInformation styleInfo, PublishingContextStruct context, out bool computed)
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
						string text = null;
						bool flag2 = false;
						switch (this.m_reader.LocalName)
						{
						case "Default":
						case "Left":
						case "Right":
						case "Top":
						case "Bottom":
							text = ((!("Default" == this.m_reader.LocalName)) ? this.m_reader.LocalName : string.Empty);
							styleInfo.AddAttribute(borderAttribute + text, this.ReadExpression(borderAttribute, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out flag2));
							break;
						}
						computed |= flag2;
						break;
					}
					case XmlNodeType.EndElement:
						if (borderAttribute == this.m_reader.LocalName)
						{
							flag = true;
						}
						break;
					}
				}
				while (!flag);
			}
		}

		private void ReadBackgroundImage(StyleInformation styleInfo, PublishingContextStruct context, out bool computed)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
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
						ExpressionInfo expressionInfo = new ExpressionInfo();
						expressionInfo.Type = ExpressionInfo.Types.Constant;
						expressionInfo.IntValue = (int)this.ReadSource();
						styleInfo.AddAttribute("BackgroundImageSource", expressionInfo);
						if (expressionInfo.IntValue == 0)
						{
							this.m_hasExternalImages = true;
						}
						break;
					}
					case "Value":
						styleInfo.AddAttribute("BackgroundImageValue", this.ReadExpression("BackgroundImageValue", ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out flag));
						break;
					case "MIMEType":
						styleInfo.AddAttribute("BackgroundImageMIMEType", this.ReadExpression("BackgroundImageMIMEType", ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out flag2));
						break;
					case "BackgroundRepeat":
						styleInfo.AddAttribute(this.m_reader.LocalName, this.ReadExpression(this.m_reader.LocalName, ExpressionParser.ExpressionType.General, ExpressionParser.ConstantType.String, context, out flag3));
						break;
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
			computed = (flag | flag2 | flag3);
			this.m_hasImageStreams = true;
		}

		private string ReadSize()
		{
			return this.m_reader.ReadString();
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
					EndUserSort userSort = this.m_textBoxesWithUserSortTarget[i].UserSort;
					ISortFilterScope sortFilterScope = this.m_reportScopes[userSort.SortTargetString] as ISortFilterScope;
					if (sortFilterScope != null)
					{
						userSort.SetSortTarget(sortFilterScope);
					}
				}
			}
			this.m_report.MergeOnePass = (!this.m_hasGrouping && !this.m_hasSorting && !this.m_aggregateInDetailSections && !this.m_reportCT.BodyRefersToReportItems && !this.m_reportCT.ValueReferencedGlobal && !this.m_subReportMergeTransactions && !this.m_hasUserSort);
			this.m_report.PageMergeOnePass = (this.m_report.PageAggregates.Count == 0 && !this.m_reportCT.PageSectionRefersToReportItems);
			this.m_report.SubReportMergeTransactions = this.m_subReportMergeTransactions;
			this.m_report.NeedPostGroupProcessing = (this.m_hasSorting | this.m_hasGroupFilters);
			this.m_report.HasSpecialRecursiveAggregates = this.m_hasSpecialRecursiveAggregates;
			this.m_report.HasReportItemReferences = this.m_reportCT.BodyRefersToReportItems;
			this.m_report.HasImageStreams = this.m_hasImageStreams;
			this.m_report.HasBookmarks = this.m_hasBookmarks;
			this.m_report.HasLabels = this.m_hasLabels;
			this.m_report.HasUserSortFilter = this.m_hasUserSort;
		}

		private void Phase3(ICatalogItemContext reportContext, out ParameterInfoCollection parameters, AppDomain compilationTempAppDomain, bool generateExpressionHostWithRefusedPermissions)
		{
			try
			{
				this.m_reportCT.Builder.ReportStart();
				this.m_report.LastAggregateID = this.m_reportCT.LastAggregateID;
				InitializationContext context = new InitializationContext(reportContext, this.m_hasFilters, this.m_dataSourceNames, this.m_dataSets, this.m_dynamicParameters, this.m_dataSetQueryInfo, this.m_errorContext, this.m_reportCT.Builder, this.m_report, this.m_reportLanguage, this.m_reportScopes, this.m_hasUserSortPeerScopes, this.m_dataRegionCount);
				this.m_report.Initialize(context);
				bool parametersNotUsedInQuery = false;
				ParameterInfo parameterInfo = null;
				parameters = new ParameterInfoCollection();
				ParameterDefList parameters2 = this.m_report.Parameters;
				if (parameters2 != null && parameters2.Count > 0)
				{
					context.InitializeParameters(this.m_report.Parameters, this.m_dataSets);
					for (int i = 0; i < parameters2.Count; i++)
					{
						ParameterDef parameterDef = parameters2[i];
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
						parameterInfo = new ParameterInfo(parameterDef);
						if (parameterDef.Dependencies != null && parameterDef.Dependencies.Count > 0)
						{
							int num = 0;
							IDictionaryEnumerator enumerator = parameterDef.Dependencies.GetEnumerator();
							ParameterDefList parameterDefList = new ParameterDefList();
							ParameterInfoCollection parameterInfoCollection = new ParameterInfoCollection();
							while (enumerator.MoveNext())
							{
								num = (int)enumerator.Value;
								parameterDefList.Add(parameters2[num]);
								parameterInfoCollection.Add(parameters[num]);
								if (parameterDef.UsedInQuery)
								{
									parameters[num].UsedInQuery = true;
								}
							}
							parameterDef.DependencyList = parameterDefList;
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
								ExpressionInfo expressionInfo = parameterDef.ValidValuesValueExpressions[j];
								ExpressionInfo expressionInfo2 = parameterDef.ValidValuesLabelExpressions[j];
								if (expressionInfo != null && ExpressionInfo.Types.Constant != expressionInfo.Type)
								{
									goto IL_0288;
								}
								if (expressionInfo2 != null && ExpressionInfo.Types.Constant != expressionInfo2.Type)
								{
									goto IL_0288;
								}
								continue;
								IL_0288:
								parameterInfo.DynamicValidValues = true;
							}
							if (!parameterInfo.DynamicValidValues)
							{
								parameterInfo.ValidValues = new ValidValueList(count);
								for (int k = 0; k < count; k++)
								{
									ExpressionInfo expressionInfo3 = parameterDef.ValidValuesValueExpressions[k];
									ExpressionInfo expressionInfo4 = parameterDef.ValidValuesLabelExpressions[k];
									parameterInfo.AddValidValue((expressionInfo3 != null) ? expressionInfo3.Value : null, (expressionInfo4 != null) ? expressionInfo4.Value : null, this.m_errorContext, CultureInfo.InvariantCulture);
								}
							}
						}
						parameterInfo.DynamicDefaultValue = (parameterDef.DefaultDataSource != null || null != parameterDef.DefaultExpressions);
						parameterInfo.Values = parameterDef.DefaultValues;
						parameters.Add(parameterInfo);
					}
				}
				this.m_parametersNotUsedInQuery = parametersNotUsedInQuery;
				this.m_report.ParametersNotUsedInQuery = this.m_parametersNotUsedInQuery;
				this.m_reportCT.Builder.ReportEnd();
				if (!this.m_errorContext.HasError)
				{
					this.m_report.CompiledCode = this.m_reportCT.Compile(this.m_report, compilationTempAppDomain, generateExpressionHostWithRefusedPermissions);
				}
				if (this.m_report.MergeOnePass)
				{
					int num2 = 0;
					int num3 = 0;
					while (true)
					{
						if (num3 < this.m_dataSets.Count)
						{
							if (!this.m_dataSets[num3].UsedOnlyInParameters)
							{
								num2++;
								if (1 < num2)
								{
									break;
								}
							}
							num3++;
							continue;
						}
						return;
					}
					this.m_report.MergeOnePass = false;
				}
			}
			finally
			{
				this.m_reportCT = null;
			}
		}

		private void Phase4()
		{
			this.PopulateReportItemCollections();
			this.CompactAggregates();
			this.CompactRunningValues();
		}

		private void PopulateReportItemCollections()
		{
			try
			{
				Global.Tracer.Assert(null != this.m_reportItemCollectionList);
				for (int i = 0; i < this.m_reportItemCollectionList.Count; i++)
				{
					((ReportItemCollection)this.m_reportItemCollectionList[i]).Populate(this.m_errorContext);
				}
			}
			finally
			{
				this.m_reportItemCollectionList = null;
			}
		}

		private void CompactAggregates()
		{
			try
			{
				Hashtable aggregateHashByType = new Hashtable();
				for (int i = 0; i < this.m_aggregateHolderList.Count; i++)
				{
					IAggregateHolder aggregateHolder = (IAggregateHolder)this.m_aggregateHolderList[i];
					Global.Tracer.Assert(null != aggregateHolder);
					DataAggregateInfoList[] aggregateLists = aggregateHolder.GetAggregateLists();
					Global.Tracer.Assert(null != aggregateLists);
					Global.Tracer.Assert(0 < aggregateLists.Length);
					this.CompactAggregates(aggregateLists, aggregateHashByType);
					if (this.CompactAggregates(aggregateHolder.GetPostSortAggregateLists(), aggregateHashByType))
					{
						this.m_report.HasPostSortAggregates = true;
					}
					if (aggregateHolder is Grouping && this.CompactAggregates(((Grouping)aggregateHolder).RecursiveAggregates, aggregateHashByType))
					{
						this.m_report.NeedPostGroupProcessing = true;
					}
					aggregateHolder.ClearIfEmpty();
				}
			}
			finally
			{
				this.m_aggregateHolderList = null;
			}
		}

		private bool CompactAggregates(DataAggregateInfoList[] aggregateLists, Hashtable aggregateHashByType)
		{
			bool result = false;
			if (aggregateLists != null)
			{
				foreach (DataAggregateInfoList dataAggregateInfoList in aggregateLists)
				{
					Global.Tracer.Assert(null != dataAggregateInfoList);
					if (this.CompactAggregates(dataAggregateInfoList, aggregateHashByType))
					{
						result = true;
					}
					aggregateHashByType.Clear();
				}
			}
			return result;
		}

		private bool CompactAggregates(DataAggregateInfoList aggregateList, Hashtable aggregateHashByType)
		{
			bool result = false;
			for (int num = aggregateList.Count - 1; num >= 0; num--)
			{
				result = true;
				DataAggregateInfo dataAggregateInfo = aggregateList[num];
				Global.Tracer.Assert(null != dataAggregateInfo);
				if (!dataAggregateInfo.IsCopied)
				{
					Hashtable hashtable = (Hashtable)aggregateHashByType[dataAggregateInfo.AggregateType];
					if (hashtable == null)
					{
						hashtable = new Hashtable();
						aggregateHashByType[dataAggregateInfo.AggregateType] = hashtable;
					}
					DataAggregateInfo dataAggregateInfo2 = (DataAggregateInfo)hashtable[dataAggregateInfo.ExpressionText];
					if (dataAggregateInfo2 == null)
					{
						hashtable[dataAggregateInfo.ExpressionText] = dataAggregateInfo;
					}
					else
					{
						if (dataAggregateInfo2.DuplicateNames == null)
						{
							dataAggregateInfo2.DuplicateNames = new StringList();
						}
						dataAggregateInfo2.DuplicateNames.Add(dataAggregateInfo.Name);
						aggregateList.RemoveAt(num);
					}
				}
			}
			return result;
		}

		private void CompactRunningValues()
		{
			try
			{
				Hashtable runningValueHashByType = new Hashtable();
				for (int i = 0; i < this.m_runningValueHolderList.Count; i++)
				{
					IRunningValueHolder runningValueHolder = (IRunningValueHolder)this.m_runningValueHolderList[i];
					Global.Tracer.Assert(null != runningValueHolder);
					this.CompactRunningValueList(runningValueHolder.GetRunningValueList(), runningValueHashByType);
					if (runningValueHolder is OWCChart)
					{
						this.CompactRunningValueList(((OWCChart)runningValueHolder).DetailRunningValues, runningValueHashByType);
					}
					else if (runningValueHolder is Chart)
					{
						this.CompactRunningValueList(((Chart)runningValueHolder).CellRunningValues, runningValueHashByType);
					}
					else if (runningValueHolder is CustomReportItem)
					{
						this.CompactRunningValueList(((CustomReportItem)runningValueHolder).CellRunningValues, runningValueHashByType);
					}
					runningValueHolder.ClearIfEmpty();
				}
			}
			finally
			{
				this.m_runningValueHolderList = null;
			}
		}

		private void CompactRunningValueList(RunningValueInfoList runningValueList, Hashtable runningValueHashByType)
		{
			Global.Tracer.Assert(null != runningValueList);
			Global.Tracer.Assert(null != runningValueHashByType);
			for (int num = runningValueList.Count - 1; num >= 0; num--)
			{
				this.m_report.HasPostSortAggregates = true;
				RunningValueInfo runningValueInfo = runningValueList[num];
				Global.Tracer.Assert(null != runningValueInfo);
				AllowNullKeyHashtable allowNullKeyHashtable = (AllowNullKeyHashtable)runningValueHashByType[runningValueInfo.AggregateType];
				if (allowNullKeyHashtable == null)
				{
					allowNullKeyHashtable = new AllowNullKeyHashtable();
					runningValueHashByType[runningValueInfo.AggregateType] = allowNullKeyHashtable;
				}
				Hashtable hashtable = (Hashtable)allowNullKeyHashtable[runningValueInfo.Scope];
				if (hashtable == null)
				{
					hashtable = new Hashtable();
					allowNullKeyHashtable[runningValueInfo.Scope] = hashtable;
				}
				RunningValueInfo runningValueInfo2 = (RunningValueInfo)hashtable[runningValueInfo.ExpressionText];
				if (runningValueInfo2 == null)
				{
					hashtable[runningValueInfo.ExpressionText] = runningValueInfo;
				}
				else
				{
					if (runningValueInfo2.DuplicateNames == null)
					{
						runningValueInfo2.DuplicateNames = new StringList();
					}
					runningValueInfo2.DuplicateNames.Add(runningValueInfo.Name);
					runningValueList.RemoveAt(num);
				}
			}
			runningValueHashByType.Clear();
		}

		internal static void CalculateChildrenDependencies(ReportItem reportItem)
		{
			ReportItemCollection reportItemCollection = null;
			if (!(reportItem is DataRegion) && !(reportItem is Rectangle) && !(reportItem is Report))
			{
				return;
			}
			if (reportItem is Rectangle)
			{
				reportItemCollection = ((Rectangle)reportItem).ReportItems;
			}
			else if (reportItem is List)
			{
				reportItemCollection = ((List)reportItem).ReportItems;
			}
			else if (reportItem is Report)
			{
				reportItemCollection = ((Report)reportItem).ReportItems;
			}
			if (reportItemCollection != null && reportItemCollection.Count >= 1)
			{
				double num = -1.0;
				double num2 = 0.0;
				for (int i = 0; i < reportItemCollection.Count; i++)
				{
					ReportItem reportItem2 = reportItemCollection[i];
					num2 = reportItem2.TopValue + reportItem2.HeightValue;
					num = -1.0;
					bool flag = ReportPublishing.HasPageBreakAtStart(reportItem2);
					for (int j = i + 1; j < reportItemCollection.Count; j++)
					{
						ReportItem reportItem3 = reportItemCollection[j];
						bool flag3;
						if (!(reportItem3.TopValue < reportItem2.TopValue))
						{
							bool flag2 = false;
							if (flag && reportItem3.TopValue >= reportItem2.TopValue && reportItem3.TopValue <= num2)
							{
								flag2 = true;
							}
							if (num >= 0.0 && num <= reportItem3.TopValue + 0.0009)
							{
								break;
							}
							if (!reportItemCollection.IsReportItemComputed(j))
							{
								flag2 = true;
							}
							flag3 = false;
							if (!(num2 <= reportItem3.TopValue + 0.0009))
							{
								switch (flag2)
								{
								case false:
									goto IL_0180;
								}
							}
							flag3 = true;
							if (!flag2 && (num < 0.0 || num > reportItem3.TopValue + reportItem3.HeightValue))
							{
								num = reportItem3.TopValue + reportItem3.HeightValue;
							}
							goto IL_0193;
						}
						continue;
						IL_0180:
						if (i + 1 == j && reportItem3.DistanceBeforeTop == 0)
						{
							flag3 = true;
						}
						goto IL_0193;
						IL_0193:
						if (flag3)
						{
							if (reportItem3.SiblingAboveMe == null)
							{
								reportItem3.SiblingAboveMe = new IntList();
							}
							reportItem3.SiblingAboveMe.Add(i);
						}
					}
					ReportPublishing.CalculateChildrenDependencies(reportItem2);
				}
			}
		}

		private static bool HasPageBreakAtStart(ReportItem reportItem)
		{
			if (!(reportItem is DataRegion) && !(reportItem is Rectangle))
			{
				return false;
			}
			if (reportItem is List)
			{
				List list = (List)reportItem;
				return list.PropagatedPageBreakAtStart;
			}
			if (reportItem is Table)
			{
				Table table = (Table)reportItem;
				return table.PropagatedPageBreakAtStart;
			}
			if (reportItem is Matrix)
			{
				Matrix matrix = (Matrix)reportItem;
				return matrix.PropagatedPageBreakAtStart;
			}
			IPageBreakItem pageBreakItem = (IPageBreakItem)reportItem;
			if (pageBreakItem != null)
			{
				if (pageBreakItem.IgnorePageBreaks())
				{
					return false;
				}
				return pageBreakItem.HasPageBreaks(true);
			}
			return false;
		}

		internal static void CalculateChildrenPostions(ReportItem reportItem)
		{
			ReportItemCollection reportItemCollection = null;
			if (!(reportItem is DataRegion) && !(reportItem is Rectangle) && !(reportItem is Report))
			{
				return;
			}
			if (reportItem is Rectangle)
			{
				reportItemCollection = ((Rectangle)reportItem).ReportItems;
			}
			else if (reportItem is List)
			{
				reportItemCollection = ((List)reportItem).ReportItems;
			}
			else if (reportItem is Report)
			{
				reportItemCollection = ((Report)reportItem).ReportItems;
				if (-1 == reportItem.DistanceFromReportTop)
				{
					reportItem.DistanceFromReportTop = 0;
				}
			}
			if (reportItemCollection != null)
			{
				double heightValue = reportItem.HeightValue;
				for (int i = 0; i < reportItemCollection.Count; i++)
				{
					ReportItem reportItem2 = reportItemCollection[i];
					reportItem2.DistanceBeforeTop = (int)reportItem2.TopValue;
					double topValue = reportItem2.TopValue;
					double heightValue2 = reportItem2.HeightValue;
					reportItem2.DistanceFromReportTop = reportItem.DistanceFromReportTop + (int)reportItem2.TopValue;
					if (reportItem2 is List)
					{
						((List)reportItem2).IsListMostInner = ReportPublishing.IsListMostInner(((List)reportItem2).ReportItems);
					}
					for (int j = 0; j < i; j++)
					{
						ReportItem reportItem3 = reportItemCollection[j];
						double num = reportItem3.TopValue + reportItem3.HeightValue;
						if (num < reportItem2.TopValue && !(reportItem2.LeftValue > reportItem3.LeftValue + reportItem3.WidthValue) && !(reportItem2.LeftValue + reportItem2.WidthValue < reportItem3.LeftValue))
						{
							reportItem2.DistanceBeforeTop = Math.Min(reportItem2.DistanceBeforeTop, (int)(reportItem2.TopValue - num));
						}
						else if (0.5 > Math.Abs(reportItemCollection[j].TopValue - reportItem2.TopValue))
						{
							reportItem2.DistanceBeforeTop = 0;
						}
					}
					ReportPublishing.CalculateChildrenPostions(reportItem2);
				}
			}
		}

		private static bool IsListMostInner(ReportItemCollection reportItemCollection)
		{
			if (reportItemCollection != null && reportItemCollection.Count >= 1)
			{
				for (int i = 0; i < reportItemCollection.Count; i++)
				{
					ReportItem reportItem = reportItemCollection[i];
					if (reportItem is DataRegion)
					{
						return false;
					}
					if (reportItem is Rectangle && ((Rectangle)reportItem).ReportItems.ComputedReportItems != null)
					{
						return false;
					}
				}
				return true;
			}
			return true;
		}
	}
}
