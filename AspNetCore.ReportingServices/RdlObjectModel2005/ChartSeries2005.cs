using AspNetCore.ReportingServices.RdlObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace AspNetCore.ReportingServices.RdlObjectModel2005
{
	internal class ChartSeries2005 : ChartSeries
	{
		internal new class Definition : DefinitionStore<ChartSeries2005, Definition.Properties>
		{
			internal enum Properties
			{
				DataPoints,
				PlotType
			}

			private Definition()
			{
			}
		}

		[XmlArrayItem("DataPoint", typeof(DataPoint2005))]
		[XmlElement(typeof(RdlCollection<DataPoint2005>))]
		public IList<DataPoint2005> DataPoints
		{
			get
			{
				return (IList<DataPoint2005>)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		[DefaultValue(PlotTypes2005.Auto)]
		public PlotTypes2005 PlotType
		{
			get
			{
				return (PlotTypes2005)base.PropertyStore.GetInteger(1);
			}
			set
			{
				base.PropertyStore.SetInteger(1, (int)value);
			}
		}

		public ChartSeries2005()
		{
		}

		public ChartSeries2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			this.DataPoints = new RdlCollection<DataPoint2005>();
		}
	}
}
