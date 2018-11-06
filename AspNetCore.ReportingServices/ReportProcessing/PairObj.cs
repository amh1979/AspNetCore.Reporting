namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal class PairObj<T, U>
	{
		internal T First;

		internal U Second;

		internal PairObj(T first, U second)
		{
			this.First = first;
			this.Second = second;
		}
	}
}
