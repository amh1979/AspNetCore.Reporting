namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class DataSourceExprHost : ReportObjectModelProxy
	{
		public virtual object ConnectStringExpr
		{
			get
			{
				return null;
			}
		}
	}
}
