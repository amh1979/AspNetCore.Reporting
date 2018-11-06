using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal class GlobalStyle : Style
	{
		private readonly IStyleModel mModel;

		internal GlobalStyle(IStyleModel model)
			: base(model)
		{
			this.mModel = model;
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is GlobalStyle)
			{
				if (obj == this)
				{
					return true;
				}
				GlobalStyle globalStyle = (GlobalStyle)obj;
				return globalStyle.mModel.Equals(this.mModel);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.mModel.GetHashCode();
		}
	}
}
