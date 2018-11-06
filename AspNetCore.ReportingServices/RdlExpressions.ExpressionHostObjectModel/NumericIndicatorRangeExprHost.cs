namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class NumericIndicatorRangeExprHost : ReportObjectModelProxy
	{
		public GaugeInputValueExprHost StartValueHost;

		public GaugeInputValueExprHost EndValueHost;

		public virtual object DecimalDigitColorExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DigitColorExpr
		{
			get
			{
				return null;
			}
		}
	}
}
