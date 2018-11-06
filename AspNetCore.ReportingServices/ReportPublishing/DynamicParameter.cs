namespace AspNetCore.ReportingServices.ReportPublishing
{
	internal sealed class DynamicParameter
	{
		private DataSetReference m_validValueDataSet;

		private DataSetReference m_defaultDataSet;

		private int m_index;

		private bool m_isComplex;

		internal DataSetReference ValidValueDataSet
		{
			get
			{
				return this.m_validValueDataSet;
			}
		}

		internal DataSetReference DefaultDataSet
		{
			get
			{
				return this.m_defaultDataSet;
			}
		}

		internal int Index
		{
			get
			{
				return this.m_index;
			}
		}

		internal bool IsComplex
		{
			get
			{
				return this.m_isComplex;
			}
		}

		internal DynamicParameter(DataSetReference validValueDataSet, DataSetReference defaultDataSet, int index, bool isComplex)
		{
			this.m_validValueDataSet = validValueDataSet;
			this.m_defaultDataSet = defaultDataSet;
			this.m_index = index;
			this.m_isComplex = isComplex;
		}
	}
}
