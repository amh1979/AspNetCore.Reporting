using AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.dc.elements.x1_1;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.package.x2006.metadata.core_properties
{
	internal class CT_CoreProperties : OoxmlComplexType, IOoxmlComplexType
	{
		private string _category;

		private string _contentStatus;

		private string _contentType;

		private SimpleLiteral _creator;

		private SimpleLiteral _description;

		private SimpleLiteral _identifier;

		private string _keywords;

		private SimpleLiteral _language;

		private string _lastModifiedBy;

		private string _revision;

		private SimpleLiteral _subject;

		private SimpleLiteral _title;

		private string _version;

		public string Category
		{
			get
			{
				return this._category;
			}
			set
			{
				this._category = value;
			}
		}

		public string ContentStatus
		{
			get
			{
				return this._contentStatus;
			}
			set
			{
				this._contentStatus = value;
			}
		}

		public string ContentType
		{
			get
			{
				return this._contentType;
			}
			set
			{
				this._contentType = value;
			}
		}

		public SimpleLiteral Creator
		{
			get
			{
				return this._creator;
			}
			set
			{
				this._creator = value;
			}
		}

		public SimpleLiteral Description
		{
			get
			{
				return this._description;
			}
			set
			{
				this._description = value;
			}
		}

		public SimpleLiteral Identifier
		{
			get
			{
				return this._identifier;
			}
			set
			{
				this._identifier = value;
			}
		}

		public string Keywords
		{
			get
			{
				return this._keywords;
			}
			set
			{
				this._keywords = value;
			}
		}

		public SimpleLiteral Language
		{
			get
			{
				return this._language;
			}
			set
			{
				this._language = value;
			}
		}

		public string LastModifiedBy
		{
			get
			{
				return this._lastModifiedBy;
			}
			set
			{
				this._lastModifiedBy = value;
			}
		}

		public string Revision
		{
			get
			{
				return this._revision;
			}
			set
			{
				this._revision = value;
			}
		}

		public SimpleLiteral Subject
		{
			get
			{
				return this._subject;
			}
			set
			{
				this._subject = value;
			}
		}

		public SimpleLiteral Title
		{
			get
			{
				return this._title;
			}
			set
			{
				this._title = value;
			}
		}

		public string Version
		{
			get
			{
				return this._version;
			}
			set
			{
				this._version = value;
			}
		}

		public static string CreatorElementName
		{
			get
			{
				return "creator";
			}
		}

		public static string DescriptionElementName
		{
			get
			{
				return "description";
			}
		}

		public static string IdentifierElementName
		{
			get
			{
				return "identifier";
			}
		}

		public static string LanguageElementName
		{
			get
			{
				return "language";
			}
		}

		public static string SubjectElementName
		{
			get
			{
				return "subject";
			}
		}

		public static string TitleElementName
		{
			get
			{
				return "title";
			}
		}

		public static string CategoryElementName
		{
			get
			{
				return "category";
			}
		}

		public static string ContentStatusElementName
		{
			get
			{
				return "contentStatus";
			}
		}

		public static string ContentTypeElementName
		{
			get
			{
				return "contentType";
			}
		}

		public static string KeywordsElementName
		{
			get
			{
				return "keywords";
			}
		}

		public static string LastModifiedByElementName
		{
			get
			{
				return "lastModifiedBy";
			}
		}

		public static string RevisionElementName
		{
			get
			{
				return "revision";
			}
		}

		public static string VersionElementName
		{
			get
			{
				return "version";
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

		public override void Write(TextWriter s, string tagName)
		{
			this.WriteOpenTag(s, tagName, null);
			this.WriteElements(s);
			this.WriteCloseTag(s, tagName);
		}

		public override void WriteOpenTag(TextWriter s, string tagName, Dictionary<string, string> namespaces)
		{
			base.WriteOpenTag(s, tagName, "cp", namespaces);
		}

		public override void WriteCloseTag(TextWriter s, string tagName)
		{
			s.Write("</cp:");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
		}

		public override void WriteElements(TextWriter s)
		{
			this.Write_category(s);
			this.Write_contentStatus(s);
			this.Write_contentType(s);
			this.Write_creator(s);
			this.Write_description(s);
			this.Write_identifier(s);
			this.Write_keywords(s);
			this.Write_language(s);
			this.Write_lastModifiedBy(s);
			this.Write_revision(s);
			this.Write_subject(s);
			this.Write_title(s);
			this.Write_version(s);
		}

		public void Write_creator(TextWriter s)
		{
			if (this._creator != null)
			{
				this._creator.Write(s, "creator");
			}
		}

		public void Write_description(TextWriter s)
		{
			if (this._description != null)
			{
				this._description.Write(s, "description");
			}
		}

		public void Write_identifier(TextWriter s)
		{
			if (this._identifier != null)
			{
				this._identifier.Write(s, "identifier");
			}
		}

		public void Write_language(TextWriter s)
		{
			if (this._language != null)
			{
				this._language.Write(s, "language");
			}
		}

		public void Write_subject(TextWriter s)
		{
			if (this._subject != null)
			{
				this._subject.Write(s, "subject");
			}
		}

		public void Write_title(TextWriter s)
		{
			if (this._title != null)
			{
				this._title.Write(s, "title");
			}
		}

		public void Write_category(TextWriter s)
		{
			if (this._category != null)
			{
				OoxmlComplexType.WriteRawTag(s, "category", "cp", this._category);
			}
		}

		public void Write_contentStatus(TextWriter s)
		{
			if (this._contentStatus != null)
			{
				OoxmlComplexType.WriteRawTag(s, "contentStatus", "cp", this._contentStatus);
			}
		}

		public void Write_contentType(TextWriter s)
		{
			if (this._contentType != null)
			{
				OoxmlComplexType.WriteRawTag(s, "contentType", "cp", this._contentType);
			}
		}

		public void Write_keywords(TextWriter s)
		{
			if (this._keywords != null)
			{
				OoxmlComplexType.WriteRawTag(s, "keywords", "cp", this._keywords);
			}
		}

		public void Write_lastModifiedBy(TextWriter s)
		{
			if (this._lastModifiedBy != null)
			{
				OoxmlComplexType.WriteRawTag(s, "lastModifiedBy", "cp", this._lastModifiedBy);
			}
		}

		public void Write_revision(TextWriter s)
		{
			if (this._revision != null)
			{
				OoxmlComplexType.WriteRawTag(s, "revision", "cp", this._revision);
			}
		}

		public void Write_version(TextWriter s)
		{
			if (this._version != null)
			{
				OoxmlComplexType.WriteRawTag(s, "version", "cp", this._version);
			}
		}
	}
}
