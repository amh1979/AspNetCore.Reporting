using System;

namespace AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel
{
	public sealed class  RenderFormat : MarshalByRefObject
	{
		private RenderFormatImplBase m_renderFormatImpl;

		public string Name
		{
			get
			{
				return this.m_renderFormatImpl.Name;
			}
		}

		public bool IsInteractive
		{
			get
			{
				return this.m_renderFormatImpl.IsInteractive;
			}
		}

		public ReadOnlyNameValueCollection DeviceInfo
		{
			get
			{
				return this.m_renderFormatImpl.DeviceInfo;
			}
		}

		internal RenderFormat(RenderFormatImplBase renderFormatImpl)
		{
			this.m_renderFormatImpl = renderFormatImpl;
		}
	}
}
