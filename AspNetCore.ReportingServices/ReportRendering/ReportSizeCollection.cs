using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal class ReportSizeCollection
	{
		private ReportSize[] m_reportSizeCollection;

		public virtual ReportSize this[int index]
		{
			get
			{
				if (0 <= index && index < this.Count)
				{
					return this.m_reportSizeCollection[index];
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
			set
			{
				if (0 <= index && index < this.Count)
				{
					this.m_reportSizeCollection[index] = value;
					return;
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public virtual int Count
		{
			get
			{
				if (this.m_reportSizeCollection == null)
				{
					return 0;
				}
				return this.m_reportSizeCollection.Length;
			}
		}

		internal ReportSizeCollection()
		{
		}

		internal ReportSizeCollection(int count)
		{
			this.m_reportSizeCollection = new ReportSize[count];
		}
	}
}
