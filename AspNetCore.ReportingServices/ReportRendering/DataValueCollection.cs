using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class DataValueCollection
	{
		private DataValueInstanceList m_instances;

		private DataValueCRIList m_expressions;

		public DataValue this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					string name = null;
					object value = null;
					if (ExpressionInfo.Types.Constant == ((DataValueList)this.m_expressions)[index].Name.Type)
					{
						name = ((DataValueList)this.m_expressions)[index].Name.Value;
					}
					else if (this.m_instances != null)
					{
						name = this.m_instances[index].Name;
					}
					if (ExpressionInfo.Types.Constant == ((DataValueList)this.m_expressions)[index].Value.Type)
					{
						value = ((DataValueList)this.m_expressions)[index].Value.Value;
					}
					else if (this.m_instances != null)
					{
						value = this.m_instances[index].Value;
					}
					return new DataValue(name, value);
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public int Count
		{
			get
			{
				return this.m_expressions.Count;
			}
		}

		internal DataValueCollection(DataValueCRIList expressions, DataValueInstanceList instances)
		{
			this.m_expressions = expressions;
			this.m_instances = instances;
			Global.Tracer.Assert(null != this.m_expressions);
			Global.Tracer.Assert(this.m_instances == null || this.m_instances.Count == this.m_expressions.Count);
		}
	}
}
