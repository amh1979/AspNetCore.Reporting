using System.Security.Permissions;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	[StrongNameIdentityPermission(SecurityAction.InheritanceDemand, PublicKey = "0024000004800000940000000602000000240000525341310004000001000100272736ad6e5f9586bac2d531eabc3acc666c2f8ec879fa94f8f7b0327d2ff2ed523448f83c3d5c5dd2dfc7bc99c5286b2c125117bf5cbe242b9d41750732b2bdffe649c6efb8e5526d526fdd130095ecdb7bf210809c6cdad8824faa9ac0310ac3cba2aa0523567b2dfa7fe250b30facbd62d4ec99b94ac47c7d3b28f1f6e4c8")]
	internal abstract class ParagraphInstance : ReportElementInstance
	{
		protected TextRunInstanceCollection m_textRunInstances;

		protected string m_uniqueName;

		public abstract string UniqueName
		{
			get;
		}

		public virtual ReportSize LeftIndent
		{
			get
			{
				return null;
			}
		}

		public virtual ReportSize RightIndent
		{
			get
			{
				return null;
			}
		}

		public virtual ReportSize HangingIndent
		{
			get
			{
				return null;
			}
		}

		public virtual ListStyle ListStyle
		{
			get
			{
				return ListStyle.None;
			}
		}

		public virtual int ListLevel
		{
			get
			{
				return 0;
			}
		}

		public virtual ReportSize SpaceBefore
		{
			get
			{
				return null;
			}
		}

		public virtual ReportSize SpaceAfter
		{
			get
			{
				return null;
			}
		}

		public Paragraph Definition
		{
			get
			{
				return (Paragraph)base.m_reportElementDef;
			}
		}

		public TextRunInstanceCollection TextRunInstances
		{
			get
			{
				if (this.m_textRunInstances == null)
				{
					this.m_textRunInstances = new TextRunInstanceCollection(this);
				}
				return this.m_textRunInstances;
			}
		}

		public abstract bool IsCompiled
		{
			get;
		}

		internal ParagraphInstance(Paragraph paragraphDef)
			: base(paragraphDef)
		{
		}

		protected ParagraphInstance(ReportElement reportElementDef)
			: base(reportElementDef)
		{
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_uniqueName = null;
		}
	}
}
