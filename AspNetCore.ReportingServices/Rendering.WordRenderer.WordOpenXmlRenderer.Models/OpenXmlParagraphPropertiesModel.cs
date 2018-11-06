using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class OpenXmlParagraphPropertiesModel
	{
		internal enum HorizontalAlignment
		{
			Center,
			Left,
			Right
		}

		private int? _listLevel;

		private int? _listStyleId;

		private HorizontalAlignment? _horizontalAlignment;

		private OpenXmlParagraphIndentationModel _indentation;

		private double? _pointsBefore;

		private double _lineHeight;

		private bool _lineSpacingAtLeast;

		public OpenXmlParagraphIndentationModel Indentation
		{
			get
			{
				return this._indentation;
			}
		}

		public bool RightToLeft
		{
			private get;
			set;
		}

		public int ListLevel
		{
			set
			{
				this._listLevel = value;
			}
		}

		public int ListStyleId
		{
			set
			{
				this._listStyleId = value;
			}
		}

		public HorizontalAlignment HorizontalAlign
		{
			set
			{
				this._horizontalAlignment = value;
			}
		}

		public double PointsBefore
		{
			set
			{
				this._pointsBefore = value;
			}
		}

		public double PointsAfter
		{
			private get;
			set;
		}

		public double LineHeight
		{
			set
			{
				this._lineSpacingAtLeast = true;
				this._lineHeight = value;
			}
		}

		public OpenXmlParagraphPropertiesModel()
		{
			this._indentation = new OpenXmlParagraphIndentationModel();
			this.PointsAfter = 0.0;
			this._lineSpacingAtLeast = false;
			this._lineHeight = 12.0;
		}

		public void Write(TextWriter writer)
		{
			writer.Write("<w:pPr>");
			if (this._listLevel.HasValue || this._listStyleId.HasValue)
			{
				writer.Write("<w:numPr>");
				if (this._listLevel.HasValue)
				{
					writer.Write("<w:ilvl w:val=\"");
					writer.Write(this._listLevel);
					writer.Write("\"/>");
				}
				if (this._listStyleId.HasValue)
				{
					writer.Write("<w:numId w:val=\"");
					writer.Write(this._listStyleId);
					writer.Write("\"/>");
				}
				writer.Write("</w:numPr>");
			}
			if (this.RightToLeft)
			{
				writer.Write("<w:bidi/>");
			}
			writer.Write("<w:spacing ");
			if (this._pointsBefore.HasValue)
			{
				writer.Write("w:before=\"");
				writer.Write(WordOpenXmlUtils.TwipsToString(WordOpenXmlUtils.PointsToTwips(this._pointsBefore.Value), 0, 31680));
				writer.Write("\" ");
			}
			writer.Write("w:after=\"");
			writer.Write(WordOpenXmlUtils.TwipsToString(WordOpenXmlUtils.PointsToTwips(this.PointsAfter), 0, 31680));
			writer.Write("\" w:line=\"");
			writer.Write(WordOpenXmlUtils.TwipsToString(WordOpenXmlUtils.PointsToTwips(this._lineHeight), 0, 31680));
			writer.Write(this._lineSpacingAtLeast ? "\" w:lineRule=\"atLeast\"/>" : "\" w:lineRule=\"auto\"/>");
			this._indentation.Write(writer);
			if (this._horizontalAlignment.HasValue)
			{
				switch (this._horizontalAlignment.Value)
				{
				case HorizontalAlignment.Center:
					writer.Write("<w:jc w:val=\"center\"/>");
					break;
				case HorizontalAlignment.Left:
					writer.Write("<w:jc w:val=\"left\"/>");
					break;
				case HorizontalAlignment.Right:
					writer.Write("<w:jc w:val=\"right\"/>");
					break;
				}
			}
			writer.Write("</w:pPr>");
		}
	}
}
