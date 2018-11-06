using AspNetCore.ReportingServices.Diagnostics.Utilities;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	[Serializable]
	internal class ReportRenderingException : Exception
	{
		private ErrorCode m_ErrorCode;

		private bool m_Unexpected;

		public ErrorCode ErrorCode
		{
			get
			{
				return this.m_ErrorCode;
			}
		}

		public bool Unexpected
		{
			get
			{
				return this.m_Unexpected;
			}
		}

		protected ReportRenderingException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public ReportRenderingException(ErrorCode errCode)
			: this(errCode, RenderErrors.Keys.GetString(errCode.ToString()), false)
		{
			this.m_ErrorCode = errCode;
		}

		public ReportRenderingException(string message)
			: this(ErrorCode.rrRenderingError, message, false)
		{
		}

		public ReportRenderingException(string message, bool unexpected)
			: this(ErrorCode.rrRenderingError, message, unexpected)
		{
		}

		public ReportRenderingException(ErrorCode errCode, string message, bool unexpected)
			: base(message)
		{
			this.m_ErrorCode = errCode;
			this.m_Unexpected = unexpected;
		}

		public ReportRenderingException(Exception innerException)
			: this(ErrorCode.rrRenderingError, innerException, false)
		{
		}

		public ReportRenderingException(Exception innerException, bool unexpected)
			: this(ErrorCode.rrRenderingError, innerException, unexpected)
		{
		}

		public ReportRenderingException(ErrorCode errCode, Exception innerException)
			: this(errCode, RenderErrors.Keys.GetString(errCode.ToString()), innerException, false)
		{
		}

		public ReportRenderingException(ErrorCode errCode, Exception innerException, bool unexpected)
			: this(errCode, RenderErrors.Keys.GetString(errCode.ToString()), innerException, unexpected)
		{
		}

		public ReportRenderingException(string message, Exception innerException)
			: this(ErrorCode.rrRenderingError, message, innerException, false)
		{
		}

		public ReportRenderingException(string message, Exception innerException, bool unexpected)
			: this(ErrorCode.rrRenderingError, message, innerException, unexpected)
		{
		}

		public ReportRenderingException(ErrorCode errCode, string message, Exception innerException, bool unexpected)
			: base(message, innerException)
		{
			this.m_ErrorCode = errCode;
			this.m_Unexpected = unexpected;
		}

		public ReportRenderingException(ErrorCode errCode, params object[] arguments)
			: base(string.Format(CultureInfo.CurrentCulture, RenderErrors.Keys.GetString(errCode.ToString()), arguments))
		{
			this.m_ErrorCode = errCode;
		}
	}
}
