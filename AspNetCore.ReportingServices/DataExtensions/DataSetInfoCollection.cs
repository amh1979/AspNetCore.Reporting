using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.DataExtensions
{
	[Serializable]
	internal sealed class DataSetInfoCollection
	{
		private Dictionary<string, DataSetInfo> m_dataSetsByName;

		private Dictionary<Guid, DataSetInfo> m_dataSetsByID;

		public int Count
		{
			get
			{
				return this.m_dataSetsByID.Count;
			}
		}

		public DataSetInfoCollection()
		{
			this.m_dataSetsByID = new Dictionary<Guid, DataSetInfo>();
			this.m_dataSetsByName = new Dictionary<string, DataSetInfo>(StringComparer.Ordinal);
		}

		public IEnumerator<DataSetInfo> GetEnumerator()
		{
			return (IEnumerator<DataSetInfo>)(object)this.m_dataSetsByID.Values.GetEnumerator();
		}

		public void Add(DataSetInfo dataSet)
		{
			this.m_dataSetsByID.Add(dataSet.ID, dataSet);
			if (!this.m_dataSetsByName.ContainsKey(dataSet.DataSetName))
			{
				this.m_dataSetsByName.Add(dataSet.DataSetName, dataSet);
			}
		}

		public DataSetInfo GetByName(string name)
		{
			DataSetInfo result = null;
			if (this.m_dataSetsByName != null)
			{
				this.m_dataSetsByName.TryGetValue(name, out result);
			}
			return result;
		}

		public void CombineOnSetDataSets(DataSetInfoCollection newDataSets)
		{
			if (newDataSets != null)
			{
				foreach (DataSetInfo newDataSet in newDataSets)
				{
					DataSetInfo byName = this.GetByName(newDataSet.DataSetName);
					if (byName == null)
					{
						throw new DataSetNotFoundException(newDataSet.DataSetName.MarkAsPrivate());
					}
					byName.AbsolutePath = newDataSet.AbsolutePath;
					byName.LinkedSharedDataSetID = Guid.Empty;
				}
			}
		}

		public DataSetInfoCollection CombineOnSetDefinition(DataSetInfoCollection newDataSets)
		{
			DataSetInfoCollection dataSetInfoCollection = new DataSetInfoCollection();
			if (newDataSets == null)
			{
				return dataSetInfoCollection;
			}
			foreach (DataSetInfo newDataSet in newDataSets)
			{
				DataSetInfo byName = this.GetByName(newDataSet.DataSetName);
				if (byName == null)
				{
					dataSetInfoCollection.Add(newDataSet);
				}
				else
				{
					byName.ID = newDataSet.ID;
					dataSetInfoCollection.Add(byName);
				}
			}
			return dataSetInfoCollection;
		}
	}
}
