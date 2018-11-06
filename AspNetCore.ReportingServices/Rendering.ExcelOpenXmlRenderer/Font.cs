using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.officeDocument.x2006.sharedTypes;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel;
using System;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal sealed class Font : IFont
	{
		private readonly IFontModel mModel;

		public int Bold
		{
			set
			{
				this.mModel.Bold = Font.ToInternalBold(value);
			}
		}

		public IColor Color
		{
			set
			{
				this.mModel.Color = Font.ToInternalColor(value);
			}
		}

		public bool Italic
		{
			set
			{
				this.mModel.Italic = value;
			}
		}

		internal IFontModel Model
		{
			get
			{
				return this.mModel;
			}
		}

		public string Name
		{
			set
			{
				this.mModel.Name = value;
			}
		}

		public ScriptStyle ScriptStyle
		{
			set
			{
				this.mModel.ScriptStyle = Font.ToInternalScriptStyle(value);
			}
		}

		public double Size
		{
			set
			{
				this.mModel.Size = Font.ToInternalSize(value);
			}
		}

		public bool Strikethrough
		{
			set
			{
				this.mModel.Strikethrough = value;
			}
		}

		public Underline Underline
		{
			set
			{
				this.mModel.Underline = Font.ToInternalUnderline(value);
			}
		}

		internal Font(IFontModel model)
		{
			this.mModel = model;
		}

		public void SetFont(Font font)
		{
			this.mModel.SetFont(font.Model);
		}

		public static bool ToInternalBold(int bold)
		{
			return bold > 500;
		}

		public static ColorModel ToInternalColor(IColor color)
		{
			return ((Color)color).Model;
		}

		public static ST_VerticalAlignRun ToInternalScriptStyle(ScriptStyle scriptStyle)
		{
			switch (scriptStyle)
			{
			case ScriptStyle.Superscript:
				return ST_VerticalAlignRun.superscript;
			case ScriptStyle.Subscript:
				return ST_VerticalAlignRun.subscript;
			default:
				return ST_VerticalAlignRun.baseline;
			}
		}

		public static double ToInternalSize(double size)
		{
			return Math.Max(1.0, Math.Min(409.55, size));
		}

		public static ST_UnderlineValues ToInternalUnderline(Underline underline)
		{
			switch (underline)
			{
			case Underline.Single:
				return ST_UnderlineValues.single;
			case Underline.Double:
				return ST_UnderlineValues._double;
			case Underline.Accounting:
				return ST_UnderlineValues.singleAccounting;
			case Underline.DoubleAccounting:
				return ST_UnderlineValues.doubleAccounting;
			default:
				return ST_UnderlineValues.none;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is Font)
			{
				if (obj == this)
				{
					return true;
				}
				Font font = (Font)obj;
				return font.mModel.Equals(this.mModel);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.mModel.GetHashCode();
		}
	}
}
