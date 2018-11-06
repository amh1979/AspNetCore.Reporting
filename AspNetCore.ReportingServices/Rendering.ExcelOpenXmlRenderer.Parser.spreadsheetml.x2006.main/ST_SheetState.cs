namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_SheetState
	{
		private string _ooxmlEnumerationValue;

		private static ST_SheetState _visible;

		private static ST_SheetState _hidden;

		private static ST_SheetState _veryHidden;

		public static ST_SheetState visible
		{
			get
			{
				return ST_SheetState._visible;
			}
			private set
			{
				ST_SheetState._visible = value;
			}
		}

		public static ST_SheetState hidden
		{
			get
			{
				return ST_SheetState._hidden;
			}
			private set
			{
				ST_SheetState._hidden = value;
			}
		}

		public static ST_SheetState veryHidden
		{
			get
			{
				return ST_SheetState._veryHidden;
			}
			private set
			{
				ST_SheetState._veryHidden = value;
			}
		}

		static ST_SheetState()
		{
			ST_SheetState.visible = new ST_SheetState("visible");
			ST_SheetState.hidden = new ST_SheetState("hidden");
			ST_SheetState.veryHidden = new ST_SheetState("veryHidden");
		}

		private ST_SheetState(string val)
		{
			this._ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return this._ooxmlEnumerationValue;
		}

		public bool Equals(ST_SheetState other)
		{
			if (other == (ST_SheetState)null)
			{
				return false;
			}
			return this._ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_SheetState one, ST_SheetState two)
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

		public static bool operator !=(ST_SheetState one, ST_SheetState two)
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
