using AspNetCore.ReportingServices.Common;
using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportProcessingException_ComparisonError : Exception, IDataComparisonError
	{
		private const string TypeXSerializationID = "typex";

		private const string TypeYSerializationID = "typey";

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

		internal ReportProcessingException_ComparisonError(string typeX, string typeY)
		{
			this.m_typeX = typeX;
			this.m_typeY = typeY;
		}

		private ReportProcessingException_ComparisonError(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.m_typeX = info.GetString("typex");
			this.m_typeY = info.GetString("typey");
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("typex", this.m_typeX);
			info.AddValue("typex", this.m_typeY);
		}
	}
}
