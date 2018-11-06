using System;

namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.SPBIF.ExcelCallbacks.Convert
{
	internal class Operator
	{
		internal enum OperatorType
		{
			ARITHMETIC,
			LOGICAL,
			FUNCTION,
			DELIMITER
		}

		private string m_operator;

		private int m_precedence;

		private OperatorType m_type;

		private ushort m_biffCode;

		private uint m_functionCode;

		private short m_numOfArgs;

		private bool m_variableArgs;

		internal string Name
		{
			get
			{
				return this.m_operator;
			}
		}

		internal int Precedence
		{
			get
			{
				return this.m_precedence;
			}
		}

		internal OperatorType Type
		{
			get
			{
				return this.m_type;
			}
		}

		internal ushort BCode
		{
			get
			{
				return this.m_biffCode;
			}
		}

		internal uint FCode
		{
			get
			{
				return this.m_functionCode;
			}
		}

		internal short ArgumentCount
		{
			get
			{
				return this.m_numOfArgs;
			}
			set
			{
				this.m_numOfArgs = value;
			}
		}

		internal byte[] BiffOperator
		{
			get
			{
				return BitConverter.GetBytes(this.m_biffCode);
			}
		}

		internal byte[] FunctionCode
		{
			get
			{
				return BitConverter.GetBytes(this.m_functionCode);
			}
		}

		internal byte[] NumberOfArguments
		{
			get
			{
				return BitConverter.GetBytes(this.m_numOfArgs);
			}
		}

		internal Operator(string op, int precedence, OperatorType ot, ushort biffCode)
		{
			this.m_operator = op;
			this.m_precedence = precedence;
			this.m_type = ot;
			this.m_biffCode = biffCode;
		}

		internal Operator(string op, int precedence, OperatorType ot, ushort biffCode, uint functionCode)
		{
			this.m_operator = op;
			this.m_precedence = precedence;
			this.m_type = ot;
			this.m_biffCode = biffCode;
			this.m_functionCode = functionCode;
		}

		internal Operator(string op, int precedence, OperatorType ot, ushort biffCode, uint functionCode, short numOfArgs)
		{
			this.m_operator = op;
			this.m_precedence = precedence;
			this.m_type = ot;
			this.m_biffCode = biffCode;
			this.m_functionCode = functionCode;
			if (biffCode == 66 && numOfArgs == -1)
			{
				this.m_numOfArgs = 0;
				this.m_variableArgs = true;
			}
			else
			{
				this.m_numOfArgs = numOfArgs;
			}
		}

		internal Operator(Operator op)
		{
			this.m_operator = op.m_operator;
			this.m_precedence = op.m_precedence;
			this.m_type = op.m_type;
			this.m_biffCode = op.m_biffCode;
			this.m_functionCode = op.m_functionCode;
			this.m_numOfArgs = op.m_numOfArgs;
		}

		internal bool HasVariableArguments()
		{
			return this.m_variableArgs;
		}
	}
}
