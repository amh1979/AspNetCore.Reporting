namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class RadialPointerExprHost : GaugePointerExprHost
	{
		public PointerCapExprHost PointerCapHost;

		public virtual object TypeExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object NeedleStyleExpr
		{
			get
			{
				return null;
			}
		}
	}
}
