using System;

namespace AspNetCore.ReportingServices.ProgressivePackaging
{
	internal abstract class ImageMessageElement
	{
		public string ImageUrl
		{
			get;
			protected set;
		}

		public string ImageWidth
		{
			get;
			protected set;
		}

		public string ImageHeight
		{
			get;
			protected set;
		}

		public ImageMessageElement()
		{
		}

		public ImageMessageElement(string imageUrl, string imageWidth, string imageHeight)
		{
			this.ImageUrl = imageUrl;
			this.ImageWidth = imageWidth;
			this.ImageHeight = imageHeight;
		}

		public override bool Equals(object obj)
		{
			ImageMessageElement imageMessageElement = obj as ImageMessageElement;
			if (imageMessageElement == null)
			{
				return false;
			}
			if (string.Compare(this.ImageUrl, imageMessageElement.ImageUrl, StringComparison.Ordinal) == 0 && string.Compare(this.ImageWidth, imageMessageElement.ImageWidth, StringComparison.Ordinal) == 0)
			{
				return string.Compare(this.ImageHeight, imageMessageElement.ImageHeight, StringComparison.Ordinal) == 0;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ((this.ImageUrl != null) ? this.ImageUrl.GetHashCode() : 0) ^ ((this.ImageWidth != null) ? this.ImageWidth.GetHashCode() : 0) ^ ((this.ImageHeight != null) ? this.ImageHeight.GetHashCode() : 0);
		}
	}
}
