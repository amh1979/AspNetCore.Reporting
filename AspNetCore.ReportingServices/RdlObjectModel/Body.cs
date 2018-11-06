using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace AspNetCore.ReportingServices.RdlObjectModel
{
	internal class Body : ReportElement
	{
		internal new class Definition : DefinitionStore<Report, Definition.Properties>
		{
			internal enum Properties
			{
				Style,
				ReportItems,
				Height,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[XmlElement(typeof(RdlCollection<ReportItem>))]
		public IList<ReportItem> ReportItems
		{
			get
			{
				return (IList<ReportItem>)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}
        [DefaultValue(0.0)]
		public ReportSize Height
		{
			get
			{
				return base.PropertyStore.GetSize(2);
			}
			set
			{
				base.PropertyStore.SetSize(2, value);
			}
		}

		public Body()
		{
		}

		internal Body(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			this.ReportItems = new RdlCollection<ReportItem>();
			this.Height = Constants.DefaultZeroSize;
		}
	}
}
