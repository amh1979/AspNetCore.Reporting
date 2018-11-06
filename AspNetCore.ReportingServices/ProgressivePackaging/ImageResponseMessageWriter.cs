using System.IO;

namespace AspNetCore.ReportingServices.ProgressivePackaging
{
	internal class ImageResponseMessageWriter
	{
		private IMessageWriter m_writer;

		public ImageResponseMessageWriter(IMessageWriter writer)
		{
			this.m_writer = writer;
		}

		public void WriteElement(ImageResponseMessageElement messageElement)
		{
			using (Stream output = this.m_writer.CreateWritableStream("getExternalImagesResponse"))
			{
				BinaryWriter binaryWriter = new BinaryWriter(output, MessageUtil.StringEncoding);
				messageElement.Write(binaryWriter);
				binaryWriter.Flush();
			}
		}
	}
}
