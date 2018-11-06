namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class BackgroundImage : IImage
	{
		private InternalImage m_internalImage;

		public byte[] ImageData
		{
			get
			{
				return this.m_internalImage.ImageData;
			}
		}

		public string MIMEType
		{
			get
			{
				return this.m_internalImage.MIMEType;
			}
		}

		public string StreamName
		{
			get
			{
				return this.m_internalImage.StreamName;
			}
		}

		internal BackgroundImage(RenderingContext context, Image.SourceType imageSource, object imageValue, string mimeType)
		{
			this.m_internalImage = new InternalImage(imageSource, mimeType, imageValue, context);
		}
	}
}
