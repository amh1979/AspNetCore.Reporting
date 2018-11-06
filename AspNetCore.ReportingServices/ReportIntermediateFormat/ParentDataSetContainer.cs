namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class ParentDataSetContainer
	{
		private readonly DataSet m_rowParentDataSet;

		private readonly DataSet m_columnParentDataSet;

		private readonly int m_count;

		public int Count
		{
			get
			{
				return this.m_count;
			}
		}

		public DataSet ParentDataSet
		{
			get
			{
				return this.m_rowParentDataSet;
			}
		}

		public DataSet RowParentDataSet
		{
			get
			{
				return this.m_rowParentDataSet;
			}
		}

		public DataSet ColumnParentDataSet
		{
			get
			{
				return this.m_columnParentDataSet;
			}
		}

		public ParentDataSetContainer(DataSet parentDataSet)
		{
			this.m_rowParentDataSet = parentDataSet;
			this.m_columnParentDataSet = null;
			this.m_count = 1;
		}

		public ParentDataSetContainer(DataSet rowParentDataSet, DataSet columnParentDataSet)
		{
			this.m_rowParentDataSet = rowParentDataSet;
			this.m_columnParentDataSet = columnParentDataSet;
			this.m_count = 2;
		}

		public bool AreAllSameDataSet()
		{
			if (this.Count == 1)
			{
				return true;
			}
			return DataSet.AreEqualById(this.m_rowParentDataSet, this.m_columnParentDataSet);
		}
	}
}
