namespace AspNetCore.ReportingServices.ReportRendering
{
	internal class MemberBase
	{
		private bool m_customControl;

		internal bool IsCustomControl
		{
			get
			{
				return this.m_customControl;
			}
		}

		internal MemberBase(bool isCustomControl)
		{
			this.m_customControl = isCustomControl;
		}
	}
}
