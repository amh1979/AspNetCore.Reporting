namespace AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class ChartTitleExprHost : StyleExprHost
	{
		public virtual object CaptionExpr
		{
			get
			{
				return null;
			}
		}
	}
}
