namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_FontScheme
	{
		private string _ooxmlEnumerationValue;

		private static ST_FontScheme _none;

		private static ST_FontScheme _major;

		private static ST_FontScheme _minor;

		public static ST_FontScheme none
		{
			get
			{
				return ST_FontScheme._none;
			}
			private set
			{
				ST_FontScheme._none = value;
			}
		}

		public static ST_FontScheme major
		{
			get
			{
				return ST_FontScheme._major;
			}
			private set
			{
				ST_FontScheme._major = value;
			}
		}

		public static ST_FontScheme minor
		{
			get
			{
				return ST_FontScheme._minor;
			}
			private set
			{
				ST_FontScheme._minor = value;
			}
		}

		static ST_FontScheme()
		{
			ST_FontScheme.none = new ST_FontScheme("none");
			ST_FontScheme.major = new ST_FontScheme("major");
			ST_FontScheme.minor = new ST_FontScheme("minor");
		}

		private ST_FontScheme(string val)
		{
			this._ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return this._ooxmlEnumerationValue;
		}

		public bool Equals(ST_FontScheme other)
		{
			if (other == (ST_FontScheme)null)
			{
				return false;
			}
			return this._ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_FontScheme one, ST_FontScheme two)
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

		public static bool operator !=(ST_FontScheme one, ST_FontScheme two)
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
