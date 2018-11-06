namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class YukonDataSetInfo
	{
		private StringList m_parameterNames;

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

		internal StringList ParameterNames
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

		internal YukonDataSetInfo(int index, bool isComplex, StringList parameterNames)
		{
			this.m_dataSetDefIndex = index;
			this.m_isComplex = isComplex;
			this.m_parameterNames = parameterNames;
		}

		internal void MergeFlagsFromDataSource(bool isComplex, StringList datasourceParameterNames)
		{
			this.m_isComplex |= isComplex;
			if (this.m_parameterNames == null)
			{
				this.m_parameterNames = datasourceParameterNames;
			}
			else if (datasourceParameterNames != null)
			{
				this.m_parameterNames.InsertRange(0, datasourceParameterNames);
			}
		}
	}
}
