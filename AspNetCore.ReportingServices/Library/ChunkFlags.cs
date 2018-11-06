using System;

namespace AspNetCore.ReportingServices.Library
{
	[Flags]
	internal enum ChunkFlags
	{
		None = 0,
		Compressed = 1,
		FileSystem = 2,
		CrossDatabaseSharing = 4
	}
}
