namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class ST_HorizontalAlignment
	{
		private string _ooxmlEnumerationValue;

		private static ST_HorizontalAlignment _general;

		private static ST_HorizontalAlignment _left;

		private static ST_HorizontalAlignment _center;

		private static ST_HorizontalAlignment _right;

		private static ST_HorizontalAlignment _fill;

		private static ST_HorizontalAlignment _justify;

		private static ST_HorizontalAlignment _centerContinuous;

		private static ST_HorizontalAlignment _distributed;

		public static ST_HorizontalAlignment general
		{
			get
			{
				return ST_HorizontalAlignment._general;
			}
			private set
			{
				ST_HorizontalAlignment._general = value;
			}
		}

		public static ST_HorizontalAlignment left
		{
			get
			{
				return ST_HorizontalAlignment._left;
			}
			private set
			{
				ST_HorizontalAlignment._left = value;
			}
		}

		public static ST_HorizontalAlignment center
		{
			get
			{
				return ST_HorizontalAlignment._center;
			}
			private set
			{
				ST_HorizontalAlignment._center = value;
			}
		}

		public static ST_HorizontalAlignment right
		{
			get
			{
				return ST_HorizontalAlignment._right;
			}
			private set
			{
				ST_HorizontalAlignment._right = value;
			}
		}

		public static ST_HorizontalAlignment fill
		{
			get
			{
				return ST_HorizontalAlignment._fill;
			}
			private set
			{
				ST_HorizontalAlignment._fill = value;
			}
		}

		public static ST_HorizontalAlignment justify
		{
			get
			{
				return ST_HorizontalAlignment._justify;
			}
			private set
			{
				ST_HorizontalAlignment._justify = value;
			}
		}

		public static ST_HorizontalAlignment centerContinuous
		{
			get
			{
				return ST_HorizontalAlignment._centerContinuous;
			}
			private set
			{
				ST_HorizontalAlignment._centerContinuous = value;
			}
		}

		public static ST_HorizontalAlignment distributed
		{
			get
			{
				return ST_HorizontalAlignment._distributed;
			}
			private set
			{
				ST_HorizontalAlignment._distributed = value;
			}
		}

		static ST_HorizontalAlignment()
		{
			ST_HorizontalAlignment.general = new ST_HorizontalAlignment("general");
			ST_HorizontalAlignment.left = new ST_HorizontalAlignment("left");
			ST_HorizontalAlignment.center = new ST_HorizontalAlignment("center");
			ST_HorizontalAlignment.right = new ST_HorizontalAlignment("right");
			ST_HorizontalAlignment.fill = new ST_HorizontalAlignment("fill");
			ST_HorizontalAlignment.justify = new ST_HorizontalAlignment("justify");
			ST_HorizontalAlignment.centerContinuous = new ST_HorizontalAlignment("centerContinuous");
			ST_HorizontalAlignment.distributed = new ST_HorizontalAlignment("distributed");
		}

		private ST_HorizontalAlignment(string val)
		{
			this._ooxmlEnumerationValue = val;
		}

		public override string ToString()
		{
			return this._ooxmlEnumerationValue;
		}

		public bool Equals(ST_HorizontalAlignment other)
		{
			if (other == (ST_HorizontalAlignment)null)
			{
				return false;
			}
			return this._ooxmlEnumerationValue == other._ooxmlEnumerationValue;
		}

		public static bool operator ==(ST_HorizontalAlignment one, ST_HorizontalAlignment two)
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

		public static bool operator !=(ST_HorizontalAlignment one, ST_HorizontalAlignment two)
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
