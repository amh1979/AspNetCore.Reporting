using AspNetCore.ReportingServices.Diagnostics.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel
{
	public sealed class AggregatesImpl : Aggregates
	{
		internal const string Name = "Aggregates";

		internal const string FullName = "AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel.Aggregates";

		private bool m_lockAdd;

		private Hashtable m_collection;

		private Hashtable m_duplicateNames;

		private IErrorContext m_iErrorContext;

		public override object this[string key]
		{
			get
			{
				DataAggregateObj aggregateObj = this.GetAggregateObj(key);
				if (aggregateObj == null && this.m_duplicateNames != null)
				{
					string text = (string)this.m_duplicateNames[key];
					if (text != null)
					{
						aggregateObj = this.GetAggregateObj(text);
					}
				}
				if (aggregateObj == null)
				{
					return null;
				}
				aggregateObj.UsedInExpression = true;
				DataAggregateObjResult dataAggregateObjResult = aggregateObj.AggregateResult();
				if (dataAggregateObjResult.HasCode)
				{
					if (dataAggregateObjResult.FieldStatus == DataFieldStatus.None && dataAggregateObjResult.Code != 0)
					{
						this.m_iErrorContext.Register(dataAggregateObjResult.Code, dataAggregateObjResult.Severity, dataAggregateObjResult.Arguments);
					}
					if (dataAggregateObjResult.ErrorOccurred)
					{
						throw new ReportProcessingException_InvalidOperationException();
					}
				}
				if (dataAggregateObjResult.ErrorOccurred)
				{
					throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
				}
				return dataAggregateObjResult.Value;
			}
		}

		internal ICollection Objects
		{
			get
			{
				return this.m_collection.Values;
			}
		}

		internal AggregatesImpl(IErrorContext iErrorContext)
			: this(false, iErrorContext)
		{
		}

		internal AggregatesImpl(bool lockAdd, IErrorContext iErrorContext)
		{
			this.m_lockAdd = lockAdd;
			this.m_collection = new Hashtable();
			this.m_duplicateNames = null;
			this.m_iErrorContext = iErrorContext;
		}

		internal void Add(DataAggregateObj newObject)
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

		internal void Set(string name, DataAggregateInfo aggregateDef, StringList duplicateNames, DataAggregateObjResult aggregateResult)
		{
			DataAggregateObj aggregateObj = this.GetAggregateObj(name);
			if (aggregateObj == null)
			{
				try
				{
					if (this.m_lockAdd)
					{
						Monitor.Enter(this.m_collection);
					}
					aggregateObj = new DataAggregateObj(aggregateDef, aggregateResult);
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

		internal DataAggregateObj GetAggregateObj(string name)
		{
			return (DataAggregateObj)this.m_collection[name];
		}

		private void PopulateDuplicateNames(string name, StringList duplicateNames)
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

		internal void ResetUsedInExpression()
		{
			foreach (DataAggregateObj value in this.m_collection.Values)
			{
				value.UsedInExpression = false;
			}
		}

		internal void AddFieldsUsedInExpression(List<string> fieldsUsedInValueExpression)
		{
			foreach (DataAggregateObj value in this.m_collection.Values)
			{
				if (value.UsedInExpression && value.AggregateDef != null && value.AggregateDef.FieldsUsedInValueExpression != null)
				{
					fieldsUsedInValueExpression.AddRange(value.AggregateDef.FieldsUsedInValueExpression);
				}
			}
		}
	}
}
