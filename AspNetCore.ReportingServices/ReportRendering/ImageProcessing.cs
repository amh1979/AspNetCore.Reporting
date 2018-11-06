namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class ImageProcessing : ImageBase
	{
		internal byte[] m_imageData;

		internal string m_mimeType;

		internal Image.Sizings m_sizing;

		internal ImageProcessing DeepClone()
		{
			ImageProcessing imageProcessing = new ImageProcessing();
			if (this.m_imageData != null)
			{
				imageProcessing.m_imageData = new byte[this.m_imageData.Length];
				this.m_imageData.CopyTo(imageProcessing.m_imageData, 0);
			}
			if (this.m_mimeType != null)
			{
				imageProcessing.m_mimeType = string.Copy(this.m_mimeType);
			}
			imageProcessing.m_sizing = this.m_sizing;
			return imageProcessing;
		}
	}
}
