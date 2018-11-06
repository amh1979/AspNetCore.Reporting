using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal abstract class LinearIdcDataManager : BaseIdcDataManager
	{
		private readonly Relationship m_activeRelationship;

		private VariantResult[] m_lastPrimaryKeyValues;

		public LinearIdcDataManager(OnDemandProcessingContext odpContext, DataSet idcDataSet, Relationship activeRelationship)
			: base(odpContext, idcDataSet)
		{
			this.m_activeRelationship = activeRelationship;
		}

		public void RegisterActiveParent(IReference<IOnDemandScopeInstance> parentScopeInstanceRef)
		{
			using (parentScopeInstanceRef.PinValue())
			{
				IOnDemandScopeInstance onDemandScopeInstance = parentScopeInstanceRef.Value();
				onDemandScopeInstance.SetupEnvironment();
				this.m_lastPrimaryKeyValues = this.m_activeRelationship.EvaluateJoinConditionKeys(true, base.m_odpContext.ReportRuntime);
				this.UpdateActiveParent(parentScopeInstanceRef);
			}
		}

		protected abstract void UpdateActiveParent(IReference<IOnDemandScopeInstance> parentScopeInstanceRef);

		protected override void SetupRelationshipQueryRestart()
		{
			base.AddRelationshipRestartPosition(this.m_activeRelationship, this.m_lastPrimaryKeyValues);
		}

		protected bool SetupCorrelatedRow(bool startWithCurrentRow)
		{
			bool advancedRowCursor = false;
			if (!startWithCurrentRow || !base.IsDataPipelineSetup)
			{
				if (!this.ReadRowFromDataSet())
				{
					return false;
				}
				advancedRowCursor = true;
			}
			bool flag = base.Correlate(this.m_activeRelationship, this.m_lastPrimaryKeyValues, advancedRowCursor);
			if (!flag)
			{
				this.PushBackLastRow();
			}
			return flag;
		}
	}
}
