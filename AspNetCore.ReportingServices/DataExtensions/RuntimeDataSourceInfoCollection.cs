using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace AspNetCore.ReportingServices.DataExtensions
{
	[Serializable]
	internal sealed class RuntimeDataSourceInfoCollection
	{
		private Hashtable m_collectionByID;

		private Hashtable m_collectionByReport;

		private CollectionByPrompt m_collectionByPrompt;

		public bool NeedPrompt
		{
			get
			{
				if (this.m_collectionByPrompt == null)
				{
					return false;
				}
				return this.m_collectionByPrompt.NeedPrompt;
			}
		}

		public RuntimeDataSourceInfoCollection()
		{
		}

		public RuntimeDataSourceInfoCollection(RuntimeDataSourceInfoCollection other)
		{
			Global.Tracer.Assert(null != other, "(null != other)");
			if (other.m_collectionByID != null)
			{
				this.m_collectionByID = (Hashtable)other.m_collectionByID.Clone();
			}
			if (other.m_collectionByReport != null)
			{
				this.m_collectionByReport = (Hashtable)other.m_collectionByReport.Clone();
			}
			if (other.m_collectionByPrompt != null)
			{
				this.m_collectionByPrompt = other.m_collectionByPrompt.Clone();
			}
		}

		public RuntimeDataSourceInfoCollection(SerializationInfo info, StreamingContext context)
		{
			this.m_collectionByID = (Hashtable)info.GetValue("dscollectionbyid", typeof(Hashtable));
			this.m_collectionByReport = (Hashtable)info.GetValue("dscollectionbyreport", typeof(Hashtable));
			this.m_collectionByPrompt = (CollectionByPrompt)info.GetValue("dscollectionbyprompt", typeof(CollectionByPrompt));
		}

		public byte[] Serialize()
		{
			MemoryStream memoryStream = null;
			try
			{
				memoryStream = new MemoryStream();
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(memoryStream, this);
				return memoryStream.ToArray();
			}
			finally
			{
				if (memoryStream != null)
				{
					memoryStream.Close();
				}
			}
		}

		public static RuntimeDataSourceInfoCollection Deserialize(byte[] data)
		{
			if (data == null)
			{
				return null;
			}
			MemoryStream memoryStream = null;
			try
			{
				memoryStream = new MemoryStream(data, false);
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				return (RuntimeDataSourceInfoCollection)binaryFormatter.Deserialize(memoryStream);
			}
			finally
			{
				if (memoryStream != null)
				{
					memoryStream.Close();
				}
			}
		}

		public void SetCredentials(DatasourceCredentialsCollection allCredentials, IDataProtection dataProtection)
		{
			if (allCredentials != null)
			{
				foreach (DatasourceCredentials allCredential in allCredentials)
				{
					this.SetCredentials(allCredential, dataProtection);
				}
			}
		}

		private void SetCredentials(DatasourceCredentials credentials, IDataProtection dataProtection)
		{
			string promptID = credentials.PromptID;
			if (this.m_collectionByPrompt == null)
			{
				if (this.GetByOriginalName(promptID) != null)
				{
					throw new DataSourceNoPromptException(promptID);
				}
				throw new DataSourceNotFoundException(promptID);
			}
			PromptBucket bucketByOriginalName = this.m_collectionByPrompt.GetBucketByOriginalName(promptID);
			if (bucketByOriginalName == null)
			{
				if (this.GetByOriginalName(promptID) != null)
				{
					throw new DataSourceNoPromptException(promptID);
				}
				throw new DataSourceNotFoundException(promptID);
			}
			bucketByOriginalName.SetCredentials(credentials, dataProtection);
		}

		public DataSourceInfo GetByOriginalName(string originalName)
		{
			if (this.m_collectionByID == null)
			{
				return null;
			}
			foreach (DataSourceInfo value in this.m_collectionByID.Values)
			{
				if (value.OriginalName == originalName)
				{
					return value;
				}
			}
			return null;
		}

		public bool CredentialsAreSame(DatasourceCredentialsCollection creds, bool noCredentialsMeansSame, IDataProtection dataProtection)
		{
			if (noCredentialsMeansSame && (creds == null || creds.Count == 0))
			{
				return true;
			}
			if ((this.m_collectionByPrompt == null || this.m_collectionByPrompt.Count == 0) != (creds == null || creds.Count == 0))
			{
				return false;
			}
			if (creds != null && creds.Count != 0)
			{
				if (creds.Count != this.m_collectionByPrompt.Count)
				{
					return false;
				}
				foreach (DatasourceCredentials cred in creds)
				{
					DataSourceInfo representative = this.m_collectionByPrompt.GetBucketByOriginalName(cred.PromptID).GetRepresentative();
					if (representative == null)
					{
						return false;
					}
					if (representative.CredentialsRetrieval != DataSourceInfo.CredentialsRetrievalOption.Prompt)
					{
						return false;
					}
					if (representative.GetPasswordDecrypted(dataProtection) != cred.Password)
					{
						return false;
					}
					if (representative.GetUserName(dataProtection) != cred.UserName)
					{
						return false;
					}
				}
				return true;
			}
			return true;
		}

		public bool TrueForAll(Predicate<DataSourceInfo> predicate)
		{
			if (this.m_collectionByID != null)
			{
				foreach (DataSourceInfo value in this.m_collectionByID.Values)
				{
					if (!predicate(value))
					{
						return false;
					}
				}
			}
			if (this.m_collectionByReport != null)
			{
				foreach (DataSourceInfoCollection value2 in this.m_collectionByReport.Values)
				{
					foreach (DataSourceInfo item in value2)
					{
						if (!predicate(item))
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		public bool GoodForDataCaching()
		{
			if (this.m_collectionByID != null)
			{
				foreach (DataSourceInfo value in this.m_collectionByID.Values)
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
			}
			if (this.m_collectionByReport != null)
			{
				foreach (DataSourceInfoCollection value2 in this.m_collectionByReport.Values)
				{
					if (!value2.GoodForDataCaching())
					{
						return false;
					}
				}
			}
			return true;
		}

		public bool HasConnectionStringUseridReference()
		{
			if (this.m_collectionByID != null)
			{
				foreach (DataSourceInfo value in this.m_collectionByID.Values)
				{
					if (value.HasConnectionStringUseridReference)
					{
						return true;
					}
				}
			}
			if (this.m_collectionByReport != null)
			{
				foreach (DataSourceInfoCollection value2 in this.m_collectionByReport.Values)
				{
					if (value2.HasConnectionStringUseridReference())
					{
						return true;
					}
				}
			}
			return false;
		}

		internal void Add(DataSourceInfo dataSource, ICatalogItemContext report)
		{
			if (Guid.Empty == dataSource.ID)
			{
				this.AddToCollectionByReport(dataSource, report);
			}
			else
			{
				this.AddToCollectionByID(dataSource);
			}
			this.CheckedAddByPrompt(dataSource);
		}

		internal DataSourceInfo GetForSharedDataSetExecution()
		{
			if (this.m_collectionByID != null)
			{
				Global.Tracer.Assert(1 == this.m_collectionByID.Count, "Shared dataset: RuntimeDataSourceInfoCollection must contain 1 data source");
				{
					IEnumerator enumerator = this.m_collectionByID.Values.GetEnumerator();
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
				}
			}
			return null;
		}

		internal DataSourceInfo GetByID(Guid ID)
		{
			if (this.m_collectionByID != null)
			{
				return (DataSourceInfo)this.m_collectionByID[ID];
			}
			return null;
		}

		internal DataSourceInfo GetByName(string name, ICatalogItemContext report)
		{
			if (this.m_collectionByReport != null)
			{
				DataSourceInfoCollection dataSourceInfoCollection = (DataSourceInfoCollection)this.m_collectionByReport[report.StableItemPath];
				if (dataSourceInfoCollection != null)
				{
					return dataSourceInfoCollection.GetByOriginalName(name);
				}
			}
			return null;
		}

		public DataSourcePromptCollection GetPromptRepresentatives(ServerDataSourceSettings serverDatasourceSettings)
		{
			if (this.m_collectionByPrompt == null)
			{
				return new DataSourcePromptCollection();
			}
			return this.m_collectionByPrompt.GetPromptRepresentatives(serverDatasourceSettings);
		}

		private void AddToCollectionByReport(DataSourceInfo dataSource, ICatalogItemContext report)
		{
			DataSourceInfoCollection dataSourceInfoCollection = null;
			if (this.m_collectionByReport == null)
			{
				this.m_collectionByReport = new Hashtable();
			}
			else
			{
				dataSourceInfoCollection = (DataSourceInfoCollection)this.m_collectionByReport[report.StableItemPath];
			}
			if (dataSourceInfoCollection == null)
			{
				dataSourceInfoCollection = new DataSourceInfoCollection();
				this.m_collectionByReport.Add(report.StableItemPath, dataSourceInfoCollection);
			}
			dataSourceInfoCollection.Add(dataSource);
		}

		private void AddToCollectionByID(DataSourceInfo dataSource)
		{
			if (this.m_collectionByID == null)
			{
				this.m_collectionByID = new Hashtable();
			}
			else if (this.m_collectionByID.ContainsKey(dataSource.ID))
			{
				return;
			}
			this.m_collectionByID.Add(dataSource.ID, dataSource);
		}

		private void CheckedAddByPrompt(DataSourceInfo dataSource)
		{
			if (dataSource.CredentialsRetrieval == DataSourceInfo.CredentialsRetrievalOption.Prompt)
			{
				if (this.m_collectionByPrompt == null)
				{
					this.m_collectionByPrompt = new CollectionByPrompt();
				}
				this.m_collectionByPrompt.CheckedAdd(dataSource);
			}
		}
	}
}
