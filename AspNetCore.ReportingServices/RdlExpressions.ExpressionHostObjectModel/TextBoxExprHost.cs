using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class TextBoxExprHost : ReportItemExprHost
	{
		public const string ValueName = "Value";

		[CLSCompliant(false)]
		protected IList<ParagraphExprHost> m_paragraphHostsRemotable;

		private ReportItem m_textBox;

		internal IList<ParagraphExprHost> ParagraphHostsRemotable
		{
			get
			{
				return this.m_paragraphHostsRemotable;
			}
		}

		public object Value
		{
			get
			{
				return this.m_textBox.Value;
			}
		}

		internal ReportItem ReportObjectModelTextBox
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

		internal void SetTextBox(ReportItem textBox)
		{
			this.m_textBox = textBox;
		}
	}
}
