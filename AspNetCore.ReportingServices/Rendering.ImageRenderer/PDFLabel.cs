using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class PDFLabel
	{
		internal readonly string UniqueName;

		internal readonly string Label;

		internal List<PDFLabel> Children;

		internal PDFLabel Parent;

		internal PDFLabel(string uniqueName, string label)
		{
			this.UniqueName = uniqueName;
			this.Label = label;
		}
	}
}
