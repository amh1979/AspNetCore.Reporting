using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models.Relationships;
using AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser;
using AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Packaging;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class OpenXmlDocumentModel
	{
		private sealed class PartInfo
		{
			public OoxmlPart Part
			{
				get;
				set;
			}

			public string PartName
			{
				get;
				set;
			}

			public Stream Stream
			{
				get;
				set;
			}

			public InterleavingWriter Writer
			{
				get;
				set;
			}

			public TableContext TableContext
			{
				get;
				set;
			}

			public long StartingLocation
			{
				get;
				set;
			}
		}

		private struct TemporaryHeaderFooterReferences : OpenXmlSectionPropertiesModel.IHeaderFooterReferences
		{
			public string Header
			{
				get;
				set;
			}

			public string Footer
			{
				get;
				set;
			}

			public string FirstPageHeader
			{
				get;
				set;
			}

			public string FirstPageFooter
			{
				get;
				set;
			}

			public HeaderFooterReferences Store(int index, long location)
			{
				return new HeaderFooterReferences(index, location, this.Footer, this.Header, this.FirstPageHeader, this.FirstPageFooter);
			}
		}

		private Package _zipPackage;

		private PartManager _manager;

		private PartInfo _currentPart;

		private PartInfo _documentPart;

		private Stack<OoxmlComplexType> _tags;

		private OpenXmlListNumberingManager _listManager;

		private OpenXmlSectionPropertiesModel _sectionProperties;

		private WordOpenXmlWriter.CreateXmlStream _createXmlStream;

		private int _headerAndFooterIndex;

		private bool _firstSection = true;

		private TemporaryHeaderFooterReferences _currentHeaderFooterReferences;

		internal Package ZipPackage
		{
			get
			{
				return this._zipPackage;
			}
		}

		internal OoxmlPart Part
		{
			get
			{
				return this._currentPart.Part;
			}
		}

		internal OpenXmlListNumberingManager ListManager
		{
			get
			{
				return this._listManager;
			}
		}

		internal OpenXmlSectionPropertiesModel SectionProperties
		{
			get
			{
				return this._sectionProperties;
			}
		}

		public TableContext TableContext
		{
			get
			{
				return this._currentPart.TableContext;
			}
		}

		public bool SectionHasTitlePage
		{
			set
			{
				this._sectionProperties.HasTitlePage = value;
			}
		}

		public OpenXmlDocumentModel(Stream output, WordOpenXmlWriter.CreateXmlStream createXmlStream, ScalabilityCache scalabilityCache)
		{
			this._createXmlStream = createXmlStream;
			this._zipPackage = Package.Open(output, FileMode.Create);
			this._documentPart = (this._currentPart = new PartInfo());
			this._currentPart.Part = new DocumentPart();
			this._manager = new PartManager(this.ZipPackage);
			Relationship relationship = this._manager.AddStreamingRootPartToTree("application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument", "word/document.xml");
			this.WriteStylesheet();
			this.WriteSettings();
			this._currentPart.PartName = relationship.RelatedPart;
			this._currentPart.Stream = this._createXmlStream("document");
			this._currentPart.Writer = new InterleavingWriter(this._currentPart.Stream, scalabilityCache);
			this._currentHeaderFooterReferences = default(TemporaryHeaderFooterReferences);
			this._tags = new Stack<OoxmlComplexType>();
			this._currentPart.Writer.TextWriter.Write(OoxmlPart.XmlDeclaration);
			CT_Document cT_Document = new CT_Document();
			cT_Document.WriteOpenTag(this._currentPart.Writer.TextWriter, this._currentPart.Part.Tag, this._currentPart.Part.Namespaces);
			this._tags.Push(cT_Document);
			CT_Body ctObject = new CT_Body();
			this.WriteStartTag(ctObject, CT_Document.BodyElementName);
			this._currentPart.TableContext = new TableContext(this._currentPart.Writer, false);
			this._listManager = new OpenXmlListNumberingManager();
			this._sectionProperties = new OpenXmlSectionPropertiesModel();
		}

		private CT_OnOff On()
		{
			CT_OnOff cT_OnOff = new CT_OnOff();
			cT_OnOff.Val_Attr = true;
			return cT_OnOff;
		}

		private void WriteSettings()
		{
			SettingsPart settingsPart = new SettingsPart();
			CT_Settings cT_Settings = (CT_Settings)settingsPart.Root;
			cT_Settings.Compat = new CT_Compat
			{
				FootnoteLayoutLikeWW8 = this.On(),
				ShapeLayoutLikeWW8 = this.On(),
				AlignTablesRowByRow = this.On(),
				ForgetLastTabAlignment = this.On(),
				DoNotUseHTMLParagraphAutoSpacing = this.On(),
				LayoutRawTableWidth = this.On(),
				LayoutTableRowsApart = this.On(),
				UseWord97LineBreakRules = this.On(),
				DoNotBreakWrappedTables = this.On(),
				DoNotSnapToGridInCell = this.On(),
				SelectFldWithFirstOrLastChar = this.On(),
				DoNotWrapTextWithPunct = this.On(),
				DoNotUseEastAsianBreakRules = this.On(),
				UseWord2002TableStyleRules = this.On(),
				GrowAutofit = this.On(),
				UseNormalStyleForList = this.On(),
				DoNotUseIndentAsNumberingTabStop = this.On(),
				UseAltKinsokuLineBreakRules = this.On(),
				AllowSpaceOfSameStyleInTable = this.On(),
				DoNotSuppressIndentation = this.On(),
				DoNotAutofitConstrainedTables = this.On(),
				AutofitToFirstFixedWidthCell = this.On(),
				UnderlineTabInNumList = this.On(),
				DisplayHangulFixedWidth = this.On(),
				SplitPgBreakAndParaMark = this.On(),
				DoNotVertAlignCellWithSp = this.On(),
				DoNotBreakConstrainedForcedTable = this.On(),
				DoNotVertAlignInTxbx = this.On(),
				UseAnsiKerningPairs = this.On(),
				CachedColBalance = this.On()
			};
			this._manager.WriteStaticPart(settingsPart, "application/vnd.openxmlformats-officedocument.wordprocessingml.settings+xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/settings", "word/settings.xml", this._manager.GetRootPart());
		}

		private void WriteNumberingPart()
		{
			this._manager.WriteStaticPart(this._listManager.CreateNumberingPart(), "application/vnd.openxmlformats-officedocument.wordprocessingml.numbering+xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/numbering", "word/numbering.xml", this._manager.GetRootPart());
		}

		private void WriteStylesheet()
		{
			CT_RPr cT_RPr = new CT_RPr();
			cT_RPr.RFonts = new CT_Fonts
			{
				Ascii_Attr = "Times New Roman",
				EastAsia_Attr = "Times New Roman",
				HAnsi_Attr = "Times New Roman",
				Cs_Attr = "Times New Roman"
			};
			CT_RPr rPr = cT_RPr;
			CT_DocDefaults cT_DocDefaults = new CT_DocDefaults();
			cT_DocDefaults.RPrDefault = new CT_RPrDefault
			{
				RPr = rPr
			};
			CT_DocDefaults docDefaults = cT_DocDefaults;
			CT_Style cT_Style = new CT_Style();
			cT_Style.Name = new CT_String
			{
				Val_Attr = "EmptyCellLayoutStyle"
			};
			cT_Style.BasedOn = new CT_String
			{
				Val_Attr = "Normal"
			};
			cT_Style.RPr = new CT_RPr
			{
				Sz = new CT_HpsMeasure
				{
					Val_Attr = 2.ToString(CultureInfo.InvariantCulture)
				}
			};
			CT_Style item = cT_Style;
			StylesPart stylesPart = new StylesPart();
			((CT_Styles)stylesPart.Root).DocDefaults = docDefaults;
			((CT_Styles)stylesPart.Root).Style.Add(item);
			this._manager.WriteStaticPart(stylesPart, "application/vnd.openxmlformats-officedocument.wordprocessingml.styles+xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles", "word/styles.xml", this._manager.GetRootPart());
		}

		private void StartHeaderOrFooter(OoxmlPart part, Relationship relationship)
		{
			this._currentPart = new PartInfo();
			this._currentPart.Part = part;
			this._currentPart.PartName = relationship.RelatedPart;
			this._currentPart.Stream = this._createXmlStream(this._currentPart.PartName);
			this._currentPart.Writer = new InterleavingWriter(this._currentPart.Stream);
			this._currentPart.Writer.TextWriter.Write(OoxmlPart.XmlDeclaration);
			CT_HdrFtr cT_HdrFtr = new CT_HdrFtr();
			cT_HdrFtr.WriteOpenTag(this._currentPart.Writer.TextWriter, this._currentPart.Part.Tag, this._currentPart.Part.Namespaces);
			this._tags.Push(cT_HdrFtr);
			this._currentPart.TableContext = new TableContext(this._currentPart.Writer, true);
			this._currentPart.StartingLocation = this._currentPart.Writer.Location;
		}

		private void FinishHeaderOrFooter()
		{
			if (this._currentPart.Writer.Location == this._currentPart.StartingLocation)
			{
				OpenXmlParagraphModel.WriteInvisibleParagraph(this._currentPart.Writer.TextWriter);
			}
			this.WriteCloseTag(this._currentPart.Part.Tag);
			Package zipPackage = this.ZipPackage;
			PackagePart part = zipPackage.GetPart(new Uri(PartManager.CleanName(this._currentPart.PartName), UriKind.Relative));
			Stream stream = part.GetStream();
			this._currentPart.Writer.Interleave(stream, this.WriteInterleaverToHeaderOrFooter);
			this._currentPart.Stream.Dispose();
			this._currentPart = this._documentPart;
		}

		internal Relationship WriteImageData(byte[] imgBuf, ImageHash hash, string extension)
		{
			return this._manager.AddImageToTree(new MemoryStream(imgBuf), hash, extension, "http://schemas.openxmlformats.org/officeDocument/2006/relationships/image", "word/media/img{0}.{{0}}", this._currentPart.PartName);
		}

		internal void WriteDocumentProperties(string title, string author, string description)
		{
			OpenXmlDocumentPropertiesModel openXmlDocumentPropertiesModel = new OpenXmlDocumentPropertiesModel(author, title, description);
			this._manager.WriteStaticRootPart(openXmlDocumentPropertiesModel.PropertiesPart, "application/vnd.openxmlformats-package.core-properties+xml", "http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties", "docProps/core.xml");
		}

		internal void WriteParagraph(OpenXmlParagraphModel paragraph)
		{
			paragraph.Write(this._currentPart.Writer.TextWriter);
		}

		internal void WriteEmptyParagraph()
		{
			OpenXmlParagraphModel.WriteEmptyParagraph(this._currentPart.Writer.TextWriter);
		}

		internal void WritePageBreak()
		{
			OpenXmlParagraphModel.WritePageBreakParagraph(this._currentPart.Writer.TextWriter);
		}

		public void WriteSectionBreak()
		{
			this._currentPart.Writer.WriteInterleaver(((TemporaryHeaderFooterReferences)(object)this._currentHeaderFooterReferences).Store);
			this._currentHeaderFooterReferences = default(TemporaryHeaderFooterReferences);
			if (this._firstSection)
			{
				this._sectionProperties.Continuous = true;
				this._firstSection = false;
			}
		}

		internal void Save()
		{
			this._sectionProperties.WriteToBody(this._currentPart.Writer.TextWriter, (OpenXmlSectionPropertiesModel.IHeaderFooterReferences)(object)this._currentHeaderFooterReferences);
			this._firstSection = true;
			this._sectionProperties.Continuous = false;
			this.WriteCloseTag(CT_Document.BodyElementName);
			this.WriteCloseTag(this._currentPart.Part.Tag);
			Package zipPackage = this.ZipPackage;
			PackagePart part = zipPackage.GetPart(new Uri(PartManager.CleanName(this._currentPart.PartName), UriKind.Relative));
			Stream stream = part.GetStream();
			this._currentPart.Writer.Interleave(stream, this.WriteInterleaverToDocument);
			this.WriteNumberingPart();
			this._manager.Write();
		}

		public void WriteStartTag<T>(T ctObject, string elementName) where T : OoxmlComplexType
		{
			this._tags.Push((OoxmlComplexType)(object)ctObject);
			ctObject.WriteOpenTag(this._currentPart.Writer.TextWriter, elementName, null);
		}

		public void WriteCloseTag(string elementName)
		{
			this._tags.Pop().WriteCloseTag(this._currentPart.Writer.TextWriter, elementName);
		}

		public void StartHeader()
		{
			string locationHint = string.Format(CultureInfo.InvariantCulture, "word/header{0}.xml", this._headerAndFooterIndex++);
			Relationship relationship = this._manager.AddStreamingPartToTree("application/vnd.openxmlformats-officedocument.wordprocessingml.header+xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/header", locationHint, this._manager.GetRootPart());
			this._currentHeaderFooterReferences.Header = relationship.RelationshipId;
			this.StartHeaderOrFooter(new HdrPart(), relationship);
		}

		public void StartFirstPageHeader()
		{
			string locationHint = string.Format(CultureInfo.InvariantCulture, "word/header{0}.xml", this._headerAndFooterIndex++);
			Relationship relationship = this._manager.AddStreamingPartToTree("application/vnd.openxmlformats-officedocument.wordprocessingml.header+xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/header", locationHint, this._manager.GetRootPart());
			this._currentHeaderFooterReferences.FirstPageHeader = relationship.RelationshipId;
			this.StartHeaderOrFooter(new HdrPart(), relationship);
		}

		public void StartFooter()
		{
			string locationHint = string.Format(CultureInfo.InvariantCulture, "word/footer{0}.xml", this._headerAndFooterIndex++);
			Relationship relationship = this._manager.AddStreamingPartToTree("application/vnd.openxmlformats-officedocument.wordprocessingml.footer+xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/footer", locationHint, this._manager.GetRootPart());
			this._currentHeaderFooterReferences.Footer = relationship.RelationshipId;
			this.StartHeaderOrFooter(new FtrPart(), relationship);
		}

		public void StartFirstPageFooter()
		{
			string locationHint = string.Format(CultureInfo.InvariantCulture, "word/footer{0}.xml", this._headerAndFooterIndex++);
			Relationship relationship = this._manager.AddStreamingPartToTree("application/vnd.openxmlformats-officedocument.wordprocessingml.footer+xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/footer", locationHint, this._manager.GetRootPart());
			this._currentHeaderFooterReferences.FirstPageFooter = relationship.RelationshipId;
			this.StartHeaderOrFooter(new FtrPart(), relationship);
		}

		public void FinishHeader()
		{
			this.FinishHeaderOrFooter();
		}

		public void FinishFooter()
		{
			this.FinishHeaderOrFooter();
		}

		private void WriteInterleaverToHeaderOrFooter(IInterleave interleaver, TextWriter output)
		{
			interleaver.Write(output);
		}

		private void WriteInterleaverToDocument(IInterleave interleaver, TextWriter output)
		{
			HeaderFooterReferences headerFooterReferences = interleaver as HeaderFooterReferences;
			if (headerFooterReferences != null)
			{
				this._sectionProperties.SetHeaderFooterReferences(headerFooterReferences);
				CT_P cT_P = new CT_P();
				cT_P.PPr = new CT_PPr
				{
					SectPr = this._sectionProperties.CtSectPr
				};
				CT_P cT_P2 = cT_P;
				cT_P2.Write(output, CT_Body.PElementName);
				this._sectionProperties.ResetHeadersAndFooters();
				if (this._firstSection)
				{
					this._sectionProperties.Continuous = true;
					this._firstSection = false;
				}
			}
			else
			{
				interleaver.Write(output);
			}
		}
	}
}
