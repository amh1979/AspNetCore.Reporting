namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal class BorderContext
	{
		private const int TopBit = 1;

		private const int LeftBit = 2;

		private const int BottomBit = 4;

		private const int RightBit = 8;

		internal static readonly BorderContext EmptyBorder = new BorderContext(0);

		internal static readonly BorderContext TopBorder = new BorderContext(1);

		internal static readonly BorderContext LeftBorder = new BorderContext(2);

		internal static readonly BorderContext RightBorder = new BorderContext(8);

		internal static readonly BorderContext BottomBorder = new BorderContext(4);

		internal static readonly BorderContext TopLeftBorder = new BorderContext(3);

		internal static readonly BorderContext TopRightBorder = new BorderContext(9);

		internal static readonly BorderContext BottomLeftBorder = new BorderContext(6);

		internal static readonly BorderContext BottomRightBorder = new BorderContext(12);

		private int m_borderContext;

		internal bool Top
		{
			get
			{
				return (this.m_borderContext & 1) > 0;
			}
			set
			{
				if (value)
				{
					this.m_borderContext |= 1;
				}
				else
				{
					this.m_borderContext &= -2;
				}
			}
		}

		internal bool Left
		{
			get
			{
				return (this.m_borderContext & 2) > 0;
			}
			set
			{
				if (value)
				{
					this.m_borderContext |= 2;
				}
				else
				{
					this.m_borderContext &= -3;
				}
			}
		}

		internal bool Bottom
		{
			get
			{
				return (this.m_borderContext & 4) > 0;
			}
			set
			{
				if (value)
				{
					this.m_borderContext |= 4;
				}
				else
				{
					this.m_borderContext &= -5;
				}
			}
		}

		internal bool Right
		{
			get
			{
				return (this.m_borderContext & 8) > 0;
			}
			set
			{
				if (value)
				{
					this.m_borderContext |= 8;
				}
				else
				{
					this.m_borderContext &= -9;
				}
			}
		}

		internal BorderContext()
		{
			this.m_borderContext = 0;
		}

		internal BorderContext(BorderContext borderContext)
		{
			this.m_borderContext = borderContext.m_borderContext;
		}

		internal BorderContext(int borderContext)
		{
			this.m_borderContext = borderContext;
		}

		internal void Reset()
		{
			this.m_borderContext = 0;
		}
	}
}
