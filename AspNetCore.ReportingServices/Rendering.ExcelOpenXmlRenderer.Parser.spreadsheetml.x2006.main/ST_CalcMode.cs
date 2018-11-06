namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_CalcMode
	{
		private string _ooxmlEnumerationValue;

		private static ST_CalcMode _manual;

		private static ST_CalcMode _auto;

		private static ST_CalcMode _autoNoTable;

		public static ST_CalcMode manual
		{
			get
			{
				return ST_CalcMode._manual;
			}
			private set
			{
				ST_CalcMode._manual = value;
			}
		}

		public static ST_CalcMode auto
		{
			get
			{
				return ST_CalcMode._auto;
			}
			private set
			{
				ST_CalcMode._auto = value;
			}
		}

		public static ST_CalcMode autoNoTable
		{
			get
			{
				return ST_CalcMode._autoNoTable;
			}
			private set
			{
				ST_CalcMode._autoNoTable = value;
			}
		}

		static ST_CalcMode()
		{
			ST_CalcMode.manual = new ST_CalcMode("manual");
			ST_CalcMode.auto = new ST_CalcMode("auto");
			ST_CalcMode.autoNoTable = new ST_CalcMode("autoNoTable");
		}

		private ST_CalcMode(string val)
		{
			this._ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return this._ooxmlEnumerationValue;
		}

		public bool Equals(ST_CalcMode other)
		{
			if (other == (ST_CalcMode)null)
			{
				return false;
			}
			return this._ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_CalcMode one, ST_CalcMode two)
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

		public static bool operator !=(ST_CalcMode one, ST_CalcMode two)
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
