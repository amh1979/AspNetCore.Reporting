namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class ST_MultiLevelType
	{
		private string _ooxmlEnumerationValue;

		private static ST_MultiLevelType _singleLevel;

		private static ST_MultiLevelType _multilevel;

		private static ST_MultiLevelType _hybridMultilevel;

		public static ST_MultiLevelType singleLevel
		{
			get
			{
				return ST_MultiLevelType._singleLevel;
			}
			private set
			{
				ST_MultiLevelType._singleLevel = value;
			}
		}

		public static ST_MultiLevelType multilevel
		{
			get
			{
				return ST_MultiLevelType._multilevel;
			}
			private set
			{
				ST_MultiLevelType._multilevel = value;
			}
		}

		public static ST_MultiLevelType hybridMultilevel
		{
			get
			{
				return ST_MultiLevelType._hybridMultilevel;
			}
			private set
			{
				ST_MultiLevelType._hybridMultilevel = value;
			}
		}

		static ST_MultiLevelType()
		{
			ST_MultiLevelType.singleLevel = new ST_MultiLevelType("singleLevel");
			ST_MultiLevelType.multilevel = new ST_MultiLevelType("multilevel");
			ST_MultiLevelType.hybridMultilevel = new ST_MultiLevelType("hybridMultilevel");
		}

		private ST_MultiLevelType(string val)
		{
			this._ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return this._ooxmlEnumerationValue;
		}
	}
}
