namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class IndicatorImageExprHost : BaseGaugeImageExprHost
	{
		public virtual object HueColorExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object TransparencyExpr
		{
			get
			{
				return null;
			}
		}
	}
}
