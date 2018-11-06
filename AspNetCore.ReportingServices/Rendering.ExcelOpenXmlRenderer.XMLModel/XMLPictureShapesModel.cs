using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.spreadsheetDrawing;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Rels.Relationships;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLPictureShapesModel : IPictureShapesModel
	{
		private Pictures _interface;

		private readonly PartManager _manager;

		private readonly XmlPart _parent;

		private readonly WsDrPart _drawing;

		private readonly Relationship _drawingrel;

		private readonly List<XMLPictureShapeModel> _pictures;

		private uint _nextid;

		public Pictures Interface
		{
			get
			{
				if (this._interface == null)
				{
					this._interface = new Pictures(this);
				}
				return this._interface;
			}
		}

		private uint NextId
		{
			get
			{
				return this._nextid += 1u;
			}
		}

		public XMLPictureShapesModel(PartManager manager, CT_Sheet sheetEntry, string drawingId)
		{
			this._manager = manager;
			this._parent = this._manager.GetWorksheetXmlPart(sheetEntry);
			this._pictures = new List<XMLPictureShapeModel>();
			foreach (Relationship item in this._manager.GetRelationshipsForSheet(sheetEntry, "http://schemas.openxmlformats.org/officeDocument/2006/relationships/drawing"))
			{
				if (item.RelationshipId == drawingId)
				{
					this._drawingrel = item;
					this._drawing = (WsDrPart)this._manager.GetPartByLocation(item.RelatedPart).HydratedPart;
					AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.spreadsheetDrawing.CT_Drawing cT_Drawing = (AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.spreadsheetDrawing.CT_Drawing)this._drawing.Root;
					break;
				}
			}
		}

		public IPictureShapeModel CreatePicture(string uniqueId, string extension, Stream pictureStream, AnchorModel startPosition, AnchorModel endPosition)
		{
			XMLPictureShapeModel xMLPictureShapeModel = new XMLPictureShapeModel(this._manager, this._drawing, this._manager.GetPartByLocation(this._drawingrel.RelatedPart), startPosition, endPosition, this.NextId);
			this._pictures.Add(xMLPictureShapeModel);
			Relationship relationship = this._manager.AddImageToTree(uniqueId, pictureStream, extension, "http://schemas.openxmlformats.org/officeDocument/2006/relationships/image", "xl/media/image{0}." + extension, this._drawingrel.RelatedPart, ContentTypeAction.Default);
			xMLPictureShapeModel.RelId = relationship.RelationshipId;
			return xMLPictureShapeModel;
		}
	}
}
