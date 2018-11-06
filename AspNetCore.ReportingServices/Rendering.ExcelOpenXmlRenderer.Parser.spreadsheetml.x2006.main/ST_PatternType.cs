namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_PatternType
	{
		private string _ooxmlEnumerationValue;

		private static ST_PatternType _none;

		private static ST_PatternType _solid;

		private static ST_PatternType _mediumGray;

		private static ST_PatternType _darkGray;

		private static ST_PatternType _lightGray;

		private static ST_PatternType _darkHorizontal;

		private static ST_PatternType _darkVertical;

		private static ST_PatternType _darkDown;

		private static ST_PatternType _darkUp;

		private static ST_PatternType _darkGrid;

		private static ST_PatternType _darkTrellis;

		private static ST_PatternType _lightHorizontal;

		private static ST_PatternType _lightVertical;

		private static ST_PatternType _lightDown;

		private static ST_PatternType _lightUp;

		private static ST_PatternType _lightGrid;

		private static ST_PatternType _lightTrellis;

		private static ST_PatternType _gray125;

		private static ST_PatternType _gray0625;

		public static ST_PatternType none
		{
			get
			{
				return ST_PatternType._none;
			}
			private set
			{
				ST_PatternType._none = value;
			}
		}

		public static ST_PatternType solid
		{
			get
			{
				return ST_PatternType._solid;
			}
			private set
			{
				ST_PatternType._solid = value;
			}
		}

		public static ST_PatternType mediumGray
		{
			get
			{
				return ST_PatternType._mediumGray;
			}
			private set
			{
				ST_PatternType._mediumGray = value;
			}
		}

		public static ST_PatternType darkGray
		{
			get
			{
				return ST_PatternType._darkGray;
			}
			private set
			{
				ST_PatternType._darkGray = value;
			}
		}

		public static ST_PatternType lightGray
		{
			get
			{
				return ST_PatternType._lightGray;
			}
			private set
			{
				ST_PatternType._lightGray = value;
			}
		}

		public static ST_PatternType darkHorizontal
		{
			get
			{
				return ST_PatternType._darkHorizontal;
			}
			private set
			{
				ST_PatternType._darkHorizontal = value;
			}
		}

		public static ST_PatternType darkVertical
		{
			get
			{
				return ST_PatternType._darkVertical;
			}
			private set
			{
				ST_PatternType._darkVertical = value;
			}
		}

		public static ST_PatternType darkDown
		{
			get
			{
				return ST_PatternType._darkDown;
			}
			private set
			{
				ST_PatternType._darkDown = value;
			}
		}

		public static ST_PatternType darkUp
		{
			get
			{
				return ST_PatternType._darkUp;
			}
			private set
			{
				ST_PatternType._darkUp = value;
			}
		}

		public static ST_PatternType darkGrid
		{
			get
			{
				return ST_PatternType._darkGrid;
			}
			private set
			{
				ST_PatternType._darkGrid = value;
			}
		}

		public static ST_PatternType darkTrellis
		{
			get
			{
				return ST_PatternType._darkTrellis;
			}
			private set
			{
				ST_PatternType._darkTrellis = value;
			}
		}

		public static ST_PatternType lightHorizontal
		{
			get
			{
				return ST_PatternType._lightHorizontal;
			}
			private set
			{
				ST_PatternType._lightHorizontal = value;
			}
		}

		public static ST_PatternType lightVertical
		{
			get
			{
				return ST_PatternType._lightVertical;
			}
			private set
			{
				ST_PatternType._lightVertical = value;
			}
		}

		public static ST_PatternType lightDown
		{
			get
			{
				return ST_PatternType._lightDown;
			}
			private set
			{
				ST_PatternType._lightDown = value;
			}
		}

		public static ST_PatternType lightUp
		{
			get
			{
				return ST_PatternType._lightUp;
			}
			private set
			{
				ST_PatternType._lightUp = value;
			}
		}

		public static ST_PatternType lightGrid
		{
			get
			{
				return ST_PatternType._lightGrid;
			}
			private set
			{
				ST_PatternType._lightGrid = value;
			}
		}

		public static ST_PatternType lightTrellis
		{
			get
			{
				return ST_PatternType._lightTrellis;
			}
			private set
			{
				ST_PatternType._lightTrellis = value;
			}
		}

		public static ST_PatternType gray125
		{
			get
			{
				return ST_PatternType._gray125;
			}
			private set
			{
				ST_PatternType._gray125 = value;
			}
		}

		public static ST_PatternType gray0625
		{
			get
			{
				return ST_PatternType._gray0625;
			}
			private set
			{
				ST_PatternType._gray0625 = value;
			}
		}

		static ST_PatternType()
		{
			ST_PatternType.none = new ST_PatternType("none");
			ST_PatternType.solid = new ST_PatternType("solid");
			ST_PatternType.mediumGray = new ST_PatternType("mediumGray");
			ST_PatternType.darkGray = new ST_PatternType("darkGray");
			ST_PatternType.lightGray = new ST_PatternType("lightGray");
			ST_PatternType.darkHorizontal = new ST_PatternType("darkHorizontal");
			ST_PatternType.darkVertical = new ST_PatternType("darkVertical");
			ST_PatternType.darkDown = new ST_PatternType("darkDown");
			ST_PatternType.darkUp = new ST_PatternType("darkUp");
			ST_PatternType.darkGrid = new ST_PatternType("darkGrid");
			ST_PatternType.darkTrellis = new ST_PatternType("darkTrellis");
			ST_PatternType.lightHorizontal = new ST_PatternType("lightHorizontal");
			ST_PatternType.lightVertical = new ST_PatternType("lightVertical");
			ST_PatternType.lightDown = new ST_PatternType("lightDown");
			ST_PatternType.lightUp = new ST_PatternType("lightUp");
			ST_PatternType.lightGrid = new ST_PatternType("lightGrid");
			ST_PatternType.lightTrellis = new ST_PatternType("lightTrellis");
			ST_PatternType.gray125 = new ST_PatternType("gray125");
			ST_PatternType.gray0625 = new ST_PatternType("gray0625");
		}

		private ST_PatternType(string val)
		{
			this._ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return this._ooxmlEnumerationValue;
		}

		public bool Equals(ST_PatternType other)
		{
			if (other == (ST_PatternType)null)
			{
				return false;
			}
			return this._ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_PatternType one, ST_PatternType two)
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

		public static bool operator !=(ST_PatternType one, ST_PatternType two)
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
