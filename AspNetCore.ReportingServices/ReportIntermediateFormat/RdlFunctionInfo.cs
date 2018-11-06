using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class RdlFunctionInfo : IPersistable
	{
		internal enum RdlFunctionType
		{
			MinValue,
			MaxValue,
			ScopeKeys,
			Comparable,
			Array
		}

		private RdlFunctionType m_functionType;

		private List<ExpressionInfo> m_simpleExpressions;

		[NonSerialized]
		private static readonly Declaration m_Declaration = RdlFunctionInfo.GetDeclaration();

		internal RdlFunctionType FunctionType
		{
			get
			{
				return this.m_functionType;
			}
			set
			{
				this.m_functionType = value;
			}
		}

		internal List<ExpressionInfo> Expressions
		{
			get
			{
				return this.m_simpleExpressions;
			}
			set
			{
				this.m_simpleExpressions = value;
			}
		}

		internal void SetFunctionType(string functionName)
		{
			this.FunctionType = (RdlFunctionType)Enum.Parse(typeof(RdlFunctionType), functionName, true);
		}

		internal void Initialize(string propertyName, InitializationContext context, bool initializeDataOnError)
		{
			foreach (ExpressionInfo simpleExpression in this.m_simpleExpressions)
			{
				simpleExpression.Initialize(propertyName, context, initializeDataOnError);
			}
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			RdlFunctionInfo rdlFunctionInfo = (RdlFunctionInfo)base.MemberwiseClone();
			rdlFunctionInfo.m_simpleExpressions = new List<ExpressionInfo>(this.m_simpleExpressions.Count);
			foreach (ExpressionInfo simpleExpression in this.m_simpleExpressions)
			{
				rdlFunctionInfo.m_simpleExpressions.Add((ExpressionInfo)simpleExpression.PublishClone(context));
			}
			return rdlFunctionInfo;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.RdlFunctionType, Token.Enum));
			list.Add(new MemberInfo(MemberName.Expressions, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RdlFunctionInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(RdlFunctionInfo.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.RdlFunctionType:
					writer.WriteEnum((int)this.m_functionType);
					break;
				case MemberName.Expressions:
					writer.Write(this.m_simpleExpressions);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(RdlFunctionInfo.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.RdlFunctionType:
					this.m_functionType = (RdlFunctionType)reader.ReadEnum();
					break;
				case MemberName.Expressions:
					this.m_simpleExpressions = reader.ReadGenericListOfRIFObjects<ExpressionInfo>();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RdlFunctionInfo;
		}
	}
}
