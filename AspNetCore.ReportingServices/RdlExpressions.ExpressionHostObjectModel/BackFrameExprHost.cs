namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class BackFrameExprHost : StyleExprHost
	{
		public FrameBackgroundExprHost FrameBackgroundHost;

		public FrameImageExprHost FrameImageHost;

		public virtual object FrameStyleExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object FrameShapeExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object FrameWidthExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object GlassEffectExpr
		{
			get
			{
				return null;
			}
		}
	}
}
