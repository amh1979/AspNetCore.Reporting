namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.spreadsheetDrawing
{
	internal class ST_EditAs
	{
		private string _ooxmlEnumerationValue;

		private static ST_EditAs _twoCell;

		private static ST_EditAs _oneCell;

		private static ST_EditAs _absolute;

		public static ST_EditAs twoCell
		{
			get
			{
				return ST_EditAs._twoCell;
			}
			private set
			{
				ST_EditAs._twoCell = value;
			}
		}

		public static ST_EditAs oneCell
		{
			get
			{
				return ST_EditAs._oneCell;
			}
			private set
			{
				ST_EditAs._oneCell = value;
			}
		}

		public static ST_EditAs absolute
		{
			get
			{
				return ST_EditAs._absolute;
			}
			private set
			{
				ST_EditAs._absolute = value;
			}
		}

		static ST_EditAs()
		{
			ST_EditAs.twoCell = new ST_EditAs("twoCell");
			ST_EditAs.oneCell = new ST_EditAs("oneCell");
			ST_EditAs.absolute = new ST_EditAs("absolute");
		}

		private ST_EditAs(string val)
		{
			this._ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return this._ooxmlEnumerationValue;
		}

		public bool Equals(ST_EditAs other)
		{
			if (other == (ST_EditAs)null)
			{
				return false;
			}
			return this._ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_EditAs one, ST_EditAs two)
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

		public static bool operator !=(ST_EditAs one, ST_EditAs two)
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
