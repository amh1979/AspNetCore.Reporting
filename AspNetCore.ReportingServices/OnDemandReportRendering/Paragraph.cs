using AspNetCore.ReportingServices.ReportRendering;
using System.Security.Permissions;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	[StrongNameIdentityPermission(SecurityAction.InheritanceDemand, PublicKey = "0024000004800000940000000602000000240000525341310004000001000100272736ad6e5f9586bac2d531eabc3acc666c2f8ec879fa94f8f7b0327d2ff2ed523448f83c3d5c5dd2dfc7bc99c5286b2c125117bf5cbe242b9d41750732b2bdffe649c6efb8e5526d526fdd130095ecdb7bf210809c6cdad8824faa9ac0310ac3cba2aa0523567b2dfa7fe250b30facbd62d4ec99b94ac47c7d3b28f1f6e4c8")]
	internal abstract class Paragraph : ReportElement
	{
		protected ReportIntProperty m_listLevel;

		protected ReportEnumProperty<ListStyle> m_listStyle;

		private string m_definitionPath;

		protected int m_indexIntoParentCollectionDef;

		protected ParagraphInstance m_instance;

		protected TextRunCollection m_textRunCollection;

		protected TextBox m_textBox;

		public override string DefinitionPath
		{
			get
			{
				if (this.m_definitionPath == null)
				{
					this.m_definitionPath = DefinitionPathConstants.GetCollectionDefinitionPath(base.m_parentDefinitionPath, this.m_indexIntoParentCollectionDef);
				}
				return this.m_definitionPath;
			}
		}

		internal override string InstanceUniqueName
		{
			get
			{
				if (base.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				return this.Instance.UniqueName;
			}
		}

		public TextRunCollection TextRuns
		{
			get
			{
				if (this.m_textRunCollection == null)
				{
					this.m_textRunCollection = new TextRunCollection(this);
				}
				return this.m_textRunCollection;
			}
		}

		public virtual ReportSizeProperty LeftIndent
		{
			get
			{
				return null;
			}
		}

		public virtual ReportSizeProperty RightIndent
		{
			get
			{
				return null;
			}
		}

		public virtual ReportSizeProperty HangingIndent
		{
			get
			{
				return null;
			}
		}

		public abstract ReportEnumProperty<ListStyle> ListStyle
		{
			get;
		}

		public abstract ReportIntProperty ListLevel
		{
			get;
		}

		public virtual ReportSizeProperty SpaceBefore
		{
			get
			{
				return null;
			}
		}

		public virtual ReportSizeProperty SpaceAfter
		{
			get
			{
				return null;
			}
		}

		internal TextBox TextBox
		{
			get
			{
				return this.m_textBox;
			}
		}

		internal override ReportElementInstance ReportElementInstance
		{
			get
			{
				return this.Instance;
			}
		}

		public new abstract ParagraphInstance Instance
		{
			get;
		}

		internal Paragraph(TextBox textBox, int indexIntoParentCollectionDef, RenderingContext renderingContext)
			: base(textBox.ReportScope, textBox, textBox.ReportItemDef, renderingContext)
		{
			this.m_textBox = textBox;
			this.m_indexIntoParentCollectionDef = indexIntoParentCollectionDef;
		}

		internal Paragraph(TextBox textBox, RenderingContext renderingContext)
			: base(textBox, textBox.RenderReportItem, renderingContext)
		{
			this.m_textBox = textBox;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
		}

		internal override void SetNewContextChildren()
		{
			if (this.m_textRunCollection != null)
			{
				this.m_textRunCollection.SetNewContext();
			}
		}

		internal virtual void UpdateRenderReportItem(AspNetCore.ReportingServices.ReportRendering.ReportItem renderReportItem)
		{
			if (this.m_textRunCollection != null && ((ReportElementCollectionBase<TextRun>)this.m_textRunCollection)[0] != null)
			{
				((ReportElementCollectionBase<TextRun>)this.m_textRunCollection)[0].UpdateRenderReportItem(renderReportItem);
			}
		}
	}
}
