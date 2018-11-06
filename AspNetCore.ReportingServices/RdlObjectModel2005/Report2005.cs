using AspNetCore.ReportingServices.RdlObjectModel;
using AspNetCore.ReportingServices.RdlObjectModel.Serialization;
using AspNetCore.ReportingServices.RdlObjectModel2005.Upgrade;
using AspNetCore.ReportingServices.RdlObjectModel2008;

namespace AspNetCore.ReportingServices.RdlObjectModel2005
{
	[XmlElementClass("Report", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2005/01/reportdefinition")]
	internal class Report2005 : Report2008, IUpgradeable
	{
		internal new class Definition : DefinitionStore<Report2005, Definition.Properties>
		{
			internal enum Properties
			{
				PropertyCount = 28
			}

			private Definition()
			{
			}
		}

		public new const string DesignerNamespace = "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner";

		public PageSection PageHeader
		{
			get
			{
				return this.Page.PageHeader;
			}
			set
			{
				this.Page.PageHeader = value;
			}
		}

		public PageSection PageFooter
		{
			get
			{
				return this.Page.PageFooter;
			}
			set
			{
				this.Page.PageFooter = value;
			}
		}

		public ReportSize PageHeight
		{
			get
			{
				return this.Page.PageHeight;
			}
			set
			{
				this.Page.PageHeight = value;
			}
		}

		public ReportSize PageWidth
		{
			get
			{
				return this.Page.PageWidth;
			}
			set
			{
				this.Page.PageWidth = value;
			}
		}

		public ReportSize InteractiveHeight
		{
			get
			{
				return this.Page.InteractiveHeight;
			}
			set
			{
				this.Page.InteractiveHeight = value;
			}
		}

		public ReportSize InteractiveWidth
		{
			get
			{
				return this.Page.InteractiveWidth;
			}
			set
			{
				this.Page.InteractiveWidth = value;
			}
		}

		public ReportSize LeftMargin
		{
			get
			{
				return this.Page.LeftMargin;
			}
			set
			{
				this.Page.LeftMargin = value;
			}
		}

		public ReportSize RightMargin
		{
			get
			{
				return this.Page.RightMargin;
			}
			set
			{
				this.Page.RightMargin = value;
			}
		}

		public ReportSize TopMargin
		{
			get
			{
				return this.Page.TopMargin;
			}
			set
			{
				this.Page.TopMargin = value;
			}
		}

		public ReportSize BottomMargin
		{
			get
			{
				return this.Page.BottomMargin;
			}
			set
			{
				this.Page.BottomMargin = value;
			}
		}

		public new DataElementStyles2005 DataElementStyle
		{
			get
			{
				return (DataElementStyles2005)base.DataElementStyle;
			}
			set
			{
				base.DataElementStyle = (DataElementStyles)value;
			}
		}

		public Report2005()
		{
		}

		public Report2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public void Upgrade(UpgradeImpl2005 upgrader)
		{
			upgrader.UpgradeReport(this);
		}
	}
}
