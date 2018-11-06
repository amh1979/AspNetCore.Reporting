using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.Layout;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.SPBIFReader.Callbacks;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer
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
			case ObjectType.ExcelRowInfo:
				persistObj = new LayoutEngine.RowInfo();
				break;
			case ObjectType.RowItemStruct:
				persistObj = new LayoutEngine.RowItemStruct();
				break;
			case ObjectType.TablixStruct:
				persistObj = new LayoutEngine.TablixStruct();
				break;
			case ObjectType.TablixMemberStruct:
				persistObj = new LayoutEngine.TablixMemberStruct();
				break;
			case ObjectType.ToggleParent:
				persistObj = new ToggleParent();
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
			List<Declaration> list = new List<Declaration>(6);
			list.Add(LayoutEngine.RowInfo.GetDeclaration());
			list.Add(LayoutEngine.RowItemStruct.GetDeclaration());
			list.Add(LayoutEngine.TablixItemStruct.GetDeclaration());
			list.Add(LayoutEngine.TablixStruct.GetDeclaration());
			list.Add(LayoutEngine.TablixMemberStruct.GetDeclaration());
			list.Add(ToggleParent.GetDeclaration());
			return list;
		}
	}
}
