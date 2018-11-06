using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	[SkipStaticValidation]
	internal sealed class SyntheticTriangulatedCellReference : SyntheticReferenceBase<IOnDemandScopeInstance>, IReference<RuntimeCell>, IReference, IStorable, IPersistable
	{
		private IReference<IOnDemandMemberInstance> m_outerGroupLeafRef;

		private IReference<IOnDemandMemberInstance> m_innerGroupLeafRef;

		public SyntheticTriangulatedCellReference(IReference<IOnDemandMemberInstance> outerGroupLeafRef, IReference<IOnDemandMemberInstance> innerGroupLeafRef)
		{
			this.UpdateGroupLeafReferences(outerGroupLeafRef, innerGroupLeafRef);
		}

		public void UpdateGroupLeafReferences(IReference<IOnDemandMemberInstance> outerGroupLeafRef, IReference<IOnDemandMemberInstance> innerGroupLeafRef)
		{
			this.m_outerGroupLeafRef = outerGroupLeafRef;
			this.m_innerGroupLeafRef = innerGroupLeafRef;
		}

		RuntimeCell IReference<RuntimeCell>.Value()
		{
			return (RuntimeCell)this.Value();
		}

		public override IOnDemandScopeInstance Value()
		{
			IReference<IOnDemandScopeInstance> reference = default(IReference<IOnDemandScopeInstance>);
			return SyntheticTriangulatedCellReference.GetCellInstance(this.m_outerGroupLeafRef, this.m_innerGroupLeafRef, out reference);
		}

		internal static IOnDemandScopeInstance GetCellInstance(IReference<IOnDemandMemberInstance> outerGroupLeafRef, IReference<IOnDemandMemberInstance> innerGroupLeafRef, out IReference<IOnDemandScopeInstance> cellRef)
		{
			using (innerGroupLeafRef.PinValue())
			{
				IOnDemandMemberInstance onDemandMemberInstance = innerGroupLeafRef.Value();
				return onDemandMemberInstance.GetCellInstance((IOnDemandMemberInstanceReference)outerGroupLeafRef, out cellRef);
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.SyntheticTriangulatedCellReference;
		}

		public override bool Equals(object obj)
		{
			SyntheticTriangulatedCellReference syntheticTriangulatedCellReference = obj as SyntheticTriangulatedCellReference;
			if (syntheticTriangulatedCellReference == null)
			{
				return false;
			}
			if (object.Equals(this.m_outerGroupLeafRef, syntheticTriangulatedCellReference.m_outerGroupLeafRef))
			{
				return object.Equals(this.m_innerGroupLeafRef, syntheticTriangulatedCellReference.m_innerGroupLeafRef);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.m_outerGroupLeafRef.GetHashCode() ^ this.m_innerGroupLeafRef.GetHashCode();
		}
	}
}
