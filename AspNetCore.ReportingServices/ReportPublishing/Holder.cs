namespace AspNetCore.ReportingServices.ReportPublishing
{
	internal class Holder<T> where T : struct
	{
		private T m_t = default(T);

		internal T Value
		{
			get
			{
				return this.m_t;
			}
			set
			{
				this.m_t = value;
			}
		}
	}
}
