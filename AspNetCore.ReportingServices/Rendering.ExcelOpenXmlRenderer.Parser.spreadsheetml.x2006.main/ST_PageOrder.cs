namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_PageOrder
	{
		private string _ooxmlEnumerationValue;

		private static ST_PageOrder _downThenOver;

		private static ST_PageOrder _overThenDown;

		public static ST_PageOrder downThenOver
		{
			get
			{
				return ST_PageOrder._downThenOver;
			}
			private set
			{
				ST_PageOrder._downThenOver = value;
			}
		}

		public static ST_PageOrder overThenDown
		{
			get
			{
				return ST_PageOrder._overThenDown;
			}
			private set
			{
				ST_PageOrder._overThenDown = value;
			}
		}

		static ST_PageOrder()
		{
			ST_PageOrder.downThenOver = new ST_PageOrder("downThenOver");
			ST_PageOrder.overThenDown = new ST_PageOrder("overThenDown");
		}

		private ST_PageOrder(string val)
		{
			this._ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return this._ooxmlEnumerationValue;
		}

		public bool Equals(ST_PageOrder other)
		{
			if (other == (ST_PageOrder)null)
			{
				return false;
			}
			return this._ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_PageOrder one, ST_PageOrder two)
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

		public static bool operator !=(ST_PageOrder one, ST_PageOrder two)
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
