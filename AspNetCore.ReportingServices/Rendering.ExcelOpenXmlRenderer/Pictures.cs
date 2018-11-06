using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal sealed class Pictures
	{
		private readonly IPictureShapesModel mModel;

		internal Pictures(IPictureShapesModel model)
		{
			this.mModel = model;
		}

		public Picture CreatePicture(string uniqueId, string extension, Stream pictureStream, Anchor startPosition, Anchor endPosition)
		{
			return this.mModel.CreatePicture(uniqueId, extension, pictureStream, startPosition.Model, endPosition.Model).Interface;
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is Pictures)
			{
				if (obj == this)
				{
					return true;
				}
				Pictures pictures = (Pictures)obj;
				return pictures.mModel.Equals(this.mModel);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.mModel.GetHashCode();
		}
	}
}
