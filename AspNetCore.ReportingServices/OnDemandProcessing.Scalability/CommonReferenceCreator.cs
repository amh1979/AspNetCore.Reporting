using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class CommonReferenceCreator : IReferenceCreator
	{
		private static CommonReferenceCreator m_instance;

		internal static CommonReferenceCreator Instance
		{
			get
			{
				if (CommonReferenceCreator.m_instance == null)
				{
					CommonReferenceCreator.m_instance = new CommonReferenceCreator();
				}
				return CommonReferenceCreator.m_instance;
			}
		}

		private CommonReferenceCreator()
		{
		}

		public bool TryCreateReference(IStorable refTarget, out BaseReference newReference)
		{
			switch (refTarget.GetObjectType())
			{
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StorableArray:
				return this.TryCreateReference(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StorableArrayReference, out newReference);
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNode:
				return this.TryCreateReference(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNodeReference, out newReference);
			default:
				newReference = null;
				return false;
			}
		}

		public bool TryCreateReference(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType referenceObjectType, out BaseReference reference)
		{
			switch (referenceObjectType)
			{
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Null:
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None:
				Global.Tracer.Assert(false, "Cannot create reference to Nothing or Null");
				reference = null;
				return false;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StorableArrayReference:
				reference = new SimpleReference<StorableArray>(referenceObjectType);
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNodeReference:
				reference = new ScalableDictionaryNodeReference();
				break;
			default:
				reference = null;
				return false;
			}
			return true;
		}
	}
}
