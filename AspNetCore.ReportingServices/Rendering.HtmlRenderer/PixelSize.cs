namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
	internal sealed class PixelSize : ISize
	{
		private float m_sizeInPx;

		public PixelSize(float mSizeInPx)
		{
			this.m_sizeInPx = mSizeInPx;
		}

		public void Render(IOutputStream outputStream)
		{
			outputStream.Write(this.m_sizeInPx.ToString());
			outputStream.Write(HTMLElements.m_px);
		}
	}
}
