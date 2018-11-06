namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
	internal sealed class MillimeterSize : ISize
	{
		private float m_sizeInMm;

		public MillimeterSize(float mSizeInMm)
		{
			this.m_sizeInMm = mSizeInMm;
		}

		public void Render(IOutputStream outputStream)
		{
			outputStream.Write(this.m_sizeInMm.ToString());
			outputStream.Write(HTMLElements.m_mm);
		}
	}
}
