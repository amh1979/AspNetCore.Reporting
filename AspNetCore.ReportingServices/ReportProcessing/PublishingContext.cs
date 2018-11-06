using AspNetCore.ReportingServices.DataExtensions;
using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.ReportPublishing;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class PublishingContext : PublishingContextBase
	{
		private byte[] m_definition;

		internal byte[] Definition
		{
			get
			{
				return this.m_definition;
			}
		}

		public PublishingContext(ICatalogItemContext catalogContext, byte[] datasetDefinition, IChunkFactory createChunkFactory, AppDomain compilationTempAppDomain, bool generateExpressionHostWithRefusedPermissions, ReportProcessing.CheckSharedDataSource checkDataSourceCallback, IConfiguration configuration)
			: base(PublishingContextKind.SharedDataSet, catalogContext, createChunkFactory, compilationTempAppDomain, generateExpressionHostWithRefusedPermissions, ReportProcessingFlags.NotSet, checkDataSourceCallback, null, null, null, null, null, configuration, null, false, false, false, false)
		{
			this.m_definition = datasetDefinition;
		}

		public PublishingContext(ICatalogItemContext catalogContext, byte[] reportDefinition, IChunkFactory createChunkFactory, AppDomain compilationTempAppDomain, bool generateExpressionHostWithRefusedPermissions, ReportProcessingFlags processingFlags, IConfiguration configuration, IDataProtection dataProtection)
			: this(catalogContext, reportDefinition, createChunkFactory, compilationTempAppDomain, generateExpressionHostWithRefusedPermissions, processingFlags, null, null, null, null, null, null, configuration, dataProtection, false, false, false)
		{
		}

		public PublishingContext(ICatalogItemContext catalogContext, byte[] reportDefinition, IChunkFactory createChunkFactory, AppDomain compilationTempAppDomain, bool generateExpressionHostWithRefusedPermissions, ReportProcessingFlags processingFlags, ReportProcessing.CheckSharedDataSource checkDataSourceCallback, ReportProcessing.ResolveTemporaryDataSource resolveTemporaryDataSourceCallback, DataSourceInfoCollection originalDataSources, ReportProcessing.CheckSharedDataSet checkDataSetCallback, ReportProcessing.ResolveTemporaryDataSet resolveTemporaryDataSetCallback, DataSetInfoCollection originalDataSets, IConfiguration configuration, IDataProtection dataProtection,
            bool isInternalRepublish, bool isPackagedReportArchive, bool isRdlx)
			: base(PublishingContextKind.Full, catalogContext, createChunkFactory, compilationTempAppDomain, generateExpressionHostWithRefusedPermissions, processingFlags, checkDataSourceCallback, resolveTemporaryDataSourceCallback, originalDataSources, checkDataSetCallback, resolveTemporaryDataSetCallback, originalDataSets, configuration, dataProtection,
                  isInternalRepublish, isPackagedReportArchive, isRdlx, false)
		{
			this.m_definition = reportDefinition;
		}
	}
}
