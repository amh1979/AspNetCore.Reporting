namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_PrintError
	{
		private string _ooxmlEnumerationValue;

		private static ST_PrintError _displayed;

		private static ST_PrintError _blank;

		private static ST_PrintError _dash;

		private static ST_PrintError _NA;

		public static ST_PrintError displayed
		{
			get
			{
				return ST_PrintError._displayed;
			}
			private set
			{
				ST_PrintError._displayed = value;
			}
		}

		public static ST_PrintError blank
		{
			get
			{
				return ST_PrintError._blank;
			}
			private set
			{
				ST_PrintError._blank = value;
			}
		}

		public static ST_PrintError dash
		{
			get
			{
				return ST_PrintError._dash;
			}
			private set
			{
				ST_PrintError._dash = value;
			}
		}

		public static ST_PrintError NA
		{
			get
			{
				return ST_PrintError._NA;
			}
			private set
			{
				ST_PrintError._NA = value;
			}
		}

		static ST_PrintError()
		{
			ST_PrintError.displayed = new ST_PrintError("displayed");
			ST_PrintError.blank = new ST_PrintError("blank");
			ST_PrintError.dash = new ST_PrintError("dash");
			ST_PrintError.NA = new ST_PrintError("NA");
		}

		private ST_PrintError(string val)
		{
			this._ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return this._ooxmlEnumerationValue;
		}

		public bool Equals(ST_PrintError other)
		{
			if (other == (ST_PrintError)null)
			{
				return false;
			}
			return this._ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_PrintError one, ST_PrintError two)
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

		public static bool operator !=(ST_PrintError one, ST_PrintError two)
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
