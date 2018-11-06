using System;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[Flags]
	internal enum LEDSegment7 : ulong
	{
		Empty = 0uL,
		SA = 1uL,
		SB = 2uL,
		SC = 4uL,
		SD = 8uL,
		SE = 0x10,
		SF = 0x20,
		SG = 0x40,
		SDP = 0x4000,
		SComma = 0x8000,
		N1 = 6uL,
		N2 = 0x5B,
		N3 = 0x4F,
		N4 = 0x66,
		N5 = 0x6D,
		N6 = 0x7D,
		N7 = 7uL,
		N8 = 0x7F,
		N9 = 0x6F,
		N0 = 0x3F,
		Neg = 0x40,
		Unknown = 0x79,
		All = 0x407F
	}
}
