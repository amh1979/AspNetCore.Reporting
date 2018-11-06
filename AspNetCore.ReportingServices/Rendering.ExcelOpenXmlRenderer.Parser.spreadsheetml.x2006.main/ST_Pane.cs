namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_Pane
	{
		private string _ooxmlEnumerationValue;

		private static ST_Pane _bottomRight;

		private static ST_Pane _topRight;

		private static ST_Pane _bottomLeft;

		private static ST_Pane _topLeft;

		public static ST_Pane bottomRight
		{
			get
			{
				return ST_Pane._bottomRight;
			}
			private set
			{
				ST_Pane._bottomRight = value;
			}
		}

		public static ST_Pane topRight
		{
			get
			{
				return ST_Pane._topRight;
			}
			private set
			{
				ST_Pane._topRight = value;
			}
		}

		public static ST_Pane bottomLeft
		{
			get
			{
				return ST_Pane._bottomLeft;
			}
			private set
			{
				ST_Pane._bottomLeft = value;
			}
		}

		public static ST_Pane topLeft
		{
			get
			{
				return ST_Pane._topLeft;
			}
			private set
			{
				ST_Pane._topLeft = value;
			}
		}

		static ST_Pane()
		{
			ST_Pane.bottomRight = new ST_Pane("bottomRight");
			ST_Pane.topRight = new ST_Pane("topRight");
			ST_Pane.bottomLeft = new ST_Pane("bottomLeft");
			ST_Pane.topLeft = new ST_Pane("topLeft");
		}

		private ST_Pane(string val)
		{
			this._ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return this._ooxmlEnumerationValue;
		}

		public bool Equals(ST_Pane other)
		{
			if (other == (ST_Pane)null)
			{
				return false;
			}
			return this._ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_Pane one, ST_Pane two)
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

		public static bool operator !=(ST_Pane one, ST_Pane two)
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
