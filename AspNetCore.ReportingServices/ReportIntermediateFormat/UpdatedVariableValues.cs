using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class UpdatedVariableValues : IPersistable
	{
		private Dictionary<int, object> m_variableValues;

		[NonSerialized]
		private static readonly Declaration m_Declaration = UpdatedVariableValues.GetDeclaration();

		internal Dictionary<int, object> VariableValues
		{
			get
			{
				return this.m_variableValues;
			}
			set
			{
				this.m_variableValues = value;
			}
		}

		internal UpdatedVariableValues()
		{
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.UpdatedVariableValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Int32SerializableDictionary, Token.Serializable));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.UpdatedVariableValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(UpdatedVariableValues.m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.UpdatedVariableValues)
				{
					writer.Int32SerializableDictionary(this.m_variableValues);
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(UpdatedVariableValues.m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.UpdatedVariableValues)
				{
					this.m_variableValues = reader.Int32SerializableDictionary();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.UpdatedVariableValues;
		}
	}
}
