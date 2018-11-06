using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal class AggregateUpdateContext : ITraversalContext
	{
		private AggregateMode m_mode;

		private OnDemandProcessingContext m_odpContext;

		private AggregateUpdateCollection m_activeAggregates;

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> m_aggsForUpdateAtRowScope;

		private List<string> m_runningValuesForUpdateAtRow;

		public AggregateMode Mode
		{
			get
			{
				return this.m_mode;
			}
		}

		public AggregateUpdateContext(OnDemandProcessingContext odpContext, AggregateMode mode)
		{
			this.m_mode = mode;
			this.m_odpContext = odpContext;
			this.m_activeAggregates = null;
		}

		public AggregateUpdateQueue ReplaceAggregatesToUpdate(BucketedDataAggregateObjs aggBuckets)
		{
			return this.HandleNewBuckets(aggBuckets, false);
		}

		public AggregateUpdateQueue RegisterAggregatesToUpdate(BucketedDataAggregateObjs aggBuckets)
		{
			return this.HandleNewBuckets(aggBuckets, true);
		}

		public AggregateUpdateQueue RegisterRunningValuesToUpdate(AggregateUpdateQueue workQueue, List<AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo> runningValues)
		{
			if (runningValues != null && runningValues.Count != 0)
			{
				if (workQueue == null)
				{
					workQueue = new AggregateUpdateQueue(this.m_activeAggregates);
					AggregateUpdateCollection aggregateUpdateCollection = new AggregateUpdateCollection(runningValues);
					aggregateUpdateCollection.LinkedCollection = this.m_activeAggregates;
					this.m_activeAggregates = aggregateUpdateCollection;
				}
				else
				{
					this.m_activeAggregates.MergeRunningValues(runningValues);
				}
				return workQueue;
			}
			return workQueue;
		}

		private AggregateUpdateQueue HandleNewBuckets(BucketedDataAggregateObjs aggBuckets, bool canMergeActiveAggs)
		{
			bool flag = aggBuckets == null || aggBuckets.Buckets.Count == 0;
			if (canMergeActiveAggs && flag)
			{
				return null;
			}
			AggregateUpdateQueue aggregateUpdateQueue = new AggregateUpdateQueue(this.m_activeAggregates);
			AggregateUpdateCollection aggregateUpdateCollection = null;
			if (canMergeActiveAggs)
			{
				aggregateUpdateCollection = this.m_activeAggregates;
			}
			this.m_activeAggregates = null;
			if (flag)
			{
				return aggregateUpdateQueue;
			}
			for (int i = 0; i < aggBuckets.Buckets.Count; i++)
			{
				AggregateBucket<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> aggregateBucket = aggBuckets.Buckets[i];
				AggregateUpdateCollection aggregateUpdateCollection2 = new AggregateUpdateCollection(aggregateBucket);
				if (aggregateUpdateCollection != null)
				{
					if (aggregateUpdateCollection.Level == aggregateBucket.Level)
					{
						aggregateUpdateCollection2.LinkedCollection = aggregateUpdateCollection;
						aggregateUpdateCollection = null;
					}
					else if (aggregateUpdateCollection.Level < aggregateBucket.Level)
					{
						aggregateUpdateCollection2 = aggregateUpdateCollection;
						i--;
						aggregateUpdateCollection = null;
					}
				}
				if (this.m_activeAggregates == null)
				{
					this.m_activeAggregates = aggregateUpdateCollection2;
				}
				else
				{
					aggregateUpdateQueue.Enqueue(aggregateUpdateCollection2);
				}
			}
			if (aggregateUpdateCollection != null)
			{
				aggregateUpdateQueue.Enqueue(aggregateUpdateCollection);
			}
			return aggregateUpdateQueue;
		}

		public bool AdvanceQueue(AggregateUpdateQueue queue)
		{
			if (queue == null)
			{
				return false;
			}
			if (queue.Count == 0)
			{
				this.RestoreOriginalState(queue);
				return false;
			}
			this.m_activeAggregates = queue.Dequeue();
			return true;
		}

		public void RestoreOriginalState(AggregateUpdateQueue queue)
		{
			if (queue != null)
			{
				this.m_activeAggregates = queue.OriginalState;
			}
		}

		public bool UpdateAggregates(DataScopeInfo scopeInfo, IDataRowHolder scopeInst, AggregateUpdateFlags updateFlags, bool needsSetupEnvironment)
		{
			this.m_aggsForUpdateAtRowScope = null;
			this.m_runningValuesForUpdateAtRow = null;
			if (this.m_activeAggregates == null)
			{
				return false;
			}
			for (AggregateUpdateCollection aggregateUpdateCollection = this.m_activeAggregates; aggregateUpdateCollection != null; aggregateUpdateCollection = aggregateUpdateCollection.LinkedCollection)
			{
				List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> list = default(List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj>);
				if (aggregateUpdateCollection.GetAggregatesForScope(scopeInfo.ScopeID, out list))
				{
					if (needsSetupEnvironment)
					{
						scopeInst.SetupEnvironment();
						needsSetupEnvironment = false;
					}
					foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj item in list)
					{
						item.Update();
					}
				}
				if (aggregateUpdateCollection.GetAggregatesForRowScope(scopeInfo.ScopeID, out list))
				{
					if (this.m_aggsForUpdateAtRowScope == null)
					{
						this.m_aggsForUpdateAtRowScope = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj>();
					}
					this.m_aggsForUpdateAtRowScope.AddRange(list);
				}
				List<string> list2 = default(List<string>);
				if (aggregateUpdateCollection.GetRunningValuesForScope(scopeInfo.ScopeID, out list2))
				{
					if (needsSetupEnvironment)
					{
						scopeInst.SetupEnvironment();
						needsSetupEnvironment = false;
					}
					RuntimeDataTablixObj.UpdateRunningValues(this.m_odpContext, list2);
				}
				if (aggregateUpdateCollection.GetRunningValuesForRowScope(scopeInfo.ScopeID, out list2))
				{
					if (this.m_runningValuesForUpdateAtRow == null)
					{
						this.m_runningValuesForUpdateAtRow = new List<string>();
					}
					this.m_runningValuesForUpdateAtRow.AddRange(list2);
				}
			}
			if (this.m_aggsForUpdateAtRowScope != null || this.m_runningValuesForUpdateAtRow != null)
			{
				if (needsSetupEnvironment)
				{
					scopeInst.SetupEnvironment();
				}
				if (FlagUtils.HasFlag(updateFlags, AggregateUpdateFlags.RowAggregates))
				{
					scopeInst.ReadRows(DataActions.AggregatesOfAggregates, this);
				}
			}
			return scopeInfo.ScopeID != this.m_activeAggregates.InnermostUpdateScopeID;
		}

		public void UpdateAggregatesForRow()
		{
			Global.Tracer.Assert(this.m_aggsForUpdateAtRowScope != null || this.m_runningValuesForUpdateAtRow != null, "UpdateAggregatesForRow must be driven by a call to UpdateAggregates.");
			if (this.m_aggsForUpdateAtRowScope != null)
			{
				foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj item in this.m_aggsForUpdateAtRowScope)
				{
					item.Update();
				}
			}
			if (this.m_runningValuesForUpdateAtRow != null)
			{
				RuntimeDataTablixObj.UpdateRunningValues(this.m_odpContext, this.m_runningValuesForUpdateAtRow);
			}
		}

		public bool LastScopeNeedsRowAggregateProcessing()
		{
			if (this.m_aggsForUpdateAtRowScope == null)
			{
				return this.m_runningValuesForUpdateAtRow != null;
			}
			return true;
		}
	}
}
