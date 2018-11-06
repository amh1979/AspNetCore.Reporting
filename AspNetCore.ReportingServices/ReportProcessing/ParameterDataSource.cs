using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal class ParameterDataSource : IParameterDataSource
	{
		private int m_dataSourceIndex = -1;

		private int m_dataSetIndex = -1;

		private int m_valueFieldIndex = -1;

		private int m_labelFieldIndex = -1;

		public int DataSourceIndex
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

		public int DataSetIndex
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

		public int ValueFieldIndex
		{
			get
			{
				return this.m_valueFieldIndex;
			}
			set
			{
				this.m_valueFieldIndex = value;
			}
		}

		public int LabelFieldIndex
		{
			get
			{
				return this.m_labelFieldIndex;
			}
			set
			{
				this.m_labelFieldIndex = value;
			}
		}

		internal ParameterDataSource()
		{
		}

		internal ParameterDataSource(int dataSourceIndex, int dataSetIndex)
		{
			this.m_dataSourceIndex = dataSourceIndex;
			this.m_dataSetIndex = dataSetIndex;
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.DataSourceIndex, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.DataSetIndex, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.ValueFieldIndex, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.LabelFieldIndex, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
