namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_CellComments
	{
		private string _ooxmlEnumerationValue;

		private static ST_CellComments _none;

		private static ST_CellComments _asDisplayed;

		private static ST_CellComments _atEnd;

		public static ST_CellComments none
		{
			get
			{
				return ST_CellComments._none;
			}
			private set
			{
				ST_CellComments._none = value;
			}
		}

		public static ST_CellComments asDisplayed
		{
			get
			{
				return ST_CellComments._asDisplayed;
			}
			private set
			{
				ST_CellComments._asDisplayed = value;
			}
		}

		public static ST_CellComments atEnd
		{
			get
			{
				return ST_CellComments._atEnd;
			}
			private set
			{
				ST_CellComments._atEnd = value;
			}
		}

		static ST_CellComments()
		{
			ST_CellComments.none = new ST_CellComments("none");
			ST_CellComments.asDisplayed = new ST_CellComments("asDisplayed");
			ST_CellComments.atEnd = new ST_CellComments("atEnd");
		}

		private ST_CellComments(string val)
		{
			this._ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return this._ooxmlEnumerationValue;
		}

		public bool Equals(ST_CellComments other)
		{
			if (other == (ST_CellComments)null)
			{
				return false;
			}
			return this._ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_CellComments one, ST_CellComments two)
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

		public static bool operator !=(ST_CellComments one, ST_CellComments two)
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
