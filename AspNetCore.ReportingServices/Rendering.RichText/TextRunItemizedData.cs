using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.RichText
{
	internal class TextRunItemizedData
	{
		internal List<int> SplitIndexes;

		internal List<TexRunShapeData> GlyphData;

		internal TextRunItemizedData(List<int> splitIndexes, List<TexRunShapeData> textRunsShapeData)
		{
			this.SplitIndexes = splitIndexes;
			this.GlyphData = textRunsShapeData;
		}
	}
}
