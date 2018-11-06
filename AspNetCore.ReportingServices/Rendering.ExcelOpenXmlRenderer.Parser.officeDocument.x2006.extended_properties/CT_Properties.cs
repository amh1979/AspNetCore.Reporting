using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.officeDocument.x2006.extended_properties
{
	internal class CT_Properties : OoxmlComplexType
	{
		private string _Template;

		private string _Manager;

		private string _Company;

		private int _Pages;

		private int _Words;

		private int _Characters;

		private string _PresentationFormat;

		private int _Lines;

		private int _Paragraphs;

		private int _Slides;

		private int _Notes;

		private int _TotalTime;

		private int _HiddenSlides;

		private int _MMClips;

		private OoxmlBool _ScaleCrop;

		private CT_VectorVariant _HeadingPairs;

		private CT_VectorLpstr _TitlesOfParts;

		private OoxmlBool _LinksUpToDate;

		private int _CharactersWithSpaces;

		private OoxmlBool _SharedDoc;

		private string _HyperlinkBase;

		private CT_VectorVariant _HLinks;

		private OoxmlBool _HyperlinksChanged;

		private string _Application;

		private string _AppVersion;

		private int _DocSecurity;

		public string Template
		{
			get
			{
				return this._Template;
			}
			set
			{
				this._Template = value;
			}
		}

		public string Manager
		{
			get
			{
				return this._Manager;
			}
			set
			{
				this._Manager = value;
			}
		}

		public string Company
		{
			get
			{
				return this._Company;
			}
			set
			{
				this._Company = value;
			}
		}

		public int Pages
		{
			get
			{
				return this._Pages;
			}
			set
			{
				this._Pages = value;
			}
		}

		public int Words
		{
			get
			{
				return this._Words;
			}
			set
			{
				this._Words = value;
			}
		}

		public int Characters
		{
			get
			{
				return this._Characters;
			}
			set
			{
				this._Characters = value;
			}
		}

		public string PresentationFormat
		{
			get
			{
				return this._PresentationFormat;
			}
			set
			{
				this._PresentationFormat = value;
			}
		}

		public int Lines
		{
			get
			{
				return this._Lines;
			}
			set
			{
				this._Lines = value;
			}
		}

		public int Paragraphs
		{
			get
			{
				return this._Paragraphs;
			}
			set
			{
				this._Paragraphs = value;
			}
		}

		public int Slides
		{
			get
			{
				return this._Slides;
			}
			set
			{
				this._Slides = value;
			}
		}

		public int Notes
		{
			get
			{
				return this._Notes;
			}
			set
			{
				this._Notes = value;
			}
		}

		public int TotalTime
		{
			get
			{
				return this._TotalTime;
			}
			set
			{
				this._TotalTime = value;
			}
		}

		public int HiddenSlides
		{
			get
			{
				return this._HiddenSlides;
			}
			set
			{
				this._HiddenSlides = value;
			}
		}

		public int MMClips
		{
			get
			{
				return this._MMClips;
			}
			set
			{
				this._MMClips = value;
			}
		}

		public OoxmlBool ScaleCrop
		{
			get
			{
				return this._ScaleCrop;
			}
			set
			{
				this._ScaleCrop = value;
			}
		}

		public CT_VectorVariant HeadingPairs
		{
			get
			{
				return this._HeadingPairs;
			}
			set
			{
				this._HeadingPairs = value;
			}
		}

		public CT_VectorLpstr TitlesOfParts
		{
			get
			{
				return this._TitlesOfParts;
			}
			set
			{
				this._TitlesOfParts = value;
			}
		}

		public OoxmlBool LinksUpToDate
		{
			get
			{
				return this._LinksUpToDate;
			}
			set
			{
				this._LinksUpToDate = value;
			}
		}

		public int CharactersWithSpaces
		{
			get
			{
				return this._CharactersWithSpaces;
			}
			set
			{
				this._CharactersWithSpaces = value;
			}
		}

		public OoxmlBool SharedDoc
		{
			get
			{
				return this._SharedDoc;
			}
			set
			{
				this._SharedDoc = value;
			}
		}

		public string HyperlinkBase
		{
			get
			{
				return this._HyperlinkBase;
			}
			set
			{
				this._HyperlinkBase = value;
			}
		}

		public CT_VectorVariant HLinks
		{
			get
			{
				return this._HLinks;
			}
			set
			{
				this._HLinks = value;
			}
		}

		public OoxmlBool HyperlinksChanged
		{
			get
			{
				return this._HyperlinksChanged;
			}
			set
			{
				this._HyperlinksChanged = value;
			}
		}

		public string Application
		{
			get
			{
				return this._Application;
			}
			set
			{
				this._Application = value;
			}
		}

		public string AppVersion
		{
			get
			{
				return this._AppVersion;
			}
			set
			{
				this._AppVersion = value;
			}
		}

		public int DocSecurity
		{
			get
			{
				return this._DocSecurity;
			}
			set
			{
				this._DocSecurity = value;
			}
		}

		public static string HeadingPairsElementName
		{
			get
			{
				return "HeadingPairs";
			}
		}

		public static string TitlesOfPartsElementName
		{
			get
			{
				return "TitlesOfParts";
			}
		}

		public static string HLinksElementName
		{
			get
			{
				return "HLinks";
			}
		}

		public static string PagesElementName
		{
			get
			{
				return "Pages";
			}
		}

		public static string WordsElementName
		{
			get
			{
				return "Words";
			}
		}

		public static string CharactersElementName
		{
			get
			{
				return "Characters";
			}
		}

		public static string LinesElementName
		{
			get
			{
				return "Lines";
			}
		}

		public static string ParagraphsElementName
		{
			get
			{
				return "Paragraphs";
			}
		}

		public static string SlidesElementName
		{
			get
			{
				return "Slides";
			}
		}

		public static string NotesElementName
		{
			get
			{
				return "Notes";
			}
		}

		public static string TotalTimeElementName
		{
			get
			{
				return "TotalTime";
			}
		}

		public static string HiddenSlidesElementName
		{
			get
			{
				return "HiddenSlides";
			}
		}

		public static string MMClipsElementName
		{
			get
			{
				return "MMClips";
			}
		}

		public static string ScaleCropElementName
		{
			get
			{
				return "ScaleCrop";
			}
		}

		public static string LinksUpToDateElementName
		{
			get
			{
				return "LinksUpToDate";
			}
		}

		public static string CharactersWithSpacesElementName
		{
			get
			{
				return "CharactersWithSpaces";
			}
		}

		public static string SharedDocElementName
		{
			get
			{
				return "SharedDoc";
			}
		}

		public static string HyperlinksChangedElementName
		{
			get
			{
				return "HyperlinksChanged";
			}
		}

		public static string DocSecurityElementName
		{
			get
			{
				return "DocSecurity";
			}
		}

		public static string TemplateElementName
		{
			get
			{
				return "Template";
			}
		}

		public static string ManagerElementName
		{
			get
			{
				return "Manager";
			}
		}

		public static string CompanyElementName
		{
			get
			{
				return "Company";
			}
		}

		public static string PresentationFormatElementName
		{
			get
			{
				return "PresentationFormat";
			}
		}

		public static string HyperlinkBaseElementName
		{
			get
			{
				return "HyperlinkBase";
			}
		}

		public static string ApplicationElementName
		{
			get
			{
				return "Application";
			}
		}

		public static string AppVersionElementName
		{
			get
			{
				return "AppVersion";
			}
		}

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
		}

		public override void WriteAsRoot(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			this.WriteOpenTag(s, tagName, depth, namespaces, true);
			this.WriteElements(s, depth, namespaces);
			this.WriteCloseTag(s, tagName, depth, namespaces);
		}

		public override void Write(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			this.WriteOpenTag(s, tagName, depth, namespaces, false);
			this.WriteElements(s, depth, namespaces);
			this.WriteCloseTag(s, tagName, depth, namespaces);
		}

		public override void WriteOpenTag(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces, bool root)
		{
			s.Write("<");
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties");
			s.Write(tagName);
			this.WriteAttributes(s);
			if (root)
			{
				foreach (string key in namespaces.Keys)
				{
					s.Write(" xmlns");
					if (namespaces[key] != "")
					{
						s.Write(":");
						s.Write(namespaces[key]);
					}
					s.Write("=\"");
					s.Write(key);
					s.Write("\"");
				}
			}
			s.Write(">");
		}

		public override void WriteCloseTag(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			s.Write("</");
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			this.Write_Template(s, depth, namespaces);
			this.Write_Manager(s, depth, namespaces);
			this.Write_Company(s, depth, namespaces);
			this.Write_Pages(s, depth, namespaces);
			this.Write_Words(s, depth, namespaces);
			this.Write_Characters(s, depth, namespaces);
			this.Write_PresentationFormat(s, depth, namespaces);
			this.Write_Lines(s, depth, namespaces);
			this.Write_Paragraphs(s, depth, namespaces);
			this.Write_Slides(s, depth, namespaces);
			this.Write_Notes(s, depth, namespaces);
			this.Write_TotalTime(s, depth, namespaces);
			this.Write_HiddenSlides(s, depth, namespaces);
			this.Write_MMClips(s, depth, namespaces);
			this.Write_ScaleCrop(s, depth, namespaces);
			this.Write_HeadingPairs(s, depth, namespaces);
			this.Write_TitlesOfParts(s, depth, namespaces);
			this.Write_LinksUpToDate(s, depth, namespaces);
			this.Write_CharactersWithSpaces(s, depth, namespaces);
			this.Write_SharedDoc(s, depth, namespaces);
			this.Write_HyperlinkBase(s, depth, namespaces);
			this.Write_HLinks(s, depth, namespaces);
			this.Write_HyperlinksChanged(s, depth, namespaces);
			this.Write_Application(s, depth, namespaces);
			this.Write_AppVersion(s, depth, namespaces);
			this.Write_DocSecurity(s, depth, namespaces);
		}

		public void Write_HeadingPairs(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._HeadingPairs != null)
			{
				this._HeadingPairs.Write(s, "HeadingPairs", depth + 1, namespaces);
			}
		}

		public void Write_TitlesOfParts(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._TitlesOfParts != null)
			{
				this._TitlesOfParts.Write(s, "TitlesOfParts", depth + 1, namespaces);
			}
		}

		public void Write_HLinks(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._HLinks != null)
			{
				this._HLinks.Write(s, "HLinks", depth + 1, namespaces);
			}
		}

		public void Write_Pages(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "Pages", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", this._Pages);
		}

		public void Write_Words(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "Words", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", this._Words);
		}

		public void Write_Characters(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "Characters", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", this._Characters);
		}

		public void Write_Lines(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "Lines", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", this._Lines);
		}

		public void Write_Paragraphs(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "Paragraphs", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", this._Paragraphs);
		}

		public void Write_Slides(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "Slides", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", this._Slides);
		}

		public void Write_Notes(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "Notes", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", this._Notes);
		}

		public void Write_TotalTime(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "TotalTime", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", this._TotalTime);
		}

		public void Write_HiddenSlides(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "HiddenSlides", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", this._HiddenSlides);
		}

		public void Write_MMClips(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "MMClips", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", this._MMClips);
		}

		public void Write_ScaleCrop(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "ScaleCrop", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", this._ScaleCrop);
		}

		public void Write_LinksUpToDate(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "LinksUpToDate", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", this._LinksUpToDate);
		}

		public void Write_CharactersWithSpaces(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "CharactersWithSpaces", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", this._CharactersWithSpaces);
		}

		public void Write_SharedDoc(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "SharedDoc", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", this._SharedDoc);
		}

		public void Write_HyperlinksChanged(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "HyperlinksChanged", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", this._HyperlinksChanged);
		}

		public void Write_DocSecurity(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, "DocSecurity", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", this._DocSecurity);
		}

		public void Write_Template(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._Template != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "Template", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", this._Template);
			}
		}

		public void Write_Manager(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._Manager != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "Manager", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", this._Manager);
			}
		}

		public void Write_Company(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._Company != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "Company", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", this._Company);
			}
		}

		public void Write_PresentationFormat(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._PresentationFormat != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "PresentationFormat", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", this._PresentationFormat);
			}
		}

		public void Write_HyperlinkBase(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._HyperlinkBase != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "HyperlinkBase", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", this._HyperlinkBase);
			}
		}

		public void Write_Application(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._Application != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "Application", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", this._Application);
			}
		}

		public void Write_AppVersion(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._AppVersion != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "AppVersion", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties", this._AppVersion);
			}
		}
	}
}
