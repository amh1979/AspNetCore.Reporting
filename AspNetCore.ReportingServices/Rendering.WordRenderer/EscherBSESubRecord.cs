using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal class EscherBSESubRecord : EscherRecord
	{
		internal const int MD4HASH_LENGTH = 16;

		private byte[] mHash;

		private byte mBoundary = 255;

		private byte[] mImage;

		internal override int RecordSize
		{
			get
			{
				return 8 + this.mHash.Length + ((this.mImage != null) ? (1 + this.mImage.Length) : 0);
			}
		}

		internal override string RecordName
		{
			get
			{
				return "BSESub";
			}
		}

		internal virtual byte[] Hash
		{
			get
			{
				return this.mHash;
			}
			set
			{
				this.mHash = value;
			}
		}

		internal virtual byte[] Image
		{
			get
			{
				return this.mImage;
			}
			set
			{
				this.mImage = value;
			}
		}

		internal EscherBSESubRecord()
		{
		}

		internal override int Serialize(BinaryWriter dataWriter)
		{
			dataWriter.Write(this.getOptions());
			dataWriter.Write(this.GetRecordId());
			dataWriter.Write(this.RecordSize - 8);
			dataWriter.Write(this.mHash);
			dataWriter.Write(this.mBoundary);
			dataWriter.Write(this.mImage);
			return this.RecordSize;
		}
	}
}
