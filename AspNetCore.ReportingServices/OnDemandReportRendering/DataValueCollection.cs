using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class DataValueCollection : ReportElementCollectionBase<DataValue>
	{
		private bool m_isChartValues;

		private DataValue[] m_cachedDataValues;

		private IList<AspNetCore.ReportingServices.ReportIntermediateFormat.DataValue> m_dataValues;

		private IReportScope m_reportScope;

		private Cell m_cell;

		private RenderingContext m_renderingContext;

		private string m_objectName;

		public DataValue this[string name]
		{
			get
			{
				foreach (DataValue item in this)
				{
					if (item != null && string.CompareOrdinal(name, item.Instance.Name) == 0)
					{
						return item;
					}
				}
				return null;
			}
		}

		public override DataValue this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					DataValue dataValue = null;
					if (this.m_cachedDataValues == null)
					{
						this.m_cachedDataValues = new DataValue[this.m_dataValues.Count];
					}
					else
					{
						dataValue = this.m_cachedDataValues[index];
					}
					if (dataValue == null)
					{
						dataValue = new DataValue(this.m_reportScope, this.m_renderingContext, this.m_dataValues[index], this.m_isChartValues, this.m_cell, this.m_objectName);
						this.m_cachedDataValues[index] = dataValue;
					}
					return dataValue;
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public override int Count
		{
			get
			{
				if (this.m_cachedDataValues != null)
				{
					return this.m_cachedDataValues.Length;
				}
				if (this.m_dataValues != null)
				{
					return this.m_dataValues.Count;
				}
				return 0;
			}
		}

		internal DataValueCollection(RenderingContext renderingContext, AspNetCore.ReportingServices.ReportRendering.ChartDataPoint dataPoint)
		{
			this.m_isChartValues = true;
			if (dataPoint.DataValues == null)
			{
				this.m_cachedDataValues = new DataValue[0];
			}
			else
			{
				int num = dataPoint.DataValues.Length;
				this.m_cachedDataValues = new DataValue[num];
				for (int i = 0; i < num; i++)
				{
					this.m_cachedDataValues[i] = new DataValue(renderingContext, dataPoint.DataValues[i]);
				}
			}
		}

		internal DataValueCollection(RenderingContext renderingContext, AspNetCore.ReportingServices.ReportRendering.DataCell dataCell)
		{
			this.m_isChartValues = false;
			if (dataCell.DataValues == null)
			{
				this.m_cachedDataValues = new DataValue[0];
			}
			else
			{
				int count = dataCell.DataValues.Count;
				this.m_cachedDataValues = new DataValue[count];
				for (int i = 0; i < count; i++)
				{
					this.m_cachedDataValues[i] = new DataValue(renderingContext, dataCell.DataValues[i]);
				}
			}
		}

		internal DataValueCollection(Cell cell, IReportScope reportScope, RenderingContext renderingContext, IList<AspNetCore.ReportingServices.ReportIntermediateFormat.DataValue> dataValues, string objectName, bool isChart)
		{
			this.m_isChartValues = isChart;
			this.m_cell = cell;
			this.m_reportScope = reportScope;
			this.m_dataValues = dataValues;
			this.m_renderingContext = renderingContext;
			this.m_objectName = objectName;
		}

		internal void UpdateChartDataValues(object[] datavalues)
		{
			if (this.m_isChartValues)
			{
				int num = this.m_cachedDataValues.Length;
				for (int i = 0; i < num; i++)
				{
					this.m_cachedDataValues[i].UpdateChartDataValue((datavalues == null) ? null : datavalues[i]);
				}
			}
		}

		internal void UpdateDataCellValues(AspNetCore.ReportingServices.ReportRendering.DataCell dataCell)
		{
			if (!this.m_isChartValues)
			{
				int num = this.m_cachedDataValues.Length;
				for (int i = 0; i < num; i++)
				{
					this.m_cachedDataValues[i].UpdateDataCellValue((dataCell == null || dataCell.DataValues == null) ? null : dataCell.DataValues[i]);
				}
			}
		}

		internal void SetNewContext()
		{
			if (this.m_cachedDataValues != null)
			{
				for (int i = 0; i < this.m_cachedDataValues.Length; i++)
				{
					if (this.m_cachedDataValues[i] != null)
					{
						this.m_cachedDataValues[i].SetNewContext();
					}
				}
			}
		}
	}
}
