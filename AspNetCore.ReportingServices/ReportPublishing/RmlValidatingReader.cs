using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace AspNetCore.ReportingServices.ReportPublishing
{
	internal sealed class RmlValidatingReader : RDLValidatingReader
	{
		internal enum CustomFlags
		{
			None,
			InCustomElement,
			AfterCustomElement
		}

		internal enum ItemType
		{
			Rdl,
			Rdlx,
			Rsd
		}

		private CustomFlags m_custom;

		private PublishingErrorContext m_errorContext;

		private ItemType m_itemType;

		private readonly NameValueCollection m_microversioningValidationStructureElements;

		private readonly NameValueCollection m_microversioningValidationStructureAttributes;

		private readonly Stack<Pair<string, string>> m_rdlElementHierarchy;

		private List<string> m_serverSupportedSchemas;

		internal RmlValidatingReader(Stream stream, List<Pair<string, Stream>> namespaceSchemaStreamMap, PublishingErrorContext errorContext, ItemType itemType)
			: base(stream, namespaceSchemaStreamMap)
		{
			this.m_rdlElementHierarchy = new Stack<Pair<string, string>>();
			this.m_microversioningValidationStructureElements = RmlValidatingReader.SetupMicroVersioningValidationStructureForElements();
			this.m_microversioningValidationStructureAttributes = RmlValidatingReader.SetupMicroVersioningValidationStructureForAttributes();
			this.SetupMicroVersioningSchemas();
			base.ValidationEventHandler += this.ValidationCallBack;
			this.m_errorContext = errorContext;
			this.m_itemType = itemType;
		}

		public override bool Read()
		{
			try
			{
				if (CustomFlags.AfterCustomElement != this.m_custom)
				{
					base.Read();
					string message = default(string);
					if (!base.Validate(out message))
					{
						this.RegisterErrorAndThrow(message);
					}
					if (this.m_itemType == ItemType.Rdl || this.m_itemType == ItemType.Rdlx)
					{
						if (!this.RdlAdditionElementLocationValidation(out message))
						{
							this.RegisterErrorAndThrow(message);
						}
						if (!this.RdlAdditionAttributeLocationValidation(out message))
						{
							this.RegisterErrorAndThrow(message);
						}
						if (!this.ForceLaxSkippedValidation(out message))
						{
							this.RegisterErrorAndThrow(message);
						}
					}
				}
				else
				{
					this.m_custom = CustomFlags.None;
				}
				if (CustomFlags.InCustomElement != this.m_custom)
				{
					while (!base.EOF && XmlNodeType.Element == base.NodeType && !ListUtils.ContainsWithOrdinalComparer(base.NamespaceURI, base.m_validationNamespaceList))
					{
						this.Skip();
					}
				}
				return !base.EOF;
			}
			catch (ArgumentException ex)
			{
				this.RegisterErrorAndThrow(ex.Message);
				return false;
			}
		}

		private static NameValueCollection SetupMicroVersioningValidationStructureForElements()
		{
			NameValueCollection nameValueCollection = new NameValueCollection(StringComparer.Ordinal);
			RmlValidatingReader.SetMicroVersionValidationStructure(nameValueCollection, "http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition");
			RmlValidatingReader.SetMicroVersionValidationStructure(nameValueCollection, "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition");
			return nameValueCollection;
		}

		private static void SetMicroVersionValidationStructure(NameValueCollection validationStructure, string expandToThisNamespace)
		{
			validationStructure.Add(RmlValidatingReader.GetExpandedName("CanScroll", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("Tablix", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("CanScrollVertically", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("Textbox", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("NaturalGroup", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("Group", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("NaturalSort", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("SortExpression", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("DeferredSort", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("SortExpression", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("Relationship", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("Tablix", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("Relationship", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("Chart", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("Relationship", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("MapDataRegion", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("Relationship", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("GaugePanel", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("Relationship", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("CustomData", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("Relationship", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("Group", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("Relationship", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("Relationships", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("Relationships", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("TablixCell", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("Relationships", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("ChartDataPoint", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("Relationships", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("DataCell", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("DataSetName", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("Group", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("DataSetName", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("TablixCell", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("DataSetName", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("ChartDataPoint", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("DataSetName", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("DataCell", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("DefaultRelationships", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("DataSet", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("BandLayoutOptions", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("Tablix", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("LeftMargin", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("Tablix", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("RightMargin", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("Tablix", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("TopMargin", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("Tablix", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("BottomMargin", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("Tablix", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("DataSetName", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("LabelData", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("HighlightX", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("ChartDataPointValues", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("HighlightY", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("ChartDataPointValues", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("HighlightSize", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("ChartDataPointValues", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("AggregateIndicatorField", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("Field", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("NullsAsBlanks", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("DataSet", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("CollationCulture", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("DataSet", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("Tag", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("Image", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("Subtype", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"), RmlValidatingReader.GetExpandedName("ChartSeries", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("EmbeddingMode", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"), RmlValidatingReader.GetExpandedName("Image", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("EmbeddingMode", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"), RmlValidatingReader.GetExpandedName("BackgroundImage", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("LayoutDirection", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"), RmlValidatingReader.GetExpandedName("ReportSection", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("FontFamily", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"), RmlValidatingReader.GetExpandedName("Style", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("EnableDrilldown", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"), RmlValidatingReader.GetExpandedName("TablixRowHierarchy", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("EnableDrilldown", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"), RmlValidatingReader.GetExpandedName("TablixColumnHierarchy", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("BackgroundColor", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"), RmlValidatingReader.GetExpandedName("Style", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("Color", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"), RmlValidatingReader.GetExpandedName("Style", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("EnableDrilldown", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"), RmlValidatingReader.GetExpandedName("ChartCategoryHierarchy", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("BackgroundRepeat", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"), RmlValidatingReader.GetExpandedName("BackgroundImage", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("Transparency", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"), RmlValidatingReader.GetExpandedName("BackgroundImage", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("KeyFields", "http://schemas.microsoft.com/sqlserver/reporting/2013/01/reportdefinition"), RmlValidatingReader.GetExpandedName("LabelData", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("Tags", "http://schemas.microsoft.com/sqlserver/reporting/2013/01/reportdefinition"), RmlValidatingReader.GetExpandedName("Image", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("FormatX", "http://schemas.microsoft.com/sqlserver/reporting/2013/01/reportdefinition"), RmlValidatingReader.GetExpandedName("ChartDataPointValues", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("FormatY", "http://schemas.microsoft.com/sqlserver/reporting/2013/01/reportdefinition"), RmlValidatingReader.GetExpandedName("ChartDataPointValues", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("FormatSize", "http://schemas.microsoft.com/sqlserver/reporting/2013/01/reportdefinition"), RmlValidatingReader.GetExpandedName("ChartDataPointValues", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("CurrencyLanguageX", "http://schemas.microsoft.com/sqlserver/reporting/2013/01/reportdefinition"), RmlValidatingReader.GetExpandedName("ChartDataPointValues", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("CurrencyLanguageY", "http://schemas.microsoft.com/sqlserver/reporting/2013/01/reportdefinition"), RmlValidatingReader.GetExpandedName("ChartDataPointValues", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("CurrencyLanguageSize", "http://schemas.microsoft.com/sqlserver/reporting/2013/01/reportdefinition"), RmlValidatingReader.GetExpandedName("ChartDataPointValues", expandToThisNamespace));
			validationStructure.Add(RmlValidatingReader.GetExpandedName("CurrencyLanguage", "http://schemas.microsoft.com/sqlserver/reporting/2013/01/reportdefinition"), RmlValidatingReader.GetExpandedName("Style", expandToThisNamespace));
			if (expandToThisNamespace == "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition")
			{
				RmlValidatingReader.SetMicroVersionValidationStructureForRDL2016(validationStructure);
			}
		}

		private static void SetMicroVersionValidationStructureForRDL2016(NameValueCollection validationStructure)
		{
			validationStructure.Add(RmlValidatingReader.GetExpandedName("DefaultFontFamily", "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition/defaultfontfamily"), RmlValidatingReader.GetExpandedName("Report", "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition"));
		}

		private static NameValueCollection SetupMicroVersioningValidationStructureForAttributes()
		{
			NameValueCollection nameValueCollection = new NameValueCollection(StringComparer.Ordinal);
			nameValueCollection.Add(RmlValidatingReader.GetExpandedName("Name", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("ReportSection", "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition"));
			nameValueCollection.Add(RmlValidatingReader.GetExpandedName("Name", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition"), RmlValidatingReader.GetExpandedName("ReportSection", "http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition"));
			nameValueCollection.Add(RmlValidatingReader.GetExpandedName("ValueType", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"), RmlValidatingReader.GetExpandedName("FontFamily", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"));
			nameValueCollection.Add(RmlValidatingReader.GetExpandedName("ValueType", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"), RmlValidatingReader.GetExpandedName("Color", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"));
			nameValueCollection.Add(RmlValidatingReader.GetExpandedName("ValueType", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"), RmlValidatingReader.GetExpandedName("BackgroundColor", "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition"));
			return nameValueCollection;
		}

		private void SetupMicroVersioningSchemas()
		{
			this.m_serverSupportedSchemas = new List<string>
			{
				"http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition",
				"http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition/defaultfontfamily"
			};
		}

		private static string GetExpandedName(string localName, string namespaceURI)
		{
			StringBuilder stringBuilder = new StringBuilder(namespaceURI);
			stringBuilder.Append(":");
			stringBuilder.Append(localName);
			return stringBuilder.ToString();
		}

		private bool RdlAdditionElementLocationValidation(out string message)
		{
			Pair<string, string>? nullable = null;
			string text = null;
			bool flag = false;
			message = null;
			if (ListUtils.ContainsWithOrdinalComparer(this.NamespaceURI, base.m_validationNamespaceList))
			{
				switch (this.NodeType)
				{
				case XmlNodeType.Element:
				{
					text = RmlValidatingReader.GetExpandedName(this.LocalName, this.NamespaceURI);
					bool flag2 = this.IsPowerViewMicroVersionedNamespace();
					if ((this.m_itemType == ItemType.Rdl || this.m_itemType == ItemType.Rsd) && flag2)
					{
						message = RDLValidatingReaderStrings.rdlValidationInvalidNamespaceElement(text, this.NamespaceURI, base.LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), base.LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat));
						return false;
					}
					if (this.m_rdlElementHierarchy.Count > 0)
					{
						nullable = this.m_rdlElementHierarchy.Peek();
					}
					if (!nullable.HasValue)
					{
						Global.Tracer.Assert(this.LocalName == "Report", "(this.LocalName == Constants.Report)");
						Global.Tracer.Assert(this.NamespaceURI == "http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition" || this.NamespaceURI == "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition", "(this.NamespaceURI == Constants.RDL2010NamespaceURI) || (this.NamespaceURI == Constants.RDL2016NamespaceURI)");
					}
					if (!this.IsEmptyElement)
					{
						this.m_rdlElementHierarchy.Push(new Pair<string, string>(text, this.NamespaceURI));
					}
					if (!flag2)
					{
						break;
					}
					string[] values = this.m_microversioningValidationStructureElements.GetValues(text);
					if (values != null)
					{
						int num = 0;
						while (num < values.Length)
						{
							if (!nullable.Value.First.Equals(values[num], StringComparison.Ordinal))
							{
								num++;
								continue;
							}
							flag = true;
							break;
						}
						if (flag)
						{
							break;
						}
						message = RDLValidatingReaderStrings.rdlValidationInvalidParent(this.Name, this.NamespaceURI, nullable.Value.First, base.LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), base.LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat));
						return false;
					}
					if (nullable.Value.Second.Equals(this.NamespaceURI, StringComparison.Ordinal))
					{
						break;
					}
					message = RDLValidatingReaderStrings.rdlValidationInvalidMicroVersionedElement(this.Name, nullable.Value.First, base.LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), base.LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat));
					return false;
				}
				case XmlNodeType.EndElement:
					this.m_rdlElementHierarchy.Pop();
					break;
				}
			}
			return true;
		}

		private bool RdlAdditionAttributeLocationValidation(out string message)
		{
			message = null;
			HashSet<string> hashSet = null;
			if (this.NodeType == XmlNodeType.Element && this.HasAttributes)
			{
				string expandedName = RmlValidatingReader.GetExpandedName(this.LocalName, this.NamespaceURI);
				string namespaceURI = this.NamespaceURI;
				if (string.CompareOrdinal(expandedName, RmlValidatingReader.GetExpandedName("Report", "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition")) == 0 && base.GetAttributeLocalName("MustUnderstand") != null)
				{
					hashSet = new HashSet<string>(base.GetAttributeLocalName("MustUnderstand").Split());
				}
				while (this.MoveToNextAttribute())
				{
					string text = this.NamespaceURI;
					if (string.IsNullOrEmpty(text))
					{
						text = namespaceURI;
					}
					if (this.IsMicroVersionedAttributeNamespace(text))
					{
						string expandedName2 = RmlValidatingReader.GetExpandedName(this.LocalName, text);
						if (this.m_itemType == ItemType.Rdl || this.m_itemType == ItemType.Rsd)
						{
							message = RDLValidatingReaderStrings.rdlValidationInvalidNamespaceAttribute(expandedName2, text, base.LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), base.LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat));
						}
						else
						{
							string[] values = this.m_microversioningValidationStructureAttributes.GetValues(expandedName2);
							if (values != null)
							{
								for (int i = 0; i < values.Length; i++)
								{
									if (values[i].Equals(expandedName, StringComparison.Ordinal))
									{
										this.MoveToElement();
										return true;
									}
								}
							}
							message = RDLValidatingReaderStrings.rdlValidationInvalidMicroVersionedAttribute(expandedName2, expandedName, base.LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), base.LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat));
						}
					}
					if (hashSet != null && this.Prefix == "xmlns" && hashSet.Contains(this.LocalName))
					{
						hashSet.Remove(this.LocalName);
						if (!this.m_serverSupportedSchemas.Contains(this.Value))
						{
							message = RDLValidatingReaderStrings.rdlValidationUnsupportedSchema(this.Value, this.LocalName, base.LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), base.LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat));
							this.m_errorContext.Register(ProcessingErrorCode.rsInvalidMustUnderstandNamespaces, Severity.Error, ObjectType.Report, null, "MustUnderstand", message);
							throw new ReportProcessingException(this.m_errorContext.Messages);
						}
					}
				}
				if (hashSet != null && hashSet.Count != 0)
				{
					if (hashSet.Count == 1)
					{
						message = RDLValidatingReaderStrings.rdlValidationUndefinedSchemaNamespace(hashSet.First(), "MustUnderstand", base.LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), base.LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat));
						this.m_errorContext.Register(ProcessingErrorCode.rsInvalidMustUnderstandNamespaces, Severity.Error, ObjectType.Report, null, "MustUnderstand", message);
						throw new ReportProcessingException(this.m_errorContext.Messages);
					}
					message = RDLValidatingReaderStrings.rdlValidationMultipleUndefinedSchemaNamespaces(string.Join(", ", hashSet.ToArray()), "MustUnderstand", base.LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), base.LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat));
					this.m_errorContext.Register(ProcessingErrorCode.rsInvalidMustUnderstandNamespaces, Severity.Error, ObjectType.Report, null, "MustUnderstand", message);
					throw new ReportProcessingException(this.m_errorContext.Messages);
				}
				this.MoveToElement();
			}
			return message == null;
		}

		private bool IsPowerViewMicroVersionedNamespace()
		{
			Global.Tracer.Assert(ListUtils.ContainsWithOrdinalComparer(this.NamespaceURI, base.m_validationNamespaceList), "Not rdl namespace: " + this.NamespaceURI);
			if (string.CompareOrdinal(this.NamespaceURI, "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition") != 0 && string.CompareOrdinal(this.NamespaceURI, "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition") != 0)
			{
				return string.CompareOrdinal(this.NamespaceURI, "http://schemas.microsoft.com/sqlserver/reporting/2013/01/reportdefinition") == 0;
			}
			return true;
		}

		private bool IsMicroVersionedAttributeNamespace(string namespaceUri)
		{
			return string.CompareOrdinal(namespaceUri, "http://schemas.microsoft.com/sqlserver/reporting/2011/01/reportdefinition") == 0;
		}

		private bool ForceLaxSkippedValidation(out string message)
		{
			bool result = true;
			message = null;
			if (RDLValidatingReader.m_processContent == XmlSchemaContentProcessing.Lax && base.m_reader.NodeType == XmlNodeType.EndElement && base.m_reader.SchemaInfo != null && base.m_reader.SchemaInfo.Validity == XmlSchemaValidity.NotKnown && ListUtils.ContainsWithOrdinalComparer(base.m_reader.NamespaceURI, base.m_validationNamespaceList))
			{
				result = false;
				message = RDLValidatingReaderStrings.rdlValidationNoElementDecl(RmlValidatingReader.GetExpandedName(base.m_reader.LocalName, base.m_reader.NamespaceURI), base.m_reader.LocalName, base.m_reader.NamespaceURI, base.LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat));
			}
			return result;
		}

		public override string ReadString()
		{
			if (base.IsEmptyElement)
			{
				return string.Empty;
			}
			return base.ReadString();
		}

		internal bool ReadBoolean(ObjectType objectType, string objectName, string propertyName)
		{
			bool result = false;
			if (base.IsEmptyElement)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsInvalidBooleanConstant, Severity.Error, objectType, objectName, propertyName, string.Empty);
				return result;
			}
			string text = base.ReadString();
			try
			{
				result = XmlConvert.ToBoolean(text);
				return result;
			}
			catch
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsInvalidBooleanConstant, Severity.Error, objectType, objectName, propertyName, text);
				return result;
			}
		}

		internal int ReadInteger(ObjectType objectType, string objectName, string propertyName)
		{
			int result = 0;
			if (base.IsEmptyElement)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsInvalidIntegerConstant, Severity.Error, objectType, objectName, propertyName, string.Empty);
				return result;
			}
			string text = base.ReadString();
			try
			{
				result = XmlConvert.ToInt32(text);
				return result;
			}
			catch
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsInvalidIntegerConstant, Severity.Error, objectType, objectName, propertyName, text);
				return result;
			}
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
			if (ListUtils.ContainsWithOrdinalComparer(base.NamespaceURI, base.m_validationNamespaceList))
			{
				this.RegisterErrorAndThrow(args.Message);
			}
			else
			{
				XmlNodeType nodeType = base.NodeType;
			}
		}

		private void RegisterErrorAndThrow(string message)
		{
			if (this.m_itemType != 0 && this.m_itemType != ItemType.Rdlx)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsInvalidSharedDataSetDefinition, Severity.Error, ObjectType.SharedDataSet, null, null, message);
				throw new DataSetPublishingException(this.m_errorContext.Messages);
			}
			this.m_errorContext.Register(ProcessingErrorCode.rsInvalidReportDefinition, Severity.Error, ObjectType.Report, null, null, message);
			throw new ReportProcessingException(this.m_errorContext.Messages);
		}
	}
}
