using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLFillModel : IDeepCloneable<XMLFillModel>
	{
		private readonly CT_Fill _fill;

		private XMLColorModel _foreground;

		private XMLColorModel _background;

		private readonly XMLPaletteModel _palette;

		public CT_Fill Data
		{
			get
			{
				return this._fill;
			}
		}

		public XMLColorModel Color
		{
			get
			{
				return this._foreground;
			}
			set
			{
				if (this._fill.Choice_0 == CT_Fill.ChoiceBucket_0.patternFill)
				{
					this._foreground = value;
					this._fill.PatternFill.PatternType_Attr = ((value == null) ? ST_PatternType.none : ST_PatternType.solid);
					this._background = value;
					return;
				}
				throw new FatalException();
			}
		}

		public XMLFillModel(XMLPaletteModel palette)
		{
			this._fill = new CT_Fill();
			this._fill.Choice_0 = CT_Fill.ChoiceBucket_0.patternFill;
			this._fill.PatternFill = new CT_PatternFill();
			this._fill.PatternFill.PatternType_Attr = ST_PatternType.none;
			this._palette = palette;
		}

		public XMLFillModel(CT_Fill fill, XMLPaletteModel palette)
		{
			this._fill = fill;
			this._palette = palette;
			if (this._fill.PatternFill != null)
			{
				if (this._fill.PatternFill.FgColor != null)
				{
					this._foreground = this._palette.GetColorFromCT(this._fill.PatternFill.FgColor);
				}
				if (this._fill.PatternFill.BgColor != null)
				{
					this._background = this._palette.GetColorFromCT(this._fill.PatternFill.BgColor);
				}
			}
		}

		public XMLFillModel DeepClone()
		{
			CT_Fill cT_Fill = new CT_Fill();
			if (this._fill.PatternFill != null)
			{
				CT_PatternFill patternFill = this._fill.PatternFill;
				CT_PatternFill cT_PatternFill2 = cT_Fill.PatternFill = new CT_PatternFill();
				if (patternFill.BgColor != null)
				{
					cT_PatternFill2.BgColor = this._palette.GetColorFromCT(patternFill.BgColor).Clone().Data;
				}
				if (patternFill.FgColor != null)
				{
					cT_PatternFill2.FgColor = this._palette.GetColorFromCT(patternFill.FgColor).Clone().Data;
				}
				cT_PatternFill2.PatternType_Attr = patternFill.PatternType_Attr;
			}
			cT_Fill.Choice_0 = this._fill.Choice_0;
			XMLFillModel xMLFillModel = new XMLFillModel(cT_Fill, this._palette);
			if (this._background != null)
			{
				xMLFillModel._background = this._background.Clone();
			}
			if (this._foreground != null)
			{
				xMLFillModel._foreground = this._foreground.Clone();
			}
			return xMLFillModel;
		}

		public override bool Equals(object o)
		{
			if (!(o is XMLFillModel))
			{
				return false;
			}
			XMLFillModel xMLFillModel = (XMLFillModel)o;
			if (this._fill.Choice_0 != xMLFillModel._fill.Choice_0)
			{
				return false;
			}
			this.Cleanup();
			xMLFillModel.Cleanup();
			if (this._palette.GetColorFromCT(this._fill.PatternFill.BgColor).Equals(this._palette.GetColorFromCT(xMLFillModel._fill.PatternFill.BgColor)) && this._palette.GetColorFromCT(this._fill.PatternFill.FgColor).Equals(this._palette.GetColorFromCT(xMLFillModel._fill.PatternFill.FgColor)))
			{
				return this._fill.PatternFill.PatternType_Attr == xMLFillModel._fill.PatternFill.PatternType_Attr;
			}
			return false;
		}

		public override int GetHashCode()
		{
			this.Cleanup();
			int num = 0;
			num ^= this._palette.GetColorFromCT(this._fill.PatternFill.BgColor).GetHashCode();
			num ^= this._palette.GetColorFromCT(this._fill.PatternFill.FgColor).GetHashCode();
			return num ^ this._fill.PatternFill.PatternType_Attr.GetHashCode();
		}

		public void Cleanup()
		{
			if (this._fill.Choice_0 == CT_Fill.ChoiceBucket_0.patternFill)
			{
				if (this._foreground != null)
				{
					this._fill.PatternFill.FgColor = this._foreground.Data;
				}
				if (this._background != null)
				{
					this._fill.PatternFill.BgColor = this._background.Data;
				}
			}
		}
	}
}
