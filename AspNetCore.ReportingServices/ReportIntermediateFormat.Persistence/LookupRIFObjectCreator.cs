using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal struct LookupRIFObjectCreator : IRIFObjectCreator, IScalabilityObjectCreator
	{
		private static List<Declaration> m_declarations = LookupRIFObjectCreator.BuildDeclarations();

		public IPersistable CreateRIFObject(ObjectType objectType, ref IntermediateFormatReader context)
		{
			IPersistable persistable = null;
			if (objectType == ObjectType.Null)
			{
				return null;
			}
			Global.Tracer.Assert(this.TryCreateObject(objectType, out persistable));
			persistable.Deserialize(context);
			return persistable;
		}

		public bool TryCreateObject(ObjectType objectType, out IPersistable persistObj)
		{
			switch (objectType)
			{
			case ObjectType.LookupMatches:
				persistObj = new LookupMatches();
				break;
			case ObjectType.LookupMatchesWithRows:
				persistObj = new LookupMatchesWithRows();
				break;
			case ObjectType.LookupTable:
				persistObj = new LookupTable();
				break;
			case ObjectType.IntermediateFormatVersion:
				persistObj = new IntermediateFormatVersion();
				break;
			default:
				persistObj = null;
				return false;
			}
			return true;
		}

		public List<Declaration> GetDeclarations()
		{
			return LookupRIFObjectCreator.m_declarations;
		}

		private static List<Declaration> BuildDeclarations()
		{
			List<Declaration> list = new List<Declaration>(3);
			list.Add(LookupMatches.GetDeclaration());
			list.Add(LookupMatchesWithRows.GetDeclaration());
			list.Add(LookupTable.GetDeclaration());
			return list;
		}
	}
}
