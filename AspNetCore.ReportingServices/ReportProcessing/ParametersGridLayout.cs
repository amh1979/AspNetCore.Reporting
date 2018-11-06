using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ParametersGridLayout : IPersistable
	{
		public int NumberOfColumns;

		public int NumberOfRows;

		public ParametersGridCellDefinitionList CellDefinitions;

		private static Declaration m_Declaration = ParametersGridLayout.GetDeclaration();

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ParametersLayoutNumberOfColumns, Token.Int32, Lifetime.AddedIn(300)));
			list.Add(new MemberInfo(MemberName.ParametersLayoutNumberOfRows, Token.Int32, Lifetime.AddedIn(300)));
			list.Add(new MemberInfo(MemberName.ParametersLayoutCellDefinitions, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterGridLayoutCellDefinition, Lifetime.AddedIn(300)));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParametersLayout, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ParametersGridLayout.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ParametersLayoutNumberOfColumns:
					writer.Write(this.NumberOfColumns);
					break;
				case MemberName.ParametersLayoutNumberOfRows:
					writer.Write(this.NumberOfRows);
					break;
				case MemberName.ParametersLayoutCellDefinitions:
					writer.Write(this.CellDefinitions);
					break;
				default:
					Global.Tracer.Assert(false, "Unexpected RIF Member for ParametersGridLayout");
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ParametersGridLayout.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ParametersLayoutNumberOfColumns:
					this.NumberOfColumns = reader.ReadInt32();
					break;
				case MemberName.ParametersLayoutNumberOfRows:
					this.NumberOfRows = reader.ReadInt32();
					break;
				case MemberName.ParametersLayoutCellDefinitions:
					this.CellDefinitions = reader.ReadListOfRIFObjects<ParametersGridCellDefinitionList>();
					break;
				default:
					Global.Tracer.Assert(false, "Unexpected RIF Member for ParametersGridLayout");
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			throw new NotImplementedException();
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParametersLayout;
		}
	}
}
