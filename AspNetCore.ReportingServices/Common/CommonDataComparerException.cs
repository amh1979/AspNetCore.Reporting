using System;

namespace AspNetCore.ReportingServices.Common
{
	internal sealed class CommonDataComparerException : Exception, IDataComparisonError
	{
		private string m_typeX;

		private string m_typeY;

		public string TypeX
		{
			get
			{
				return this.m_typeX;
			}
		}

		public string TypeY
		{
			get
			{
				return this.m_typeY;
			}
		}

		internal CommonDataComparerException(string typeX, string typeY)
		{
			this.m_typeX = typeX;
			this.m_typeY = typeY;
		}
	}
}
