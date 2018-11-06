using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class CommonObjectCreator : IScalabilityObjectCreator
	{
		private static List<Declaration> m_declarations = CommonObjectCreator.BuildDeclarations();

		private static CommonObjectCreator m_instance = null;

		internal static CommonObjectCreator Instance
		{
			get
			{
				if (CommonObjectCreator.m_instance == null)
				{
					CommonObjectCreator.m_instance = new CommonObjectCreator();
				}
				return CommonObjectCreator.m_instance;
			}
		}

		private CommonObjectCreator()
		{
		}

		public bool TryCreateObject(ObjectType objectType, out IPersistable persistObj)
		{
			switch (objectType)
			{
			case ObjectType.StorageItem:
				persistObj = new StorageItem();
				break;
			case ObjectType.ScalableDictionaryNode:
				persistObj = new ScalableDictionaryNode();
				break;
			case ObjectType.ScalableDictionaryValues:
				persistObj = new ScalableDictionaryValues();
				break;
			case ObjectType.StorableArray:
				persistObj = new StorableArray();
				break;
			case ObjectType.ScalableHybridListEntry:
				persistObj = new ScalableHybridListEntry();
				break;
			default:
				persistObj = null;
				return false;
			}
			return true;
		}

		public List<Declaration> GetDeclarations()
		{
			return CommonObjectCreator.m_declarations;
		}

		private static List<Declaration> BuildDeclarations()
		{
			List<Declaration> list = new List<Declaration>(8);
			list.Add(BaseReference.GetDeclaration());
			list.Add(ScalableList<StorageItem>.GetDeclaration());
			list.Add(ScalableDictionary<int, StorageItem>.GetDeclaration());
			list.Add(ScalableDictionaryNode.GetDeclaration());
			list.Add(ScalableDictionaryValues.GetDeclaration());
			list.Add(StorageItem.GetDeclaration());
			list.Add(StorableArray.GetDeclaration());
			list.Add(ScalableHybridListEntry.GetDeclaration());
			return list;
		}
	}
}
