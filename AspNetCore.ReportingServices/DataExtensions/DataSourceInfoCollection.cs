using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using System;
using System.Collections;
using System.Globalization;
using System.Xml;

namespace AspNetCore.ReportingServices.DataExtensions
{
	[Serializable]
	internal sealed class DataSourceInfoCollection : IPowerViewDataSourceCollection, IEnumerable
	{
		private Hashtable m_collection = new Hashtable();

		public int Count
		{
			get
			{
				return this.m_collection.Count;
			}
		}

		public DataSourceInfoCollection()
		{
		}

		public DataSourceInfoCollection(DataSourceInfoCollection other)
		{
			RSTrace processingTracer = RSTrace.ProcessingTracer;
			processingTracer.Assert(null != other);
			processingTracer.Assert(null != other.m_collection);
			this.m_collection = (Hashtable)other.m_collection.Clone();
		}

		public DataSourceInfoCollection(string dataSourcesXml, IDataProtection dataProtection)
		{
			this.ConstructFromXml(dataSourcesXml, false, dataProtection);
		}

		public DataSourceInfoCollection(string dataSourcesXml, bool clientLoad, IDataProtection dataProtection)
		{
			this.ConstructFromXml(dataSourcesXml, clientLoad, dataProtection);
		}

		private void ConstructFromXml(string dataSourcesXml, bool clientLoad, IDataProtection dataProtection)
		{
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				XmlUtil.SafeOpenXmlDocumentString(xmlDocument, dataSourcesXml);
			}
			catch (XmlException ex)
			{
				throw new MalformedXmlException(ex);
			}
			try
			{
				XmlNode xmlNode = xmlDocument.SelectSingleNode("/DataSources");
				if (xmlNode == null)
				{
					throw new InvalidXmlException();
				}
				foreach (XmlNode childNode in xmlNode.ChildNodes)
				{
					DataSourceInfo dataSource = DataSourceInfo.ParseDataSourceNode(childNode, clientLoad, dataProtection);
					this.Add(dataSource);
				}
			}
			catch (XmlException)
			{
				throw new InvalidXmlException();
			}
		}

