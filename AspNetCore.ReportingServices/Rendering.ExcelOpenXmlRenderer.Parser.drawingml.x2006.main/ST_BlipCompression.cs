namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.main
{
	internal class ST_BlipCompression
	{
		private string _ooxmlEnumerationValue;

		private static ST_BlipCompression _email;

		private static ST_BlipCompression _screen;

		private static ST_BlipCompression _print;

		private static ST_BlipCompression _hqprint;

		private static ST_BlipCompression _none;

		public static ST_BlipCompression email
		{
			get
			{
				return ST_BlipCompression._email;
			}
			private set
			{
				ST_BlipCompression._email = value;
			}
		}

		public static ST_BlipCompression screen
		{
			get
			{
				return ST_BlipCompression._screen;
			}
			private set
			{
				ST_BlipCompression._screen = value;
			}
		}

		public static ST_BlipCompression print
		{
			get
			{
				return ST_BlipCompression._print;
			}
			private set
			{
				ST_BlipCompression._print = value;
			}
		}

		public static ST_BlipCompression hqprint
		{
			get
			{
				return ST_BlipCompression._hqprint;
			}
			private set
			{
				ST_BlipCompression._hqprint = value;
			}
		}

		public static ST_BlipCompression none
		{
			get
			{
				return ST_BlipCompression._none;
			}
			private set
			{
				ST_BlipCompression._none = value;
			}
		}

		static ST_BlipCompression()
		{
			ST_BlipCompression.email = new ST_BlipCompression("email");
			ST_BlipCompression.screen = new ST_BlipCompression("screen");
			ST_BlipCompression.print = new ST_BlipCompression("print");
			ST_BlipCompression.hqprint = new ST_BlipCompression("hqprint");
			ST_BlipCompression.none = new ST_BlipCompression("none");
		}

		private ST_BlipCompression(string val)
		{
			this._ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return this._ooxmlEnumerationValue;
		}

		public bool Equals(ST_BlipCompression other)
		{
			if (other == (ST_BlipCompression)null)
			{
				return false;
			}
			return this._ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_BlipCompression one, ST_BlipCompression two)
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

		public static bool operator !=(ST_BlipCompression one, ST_BlipCompression two)
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
