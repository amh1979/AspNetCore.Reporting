using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal abstract class XMLWorkbookModel : IWorkbookModel
	{
		protected PartManager _manager;

		protected XMLWorksheetsModel _worksheets;

		protected XMLDefinedNamesManager _nameManager;

		public abstract IWorksheetsModel Worksheets
		{
			get;
		}

		public IPaletteModel Palette
		{
			get
			{
				return this._manager.StyleSheet.Palette;
			}
		}

		public XMLDefinedNamesManager NameManager
		{
			get
			{
				return this._nameManager;
			}
		}

		public IWorksheetModel getWorksheet(int sheetOffset)
		{
			return this.Worksheets.GetWorksheet(sheetOffset);
		}

		public IStyleModel createGlobalStyle()
		{
			return this._manager.StyleSheet.CreateStyle();
		}

		public void Cleanup()
		{
			foreach (IWorksheetModel worksheet in this.Worksheets)
			{
				((XMLWorksheetModel)worksheet).Cleanup();
			}
			this._nameManager.Cleanup();
		}
	}
}
