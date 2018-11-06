using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class DocumentMapEndContainer : IPersistable
	{
		[NonSerialized]
		private static readonly DocumentMapEndContainer m_instance = new DocumentMapEndContainer();

		[NonSerialized]
		private static readonly Declaration m_Declaration = DocumentMapEndContainer.GetDeclaration();

		internal static DocumentMapEndContainer Instance
		{
			get
			{
				return DocumentMapEndContainer.m_instance;
			}
		}

		private DocumentMapEndContainer()
		{
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> memberInfoList = new List<MemberInfo>();
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DocumentMapEndContainer, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, memberInfoList);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(DocumentMapEndContainer.m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				Global.Tracer.Assert(false);
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(DocumentMapEndContainer.m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				Global.Tracer.Assert(false);
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DocumentMapEndContainer;
		}
	}
}
