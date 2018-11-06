namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class GaugeImageExprHost : GaugePanelItemExprHost
	{
		public virtual object SourceExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ValueExpr
		{
			get
			{
				return null;
			}
		}
	}
}
