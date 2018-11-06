using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal class ExpressionInfoTypeValuePair : IPersistable
	{
		private DataType m_constantDataType;

		private ExpressionInfo m_value;

		[NonSerialized]
		private bool m_hadExplicitDataType;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ExpressionInfoTypeValuePair.GetDeclaration();

		internal DataType DataType
		{
			get
			{
				return this.m_constantDataType;
			}
		}

		internal ExpressionInfo Value
		{
			get
			{
				return this.m_value;
			}
		}

		internal bool HadExplicitDataType
		{
			get
			{
				return this.m_hadExplicitDataType;
			}
		}

		internal ExpressionInfoTypeValuePair(DataType constantType, bool hadExplicitDataType, ExpressionInfo value)
		{
			this.m_constantDataType = constantType;
			this.m_hadExplicitDataType = hadExplicitDataType;
			this.m_value = value;
		}

		internal ExpressionInfoTypeValuePair()
		{
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.DataType, Token.Enum));
			list.Add(new MemberInfo(MemberName.Value, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfoTypeValuePair, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ExpressionInfoTypeValuePair.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.DataType:
					writer.WriteEnum((int)this.m_constantDataType);
					break;
				case MemberName.Value:
					writer.Write(this.m_value);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ExpressionInfoTypeValuePair.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.DataType:
					this.m_constantDataType = (DataType)reader.ReadEnum();
					break;
				case MemberName.Value:
					this.m_value = (ExpressionInfo)reader.ReadRIFObject();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfoTypeValuePair;
		}
	}
}
