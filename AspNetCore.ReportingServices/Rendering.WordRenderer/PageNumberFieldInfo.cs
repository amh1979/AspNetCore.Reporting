namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal class PageNumberFieldInfo : FieldInfo
	{
		private const byte PageNumberCode = 33;

		private static readonly byte[] StartData = new byte[2]
		{
			19,
			33
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
				return PageNumberFieldInfo.StartData;
			}
		}

		internal override byte[] Middle
		{
			get
			{
				return PageNumberFieldInfo.MiddleData;
			}
		}

		internal override byte[] End
		{
			get
			{
				return PageNumberFieldInfo.EndData;
			}
		}

		internal PageNumberFieldInfo(int offset, Location location)
			: base(offset, location)
		{
		}
	}
}
