using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model
{
	internal class XMLCharacterRunModel : ICharacterRunModel
	{
		private readonly CharacterRun _interface;

		private readonly int _startIndex;

		private readonly int _length;

		private readonly List<CT_RElt> _runs;

		private readonly XMLFontRunModel _font;

		public CharacterRun Interface
		{
			get
			{
				return this._interface;
			}
		}

		public int StartIndex
		{
			get
			{
				return this._startIndex;
			}
		}

		public int Length
		{
			get
			{
				return this._length;
			}
		}

		public XMLCharacterRunModel(List<CT_RElt> runs, int startIndex, int length, XMLPaletteModel palette)
		{
			this._runs = runs;
			this._startIndex = startIndex;
			this._length = length;
			this._interface = new CharacterRun(this);
			this._font = new XMLFontRunModel(palette);
			foreach (CT_RElt run in runs)
			{
				if (run.RPr == null)
				{
					run.RPr = new CT_RPrElt();
				}
				this._font.Add(run.RPr);
			}
		}

		public void SetFont(Font font)
		{
			XMLFontModel xMLFontModel = font.Model as XMLFontModel;
			if (xMLFontModel != null)
			{
				this._font.SetFont(xMLFontModel);
				return;
			}
			throw new FatalException();
		}

		public void Split(CT_RElt original, CT_RElt added)
		{
			if (this._runs.Contains(original))
			{
				this._runs.Add(added);
				this._font.Add(added.RPr);
			}
		}
	}
}
