using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLDefinedNamesManager
	{
		private readonly CT_Workbook _workbook;

		public XMLDefinedNamesManager(CT_Workbook workbook)
		{
			this._workbook = workbook;
		}

		public XMLDefinedName CreateDefinedName(string name)
		{
			if (this._workbook.DefinedNames == null)
			{
				this._workbook.DefinedNames = new CT_DefinedNames();
			}
			CT_DefinedName cT_DefinedName = new CT_DefinedName();
			XMLDefinedName xMLDefinedName = new XMLDefinedName(cT_DefinedName);
			xMLDefinedName.Name = name;
			this._workbook.DefinedNames.DefinedName.Add(cT_DefinedName);
			return xMLDefinedName;
		}

		public void Cleanup()
		{
			if (this._workbook.DefinedNames != null)
			{
				for (int num = this._workbook.DefinedNames.DefinedName.Count - 1; num >= 0; num--)
				{
					if (string.IsNullOrEmpty(this._workbook.DefinedNames.DefinedName[num].Content))
					{
						this._workbook.DefinedNames.DefinedName.RemoveAt(num);
					}
				}
			}
		}
	}
}
