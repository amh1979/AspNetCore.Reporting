using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public sealed class AggregatesImpl : Aggregates, IStaticReferenceable
	{
		private bool m_lockAdd;

		private Hashtable m_collection;

		private Hashtable m_duplicateNames;

		private OnDemandProcessingContext m_odpContext;

		private int m_id = -2147483648;

		public override object this[string key]
		{
			get
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj = this.GetAggregateObj(key);
				if (aggregateObj == null)
				{
					if (this.m_odpContext.IsTablixProcessingMode)
					{
						this.m_odpContext.ReportRuntime.UnfulfilledDependency = true;
					}
					if (this.m_odpContext.CalculateAggregate(key))
					{
						aggregateObj = this.GetAggregateObj(key);
						Global.Tracer.Assert(null != aggregateObj, "(null != aggregateObj)");
						goto IL_0059;
					}
					return null;
				}
				goto IL_0059;
				IL_0059:
				object aggregateValue = this.GetAggregateValue(key, aggregateObj);
				if (aggregateValue == null && aggregateObj.AggregateDef.AggregateType == AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Aggregate && this.m_odpContext.StreamingMode && this.m_odpContext.StateManager.CheckForPrematureServerAggregate(key))
				{
					aggregateValue = this.GetAggregateValue(key, this.GetAggregateObj(key));
				}
				return aggregateValue;
			}
		}

		private IErrorContext ErrorContext
		{
			get
			{
				return this.m_odpContext.ReportRuntime;
			}
		}

		internal ICollection Objects
		{
			get
			{
				return this.m_collection.Values;
			}
		}

		public int ID
		{
			get
			{
				return this.m_id;
			}
		}

		internal AggregatesImpl()
		{
		}

		internal AggregatesImpl(OnDemandProcessingContext odpContext)
			: this(false, odpContext)
		{
		}

		internal AggregatesImpl(bool lockAdd, OnDemandProcessingContext odpContext)
		{
			this.m_lockAdd = lockAdd;
			this.m_odpContext = odpContext;
			this.ClearAll();
		}

		private object GetAggregateValue(string key, AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj)
		{
			aggregateObj.UsedInExpression = true;
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult dataAggregateObjResult = aggregateObj.AggregateResult();
			if (dataAggregateObjResult == null)
			{
				Global.Tracer.Assert(this.m_odpContext.IsTablixProcessingMode, "Missing aggregate result outside of tablix processing");
				throw new ReportProcessingException_MissingAggregateDependency();
			}
			if (dataAggregateObjResult.HasCode)
			{
				if ((dataAggregateObjResult.FieldStatus == DataFieldStatus.None || dataAggregateObjResult.FieldStatus == DataFieldStatus.IsError) && dataAggregateObjResult.Code != 0)
				{
					this.ErrorContext.Register(dataAggregateObjResult.Code, dataAggregateObjResult.Severity, dataAggregateObjResult.Arguments);
					goto IL_0091;
				}
				if (dataAggregateObjResult.FieldStatus == DataFieldStatus.UnSupportedDataType)
				{
					this.ErrorContext.Register(ProcessingErrorCode.rsAggregateOfInvalidExpressionDataType, Severity.Warning, dataAggregateObjResult.Arguments);
				}
				goto IL_0091;
			}
			goto IL_009f;
			IL_009f:
			if (dataAggregateObjResult.ErrorOccurred)
			{
				throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
			}
			return dataAggregateObjResult.Value;
			IL_0091:
			if (dataAggregateObjResult.ErrorOccurred)
			{
				throw new ReportProcessingException_InvalidOperationException();
			}
			goto IL_009f;
		}

		internal void ClearAll()
		{
			if (this.m_collection != null)
			{
				this.m_collection.Clear();
			}
			else
			{
				this.m_collection = new Hashtable();
			}
			this.m_duplicateNames = null;
		}

		internal void ResetAll()
		{
			foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj value in this.m_collection.Values)
			{
				value.Init();
			}
		}

		internal void ResetAll<T>(IEnumerable<T> aggregateDefs) where T : AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo
		{
			if (aggregateDefs != null)
			{
				foreach (T aggregateDef2 in aggregateDefs)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo aggregateDef = (AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo)(object)aggregateDef2;
					this.Reset(aggregateDef);
				}
			}
		}

		internal void Reset(AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo aggregateDef)
		{
			if (this.m_collection != null)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj = this.GetAggregateObj(aggregateDef.Name);
				if (aggregateObj != null)
				{
					aggregateObj.ResetForNoRows();
				}
			}
		}

		internal void Add(AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj newObject)
		{
			Global.Tracer.Assert(!newObject.NonAggregateMode, "( !newObject.NonAggregateMode )");
			try
			{
				if (this.m_lockAdd)
				{
					Monitor.Enter(this.m_collection);
				}
				this.m_collection.Add(newObject.Name, newObject);
				this.PopulateDuplicateNames(newObject.Name, newObject.DuplicateNames);
			}
			finally
			{
				if (this.m_lockAdd)
				{
					Monitor.Exit(this.m_collection);
				}
			}
		}

		internal void Remove(AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo aggDef)
		{
			try
			{
				if (this.m_lockAdd)
				{
					Monitor.Enter(this.m_collection);
				}
				if (this.m_collection != null)
				{
					this.m_collection.Remove(aggDef.Name);
					List<string> duplicateNames = aggDef.DuplicateNames;
					if (this.m_duplicateNames != null && duplicateNames != null)
					{
						for (int i = 0; i < duplicateNames.Count; i++)
						{
							this.m_duplicateNames.Remove(duplicateNames[i]);
						}
					}
				}
			}
			finally
			{
				if (this.m_lockAdd)
				{
					Monitor.Exit(this.m_collection);
				}
			}
		}

		internal void Set(string name, AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo aggregateDef, List<string> duplicateNames, AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult aggregateResult)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj = this.GetAggregateObj(name);
			if (aggregateObj == null)
			{
				try
				{
					if (this.m_lockAdd)
					{
						Monitor.Enter(this.m_collection);
					}
					aggregateObj = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj(aggregateDef, aggregateResult);
					this.m_collection.Add(name, aggregateObj);
					this.PopulateDuplicateNames(name, duplicateNames);
				}
				finally
				{
					if (this.m_lockAdd)
					{
						Monitor.Exit(this.m_collection);
					}
				}
			}
			else
			{
				aggregateObj.Set(aggregateResult);
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj GetAggregateObj(string name)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj dataAggregateObj = (AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj)this.m_collection[name];
			if (dataAggregateObj == null && this.m_duplicateNames != null)
			{
				string text = (string)this.m_duplicateNames[name];
				if (text != null)
				{
					dataAggregateObj = (AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj)this.m_collection[text];
				}
			}
			return dataAggregateObj;
		}

		private void PopulateDuplicateNames(string name, List<string> duplicateNames)
		{
			if (duplicateNames != null && 0 < duplicateNames.Count)
			{
				if (this.m_duplicateNames == null)
				{
					this.m_duplicateNames = new Hashtable();
				}
				for (int i = 0; i < duplicateNames.Count; i++)
				{
					this.m_duplicateNames[duplicateNames[i]] = name;
				}
			}
		}

		internal void ResetFieldsUsedInExpression()
		{
			foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj value in this.m_collection.Values)
			{
				value.UsedInExpression = false;
			}
		}

		internal void AddFieldsUsedInExpression(OnDemandProcessingContext odpContext, List<string> fieldsUsedInValueExpression)
		{
			Dictionary<string, List<string>> aggregateFieldReferences = odpContext.OdpMetadata.ReportSnapshot.AggregateFieldReferences;
			foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj value in this.m_collection.Values)
			{
				List<string> collection = default(List<string>);
				if (value.UsedInExpression && value.AggregateDef != null && aggregateFieldReferences.TryGetValue(value.AggregateDef.Name, out collection))
				{
					fieldsUsedInValueExpression.AddRange(collection);
				}
			}
		}

		public void SetID(int id)
		{
			this.m_id = id;
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.AggregatesImpl;
		}
	}
}
