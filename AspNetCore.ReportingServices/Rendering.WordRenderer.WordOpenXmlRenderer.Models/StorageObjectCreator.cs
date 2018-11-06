using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class StorageObjectCreator : IScalabilityObjectCreator
	{
		private static StorageObjectCreator _instance = null;

		private static List<Declaration> _declarations = StorageObjectCreator.BuildDeclarations();

		internal static StorageObjectCreator Instance
		{
			get
			{
				if (StorageObjectCreator._instance == null)
				{
					StorageObjectCreator._instance = new StorageObjectCreator();
				}
				return StorageObjectCreator._instance;
			}
		}

		private StorageObjectCreator()
		{
		}

		public bool TryCreateObject(ObjectType objectType, out IPersistable persistObj)
		{
			switch (objectType)
			{
			case ObjectType.WordOpenXmlTableGrid:
				persistObj = new OpenXmlTableGridModel();
				break;
			case ObjectType.WordOpenXmlTableRowProperties:
				persistObj = new OpenXmlTableRowPropertiesModel();
				break;
			case ObjectType.WordOpenXmlBorderProperties:
				persistObj = new OpenXmlBorderPropertiesModel();
				break;
			case ObjectType.WordOpenXmlHeaderFooterReferences:
				persistObj = new HeaderFooterReferences();
				break;
			default:
				persistObj = null;
				return false;
			}
			return true;
		}

		public List<Declaration> GetDeclarations()
		{
			return StorageObjectCreator._declarations;
		}

		private static List<Declaration> BuildDeclarations()
		{
			List<Declaration> list = new List<Declaration>(6);
			list.Add(BaseInterleaver.GetDeclaration());
			list.Add(OpenXmlTableGridModel.GetDeclaration());
			list.Add(OpenXmlTableRowPropertiesModel.GetDeclaration());
			list.Add(OpenXmlBorderPropertiesModel.GetDeclaration());
			list.Add(HeaderFooterReferences.GetDeclaration());
			return list;
		}
	}
}
