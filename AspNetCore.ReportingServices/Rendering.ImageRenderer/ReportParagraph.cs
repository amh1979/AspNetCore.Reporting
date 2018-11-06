using AspNetCore.ReportingServices.Rendering.RichText;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;

namespace AspNetCore.ReportingServices.Rendering.ImageRenderer
{
	internal class ReportParagraph : IParagraphProps
	{
		private RPLParagraphProps m_source;

		private RPLElementStyle m_style;

		private string m_uniqueName;

		private int m_paragraphNumber;

		public RPLFormat.TextAlignments Alignment
		{
			get
			{
				RPLElementStyle style = this.m_style;
				if (style == null && this.m_source != null)
				{
					style = this.m_source.Style;
				}
				RPLFormat.TextAlignments result = RPLFormat.TextAlignments.General;
				if (style != null)
				{
					object obj = style[25];
					if (obj != null)
					{
						result = (RPLFormat.TextAlignments)obj;
					}
				}
				return result;
			}
		}

		public float SpaceBefore
		{
			get
			{
				if (this.m_source != null)
				{
					RPLReportSize spaceBefore = this.m_source.SpaceBefore;
					if (spaceBefore == null)
					{
						spaceBefore = ((RPLParagraphPropsDef)this.m_source.Definition).SpaceBefore;
					}
					if (spaceBefore != null)
					{
						return (float)spaceBefore.ToMillimeters();
					}
				}
				return 0f;
			}
		}

		public float SpaceAfter
		{
			get
			{
				if (this.m_source != null)
				{
					RPLReportSize spaceAfter = this.m_source.SpaceAfter;
					if (spaceAfter == null)
					{
						spaceAfter = ((RPLParagraphPropsDef)this.m_source.Definition).SpaceAfter;
					}
					if (spaceAfter != null)
					{
						return (float)spaceAfter.ToMillimeters();
					}
				}
				return 0f;
			}
		}

		public float LeftIndent
		{
			get
			{
				if (this.m_source != null)
				{
					RPLReportSize leftIndent = this.m_source.LeftIndent;
					if (leftIndent == null)
					{
						leftIndent = ((RPLParagraphPropsDef)this.m_source.Definition).LeftIndent;
					}
					if (leftIndent != null)
					{
						return (float)leftIndent.ToMillimeters();
					}
				}
				return 0f;
			}
		}

		public float RightIndent
		{
			get
			{
				if (this.m_source != null)
				{
					RPLReportSize rightIndent = this.m_source.RightIndent;
					if (rightIndent == null)
					{
						rightIndent = ((RPLParagraphPropsDef)this.m_source.Definition).RightIndent;
					}
					if (rightIndent != null)
					{
						return (float)rightIndent.ToMillimeters();
					}
				}
				return 0f;
			}
		}

		public float HangingIndent
		{
			get
			{
				if (this.m_source != null)
				{
					RPLReportSize hangingIndent = this.m_source.HangingIndent;
					if (hangingIndent == null)
					{
						hangingIndent = ((RPLParagraphPropsDef)this.m_source.Definition).HangingIndent;
					}
					if (hangingIndent != null)
					{
						return (float)hangingIndent.ToMillimeters();
					}
				}
				return 0f;
			}
		}

		public int ListLevel
		{
			get
			{
				int result = 0;
				if (this.m_source != null)
				{
					int? nullable = this.m_source.ListLevel;
					if (!nullable.HasValue)
					{
						nullable = ((RPLParagraphPropsDef)this.m_source.Definition).ListLevel;
					}
					if (nullable.HasValue)
					{
						result = nullable.Value;
					}
				}
				return result;
			}
		}

		public RPLFormat.ListStyles ListStyle
		{
			get
			{
				RPLFormat.ListStyles result = RPLFormat.ListStyles.None;
				if (this.m_source != null)
				{
					RPLFormat.ListStyles? nullable = this.m_source.ListStyle;
					if (!nullable.HasValue)
					{
						nullable = ((RPLParagraphPropsDef)this.m_source.Definition).ListStyle;
					}
					if (nullable.HasValue)
					{
						result = nullable.Value;
					}
				}
				return result;
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

		internal string UniqueName
		{
			get
			{
				return this.m_uniqueName;
			}
		}

		internal ReportParagraph(RPLParagraphProps source)
		{
			this.m_source = source;
			this.m_uniqueName = source.UniqueName;
			this.ParagraphNumber = source.ParagraphNumber;
		}

		internal ReportParagraph(RPLElementStyle style, string uniqueName)
		{
			this.m_style = style;
			this.m_uniqueName = uniqueName;
		}
	}
}
