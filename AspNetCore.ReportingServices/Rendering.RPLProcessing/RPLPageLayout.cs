namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLPageLayout
	{
		private string m_pageName;

		private float m_pageHeight;

		private float m_pageWidth;

		private float m_marginTop;

		private float m_marginBottom;

		private float m_marginLeft;

		private float m_marginRight;

		private RPLElementStyle m_pageStyle;

		public string PageName
		{
			get
			{
				return this.m_pageName;
			}
			set
			{
				this.m_pageName = value;
			}
		}

		public float PageWidth
		{
			get
			{
				return this.m_pageWidth;
			}
			set
			{
				this.m_pageWidth = value;
			}
		}

		public float PageHeight
		{
			get
			{
				return this.m_pageHeight;
			}
			set
			{
				this.m_pageHeight = value;
			}
		}

		public float MarginTop
		{
			get
			{
				return this.m_marginTop;
			}
			set
			{
				this.m_marginTop = value;
			}
		}

		public float MarginBottom
		{
			get
			{
				return this.m_marginBottom;
			}
			set
			{
				this.m_marginBottom = value;
			}
		}

		public float MarginLeft
		{
			get
			{
				return this.m_marginLeft;
			}
			set
			{
				this.m_marginLeft = value;
			}
		}

		public float MarginRight
		{
			get
			{
				return this.m_marginRight;
			}
			set
			{
				this.m_marginRight = value;
			}
		}

		public RPLElementStyle Style
		{
			get
			{
				return this.m_pageStyle;
			}
			set
			{
				this.m_pageStyle = value;
			}
		}

		internal RPLPageLayout()
		{
		}
	}
}
