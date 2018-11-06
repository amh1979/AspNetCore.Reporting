namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class ST_HdrFtr
	{
		private string _ooxmlEnumerationValue;

		private static ST_HdrFtr _even;

		private static ST_HdrFtr __default;

		private static ST_HdrFtr _first;

		public static ST_HdrFtr even
		{
			get
			{
				return ST_HdrFtr._even;
			}
			private set
			{
				ST_HdrFtr._even = value;
			}
		}

		public static ST_HdrFtr _default
		{
			get
			{
				return ST_HdrFtr.__default;
			}
			private set
			{
				ST_HdrFtr.__default = value;
			}
		}

		public static ST_HdrFtr first
		{
			get
			{
				return ST_HdrFtr._first;
			}
			private set
			{
				ST_HdrFtr._first = value;
			}
		}

		static ST_HdrFtr()
		{
			ST_HdrFtr.even = new ST_HdrFtr("even");
			ST_HdrFtr._default = new ST_HdrFtr("default");
			ST_HdrFtr.first = new ST_HdrFtr("first");
		}

		private ST_HdrFtr(string val)
		{
			this._ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return this._ooxmlEnumerationValue;
		}
	}
}
