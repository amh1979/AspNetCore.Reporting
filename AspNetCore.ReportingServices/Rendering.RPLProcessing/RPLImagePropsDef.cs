namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLImagePropsDef : RPLItemPropsDef
	{
		private RPLFormat.Sizings m_sizing;

		public RPLFormat.Sizings Sizing
		{
			get
			{
				return this.m_sizing;
			}
			set
			{
				this.m_sizing = value;
			}
		}

		internal RPLImagePropsDef()
		{
		}
	}
}
