namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_Visibility
	{
		private string _ooxmlEnumerationValue;

		private static ST_Visibility _visible;

		private static ST_Visibility _hidden;

		private static ST_Visibility _veryHidden;

		public static ST_Visibility visible
		{
			get
			{
				return ST_Visibility._visible;
			}
			private set
			{
				ST_Visibility._visible = value;
			}
		}

		public static ST_Visibility hidden
		{
			get
			{
				return ST_Visibility._hidden;
			}
			private set
			{
				ST_Visibility._hidden = value;
			}
		}

		public static ST_Visibility veryHidden
		{
			get
			{
				return ST_Visibility._veryHidden;
			}
			private set
			{
				ST_Visibility._veryHidden = value;
			}
		}

		static ST_Visibility()
		{
			ST_Visibility.visible = new ST_Visibility("visible");
			ST_Visibility.hidden = new ST_Visibility("hidden");
			ST_Visibility.veryHidden = new ST_Visibility("veryHidden");
		}

		private ST_Visibility(string val)
		{
			this._ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return this._ooxmlEnumerationValue;
		}

		public bool Equals(ST_Visibility other)
		{
			if (other == (ST_Visibility)null)
			{
				return false;
			}
			return this._ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_Visibility one, ST_Visibility two)
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

		public static bool operator !=(ST_Visibility one, ST_Visibility two)
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
