using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal class DataAggregateObjResult : IStorable, IPersistable
	{
		private struct CloneHelperStruct
		{
			internal object Value;

			internal CloneHelperStruct(object value)
			{
				this.Value = value;
			}
		}

		internal bool ErrorOccurred;

		internal object Value;

		internal bool HasCode;

		internal ProcessingErrorCode Code;

		internal Severity Severity;

		internal string[] Arguments;

		internal DataFieldStatus FieldStatus;

		[NonSerialized]
		private static readonly Declaration m_Declaration = DataAggregateObjResult.GetDeclaration();

		public int Size
		{
			get
			{
				return 1 + ItemSizes.SizeOf(this.Value) + 1 + 4 + 4 + ItemSizes.SizeOf(this.Arguments) + 4;
			}
		}

		internal DataAggregateObjResult()
		{
		}

		internal DataAggregateObjResult(DataAggregateObjResult original)
		{
			this.ErrorOccurred = original.ErrorOccurred;
			this.HasCode = original.HasCode;
			this.Code = original.Code;
			this.Severity = original.Severity;
			this.FieldStatus = original.FieldStatus;
			CloneHelperStruct cloneHelperStruct = new CloneHelperStruct(original.Value);
			this.Value = cloneHelperStruct.Value;
			this.Arguments = original.Arguments;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ErrorOccurred, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Value, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Variant));
			list.Add(new MemberInfo(MemberName.HasCode, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Code, Token.Enum));
			list.Add(new MemberInfo(MemberName.Severity, Token.Enum));
			list.Add(new MemberInfo(MemberName.FieldStatus, Token.Enum));
			list.Add(new MemberInfo(MemberName.Arguments, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, Token.String));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObjResult, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(DataAggregateObjResult.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ErrorOccurred:
					writer.Write(this.ErrorOccurred);
					break;
				case MemberName.Value:
					writer.Write(this.Value);
					break;
				case MemberName.HasCode:
					writer.Write(this.HasCode);
					break;
				case MemberName.Code:
					writer.WriteEnum((int)this.Code);
					break;
				case MemberName.Severity:
					writer.WriteEnum((int)this.Severity);
					break;
				case MemberName.FieldStatus:
					writer.WriteEnum((int)this.FieldStatus);
					break;
				case MemberName.Arguments:
					writer.Write(this.Arguments);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(DataAggregateObjResult.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ErrorOccurred:
					this.ErrorOccurred = reader.ReadBoolean();
					break;
				case MemberName.Value:
					this.Value = reader.ReadVariant();
					break;
				case MemberName.HasCode:
					this.HasCode = reader.ReadBoolean();
					break;
				case MemberName.Code:
					this.Code = (ProcessingErrorCode)reader.ReadEnum();
					break;
				case MemberName.Severity:
					this.Severity = (Severity)reader.ReadEnum();
					break;
				case MemberName.FieldStatus:
					this.FieldStatus = (DataFieldStatus)reader.ReadEnum();
					break;
				case MemberName.Arguments:
					this.Arguments = reader.ReadStringArray();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObjResult;
		}
	}
}
