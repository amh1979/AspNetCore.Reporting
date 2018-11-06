using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public sealed class LookupImpl : Lookup
	{
		private LookupInfo m_lookupInfo;

		private AspNetCore.ReportingServices.RdlExpressions.ReportRuntime m_reportRuntime;

		private static readonly object[] EmptyResult = new object[0];

		public override object Value
		{
			get
			{
				object result = null;
				object[] array = this.EvaluateLookup();
				if (array != null && array.Length > 0)
				{
					result = array[0];
				}
				return result;
			}
		}

		public override object[] Values
		{
			get
			{
				return this.EvaluateLookup();
			}
		}

		internal string Name
		{
			get
			{
				return this.m_lookupInfo.Name;
			}
		}

		internal LookupImpl(LookupInfo lookupInfo, AspNetCore.ReportingServices.RdlExpressions.ReportRuntime reportRuntime)
		{
			this.m_lookupInfo = lookupInfo;
			this.m_reportRuntime = reportRuntime;
		}

		internal object[] EvaluateLookup()
		{
			bool flag = this.m_lookupInfo.ReturnFirstMatchOnly();
			OnDemandProcessingContext odpContext = this.m_reportRuntime.ReportObjectModel.OdpContext;
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet = odpContext.ReportDefinition.MappingDataSetIndexToDataSet[this.m_lookupInfo.DataSetIndexInCollection];
			DataSetInstance dataSetInstance = odpContext.GetDataSetInstance(dataSet);
			if (dataSetInstance == null)
			{
				throw new ReportProcessingException_InvalidOperationException();
			}
			if (dataSetInstance.NoRows)
			{
				return LookupImpl.EmptyResult;
			}
			if (dataSetInstance.LookupResults == null || dataSetInstance.LookupResults[this.m_lookupInfo.DestinationIndexInCollection] == null)
			{
				if (!odpContext.CalculateLookup(this.m_lookupInfo))
				{
					return LookupImpl.EmptyResult;
				}
				Global.Tracer.Assert(null != dataSetInstance.LookupResults, "Lookup not initialized correctly by tablix processing");
			}
			LookupObjResult lookupObjResult = dataSetInstance.LookupResults[this.m_lookupInfo.DestinationIndexInCollection];
			if (lookupObjResult.ErrorOccured)
			{
				IErrorContext reportRuntime = this.m_reportRuntime;
				if (lookupObjResult.DataFieldStatus == DataFieldStatus.None && lookupObjResult.ErrorCode != 0)
				{
					reportRuntime.Register(lookupObjResult.ErrorCode, lookupObjResult.ErrorSeverity, lookupObjResult.ErrorMessageArgs);
				}
				else if (lookupObjResult.DataFieldStatus == DataFieldStatus.UnSupportedDataType)
				{
					reportRuntime.Register(ProcessingErrorCode.rsLookupOfInvalidExpressionDataType, Severity.Warning, lookupObjResult.ErrorMessageArgs);
				}
				throw new ReportProcessingException_InvalidOperationException();
			}
			AspNetCore.ReportingServices.RdlExpressions.VariantResult result = this.m_lookupInfo.EvaluateSourceExpr(this.m_reportRuntime);
			this.CheckExprResultError(result);
			bool flag2 = lookupObjResult.HasBeenTransferred || odpContext.CurrentDataSetIndex != dataSet.IndexInCollection;
			List<object> list = null;
			CompareInfo compareInfo = null;
			CompareOptions clrCompareOptions = CompareOptions.None;
			bool nullsAsBlanks = false;
			bool useOrdinalStringKeyGeneration = false;
			try
			{
				if (flag2)
				{
					compareInfo = odpContext.CompareInfo;
					clrCompareOptions = odpContext.ClrCompareOptions;
					nullsAsBlanks = odpContext.NullsAsBlanks;
					useOrdinalStringKeyGeneration = odpContext.UseOrdinalStringKeyGeneration;
					dataSetInstance.SetupCollationSettings(odpContext);
				}
				LookupTable lookupTable = lookupObjResult.GetLookupTable(odpContext);
				Global.Tracer.Assert(lookupTable != null, "LookupTable must not be null");
				ObjectModelImpl reportObjectModel = odpContext.ReportObjectModel;
				AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader dataChunkReader = null;
				if (flag2)
				{
					dataChunkReader = odpContext.GetDataChunkReader(dataSet.IndexInCollection);
				}
				using (reportObjectModel.SetupNewFieldsWithBackup(dataSet, dataSetInstance, dataChunkReader))
				{
					object[] array = result.Value as object[];
					if (array == null)
					{
						array = new object[1]
						{
							result.Value
						};
					}
					else
					{
						list = new List<object>(array.Length);
					}
					object[] array2 = array;
					foreach (object key in array2)
					{
						LookupMatches lookupMatches = default(LookupMatches);
						if (lookupTable.TryGetValue(key, out lookupMatches))
						{
							int num = flag ? 1 : lookupMatches.MatchCount;
							if (list == null)
							{
								list = new List<object>(num);
							}
							for (int j = 0; j < num; j++)
							{
								lookupMatches.SetupRow(j, odpContext);
								AspNetCore.ReportingServices.RdlExpressions.VariantResult result2 = this.m_lookupInfo.EvaluateResultExpr(this.m_reportRuntime);
								this.CheckExprResultError(result2);
								list.Add(result2.Value);
							}
						}
					}
				}
			}
			finally
			{
				if (compareInfo != null)
				{
					odpContext.SetComparisonInformation(compareInfo, clrCompareOptions, nullsAsBlanks, useOrdinalStringKeyGeneration);
				}
			}
			object[] result3 = LookupImpl.EmptyResult;
			if (list != null)
			{
				result3 = list.ToArray();
			}
			return result3;
		}

		private void CheckExprResultError(AspNetCore.ReportingServices.RdlExpressions.VariantResult result)
		{
			if (!result.ErrorOccurred)
			{
				return;
			}
			throw new ReportProcessingException_InvalidOperationException();
		}
	}
}
