using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class DataAggregateObj : IErrorContext, IStorable, IPersistable
	{
		private bool m_nonAggregateMode;

		private DataAggregate m_aggregator;

		[StaticReference]
		private DataAggregateInfo m_aggregateDef;

		[StaticReference]
		private AspNetCore.ReportingServices.RdlExpressions.ReportRuntime m_reportRT;

		private bool m_usedInExpression;

		private DataAggregateObjResult m_aggregateResult;

		[NonSerialized]
		private static Declaration m_declaration = DataAggregateObj.GetDeclaration();

		internal string Name
		{
			get
			{
				return this.m_aggregateDef.Name;
			}
		}

		internal List<string> DuplicateNames
		{
			get
			{
				return this.m_aggregateDef.DuplicateNames;
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

		public int Size
		{
			get
			{
				return 1 + ItemSizes.SizeOf(this.m_aggregator) + ItemSizes.ReferenceSize + ItemSizes.ReferenceSize + 1 + ItemSizes.SizeOf(this.m_aggregateResult);
			}
		}

		internal DataAggregateObj()
		{
		}

		internal DataAggregateObj(DataAggregateInfo aggInfo, OnDemandProcessingContext odpContext)
		{
			this.m_nonAggregateMode = false;
			this.m_aggregator = AggregatorFactory.Instance.CreateAggregator(odpContext, aggInfo);
			this.m_aggregateDef = aggInfo;
			this.m_reportRT = odpContext.ReportRuntime;
			if (this.m_reportRT.ReportExprHost != null)
			{
				this.m_aggregateDef.SetExprHosts(this.m_reportRT.ReportExprHost, odpContext.ReportObjectModel);
			}
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
				this.m_aggregateResult = new DataAggregateObjResult();
			}
		}

		internal void ResetForNoRows()
		{
			if (this.m_nonAggregateMode)
			{
				this.m_aggregateResult = new DataAggregateObjResult();
				this.m_aggregateResult.Value = AggregatorFactory.Instance.GetNoRowsResult(this.m_aggregateDef);
			}
			else
			{
				this.Init();
			}
		}

		internal void Update()
		{
			if (!this.m_aggregateResult.ErrorOccurred && !this.m_nonAggregateMode)
			{
				if (this.m_aggregateDef.ShouldRecordFieldReferences())
				{
					this.m_reportRT.ReportObjectModel.FieldsImpl.ResetFieldsUsedInExpression();
				}
				object[] expressions = default(object[]);
				DataFieldStatus dataFieldStatus = default(DataFieldStatus);
				this.m_aggregateResult.ErrorOccurred = this.EvaluateParameters(out expressions, out dataFieldStatus);
				if (dataFieldStatus != 0)
				{
					this.m_aggregateResult.HasCode = true;
					this.m_aggregateResult.FieldStatus = dataFieldStatus;
				}
				if (this.m_aggregateDef.ShouldRecordFieldReferences())
				{
					List<string> list = new List<string>();
					this.m_reportRT.ReportObjectModel.FieldsImpl.AddFieldsUsedInExpression(list);
					this.m_aggregateDef.StoreFieldReferences(this.m_reportRT.ReportObjectModel.OdpContext, list);
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
			if (this.m_aggregateDef.MustCopyAggregateResult)
			{
				return new DataAggregateObjResult(this.m_aggregateResult);
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
				try
				{
					AspNetCore.ReportingServices.RdlExpressions.VariantResult variantResult = this.m_reportRT.EvaluateAggregateVariantOrBinaryParamExpr(this.m_aggregateDef, i, this);
					values[i] = variantResult.Value;
					flag |= variantResult.ErrorOccurred;
					if (variantResult.FieldStatus != 0)
					{
						fieldStatus = variantResult.FieldStatus;
					}
				}
				catch (ReportProcessingException_MissingAggregateDependency)
				{
					if (this.m_aggregateDef.AggregateType == DataAggregateInfo.AggregateTypes.Previous)
					{
						values[i] = null;
						fieldStatus = DataFieldStatus.None;
						return false;
					}
					Global.Tracer.Assert(false, "Unfulfilled aggregate dependency outside of a previous");
					throw;
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

		void IErrorContext.Register(ProcessingErrorCode code, Severity severity, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, params string[] arguments)
		{
			if (!this.m_aggregateResult.HasCode)
			{
				this.m_aggregateResult.HasCode = true;
				this.m_aggregateResult.Code = code;
				this.m_aggregateResult.Severity = severity;
				this.m_aggregateResult.Arguments = arguments;
			}
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(DataAggregateObj.m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.NonAggregateMode:
					writer.Write(this.m_nonAggregateMode);
					break;
				case MemberName.Aggregator:
					writer.Write(this.m_aggregator);
					break;
				case MemberName.AggregateDef:
					writer.Write(scalabilityCache.StoreStaticReference(this.m_aggregateDef));
					break;
				case MemberName.ReportRuntime:
				{
					int value = scalabilityCache.StoreStaticReference(this.m_reportRT);
					writer.Write(value);
					break;
				}
				case MemberName.UsedInExpression:
					writer.Write(this.m_usedInExpression);
					break;
				case MemberName.AggregateResult:
					writer.Write(this.m_aggregateResult);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(DataAggregateObj.m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.NonAggregateMode:
					this.m_nonAggregateMode = reader.ReadBoolean();
					break;
				case MemberName.Aggregator:
					this.m_aggregator = (DataAggregate)reader.ReadRIFObject();
					break;
				case MemberName.AggregateDef:
				{
					int id2 = reader.ReadInt32();
					this.m_aggregateDef = (DataAggregateInfo)scalabilityCache.FetchStaticReference(id2);
					break;
				}
				case MemberName.ReportRuntime:
				{
					int id = reader.ReadInt32();
					this.m_reportRT = (AspNetCore.ReportingServices.RdlExpressions.ReportRuntime)scalabilityCache.FetchStaticReference(id);
					break;
				}
				case MemberName.UsedInExpression:
					this.m_usedInExpression = reader.ReadBoolean();
					break;
				case MemberName.AggregateResult:
					this.m_aggregateResult = (DataAggregateObjResult)reader.ReadRIFObject();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObj;
		}

		internal static Declaration GetDeclaration()
		{
			if (DataAggregateObj.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.NonAggregateMode, Token.Boolean));
				list.Add(new MemberInfo(MemberName.Aggregator, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregate));
				list.Add(new MemberInfo(MemberName.AggregateDef, Token.Int32));
				list.Add(new MemberInfo(MemberName.ReportRuntime, Token.Int32));
				list.Add(new MemberInfo(MemberName.UsedInExpression, Token.Boolean));
				list.Add(new MemberInfo(MemberName.AggregateResult, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObjResult));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObj, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return DataAggregateObj.m_declaration;
		}
	}
}
