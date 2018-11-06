namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
	internal class StyleContext
	{
		private bool m_inTablix;

		private bool m_styleOnCell;

		private bool m_renderMeasurements = true;

		private bool m_noBorders;

		private bool m_emptyTextBox;

		private bool m_onlyRenderMeasurementsBackgroundBorders;

		private byte m_omitBordersState;

		private bool m_ignoreVerticalAlign;

		private bool m_renderMinMeasurements;

		private bool m_ignorePadding;

		private bool m_rotationApplied;

		private bool m_zeroWidth;

		public bool EmptyTextBox
		{
			get
			{
				return this.m_emptyTextBox;
			}
			set
			{
				this.m_emptyTextBox = value;
			}
		}

		public bool NoBorders
		{
			get
			{
				return this.m_noBorders;
			}
			set
			{
				this.m_noBorders = value;
			}
		}

		public bool InTablix
		{
			get
			{
				return this.m_inTablix;
			}
			set
			{
				this.m_inTablix = value;
			}
		}

		public bool StyleOnCell
		{
			get
			{
				return this.m_styleOnCell;
			}
			set
			{
				this.m_styleOnCell = value;
			}
		}

		public bool RenderMeasurements
		{
			get
			{
				return this.m_renderMeasurements;
			}
			set
			{
				this.m_renderMeasurements = value;
			}
		}

		public bool RenderMinMeasurements
		{
			get
			{
				return this.m_renderMinMeasurements;
			}
			set
			{
				this.m_renderMinMeasurements = value;
			}
		}

		public bool OnlyRenderMeasurementsBackgroundBorders
		{
			get
			{
				return this.m_onlyRenderMeasurementsBackgroundBorders;
			}
			set
			{
				this.m_onlyRenderMeasurementsBackgroundBorders = value;
			}
		}

		public byte OmitBordersState
		{
			get
			{
				return this.m_omitBordersState;
			}
			set
			{
				this.m_omitBordersState = value;
			}
		}

		public bool IgnoreVerticalAlign
		{
			get
			{
				return this.m_ignoreVerticalAlign;
			}
			set
			{
				this.m_ignoreVerticalAlign = value;
			}
		}

		public bool IgnorePadding
		{
			get
			{
				return this.m_ignorePadding;
			}
			set
			{
				this.m_ignorePadding = value;
			}
		}

		public bool RotationApplied
		{
			get
			{
				return this.m_rotationApplied;
			}
			set
			{
				this.m_rotationApplied = value;
			}
		}

		public bool ZeroWidth
		{
			get
			{
				return this.m_zeroWidth;
			}
			set
			{
				this.m_zeroWidth = value;
			}
		}

		public void Reset()
		{
			this.m_inTablix = false;
			this.m_styleOnCell = false;
			this.m_renderMeasurements = true;
			this.m_noBorders = false;
			this.m_emptyTextBox = false;
			this.m_omitBordersState = 0;
			this.m_ignoreVerticalAlign = false;
			this.m_ignorePadding = false;
			this.m_rotationApplied = false;
			this.m_ignoreVerticalAlign = false;
			this.m_zeroWidth = false;
		}
	}
}
