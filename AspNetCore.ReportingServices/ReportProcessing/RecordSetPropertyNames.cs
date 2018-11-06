using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class RecordSetPropertyNames
	{
		private StringList m_propertyNames;

		internal StringList PropertyNames
		{
			get
			{
				return this.m_propertyNames;
			}
			set
			{
				this.m_propertyNames = value;
			}
		}

		internal RecordSetPropertyNames()
		{
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.PropertyNames, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.StringList));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
