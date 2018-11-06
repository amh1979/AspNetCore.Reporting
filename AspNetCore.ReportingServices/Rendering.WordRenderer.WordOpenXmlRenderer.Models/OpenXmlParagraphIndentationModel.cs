using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class OpenXmlParagraphIndentationModel
	{
		private double? _hanging;

		private double? _first;

		private double? _left;

		private double? _right;

		internal double Hanging
		{
			set
			{
				this._hanging = value;
			}
		}

		internal double First
		{
			set
			{
				this._first = value;
			}
		}

		internal double Left
		{
			set
			{
				this._left = value;
			}
		}

		internal double Right
		{
			set
			{
				this._right = value;
			}
		}

		public void Write(TextWriter writer)
		{
			if (!this._hanging.HasValue && !this._first.HasValue && !this._left.HasValue && !this._right.HasValue)
			{
				return;
			}
			writer.Write("<w:ind");
			if (this._left.HasValue)
			{
				writer.Write(" w:left=\"");
				writer.Write(WordOpenXmlUtils.TwipsToString(WordOpenXmlUtils.PointsToTwips(this._left.Value), -31680, 31680));
				writer.Write("\"");
			}
			if (this._right.HasValue)
			{
				writer.Write(" w:right=\"");
				writer.Write(WordOpenXmlUtils.TwipsToString(WordOpenXmlUtils.PointsToTwips(this._right.Value), -31680, 31680));
				writer.Write("\"");
			}
			if (this._hanging.HasValue)
			{
				writer.Write(" w:hanging=\"");
				writer.Write(WordOpenXmlUtils.TwipsToString(WordOpenXmlUtils.PointsToTwips(this._hanging.Value), 0, 31680));
				writer.Write("\"");
			}
			if (this._first.HasValue)
			{
				writer.Write(" w:firstLine=\"");
				writer.Write(WordOpenXmlUtils.TwipsToString(WordOpenXmlUtils.PointsToTwips(this._first.Value), 0, 31680));
				writer.Write("\"");
			}
			writer.Write("/>");
		}
	}
}
