namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal class RPLSizes
	{
		protected float m_left;

		protected float m_top;

		protected float m_width;

		protected float m_height;

		public float Left
		{
			get
			{
				return this.m_left;
			}
			set
			{
				this.m_left = value;
			}
		}

		public float Top
		{
			get
			{
				return this.m_top;
			}
			set
			{
				this.m_top = value;
			}
		}

		public float Width
		{
			get
			{
				return this.m_width;
			}
			set
			{
				this.m_width = value;
			}
		}

		public float Height
		{
			get
			{
				return this.m_height;
			}
			set
			{
				this.m_height = value;
			}
		}

		public RPLSizes()
		{
		}

		public RPLSizes(float top, float left, float height, float width)
		{
			this.m_top = top;
			this.m_left = left;
			this.m_height = height;
			this.m_width = width;
		}

		public RPLSizes(RPLSizes sizes)
		{
			this.m_top = sizes.Top;
			this.m_left = sizes.Left;
			this.m_height = sizes.Height;
			this.m_width = sizes.Width;
		}
	}
}
