using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TextBoxInstanceInfo : ReportItemInstanceInfo, IShowHideSender
	{
		private string m_formattedValue;

		private object m_originalValue;

		private bool m_duplicate;

		private ActionInstance m_action;

		private bool m_initialToggleState;

		internal string FormattedValue
		{
			get
			{
				return this.m_formattedValue;
			}
			set
			{
				this.m_formattedValue = value;
			}
		}

		internal object OriginalValue
		{
			get
			{
				return this.m_originalValue;
			}
			set
			{
				this.m_originalValue = value;
			}
		}

		internal bool Duplicate
		{
			get
			{
				return this.m_duplicate;
			}
			set
			{
				this.m_duplicate = value;
			}
		}

		internal ActionInstance Action
		{
			get
			{
				return this.m_action;
			}
			set
			{
				this.m_action = value;
			}
		}

		internal bool InitialToggleState
		{
			get
			{
				return this.m_initialToggleState;
			}
			set
			{
				this.m_initialToggleState = value;
			}
		}

		internal TextBoxInstanceInfo(ReportProcessing.ProcessingContext pc, TextBox reportItemDef, TextBoxInstance owner, int index)
			: base(pc, reportItemDef, owner, index)
		{
		}

		internal TextBoxInstanceInfo(TextBox reportItemDef)
			: base(reportItemDef)
		{
		}

		void IShowHideSender.ProcessSender(ReportProcessing.ProcessingContext context, int uniqueName)
		{
			this.m_initialToggleState = context.ProcessSender(uniqueName, base.m_startHidden, (TextBox)base.m_reportItemDef);
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.FormattedValue, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.OriginalValue, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.Duplicate, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.Action, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ActionInstance));
			memberInfoList.Add(new MemberInfo(MemberName.InitialToggleState, Token.Boolean));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstanceInfo, memberInfoList);
		}
	}
}
