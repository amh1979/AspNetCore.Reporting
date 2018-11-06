namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.BIFF8.Records
{
	internal struct Pair<T, U>
	{
		private T m_first;

		private U m_second;

		internal T First
		{
			get
			{
				return this.m_first;
			}
		}

		internal U Second
		{
			get
			{
				return this.m_second;
			}
		}

		internal Pair(T first, U second)
		{
			this.m_first = first;
			this.m_second = second;
		}
	}
}
