using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class AttributeInfo : IPersistable
	{
		private bool m_isExpression;

		private string m_stringValue;

		private bool m_boolValue;

		private int m_intValue;

		private double m_floatValue;

		private ValueType m_valueType;

		[NonSerialized]
		private static readonly Declaration m_Declaration = AttributeInfo.GetDeclaration();

		internal bool IsExpression
		{
			get
			{
				return this.m_isExpression;
			}
			set
			{
				this.m_isExpression = value;
			}
		}

		internal string Value
		{
			get
			{
				return this.m_stringValue;
			}
			set
			{
				this.m_stringValue = value;
			}
		}

		internal bool BoolValue
		{
			get
			{
				return this.m_boolValue;
			}
			set
			{
				this.m_boolValue = value;
			}
		}

		internal int IntValue
		{
			get
			{
				return this.m_intValue;
			}
			set
			{
				this.m_intValue = value;
			}
		}

		internal double FloatValue
		{
			get
			{
				return this.m_floatValue;
			}
			set
			{
				this.m_floatValue = value;
			}
		}

		internal ValueType ValueType
		{
			get
			{
				return this.m_valueType;
			}
			set
			{
				this.m_valueType = value;
			}
		}

		internal AttributeInfo PublishClone(AutomaticSubtotalContext context)
		{
			AttributeInfo attributeInfo = (AttributeInfo)base.MemberwiseClone();
			if (this.m_stringValue != null)
			{
				attributeInfo.m_stringValue = (string)this.m_stringValue.Clone();
			}
			return attributeInfo;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.IsExpression, Token.Boolean));
			list.Add(new MemberInfo(MemberName.StringValue, Token.String));
			list.Add(new MemberInfo(MemberName.BoolValue, Token.Boolean));
			list.Add(new MemberInfo(MemberName.IntValue, Token.Int32));
			list.Add(new MemberInfo(MemberName.FloatValue, Token.Double, Lifetime.AddedIn(200)));
			list.Add(new MemberInfo(MemberName.ValueType, Token.Enum, Lifetime.AddedIn(200)));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.AttributeInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(AttributeInfo.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.IsExpression:
					writer.Write(this.m_isExpression);
					break;
				case MemberName.StringValue:
					writer.Write(this.m_stringValue);
					break;
				case MemberName.BoolValue:
					writer.Write(this.m_boolValue);
					break;
				case MemberName.IntValue:
					writer.Write(this.m_intValue);
					break;
				case MemberName.FloatValue:
					writer.Write(this.m_floatValue);
					break;
				case MemberName.ValueType:
					writer.WriteEnum((int)this.m_valueType);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(AttributeInfo.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.IsExpression:
					this.m_isExpression = reader.ReadBoolean();
					break;
				case MemberName.StringValue:
					this.m_stringValue = reader.ReadString();
					break;
				case MemberName.BoolValue:
					this.m_boolValue = reader.ReadBoolean();
					break;
				case MemberName.IntValue:
					this.m_intValue = reader.ReadInt32();
					break;
				case MemberName.FloatValue:
					this.m_floatValue = reader.ReadDouble();
					break;
				case MemberName.ValueType:
					this.m_valueType = (ValueType)reader.ReadEnum();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.AttributeInfo;
		}
	}
}
