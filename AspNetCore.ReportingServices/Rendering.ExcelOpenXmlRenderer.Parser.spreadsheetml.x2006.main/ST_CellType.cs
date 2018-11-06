namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_CellType
	{
		private string _ooxmlEnumerationValue;

		private static ST_CellType _b;

		private static ST_CellType _d;

		private static ST_CellType _n;

		private static ST_CellType _e;

		private static ST_CellType _s;

		private static ST_CellType _str;

		private static ST_CellType _inlineStr;

		public static ST_CellType b
		{
			get
			{
				return ST_CellType._b;
			}
			private set
			{
				ST_CellType._b = value;
			}
		}

		public static ST_CellType d
		{
			get
			{
				return ST_CellType._d;
			}
			private set
			{
				ST_CellType._d = value;
			}
		}

		public static ST_CellType n
		{
			get
			{
				return ST_CellType._n;
			}
			private set
			{
				ST_CellType._n = value;
			}
		}

		public static ST_CellType e
		{
			get
			{
				return ST_CellType._e;
			}
			private set
			{
				ST_CellType._e = value;
			}
		}

		public static ST_CellType s
		{
			get
			{
				return ST_CellType._s;
			}
			private set
			{
				ST_CellType._s = value;
			}
		}

		public static ST_CellType str
		{
			get
			{
				return ST_CellType._str;
			}
			private set
			{
				ST_CellType._str = value;
			}
		}

		public static ST_CellType inlineStr
		{
			get
			{
				return ST_CellType._inlineStr;
			}
			private set
			{
				ST_CellType._inlineStr = value;
			}
		}

		static ST_CellType()
		{
			ST_CellType.b = new ST_CellType("b");
			ST_CellType.d = new ST_CellType("d");
			ST_CellType.n = new ST_CellType("n");
			ST_CellType.e = new ST_CellType("e");
			ST_CellType.s = new ST_CellType("s");
			ST_CellType.str = new ST_CellType("str");
			ST_CellType.inlineStr = new ST_CellType("inlineStr");
		}

		private ST_CellType(string val)
		{
			this._ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return this._ooxmlEnumerationValue;
		}

		public bool Equals(ST_CellType other)
		{
			if (other == (ST_CellType)null)
			{
				return false;
			}
			return this._ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_CellType one, ST_CellType two)
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

		public static bool operator !=(ST_CellType one, ST_CellType two)
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
