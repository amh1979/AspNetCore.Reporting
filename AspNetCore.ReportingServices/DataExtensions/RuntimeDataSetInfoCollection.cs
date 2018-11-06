using AspNetCore.ReportingServices.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AspNetCore.ReportingServices.DataExtensions
{
	[Serializable]
	internal sealed class RuntimeDataSetInfoCollection
	{
		private Dictionary<Guid, DataSetInfo> m_collectionByID;

		private Dictionary<string, DataSetInfoCollection> m_collectionByReport;

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

		public static RuntimeDataSetInfoCollection Deserialize(byte[] data)
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
				return (RuntimeDataSetInfoCollection)binaryFormatter.Deserialize(memoryStream);
			}
			finally
			{
				if (memoryStream != null)
				{
					memoryStream.Close();
				}
			}
		}

		internal DataSetInfo GetByID(Guid ID)
		{
			DataSetInfo result = null;
			if (this.m_collectionByID != null)
			{
				this.m_collectionByID.TryGetValue(ID, out result);
			}
			return result;
		}

		internal DataSetInfo GetByName(string name, ICatalogItemContext item)
		{
			DataSetInfo result = null;
			if (this.m_collectionByReport != null)
			{
				DataSetInfoCollection dataSetInfoCollection = null;
				if (this.m_collectionByReport.TryGetValue(item.StableItemPath, out dataSetInfoCollection))
				{
					result = dataSetInfoCollection.GetByName(name);
				}
			}
			return result;
		}

		internal void Add(DataSetInfo dataSet, ICatalogItemContext report)
		{
			if (Guid.Empty == dataSet.ID)
			{
				this.AddToCollectionByReport(dataSet, report);
			}
			else
			{
				this.AddToCollectionByID(dataSet);
			}
		}

		private void AddToCollectionByReport(DataSetInfo dataSet, ICatalogItemContext report)
		{
			DataSetInfoCollection dataSetInfoCollection = null;
			if (this.m_collectionByReport == null)
			{
				this.m_collectionByReport = new Dictionary<string, DataSetInfoCollection>(StringComparer.Ordinal);
			}
			else
			{
				this.m_collectionByReport.TryGetValue(report.StableItemPath, out dataSetInfoCollection);
			}
			if (dataSetInfoCollection == null)
			{
				dataSetInfoCollection = new DataSetInfoCollection();
				this.m_collectionByReport.Add(report.StableItemPath, dataSetInfoCollection);
			}
			dataSetInfoCollection.Add(dataSet);
		}

		private void AddToCollectionByID(DataSetInfo dataSet)
		{
			if (this.m_collectionByID == null)
			{
				this.m_collectionByID = new Dictionary<Guid, DataSetInfo>();
			}
			else if (this.m_collectionByID.ContainsKey(dataSet.ID))
			{
				return;
			}
			this.m_collectionByID.Add(dataSet.ID, dataSet);
		}
	}
}
