using AspNetCore.Reporting;
using AspNetCore.ReportingServices.DataExtensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices
{
	[Serializable]
	internal sealed class StandalonePreviewStore : ILocalCatalog
	{
		private Dictionary<string, byte[]> m_directReportDefinitions = new Dictionary<string, byte[]>();

		public byte[] GetResource(string resourcePath, out string mimeType)
		{
			mimeType = null;
			return null;
		}

		public DataSetInfo GetDataSet(string dataSetPath)
		{
			return null;
		}

		public DataSourceInfo GetDataSource(string dataSourcePath)
		{
			return null;
		}

		public byte[] GetReportDefinition(PreviewItemContext itemContext)
		{
			byte[] array = null;
			Exception ex = null;
			try
			{
				if (itemContext.DefinitionSource == DefinitionSource.Direct)
				{
					this.m_directReportDefinitions.TryGetValue(itemContext.PreviewStorePath, out array);
				}
				else
				{
					Stream stream = null;
					try
					{
						if (itemContext.DefinitionSource == DefinitionSource.File)
						{
							stream = File.OpenRead(itemContext.PreviewStorePath);
						}
						else if (itemContext.DefinitionSource == DefinitionSource.EmbeddedResource)
						{
							stream = itemContext.EmbeddedResourceAssembly.GetManifestResourceStream(itemContext.PreviewStorePath);
						}
						array = new byte[stream.Length];
						stream.Read(array, 0, (int)stream.Length);
					}
					finally
					{
						if (stream != null)
						{
							stream.Close();
						}
					}
				}
			}
			catch (Exception ex2)
			{
				ex = ex2;
			}
			if (array != null && ex == null)
			{
				return array;
			}
			throw new ApplicationException(ProcessingStrings.MissingDefinition(itemContext.ItemName), ex);
		}

		public void SetReportDefinition(string reportName, byte[] reportBytes)
		{
			this.m_directReportDefinitions[reportName] = reportBytes;
		}

		public bool HasDirectReportDefinition(string reportName)
		{
			return this.m_directReportDefinitions.ContainsKey(reportName);
		}

		public void GetReportDataSourceCredentials(PreviewItemContext itemContext, string dataSourceName, out string userName, out string password)
		{
			userName = (password = null);
		}

		public string GetReportDataFileInfo(PreviewItemContext itemContext, out bool isOutOfDate)
		{
			isOutOfDate = false;
			return null;
		}

		public void UpdateReportDataFileStatus(PreviewItemContext itemContext, bool isOutOfDate)
		{
			throw new NotSupportedException();
		}
	}
}
