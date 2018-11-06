using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportInstanceInfo : ReportItemInstanceInfo
	{
		private ParameterInfoCollection m_parameters;

		private string m_reportName;

		private bool m_noRows;

		private int m_bodyUniqueName;

		internal ParameterInfoCollection Parameters
		{
			get
			{
				return this.m_parameters;
			}
			set
			{
				this.m_parameters = value;
			}
		}

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

		internal bool NoRows
		{
			get
			{
				return this.m_noRows;
			}
			set
			{
				this.m_noRows = value;
			}
		}

		internal int BodyUniqueName
		{
			get
			{
				return this.m_bodyUniqueName;
			}
			set
			{
				this.m_bodyUniqueName = value;
			}
		}

		internal ReportInstanceInfo(ReportProcessing.ProcessingContext pc, Report reportItemDef, ReportInstance owner, ParameterInfoCollection parameters, bool noRows)
			: base(pc, reportItemDef, owner, true)
		{
			this.m_bodyUniqueName = pc.CreateUniqueName();
			this.m_reportName = pc.ReportContext.ItemName;
			this.m_parameters = new ParameterInfoCollection();
			if (parameters != null && parameters.Count > 0)
			{
				parameters.CopyTo(this.m_parameters);
			}
			this.m_noRows = noRows;
		}

		internal ReportInstanceInfo(Report reportItemDef)
			: base(reportItemDef)
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Parameters, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ParameterInfoCollection));
			memberInfoList.Add(new MemberInfo(MemberName.ReportName, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.NoRows, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.BodyUniqueName, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstanceInfo, memberInfoList);
		}
	}
}
