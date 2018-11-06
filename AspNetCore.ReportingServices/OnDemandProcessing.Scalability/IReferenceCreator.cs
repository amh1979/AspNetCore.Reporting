using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal interface IReferenceCreator
	{
		bool TryCreateReference(IStorable refTarget, out BaseReference newReference);

		bool TryCreateReference(ObjectType referenceObjectType, out BaseReference newReference);
	}
}
