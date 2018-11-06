namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_BorderStyle
	{
		private string _ooxmlEnumerationValue;

		private static ST_BorderStyle _none;

		private static ST_BorderStyle _thin;

		private static ST_BorderStyle _medium;

		private static ST_BorderStyle _dashed;

		private static ST_BorderStyle _dotted;

		private static ST_BorderStyle _thick;

		private static ST_BorderStyle __double;

		private static ST_BorderStyle _hair;

		private static ST_BorderStyle _mediumDashed;

		private static ST_BorderStyle _dashDot;

		private static ST_BorderStyle _mediumDashDot;

		private static ST_BorderStyle _dashDotDot;

		private static ST_BorderStyle _mediumDashDotDot;

		private static ST_BorderStyle _slantDashDot;

		public static ST_BorderStyle none
		{
			get
			{
				return ST_BorderStyle._none;
			}
			private set
			{
				ST_BorderStyle._none = value;
			}
		}

		public static ST_BorderStyle thin
		{
			get
			{
				return ST_BorderStyle._thin;
			}
			private set
			{
				ST_BorderStyle._thin = value;
			}
		}

		public static ST_BorderStyle medium
		{
			get
			{
				return ST_BorderStyle._medium;
			}
			private set
			{
				ST_BorderStyle._medium = value;
			}
		}

		public static ST_BorderStyle dashed
		{
			get
			{
				return ST_BorderStyle._dashed;
			}
			private set
			{
				ST_BorderStyle._dashed = value;
			}
		}

		public static ST_BorderStyle dotted
		{
			get
			{
				return ST_BorderStyle._dotted;
			}
			private set
			{
				ST_BorderStyle._dotted = value;
			}
		}

		public static ST_BorderStyle thick
		{
			get
			{
				return ST_BorderStyle._thick;
			}
			private set
			{
				ST_BorderStyle._thick = value;
			}
		}

		public static ST_BorderStyle _double
		{
			get
			{
				return ST_BorderStyle.__double;
			}
			private set
			{
				ST_BorderStyle.__double = value;
			}
		}

		public static ST_BorderStyle hair
		{
			get
			{
				return ST_BorderStyle._hair;
			}
			private set
			{
				ST_BorderStyle._hair = value;
			}
		}

		public static ST_BorderStyle mediumDashed
		{
			get
			{
				return ST_BorderStyle._mediumDashed;
			}
			private set
			{
				ST_BorderStyle._mediumDashed = value;
			}
		}

		public static ST_BorderStyle dashDot
		{
			get
			{
				return ST_BorderStyle._dashDot;
			}
			private set
			{
				ST_BorderStyle._dashDot = value;
			}
		}

		public static ST_BorderStyle mediumDashDot
		{
			get
			{
				return ST_BorderStyle._mediumDashDot;
			}
			private set
			{
				ST_BorderStyle._mediumDashDot = value;
			}
		}

		public static ST_BorderStyle dashDotDot
		{
			get
			{
				return ST_BorderStyle._dashDotDot;
			}
			private set
			{
				ST_BorderStyle._dashDotDot = value;
			}
		}

		public static ST_BorderStyle mediumDashDotDot
		{
			get
			{
				return ST_BorderStyle._mediumDashDotDot;
			}
			private set
			{
				ST_BorderStyle._mediumDashDotDot = value;
			}
		}

		public static ST_BorderStyle slantDashDot
		{
			get
			{
				return ST_BorderStyle._slantDashDot;
			}
			private set
			{
				ST_BorderStyle._slantDashDot = value;
			}
		}

		static ST_BorderStyle()
		{
			ST_BorderStyle.none = new ST_BorderStyle("none");
			ST_BorderStyle.thin = new ST_BorderStyle("thin");
			ST_BorderStyle.medium = new ST_BorderStyle("medium");
			ST_BorderStyle.dashed = new ST_BorderStyle("dashed");
			ST_BorderStyle.dotted = new ST_BorderStyle("dotted");
			ST_BorderStyle.thick = new ST_BorderStyle("thick");
			ST_BorderStyle._double = new ST_BorderStyle("double");
			ST_BorderStyle.hair = new ST_BorderStyle("hair");
			ST_BorderStyle.mediumDashed = new ST_BorderStyle("mediumDashed");
			ST_BorderStyle.dashDot = new ST_BorderStyle("dashDot");
			ST_BorderStyle.mediumDashDot = new ST_BorderStyle("mediumDashDot");
			ST_BorderStyle.dashDotDot = new ST_BorderStyle("dashDotDot");
			ST_BorderStyle.mediumDashDotDot = new ST_BorderStyle("mediumDashDotDot");
			ST_BorderStyle.slantDashDot = new ST_BorderStyle("slantDashDot");
		}

		private ST_BorderStyle(string val)
		{
			this._ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return this._ooxmlEnumerationValue;
		}

		public bool Equals(ST_BorderStyle other)
		{
			if (other == (ST_BorderStyle)null)
			{
				return false;
			}
			return this._ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_BorderStyle one, ST_BorderStyle two)
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

		public static bool operator !=(ST_BorderStyle one, ST_BorderStyle two)
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
