namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapPolygonTemplateExprHost : MapSpatialElementTemplateExprHost
	{
		public virtual object ScaleFactorExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object CenterPointOffsetXExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object CenterPointOffsetYExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ShowLabelExpr
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
