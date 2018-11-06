using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class GroupExpressionValueCollection
	{
		private object[] m_values;

		public object this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					return this.m_values[index];
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public int Count
		{
			get
			{
				if (this.m_values != null)
				{
					return this.m_values.Length;
				}
				return 0;
			}
		}

		internal GroupExpressionValueCollection()
		{
		}

		internal void UpdateValues(object exprValue)
		{
			this.m_values = new object[1];
			this.m_values[0] = exprValue;
		}

		internal void UpdateValues(object[] exprValues)
		{
			this.m_values = exprValues;
		}
	}
}
