namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal interface ISearchByUniqueName
	{
		object Find(int targetUniqueName, ref NonComputedUniqueNames nonCompNames, ChunkManager.RenderingChunkManager chunkManager);
	}
}
