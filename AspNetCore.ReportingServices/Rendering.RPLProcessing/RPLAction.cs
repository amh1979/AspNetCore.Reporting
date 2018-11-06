namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLAction
	{
		private string m_label;

		private string m_hyperlink;

		private string m_bookmarkLink;

		private string m_drillthroughId;

		private string m_drillthroughUrl;

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

		public string Hyperlink
		{
			get
			{
				return this.m_hyperlink;
			}
			set
			{
				this.m_hyperlink = value;
			}
		}

		public string BookmarkLink
		{
			get
			{
				return this.m_bookmarkLink;
			}
			set
			{
				this.m_bookmarkLink = value;
			}
		}

		public string DrillthroughId
		{
			get
			{
				return this.m_drillthroughId;
			}
			set
			{
				this.m_drillthroughId = value;
			}
		}

		public string DrillthroughUrl
		{
			get
			{
				return this.m_drillthroughUrl;
			}
			set
			{
				this.m_drillthroughUrl = value;
			}
		}

		internal RPLAction()
		{
		}

		internal RPLAction(string label)
		{
			this.m_label = label;
		}
	}
}
