using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class DrillthroughInformation
	{
		private string m_reportName;

		private DrillthroughParameters m_reportParameters;

		private IntList m_dataSetTokenIDs;

		internal string ReportName
		{
			get
			{
				return this.m_reportName;
			}
			set
			{
				this.m_reportName = value;
			}
		}

		internal DrillthroughParameters ReportParameters
		{
			get
			{
				return this.m_reportParameters;
			}
			set
			{
				this.m_reportParameters = value;
			}
		}

		internal IntList DataSetTokenIDs
		{
			get
			{
				return this.m_dataSetTokenIDs;
			}
			set
			{
				this.m_dataSetTokenIDs = value;
			}
		}

		internal DrillthroughInformation()
		{
		}

		internal DrillthroughInformation(string reportName, DrillthroughParameters reportParameters, IntList dataSetTokenIDs)
		{
			this.m_reportName = reportName;
			this.m_reportParameters = reportParameters;
			this.m_dataSetTokenIDs = dataSetTokenIDs;
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.DrillthroughReportName, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.DrillthroughParameters, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.DrillthroughParameters));
			memberInfoList.Add(new MemberInfo(MemberName.DataSets, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.IntList));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}

		internal void ResolveDataSetTokenIDs(TokensHashtable dataSetTokenIDs)
		{
			if (dataSetTokenIDs != null && this.m_dataSetTokenIDs != null)
			{
				DrillthroughParameters drillthroughParameters = new DrillthroughParameters();
				object obj = null;
				for (int i = 0; i < this.m_dataSetTokenIDs.Count; i++)
				{
					obj = ((this.m_dataSetTokenIDs[i] < 0) ? this.m_reportParameters.GetValue(i) : dataSetTokenIDs[this.m_dataSetTokenIDs[i]]);
					drillthroughParameters.Add(this.m_reportParameters.GetKey(i), obj);
				}
				this.m_reportParameters = drillthroughParameters;
			}
		}
	}
}
