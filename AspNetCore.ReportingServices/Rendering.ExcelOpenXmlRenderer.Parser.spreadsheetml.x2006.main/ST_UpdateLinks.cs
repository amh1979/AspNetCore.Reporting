namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_UpdateLinks
	{
		private string _ooxmlEnumerationValue;

		private static ST_UpdateLinks _userSet;

		private static ST_UpdateLinks _never;

		private static ST_UpdateLinks _always;

		public static ST_UpdateLinks userSet
		{
			get
			{
				return ST_UpdateLinks._userSet;
			}
			private set
			{
				ST_UpdateLinks._userSet = value;
			}
		}

		public static ST_UpdateLinks never
		{
			get
			{
				return ST_UpdateLinks._never;
			}
			private set
			{
				ST_UpdateLinks._never = value;
			}
		}

		public static ST_UpdateLinks always
		{
			get
			{
				return ST_UpdateLinks._always;
			}
			private set
			{
				ST_UpdateLinks._always = value;
			}
		}

		static ST_UpdateLinks()
		{
			ST_UpdateLinks.userSet = new ST_UpdateLinks("userSet");
			ST_UpdateLinks.never = new ST_UpdateLinks("never");
			ST_UpdateLinks.always = new ST_UpdateLinks("always");
		}

		private ST_UpdateLinks(string val)
		{
			this._ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return this._ooxmlEnumerationValue;
		}

		public bool Equals(ST_UpdateLinks other)
		{
			if (other == (ST_UpdateLinks)null)
			{
				return false;
			}
			return this._ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_UpdateLinks one, ST_UpdateLinks two)
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

		public static bool operator !=(ST_UpdateLinks one, ST_UpdateLinks two)
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
