namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ScalePinExprHost : TickMarkStyleExprHost
	{
		public PinLabelExprHost PinLabelHost;

		public virtual object LocationExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object EnableExpr
		{
			get
			{
				return null;
			}
		}
	}
}
