using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.officeDocument.x2006.sharedTypes;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal class Style : IStyle, IFont
	{
		private IStyleModel mModel;

		public IColor BackgroundColor
		{
			set
			{
				if (value == null && this.mModel.BackgroundColor == null)
				{
					return;
				}
				ColorModel colorModel = (value == null) ? null : ((Color)value).Model;
				this.Clone(this.mModel.BackgroundColor, colorModel);
				this.mModel.BackgroundColor = colorModel;
			}
		}

		public Font Font
		{
			get
			{
				return this.mModel.Font.Interface;
			}
		}

		public HorizontalAlignment HorizontalAlignment
		{
			set
			{
				ST_HorizontalAlignment sT_HorizontalAlignment = Style.ToInternalHorizontalAlignment(value);
				this.Clone(this.mModel.HorizontalAlignment, sT_HorizontalAlignment);
				this.mModel.HorizontalAlignment = sT_HorizontalAlignment;
			}
		}

		public int IndentLevel
		{
			set
			{
				if (value < 0)
				{
					this.CloneNullable(this.mModel.IndentLevel, 0);
					this.mModel.IndentLevel = 0;
				}
				else if (value > 255)
				{
					this.CloneNullable(this.mModel.IndentLevel, 255);
					this.mModel.IndentLevel = 255;
				}
				else
				{
					this.CloneNullable(this.mModel.IndentLevel, value);
					this.mModel.IndentLevel = value;
				}
			}
		}

		internal IStyleModel Model
		{
			get
			{
				return this.mModel;
			}
		}

		public string NumberFormat
		{
			set
			{
				this.Clone(this.mModel.NumberFormat, value);
				this.mModel.NumberFormat = value;
			}
		}

		public int Orientation
		{
			set
			{
				if (value == AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel.Orientation.Rotate90ClockWise)
				{
					this.CloneNullable(this.mModel.Orientation, 180);
					this.mModel.Orientation = 180;
				}
				else
				{
					this.CloneNullable(this.mModel.Orientation, value);
					this.mModel.Orientation = value;
				}
			}
		}

		public TextDirection TextDirection
		{
			set
			{
				byte b = Style.ToInternalTextDirection(value);
				this.CloneNullable(this.mModel.TextDirection, b);
				this.mModel.TextDirection = b;
			}
		}

		public VerticalAlignment VerticalAlignment
		{
			set
			{
				ST_VerticalAlignment sT_VerticalAlignment = Style.ToInternalVerticalAlignment(value);
				this.Clone(this.mModel.VerticalAlignment, sT_VerticalAlignment);
				this.mModel.VerticalAlignment = sT_VerticalAlignment;
			}
		}

		public bool WrapText
		{
			set
			{
				this.CloneNullable(this.mModel.WrapText, value);
				this.mModel.WrapText = value;
			}
		}

		public int Bold
		{
			set
			{
				bool flag = Font.ToInternalBold(value);
				this.CloneValueType(this.mModel.Font.Bold, flag);
				this.mModel.Font.Bold = flag;
			}
		}

		public bool Italic
		{
			set
			{
				this.CloneValueType(this.mModel.Font.Italic, value);
				this.mModel.Font.Italic = value;
			}
		}

		public bool Strikethrough
		{
			set
			{
				this.CloneValueType(this.mModel.Font.Strikethrough, value);
				this.mModel.Font.Strikethrough = value;
			}
		}

		public ScriptStyle ScriptStyle
		{
			set
			{
				ST_VerticalAlignRun sT_VerticalAlignRun = Font.ToInternalScriptStyle(value);
				this.Clone(this.mModel.Font.ScriptStyle, sT_VerticalAlignRun);
				this.mModel.Font.ScriptStyle = sT_VerticalAlignRun;
			}
		}

		public IColor Color
		{
			set
			{
				ColorModel colorModel = Font.ToInternalColor(value);
				this.Clone(this.mModel.Font.Color, colorModel);
				this.mModel.Font.Color = colorModel;
			}
		}

		public Underline Underline
		{
			set
			{
				ST_UnderlineValues sT_UnderlineValues = Font.ToInternalUnderline(value);
				this.Clone(this.mModel.Font.Underline, sT_UnderlineValues);
				this.mModel.Font.Underline = sT_UnderlineValues;
			}
		}

		public string Name
		{
			set
			{
				this.Clone(this.mModel.Font.Name, value);
				this.mModel.Font.Name = value;
			}
		}

		public double Size
		{
			set
			{
				value = Font.ToInternalSize(value);
				this.CloneValueType(this.mModel.Font.Size, value);
				this.mModel.Font.Size = value;
			}
		}

		public ExcelBorderStyle BorderLeftStyle
		{
			set
			{
				ST_BorderStyle sT_BorderStyle = Style.ToInternalBorderStyle(value);
				this.Clone(this.mModel.BorderModel.LeftBorder.Style, sT_BorderStyle);
				this.mModel.BorderModel.LeftBorder.Style = sT_BorderStyle;
			}
		}

		public ExcelBorderStyle BorderRightStyle
		{
			set
			{
				ST_BorderStyle sT_BorderStyle = Style.ToInternalBorderStyle(value);
				this.Clone(this.mModel.BorderModel.RightBorder.Style, sT_BorderStyle);
				this.mModel.BorderModel.RightBorder.Style = sT_BorderStyle;
			}
		}

		public ExcelBorderStyle BorderTopStyle
		{
			set
			{
				ST_BorderStyle sT_BorderStyle = Style.ToInternalBorderStyle(value);
				this.Clone(this.mModel.BorderModel.TopBorder.Style, sT_BorderStyle);
				this.mModel.BorderModel.TopBorder.Style = sT_BorderStyle;
			}
		}

		public ExcelBorderStyle BorderBottomStyle
		{
			set
			{
				ST_BorderStyle sT_BorderStyle = Style.ToInternalBorderStyle(value);
				this.Clone(this.mModel.BorderModel.BottomBorder.Style, sT_BorderStyle);
				this.mModel.BorderModel.BottomBorder.Style = sT_BorderStyle;
			}
		}

		public ExcelBorderStyle BorderDiagStyle
		{
			set
			{
				ST_BorderStyle sT_BorderStyle = Style.ToInternalBorderStyle(value);
				this.Clone(this.mModel.BorderModel.DiagonalBorder.Style, sT_BorderStyle);
				this.mModel.BorderModel.DiagonalBorder.Style = sT_BorderStyle;
			}
		}

		public ExcelBorderPart BorderDiagPart
		{
			set
			{
				this.mModel.BorderModel.DiagonalPartDirection = value;
			}
		}

		public IColor BorderLeftColor
		{
			set
			{
				ColorModel colorModel = (value == null) ? null : ((Color)value).Model;
				this.Clone(this.mModel.BorderModel.LeftBorder.Color, colorModel);
				this.mModel.BorderModel.LeftBorder.Color = colorModel;
			}
		}

		public IColor BorderRightColor
		{
			set
			{
				ColorModel colorModel = (value == null) ? null : ((Color)value).Model;
				this.Clone(this.mModel.BorderModel.RightBorder.Color, colorModel);
				this.mModel.BorderModel.RightBorder.Color = colorModel;
			}
		}

		public IColor BorderTopColor
		{
			set
			{
				ColorModel colorModel = (value == null) ? null : ((Color)value).Model;
				this.Clone(this.mModel.BorderModel.TopBorder.Color, colorModel);
				this.mModel.BorderModel.TopBorder.Color = colorModel;
			}
		}

		public IColor BorderBottomColor
		{
			set
			{
				ColorModel colorModel = (value == null) ? null : ((Color)value).Model;
				this.Clone(this.mModel.BorderModel.BottomBorder.Color, colorModel);
				this.mModel.BorderModel.BottomBorder.Color = colorModel;
			}
		}

		public IColor BorderDiagColor
		{
			set
			{
				ColorModel colorModel = (value == null) ? null : ((Color)value).Model;
				this.Clone(this.mModel.BorderModel.DiagonalBorder.Color, colorModel);
				this.mModel.BorderModel.DiagonalBorder.Color = colorModel;
			}
		}

		public bool HasBeenModified
		{
			get
			{
				return this.mModel.HasBeenModified;
			}
		}

		internal Style(IStyleModel model)
		{
			this.mModel = model;
		}

		private void Clone<T>(T currentValue, T newValue) where T : class
		{
			if (!(newValue == null ^ currentValue == null))
			{
				if (newValue == null)
				{
					return;
				}
				if (newValue.Equals(currentValue))
				{
					return;
				}
			}
			this.mModel = this.mModel.cloneStyle(true);
		}

		private void CloneNullable<T>(T? currentValue, T newValue) where T : struct
		{
			if (currentValue.HasValue && newValue.Equals(currentValue))
			{
				return;
			}
			this.mModel = this.mModel.cloneStyle(true);
		}

		private void CloneValueType<T>(T currentValue, T newValue) where T : struct
		{
			if (!newValue.Equals(currentValue))
			{
				this.mModel = this.mModel.cloneStyle(true);
			}
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is Style)
			{
				if (obj == this)
				{
					return true;
				}
				Style style = (Style)obj;
				return style.mModel.Equals(this.mModel);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.mModel.GetHashCode();
		}

		private static ST_BorderStyle ToInternalBorderStyle(ExcelBorderStyle borderStyle)
		{
			switch (borderStyle)
			{
			case ExcelBorderStyle.None:
				return ST_BorderStyle.none;
			case ExcelBorderStyle.Thin:
				return ST_BorderStyle.thin;
			case ExcelBorderStyle.Medium:
				return ST_BorderStyle.medium;
			case ExcelBorderStyle.Dashed:
				return ST_BorderStyle.dashed;
			case ExcelBorderStyle.Dotted:
				return ST_BorderStyle.dotted;
			case ExcelBorderStyle.Thick:
				return ST_BorderStyle.thick;
			case ExcelBorderStyle.Double:
				return ST_BorderStyle._double;
			case ExcelBorderStyle.Hair:
				return ST_BorderStyle.hair;
			case ExcelBorderStyle.MedDashed:
				return ST_BorderStyle.mediumDashed;
			case ExcelBorderStyle.DashDot:
				return ST_BorderStyle.dashDot;
			case ExcelBorderStyle.MedDashDot:
				return ST_BorderStyle.mediumDashDot;
			case ExcelBorderStyle.DashDotDot:
				return ST_BorderStyle.dashDotDot;
			case ExcelBorderStyle.MedDashDotDot:
				return ST_BorderStyle.mediumDashDotDot;
			case ExcelBorderStyle.SlantedDashDot:
				return ST_BorderStyle.slantDashDot;
			default:
				return ST_BorderStyle.none;
			}
		}

		private static ST_HorizontalAlignment ToInternalHorizontalAlignment(HorizontalAlignment horizontalAlignment)
		{
			switch (horizontalAlignment)
			{
			case HorizontalAlignment.Left:
				return ST_HorizontalAlignment.left;
			case HorizontalAlignment.Center:
				return ST_HorizontalAlignment.center;
			case HorizontalAlignment.Right:
				return ST_HorizontalAlignment.right;
			case HorizontalAlignment.Fill:
				return ST_HorizontalAlignment.fill;
			case HorizontalAlignment.Justify:
				return ST_HorizontalAlignment.justify;
			case HorizontalAlignment.CenterAcrossSelection:
				return ST_HorizontalAlignment.centerContinuous;
			case HorizontalAlignment.Distributed:
				return ST_HorizontalAlignment.distributed;
			default:
				return ST_HorizontalAlignment.general;
			}
		}

		private static byte ToInternalTextDirection(TextDirection textDirection)
		{
			switch (textDirection)
			{
			case TextDirection.LeftToRight:
				return 1;
			case TextDirection.RightToLeft:
				return 2;
			default:
				return 0;
			}
		}

		private static ST_VerticalAlignment ToInternalVerticalAlignment(VerticalAlignment verticalAlignment)
		{
			switch (verticalAlignment)
			{
			case VerticalAlignment.Center:
				return ST_VerticalAlignment.center;
			case VerticalAlignment.Distributed:
				return ST_VerticalAlignment.distributed;
			case VerticalAlignment.Justify:
				return ST_VerticalAlignment.justify;
			case VerticalAlignment.Top:
				return ST_VerticalAlignment.top;
			default:
				return ST_VerticalAlignment.bottom;
			}
		}
	}
}
