using AspNetCore.ReportingServices.DataExtensions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AspNetCore.Reporting
{
	[Serializable]
	internal class LocalCatalogTempDB : IDisposable
	{
		[Serializable]
		protected class ReportID
		{
			public string Name
			{
				get;
				private set;
			}

			public DefinitionSource Source
			{
				get;
				private set;
			}

			public Assembly EmbeddedResourceAssembly
			{
				get;
				private set;
			}

			public ReportID(string directDefinitionReportName)
				: this(directDefinitionReportName, DefinitionSource.Direct, null)
			{
			}

			public ReportID(PreviewItemContext itemContext)
				: this(itemContext.PreviewStorePath, itemContext.DefinitionSource, itemContext.EmbeddedResourceAssembly)
			{
			}

			private ReportID(string name, DefinitionSource source, Assembly embeddedResourceAssembly)
			{
				this.Name = name;
				this.Source = source;
				this.EmbeddedResourceAssembly = embeddedResourceAssembly;
			}

			public override bool Equals(object obj)
			{
				ReportID reportID = obj as ReportID;
				if (reportID == null)
				{
					return false;
				}
				if (reportID.Source == this.Source && string.Compare(reportID.Name, this.Name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return reportID.EmbeddedResourceAssembly == this.EmbeddedResourceAssembly;
				}
				return false;
			}

			public override int GetHashCode()
			{
				if (this.Name == null)
				{
					return 0;
				}
				return this.Name.GetHashCode();
			}
		}

		private Dictionary<ReportID, StoredReport> m_compiledReports = new Dictionary<ReportID, StoredReport>();

		private readonly Dictionary<string, StoredDataSet> m_compiledDataSets = new Dictionary<string, StoredDataSet>();

		public void Dispose()
		{
			foreach (StoredReport value in this.m_compiledReports.Values)
			{
				value.Dispose();
			}
			this.m_compiledReports.Clear();
			this.m_compiledDataSets.Clear();
			GC.SuppressFinalize(this);
		}

		public StoredDataSet GetCompiledDataSet(DataSetInfo dataSetInfo)
		{
			if (dataSetInfo == null)
			{
				throw new ArgumentNullException("dataSetInfo");
			}
			StoredDataSet result = default(StoredDataSet);
			this.m_compiledDataSets.TryGetValue(dataSetInfo.AbsolutePath, out result);
			return result;
		}

		public void SetCompiledDataSet(DataSetInfo dataSetInfo, StoredDataSet storedDataSet)
		{
			if (storedDataSet == null)
			{
				throw new ArgumentNullException("storedDataSet");
			}
			this.m_compiledDataSets[dataSetInfo.AbsolutePath] = storedDataSet;
		}

		public StoredReport GetCompiledReport(PreviewItemContext context)
		{
			ReportID key = new ReportID(context);
			StoredReport result = default(StoredReport);
			this.m_compiledReports.TryGetValue(key, out result);
			return result;
		}

		public void SetCompiledReport(PreviewItemContext context, StoredReport storedReport)
		{
			ReportID key = new ReportID(context);
			this.m_compiledReports[key] = storedReport;
		}
	}
}
