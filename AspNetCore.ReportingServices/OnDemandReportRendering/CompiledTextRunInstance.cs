using System;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class CompiledTextRunInstance : TextRunInstance, ICompiledTextRunInstance
	{
		private CompiledRichTextInstance m_compiledRichTextInstance;

		private MarkupType m_markupType;

		private string m_toolTip;

		private string m_label;

		private string m_value;

		private ActionInstance m_actionInstance;

		public override string UniqueName
		{
			get
			{
				if (base.m_uniqueName == null)
				{
					base.m_uniqueName = base.m_reportElementDef.InstanceUniqueName + 'x' + this.m_compiledRichTextInstance.GenerateID();
				}
				return base.m_uniqueName;
			}
		}

		public override StyleInstance Style
		{
			get
			{
				return base.m_style;
			}
		}

		public override string Value
		{
			get
			{
				return this.m_value ?? "";
			}
		}

		public override object OriginalValue
		{
			get
			{
				return this.m_value ?? "";
			}
		}

		public override string ToolTip
		{
			get
			{
				if (this.m_toolTip == null)
				{
					this.m_toolTip = base.Definition.Instance.ToolTip;
				}
				return this.m_toolTip;
			}
		}

		public override MarkupType MarkupType
		{
			get
			{
				return this.m_markupType;
			}
		}

		public ActionInstance ActionInstance
		{
			get
			{
				if (this.m_actionInstance == null && base.Definition.ActionInfo != null)
				{
					ActionCollection actions = base.Definition.ActionInfo.Actions;
					if (actions != null && actions.Count > 0)
					{
						this.m_actionInstance = ((ReportElementCollectionBase<Action>)actions)[0].Instance;
					}
				}
				return this.m_actionInstance;
			}
		}

		public override TypeCode TypeCode
		{
			get
			{
				return TypeCode.String;
			}
		}

		public override bool IsCompiled
		{
			get
			{
				return true;
			}
		}

		public override bool ProcessedWithError
		{
			get
			{
				return false;
			}
		}

		ICompiledStyleInstance ICompiledTextRunInstance.Style
		{
			get
			{
				return (ICompiledStyleInstance)base.m_style;
			}
			set
			{
				base.m_style = (CompiledRichTextStyleInstance)value;
			}
		}

		string ICompiledTextRunInstance.Label
		{
			get
			{
				return this.m_label;
			}
			set
			{
				if (value == null)
				{
					this.m_label = string.Empty;
				}
				else
				{
					this.m_label = value;
				}
			}
		}

		string ICompiledTextRunInstance.Value
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

		string ICompiledTextRunInstance.ToolTip
		{
			get
			{
				return this.m_toolTip;
			}
			set
			{
				if (value == null)
				{
					this.m_toolTip = string.Empty;
				}
				else
				{
					this.m_toolTip = value;
				}
			}
		}

		MarkupType ICompiledTextRunInstance.MarkupType
		{
			get
			{
				return this.m_markupType;
			}
			set
			{
				this.m_markupType = value;
			}
		}

		IActionInstance ICompiledTextRunInstance.ActionInstance
		{
			get
			{
				return this.m_actionInstance;
			}
			set
			{
				this.m_actionInstance = (ActionInstance)value;
			}
		}

		internal CompiledTextRunInstance(CompiledRichTextInstance compiledRichTextInstance)
			: base(compiledRichTextInstance.TextRunDefinition)
		{
			this.m_compiledRichTextInstance = compiledRichTextInstance;
		}
	}
}
