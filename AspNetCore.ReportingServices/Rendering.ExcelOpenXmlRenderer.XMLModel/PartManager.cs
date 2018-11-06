using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.officeDocument.x2006.docPropsVTypes;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.officeDocument.x2006.extended_properties;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Rels.Archive;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Rels.Relationships;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class PartManager
	{
		private WorkbookPart _workbook;

		private XMLWorkbookModel _workbookmodel;

		private StyleManager _stylesheet;

		private OPCRelationshipTree _relationshipTree;

		internal WorkbookPart Workbook
		{
			get
			{
				return this._workbook;
			}
		}

		internal StyleManager StyleSheet
		{
			get
			{
				return this._stylesheet;
			}
		}

		public PartManager(XMLWorkbookModel workbookModel)
		{
			this._workbookmodel = workbookModel;
			this._relationshipTree = new OPCRelationshipTree("http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument", ((IStreambookModel)this._workbookmodel).ZipPackage);
			WorkbookPart workbookPart = new WorkbookPart();
			CT_Workbook cT_Workbook = (CT_Workbook)workbookPart.Root;
			cT_Workbook.FileVersion = new CT_FileVersion();
			cT_Workbook.FileVersion.AppName_Attr = "xl";
			cT_Workbook.FileVersion.LastEdited_Attr = "4";
			cT_Workbook.FileVersion.LowestEdited_Attr = "4";
			cT_Workbook.FileVersion.RupBuild_Attr = "4506";
			cT_Workbook.WorkbookPr = new CT_WorkbookPr();
			cT_Workbook.WorkbookPr.DefaultThemeVersion_Attr = 124226u;
			cT_Workbook.BookViews = new CT_BookViews();
			CT_BookView item = new CT_BookView
			{
				XWindow_Attr = 240,
				YWindow_Attr = 120,
				WindowWidth_Attr = 18060u,
				WindowHeight_Attr = 7050u
			};
			cT_Workbook.BookViews.WorkbookView.Add(item);
			cT_Workbook.CalcPr = new CT_CalcPr();
			cT_Workbook.CalcPr.CalcId_Attr = 125725u;
			Relationship relationship = this._relationshipTree.AddRootPartToTree(workbookPart, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument", "xl/workbook.xml");
			this._workbook = workbookPart;
			StyleSheetPart styleSheetPart = new StyleSheetPart();
			CT_Stylesheet cT_Stylesheet = (CT_Stylesheet)styleSheetPart.Root;
			CT_Font cT_Font = new CT_Font();
			cT_Font.Sz = new CT_FontSize();
			cT_Font.Sz.Val_Attr = 11.0;
			cT_Font.Color = new CT_Color();
			cT_Font.Color.Rgb_Attr = "FF000000";
			cT_Font.Name = new CT_FontName();
			cT_Font.Name.Val_Attr = "Calibri";
			cT_Font.Family = new CT_IntProperty();
			cT_Font.Family.Val_Attr = 2;
			cT_Font.Scheme = new CT_FontScheme();
			cT_Font.Scheme.Val_Attr = ST_FontScheme.minor;
			cT_Stylesheet.Fonts = new CT_Fonts();
			cT_Stylesheet.Fonts.Font.Add(cT_Font);
			cT_Stylesheet.Fonts.Count_Attr = 1u;
			CT_Fill cT_Fill = new CT_Fill();
			cT_Fill.PatternFill = new CT_PatternFill();
			cT_Fill.PatternFill.PatternType_Attr = ST_PatternType.none;
			CT_Fill cT_Fill2 = new CT_Fill();
			cT_Fill2.PatternFill = new CT_PatternFill();
			cT_Fill2.PatternFill.PatternType_Attr = ST_PatternType.gray125;
			cT_Stylesheet.Fills = new CT_Fills();
			cT_Stylesheet.Fills.Fill.Add(cT_Fill);
			cT_Stylesheet.Fills.Fill.Add(cT_Fill2);
			cT_Stylesheet.Fills.Count_Attr = 2u;
			CT_Border item2 = new CT_Border
			{
				Left = new CT_BorderPr(),
				Right = new CT_BorderPr(),
				Top = new CT_BorderPr(),
				Bottom = new CT_BorderPr(),
				Diagonal = new CT_BorderPr()
			};
			cT_Stylesheet.Borders = new CT_Borders();
			cT_Stylesheet.Borders.Border.Add(item2);
			cT_Stylesheet.Borders.Count_Attr = 1u;
			CT_Xf item3 = new CT_Xf
			{
				NumFmtId_Attr = 0u,
				FontId_Attr = 0u,
				FillId_Attr = 0u,
				BorderId_Attr = 0u
			};
			cT_Stylesheet.CellStyleXfs = new CT_CellStyleXfs();
			cT_Stylesheet.CellStyleXfs.Xf.Add(item3);
			cT_Stylesheet.CellXfs = new CT_CellXfs();
			cT_Stylesheet.CellXfs.Xf.Add(StyleManager.CreateDefaultXf());
			CT_CellStyle item4 = new CT_CellStyle
			{
				Name_Attr = "Normal",
				XfId_Attr = 0u,
				BuiltinId_Attr = 0u
			};
			cT_Stylesheet.CellStyles = new CT_CellStyles();
			cT_Stylesheet.CellStyles.CellStyle.Add(item4);
			cT_Stylesheet.Dxfs = new CT_Dxfs();
			cT_Stylesheet.Dxfs.Count_Attr = 0u;
			cT_Stylesheet.TableStyles = new CT_TableStyles();
			cT_Stylesheet.TableStyles.Count_Attr = 0u;
			cT_Stylesheet.TableStyles.DefaultTableStyle_Attr = "TableStyleMedium9";
			cT_Stylesheet.TableStyles.DefaultPivotStyle_Attr = "PivotStyleLight16";
			this._relationshipTree.AddPartToTree(styleSheetPart, "application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles", "xl/styles.xml", (XmlPart)this._relationshipTree.GetPartByLocation(relationship.RelatedPart));
			this._stylesheet = new StyleManager(styleSheetPart);
			OpcCorePropertiesPart part = new OpcCorePropertiesPart();
			this._relationshipTree.AddRootPartToTree(part, "application/vnd.openxmlformats-package.core-properties+xml", "http://schemas.openxmlformats.org/package/2006/relationships/meatadata/core-properties", "docProps/core.xml");
			PropertiesPart propertiesPart = new PropertiesPart();
			CT_Properties cT_Properties = (CT_Properties)propertiesPart.Root;
			cT_Properties.Application = "Microsoft Excel";
			cT_Properties.DocSecurity = 0;
			cT_Properties.ScaleCrop = false;
			cT_Properties.HeadingPairs = new CT_VectorVariant();
			cT_Properties.HeadingPairs.Vector = new CT_Vector();
			cT_Properties.HeadingPairs.Vector.Size_Attr = 2u;
			cT_Properties.HeadingPairs.Vector.BaseType_Attr = ST_VectorBaseType.variant;
			CT_Variant item5 = new CT_Variant
			{
				Choice_0 = CT_Variant.ChoiceBucket_0.lpstr,
				Lpstr = "Worksheets"
			};
			CT_Variant item6 = new CT_Variant
			{
				Choice_0 = CT_Variant.ChoiceBucket_0.i4,
				I4 = 1
			};
			cT_Properties.HeadingPairs.Vector.Variant.Add(item5);
			cT_Properties.HeadingPairs.Vector.Variant.Add(item6);
			cT_Properties.TitlesOfParts = new CT_VectorLpstr();
			cT_Properties.TitlesOfParts.Vector = new CT_Vector();
			cT_Properties.TitlesOfParts.Vector.Size_Attr = 0u;
			cT_Properties.TitlesOfParts.Vector.BaseType_Attr = ST_VectorBaseType.lpstr;
			cT_Properties.LinksUpToDate = false;
			cT_Properties.SharedDoc = false;
			cT_Properties.HyperlinksChanged = false;
			cT_Properties.AppVersion = "12.0000";
			this._relationshipTree.AddRootPartToTree(propertiesPart, "application/vnd.openxmlformats-officedocument.extended-properties+xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties", "docProps/app.xml");
		}

		public void Write()
		{
			try
			{
				this.WriteCommon();
			}
			finally
			{
				Package zipPackage = ((IStreambookModel)this._workbookmodel).ZipPackage;
				zipPackage.Flush();
				zipPackage.Close();
			}
		}

		private void WriteCommon()
		{
			this._workbookmodel.Cleanup();
			if (this._stylesheet != null)
			{
				this._stylesheet.Cleanup();
			}
			this._relationshipTree.WriteTree();
		}

		public Relationship AddPartToTree(OoxmlPart part, string contentType, string relationshipType, string locationHint, XmlPart parent)
		{
			return this._relationshipTree.AddPartToTree(part, contentType, relationshipType, locationHint, parent);
		}

		public Relationship AddStreamingPartToTree(string contentType, string relationshipType, string locationHint, XmlPart parent)
		{
			return this._relationshipTree.AddStreamingPartToTree(contentType, relationshipType, locationHint, parent);
		}

		public Relationship AddExternalPartToTree(string relationshipType, string externalTarget, XmlPart parent, TargetMode targetMode)
		{
			return this._relationshipTree.AddExternalPartToTree(relationshipType, externalTarget, parent, targetMode);
		}

		public Relationship AddImageToTree(string uniqueId, Stream data, string extension, string relationshipType, string locationHint, string parentLocation, ContentTypeAction ctypeAction)
		{
			bool flag = default(bool);
			Relationship relationship = this._relationshipTree.AddImageToTree(uniqueId, extension, relationshipType, locationHint, parentLocation, ctypeAction, out flag);
			if (flag)
			{
				Package zipPackage = ((IStreambookModel)this._workbookmodel).ZipPackage;
				PackagePart part = zipPackage.GetPart(new Uri(Utils.CleanName(relationship.RelatedPart), UriKind.Relative));
				Stream stream = part.GetStream();
				SupportClass.CopyStream(data, stream);
			}
			return relationship;
		}

		public RelPart GetPartByContentType(string contenttype)
		{
			return this._relationshipTree.GetPartByContentType(contenttype);
		}

		public List<Relationship> GetRelationshipsForSheet(CT_Sheet sheetEntry, string relationshipType)
		{
			List<Relationship> list = new List<Relationship>();
			foreach (Relationship workbookRelationship in this.GetWorkbookRelationships())
			{
				if (workbookRelationship.RelationshipId == sheetEntry.Id_Attr)
				{
					List<Relationship> relationshipsByPath = this._relationshipTree.GetRelationshipsByPath(workbookRelationship.RelatedPart);
					foreach (Relationship item in relationshipsByPath)
					{
						if (item.RelationshipType == relationshipType)
						{
							list.Add(item);
						}
					}
				}
			}
			return list;
		}

		public XmlPart GetWorksheetXmlPart(CT_Sheet sheetentry)
		{
			foreach (Relationship workbookRelationship in this.GetWorkbookRelationships())
			{
				if (workbookRelationship.RelationshipId == sheetentry.Id_Attr)
				{
					return (XmlPart)this._relationshipTree.GetPartByLocation(workbookRelationship.RelatedPart);
				}
			}
			throw new FatalException();
		}

		public XmlPart GetPartByLocation(string location)
		{
			return (XmlPart)this._relationshipTree.GetPartByLocation(location);
		}

		private List<Relationship> GetWorkbookRelationships()
		{
			string[] array = new string[2]
			{
				"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml",
				"application/vnd.ms-excel.sheet.macroEnabled.main+xml"
			};
			string[] array2 = array;
			foreach (string contenttype in array2)
			{
				RelPart partByContentType = this._relationshipTree.GetPartByContentType(contenttype);
				if (partByContentType != null)
				{
					return this._relationshipTree.GetRelationshipsByPath(partByContentType.Location);
				}
			}
			throw new FatalException();
		}
	}
}
