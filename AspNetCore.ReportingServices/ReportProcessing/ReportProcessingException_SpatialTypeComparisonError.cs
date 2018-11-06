using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportProcessingException_SpatialTypeComparisonError : Exception
	{
		private const string TypeSerializationID = "type";

		private string m_type;

		internal string Type
		{
			get
			{
				return this.m_type;
			}
		}

		internal ReportProcessingException_SpatialTypeComparisonError(string type)
		{
			this.m_type = type;
		}

		internal ReportProcessingException_SpatialTypeComparisonError(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.m_type = info.GetString("type");
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("type", this.m_type);
		}
	}
}
