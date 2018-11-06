namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class LookupScalabilityCache : PartitionedTreeScalabilityCache
	{
		private const long CacheExpansionIntervalMs = 2000L;

		private const double CacheExpansionRatio = 0.3;

		private const long MinReservedMemoryBytes = 2097152L;

		public override ScalabilityCacheType CacheType
		{
			get
			{
				return ScalabilityCacheType.Lookup;
			}
		}

		internal LookupScalabilityCache(TreePartitionManager partitionManager, IStorage storage)
			: base(partitionManager, storage, 2000L, 0.3, 2097152L)
		{
		}

		internal override BaseReference TransferTo(BaseReference reference)
		{
			IStorable storable = reference.InternalValue();
			BaseReference baseReference = base.AllocateAndPin(storable, ItemSizes.SizeOf(storable));
			ITransferable transferable = storable as ITransferable;
			if (transferable != null)
			{
				transferable.TransferTo(this);
			}
			baseReference.UnPinValue();
			reference.ScalabilityCache.Free(reference);
			return baseReference;
		}
	}
}
