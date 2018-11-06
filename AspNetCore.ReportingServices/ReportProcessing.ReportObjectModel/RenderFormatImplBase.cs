using System;

namespace AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel
{
	public abstract class RenderFormatImplBase : MarshalByRefObject
	{
		internal abstract string Name
		{
			get;
		}

		internal abstract bool IsInteractive
		{
			get;
		}

		internal abstract ReadOnlyNameValueCollection DeviceInfo
		{
			get;
		}
	}
}