		public DataSourceInfo GetTheOnlyDataSource()
		{
			if (this.Count != 1)
			{
				throw new InternalCatalogException(string.Format(CultureInfo.CurrentCulture, "Data source collection for a standalone datasource contains {0} items, must be 1.", this.Count));
			}
			IEnumerator enumerator = this.GetEnumerator();
			try
			{
				if (enumerator.MoveNext())
				{
					return (DataSourceInfo)enumerator.Current;
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return null;
		}

		public DataSourceInfoCollection CombineOnSetDefinition(DataSourceInfoCollection newDataSources)
		{
			return this.CombineOnSetDefinition(newDataSources, false, true);
		}

		public DataSourceInfoCollection CombineOnSetDefinitionWithoutSideEffects(DataSourceInfoCollection newDataSources)
		{
			return this.CombineOnSetDefinition(newDataSources, false, false);
		}

		public DataSourceInfoCollection CombineOnSetDefinitionKeepOriginalDataSourceId(DataSourceInfoCollection newDataSources)
		{
			return this.CombineOnSetDefinition(newDataSources, true, true);
		}

		private DataSourceInfoCollection CombineOnSetDefinition(DataSourceInfoCollection newDataSources, bool keepOriginalDataSourceId, bool overrideOriginalConnectString)
		{
			DataSourceInfoCollection dataSourceInfoCollection = new DataSourceInfoCollection();
			foreach (DataSourceInfo newDataSource in newDataSources)
			{
				DataSourceInfo byOriginalName = this.GetByOriginalName(newDataSource.OriginalName);
				if (byOriginalName == null)
				{
					dataSourceInfoCollection.Add(newDataSource);
				}
				else
				{
					if (!keepOriginalDataSourceId)
					{
						byOriginalName.ID = newDataSource.ID;
					}
					if (overrideOriginalConnectString)
					{
						byOriginalName.SetOriginalConnectionString(newDataSource.OriginalConnectionStringEncrypted);
						byOriginalName.SetOriginalConnectStringExpressionBased(newDataSource.OriginalConnectStringExpressionBased);
					}
					dataSourceInfoCollection.Add(byOriginalName);
				}
			}
			return dataSourceInfoCollection;
		}

		public DataSourceInfoCollection CombineOnSetDataSources(DataSourceInfoCollection newDataSources)
		{
			DataSourceInfoCollection dataSourceInfoCollection = new DataSourceInfoCollection();
			foreach (DataSourceInfo newDataSource in newDataSources)
			{
				DataSourceInfo byOriginalName = this.GetByOriginalName(newDataSource.OriginalName);
				if (byOriginalName == null)
				{
					throw new DataSourceNotFoundException(newDataSource.OriginalName);
				}
				newDataSource.ID = byOriginalName.ID;
				newDataSource.SetOriginalConnectionString(byOriginalName.OriginalConnectionStringEncrypted);
				newDataSource.SetOriginalConnectStringExpressionBased(byOriginalName.OriginalConnectStringExpressionBased);
				dataSourceInfoCollection.Add(newDataSource);
			}
			foreach (DataSourceInfo item in this)
			{
				DataSourceInfo byOriginalName2 = newDataSources.GetByOriginalName(item.OriginalName);
				if (byOriginalName2 == null)
				{
					dataSourceInfoCollection.Add(item);
				}
			}
			return dataSourceInfoCollection;
		}

		public bool TryGetCachedDataSourceId(string dataSourceName, out Guid dataSourceId)
		{
			dataSourceId = Guid.Empty;
			DataSourceInfo byOriginalName = this.GetByOriginalName(dataSourceName);
			if (byOriginalName != null)
			{
				dataSourceId = byOriginalName.ID;
				return true;
			}
			return false;
		}

		public void Add(DataSourceInfo dataSource)
		{
			if (dataSource.OriginalName == null)
			{
				RSTrace processingTracer = RSTrace.ProcessingTracer;
				processingTracer.Assert(this.m_collection.Count == 0, "Adding more than one data source with null original name");
				this.m_collection.Add("", dataSource);
			}
			else if (!this.m_collection.ContainsKey(dataSource.OriginalName))
			{
				this.m_collection.Add(dataSource.OriginalName, dataSource);
			}
		}

		public DataSourceInfo GetByOriginalName(string name)
		{
			return (DataSourceInfo)this.m_collection[name];
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_collection.Values.GetEnumerator();
		}

		public bool GoodForDataCaching()
		{
			foreach (DataSourceInfo value in this.m_collection.Values)
			{
				if (value.CredentialsRetrieval == DataSourceInfo.CredentialsRetrievalOption.Prompt)
				{
					return false;
				}
				if (value.HasConnectionStringUseridReference)
				{
					return false;
				}
			}
			return true;
		}

		public bool HasConnectionStringUseridReference()
		{
			foreach (DataSourceInfo value in this.m_collection.Values)
			{
				if (value.HasConnectionStringUseridReference)
				{
					return true;
				}
			}
			return false;
		}

		public void AddOrUpdate(string key, DataSourceInfo dsInfo)
		{
			RSTrace.ProcessingTracer.Assert(key == ((dsInfo.OriginalName != null) ? dsInfo.OriginalName : string.Empty), "DataSourceInfo.AddOrUpdate: (dsInfo.OriginalName != null ? dsInfo.OriginalName : string.Empty)");
			if (this.m_collection.ContainsKey(key))
			{
				this.m_collection.Remove(key);
			}
			this.Add(dsInfo);
		}

		public DataSourceInfo GetDataSourceFromKey(string key)
		{
			return this.GetByOriginalName(key);
		}
	}
}
