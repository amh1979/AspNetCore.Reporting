namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal class HyperlinkFieldInfo : FieldInfo
	{
		private const byte HyperlinkCode = 88;

		private static readonly byte[] StartData = new byte[2]
		{
			19,
			88
		};

		private static readonly byte[] MiddleData = new byte[2]
		{
			20,
			255
		};

		private static readonly byte[] EndData = new byte[2]
		{
			21,
			128
		};

		internal override byte[] Start
		{
			get
			{
				return HyperlinkFieldInfo.StartData;
			}
		}

		internal override byte[] Middle
		{
			get
			{
				return HyperlinkFieldInfo.MiddleData;
			}
		}

		internal override byte[] End
		{
			get
			{
				return HyperlinkFieldInfo.EndData;
			}
		}

		internal HyperlinkFieldInfo(int offset, Location location)
			: base(offset, location)
		{
		}
	}
}
