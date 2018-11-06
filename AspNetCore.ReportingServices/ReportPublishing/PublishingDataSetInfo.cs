using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportPublishing
{
	internal sealed class PublishingDataSetInfo
	{
		private string m_dataSetName;

		private Dictionary<string, bool> m_parameterNames;

		private bool m_isComplex;

		private int m_dataSetDefIndex = -1;

		private int m_dataSourceIndex = -1;

		private int m_dataSetIndex = -1;

		private int m_calculatedFieldIndex;

		internal int DataSourceIndex
		{
			get
			{
				return this.m_dataSourceIndex;
			}
			set
			{
				this.m_dataSourceIndex = value;
			}
		}

		internal int DataSetIndex
		{
			get
			{
				return this.m_dataSetIndex;
			}
			set
			{
				this.m_dataSetIndex = value;
			}
		}

		internal string DataSetName
		{
			get
			{
				return this.m_dataSetName;
			}
		}

		internal int DataSetDefIndex
		{
			get
			{
				return this.m_dataSetDefIndex;
			}
		}

		internal bool IsComplex
		{
			get
			{
				return this.m_isComplex;
			}
		}

		internal Dictionary<string, bool> ParameterNames
		{
			get
			{
				return this.m_parameterNames;
			}
		}

		internal int CalculatedFieldIndex
		{
			get
			{
				return this.m_calculatedFieldIndex;
			}
			set
			{
				this.m_calculatedFieldIndex = value;
			}
		}

		internal PublishingDataSetInfo(string dataSetName, int dataSetDefIndex, bool isComplex, Dictionary<string, bool> parameterNames)
		{
			this.m_dataSetName = dataSetName;
			this.m_dataSetDefIndex = dataSetDefIndex;
			this.m_isComplex = isComplex;
			this.m_parameterNames = parameterNames;
		}

		internal void MergeFlagsFromDataSource(bool isComplex, Dictionary<string, bool> datasourceParameterNames)
		{
			this.m_isComplex |= isComplex;
			if (datasourceParameterNames != null)
			{
				foreach (string key in datasourceParameterNames.Keys)
				{
					this.m_parameterNames[key] = true;
				}
			}
		}
	}
}
