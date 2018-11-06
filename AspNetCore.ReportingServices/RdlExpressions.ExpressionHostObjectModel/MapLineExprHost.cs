namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapLineExprHost : MapSpatialElementExprHost
	{
		public MapLineTemplateExprHost MapLineTemplateHost;

		public virtual object UseCustomLineTemplateExpr
		{
			get
			{
				return null;
			}
		}
	}
}
