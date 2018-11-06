using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalParagraph : Paragraph
	{
		private ReportSizeProperty m_leftIndent;

		private ReportSizeProperty m_rightIndent;

		private ReportSizeProperty m_hangingIndent;

		private ReportSizeProperty m_spaceBefore;

		private ReportSizeProperty m_spaceAfter;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Paragraph m_paragraphDef;

		internal override IStyleContainer StyleContainer
		{
			get
			{
				return this.m_paragraphDef;
			}
		}

		public override string ID
		{
			get
			{
				return this.m_paragraphDef.RenderingModelID;
			}
		}

		public override ReportSizeProperty LeftIndent
		{
			get
			{
				if (this.m_leftIndent == null && this.m_paragraphDef.LeftIndent != null)
				{
					this.m_leftIndent = new ReportSizeProperty(this.m_paragraphDef.LeftIndent);
				}
				return this.m_leftIndent;
			}
		}

		public override ReportSizeProperty RightIndent
		{
			get
			{
				if (this.m_rightIndent == null && this.m_paragraphDef.RightIndent != null)
				{
					this.m_rightIndent = new ReportSizeProperty(this.m_paragraphDef.RightIndent);
				}
				return this.m_rightIndent;
			}
		}

		public override ReportSizeProperty HangingIndent
		{
			get
			{
				if (this.m_hangingIndent == null && this.m_paragraphDef.HangingIndent != null)
				{
					this.m_hangingIndent = new ReportSizeProperty(this.m_paragraphDef.HangingIndent, true);
				}
				return this.m_hangingIndent;
			}
		}

		public override ReportEnumProperty<ListStyle> ListStyle
		{
			get
			{
				if (base.m_listStyle == null)
				{
					ExpressionInfo listStyle = this.m_paragraphDef.ListStyle;
					if (listStyle != null)
					{
						ListStyle value = AspNetCore.ReportingServices.OnDemandReportRendering.ListStyle.None;
						if (!listStyle.IsExpression)
						{
							value = RichTextHelpers.TranslateListStyle(listStyle.StringValue);
						}
						base.m_listStyle = new ReportEnumProperty<ListStyle>(listStyle.IsExpression, listStyle.OriginalText, value);
					}
					else
					{
						base.m_listStyle = new ReportEnumProperty<ListStyle>(AspNetCore.ReportingServices.OnDemandReportRendering.ListStyle.None);
					}
				}
				return base.m_listStyle;
			}
		}

		public override ReportIntProperty ListLevel
		{
			get
			{
				if (base.m_listLevel == null)
				{
					if (this.m_paragraphDef.ListLevel != null)
					{
						base.m_listLevel = new ReportIntProperty(this.m_paragraphDef.ListLevel);
					}
					else
					{
						base.m_listLevel = new ReportIntProperty((this.Instance.ListStyle != 0) ? 1 : 0);
					}
				}
				return base.m_listLevel;
			}
		}

		public override ReportSizeProperty SpaceBefore
		{
			get
			{
				if (this.m_spaceBefore == null && this.m_paragraphDef.SpaceBefore != null)
				{
					this.m_spaceBefore = new ReportSizeProperty(this.m_paragraphDef.SpaceBefore);
				}
				return this.m_spaceBefore;
			}
		}

		public override ReportSizeProperty SpaceAfter
		{
			get
			{
				if (this.m_spaceAfter == null && this.m_paragraphDef.SpaceAfter != null)
				{
					this.m_spaceAfter = new ReportSizeProperty(this.m_paragraphDef.SpaceAfter);
				}
				return this.m_spaceAfter;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.Paragraph ParagraphDef
		{
			get
			{
				return this.m_paragraphDef;
			}
		}

		public override ParagraphInstance Instance
		{
			get
			{
				if (base.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (base.m_instance == null)
				{
					base.m_instance = new InternalParagraphInstance(this);
				}
				return base.m_instance;
			}
		}

		internal InternalParagraph(TextBox textBox, int indexIntoParentCollectionDef, AspNetCore.ReportingServices.ReportIntermediateFormat.Paragraph paragraph, RenderingContext renderingContext)
			: base(textBox, indexIntoParentCollectionDef, renderingContext)
		{
			this.m_paragraphDef = paragraph;
		}
	}
}
