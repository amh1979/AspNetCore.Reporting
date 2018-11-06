using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal class EscherSimpleProperty : EscherProperty
	{
		protected internal int m_propertyValue;

		internal virtual int PropertyValue
		{
			get
			{
				return this.m_propertyValue;
			}
			set
			{
				this.m_propertyValue = value;
			}
		}

		internal EscherSimpleProperty(ushort id, int propertyValue)
			: base(id)
		{
			this.m_propertyValue = propertyValue;
		}

		internal EscherSimpleProperty(ushort propertyNumber, bool isComplex, bool isBlipId, int propertyValue)
			: base(propertyNumber, isComplex, isBlipId)
		{
			this.m_propertyValue = propertyValue;
		}

		internal override int serializeSimplePart(BinaryWriter dataWriter)
		{
			dataWriter.Write(this.Id);
			dataWriter.Write(this.m_propertyValue);
			return 6;
		}

		internal override int serializeComplexPart(BinaryWriter dataWriter)
		{
			return 0;
		}

		public override bool Equals(object o)
		{
			if (this == o)
			{
				return true;
			}
			if (!(o is EscherSimpleProperty))
			{
				return false;
			}
			EscherSimpleProperty escherSimpleProperty = (EscherSimpleProperty)o;
			if (this.m_propertyValue != escherSimpleProperty.m_propertyValue)
			{
				return false;
			}
			if (this.Id != escherSimpleProperty.Id)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return this.m_propertyValue;
		}
	}
}
