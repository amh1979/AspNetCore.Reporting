namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.officeDocument.x2006.sharedTypes
{
	internal class ST_VerticalAlignRun
	{
		private string _ooxmlEnumerationValue;

		private static ST_VerticalAlignRun _baseline;

		private static ST_VerticalAlignRun _superscript;

		private static ST_VerticalAlignRun _subscript;

		public static ST_VerticalAlignRun baseline
		{
			get
			{
				return ST_VerticalAlignRun._baseline;
			}
			private set
			{
				ST_VerticalAlignRun._baseline = value;
			}
		}

		public static ST_VerticalAlignRun superscript
		{
			get
			{
				return ST_VerticalAlignRun._superscript;
			}
			private set
			{
				ST_VerticalAlignRun._superscript = value;
			}
		}

		public static ST_VerticalAlignRun subscript
		{
			get
			{
				return ST_VerticalAlignRun._subscript;
			}
			private set
			{
				ST_VerticalAlignRun._subscript = value;
			}
		}

		static ST_VerticalAlignRun()
		{
			ST_VerticalAlignRun.baseline = new ST_VerticalAlignRun("baseline");
			ST_VerticalAlignRun.superscript = new ST_VerticalAlignRun("superscript");
			ST_VerticalAlignRun.subscript = new ST_VerticalAlignRun("subscript");
		}

		private ST_VerticalAlignRun(string val)
		{
			this._ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return this._ooxmlEnumerationValue;
		}

		public bool Equals(ST_VerticalAlignRun other)
		{
			if (other == (ST_VerticalAlignRun)null)
			{
				return false;
			}
			return this._ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_VerticalAlignRun one, ST_VerticalAlignRun two)
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

		public static bool operator !=(ST_VerticalAlignRun one, ST_VerticalAlignRun two)
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
