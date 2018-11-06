namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class PointerCapExprHost : StyleExprHost
	{
		public CapImageExprHost CapImageHost;

		public virtual object OnTopExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ReflectionExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object CapStyleExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object HiddenExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object WidthExpr
		{
			get
			{
				return null;
			}
		}
	}
}
