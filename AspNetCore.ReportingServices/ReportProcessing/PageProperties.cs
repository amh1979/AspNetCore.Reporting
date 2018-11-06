using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal class PageProperties
	{
		protected double m_pageHeight = 279.4;

		protected double m_pageWidth = 215.89999999999998;

		protected double m_topMargin = 12.7;

		protected double m_bottomMargin = 12.7;

		protected double m_leftMargin = 12.7;

		protected double m_rightMargin = 12.7;

		public double PageHeight
		{
			get
			{
				return this.m_pageHeight;
			}
		}

		public double PageWidth
		{
			get
			{
				return this.m_pageWidth;
			}
		}

		public double TopMargin
		{
			get
			{
				return this.m_topMargin;
			}
		}

		public double BottomMargin
		{
			get
			{
				return this.m_bottomMargin;
			}
		}

		public double LeftMargin
		{
			get
			{
				return this.m_leftMargin;
			}
		}

		public double RightMargin
		{
			get
			{
				return this.m_rightMargin;
			}
		}

		public PageProperties(double pageHeight, double pageWidth, double topMargin, double bottomMargin, double leftMargin, double rightMargin)
		{
			this.m_pageHeight = pageHeight;
			this.m_pageWidth = pageWidth;
			this.m_topMargin = topMargin;
			this.m_bottomMargin = bottomMargin;
			this.m_leftMargin = leftMargin;
			this.m_rightMargin = rightMargin;
		}

		protected PageProperties()
		{
		}
	}
}
