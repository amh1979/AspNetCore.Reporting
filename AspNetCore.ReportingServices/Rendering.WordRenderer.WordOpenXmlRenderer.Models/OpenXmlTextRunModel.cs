using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal class OpenXmlTextRunModel : OpenXmlParagraphModel.IParagraphContent
	{
		private string _text;

		private bool _startsWithBreak;

		private OpenXmlRunPropertiesModel _properties;

		public OpenXmlTextRunModel(string text, bool startsWithBreak, OpenXmlRunPropertiesModel properties)
		{
			this._text = text;
			this._properties = properties;
			this._startsWithBreak = startsWithBreak;
		}

		public void Write(TextWriter writer)
		{
			writer.Write("<w:r>");
			this._properties.Write(writer);
			if (this._startsWithBreak)
			{
				writer.Write("<w:br/>");
			}
			if (!string.IsNullOrEmpty(this._text))
			{
				writer.Write("<w:t xml:space=\"preserve\">");
				writer.Write(this._text);
				writer.Write("</w:t>");
			}
			writer.Write("</w:r>");
		}
	}
}
