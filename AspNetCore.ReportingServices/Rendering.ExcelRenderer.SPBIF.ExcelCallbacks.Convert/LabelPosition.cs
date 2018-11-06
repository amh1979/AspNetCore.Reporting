namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.SPBIF.ExcelCallbacks.Convert
{
	internal sealed class LabelPosition
	{
		private string m_label;

		private long m_position;

		private long m_startPosition;

		internal string Label
		{
			get
			{
				return this.m_label;
			}
		}

		internal long Position
		{
			get
			{
				return this.m_position;
			}
		}

		internal long StartPosition
		{
			get
			{
				return this.m_startPosition;
			}
		}

		internal LabelPosition(string label, long position)
		{
			this.m_label = label;
			this.m_position = position;
		}

		internal LabelPosition(string label, long position, long startPosition)
		{
			this.m_label = label;
			this.m_position = position;
			this.m_startPosition = startPosition;
		}
	}
}
