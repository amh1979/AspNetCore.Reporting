using System.IO;
using System.Text;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal class HyperlinkWriter
	{
		internal enum LinkType
		{
			Hyperlink,
			File,
			Bookmark
		}

		private const byte Unknown = 0;

		private const int Unknown1 = 2;

		private const int Unknown2 = 3;

		private const short Unknown3 = 3;

		private const int Unknown4 = 8;

		private const byte Unknown5 = 8;

		internal static readonly byte[] EMPTY_PIC = new byte[64]
		{
			68,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0
		};

		internal static readonly byte[] MAGIC_NUM1 = new byte[16]
		{
			208,
			201,
			234,
			121,
			249,
			186,
			206,
			17,
			140,
			130,
			0,
			170,
			0,
			75,
			169,
			11
		};

		internal static readonly byte[] MAGIC_NUM2 = new byte[16]
		{
			224,
			201,
			234,
			121,
			249,
			186,
			206,
			17,
			140,
			130,
			0,
			170,
			0,
			75,
			169,
			11
		};

		internal static readonly byte[] MAGIC_NUM3 = new byte[18]
		{
			3,
			3,
			0,
			0,
			0,
			0,
			0,
			0,
			192,
			0,
			0,
			0,
			0,
			0,
			0,
			70,
			0,
			0
		};

		internal static readonly byte[] MAGIC_NUM4 = new byte[24]
		{
			255,
			255,
			173,
			222,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0
		};

		internal static void WriteHyperlink(Stream dataStream, string target, LinkType type)
		{
			int num = 4 + HyperlinkWriter.EMPTY_PIC.Length + 1 + HyperlinkWriter.MAGIC_NUM1.Length + 4 + 4;
			switch (type)
			{
			case LinkType.File:
				num += HyperlinkWriter.MAGIC_NUM3.Length + 4 + target.Length + 1 + HyperlinkWriter.MAGIC_NUM4.Length + 4 + 4 + 2 + (target.Length + 1) * 2;
				break;
			case LinkType.Hyperlink:
				num += HyperlinkWriter.MAGIC_NUM2.Length + 4 + (target.Length + 1) * 2;
				break;
			case LinkType.Bookmark:
				target = Bookmarks.CleanseName(target);
				num += 4 + (target.Length + 1) * 2;
				break;
			}
			BinaryWriter binaryWriter = new BinaryWriter(dataStream);
			binaryWriter.Write(num);
			binaryWriter.Write(HyperlinkWriter.EMPTY_PIC, 0, HyperlinkWriter.EMPTY_PIC.Length);
			if (type == LinkType.Bookmark)
			{
				binaryWriter.Write((byte)8);
			}
			else
			{
				binaryWriter.Write((byte)0);
			}
			binaryWriter.Flush();
			binaryWriter.Write(HyperlinkWriter.MAGIC_NUM1, 0, HyperlinkWriter.MAGIC_NUM1.Length);
			binaryWriter.Write(2);
			if (type == LinkType.Bookmark)
			{
				binaryWriter.Write(8);
			}
			else
			{
				binaryWriter.Write(3);
			}
			binaryWriter.Flush();
			switch (type)
			{
			case LinkType.File:
			{
				binaryWriter.Write(HyperlinkWriter.MAGIC_NUM3, 0, HyperlinkWriter.MAGIC_NUM3.Length);
				byte[] bytes3 = Encoding.ASCII.GetBytes(target);
				binaryWriter.Write(bytes3.Length + 1);
				binaryWriter.Write(bytes3, 0, bytes3.Length);
				binaryWriter.Write((byte)0);
				binaryWriter.Write(HyperlinkWriter.MAGIC_NUM4, 0, HyperlinkWriter.MAGIC_NUM4.Length);
				binaryWriter.Write(target.Length * 2 + 6);
				binaryWriter.Write(target.Length * 2);
				binaryWriter.Write((short)3);
				bytes3 = Encoding.Unicode.GetBytes(target);
				binaryWriter.Write(bytes3, 0, bytes3.Length);
				binaryWriter.Write((short)0);
				break;
			}
			case LinkType.Hyperlink:
			{
				binaryWriter.Write(HyperlinkWriter.MAGIC_NUM2, 0, HyperlinkWriter.MAGIC_NUM2.Length);
				binaryWriter.Write((target.Length + 1) * 2);
				binaryWriter.Flush();
				byte[] bytes2 = Encoding.Unicode.GetBytes(target);
				binaryWriter.Write(bytes2);
				binaryWriter.Write((short)0);
				break;
			}
			case LinkType.Bookmark:
			{
				binaryWriter.Write(target.Length + 1);
				byte[] bytes = Encoding.Unicode.GetBytes(target);
				binaryWriter.Write(bytes);
				binaryWriter.Write((short)0);
				break;
			}
			}
			binaryWriter.Write(0);
			binaryWriter.Flush();
		}
	}
}
