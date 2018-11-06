namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class ST_HeightRule
	{
		private string _ooxmlEnumerationValue;

		private static ST_HeightRule _auto;

		private static ST_HeightRule _exact;

		private static ST_HeightRule _atLeast;

		public static ST_HeightRule auto
		{
			get
			{
				return ST_HeightRule._auto;
			}
			private set
			{
				ST_HeightRule._auto = value;
			}
		}

		public static ST_HeightRule exact
		{
			get
			{
				return ST_HeightRule._exact;
			}
			private set
			{
				ST_HeightRule._exact = value;
			}
		}

		public static ST_HeightRule atLeast
		{
			get
			{
				return ST_HeightRule._atLeast;
			}
			private set
			{
				ST_HeightRule._atLeast = value;
			}
		}

		static ST_HeightRule()
		{
			ST_HeightRule.auto = new ST_HeightRule("auto");
			ST_HeightRule.exact = new ST_HeightRule("exact");
			ST_HeightRule.atLeast = new ST_HeightRule("atLeast");
		}

		private ST_HeightRule(string val)
		{
			this._ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return this._ooxmlEnumerationValue;
		}
	}
}
