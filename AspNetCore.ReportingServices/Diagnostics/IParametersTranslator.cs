using System.Collections.Specialized;

namespace AspNetCore.ReportingServices.Diagnostics
{
	internal interface IParametersTranslator
	{
		void GetParamsInstance(string paramsInstanceId, out ExternalItemPath itemPath, out NameValueCollection parameters);
	}
}
