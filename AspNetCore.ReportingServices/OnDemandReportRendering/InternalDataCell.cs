using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalDataCell : DataCell
	{
		private AspNetCore.ReportingServices.ReportIntermediateFormat.DataCell m_dataCellDef;

		public override DataValueCollection DataValues
		{
			get
			{
				if (base.m_dataValues == null)
				{
					base.m_dataValues = new DataValueCollection(this.m_dataCellDef, this, base.m_owner.RenderingContext, this.m_dataCellDef.DataValues, base.m_owner.Name, false);
				}
				return base.m_dataValues;
			}
		}

		internal override AspNetCore.ReportingServices.ReportIntermediateFormat.DataCell DataCellDef
		{
			get
			{
				return this.m_dataCellDef;
			}
		}

		internal override AspNetCore.ReportingServices.ReportRendering.DataCell RenderItem
		{
			get
			{
				return null;
			}
		}

		internal override IRIFReportScope RIFReportScope
		{
			get
			{
				return this.m_dataCellDef;
			}
		}

		internal InternalDataCell(CustomReportItem owner, int rowIndex, int colIndex, AspNetCore.ReportingServices.ReportIntermediateFormat.DataCell dataCellDef)
			: base(owner, rowIndex, colIndex)
		{
			this.m_dataCellDef = dataCellDef;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (this.m_dataCellDef != null)
			{
				this.m_dataCellDef.ClearStreamingScopeInstanceBinding();
			}
		}
	}
}
