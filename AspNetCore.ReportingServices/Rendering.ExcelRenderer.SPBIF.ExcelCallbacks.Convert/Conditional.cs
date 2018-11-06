namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.SPBIF.ExcelCallbacks.Convert
{
	internal sealed class Conditional : Operator
	{
		private string m_gotoLabel;

		private string m_label;

		internal string Label
		{
			get
			{
				return this.m_label;
			}
			set
			{
				this.m_label = value;
			}
		}

		internal string GotoLabel
		{
			get
			{
				return this.m_gotoLabel;
			}
			set
			{
				this.m_gotoLabel = value;
			}
		}

		internal Conditional(string op, int precedence, OperatorType ot, ushort biffCode)
			: base(op, precedence, ot, biffCode)
		{
		}

		internal Conditional(string op, int precedence, OperatorType ot, ushort biffCode, uint functionCode, short numOfArgs)
			: base(op, precedence, ot, biffCode, functionCode, numOfArgs)
		{
		}

		internal Conditional(Conditional conditionalOp)
			: base(conditionalOp.Name, conditionalOp.Precedence, conditionalOp.Type, conditionalOp.BCode, conditionalOp.FCode, conditionalOp.ArgumentCount)
		{
			this.m_label = conditionalOp.Label;
		}
	}
}
