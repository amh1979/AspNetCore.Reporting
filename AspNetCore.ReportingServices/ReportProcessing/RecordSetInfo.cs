using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class RecordSetInfo
	{
		private bool m_readerExtensionsSupported;

		private RecordSetPropertyNamesList m_fieldPropertyNames;

		private CompareOptions m_compareOptions;

		[NonSerialized]
		private bool m_validCompareOptions;

		internal bool ReaderExtensionsSupported
		{
			get
			{
				return this.m_readerExtensionsSupported;
			}
			set
			{
				this.m_readerExtensionsSupported = value;
			}
		}

		internal RecordSetPropertyNamesList FieldPropertyNames
		{
			get
			{
				return this.m_fieldPropertyNames;
			}
			set
			{
				this.m_fieldPropertyNames = value;
			}
		}

		internal CompareOptions CompareOptions
		{
			get
			{
				return this.m_compareOptions;
			}
			set
			{
				this.m_compareOptions = value;
			}
		}

		internal bool ValidCompareOptions
		{
			get
			{
				return this.m_validCompareOptions;
			}
			set
			{
				this.m_validCompareOptions = value;
			}
		}

		internal RecordSetInfo(bool readerExtensionsSupported, CompareOptions compareOptions)
		{
			this.m_readerExtensionsSupported = readerExtensionsSupported;
			this.m_compareOptions = compareOptions;
		}

		internal RecordSetInfo()
		{
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ReaderExtensionsSupported, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.FieldPropertyNames, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.RecordSetPropertyNamesList));
			memberInfoList.Add(new MemberInfo(MemberName.CompareOptions, Token.Enum));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
