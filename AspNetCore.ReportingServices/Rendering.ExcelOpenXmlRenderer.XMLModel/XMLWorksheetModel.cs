using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Rels.Relationships;
using System;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal abstract class XMLWorksheetModel : IWorksheetModel, ICloneable
	{
		protected XMLWorkbookModel _workbookModel;

		protected XMLWorksheetsModel _worksheetsModel;

		protected PartManager _manager;

		protected XMLPictureShapesModel PicturesModel;

		protected CT_Sheet _sheetentry;

		public abstract Streamsheet Interface
		{
			get;
		}

		public IWorkbookModel Workbook
		{
			get
			{
				return this._workbookModel;
			}
		}

		public abstract IPictureShapesModel Pictures
		{
			get;
		}

		public abstract IPageSetupModel PageSetup
		{
			get;
		}

		public string Name
		{
			get
			{
				return this._sheetentry.Name_Attr;
			}
			set
			{
				this._sheetentry.Name_Attr = value;
			}
		}

		public int Position
		{
			get
			{
				return this._worksheetsModel.getSheetPosition(this.Name);
			}
		}

		public abstract bool ShowGridlines
		{
			set;
		}

		public XMLDefinedNamesManager NameManager
		{
			get
			{
				return this._workbookModel.NameManager;
			}
		}

		public abstract IColumnModel getColumn(int index);

		public AnchorModel createAnchor(int row, int column, double offsetX, double offsetY)
		{
			return new AnchorModel(row, column, offsetX, offsetY);
		}

		public abstract void SetBackgroundPicture(string uniqueId, string extension, Stream pictureStream);

		protected string InsertBackgroundPicture(string uniqueId, string extension, Stream pictureStream)
		{
			Relationship relationship = this._manager.AddImageToTree(uniqueId, pictureStream, extension, "http://schemas.openxmlformats.org/officeDocument/2006/relationships/image", "xl/media/image{0}." + extension, this._manager.GetWorksheetXmlPart(this._sheetentry).Location, ContentTypeAction.Default);
			return relationship.RelationshipId;
		}

		public object Clone()
		{
			return base.MemberwiseClone();
		}

		public abstract void Cleanup();
	}
}
