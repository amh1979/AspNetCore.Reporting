using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class CheckBox : ReportItem
	{
		private ExpressionInfo m_value;

		private string m_hideDuplicates;

		[NonSerialized]
		private bool m_oldValue;

		[NonSerialized]
		private bool m_hasOldValue;

		internal override ObjectType ObjectType
		{
			get
			{
				return ObjectType.Checkbox;
			}
		}

		internal ExpressionInfo Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				this.m_value = value;
			}
		}

		internal string HideDuplicates
		{
			get
			{
				return this.m_hideDuplicates;
			}
			set
			{
				this.m_hideDuplicates = value;
			}
		}

		internal bool OldValue
		{
			get
			{
				return this.m_oldValue;
			}
			set
			{
				this.m_oldValue = value;
				this.m_hasOldValue = true;
			}
		}

		internal bool HasOldValue
		{
			get
			{
				return this.m_hasOldValue;
			}
			set
			{
				this.m_hasOldValue = value;
			}
		}

		internal CheckBox(ReportItem parent)
			: base(parent)
		{
		}

		internal CheckBox(int id, ReportItem parent)
			: base(id, parent)
		{
			base.m_height = "3.175mm";
			base.m_width = "3.175mm";
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.ObjectType = this.ObjectType;
			context.ObjectName = base.m_name;
			base.Initialize(context);
			if (base.m_visibility != null)
			{
				base.m_visibility.Initialize(context, false, false);
			}
			if (this.m_value != null)
			{
				this.m_value.Initialize("Value", context);
			}
			if (this.m_hideDuplicates != null)
			{
				context.ValidateHideDuplicateScope(this.m_hideDuplicates, this);
			}
			return true;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(false, string.Empty);
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Value, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.HideDuplicates, Token.String));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItem, memberInfoList);
		}
	}
}
