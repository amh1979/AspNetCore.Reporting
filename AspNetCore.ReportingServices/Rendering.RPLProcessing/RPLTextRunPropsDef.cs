namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLTextRunPropsDef : RPLElementPropsDef
	{
		private string m_value;

		private string m_label;

		private string m_toolTip;

		private RPLFormat.MarkupStyles m_markup;

		private string m_formula;

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

		public string Formula
		{
			get
			{
				return this.m_formula;
			}
			set
			{
				this.m_formula = value;
			}
		}

		internal RPLTextRunPropsDef()
		{
		}
	}
}
