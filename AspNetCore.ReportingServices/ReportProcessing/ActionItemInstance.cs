using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ActionItemInstance
	{
		private string m_hyperLinkURL;

		private string m_bookmarkLink;

		private string m_label;

		private string m_drillthroughReportName;

		private object[] m_drillthroughParametersValues;

		private BoolList m_drillthroughParametersOmits;

		[NonSerialized]
		private IntList m_dataSetTokenIDs;

		internal string HyperLinkURL
		{
			get
			{
				return this.m_hyperLinkURL;
			}
			set
			{
				this.m_hyperLinkURL = value;
			}
		}

		internal string BookmarkLink
		{
			get
			{
				return this.m_bookmarkLink;
			}
			set
			{
				this.m_bookmarkLink = value;
			}
		}

		internal string Label
		{
			get
			{
				return this.m_label;
			}
			set
			{
				this.m_label = value;
			}
		}

		internal string DrillthroughReportName
		{
			get
			{
				return this.m_drillthroughReportName;
			}
			set
			{
				this.m_drillthroughReportName = value;
			}
		}

		internal object[] DrillthroughParametersValues
		{
			get
			{
				return this.m_drillthroughParametersValues;
			}
			set
			{
				this.m_drillthroughParametersValues = value;
			}
		}

		internal BoolList DrillthroughParametersOmits
		{
			get
			{
				return this.m_drillthroughParametersOmits;
			}
			set
			{
				this.m_drillthroughParametersOmits = value;
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

		internal ActionItemInstance(ReportProcessing.ProcessingContext pc, ActionItem actionItemDef)
		{
			ParameterValueList drillthroughParameters = actionItemDef.DrillthroughParameters;
			if (drillthroughParameters != null)
			{
				this.m_drillthroughParametersValues = new object[drillthroughParameters.Count];
				this.m_drillthroughParametersOmits = new BoolList(drillthroughParameters.Count);
				this.m_dataSetTokenIDs = new IntList(drillthroughParameters.Count);
				for (int i = 0; i < drillthroughParameters.Count; i++)
				{
					if (drillthroughParameters[i].Value != null && drillthroughParameters[i].Value.Type == ExpressionInfo.Types.Token)
					{
						this.m_dataSetTokenIDs.Add(drillthroughParameters[i].Value.IntValue);
					}
					else
					{
						this.m_dataSetTokenIDs.Add(-1);
					}
				}
			}
		}

		internal ActionItemInstance()
		{
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.HyperLinkURL, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.BookmarkLink, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.DrillthroughReportName, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.DrillthroughParameters, Token.Array, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.DrillthroughParametersOmits, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.BoolList));
			memberInfoList.Add(new MemberInfo(MemberName.Label, Token.String));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
