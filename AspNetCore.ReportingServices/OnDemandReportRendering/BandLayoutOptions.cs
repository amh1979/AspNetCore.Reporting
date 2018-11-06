using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class BandLayoutOptions
	{
		private readonly AspNetCore.ReportingServices.ReportIntermediateFormat.BandLayoutOptions m_bandLayoutDef;

		private Navigation m_navigation;

		public int RowCount
		{
			get
			{
				return this.m_bandLayoutDef.RowCount;
			}
		}

		public int ColumnCount
		{
			get
			{
				return this.m_bandLayoutDef.ColumnCount;
			}
		}

		public Navigation Navigation
		{
			get
			{
				if (this.m_navigation == null && this.m_bandLayoutDef.Navigation != null)
				{
					switch (this.m_bandLayoutDef.Navigation.GetObjectType())
					{
					case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Coverflow:
						this.m_navigation = new Coverflow(this.m_bandLayoutDef);
						break;
					case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PlayAxis:
						this.m_navigation = new PlayAxis(this.m_bandLayoutDef);
						break;
					case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Tabstrip:
						this.m_navigation = new Tabstrip(this.m_bandLayoutDef);
						break;
					default:
						Global.Tracer.Assert(false, "Unknown Band Navigation Type: {0}", this.m_bandLayoutDef.Navigation.GetObjectType());
						break;
					}
				}
				return this.m_navigation;
			}
		}

		internal BandLayoutOptions(AspNetCore.ReportingServices.ReportIntermediateFormat.BandLayoutOptions bandLayoutDef)
		{
			this.m_bandLayoutDef = bandLayoutDef;
		}
	}
}
