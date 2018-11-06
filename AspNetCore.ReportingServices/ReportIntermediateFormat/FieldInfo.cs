using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal class FieldInfo : IPersistable
	{
		private List<int> m_propertyReaderIndices;

		private List<string> m_propertyNames;

		[NonSerialized]
		internal bool ErrorRegistered;

		[NonSerialized]
		internal bool Missing;

		[NonSerialized]
		private readonly bool[] m_propertyErrorRegistered;

		[NonSerialized]
		private static readonly Declaration m_declaration = FieldInfo.GetDeclaration();

		internal int PropertyCount
		{
			get
			{
				if (this.PropertyReaderIndices == null)
				{
					return 0;
				}
				return this.PropertyReaderIndices.Count;
			}
		}

		internal List<int> PropertyReaderIndices
		{
			get
			{
				return this.m_propertyReaderIndices;
			}
		}

		internal List<string> PropertyNames
		{
			get
			{
				return this.m_propertyNames;
			}
		}

		internal FieldInfo()
		{
			this.m_propertyErrorRegistered = new bool[0];
		}

		internal FieldInfo(List<int> aPropIndices, List<string> aPropNames)
		{
			this.m_propertyReaderIndices = aPropIndices;
			this.m_propertyNames = aPropNames;
			this.m_propertyErrorRegistered = new bool[aPropIndices.Count];
		}

		internal bool IsPropertyErrorRegistered(int aIndex)
		{
			if (this.m_propertyErrorRegistered == null)
			{
				return false;
			}
			return this.m_propertyErrorRegistered[aIndex];
		}

		internal void SetPropertyErrorRegistered(int aIndex)
		{
			this.m_propertyErrorRegistered[aIndex] = true;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.FieldPropertyNames, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.String));
			list.Add(new MemberInfo(MemberName.FieldPropertyReaderIndices, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.FieldInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(FieldInfo.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.FieldPropertyNames:
					writer.WriteListOfPrimitives(this.m_propertyNames);
					break;
				case MemberName.FieldPropertyReaderIndices:
					writer.WriteListOfPrimitives(this.m_propertyReaderIndices);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(FieldInfo.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.FieldPropertyNames:
					this.m_propertyNames = reader.ReadListOfPrimitives<string>();
					break;
				case MemberName.FieldPropertyReaderIndices:
					this.m_propertyReaderIndices = reader.ReadListOfPrimitives<int>();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public virtual AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.FieldInfo;
		}
	}
}
