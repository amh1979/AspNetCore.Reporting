using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class TextRunExprHost : StyleExprHost
	{
		public ActionInfoExprHost ActionInfoHost;

		private TextRun m_textRun;

		public object Value
		{
			get
			{
				return this.m_textRun.Value;
			}
		}

		public virtual object LabelExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ValueExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ToolTipExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object MarkupTypeExpr
		{
			get
			{
				return null;
			}
		}

		internal void SetTextRun(TextRun textRun)
		{
			this.m_textRun = textRun;
		}
	}
}
