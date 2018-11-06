namespace AspNetCore.ReportingServices.Rendering.RichText
{
	internal struct SCRIPT_LOGATTR
	{
		private byte m_value;

		internal bool IsWhiteSpace
		{
			get
			{
				return (this.m_value >> 1 & 1) > 0;
			}
		}

		internal bool IsSoftBreak
		{
			get
			{
				return (this.m_value & 1 & 1) > 0;
			}
		}
	}
}
