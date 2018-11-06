using System.Globalization;

namespace AspNetCore.ReportingServices.Rendering.RichText
{
	internal class TextBoxContext
	{
		internal int ParagraphIndex;

		internal int TextRunIndex;

		internal int TextRunCharacterIndex;

		internal TextBoxContext()
		{
		}

		internal void IncrementParagraph()
		{
			this.ParagraphIndex++;
			this.TextRunIndex = 0;
			this.TextRunCharacterIndex = 0;
		}

		internal TextBoxContext Clone()
		{
			TextBoxContext textBoxContext = new TextBoxContext();
			textBoxContext.ParagraphIndex = this.ParagraphIndex;
			textBoxContext.TextRunIndex = this.TextRunIndex;
			textBoxContext.TextRunCharacterIndex = this.TextRunCharacterIndex;
			return textBoxContext;
		}

		internal void Reset()
		{
			this.ParagraphIndex = 0;
			this.TextRunIndex = 0;
			this.TextRunCharacterIndex = 0;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "P:{0} TR:{1} NCI:{2}", this.ParagraphIndex, this.TextRunIndex, this.TextRunCharacterIndex);
		}
	}
}
