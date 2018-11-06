using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal class LookupReferenceCreator : IReferenceCreator
	{
		private static LookupReferenceCreator m_instance;

		internal static LookupReferenceCreator Instance
		{
			get
			{
				if (LookupReferenceCreator.m_instance == null)
				{
					LookupReferenceCreator.m_instance = new LookupReferenceCreator();
				}
				return LookupReferenceCreator.m_instance;
			}
		}

		private LookupReferenceCreator()
		{
		}

		public bool TryCreateReference(IStorable refTarget, out BaseReference newReference)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType objectType = refTarget.GetObjectType();
			AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType referenceObjectType = default(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType);
			if (this.TryMapObjectTypeToReferenceType(objectType, out referenceObjectType))
			{
				return this.TryCreateReference(referenceObjectType, out newReference);
			}
			newReference = null;
			return false;
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
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupTableReference:
				reference = new SimpleReference<LookupTable>(referenceObjectType);
				return true;
			default:
				reference = null;
				return false;
			}
		}

		private bool TryMapObjectTypeToReferenceType(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType targetType, out AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType referenceType)
		{
			if (targetType == AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupTable)
			{
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupTableReference;
				return true;
			}
			referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None;
			return false;
		}
	}
}
