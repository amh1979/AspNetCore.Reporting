namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal class RPLItemProps : RPLElementProps
	{
		private string m_label;

		private string m_bookmark;

		private string m_tooltip;

		public string ToolTip
		{
			get
			{
				return this.m_tooltip;
			}
			set
			{
				this.m_tooltip = value;
			}
		}

		public string Bookmark
		{
			get
			{
				return this.m_bookmark;
			}
			set
			{
				this.m_bookmark = value;
			}
		}

		public string Label
		{
			get
			{
				return this.m_label;
			}
			set
			{
				this.m_label = value;
			}
		}

		internal RPLItemProps()
		{
		}
	}
}
