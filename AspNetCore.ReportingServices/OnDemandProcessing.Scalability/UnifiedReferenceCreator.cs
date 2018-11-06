using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class UnifiedReferenceCreator : IReferenceCreator
	{
		private IReferenceCreator[] m_referenceCreators;

		internal UnifiedReferenceCreator(IReferenceCreator appReferenceCreator)
		{
			this.m_referenceCreators = new IReferenceCreator[2];
			this.m_referenceCreators[0] = CommonReferenceCreator.Instance;
			this.m_referenceCreators[1] = appReferenceCreator;
		}

		public bool TryCreateReference(IStorable refTarget, out BaseReference newReference)
		{
			bool flag = false;
			newReference = null;
			for (int i = 0; i < this.m_referenceCreators.Length; i++)
			{
				if (flag)
				{
					break;
				}
				flag = this.m_referenceCreators[i].TryCreateReference(refTarget, out newReference);
			}
			return flag;
		}

		public bool TryCreateReference(ObjectType referenceObjectType, out BaseReference newReference)
		{
			bool flag = false;
			newReference = null;
			for (int i = 0; i < this.m_referenceCreators.Length; i++)
			{
				if (flag)
				{
					break;
				}
				flag = this.m_referenceCreators[i].TryCreateReference(referenceObjectType, out newReference);
			}
			return flag;
		}
	}
}
