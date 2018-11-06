using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class RuntimeExpressionInfo : IStorable, IPersistable
	{
		[StaticReference]
		private ExpressionInfo m_expression;

		private bool m_direction = true;

		[StaticReference]
		private IndexedExprHost m_expressionsHost;

		private int m_expressionIndex;

		private static Declaration m_declaration = RuntimeExpressionInfo.GetDeclaration();

		internal ExpressionInfo Expression
		{
			get
			{
				return this.m_expression;
			}
		}

		internal bool Direction
		{
			get
			{
				return this.m_direction;
			}
		}

		internal IndexedExprHost ExpressionsHost
		{
			get
			{
				return this.m_expressionsHost;
			}
		}

		internal int ExpressionIndex
		{
			get
			{
				return this.m_expressionIndex;
			}
		}

		public int Size
		{
			get
			{
				return ItemSizes.ReferenceSize + 1 + ItemSizes.ReferenceSize + 4;
			}
		}

		internal RuntimeExpressionInfo()
		{
		}

		internal RuntimeExpressionInfo(List<ExpressionInfo> expressions, IndexedExprHost expressionsHost, List<bool> directions, int expressionIndex)
		{
			this.m_expressionsHost = expressionsHost;
			this.m_expressionIndex = expressionIndex;
			this.m_expression = expressions[this.m_expressionIndex];
			if (directions != null)
			{
				this.m_direction = directions[this.m_expressionIndex];
			}
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(RuntimeExpressionInfo.m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Expression:
				{
					int value2 = scalabilityCache.StoreStaticReference(this.m_expression);
					writer.Write(value2);
					break;
				}
				case MemberName.Direction:
					writer.Write(this.m_direction);
					break;
				case MemberName.ExpressionsHost:
				{
					int value = scalabilityCache.StoreStaticReference(this.m_expressionsHost);
					writer.Write(value);
					break;
				}
				case MemberName.ExpressionIndex:
					writer.Write(this.m_expressionIndex);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(RuntimeExpressionInfo.m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Expression:
				{
					int id2 = reader.ReadInt32();
					this.m_expression = (ExpressionInfo)scalabilityCache.FetchStaticReference(id2);
					break;
				}
				case MemberName.Direction:
					this.m_direction = reader.ReadBoolean();
					break;
				case MemberName.ExpressionsHost:
				{
					int id = reader.ReadInt32();
					this.m_expressionsHost = (IndexedExprHost)scalabilityCache.FetchStaticReference(id);
					break;
				}
				case MemberName.ExpressionIndex:
					this.m_expressionIndex = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeExpressionInfo;
		}

		public static Declaration GetDeclaration()
		{
			if (RuntimeExpressionInfo.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Expression, Token.Int32));
				list.Add(new MemberInfo(MemberName.Direction, Token.Boolean));
				list.Add(new MemberInfo(MemberName.ExpressionsHost, Token.Int32));
				list.Add(new MemberInfo(MemberName.ExpressionIndex, Token.Int32));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeExpressionInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return RuntimeExpressionInfo.m_declaration;
		}
	}
}
