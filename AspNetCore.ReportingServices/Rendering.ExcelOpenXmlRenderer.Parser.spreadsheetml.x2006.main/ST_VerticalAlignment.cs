namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_VerticalAlignment
	{
		private string _ooxmlEnumerationValue;

		private static ST_VerticalAlignment _top;

		private static ST_VerticalAlignment _center;

		private static ST_VerticalAlignment _bottom;

		private static ST_VerticalAlignment _justify;

		private static ST_VerticalAlignment _distributed;

		public static ST_VerticalAlignment top
		{
			get
			{
				return ST_VerticalAlignment._top;
			}
			private set
			{
				ST_VerticalAlignment._top = value;
			}
		}

		public static ST_VerticalAlignment center
		{
			get
			{
				return ST_VerticalAlignment._center;
			}
			private set
			{
				ST_VerticalAlignment._center = value;
			}
		}

		public static ST_VerticalAlignment bottom
		{
			get
			{
				return ST_VerticalAlignment._bottom;
			}
			private set
			{
				ST_VerticalAlignment._bottom = value;
			}
		}

		public static ST_VerticalAlignment justify
		{
			get
			{
				return ST_VerticalAlignment._justify;
			}
			private set
			{
				ST_VerticalAlignment._justify = value;
			}
		}

		public static ST_VerticalAlignment distributed
		{
			get
			{
				return ST_VerticalAlignment._distributed;
			}
			private set
			{
				ST_VerticalAlignment._distributed = value;
			}
		}

		static ST_VerticalAlignment()
		{
			ST_VerticalAlignment.top = new ST_VerticalAlignment("top");
			ST_VerticalAlignment.center = new ST_VerticalAlignment("center");
			ST_VerticalAlignment.bottom = new ST_VerticalAlignment("bottom");
			ST_VerticalAlignment.justify = new ST_VerticalAlignment("justify");
			ST_VerticalAlignment.distributed = new ST_VerticalAlignment("distributed");
		}

		private ST_VerticalAlignment(string val)
		{
			this._ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return this._ooxmlEnumerationValue;
		}

		public bool Equals(ST_VerticalAlignment other)
		{
			if (other == (ST_VerticalAlignment)null)
			{
				return false;
			}
			return this._ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_VerticalAlignment one, ST_VerticalAlignment two)
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

		public static bool operator !=(ST_VerticalAlignment one, ST_VerticalAlignment two)
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
