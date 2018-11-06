using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;

namespace AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class TextBoxExprHost : ReportItemExprHost
	{
		public const string ValueName = "Value";

        private ReportObjectModel.ReportItem m_textBox;

		public object Value
		{
			get
			{
				return this.m_textBox.Value;
			}
		}

        internal ReportObjectModel.ReportItem ReportObjectModelTextBox
		{
			get
			{
				return this.m_textBox;
			}
		}

		public virtual object ValueExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ToggleImageInitialStateExpr
		{
			get
			{
				return null;
			}
		}

		internal void SetTextBox(ReportObjectModel.ReportItem textBox)
		{
			this.m_textBox = textBox;
		}
	}
}
