namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapBindingFieldPairExprHost : ReportObjectModelProxy
	{
		public virtual object FieldNameExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object BindingExpressionExpr
		{
			get
			{
				return null;
			}
		}
	}
}
