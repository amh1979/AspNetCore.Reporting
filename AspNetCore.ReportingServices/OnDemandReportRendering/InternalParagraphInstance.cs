using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalParagraphInstance : ParagraphInstance
	{
		private ReportSize m_leftIndent;

		private ReportSize m_rightIndent;

		private ReportSize m_hangingIndent;

		private ReportSize m_spaceBefore;

		private ReportSize m_spaceAfter;

		private ListStyle? m_listStyle = null;

		private int? m_listLevel = null;

		public override string UniqueName
		{
			get
			{
				if (base.m_uniqueName == null)
				{
					base.m_uniqueName = InstancePathItem.GenerateUniqueNameString(this.ParagraphDef.IDString, this.ParagraphDef.InstancePath);
				}
				return base.m_uniqueName;
			}
		}

		public override ReportSize LeftIndent
		{
			get
			{
				if (this.m_leftIndent == null)
				{
					this.m_leftIndent = this.GetLeftIndent(true);
				}
				return this.m_leftIndent;
			}
		}

		public override ReportSize RightIndent
		{
			get
			{
				if (this.m_rightIndent == null)
				{
					this.m_rightIndent = this.GetRightIndent(true);
				}
				return this.m_rightIndent;
			}
		}

		public override ReportSize HangingIndent
		{
			get
			{
				if (this.m_hangingIndent == null)
				{
					this.m_hangingIndent = this.GetHangingIndent(true);
				}
				return this.m_hangingIndent;
			}
		}

		public override ListStyle ListStyle
		{
			get
			{
				if (!this.m_listStyle.HasValue)
				{
					ExpressionInfo listStyle = this.ParagraphDef.ListStyle;
					if (listStyle != null)
					{
						if (listStyle.IsExpression)
						{
							this.m_listStyle = RichTextHelpers.TranslateListStyle(this.ParagraphDef.EvaluateListStyle(this.ReportScopeInstance, base.m_reportElementDef.RenderingContext.OdpContext));
						}
						else
						{
							this.m_listStyle = RichTextHelpers.TranslateListStyle(listStyle.StringValue);
						}
					}
					else
					{
						this.m_listStyle = ListStyle.None;
					}
				}
				return this.m_listStyle.Value;
			}
		}

		public override int ListLevel
		{
			get
			{
				if (!this.m_listLevel.HasValue)
				{
					ExpressionInfo listLevel = this.ParagraphDef.ListLevel;
					if (listLevel != null)
					{
						if (listLevel.IsExpression)
						{
							this.m_listLevel = this.ParagraphDef.EvaluateListLevel(this.ReportScopeInstance, base.m_reportElementDef.RenderingContext.OdpContext);
						}
						else
						{
							this.m_listLevel = listLevel.IntValue;
						}
					}
					if (!this.m_listLevel.HasValue)
					{
						this.m_listLevel = ((this.ListStyle != 0) ? 1 : 0);
					}
				}
				return this.m_listLevel.Value;
			}
		}

		public override ReportSize SpaceBefore
		{
			get
			{
				if (this.m_spaceBefore == null)
				{
					this.m_spaceBefore = this.GetSpaceBefore(true);
				}
				return this.m_spaceBefore;
			}
		}

		public override ReportSize SpaceAfter
		{
			get
			{
				if (this.m_spaceAfter == null)
				{
					this.m_spaceAfter = this.GetSpaceAfter(true);
				}
				return this.m_spaceAfter;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.Paragraph ParagraphDef
		{
			get
			{
				return ((InternalParagraph)base.m_reportElementDef).ParagraphDef;
			}
		}

		public override bool IsCompiled
		{
			get
			{
				return false;
			}
		}

		internal InternalParagraphInstance(Paragraph paragraphDef)
			: base(paragraphDef)
		{
		}

		internal InternalParagraphInstance(ReportElement reportElementDef)
			: base(reportElementDef)
		{
		}

		internal ReportSize GetLeftIndent(bool constantUsable)
		{
			ExpressionInfo leftIndent = this.ParagraphDef.LeftIndent;
			if (leftIndent != null)
			{
				if (leftIndent.IsExpression)
				{
					return new ReportSize(this.ParagraphDef.EvaluateLeftIndent(this.ReportScopeInstance, base.m_reportElementDef.RenderingContext.OdpContext), false, false);
				}
				if (constantUsable)
				{
					return new ReportSize(leftIndent.StringValue, false, false);
				}
			}
			return null;
		}

		internal ReportSize GetRightIndent(bool constantUsable)
		{
			ExpressionInfo rightIndent = this.ParagraphDef.RightIndent;
			if (rightIndent != null)
			{
				if (rightIndent.IsExpression)
				{
					return new ReportSize(this.ParagraphDef.EvaluateRightIndent(this.ReportScopeInstance, base.m_reportElementDef.RenderingContext.OdpContext), false, false);
				}
				if (constantUsable)
				{
					return new ReportSize(rightIndent.StringValue, false, false);
				}
			}
			return null;
		}

		internal ReportSize GetHangingIndent(bool constantUsable)
		{
			ExpressionInfo hangingIndent = this.ParagraphDef.HangingIndent;
			if (hangingIndent != null)
			{
				if (hangingIndent.IsExpression)
				{
					return new ReportSize(this.ParagraphDef.EvaluateHangingIndent(this.ReportScopeInstance, base.m_reportElementDef.RenderingContext.OdpContext), false, true);
				}
				if (constantUsable)
				{
					return new ReportSize(hangingIndent.StringValue, false, true);
				}
			}
			return null;
		}

		internal ReportSize GetSpaceBefore(bool constantUsable)
		{
			ExpressionInfo spaceBefore = this.ParagraphDef.SpaceBefore;
			if (spaceBefore != null)
			{
				if (spaceBefore.IsExpression)
				{
					return new ReportSize(this.ParagraphDef.EvaluateSpaceBefore(this.ReportScopeInstance, base.m_reportElementDef.RenderingContext.OdpContext), false, false);
				}
				if (constantUsable)
				{
					return new ReportSize(spaceBefore.StringValue, false, false);
				}
			}
			return null;
		}

		internal ReportSize GetSpaceAfter(bool constantUsable)
		{
			ExpressionInfo spaceAfter = this.ParagraphDef.SpaceAfter;
			if (spaceAfter != null)
			{
				if (spaceAfter.IsExpression)
				{
					return new ReportSize(this.ParagraphDef.EvaluateSpaceAfter(this.ReportScopeInstance, base.m_reportElementDef.RenderingContext.OdpContext), false, false);
				}
				if (constantUsable)
				{
					return new ReportSize(spaceAfter.StringValue, false, false);
				}
			}
			return null;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_leftIndent = null;
			this.m_rightIndent = null;
			this.m_hangingIndent = null;
			this.m_spaceBefore = null;
			this.m_spaceAfter = null;
			this.m_listStyle = null;
			this.m_listLevel = null;
		}
	}
}
