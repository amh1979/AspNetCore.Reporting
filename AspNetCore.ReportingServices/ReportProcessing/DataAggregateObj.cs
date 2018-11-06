using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class DataAggregateObj : IErrorContext
	{
		private bool m_nonAggregateMode;

		private string m_name;

		private StringList m_duplicateNames;

		private DataAggregate m_aggregator;

		private DataAggregateInfo m_aggregateDef;

		private ReportRuntime m_reportRT;

		private bool m_usedInExpression;

		private DataAggregateObjResult m_aggregateResult;

		internal string Name
		{
			get
			{
				return this.m_name;
			}
		}

		internal StringList DuplicateNames
		{
			get
			{
				return this.m_duplicateNames;
			}
		}

		internal bool NonAggregateMode
		{
			get
			{
				return this.m_nonAggregateMode;
			}
		}

		internal DataAggregateInfo AggregateDef
		{
			get
			{
				return this.m_aggregateDef;
			}
		}

		internal bool UsedInExpression
		{
			get
			{
				return this.m_usedInExpression;
			}
			set
			{
				this.m_usedInExpression = value;
			}
		}

		internal DataAggregateObj(DataAggregateInfo aggInfo, ReportProcessing.ProcessingContext processingContext)
		{
			this.m_nonAggregateMode = false;
			this.m_name = aggInfo.Name;
			this.m_duplicateNames = aggInfo.DuplicateNames;
			switch (aggInfo.AggregateType)
			{
			case DataAggregateInfo.AggregateTypes.First:
				this.m_aggregator = new First();
				break;
			case DataAggregateInfo.AggregateTypes.Last:
				this.m_aggregator = new Last();
				break;
			case DataAggregateInfo.AggregateTypes.Sum:
				this.m_aggregator = new Sum();
				break;
			case DataAggregateInfo.AggregateTypes.Avg:
				this.m_aggregator = new Avg();
				break;
			case DataAggregateInfo.AggregateTypes.Max:
				this.m_aggregator = new Max(processingContext.CompareInfo, processingContext.ClrCompareOptions);
				break;
			case DataAggregateInfo.AggregateTypes.Min:
				this.m_aggregator = new Min(processingContext.CompareInfo, processingContext.ClrCompareOptions);
				break;
			case DataAggregateInfo.AggregateTypes.CountDistinct:
				this.m_aggregator = new CountDistinct();
				break;
			case DataAggregateInfo.AggregateTypes.CountRows:
				this.m_aggregator = new CountRows();
				break;
			case DataAggregateInfo.AggregateTypes.Count:
				this.m_aggregator = new Count();
				break;
			case DataAggregateInfo.AggregateTypes.StDev:
				this.m_aggregator = new StDev();
				break;
			case DataAggregateInfo.AggregateTypes.Var:
				this.m_aggregator = new Var();
				break;
			case DataAggregateInfo.AggregateTypes.StDevP:
				this.m_aggregator = new StDevP();
				break;
			case DataAggregateInfo.AggregateTypes.VarP:
				this.m_aggregator = new VarP();
				break;
			case DataAggregateInfo.AggregateTypes.Aggregate:
				this.m_aggregator = new Aggregate();
				break;
			case DataAggregateInfo.AggregateTypes.Previous:
				this.m_aggregator = new Previous();
				break;
			default:
				Global.Tracer.Assert(false, "Unsupport aggregate type.");
				break;
			}
			this.m_aggregateDef = aggInfo;
			this.m_reportRT = processingContext.ReportRuntime;
			if (this.m_reportRT.ReportExprHost != null)
			{
				this.m_aggregateDef.SetExprHosts(this.m_reportRT.ReportExprHost, processingContext.ReportObjectModel);
			}
			this.m_aggregateResult = default(DataAggregateObjResult);
			this.Init();
		}

		internal DataAggregateObj(DataAggregateInfo aggrDef, DataAggregateObjResult aggrResult)
		{
			this.m_nonAggregateMode = true;
			this.m_aggregateDef = aggrDef;
			this.m_aggregateResult = aggrResult;
		}

		internal void Init()
		{
			if (!this.m_nonAggregateMode)
			{
				this.m_aggregator.Init();
				this.m_aggregateResult = default(DataAggregateObjResult);
			}
		}

		internal void Update()
		{
			if (!this.m_aggregateResult.ErrorOccurred && !this.m_nonAggregateMode)
			{
				if (this.m_aggregateDef.FieldsUsedInValueExpression == null)
				{
					this.m_reportRT.ReportObjectModel.FieldsImpl.ResetUsedInExpression();
				}
				object[] expressions = default(object[]);
				DataFieldStatus dataFieldStatus = default(DataFieldStatus);
				this.m_aggregateResult.ErrorOccurred = this.EvaluateParameters(out expressions, out dataFieldStatus);
				if (dataFieldStatus != 0)
				{
					this.m_aggregateResult.HasCode = true;
					this.m_aggregateResult.FieldStatus = dataFieldStatus;
				}
				if (this.m_aggregateDef.FieldsUsedInValueExpression == null)
				{
					this.m_aggregateDef.FieldsUsedInValueExpression = new List<string>();
					this.m_reportRT.ReportObjectModel.FieldsImpl.AddFieldsUsedInExpression(this.m_aggregateDef.FieldsUsedInValueExpression);
				}
				if (!this.m_aggregateResult.ErrorOccurred)
				{
					try
					{
						this.m_aggregator.Update(expressions, this);
					}
					catch (ReportProcessingException)
					{
						this.m_aggregateResult.ErrorOccurred = true;
					}
				}
			}
		}

		internal DataAggregateObjResult AggregateResult()
		{
			if (!this.m_nonAggregateMode && !this.m_aggregateResult.ErrorOccurred)
			{
				try
				{
					this.m_aggregateResult.Value = this.m_aggregator.Result();
				}
				catch (ReportProcessingException)
				{
					this.m_aggregateResult.ErrorOccurred = true;
					this.m_aggregateResult.Value = null;
				}
			}
			return this.m_aggregateResult;
		}

		internal bool EvaluateParameters(out object[] values, out DataFieldStatus fieldStatus)
		{
			bool flag = false;
			fieldStatus = DataFieldStatus.None;
			values = new object[this.m_aggregateDef.Expressions.Length];
			for (int i = 0; i < this.m_aggregateDef.Expressions.Length; i++)
			{
				VariantResult variantResult = this.m_reportRT.EvaluateAggregateVariantOrBinaryParamExpr(this.m_aggregateDef, i, this);
				values[i] = variantResult.Value;
				flag |= variantResult.ErrorOccurred;
				if (variantResult.FieldStatus != 0)
				{
					fieldStatus = variantResult.FieldStatus;
				}
			}
			return flag;
		}

		internal void Set(DataAggregateObjResult aggregateResult)
		{
			this.m_nonAggregateMode = true;
			this.m_aggregateResult = aggregateResult;
		}

		void IErrorContext.Register(ProcessingErrorCode code, Severity severity, params string[] arguments)
		{
			if (!this.m_aggregateResult.HasCode)
			{
				this.m_aggregateResult.HasCode = true;
				this.m_aggregateResult.Code = code;
				this.m_aggregateResult.Severity = severity;
				this.m_aggregateResult.Arguments = arguments;
			}
		}

		void IErrorContext.Register(ProcessingErrorCode code, Severity severity, ObjectType objectType, string objectName, string propertyName, params string[] arguments)
		{
			if (!this.m_aggregateResult.HasCode)
			{
				this.m_aggregateResult.HasCode = true;
				this.m_aggregateResult.Code = code;
				this.m_aggregateResult.Severity = severity;
				this.m_aggregateResult.Arguments = arguments;
			}
		}
	}
}
