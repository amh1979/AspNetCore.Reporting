using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class DocumentMapBeginContainer : IPersistable
	{
		[NonSerialized]
		private static readonly DocumentMapBeginContainer m_instance = new DocumentMapBeginContainer();

		[NonSerialized]
		private static readonly Declaration m_Declaration = DocumentMapBeginContainer.GetDeclaration();

		internal static DocumentMapBeginContainer Instance
		{
			get
			{
				return DocumentMapBeginContainer.m_instance;
			}
		}

		private DocumentMapBeginContainer()
		{
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> memberInfoList = new List<MemberInfo>();
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DocumentMapBeginContainer, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, memberInfoList);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(DocumentMapBeginContainer.m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				Global.Tracer.Assert(false);
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(DocumentMapBeginContainer.m_Declaration);
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DocumentMapBeginContainer;
		}
	}
}
