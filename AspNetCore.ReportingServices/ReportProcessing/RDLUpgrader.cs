using AspNetCore.ReportingServices.RdlObjectModel2005.Upgrade;
using AspNetCore.ReportingServices.RdlObjectModel2008.Upgrade;
using AspNetCore.ReportingServices.RdlObjectModel2010.Upgrade;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class RDLUpgrader
	{
		private sealed class RdlUpgrader
		{
			private const string M5NamespaceURI = "http://schemas.microsoft.com/SQLServer/reporting/reportdefinition-1";

			private const string M5NamespaceURI2 = "http://schemas.microsoft.com/SQLServer/reporting/reportdefinition-2";

			private const string NamespaceURI200304 = "http://schemas.microsoft.com/sqlserver/reporting/2003/04/reportdefinition";

			private const string NamespaceURI200304_2 = "http://schemas.microsoft.com/sqlserver/reporting/2003/04/reportdefinition-1";

			private const string NamespaceURI200310 = "http://schemas.microsoft.com/sqlserver/reporting/2003/10/reportdefinition";

			private const string NamespaceURI200501 = "http://schemas.microsoft.com/sqlserver/reporting/2005/01/reportdefinition";

			private const string NamespaceURI200701 = "http://schemas.microsoft.com/sqlserver/reporting/2007/01/reportdefinition";

			private const string NamespaceURI200801 = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition";

			private const string NamespaceURI200901 = "http://schemas.microsoft.com/sqlserver/reporting/2009/01/reportdefinition";

			private const string NamespaceURI201001 = "http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition";

			private const string NamespaceURI201601 = "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition";

			private const string LatestNamespaceURI = "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition";

			private XmlDocument m_definition;

			internal RdlUpgrader()
			{
			}

			internal static string Get2007NamespaceURI()
			{
				return "http://schemas.microsoft.com/sqlserver/reporting/2007/01/reportdefinition";
			}

			internal static string Get2008NamespaceURI()
			{
				return "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition";
			}

			internal static string Get2009NamespaceURI()
			{
				return "http://schemas.microsoft.com/sqlserver/reporting/2009/01/reportdefinition";
			}

			internal static string Get2010NamespaceURI()
			{
				return "http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition";
			}

			internal static string Get2016NamespaceURI()
			{
				return "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition";
			}

			internal static string Get2005NamespaceURI()
			{
				return "http://schemas.microsoft.com/sqlserver/reporting/2005/01/reportdefinition";
			}

			internal Stream Upgrade(XmlReader xmlReader, string namespaceURI, bool throwUpgradeException, bool upgradeDundasCRIToNative, out RDLUpgradeResult upgradeResults)
			{
				return this.UpgradeUnified((Stream)null, xmlReader, namespaceURI, throwUpgradeException, upgradeDundasCRIToNative, true, out upgradeResults);
			}

			internal Stream Upgrade(Stream stream, bool throwUpgradeException, bool upgradeDundasCRIToNative, bool renameInvalidDataSources, out RDLUpgradeResult upgradeResults)
			{
				return this.UpgradeUnified(stream, (XmlReader)null, (string)null, throwUpgradeException, upgradeDundasCRIToNative, renameInvalidDataSources, out upgradeResults);
			}

			private Stream UpgradeUnified(Stream stream, XmlReader xmlReader, string namespaceURI, bool throwUpgradeException, bool upgradeDundasCRIToNative, bool renameInvalidDataSources, out RDLUpgradeResult upgradeResults)
			{
				if (stream == null && xmlReader == null)
				{
					throw new ArgumentNullException("Stream or XmlReader must be non-null");
				}
				if (xmlReader != null && namespaceURI == null)
				{
					throw new ArgumentException("namespaceURI must not be null if xmlReader is specified");
				}
				if (namespaceURI == null)
				{
					xmlReader = this.CreateXmlReader(stream);
					xmlReader.MoveToContent();
					namespaceURI = xmlReader.NamespaceURI;
					xmlReader.Close();
					xmlReader = null;
					stream.Seek(0L, SeekOrigin.Begin);
				}
				upgradeResults = null;
				switch (namespaceURI)
				{
				case "http://schemas.microsoft.com/SQLServer/reporting/reportdefinition-1":
				case "http://schemas.microsoft.com/SQLServer/reporting/reportdefinition-2":
				case "http://schemas.microsoft.com/sqlserver/reporting/2003/04/reportdefinition":
				case "http://schemas.microsoft.com/sqlserver/reporting/2003/04/reportdefinition-1":
				case "http://schemas.microsoft.com/sqlserver/reporting/2003/10/reportdefinition":
					this.LoadDefinitionXml(ref xmlReader, stream, false);
					stream = this.SaveDefinitionXml();
					goto case "http://schemas.microsoft.com/sqlserver/reporting/2005/01/reportdefinition";
				case "http://schemas.microsoft.com/sqlserver/reporting/2005/01/reportdefinition":
					stream = this.UpgradeFrom200501(this.EnsureReaderSetup(xmlReader, stream), throwUpgradeException, upgradeDundasCRIToNative, renameInvalidDataSources, out upgradeResults);
					goto case "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition";
				case "http://schemas.microsoft.com/sqlserver/reporting/2007/01/reportdefinition":
					this.LoadDefinitionXml(ref xmlReader, stream, true);
					this.UpgradeFrom200701();
					stream = this.SaveDefinitionXml();
					goto case "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition";
				case "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition":
					stream = this.UpgradeFrom200801(this.EnsureReaderSetup(xmlReader, stream), out upgradeResults);
					goto case "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition";
				case "http://schemas.microsoft.com/sqlserver/reporting/2009/01/reportdefinition":
					this.LoadDefinitionXml(ref xmlReader, stream, true);
					this.UpgradeFrom200901();
					stream = this.SaveDefinitionXml();
					goto case "http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition";
				case "http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition":
					stream = this.UpgradeFrom201001(this.EnsureReaderSetup(xmlReader, stream), out upgradeResults);
					goto case "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition";
				case "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition":
					if (upgradeResults == null)
					{
						upgradeResults = new RDLUpgradeResult();
					}
					if (stream == null)
					{
						this.LoadDefinitionXml(ref xmlReader, stream, true);
						stream = this.SaveDefinitionXml();
					}
					return stream;
				default:
					throw new RDLUpgradeException(RDLUpgradeStrings.rdlInvalidTargetNamespace(namespaceURI));
				}
			}

			private XmlReader EnsureReaderSetup(XmlReader xmlReader, Stream stream)
			{
				if (xmlReader == null)
				{
					XmlTextReader xmlTextReader = new XmlTextReader(stream);
					xmlTextReader.ProhibitDtd = true;
					xmlReader = xmlTextReader;
				}
				return xmlReader;
			}

			private void LoadDefinitionXml(ref XmlReader xmlReader, Stream stream, bool preserveWhitespace)
			{
				this.m_definition = new XmlDocument();
				if (preserveWhitespace)
				{
					this.m_definition.PreserveWhitespace = true;
				}
				xmlReader = this.EnsureReaderSetup(xmlReader, stream);
				this.m_definition.Load(xmlReader);
				xmlReader.Close();
				xmlReader = null;
			}

			private Stream SaveDefinitionXml()
			{
				Stream stream = new MemoryStream();
				this.m_definition.Save(stream);
				this.m_definition = null;
				stream.Seek(0L, SeekOrigin.Begin);
				return stream;
			}

			private XmlReader CreateXmlReader(Stream stream)
			{
				XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
				xmlReaderSettings.CheckCharacters = false;
				return XmlReader.Create(stream, xmlReaderSettings);
			}

			private Stream UpgradeFrom200501(XmlReader xmlReader, bool throwUpgradeException, bool upgradeDundasCRIToNative, bool renameInvalidDataSources, out RDLUpgradeResult upgradeResults)
			{
				MemoryStream memoryStream = new MemoryStream();
				UpgradeImpl2005 upgradeImpl = new UpgradeImpl2005(throwUpgradeException, upgradeDundasCRIToNative, renameInvalidDataSources);
				upgradeImpl.Upgrade(xmlReader, memoryStream);
				upgradeResults = upgradeImpl.UpgradeResults;
				memoryStream.Seek(0L, SeekOrigin.Begin);
				return memoryStream;
			}

			private Stream UpgradeFrom200801(XmlReader xmlReader, out RDLUpgradeResult upgradeResults)
			{
				MemoryStream memoryStream = new MemoryStream();
				UpgradeImpl2008 upgradeImpl = new UpgradeImpl2008();
				upgradeImpl.Upgrade(xmlReader, memoryStream);
				upgradeResults = upgradeImpl.UpgradeResults;
				memoryStream.Seek(0L, SeekOrigin.Begin);
				return memoryStream;
			}

			private Stream UpgradeFrom201001(XmlReader xmlReader, out RDLUpgradeResult upgradeResults)
			{
				MemoryStream memoryStream = new MemoryStream();
				UpgradeImpl2010 upgradeImpl = new UpgradeImpl2010();
				upgradeImpl.Upgrade(xmlReader, memoryStream);
				upgradeResults = upgradeImpl.UpgradeResults;
				memoryStream.Seek(0L, SeekOrigin.Begin);
				return memoryStream;
			}

			private void UpgradeFrom200701()
			{
				XmlElement documentElement = this.m_definition.DocumentElement;
				string prefixOfNamespace = documentElement.GetPrefixOfNamespace("http://schemas.microsoft.com/sqlserver/reporting/2007/01/reportdefinition");
				string text = this.BuildTempNamespacePrefix(prefixOfNamespace, documentElement);
				XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(this.m_definition.NameTable);
				xmlNamespaceManager.AddNamespace(text, "http://schemas.microsoft.com/sqlserver/reporting/2007/01/reportdefinition");
				XmlNodeList elementsByTagName = documentElement.GetElementsByTagName("Textbox");
				foreach (XmlNode item in elementsByTagName)
				{
					XmlNode xmlNode2 = item.SelectSingleNode(text + ":Style", xmlNamespaceManager);
					XmlNode xmlNode3 = item.SelectSingleNode(text + ":Value", xmlNamespaceManager);
					XmlElement xmlElement = this.m_definition.CreateElement(prefixOfNamespace, "Paragraphs", "http://schemas.microsoft.com/sqlserver/reporting/2007/01/reportdefinition");
					XmlElement xmlElement2 = this.m_definition.CreateElement(prefixOfNamespace, "Paragraph", "http://schemas.microsoft.com/sqlserver/reporting/2007/01/reportdefinition");
					XmlElement xmlElement3 = this.m_definition.CreateElement(prefixOfNamespace, "TextRuns", "http://schemas.microsoft.com/sqlserver/reporting/2007/01/reportdefinition");
					XmlElement xmlElement4 = this.m_definition.CreateElement(prefixOfNamespace, "TextRun", "http://schemas.microsoft.com/sqlserver/reporting/2007/01/reportdefinition");
					xmlElement3.AppendChild(xmlElement4);
					xmlElement2.AppendChild(xmlElement3);
					xmlElement.AppendChild(xmlElement2);
					item.AppendChild(xmlElement);
					if (xmlNode3 != null)
					{
						xmlNode3 = item.RemoveChild(xmlNode3);
						xmlElement4.AppendChild(xmlNode3);
					}
					else
					{
						xmlNode3 = this.m_definition.CreateElement(prefixOfNamespace, "Value", "http://schemas.microsoft.com/sqlserver/reporting/2007/01/reportdefinition");
						xmlElement4.AppendChild(xmlNode3);
					}
					if (xmlNode2 != null)
					{
						string value = item.Attributes.GetNamedItem("Name").Value;
						XmlNode xmlNode4 = this.m_definition.CreateElement(prefixOfNamespace, "Style", "http://schemas.microsoft.com/sqlserver/reporting/2007/01/reportdefinition");
						this.MoveStyleItemIfExists("LineHeight", xmlNode2, xmlNode4, text, xmlNamespaceManager);
						this.MoveStyleItemIfExists("TextAlign", xmlNode2, xmlNode4, text, xmlNamespaceManager);
						if (xmlNode4.HasChildNodes)
						{
							this.ConvertMeDotValueExpressions(xmlNode4.ChildNodes, value);
						}
						xmlElement2.AppendChild(xmlNode4);
						XmlNode xmlNode5 = this.m_definition.CreateElement(prefixOfNamespace, "Style", "http://schemas.microsoft.com/sqlserver/reporting/2007/01/reportdefinition");
						this.MoveStyleItemIfExists("FontStyle", xmlNode2, xmlNode5, text, xmlNamespaceManager);
						this.MoveStyleItemIfExists("FontFamily", xmlNode2, xmlNode5, text, xmlNamespaceManager);
						this.MoveStyleItemIfExists("FontSize", xmlNode2, xmlNode5, text, xmlNamespaceManager);
						this.MoveStyleItemIfExists("FontWeight", xmlNode2, xmlNode5, text, xmlNamespaceManager);
						this.MoveStyleItemIfExists("Format", xmlNode2, xmlNode5, text, xmlNamespaceManager);
						this.MoveStyleItemIfExists("TextDecoration", xmlNode2, xmlNode5, text, xmlNamespaceManager);
						this.MoveStyleItemIfExists("Color", xmlNode2, xmlNode5, text, xmlNamespaceManager);
						this.MoveStyleItemIfExists("Language", xmlNode2, xmlNode5, text, xmlNamespaceManager);
						this.MoveStyleItemIfExists("Calendar", xmlNode2, xmlNode5, text, xmlNamespaceManager);
						this.MoveStyleItemIfExists("NumeralLanguage", xmlNode2, xmlNode5, text, xmlNamespaceManager);
						this.MoveStyleItemIfExists("NumeralVariant", xmlNode2, xmlNode5, text, xmlNamespaceManager);
						if (xmlNode5.HasChildNodes)
						{
							this.ConvertMeDotValueExpressions(xmlNode5.ChildNodes, value);
						}
						xmlElement4.AppendChild(xmlNode5);
					}
				}
				this.UpgradeCharts(documentElement, xmlNamespaceManager, "http://schemas.microsoft.com/sqlserver/reporting/2007/01/reportdefinition", prefixOfNamespace, text);
				this.UpdateNamespaceURI(documentElement, "http://schemas.microsoft.com/sqlserver/reporting/2007/01/reportdefinition", RdlUpgrader.Get2008NamespaceURI());
			}

			private string BuildTempNamespacePrefix(string nsPrefix, XmlElement root)
			{
				string text = nsPrefix;
				if (string.IsNullOrEmpty(text))
				{
					text = "rs";
					int num = 0;
					while (!string.IsNullOrEmpty(root.GetNamespaceOfPrefix(text)))
					{
						text = "rs" + num;
						num++;
					}
				}
				return text;
			}

			private void UpgradeFrom200901()
			{
				XmlElement documentElement = this.m_definition.DocumentElement;
				string prefixOfNamespace = documentElement.GetPrefixOfNamespace("http://schemas.microsoft.com/sqlserver/reporting/2009/01/reportdefinition");
				string text = this.BuildTempNamespacePrefix(prefixOfNamespace, documentElement);
				XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(this.m_definition.NameTable);
				xmlNamespaceManager.AddNamespace(text, "http://schemas.microsoft.com/sqlserver/reporting/2009/01/reportdefinition");
				XmlNodeList xmlNodeList = documentElement.SelectNodes(string.Format(CultureInfo.InvariantCulture, "//{0}:Chart", text), xmlNamespaceManager);
				foreach (XmlNode item in xmlNodeList)
				{
					XmlNode xmlNode2 = item.SelectSingleNode(text + ":Code", xmlNamespaceManager);
					if (xmlNode2 != null)
					{
						item.RemoveChild(xmlNode2);
					}
					XmlNode xmlNode3 = item.SelectSingleNode(text + ":CodeLanguage", xmlNamespaceManager);
					if (xmlNode3 != null)
					{
						item.RemoveChild(xmlNode3);
					}
					XmlNode xmlNode4 = item.SelectSingleNode(text + ":ChartCodeParameters", xmlNamespaceManager);
					if (xmlNode4 != null)
					{
						item.RemoveChild(xmlNode4);
					}
					string xpath = string.Format(CultureInfo.InvariantCulture, "{0}:ChartAreas/{0}:ChartArea/*/{0}:ChartAxis/{0}:ChartStripLines/{0}:ChartStripLine", text);
					XmlNodeList xmlNodeList2 = item.SelectNodes(xpath, xmlNamespaceManager);
					foreach (XmlNode item2 in xmlNodeList2)
					{
						XmlNode xmlNode6 = item2.SelectSingleNode(text + ":TitleAngle", xmlNamespaceManager);
						if (xmlNode6 != null)
						{
							item2.RemoveChild(xmlNode6);
						}
					}
				}
				this.UpdateNamespaceURI(documentElement, "http://schemas.microsoft.com/sqlserver/reporting/2009/01/reportdefinition", RdlUpgrader.Get2010NamespaceURI());
			}

			private void UpgradeCharts(XmlElement report, XmlNamespaceManager nsm, string oldNamespaceURI, string oldNsPrefix, string tempNsPrefix)
			{
				XmlNodeList elementsByTagName = report.GetElementsByTagName("ChartCategoryHierarchy");
				foreach (XmlNode item in elementsByTagName)
				{
					bool flag = false;
					XmlNode xmlNode2 = item.ParentNode.SelectSingleNode(tempNsPrefix + ":CustomProperties", nsm);
					if (xmlNode2 != null)
					{
						foreach (XmlNode childNode in xmlNode2.ChildNodes)
						{
							XmlNode xmlNode4 = childNode.SelectSingleNode(tempNsPrefix + ":Value", nsm);
							if (xmlNode4 != null && xmlNode4.InnerText == "__Upgraded2005__")
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						bool flag2 = true;
						XmlNode xmlNode5 = item;
						do
						{
							XmlNode xmlNode6 = xmlNode5.SelectSingleNode(tempNsPrefix + ":ChartMembers", nsm);
							if (xmlNode6 == null)
							{
								if (!flag2 && xmlNode5.SelectSingleNode(tempNsPrefix + ":Group", nsm) == null && xmlNode5.ParentNode.ParentNode.SelectSingleNode(tempNsPrefix + ":Group", nsm) != null)
								{
									xmlNode5.ParentNode.ParentNode.RemoveChild(xmlNode5.ParentNode);
								}
								xmlNode5 = null;
							}
							else
							{
								XmlNodeList xmlNodeList = xmlNode6.SelectNodes(tempNsPrefix + ":ChartMember", nsm);
								if (xmlNodeList.Count == 1)
								{
									xmlNode5 = xmlNodeList[0];
									flag2 = false;
								}
								else
								{
									xmlNode5 = null;
								}
							}
						}
						while (xmlNode5 != null);
					}
				}
				XmlNodeList elementsByTagName2 = report.GetElementsByTagName("ChartThreeDProperties");
				foreach (XmlNode item2 in elementsByTagName2)
				{
					XmlNode xmlNode8 = item2.SelectSingleNode(tempNsPrefix + ":Inclination", nsm);
					XmlNode xmlNode9 = item2.SelectSingleNode(tempNsPrefix + ":Rotation", nsm);
					if (xmlNode8 != null)
					{
						this.RenameElement(oldNsPrefix, "Rotation", oldNamespaceURI, (XmlElement)xmlNode8);
					}
					if (xmlNode9 != null)
					{
						this.RenameElement(oldNsPrefix, "Inclination", oldNamespaceURI, (XmlElement)xmlNode9);
					}
					XmlNode xmlNode10 = item2.SelectSingleNode(tempNsPrefix + ":Clustered", nsm);
					if (xmlNode10 != null)
					{
						bool flag3 = false;
						if (bool.TryParse(xmlNode10.InnerText, out flag3))
						{
							xmlNode10.InnerText = (flag3 ? "false" : "true");
						}
						else
						{
							string str = xmlNode10.InnerText.Trim().TrimStart('=').Trim();
							xmlNode10.InnerText = "=NOT(" + str + ")";
						}
					}
					else
					{
						XmlNode xmlNode11 = this.m_definition.CreateElement(oldNsPrefix, "Clustered", oldNamespaceURI);
						xmlNode11.InnerText = "true";
						item2.AppendChild(xmlNode11);
					}
				}
				XmlNodeList elementsByTagName3 = report.GetElementsByTagName("Chart");
				foreach (XmlNode item3 in elementsByTagName3)
				{
					XmlNode xmlNode13 = item3.SelectSingleNode(tempNsPrefix + ":Palette", nsm);
					if (xmlNode13 != null && xmlNode13.InnerText == "GrayScale")
					{
						XmlNode xmlNode14 = item3.SelectSingleNode(tempNsPrefix + ":CustomProperties", nsm);
						if (xmlNode14 != null)
						{
							foreach (XmlNode childNode2 in xmlNode14.ChildNodes)
							{
								XmlNode xmlNode16 = childNode2.SelectSingleNode(tempNsPrefix + ":Value", nsm);
								if (xmlNode16 != null && xmlNode16.InnerText == "__Upgraded2005__")
								{
									XmlNode xmlNode17 = this.m_definition.CreateElement(oldNsPrefix, "PaletteHatchBehavior", oldNamespaceURI);
									xmlNode17.InnerText = "Always";
									item3.AppendChild(xmlNode17);
									break;
								}
							}
						}
					}
				}
				XmlNodeList elementsByTagName4 = report.GetElementsByTagName("ChartEmptyPoints");
				foreach (XmlNode item4 in elementsByTagName4)
				{
					XmlNode xmlNode19 = item4.SelectSingleNode(tempNsPrefix + ":ChartDataPointInLegend", nsm);
					if (xmlNode19 != null)
					{
						item4.RemoveChild(xmlNode19);
					}
				}
				XmlNodeList elementsByTagName5 = report.GetElementsByTagName("ChartDataPointInLegend");
				for (int i = 0; i < elementsByTagName5.Count; i++)
				{
					this.RenameElement(oldNsPrefix, "ChartItemInLegend", oldNamespaceURI, (XmlElement)elementsByTagName5[i]);
				}
				XmlNodeList elementsByTagName6 = report.GetElementsByTagName("ChartSeries");
				foreach (XmlNode item5 in elementsByTagName6)
				{
					XmlNode xmlNode21 = item5.SelectSingleNode(tempNsPrefix + ":LegendText", nsm);
					XmlNode xmlNode22 = item5.SelectSingleNode(tempNsPrefix + ":ToolTip", nsm);
					XmlNode xmlNode23 = item5.SelectSingleNode(tempNsPrefix + ":ActionInfo", nsm);
					XmlNode xmlNode24 = item5.SelectSingleNode(tempNsPrefix + ":HideInLegend", nsm);
					if (xmlNode21 != null || xmlNode22 != null || xmlNode23 != null || xmlNode24 != null)
					{
						XmlNode xmlNode25 = this.m_definition.CreateElement(oldNsPrefix, "ChartItemInLegend", oldNamespaceURI);
						if (xmlNode21 != null)
						{
							xmlNode25.AppendChild(xmlNode21);
						}
						if (xmlNode22 != null)
						{
							xmlNode25.AppendChild(xmlNode22);
						}
						if (xmlNode23 != null)
						{
							xmlNode25.AppendChild(xmlNode23);
						}
						if (xmlNode24 != null)
						{
							xmlNode25.AppendChild(xmlNode24);
							this.RenameElement(oldNsPrefix, "Hidden", oldNamespaceURI, (XmlElement)xmlNode24);
						}
						item5.AppendChild(xmlNode25);
					}
					XmlNode xmlNode26 = item5.SelectSingleNode(tempNsPrefix + ":ChartDataPoints", nsm);
					if (xmlNode26 != null && item5.ParentNode.LocalName != "ChartDerivedSeries")
					{
						XmlNode xmlNode27 = item5.SelectSingleNode(tempNsPrefix + ":ChartMarker", nsm);
						XmlNode xmlNode28 = item5.SelectSingleNode(tempNsPrefix + ":ChartDataLabel", nsm);
						XmlNode xmlNode29 = item5.SelectSingleNode(tempNsPrefix + ":Style", nsm);
						XmlNode xmlNode30 = null;
						XmlNode xmlNode31 = null;
						if (xmlNode29 != null)
						{
							xmlNode30 = xmlNode29.SelectSingleNode(tempNsPrefix + ":ShadowOffset", nsm);
							if (xmlNode30 != null)
							{
								xmlNode30 = xmlNode29.RemoveChild(xmlNode30);
							}
							xmlNode31 = xmlNode29.SelectSingleNode(tempNsPrefix + ":ShadowColor", nsm);
							if (xmlNode31 != null)
							{
								xmlNode31 = xmlNode29.RemoveChild(xmlNode31);
							}
						}
						XmlNodeList xmlNodeList2 = xmlNode26.SelectNodes(tempNsPrefix + ":ChartDataPoint", nsm);
						foreach (XmlNode item6 in xmlNodeList2)
						{
							this.MergeElementsWithPreference(tempNsPrefix, "Style", item5, item6, nsm);
							this.MergeElementsWithPreference(tempNsPrefix, "ChartMarker", item5, item6, nsm);
							this.MergeElementsWithPreference(tempNsPrefix, "ChartDataLabel", item5, item6, nsm);
						}
						if (xmlNode29 != null)
						{
							item5.RemoveChild(xmlNode29);
						}
						if (xmlNode27 != null)
						{
							item5.RemoveChild(xmlNode27);
						}
						if (xmlNode28 != null)
						{
							item5.RemoveChild(xmlNode28);
						}
						if (xmlNode31 != null || xmlNode30 != null)
						{
							XmlNode xmlNode32 = this.m_definition.CreateElement(oldNsPrefix, "Style", oldNamespaceURI);
							if (xmlNode31 != null)
							{
								xmlNode32.AppendChild(xmlNode31);
							}
							if (xmlNode30 != null)
							{
								xmlNode32.AppendChild(xmlNode30);
							}
							item5.AppendChild(xmlNode32);
						}
					}
				}
			}

			private void MoveStyleItemIfExists(string name, XmlNode sourceStyle, XmlNode destStyle, string tempNsPrefix, XmlNamespaceManager nsManager)
			{
				XmlNode xmlNode = sourceStyle.SelectSingleNode(tempNsPrefix + ":" + name, nsManager);
				if (xmlNode != null)
				{
					xmlNode = sourceStyle.RemoveChild(xmlNode);
					destStyle.AppendChild(xmlNode);
				}
			}

			private void ConvertMeDotValueExpressions(XmlNodeList styleNodes, string textboxName)
			{
				foreach (XmlNode styleNode in styleNodes)
				{
					if (styleNode.HasChildNodes)
					{
						this.ConvertMeDotValueExpressions(styleNode.ChildNodes, textboxName);
					}
					else if (styleNode.Value != null)
					{
						styleNode.Value = this.ConvertMeDotValue(styleNode.Value, textboxName);
					}
				}
			}

			private string ConvertMeDotValue(string expression, string textboxName)
			{
				int num = 0;
				StringBuilder stringBuilder = new StringBuilder();
				MatchCollection matchCollection = ReportRegularExpressions.Value.MeDotValueExpression.Matches(expression);
				for (int i = 0; i < matchCollection.Count; i++)
				{
					Group group = matchCollection[i].Groups["medotvalue"];
					if (group.Value != null && group.Value.Length > 0)
					{
						stringBuilder.Append(expression.Substring(num, group.Index - num));
						stringBuilder.Append("ReportItems!");
						stringBuilder.Append(textboxName);
						stringBuilder.Append(".Value");
						num = group.Index + group.Length;
					}
				}
				if (num == 0)
				{
					return expression;
				}
				if (num < expression.Length)
				{
					stringBuilder.Append(expression.Substring(num));
				}
				return stringBuilder.ToString();
			}

			private void MergeElementsWithPreference(string prefix, string elementToMerge, XmlNode sourceParent, XmlNode targetParent, XmlNamespaceManager nsManager)
			{
				if (sourceParent != null && targetParent != null)
				{
					XmlNode xmlNode = sourceParent.SelectSingleNode(prefix + ":" + elementToMerge, nsManager);
					if (xmlNode != null)
					{
						XmlNode xmlNode2 = targetParent.SelectSingleNode(prefix + ":" + elementToMerge, nsManager);
						if (xmlNode2 == null)
						{
							targetParent.AppendChild(xmlNode.CloneNode(true));
						}
						else
						{
							for (int i = 0; xmlNode.ChildNodes.Count > i; i++)
							{
								XmlNode xmlNode3 = xmlNode.ChildNodes[i];
								if (xmlNode3.HasChildNodes)
								{
									this.MergeElementsWithPreference(prefix, xmlNode3.LocalName, xmlNode, xmlNode2, nsManager);
								}
								else if (xmlNode3.NodeType == XmlNodeType.Text && targetParent.SelectSingleNode(prefix + ":" + xmlNode3.ParentNode.LocalName, nsManager) == null)
								{
									targetParent.AppendChild(xmlNode.CloneNode(true));
								}
							}
						}
					}
				}
			}

			private void UpdateNamespaceURI(XmlNode root, string oldNamespaceURI, string newNamespaceURI)
			{
				string prefixOfNamespace = root.GetPrefixOfNamespace(oldNamespaceURI);
				string text = "xmlns";
				if (prefixOfNamespace.Length > 0)
				{
					text = text + ":" + prefixOfNamespace;
				}
				XmlAttribute xmlAttribute = root.Attributes[text];
				if (xmlAttribute != null)
				{
					xmlAttribute.Value = newNamespaceURI;
				}
			}

			private XmlElement RenameElement(string prefix, string name, string URI, XmlElement oldElement)
			{
				XmlElement xmlElement = this.m_definition.CreateElement(prefix, name, URI);
				int count = oldElement.Attributes.Count;
				for (int i = 0; i < count; i++)
				{
					xmlElement.Attributes.Append(oldElement.Attributes[i]);
				}
				while (oldElement.ChildNodes.Count > 0)
				{
					xmlElement.AppendChild(oldElement.ChildNodes[0]);
				}
				XmlNode parentNode = oldElement.ParentNode;
				parentNode.ReplaceChild(xmlElement, oldElement);
				return xmlElement;
			}
		}

		internal static Stream UpgradeToCurrent(XmlReader rdlReader, string namespaceURI, bool throwUpgradeException, bool upgradeDundasCRIToNative, out RDLUpgradeResult upgradeResults)
		{
			RdlUpgrader rdlUpgrader = new RdlUpgrader();
			return rdlUpgrader.Upgrade(rdlReader, namespaceURI, throwUpgradeException, upgradeDundasCRIToNative, out upgradeResults);
		}

		internal static Stream UpgradeToCurrent(XmlReader rdlReader, string namespaceURI, bool throwUpgradeException, bool upgradeDundasCRIToNative)
		{
			RDLUpgradeResult rDLUpgradeResult = null;
			return RDLUpgrader.UpgradeToCurrent(rdlReader, namespaceURI, throwUpgradeException, upgradeDundasCRIToNative, out rDLUpgradeResult);
		}

		internal static Stream UpgradeToCurrent(Stream stream, bool throwUpgradeException)
		{
			RDLUpgradeResult rDLUpgradeResult = null;
			return RDLUpgrader.UpgradeToCurrent(stream, throwUpgradeException, true, out rDLUpgradeResult);
		}

		internal static Stream UpgradeToCurrent(Stream stream, bool throwUpgradeException, bool renameInvalidDataSources)
		{
			RDLUpgradeResult rDLUpgradeResult = null;
			return RDLUpgrader.UpgradeToCurrent(stream, throwUpgradeException, renameInvalidDataSources, out rDLUpgradeResult);
		}

		internal static Stream UpgradeToCurrent(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			return RDLUpgrader.UpgradeToCurrent(stream, false, true);
		}

		internal static Stream UpgradeToCurrent(Stream stream, bool throwUpgradeException, bool renameInvalidDataSources, out RDLUpgradeResult upgradeResults)
		{
			if (!stream.CanSeek)
			{
				throw new ArgumentException("Upgrade reqires Stream.CanSeek.");
			}
			RdlUpgrader rdlUpgrader = new RdlUpgrader();
			return rdlUpgrader.Upgrade(stream, throwUpgradeException, true, renameInvalidDataSources, out upgradeResults);
		}

		internal static RDLUpgradeResult CheckForDundasCRI(XmlTextReader xmlTextReader)
		{
			RDLUpgradeResult rDLUpgradeResult = new RDLUpgradeResult();
			try
			{
				xmlTextReader.MoveToContent();
				if (xmlTextReader.NamespaceURI.Equals(RdlUpgrader.Get2005NamespaceURI(), StringComparison.OrdinalIgnoreCase))
				{
					while (xmlTextReader.ReadToFollowing("CustomReportItem"))
					{
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
						bool flag11 = false;
						xmlTextReader.ReadStartElement();
						do
						{
							if (xmlTextReader.NodeType == XmlNodeType.Element)
							{
								if (xmlTextReader.Name == "Type")
								{
									string text = xmlTextReader.ReadInnerXml();
									flag = text.Equals("DUNDASCHARTCONTROL", StringComparison.OrdinalIgnoreCase);
									flag2 = text.Equals("DUNDASGAUGECONTROL", StringComparison.OrdinalIgnoreCase);
									if (!flag && !flag2)
									{
										break;
									}
								}
								else if (xmlTextReader.Name == "CustomProperties")
								{
									xmlTextReader.ReadStartElement();
									do
									{
										if (xmlTextReader.Name == "Name")
										{
											string text2 = xmlTextReader.ReadInnerXml();
											if (text2.StartsWith("CHART.ANNOTATIONS.", StringComparison.OrdinalIgnoreCase))
											{
												flag3 = true;
											}
											if (text2.StartsWith("CHART.LEGENDS", StringComparison.OrdinalIgnoreCase) && (text2.IndexOf("LEGEND.CUSTOMITEMS.", StringComparison.OrdinalIgnoreCase) > 0 || text2.IndexOf("LEGEND.CELLCOLUMNS.", StringComparison.OrdinalIgnoreCase) > 0))
											{
												flag4 = true;
											}
											if (text2.StartsWith("GAUGECORE.NUMERICINDICATORS.", StringComparison.OrdinalIgnoreCase))
											{
												flag5 = true;
											}
											if (text2.StartsWith("GAUGECORE.STATEINDICATORS.", StringComparison.OrdinalIgnoreCase))
											{
												flag6 = true;
											}
											if (text2.StartsWith("GAUGECORE.NAMEDIMAGES.", StringComparison.OrdinalIgnoreCase) || text2.StartsWith("GAUGECORE.IMAGES.", StringComparison.OrdinalIgnoreCase))
											{
												flag7 = true;
											}
											if (text2.StartsWith("CUSTOM_CODE_CS", StringComparison.OrdinalIgnoreCase) || text2.StartsWith("CUSTOM_CODE_VB", StringComparison.OrdinalIgnoreCase) || text2.StartsWith("CUSTOM_CODE_COMPILED_ASSEMBLY", StringComparison.OrdinalIgnoreCase))
											{
												flag8 = true;
											}
										}
									}
									while ((!(xmlTextReader.Name == "CustomProperties") || xmlTextReader.NodeType != XmlNodeType.EndElement) && xmlTextReader.Read());
								}
								else if (xmlTextReader.Name == "CustomData")
								{
									bool flag12 = false;
									bool flag13 = false;
									xmlTextReader.ReadStartElement();
									do
									{
										if (xmlTextReader.Name == "DataRowGroupings")
										{
											if (xmlTextReader.NodeType == XmlNodeType.Element)
											{
												flag12 = true;
											}
											else if (xmlTextReader.NodeType == XmlNodeType.EndElement)
											{
												flag12 = false;
											}
										}
										else if (flag12 && xmlTextReader.Name == "Name")
										{
											string text3 = xmlTextReader.ReadInnerXml();
											if (text3.Equals("ERRORFORMULA:BOXPLOT", StringComparison.OrdinalIgnoreCase) || text3.Equals("FINANCIALFORMULA:FORECASTING", StringComparison.OrdinalIgnoreCase))
											{
												flag9 = true;
											}
											if (text3.StartsWith("ERRORFORMULA", StringComparison.OrdinalIgnoreCase) || text3.StartsWith("FINANCIALFORMULA", StringComparison.OrdinalIgnoreCase) || text3.StartsWith("STATISTICALFORMULA", StringComparison.OrdinalIgnoreCase))
											{
												xmlTextReader.Skip();
												string[] array = xmlTextReader.ReadInnerXml().Split(';');
												string[] array2 = array;
												foreach (string text4 in array2)
												{
													if (text4.Trim().StartsWith("SECONDARYAXIS", StringComparison.OrdinalIgnoreCase))
													{
														flag10 = true;
													}
												}
											}
										}
										else if (xmlTextReader.Name == "DataRows")
										{
											if (xmlTextReader.NodeType == XmlNodeType.Element)
											{
												flag13 = true;
											}
											else if (xmlTextReader.NodeType == XmlNodeType.EndElement)
											{
												flag13 = false;
											}
										}
										else if (flag13 && xmlTextReader.Name == "Name")
										{
											string text5 = xmlTextReader.ReadInnerXml();
											if (text5.StartsWith("CUSTOMVALUE:", StringComparison.OrdinalIgnoreCase))
											{
												flag11 = true;
											}
										}
									}
									while ((!(xmlTextReader.Name == "CustomData") || xmlTextReader.NodeType != XmlNodeType.EndElement) && xmlTextReader.Read());
								}
							}
							xmlTextReader.Skip();
						}
						while (xmlTextReader.NodeType != XmlNodeType.EndElement);
						if (flag)
						{
							if (flag3 || flag4 || flag8 || flag9 || flag10 || flag11)
							{
								rDLUpgradeResult.HasUnsupportedDundasChartFeatures = true;
							}
						}
						else if (flag2 && (flag5 || flag6 || flag7 || flag8))
						{
							rDLUpgradeResult.HasUnsupportedDundasGaugeFeatures = true;
						}
					}
					return rDLUpgradeResult;
				}
				return rDLUpgradeResult;
			}
			catch
			{
				return rDLUpgradeResult;
			}
		}

		internal static string GetCurrentNamespaceURI()
		{
			return RdlUpgrader.Get2016NamespaceURI();
		}

		internal static string Get2010NamespaceURI()
		{
			return RdlUpgrader.Get2010NamespaceURI();
		}

		internal static string Get2009NamespaceURI()
		{
			return RdlUpgrader.Get2009NamespaceURI();
		}

		internal static string Get2008NamespaceURI()
		{
			return RdlUpgrader.Get2008NamespaceURI();
		}

		internal static string Get2007NamespaceURI()
		{
			return RdlUpgrader.Get2007NamespaceURI();
		}

		internal static string Get2005NamespaceURI()
		{
			return RdlUpgrader.Get2005NamespaceURI();
		}
	}
}
