namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_SheetViewType
	{
		private string _ooxmlEnumerationValue;

		private static ST_SheetViewType _normal;

		private static ST_SheetViewType _pageBreakPreview;

		private static ST_SheetViewType _pageLayout;

		public static ST_SheetViewType normal
		{
			get
			{
				return ST_SheetViewType._normal;
			}
			private set
			{
				ST_SheetViewType._normal = value;
			}
		}

		public static ST_SheetViewType pageBreakPreview
		{
			get
			{
				return ST_SheetViewType._pageBreakPreview;
			}
			private set
			{
				ST_SheetViewType._pageBreakPreview = value;
			}
		}

		public static ST_SheetViewType pageLayout
		{
			get
			{
				return ST_SheetViewType._pageLayout;
			}
			private set
			{
				ST_SheetViewType._pageLayout = value;
			}
		}

		static ST_SheetViewType()
		{
			ST_SheetViewType.normal = new ST_SheetViewType("normal");
			ST_SheetViewType.pageBreakPreview = new ST_SheetViewType("pageBreakPreview");
			ST_SheetViewType.pageLayout = new ST_SheetViewType("pageLayout");
		}

		private ST_SheetViewType(string val)
		{
			this._ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return this._ooxmlEnumerationValue;
		}

		public bool Equals(ST_SheetViewType other)
		{
			if (other == (ST_SheetViewType)null)
			{
				return false;
			}
			return this._ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_SheetViewType one, ST_SheetViewType two)
		{
			if ((object)one == null && (object)two == null)
			{
				return true;
			}
			if ((object)one != null && (object)two != null)
			{
				return one._ooxmlEnumerationValue == two._ooxmlEnumerationValue;
			}
			return false;
		}

		public static bool operator !=(ST_SheetViewType one, ST_SheetViewType two)
		{
			return !(one == two);
		}

		public override int GetHashCode()
		{
			return this._ooxmlEnumerationValue.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}
	}
}
