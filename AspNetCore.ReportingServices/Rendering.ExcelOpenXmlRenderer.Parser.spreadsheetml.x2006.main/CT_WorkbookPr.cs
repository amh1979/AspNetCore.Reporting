using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_WorkbookPr : OoxmlComplexType
	{
		private OoxmlBool _date1904_attr;

		private OoxmlBool _dateCompatibility_attr;

		private ST_Objects _showObjects_attr;

		private OoxmlBool _showBorderUnselectedTables_attr;

		private OoxmlBool _filterPrivacy_attr;

		private OoxmlBool _promptedSolutions_attr;

		private OoxmlBool _showInkAnnotation_attr;

		private OoxmlBool _backupFile_attr;

		private OoxmlBool _saveExternalLinkValues_attr;

		private ST_UpdateLinks _updateLinks_attr;

		private OoxmlBool _hidePivotFieldList_attr;

		private OoxmlBool _showPivotChartFilter_attr;

		private OoxmlBool _allowRefreshQuery_attr;

		private OoxmlBool _publishItems_attr;

		private OoxmlBool _checkCompatibility_attr;

		private OoxmlBool _autoCompressPictures_attr;

		private OoxmlBool _refreshAllConnections_attr;

		private string _codeName_attr;

		private bool _codeName_attr_is_specified;

		private uint _defaultThemeVersion_attr;

		private bool _defaultThemeVersion_attr_is_specified;

		public OoxmlBool Date1904_Attr
		{
			get
			{
				return this._date1904_attr;
			}
			set
			{
				this._date1904_attr = value;
			}
		}

		public OoxmlBool DateCompatibility_Attr
		{
			get
			{
				return this._dateCompatibility_attr;
			}
			set
			{
				this._dateCompatibility_attr = value;
			}
		}

		public ST_Objects ShowObjects_Attr
		{
			get
			{
				return this._showObjects_attr;
			}
			set
			{
				this._showObjects_attr = value;
			}
		}

		public OoxmlBool ShowBorderUnselectedTables_Attr
		{
			get
			{
				return this._showBorderUnselectedTables_attr;
			}
			set
			{
				this._showBorderUnselectedTables_attr = value;
			}
		}

		public OoxmlBool FilterPrivacy_Attr
		{
			get
			{
				return this._filterPrivacy_attr;
			}
			set
			{
				this._filterPrivacy_attr = value;
			}
		}

		public OoxmlBool PromptedSolutions_Attr
		{
			get
			{
				return this._promptedSolutions_attr;
			}
			set
			{
				this._promptedSolutions_attr = value;
			}
		}

		public OoxmlBool ShowInkAnnotation_Attr
		{
			get
			{
				return this._showInkAnnotation_attr;
			}
			set
			{
				this._showInkAnnotation_attr = value;
			}
		}

		public OoxmlBool BackupFile_Attr
		{
			get
			{
				return this._backupFile_attr;
			}
			set
			{
				this._backupFile_attr = value;
			}
		}

		public OoxmlBool SaveExternalLinkValues_Attr
		{
			get
			{
				return this._saveExternalLinkValues_attr;
			}
			set
			{
				this._saveExternalLinkValues_attr = value;
			}
		}

		public ST_UpdateLinks UpdateLinks_Attr
		{
			get
			{
				return this._updateLinks_attr;
			}
			set
			{
				this._updateLinks_attr = value;
			}
		}

		public OoxmlBool HidePivotFieldList_Attr
		{
			get
			{
				return this._hidePivotFieldList_attr;
			}
			set
			{
				this._hidePivotFieldList_attr = value;
			}
		}

		public OoxmlBool ShowPivotChartFilter_Attr
		{
			get
			{
				return this._showPivotChartFilter_attr;
			}
			set
			{
				this._showPivotChartFilter_attr = value;
			}
		}

		public OoxmlBool AllowRefreshQuery_Attr
		{
			get
			{
				return this._allowRefreshQuery_attr;
			}
			set
			{
				this._allowRefreshQuery_attr = value;
			}
		}

		public OoxmlBool PublishItems_Attr
		{
			get
			{
				return this._publishItems_attr;
			}
			set
			{
				this._publishItems_attr = value;
			}
		}

		public OoxmlBool CheckCompatibility_Attr
		{
			get
			{
				return this._checkCompatibility_attr;
			}
			set
			{
				this._checkCompatibility_attr = value;
			}
		}

		public OoxmlBool AutoCompressPictures_Attr
		{
			get
			{
				return this._autoCompressPictures_attr;
			}
			set
			{
				this._autoCompressPictures_attr = value;
			}
		}

		public OoxmlBool RefreshAllConnections_Attr
		{
			get
			{
				return this._refreshAllConnections_attr;
			}
			set
			{
				this._refreshAllConnections_attr = value;
			}
		}

		public uint DefaultThemeVersion_Attr
		{
			get
			{
				return this._defaultThemeVersion_attr;
			}
			set
			{
				this._defaultThemeVersion_attr = value;
				this._defaultThemeVersion_attr_is_specified = true;
			}
		}

		public bool DefaultThemeVersion_Attr_Is_Specified
		{
			get
			{
				return this._defaultThemeVersion_attr_is_specified;
			}
			set
			{
				this._defaultThemeVersion_attr_is_specified = value;
			}
		}

		public string CodeName_Attr
		{
			get
			{
				return this._codeName_attr;
			}
			set
			{
				this._codeName_attr = value;
				this._codeName_attr_is_specified = (value != null);
			}
		}

		protected override void InitAttributes()
		{
			this._date1904_attr = OoxmlBool.OoxmlFalse;
			this._dateCompatibility_attr = OoxmlBool.OoxmlTrue;
			this._showObjects_attr = ST_Objects.all;
			this._showBorderUnselectedTables_attr = OoxmlBool.OoxmlTrue;
			this._filterPrivacy_attr = OoxmlBool.OoxmlFalse;
			this._promptedSolutions_attr = OoxmlBool.OoxmlFalse;
			this._showInkAnnotation_attr = OoxmlBool.OoxmlTrue;
			this._backupFile_attr = OoxmlBool.OoxmlFalse;
			this._saveExternalLinkValues_attr = OoxmlBool.OoxmlTrue;
			this._updateLinks_attr = ST_UpdateLinks.userSet;
			this._hidePivotFieldList_attr = OoxmlBool.OoxmlFalse;
			this._showPivotChartFilter_attr = OoxmlBool.OoxmlFalse;
			this._allowRefreshQuery_attr = OoxmlBool.OoxmlFalse;
			this._publishItems_attr = OoxmlBool.OoxmlFalse;
			this._checkCompatibility_attr = OoxmlBool.OoxmlFalse;
			this._autoCompressPictures_attr = OoxmlBool.OoxmlTrue;
			this._refreshAllConnections_attr = OoxmlBool.OoxmlFalse;
			this._codeName_attr_is_specified = false;
			this._defaultThemeVersion_attr_is_specified = false;
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
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
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
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
			if ((bool)(this._date1904_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" date1904=\"");
				OoxmlComplexType.WriteData(s, this._date1904_attr);
				s.Write("\"");
			}
			if ((bool)(this._dateCompatibility_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" dateCompatibility=\"");
				OoxmlComplexType.WriteData(s, this._dateCompatibility_attr);
				s.Write("\"");
			}
			if (this._showObjects_attr != ST_Objects.all)
			{
				s.Write(" showObjects=\"");
				OoxmlComplexType.WriteData(s, this._showObjects_attr);
				s.Write("\"");
			}
			if ((bool)(this._showBorderUnselectedTables_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" showBorderUnselectedTables=\"");
				OoxmlComplexType.WriteData(s, this._showBorderUnselectedTables_attr);
				s.Write("\"");
			}
			if ((bool)(this._filterPrivacy_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" filterPrivacy=\"");
				OoxmlComplexType.WriteData(s, this._filterPrivacy_attr);
				s.Write("\"");
			}
			if ((bool)(this._promptedSolutions_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" promptedSolutions=\"");
				OoxmlComplexType.WriteData(s, this._promptedSolutions_attr);
				s.Write("\"");
			}
			if ((bool)(this._showInkAnnotation_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" showInkAnnotation=\"");
				OoxmlComplexType.WriteData(s, this._showInkAnnotation_attr);
				s.Write("\"");
			}
			if ((bool)(this._backupFile_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" backupFile=\"");
				OoxmlComplexType.WriteData(s, this._backupFile_attr);
				s.Write("\"");
			}
			if ((bool)(this._saveExternalLinkValues_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" saveExternalLinkValues=\"");
				OoxmlComplexType.WriteData(s, this._saveExternalLinkValues_attr);
				s.Write("\"");
			}
			if (this._updateLinks_attr != ST_UpdateLinks.userSet)
			{
				s.Write(" updateLinks=\"");
				OoxmlComplexType.WriteData(s, this._updateLinks_attr);
				s.Write("\"");
			}
			if ((bool)(this._hidePivotFieldList_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" hidePivotFieldList=\"");
				OoxmlComplexType.WriteData(s, this._hidePivotFieldList_attr);
				s.Write("\"");
			}
			if ((bool)(this._showPivotChartFilter_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" showPivotChartFilter=\"");
				OoxmlComplexType.WriteData(s, this._showPivotChartFilter_attr);
				s.Write("\"");
			}
			if ((bool)(this._allowRefreshQuery_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" allowRefreshQuery=\"");
				OoxmlComplexType.WriteData(s, this._allowRefreshQuery_attr);
				s.Write("\"");
			}
			if ((bool)(this._publishItems_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" publishItems=\"");
				OoxmlComplexType.WriteData(s, this._publishItems_attr);
				s.Write("\"");
			}
			if ((bool)(this._checkCompatibility_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" checkCompatibility=\"");
				OoxmlComplexType.WriteData(s, this._checkCompatibility_attr);
				s.Write("\"");
			}
			if ((bool)(this._autoCompressPictures_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" autoCompressPictures=\"");
				OoxmlComplexType.WriteData(s, this._autoCompressPictures_attr);
				s.Write("\"");
			}
			if ((bool)(this._refreshAllConnections_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" refreshAllConnections=\"");
				OoxmlComplexType.WriteData(s, this._refreshAllConnections_attr);
				s.Write("\"");
			}
			if (this._codeName_attr_is_specified)
			{
				s.Write(" codeName=\"");
				OoxmlComplexType.WriteData(s, this._codeName_attr);
				s.Write("\"");
			}
			if (this._defaultThemeVersion_attr_is_specified)
			{
				s.Write(" defaultThemeVersion=\"");
				OoxmlComplexType.WriteData(s, this._defaultThemeVersion_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
		}
	}
}
