using AspNetCore.ReportingServices.RdlObjectModel;
using AspNetCore.ReportingServices.RdlObjectModel.Serialization;
using AspNetCore.ReportingServices.RdlObjectModel2008.Upgrade;
using AspNetCore.ReportingServices.RdlObjectModel2010;

namespace AspNetCore.ReportingServices.RdlObjectModel2008
{
	[XmlElementClass("Report", Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")]
	internal class Report2008 : Report2010, IUpgradeable2008
	{
		public new class Definition : DefinitionStore<Report2008, Definition.Properties>
		{
			internal enum Properties
			{
				Body = 25,
				Width,
				Page,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public new const string DesignerNamespace = "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner";

		public override Body Body
		{
			get
			{
				return (Body)base.PropertyStore.GetObject(25);
			}
			set
			{
				base.PropertyStore.SetObject(25, value);
			}
		}

		public override ReportSize Width
		{
			get
			{
				return base.PropertyStore.GetSize(26);
			}
			set
			{
				base.PropertyStore.SetSize(26, value);
			}
		}

		public override Page Page
		{
			get
			{
				return (Page)base.PropertyStore.GetObject(27);
			}
			set
			{
				base.PropertyStore.SetObject(27, value);
			}
		}

		public Report2008()
		{
		}

		public Report2008(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			this.Width = Constants.DefaultZeroSize;
			this.Body = new Body();
			this.Page = new Page();
			base.Initialize();
		}

		public void Upgrade(UpgradeImpl2008 upgrader)
		{
			upgrader.UpgradeReport(this);
		}
	}
}
