namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLParagraphProps : RPLElementProps
	{
		private RPLFormat.ListStyles? m_listStyle = null;

		private int? m_listLevel = null;

		private int m_paragraphNumber;

		private RPLReportSize m_leftIndent;

		private RPLReportSize m_rightIndent;

		private RPLReportSize m_hangingIndent;

		private RPLReportSize m_spaceBefore;

		private RPLReportSize m_spaceAfter;

		private bool m_firstLine = true;

		public RPLReportSize LeftIndent
		{
			get
			{
				return this.m_leftIndent;
			}
			set
			{
				this.m_leftIndent = value;
			}
		}

		public RPLReportSize RightIndent
		{
			get
			{
				return this.m_rightIndent;
			}
			set
			{
				this.m_rightIndent = value;
			}
		}

		public RPLReportSize HangingIndent
		{
			get
			{
				return this.m_hangingIndent;
			}
			set
			{
				this.m_hangingIndent = value;
			}
		}

		public RPLFormat.ListStyles? ListStyle
		{
			get
			{
				return this.m_listStyle;
			}
			set
			{
				this.m_listStyle = value;
			}
		}

		public int? ListLevel
		{
			get
			{
				return this.m_listLevel;
			}
			set
			{
				this.m_listLevel = value;
			}
		}

		public int ParagraphNumber
		{
			get
			{
				return this.m_paragraphNumber;
			}
			set
			{
				this.m_paragraphNumber = value;
			}
		}

		public RPLReportSize SpaceBefore
		{
			get
			{
				return this.m_spaceBefore;
			}
			set
			{
				this.m_spaceBefore = value;
			}
		}

		public RPLReportSize SpaceAfter
		{
			get
			{
				return this.m_spaceAfter;
			}
			set
			{
				this.m_spaceAfter = value;
			}
		}

		public bool FirstLine
		{
			get
			{
				return this.m_firstLine;
			}
			set
			{
				this.m_firstLine = value;
			}
		}

		internal RPLParagraphProps()
		{
		}
	}
}
