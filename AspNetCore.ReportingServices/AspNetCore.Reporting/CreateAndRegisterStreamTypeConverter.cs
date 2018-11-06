using AspNetCore.ReportingServices.Interfaces;
using System.Text;

namespace AspNetCore.Reporting
{
	internal static class CreateAndRegisterStreamTypeConverter
	{
		internal static AspNetCore.ReportingServices.Interfaces.CreateAndRegisterStream ToInnerType(this AspNetCore.ReportingServices.Interfaces.CreateAndRegisterStream callback)
		{
			return (string name, string extension, Encoding encoding, string mimeType, bool willSeek, AspNetCore.ReportingServices.Interfaces.StreamOper operation) => callback(name, extension, encoding, mimeType, willSeek, (StreamOper)operation);
		}

		internal static AspNetCore.ReportingServices.Interfaces.CreateAndRegisterStream ToOuterType(this AspNetCore.ReportingServices.Interfaces.CreateAndRegisterStream callback)
		{
			return (AspNetCore.ReportingServices.Interfaces.CreateAndRegisterStream)(CreateAndRegisterStream)((string name, string extension, Encoding encoding, string mimeType, bool willSeek, AspNetCore.ReportingServices.Interfaces.StreamOper operation) => callback(name, extension, encoding, mimeType, willSeek, (AspNetCore.ReportingServices.Interfaces.StreamOper)operation));
		}
	}
}
