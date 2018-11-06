namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLParagraphPropsDef : RPLElementPropsDef
	{
		private RPLFormat.ListStyles m_listStyle;

		private int m_listLevel;

		private RPLReportSize m_leftIndent;

		private RPLReportSize m_rightIndent;

		private RPLReportSize m_hangingIndent;

		private RPLReportSize m_spaceBefore;

		private RPLReportSize m_spaceAfter;

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

		public RPLFormat.ListStyles ListStyle
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

		public int ListLevel
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

		internal RPLParagraphPropsDef()
		{
		}
	}
}
