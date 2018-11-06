using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class StyleEnumerator : IEnumerator
	{
		private StyleProperties m_sharedProperties;

		private StyleProperties m_nonSharedProperties;

		private int m_total;

		private int m_current = -1;

		public object Current
		{
			get
			{
				if (0 > this.m_current)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				int num = 0;
				if (this.m_sharedProperties != null)
				{
					num = this.m_sharedProperties.Count;
				}
				if (this.m_current < num)
				{
					return this.m_sharedProperties[this.m_current];
				}
				Global.Tracer.Assert(null != this.m_nonSharedProperties);
				return this.m_nonSharedProperties[this.m_current - num];
			}
		}

		internal StyleEnumerator(StyleProperties sharedProps, StyleProperties nonSharedProps)
		{
			this.m_sharedProperties = sharedProps;
			this.m_nonSharedProperties = nonSharedProps;
			this.m_total = 0;
			if (this.m_sharedProperties != null)
			{
				this.m_total += this.m_sharedProperties.Count;
			}
			if (this.m_nonSharedProperties != null)
			{
				this.m_total += this.m_nonSharedProperties.Count;
			}
		}

		public bool MoveNext()
		{
			if (this.m_current < this.m_total - 1)
			{
				this.m_current++;
				return true;
			}
			return false;
		}

		public void Reset()
		{
			this.m_current = -1;
		}
	}
}
