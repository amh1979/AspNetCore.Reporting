namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
	internal class PaddingSharedInfo
	{
		private double m_padH;

		private double m_padV;

		private int m_paddingContext;

		internal double PadH
		{
			get
			{
				return this.m_padH;
			}
		}

		internal double PadV
		{
			get
			{
				return this.m_padV;
			}
		}

		internal int PaddingContext
		{
			get
			{
				return this.m_paddingContext;
			}
		}

		internal PaddingSharedInfo(int paddingContext, double padH, double padV)
		{
			this.m_padH = padH;
			this.m_padV = padV;
			this.m_paddingContext = paddingContext;
		}
	}
}
