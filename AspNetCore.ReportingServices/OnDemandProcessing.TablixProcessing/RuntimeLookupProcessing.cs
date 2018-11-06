using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal sealed class RuntimeLookupProcessing
	{
		private RuntimeOnDemandDataSetObj m_lookupOwner;

		private OnDemandProcessingContext m_odpContext;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet m_dataSet;

		private DataSetInstance m_dataSetInstance;

		private bool m_mustBufferAllRows;

		private int m_firstRowCacheIndex = -1;

		internal bool MustBufferAllRows
		{
			get
			{
				return this.m_mustBufferAllRows;
			}
		}

		internal RuntimeLookupProcessing(OnDemandProcessingContext odpContext, AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet, DataSetInstance dataSetInstance, RuntimeOnDemandDataSetObj lookupOwner)
		{
			this.m_odpContext = odpContext;
			this.m_dataSet = dataSet;
			this.m_dataSetInstance = dataSetInstance;
			this.m_lookupOwner = lookupOwner;
			this.m_mustBufferAllRows = dataSet.HasSameDataSetLookups;
			this.InitializeRuntimeStructures();
		}

		private void InitializeRuntimeStructures()
		{
			Global.Tracer.Assert(this.m_dataSet.LookupDestinationInfos != null && this.m_dataSet.LookupDestinationInfos.Count > 0, "Attempted to perform Lookup processing on a DataSet with no Lookups");
			IScalabilityCache tablixProcessingScalabilityCache = this.m_odpContext.TablixProcessingScalabilityCache;
			Global.Tracer.Assert(tablixProcessingScalabilityCache != null, "Cannot start Lookup processing unless Scalability is setup");
			if (this.m_mustBufferAllRows)
			{
				this.m_odpContext.TablixProcessingLookupRowCache = new CommonRowCache(tablixProcessingScalabilityCache);
			}
			int count = this.m_dataSet.LookupDestinationInfos.Count;
			List<LookupObjResult> list = new List<LookupObjResult>(count);
			this.m_dataSetInstance.LookupResults = list;
			for (int i = 0; i < count; i++)
			{
				LookupDestinationInfo lookupDestinationInfo = this.m_dataSet.LookupDestinationInfos[i];
				LookupTable lookupTable = new LookupTable(tablixProcessingScalabilityCache, this.m_odpContext.ProcessingComparer, lookupDestinationInfo.UsedInSameDataSetTablixProcessing);
				LookupObjResult item = new LookupObjResult(lookupTable);
				list.Add(item);
			}
		}

		internal void NextRow()
		{
			long streamOffset = this.m_odpContext.ReportObjectModel.FieldsImpl.StreamOffset;
			int num = -1;
			CommonRowCache tablixProcessingLookupRowCache = this.m_odpContext.TablixProcessingLookupRowCache;
			if (this.m_mustBufferAllRows)
			{
				num = tablixProcessingLookupRowCache.AddRow(RuntimeDataTablixObj.SaveData(this.m_odpContext));
				if (this.m_firstRowCacheIndex == -1)
				{
					this.m_firstRowCacheIndex = num;
				}
			}
			IScalabilityCache tablixProcessingScalabilityCache = this.m_odpContext.TablixProcessingScalabilityCache;
			for (int i = 0; i < this.m_dataSet.LookupDestinationInfos.Count; i++)
			{
				LookupDestinationInfo lookupDestinationInfo = this.m_dataSet.LookupDestinationInfos[i];
				LookupObjResult lookupObjResult = this.m_dataSetInstance.LookupResults[i];
				if (!lookupObjResult.ErrorOccured)
				{
					AspNetCore.ReportingServices.RdlExpressions.VariantResult variantResult = lookupDestinationInfo.EvaluateDestExpr(this.m_odpContext, lookupObjResult);
					if (variantResult.ErrorOccurred)
					{
						lookupObjResult.DataFieldStatus = variantResult.FieldStatus;
					}
					else
					{
						object value = variantResult.Value;
						LookupTable lookupTable = lookupObjResult.GetLookupTable(this.m_odpContext);
						try
						{
							LookupMatches lookupMatches = default(LookupMatches);
							IDisposable disposable = default(IDisposable);
							if (!lookupTable.TryGetAndPinValue(value, out lookupMatches, out disposable))
							{
								lookupMatches = ((!lookupDestinationInfo.UsedInSameDataSetTablixProcessing) ? new LookupMatches() : new LookupMatchesWithRows());
								disposable = lookupTable.AddAndPin(value, lookupMatches);
							}
							if (lookupDestinationInfo.IsMultiValue || !lookupMatches.HasRow)
							{
								lookupMatches.AddRow(streamOffset, num, tablixProcessingScalabilityCache);
							}
							disposable.Dispose();
						}
						catch (ReportProcessingException_SpatialTypeComparisonError reportProcessingException_SpatialTypeComparisonError)
						{
							throw new ReportProcessingException(this.m_lookupOwner.RegisterSpatialElementComparisonError(reportProcessingException_SpatialTypeComparisonError.Type));
						}
					}
				}
			}
			if (!this.m_mustBufferAllRows)
			{
				this.m_lookupOwner.PostLookupNextRow();
			}
		}

		internal void FinishReadingRows()
		{
			if (this.m_mustBufferAllRows)
			{
				CommonRowCache tablixProcessingLookupRowCache = this.m_odpContext.TablixProcessingLookupRowCache;
				if (this.m_firstRowCacheIndex != -1)
				{
					for (int i = this.m_firstRowCacheIndex; i < tablixProcessingLookupRowCache.Count; i++)
					{
						tablixProcessingLookupRowCache.SetupRow(i, this.m_odpContext);
						this.m_lookupOwner.PostLookupNextRow();
					}
				}
			}
		}

		internal void CompleteLookupProcessing()
		{
			for (int i = 0; i < this.m_dataSetInstance.LookupResults.Count; i++)
			{
				LookupObjResult lookupObjResult = this.m_dataSetInstance.LookupResults[i];
				lookupObjResult.TransferToLookupCache(this.m_odpContext);
			}
			if (this.m_odpContext.TablixProcessingLookupRowCache != null)
			{
				this.m_odpContext.TablixProcessingLookupRowCache.Dispose();
				this.m_odpContext.TablixProcessingLookupRowCache = null;
			}
		}
	}
}
