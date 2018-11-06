namespace AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class ChartDynamicGroupExprHost : DynamicGroupExprHost
	{
		public virtual object HeadingLabelExpr
		{
			get
			{
				return null;
			}
		}
	}
}
