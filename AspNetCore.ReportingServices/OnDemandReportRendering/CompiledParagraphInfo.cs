namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class CompiledParagraphInfo
	{
		internal class FlattenedPropertyStore
		{
			private ListStyle m_listStyle;

			private int m_listLevel;

			private ReportSize m_spaceBefore;

			private ReportSize m_marginTop;

			private ReportSize m_pendingMarginBottom;

			private ICompiledParagraphInstance m_lastPopulatedParagraph;

			internal int ListLevel
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

			internal ListStyle ListStyle
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

			internal ReportSize SpaceBefore
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

			internal ReportSize MarginTop
			{
				get
				{
					return this.m_marginTop;
				}
			}

			internal ReportSize PendingMarginBottom
			{
				get
				{
					return this.m_pendingMarginBottom;
				}
			}

			internal ICompiledParagraphInstance LastPopulatedParagraph
			{
				get
				{
					return this.m_lastPopulatedParagraph;
				}
				set
				{
					this.m_lastPopulatedParagraph = value;
				}
			}

			internal void ClearMarginTop()
			{
				this.m_marginTop = null;
			}

			internal void UpdateMarginTop(ReportSize marginTop)
			{
				this.m_marginTop = this.GetLargest(this.m_marginTop, marginTop);
			}

			internal void AddMarginTop(ReportSize margin)
			{
				this.m_marginTop = ReportSize.SumSizes(this.m_marginTop, margin);
			}

			internal void ClearPendingMarginBottom()
			{
				this.m_pendingMarginBottom = null;
			}

			internal void UpdatePendingMarginBottom(ReportSize marginBottom)
			{
				this.m_pendingMarginBottom = this.GetLargest(this.m_pendingMarginBottom, marginBottom);
			}

			private ReportSize GetLargest(ReportSize size1, ReportSize size2)
			{
				if (size1 == null)
				{
					return size2;
				}
				if (size2 == null)
				{
					return size1;
				}
				if (size1.ToMillimeters() > size2.ToMillimeters())
				{
					return size1;
				}
				return size2;
			}
		}

		private HtmlElement.HtmlElementType m_elementType;

		private CompiledParagraphInfo m_parentParagraph;

		private CompiledParagraphInfo m_childParagraph;

		private ReportSize m_leftIndent;

		private ReportSize m_rightIndent;

		private ReportSize m_hangingIndent;

		private ReportSize m_spaceAfter;

		private ReportSize m_marginBottom;

		private bool m_hasSpaceBefore;

		private bool m_marginBottomSet;

		private bool m_leftIndentSet;

		private bool m_rightIndentSet;

		private bool m_hangingIndentSet;

		private bool m_spaceAfterSet;

		private FlattenedPropertyStore m_flatStore;

		private ICompiledParagraphInstance m_lastParagraph;

		internal HtmlElement.HtmlElementType ElementType
		{
			get
			{
				return this.m_elementType;
			}
			set
			{
				this.m_elementType = value;
			}
		}

		internal int ListLevel
		{
			get
			{
				return this.m_flatStore.ListLevel;
			}
			set
			{
				this.m_flatStore.ListLevel = value;
			}
		}

		internal ListStyle ListStyle
		{
			get
			{
				return this.m_flatStore.ListStyle;
			}
			set
			{
				this.m_flatStore.ListStyle = value;
			}
		}

		internal ReportSize LeftIndent
		{
			get
			{
				if (this.m_leftIndentSet)
				{
					return this.m_leftIndent;
				}
				if (this.m_parentParagraph != null)
				{
					return this.m_parentParagraph.LeftIndent;
				}
				return null;
			}
		}

		internal ReportSize RightIndent
		{
			get
			{
				if (this.m_rightIndentSet)
				{
					return this.m_rightIndent;
				}
				if (this.m_parentParagraph != null)
				{
					return this.m_parentParagraph.RightIndent;
				}
				return null;
			}
		}

		internal ReportSize HangingIndent
		{
			get
			{
				if (this.m_hangingIndentSet)
				{
					return this.m_hangingIndent;
				}
				if (this.m_parentParagraph != null)
				{
					return this.m_parentParagraph.HangingIndent;
				}
				return null;
			}
			set
			{
				this.m_hangingIndent = value;
				this.m_hangingIndentSet = true;
			}
		}

		internal ReportSize MarginTop
		{
			get
			{
				return this.m_flatStore.MarginTop;
			}
		}

		internal ReportSize MarginBottom
		{
			get
			{
				if (this.m_marginBottomSet)
				{
					return this.m_marginBottom;
				}
				return null;
			}
		}

		internal ReportSize SpaceBefore
		{
			get
			{
				return this.m_flatStore.SpaceBefore;
			}
		}

		internal ReportSize SpaceAfter
		{
			get
			{
				if (this.m_spaceAfterSet)
				{
					return this.m_spaceAfter;
				}
				return null;
			}
		}

		internal CompiledParagraphInfo()
		{
			this.m_flatStore = new FlattenedPropertyStore();
		}

		internal void AddLeftIndent(ReportSize size)
		{
			this.m_leftIndent = ReportSize.SumSizes(this.LeftIndent, size);
			this.m_leftIndentSet = true;
		}

		internal void AddRightIndent(ReportSize size)
		{
			this.m_rightIndent = ReportSize.SumSizes(this.RightIndent, size);
			this.m_rightIndentSet = true;
		}

		internal void UpdateMarginTop(ReportSize value)
		{
			this.m_flatStore.UpdateMarginTop(value);
		}

		internal void AddMarginBottom(ReportSize size)
		{
			this.m_marginBottom = size;
			this.m_marginBottomSet = true;
		}

		internal void AddSpaceBefore(ReportSize size)
		{
			ReportSize spaceBefore = this.m_flatStore.SpaceBefore;
			ReportSize reportSize = ReportSize.SumSizes(spaceBefore, size);
			this.m_hasSpaceBefore = (reportSize != null && reportSize.ToMillimeters() > 0.0 && (spaceBefore == null || reportSize.ToMillimeters() != spaceBefore.ToMillimeters()));
			this.m_flatStore.SpaceBefore = reportSize;
		}

		internal void AddSpaceAfter(ReportSize size)
		{
			if (this.m_spaceAfterSet)
			{
				this.m_spaceAfter = ReportSize.SumSizes(this.m_spaceAfter, size);
			}
			else
			{
				this.m_spaceAfter = size;
				this.m_spaceAfterSet = true;
			}
		}

		internal CompiledParagraphInfo CreateChildParagraph(HtmlElement.HtmlElementType elementType)
		{
			CompiledParagraphInfo compiledParagraphInfo = new CompiledParagraphInfo();
			compiledParagraphInfo.ElementType = elementType;
			compiledParagraphInfo.m_parentParagraph = this;
			compiledParagraphInfo.m_flatStore = this.m_flatStore;
			this.m_childParagraph = compiledParagraphInfo;
			return compiledParagraphInfo;
		}

		internal CompiledParagraphInfo RemoveAll()
		{
			CompiledParagraphInfo compiledParagraphInfo = this;
			while (compiledParagraphInfo.m_parentParagraph != null)
			{
				compiledParagraphInfo = compiledParagraphInfo.RemoveParagraph(compiledParagraphInfo.ElementType);
			}
			this.ApplyPendingMargins();
			compiledParagraphInfo.ResetParagraph();
			return compiledParagraphInfo;
		}

		internal CompiledParagraphInfo RemoveParagraph(HtmlElement.HtmlElementType elementType)
		{
			if (this.m_elementType == elementType)
			{
				this.ApplySpaceAfter();
				if (this.m_parentParagraph != null)
				{
					this.m_parentParagraph.m_childParagraph = null;
					return this.m_parentParagraph;
				}
				this.ResetParagraph();
			}
			else if (this.m_parentParagraph != null)
			{
				this.m_parentParagraph.InternalRemoveParagraph(elementType);
			}
			return this;
		}

		internal void InternalRemoveParagraph(HtmlElement.HtmlElementType elementType)
		{
			if (this.m_elementType == elementType)
			{
				this.ApplySpaceAfter();
				if (this.m_parentParagraph != null)
				{
					this.m_parentParagraph.m_childParagraph = this.m_childParagraph;
					this.m_childParagraph.m_parentParagraph = this.m_parentParagraph;
				}
				else if (this.m_parentParagraph == null)
				{
					this.m_childParagraph.m_parentParagraph = null;
				}
			}
			else if (this.m_parentParagraph != null)
			{
				this.m_parentParagraph.InternalRemoveParagraph(elementType);
			}
		}

		private void ApplySpaceAfter()
		{
			ReportSize spaceAfter = this.SpaceAfter;
			if (this.m_lastParagraph == null)
			{
				if (this.IsNonEmptySize(spaceAfter) || this.m_hasSpaceBefore)
				{
					this.ApplyPendingMargins();
					this.AddSpaceBefore(this.MarginTop);
					this.m_flatStore.ClearMarginTop();
					this.AddSpaceBefore(spaceAfter);
					this.m_flatStore.UpdatePendingMarginBottom(this.MarginBottom);
				}
				else
				{
					this.m_flatStore.UpdateMarginTop(this.MarginBottom);
				}
			}
			else
			{
				this.m_flatStore.UpdatePendingMarginBottom(this.MarginBottom);
				this.AddToParagraphSpaceAfter(this.m_lastParagraph, spaceAfter);
			}
		}

		private void ApplyPendingMargins()
		{
			ICompiledParagraphInstance lastPopulatedParagraph = this.m_flatStore.LastPopulatedParagraph;
			ReportSize pendingMarginBottom = this.m_flatStore.PendingMarginBottom;
			if (this.IsNonEmptySize(pendingMarginBottom))
			{
				if (lastPopulatedParagraph != null)
				{
					ReportSize marginTop = this.m_flatStore.MarginTop;
					if (marginTop == null)
					{
						this.AddToParagraphSpaceAfter(lastPopulatedParagraph, pendingMarginBottom);
					}
					else if (pendingMarginBottom.ToMillimeters() >= marginTop.ToMillimeters())
					{
						this.AddToParagraphSpaceAfter(lastPopulatedParagraph, pendingMarginBottom);
						this.m_flatStore.ClearMarginTop();
					}
				}
				else
				{
					this.m_flatStore.UpdateMarginTop(pendingMarginBottom);
				}
				this.m_flatStore.ClearPendingMarginBottom();
			}
			this.m_flatStore.LastPopulatedParagraph = null;
		}

		private void AddToParagraphSpaceAfter(ICompiledParagraphInstance paragraphInstance, ReportSize additionalSpace)
		{
			paragraphInstance.SpaceAfter = ReportSize.SumSizes(paragraphInstance.SpaceAfter, additionalSpace);
		}

		private bool IsNonEmptySize(ReportSize size)
		{
			if (size != null)
			{
				return size.ToMillimeters() > 0.0;
			}
			return false;
		}

		private void ResetParagraph()
		{
			this.m_leftIndentSet = false;
			this.m_rightIndentSet = false;
			this.m_hangingIndentSet = false;
			this.m_spaceAfterSet = false;
			this.m_marginBottomSet = false;
			this.m_hasSpaceBefore = false;
			this.m_lastParagraph = null;
		}

		internal void PopulateParagraph(ICompiledParagraphInstance paragraphInstance)
		{
			this.ApplyPendingMargins();
			this.m_flatStore.LastPopulatedParagraph = paragraphInstance;
			paragraphInstance.ListStyle = this.m_flatStore.ListStyle;
			this.m_flatStore.ListStyle = ListStyle.None;
			paragraphInstance.ListLevel = this.m_flatStore.ListLevel;
			ReportSize leftIndent = this.LeftIndent;
			if (leftIndent != null)
			{
				paragraphInstance.LeftIndent = leftIndent;
			}
			ReportSize rightIndent = this.RightIndent;
			if (rightIndent != null)
			{
				paragraphInstance.RightIndent = rightIndent;
			}
			ReportSize hangingIndent = this.HangingIndent;
			if (hangingIndent != null)
			{
				paragraphInstance.HangingIndent = hangingIndent;
			}
			ReportSize marginTop = this.MarginTop;
			ReportSize spaceBefore = this.SpaceBefore;
			if (spaceBefore != null || marginTop != null)
			{
				paragraphInstance.SpaceBefore = ReportSize.SumSizes(marginTop, spaceBefore);
				this.m_flatStore.SpaceBefore = null;
				this.m_flatStore.ClearMarginTop();
			}
			this.StoreLastParagraph(paragraphInstance);
		}

		private void StoreLastParagraph(ICompiledParagraphInstance paragraphInstance)
		{
			this.m_lastParagraph = paragraphInstance;
			if (this.m_parentParagraph != null)
			{
				this.m_parentParagraph.StoreLastParagraph(paragraphInstance);
			}
		}
	}
}
