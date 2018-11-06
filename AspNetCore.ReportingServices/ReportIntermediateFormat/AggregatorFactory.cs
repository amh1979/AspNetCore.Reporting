using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class AggregatorFactory
	{
		private readonly DataAggregate[] m_prototypes;

		private static AggregatorFactory m_instance;

		public static AggregatorFactory Instance
		{
			get
			{
				if (AggregatorFactory.m_instance == null)
				{
					AggregatorFactory.m_instance = new AggregatorFactory();
				}
				return AggregatorFactory.m_instance;
			}
		}

		private AggregatorFactory()
		{
			int length = Enum.GetValues(typeof(DataAggregateInfo.AggregateTypes)).Length;
			this.m_prototypes = new DataAggregate[length];
			this.Add(new First());
			this.Add(new Last());
			this.Add(new Sum());
			this.Add(new Avg());
			this.Add(new Max());
			this.Add(new Min());
			this.Add(new CountDistinct());
			this.Add(new CountRows());
			this.Add(new Count());
			this.Add(new StDev());
			this.Add(new Var());
			this.Add(new StDevP());
			this.Add(new VarP());
			this.Add(new Aggregate());
			this.Add(new Previous());
			this.Add(new Union());
		}

		private void Add(DataAggregate aggregator)
		{
			int aggregateType = (int)aggregator.AggregateType;
			this.m_prototypes[aggregateType] = aggregator;
		}

		[Conditional("DEBUG")]
		private void VerifyAllPrototypesCreated()
		{
			for (int i = 0; i < this.m_prototypes.Length; i++)
			{
				Global.Tracer.Assert(this.m_prototypes[i] != null, "Missing aggregate prototype for: {0}", (DataAggregateInfo.AggregateTypes)i);
			}
		}

		public DataAggregate CreateAggregator(OnDemandProcessingContext odpContext, DataAggregateInfo aggregateDef)
		{
			DataAggregate prototype = this.GetPrototype(aggregateDef);
			return prototype.ConstructAggregator(odpContext, aggregateDef);
		}

		public object GetNoRowsResult(DataAggregateInfo aggregateDef)
		{
			return this.GetPrototype(aggregateDef).Result();
		}

		private DataAggregate GetPrototype(DataAggregateInfo aggregateDef)
		{
			return this.m_prototypes[(int)aggregateDef.AggregateType];
		}
	}
}
