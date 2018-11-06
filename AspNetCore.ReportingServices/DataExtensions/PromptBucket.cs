using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections;

namespace AspNetCore.ReportingServices.DataExtensions
{
	[Serializable]
	internal sealed class PromptBucket : ArrayList
	{
		internal new DataSourceInfo this[int index]
		{
			get
			{
				return (DataSourceInfo)base[index];
			}
		}

		internal bool NeedPrompt
		{
			get
			{
				return this.GetRepresentative().NeedPrompt;
			}
		}

		internal PromptBucket()
		{
		}

		internal DataSourceInfo GetRepresentative()
		{
			Global.Tracer.Assert(this.Count > 0, "Prompt Bucket is empty on get representative");
			return this[0];
		}

		internal bool HasItemWithLinkID(Guid linkID)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (this[i].LinkID == linkID)
				{
					return true;
				}
			}
			return false;
		}

		internal bool HasItemWithOriginalName(string originalName)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (this[i].OriginalName == originalName)
				{
					return true;
				}
			}
			return false;
		}

		internal void SetCredentials(DatasourceCredentials credentials, IDataProtection dataProtection)
		{
			int num = 0;
			while (true)
			{
				if (num < this.Count)
				{
					DataSourceInfo dataSourceInfo = this[num];
					if (dataSourceInfo.CredentialsRetrieval == DataSourceInfo.CredentialsRetrievalOption.Prompt)
					{
						dataSourceInfo.SetUserName(credentials.UserName, dataProtection);
						dataSourceInfo.SetPassword(credentials.Password, dataProtection);
						num++;
						continue;
					}
					break;
				}
				return;
			}
			throw new InternalCatalogException("Non-promptable data source appeared in prompt collection!");
		}
	}
}
