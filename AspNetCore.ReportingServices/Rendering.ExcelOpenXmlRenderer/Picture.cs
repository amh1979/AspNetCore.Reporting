using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal sealed class Picture
	{
		private readonly IShapeModel mModel;

		public string Hyperlink
		{
			set
			{
				this.mModel.Hyperlink = value;
			}
		}

		internal Picture(IShapeModel model)
		{
			this.mModel = model;
		}

		public void UpdateColumnOffset(double sizeInPoints, bool start)
		{
			this.mModel.UpdateColumnOffset(sizeInPoints, start);
		}

		public void UpdateRowOffset(double sizeInPoints, bool start)
		{
			this.mModel.UpdateRowOffset(sizeInPoints, start);
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is Picture)
			{
				if (obj == this)
				{
					return true;
				}
				Picture picture = (Picture)obj;
				return picture.mModel.Equals(this.mModel);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.mModel.GetHashCode();
		}
	}
}
