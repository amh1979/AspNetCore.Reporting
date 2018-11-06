namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class ST_LineSpacingRule
	{
		private string _ooxmlEnumerationValue;

		private static ST_LineSpacingRule _auto;

		private static ST_LineSpacingRule _exact;

		private static ST_LineSpacingRule _atLeast;

		public static ST_LineSpacingRule auto
		{
			get
			{
				return ST_LineSpacingRule._auto;
			}
			private set
			{
				ST_LineSpacingRule._auto = value;
			}
		}

		public static ST_LineSpacingRule exact
		{
			get
			{
				return ST_LineSpacingRule._exact;
			}
			private set
			{
				ST_LineSpacingRule._exact = value;
			}
		}

		public static ST_LineSpacingRule atLeast
		{
			get
			{
				return ST_LineSpacingRule._atLeast;
			}
			private set
			{
				ST_LineSpacingRule._atLeast = value;
			}
		}

		static ST_LineSpacingRule()
		{
			ST_LineSpacingRule.auto = new ST_LineSpacingRule("auto");
			ST_LineSpacingRule.exact = new ST_LineSpacingRule("exact");
			ST_LineSpacingRule.atLeast = new ST_LineSpacingRule("atLeast");
		}

		private ST_LineSpacingRule(string val)
		{
			this._ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return this._ooxmlEnumerationValue;
		}
	}
}
