namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapPointTemplateExprHost : MapSpatialElementTemplateExprHost
	{
		public virtual object SizeExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object LabelPlacementExpr
		{
			get
			{
				return null;
			}
		}
	}
}
