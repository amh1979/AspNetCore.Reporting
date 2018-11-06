using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.main;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.spreadsheetDrawing;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Rels.Relationships;
using System.IO.Packaging;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLPictureShapeModel : XMLShapeModel, IPictureShapeModel, IShapeModel
	{
		private Picture _interface;

		private XmlPart _parent;

		public Picture Interface
		{
			get
			{
				if (this._interface == null)
				{
					this._interface = new Picture(this);
				}
				return this._interface;
			}
		}

		public string RelId
		{
			set
			{
				this.BlipFill.Blip.Embed_Attr = value;
			}
		}

		public override string Hyperlink
		{
			set
			{
				Relationship relationship = base.Manager.AddExternalPartToTree("http://schemas.openxmlformats.org/officeDocument/2006/relationships/hyperlink", value, this._parent, (TargetMode)(value.Contains("://") ? 1 : 0));
				this.NonVisualDrawingProps.HlinkClick = new CT_Hyperlink();
				this.NonVisualDrawingProps.HlinkClick.Id_Attr = relationship.RelationshipId;
				this.NonVisualDrawingProps.Descr_Attr = "Hyperlink";
			}
		}

		private CT_Picture Picture
		{
			get
			{
				if (base.TwoCellAnchor.Pic == null)
				{
					base.TwoCellAnchor.Pic = new CT_Picture();
				}
				return base.TwoCellAnchor.Pic;
			}
		}

		private CT_PictureNonVisual NonVisualProperties
		{
			get
			{
				if (this.Picture.NvPicPr == null)
				{
					this.Picture.NvPicPr = new CT_PictureNonVisual();
				}
				return this.Picture.NvPicPr;
			}
		}

		private CT_NonVisualDrawingProps NonVisualDrawingProps
		{
			get
			{
				if (this.NonVisualProperties.CNvPr == null)
				{
					this.NonVisualProperties.CNvPr = new CT_NonVisualDrawingProps();
				}
				return this.NonVisualProperties.CNvPr;
			}
		}

		private CT_BlipFillProperties BlipFill
		{
			get
			{
				if (this.Picture.BlipFill == null)
				{
					this.Picture.BlipFill = new CT_BlipFillProperties();
				}
				if (this.Picture.BlipFill.Blip == null)
				{
					this.Picture.BlipFill.Blip = new CT_Blip();
				}
				return this.Picture.BlipFill;
			}
		}

		public XMLPictureShapeModel(PartManager manager, WsDrPart part, XmlPart parent, AnchorModel startAnchor, AnchorModel endAnchor, uint uniqueId)
			: base(manager, part, startAnchor, endAnchor)
		{
			this._parent = parent;
			base.TwoCellAnchor.Choice_0 = CT_TwoCellAnchor.ChoiceBucket_0.pic;
			base.TwoCellAnchor.ClientData = new CT_AnchorClientData();
			this.Picture.NvPicPr = new CT_PictureNonVisual();
			this.Picture.NvPicPr.CNvPicPr = new CT_NonVisualPictureProperties();
			this.Picture.NvPicPr.CNvPr = new CT_NonVisualDrawingProps();
			this.Picture.NvPicPr.CNvPr.Id_Attr = uniqueId;
			this.Picture.NvPicPr.CNvPr.Name_Attr = "Picture " + uniqueId;
			this.Picture.BlipFill = new CT_BlipFillProperties();
			this.Picture.BlipFill.Blip = new CT_Blip();
			this.Picture.BlipFill.Blip.Cstate_Attr = ST_BlipCompression.print;
			this.Picture.BlipFill.Choice_0 = CT_BlipFillProperties.ChoiceBucket_0.stretch;
			this.Picture.BlipFill.Stretch = new CT_StretchInfoProperties();
			this.Picture.BlipFill.Stretch.FillRect = new CT_RelativeRect();
			this.Picture.SpPr.Choice_0 = CT_ShapeProperties.ChoiceBucket_0.prstGeom;
			this.Picture.SpPr.PrstGeom = new CT_PresetGeometry2D();
			this.Picture.SpPr.PrstGeom.Prst_Attr = ST_ShapeType.rect;
			this.Picture.SpPr.PrstGeom.AvLst = new CT_GeomGuideList();
		}
	}
}
