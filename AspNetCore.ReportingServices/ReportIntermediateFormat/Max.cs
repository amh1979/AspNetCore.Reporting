using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class Max : DataAggregate
	{
		private AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode m_expressionType;

		private object m_currentMax;

		private IDataComparer m_comparer;

		private static Declaration m_declaration = Max.GetDeclaration();

		internal override DataAggregateInfo.AggregateTypes AggregateType
		{
			get
			{
				return DataAggregateInfo.AggregateTypes.Max;
			}
		}

		public override int Size
		{
			get
			{
				return 4 + ItemSizes.SizeOf(this.m_currentMax) + ItemSizes.ReferenceSize;
			}
		}

		internal Max()
		{
		}

		internal Max(IDataComparer comparer)
		{
			this.m_currentMax = null;
			this.m_comparer = comparer;
		}

		internal override void Init()
		{
			this.m_currentMax = null;
		}

		internal override void Update(object[] expressions, IErrorContext iErrorContext)
		{
			object obj = expressions[0];
			AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode typeCode = DataAggregate.GetTypeCode(obj);
			if (!DataAggregate.IsNull(typeCode))
			{
				if (!DataAggregate.IsVariant(typeCode) || DataTypeUtility.IsSpatial(typeCode))
				{
					iErrorContext.Register(ProcessingErrorCode.rsMinMaxOfNonSortableData, Severity.Warning);
					throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
				}
				if (this.m_currentMax == null)
				{
					this.m_currentMax = obj;
					this.m_expressionType = typeCode;
				}
				else
				{
					bool flag = default(bool);
					int num = this.m_comparer.Compare(this.m_currentMax, obj, false, false, out flag);
					if (!flag)
					{
						if (typeCode != this.m_expressionType)
						{
							iErrorContext.Register(ProcessingErrorCode.rsAggregateOfMixedDataTypes, Severity.Warning);
							throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
						}
						iErrorContext.Register(ProcessingErrorCode.rsMinMaxOfNonSortableData, Severity.Warning);
					}
					else if (num < 0)
					{
						this.m_currentMax = obj;
						this.m_expressionType = typeCode;
					}
				}
			}
		}

		internal override object Result()
		{
			return this.m_currentMax;
		}

		internal override DataAggregate ConstructAggregator(OnDemandProcessingContext odpContext, DataAggregateInfo aggregateDef)
		{
			return new Max(odpContext.ProcessingComparer);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(Max.m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ExpressionType:
					writer.WriteEnum((int)this.m_expressionType);
					break;
				case MemberName.CurrentMax:
					writer.Write(this.m_currentMax);
					break;
				case MemberName.Comparer:
				{
					int value = scalabilityCache.StoreStaticReference(this.m_comparer);
					writer.Write(value);
					break;
				}
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(Max.m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ExpressionType:
					this.m_expressionType = (AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode)reader.ReadEnum();
					break;
				case MemberName.CurrentMax:
					this.m_currentMax = reader.ReadVariant();
					break;
				case MemberName.Comparer:
				{
					int id = reader.ReadInt32();
					this.m_comparer = (AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ProcessingComparer)scalabilityCache.FetchStaticReference(id);
					break;
				}
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false, "Max should not resolve references");
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Max;
		}

		public static Declaration GetDeclaration()
		{
			if (Max.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.ExpressionType, Token.Enum));
				list.Add(new MemberInfo(MemberName.CurrentMax, Token.Object));
				list.Add(new MemberInfo(MemberName.Comparer, Token.Int32));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Max, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return Max.m_declaration;
		}
	}
}
