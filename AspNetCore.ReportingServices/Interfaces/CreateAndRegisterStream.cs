using System.IO;
using System.Text;

namespace AspNetCore.ReportingServices.Interfaces
{
	internal delegate Stream CreateAndRegisterStream(string name, string extension, Encoding encoding, string mimeType, bool willSeek, StreamOper operation);
}
