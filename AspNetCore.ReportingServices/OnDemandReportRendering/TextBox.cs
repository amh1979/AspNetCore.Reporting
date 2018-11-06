using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportRendering;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class TextBox : ReportItem, IROMActionOwner
	{
		private ActionInfo m_actionInfo;

		private AspNetCore.ReportingServices.ReportRendering.TextBox m_renderTextBox;

		private ParagraphCollection m_paragraphCollection;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox m_textBoxDef;

		public override Style Style
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					if (base.m_style == null)
					{
						base.m_style = new TextBoxFilteredStyle(this.RenderReportItem, base.RenderingContext, this.UseRenderStyle);
					}
					return base.m_style;
				}
				return base.Style;
			}
		}

		public bool CanScrollVertically
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return false;
				}
				return this.m_textBoxDef.CanScrollVertically;
			}
		}

		public bool CanGrow
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return this.m_renderTextBox.CanGrow;
				}
				return this.m_textBoxDef.CanGrow;
			}
		}

		public bool CanShrink
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return this.m_renderTextBox.CanShrink;
				}
				return this.m_textBoxDef.CanShrink;
			}
		}

		public bool HideDuplicates
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return this.m_renderTextBox.HideDuplicates;
				}
				if (this.m_textBoxDef.IsSimple)
				{
					return this.m_textBoxDef.HideDuplicates != null;
				}
				return false;
			}
		}

		public string UniqueName
		{
			get
			{
				return base.m_reportItemDef.UniqueName;
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				if (this.m_actionInfo == null)
				{
					if (base.m_isOldSnapshot)
					{
						if (this.m_renderTextBox.ActionInfo != null)
						{
							this.m_actionInfo = new ActionInfo(base.RenderingContext, this.m_renderTextBox.ActionInfo);
						}
					}
					else if (this.m_textBoxDef.Action != null)
					{
						this.m_actionInfo = new ActionInfo(base.RenderingContext, this.ReportScope, this.m_textBoxDef.Action, base.m_reportItemDef, this, base.m_reportItemDef.ObjectType, base.m_reportItemDef.Name, this);
					}
				}
				return this.m_actionInfo;
			}
		}

		public TypeCode SharedTypeCode
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return this.m_renderTextBox.SharedTypeCode;
				}
				if (this.IsSimple)
				{
					return ((ReportElementCollectionBase<TextRun>)((ReportElementCollectionBase<Paragraph>)this.Paragraphs)[0].TextRuns)[0].SharedTypeCode;
				}
				return TypeCode.String;
			}
		}

		public bool IsToggleParent
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return this.m_renderTextBox.IsSharedToggleParent;
				}
				return this.m_textBoxDef.IsToggle;
			}
		}

		public bool CanSort
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return this.m_renderTextBox.CanSort;
				}
				return null != this.m_textBoxDef.UserSort;
			}
		}

		public Report.DataElementStyles DataElementStyle
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return (Report.DataElementStyles)this.m_renderTextBox.DataElementStyle;
				}
				if (!this.m_textBoxDef.DataElementStyleAttribute)
				{
					return Report.DataElementStyles.Element;
				}
				return Report.DataElementStyles.Attribute;
			}
		}

		public bool KeepTogether
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return true;
				}
				return this.m_textBoxDef.KeepTogether;
			}
		}

		public ParagraphCollection Paragraphs
		{
			get
			{
				if (this.m_paragraphCollection == null)
				{
					this.m_paragraphCollection = new ParagraphCollection(this);
				}
				return this.m_paragraphCollection;
			}
		}

		public bool IsSimple
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return true;
				}
				return this.m_textBoxDef.IsSimple;
			}
		}

		public bool FormattedValueExpressionBased
		{
			get
			{
				if (this.IsSimple)
				{
					return ((ReportElementCollectionBase<TextRun>)((ReportElementCollectionBase<Paragraph>)this.Paragraphs)[0].TextRuns)[0].FormattedValueExpressionBased;
				}
				return false;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox TexBoxDef
		{
			get
			{
				return this.m_textBoxDef;
			}
		}

		List<string> IROMActionOwner.FieldsUsedInValueExpression
		{
			get
			{
				if (base.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				TextBoxInstance textBoxInstance = (TextBoxInstance)this.GetOrCreateInstance();
				return textBoxInstance.GetFieldsUsedInValueExpression();
			}
		}

		internal TextBox(IReportScope reportScope, IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox reportItemDef, RenderingContext renderingContext)
			: base(reportScope, parentDefinitionPath, indexIntoParentCollectionDef, reportItemDef, renderingContext)
		{
			this.m_textBoxDef = reportItemDef;
		}

		internal TextBox(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, bool inSubtotal, AspNetCore.ReportingServices.ReportRendering.TextBox renderTextBox, RenderingContext renderingContext)
			: base(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, renderTextBox, renderingContext)
		{
			this.m_renderTextBox = renderTextBox;
		}

		internal override ReportItemInstance GetOrCreateInstance()
		{
			if (base.m_instance == null)
			{
				base.m_instance = new TextBoxInstance(this);
			}
			return base.m_instance;
		}

		internal override void UpdateRenderReportItem(AspNetCore.ReportingServices.ReportRendering.ReportItem renderReportItem)
		{
			base.UpdateRenderReportItem(renderReportItem);
			this.m_renderTextBox = (AspNetCore.ReportingServices.ReportRendering.TextBox)base.m_renderReportItem;
			if (this.m_actionInfo != null)
			{
				this.m_actionInfo.Update(this.m_renderTextBox.ActionInfo);
			}
			if (this.m_paragraphCollection != null && ((ReportElementCollectionBase<Paragraph>)this.m_paragraphCollection)[0] != null)
			{
				((ReportElementCollectionBase<Paragraph>)this.m_paragraphCollection)[0].UpdateRenderReportItem(renderReportItem);
			}
		}

		internal override void SetNewContextChildren()
		{
			base.SetNewContextChildren();
			if (this.m_actionInfo != null)
			{
				this.m_actionInfo.SetNewContext();
			}
			if (this.m_paragraphCollection != null)
			{
				this.m_paragraphCollection.SetNewContext();
			}
		}
	}
}
