namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal static class CellReference
	{
		internal static string CreateExcelReference(string sheetName, int row, int column)
		{
			return "'" + sheetName + "'!" + CellReference.ConvertToLetter(column) + (row + 1);
		}

		private static string ConvertToLetter(int val)
		{
			string arg = "";
			int num = 26;
			if (val / num > 0)
			{
				arg += (char)(val / num + 65 - 1);
			}
			return arg + (char)(val % num + 65);
		}
	}
}
