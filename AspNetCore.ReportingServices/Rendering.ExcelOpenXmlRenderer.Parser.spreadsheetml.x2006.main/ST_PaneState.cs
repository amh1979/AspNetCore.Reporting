namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_PaneState
	{
		private string _ooxmlEnumerationValue;

		private static ST_PaneState _split;

		private static ST_PaneState _frozen;

		private static ST_PaneState _frozenSplit;

		public static ST_PaneState split
		{
			get
			{
				return ST_PaneState._split;
			}
			private set
			{
				ST_PaneState._split = value;
			}
		}

		public static ST_PaneState frozen
		{
			get
			{
				return ST_PaneState._frozen;
			}
			private set
			{
				ST_PaneState._frozen = value;
			}
		}

		public static ST_PaneState frozenSplit
		{
			get
			{
				return ST_PaneState._frozenSplit;
			}
			private set
			{
				ST_PaneState._frozenSplit = value;
			}
		}

		static ST_PaneState()
		{
			ST_PaneState.split = new ST_PaneState("split");
			ST_PaneState.frozen = new ST_PaneState("frozen");
			ST_PaneState.frozenSplit = new ST_PaneState("frozenSplit");
		}

		private ST_PaneState(string val)
		{
			this._ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return this._ooxmlEnumerationValue;
		}

		public bool Equals(ST_PaneState other)
		{
			if (other == (ST_PaneState)null)
			{
				return false;
			}
			return this._ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_PaneState one, ST_PaneState two)
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

		public static bool operator !=(ST_PaneState one, ST_PaneState two)
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
