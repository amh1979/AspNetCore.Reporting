using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal class GroupTreeReferenceCreator : IReferenceCreator
	{
		private static GroupTreeReferenceCreator m_instance;

		internal static GroupTreeReferenceCreator Instance
		{
			get
			{
				if (GroupTreeReferenceCreator.m_instance == null)
				{
					GroupTreeReferenceCreator.m_instance = new GroupTreeReferenceCreator();
				}
				return GroupTreeReferenceCreator.m_instance;
			}
		}

		private GroupTreeReferenceCreator()
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
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegionInstanceReference:
				reference = new DataRegionInstanceReference();
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReportInstanceReference:
				reference = new SubReportInstanceReference();
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportInstanceReference:
				reference = new ReportInstanceReference();
				break;
			default:
				reference = null;
				return false;
			}
			return true;
		}

		private bool TryMapObjectTypeToReferenceType(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType targetType, out AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType referenceType)
		{
			switch (targetType)
			{
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegionInstance:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegionInstanceReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReportInstance:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReportInstanceReference;
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportInstance:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportInstanceReference;
				break;
			default:
				referenceType = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None;
				return false;
			}
			return true;
		}
	}
}
