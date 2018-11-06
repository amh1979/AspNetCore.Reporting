namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_UnderlineValues
	{
		private string _ooxmlEnumerationValue;

		private static ST_UnderlineValues _single;

		private static ST_UnderlineValues __double;

		private static ST_UnderlineValues _singleAccounting;

		private static ST_UnderlineValues _doubleAccounting;

		private static ST_UnderlineValues _none;

		public static ST_UnderlineValues single
		{
			get
			{
				return ST_UnderlineValues._single;
			}
			private set
			{
				ST_UnderlineValues._single = value;
			}
		}

		public static ST_UnderlineValues _double
		{
			get
			{
				return ST_UnderlineValues.__double;
			}
			private set
			{
				ST_UnderlineValues.__double = value;
			}
		}

		public static ST_UnderlineValues singleAccounting
		{
			get
			{
				return ST_UnderlineValues._singleAccounting;
			}
			private set
			{
				ST_UnderlineValues._singleAccounting = value;
			}
		}

		public static ST_UnderlineValues doubleAccounting
		{
			get
			{
				return ST_UnderlineValues._doubleAccounting;
			}
			private set
			{
				ST_UnderlineValues._doubleAccounting = value;
			}
		}

		public static ST_UnderlineValues none
		{
			get
			{
				return ST_UnderlineValues._none;
			}
			private set
			{
				ST_UnderlineValues._none = value;
			}
		}

		static ST_UnderlineValues()
		{
			ST_UnderlineValues.single = new ST_UnderlineValues("single");
			ST_UnderlineValues._double = new ST_UnderlineValues("double");
			ST_UnderlineValues.singleAccounting = new ST_UnderlineValues("singleAccounting");
			ST_UnderlineValues.doubleAccounting = new ST_UnderlineValues("doubleAccounting");
			ST_UnderlineValues.none = new ST_UnderlineValues("none");
		}

		private ST_UnderlineValues(string val)
		{
			this._ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return this._ooxmlEnumerationValue;
		}

		public bool Equals(ST_UnderlineValues other)
		{
			if (other == (ST_UnderlineValues)null)
			{
				return false;
			}
			return this._ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_UnderlineValues one, ST_UnderlineValues two)
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

		public static bool operator !=(ST_UnderlineValues one, ST_UnderlineValues two)
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
