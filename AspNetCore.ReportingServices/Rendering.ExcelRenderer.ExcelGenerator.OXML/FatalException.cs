using System;

namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML
{
	internal class FatalException : Exception
	{
		public FatalException()
			: base(ExcelRenderRes.ArgumentInvalid)
		{
		}
	}
}
