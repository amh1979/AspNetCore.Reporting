using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal abstract class FieldInfo
	{
		internal enum Location
		{
			Start,
			Middle,
			End
		}

		internal const byte StartCode = 19;

		internal const byte MiddleCode = 20;

		internal const byte EndCode = 21;

		protected int m_offset;

		protected Location m_location;

		internal int Offset
		{
			get
			{
				return this.m_offset;
			}
		}

		internal abstract byte[] Start
		{
			get;
		}

		internal abstract byte[] Middle
		{
			get;
		}

		internal abstract byte[] End
		{
			get;
		}

		internal FieldInfo(int offset, Location location)
		{
			this.m_offset = offset;
			this.m_location = location;
		}

		internal void WriteData(BinaryWriter dataWriter)
		{
			switch (this.m_location)
			{
			case Location.Start:
				dataWriter.Write(this.Start);
				break;
			case Location.Middle:
				dataWriter.Write(this.Middle);
				break;
			case Location.End:
				dataWriter.Write(this.End);
				break;
			}
		}
	}
}
