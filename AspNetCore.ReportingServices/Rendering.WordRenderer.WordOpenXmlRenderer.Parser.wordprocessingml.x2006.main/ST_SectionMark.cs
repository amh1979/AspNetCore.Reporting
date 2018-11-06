namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class ST_SectionMark
	{
		private string _ooxmlEnumerationValue;

		private static ST_SectionMark _nextPage;

		private static ST_SectionMark _nextColumn;

		private static ST_SectionMark _continuous;

		private static ST_SectionMark _evenPage;

		private static ST_SectionMark _oddPage;

		public static ST_SectionMark nextPage
		{
			get
			{
				return ST_SectionMark._nextPage;
			}
			private set
			{
				ST_SectionMark._nextPage = value;
			}
		}

		public static ST_SectionMark nextColumn
		{
			get
			{
				return ST_SectionMark._nextColumn;
			}
			private set
			{
				ST_SectionMark._nextColumn = value;
			}
		}

		public static ST_SectionMark continuous
		{
			get
			{
				return ST_SectionMark._continuous;
			}
			private set
			{
				ST_SectionMark._continuous = value;
			}
		}

		public static ST_SectionMark evenPage
		{
			get
			{
				return ST_SectionMark._evenPage;
			}
			private set
			{
				ST_SectionMark._evenPage = value;
			}
		}

		public static ST_SectionMark oddPage
		{
			get
			{
				return ST_SectionMark._oddPage;
			}
			private set
			{
				ST_SectionMark._oddPage = value;
			}
		}

		static ST_SectionMark()
		{
			ST_SectionMark.nextPage = new ST_SectionMark("nextPage");
			ST_SectionMark.nextColumn = new ST_SectionMark("nextColumn");
			ST_SectionMark.continuous = new ST_SectionMark("continuous");
			ST_SectionMark.evenPage = new ST_SectionMark("evenPage");
			ST_SectionMark.oddPage = new ST_SectionMark("oddPage");
		}

		private ST_SectionMark(string val)
		{
			this._ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return this._ooxmlEnumerationValue;
		}
	}
}
