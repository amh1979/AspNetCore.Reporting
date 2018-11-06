using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLDefinedName
	{
		private readonly CT_DefinedName _definedname;

		public string Name
		{
			set
			{
				XMLDefinedName.TryValidName(value);
				this._definedname.Name_Attr = value;
			}
		}

		public string Content
		{
			set
			{
				this._definedname.Content = value;
			}
		}

		public int SheetIndex
		{
			set
			{
				this._definedname.LocalSheetId_Attr = (uint)value;
			}
		}

		public XMLDefinedName(CT_DefinedName definedName)
		{
			this._definedname = definedName;
		}

		private static void TryValidName(string name)
		{
			if (name == null)
			{
				throw new FatalException();
			}
			if (name.Length >= 1 && name.Length <= 255)
			{
				if (!name.Equals("true", StringComparison.OrdinalIgnoreCase) && !name.Equals("false", StringComparison.OrdinalIgnoreCase))
				{
					if (XMLConstants.DefinedNames.DefinedNameValidationRe.IsMatch(name))
					{
						return;
					}
					throw new FatalException();
				}
				throw new FatalException();
			}
			throw new FatalException();
		}
	}
}
