using System.IO;
using System.Text;

namespace AspNetCore.ReportingServices.Interfaces
{
    internal abstract class RenderedOutputFile
	{
		public abstract string FileName
		{
			get;
		}

		public abstract string Type
		{
			get;
		}

		public abstract Stream Data
		{
			get;
		}

		public abstract string Extension
		{
			get;
		}

		public abstract Encoding Encoding
		{
			get;
		}
	}
}
