namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapLineTemplateExprHost : MapSpatialElementTemplateExprHost
	{
		public virtual object WidthExpr
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
