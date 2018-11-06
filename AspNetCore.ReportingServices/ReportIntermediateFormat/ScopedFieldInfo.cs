using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class ScopedFieldInfo : IPersistable
	{
		private int m_fieldIndex;

		[NonSerialized]
		private string m_fieldName;

		private static Declaration m_declaration = ScopedFieldInfo.GetDeclaration();

		public int FieldIndex
		{
			get
			{
				return this.m_fieldIndex;
			}
			set
			{
				this.m_fieldIndex = value;
			}
		}

		public string FieldName
		{
			get
			{
				return this.m_fieldName;
			}
			set
			{
				this.m_fieldName = value;
			}
		}

		public static Declaration GetDeclaration()
		{
			if (ScopedFieldInfo.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.FieldIndex, Token.Int32, Lifetime.AddedIn(200)));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScopedFieldInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return ScopedFieldInfo.m_declaration;
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ScopedFieldInfo.m_declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.FieldIndex)
				{
					writer.Write(this.m_fieldIndex);
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ScopedFieldInfo.m_declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.FieldIndex)
				{
					this.m_fieldIndex = reader.ReadInt32();
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScopedFieldInfo;
		}
	}
}
