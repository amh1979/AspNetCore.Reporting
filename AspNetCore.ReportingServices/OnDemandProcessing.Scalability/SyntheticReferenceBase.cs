using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	[SkipStaticValidation]
	internal abstract class SyntheticReferenceBase<T> : IReference<T>, IReference, IStorable, IPersistable
	{
		public ReferenceID Id
		{
			get
			{
				Global.Tracer.Assert(false, "Id may not be used on a synthetic reference.");
				throw new InvalidOperationException();
			}
		}

		public int Size
		{
			get
			{
				Global.Tracer.Assert(false, "Size may not be used on a synthetic reference.");
				throw new InvalidOperationException();
			}
		}

		public abstract T Value();

		public IDisposable PinValue()
		{
			Global.Tracer.Assert(false, "PinValue() may not be used on a synthetic reference.");
			throw new InvalidOperationException();
		}

		public void UnPinValue()
		{
			Global.Tracer.Assert(false, "UnPinValue() may not be used on a synthetic reference.");
			throw new InvalidOperationException();
		}

		public void Free()
		{
			Global.Tracer.Assert(false, "Free() may not be used on a synthetic reference.");
			throw new InvalidOperationException();
		}

		public void UpdateSize(int sizeDeltaBytes)
		{
			Global.Tracer.Assert(false, "UpdateSize(int) may not be used on a synthetic reference.");
			throw new InvalidOperationException();
		}

		public IReference TransferTo(IScalabilityCache scaleCache)
		{
			Global.Tracer.Assert(false, "TransferTo(IScalabilityCache) may not be used on a synthetic reference.");
			throw new InvalidOperationException();
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			Global.Tracer.Assert(false, "Serialize may not be used on a synthetic reference.");
			throw new InvalidOperationException();
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			Global.Tracer.Assert(false, "Deserialize may not be used on a synthetic reference.");
			throw new InvalidOperationException();
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false, "ResolveReferences may not be used on a synthetic reference.");
			throw new InvalidOperationException();
		}

		public abstract AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType();
	}
}
