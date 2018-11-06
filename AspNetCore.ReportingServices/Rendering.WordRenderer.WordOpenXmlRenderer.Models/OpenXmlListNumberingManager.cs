using AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main;
using System.Globalization;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class OpenXmlListNumberingManager
	{
		private int _numberedListCount;

		internal OpenXmlListNumberingManager()
		{
			this._numberedListCount = 0;
		}

		private CT_Lvl NumberingLevel(int level, ST_NumberFormat numFmt, string text, string font, ST_Jc jc, string size)
		{
			CT_Lvl cT_Lvl = new CT_Lvl();
			cT_Lvl.Ilvl_Attr = level;
			cT_Lvl.Start = new CT_DecimalNumber
			{
				Val_Attr = 1
			};
			cT_Lvl.NumFmt = new CT_NumFmt
			{
				Val_Attr = numFmt
			};
			cT_Lvl.LvlText = new CT_LevelText
			{
				Val_Attr = text
			};
			cT_Lvl.LvlJc = new CT_Jc
			{
				Val_Attr = jc
			};
			cT_Lvl.RPr = new CT_RPr
			{
				RFonts = new CT_Fonts
				{
					Ascii_Attr = font,
					EastAsia_Attr = font,
					HAnsi_Attr = font,
					Cs_Attr = font
				},
				Sz = new CT_HpsMeasure
				{
					Val_Attr = size
				}
			};
			return cT_Lvl;
		}

		private CT_AbstractNum NumberedListDefinition(int count)
		{
			CT_AbstractNum cT_AbstractNum = new CT_AbstractNum();
			cT_AbstractNum.AbstractNumId_Attr = count + 1;
			cT_AbstractNum.Nsid = new CT_LongHexNumber
			{
				Val_Attr = (count + 2).ToString("X", CultureInfo.InvariantCulture).PadLeft(8, '0')
			};
			cT_AbstractNum.MultiLevelType = new CT_MultiLevelType
			{
				Val_Attr = ST_MultiLevelType.multilevel
			};
			cT_AbstractNum.Tmpl = new CT_LongHexNumber
			{
				Val_Attr = cT_AbstractNum.Nsid.Val_Attr
			};
			for (int i = 0; i < 9; i++)
			{
				cT_AbstractNum.Lvl.Add(this.NumberingLevel(i, WordOpenXmlConstants.Lists.Numbered.LevelStyles[i % WordOpenXmlConstants.Lists.Numbered.LevelStyles.Length], string.Format(CultureInfo.InvariantCulture, "%{0}.", i + 1), "Arial", WordOpenXmlConstants.Lists.Numbered.LevelJc, "20"));
			}
			return cT_AbstractNum;
		}

		internal int RegisterNewNumberedList()
		{
			int result = 2 + this._numberedListCount;
			this._numberedListCount++;
			return result;
		}

		internal NumberingPart CreateNumberingPart()
		{
			CT_AbstractNum cT_AbstractNum = new CT_AbstractNum();
			cT_AbstractNum.AbstractNumId_Attr = 0;
			cT_AbstractNum.Nsid = new CT_LongHexNumber
			{
				Val_Attr = "00000001"
			};
			cT_AbstractNum.MultiLevelType = new CT_MultiLevelType
			{
				Val_Attr = ST_MultiLevelType.multilevel
			};
			cT_AbstractNum.Tmpl = new CT_LongHexNumber
			{
				Val_Attr = "00000001"
			};
			for (int i = 0; i < 9; i++)
			{
				cT_AbstractNum.Lvl.Add(this.NumberingLevel(i, WordOpenXmlConstants.Lists.Bulleted.LevelStyle, WordOpenXmlConstants.Lists.Bulleted.BulletTexts[i % WordOpenXmlConstants.Lists.Bulleted.BulletTexts.Length], WordOpenXmlConstants.Lists.Bulleted.BulletFonts[i % WordOpenXmlConstants.Lists.Bulleted.BulletFonts.Length], WordOpenXmlConstants.Lists.Bulleted.LevelJc, "20"));
			}
			CT_Num cT_Num = new CT_Num();
			cT_Num.NumId_Attr = 1;
			cT_Num.AbstractNumId = new CT_DecimalNumber
			{
				Val_Attr = 0
			};
			CT_Num item = cT_Num;
			NumberingPart numberingPart = new NumberingPart();
			CT_Numbering cT_Numbering = (CT_Numbering)numberingPart.Root;
			cT_Numbering.AbstractNum.Add(cT_AbstractNum);
			cT_Numbering.Num.Add(item);
			for (int j = 0; j < this._numberedListCount; j++)
			{
				CT_AbstractNum cT_AbstractNum2 = this.NumberedListDefinition(j);
				CT_Num cT_Num2 = new CT_Num();
				cT_Num2.NumId_Attr = 2 + j;
				cT_Num2.AbstractNumId = new CT_DecimalNumber
				{
					Val_Attr = cT_AbstractNum2.AbstractNumId_Attr
				};
				CT_Num item2 = cT_Num2;
				cT_Numbering.AbstractNum.Add(cT_AbstractNum2);
				cT_Numbering.Num.Add(item2);
			}
			return numberingPart;
		}
	}
}
