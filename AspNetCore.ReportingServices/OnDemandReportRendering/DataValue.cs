using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class DataValue
	{
		private bool m_isChartValue;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.DataValue m_dataValue;

		private ReportStringProperty m_name;

		private ReportVariantProperty m_value;

		private DataValueInstance m_instance;

		private IInstancePath m_instancePath;

		private RenderingContext m_renderingContext;

		private string m_objectName;

		public ReportStringProperty Name
		{
			get
			{
				return this.m_name;
			}
		}

		public ReportVariantProperty Value
		{
			get
			{
				return this.m_value;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.DataValue DataValueDef
		{
			get
			{
				return this.m_dataValue;
			}
		}

		internal RenderingContext RenderingContext
		{
			get
			{
				return this.m_renderingContext;
			}
		}

		internal bool IsChart
		{
			get
			{
				return this.m_isChartValue;
			}
		}

		internal IInstancePath InstancePath
		{
			get
			{
				return this.m_instancePath;
			}
		}

		internal string ObjectName
		{
			get
			{
				return this.m_objectName;
			}
		}

		public DataValueInstance Instance
		{
			get
			{
				if (this.m_renderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				return this.m_instance;
			}
		}

		internal DataValue(RenderingContext renderingContext, object chartDataValue)
		{
			this.m_isChartValue = true;
			this.m_name = new ReportStringProperty();
			this.m_value = new ReportVariantProperty(true);
			this.m_instance = new ShimDataValueInstance(null, chartDataValue);
			this.m_renderingContext = renderingContext;
		}

		internal DataValue(RenderingContext renderingContext, AspNetCore.ReportingServices.ReportRendering.DataValue dataValue)
		{
			this.m_isChartValue = false;
			string name = (dataValue != null) ? dataValue.Name : null;
			object value = (dataValue != null) ? dataValue.Value : null;
			this.m_name = new ReportStringProperty(true, null, null);
			this.m_value = new ReportVariantProperty(true);
			this.m_instance = new ShimDataValueInstance(name, value);
			this.m_renderingContext = renderingContext;
		}

		internal DataValue(IReportScope reportScope, RenderingContext renderingContext, AspNetCore.ReportingServices.ReportIntermediateFormat.DataValue dataValue, bool isChart, IInstancePath instancePath, string objectName)
		{
			this.m_isChartValue = isChart;
			this.m_instancePath = instancePath;
			this.m_dataValue = dataValue;
			this.m_name = new ReportStringProperty(dataValue.Name);
			this.m_value = new ReportVariantProperty(dataValue.Value);
			this.m_instance = new InternalDataValueInstance(reportScope, this);
			this.m_renderingContext = renderingContext;
			this.m_objectName = objectName;
		}

		internal void UpdateChartDataValue(object dataValue)
		{
			if (this.m_dataValue == null && this.m_isChartValue)
			{
				((ShimDataValueInstance)this.m_instance).Update(null, dataValue);
			}
		}

		internal void UpdateDataCellValue(AspNetCore.ReportingServices.ReportRendering.DataValue dataValue)
		{
			if (this.m_dataValue == null && !this.m_isChartValue)
			{
				string name = (dataValue != null) ? dataValue.Name : null;
				object value = (dataValue != null) ? dataValue.Value : null;
				((ShimDataValueInstance)this.m_instance).Update(name, value);
			}
		}

		internal void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
		}
	}
}
