namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal class RoundedDouble
	{
		internal double m_value;

		internal bool m_forOverlapDetection;

		internal double Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				this.m_value = value;
			}
		}

		public RoundedDouble(double x)
			: this(x, false)
		{
		}

		public RoundedDouble(double x, bool forOverlapDetection)
		{
			this.m_value = x;
			this.m_forOverlapDetection = forOverlapDetection;
		}

		internal static double GetRoundingDelta(RoundedDouble x)
		{
			if (!x.m_forOverlapDetection)
			{
				return 0.01;
			}
			return 0.0001;
		}

		public static bool operator ==(RoundedDouble x1, double x2)
		{
			if (x2 - RoundedDouble.GetRoundingDelta(x1) <= x1.m_value)
			{
				return x2 + RoundedDouble.GetRoundingDelta(x1) >= x1.m_value;
			}
			return false;
		}

		public static bool operator >(RoundedDouble x1, double x2)
		{
			if (!(x1 == x2))
			{
				return x1.m_value - RoundedDouble.GetRoundingDelta(x1) > x2;
			}
			return false;
		}

		public static bool operator >=(RoundedDouble x1, double x2)
		{
			if (!(x1 == x2))
			{
				return x1.m_value - RoundedDouble.GetRoundingDelta(x1) >= x2;
			}
			return true;
		}

		public static bool operator <(RoundedDouble x1, double x2)
		{
			if (!(x1 == x2))
			{
				return x1.m_value + RoundedDouble.GetRoundingDelta(x1) < x2;
			}
			return false;
		}

		public static bool operator <=(RoundedDouble x1, double x2)
		{
			if (!(x1 == x2))
			{
				return x1.m_value + RoundedDouble.GetRoundingDelta(x1) <= x2;
			}
			return true;
		}

		public static bool operator !=(RoundedDouble x1, double x2)
		{
			return !(x1 == x2);
		}

		public static RoundedDouble operator +(RoundedDouble x1, double x2)
		{
			x1.m_value += x2;
			return x1;
		}

		public static RoundedDouble operator -(RoundedDouble x1, double x2)
		{
			x1.m_value -= x2;
			return x1;
		}

		public static explicit operator RoundedDouble(double x)
		{
			return new RoundedDouble(x, false);
		}

		public override bool Equals(object x1)
		{
			return (object)this.m_value == x1;
		}

		public override int GetHashCode()
		{
			return this.m_value.GetHashCode();
		}
	}
}
