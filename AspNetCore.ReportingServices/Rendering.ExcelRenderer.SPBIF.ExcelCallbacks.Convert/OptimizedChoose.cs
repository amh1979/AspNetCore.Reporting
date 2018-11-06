using System.Collections;

namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.SPBIF.ExcelCallbacks.Convert
{
	internal sealed class OptimizedChoose : Operator
	{
		private ArrayList m_gotoLabels;

		internal ArrayList GotoLabelList
		{
			get
			{
				return this.m_gotoLabels;
			}
			set
			{
				this.m_gotoLabels = value;
			}
		}

		internal OptimizedChoose(string op, int precedence, OperatorType ot, ushort biffCode)
			: base(op, precedence, ot, biffCode)
		{
		}

		internal OptimizedChoose(OptimizedChoose oc)
			: base(oc.Name, oc.Precedence, oc.Type, oc.BCode)
		{
		}
	}
}
