using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class OpenXmlTableCellPropertiesModel : IHaveABorderAndShading
	{
		internal enum MergeState : byte
		{
			None,
			Continue,
			Start
		}

		internal enum VerticalAlign : byte
		{
			Top,
			Bottom,
			Middle
		}

		internal enum TextOrientationEnum : byte
		{
			Horizontal,
			Rotate270,
			Rotate90
		}

		private int _width;

		private MergeState _horizontalMerge;

		private MergeState _verticalMerge;

		private VerticalAlign _verticalAlignment;

		private TextOrientationEnum _textOrientation;

		private double _paddingTop;

		private double _paddingBottom;

		private double _paddingLeft;

		private double _paddingRight;

		private OpenXmlBorderPropertiesModel _borderTop;

		private OpenXmlBorderPropertiesModel _borderBottom;

		private OpenXmlBorderPropertiesModel _borderLeft;

		private OpenXmlBorderPropertiesModel _borderRight;

		private OpenXmlBorderPropertiesModel _borderDiagonalUp;

		private OpenXmlBorderPropertiesModel _borderDiagonalDown;

		private string _bgColor;

		public int Width
		{
			set
			{
				this._width = value;
			}
		}

		public double PaddingTop
		{
			set
			{
				this._paddingTop = ((value < 0.0) ? 0.0 : value);
			}
		}

		public double PaddingBottom
		{
			set
			{
				this._paddingBottom = ((value < 0.0) ? 0.0 : value);
			}
		}

		public double PaddingLeft
		{
			set
			{
				this._paddingLeft = ((value < 0.0) ? 0.0 : value);
			}
		}

		public double PaddingRight
		{
			set
			{
				this._paddingRight = ((value < 0.0) ? 0.0 : value);
			}
		}

		public string BackgroundColor
		{
			set
			{
				this._bgColor = value;
			}
		}

		public OpenXmlBorderPropertiesModel BorderTop
		{
			get
			{
				if (this._borderTop == null)
				{
					this._borderTop = new OpenXmlBorderPropertiesModel();
				}
				return this._borderTop;
			}
		}

		public OpenXmlBorderPropertiesModel BorderBottom
		{
			get
			{
				if (this._borderBottom == null)
				{
					this._borderBottom = new OpenXmlBorderPropertiesModel();
				}
				return this._borderBottom;
			}
		}

		public OpenXmlBorderPropertiesModel BorderLeft
		{
			get
			{
				if (this._borderLeft == null)
				{
					this._borderLeft = new OpenXmlBorderPropertiesModel();
				}
				return this._borderLeft;
			}
		}

		public OpenXmlBorderPropertiesModel BorderRight
		{
			get
			{
				if (this._borderRight == null)
				{
					this._borderRight = new OpenXmlBorderPropertiesModel();
				}
				return this._borderRight;
			}
		}

		public OpenXmlBorderPropertiesModel BorderDiagonalUp
		{
			get
			{
				if (this._borderDiagonalUp == null)
				{
					this._borderDiagonalUp = new OpenXmlBorderPropertiesModel();
				}
				return this._borderDiagonalUp;
			}
		}

		public OpenXmlBorderPropertiesModel BorderDiagonalDown
		{
			get
			{
				if (this._borderDiagonalDown == null)
				{
					this._borderDiagonalDown = new OpenXmlBorderPropertiesModel();
				}
				return this._borderDiagonalDown;
			}
		}

		public MergeState HorizontalMerge
		{
			set
			{
				this._horizontalMerge = value;
			}
		}

		public MergeState VerticalMerge
		{
			set
			{
				this._verticalMerge = value;
			}
		}

		public VerticalAlign VerticalAlignment
		{
			set
			{
				this._verticalAlignment = value;
			}
		}

		public TextOrientationEnum TextOrientation
		{
			set
			{
				this._textOrientation = value;
			}
		}

		public OpenXmlTableCellPropertiesModel()
		{
			this._paddingTop = -1.0;
			this._paddingBottom = -1.0;
			this._paddingLeft = -1.0;
			this._paddingRight = -1.0;
		}

		public void ClearBorderTop()
		{
			this._borderTop.Style = OpenXmlBorderPropertiesModel.BorderStyle.None;
		}

		public void ClearBorderBottom()
		{
			this._borderBottom.Style = OpenXmlBorderPropertiesModel.BorderStyle.None;
		}

		public void ClearBorderLeft()
		{
			this._borderLeft.Style = OpenXmlBorderPropertiesModel.BorderStyle.None;
		}

		public void ClearBorderRight()
		{
			this._borderRight.Style = OpenXmlBorderPropertiesModel.BorderStyle.None;
		}

		public void Write(TextWriter output)
		{
			output.Write("<w:tcPr>");
			output.Write("<w:tcW w:w=\"");
			output.Write(WordOpenXmlUtils.TwipsToString(this._width, 0, 31680));
			output.Write("\" w:type=\"dxa\"/>");
			if (this._horizontalMerge == MergeState.Start)
			{
				output.Write("<w:hMerge w:val=\"restart\"/>");
			}
			else if (this._horizontalMerge == MergeState.Continue)
			{
				output.Write("<w:hMerge w:val=\"continue\"/>");
			}
			if (this._verticalMerge == MergeState.Start)
			{
				output.Write("<w:vMerge w:val=\"restart\"/>");
			}
			else if (this._verticalMerge == MergeState.Continue)
			{
				output.Write("<w:vMerge w:val=\"continue\"/>");
			}
			if (this._borderTop != null || this._borderBottom != null || this._borderLeft != null || this._borderRight != null || this._borderDiagonalUp != null || this._borderDiagonalDown != null)
			{
				output.Write("<w:tcBorders>");
				if (this._borderTop != null)
				{
					this._borderTop.Write(output, "top");
				}
				if (this._borderLeft != null)
				{
					this._borderLeft.Write(output, "left");
				}
				if (this._borderBottom != null)
				{
					this._borderBottom.Write(output, "bottom");
				}
				if (this._borderRight != null)
				{
					this._borderRight.Write(output, "right");
				}
				if (this._borderDiagonalDown != null)
				{
					this._borderDiagonalDown.Write(output, "tl2br");
				}
				if (this._borderDiagonalUp != null)
				{
					this._borderDiagonalUp.Write(output, "tr2bl");
				}
				output.Write("</w:tcBorders>");
			}
			if (this._bgColor != null)
			{
				output.Write("<w:shd w:val=\"clear\" w:fill=\"");
				output.Write(this._bgColor);
				output.Write("\"/>");
			}
			this.WritePadding(output);
			if (this._textOrientation == TextOrientationEnum.Rotate90)
			{
				output.Write("<w:textDirection w:val=\"tbRlV\"/>");
			}
			else if (this._textOrientation == TextOrientationEnum.Rotate270)
			{
				output.Write("<w:textDirection w:val=\"btLr\"/>");
			}
			if (this._verticalAlignment == VerticalAlign.Bottom)
			{
				output.Write("<w:vAlign w:val=\"bottom\"/>");
			}
			else if (this._verticalAlignment == VerticalAlign.Middle)
			{
				output.Write("<w:vAlign w:val=\"center\"/>");
			}
			output.Write("</w:tcPr>");
		}

		private void WritePadding(TextWriter output)
		{
			if (this._paddingTop < 0.0 && this._paddingBottom < 0.0 && this._paddingLeft < 0.0 && this._paddingRight < 0.0)
			{
				return;
			}
			output.Write("<w:tcMar>");
			if (this._paddingTop >= 0.0)
			{
				output.Write("<w:top w:w=\"");
				output.Write(this.CellPadding(this._paddingTop));
				output.Write("\" w:type=\"dxa\"/>");
			}
			if (this._paddingLeft >= 0.0)
			{
				output.Write("<w:left w:w=\"");
				output.Write(this.CellPadding(this._paddingLeft));
				output.Write("\" w:type=\"dxa\"/>");
			}
			if (this._paddingBottom >= 0.0)
			{
				output.Write("<w:bottom w:w=\"");
				output.Write(this.CellPadding(this._paddingBottom));
				output.Write("\" w:type=\"dxa\"/>");
			}
			if (this._paddingRight >= 0.0)
			{
				output.Write("<w:right w:w=\"");
				output.Write(this.CellPadding(this._paddingRight));
				output.Write("\" w:type=\"dxa\"/>");
			}
			output.Write("</w:tcMar>");
		}

		private string CellPadding(double points)
		{
			return WordOpenXmlUtils.TwipsToString(WordOpenXmlUtils.PointsToTwips(points), 0, 31680);
		}
	}
}
