namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal struct Space
	{
		internal long Offset;

		internal long Size;

		internal Space(long freeOffset, long freeSize)
		{
			this.Offset = freeOffset;
			this.Size = freeSize;
		}
	}
}
