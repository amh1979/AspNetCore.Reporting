using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal class EscherComplexProperty : EscherProperty
	{
		internal byte[] complexData = new byte[0];

		internal virtual byte[] ComplexData
		{
			get
			{
				return this.complexData;
			}
			set
			{
				this.complexData = value;
			}
		}

		internal override int PropertySize
		{
			get
			{
				return 6 + this.complexData.Length;
			}
		}

		internal EscherComplexProperty(ushort id, byte[] complexData)
			: base(id)
		{
			this.complexData = complexData;
		}

		internal EscherComplexProperty(ushort propertyNumber, bool isBlipId, byte[] complexData)
			: base(propertyNumber, true, isBlipId)
		{
			this.complexData = complexData;
		}

		internal override int serializeSimplePart(BinaryWriter dataWriter)
		{
			dataWriter.Write(this.Id);
			dataWriter.Write(this.complexData.Length);
			return 6;
		}

		internal override int serializeComplexPart(BinaryWriter dataWriter)
		{
			dataWriter.Write(this.complexData);
			return this.complexData.Length;
		}

		public override int GetHashCode()
		{
			return this.Id * 11;
		}
	}
}
