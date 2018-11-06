using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Library;
using AspNetCore.ReportingServices.ReportProcessing;
using System;

namespace AspNetCore.Reporting
{
	internal static class ReportCompiler
	{
		public static PublishingResult CompileReport(ICatalogItemContext context, byte[] reportDefinition, bool generateExpressionHostWithRefusedPermissions, out ControlSnapshot snapshot)
		{
			PublishingResult publishingResult = null;
			snapshot = null;
			
			try
			{
				ReportProcessing reportProcessing = new ReportProcessing();
				snapshot = new ControlSnapshot();
                AppDomain appDomain = AppDomain.CurrentDomain;
                PublishingContext reportPublishingContext = new PublishingContext(context, reportDefinition, snapshot, appDomain, generateExpressionHostWithRefusedPermissions,
                    snapshot.ReportProcessingFlags, reportProcessing.Configuration, DataProtectionLocal.Instance);
				return reportProcessing.CreateIntermediateFormat(reportPublishingContext);
			}
			catch (Exception inner)
			{
				string text = context.ItemPathAsString;
				if (text == null)
				{
					text = ProcessingStrings.MainReport;
				}
				throw new DefinitionInvalidException(text, inner);
			}
		}
	}
}
