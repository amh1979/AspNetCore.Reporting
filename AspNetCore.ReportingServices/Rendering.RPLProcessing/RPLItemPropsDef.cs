namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal class RPLItemPropsDef : RPLElementPropsDef
	{
		private string m_name;

		private string m_tooltip;

		private string m_bookmark;

		private string m_label;

		private string m_toggleItem;

		public string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				this.m_name = value;
			}
		}

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

		public string ToggleItem
		{
			get
			{
				return this.m_toggleItem;
			}
			set
			{
				this.m_toggleItem = value;
			}
		}

		internal RPLItemPropsDef()
		{
		}
	}
}
