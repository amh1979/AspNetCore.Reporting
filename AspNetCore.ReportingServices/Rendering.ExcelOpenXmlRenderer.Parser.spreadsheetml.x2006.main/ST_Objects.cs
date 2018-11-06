namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_Objects
	{
		private string _ooxmlEnumerationValue;

		private static ST_Objects _all;

		private static ST_Objects _placeholders;

		private static ST_Objects _none;

		public static ST_Objects all
		{
			get
			{
				return ST_Objects._all;
			}
			private set
			{
				ST_Objects._all = value;
			}
		}

		public static ST_Objects placeholders
		{
			get
			{
				return ST_Objects._placeholders;
			}
			private set
			{
				ST_Objects._placeholders = value;
			}
		}

		public static ST_Objects none
		{
			get
			{
				return ST_Objects._none;
			}
			private set
			{
				ST_Objects._none = value;
			}
		}

		static ST_Objects()
		{
			ST_Objects.all = new ST_Objects("all");
			ST_Objects.placeholders = new ST_Objects("placeholders");
			ST_Objects.none = new ST_Objects("none");
		}

		private ST_Objects(string val)
		{
			this._ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return this._ooxmlEnumerationValue;
		}

		public bool Equals(ST_Objects other)
		{
			if (other == (ST_Objects)null)
			{
				return false;
			}
			return this._ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_Objects one, ST_Objects two)
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

		public static bool operator !=(ST_Objects one, ST_Objects two)
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
