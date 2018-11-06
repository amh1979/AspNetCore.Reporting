using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
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
			case ObjectType.RowMemberInfo:
				persistObj = new Tablix.RowMemberInfo();
				break;
			case ObjectType.SizeInfo:
				persistObj = new Tablix.SizeInfo();
				break;
			case ObjectType.DetailCell:
				persistObj = new Tablix.DetailCell();
				break;
			case ObjectType.CornerCell:
				persistObj = new Tablix.CornerCell();
				break;
			case ObjectType.MemberCell:
				persistObj = new Tablix.MemberCell();
				break;
			case ObjectType.StreamMemberCell:
				persistObj = new Tablix.StreamMemberCell();
				break;
			case ObjectType.RPLMemberCell:
				persistObj = new Tablix.RPLMemberCell();
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
			List<Declaration> list = new List<Declaration>(8);
			list.Add(Tablix.RowMemberInfo.GetDeclaration());
			list.Add(Tablix.SizeInfo.GetDeclaration());
			list.Add(Tablix.DetailCell.GetDeclaration());
			list.Add(Tablix.CornerCell.GetDeclaration());
			list.Add(Tablix.MemberCell.GetDeclaration());
			list.Add(Tablix.PageMemberCell.GetDeclaration());
			list.Add(Tablix.StreamMemberCell.GetDeclaration());
			list.Add(Tablix.RPLMemberCell.GetDeclaration());
			return list;
		}
	}
}
