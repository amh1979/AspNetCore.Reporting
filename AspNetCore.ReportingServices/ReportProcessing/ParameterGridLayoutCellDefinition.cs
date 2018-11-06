using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ParameterGridLayoutCellDefinition : IPersistable
	{
		public int RowIndex;

		public int ColumnIndex;

		public string ParameterName;

		private static Declaration m_Declaration = ParameterGridLayoutCellDefinition.GetDeclaration();

		public void WriteXml(XmlTextWriter resultXml)
		{
			resultXml.WriteStartElement("CellDefinition");
			resultXml.WriteElementString("ColumnIndex", this.ColumnIndex.ToString(CultureInfo.InvariantCulture));
			resultXml.WriteElementString("RowIndex", this.RowIndex.ToString(CultureInfo.InvariantCulture));
			resultXml.WriteElementString("ParameterName", this.ParameterName);
			resultXml.WriteEndElement();
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ParameterCellColumnIndex, Token.Int32, Lifetime.AddedIn(300)));
			list.Add(new MemberInfo(MemberName.ParameterCellRowIndex, Token.Int32, Lifetime.AddedIn(300)));
			list.Add(new MemberInfo(MemberName.ParameterName, Token.String, Lifetime.AddedIn(300)));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterGridLayoutCellDefinition, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ParameterGridLayoutCellDefinition.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ParameterCellColumnIndex:
					writer.Write(this.ColumnIndex);
					break;
				case MemberName.ParameterCellRowIndex:
					writer.Write(this.RowIndex);
					break;
				case MemberName.ParameterName:
					writer.Write(this.ParameterName);
					break;
				default:
					Global.Tracer.Assert(false, "Unexpected RIF Member for ParametersGridCellDefinition");
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ParameterGridLayoutCellDefinition.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ParameterCellColumnIndex:
					this.ColumnIndex = reader.ReadInt32();
					break;
				case MemberName.ParameterCellRowIndex:
					this.RowIndex = reader.ReadInt32();
					break;
				case MemberName.ParameterName:
					this.ParameterName = reader.ReadString();
					break;
				default:
					Global.Tracer.Assert(false, "Unexpected RIF Member for ParametersGridCellDefinition");
					break;
				}
			}
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterGridLayoutCellDefinition;
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			throw new NotImplementedException();
		}
	}
}
