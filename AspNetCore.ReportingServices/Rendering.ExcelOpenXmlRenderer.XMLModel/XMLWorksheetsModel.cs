using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLWorksheetsModel : IWorksheetsModel, IEnumerable<IWorksheetModel>, IEnumerable
	{
		private readonly Worksheets _interface;

		protected readonly XMLWorkbookModel Workbook;

		protected readonly PartManager Manager;

		protected List<IWorksheetModel> Worksheets;

		private uint _nextId;

		internal uint NextId
		{
			get
			{
				return this._nextId++;
			}
			set
			{
				this._nextId = value;
			}
		}

		public Worksheets Interface
		{
			get
			{
				return this._interface;
			}
		}

		public int Count
		{
			get
			{
				return this.SheetModels.Count;
			}
		}

		private IList<IWorksheetModel> SheetModels
		{
			get
			{
				if (this.Worksheets == null)
				{
					this.Worksheets = new List<IWorksheetModel>();
				}
				return this.Worksheets;
			}
		}

		public XMLWorksheetsModel(XMLWorkbookModel workbook, PartManager manager)
		{
			this.Workbook = workbook;
			this.Manager = manager;
			this._interface = new Worksheets(this);
		}

		public IWorksheetModel GetWorksheet(int position)
		{
			if (position >= 0 && position < this.Count)
			{
				return this.SheetModels[position];
			}
			throw new FatalException();
		}

		public int getSheetPosition(string name)
		{
			if (this.SheetModels.Count == 0)
			{
				return -1;
			}
			for (int i = 0; i < this.SheetModels.Count; i++)
			{
				if (this.SheetModels[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
				{
					return i;
				}
			}
			return -1;
		}

		public virtual IStreamsheetModel CreateStreamsheet(string sheetName, ExcelGeneratorConstants.CreateTempStream createTempStream)
		{
			throw new FatalException();
		}

		public IEnumerator<IWorksheetModel> GetEnumerator()
		{
			return this.SheetModels.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
