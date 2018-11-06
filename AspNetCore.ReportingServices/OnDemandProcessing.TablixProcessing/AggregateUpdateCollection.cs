using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal class AggregateUpdateCollection
	{
		private class AggregatesByScopeId : Dictionary<int, List<DataAggregateObj>>
		{
			public AggregatesByScopeId()
				: base(5)
			{
			}
		}

		private class RunningValuesByScopeId : Dictionary<int, List<string>>
		{
			public RunningValuesByScopeId()
				: base(5)
			{
			}
		}

		private int m_level;

		private int m_innermostUpdateScopeID;

		private int m_innermostUpdateScopeDepth;

		private AggregateUpdateCollection m_linkedCol;

		private AggregatesByScopeId m_aggsByUpdateScope;

		private AggregatesByScopeId m_rowAggsByUpdateScope;

		private RunningValuesByScopeId m_runningValuesByUpdateScope;

		private RunningValuesByScopeId m_rowRunningValuesByUpdateScope;

		public int Level
		{
			get
			{
				return this.m_level;
			}
			set
			{
				this.m_level = value;
			}
		}

		public int InnermostUpdateScopeID
		{
			get
			{
				return this.m_innermostUpdateScopeID;
			}
		}

		public int InnermostUpdateScopeDepth
		{
			get
			{
				return this.m_innermostUpdateScopeDepth;
			}
		}

		public AggregateUpdateCollection LinkedCollection
		{
			get
			{
				return this.m_linkedCol;
			}
			set
			{
				this.m_linkedCol = value;
				if (this.m_linkedCol != null && this.m_linkedCol.m_innermostUpdateScopeDepth > this.m_innermostUpdateScopeDepth)
				{
					this.m_innermostUpdateScopeDepth = this.m_linkedCol.m_innermostUpdateScopeDepth;
					this.m_innermostUpdateScopeID = this.m_linkedCol.m_innermostUpdateScopeID;
				}
			}
		}

		public AggregateUpdateCollection(AggregateBucket<DataAggregateObj> bucket)
		{
			this.m_level = bucket.Level;
			this.m_innermostUpdateScopeID = -1;
			this.m_innermostUpdateScopeDepth = -1;
			foreach (DataAggregateObj aggregate in bucket.Aggregates)
			{
				DataAggregateInfo aggregateDef = aggregate.AggregateDef;
				if (aggregateDef.UpdatesAtRowScope)
				{
					this.Add<AggregatesByScopeId, DataAggregateObj>(ref this.m_rowAggsByUpdateScope, aggregateDef, aggregate);
				}
				else
				{
					this.Add<AggregatesByScopeId, DataAggregateObj>(ref this.m_aggsByUpdateScope, aggregateDef, aggregate);
				}
			}
		}

		public AggregateUpdateCollection(List<RunningValueInfo> runningValues)
		{
			this.m_level = 0;
			this.MergeRunningValues(runningValues);
		}

		public void MergeRunningValues(List<RunningValueInfo> runningValues)
		{
			foreach (RunningValueInfo runningValue in runningValues)
			{
				if (runningValue.UpdatesAtRowScope)
				{
					this.Add<RunningValuesByScopeId, string>(ref this.m_rowRunningValuesByUpdateScope, (DataAggregateInfo)runningValue, runningValue.Name);
				}
				else
				{
					this.Add<RunningValuesByScopeId, string>(ref this.m_runningValuesByUpdateScope, (DataAggregateInfo)runningValue, runningValue.Name);
				}
			}
		}

		private void Add<T, U>(ref T colByScope, DataAggregateInfo agg, U item) where T : Dictionary<int, List<U>>, new()
		{
			if (colByScope == null)
			{
				colByScope = new T();
			}
			int updateScopeID = agg.UpdateScopeID;
			List<U> list = default(List<U>);
			if (!((Dictionary<int, List<U>>)colByScope).TryGetValue(updateScopeID, out list))
			{
				list = new List<U>();
				((Dictionary<int, List<U>>)colByScope).Add(updateScopeID, list);
			}
			list.Add(item);
			if (agg.UpdateScopeDepth > this.m_innermostUpdateScopeDepth)
			{
				this.m_innermostUpdateScopeID = agg.UpdateScopeID;
				this.m_innermostUpdateScopeDepth = agg.UpdateScopeDepth;
			}
		}

		public bool GetAggregatesForScope(int scopeId, out List<DataAggregateObj> aggs)
		{
			return this.NullCheckTryGetValue<DataAggregateObj>((Dictionary<int, List<DataAggregateObj>>)this.m_aggsByUpdateScope, scopeId, out aggs);
		}

		public bool GetAggregatesForRowScope(int scopeId, out List<DataAggregateObj> aggs)
		{
			return this.NullCheckTryGetValue<DataAggregateObj>((Dictionary<int, List<DataAggregateObj>>)this.m_rowAggsByUpdateScope, scopeId, out aggs);
		}

		public bool GetRunningValuesForScope(int scopeId, out List<string> aggs)
		{
			return this.NullCheckTryGetValue<string>((Dictionary<int, List<string>>)this.m_runningValuesByUpdateScope, scopeId, out aggs);
		}

		public bool GetRunningValuesForRowScope(int scopeId, out List<string> aggs)
		{
			return this.NullCheckTryGetValue<string>((Dictionary<int, List<string>>)this.m_rowRunningValuesByUpdateScope, scopeId, out aggs);
		}

		private bool NullCheckTryGetValue<T>(Dictionary<int, List<T>> aggsById, int scopeId, out List<T> aggs)
		{
			if (aggsById == null)
			{
				aggs = null;
				return false;
			}
			return aggsById.TryGetValue(scopeId, out aggs);
		}
	}
}
