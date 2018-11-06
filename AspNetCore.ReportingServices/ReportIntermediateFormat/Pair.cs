namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal struct Pair<T, U>
	{
		internal T First;

		internal U Second;

		internal Pair(T first, U second)
		{
			this.First = first;
			this.Second = second;
		}
	}
}
