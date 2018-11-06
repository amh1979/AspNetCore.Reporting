namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class LinearPointerExprHost : GaugePointerExprHost
	{
		public ThermometerExprHost ThermometerHost;

		public virtual object TypeExpr
		{
			get
			{
				return null;
			}
		}
	}
}
