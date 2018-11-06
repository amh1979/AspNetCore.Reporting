using System;

namespace AspNetCore.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class CMapMapping : IComparable<CMapMapping>
	{
		internal readonly ushort Source;

		internal readonly ushort Destination;

		internal CMapMapping(ushort source, ushort destination)
		{
			this.Source = source;
			this.Destination = destination;
		}

		public int CompareTo(CMapMapping other)
		{
			if (other == null)
			{
				return 1;
			}
			return this.Source.CompareTo(other.Source);
		}

		internal int GetSourceLeftByte()
		{
			return this.Source >> 8;
		}

		internal int GetSourceDelta(CMapMapping other)
		{
			return this.Source - other.Source;
		}

		internal int GetDestinationDelta(CMapMapping other)
		{
			return this.Destination - other.Destination;
		}
	}
}
