namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class ST_TblWidth
	{
		private string _ooxmlEnumerationValue;

		private static ST_TblWidth _nil;

		private static ST_TblWidth _pct;

		private static ST_TblWidth _dxa;

		private static ST_TblWidth _auto;

		public static ST_TblWidth nil
		{
			get
			{
				return ST_TblWidth._nil;
			}
			private set
			{
				ST_TblWidth._nil = value;
			}
		}

		public static ST_TblWidth pct
		{
			get
			{
				return ST_TblWidth._pct;
			}
			private set
			{
				ST_TblWidth._pct = value;
			}
		}

		public static ST_TblWidth dxa
		{
			get
			{
				return ST_TblWidth._dxa;
			}
			private set
			{
				ST_TblWidth._dxa = value;
			}
		}

		public static ST_TblWidth auto
		{
			get
			{
				return ST_TblWidth._auto;
			}
			private set
			{
				ST_TblWidth._auto = value;
			}
		}

		static ST_TblWidth()
		{
			ST_TblWidth.nil = new ST_TblWidth("nil");
			ST_TblWidth.pct = new ST_TblWidth("pct");
			ST_TblWidth.dxa = new ST_TblWidth("dxa");
			ST_TblWidth.auto = new ST_TblWidth("auto");
		}

		private ST_TblWidth(string val)
		{
			this._ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return this._ooxmlEnumerationValue;
		}
	}
}
