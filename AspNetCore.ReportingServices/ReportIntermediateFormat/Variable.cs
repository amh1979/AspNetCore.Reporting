using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class Variable : IPersistable
	{
		private DataType m_constantDataType;

		private string m_name;

		private ExpressionInfo m_value;

		private int m_sequenceID = -1;

		private bool m_writable;

		[NonSerialized]
		private bool m_isClone;

		[NonSerialized]
		private string m_propertyName;

		[NonSerialized]
		private VariableImpl m_cachedVariableObj;

		[NonSerialized]
		private static readonly Declaration m_Declaration = Variable.GetDeclaration();

		internal string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				this.m_name = value;
			}
		}

		internal ExpressionInfo Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				this.m_value = value;
			}
		}

		internal DataType DataType
		{
			get
			{
				return this.m_constantDataType;
			}
			set
			{
				this.m_constantDataType = value;
			}
		}

		internal int SequenceID
		{
			get
			{
				return this.m_sequenceID;
			}
			set
			{
				this.m_sequenceID = value;
			}
		}

		internal bool Writable
		{
			get
			{
				return this.m_writable;
			}
			set
			{
				this.m_writable = value;
			}
		}

		internal Variable()
		{
		}

		internal void Initialize(InitializationContext context)
		{
			if (this.m_value != null)
			{
				this.m_value.Initialize(this.GetPropertyName(), context);
			}
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			Variable variable = (Variable)base.MemberwiseClone();
			variable.SequenceID = context.GenerateVariableSequenceID();
			variable.m_isClone = true;
			if (this.m_name != null)
			{
				variable.m_name = context.CreateUniqueVariableName(this.m_name, this.m_isClone);
			}
			if (this.m_value != null)
			{
				variable.m_value = (ExpressionInfo)this.m_value.PublishClone(context);
			}
			return variable;
		}

		internal string GetPropertyName()
		{
			if (this.m_propertyName == null)
			{
				StringBuilder stringBuilder = new StringBuilder("Variable");
				stringBuilder.Append("(");
				stringBuilder.Append(this.m_name);
				stringBuilder.Append(")");
				this.m_propertyName = stringBuilder.ToString();
			}
			return this.m_propertyName;
		}

		internal VariableImpl GetCachedVariableObj(OnDemandProcessingContext odpContext)
		{
			if (this.m_cachedVariableObj == null)
			{
				VariableImpl variableImpl = this.m_cachedVariableObj = (((Variables)odpContext.ReportObjectModel.VariablesImpl)[this.m_name] as VariableImpl);
			}
			return this.m_cachedVariableObj;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Value, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DataType, Token.Enum));
			list.Add(new MemberInfo(MemberName.SequenceID, Token.Int32));
			list.Add(new MemberInfo(MemberName.Writable, Token.Boolean));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Variable, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(Variable.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Name:
					writer.Write(this.m_name);
					break;
				case MemberName.Value:
					writer.Write(this.m_value);
					break;
				case MemberName.DataType:
					writer.WriteEnum((int)this.m_constantDataType);
					break;
				case MemberName.SequenceID:
					writer.Write(this.m_sequenceID);
					break;
				case MemberName.Writable:
					writer.Write(this.m_writable);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(Variable.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Name:
					this.m_name = reader.ReadString();
					break;
				case MemberName.Value:
					this.m_value = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DataType:
					this.m_constantDataType = (DataType)reader.ReadEnum();
					break;
				case MemberName.SequenceID:
					this.m_sequenceID = reader.ReadInt32();
					break;
				case MemberName.Writable:
					this.m_writable = reader.ReadBoolean();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Variable;
		}
	}
}
