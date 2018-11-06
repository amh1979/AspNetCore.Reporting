namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class DataSetReference
	{
		private string m_dataSet;

		private string m_valueAlias;

		private string m_labelAlias;

		internal string DataSet
		{
			get
			{
				return this.m_dataSet;
			}
		}

		internal string ValueAlias
		{
			get
			{
				return this.m_valueAlias;
			}
		}

		internal string LabelAlias
		{
			get
			{
				return this.m_labelAlias;
			}
		}

		internal DataSetReference(string dataSet, string valueAlias, string labelAlias)
		{
			this.m_dataSet = dataSet;
			this.m_valueAlias = valueAlias;
			this.m_labelAlias = labelAlias;
		}
	}
}
