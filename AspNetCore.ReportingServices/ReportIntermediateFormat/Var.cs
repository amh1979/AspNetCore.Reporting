using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal class Var : VarBase
	{
		private static Declaration m_declaration = Var.GetDeclaration();

		internal override DataAggregateInfo.AggregateTypes AggregateType
		{
			get
			{
				return DataAggregateInfo.AggregateTypes.Var;
			}
		}

		internal override object Result()
		{
			if (1 == base.m_currentCount)
			{
				return null;
			}
			switch (base.m_sumOfXType)
			{
			case AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Null:
				return null;
			case AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Double:
				return ((double)base.m_currentCount * (double)base.m_sumOfXSquared - (double)base.m_sumOfX * (double)base.m_sumOfX) / (double)(base.m_currentCount * (base.m_currentCount - 1));
			case AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Decimal:
				return ((decimal)base.m_currentCount * (decimal)base.m_sumOfXSquared - (decimal)base.m_sumOfX * (decimal)base.m_sumOfX) / (decimal)(base.m_currentCount * (base.m_currentCount - 1));
			default:
				Global.Tracer.Assert(false);
				throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
			}
		}

		internal override DataAggregate ConstructAggregator(OnDemandProcessingContext odpContext, DataAggregateInfo aggregateDef)
		{
			return new Var();
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Var;
		}

		public new static Declaration GetDeclaration()
		{
			if (Var.m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Var, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.VarBase, memberInfoList);
			}
			return Var.m_declaration;
		}
	}
}
