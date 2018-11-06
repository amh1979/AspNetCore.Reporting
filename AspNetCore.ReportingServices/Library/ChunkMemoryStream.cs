using System;
using System.IO;

namespace AspNetCore.ReportingServices.Library
{
	[Serializable]
	internal sealed class ChunkMemoryStream : MemoryStream
	{
		[NonSerialized]
		public bool CanBeClosed;

		public override void Close()
		{
			if (this.CanBeClosed)
			{
				base.Close();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (this.CanBeClosed)
			{
				base.Dispose(disposing);
			}
		}
	}
}
