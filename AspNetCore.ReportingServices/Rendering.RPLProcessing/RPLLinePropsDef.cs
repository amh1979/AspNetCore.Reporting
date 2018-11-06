namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLLinePropsDef : RPLItemPropsDef
	{
		private bool m_slant;

		public bool Slant
		{
			get
			{
				return this.m_slant;
			}
			set
			{
				this.m_slant = value;
			}
		}

		internal RPLLinePropsDef()
		{
		}
	}
}
