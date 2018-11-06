namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapViewportExprHost : MapSubItemExprHost
	{
		public MapLimitsExprHost MapLimitsHost;

		public MapViewExprHost MapViewHost;

		public MapGridLinesExprHost MapMeridiansHost;

		public MapGridLinesExprHost MapParallelsHost;

		public virtual object MapCoordinateSystemExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object MapProjectionExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ProjectionCenterXExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ProjectionCenterYExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object MaximumZoomExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object MinimumZoomExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ContentMarginExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object GridUnderContentExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object SimplificationResolutionExpr
		{
			get
			{
				return null;
			}
		}
	}
}
