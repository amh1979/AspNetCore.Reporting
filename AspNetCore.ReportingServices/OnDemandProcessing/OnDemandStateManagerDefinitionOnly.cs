using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class OnDemandStateManagerDefinitionOnly : OnDemandStateManager
	{
		internal override IReportScopeInstance LastROMInstance
		{
			get
			{
				this.FireAssert("LastROMInstance");
				return null;
			}
		}

		internal override IRIFReportScope LastTablixProcessingReportScope
		{
			get
			{
				this.FireAssert("LastTablixProcessingReportScope");
				return null;
			}
			set
			{
				this.FireAssert("LastTablixProcessingReportScope");
			}
		}

		internal override IInstancePath LastRIFObject
		{
			get
			{
				this.FireAssert("LastRIFObject");
				return null;
			}
			set
			{
				this.FireAssert("LastRIFObject");
			}
		}

		internal override QueryRestartInfo QueryRestartInfo
		{
			get
			{
				return null;
			}
		}

		internal override ExecutedQueryCache ExecutedQueryCache
		{
			get
			{
				return null;
			}
		}

		public OnDemandStateManagerDefinitionOnly(OnDemandProcessingContext odpContext)
			: base(odpContext)
		{
		}

		internal override ExecutedQueryCache SetupExecutedQueryCache()
		{
			return this.ExecutedQueryCache;
		}

		internal override void ResetOnDemandState()
		{
		}

		internal override int RecursiveLevel(string scopeName)
		{
			this.FireAssert("RecursiveLevel");
			return -1;
		}

		internal override bool InScope(string scopeName)
		{
			this.FireAssert("InScope");
			return false;
		}

		internal override Dictionary<string, object> GetCurrentSpecialGroupingValues()
		{
			this.FireAssert("GetCurrentSpecialGroupingValues");
			return null;
		}

		internal override void RestoreContext(IInstancePath originalObject)
		{
			this.FireAssert("RestoreContext");
		}

		internal override void SetupContext(IInstancePath rifObject, IReportScopeInstance romInstance)
		{
			this.FireAssert("SetupContext");
		}

		internal override void SetupContext(IInstancePath rifObject, IReportScopeInstance romInstance, int moveNextInstanceIndex)
		{
			this.FireAssert("SetupContext");
		}

		internal override bool CalculateAggregate(string aggregateName)
		{
			this.FireAssert("CalculateAggregate");
			return false;
		}

		internal override bool CalculateLookup(LookupInfo lookup)
		{
			this.FireAssert("CalculateLookup");
			return false;
		}

		internal override bool PrepareFieldsCollectionForDirectFields()
		{
			this.FireAssert("PrepareFieldsCollectionForDirectFields");
			return false;
		}

		internal override void EvaluateScopedFieldReference(string scopeName, int fieldIndex, ref AspNetCore.ReportingServices.RdlExpressions.VariantResult result)
		{
			this.FireAssert("EvaluateScopedFieldReference");
		}

		private void FireAssert(string methodOrPropertyName)
		{
			Global.Tracer.Assert(false, methodOrPropertyName + " should not be called in Definition-only mode.");
		}

		internal override IRecordRowReader CreateSequentialDataReader(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet, out AspNetCore.ReportingServices.ReportIntermediateFormat.DataSetInstance dataSetInstance)
		{
			this.FireAssert("CreateSequentialDataReader");
			throw new InvalidOperationException("This method is not valid for this StateManager type.");
		}

		internal override void BindNextMemberInstance(IInstancePath rifObject, IReportScopeInstance romInstance, int moveNextInstanceIndex)
		{
			this.FireAssert("BindNextMemberInstance");
			throw new InvalidOperationException("This method is not valid for this StateManager type.");
		}

		internal override bool ShouldStopPipelineAdvance(bool rowAccepted)
		{
			this.FireAssert("ShouldStopPipelineAdvance");
			throw new InvalidOperationException("This method is not valid for this StateManager type.");
		}

		internal override void CreatedScopeInstance(IRIFReportDataScope scope)
		{
			this.FireAssert("CreateScopeInstance");
			throw new InvalidOperationException("This method is not valid for this StateManager type.");
		}

		internal override bool ProcessOneRow(IRIFReportDataScope scope)
		{
			this.FireAssert("ProcessOneRow");
			throw new InvalidOperationException("This method is not valid for this StateManager type.");
		}

		internal override bool CheckForPrematureServerAggregate(string aggregateName)
		{
			this.FireAssert("CheckForPrematureServerAggregate");
			throw new InvalidOperationException("This method is not valid for this StateManager type.");
		}
	}
}
