using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model
{
	internal interface IBorderModel
	{
		bool HasBeenModified
		{
			get;
		}

		XMLBorderPartModel TopBorder
		{
			get;
		}

		XMLBorderPartModel BottomBorder
		{
			get;
		}

		XMLBorderPartModel LeftBorder
		{
			get;
		}

		XMLBorderPartModel RightBorder
		{
			get;
		}

		XMLBorderPartModel DiagonalBorder
		{
			get;
		}

		ExcelBorderPart DiagonalPartDirection
		{
			set;
		}
	}
}
