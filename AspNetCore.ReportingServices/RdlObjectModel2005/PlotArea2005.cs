using AspNetCore.ReportingServices.RdlObjectModel;

namespace AspNetCore.ReportingServices.RdlObjectModel2005
{
	internal class PlotArea2005 : ReportObject
	{
		internal class Definition : DefinitionStore<PlotArea2005, Definition.Properties>
		{
			internal enum Properties
			{
				Style
			}

			private Definition()
			{
			}
		}

		public Style2005 Style
		{
			get
			{
				return (Style2005)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		public PlotArea2005()
		{
		}

		public PlotArea2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
