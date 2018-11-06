using System.IO;
using System.Text;

namespace AspNetCore.Reporting
{
	internal delegate Stream CreateStreamCallback(string name, string extension, Encoding encoding, string mimeType, bool willSeek);
}
