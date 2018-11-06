namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal class RPLActionInfo
	{
		private RPLAction[] m_actions;

		public RPLAction[] Actions
		{
			get
			{
				return this.m_actions;
			}
			set
			{
				this.m_actions = value;
			}
		}

		internal RPLActionInfo()
		{
		}

		internal RPLActionInfo(int count)
		{
			this.m_actions = new RPLAction[count];
		}
	}
}
