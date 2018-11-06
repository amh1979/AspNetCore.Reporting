using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class OpenXmlParagraphModel : OpenXmlTableCellModel.ICellContent
	{
		internal sealed class PageBreakParagraph : OpenXmlTableCellModel.ICellContent
		{
			public void Write(TextWriter writer)
			{
				writer.Write("<w:p><w:pPr><w:spacing w:after=\"0\" w:line=\"240\" w:lineRule=\"auto\"/><w:rPr><w:sz w:val=\"0\"/></w:rPr></w:pPr><w:r><w:br w:type=\"page\"/></w:r></w:p>");
			}
		}

		internal sealed class EmptyParagraph : OpenXmlTableCellModel.ICellContent
		{
			public void Write(TextWriter writer)
			{
				writer.Write("<w:p><w:pPr><w:spacing w:after=\"0\" w:line=\"240\" w:lineRule=\"auto\"/></w:pPr></w:p>");
			}
		}

		internal interface IParagraphContent
		{
			void Write(TextWriter writer);
		}

		private OpenXmlParagraphPropertiesModel _properties;

		private List<IParagraphContent> _contents;

		public OpenXmlParagraphPropertiesModel Properties
		{
			get
			{
				return this._properties;
			}
		}

		public OpenXmlParagraphModel()
		{
			this._properties = new OpenXmlParagraphPropertiesModel();
			this._contents = new List<IParagraphContent>();
		}

		private void AddRun(StringBuilder text, bool breakFirst, OpenXmlRunPropertiesModel style)
		{
			this._contents.Add(new OpenXmlTextRunModel(text.ToString(), breakFirst, style));
		}

		public void AddText(string text, OpenXmlRunPropertiesModel style)
		{
			if (!string.IsNullOrEmpty(text))
			{
				StringBuilder stringBuilder = new StringBuilder();
				bool flag = true;
				foreach (char c in text)
				{
					if (c != '\r')
					{
						if (c == '\n')
						{
							this.AddRun(stringBuilder, !flag, style);
							stringBuilder = new StringBuilder();
							flag = false;
						}
						else if (c < ' ')
						{
							stringBuilder.Append(' ');
						}
						else
						{
							stringBuilder.Append(WordOpenXmlUtils.EscapeChar(c));
						}
					}
				}
				this.AddRun(stringBuilder, !flag, style);
			}
		}

		public void AddPageNumberField(OpenXmlRunPropertiesModel textStyle)
		{
			this._contents.Add(OpenXmlFieldGenerators.PageNumberField(textStyle));
		}

		public void AddPageCountField(OpenXmlRunPropertiesModel textStyle)
		{
			this._contents.Add(OpenXmlFieldGenerators.PageCountField(textStyle));
		}

		public void AddLabel(string label, int level, OpenXmlRunPropertiesModel textStyle)
		{
			this._contents.Add(OpenXmlFieldGenerators.TableOfContentsEntry(label, level));
		}

		public void StartHyperlink(string target, bool bookmarkLink, OpenXmlRunPropertiesModel textStyle)
		{
			this._contents.Add(OpenXmlFieldGenerators.StartHyperlink(target, bookmarkLink));
		}

		public void EndHyperlink(OpenXmlRunPropertiesModel textStyle)
		{
			this._contents.Add(OpenXmlFieldGenerators.EndHyperlink());
		}

		public void AddBookmark(string name, int id)
		{
			this._contents.Add(new OpenXmlBookmarkModel(name, id));
		}

		public void AddImage(OpenXmlPictureModel picture)
		{
			this._contents.Add(picture);
		}

		public void Write(TextWriter writer)
		{
			writer.Write("<w:p>");
			this._properties.Write(writer);
			for (int i = 0; i < this._contents.Count; i++)
			{
				this._contents[i].Write(writer);
			}
			writer.Write("</w:p>");
		}

		void OpenXmlTableCellModel.ICellContent.Write(TextWriter writer)
		{
			this.Write(writer);
		}

		public static void WritePageBreakParagraph(TextWriter writer)
		{
			writer.Write("<w:p><w:pPr><w:spacing w:after=\"0\" w:line=\"240\" w:lineRule=\"auto\"/><w:rPr><w:sz w:val=\"0\"/></w:rPr></w:pPr><w:r><w:br w:type=\"page\"/></w:r></w:p>");
		}

		public static void WriteInvisibleParagraph(TextWriter writer)
		{
			writer.Write("<w:p><w:pPr><w:spacing w:after=\"0\" w:line=\"0\" w:lineRule=\"auto\"/><w:rPr><w:sz w:val=\"0\"/></w:rPr></w:pPr></w:p>");
		}

		public static void WriteEmptyParagraph(TextWriter writer)
		{
			writer.Write("<w:p><w:pPr><w:spacing w:after=\"0\" w:line=\"240\" w:lineRule=\"auto\"/></w:pPr></w:p>");
		}

		public static void WriteEmptyLayoutCellParagraph(TextWriter writer)
		{
			writer.Write("<w:p><w:pPr><w:pStyle w:val=\"EmptyCellLayoutStyle\"/><w:spacing w:after=\"0\" w:line=\"240\" w:lineRule=\"auto\"/></w:pPr></w:p>");
		}
	}
}
