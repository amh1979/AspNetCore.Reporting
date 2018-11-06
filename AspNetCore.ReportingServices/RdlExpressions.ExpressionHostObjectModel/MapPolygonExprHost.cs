namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapPolygonExprHost : MapSpatialElementExprHost
	{
		public MapPolygonTemplateExprHost MapPolygonTemplateHost;

		public MapPointTemplateExprHost MapPointTemplateHost;

		public virtual object UseCustomPolygonTemplateExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object UseCustomPointTemplateExpr
		{
			get
			{
				return null;
			}
		}
	}
}
