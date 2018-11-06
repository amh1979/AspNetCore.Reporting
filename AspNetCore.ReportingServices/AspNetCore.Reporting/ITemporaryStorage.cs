using System.IO;

namespace AspNetCore.Reporting
{
	internal interface ITemporaryStorage
	{
		Stream CreateTemporaryStream();
	}
}
