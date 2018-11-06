namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.Layout
{
	internal class PaddingInformation
	{
		private int m_paddingLeft;

		private int m_paddingRight;

		private int m_paddingTop;

		private int m_paddingBottom;

		internal int PaddingLeft
		{
			get
			{
				return this.m_paddingLeft;
			}
		}

		internal int PaddingRight
		{
			get
			{
				return this.m_paddingRight;
			}
		}

		internal int PaddingTop
		{
			get
			{
				return this.m_paddingTop;
			}
		}

		internal int PaddingBottom
		{
			get
			{
				return this.m_paddingBottom;
			}
		}

		internal PaddingInformation(int paddingLeft, int paddingRight, int paddingTop, int paddingBottom)
		{
			this.m_paddingLeft = paddingLeft;
			this.m_paddingRight = paddingRight;
			this.m_paddingTop = paddingTop;
			this.m_paddingBottom = paddingBottom;
		}
	}
}
