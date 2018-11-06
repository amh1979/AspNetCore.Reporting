namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class FrameImageExprHost : BaseGaugeImageExprHost
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

		public virtual object ClipImageExpr
		{
			get
			{
				return null;
			}
		}
	}
}
