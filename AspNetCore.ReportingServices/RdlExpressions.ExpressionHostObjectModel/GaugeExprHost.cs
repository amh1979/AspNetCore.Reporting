namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class GaugeExprHost : GaugePanelItemExprHost
	{
		public BackFrameExprHost BackFrameHost;

		public TopImageExprHost TopImageHost;

		public virtual object ClipContentExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object AspectRatioExpr
		{
			get
			{
				return null;
			}
		}
	}
}
