namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.SPBIF.ExcelCallbacks.Convert
{
	internal sealed class Operand
	{
		internal enum OperandType
		{
			USHORT,
			DOUBLE,
			BOOLEAN,
			STRING,
			NAME
		}

		private object m_operandValue;

		private OperandType m_type;

		internal object OperandValue
		{
			get
			{
				return this.m_operandValue;
			}
		}

		internal OperandType Type
		{
			get
			{
				return this.m_type;
			}
		}

		internal Operand(object operandValue, OperandType type)
		{
			this.m_operandValue = operandValue;
			this.m_type = type;
		}
	}
}
