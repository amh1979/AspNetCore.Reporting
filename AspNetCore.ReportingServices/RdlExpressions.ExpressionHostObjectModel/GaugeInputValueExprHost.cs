namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class GaugeInputValueExprHost : ReportObjectModelProxy
	{
		public virtual object ValueExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object FormulaExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object MinPercentExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object MaxPercentExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object MultiplierExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object AddConstantExpr
		{
			get
			{
				return null;
			}
		}
	}
}
