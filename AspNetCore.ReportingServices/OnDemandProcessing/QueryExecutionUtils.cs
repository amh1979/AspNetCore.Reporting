using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal static class QueryExecutionUtils
	{
		internal static void DisposeDataExtensionObject<T>(ref T obj, string objectType, string dataSetName) where T : class, IDisposable
		{
			QueryExecutionUtils.DisposeDataExtensionObject<T>(ref obj, objectType, dataSetName, (DataProcessingMetrics)null, (DataProcessingMetrics.MetricType?)null);
		}

		internal static void DisposeDataExtensionObject<T>(ref T obj, string objectType, string dataSetName, DataProcessingMetrics executionMetrics, DataProcessingMetrics.MetricType? metricType) where T : class, IDisposable
		{
			if (obj != null)
			{
				if (metricType.HasValue)
				{
					executionMetrics.StartTimer(metricType.Value);
				}
				try
				{
					obj.Dispose();
				}
				catch (RSException)
				{
					throw;
				}
				catch (Exception ex2)
				{
					if (AsynchronousExceptionDetection.IsStoppingException(ex2))
					{
						throw;
					}
					Global.Tracer.Trace(TraceLevel.Warning, "Error occurred while disposing the " + objectType + " for DataSet '" + dataSetName.MarkAsPrivate() + "'. Details: " + ex2.ToString());
				}
				finally
				{
					obj = null;
					if (metricType.HasValue)
					{
						executionMetrics.RecordTimerMeasurementWithUpdatedTotal(metricType.Value);
					}
				}
			}
		}
	}
}
