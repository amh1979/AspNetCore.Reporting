using System.IO;

namespace AspNetCore.ReportingServices.ProgressivePackaging
{
	internal sealed class ImageResponseMessageElement : ImageMessageElement
	{
		public string ServerErrorCode
		{
			get;
			private set;
		}

		public byte[] ImageBytes
		{
			get;
			private set;
		}

		public ImageResponseMessageElement()
		{
		}

		public ImageResponseMessageElement(string imageUrl, string imageWidth, string imageHeight, byte[] imageBytes, string serverErrorCode)
			: base(imageUrl, imageWidth, imageHeight)
		{
			this.ServerErrorCode = serverErrorCode;
			this.ImageBytes = imageBytes;
		}

		public void Write(BinaryWriter writer)
		{
			this.WriteStringValue(base.ImageUrl, writer);
			this.WriteStringValue(base.ImageWidth, writer);
			this.WriteStringValue(base.ImageHeight, writer);
			this.WriteStringValue(this.ServerErrorCode, writer);
			if (this.ImageBytes != null)
			{
				writer.Write(this.ImageBytes.Length);
				writer.Write(this.ImageBytes);
			}
			else
			{
				writer.Write(0);
			}
		}

		private void WriteStringValue(string value, BinaryWriter writer)
		{
			if (value == null)
			{
				writer.Write(string.Empty);
			}
			else
			{
				writer.Write(value);
			}
		}

		public void Read(BinaryReader reader)
		{
			base.ImageUrl = reader.ReadString();
			base.ImageWidth = reader.ReadString();
			base.ImageHeight = reader.ReadString();
			this.ServerErrorCode = reader.ReadString();
			int count = reader.ReadInt32();
			this.ImageBytes = reader.ReadBytes(count);
		}
	}
}
