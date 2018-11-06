namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLTextRunProps : RPLElementProps
	{
		private RPLActionInfo m_actionInfo;

		private string m_value;

		private string m_toolTip;

		private RPLFormat.MarkupStyles m_markup;

		private bool m_processedWithError;

		public string Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				this.m_value = value;
			}
		}

		public string ToolTip
		{
			get
			{
				return this.m_toolTip;
			}
			set
			{
				this.m_toolTip = value;
			}
		}

		public RPLActionInfo ActionInfo
		{
			get
			{
				return this.m_actionInfo;
			}
			set
			{
				this.m_actionInfo = value;
			}
		}

		public RPLFormat.MarkupStyles Markup
		{
			get
			{
				return this.m_markup;
			}
			set
			{
				this.m_markup = value;
			}
		}

		public bool ProcessedWithError
		{
			get
			{
				return this.m_processedWithError;
			}
			set
			{
				this.m_processedWithError = value;
			}
		}

		internal RPLTextRunProps()
		{
		}
	}
}
