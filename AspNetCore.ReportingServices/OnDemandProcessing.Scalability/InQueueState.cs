namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal enum InQueueState : byte
	{
		None,
		InQueue,
		Exempt
	}
}
