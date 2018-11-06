using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal class StorageObjectCreator : IScalabilityObjectCreator
	{
		private static StorageObjectCreator m_instance = null;

		private static List<Declaration> m_declarations = StorageObjectCreator.BuildDeclarations();

		internal static StorageObjectCreator Instance
		{
			get
			{
				if (StorageObjectCreator.m_instance == null)
				{
					StorageObjectCreator.m_instance = new StorageObjectCreator();
				}
				return StorageObjectCreator.m_instance;
			}
		}

		private StorageObjectCreator()
		{
		}

		public bool TryCreateObject(ObjectType objectType, out IPersistable persistObj)
		{
			switch (objectType)
			{
			case ObjectType.ItemSizes:
				persistObj = new ItemSizes();
				break;
			case ObjectType.PageBreakProperties:
				persistObj = new PageBreakProperties();
				break;
			case ObjectType.HiddenPageItem:
				persistObj = new HiddenPageItem();
				break;
			case ObjectType.NoRowsItem:
				persistObj = new NoRowsItem();
				break;
			case ObjectType.SubReport:
				persistObj = new SubReport();
				break;
			case ObjectType.ReportBody:
				persistObj = new ReportBody();
				break;
			case ObjectType.Rectangle:
				persistObj = new Rectangle();
				break;
			case ObjectType.TextBox:
				persistObj = new TextBox();
				break;
			case ObjectType.Paragraph:
				persistObj = new Paragraph();
				break;
			case ObjectType.TextRun:
				persistObj = new TextRun();
				break;
			case ObjectType.TextBoxOffset:
				persistObj = new TextBox.TextBoxOffset();
				break;
			case ObjectType.Line:
				persistObj = new Line();
				break;
			case ObjectType.Chart:
				persistObj = new Chart();
				break;
			case ObjectType.GaugePanel:
				persistObj = new GaugePanel();
				break;
			case ObjectType.Map:
				persistObj = new Map();
				break;
			case ObjectType.Image:
				persistObj = new Image();
				break;
			case ObjectType.Tablix:
				persistObj = new Tablix();
				break;
			case ObjectType.RowInfo:
				persistObj = new Tablix.RowInfo();
				break;
			case ObjectType.SizeInfo:
				persistObj = new Tablix.SizeInfo();
				break;
			case ObjectType.ColumnInfo:
				persistObj = new Tablix.ColumnInfo();
				break;
			case ObjectType.PageDetailCell:
				persistObj = new Tablix.PageDetailCell();
				break;
			case ObjectType.PageCornerCell:
				persistObj = new Tablix.PageCornerCell();
				break;
			case ObjectType.PageMemberCell:
				persistObj = new Tablix.PageMemberCell();
				break;
			case ObjectType.PageStructStaticMemberCell:
				persistObj = new Tablix.PageStructStaticMemberCell();
				break;
			case ObjectType.PageStructDynamicMemberCell:
				persistObj = new Tablix.PageStructDynamicMemberCell();
				break;
			default:
				persistObj = null;
				return false;
			}
			return true;
		}

		public List<Declaration> GetDeclarations()
		{
			return StorageObjectCreator.m_declarations;
		}

		private static List<Declaration> BuildDeclarations()
		{
			List<Declaration> list = new List<Declaration>(30);
			list.Add(PageItem.GetDeclaration());
			list.Add(PageItemContainer.GetDeclaration());
			list.Add(ItemSizes.GetDeclaration());
			list.Add(HiddenPageItem.GetDeclaration());
			list.Add(NoRowsItem.GetDeclaration());
			list.Add(SubReport.GetDeclaration());
			list.Add(ReportBody.GetDeclaration());
			list.Add(Rectangle.GetDeclaration());
			list.Add(TextBox.GetDeclaration());
			list.Add(TextBox.TextBoxOffset.GetDeclaration());
			list.Add(Paragraph.GetDeclaration());
			list.Add(TextRun.GetDeclaration());
			list.Add(Line.GetDeclaration());
			list.Add(DynamicImage.GetDeclaration());
			list.Add(Chart.GetDeclaration());
			list.Add(GaugePanel.GetDeclaration());
			list.Add(Image.GetDeclaration());
			list.Add(Tablix.GetDeclaration());
			list.Add(Tablix.RowInfo.GetDeclaration());
			list.Add(Tablix.SizeInfo.GetDeclaration());
			list.Add(Tablix.ColumnInfo.GetDeclaration());
			list.Add(Tablix.PageTalixCell.GetDeclaration());
			list.Add(Tablix.PageDetailCell.GetDeclaration());
			list.Add(Tablix.PageCornerCell.GetDeclaration());
			list.Add(Tablix.PageMemberCell.GetDeclaration());
			list.Add(Tablix.PageStructMemberCell.GetDeclaration());
			list.Add(Tablix.PageStructStaticMemberCell.GetDeclaration());
			list.Add(Tablix.PageStructDynamicMemberCell.GetDeclaration());
			list.Add(Map.GetDeclaration());
			list.Add(PageBreakProperties.GetDeclaration());
			return list;
		}
	}
}
