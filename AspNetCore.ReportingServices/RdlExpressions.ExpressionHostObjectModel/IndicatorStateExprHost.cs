namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class IndicatorStateExprHost : ReportObjectModelProxy
	{
		public GaugeInputValueExprHost StartValueHost;

		public GaugeInputValueExprHost EndValueHost;

		public IndicatorImageExprHost IndicatorImageHost;

		public virtual object ColorExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ScaleFactorExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object IndicatorStyleExpr
		{
			get
			{
				return null;
			}
		}
	}
}
