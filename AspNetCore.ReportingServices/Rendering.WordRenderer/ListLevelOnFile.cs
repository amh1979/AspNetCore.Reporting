using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System.IO;
using System.Text;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal class ListLevelOnFile
	{
		internal enum WordNumberFormat
		{
			Arabic,
			Lowercase = 4,
			RomanLower = 2,
			Bullet = 23
		}

		internal const int TAB = 0;

		internal const int SPACE = 1;

		internal const int NOTHING = 2;

		private int _iStartAt;

		private byte _nfc;

		private byte _info;

		private byte[] _rgbxchNums;

		private byte _ixchFollow;

		private int _dxaSpace;

		private int _dxaIndent;

		private short _reserved;

		private byte[] _grpprlPapx;

		private byte[] _grpprlChpx;

		private char[] _bulletText;

		private RPLFormat.ListStyles m_style;

		internal virtual int SizeInBytes
		{
			get
			{
				return 28 + this._grpprlChpx.Length + this._grpprlPapx.Length + this._bulletText.Length * 2 + 2;
			}
		}

		internal virtual byte FollowChar
		{
			get
			{
				return this._ixchFollow;
			}
		}

		internal RPLFormat.ListStyles ListStyle
		{
			get
			{
				return this.m_style;
			}
		}

		internal ListLevelOnFile(int level, RPLFormat.ListStyles listStyle, Word97Writer writer)
		{
			this.m_style = listStyle;
			this._iStartAt = 1;
			int num = 0;
			this._grpprlPapx = new byte[0];
			this._dxaSpace = 360;
			this._rgbxchNums = new byte[9];
			this._dxaIndent = 0;
			this._ixchFollow = 0;
			string fontName = "Arial";
			switch (listStyle)
			{
			case RPLFormat.ListStyles.Numbered:
				this._rgbxchNums[0] = 1;
				this._bulletText = new char[2]
				{
					(char)(ushort)level,
					'.'
				};
				switch (level % 3)
				{
				case 0:
					this.setNumberFormatInternal(WordNumberFormat.Arabic);
					break;
				case 1:
					this.setNumberFormatInternal(WordNumberFormat.RomanLower);
					break;
				case 2:
					this.setNumberFormatInternal(WordNumberFormat.Lowercase);
					break;
				}
				break;
			case RPLFormat.ListStyles.Bulleted:
				this.setNumberFormatInternal(WordNumberFormat.Bullet);
				switch (level % 3)
				{
				case 0:
					this._bulletText = new char[1]
					{
						'·'
					};
					fontName = "Symbol";
					break;
				case 1:
					this._bulletText = new char[1]
					{
						'o'
					};
					fontName = "Courier New";
					break;
				case 2:
					this._bulletText = new char[1]
					{
						'§'
					};
					fontName = "Wingdings";
					break;
				}
				break;
			default:
				this.setNumberFormatInternal(WordNumberFormat.Bullet);
				this._bulletText = new char[1]
				{
					' '
				};
				break;
			}
			this._grpprlChpx = new byte[20];
			num = 0;
			int param = writer.WriteFont(fontName);
			num += Word97Writer.AddSprm(this._grpprlChpx, num, 19023, param, null);
			num += Word97Writer.AddSprm(this._grpprlChpx, num, 19038, param, null);
			num += Word97Writer.AddSprm(this._grpprlChpx, num, 19024, param, null);
			num += Word97Writer.AddSprm(this._grpprlChpx, num, 19025, param, null);
			int param2 = 20;
			num += Word97Writer.AddSprm(this._grpprlChpx, num, 19011, param2, null);
		}

		internal void Write(BinaryWriter writer)
		{
			writer.Write(this._iStartAt);
			writer.Write(this._nfc);
			writer.Write(this._info);
			writer.Flush();
			writer.Write(this._rgbxchNums, 0, this._rgbxchNums.Length);
			writer.Write(this._ixchFollow);
			writer.Write(this._dxaSpace);
			writer.Write(this._dxaIndent);
			writer.Write((byte)this._grpprlChpx.Length);
			writer.Write((byte)this._grpprlPapx.Length);
			writer.Write(this._reserved);
			writer.Flush();
			writer.Write(this._grpprlPapx, 0, this._grpprlPapx.Length);
			writer.Write(this._grpprlChpx, 0, this._grpprlChpx.Length);
			writer.Flush();
			writer.Write((short)this._bulletText.Length);
			byte[] bytes = Encoding.Unicode.GetBytes(this._bulletText);
			writer.Flush();
			writer.Write(bytes, 0, bytes.Length);
		}

		private void setNumberFormatInternal(WordNumberFormat nfc)
		{
			this._nfc = (byte)nfc;
		}
	}
}
