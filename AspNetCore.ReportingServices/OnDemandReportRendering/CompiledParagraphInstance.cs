using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class CompiledParagraphInstance : ParagraphInstance, ICompiledParagraphInstance
	{
		private CompiledRichTextInstance m_compiledRichTextInstance;

		private CompiledTextRunInstanceCollection m_compiledTextRunInstances;

		private ReportSize m_leftIndent;

		private ReportSize m_rightIndent;

		private ReportSize m_hangingIndent;

		private ListStyle m_listStyle;

		private int m_listLevel;

		private ReportSize m_spaceBefore;

		private ReportSize m_spaceAfter;

		private InternalParagraphInstance NativeParagraphInstance
		{
			get
			{
				return (InternalParagraphInstance)base.Definition.Instance;
			}
		}

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

		public override ReportSize LeftIndent
		{
			get
			{
				if (this.m_leftIndent == null)
				{
					this.m_leftIndent = this.NativeParagraphInstance.GetLeftIndent(false);
				}
				return this.m_leftIndent;
			}
		}

		public override ReportSize RightIndent
		{
			get
			{
				if (this.m_rightIndent == null)
				{
					this.m_rightIndent = this.NativeParagraphInstance.GetRightIndent(false);
				}
				return this.m_rightIndent;
			}
		}

		public override ReportSize HangingIndent
		{
			get
			{
				if (this.m_hangingIndent == null)
				{
					this.m_hangingIndent = this.NativeParagraphInstance.GetHangingIndent(false);
				}
				return this.m_hangingIndent;
			}
		}

		public override ListStyle ListStyle
		{
			get
			{
				if (this.m_listStyle == ListStyle.None)
				{
					return this.NativeParagraphInstance.ListStyle;
				}
				return this.m_listStyle;
			}
		}

		public override int ListLevel
		{
			get
			{
				return this.m_listLevel + this.NativeParagraphInstance.ListLevel;
			}
		}

		public override ReportSize SpaceBefore
		{
			get
			{
				if (this.m_spaceBefore == null)
				{
					this.m_spaceBefore = this.NativeParagraphInstance.GetSpaceBefore(false);
				}
				return this.m_spaceBefore;
			}
		}

		public override ReportSize SpaceAfter
		{
			get
			{
				if (this.m_spaceAfter == null)
				{
					this.m_spaceAfter = this.NativeParagraphInstance.GetSpaceAfter(false);
				}
				return this.m_spaceAfter;
			}
		}

		public CompiledTextRunInstanceCollection CompiledTextRunInstances
		{
			get
			{
				return this.m_compiledTextRunInstances;
			}
			internal set
			{
				this.m_compiledTextRunInstances = value;
			}
		}

		internal TextRun TextRunDefinition
		{
			get
			{
				return this.m_compiledRichTextInstance.TextRunDefinition;
			}
		}

		public override bool IsCompiled
		{
			get
			{
				return true;
			}
		}

		IList<ICompiledTextRunInstance> ICompiledParagraphInstance.CompiledTextRunInstances
		{
			get
			{
				return this.m_compiledTextRunInstances;
			}
			set
			{
				this.m_compiledTextRunInstances = (CompiledTextRunInstanceCollection)value;
			}
		}

		ICompiledStyleInstance ICompiledParagraphInstance.Style
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

		ReportSize ICompiledParagraphInstance.LeftIndent
		{
			get
			{
				return this.m_leftIndent;
			}
			set
			{
				this.m_leftIndent = value;
			}
		}

		ReportSize ICompiledParagraphInstance.RightIndent
		{
			get
			{
				return this.m_rightIndent;
			}
			set
			{
				this.m_rightIndent = value;
			}
		}

		ReportSize ICompiledParagraphInstance.HangingIndent
		{
			get
			{
				return this.m_hangingIndent;
			}
			set
			{
				this.m_hangingIndent = value;
			}
		}

		ListStyle ICompiledParagraphInstance.ListStyle
		{
			get
			{
				return this.m_listStyle;
			}
			set
			{
				this.m_listStyle = value;
			}
		}

		int ICompiledParagraphInstance.ListLevel
		{
			get
			{
				return this.m_listLevel;
			}
			set
			{
				this.m_listLevel = value;
			}
		}

		ReportSize ICompiledParagraphInstance.SpaceBefore
		{
			get
			{
				return this.m_spaceBefore;
			}
			set
			{
				this.m_spaceBefore = value;
			}
		}

		ReportSize ICompiledParagraphInstance.SpaceAfter
		{
			get
			{
				return this.m_spaceAfter;
			}
			set
			{
				this.m_spaceAfter = value;
			}
		}

		internal CompiledParagraphInstance(CompiledRichTextInstance compiledRichTextInstance)
			: base(compiledRichTextInstance.ParagraphDefinition)
		{
			this.m_compiledRichTextInstance = compiledRichTextInstance;
		}
	}
}
