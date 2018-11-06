namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class ST_Orientation
	{
		private string _ooxmlEnumerationValue;

		private static ST_Orientation __default;

		private static ST_Orientation _portrait;

		private static ST_Orientation _landscape;

		public static ST_Orientation _default
		{
			get
			{
				return ST_Orientation.__default;
			}
			private set
			{
				ST_Orientation.__default = value;
			}
		}

		public static ST_Orientation portrait
		{
			get
			{
				return ST_Orientation._portrait;
			}
			private set
			{
				ST_Orientation._portrait = value;
			}
		}

		public static ST_Orientation landscape
		{
			get
			{
				return ST_Orientation._landscape;
			}
			private set
			{
				ST_Orientation._landscape = value;
			}
		}

		static ST_Orientation()
		{
			ST_Orientation._default = new ST_Orientation("default");
			ST_Orientation.portrait = new ST_Orientation("portrait");
			ST_Orientation.landscape = new ST_Orientation("landscape");
		}

		private ST_Orientation(string val)
		{
			this._ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return this._ooxmlEnumerationValue;
		}

		public bool Equals(ST_Orientation other)
		{
			if (other == (ST_Orientation)null)
			{
				return false;
			}
			return this._ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_Orientation one, ST_Orientation two)
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

		public static bool operator !=(ST_Orientation one, ST_Orientation two)
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
