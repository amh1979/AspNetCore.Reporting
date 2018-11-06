using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class StDevP : VarP
	{
		private static Declaration m_declaration = StDevP.GetDeclaration();

		internal override DataAggregateInfo.AggregateTypes AggregateType
		{
			get
			{
				return DataAggregateInfo.AggregateTypes.StDevP;
			}
		}

		internal override object Result()
		{
			switch (base.m_sumOfXType)
			{
			case AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Null:
				return null;
			case AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Double:
				return Math.Sqrt((double)base.Result());
			case AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Decimal:
				return Math.Sqrt(Convert.ToDouble((decimal)base.Result()));
			default:
				Global.Tracer.Assert(false);
				throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
			}
		}

		internal override DataAggregate ConstructAggregator(OnDemandProcessingContext odpContext, DataAggregateInfo aggregateDef)
		{
			return new StDevP();
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StDevP;
		}

		public new static Declaration GetDeclaration()
		{
			if (StDevP.m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StDevP, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.VarP, memberInfoList);
			}
			return StDevP.m_declaration;
		}
	}
}
