namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class ST_Jc
	{
		private string _ooxmlEnumerationValue;

		private static ST_Jc _left;

		private static ST_Jc _center;

		private static ST_Jc _right;

		private static ST_Jc _both;

		private static ST_Jc _mediumKashida;

		private static ST_Jc _distribute;

		private static ST_Jc _numTab;

		private static ST_Jc _highKashida;

		private static ST_Jc _lowKashida;

		private static ST_Jc _thaiDistribute;

		public static ST_Jc left
		{
			get
			{
				return ST_Jc._left;
			}
			private set
			{
				ST_Jc._left = value;
			}
		}

		public static ST_Jc center
		{
			get
			{
				return ST_Jc._center;
			}
			private set
			{
				ST_Jc._center = value;
			}
		}

		public static ST_Jc right
		{
			get
			{
				return ST_Jc._right;
			}
			private set
			{
				ST_Jc._right = value;
			}
		}

		public static ST_Jc both
		{
			get
			{
				return ST_Jc._both;
			}
			private set
			{
				ST_Jc._both = value;
			}
		}

		public static ST_Jc mediumKashida
		{
			get
			{
				return ST_Jc._mediumKashida;
			}
			private set
			{
				ST_Jc._mediumKashida = value;
			}
		}

		public static ST_Jc distribute
		{
			get
			{
				return ST_Jc._distribute;
			}
			private set
			{
				ST_Jc._distribute = value;
			}
		}

		public static ST_Jc numTab
		{
			get
			{
				return ST_Jc._numTab;
			}
			private set
			{
				ST_Jc._numTab = value;
			}
		}

		public static ST_Jc highKashida
		{
			get
			{
				return ST_Jc._highKashida;
			}
			private set
			{
				ST_Jc._highKashida = value;
			}
		}

		public static ST_Jc lowKashida
		{
			get
			{
				return ST_Jc._lowKashida;
			}
			private set
			{
				ST_Jc._lowKashida = value;
			}
		}

		public static ST_Jc thaiDistribute
		{
			get
			{
				return ST_Jc._thaiDistribute;
			}
			private set
			{
				ST_Jc._thaiDistribute = value;
			}
		}

		static ST_Jc()
		{
			ST_Jc.left = new ST_Jc("left");
			ST_Jc.center = new ST_Jc("center");
			ST_Jc.right = new ST_Jc("right");
			ST_Jc.both = new ST_Jc("both");
			ST_Jc.mediumKashida = new ST_Jc("mediumKashida");
			ST_Jc.distribute = new ST_Jc("distribute");
			ST_Jc.numTab = new ST_Jc("numTab");
			ST_Jc.highKashida = new ST_Jc("highKashida");
			ST_Jc.lowKashida = new ST_Jc("lowKashida");
			ST_Jc.thaiDistribute = new ST_Jc("thaiDistribute");
		}

		private ST_Jc(string val)
		{
			this._ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return this._ooxmlEnumerationValue;
		}
	}
}
